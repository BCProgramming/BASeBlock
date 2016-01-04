using System.Drawing;

namespace BASeCamp.BASeBlock.Powerups
{
    public class ExtraLifePowerup : GamePowerUp
    {
        //oneup.png...
        public static float PowerupChance()
        {

            return 0.2f;
        }
        public bool PowerUpCallback(BCBlockGameState gamestate)
        {
            gamestate.playerLives++;
            AddScore(gamestate, 30, "LIFE PLUS ONE");
            return true;

        }
        public ExtraLifePowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, new Image[] { BCBlockGameState.Imageman.getLoadedImage("oneup") }, 6, null)
        {
            usefunction = PowerUpCallback;

        }

    }
}