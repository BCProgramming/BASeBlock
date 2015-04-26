using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
using BASeBlock.Events;
using BASeBlock.GameStates;
using BASeBlock.Particles;
using BASeBlock.Projectiles;

namespace BASeBlock.PaddleBehaviours
{
    /// <summary>
    /// Abstract base class for new set of Terminator behaviours.
    /// </summary>
    public class TerminatorBehaviour : BasePaddleBehaviour
    {
        private static int MaxPowerup = 10;
        private bool Allowcharge = true;
        private bool ButtonBPressed = false;
        private System.Threading.Thread ChargeWatcher = null;
        private System.Threading.Timer MachineGunTimer = null;
        public int PowerLevel = 1;
        private BCBlockGameState mstate;

        public TerminatorBehaviour(BCBlockGameState stateobject)
        {
            mstate = stateobject;
            stateobject.ClientObject.ButtonDown += ClientObject_ButtonDown;
            stateobject.ClientObject.ButtonUp += ClientObject_ButtonUp;
        }

        public override string getName()
        {
            return "Terminator";
        }

        ~TerminatorBehaviour()
        {
            BehaviourRemoved(attachedPaddle, attachedstate);
        }

        public override Image GetIcon()
        {
            return BCBlockGameState.Imageman.getLoadedImage("TERMINATOR");
        }

        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            toPaddle.Energy = Math.Max(100, toPaddle.Energy); //recharge...
            base.BehaviourAdded(toPaddle, gamestate);
        }

        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            //base.BehaviourRemoved(fromPaddle, gamestate);
            if (mstate != null)
            {
                mstate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
                mstate.ClientObject.ButtonUp -= ClientObject_ButtonUp;
            }
        }


        private void ClientObject_ButtonUp(Object sender, ButtonEventArgs<bool> e)
        {
            if (e.Button == ButtonConstants.Button_Shift) Deflection = false;
            if (e.Button == ButtonConstants.Button_B)
            {
                if (MachineGunTimer != null)
                    MachineGunTimer.Dispose();


                ButtonBPressed = false;
            }
        }
        private bool Deflection = false;
        private void ClientObject_ButtonDown(Object sender, ButtonEventArgs<bool> e)
        {
            //make sure we exist!

            Deflection=(e.Button == ButtonConstants.Button_Shift);
            if (mstate.PlayerPaddle != null && !mstate.PlayerPaddle.Behaviours.Contains(this))
            {
                //unhook
                mstate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
                mstate.ClientObject.ButtonUp -= ClientObject_ButtonUp;
                return;
            }
            //don't shoot if we are not in the "run" state.
            if (mstate.ClientObject.ActiveState is StateRunning)
            {
                if (e.Button == ButtonConstants.Button_B)
                {
                    if (Allowcharge)
                    {
                        //button down waits until buttonup, using the charge amount.
                        ThreadStart threadfunction = () =>
                                                         {
                                                             int chargeticks = 0;
                                                             while (ButtonBPressed)
                                                             {
                                                                 Thread.Sleep(100);
                                                                 chargeticks++;
                                                             }
                                                             Shoot(chargeticks);
                                                         };

                        ChargeWatcher = new Thread(threadfunction);
                        ChargeWatcher.Start();
                    }
                    else
                    {
                        ButtonBPressed = true;

                        Shoot(0);
                    }
                }
            }
        }

        private void MachineGun(Object param)
        {
            if (mstate.ClientObject.ActiveState is StateRunning) //ignore if not playing.
            {
                if (!ButtonBPressed) return;
                Shoot(1);
            }
            else
            {
                //in fact, disable if not playing.
                MachineGunTimer.Dispose();
            }
        }

        public virtual String GetDescription()
        {
            return getPowerLevelDescription(this.PowerLevel);
        }

        /// <summary>
        /// returns a text description for the given power level.
        /// </summary>
        /// <param name="powerlevel">powerlevel value to get the description of.</param>
        /// <returns></returns>
        public static string getPowerLevelDescription(int PowerLevel)
        {
            String buildresult = "";
            if (PowerLevel > PowerLevel*2) PowerLevel = PowerLevel*2;
            int pPowerLevel = ((PowerLevel - 1)%(MaxPowerup)) + 1;

            switch (pPowerLevel)
            {
                case 1:
                    buildresult = "Gun";
                    break;
                case 2:
                    buildresult = "Ball Shooter";
                    break;
                case 3:
                    buildresult = "Dual Ball Shooter";
                    break;
                case 4:
                    buildresult = "hitscan Pistol";
                    break;
                case 5:
                    buildresult = "shotgun";
                    break;
                case 6:
                    buildresult = "Laser";
                    break;
                case 7:
                    buildresult = "Dual Laser";
                    break;
                case 8:
                    buildresult = "Laser Ball Shot";
                    break;
                case 9:
                    buildresult = "Dual Laser Ball Shot";
                    break;
                case 10:
                    buildresult = "SpinShot";
                    break;
            }
            if (PowerLevel > MaxPowerup)
                buildresult = "Machine " + buildresult;

            return buildresult;
        }

        private void Shoot(int ChargeTicks)
        {
            mstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => ShootFunction(ChargeTicks)));
        }

        private void ShootFunction(int chargeticks)
        {
            // TerminatorBehaviour Power Levels
            // Each Terminator Power up will add one to the power.
            //Power Level One: Machine gun
            // Power level Two: Shoot a single ball from the center of the paddle upwards.
            // Power Level Three: similar to Level one, but it shoots from each side of the paddle.
            //power level Four: shoots a single laser from the center of the paddle.
            //power level Five: shoots a laser from each side.
            // power level Six: Shoot a single ball from the center of the paddle upwards: however,
            //this will have a laserspinball behaviour, delay set to half a second.
            //power level Seven, same as five, but with laser spin
            //power level eight: SpinShot.


            if (mstate.PlayerPaddle == null) return;

            //only shoot if we are the first in the list of behaviours that are this type....
            if (!mstate.PlayerPaddle.Behaviours.Contains(this)) return;
            if (mstate.PlayerPaddle.Behaviours.First((x) => x.GetType() == typeof (TerminatorBehaviour)) == this)
            {
                if (mstate.PlayerPaddle.Energy == 0)
                {
                    BCBlockGameState.Soundman.PlaySound("shootfail");
                    //and, again, showmanship at work- "shoot" some black light orbs... to simulate smoke...

                    for (int i = 0; i < 6; i++)
                    {
                        PointF usespeed = new PointF(0, (float) (-3 - (BCBlockGameState.rgen.NextDouble())));
                        usespeed = BCBlockGameState.VaryVelocity(usespeed, Math.PI/6);
                        LightOrb addorb = new LightOrb(mstate.PlayerPaddle.Getrect().CenterPoint(),
                                                       BCBlockGameState.rgen.NextDouble() > 0.5d
                                                           ? Color.Black
                                                           : Color.Gray,
                                                       15 + (float) (15*BCBlockGameState.rgen.NextDouble()));
                        addorb.Velocity = usespeed;
                        addorb.VelocityDecay = new PointF(0.9f, 0.85f);
                        mstate.Particles.Add(addorb);
                        //take damage, too...
                    }
                    mstate.PlayerPaddle.HP -= 10;
                    return;
                }
                else
                {
                    BCBlockGameState.Soundman.PlaySound("firelaser");
                }
                mstate.PlayerPaddle.Energy--;
                Paddle pad = mstate.PlayerPaddle;
                RectangleF paddlerect = mstate.PlayerPaddle.Getrect();
                PointF MiddleTop = new PointF(paddlerect.Left + (paddlerect.Width/2), paddlerect.Top);
                PointF LeftTop = new PointF(paddlerect.Left, paddlerect.Top);
                PointF RightTop = new PointF(paddlerect.Right, paddlerect.Top);
                Pen ChosenPenColor = new Pen(Color.Blue, 2);
                //TODO: make it depend on the paddle health. (blue when full, red when damaged).


                //if the powerlevel exceeds MaxPowerup...
                if (PowerLevel > MaxPowerup)
                {
                    MachineGunTimer = new Timer(MachineGun, null, 60, 130); //120 ms delay...
                }
                //if it's larger than powerLevel*2, set it to powerLevel*2...
                if (PowerLevel > PowerLevel*2) PowerLevel = PowerLevel*2;
                int pPowerLevel = ((PowerLevel - 1)%(MaxPowerup)) + 1;
                //Power Level One: Machine gun


                if (pPowerLevel == 1)
                {
                    if (PowerLevel == 1)
                    {
                        for (int i = 0; i < 5; i++)
                        {
                            Bullet firebullet =
                                new Bullet(
                                    new PointF(
                                        mstate.PlayerPaddle.Getrect().Left + (mstate.PlayerPaddle.Getrect().Width/2),
                                        mstate.PlayerPaddle.Getrect().Top),
                                    BCBlockGameState.VaryVelocity(new PointF(0, -10), Math.PI/7));
                            mstate.GameObjects.AddLast(firebullet);
                        }
                    }
                    else
                    {
                        Bullet firebullet =
                            new Bullet(
                                new PointF(
                                    mstate.PlayerPaddle.Getrect().Left + (mstate.PlayerPaddle.Getrect().Width/2),
                                    mstate.PlayerPaddle.Getrect().Top),
                                BCBlockGameState.VaryVelocity(new PointF(0, -10), Math.PI/7));
                        mstate.GameObjects.AddLast(firebullet);
                    }
                }
                else if (pPowerLevel == 2)
                {
                    // Power level Two: Shoot a single ball from the center of the paddle upwards. (temp)
                    cBall shootball = new cBall(MiddleTop,
                                                BCBlockGameState.VaryVelocity(new PointF(0, -8), Math.PI/8));
                    shootball.Radius = 3;
                    shootball.DrawColor = Color.Yellow;
                    shootball.DrawPen = new Pen(Color.GreenYellow);
                    shootball.Behaviours.Add(new TempBallBehaviour());
                    mstate.ShootBalls.AddRange(new cBall[] {shootball});
                }
                else if (pPowerLevel == 3)
                {
                    // Power Level Three: similar to Level one, but it shoots from each side of the paddle.
                    PointF[] Originspots = new PointF[] {LeftTop, RightTop};
                    foreach (PointF ShotOrigin in Originspots)
                    {
                        cBall shootit = new cBall(ShotOrigin,
                                                  BCBlockGameState.VaryVelocity(new PointF(0f, -8.8f), Math.PI/8.8f));
                        shootit.Radius = 3;
                        shootit.DrawColor = Color.Yellow;
                        shootit.DrawPen = new Pen(Color.RoyalBlue);
                        shootit.Behaviours.Add(new TempBallBehaviour());
                        mstate.ShootBalls.Add(shootit);
                    }
                }
                else if (pPowerLevel == 4)
                {
                    //weak hitscan, straight upwards.
                    HitscanBullet hsb = new HitscanBullet(paddlerect.TopCenter(), new PointF(0, -2));
                    hsb.Penetrate = false;
                    hsb.Strength = HitscanBullet.HitscanStrengthConstants.hitscan_bullet;
                    mstate.GameObjects.AddLast(hsb);
                    BCBlockGameState.Soundman.PlaySound("laser2");
                }
                else if (pPowerLevel == 5)
                {
                    for (int i = 0; i < 5; i++)
                    {
                        PointF selectedvelocity = BCBlockGameState.VaryVelocity(new PointF(0, -8), Math.PI/16);
                        HitscanBullet hsb = new HitscanBullet(paddlerect.TopCenter(), selectedvelocity);
                        hsb.Penetrate = false;
                        hsb.Strength = HitscanBullet.HitscanStrengthConstants.hitscan_bullet;
                        mstate.GameObjects.AddLast(hsb);
                        BCBlockGameState.Soundman.PlaySound("laser2");
                    }
                }
                else if (pPowerLevel == 6)
                {
                    //power level Four: shoots a single laser from the center of the paddle.
                    LaserShot ShootLaser = new LaserShot(MiddleTop, new PointF(0, -9), ChosenPenColor, 36);
                    mstate.GameObjects.AddLast(ShootLaser);
                }
                else if (pPowerLevel == 7)
                {
                    PointF[] Originspots = new PointF[] {LeftTop, RightTop};
                    PointF usevelocity = BCBlockGameState.VaryVelocity(new PointF(0f, -8.8f), Math.PI/8.9f);
                    foreach (PointF ShotOrigin in Originspots)
                    {
                        LaserShot ShootLaser = new LaserShot(ShotOrigin, usevelocity, ChosenPenColor);
                        mstate.GameObjects.AddLast(ShootLaser);
                        //power level Five: shoots a laser from each side.
                    }
                }
                else if (pPowerLevel == 8)
                {
                    cBall shootball = new cBall(MiddleTop,
                                                BCBlockGameState.VaryVelocity(new PointF(0f, -1f), Math.PI/10));
                    shootball.Behaviours.Add(new LaserSpinBehaviour(new TimeSpan(0, 0, 0, 0, 300)));
                    shootball.Behaviours.Add(new TempBallBehaviour());
                    shootball.Radius = 3;
                    shootball.DrawColor = Color.Yellow;
                    shootball.DrawPen = new Pen(Color.GreenYellow);
                    mstate.ShootBalls.Add(shootball);
                    // power level Six: Shoot a single ball from the center of the paddle upwards: however,
                    //this will have a laserspinball behaviour, delay set to half a second.
                }
                else if (pPowerLevel == 9)
                {
                    //power level Seven, same as five, but with laser spin
                    //also same as 6 but with two of them...
                    PointF[] Originspots = new PointF[] {LeftTop, RightTop};
                    foreach (PointF ShotOrigin in Originspots)
                    {
                        cBall shootball = new cBall(ShotOrigin,
                                                    BCBlockGameState.VaryVelocity(new PointF(0f, -9f), Math.PI/10));
                        shootball.Behaviours.Add(new LaserSpinBehaviour(new TimeSpan(0, 0, 0, 300)));
                        shootball.Behaviours.Add(new TempBallBehaviour());
                        shootball.Radius = 3;
                        shootball.DrawColor = Color.Yellow;
                        shootball.DrawPen = new Pen(Color.GreenYellow);
                        mstate.ShootBalls.Add(shootball);
                    }
                }
                else if (pPowerLevel == 10)
                {
                    //spinshot
                    PointF selectedspeed = BCBlockGameState.VaryVelocity(new PointF(0, -9f), Math.PI/10);
                    SpinShot shootit = new SpinShot(mstate,
                                                    new PointF(
                                                        mstate.PlayerPaddle.Getrect().Left +
                                                        (mstate.PlayerPaddle.Getrect().Width/2),
                                                        mstate.PlayerPaddle.Getrect().Top), 12, 16,
                                                    ((float) Math.PI/6), selectedspeed);

                    mstate.GameObjects.AddLast(shootit);
                }
            }
            else
            {
                Debug.Print("Terminator Behaviour not first Terminator in behaviours collection... ignoring...");
            }
        }

        #region iPaddleBehaviour Members

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            PointF Position = onPaddle.Position;
            SizeF PaddleSize = onPaddle.PaddleSize;
            RectangleF drawrect = new RectangleF(Position.X - PaddleSize.Width/2, Position.Y - PaddleSize.Height/2,
                                                 PaddleSize.Width, PaddleSize.Height);
            //g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Green)), drawrect);
            //draw the "sticky" overlay...
            g.DrawImage(BCBlockGameState.Imageman.getLoadedImage("Terminator"), drawrect);
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            //throw new NotImplementedException();
            return false;
        }


        public override void calcball(Paddle onPaddle, cBall withball)
        {
            base.calcball(onPaddle, withball);
        }

        #endregion
    }
}