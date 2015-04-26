using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeBlock
{
    /// <summary>
    /// Implemented by GameObjects and possibly other objects that want to be notified when they are "hit" by explosions.
    /// </summary>
    interface IExplodable
    {
        void ExplosionInteract(Object sender, PointF Origin, double Strength);


    }
}
