using System.Drawing;

namespace BASeBlock.Powerups
{
    public class IcePowerUp : PaddlePowerUp<PaddleBehaviours.FrozenBehaviour>
    {
        public IcePowerUp(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize)
        {
            mScoreValue = -20;

        }
        public static float PowerupChance()
        {

            return 0.7f;
        }
        public override Image[] GetPowerUpImages()
        {
            return new Image[] { BCBlockGameState.Imageman.getLoadedImage("ICEEFFECT") };
        }
        protected override bool SupportsMulti()
        {
            return false;
        }
    }
}