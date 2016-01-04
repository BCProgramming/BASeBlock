using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// BossableAttribute gets set on any GameEnemy that implements
    /// a static CreateBoss(PointF pPosition,BCBlockGameState mGameState).
    /// </summary>
    class BossableAttribute :Attribute 
    {
        public BossableAttribute()
        {
            
        }
    }
}
