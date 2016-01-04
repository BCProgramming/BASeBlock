using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [ImpactEffectBlockCategory]
    [BlockDescription("Recreates destroyed blocks for a given length of time")]
    public class FrustratorBlock : ImageBlock
    {
        private static System.Threading.Timer FrustratorTimer;
        private static DateTime StartFrustration;
        private static TimeSpan Frustrationlength = new TimeSpan(0, 0, 0, 5);
        private static DateTime LastFrustrate;
        private static TimeSpan Frustrationperiod = new TimeSpan(0, 0, 0, 0, 400);
        public FrustratorBlock(RectangleF BlockRect)
            : base(BlockRect, "Frustrator")
        {


        }

        public FrustratorBlock(FrustratorBlock clonethis)
            : base(clonethis)
        {

        }
        public override object Clone()
        {
            
            return new FrustratorBlock(this);
        }
        public override bool MustDestroy()
        {
            return false;
        }
        /// <summary>
        /// routine finds all the blocks that can be candidates for respawning.
        /// </summary>
        /// <returns></returns>
        public static List<Block> getRespawnCandidates(BCBlockGameState stateobj)
        {

            //first, we copy the Blocks from the state. This is to prevent a race condition. Note that
            //since it is a shallow clone, it shouldn't have a huge impact on memory usage.
            if (stateobj.PlayingLevel == null) return new List<Block>();
            List<Block> clonedlist;
            try
            {
                clonedlist = stateobj.Blocks.ShallowClone().ToList();

            }
            catch
            {
                return new List<Block>();
            }

            //the primary "issue" here is that we cloned them to make them disparate entities, so we can't just do a direct comparison.
            //truly, the best we can do is compare BlockRectangles. This has something of a weird effect in that Blocks that move can and may very well be 
            //"respawned" by virtue of them no longer being in their original positions. It sounds like it could be an interestiong quirk and there isn't
            //a whole lot I can do to prevent it without keeping track of a lot of crap, so I'll see how that goes.


            //get all blocks in stateobj.PlayingLevel.levelblocks for which there is no block in clonedlist with the same BlockRectangle...


            var queried = from bb in stateobj.PlayingLevel.levelblocks
                          where (bb.GetType() != typeof(FrustratorBlock) &&
                                 !(clonedlist.Any((wep) => wep.BlockRectangle == bb.BlockRectangle)))
                          select bb;


            //convert to list, and return.
            return queried.ToList();







        }

        private void FrustrationTimerProc(Object value)
        {
            //The timer proc is here to do one thing:
            //first, see if DateTime.Now - StartFrustration is larger than Frustrationlength. if so, disable the timer, and break out.
            Debug.Print("frustration Timer Proc");
            BCBlockGameState bstate = (BCBlockGameState)value;
            try
            {
                if ((DateTime.Now - StartFrustration) > Frustrationlength)
                {
                    if (FrustratorTimer != null)
                    {
                        Debug.Print("Frustrator destroying itself...");
                        FrustratorTimer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
                        FrustratorTimer.Dispose();
                        FrustratorTimer = null;
                    }

                }
            }
            catch (NullReferenceException nre)
            {
                //nothing...

            }

            if (DateTime.Now - LastFrustrate > Frustrationperiod)
            {
                //next, if DateTime.Now-LastFrustrate is larger than Frustrationperiod, choose one block from this level and regenerate it.
                //note that value will be the gamestate.

                //"PlayingLevel" <SHOULD> be the virgin level, with all the blocks. the gamestate has a separate blocks/balls/etc array
                //that should be disparate. So, we should be able to choose one block in the PlayingLevel array that is NOT in play, and then clone it and add it
                //to the current gamestate.

                //first, use LINQ to grab a block from the PlayingLevel that matches that criteria.
                var respawnblocks = getRespawnCandidates(bstate);
                //respawn blocks contains all possible blocks we could choose from.
                //choose a random one.
                Block respawnit;
                if (respawnblocks.Count() == 1)
                    respawnit = respawnblocks.First();
                else if (respawnblocks.Count > 1)
                    respawnit = respawnblocks.ElementAt(BCBlockGameState.rgen.Next(1, respawnblocks.Count()));
                else
                    return; //nothing to respawn...

                //now, Clone this block...
                Block cloned = (Block)respawnit.Clone();
                //add add it to the gamestate...
                bstate.Blocks.AddLast(cloned);

                //for effect, instead of just having it poof into existence (well, that is what we are doing)
                //we'll have some orb particles spawn too.
                //6 should do it.
                spawnpoof(bstate, cloned);


                //also, make a funny noise. Because I want to. That's why.
                BCBlockGameState.Soundman.PlaySound("doh");



                //also, forcerefresh.
                bstate.Forcerefresh = true;


                LastFrustrate = DateTime.Now;



            }

            





        }

        public static void spawnpoof(BCBlockGameState bstate, Block cloned)
        {
            Random rg = BCBlockGameState.rgen;
            for (int i = 0; i < 6; i++)
            {
                //choose a random location within the block.
                PointF randomspot =
                    new PointF((float) (rg.NextDouble()*cloned.BlockRectangle.Width + cloned.BlockRectangle.Left),
                               (float) (rg.NextDouble()*cloned.BlockRectangle.Height + cloned.BlockRectangle.Top));
                //now, a random speed.
                PointF randomspeed = BCBlockGameState.GetRandomVelocity(0.5f, 2);
                //and last, a random radius...
                float useradius = (float) (rg.NextDouble()*cloned.BlockRectangle.Width);


                LightOrb addorb = new LightOrb(randomspot, Color.Blue, useradius);

                addorb.Velocity = randomspeed;
                bstate.Particles.Add(addorb);
            }
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {

            //this block will be destroyed. However, we start A timer.
            StartFrustration = DateTime.Now;
            LastFrustrate = DateTime.Now;
            if (FrustratorTimer == null)
            {
                //if it's not null, ignore; a 'frustration' is already occuring. we will reset it's attributes, though.

                //since it is, we need to start it up.
                FrustratorTimer = new System.Threading.Timer((TimerCallback)FrustrationTimerProc, parentstate,
                                                             100, (int)(Frustrationlength.TotalMilliseconds / Frustrationperiod.TotalMilliseconds));





            }


            base.PerformBlockHit(parentstate, ballhit);
            return true;
        }

        public FrustratorBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {


        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }


    }
}