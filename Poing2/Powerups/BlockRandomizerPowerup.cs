using System;
using System.Drawing;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock.Powerups
{
    public class BlockRandomizerPowerup : GamePowerUp
    {
        private static Type[] PossibleTypes = new Type[] {typeof(NormalBlock),typeof(StrongBlock),typeof(PowerupBlock),typeof(PowerupCycleBlock),
                                                          typeof(InvincibleBlock),typeof(RayBlock),typeof(VomitBlock)};
        public bool Callback(BCBlockGameState gamestate)
        {
            BCBlockGameState.Soundman.PlaySound("RANDOMY");
            int numchange = Math.Min(BCBlockGameState.rgen.Next(4, 10),gamestate.Blocks.Count);


            for (int i = 0; i < numchange; i++)
            {
                bool err = true;
                while (err)
                {
                    try
                    {
                        //choose a random block...
                        Block chosenblock = BCBlockGameState.Choose(gamestate.Blocks);
                        //choose a random block type to replace it with.
                        //Type replacewith = BCBlockGameState.Choose(BCBlockGameState.BlockTypes.ManagedTypes);
                        Type replacewith = BCBlockGameState.Choose(PossibleTypes);
                        Block createdblock = (Block)Activator.CreateInstance(replacewith, chosenblock.BlockRectangle);

                        //remove the old block...
                        gamestate.Blocks.Remove(chosenblock);
                        //add the new one (the "swapped" one...
                        gamestate.Blocks.AddLast(createdblock);
                        //set refresh variable to tell the paint routine that it has to refresh the blocks.
                        


                        err = false;
                    }
                    catch
                    {
                        err = true;

                    }
                    //create the new block. remember, block constructor is a single RectangleF. catch errors
                }


            }
            gamestate.Forcerefresh = true;
            gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup((a, q) => { gamestate.Forcerefresh = true; return true; },null));
            
            
            return true;
        }
        public static float PowerupChance()
        {
            return 1f;
        }
        public BlockRandomizerPowerup(PointF pLocation, SizeF psize)
            : base(pLocation, psize, new Image[] { BCBlockGameState.Imageman.getLoadedImage("COLOURIZE(255,255,0,255):RANDOMIZER") }, 10, null)
        {

            usefunction = Callback;
        }
    }
}