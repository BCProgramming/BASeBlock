using System.Drawing;

namespace BASeCamp.BASeBlock.Powerups
{
    public class LifeUpPowerUp : GamePowerUp
    {
        public static float PowerupChance()
        {

            return 1f;
        }
        public bool PowerUpCallback(BCBlockGameState gamestate)
        {
            if (gamestate.PlayerPaddle.HP < 200)
            {
                gamestate.PlayerPaddle.HP += 20;
            }
            AddScore(gamestate, 25, "HP++");
            return true;
        }
        public LifeUpPowerUp(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, BCBlockGameState.Imageman.getImageFrames("life"), 6, null)
        {

            usefunction = PowerUpCallback;
        }



    }
}