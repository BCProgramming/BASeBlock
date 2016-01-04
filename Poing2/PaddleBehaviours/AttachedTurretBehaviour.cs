using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.WeaponTurrets;

namespace BASeCamp.BASeBlock.PaddleBehaviours
{
    public class AttachedTurretBehaviour : BaseTerminatorBehaviour,ITurretOwner
    {
        //implementation currently attaches A set of ITurrets to the paddle.


        //Dictionary<ITurret, float> TurretOffsetPercentages = new Dictionary<ITurret, float>();
        //List<ITurret> _Turrets = new List<ITurret>();
        //public List<ITurret> getTurrets() { return _Turrets.ShallowClone(); }
        private ITurret _Turret;
        public ITurret Turret { get { return _Turret; } set { _Turret = value; } }
        public AttachedTurretBehaviour(BCBlockGameState stateobject):this(stateobject,2)
        {

        }
        public AttachedTurretBehaviour(BCBlockGameState stateobject,int count):this(stateobject,count,typeof (ShooterTurret))
        {
        
        }
        public AttachedTurretBehaviour(BCBlockGameState stateobject,int count,Type TurretType) : base(stateobject)
        {
            if (!(typeof(ITurret).IsAssignableFrom(TurretType)))
            {
                throw new ArgumentException("TurretType argument must implement ITurret");
            }
            float currentpercentage = 0;
            float addAmount=0;
            
            Turret = Activator.CreateInstance(TurretType, this, stateobject) as ITurret;
            
            //_Turret = new ShooterTurret(this, stateobject);
            _OwnerPaddle = new WeakReference(stateobject.PlayerPaddle);
         
        }
        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            mstate.ClientObject.ButtonDown += ClientObject_ButtonDown;
            mstate.ClientObject.ButtonUp += ClientObject_ButtonUp;
            base.BehaviourAdded(toPaddle, gamestate);
        }

        void ClientObject_ButtonUp(object sender, Events.ButtonEventArgs<bool> e)
        {
            if ((e.Button & ButtonConstants.Button_A) == ButtonConstants.Button_A)
            {
                (_OwnerPaddle.Target as Paddle).Interactive = true;
            }
        }
        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            mstate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
        }
        void ClientObject_ButtonDown(object sender, Events.ButtonEventArgs<bool> e)
        {
            if ((e.Button & ButtonConstants.Button_A) == ButtonConstants.Button_A)
            {
                (_OwnerPaddle.Target as Paddle).Interactive = false;
            }
        }

        public PointF getTurretPositionOffset(ITurret CheckTurret)
        {
            

            return new PointF(Owner.getRectangle().Width / 2, 0);


            
        }

        public PointF Location { get {
            if (_OwnerPaddle==null || _OwnerPaddle.IsAlive)
            {
                return new PointF(Owner.Getrect().Left,Owner.Getrect().Top);
                
            }
            return PointF.Empty;
        } }
        public PointF Velocity { get { return PointF.Empty; } }
        public override void PerformFrame(BCBlockGameState gamestate, Paddle pPaddle)
        {
            _Turret.PerformFrame(this, gamestate);

            base.PerformFrame(gamestate, pPaddle);
        }
        public override bool RequiresPerformFrame(BCBlockGameState gamestate, Paddle withpaddle)
        {
            return true;
        }
        public override void Draw(Paddle onPaddle, Graphics g)
        {
            _Turret.Draw(this, g);
            

            //base.Draw(onPaddle, g);
        }
    }
}
