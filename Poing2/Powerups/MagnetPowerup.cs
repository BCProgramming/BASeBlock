using System;
using System.Drawing;
using System.Linq;
using BASeCamp.BASeBlock.PaddleBehaviours;

namespace BASeCamp.BASeBlock.Powerups
{
    public class MagnetPowerup : PaddlePowerUp<PaddleBehaviours.MagnetBehaviour>
    {
        public MagnetPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize)
        {
            mScoreValue = 60;
            usefunction = ourfunction;
        }
        public static float PowerupChance()
        {

            return 1f;
        }
        private bool ourfunction(BCBlockGameState gstate)
        {
            gstate.EnqueueMessage("use Button C to activate the magnet.");
            if (gstate.PlayerPaddle.Behaviours.Any((q) => q.GetType() == typeof(MagnetBehaviour)))
            {
                //re-charge the battery :P
                AddScore(gstate, 65);
                gstate.PlayerPaddle.Energy = Math.Max(100, gstate.PlayerPaddle.Energy);

                return true;

            }
            else
            {
                return base.GivePaddlePower(gstate);
            }

        }

        public override Image[] GetPowerUpImages()
        {
            return new Image[] { BCBlockGameState.Imageman.getLoadedImage("MAGNET") };

        }
        protected override bool SupportsMulti()
        {
            return false;

        }

    }
}