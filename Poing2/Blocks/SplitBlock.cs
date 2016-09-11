using System;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [BlockDescription("When hit, splits in half along the major axis of the block. Destroyed when it reaches a minimum size.")]
    public class SplitBlock : ImageBlock
    {
        private float _MinimumSize = 16;
        public SplitBlock(RectangleF blockrect)
            : base(blockrect, "SPLITBLOCK")
        {


        }
        public SplitBlock(SplitBlock clonethis):base(clonethis)
        {


        }
        public SplitBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            var evennumbers = Enumerable.Range(0, 100).Where((d) => d % 2 == 0);
            
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            return base.GetXmlData(pNodeName,pPersistenceData);
        }
        public SplitBlock(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
        {

        }
        public override Object Clone()
        {
            return new SplitBlock(this);

        }
        protected override Particle AddStandardSprayParticle(BCBlockGameState parentstate, cBall ballhit)
        {
            return AddStandardSprayParticle<DustParticle>(parentstate, ballhit);
        }
        protected override void CreateOrbs(PointF Location, BCBlockGameState gstate)
        {
            //base.CreateOrbs(Location, gstate);
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {

            float mindimension = Math.Min(BlockRectangle.Width, BlockRectangle.Height);
            if (mindimension > _MinimumSize)
            {
                //split us in half. 
                RectangleF newA, newB; //the "new" block rectangles.
                if (BlockRectangle.Width > BlockRectangle.Height)
                {
                    //split vertically.
                    newA = new RectangleF(BlockRectangle.Left, BlockRectangle.Top, BlockRectangle.Width / 2, BlockRectangle.Height);
                    newB = new RectangleF(BlockRectangle.Left + BlockRectangle.Width / 2, BlockRectangle.Top, BlockRectangle.Width / 2, BlockRectangle.Height);



                }
                else
                {
                    //split horizontally.
                    newA = new RectangleF(BlockRectangle.Left, BlockRectangle.Top, BlockRectangle.Width, BlockRectangle.Height / 2);
                    newB = new RectangleF(BlockRectangle.Left, BlockRectangle.Top + BlockRectangle.Height / 2, BlockRectangle.Width, BlockRectangle.Height / 2);

                }


                parentstate.Blocks.AddLast(new SplitBlock(newA));
                parentstate.Blocks.AddLast(new SplitBlock(newB));
                parentstate.Forcerefresh = true;

                BCBlockGameState.Soundman.PlaySound("bbounce");

            }
            else
            {
                Block.PlayDefaultSound(ballhit);
            }

            return true; //we are destroyed.
           

        }
    }
}