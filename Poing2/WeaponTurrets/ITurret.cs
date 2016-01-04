using System.Drawing;

namespace BASeCamp.BASeBlock.WeaponTurrets
{
    /// <summary>
    /// Interface that represents a Turret. A Turret is a special object that can be
    /// connected to other objects and performs pieces of logic and can be made to shoot.
    /// ITurret implementations must also provide a constructor that accepts a BCBlockGameState and a ITurretOwner
    /// new(ITurretOwner,BCBlockGameState)
    /// </summary>
    public interface ITurret
    {
        //A Turret is the object that performs the shooty action.
        void ShootTurret(BCBlockGameState state); //Shoot.
        //ITurret Implementations must also have a constructor that accepts a BCBlockGameState and
        //a ITurretOwner implementation.

        /// <summary>
        /// Draws this Turret.
        /// </summary>
        /// <param name="pOwner">Owner of this Turret</param>
        /// <param name="g">Graphics canvas to draw on.</param>
        void Draw(ITurretOwner pOwner, Graphics g);
        bool PerformFrame(ITurretOwner pOwner, BCBlockGameState gameState);


    }
}
