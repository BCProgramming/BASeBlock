/*
 * BASeCamp BASeBlock
Copyright (c) 2011, Michael Burgwin
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    Neither the name of BASeCamp Corporation nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.PaddleBehaviours;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.BASeBlock.Projectiles;
using Img;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// abstract GameObject class, will be used to implement scores being displayed "in-place" in the game screen, bullets (probably) and so forth.
    /// </summary>
    public abstract class GameObject
    {
        public delegate void GameObjectFrameFunction(GameObject sourceobject, BCBlockGameState gamestate);
        public event GameObjectFrameFunction ObjectFrame;

        protected void InvokeFrameEvent(BCBlockGameState gamestate)
        {
            var copied = ObjectFrame;
            if (copied != null)
                copied(this, gamestate);


        }
        protected GameObject()
        {


        }


        public override string ToString()
        {
            return base.ToString() + "\n" +
                "Frozen:" + this.Frozen + "\n";
                
        }


        //Note: had to do a bit of hackney-ing here to figure out how best to do this. Ball and Blocks have parameters indicating objects to remove,, so I designed it that way for the 
        //GameObjects from the start.
        /// <summary>
        /// used to perform a single frame of this gameobjects animation.
        /// </summary>
        /// <param name="gamestate">Game State object</param>
        /// <returns>true to indicate that this gameobject should be removed. False otherwise.</returns>
        public virtual bool PerformFrame(BCBlockGameState gamestate)
        {

            InvokeFrameEvent(gamestate);
            return false;

        }
        private bool _frozen = false;

        public virtual bool getFrozen()
        {

            return _frozen;

        }
        public virtual void setFrozen(bool newvalue)
        {
            _frozen = newvalue;

        }
        /// <summary>
        /// if True, means this Object will not animate while the game is paused (enemies, for example).
        /// false means it will, which could be desirable for other effects. Derived classes can hook get/set access by overriding
        /// the virtual setFrozen and getFrozen methods.
        /// </summary>
        /// <returns></returns>
        public bool Frozen { get { return getFrozen(); } set { setFrozen(value); } }
        
        public abstract void Draw(Graphics g);
        public static double Angle(double px1, double py1, double px2, double py2)
        {

            // Negate X and Y values
            double pxRes = px2 - px1;

            double pyRes = py2 - py1;
            double angle = 0.0;


            double drawangle = 0;
            const double drawangleincrement = Math.PI / 20;

            // Calculate the angle
            if (pxRes == 0.0)
            {
                if (pxRes == 0.0)

                    angle = 0.0;
                else if (pyRes > 0.0) angle = System.Math.PI / 2.0;

                else
                    angle = System.Math.PI * 3.0 / 2.0;

            }
            else if (pyRes == 0.0)
            {
                if (pxRes > 0.0)

                    angle = 0.0;

                else
                    angle = System.Math.PI;

            }

            else
            {
                if (pxRes < 0.0)

                    angle = System.Math.Atan(pyRes / pxRes) + System.Math.PI;
                else if (pyRes < 0.0) angle = System.Math.Atan(pyRes / pxRes) + (2 * System.Math.PI);

                else
                    angle = System.Math.Atan(pyRes / pxRes);

            }

            // Convert to degrees
            return angle;



        }

    }
    public abstract class SizeableGameObject : GameObject, iLocatable
    {
        protected PointF _Location;
        public SizeF Size { get; set; }
        public PointF Location { get { return _Location; } set { _Location = value; } }

        protected SizeableGameObject(PointF pLocation, SizeF objectsize)
        {
            Size = objectsize;
            Location = pLocation;

        }

        public PointF CenterPoint()
        {
            return new PointF(Location.X + (Size.Width / 2), Location.Y + (Size.Height / 2));



        }
        public RectangleF getRectangle()
        {
            return new RectangleF(Location, Size);


        }

    }
    public class AnimatedImageObject : SizeableGameObject
    {

        public Image[] ObjectImages;
        protected int frameadvancedelay = 3;
        protected int countframe = 0;
        protected int currimageframe = 0;
        protected PointF _Velocity;
        public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }
        protected VelocityChanger _VelocityChange = new VelocityChangerLinear();

        public VelocityChanger VelocityChange
        {
            get { return _VelocityChange; }
            set { _VelocityChange = value; }


        }

        public int CurrentFrame { get { return currimageframe; } set { currimageframe = value; } }
        public Image CurrentFrameImage
        {
            get
            {
                try
                {

                    return ObjectImages[CurrentFrame];


                }
                catch (IndexOutOfRangeException erange)
                {
                    Debug.Print("stop");

                }
                return null;
            }
        }




        public AnimatedImageObject(PointF Location, SizeF ObjectSize, Image[] pObjectImages, int pframeadvancedelay)
            : base(Location, ObjectSize)
        {
            ObjectImages = pObjectImages;
            frameadvancedelay = pframeadvancedelay;

        }

        public AnimatedImageObject(PointF Location, SizeF ObjectSize, Image[] pObjectImages)
            : this(Location, ObjectSize, pObjectImages, 3)
        {

        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //throw new NotImplementedException();
            Location = _VelocityChange.PerformFrame(gamestate,Location);
            if (ObjectImages.Length == 1) return false;
            if (frameadvancedelay > 0)
            {
                countframe++;
                if (countframe >= frameadvancedelay)
                {
                    currimageframe++;
                    if (currimageframe > ObjectImages.Length - 1) currimageframe = 0;
                    countframe = 0;
                }
            }
            return false;


        }
        public override void Draw(Graphics g)
        {
            g.DrawImage(ObjectImages[currimageframe], Location.X, Location.Y, Size.Width, Size.Height);
        }

    }




    public class GameImageObject : SizeableGameObject
    {
        public Image ObjectImage = null;

        public GameImageObject(PointF Location, SizeF ObjectSize, Image ImageUse)
            : base(Location, ObjectSize)
        {
            ObjectImage = ImageUse;

        }
        public GameImageObject(PointF Location, Image ImageUse)
            : base(Location, ImageUse.Size)
        {
            ObjectImage = ImageUse;

        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            return false;
        }

        public override void Draw(Graphics g)
        {
            g.DrawImage(ObjectImage, Location.X, Location.Y, Size.Width, Size.Height);
        }
    }
    /// <summary>
    /// Interface implemented by objects that have a location. bloody near everything, ideally.
    /// </summary>
    public interface iLocatable
    {
        PointF Location { get; set; }

    }
    public interface iProjectile : iLocatable
    {

        PointF Velocity { get; set; }


    }
    /// <summary>
    /// Interface that is used for all objects that have a size, location, and speed.
    /// (Not necessarily limited to projectiles)
    /// </summary>
    public interface iSizedProjectile : iProjectile
    {
        SizeF Size { get; set; }

    }

    //implemented by all projectiles. Or, it should be.
    public abstract class Projectile : GameObject, iProjectile
    {
        protected PointF _Location;
        protected PointF _Velocity;
        protected PointF _VelocityDecay = new PointF(1, 1);
        protected PointF _VelocityIncrement = new PointF(0, 0);
        public PointF Location
        {
            get { return getLocation(); }
            set { setLocation(value); }
        }

        public PointF Velocity
        {
            get { return getVelocity(); }
            set { setVelocity(value); }
        }

        public PointF VelocityDecay
        {
            get { return getVelocityDecay(); }
            set { setVelocityDecay(value); }
        }

        public PointF VelocityIncrement
        {
            get { return getVelocityIncrement(); }
            set { setVelocityIncrement(value); }
        }

        protected virtual PointF getLocation()
        {

            return _Location;

        }
        protected virtual void setLocation(PointF newLocation)
        {

            _Location = newLocation;

        }
        protected virtual PointF getVelocity()
        {
            return _Velocity;

        }

        protected virtual void setVelocity(PointF newVelocity)
        {

            _Velocity = newVelocity;

        }
        protected virtual PointF getVelocityDecay()
        {
            return _VelocityDecay;

        }
        protected virtual void setVelocityDecay(PointF newVelocityDecay)
        {

            _VelocityDecay = newVelocityDecay;

        }
        protected virtual PointF getVelocityIncrement()
        {
            return _VelocityIncrement;

        }
        protected virtual void setVelocityIncrement(PointF setvalue)
        {
            _VelocityIncrement = setvalue;

        }

        protected Projectile(PointF pLocation, PointF pVelocity)
        {
            _Location = pLocation;
            _Velocity = pVelocity;

        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {

            _Location = new PointF(_Location.X + _Velocity.X, _Location.Y + _Velocity.Y);
            _Velocity = new PointF(_Velocity.X + _VelocityIncrement.X, _Velocity.Y + _VelocityIncrement.Y);
            _Velocity = new PointF(_Velocity.X * _VelocityDecay.X, _Velocity.Y * _VelocityDecay.Y);
            return gamestate.GameArea.Contains((int)_Location.X, (int)_Location.Y) || base.PerformFrame(gamestate);

        }

    }

    public abstract class SizedProjectile : Projectile, iSizedProjectile
    {
        //a projectile with a size... this is measured from the "center" which we take to be the _Location inherited field.
        protected SizeF _Size;

        public SizeF Size
        {
            get { return getSize(); }
            set { setSize(value); }
        }
        protected virtual SizeF getSize()
        {
            return _Size;


        }
        protected virtual void setSize(SizeF newsize)
        {
            _Size = newsize;


        }

        protected SizedProjectile(PointF pLocation, PointF pVelocity, SizeF size)
            : base(pLocation, pVelocity)
        {
            _Size = size;

        }
        public RectangleF getRect()
        {

            return new RectangleF(_Location.X - _Size.Width / 2, _Location.Y - _Size.Height / 2, _Size.Width, _Size.Height);

        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            return base.PerformFrame(gamestate);
        }



    }

    //Orb  that draws an image in a specific colour.

    public class HitscanBullet : Projectile
    {

        public bool Penetrate = false; //whether we penetrate blocks.
        public bool Tracer = true;


        public enum HitscanStrengthConstants
        {
            /// <summary>
            /// ignore. Does not hit blocks.
            /// </summary>
            hitscan_ignore,
            /// <summary>
            /// acts like a bullet. when it encounters a block it spawns a bullet at that location. the shot stops there.
            /// </summary>
            hitscan_bullet,
            /// <summary>
            /// hits the first block it touches. The block may or may not be destroyed, depending on the block itself.
            /// </summary>
            hitscan_hit,
            
        }

        Color _BulletColor = Color.Green;
        private PointF _Origin;
        private HitscanStrengthConstants _Strength = HitscanStrengthConstants.hitscan_bullet;
        public HitscanStrengthConstants Strength { get { return _Strength; } set { _Strength = value; } }
        public PointF Origin { get { return _Origin; } set { _Origin = value; } }
        public Color BulletColor { get { return _BulletColor; } set { _BulletColor = value; } }
        //public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }
        public HitscanBullet(PointF pOrigin, PointF pVelocity):base(pOrigin,pVelocity)
        {
            _Origin = pOrigin;
            
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {

            //hitscan. We only "take" a single frame to hit something.
            //first, normalize the Velocity.
            _Velocity = _Velocity.Normalize();

            //now, starting from _Origin, advance position by _Velocity until we hit a block or leave the gamearea.
            List<Block> blockshit = new List<Block>();
            PointF currpos = _Origin;
            PointF lastpos = _Origin;
            bool scancomplete = false;
            int spawnjump = 0;
            int particlecomparator = Math.Max(gamestate.Particles.Count-500, 2);
            //particlecomparator is our modulus. This will be modulo'd with spawnjump each iteration
            
            while (!scancomplete)
            {
                spawnjump++;
                currpos = new PointF(currpos.X + _Velocity.X, currpos.Y + _Velocity.Y);

                PointF diff = new PointF(currpos.X-lastpos.X,currpos.Y-lastpos.Y);
                //spawn some particles here.
                if (spawnjump>particlecomparator && Tracer)
                {
                    spawnjump = 0;
                    for (int i = 0; i < 1; i++)
                    {
                        
                            float randomspot = (float)BCBlockGameState.rgen.NextDouble();
                            PointF randomoffset = new PointF(currpos.X + diff.X * randomspot, currpos.Y + diff.Y * randomspot);
                            if (BCBlockGameState.rgen.NextDouble() > 0.5)
                            {
                                DustParticle dp = new DustParticle(randomoffset, 3, 25, _BulletColor);
                                dp.Important = true;
                                gamestate.Particles.Add(dp);
                            }
                            else
                            {
                                LightOrb dp = new LightOrb(randomoffset, Color.Green, 16);
                                dp.TTL = 25;

                                dp.Important = true;
                                gamestate.Particles.Add(dp);
                            }
                    }
                }
                
                //are we outside gamearea?
                if (!gamestate.GameArea.Contains(currpos.ToPoint()))
                    scancomplete = true;


                //have we hit a block?
                var hitblock = BCBlockGameState.Block_HitTestOne(gamestate.Blocks, currpos);
                if (hitblock != null && !blockshit.Contains(hitblock))
                {
                    blockshit.Add(hitblock);


                    if(_Strength == HitscanStrengthConstants.hitscan_bullet)
                    {
                        //create a bullet at currpos, make it go in our direction.
                        Bullet bb = new Bullet(currpos, _Velocity, false);
                        bb.BulletBrush = new SolidBrush(Color.Transparent); //invisible bullet...
                        gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => gamestate.GameObjects.AddLast(bb)));
                    }
                else if(_Strength==HitscanStrengthConstants.hitscan_hit)
                    {
                        
                    
                    gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => BCBlockGameState.Block_Hit(gamestate, hitblock, _Velocity)));
                    }

                    if (!Penetrate) scancomplete = true;
                }


                lastpos = currpos;
            }

            if (!Tracer)
            {
                DustParticle pa = new DustParticle(Origin, 800,9000,Color.Transparent);
                DustParticle pb = new DustParticle(currpos,800,9000,Color.Transparent);
                
                pa.Velocity = PointF.Empty;
                pb.Velocity = PointF.Empty;

                LineParticle ls = new LineParticle(pa, pb,new Pen(Color.Yellow,1));
                ls.Important = true;
                ls.TTL = 750;
                gamestate.Particles.Add(ls);

                //show the impact point.
                PointF Impactpoint = currpos;
                for (int i = 0; i < 14 * BCBlockGameState.Settings.ParticleGenerationFactor; i++)
                {
                    Particle xp = BCBlockGameState.rgen.NextDouble() > 0.5 ? 
                        (Particle)
                        new EmitterParticle(Impactpoint,(gstate,emitter,aint,bint)=>
                            {
                                float ssspeed = (float)BCBlockGameState.rgen.NextDouble() * 5 + 2;
                                PointF ssusespeed = BCBlockGameState.VaryVelocity(new PointF(0,-ssspeed), Math.PI / 10);
                                DustParticle addit = new DustParticle(emitter.Location, 40);
                                addit.Velocity = ssusespeed;
                                return addit;
                            })
                            : (Particle)new DustParticle(Impactpoint);
                    float speed = (float)BCBlockGameState.rgen.NextDouble() * 5 + 2;
                    PointF usespeed = BCBlockGameState.VaryVelocity(new PointF(0,-speed), Math.PI / 10);
                    xp.Velocity = usespeed;
                    gamestate.Particles.Add(xp);
                }


            }







            return true; //we only exist for one frame.
        }
        public override void Draw(Graphics g)
        {
            //no implementation...
        }
    }


    /// <summary>
        /// SpinShot: a Large (8) ball, surrounded by a set of smaller balls.
        /// On construction: create the appropriate balls; add them to the GameState; and hook their BlockHit event.
        /// when a ball hit's a block,set it's velocity to the appropriate speed, and stop "managing" it from here.
        /// //when the middle ball hit's something (or button is pressed...) release all the small balls.
        /// </summary>
        public class SpinShot : Projectile
        {
            #region inner class
            private class SpinShotsubBallInfo
            {
                //holds data about one of the spinshot's rotating balls.
                /// <summary>
                /// The ball object itself.
                /// </summary>
                public cBall BallObject { get; set; }
                /// <summary>
                /// current angle of the ball (in relation to the origin ball)
                /// </summary>
                public double Angle { get; set; }
                /// <summary>
                /// Current speed of the ball's rotation.
                /// </summary>
                public double AngleSpeed { get; set; }
                //calculated distance from the origin.
                public double Radius { get; set; }

                /// <summary>
                /// "Origin" ball; the ball upon which our calculations are based.
                /// </summary>
                public cBall OriginBall { get; set; }


                /// <summary>
                /// 
                /// </summary>
                /// <param name="Origin">cBall object to be used as the origin location for rotary calculations.</param>
                /// <param name="Derived"></param>
                public SpinShotsubBallInfo(cBall pOrigin, cBall pDerived, double pAngle, double pAngleSpeed, double pRadius)
                {
                    OriginBall = pOrigin;
                    BallObject = pDerived;

                    Angle = pAngle;
                    AngleSpeed = pAngleSpeed;
                    Radius = pRadius;
                    //hook our ball's BallImpact Event...
                    BallObject.BallImpact += new cBall.ballimpactproc(BallObject_BallImpact);
                    //the "owner" class should be hooking it as well so that we (as a SpinShotsubballinfo) are removed from any relevant future calculations,
                    //allowing the ball to "fly off".

                }
                /// <summary>
                /// retrieves the current speed vector to be used for the ball, based on radius, origin, and angle.
                /// </summary>
                /// <returns></returns>
                private PointF getSpeedVector()
                {
                    //speed vector will be the difference between our current position and what will be our next position.
                    //get the two angle we will use...
                    double currentangle = Angle;
                    double nextangle = currentangle + AngleSpeed;

                    //acquire the positions 
                    PointF currpos = getPosition();
                    PointF nextpos = calcPosition(nextangle, Radius);
                    //get the difference, and return that as a PointF structure.
                    return new PointF((float)(nextpos.X - currpos.X), (float)(nextpos.Y - currpos.Y));



                }



                //retrieves the current position to be used for the ball, based on Origin, Angle, and radius.
                private PointF getPosition()
                {
                    return calcPosition(Angle, Radius);

                }
                private PointF calcPosition(double Angle, double Radius)
                {
                    //Debug.Print("Originball:" + OriginBall.Location);
                    PointF resultp = new PointF((float)((Math.Cos(Angle) * Radius) + OriginBall.Location.X),
                        (float)((Math.Sin(Angle) * Radius) + OriginBall.Location.Y));
                    return resultp;

                }
                public void ReleaseBall()
                {
                    PointF gotv = getSpeedVector();
                    BallObject.Velocity = new PointF(gotv.X * 10, gotv.Y * 10);
                    BallObject.BallImpact -= BallObject_BallImpact;
                    BallObject = null;

                }

                void BallObject_BallImpact(cBall ballimpact)
                {
                    //release the ball... Set it's Speed to getSpeedVector.
                    ballimpact.Velocity = getSpeedVector();
                    //"owner" SpinShot will properly 'release' the ball by removing us from the collection.

                }
                public void PerformFrame(BCBlockGameState gamestate)
                {
                    //performs a single frame; we only move the ball, though- we don't actually affect it's velocity...
                    BallObject.Velocity = getSpeedVector();
                    Angle += AngleSpeed;
                    BallObject.Location = getPosition();



                }

            }
            #endregion

            private List<SpinShotsubBallInfo> SubObjects;
            public cBall OurBall;


            public SpinShot(BCBlockGameState gamestate, PointF pLocation, int Numorbit, float radius, float rotationspeed, PointF pVelocity)
                : this(gamestate, new cBall(pLocation, pVelocity), pLocation, Numorbit, radius, rotationspeed)
            {
                LaserSpinBehaviour SpinshotSpinBehaviour = new LaserSpinBehaviour(new TimeSpan(0, 0, 0, 0, 300));
                SpinshotSpinBehaviour.EventFunction = SpinShotLaser;
                OurBall.Behaviours.Add(SpinshotSpinBehaviour);
                OurBall.Radius = 5;
                OurBall.DrawColor = Color.Blue;
                OurBall.Behaviours.Add(new TempBallBehaviour());

            }
            private void SpinShotLaser(BCBlockGameState gstate, LaserShot shot, LinkedList<PointF> laserPoints)
            {


            }
            protected override PointF getLocation()
            {
                return OurBall.Location;
            }
            protected override void setLocation(PointF newlocation)
            {
                OurBall.Location = newlocation;

            }
            protected override PointF getVelocity()
            {
                return OurBall.Velocity;
            }
            protected override void setVelocity(PointF newVelocity)
            {
                OurBall.Velocity = newVelocity;
            }
            public void SparkleEmitter(cBall ballobj, BCBlockGameState gamestate)
            {
                PointF VelocityUse = new PointF((float)(ballobj.Velocity.X * 0.1 + (BCBlockGameState.rgen.NextDouble() - 0.5) * 2),
                    (float)(ballobj.Velocity.Y * 0.1 + (BCBlockGameState.rgen.NextDouble() - 0.5) * 2));
                Color usesparklecolor = BCBlockGameState.Choose(new Color[] { Color.Red, Color.Yellow, Color.LimeGreen, Color.Pink, Color.Magenta, Color.Chartreuse });
                if (BCBlockGameState.rgen.NextDouble() > 0.8) return; //don't add a particle.
                if (BCBlockGameState.rgen.NextDouble() > 0.5)
                {

                    VelocityUse = new PointF(0, 0);


                }
                gamestate.Particles.Add(new Sparkle(ballobj.Location, VelocityUse, usesparklecolor));
            }

            protected static TextureBrush spinshotbrush = new TextureBrush(BCBlockGameState.Imageman.getLoadedImage("spinshotbg"));
            /// <summary>
            /// Creates the Spinshot, using the given ball as the origin
            /// </summary>
            /// <param name="gamestate"></param>
            /// <param name="useorigin"></param>
            /// <param name="pLocation"></param>
            /// <param name="Numorbit"></param>
            /// <param name="radius"></param>
            /// <param name="rotationspeed"></param>
            /// <param name="pVelocity"></param>
            public SpinShot(BCBlockGameState gamestate, cBall useorigin, PointF pLocation, int Numorbit, float radius, float rotationspeed)
                : base(pLocation, useorigin.Velocity)
            {
                SubObjects = new List<SpinShotsubBallInfo>();
                OurBall = useorigin;


                OurBall.BallImpact += new cBall.ballimpactproc(OurBall_BallImpact);
                gamestate.Balls.AddLast(OurBall);
                //we let the game logic handle the ball, but our performframe will "update" the position of the sub-balls.
                //speaking of which- we need to create those.
                float Angleuse = (float)((Math.PI * 2) / Numorbit);
                float useangle = 0;
                for (int i = 0; i < Numorbit; i++)
                {
                    useangle = ((float)i) * Angleuse;
                    PointF useposition = new PointF((float)(Math.Cos(useangle) * radius) + pLocation.X,
                        (float)(Math.Sin(useangle) * radius) + pLocation.Y);
                    cBall derivedball = new cBall(useposition, new PointF(0.1f, 0.1f));
                    //derivedball.Behaviours.Add(new TempBallBehaviour());
                    derivedball.Behaviours.Add(new NonReboundableBallBehaviour());
                    derivedball.Behaviours.Add(new ParticleEmitterBehaviour(SparkleEmitter));
                    derivedball.DrawPen = new Pen(Color.Transparent);
                    derivedball.DrawBrush = spinshotbrush;
                    SpinShotsubBallInfo subinfo = new SpinShotsubBallInfo(OurBall, derivedball, useangle, rotationspeed, radius);
                    SubObjects.Add(subinfo);
                    gamestate.Balls.AddLast(derivedball);


                }




            }

            void OurBall_BallImpact(cBall ballimpact)
            {
                //throw new NotImplementedException();
                hasbeendestroyed = true;

                //release All the balls in our subobjects.
                foreach (var subobj in SubObjects)
                {
                    subobj.ReleaseBall();

                }
                SubObjects = new List<SpinShotsubBallInfo>();

            }
            bool hasbeendestroyed = false;

            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                spinshotbrush.RotateTransform((float)(Math.PI * 2 * (BCBlockGameState.rgen.NextDouble())));
                Debug.Print("Spinshot Ball Location:" + OurBall.Location);
                foreach (var subobj in SubObjects)
                {
                    subobj.PerformFrame(gamestate);


                }
                if (hasbeendestroyed)
                {
                    OurBall.BallImpact -= OurBall_BallImpact;
                    foreach (var subobj in SubObjects)
                    {

                        subobj.ReleaseBall();

                    }
                    SubObjects = new List<SpinShotsubBallInfo>();
                    gamestate.Defer(() => gamestate.GameObjects.Remove(this));
                    


                }
                return hasbeendestroyed;

            }

            public override void Draw(Graphics g)
            {
                //nothing...
            }
        }

        


        /// <summary>
        /// Class that represents a rectangular object that destroys anything in it's path.
        /// </summary>
        public class BoxDestructor : Projectile
        {
            private Image AlphadImage = null;
            private Image useDrawImage = null;
            private SizeF ObjectSize;
            public delegate bool DestructableTestFunction(Block testblock);
            private DestructableTestFunction _DestructableTest = DestructableTest_All;

            public DestructableTestFunction DestructableTest
            {
                get { return _DestructableTest; }
                set
                {
                    if (value == null)
                        _DestructableTest = DestructableTest_All;
                    else
                        _DestructableTest = value;
                }
            }

            public RectangleF ObjectRectangle
            {
                get
                {
                    return new RectangleF(Location.X - ObjectSize.Width / 2, Location.Y - ObjectSize.Height / 2,
                                          ObjectSize.Width, ObjectSize.Height);
                }
                set
                {
                    Location = value.CenterPoint();
                    ObjectSize = value.Size;


                }
            }
            /// <summary>
            /// The Block that this BoxDestructor was created to represent (or null if created some other way)
            /// </summary>
            protected Block OriginalBlock = null;
            private List<Block> ignorelist = new List<Block>();
            public static bool DestructableTest_Default(Block testthis)
            {

                return testthis.MustDestroy();

            }
            public static bool DestructableTest_All(Block testthis)
            {

                return true;

            }

            public BoxDestructor(Block basedon, PointF Velocity)
                : this(basedon, Velocity, new PointF(1f, 1f))
            {

            }
            private static ImageAttributes CachedDefaultAttributes = null;
            /// <summary>
            /// returns the Default ImageAttributes that will be used if none are supplied in the Constructor.
            /// </summary>
            /// <returns></returns>
            protected static ImageAttributes GetDefaultDestructorAttributes()
            {
                if (CachedDefaultAttributes == null)
                {
                    CachedDefaultAttributes = new ImageAttributes();
                    CachedDefaultAttributes.SetColorMatrix(ColorMatrices.GetColourizer(1, 1, 1, 0.5f));
                }
                return CachedDefaultAttributes;

            }

            public BoxDestructor(Block basedon, PointF Velocity, PointF AccelerationFactor)
                : this(basedon, Velocity, AccelerationFactor, GetDefaultDestructorAttributes())
            {

            }

            public BoxDestructor(Block basedon, PointF Velocity, PointF AccelerationFactor, ImageAttributes AffectImageAttributes)
                : base(basedon.CenterPoint(), Velocity)
            {

                _VelocityDecay = AccelerationFactor;
                ObjectSize = basedon.BlockSize;
                ignorelist.Add(basedon);
                useDrawImage = basedon.DrawToImage();
                ImageAttributes alphaizer = new ImageAttributes();
                alphaizer.SetColorMatrix(ColorMatrices.GetColourizer(1, 1, 1, 0.5f));
                AlphadImage = BCBlockGameState.AppyImageAttributes(useDrawImage, alphaizer);
                OriginalBlock = basedon;
            }
            private bool TouchesBlock(Block testblock)
            {

                return ObjectRectangle.IntersectsWith(testblock.BlockRectangle);

            }
            private bool isBlockDestructable(Block testblock)
            {
                //
                return testblock != OriginalBlock && DestructableTest(testblock);
            }


            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                List<Block> removethem = new List<Block>();
                foreach (Block iterateblock in gamestate.Blocks)
                {
                    if (TouchesBlock(iterateblock) && iterateblock != OriginalBlock)
                    {
                        if (isBlockDestructable(iterateblock))
                        {
                            gamestate.Forcerefresh = true;
                            if (!ignorelist.Contains(iterateblock))
                            {
                                var currobjects = gamestate.GameObjects;
                                gamestate.GameObjects = new LinkedList<GameObject>();
                                BCBlockGameState.Block_Hit(gamestate, iterateblock,Velocity);
                                //AddObjects.AddRange(gamestate.GameObjects);
                                //gamestate.Defer(()=>gamestate.GameObjects.A
                                gamestate.GameObjects = currobjects;

                                removethem.Add(iterateblock);



                            }
                        }
                        else
                        {
                            //the block isn't destructable. We need to be destroyed.
                            gamestate.Defer(() => gamestate.GameObjects.Remove(this));
                            
                        }

                    }


                }


                foreach (Block removeit in removethem)
                {
                    gamestate.Blocks.Remove(removeit);

                }




                base.PerformFrame(gamestate);
                return !gamestate.GameArea.Contains(ObjectRectangle.Corners());
            }
            public override void Draw(Graphics g)
            {
                //throw new NotImplementedException();
                g.DrawImage(AlphadImage, ObjectRectangle.Left - Velocity.X, ObjectRectangle.Top - Velocity.Y);
                g.DrawImage(useDrawImage, ObjectRectangle.Left, ObjectRectangle.Top);

            }


        }
        /// <summary>
        /// This is another "proxy" type of object, which doesn't actually have a representation in the game
        /// but instead manages two other existing gameobjects to provide a specific behaviour.
        /// In this case, it manages two BoxDestructor Objects to provide the attraction/repulsion behaviour used
        /// by the attractrepulseBlock.
        /// </summary>
        public class AttractRepulseDestructor : GameObject
        {
            protected BoxDestructor[] Aggregates;
            protected AttractionRepulsionBlock[] OriginalBlocks;
            private const float defaultinitialspeed = 0.05f;
            public AttractRepulseDestructor(BCBlockGameState gstate, AttractionRepulsionBlock BlockA, AttractionRepulsionBlock BlockB)
            {
                //calculate the appropriate vectors.
                OriginalBlocks = new AttractionRepulsionBlock[] { BlockA, BlockB };
                PointF midpoint = BCBlockGameState.MidPoint(BlockA.CenterPoint(), BlockB.CenterPoint());
                double AtoB = BCBlockGameState.GetAngle(BlockA.CenterPoint(), midpoint);
                double BtoA = BCBlockGameState.GetAngle(BlockB.CenterPoint(), midpoint);
                double AuseAngle = BtoA; //default to "repel"
                double BuseAngle = AtoB;
                //if they are not the same colour...
                if (!(BlockA.BlockColor.R == BlockB.BlockColor.R &&
                    BlockA.BlockColor.G == BlockB.BlockColor.G &&
                    BlockA.BlockColor.B == BlockB.BlockColor.B))
                    BCBlockGameState.Swap(ref AuseAngle, ref BuseAngle);
                //swap, if the colour RGB triplets are unequal we want to "attract" rather than repel.

                //their initial speed will be zero.
                Aggregates = new BoxDestructor[] { new BoxDestructor(BlockA, PointF.Empty), new BoxDestructor(BlockB, PointF.Empty) };
                //but we will change their 'velocitydecay' 0.05 for now, in the appropriate direction.

                Aggregates[0].VelocityDecay = new PointF((float)Math.Cos(AuseAngle) * defaultinitialspeed,
                                                         (float)(Math.Sin(AuseAngle) * defaultinitialspeed));
                Aggregates[1].VelocityDecay = new PointF((float)(Math.Cos(BuseAngle) * defaultinitialspeed),
                     (float)(Math.Sin(BuseAngle) * defaultinitialspeed));


                Aggregates[0].VelocityIncrement = Aggregates[0].VelocityDecay;
                Aggregates[1].VelocityIncrement = Aggregates[1].VelocityDecay;
                Aggregates[0].VelocityDecay = new PointF(1 + Math.Abs(Aggregates[0].VelocityDecay.X), 1 + Math.Abs(Aggregates[0].VelocityDecay.Y));
                Aggregates[1].VelocityDecay = new PointF(1 + Math.Abs(Aggregates[1].VelocityDecay.X), 1 + Math.Abs(Aggregates[1].VelocityDecay.Y));


                //now we need to add the BoxDestructor to the gamestate.
                //we do not remove the Blocks, instead deferring that to the block itself to decide. 
                //it is also the caller's responsibility to add this instance to the GameObjects list as well.
                gstate.Defer(() =>
                {
                    gstate.GameObjects.AddLast(Aggregates[0]);
                    gstate.GameObjects.AddLast(Aggregates[1]);
                });




            }
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                //What happens here?
                //not a whole lot, actually.
                //all we really need to check for is whether the two aggregates are "touching". If they are, create a explosion in that spot and remove them.


                if (Aggregates[0].ObjectRectangle.IntersectsWith(Aggregates[1].ObjectRectangle))
                {
                    float explosionsize = (BCBlockGameState.Distance(PointF.Empty, Aggregates[0].Velocity) +
                                       BCBlockGameState.Distance(PointF.Empty, Aggregates[1].Velocity)) / 2;
                    //intersection.
                    //get the midpoint between the two centers...
                    PointF explosionzero = BCBlockGameState.MidPoint(Aggregates[0].ObjectRectangle.CenterPoint(),
                        Aggregates[1].ObjectRectangle.CenterPoint());
                    BCBlockGameState.Soundman.PlaySound("bomb");
                    ExplosionEffect explode = new ExplosionEffect(explosionzero, explosionsize);
                    //add it...
                    gamestate.Defer(()=> {
                        gamestate.GameObjects.AddLast(explode);
                        gamestate.GameObjects.Remove(Aggregates[0]);
                        gamestate.GameObjects.Remove(Aggregates[1]);
                        gamestate.GameObjects.Remove(this);
                    });

                    
                


                }


                return base.PerformFrame(gamestate);
            }
            public override void Draw(Graphics g)
            {
                //no code: we have no representation.
                //however, this could be used to draw "force lines" or something
                //between the two aggregate objects.
            }


        }


    public class BlueFireball : Fireball
        {
            //bluefireball is the same as the standard fireball, but will do a little tracking of the paddle.
            //private ImageAttributes useattributes= new ImageAttributes();
            private ColorMatrix cmuse;
            private QColorMatrix qcm = new QColorMatrix();
            public BlueFireball(BCBlockGameState currstate, PointF Location, SizeF ObjectSize, PointF initialSpeed)
                : base(currstate, Location, ObjectSize, initialSpeed)
            {
                Velocity = initialSpeed;


            }
            public BlueFireball(PointF pLocation, PointF pVelocity)
                : this(pLocation, pVelocity, new SizeF(8, 8))
            {

            }
            public BlueFireball(PointF pLocation, PointF pVelocity, SizeF pSize)
                : base(pLocation, pVelocity, pSize)
            {
                setattributes();
                
            }
            private void setattributes()
            {
                float[][] colorMatrixElements = { 
   new float[] {1,  0,  0,  0, 0},        // red scaling factor of 2
   new float[] {0,  1,  0,  0, 0},        // green scaling factor of 1
   new float[] {0,  0,  2,  0, 0},        // blue scaling factor of 1
   new float[] {0,  0,  0,  1, 0},        // alpha scaling factor of 1
   new float[] {-0.5f, -0.5f, .8f, 0, 1}};    // three translations of 0.2

                cmuse = new ColorMatrix(colorMatrixElements);
                qcm = new QColorMatrix(cmuse);
                DrawAttributes = new ImageAttributes();
                DrawAttributes.SetColorMatrix(
                   qcm.ToColorMatrix(),
                   ColorMatrixFlag.Default,
                   ColorAdjustType.Bitmap);

                DrawAttributes.SetColorMatrix(cmuse);


            }
            public BlueFireball(BCBlockGameState currstate, Block originBlock, float initialspeed)
                : this(currstate, originBlock.CenterPoint(), new SizeF(8, 8), initialspeed)
            {
                setattributes();


            }
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                /*   
                   qcm.RotateHue(0.1f);
                   DrawAttributes.SetColorMatrix(
                    qcm.ToColorMatrix(),
                    ColorMatrixFlag.Default,
                    ColorAdjustType.Bitmap);
                   cmuse = qcm.ToColorMatrix();
                   */


                //TODO: change velocity to go towards paddle, but only if there is a paddle.
                if (gamestate.PlayerPaddle != null)
                {
                    if (Location.Y < gamestate.PlayerPaddle.Position.Y) //only change if we are above the paddle.
                    {
                        //step one: get angle between fireball and the middle of the paddle:
                        double anglebetween = BCBlockGameState.GetAngle(CenterPoint(),
                                                                        gamestate.PlayerPaddle.Getrect().CenterPoint());

                        //also, get the angle of our speed vector.
                        double movementangle = BCBlockGameState.GetAngle(new PointF(0, 0), Velocity);
                        double totalspeed = BCBlockGameState.Distance(0, 0, Velocity.X, Velocity.Y);
                        //now, we want to move from movementangle towards anglebetween. 25% ought to work...


                        //double distancemove = ((anglebetween - movementangle)*0.05);
                        double anglevary = Math.Sign(anglebetween - movementangle) * (Math.PI / 90) * 0.25f;

                        double changedangle = movementangle + anglevary;

                        //now get the new velocity...

                        PointF newspeed = new PointF((float)(Math.Cos(changedangle) * totalspeed),
                                                     (float)(Math.Sin(changedangle) * totalspeed));
                        Velocity = newspeed;
                    }
                }

                return base.PerformFrame(gamestate);
            }
            public override void Draw(Graphics g)
            {
                base.Draw(g); //we DO need to call into it... (bugfix)
                //no longer needed since the change to the base class FireBall...
                /*
                var drawthis = CurrentFrameImage;
                g.DrawImage(drawthis, new Rectangle((int)Location.X, (int)Location.Y, (int)Size.Width, (int)Size.Height),
                   0, 0, drawthis.Width, drawthis.Height, GraphicsUnit.Pixel, useattributes);
                //g.DrawImage(CurrentFrameImage, new RectangleF(Location.X, Location.Y, Size.Width, Size.Height),GraphicsUnit.Pixel, useattributes);
                //g.DrawRectangle(new Pen(Color.Black, 1), new Rectangle((int)Location.X, (int)Location.Y, (int)Size.Width, (int)Size.Height));
                */


            }





            public BlueFireball(BCBlockGameState currstate, PointF Location, SizeF ObjectSize, float initialsize)
                : base(currstate, Location, ObjectSize, initialsize)
            {



            }

        }
        public class ExplosionShot : GameObject
        {
            protected PointF _Velocity, _Location;
            public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }
            public PointF Location { get { return _Location; } set { _Location = value; } }

            int framecounter = 0;
            int NextDeployment = 12;
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                
                BCBlockGameState.IncrementLocation(gamestate, ref _Location, Velocity);
                framecounter++;
                if (framecounter >= NextDeployment)
                {
                    framecounter = 0;
                    NextDeployment = BCBlockGameState.rgen.Next(10, 30);

                    ExplosionEffect ee = new ExplosionEffect(Location, 10 + (float)(BCBlockGameState.rgen.NextDouble() * 20));
                    ee.DamageBlocks = false; //doesn't hurt blocks...
                    ee.ShowOrbs = false;

                    gamestate.Defer(() =>
                    {
                        gamestate.GameObjects.AddLast(ee);
                        BCBlockGameState.Soundman.PlaySound("explode");
                    });


                }
                return !gamestate.GameArea.Contains(new Point((int)Location.X, (int)Location.Y));


            }
            public override void Draw(Graphics g)
            {
                //throw new NotImplementedException();
                g.FillEllipse(new SolidBrush(Color.Red), new RectangleF(Location.X - 2, Location.Y - 2, 4, 4));
            }

            public ExplosionShot(BCBlockGameState currstate, PointF pLocation, float initialspeed)
            {
                Location = pLocation;
                Velocity = new PointF();
                PointF targetposition;
                //if the paddle exists...
                if (currstate.PlayerPaddle != null)
                    targetposition = currstate.PlayerPaddle.Position;
                else
                {
                    //otherwise, choose a random spot.
                    Random genner = BCBlockGameState.rgen;
                    targetposition = new PointF((float)(currstate.GameArea.Left + (currstate.GameArea.Width * genner.NextDouble())),
                        (float)(currstate.GameArea.Top + (currstate.GameArea.Height * genner.NextDouble())));


                }
                PointF originPoint = Location;
                double angleuse = Angle((double)Location.X, (double)Location.Y, (double)targetposition.X, (double)targetposition.Y);
                float usespeed = initialspeed;
                double degree = (Math.PI * 2) / 360;
                angleuse += (degree * 2) - degree;

                Velocity = new PointF((float)Math.Cos(angleuse) * usespeed,
                    (float)Math.Sin(angleuse) * usespeed);


            }



        }

        public class Fireball : AnimatedImageObject, iSizedProjectile
        {
            public new PointF Velocity { get { return base.Velocity; } set { base.Velocity = value; } }
            public float FireballDamage = 2f;
            public double DrawAngle = 0;
            public const double DrawAngleIncrement = 10;
            protected ImageAttributes DrawAttributes = null;


            public Fireball(BCBlockGameState currstate, Block originBlock, float initialspeed)
                : this(currstate, originBlock.CenterPoint(), new SizeF(8, 8), initialspeed)
            {
                //Velocity = new PointF();
            }



            public Fireball(BCBlockGameState currstate, PointF Location, SizeF ObjectSize, PointF initialSpeed)
                : base(Location, ObjectSize, BCBlockGameState.Imageman.getImageFrames("fireball"))
            {
                Velocity = initialSpeed;


            }
            public Fireball(PointF pLocation, PointF pVelocity, SizeF pSize):base(pLocation,pSize,BCBlockGameState.Imageman.getImageFrames("fireball"))

            {
                this.Velocity = pVelocity;

            }
            public Fireball(PointF pLocation, PointF pVelocity)
                : this(pLocation, pVelocity, new SizeF(8, 8))
            {
            }
            public Fireball(BCBlockGameState currstate, PointF Location, SizeF ObjectSize, float initialspeed)
                : base(Location, ObjectSize, BCBlockGameState.Imageman.getImageFrames("fireball"))
            {
                Velocity = currstate.AimAtPaddle( Location, initialspeed);
            }

           

            private Rectangle getRect()
            {
                return new Rectangle(new Point((int)Location.X, (int)Location.Y), new Size((int)Size.Width, (int)Size.Height));


            }
            public override void Draw(Graphics g)
            {
                SizeF DrawSize = Size;
                PointF DrawLocation = Location;
                float DrawRotation = (float)DrawAngle;
                //Debug.Print("DrawAngle=" + DrawRotation);
                Image currimage = BCBlockGameState.Imageman.getLoadedImage("fireball1");
                Matrix copyt = g.Transform;
                g.TranslateTransform(DrawSize.Width / 2 + DrawLocation.X, DrawSize.Height / 2 + DrawLocation.Y);
                g.RotateTransform(DrawRotation);
                g.TranslateTransform(-DrawSize.Width / 2, -DrawSize.Height / 2);
                DrawLocation = new PointF(0, 0);



                if (DrawSize != null)
                {
                    if (DrawAttributes == null)
                    {
                        //g.DrawImage(GetCurrentImage(), Location.X, Location.Y, DrawSize.Value.Width, DrawSize.Value.Height);
                        g.DrawImage(currimage, DrawLocation.X, DrawLocation.Y, DrawSize.Width, DrawSize.Height);
                    }
                    else
                    {

                        //g.DrawImage(BlockImage, new Rectangle((int)BlockRectangle.Left, (int)BlockRectangle.Top, (int)BlockRectangle.Width, (int)BlockRectangle.Height), 0f, 0f, BlockImage.Width, BlockImage.Height, GraphicsUnit.Pixel, DrawAttributes);
                        g.DrawImage(currimage, new Rectangle((int)DrawLocation.X, (int)DrawLocation.Y, (int)DrawSize.Width, (int)DrawSize.Height), 0f, 0f, currimage.Width, currimage.Height, GraphicsUnit.Pixel, DrawAttributes);
                        //g.DrawImage(GetCurrentImage(), Location.X, DrawSize.Value.Width, DrawSize.Value.Height, DrawAttributes);
                    }
                }
                else
                {

                    if (DrawAttributes == null)
                        g.DrawImage(currimage, DrawLocation);
                    else
                        g.DrawImage(currimage, new Rectangle((int)DrawLocation.X, (int)DrawLocation.Y, (int)currimage.Width, (int)currimage.Height), 0f, 0f, currimage.Width, currimage.Height, GraphicsUnit.Pixel, DrawAttributes);

                }
                g.Transform = copyt;




            }
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                DrawAngle += DrawAngleIncrement;

                
                BCBlockGameState.IncrementLocation(gamestate, ref _Location, Velocity);
                if (!gamestate.GameArea.Contains(new Rectangle(new Point((int)Location.X, (int)Location.Y), new Size((int)Size.Width, (int)Size.Height))))
                {
                    gamestate.Defer(() => gamestate.GameObjects.Remove(this));
                    
                    return false;

                }


                //check to see if we hit the paddle:
                if (gamestate.PlayerPaddle != null)  //check for null first...
                {
                    if (gamestate.PlayerPaddle.Getrect().IntersectsWith(this.getRect()))
                    {
                        //DIE paddle! do some damage to the paddle. Let's say- 20.
                        gamestate.PlayerPaddle.HP -= FireballDamage;
                        //return false to delete this fireball. 
                        //TODO: add particle effects for the explosion or whatever.
                        for (int i = 0; i < 15; i++)
                        {
                            gamestate.Particles.Add(new FireParticle(Location));


                        }
                        gamestate.Defer(() => gamestate.GameObjects.Remove(this));
                        
                        return false;
                    }



                }

                return base.PerformFrame(gamestate);
            }



        }


        /// <summary>
        /// A Proxy GameObject can be used for those facilities that are... lacking.
        /// For example, there is no way within a PaddleBehaviour to remove itself, since all calls to the behaviour are done within enumerations, and other concerns.
        /// The proxy object can be used to create a "proxy" game object and redirect the overridden methods to given routines.
        /// </summary>

        public class ProxyObject : GameObject
        {
            public delegate bool ProxyPerformFrame(ProxyObject sourceobject, BCBlockGameState gamestate);
            public delegate void ProxyDraw(Graphics g);

            private object _Tag = null;
            public object Tag { get { return _Tag; } set { _Tag = value; } }
            protected ProxyPerformFrame funcperformframe;
            protected ProxyDraw funcdraw;

            public ProxyObject(ProxyPerformFrame performframefunc, ProxyDraw drawfunc)
            {
                funcperformframe = performframefunc;
                funcdraw = drawfunc;


            }

            
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                if (funcperformframe != null)
                    return funcperformframe(this, gamestate);

                return false;
            }

            public override void Draw(Graphics g)
            {
                if (funcdraw != null)
                    funcdraw(g);
            }
        }

        #region Powerups




        public interface IAnimatedSpriteAction
        {
            bool PerformAnimatedSpriteFrame(AnimatedSprite sprite, BCBlockGameState gamestate);
            void Draw(AnimatedSprite sprite, Graphics g);



        }



        #region Velocity change classes
        public abstract class VelocityChanger
        {
            public delegate PointF VelocityChangerFunction(PointF Input);
            public abstract PointF PerformFrame(BCBlockGameState gstate,PointF CurrentLocation);

            public abstract PointF getVelocity();

        }

        public class VelocityChangerLinear : VelocityChanger
        {
            protected PointF _Delta = new PointF(0, 0);
            public PointF Delta
            {
                get { return _Delta; }
                set { _Delta = value; }
            }
            public VelocityChangerLinear(PointF pDelta)
            {

                _Delta = pDelta;

            }
            public VelocityChangerLinear()
                : this(new PointF(0, 2))
            {

            }
            public override PointF getVelocity()
            {
                return _Delta;
            }
            public override PointF PerformFrame(BCBlockGameState gstate, PointF CurrentLocation)
            {
                //return new PointF(CurrentLocation.X + _Delta.X, CurrentLocation.Y + _Delta.Y);
                BCBlockGameState.IncrementLocation(gstate, ref CurrentLocation, _Delta);
                return CurrentLocation;
            }


        }
        /// <summary>
        /// Class used to present "Exponential" changes to Velocity. This in most cases just means subject to gravity, really.
        /// </summary>
        public class VelocityChangerExponential : VelocityChangerLinear
        {
            private PointF _Acceleration = new PointF(1, 1.01f);

            public PointF Acceleration { get { return _Acceleration; } set { _Acceleration = value; } }

            public VelocityChangerExponential(PointF pDelta, PointF pAcceleration)
                : base(pDelta)
            {
                _Acceleration = pAcceleration;


            }
            public override PointF PerformFrame(BCBlockGameState gstate, PointF CurrentLocation)
            {
                _Delta = new PointF(_Delta.X * _Acceleration.X, _Delta.Y * _Acceleration.Y);
                return base.PerformFrame(gstate,CurrentLocation);
            }


        }
        public class VelocityChangerParametric : VelocityChangerLinear
        {


            public delegate float ParametricFunction(PointF Currposition);


            private ParametricFunction _ParametricX = null;
            private ParametricFunction _ParametricY = null;


            public ParametricFunction ParametricX
            {
                get { return _ParametricX; }
                set { _ParametricX = value; }
            }
            public ParametricFunction ParametricY
            {
                get { return _ParametricY; }
                set { _ParametricY = value; }

            }

            public VelocityChangerParametric(ParametricFunction xFunction, ParametricFunction yFunction)
            {

                _ParametricX = xFunction;
                _ParametricY = yFunction;


            }

            public override PointF PerformFrame(BCBlockGameState gstate, PointF CurrentLocation)
            {
                //throw new NotImplementedException();
                float XValue = 0, YValue = 0;

                if (_ParametricX != null) XValue = _ParametricX(CurrentLocation);
                if (_ParametricY != null) YValue = _ParametricY(CurrentLocation);
                Debug.Print("parametric: X=" + XValue + " Y=" + YValue);
                _Delta = new PointF(XValue, YValue);
                return base.PerformFrame(gstate,CurrentLocation);
                //return new PointF(CurrentLocation.X + XValue,CurrentLocation.Y+YValue);

            }




        }

        public class VelocityChangerLeafy : VelocityChangerParametric
        {
            //this bugger is a bit more complex.

            //Basically, we want our X speed to be a Sin function that goes back and forth an equal amount.
            //the Y speed needs to be similar, but needs to be centered higher.
            public VelocityChangerLeafy()
                : base(null, null)
            {
                ParametricX = LeafyX;
                ParametricY = LeafyY;


            }
            int XFrames = 0;
            public float LeafyX(PointF CurrValue)
            {
                float returnthis = (float)Math.Sin(((double)XFrames * 5)) * 8;
                XFrames++;
                return returnthis;


            }
            public float LeafyY(PointF CurrValue)
            {
                float returnthis = (float)((Math.Sin((double)XFrames * 200) / 2) * 2) + 1.5f;
                return returnthis;
            }
            //performFrame is handled by base class...

        }

        #endregion



        /// <summary>
        /// provides a GameImageObject that animates a given image.
        /// </summary>

        /// 
        public class AnimatedSprite : AnimatedImageObject, IAnimatedSpriteAction
        {

            public Image[] AnimationFrames; //the animation frames for this image.
            public decimal CurrentRotation = 0;
            public decimal RotationSpeed = 5;
            public float DrawAlpha = 1f;

            public delegate ImageAttributes NextAttributesFunction(ImageAttributes currentattributes);
            public NextAttributesFunction NextAttributesFunc;
            public IAnimatedSpriteAction SpriteAction;


            private float curralpha = 1.0f;
            private ImageAttributes NextAttribAlpha(ImageAttributes currentattributes)
            {
                curralpha -= 0.05f;
                if (curralpha < 0) curralpha = 0;




                if (useImageattributes == null) useImageattributes = new ImageAttributes();
                float[][] ptsArray =
            {
            new float[] {1, 0, 0, 0, 0},
            new float[] {0, curralpha, 0, 0, 0},
            new float[] {0, 0, 1, 0, 0},
            new float[] {0, 0, 0, curralpha, 0},
            new float[] {0, 0, 0, 0, 1}
        };
                ColorMatrix makeMatrix = new ColorMatrix(ptsArray);

                useImageattributes.SetColorMatrix(makeMatrix, ColorMatrixFlag.Default, ColorAdjustType.Default);


                return useImageattributes;

            }
            public AnimatedSprite(PointF Location, Image ImageUse, decimal initialRotation, decimal pRotationSpeed, int pFrameadvancedelay, NextAttributesFunction pAttribFunc)
                : this(Location, ImageUse, initialRotation, pRotationSpeed, pFrameadvancedelay)
            {
                NextAttributesFunc = pAttribFunc;



            }

            public AnimatedSprite(PointF Location, Image ImageUse, decimal initialRotation, decimal pRotationSpeed, int pframeadvancedelay)
                : this(Location, ImageUse.Size, ImageUse, initialRotation, pRotationSpeed, pframeadvancedelay)
            {


            }
            public AnimatedSprite(PointF Location, SizeF ObjectSize, Image ImageUse, decimal initialRotation, decimal pRotationSpeed, int pframeadvancedelay)
                : this(Location, ObjectSize, new Image[] { ImageUse }, initialRotation, pRotationSpeed, pframeadvancedelay)
            {


            }

            public AnimatedSprite(PointF Location, Image[] ImageFrames, decimal initialRotation, decimal pRotationSpeed, int pframeadvancedelay)
                : this(Location, ImageFrames[0].Size, ImageFrames, initialRotation, pRotationSpeed, pframeadvancedelay)
            {



            }
            public AnimatedSprite(PointF Location, SizeF ObjectSize, Image[] ImageFrames, decimal initialRotation, decimal pRotationSpeed, int pframeadvancedelay)
                : base(Location, ObjectSize, ImageFrames, pframeadvancedelay)
            {
                Debug.Print("AnimatedSprite Constructor...");
                //if (ImageFrames == null) throw new ArgumentNullException("ImageFrames Argument cannot be null");
                AnimationFrames = ImageFrames;
                SpriteAction = this;
                CurrentRotation = initialRotation;
                RotationSpeed = pRotationSpeed;
                NextAttributesFunc = NextAttribAlpha;

            }
            public AnimatedSprite(PointF Location, SizeF ObjectSize, String[] ImageFrameKeys, decimal initialRotation, decimal pRotationSpeed, int pframeadvancedelay)
                : this(Location, ObjectSize, BCBlockGameState.Imageman.getImageFrames(ImageFrameKeys), initialRotation, pRotationSpeed, pframeadvancedelay)
            {


            }

            public override void Draw(Graphics g)
            {
                //
                // base.Draw(g);
                Draw(this, g);
            }

            protected ImageAttributes useImageattributes;



            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                bool returnvalue = base.PerformFrame(gamestate);
                if (NextAttributesFunc != null)
                {
                    useImageattributes = NextAttributesFunc(useImageattributes);
                }
                if (AnimationFrames.Length == 1)
                    return returnvalue;
                else
                {
                    return SpriteAction.PerformAnimatedSpriteFrame(this, gamestate);
                }
            }




            #region IAnimatedSpriteAction Members
            //class variable, used to store our transformation matrix. 
            Matrix rotationmatrix = new Matrix();
            public void Draw(AnimatedSprite sprite, Graphics g)
            {
                //Draw: takes into account the rotation and the current frame.
                //First,  get the Image we need to draw:
                Image drawthis = CurrentFrameImage;
                if (drawthis == null) return;
                PointF centerpoint = CenterPoint();
                //now,proceed to change the transform matrix of the graphics object...
                //Debug.Print(CurrentRotation.ToString());
                //rotate around the center of the object.
                //create a new Matrix, also, a temporary one to cache the graphics objects original matrix.
                Matrix cached = g.Transform;





                rotationmatrix.Reset();
                //rotationmatrix.RotateAt((float)CurrentRotation, centerpoint);
                rotationmatrix.RotateAt((float)CurrentRotation, centerpoint);


                //draw it...
                g.Transform = rotationmatrix;


                //g.DrawImage(drawthis, Location.X, Location.Y, Size.Width, Size.Height, useImageattributes);
                g.DrawImage(drawthis, new Rectangle((int)Location.X, (int)Location.Y, (int)Size.Width, (int)Size.Height),
                    0, 0, drawthis.Width, drawthis.Height, GraphicsUnit.Pixel, useImageattributes);
                //now "revert" the transform...


                //g.Flush();
                g.Transform = cached;

                //g.Transform.RotateAt(-(float)CurrentRotation, centerpoint);

            }

            public bool PerformAnimatedSpriteFrame(AnimatedSprite sprite, BCBlockGameState gamestate)
            {
                //task: increment rotation and frame number.
                CurrentRotation += RotationSpeed;
                bool returnresult = !gamestate.GameArea.Contains(new Point((int)Location.X, (int)Location.Y));
                //Location= new PointF(Location.X+Velocity.X,Location.Y+Velocity.Y);
                Location = VelocityChange.PerformFrame(gamestate,Location);

                return returnresult;
            }

            #endregion
        }

    //used by special bonuses and whatnot.
    //example: a bonus level will have a CountdownObject. getting macguffins or other items will add time to that countdown.
    public class CountDownObject : SizeableGameObject 
    {
        private TimeSpan? TimerStart = null;
        private BCBlockGameState gs = null;
        public event EventHandler<EventArgs> CountDownExpired;
        private Font _DrawFont = BCBlockGameState.GetScaledFont(new Font(BCBlockGameState.GetMonospaceFont(), 18), 48);
        private Brush _DrawBrush = new SolidBrush(Color.Green);
        private Pen _DrawPen = new Pen(Color.Black);
        private TimeSpan TimeoutValue = new TimeSpan(0, 0, 0, 5);
        public Pen DrawPen { get { return _DrawPen; } set { _DrawPen = value; } }
        public Brush DrawBrush { get { return _DrawBrush; } set { _DrawBrush = value; } }
        public Font DrawFont { get { return _DrawFont; } set { _DrawFont = value; } }


        public CountDownObject(PointF pLocation, SizeF objectsize) : base(pLocation, objectsize)
        {
            CountDownExpired += CountDownObject_CountDownExpired;
        }

        void CountDownObject_CountDownExpired(object sender, EventArgs e)
        {
            gs.Blocks.Clear(); //with no blocks, the level will complete.
        }
        public static void AddTime(BCBlockGameState gstate,TimeSpan addamount)
        {

            foreach (CountDownObject cdo in (from go in gstate.GameObjects where go is CountDownObject select go))
            {
                cdo.AddTime(addamount);
            }


        }
        public void AddTime(TimeSpan addamount)
        {
            TimeoutValue += addamount;


        }
        public override void Draw(Graphics g)
        {
            //location is center spot.
            //measure size of the string.
            TimeSpan drawelapsed = getElapsed();
            //format: 00.00
            String usedrawstring = String.Format("{0:0.0}", drawelapsed.TotalSeconds);
            //we have the graphics context and the Font to use (DrawFont). we can use MeasureString() to measure the size of the text.
            var calcsize = g.MeasureString(usedrawstring, _DrawFont);
            //we want to draw it centered on the Location, so we need to offset by half the size.

            var drawlocation = new PointF(Location.X - calcsize.Width / 2, Location.Y - calcsize.Height / 2);
            //use a path.
            using (GraphicsPath gp = new GraphicsPath())
            {
                gp.AddString(usedrawstring, _DrawFont, Point.Empty, StringFormat.GenericDefault);

                g.FillPath(_DrawBrush, gp);
                g.DrawPath(_DrawPen, gp);
            }

            
        }
        

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            
            //we use gamestate.ClientObject.GetLevelTime for the countdown.
            //gamestate.ClientObject.GetLevelTime
            if (TimerStart == null)
            {
                //uninitialized, so set the timer.
                TimerStart = gamestate.ClientObject.GetLevelTime();
                gs = gamestate;
            }


            return base.PerformFrame(gamestate);
        }
        private TimeSpan getElapsed()
        {
            //retrieves the elapsed game time for this timer.
            return gs.ClientObject.GetLevelTime() - TimerStart.Value;

        }
    }

        public class PolygonProtector : SizeableGameObject
        {
            //a protective bumper that sits beneath the paddle.

            private Image[] ProtectionImages;
            private int ourHP = 10;

            private Image RandomPolyImage(Size drawsize)
            {
                Bitmap buildimage = new Bitmap(drawsize.Width, drawsize.Height);

                using (Graphics buildg = Graphics.FromImage(buildimage))
                {
                    buildg.Clear(Color.Transparent);
                    for (int i = 0; i < 90; i++)
                    {
                        //generate 500 random polygons.
                        //we'll cheat and use a PolyDebris.
                        PointF Randomcenter = new PointF((float)(BCBlockGameState.rgen.NextDouble() * drawsize.Width),
                            (float)(BCBlockGameState.rgen.NextDouble() * drawsize.Height));


                        float randomradius = (float)BCBlockGameState.rgen.NextDouble(3, 15);
                        int useA = BCBlockGameState.rgen.Next(64, 255);
                        int useR = BCBlockGameState.rgen.Next(0, 255);
                        int useG = BCBlockGameState.rgen.Next(0, 255);
                        int useB = BCBlockGameState.rgen.Next(0, 255);

                        Color randomcolor = Color.FromArgb(useA, useR, useG, useB);

                        PolyDebris drawdebris = new PolyDebris(Randomcenter, 0, randomcolor, randomradius, randomradius, 3, 9);
                        drawdebris.Draw(buildg);



                    }


                }

                return buildimage;


            }

            private void GenerateImages()
            {
                //let's go with 10 different frames.
                ProtectionImages = new Image[10];
                for (int i = 0; i < ProtectionImages.Length; i++)
                {

                    ProtectionImages[i] = RandomPolyImage(new Size(413, 32));



                }

            }

            public PolygonProtector()
                : base(new PointF(0, 427 - 32), new SizeF(413, 427))
            {

                GenerateImages();

            }


            public override void Draw(Graphics g)
            {
                Image drawthis = BCBlockGameState.Choose(ProtectionImages);
                g.DrawImageUnscaled(drawthis, new Point(0, 427 - drawthis.Height));
            }


        }

        /*
        public class SuperleafPowerUp : GamePowerUp
        {
            private Image[] LeafImages = BCBlockGameState.Imageman.getImageFrames(new String[] { "leaf", "FLIPX:leaf" });
            public bool PowerupCallback(BCBlockGameState gamestate, ref List<GameObject> addem, ref List<GameObject> removeem)
            {

                return true;
            }

            public SuperleafPowerUp(PointF Location, SizeF ObjectSize)
                : base(Location, ObjectSize, (Image[])null, -1,null)
            {
                ObjectImages = LeafImages;
                //VelocityChange = new VelocityChangerLeafy();
                usefunction = PowerupCallback;
            }
            public override bool PerformFrame(BCBlockGameState gamestate, ref List<GameObject> AddObjects, ref List<GameObject> removeobjects)
            {
                //return base.PerformFrame(gamestate, ref AddObjects, ref removeobjects);
                ObjectImages = LeafImages;
                if (Math.Sign(VelocityChange.getVelocity().X) == -1)
                {
                    //negative, moving to the right, so we need the flipped version.
                    currimageframe = 1;
                


                }
                else
                {
                    currimageframe = 0;
                
                }


                return base.PerformFrame(gamestate, ref AddObjects, ref removeobjects);
            }

        }
        */

        #endregion

        public class LineSegment : GameObject
        {

            PointF[] Points = new PointF[2];
            Pen _UsePen = new Pen(Color.Black);
            public Pen UsePen { get { return _UsePen; } set { _UsePen = value; } }
            public LineSegment(PointF PointA, PointF PointB)
            {

                Points[0] = PointA;
                Points[1] = PointB;


            }
            public PointF Magnitude()
            {
                return new PointF(Points[1].X - Points[0].X, Points[1].Y - Points[0].Y);

            }


            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                //throw new NotImplementedException();
                return true;
            }

            public override void Draw(Graphics g)
            {
                //throw new NotImplementedException();
                g.DrawLine(UsePen, Points[0], Points[1]);



            }
        }


        /// <summary>
        /// Class derived from to create timed radius-based effects; example, explosions.
        /// </summary>
        public abstract class BaseRadiusEffectBlock : SizeableGameObject
        {
            //basic concept is simple. 
            protected Brush _CurrentBrush = new SolidBrush(Color.Red);
            protected float _CurrentRadius = 0;
            protected float _MaxRadius = 0;
            protected Color _useColor = Color.Red;
            protected PointF Velocity, VelocityDecay = new PointF(0.99f, 0.99f);
            protected Func<float, float, float> _IncrementRoutine = null;
            protected Func<float, float> _AlphaRoutine = null;

            public Brush CurrentBrush { get { return _CurrentBrush; } set { _CurrentBrush = value; } }
            public float CurrentRadius
            {
                get { return _CurrentRadius; }
                set
                {
                    _CurrentRadius = value;

                }
            }
            public Func<float, float, float> IncrementRoutine { get { return _IncrementRoutine; } set { _IncrementRoutine = value; } }
            public Func<float, float> AlphaRoutine { get { return _AlphaRoutine; } set { _AlphaRoutine = value; } }





            protected static float DefaultIncrementRoutine(float x, float y)
            {

                return x + 5;

            }

            protected BaseRadiusEffectBlock(PointF Location, float InitialSize, float pMaxSize, Func<float, float, float> Incrementfunction)
                : base(Location, new SizeF(InitialSize, InitialSize))
            {
                _MaxRadius = pMaxSize;
                _CurrentRadius = InitialSize;
                IncrementRoutine = Incrementfunction;


            }

            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                
                BCBlockGameState.IncrementLocation(gamestate, ref _Location, Velocity);
                Velocity = new PointF(Velocity.X * VelocityDecay.X, Velocity.Y * VelocityDecay.Y);
                _CurrentRadius = IncrementRoutine(_CurrentRadius, _MaxRadius);
                float usealpha = (1 - (_CurrentRadius / _MaxRadius)) * 255;
                usealpha = BCBlockGameState.ClampValue(usealpha, 0, 255);
                CurrentBrush = new SolidBrush(Color.FromArgb((int)usealpha, _useColor));
                return _CurrentRadius >= _MaxRadius || (usealpha <= 5);
            }
            public override void Draw(Graphics g)
            {
                g.FillEllipse(CurrentBrush, Location.X - _CurrentRadius, Location.Y - _CurrentRadius, _CurrentRadius * 2, _CurrentRadius * 2);
            }

            
        }

        public class BlockChangeEffect : BaseRadiusEffectBlock
        {
            protected Type BlockFindType = typeof(InvincibleBlock);
            protected Type BlockChangeType = typeof(NormalBlock);
            public BlockChangeEffect(PointF Location, float InitialRadius, float MaxRadius,Type SearchForType, Type ChangeBlockTo)
                : base(Location, InitialRadius, MaxRadius, DefaultIncrementRoutine)
            {
                if (!ChangeBlockTo.IsSubclassOf(typeof(Block)))
                    throw new ArgumentException("ChangeBlockTo Parameter must be a subclass of Block");

                BlockFindType = SearchForType;

                BlockChangeType = ChangeBlockTo;
                _useColor = Color.Green;
            }

            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                var changeme = from p in gamestate.Blocks
                               where
                                   BCBlockGameState.Distance(Location.X, Location.Y, p.CenterPoint().X,
                                                             p.CenterPoint().Y) < _CurrentRadius && (BlockFindType==null || p.GetType()==BlockFindType)
                               select p;

                foreach (var loopitem in changeme)
                {
                    if (!(loopitem.GetType().Equals(BlockChangeType)))
                    {
                        //if not equal, create a new block with the same rect as loopitem
                        Block createdblock = (Block)Activator.CreateInstance(BlockChangeType, loopitem.BlockRectangle);
                        //remove the old one, and replace it with this one.
                        var nodeitem = gamestate.Blocks.Find(loopitem);
                        //add the new one...
                        var copied = loopitem;
                        //don't change the blocks right away, plonk it to a delegate.
                        //it might also be possible to plonk this entire for loop into a delayed framestartup call, too.
                        gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                        {
                            //add a "changed" block...
                            gamestate.Blocks.AddAfter(nodeitem,createdblock);
                            //remove the old one.
                            gamestate.Blocks.Remove(copied);
                        }));

                    }


                }
                gamestate.Forcerefresh = true;

                return base.PerformFrame(gamestate);
            }
        }
      
        public class CreationEffect : BaseRadiusEffectBlock
        {

            public delegate Block BlockCreationRoutine(BCBlockGameState gstate, RectangleF CreateSpot);

            public BlockCreationRoutine BlockCreateRoutine = null;
            private SizeF BlockCreateSize;

            public static Block DefaultCreationFunction(BCBlockGameState gstate, RectangleF CreateSpot)
            {
                return new NormalBlock(CreateSpot);


            }

            public CreationEffect(SizeF BlockSize, PointF Location, float maxRadius)
                : base(Location, 2, maxRadius, DefaultIncrementRoutine)
            {
                BlockCreateSize = BlockSize;
                BlockCreateRoutine = DefaultCreationFunction;
                _useColor = Color.Blue;

            }
            private List<RectangleF> createblocklocations = new List<RectangleF>();
            private bool flInit = false;
            private bool _ShowOrbs = true;
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                bool rval = base.PerformFrame(gamestate);


                for (int i = 0; i < (int)(50f * BCBlockGameState.ParticleGenerationFactor); i++)
                {
                    const float speedmult = 1;
                    //choose a random Angle...
                    double randomangle = Math.PI * 2 * BCBlockGameState.rgen.NextDouble();
                    //create the appropriate speed vector, based on our radius...
                    double usespeed = (_CurrentRadius / _MaxRadius) * speedmult; //should be proportional to how close we are to the maximum radius; max radius will have particles move 1...
                    usespeed += ((BCBlockGameState.rgen.NextDouble() * 0.5) - 0.25);

                    PointF addpointLocation = new PointF(Location.X + (float)Math.Cos(randomangle) * _CurrentRadius, Location.Y + (float)Math.Sin(randomangle) * _CurrentRadius);


                    //create a dustparticle...
                    Particle addparticle = null;
                    if ((i % 5 == 0) && _ShowOrbs)
                    {
                        addparticle = new LightOrb(addpointLocation, Color.Blue, 5 + (float)(BCBlockGameState.rgen.NextDouble() * 10));
                    }
                    else
                    {
                        addparticle = new WaterParticle(addpointLocation,
                                                                      new PointF((float)(Math.Cos(randomangle) * usespeed),
                                                                                 (float)(Math.Sin(randomangle) * usespeed)));
                    }



                    if (addparticle != null)
                    {
                        addparticle.Velocity = new PointF(addparticle.Velocity.X + Velocity.X, addparticle.Velocity.Y + Velocity.Y);
                        gamestate.Particles.Add(addparticle);
                    }



                }



                //is this the first call?
                if (!flInit)
                {
                    flInit = true;
                    //initialize data structures.
                    //the idea here is that createblocklocations will store the offset from the CreationEffect's location, rather than an absolute position.
                    for (float x = -_MaxRadius; x < (_MaxRadius); x += BlockCreateSize.Width)
                    {
                        for (float y = -_MaxRadius; y < (_MaxRadius); y += BlockCreateSize.Height)
                        {
                            //add a new rectangle to that cache
                            RectangleF createrect = new RectangleF(x, y, BlockCreateSize.Width, BlockCreateSize.Height);
                            //add it to the list.
                            createblocklocations.Add(createrect);

                        }


                    }

                }
                //createblocklocations has the offsets from our position. LINQ-ee-fy it to see if any need to be created.
                var createthese = from q in createblocklocations where BCBlockGameState.Distance(0, 0, q.CenterPoint().X, q.CenterPoint().Y) < _CurrentRadius select q;
                List<RectangleF> removethese = new List<RectangleF>();
                if (createthese.Any())
                {
                    //we need to create some blocks we do.
                    foreach (RectangleF looprect in createthese)
                    {

                        //create the PROPER rectanglef structure by cloning this one and offseting the clone.
                        RectangleF userect = looprect;
                        userect.Offset(Location.X, Location.Y);
                        Block createdblock = DefaultCreationFunction(gamestate, userect);
                        //add this block to the game.
                        gamestate.Blocks.AddLast(createdblock);
                        removethese.Add(looprect);

                    }

                    foreach (var removeit in removethese)
                    {
                        createblocklocations.Remove(removeit);

                    }

                    gamestate.Forcerefresh = true;


                }




                return rval;
            }
        }

        public class ExplosionEffect : BaseRadiusEffectBlock
        {
            protected Color _ExplosionColor = Color.Red;
            private int _ComboCount = 0;
            private bool _DamageBlocks = true;
            private bool _DamagePaddle = true;
            private bool _EffectObjects = true;
            private bool _ShowOrbs = true;
            private bool _DestroyAll = false;
            public Color ExplosionColor { get { return _ExplosionColor; } set { _ExplosionColor = value; } }
            public bool ShowOrbs { get { return _ShowOrbs; } set { _ShowOrbs = value; } }
            public bool DamageBlocks { get { return _DamageBlocks; } set { _DamageBlocks = value; } }
            public bool DamagePaddle { get { return _DamagePaddle; } set { _DamagePaddle = value; } }
            /// <summary>
            /// determines whether this explosion will find and invoke game objects with implement "IExplodable" when it hits them.
            /// </summary>
            public bool EffectObjects { get { return _EffectObjects; } set { _EffectObjects = true; } }
            public int ComboCount { get { return _ComboCount; } set { _ComboCount = value; } }
            public bool DestroyAll { get { return _DestroyAll; } set { _DestroyAll = value; } }


            //ComboNumber: this is set by the creator when needed.
            //the 

            public ExplosionEffect(PointF Location)
                : this(Location, 128)
            {


            }
            public ExplosionEffect(PointF Location, float MaxRadius)
                : base(Location, 2, MaxRadius, (w, x) => w + ((x - w) / 5))
            {



            }

            private Dictionary<cBall, int> proxylife = new Dictionary<cBall, int>();

            public List<Block> proxyperformframe(cBall ballobject, BCBlockGameState ParentGameState, ref List<cBall> ballsadded, ref List<cBall> ballsremove, out bool removethis)
            {
                removethis = false;
                if (proxylife.ContainsKey(ballobject))
                {
                    proxylife[ballobject]++;


                }
                else
                {
                    proxylife.Add(ballobject, 0);
                }
                if (proxylife[ballobject] > 0)
                {
                    Debug.Print("force removing a ball");
                    removethis = true;

                }
                return null;
            }

            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                bool retval = base.PerformFrame(gamestate);


                //add some particles around the edges of the explosion, moving outward...
                if (gamestate.Particles.Count((w)=>!w.Important) < BCBlockGameState.MaxParticles)
                {
                    for (int i = 0; i < (int)(25f * BCBlockGameState.ParticleGenerationFactor); i++)
                    {
                        const float speedmult = 1;
                        //choose a random Angle...
                        double randomangle = Math.PI * 2 * BCBlockGameState.rgen.NextDouble();
                        //create the appropriate speed vector, based on our radius...
                        double usespeed = (_CurrentRadius / _MaxRadius) * speedmult;
                        //should be proportional to how close we are to the maximum radius; max radius will have particles move 1...
                        usespeed += ((BCBlockGameState.rgen.NextDouble() * 0.6) - 0.3);

                        PointF addpointLocation = new PointF(Location.X + (float)Math.Cos(randomangle) * _CurrentRadius,
                                                             Location.Y + (float)Math.Sin(randomangle) * _CurrentRadius);


                        //create a dustparticle...
                        PointF PointSpeed = new PointF((float)(Math.Cos(randomangle) * usespeed),
                                                       (float)(Math.Sin(randomangle) * usespeed));
                        Particle addparticle = null;
                        if (i % 5 != 0)
                        {
                            addparticle = new DustParticle(addpointLocation, PointSpeed);

                        }
                        else if (_ShowOrbs)
                        {
                            //LightOrb...
                            LightOrb lo = new LightOrb(addpointLocation, ExplosionColor,
                                                       (float)BCBlockGameState.rgen.NextDouble() * 10 + 5);
                            lo.Velocity = PointSpeed;
                            addparticle = lo;

                        }


                        if (addparticle != null) gamestate.Particles.Add(addparticle);



                    }
                }



                //each frame, check for blocks that are within the given radius. Hell with it, we'll check their centerpoints...






                if (gamestate.PlayerPaddle != null)
                {
                    //if(

                    if (DamagePaddle && BCBlockGameState.RectangleIntersectsCircle(gamestate.PlayerPaddle.Getrect(), Location, _CurrentRadius))
                    {

                        gamestate.PlayerPaddle.HP -= 1f;
                    }
                }


                if (_EffectObjects)
                {
                    //find all GameObjects that implement IExplodable.
                    //we might be bothered to do the same for Balls, I suppose. No promises.

                    IEnumerable<IExplodable> result = BCBlockGameState.Join<IExplodable>(
                        (from ball in gamestate.Balls where ball is IExplodable && (BCBlockGameState.Distance(Location,ball.Location) < _CurrentRadius)  select ball as IExplodable),
                        (from obj in gamestate.GameObjects where obj is IExplodable select obj as IExplodable));

                        //in order to prevent contention issues, we will defer the loop that "effects" each item until the next gametick iteration starts.
                    gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                    {
                        foreach (var iterateresult in result)
                        {
                            
                            PointF useVelocityEffect = new PointF(Math.Max(1,Velocity.X),Math.Max(1,Velocity.Y));

                            iterateresult.ExplosionInteract(this, Location, useVelocityEffect);

                        }



                    }));






                }


                List<Block> removethese = new List<Block>();
                if (_DamageBlocks)
                {
                    var blowemup = from p in gamestate.Blocks
                                   where
                                       BCBlockGameState.Distance(Location.X, Location.Y, p.CenterPoint().X,
                                                                 p.CenterPoint().Y) < _CurrentRadius
                                   select p;
                    if (blowemup.Any())
                    {
                        foreach (var loopblock in blowemup)
                        {
                            //destroy it.

                            if (loopblock is DestructionBlock)
                            {
                                //special handling for combos
                                Debug.Print("DestructionBlock detected in ExplosionEffect...");
                                (loopblock as DestructionBlock).ComboCount = this.ComboCount + 1;

                                
                            }
                            if (DestroyAll || loopblock.MustDestroy())
                            {
                                loopblock.StandardSpray(gamestate);
                                removethese.Add(loopblock);
                            }

                        }


                        lock (gamestate.Blocks)
                        {
                            foreach (Block removeit in removethese)
                            {
                                //get angle between the center of the explosion and the block's center.
                                double Angle = BCBlockGameState.GetAngle(CenterPoint(), removeit.CenterPoint());
                                int useSpeed = BCBlockGameState.ClampValue((int)_MaxRadius / 2, 1, 5);
                                PointF useVelocity = BCBlockGameState.GetVelocity(useSpeed, Angle);
                                cBall tempball = new cBall(new PointF(removeit.CenterPoint().X-useVelocity.X,removeit.CenterPoint().Y-useVelocity.Y), useVelocity);
                                tempball.PreviousVelocity = useVelocity;
                                tempball.Behaviours.Add(new TempBallBehaviour());
                                //add a proxy behaviour to remove it as well.
                                //tempball.Behaviours.Add(new ProxyBallBehaviour("ExplosionEffect", null, proxyperformframe, null, null, null, null, null, null));
                                //gamestate.Balls.AddLast(tempball);

                                List<cBall> discardlist = new List<cBall>();
                                try
                                {
                                    //this is... well, cheating...

                                    //we cannot add GameObjects to the List, except by plopping them in the ref AddedObject parameter.
                                    //however, we cannot "force" the PerformBlockHit of a block (say a destruction block) to add a GameObject (another ExplosionEffect, in that case)

                                    //we cheat. we swap out the entire gamestate.GameObject LinkedList with a new one, call the routine, and then
                                    //we add any added GameObjects to our ref parameter, and swap the old list back in, hoping nobody notices our
                                    //audacity to fiddle with core game state objects...
                                    var copiedref = gamestate.GameObjects;
                                    gamestate.GameObjects = new LinkedList<GameObject>();

                                    removeit.PerformBlockHit(gamestate, tempball);
                                    gamestate.Blocks.Remove(removeit);
                                    var tempadded = gamestate.GameObjects;
                                    gamestate.GameObjects = copiedref;
                                    //now we add the ones we need to add to our ref array. I feel dirty.
                                    gamestate.Defer(() =>
                                    {
                                        foreach (var iterate in tempadded)
                                        {
                                            gamestate.GameObjects.AddLast(iterate);
                                        }

                                    });

                                    


                                }
                                catch
                                {

                                }
                                //we don't add the ball to our GameState, so we don't have to worry about it persisting :D

                                //the idea is that we want to invoke the actions of the block (which for many blocks will simply be destroyed).
                                //at the same time, some blocks might react weirdly to having temporary balls tossed into their centers, so we make it so the ball will only live for 
                                //two frames by adding a proxyballbehaviour that ensure that.
                                //gamestate.Blocks.Remove(removeit);
                                gamestate.Forcerefresh = true;
                            }

                            //changed: instead of removing them, create a temporary ball at their center point.






                        }


                    }
                }


                return retval;


            }


        }





        //basic class that basically takes the image, size, and speed of a object, flips it upside down, and animates it 
        //"dying" the same way they commonly do in old NES games.
        public class MarioDeathStyleObject : SizeableGameObject, iLocatable, IMovingObject
        {


            protected PointF _Velocity;
            public SizeF DrawSize { get; set; }
            private Image usedrawimage = null;
            private float maxspeed = 7;
            public PointF Velocity { get { return _Velocity; } set { _Velocity = value; } }
            public PointF VelocityAdd { get; set; }
            private PointF _VelocityDecay = new PointF(0.98f, 1);
            public PointF VelocityDecay { get { return _VelocityDecay; } set { _VelocityDecay = value; } }
            private void InitSpeeds()
            {
                Velocity = new PointF(0, -5);
                VelocityAdd = new PointF(0, 0.5f);

            }
            public override void setFrozen(bool newvalue)
            {
                //base.setFrozen(newvalue);
                //this object can't be frozen... :P
            }
            public Rectangle getRect()
            {
                return new Rectangle((int)(Location.X - DrawSize.Width), (int)(Location.Y - DrawSize.Height), (int)(DrawSize.Width), (int)(DrawSize.Height));



            }
            public override void Draw(Graphics g)
            {
                g.DrawImageUnscaled(usedrawimage, getRect());
            }
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                bool retval;
                lock (this)
                {
                    retval = !gamestate.GameArea.Contains(getRect());

                    
                    BCBlockGameState.IncrementLocation(gamestate,ref _Location,Velocity);


                    if (Velocity.Y < maxspeed)
                    {
                        Velocity = new PointF(Velocity.X + VelocityAdd.X, Velocity.Y + VelocityAdd.Y);
                        Velocity = new PointF(Velocity.X * VelocityDecay.X, Velocity.Y * VelocityDecay.Y);
                    }
                    else if (Velocity.Y > maxspeed)
                        Velocity = new PointF(Velocity.X, maxspeed);



                    base.PerformFrame(gamestate);
                }
                return retval;
            }
            public MarioDeathStyleObject(GameEnemy ge)
                : base(ge.Location,ge.DrawSize)
            {

                Location = ge.Location;
                SizeF ObjSize = ge.GetSize();
                usedrawimage = new Bitmap((int)(ObjSize.Width * 2), (int)(ObjSize.Height * 2));
                Graphics gdraw = Graphics.FromImage(usedrawimage);
                gdraw.Clear(Color.Transparent);
                //move it temporarity...
                ge.Location = new PointF(usedrawimage.Width / 2, usedrawimage.Height / 2);

                ge.Draw(gdraw);
                //move back
                ge.Location = Location;


                //flip the image vertically.
                usedrawimage.RotateFlip(RotateFlipType.RotateNoneFlipY);
                //now, set speed to our defaults.
                InitSpeeds();

                if (ge is IMovingObject)
                {
                    Velocity = new PointF((ge as IMovingObject).Velocity.X / 2, Velocity.Y);

                }




            }





        }






        public class MegamanTest : GameEnemy
        {
            int currframecounter = 0;

            public MegamanTest(PointF pLocation)
                : base(pLocation, null, 5)
            {
                Location = pLocation;
                StateFrameImageKeys = new Dictionary<string, string[]>();
                StateFrameImageKeys.Add("idle", new string[] { "megaman1", "megaman2", "megaman3", "megaman4" });
                StateFrameImageKeys.Add("green", new string[] { "megagreen1", "megagreen2", "megagreen3", "megagreen4" });

                StateFrameIndex = new Dictionary<string, int>();
                StateFrameIndex.Add("idle", 0);
                StateFrameIndex.Add("green", 0);

                FrameDelayTimes = new Dictionary<string, int[]>();
                FrameDelayTimes.Add("idle", new int[] { 25, 25, 25, 25, 25, 25, 25, 25, 25, 25 });
                FrameDelayTimes.Add("green", new int[] { 10, 10, 10, 10, 10, 10, 10, 10, 10 });

                DrawSize = new SizeF(56 * 2, 48 * 2);

            }
            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                base.PerformFrame(gamestate);
                currframecounter++;
                if (currframecounter >= 750)
                {
                    if (EnemyAction == "idle")
                    {
                        Debug.Print("Megaman entering Green");
                        EnemyAction = "green";

                    }
                    else if (EnemyAction == "green")
                    {
                        Debug.Print("Megaman entering idle ");
                        EnemyAction = "idle";


                    }
                    currframecounter = 0;

                }
                PointF VelocityUse = new PointF(0.25f, 0);
                switch (EnemyAction)
                {
                    case "idle":
                        break;
                    case "green":
                        //Location = new PointF((float)(Location.X + 3 * BCBlockGameState.rgen.NextDouble()) - 1.5f, Location.Y + (float)(3 * BCBlockGameState.rgen.NextDouble()) - 1.5f);
                        VelocityUse = new PointF(4, 0);
                        break;



                }
                
                BCBlockGameState.IncrementLocation(gamestate, ref _Location, VelocityUse);
                return !gamestate.GameArea.IntersectsWith(this.GetRectangle());
                //return false;
            }


        }

        public class SillyFace : GameEnemy
        {
            int currframecounter = 0;
            public SillyFace(PointF pLocation)
                : base(pLocation, null, 50)
            {
                this.Location = pLocation;
                StateFrameImageKeys = new Dictionary<string, string[]>();
                StateFrameImageKeys.Add("idle", new string[] { "faceidle1", "faceidle2" });
                StateFrameImageKeys.Add("attack", new string[] { "faceattack1", "faceattack2" });

                StateFrameIndex = new Dictionary<string, int>();
                StateFrameIndex.Add("idle", 0);
                StateFrameIndex.Add("attack", 0);
            }

            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                base.PerformFrame(gamestate);
                currframecounter++;
                if (currframecounter >= 750)
                {
                    if (EnemyAction == "idle")
                    {
                        Debug.Print("SillyFace entering Attack state");
                        EnemyAction = "attack";

                    }
                    else if (EnemyAction == "attack")
                    {
                        Debug.Print("SillyFace entering idle state");
                        EnemyAction = "idle";


                    }
                    currframecounter = 0;

                }

                switch (EnemyAction)
                {
                    case "idle":
                        break;
                    case "attack":
                        Location = new PointF((float)(Location.X + 3 * BCBlockGameState.rgen.NextDouble()) - 1.5f, Location.Y + (float)(3 * BCBlockGameState.rgen.NextDouble()) - 1.5f);
                        break;
                }
                return false;
            }

        }




        public class BasicFadingText : GameObject
        {
            public delegate float GetSpeedVector(BasicFadingText obj);
            
            /// <param name="obj"></param>
            /// <returns>true to cancel default processing, false otherwise.</returns>
            public delegate bool CustomFrameFunction(BasicFadingText obj);



            private int HueCycle = 0;
            public bool HueCycler(BasicFadingText obj)
            {

                obj.HueCycle=(obj.HueCycle+1)%240;
                Color usecolor = new HSLColor(obj.HueCycle, 240, 120);
                obj.TextBrush = new SolidBrush(usecolor);
                return true;
            }

            public GetSpeedVector XSpeedDelegate;
            public GetSpeedVector YSpeedDelegate;
            public long numticks = 0;
            private int maxTTL = 150; //max time to live
            private int life = 0;
            private String mText = "";
            private PointF mPosition;
            private PointF mVelocity;
            private Font mFontUse;
            private Brush mtextbrush;
            private Pen mTextPen;
            private GraphicsPath usepath;
            private CustomFrameFunction _FrameFunction = null;
            //revision to this class:
            //changed to use a Bitmap instead of drawing on the target surface.

            public String Text { get { return mText; } set { mText = value; BuildTextBitmap(); } }
            public PointF Position { get { return mPosition; } set { mPosition = value; } }
            public PointF Velocity { get { return mVelocity; } set { mVelocity = value; } }
            public Font FontUse { get { return mFontUse; } set { mFontUse = value; BuildTextBitmap(); } }
            public CustomFrameFunction FrameFunction { get { return _FrameFunction; } set { _FrameFunction = value; } }
            public Brush TextBrush { get { return mtextbrush; } set { mtextbrush = value; BuildTextBitmap(); } }
            public Pen TextPen { get { return mTextPen; } set { mTextPen = value; BuildTextBitmap(); } }


            private Bitmap TextBitmap = null;
            private Graphics TextCanvas = null;


            public float myspeedfalloff = 0.95f;

            /// <summary>
            /// Creates the bitmap of this text.
            /// </summary>
            ///
            private void BuildTextBitmap()
            {
                if (mTextPen == null)
                    mTextPen = new Pen(Color.Black);

                if (mtextbrush == null)
                    mtextbrush = new SolidBrush(Color.Black);
                //redraw the bitmap.
                if (TextCanvas != null)
                    TextCanvas.Dispose();
                if (TextBitmap != null)
                    TextBitmap.Dispose();

                SizeF ssize = CalcSize();

                //create a new bitmap of that size.
                TextBitmap = new Bitmap((int)(Math.Ceiling(ssize.Width)), (int)Math.Ceiling(ssize.Height));
                TextCanvas = Graphics.FromImage(TextBitmap);
                TextCanvas.CompositingQuality = CompositingQuality.HighQuality;
                TextCanvas.SmoothingMode = SmoothingMode.HighQuality;

                //Draw to it... using a GraphicsPath.
                usepath = new GraphicsPath();

                usepath.AddString(mText, mFontUse.FontFamily, (int)mFontUse.Style, mFontUse.Size, new Point(0, 0), StringFormat.GenericDefault);
                TextCanvas.DrawPath(mTextPen, usepath);
                TextCanvas.FillPath(mtextbrush, usepath);

                //tada...



            }
            public SizeF CalcSize()
            {

                //returns the size 
                Graphics measureg = BCBlockGameState.getmeasureg();
                return measureg.MeasureString(mText, mFontUse);



            }
            /*
            private void BuildTextBitmap()
            {
                //first, measure the text.
                int penwidth = mTextPen==null?1:(int)mTextPen.Width;
                SizeF textmeasure = BCBlockGameState.MeasureString(mText,mFontUse);
                //create a new bitmap of that size.

                TextBitmap = new Bitmap((int)textmeasure.Width+penwidth, (int)textmeasure.Height+penwidth);
                TextCanvas = Graphics.FromImage(TextBitmap);
                TextCanvas.PageUnit = GraphicsUnit.Point;
                TextCanvas.PixelOffsetMode = PixelOffsetMode.HighQuality;
                if (mtextbrush == null || mTextPen == null)
                    TextCanvas.DrawString(mText, mFontUse, new SolidBrush(Color.Black), new Point(0,0));
                else
                {
                



                    //create path.
                    GraphicsPath thispath = new GraphicsPath();
                    if (mText != null)
                    {
                        thispath.AddString(mText, mFontUse, new Point(penwidth, penwidth), StringFormat.GenericDefault);

                        //fill and stroke in our target bitmap...

                        TextCanvas.FillPath(mtextbrush, thispath);
                        TextCanvas.DrawPath(mTextPen, thispath);
                    }
                    //ta da....
                }



            }*/

            public BasicFadingText(String textrise, PointF Position, PointF Velocity, Font fontuse, Pen TextPen, Brush textbrush)
                : this(textrise, Position, Velocity, fontuse, TextPen, textbrush, 150)
            {


            }

            public BasicFadingText(String textrise, PointF Position, PointF Velocity, Font fontuse, Pen TextPen, Brush textbrush, int TTL)
            {
                mText = textrise;
                mPosition = Position;
                mVelocity = Velocity;
                mFontUse = fontuse;
                mtextbrush = textbrush;
                mTextPen = TextPen;
                maxTTL = TTL;
                FrameFunction = HueCycler;
                BuildTextBitmap();

            }

            private Rectangle RectFToRect(RectangleF convfrom)
            {
                return new Rectangle((int)convfrom.Left, (int)convfrom.Top, (int)convfrom.Width, (int)convfrom.Height);


            }

            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                numticks++;
                bool dodefault = FrameFunction == null || FrameFunction(this);
                

                if (dodefault)
                {
                    mPosition = new PointF(mPosition.X + mVelocity.X, mPosition.Y + mVelocity.Y);
                    //mVelocity = new PointF(mVelocity.X,mVelocity.Y*myspeedfalloff);

                    if (XSpeedDelegate != null && YSpeedDelegate != null)
                    {
                        mVelocity = new PointF(XSpeedDelegate(this), YSpeedDelegate(this));

                    }
                    else if ((XSpeedDelegate != null) ^ (YSpeedDelegate != null))
                    {
                        if (XSpeedDelegate != null)
                            mVelocity = new PointF(XSpeedDelegate(this), mVelocity.Y);
                        else
                            mVelocity = new PointF(mVelocity.X, YSpeedDelegate(this));

                    }
                    life++;
                }
               
                bool retval = life > maxTTL;
                if (measuredsize != null)
                {
                    retval = retval || !(gamestate.GameArea.IntersectsWith(RectFToRect(new RectangleF(mPosition, measuredsize.Value))));

                }
                return (retval);
            }
            SizeF? measuredsize;


            public override void Draw(Graphics g)
            {
                float alpharatio = 1f;
                measuredsize = TextBitmap.Size;
                PointF usePosition = new PointF(mPosition.X - (measuredsize.Value.Width / 2), mPosition.Y - (measuredsize.Value.Height / 2));
                alpharatio = 1 - ((float)life) / ((float)maxTTL);
                ImageAttributes useattributes = new ImageAttributes();

                useattributes.SetColorMatrix(ColorMatrices.GetColourizer(1f, 1f, 1f, alpharatio));
                Point usepos = new Point((int)usePosition.X, (int)usePosition.Y);
                g.DrawImage(TextBitmap, new Rectangle(usepos, TextBitmap.Size), 0, 0, TextBitmap.Width, TextBitmap.Height, GraphicsUnit.Pixel, useattributes);

            }

            /*
            public override void Draw(Graphics g)
            {
                measuredsize = g.MeasureString(mText,mFontUse);
            
                PointF usePosition = new PointF(mPosition.X - (measuredsize.Value.Width / 2), mPosition.Y - (measuredsize.Value.Height / 2));
                if (mtextbrush == null || mTextPen == null)
                    g.DrawString(mText, mFontUse, new SolidBrush(Color.Black), usePosition);
                else
                {

                    if (mText != null)
                    {
                        usepath = new GraphicsPath();
                        usepath.AddString(mText, mFontUse.FontFamily, (int) mFontUse.Style, mFontUse.Size, usePosition,
                                          StringFormat.GenericDefault);
                        if (mtextbrush != null) g.FillPath(mtextbrush, usepath);
                        if (mTextPen != null) g.DrawPath(mTextPen, usepath);
                    }
                }
            }*/
        }




    
}