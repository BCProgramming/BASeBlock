using System;
using System.Diagnostics;
using System.Drawing;
using System.Runtime.Serialization;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.Blocks
{
    [Serializable]
    [StandardBlockCategory]
    [BBEditorInvisible] //pointless block, disable it for the moment until we can think of something useful for it to do...
    [BlockDescription("Generally pointless block that breaks into non-interactive snow particles that look suspiciously like asterisks.")]
    public class SnowBlock : ImageBlock
    {
        public SnowBlock(RectangleF blockrect)
            : base(blockrect, BCBlockGameState.GenerateSnowImage(blockrect))
        {


        }
        public SnowBlock(SnowBlock clonethis)
            : base(clonethis)
        {


        }
        public SnowBlock(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            Debug.Print("SnowBlock deserialize");
            BlockImageKey = BCBlockGameState.GenerateSnowImage(BlockRectangle);
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Debug.Print("GetObjectData");
            base.GetObjectData(info, context);
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            var result = base.GetXmlData(pNodeName,pPersistenceData);
            return result;
        }
        public SnowBlock(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
        {
            BlockImageKey = BCBlockGameState.GenerateSnowImage(BlockRectangle);
        }
        public override object Clone()
        {
            return new SnowBlock(this);
        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //return base.PerformBlockHit(parentstate, ballhit, ref ballsadded);
            BCBlockGameState.Soundman.PlaySound("snowbreak");
            StandardSpray(parentstate, ballhit);
            base.PerformBlockHit(parentstate, ballhit);
            return true;
        }
        protected override void StandardSpray(BCBlockGameState parentstate, cBall ballhit)
        {
            //base.StandardSpray(parentstate, ballhit);


            //StandardSpray: create a group of QuadDebris that are moving away from the center of the block.
            const int MINWIDTH = 3, MAXWIDTH = 6;
            const int MINHEIGHT = 3, MAXHEIGHT = 6;

            int WIDTH = 0;
            //const int WIDTH = 4, HEIGHT = 4;
            const float SPEEDMULT = 0.5f;
            PointF centralPoint = CenterPoint();

            for (float x = 0; x < BlockRectangle.Width; )
            {

                float px = x + BlockRectangle.Left;
                for (float y = 0; y < BlockRectangle.Height; )
                {
                    WIDTH = BCBlockGameState.rgen.Next(MINWIDTH, MAXWIDTH);
                    int HEIGHT = BCBlockGameState.rgen.Next(MINHEIGHT, MAXHEIGHT);
                    RectangleF sourcepiece = new RectangleF(x, y, WIDTH, HEIGHT);
                    float py = y + BlockRectangle.Top;
                    //build the smaller rectangle.
                    RectangleF quadrect = new RectangleF(px, py, WIDTH, HEIGHT);
                    float totalspeed = ((BlockRectangle.Width / 2) / BCBlockGameState.Distance(CenterPoint().X, CenterPoint().Y, px, py)) * SPEEDMULT;
                    totalspeed *= (float)(BCBlockGameState.rgen.NextDouble() * 0.25) + 0.75f;
                    float angleof = (float)BCBlockGameState.GetAngle(CenterPoint(), new PointF(px, py));
                    PointF usespeed = new PointF((ballhit.Velocity.X / 2) + (float)Math.Cos(angleof) * totalspeed, (ballhit.Velocity.Y / 2) + (float)Math.Sin(angleof) * totalspeed);

                    //QuadDebris qd = new QuadDebris(new PointF(quadrect.Left, quadrect.Top), usespeed, new SizeF(WIDTH, HEIGHT), this.BlockImage, sourcepiece);
                    CharacterDebris qd = new CharacterDebris(new PointF(quadrect.Left, quadrect.Top), usespeed, Color.White, 8, 18);
                    //add it
                    parentstate.Particles.Add(qd);
                    y += HEIGHT;
                }

                x += WIDTH;
            }





        }

    }
}