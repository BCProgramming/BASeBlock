using System;
using System.Drawing;
using System.Linq;
using BASeBlock.Particles;

namespace BASeBlock.Projectiles
{
    //similar in some ways to the "RayShot" but performs a sort of angular Drunkards walk. It also branches off itself.

    class LightningShot:Projectile 
    {
        public class LightningShotEventArgs : EventArgs
        {
            private BCBlockGameState _GameState;
            private LightningShot _Source;
            public LightningShot Source { get { return _Source; } set { _Source = value; } }
            public BCBlockGameState GameState { get { return _GameState; } set { _GameState = value; } }
            public LightningShotEventArgs(BCBlockGameState gstate,LightningShot pSource)
            {
                _GameState = gstate;
                _Source = pSource;
            }

        }
        public event EventHandler<LightningShotEventArgs> LightningSpark;
        private Color _DrawColor = Color.Yellow;
        private Pen _DrawPen = null;
        private int RecurseCount = 0;
        private float _MinArcLength = 5;
        private float _MaxArcLength = 64;
        private float _AngleDeviation = (float)Math.PI / 4f;
        private bool _DamageBlocks = false;
        private bool _DamagePaddle = false;

        public bool DamageBlocks { get { return _DamageBlocks; } set { _DamageBlocks = value; } }
        public bool DamagePaddle { get { return _DamagePaddle; } set { _DamagePaddle = value; } }
        protected void RaiseLightningSpark(LightningShotEventArgs e)
        {
            var copied = LightningSpark;
            if(copied!=null)
                copied(this, e);


        }
        //properties.
        /// <summary>
        /// Sets/returns the Color of the Lightning Effect.
        /// </summary>
        /// <remarks>Setting DrawColor will recreate the DrawPen Property with the given color and a pen Width of 2. Retrieving 
        /// The Color will only retrieve a valid value when DrawColor was previously set.</remarks>
        public Color DrawColor { get { return _DrawColor; } set { _DrawColor = value; _DrawPen.Dispose(); _DrawPen = new Pen(_DrawColor); } }

        /// <summary>
        /// Sets/Returns the Pen to use for Drawing this LightingShot. 
        /// </summary>
        public Pen DrawPen { get { return _DrawPen; } set { _DrawPen = value; } }

        public float MinArcLength { get { return _MinArcLength; } set { _MinArcLength = value; } }
        public float MaxArcLength { get { return _MaxArcLength; } set { _MaxArcLength = value; } }

        public float AngleDeviation { get { return _AngleDeviation; } set { _AngleDeviation = value; } }
        


        public LightningShot(PointF pLocation,PointF pVelocity):base(pLocation,pVelocity)
        {
            DrawPen = new Pen(DrawColor, 3);
            LightningSpark += LightningShot_LightningSpark;
        }

        void LightningShot_LightningSpark(object sender, LightningShot.LightningShotEventArgs e)
        {
            //throw new NotImplementedException();
            //create LightOrbs around this LightningShot.
            int numbetween = BCBlockGameState.rgen.Next(2, 5);
            for (int i = 0; i < numbetween; i++)
            {
                //select random percent.
                float usepercentage = (float)BCBlockGameState.rgen.NextDouble();
                PointF gotbetween = GeometryHelper.PercentLine(e.Source.FirstPoint, e.Source.SecondPoint, usepercentage);
                //create lightorb at that location.
                LightOrb lo = new LightOrb(gotbetween, e.Source.DrawColor, 15);
                e.GameState.Defer(() => e.GameState.Particles.Add(lo));



            }



        }
        
        public override void Draw(Graphics g)
        {
            //throw new NotImplementedException();
        }
        private PointF _FirstPoint, _SecondPoint;
        private TimeSpan _MinArcDelay = new TimeSpan(); //minimum delay before next arc appears.
        private TimeSpan _MaxArcDelay = new TimeSpan(); //maximum delay before next arc appears.

        public TimeSpan MinArcDelay { get { return _MinArcDelay; } set { _MinArcDelay = value; } }
        public TimeSpan MaxArcDelay { get { return _MaxArcDelay; } set { _MaxArcDelay = value; } }
        private Func<BCBlockGameState, LightningShot,TimeSpan> _getDelay = (gstate, ls) => 
            new TimeSpan((int)(BCBlockGameState.rgen.NextDouble() * (ls.MaxArcDelay.Ticks - ls.MinArcDelay.Ticks)) + ls.MinArcDelay.Ticks);


        public Func<BCBlockGameState, LightningShot, TimeSpan> getDelay { get { return _getDelay; } set { _getDelay = value; } } 

        public PointF FirstPoint { get { return _FirstPoint; } set { _FirstPoint = value; } }
        public PointF SecondPoint { get { return _SecondPoint; } set { _SecondPoint = value; } }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //performframe is "where the magic happens".

            //step one: decide on a random arc length.
            
            //First Point is Location.
            _FirstPoint = this.Location;
            _SecondPoint = new PointF(FirstPoint.X + Velocity.X, FirstPoint.Y + Velocity.Y);

            DustParticle FirstDust = new DustParticle(FirstPoint,1000);
            DustParticle SecondDust = new DustParticle(SecondPoint,1000);
            FirstDust.Velocity = SecondDust.Velocity = PointF.Empty;
            var coreAngle = BCBlockGameState.GetAngle(FirstDust.Location, SecondDust.Location);
            LineParticle ls = new LineParticle(FirstDust, SecondDust,DrawPen);
            ls.TTL=400;
            gamestate.Defer(()=>gamestate.Particles.Add(ls));

            int blocksdestroyed = 0;
            //check for block destruction.
            if (DamageBlocks)
            {
                foreach (var iterate in
                    from b in gamestate.Blocks where BCBlockGameState.IntersectLine(FirstDust.Location, SecondDust.Location, b.BlockRectangle) != null select b)
                {
                    blocksdestroyed++;
                    //change: instead of simply breaking the block, we will also spawn a BoxDestructor with the Velocity of this lighting arc.

                    gamestate.Defer(() =>
                    {
                        BoxDestructor bd = new BoxDestructor(iterate, Velocity);

                        BCBlockGameState.Block_Hit(gamestate, iterate, Velocity);
                        gamestate.GameObjects.AddLast(bd);


                    });

                }


            }

            //Step two: choose a random Angle between -45 and 45 degrees. -PI/4 and PI/4...
            //Don't do this if we found blocks- block impacts will "stop" lightning.
            if(gamestate.GameArea.Contains(SecondPoint.ToPoint()) && RecurseCount< 7 && blocksdestroyed ==0 )
            {
                //branch appropros.
                for(int i=0;i<BCBlockGameState.rgen.Next(1,3);i++)
                {
                    

                    double chosenAngle =coreAngle+( (BCBlockGameState.rgen.NextDouble() * (_AngleDeviation*2)) - _AngleDeviation);    
                    float usearclength = (float)(_MinArcLength + (BCBlockGameState.rgen.NextDouble() * (_MaxArcLength - _MinArcLength)));
                    PointF makevel = new PointF((float)(Math.Cos(chosenAngle)*usearclength),(float)(Math.Sin(chosenAngle)*usearclength));
                    LightningShot lshot = new LightningShot(SecondPoint,makevel);
                    lshot.RecurseCount = RecurseCount + 1;
                    lshot.DamageBlocks = DamageBlocks;
                    lshot.DamagePaddle = DamagePaddle;
                    gamestate.Defer(getDelay(gamestate,this), ()=>gamestate.GameObjects.AddLast(lshot));
                }
            
            
            }
            if(gamestate.PlayerPaddle!=null && DamagePaddle)
                if (null!=BCBlockGameState.IntersectLine(FirstDust.Location, SecondDust.Location, gamestate.PlayerPaddle.getRectangle()))
                {
                    //damage the paddle...
                    gamestate.PlayerPaddle.HP--;
                }
            


            return true;
        }
    }
}
