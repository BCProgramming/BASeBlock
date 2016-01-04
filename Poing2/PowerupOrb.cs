using System;
using System.Drawing;
using BASeCamp.BASeBlock.GameObjects.Orbs;

namespace BASeCamp.BASeBlock.GameObjects.Orbs
{
    public class PowerupOrb : CollectibleOrb
    {
        //uses "PowerupOrb" image.
        
       
        public PowerupOrb(PointF pLocation):this(pLocation,new SizeF(16,8))
        {
        }
        public PowerupOrb(PointF pLocation, SizeF usesize):this(pLocation,usesize,new Image[]{BCBlockGameState.Imageman["POWERUPORB"]})
        {

        }
        public PowerupOrb(PointF pLocation,SizeF usesize,Image[] pFrames):base(pLocation,usesize,pFrames)
        {
        }
        protected void SpawnPowerup(BCBlockGameState gstate)
        {
            var chosentype = BCBlockGameState.Choose(gstate.PlayingLevel.AvailablePowerups);

            //constructor: (PointF Location, SizeF ObjectSize)

            //choose a location at the top of the screen...
            float Xcoordinate = (float)(BCBlockGameState.rgen.NextDouble() * gstate.GameArea.Width - 16);

            GamePowerUp spawnedobj = (GamePowerUp)Activator.CreateInstance(chosentype, new PointF(Xcoordinate, 0), new SizeF(16, 8));
            //AddObjects.Add(spawnedobj);
            gstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(()=> gstate.GameObjects.AddLast(spawnedobj)));


            BCBlockGameState.Soundman.PlaySound("drop");

        }

        /// <summary>
        /// called when this collectible touches a character. or paddle.
        /// </summary>
        /// <param name="gstate"></param>
        /// <param name="gchar"></param>
        /// <returns></returns>
        protected override bool TouchCharacter(BCBlockGameState gstate, GameCharacter gchar)
        {
            SpawnPowerup(gstate);
            return true;
        }

        protected override bool TouchPaddle(BCBlockGameState gstate, Paddle pchar)
        {
            SpawnPowerup(gstate);
            return true;
        }
    }
}