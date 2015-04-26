using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using BASeBlock.Blocks;
using BASeBlock.Events;
using BASeBlock.GameObjects;
using BASeBlock.GameObjects.Orbs;
using BASeBlock.PaddleBehaviours;
using System.Threading;
using BASeBlock.GameStates;
using BASeBlock.Particles;
using BASeBlock.Powerups;
using BASeBlock.Projectiles;

namespace BASeBlock
{

    public abstract class BaseTerminatorBehaviour : BasePaddleBehaviour
    {
        protected bool ButtonBPressed = false;
        protected BCBlockGameState mstate;
        protected int PowerLevel = 1;
        protected System.Threading.Timer MachineGunTimer = null;
        protected bool _AllowCharge=true; //whether we allow charging.
        protected bool coolingdown = false;

        protected int ChargeAmount = 0;
        protected iActiveSoundObject chargeupsound = null;
        protected iActiveSoundObject chargeloopsound = null;
        protected virtual bool ShootPrecharge()
        {
            return true;


        }

        protected TimeSpan GetCooldownTime()
        {
            return new TimeSpan(0, 0, 0, 0, 500);

        }
        public BaseTerminatorBehaviour(BCBlockGameState stateobject)
        {
            mstate = stateobject;
            stateobject.ClientObject.ButtonDown += ClientObject_ButtonDown;
            stateobject.ClientObject.ButtonUp += ClientObject_ButtonUp;

        }
        ~BaseTerminatorBehaviour()
        {
            BehaviourRemoved(attachedPaddle, attachedstate);

        }
        void ClientObject_ButtonUp(Object sender, ButtonEventArgs<bool> e)
        {
            if (e.Button == ButtonConstants.Button_B)
            {
                if (MachineGunTimer != null)
                    MachineGunTimer.Dispose();


                ButtonBPressed = false;
            }

            e.Result = true;
        }
        public virtual bool ChargeTick(int chargelevel)
        {

            return chargelevel < 20; //about 2 seconds.

        }

        protected virtual void ChargeShotEffect()
        {
            //add particles and stuff, to indicate when we have shot something powerful.



        }
        void ClientObject_ButtonDown(Object sender, ButtonEventArgs<bool> e)
        {
            //make sure we exist!
            if (coolingdown) { e.Result = true; return; }
            if (mstate.PlayerPaddle != null && !mstate.PlayerPaddle.Behaviours.Contains(this))
            {
                //unhook
                mstate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
                mstate.ClientObject.ButtonUp -= ClientObject_ButtonUp;
                e.Result = true;
                return;

            }
            Debug.Print("Active State:" + mstate.ClientObject.ActiveState.GetType().Name);
            //don't shoot if we are not in the "run" state.
            if (mstate.ClientObject.ActiveState is StateRunning)
            {



                if (e.Button == ButtonConstants.Button_B)
                {
                    //if we don't allow charging, we shoot immediately. Otherwise, we 
                    //shoot when the button is released.
                    ButtonBPressed = true;
                    if (_AllowCharge)
                    {
                        //create a new thread from a in-line lambda. We basically just want to increment a counter
                        //and call Shoot with that counter as the parameter when the button is released.
                        mstate.DelayInvoke(GetCooldownTime(), (args) => coolingdown = false);
                        coolingdown = true;
                        //Another note, is we also Shoot once, normally.
                        if(ShootPrecharge())
                            Shoot(1); 
                        //now, we start the separate thread.
                        new Thread(() =>
                        {
                            int ChargeTicks = 0;
                            while (ButtonBPressed)
                            {
                                Thread.Sleep(100);
                                if (ChargeTick(ChargeTicks))
                                    ChargeTicks++;

                                ChargeAmount = ChargeTicks;
                                if (mstate.ClientObject.ActiveState is StateNotRunning) return;
                            }
                            BCBlockGameState.Soundman.PlaySound("chargeshot");
                            ChargeShotEffect();
                            Shoot(ChargeTicks);
                            if (chargeupsound != null) chargeupsound.Stop();
                            if (chargeloopsound != null) chargeloopsound.Stop();
                            ChargeAmount = 0;
                        }

                        ).Start();
                        //additionally, we need to do something else: We want to play the charge starting sound, 
                        //and then hook the soundstop event, and when that sound stops, play the charge loop until we fire.

                        //playing the sound will give us an IActiveSoundObject...

                        chargeupsound = BCBlockGameState.Soundman.PlaySound("charging", false);

                        //stored in a protected member, so we can refer to it in the event handler.
                        //we'll define the event handler separately for clarity's sake.
                        BCBlockGameState.Soundman.Driver.OnSoundStop += new OnSoundStopDelegate(Driver_OnSoundStop);





                    }
                    else
                    {



                        Shoot(1);
                        mstate.DelayInvoke(GetCooldownTime(), (args) => coolingdown = false);
                        coolingdown = true;
                    }
                }
            }
            e.Result = true;
        }

        void Driver_OnSoundStop(iActiveSoundObject objstop)
        {
            //if the sound that stopped is the one we are interested in...
            if (chargeupsound == null) return;
            if (objstop == chargeupsound)
            {
                //unhook this event handler.
                BCBlockGameState.Soundman.Driver.OnSoundStop -= Driver_OnSoundStop;
                chargeupsound = null;
                //now, the loop sound.
                //but only if we continued pressing the button.
                if (ButtonBPressed)
                {
                    //FIX: if chargeloopsound is set, stop it.
                    if (chargeloopsound != null) chargeloopsound.Stop();
                    chargeloopsound = BCBlockGameState.Soundman.PlaySound("charged", true);
                }
                //we store this, so the iPaddleBehaviour implementation can see when we are removed from a paddle and stop it.

            }

            //throw new NotImplementedException();
        }
        
        protected virtual float GetPowerUsage()
        {
            return 1;

        }
        private void Shoot(int ChargeTicks)
        {
            if (coolingdown)
                return;
            mstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(
            () =>
            {
                if (mstate.PlayerPaddle == null) return;
                //if we don't have enough energy...
                if (mstate.PlayerPaddle.Energy <= 0)
                {
                    //fail to shoot. Make some clouds.
                    BCBlockGameState.Soundman.PlaySound("shootfail");
                    //and, again, showmanship at work- "shoot" some black light orbs... to simulate smoke...

                    for (int i = 0; i < 6; i++)
                    {
                        PointF usespeed = new PointF(0, (float)(-3 - (BCBlockGameState.rgen.NextDouble())));
                        usespeed = BCBlockGameState.VaryVelocity(usespeed, Math.PI / 6);
                        LightOrb addorb = new LightOrb(mstate.PlayerPaddle.Getrect().CenterPoint(),
                           BCBlockGameState.rgen.NextDouble() > 0.5d ? Color.Black : Color.Gray, 15 + (float)(15 * BCBlockGameState.rgen.NextDouble()));
                        addorb.Velocity = usespeed;
                        addorb.VelocityDecay = new PointF(0.9f, 0.85f);
                        mstate.Particles.Add(addorb);
                        //take damage, too...


                    }
                    mstate.PlayerPaddle.HP -= 10;
                    return;
                }
                
                    mstate.PlayerPaddle.Energy -= GetPowerUsage();
                    ShootFunction(ChargeTicks);
                
            }));
        }
        /// <summary>
        /// ShootFunction. this must be overridden to perform the actual... shooting.
        /// </summary>
        protected virtual void ShootFunction(int ChargeTicks)
        {


        }
        Image _cachedIcon = null;
        public override Image GetIcon()
        {
            //cheat, we will draw on a 128x16, and return that.
            //default to... default icon. Terminator. (from the old architecture).
            //the Draw() routine requires a paddle, so we need to create a new one temporarily.
            //arguably this could have side-effects if the Paddle Behaviour implementation cares that
            //the paddle argument is consistent, but this is why we are giving the paddle argument in the first place.

            if (_cachedIcon == null)
            {
                //create the bitmap.
                Bitmap drawcached = new Bitmap(128, 16);
                using (Graphics gcache = Graphics.FromImage(drawcached))
                {
                    gcache.Clear(Color.Transparent);
                    Paddle temporarypaddle = new Paddle(BCBlockGameState.MainGameState, new Size(128, 16), PointF.Empty, null);
                    //since we don't <actually> draw the paddle, or call it's methods (well, directly...) this should work.
                    //but, if an exception is thrown, we want to catch it and use a default.
                    try
                    {
                        Draw(temporarypaddle, gcache);
                        //all worked? good. Use drawcached as our bitmap now.
                        _cachedIcon = drawcached;


                    }
                    catch (Exception err)
                    {
                        _cachedIcon = BCBlockGameState.Imageman.getLoadedImage("TERMINATOR");
                    }
                }

            }
            return _cachedIcon;
            //return BCBlockGameState.Imageman.getLoadedImage("TERMINATOR");
        }
        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            toPaddle.Energy = Math.Max(100, toPaddle.Energy); //when added, energy is restored.
            base.BehaviourAdded(toPaddle, gamestate);
        }
        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            if (chargeloopsound != null)
                chargeloopsound.Stop();
            //base.BehaviourRemoved(fromPaddle, gamestate);
            if (mstate != null)
            {
                mstate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
                mstate.ClientObject.ButtonUp -= ClientObject_ButtonUp;

            }
        }
        public virtual Image PaddleOverlay()
        {
            return BCBlockGameState.Imageman.getLoadedImage("Terminator");

        }


        protected void DefaultChargeDraw(Paddle onPaddle, Graphics g, int maxChargeTicks = 50)
        {
            if (ChargeAmount == 0) return;
            Color[] OutlineCycles = new Color[] { Color.Red, Color.Orange, Color.Yellow, Color.Green, Color.Blue, Color.Indigo, Color.Violet };

            int useAlpha = (int)(((float)ChargeAmount) / maxChargeTicks * 255);

            if (ChargeAmount > (maxChargeTicks / 2))
            {
                //standard fill...
                Color usecolor = OutlineCycles[DateTime.Now.Millisecond % OutlineCycles.Length];
                g.FillRectangle(new SolidBrush(Color.FromArgb(useAlpha, usecolor)), onPaddle.Getrect());

            }

            //do the outline either way, though.
            Color pencolor = OutlineCycles[(DateTime.Now.Millisecond + 3) % OutlineCycles.Length];
            g.DrawRectangle(new Pen(Color.FromArgb(useAlpha, pencolor), 2), onPaddle.Getrect());

        }
        protected WeakReference _OwnerPaddle;
        protected Paddle Owner { 
            get 
            {
                return _OwnerPaddle.Target as Paddle;
                
            
            
            } set { _OwnerPaddle = new WeakReference(value); } }
        public override void Draw(Paddle onPaddle, Graphics g)
        {
            if (_OwnerPaddle == null) _OwnerPaddle = new WeakReference(onPaddle);
            PointF Position = onPaddle.Position;
            SizeF PaddleSize = onPaddle.PaddleSize;
            RectangleF drawrect = new RectangleF(Position.X - PaddleSize.Width / 2, Position.Y - PaddleSize.Height / 2, PaddleSize.Width, PaddleSize.Height);
            //g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Green)), drawrect);
            //draw the "sticky" overlay...
            g.DrawImage(PaddleOverlay(), drawrect);

            //draw "chargeamount"...
            if (_AllowCharge)
            {
                


            }

        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            return false;
            //throw new NotImplementedException();
        }


    }
    /// <summary>
    /// Generic class overridden by class to implement a PaddlePowerup for the given type.
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class TerminatorPaddlePowerup<T> : PaddlePowerUp<T> where T : iPaddleBehaviour
    {
        protected TerminatorPaddlePowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize)
        {
        }

        public override Image[] GetPowerUpImages()
        {
            return new Image[] { BCBlockGameState.Imageman.getLoadedImage("weaponpwr") };
        }

    }


    public class TerminatorPelletBehaviour : BaseTerminatorBehaviour
    {
        private float usespeed = 7;
        public TerminatorPelletBehaviour(BCBlockGameState stateobject)
            : base(stateobject)
        {
            _AllowCharge = true;
        }
        
        public static string Name
        {
            get
            {
                return "Pellet Shooter";
            }
        }
        public override Image GetIcon()
        {
            return TerminatorPelletPowerup.PowerupImage;
        }
        public override Image PaddleOverlay()
        {
            return BCBlockGameState.Imageman.getLoadedImage("terminator_pellet");
        }
        public override bool ChargeTick(int chargelevel)
        {
            return chargelevel < 40;
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            base.Draw(onPaddle, g);
            if (ChargeAmount == 0) return;
            int AlphaUse = (int)(((float)ChargeAmount/40)*255);
            if (AlphaUse > 255) AlphaUse = 255;
            int RedLevel = (int)(Math.Sin(DateTime.Now.Millisecond) * 128) + 128;
            int GreenLevel = (int)(Math.Sin(DateTime.Now.Millisecond/2) * 128) + 128;
            int BlueLevel = (int)(Math.Cos(DateTime.Now.Millisecond * 2) * 128) + 128;
            Color useColor = Color.FromArgb(AlphaUse, RedLevel, GreenLevel, 0);

            if (ChargeAmount > 20)
            {
                //draw fill!
                g.FillRectangle(new SolidBrush(useColor), onPaddle.Getrect());



            }
            //draw outline
            g.DrawRectangle(new Pen(Color.FromArgb(0, 0, BlueLevel),2),onPaddle.Getrect());




        }
        protected override void ShootFunction(int ChargeTicks)
        {
            int numticks = Math.Max(6,ChargeTicks/2);

            base.ShootFunction(ChargeTicks);
            BCBlockGameState.Soundman.PlaySound("firelaser");
            for (int i = 0; i < 6; i++)
            {
                //vary the speed a bit.
                usespeed = (float)(usespeed * (1.0 + ((float)BCBlockGameState.rgen.NextDouble() * 0.2f - 0.1f)));

                PointF usevelocity = BCBlockGameState.VaryVelocity(new PointF(0, -usespeed), Math.PI / 13);




                Bullet b = new Bullet(mstate.PlayerPaddle.Getrect().ToRectangleF().TopCenter(), usevelocity);
                



                b.BulletBrush = new SolidBrush(Color.OrangeRed);
                mstate.GameObjects.AddLast(b);


            }
        }
    }

    public class TerminatorFireworkBehaviour : BaseTerminatorBehaviour
    {
        protected float usespeed = 6;
        public TerminatorFireworkBehaviour(BCBlockGameState stateobject)
            : base(stateobject)
        {
            _AllowCharge = false;
        }
        public static string Name
        { get { return "Firework"; } }
        public override bool ChargeTick(int chargelevel)
        {
            return chargelevel < 50;
        }
        protected override void ShootFunction(int ChargeTicks)
        {
            base.ShootFunction(ChargeTicks);
            mstate.GameObjects.AddLast(new DustTrail(mstate.PlayerPaddle.Getrect().CenterPoint(),
                BCBlockGameState.VaryVelocity(new PointF(0, -2), Math.PI / 4), new TimeSpan(0, 0, 0, 2),
                new FireworkEffect(BCBlockGameState.MTypeManager[typeof(CollectibleOrb)].ManagedTypes 
                    , PointF.Empty, 12, 30, 75,new SizeF(8,8)        )));


            
        }


    }
    public class TerminatorPermanentBallShootBehaviour :BaseTerminatorBehaviour
    {
        protected float usespeed;
        public TerminatorPermanentBallShootBehaviour(BCBlockGameState pGameState):base(pGameState)
        {
            _AllowCharge = false;


        }
        public static string Name
        {
            get
            {
                return "Perma-Ball Shooter";
            }

        }
        protected override void ShootFunction(int ChargeTicks)
        {



            cBall createball = new cBall(mstate.PlayerPaddle.BlockRectangle.TopCenter(), BCBlockGameState.VaryVelocity(new PointF(0, -3), Math.PI / 8));
            mstate.Defer(() => mstate.Balls.AddLast(createball));

        }

    }
    public class TerminatorBallShotBehaviour : BaseTerminatorBehaviour
    {

        protected float usespeed = 6;
        public TerminatorBallShotBehaviour(BCBlockGameState stateobject)
            : base(stateobject)
        {
        }

        
       
        
        

        public static string Name
        {
            get { return "Ball shooter"; }
        }
        public override bool ChargeTick(int chargelevel)
        {
            return chargelevel < 50;
        }
        public override Image GetIcon()
        {
            return TerminatorBallShotPowerup.PowerupImage;
        }
        protected override void ShootFunction(int ChargeTicks)
        {


            base.ShootFunction(ChargeTicks);
            BCBlockGameState.Soundman.PlaySound("firelaser");

            int NumShot = Math.Max(1, ChargeTicks / 2);

            //vary angle from 0 to PI.

            double angleincrement = Math.PI / NumShot;
            double currangle = 0;
            for (int i = 0; i < NumShot; i++)
            {


                PointF usevelocity = BCBlockGameState.VaryVelocity(new PointF(0, -usespeed), NumShot*(Math.PI/2));
                cBall addball = new ProjectileBall(mstate.PlayerPaddle.Getrect().ToRectangleF().TopCenter(), usevelocity);
                mstate.Balls.AddLast(addball);
            }
        }
    }
    public class TerminatorHitscanshotBehaviour : BaseTerminatorBehaviour
    {
        public TerminatorHitscanshotBehaviour(BCBlockGameState stateobject)
            : base(stateobject)
        {
        }

        public static string Name
        {
            get {
            return "Laser";
            }
        }
        protected override void ShootFunction(int ChargeTicks)
        {
            base.ShootFunction(ChargeTicks);
            BCBlockGameState.Soundman.PlaySound("shoot");
            HitscanBullet hs = new HitscanBullet(mstate.PlayerPaddle.Getrect().ToRectangleF().TopCenter(), new PointF(0, -1));
            hs.Penetrate = false;
            hs.Strength = HitscanBullet.HitscanStrengthConstants.hitscan_hit;
            hs.BulletColor = Color.Black;
            mstate.GameObjects.AddFirst(hs);
        }
    }
    public class TerminatorShotgunBehaviour : BaseTerminatorBehaviour
    {
        public TerminatorShotgunBehaviour(BCBlockGameState stateobject)
            : base(stateobject)
        {

        }
        public override Image GetIcon()
        {
            return TerminatorBallShotPowerup.PowerupImage;
        }
        public override Image PaddleOverlay()
        {
            return BCBlockGameState.Imageman.getLoadedImage("terminator_shotgun");
        }
        public static string Name
        {
            get
            {
                return "shotgun";
            }
        }    
        protected override void ShootFunction(int ChargeTicks)
        {
            base.ShootFunction(ChargeTicks);
            BCBlockGameState.Soundman.PlaySound("Shotgun");
            for (int i = 0; i < Math.Max(4,ChargeTicks); i++)
            {
                PointF usevelocity = new PointF((float)(BCBlockGameState.rgen.NextDouble() * 3) - 1.5f, -5);
                HitscanBullet hs = new HitscanBullet(mstate.PlayerPaddle.Getrect().CenterPoint(), usevelocity);
                hs.Penetrate = false;
                hs.Tracer = false;
                hs.Strength = HitscanBullet.HitscanStrengthConstants.hitscan_bullet;
                hs.BulletColor = Color.Gray;
                mstate.GameObjects.AddFirst(hs);
            }

        }
    }

    /// <summary>
    /// Standard Laser Shot behaviour. Shoots dual bouncing lasers that do a bit of damage to blocks they bounce off of.
    /// </summary>
    public class TerminatorLaserBehaviour : BaseTerminatorBehaviour
    {
        public TerminatorLaserBehaviour(BCBlockGameState stateobject) : base(stateobject)
        {
        }
        public override Image PaddleOverlay()
        {
            return BCBlockGameState.Imageman.getLoadedImage("terminator_laser");
        }
        protected override float GetPowerUsage()
        {
            return 3;
        }
        public static String Name
        {
            get
            {
                return "Laser";

            }

        }
        
        public override bool ChargeTick(int chargelevel)
        {
            return chargelevel < 50;
        }
       
        public override void Draw(Paddle onPaddle, Graphics g)
        {
            base.Draw(onPaddle, g);
            //
            //if chargeamount is less than 2.5, change colour only of outline.
            //if we aren't charging, obviously this doesn't do anything.
            DefaultChargeDraw(onPaddle, g);

            
        }
        protected override void ShootFunction(int ChargeTicks)
        {

            //ChargeTicks dmax is 50 here. Divide the value by 10, and use that for width. use value divided by 5 for the number of bounces.

            int usewidth = Math.Max(2, ChargeTicks / 5);

            mstate.PlayerPaddle.Energy -= Math.Max(1, ChargeTicks / 2);


            base.ShootFunction(ChargeTicks);
            //Shoot one laser from the center of the paddle.
            var ShootOrigin = mstate.PlayerPaddle.Getrect().ToRectangleF().TopCenter();
            ShootOrigin = new PointF(ShootOrigin.X, ShootOrigin.Y - 2);

            LaserShot CentralLaser = new LaserShot(ShootOrigin, new PointF(0, Math.Min(-6, -ChargeTicks / 6)));
            CentralLaser.MaxBounces = ChargeTicks / 3;
            CentralLaser.LaserPen = new Pen(Color.Red, usewidth);
            CentralLaser.Weak = ChargeTicks < 40;
            
            CentralLaser.MaxBounces = Math.Max(ChargeTicks/5,3);
            
            mstate.GameObjects.AddLast(CentralLaser);
            


        }
    }
    public class TerminatorLaserSpinBehaviour : BaseTerminatorBehaviour
    {
        public static String Name { get { return "Laser Spin Behaviour"; } }
        protected override float GetPowerUsage()
        {
            return 50;
        }
        public TerminatorLaserSpinBehaviour(BCBlockGameState stateobject) : base(stateobject)
        {
            _AllowCharge = false;
        }
        public override Image PaddleOverlay()
        {
            return BCBlockGameState.Imageman.getLoadedImage("Terminator_LaserSpin");
        }
        
        public override string getName()
        {
            return "Laser Spin";
        }
        public override Image GetIcon()
        {
            return TerminatorLaserSpinPowerup.PowerupImage;
        }
        protected override void ShootFunction(int ChargeTicks)
        {
            
            base.ShootFunction(ChargeTicks);
            cBall addthis = new cBall(mstate.PlayerPaddle.Getrect().ToRectangleF().TopCenter(),BCBlockGameState.VaryVelocity(new PointF(0,-2),Math.PI/13));
            addthis.Behaviours.Add(new LaserSpinBehaviour(new TimeSpan(0, 0, 0, 0, 250)));
            addthis.Behaviours.Add(new TempBallBehaviour(5));
            addthis.DrawColor = Color.Blue;
            addthis.DrawPen = new Pen(Color.Yellow, 2);
            mstate.Balls.AddLast(addthis);
        }
    }


    public class TerminatorPelletPowerup : TerminatorPaddlePowerup<TerminatorPelletBehaviour>
    {
        public TerminatorPelletPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize)
        {
        }
        public static Image PowerupImage = BCBlockGameState.Imageman.getLoadedImage("TerminatorPower_Pellet");
        public override Image[] GetPowerUpImages()
        {
            return new Image[] {PowerupImage};
        }
    }
    public class TerminatorLaserSpinPowerup : TerminatorPaddlePowerup<TerminatorLaserSpinBehaviour>
    {
        public TerminatorLaserSpinPowerup(PointF pLocation, SizeF pObjectSize)
            : base(pLocation, pObjectSize)
        {

        }
        public static Image PowerupImage = BCBlockGameState.Imageman.getLoadedImage("TerminatorPower_LaserSpin");
        public override Image[] GetPowerUpImages()
        {
            return new Image[] { PowerupImage };
        }
        public override string Name
        {
            get
            {
                {
                    return "Laser SpinShot";
                }
            }



        }
    }
    public class TerminatorBallShotPowerup : TerminatorPaddlePowerup<TerminatorBallShotBehaviour>
    {
        public TerminatorBallShotPowerup(PointF Location, SizeF ObjectSize)
            : base(Location, ObjectSize)
        {
            
        }
        public static Image PowerupImage = BCBlockGameState.Imageman.getLoadedImage("TerminatorPower_ballshot");
        public override Image[] GetPowerUpImages()
        {
            return new Image[] { PowerupImage };
        }
        public override string Name
        {
            get
            {
                return "Ball Shot";
            }
        }
    }
    
    public class TerminatorPistolPowerup : TerminatorPaddlePowerup<TerminatorHitscanshotBehaviour>
    {
        public TerminatorPistolPowerup(PointF Location, SizeF ObjectSize) : base(Location, ObjectSize)
        {
            
        }
        public Image PowerupImage = BCBlockGameState.Imageman.getLoadedImage("TerminatorPower_Pistol");
        public override Image[] GetPowerUpImages()
        {
            return new Image[] { PowerupImage };
        }
        public override string Name
        {
            get
            {
                return "Pistol";
            }
        }
    }
    public class TerminatorShotgunPowerup : TerminatorPaddlePowerup<TerminatorShotgunBehaviour>
    {
        public TerminatorShotgunPowerup(PointF Location, SizeF ObjectSize) : base(Location, ObjectSize)
        {
        }
        public Image PowerupImage = BCBlockGameState.Imageman.getLoadedImage("TerminatorPower_shotgun");
        public override Image[] GetPowerUpImages()
        {
            return new Image[] { PowerupImage };
        }
        public override string Name
        {
            get
            {
                return "Shotgun";
            }
        }
    }
    public class TerminatorLaserPowerup : TerminatorPaddlePowerup<TerminatorLaserBehaviour>
    {
        public TerminatorLaserPowerup(PointF Location, SizeF ObjectSize) : base(Location, ObjectSize)
        {
        }
        public override string Name
        {
            get
            {
                return "Laser";
            }
        }
    }

    public class TerminatorLightningBehaviour : BaseTerminatorBehaviour
    {
        public TerminatorLightningBehaviour(BCBlockGameState stateobject) : base(stateobject)
        {
            _AllowCharge = false;
        }
        public override string getName()
        {
            return "Lightning";

        }
        protected override float GetPowerUsage()
        {
            return 10;
        }
        protected override void ShootFunction(int ChargeTicks)
        {
            var copied = (_OwnerPaddle.Target as Paddle);
            if (copied == null) return;
            LightningShot leffect = new LightningShot(copied.BlockRectangle.TopCenter(), new PointF(0, -4));
            var gstat = (copied.wGameState.Target as BCBlockGameState);
            gstat.GameObjects.AddLast(leffect);
            BCBlockGameState.Soundman.PlaySound("clap");
        }
        
    }

}
