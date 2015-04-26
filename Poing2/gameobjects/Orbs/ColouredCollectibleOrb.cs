using System.Drawing;
using BASeBlock.Particles;

namespace BASeBlock.GameObjects.Orbs
{
    public abstract class ColouredCollectibleOrb : CollectibleOrb
    {

        private Color _OrbColor = Color.Green;

        public Color OrbColor { get { return _OrbColor; } set { _OrbColor = value; ReAcquireImage(); } }


        protected ColouredCollectibleOrb(PointF pLocation, Color pColor)
            : this(pLocation, DefaultSize, pColor)
        {


        }
        protected ColouredCollectibleOrb(PointF pLocation, SizeF objectsize, Color pColor)
            : base(pLocation, objectsize)
        {
            _OrbColor = pColor;
            ReAcquireImage();
        }
        protected override void ReAcquireImage()
        {
            base.ObjectImages = new Image[]{ BCBlockGameState.GetSphereImage(_OrbColor, Size.ToSize())};

        }

        int framemodulus = 0;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            framemodulus = (framemodulus + 1);
            if (framemodulus == 0)
            {
                LightOrb addorb = new LightOrb(CenterPoint(), _OrbColor, Size.Width * 2);
                addorb.TTL = 3;
                gamestate.Particles.Add(addorb);

            }
            return base.PerformFrame(gamestate);

        }
    }
}