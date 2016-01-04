using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock.GameStates
{
    public class StateMenu:GameState
    {
        public override IGameState Run(BCBlockGameState GameInfo)
        {
            return null;
        }

        public override void DrawFrame(BCBlockGameState GameInfo, Graphics g, Size AreaSize)
        {
            
        }

        public override bool IsLoopingState
        {
            get { return true; }
        }
    }
}
