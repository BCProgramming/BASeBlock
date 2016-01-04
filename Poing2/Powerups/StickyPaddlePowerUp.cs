using System.Drawing;

namespace BASeCamp.BASeBlock.Powerups
{
    public class StickyPaddlePowerUp : PaddlePowerUp<PaddleBehaviours.StickyBehaviour>
    {
        public static float PowerupChance()
        {

            return 1f;
        }
        public StickyPaddlePowerUp(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize)
        {
            mScoreValue = 40;

        }
        public override Image[] GetPowerUpImages()
        {

            
            

            return new Image[] { BCBlockGameState.Imageman.getLoadedImage("SLIMEPOWER") };

            

        }
    }
}