using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [ImpactEffectBlockCategory]
    [BBEditorInvisible] //will probably deprecate for a SpeedEffectBlock or something similar.
    [BlockDescription("Imparts extra speed to the ball that hit it when it is destroyed.")]
    public class SpeedBallBlock : GenericImageBlock
    {
        private PointF speedfactor = new PointF(1.1f, 1.1f);

        public SpeedBallBlock(RectangleF blockrect, PointF pspeedfactor)
            : base(blockrect, "speedball")
        {


        }
        public SpeedBallBlock(RectangleF blockrect)
            : base(blockrect, "speedball")
        {

        }

        public SpeedBallBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            speedfactor = (PointF)info.GetValue("SpeedFactor", typeof(PointF));


        }

        protected SpeedBallBlock(SpeedBallBlock clonethis)
            : base(clonethis.BlockRectangle, clonethis.BlockImageKey)
        {
            speedfactor = clonethis.speedfactor;
        }
        protected SpeedBallBlock(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
        {
            speedfactor = (PointF)Source.ReadElement<PointF>("SpeedFactor");
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            var result = base.GetXmlData(pNodeName,pPersistenceData);
            result.Add(StandardHelper.SaveElement<PointF>(speedfactor,"SpeedFactor",pPersistenceData));
            return result;
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

            info.AddValue("SpeedFactor", speedfactor);
        }

        public override object Clone()
        {
            return new SpeedBallBlock(this);
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //BCBlockGameState.Soundman.PlaySound
            //PlayBlockSound(ballhit,"BBOUNCE");
            Block.PlayDefaultSound(ballhit);
            AddScore(parentstate, 45);
            //change velocity of ballhit.
            ballhit.Velocity = new PointF(ballhit.Velocity.X * speedfactor.X, ballhit.Velocity.Y * speedfactor.Y);

            return true;
        }
    }
}