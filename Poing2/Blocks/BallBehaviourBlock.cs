using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;

namespace BASeCamp.BASeBlock.Blocks
{
    /// <summary>
    /// BallBehaviourBlock: Generic class that can be used to create a block that gives a specific
    /// "ability" to the ball that hits it. When used directly it can be instantiated like any other block,
    /// it should be overridden to change some of the defaults (such as the image used).
    /// </summary>
    /// <typeparam name="T">The iBallBehaviour Type that will be given to the ball that hits this block.
    /// When this class is used as-is, the type specified must have a default constructor. When used as the base class the 
    /// derived type can pass in an object array that will be used to instantiate the Behaviour when needed.
    /// </typeparam>
    [Serializable]
    public class BallBehaviourBlock<T> : GenericImageBlock where T : iBallBehaviour
    {
        //private String _ImageKey="CrazyBlock";
        public String useImageKey { get { return BlockImageKey; } set { BlockImageKey = value; } }
        private object[] constructorparams = null;
        protected BallBehaviourBlock(RectangleF pBlockrect, String imagekey)
            : base(pBlockrect, imagekey)
        {



        }


        protected BallBehaviourBlock(PointF pLocation, SizeF pSize)
            : this(new RectangleF(pLocation, pSize), "")
        {

            BlockLocation = pLocation;
            BlockSize = pSize;


        }
        protected BallBehaviourBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {



        }
        protected BallBehaviourBlock(XElement Source):base(Source)
        {

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override XElement GetXmlData(string pNodeName)
        {
            return base.GetXmlData(pNodeName);
        }

        public BallBehaviourBlock(PointF pLocation, SizeF pSize, String puseImageKey)
            : this(pLocation, pSize)
        {
            if (puseImageKey != null)
                useImageKey = puseImageKey;
            BlockImageKey = useImageKey;
        }
        public BallBehaviourBlock(PointF pLocation, SizeF pSize, String pUseImageKey, Object[] pconstructorparams)
            : this(pLocation, pSize, pUseImageKey)
        {

            constructorparams = pconstructorparams;


        }

        public BallBehaviourBlock(BallBehaviourBlock<T> clonethis)
            : this(clonethis.BlockRectangle, clonethis.ImageKey)
        {





        }
        protected T CreateBehaviour()
        {
            if (constructorparams == null)
            {

                return (T)Activator.CreateInstance(typeof(T));


            }
            else
            {
                return (T)Activator.CreateInstance(typeof(T), constructorparams);
            }


        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //give the ball the appropriate behaviour
            iBallBehaviour createdbehaviour = (iBallBehaviour)CreateBehaviour();
            ballhit.Behaviours.Add(createdbehaviour);
            return true;
            //return base.PerformBlockHit(parentstate, ballhit, ref ballsadded);
        }

        public override object Clone()
        {
            return new BallBehaviourBlock<T>(this);
        }
    }
}