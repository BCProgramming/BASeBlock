using System.Drawing;

namespace BASeCamp.BASeBlock.Powerups
{
    public class AddBallPowerup : GamePowerUp
    {
        public static float PowerupChance()
        {

            return 1f;
        }
        public bool AddballCallback(BCBlockGameState gamestate)
        {

            //add a new ball at the paddle position.
            //note: atm AddballPowerup only adds a "normal" ball...
            cBall addthis = new cBall(gamestate.PlayerPaddle.Position, cBall.getRandomVelocity(3f));
            gamestate.Balls.AddLast(addthis);
            AddScore(gamestate, 50);
            return true;

        }
        public AddBallPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, new Image[] { BCBlockGameState.Imageman.getLoadedImage("addballpwr") }, 10, null)
        {
            usefunction = AddballCallback;

        }

    }
}