using System;
using System.Drawing;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.PaddleBehaviours
{
    public class InvinciblePaddleBehaviour : TimedPaddleBehaviour
    {
        private Brush useoverlaybrush = new SolidBrush(Color.Red);

        public override Image GetIcon()
        {
            return BCBlockGameState.Imageman.getLoadedImage("VC2");
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            g.FillRectangle(useoverlaybrush, onPaddle.Getrect());
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            BCBlockGameState.Soundman.PlaySoundRnd("gren");
            return false;
        }

        public override bool RequiresPerformFrame(BCBlockGameState gamestate, Paddle withpaddle)
        {
            return true;
        }

        public override void PerformFrame(BCBlockGameState gamestate, Paddle pPaddle)
        {
            var rg = BCBlockGameState.rgen;
            base.PerformFrame(gamestate, pPaddle);
            useoverlaybrush = new SolidBrush(new HSLColor(rg.NextDouble()*240, 240f, 120f));
            //spawn a sparkle.
            var paddlerect = pPaddle.Getrect();
            PointF randompoint = new PointF(paddlerect.Left + (float) (paddlerect.Width*rg.NextDouble()),
                                            paddlerect.Top + (float) (paddlerect.Height*rg.NextDouble()));

            Sparkle s = new Sparkle(randompoint, BCBlockGameState.GetRandomVelocity(0, 2),
                                    new HSLColor(rg.NextDouble()*240, 240, 120));
            gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => gamestate.Particles.Add(s)));
        }

        public override void TimedBehaviourInitialize(Paddle toPaddle, BCBlockGameState gamestate)
        {
            toPaddle.OnHPChange += toPaddle_OnHPChange;
        }

        private void toPaddle_OnHPChange(Object Sender,PaddleElementChangeEventArgs<float> earg)
        {
            Paddle ForPaddle = earg.Source;
            if (!ForPaddle.Behaviours.Contains(this))
            {
                ForPaddle.OnHPChange -= toPaddle_OnHPChange;
            }
            //false to disallow all negative changes.
            if ((earg.NewValue - earg.OldValue) < 0) { earg.Cancel = true; return; }

            earg.Cancel = true;
                
        }


        public override void TimedBehaviourRemove(Paddle frompaddle, BCBlockGameState gamestate)
        {
            frompaddle.OnHPChange -= toPaddle_OnHPChange;
        }
    }
}