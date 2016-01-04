using System;
using System.Drawing;

namespace BASeCamp.BASeBlock.Powerups
{
    public class PaddlePlusPowerup : GamePowerUp
    {
        public static float PowerupChance()
        {

            return 1f;
        }
        public bool PowerupCallback(BCBlockGameState gamestate)
        {
            //PaddlePlusPowerup not surprisingly makes the paddle larger.
            SizeF changedsize = new SizeF(gamestate.PlayerPaddle.PaddleSize.Width + PaddleMinusPowerup.paddlesizechangeamount, gamestate.PlayerPaddle.PaddleSize.Height);
            //gamestate.PlayerPaddle.PaddleSize = 
            //give it a size object that will change it's size...
            if (gamestate.PlayerPaddle.PaddleSize.Width < 192)
            {
                gamestate.PlayerPaddle.Behaviours.Add(new PaddleBehaviours.SizeChangeBehaviour(changedsize,
                                                                                               new TimeSpan(0, 0, 0, 1)));
                BCBlockGameState.Soundman.PlaySound("grow");
                AddScore(gamestate, 30, "Paddle++");
            }
            return true;

        }

        public PaddlePlusPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, BCBlockGameState.Imageman.getImageFrames("paddlePlus"), 10, null)
        {
            usefunction = PowerupCallback;


        }



    }
}