using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Projectiles;

namespace BASeCamp.BASeBlock
{
    public class PolygonObstacle : GameEnemy,IMovingObject 
    {



        public class PolygonObstacleBaseEventArgs:EventArgs
        {
            private cBall _Ball;
            private PolygonObstacle _Obstacle;
            private BCBlockGameState _gameState;
            private bool _Cancel;
            public cBall Ball { get { return _Ball; } }
            public PolygonObstacle Obstacle { get { return _Obstacle; } }
            public BCBlockGameState GameState { get { return _gameState;}}
            public bool Cancel { get { return _Cancel; } set { _Cancel = value; } }
            public PolygonObstacleBaseEventArgs(BCBlockGameState pgstate, cBall pBall, PolygonObstacle pObstacle)
            {
                _gameState = pgstate;
                _Ball = pBall;
                _Obstacle = pObstacle;
            }


        }
        public class PolygonObstacleHitEventArgs : PolygonObstacleBaseEventArgs
        {
            public PolygonObstacleHitEventArgs(BCBlockGameState pgstate, cBall pBall, PolygonObstacle pObstacle) : base(pgstate, pBall, pObstacle)
            {
            }
        }
        public class PolygonObstacleDestroyEventArgs :PolygonObstacleBaseEventArgs
        {
            public PolygonObstacleDestroyEventArgs(BCBlockGameState pgstate, cBall pBall, PolygonObstacle pObstacle) : base(pgstate, pBall, pObstacle)
            {
            }
        }



        public event EventHandler<PolygonObstacleHitEventArgs> BeforeObstacleHit;
        /// <summary>
        /// if cancelled, prevents impact effects on the PolygonObstacle but will still change the speed of the ball.
        /// </summary>
        public event EventHandler<PolygonObstacleHitEventArgs> ObstacleHit;
        public EventHandler<PolygonObstacleDestroyEventArgs> ObstacleDestroy;
        protected delegate PointF PointFDecayRoutine(PolygonObstacle obj, PointF src);
        protected delegate double floatDecayRoutine(PolygonObstacle obj, double src);
        protected Polygon _Poly;
        

        protected PointF _Velocity = PointF.Empty;
        protected PointF _VelocityDecay = new PointF(0.9f, 0.9f);

        protected double _CurrAngle;
        protected double _AngleSpeed = Math.PI / 16;
        protected double _AngleSpeedDecay = 0.9d;
        protected double _MinimumSize = 32 * 16;
        public double MinimumSize { get { return _MinimumSize; } set { _MinimumSize = value; } }

        private bool _fShoot = false;
        
        public bool fShoot { get { return _fShoot; } set { _fShoot = value; } }
        private int _ShootDelay =150;
        public int ShootDelay { get { return _ShootDelay; } set { _ShootDelay = value; } }
        public PolygonBlockDrawMethod _DrawMethod = new FilledPolyDrawMethod();


        public PolygonBlockDrawMethod DrawMethod { get { return _DrawMethod; } set { _DrawMethod = value; } }

        public PointF VelocityDecay { get { return _VelocityDecay; } set { _VelocityDecay = value; } }
        public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }

        public double CurrAngle { get { return _CurrAngle; } set { _CurrAngle = value; } }
        public double AngleSpeed { get { return _AngleSpeed; } set { _AngleSpeed = value; } }

        public Polygon Poly { get { return _Poly; } set { _Poly = value; } }

        protected PointFDecayRoutine _VelocityDecayFunction = (obj, src) => new PointF(src.X * obj.VelocityDecay.X, src.Y * obj.VelocityDecay.Y);
        protected floatDecayRoutine _AngleSpeedDecayFunction = (obj, src) => src * obj._AngleSpeedDecay;
        public override PointF Location
        {
            get
            {
                return base.Location;
            }
            set
            {
                Debug.Print("PolygonObstacle::Location- Location set to " + value.ToString());
                PointF offsetamount = new PointF(value.X - _Location.X, value.Y - _Location.Y);
                base.Location = value;
                if(_Poly!=null)
                    _Poly.Offset(offsetamount.X, offsetamount.Y);


            }
        }

        public override SizeF DrawSize
        {
            get
            {
                return _Poly.GetBounds().Size;
            }
            set
            {
                //nothing...
            }
        }
        private static PointF CenterPoint(IEnumerable<PointF> grabpoints)
        {
            
            return new Polygon(grabpoints.ToArray()).Center;
        }
        public PolygonObstacle(IEnumerable<PointF> Points)
            : this(Points, Brushes.Red, Pens.Black)
        {
        }
        public PolygonObstacle(PointF pLocation, int NumPoints, float pRadius,Brush pFill,Pen pStroke)
            :this(pLocation,NumPoints,pRadius,new FilledPolyDrawMethod(pStroke,pFill))
        {

        }

        public PolygonObstacle(PointF pLocation,int NumPoints,float pRadius,PolygonBlockDrawMethod drawMethod)
            :this(pLocation,NumPoints,pRadius,pRadius,drawMethod)
        {

        }
        public PolygonObstacle(Point pLocation,int NumPoints,float MinRadius,float MaxRadius,Brush pFill,Pen pStroke)
            :this(pLocation,NumPoints,MinRadius,MaxRadius,new FilledPolyDrawMethod(pStroke,pFill))
        {

        }
        public PolygonObstacle(PointF pLocation,int NumPoints,float MinRadius,float MaxRadius,
                               PolygonBlockDrawMethod drawMethod)
            :this(pLocation,NumPoints,NumPoints,MinRadius,MaxRadius,drawMethod)
        {

        }
        /// <summary>
        /// Generates an Obstacle at the given location with a random number of points created using a random radius.
        /// </summary>
        /// <param name="pLocation"></param>
        /// <param name="MinPoints"></param>
        /// <param name="MaxPoints"></param>
        /// <param name="MinRadius"></param>
        /// <param name="MaxRadius"></param>
        /// <param name="pDrawBrush"></param>
        /// <param name="pDrawPen"></param>
        public PolygonObstacle(PointF pLocation,int MinPoints,int MaxPoints,float MinRadius,float MaxRadius,PolygonBlockDrawMethod drawMethod)
            :base(pLocation,null,0)
        {
            _Location = pLocation;
            _Poly = new Polygon(BCBlockGameState.GenPoly(MinRadius,MaxRadius,MinPoints,MaxPoints));
            _Poly.Offset(pLocation);
            _DrawMethod = drawMethod;    
            //base.Size = _Poly.GetBounds().Size;
            
        }
        public PolygonObstacle(IEnumerable<PointF> Points,Brush pDrawBrush,Pen pDrawPen):base(CenterPoint(Points),null,0)
        {
            _DrawMethod = new FilledPolyDrawMethod(pDrawPen, pDrawBrush);
            _Poly = new Polygon(Points.ToArray());
            //base.Size = _Poly.GetBounds().Size;

        }
        protected bool InvokeObstacleHit(BCBlockGameState gstate,cBall Source,PolygonObstacle obstacle)
        {
            var copied = ObstacleHit;
            if (copied != null)
            {
                var created = new PolygonObstacleHitEventArgs(gstate, Source, obstacle);
                copied(this, created);
                return created.Cancel;
            }

            return false;
        }
        protected bool InvokeBeforeObstacleHit(BCBlockGameState gstate,cBall Source,PolygonObstacle obstacle)
        {
            var copied = BeforeObstacleHit;
            if (copied != null)
            {
                var created = new PolygonObstacleHitEventArgs(gstate, Source, obstacle);
                copied(this, created);
                return created.Cancel;
            }

            return false;

        }
        protected bool InvokeObstacleDestroy(BCBlockGameState gstate,cBall Source,PolygonObstacle obstacle)
        {
            var copied = ObstacleDestroy;
            if (copied != null)
            {
                var created = new PolygonObstacleDestroyEventArgs(gstate, Source, obstacle);
                copied(this, created);
                return created.Cancel;
            }

            return false;



        }
        
        public override void Draw(Graphics g)
        {
           // Debug.Print("Draw:" + _Poly.Center.ToString());
            _DrawMethod.Draw(this, _Poly, g);
        }
        int delayoffset = 0;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (Frozen) return false;
            PointF usedoffset=PointF.Empty;
            BCBlockGameState.IncrementLocation(gamestate, ref _Location, _Velocity,ref usedoffset);
            _Poly.Offset(usedoffset);
            if (!(new Polygon(gamestate.GameArea).Contains(_Poly)))
            {
                //reflect 
                var result = GeometryHelper.PolygonCollision(new Polygon(gamestate.GameArea), _Poly, Velocity);
                var copied = new PointF(-result.MinimumTranslationVector.X, -result.MinimumTranslationVector.Y);
                if (Math.Abs(result.MinimumTranslationVector.X) > Math.Abs(result.MinimumTranslationVector.Y))
                {
                    //mirror X speed.
                    Velocity = new PointF(-Velocity.X, Velocity.Y);
                }
                else
                {
                    Velocity = new PointF(Velocity.X, -Velocity.Y);
                }

            }
            _Velocity = _VelocityDecayFunction(this, _Velocity);
            CurrAngle += AngleSpeed;
            _AngleSpeed = _AngleSpeedDecayFunction(this, _AngleSpeed);
            
            //check for ball collisions.
            foreach (cBall checkball in gamestate.Balls)
            {
                Polygon ballpoly = checkball.GetBallPoly();
                
                if (_Poly.IntersectsWith(ballpoly) && !InvokeBeforeObstacleHit(gamestate,checkball,this))
                {
                    //only if it intersects.
                    Vector Adjustment;
                    GeometryHelper.PolygonCollisionResult pcr = GeometryHelper.PolygonCollision(_Poly, 
                                                                                                ballpoly, new Vector(checkball.Velocity.X+Velocity.X, checkball.Velocity.Y+Velocity.Y));

                    Adjustment = pcr.MinimumTranslationVector;
                    checkball.Velocity = checkball.Velocity.Mirror(pcr.MinimumTranslationVector);
                    checkball.Velocity = new PointF(checkball.Velocity.X, checkball.Velocity.Y);
                    checkball.Location = new PointF(checkball.Location.X - Adjustment.X, checkball.Location.Y - Adjustment.Y);
                    BCBlockGameState.Soundman.PlaySound("bounce");
                    //determine percentage of "knockback" to us based on the ratio between the ball radius and our average radius.
                    float ourAverage = _Poly.AverageRadius();
                    float ballradii = checkball.Radius;
                    float ratio = ballradii / ourAverage;
                    PointF mCenter = GetRectangle().CenterPoint();
                    Velocity = new PointF(Velocity.X + pcr.MinimumTranslationVector.X * ratio, Velocity.Y + pcr.MinimumTranslationVector.Y * ratio);
                    //get angle between the ball's previous location (assumed from the velocity) and it's current pos,
                    //with our center point as the pivot.
                    PointF PrevPosition = checkball.PrevLocation;
                    PointF CurrLocation = checkball.Location;
                    var PrevAngle = BCBlockGameState.GetAngle(mCenter, PrevPosition);
                    var CurrentAngle = BCBlockGameState.GetAngle(mCenter, CurrLocation);
                    AngleSpeed = CurrentAngle - PrevAngle;

                    //reduce our size, as well.
                    _Poly = _Poly.Scale(0.9f);

                    if (_Poly.Area() < MinimumSize && !InvokeObstacleHit(gamestate,checkball,this))
                    {
                        //Fire the OnDestroy event
                        if(!InvokeObstacleDestroy(gamestate,checkball,this))
                            return true; //tweak: make it explode or something, too.
                    }



                }
                


            }


            if (_fShoot)
            {
                if (delayoffset++ == _ShootDelay) 
                {
                    delayoffset = 0;
                    //shoot lightning. Because lightning is da' shit.
                    //choose a random point on this polygon.
                    PointF selectedpoint = BCBlockGameState.Choose(Poly.Points);
                    //get angle between that point and the paddle.
                    //if there is no paddle, use a random value.
                    double fireangle = gamestate.PlayerPaddle != null ?
                                           BCBlockGameState.GetAngle(selectedpoint, gamestate.PlayerPaddle.CenterPoint()) :
                                           BCBlockGameState.rgen.NextDouble() * Math.PI * 2;
                    PointF createdvel = BCBlockGameState.VaryVelocity(new PointF((float)Math.Cos(fireangle), (float)Math.Sin(fireangle)), Math.PI / 12);

                    LightningShot ls = new LightningShot(selectedpoint, createdvel)
                                           {
                                               DamageBlocks = false,
                                               DamagePaddle = true
                                           };
                    gamestate.Defer(() => gamestate.GameObjects.AddLast(ls));
                }


            }


            return !gamestate.GameArea.IntersectsWith(_Poly.GetBounds().ToRectangle());
        }


    }
}