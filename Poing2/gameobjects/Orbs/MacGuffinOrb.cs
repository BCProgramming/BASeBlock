using System.Drawing;

namespace BASeBlock.GameObjects.Orbs
{
    public class MacGuffinOrb : ColouredCollectibleOrb
    {
        private int _MacGuffinValue = 1;
        public int MacGuffinValue { get { return _MacGuffinValue; } set { _MacGuffinValue = value; RecreateImage(); } }
        public MacGuffinOrb(PointF pLocation)
            : this(pLocation, Color.Blue)
        {
        }

        public MacGuffinOrb(PointF PLocation, SizeF objectsize)
            : base(PLocation, objectsize, Color.Blue)
        {

        }
        public MacGuffinOrb(PointF pLocation, Color pColor)
            : this(pLocation,DefaultSize, pColor)
        { }

        public MacGuffinOrb(PointF pLocation,SizeF objectsize, Color pColor)
            : base(pLocation, objectsize, pColor)
        { }
        protected Color CalculateColor()
        {
            Color[] selections = new Color[] {Color.CornflowerBlue,Color.Chocolate,Color.Yellow,Color.Purple,Color.Goldenrod};
            return selections[MacGuffinValue % selections.Length];
        }
        protected void RecreateImage()
        {
            base.OrbColor = CalculateColor();
        }
        protected override CollectibleOrb.CollectibleTypeConstants getCollectibleType()
        {
            return CollectibleOrb.CollectibleTypeConstants.Collectible_Both;
        }
        protected override bool TouchCharacter(BCBlockGameState gstate, GameCharacter gchar)
        {
            gstate.MacGuffins+=_MacGuffinValue;
            return true;

        }
        protected override bool TouchPaddle(BCBlockGameState gstate, Paddle pchar)
        {
            gstate.MacGuffins+=_MacGuffinValue;
            return true;
        }
    }
}