using System.Drawing;

namespace BASeCamp.BASeBlock.Powerups
{
    public class FrustratorPowerup : GamePowerUp
    {
        public static float PowerupChance()
        {

            return 0.8f;
        }
        public bool PowerUpCallback(BCBlockGameState gamestate)
        {
            //Old code...
            //has the same effect as a frustratorblock.
            //and how do we get the frustrator effect to "engage"?
            //we add a frustrator block, and break it off-screen. couldn't be simpler.
            // FrustratorBlock createblock = new FrustratorBlock(new RectangleF(-500, -500, 50, 50));
            // gamestate.Blocks.AddLast(createblock);
            // BCBlockGameState.Block_Hit(gamestate, createblock);
            //HAHAHA...



            //Change: this powerup now spawns a frustratorball instead...
            FrustratorBall fb = new FrustratorBall(Location, BCBlockGameState.GetRandomVelocity(3, 6));
            gamestate.Balls.AddLast(fb);




            return true;
        }
        public FrustratorPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, new Image[] { BCBlockGameState.Imageman.getLoadedImage("frustrator") }, 6, null)
        {
            usefunction = PowerUpCallback;
        }
    }
}