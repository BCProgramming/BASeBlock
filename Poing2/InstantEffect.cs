using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{

  

   



    public abstract class InstantEffect:GameObject
    {

        protected InstantEffect()
        {

        }
        public override sealed void Draw(System.Drawing.Graphics g)
        {
            //throw new NotImplementedException();
        }
        public abstract void ApplyEffect(BCBlockGameState gstate);
        
        public override sealed bool PerformFrame(BCBlockGameState gamestate)
        {

            gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => ApplyEffect(gamestate)));
            
            return true;
        }


    }
}
