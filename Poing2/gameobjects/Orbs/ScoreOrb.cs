using System.Drawing;

namespace BASeCamp.BASeBlock.GameObjects.Orbs
{
    public class ScoreOrb : ColouredCollectibleOrb
    {
        private int _Value = 5;


        public ScoreOrb(PointF pLocation)
            : this(pLocation, 5)
        {

        }
        public ScoreOrb(PointF pLocation, SizeF usesize)
            : this(pLocation, 5)
        {
            Size = usesize;
            base.ReAcquireImage();
        }
        public ScoreOrb(PointF pLocation, Color pColor)
            : this(pLocation, pColor, 30)
        {

        }
        public ScoreOrb(PointF pLocation, int ScoreValue)
            : this(pLocation, ScoreValue < 0 ? Color.Red : Color.Green, ScoreValue)
        {

        }
        public ScoreOrb(PointF pLocation, Color pColor, int ScoreValue)
            : this(new SizeF(7, 7), pLocation, pColor, ScoreValue)
        { }

        public ScoreOrb(SizeF objectsize, PointF pLocation, Color pColor, int ScoreValue)
            : base(pLocation, objectsize, pColor)
        {
            _Value = ScoreValue;
        }


        protected override CollectibleTypeConstants getCollectibleType()
        {
            return CollectibleTypeConstants.Collectible_Both;
        }
        protected override bool TouchPaddle(BCBlockGameState gstate, Paddle pchar)
        {
            int AddScore = _Value + (2 * gstate.GameObjects.Count);
            gstate.GameScore += (long)(AddScore*gstate.ScoreMultiplier);
            gstate.SpawnRisingText(AddScore.ToString(), CenterPoint(), new SolidBrush(OrbColor));
            return true; //return true to destroy...

        }
        protected override bool TouchCharacter(BCBlockGameState gstate, GameCharacter gchar)
        {
            //throw new NotImplementedException();
            return false;
        }

    }
}