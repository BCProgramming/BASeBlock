using System;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.XMLSerialization;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable()]
    [PowerupEffectCategory]
    public class AddBallBlock : GenericImageBlock
    {

        private float spawnvelocity = 3;
        public AddBallBlock(RectangleF blockrect)
            : base(blockrect, "addball")
        {



        }
        protected AddBallBlock(AddBallBlock clonethis)
            : base(clonethis.BlockRectangle, clonethis.BlockImageKey)
        {




        }

        public AddBallBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {


        }
        public AddBallBlock(XElement Source):base(Source)
        {
            spawnvelocity = Source.GetAttributeFloat("SpawnVelocity", 3);
        }

        public override XElement GetXmlData(string pNodeName)
        {
            XElement baseresult = base.GetXmlData(pNodeName);
            baseresult.Add(new XAttribute("SpawnVelocity",spawnvelocity));

            return baseresult;

        }

        public override object Clone()
        {
            return new AddBallBlock(this);
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //BCBlockGameState.Soundman.PlaySound("BBOUNCE", 0.9f);
            Block.PlayDefaultSound(ballhit);
            AddScore(parentstate, 40);

            float useangle = (float)BCBlockGameState.rgen.NextDouble() * (float)(2 * Math.PI);
            PointF spawnlocation = new PointF((float)BlockRectangle.Left + (BlockRectangle.Width / 2), (float)BlockRectangle.Top + (BlockRectangle.Height / 2));

            float useXSpeed = (float)Math.Sin(useangle) * spawnvelocity;
            float useYSpeed = (float)Math.Cos(useangle) * spawnvelocity;
            PointF spawnvel = new PointF(useXSpeed, useYSpeed);
            parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => parentstate.Balls.AddLast(new cBall(spawnlocation, spawnvel))));
            //ballsadded.Add(new cBall(spawnlocation, spawnvel));
            StandardSpray(parentstate, ballhit);

            return true;
        }
    }
}