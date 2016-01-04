using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BASeCamp.BASeBlock.Blocks
{


    [Serializable]
    public class SandBlock : FallingBlock
    {

        public SandBlock(RectangleF BlockRectangle)
            : base(BlockRectangle, BCBlockGameState.GenerateSandImage(BlockRectangle))
        {
        }
        public SandBlock(SandBlock clonethis)
            : base(clonethis)
        {

        }
        public SandBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            //since ImageBlock only saves the key, we need to regenerate it each time anyway.

            BlockImageKey = BCBlockGameState.GenerateSandImage(BlockRectangle);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        public override void Draw(Graphics g)
        {
            base.Draw(g);
            g.DrawRectangle(new Pen(Color.Black, 1),BlockRectangle.ToRectangle());
        }
        public override object Clone()
        {
            return new SandBlock(this);
        }


    }
        [Serializable]
    public abstract class FallingBlock : ImageBlock
    {
        //generic falling block.
        private ProxyObject FallingProxy = null;

        public FallingBlock(RectangleF Blockrect,String useimagekey):base(Blockrect,useimagekey)
        {

        }
        public FallingBlock(FallingBlock src)
            : base(src)
        {



        }
        
        public FallingBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            


        }
        public FallingBlockObject Faller { get; set; }
        private bool PerformObjFrame(ProxyObject source,BCBlockGameState gstate )
        {
            
            //verify that we are still "resting" on something.
            bool restingon = FallingBlockObject.GetRestingBlocks(gstate, this.BlockRectangle).Any();
            //if we are resting on something, stay.
            //if not, turn into a FallingBlockObject.
            if (!restingon && Faller==null)
            {

                //make sure we aren't below the paddle tho.
                if (gstate.PlayerPaddle != null && gstate.PlayerPaddle.Getrect().Top > this.BlockRectangle.Bottom)
                {

                    Trace.WriteLine("PerformObjFrame adding new fbo");
                    Faller = new FallingBlockObject(this);
                    gstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => gstate.GameObjects.AddLast(Faller)));
                }
            }
            return !gstate.Blocks.Contains(this);

        }
        public override bool RequiresPerformFrame()
        {
            return FallingProxy == null; //we need to perform a frame to spark the FallingProxy.
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (FallingProxy == null)
            {
                Trace.WriteLine("FallingProxy PerformFrame");
                FallingProxy = new ProxyObject(PerformObjFrame, null);
                gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(()=>gamestate.GameObjects.AddLast(FallingProxy)));
            }


            return base.PerformFrame(gamestate);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
        public abstract override object Clone();
    }
}
