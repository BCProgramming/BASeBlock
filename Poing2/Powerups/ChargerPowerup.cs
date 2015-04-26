using System;
using System.Drawing;

namespace BASeBlock.Powerups
{
    public class ChargerPowerup : GamePowerUp
    {
        public static bool PowerupCallback(BCBlockGameState gamestate)
        {
            gamestate.PlayerPaddle.Energy = Math.Max(gamestate.PlayerPaddle.Energy, 100);
            return true;

        }

        public ChargerPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize, BCBlockGameState.Imageman.getImageFrames("pill"), 5, PowerupCallback)
        {
            usefunction = PowerupCallback;


        }



    }
}