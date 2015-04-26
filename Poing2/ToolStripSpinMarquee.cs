using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using Timer = System.Threading.Timer;

namespace BASeBlock
{
    class ToolStripSpinMarquee:ToolStripControlHost
    {
        private Image[] MarqueeFrames = BCBlockGameState.Imageman.getImageFrames("SPINMARQUEE");
        private bool _Animate = false;
        private int CurrentFrame = 0;
        public bool Animate { get { return _Animate; } set { _Animate = value; } }


        System.Threading.Timer AnimationTimer = null;
        private void TimerFunction(Object param)
        {
            if (_Animate)
            {
                //CurrentFrame++;
                CurrentFrame = (CurrentFrame + 1) % (MarqueeFrames.Length-1);
                Control.Invoke((MethodInvoker)(this.Invalidate));
            }

        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            Image drawthis = MarqueeFrames[CurrentFrame];
            e.Graphics.DrawImage(drawthis, base.ContentRectangle);


        }
        private void SetupControl()
        {
            AnimationTimer = new Timer(TimerFunction, null, 0, 250);


        }
        public ToolStripSpinMarquee(Control c)
            : base(c)
        {
            SetupControl();
        }
        public ToolStripSpinMarquee(Control c, String Name)
            : base(c, Name)
        {
            SetupControl();

        }
        
        
    }
}
