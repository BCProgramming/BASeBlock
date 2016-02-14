using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [ImpactEffectBlockCategory]
    [BlockDescription("Gives the ball that hits it the Crazy Ball Behaviour.")]
    public class CrazyBlock : BallBehaviourBlock<CrazyBallBehaviour>
    {

        public CrazyBlock(CrazyBlock clonethis)
            : base(clonethis.BlockRectangle, clonethis.ImageKey)
        {



        }
        public CrazyBlock(RectangleF pBlockrect)
            : this(new PointF(pBlockrect.X, pBlockrect.Y), pBlockrect.Size)
        {



        }
        public CrazyBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            //no special data to save.


        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //nothing.
            ImageKey = "CrazyBlock";
            base.GetObjectData(info, context);
        }
        public CrazyBlock(PointF pLocation, SizeF pSize)
            : base(pLocation, pSize, "crazyblock")
        {


        }
        public CrazyBlock(XElement Source):base(Source)
        {

        }
        public override XElement GetXmlData(string pNodeName)
        {
            return base.GetXmlData(pNodeName);
        }

        public override object Clone()
        {
            return new CrazyBlock(this);
        }


    }
}