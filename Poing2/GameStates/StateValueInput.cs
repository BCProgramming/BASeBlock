﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeBlock.GameStates
{
    public class StateValueInput:GameState
    {
        public override IGameState Run(BCBlockGameState GameInfo)
        {
            //throw new NotImplementedException();
            return null;
        }

        public override void DrawFrame(BCBlockGameState GameInfo, Graphics g, Size AreaSize)
        {
            
        }
        public override bool IsLoopingState
        {
            get
            {
                return true;
            }
        }
    }
}