using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using BASeBlock;    


namespace ballblock {
    [Serializable()]
    [PowerupEffectCategory]
    public class AddBallBlockEx2 : GenericImageBlock
    {

        private float spawnvelocity = 3;
        public AddBallBlockEx2(RectangleF blockrect)
            : base(blockrect, "addball")
        {



        }
        protected AddBallBlockEx2(AddBallBlockEx2 clonethis):base(clonethis.BlockRectangle,clonethis.BlockImageKey)
        {
            



        }
        
        public AddBallBlockEx2(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {


        }


        public override object Clone()
        {
            return new AddBallBlockEx2(this);  
        }

        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit, ref List<cBall> ballsadded)
        {
            BASeBlock.BCBlockGameState.Soundman.PlaySound("1UP", 0.9f);
            Block.PlayDefaultSound(ballhit);
            //base.AddScore(parentstate, 40);
		//no idea why the above is broken...
            float useangle = (float)BCBlockGameState.rgen.NextDouble() * (float)(2 * Math.PI);
            PointF spawnlocation = new PointF((float)BlockRectangle.Left+(BlockRectangle.Width/2),(float)BlockRectangle.Top + (BlockRectangle.Height/2));
            
            float useXSpeed = (float)Math.Sin(useangle) * spawnvelocity;
            float useYSpeed = (float)Math.Cos(useangle) * spawnvelocity;
            PointF spawnvel = new PointF(useXSpeed,useYSpeed);
            ballsadded.Add(new cBall(spawnlocation,spawnvel));
            StandardSpray(parentstate, ballhit);

            return true;
        }
    }
}