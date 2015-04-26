using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeBlock.WeaponTurrets
{
    /// <summary>
    /// interface implemented by classes that can have attached Turrets.
    /// 
    /// </summary>
    public interface ITurretOwner
    {
        /// <summary>
        /// Returns the positional offset of the given Turret from this Turret Owner.
        /// This is used by the ITurret implementation to retrieve the position of the Turrent for drawing and performing other pieces of logic.
        /// 
        /// </summary>
        /// <param name="CheckTurret">Turret to check</param>
        /// <returns>PointF of the positional offset from this TurrentOwner's Location of the given ITurret</returns>
        
        PointF getTurretPositionOffset(ITurret CheckTurret);
        PointF Location { get; }
        PointF Velocity { get; }
    }
}
