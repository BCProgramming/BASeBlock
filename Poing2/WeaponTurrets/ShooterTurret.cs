using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.PaddleBehaviours;
using BASeCamp.BASeBlock.Powerups;

namespace BASeCamp.BASeBlock.WeaponTurrets
{
    public class ShooterTurret : BaseTurret
    {
        HashSet<ButtonConstants> DepressedButtons = new HashSet<ButtonConstants>();
        PointF lastPoint;
        private Type _SpawnType;
        public Type SpawnType { get { return _SpawnType; } set { _SpawnType = value; } }
        public ShooterTurret(ITurretOwner pOwner, BCBlockGameState pState, Type pSpawnType):base(pOwner, pState, null, new Point(8,16))
        {
            if (!(typeof(iProjectile).IsAssignableFrom(pSpawnType)))
                throw new ArgumentException("Argument must be assignable to iProjectile interface", "pSpawnType");

            _SpawnType = pSpawnType;
            pState.ClientObject.ButtonDown += ClientObject_ButtonDown;
            pState.ClientObject.ButtonUp += ClientObject_ButtonUp;
            pState.ClientObject.OnMove += ClientObject_OnMove;
        }

        void ClientObject_OnMove(object sender, MouseEventArgs<bool> e)
        {
            if(DepressedButtons.Contains(ButtonConstants.Button_A) ) lastPoint = e.Position;
        }

        void ClientObject_ButtonUp(object sender, Events.ButtonEventArgs<bool> e)
        {
                DepressedButtons.Remove((ButtonConstants)e.Button);
            
            if ((e.Button & ButtonConstants.Button_A) == ButtonConstants.Button_A)
            {
                ShootTurret(base.GameState);
            }
        }

        void ClientObject_ButtonDown(object sender, Events.ButtonEventArgs<bool> e)
        {
            foreach(var iterate in Enum.GetValues(typeof(ButtonConstants)))
            {
                DepressedButtons.Add((ButtonConstants)iterate);
            }

        }
        public ShooterTurret(ITurretOwner pOwner, BCBlockGameState pState) : this(pOwner,pState,typeof(cBall))
        {
        }

        public override void ShootTurret(BCBlockGameState state)
        {
            //shoot from the appropriate location.
            PointF SourceLocation = this.Location;
            //shoot at the given Angle. Default to Speed of 5.
            PointF useVelocity = BCBlockGameState.GetVelocity(5, TurretAngle);
           // iProjectile createprojectile = Activator.CreateInstance(_SpawnType, SourceLocation, useVelocity) as iProjectile;
           // if (createprojectile == null) return;
            
            cBall shooterBall = new cBall(SourceLocation, useVelocity);
            shooterBall.Behaviours.Add(new TempBallBehaviour());
            state.Defer(() => state.Balls.AddLast(shooterBall));
            //state.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(()=> state.AddElement(createprojectile)));
            //emit a shooty-sound.
            BCBlockGameState.Soundman.PlaySound("SHOOT");

        }

        public override bool PerformFrame(ITurretOwner pOwner, BCBlockGameState gameState)
        {
            if (DepressedButtons.Contains(ButtonConstants.Button_B))
            {
                float grabangle = (float)(BCBlockGameState.GetAngle(this.Location, lastPoint));
                TurretAngle = grabangle;
                return false;
            }
            return false;
        }
    }
    public class ShooterTurretPowerUp: AttachedTurretPowerup<AttachedTurretBehaviour,ShooterTurret>
    {
        public ShooterTurretPowerUp(PointF pLocation, SizeF pSize, object[] constructionparams) : base(pLocation, pSize, constructionparams)
        {
        }

        public ShooterTurretPowerUp(PointF pLocation, SizeF pSize) : base(pLocation, pSize)
        {
        }
    }
}
