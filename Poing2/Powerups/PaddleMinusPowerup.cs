using System;
using System.Drawing;

namespace BASeBlock.Powerups
{
    public class PaddleMinusPowerup : GamePowerUp
    {
        public const int paddlesizechangeamount = 20;
        public static float PowerupChance()
        {

            return 1f;
        }
        public bool PowerupCallback(BCBlockGameState gamestate)
        {
            //PaddleMinusPowerup makes the paddle smaller...
            if ((gamestate.PlayerPaddle.PaddleSize.Width) > paddlesizechangeamount + 10)
            {

                SizeF changedsize = new SizeF(gamestate.PlayerPaddle.PaddleSize.Width - paddlesizechangeamount, gamestate.PlayerPaddle.PaddleSize.Height);
                //gamestate.PlayerPaddle.PaddleSize = 
                //give it a size object that will change it's size...
                gamestate.PlayerPaddle.Behaviours.Add(new PaddleBehaviours.SizeChangeBehaviour(changedsize, new TimeSpan(0, 0, 0, 1)));
                BCBlockGameState.Soundman.PlaySound("shrink");
            }
            AddScore(gamestate, -5, "Paddle--");
            return true;

        }

        public PaddleMinusPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, BCBlockGameState.Imageman.getImageFrames("paddleMinus"), 8, null)
        {

            usefunction = PowerupCallback;

        }

    }
}