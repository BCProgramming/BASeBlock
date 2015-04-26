using System;
using System.Drawing;

namespace BASeBlock.Powerups
{
    class GhostlyPowerUp : PaddlePowerUp<PaddleBehaviours.GhostlyBehaviour>
    {
        public GhostlyPowerUp(PointF pLocation, SizeF pSize)
            : base(pLocation,pSize)
        {
            
        }
        public override Image[] GetPowerUpImages()
        {
            return BCBlockGameState.Imageman.getImageFrames("GHOSTLY");
        }
    }
}
