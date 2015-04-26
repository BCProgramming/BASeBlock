using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;

namespace BASeBlock.WeaponTurrets
{
    public abstract class BaseTurret:ITurret
    {
        private ITurretOwner _Owner;
        private Image _TurretImage = null;
        private Point _Pivot = new Point(8, 16);
        private float _TurretAngle = 0;
        private SizeF _DrawSize;
        private BCBlockGameState _GameState;
        public ITurretOwner Owner { get { return _Owner; } protected set { _Owner = value; } }
        public Image TurretImage { get { return _TurretImage; } protected set { _TurretImage = value; } }
        public SizeF DrawSize { get { return _DrawSize; } protected set { _DrawSize = value; } }
        public Point Pivot { get { return _Pivot; } protected set { _Pivot = value; } }
        public float TurretAngle { get { return _TurretAngle; } set { _TurretAngle = value; } }
        public BCBlockGameState GameState { get { return _GameState; } set { _GameState = value; } }
        public PointF Location { get { PointF tloc = Owner.getTurretPositionOffset(this);
            return new PointF(tloc.X + Owner.Location.X,tloc.Y+Owner.Location.Y); } }
        protected BaseTurret(ITurretOwner pOwner,BCBlockGameState pState,Image pTurretImage,Point pTurretPivot):
            this(pOwner, pState, pTurretImage, pTurretPivot, pTurretImage!=null?pTurretImage.Size:SizeF.Empty)
        {

        }
        protected BaseTurret(ITurretOwner pOwner,BCBlockGameState pState,Image pTurretImage,Point pTurretPivot,SizeF pDrawSize)
        {
            if (pOwner == null) throw new ArgumentNullException("pOwner");
            GameState = pState;
            _Owner = pOwner;
            _TurretImage = pTurretImage;
            _DrawSize = pDrawSize;
            _TurretImage =_TurretImage ?? BCBlockGameState.Imageman.getLoadedImage("TURRET");
            if (_DrawSize == SizeF.Empty) _DrawSize = _TurretImage.Size;
            _Pivot = pTurretPivot;
            //8,16 is the pivot spot on the Turret


        }
        public abstract void ShootTurret(BCBlockGameState state);

        public virtual void Draw(ITurretOwner pOwner, Graphics g)
        {
            //draw the TurretImage.
            //First, get the offset...
            PointF graboffset = _Owner.getTurretPositionOffset(this);
            var drawlocation = new PointF(_Owner.Location.X + graboffset.X-(DrawSize.Width/2), _Owner.Location.Y + graboffset.Y-(DrawSize.Height/2));
            var LaunchLocation = new PointF(_Owner.Location.X + graboffset.X , _Owner.Location.Y + graboffset.Y);
            /*
              g.TranslateTransform(DrawSize.Width / 2 + DrawLocation.X, DrawSize.Height / 2 + DrawLocation.Y);
                g.RotateTransform(DrawRotation);
                g.TranslateTransform(-DrawSize.Width / 2, -DrawSize.Height / 2);
              
             */

            //first: TranslateTransform by -drawlocation.

            PointF Vel = BCBlockGameState.GetVelocity(50, TurretAngle);
            g.DrawLine(new Pen(Color.Black,3),  LaunchLocation, new PointF(drawlocation.X + Vel.X, drawlocation.Y + Vel.Y));

            
            var prevtransform = g.Transform;

            g.TranslateTransform(DrawSize.Width / 2 + drawlocation.X, DrawSize.Height / 2 + drawlocation.Y);
            float useAngle = (float)((TurretAngle + (Math.PI / 2)) / (Math.PI / 180));
            g.RotateTransform(useAngle);
            g.TranslateTransform(-DrawSize.Width / 2, -DrawSize.Height / 2);

/*            g.TranslateTransform(drawlocation.X+_Pivot.X, drawlocation.Y+(Pivot.Y));
            float useAngle = (float) ((TurretAngle+(Math.PI/2)) / (Math.PI / 180));
           
            Debug.Print("Angle:" + useAngle);
            g.RotateTransform(useAngle);
            g.TranslateTransform(-_Pivot.X*1.5f,-_Pivot.Y*1.5f);*/
            //now, RotateTransform by TurretAngle.
            //the angle specified will be in Radians, so translate that.
            
            
            //we can now draw our Image. We specify 0,0 because translatetransform should have us covered already.
            g.DrawImageUnscaled(TurretImage, 0, 0);

            //reset the transform to the previous one.

            g.Transform = prevtransform;




        }

        public abstract bool PerformFrame(ITurretOwner pOwner, BCBlockGameState gameState);
    }
}
