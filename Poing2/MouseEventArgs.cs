using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{
    //Func<ButtonConstants, bool>
    public class MouseEventArgs<TResult> : EventArgs
    {
        private TResult _Result;
        private ButtonConstants _Button;
        private PointF _Position;
        public TResult Result { get { return _Result; } set { _Result = value; } }
        public ButtonConstants Button { get { return _Button; } set { _Button = value; } }
        public PointF Position { get { return _Position; } set { _Position = value; } }
        public MouseEventArgs(ButtonConstants pButton,PointF pPosition)
        {
            _Button = pButton;
            _Position = pPosition;

        }



    }
    
}
