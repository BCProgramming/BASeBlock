using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.Elementizer;

namespace BASeCamp.BASeBlock.Blocks
{
    /// <summary>
    /// Same as BlockShotBlock, but will shoot in the same direction as the ball that hit it.
    /// </summary>
    ///         
    [Serializable]
    [ImpactEffectBlockCategory]
    public class BallDirectedShotBlock : BlockShotBlock
    {
        protected float _ShootSpeed = 10;

        public float ShootSpeed
        {
            get { return _ShootSpeed; }
            set { _ShootSpeed = value; }


        }



        public BallDirectedShotBlock(RectangleF BlockRect)
            : base(BlockRect, "blockballdestructor")
        {

        }

        public BallDirectedShotBlock(RectangleF BlockRect, float usespeed)
            : this(BlockRect)
        {

            _ShootSpeed = usespeed;
        }

        public BallDirectedShotBlock(BallDirectedShotBlock clonethis)
            : base(clonethis)
        {
            _ShootSpeed = clonethis.ShootSpeed;

        }

        public BallDirectedShotBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _ShootSpeed = info.GetSingle("ShootSpeed");

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {

            base.GetObjectData(info, context);
            info.AddValue("ShootSpeed", _ShootSpeed);
        }
        public BallDirectedShotBlock(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
        {
            _ShootSpeed = Source.GetAttributeFloat("ShootSpeed");
        }
        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            var result = base.GetXmlData(pNodeName,pPersistenceData);
            result.Add(new XAttribute("ShootSpeed",_ShootSpeed));
            return result;
        }

        public override object Clone()
        {
            return new BallDirectedShotBlock(this);
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
           
            float magnitude = BCBlockGameState.Distance(new PointF(0, 0), ballhit.PreviousVelocity);
            float usespeed = Math.Max(magnitude, 2);

            float useangle ;
            if(usespeed > 0.2)
                useangle = (float) BCBlockGameState.GetAngle(new PointF(0, 0), ballhit.PreviousVelocity);
            else
            {
                //otherwise choose a random direction.
                useangle = (float)(BCBlockGameState.rgen.NextDouble() * Math.PI * 2);
            }
            ShotVelocity = new PointF((float)Math.Cos(useangle) * usespeed, (float)Math.Sin(useangle) * usespeed);
            return base.PerformBlockHit(parentstate, ballhit);
        }

        
    }
}