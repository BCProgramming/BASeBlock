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
using System.IO;
using System.Linq;
using System.Text;
using BASeBlock.Blocks;
using BASeBlock.Events;
using BASeBlock.PaddleBehaviours;
using BASeBlock.Particles;
using Microsoft.Win32.SafeHandles;

namespace BASeBlock
{
   


    public class Paddle : Block
    {
        public static float dangerzone = 35;
        public List<iPaddleBehaviour> Behaviours = new List<iPaddleBehaviour>();
        public bool Interactive = true; //whether it responds to keys and whatnot...
        private float MaximumHP = 200;
        private List<iPaddleBehaviour> PrevFrameBehaviours = null;

        /// <summary>
        /// Energy is used by a few paddlebehaviours. There will be a item that recharges energy, and possibly other ways (such as simply destroying blocks to recover small bits of energy).
        /// Also, getting a powerup that adds a behaviour that uses Energy will recharge the energy to full. Note that more Powerups means more drain, as well (unless they use different keys to activate...)
        /// 
        /// </summary>
        private float _Energy = 0; //starts at 0...

        private float _HP = 100;
        private SizeF _PaddleSize;
        private PointF _Position;
        private float damagemodulus = 0;
        
        public WeakReference wGameState;


        public Paddle(BCBlockGameState stateobject, Size pPaddlesize, PointF pPosition, Image pPaddleImage)
        {
            PaddleSize = pPaddlesize;
            Position = pPosition;
            PaddleImage = pPaddleImage;
            wGameState = new WeakReference(stateobject);
            HP = 100;
            stateobject.ClientObject.ButtonDown += ClientObject_ButtonDown;
            stateobject.ClientObject.ButtonUp += ClientObject_ButtonUp;
        }
        private bool hasdeflection()
        {

            return Behaviours.Any((b) => b is DeflectorBehaviour);

        }
        void ClientObject_ButtonUp(object sender, ButtonEventArgs<bool> e)
        {
            //throw new NotImplementedException();
            if (e.Button == ButtonConstants.Button_Shift && hasdeflection())
            {
                //remove the deflector behaviour.
                BCBlockGameState.Soundman.PlaySound("deflectoroff");
                Behaviours.RemoveAll((b) => b is DeflectorBehaviour);
            }
        }

        void ClientObject_ButtonDown(object sender, ButtonEventArgs<bool> e)
        {
            //detect shift being depressed. emit deflectoron and deflectoroff
            if (e.Button == ButtonConstants.Button_Shift && !hasdeflection())
            {
                //add the deflector behaviour.
                BCBlockGameState.Soundman.PlaySound("deflectoron");
                (wGameState.Target as BCBlockGameState).NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(()=>
                Behaviours.Add(new DeflectorBehaviour())));
            }

            //throw new NotImplementedException();
        }

        public SizeF PaddleSize
        {
            get { return _PaddleSize; }
            set
            {
                _PaddleSize = value;
                setBlockRect();
            }
        }

        public PointF Position
        {
            get { return _Position; }
            set
            {
                _Position = value;
                setBlockRect();
            }
        }


        public Image PaddleImage { get; set; }

        public float Energy
        {
            get { return _Energy; }
            set
            {
                var oldvalue = _Energy;
                _Energy = value;
                if (InvokeEnergyChange(oldvalue, ref _Energy))
                    _Energy = oldvalue;
            }
        }

        public float HP
        {
            get { return _HP; }


            set
            {
                float oldvalue = _HP;
                _HP = Math.Min(value, MaximumHP);
                if (InvokeHPChange(oldvalue, ref _HP))
                    _HP = oldvalue;

                float HPDiff = -((oldvalue - _HP) + damagemodulus);
                if (HPDiff < -1)
                {
                    BCBlockGameState.Soundman.PlaySound("SHOOTFAIL", 1.0f);
                    damagemodulus = 0;
                    BCBlockGameState gstate = wGameState.Target as BCBlockGameState;
                    if (gstate != null)
                    {
                        int getvalue = Math.Abs((int) HPDiff);
                        gstate.SpawnRisingText(getvalue.ToString(), Getrect().CenterPoint(),
                                               new SolidBrush(Color.FromArgb(190, Color.Red)));
                    }
                }
                else
                {
                    damagemodulus += HPDiff;
                }


                if (_HP < dangerzone)
                {
                    if (!Behaviours.Any((w) => w is BloodDripBehaviour))
                    {
                        Behaviours.Add(new BloodDripBehaviour());
                    }
                }
                else
                {
                    Behaviours.RemoveAll((w) => w is BloodDripBehaviour);
                }
            }
        }
       
        //public event Func<Paddle, float, float, bool> OnHPChange;
        public event EventHandler<PaddleElementChangeEventArgs<float>> OnHPChange; 
            //previous HP, new HP, and a bool to indicate whether to allow the change.
        public event EventHandler<PaddleElementChangeEventArgs<float>> OnEnergyChange; 
        
        //public RectangleF paddlerect { get; set; }
        public event Func<Paddle, bool> OnDeath;
        public event Action<iPaddleBehaviour> BehaviourAdded;
        public event Action<iPaddleBehaviour> BehaviourRemoved;

        private void setBlockRect()
        {
            BlockRectangle = new RectangleF(_Position.X, _Position.Y, _PaddleSize.Width, _PaddleSize.Height);
        }

        public void InvokeOnDeath()
        {
            Func<Paddle, bool> deathev = OnDeath;
            if (deathev != null)
            {
                deathev.Invoke(this);
            }
        }

        private bool InvokeEnergyChange(float oldvalue,ref float newValue)
        {
            var clone = OnEnergyChange;
            if (clone != null)
            {
                var create = new PaddleElementChangeEventArgs<float>(this,oldvalue,newValue);
                clone(this, create);
                newValue = create.NewValue;
                return create.Cancel;
            }
            return false;
        }
        //returns true to cancel change. False otherwise.
        private bool InvokeHPChange(float oldHP,ref float newHP)
        {
            var clone = OnHPChange;
            if (clone != null)
            {
                
                var create = new PaddleElementChangeEventArgs<float>(this, oldHP,newHP);
                clone(this, create);
                newHP=create.NewValue;
                return create.Cancel;
            }

            return false;
        }


        public Rectangle Getrect()
        {
            return new Rectangle((int) (Position.X - PaddleSize.Width/2), (int) (Position.Y - PaddleSize.Height/2),
                                 (int) PaddleSize.Width, (int) PaddleSize.Height);
        }

        private void CallDraw(Graphics g)
        {
            foreach (iPaddleBehaviour loopbeh in Behaviours)
            {
                loopbeh.Draw(this, g);
            }
        }

        private bool CallImpact(BCBlockGameState gamestate, cBall withBall)
        {
            return Behaviours.Aggregate(false, (current, loopbeh) => current | loopbeh.Impact(this, withBall));
        }

        private void Callcalcball(cBall withball)
        {
            List<iPaddleBehaviour> copylist = Behaviours.ShallowClone();


            foreach (iPaddleBehaviour loopbeh in copylist)
            {
                loopbeh.calcball(this, withball);
            }
        }



        /// <summary>
        /// retrieves a listing of all the paddlebehaviours that are "exclusive" to the given type.
        /// For example, BuilderShot is exclusive to a terminator.
        /// </summary>
        /// <param name="newbehaviour"></param>
        /// <returns></returns>
        public IEnumerable<iPaddleBehaviour> GetExclusiveBehaviours(Type newbehaviour)
        {
            if (newbehaviour == typeof (TerminatorPelletBehaviour))
            {
                Debug.Print("pellet");
            }
            //first get a listing of all the types exclusive to the given type.
            List<Type> Exclusives = new List<Type>(BasePaddleBehaviour.GetExclusivityForType(newbehaviour));
            //iterate through our behaviours, yield return instances whose type is in the list or that are subclasses of a type in the list.
            return Behaviours.Where(c => Exclusives.Any((ex) => ex == c.GetType() ||
                                                                c.GetType().IsSubclassOf(ex)));
            //return Behaviours.Where(exclusive => Exclusives.Any((w)=>w == exclusive.GetType() || w.IsSubclassOf(exclusive.GetType())));
        }

        public IEnumerable<Type> GetActiveBehaviourTypes()
        {
            List<Type> acquiredtypes = new List<Type>();
            foreach (var iterate in Behaviours)
            {
                Type grabtype = iterate.GetType();
                if (!acquiredtypes.Contains(grabtype))
                {
                    acquiredtypes.Add(grabtype);
                    yield return grabtype;
                }
            }
        }

        /// <summary>
        /// compares PrevFrameBehaviours to what we have now and fires remove and add events as needed.
        /// </summary>
        private void CheckBehaviours()
        {
            //if prevframebehaviours is null, all Behaviours are new.
            var Addedbeh = BehaviourAdded;
            var removedbeh = BehaviourRemoved;
            if (Addedbeh == null && removedbeh == null) return; //no need to invoke if there aren't any sinks...
            if (Addedbeh != null)
            {
                if (PrevFrameBehaviours == null)
                {
                    foreach (var loopbeh in Behaviours)
                    {
                        Addedbeh.Invoke(loopbeh);
                    }

                    return;
                }

                //items that are added exist in current list, but not in the old one.


                foreach (var qq in (from added in Behaviours where !PrevFrameBehaviours.Contains(added) select added))
                {
                    Addedbeh.Invoke(qq);
                }
            }
            //items that are removed are in the old list, but not the new one.


            if (removedbeh == null)
            {
                foreach (
                    var qq in (from removed in PrevFrameBehaviours where !Behaviours.Contains(removed) select removed))
                {
                    removedbeh.Invoke(qq);
                }
            }
        }

        public override void Draw(Graphics g)
        {
            List<iPaddleBehaviour> behaveremove = new List<iPaddleBehaviour>();
            lock (Behaviours)
            {
                CheckBehaviours();
                try
                {
                    foreach (
                        var loopbeh in
                            (from sp in Behaviours
                             where sp.RequiresPerformFrame((BCBlockGameState) wGameState.Target, this)
                             select sp))
                    {
                        loopbeh.PerformFrame((BCBlockGameState) wGameState.Target, this);
                    }
                }
                catch
                {
                }


                PrevFrameBehaviours = Behaviours.ShallowClone();
            }
            RectangleF drawrect = new RectangleF(Position.X - PaddleSize.Width/2, Position.Y - PaddleSize.Height/2,
                                                 PaddleSize.Width, PaddleSize.Height);
            g.DrawImage(PaddleImage, drawrect);
            CallDraw(g);
        }

        public bool CheckImpact(BCBlockGameState gamestate, cBall withball, bool CallImpact)
        {
            Block.BallRelativeConstants brc;
            //determines if the given ball impacted this paddle.
            if (BCBlockGameState.CheckImpact(gamestate, Getrect(), withball, out brc))
            {
                if (CallImpact)
                {
                    Impact(gamestate, withball);
                }
                return true;
            }
            return false;
        }

        public virtual void Impact(BCBlockGameState gamestate, cBall withBall)
        {
            Impact(gamestate, withBall, BallRelative(BlockRectangle, withBall));
        }

        public virtual void Impact(BCBlockGameState gamestate, cBall withBall, Block.BallRelativeConstants brc)
        {
            var oldLocation = withBall.Location;
            var oldVelocity = withBall.Velocity;
            RectangleF prect = Getrect();
            //BCBlockGameState.Soundman.PlaySound("bounce",0.9f);
            //withBall.Velocity = new PointF(withBall.Velocity.X,-Math.Abs(withBall.Velocity.Y));
            if (!withBall.Behaviours.Any((w) => w is NonReboundableBallBehaviour))
            {
                //it hurts the paddle now
                HP--;
                    //one tick. THe idea is that this makes it necessary to keep grabbing health power ups and the health orbs.
                withBall.Velocity = new PointF(withBall.Velocity.X*1.025f, withBall.Velocity.Y*1.025f);
                    //to make it interesting...
                switch (brc)
                {
                    case Block.BallRelativeConstants.Relative_Up:
                        //move to the top sans the radius of the ball...
                        withBall.Location = new PointF(withBall.Location.X, prect.Top - withBall.Radius);
                        //velocity to move up...
                        withBall.Velocity = new PointF(withBall.Velocity.X, -Math.Abs(withBall.Velocity.Y));
                        break;
                    case Block.BallRelativeConstants.Relative_Left:
                        //left sans radius.
                        withBall.Location = new PointF(prect.Left - withBall.Radius, withBall.Location.Y);
                        withBall.Velocity = new PointF(-Math.Abs(withBall.Velocity.X), withBall.Velocity.Y);

                        break;
                    case Block.BallRelativeConstants.Relative_Right:
                        //right + radius
                        withBall.Location = new PointF(prect.Right + withBall.Radius, withBall.Location.Y);
                        withBall.Velocity = new PointF(Math.Abs(withBall.Velocity.X), withBall.Velocity.Y);

                        break;
                    case Block.BallRelativeConstants.Relative_Down:
                        //bottom + radius. Not sure how this could happen but stranger things have occured.
                        withBall.Location = new PointF(withBall.Location.X, prect.Bottom + withBall.Radius);
                        withBall.Velocity = new PointF(withBall.Velocity.X, Math.Abs(withBall.Velocity.Y));
                        break;
                }

                Particle[] paddleparticles = GeneratePaddleParticles(gamestate, withBall);

                gamestate.Particles.AddRange(paddleparticles);

                calcball(withBall);
                if (CallImpact(gamestate, withBall))
                {
                    withBall.Velocity = oldVelocity;
                    withBall.Location = oldLocation;
                }
            }
        }

        private Particle paddleparticleroutine(BCBlockGameState gstate, EmitterParticle emitter, int TTL, int TimeLive)
        {
            int redvalue = BCBlockGameState.rgen.Next(0, 255);
            Color usecolor = Color.FromArgb(redvalue, 0, 0);
            if ((emitter.Tag != null) && emitter.Tag.GetType() == typeof (cBall))
            {
                cBall resultit = ((cBall) emitter.Tag);
                //usecolor = resultit.DrawColor;
            }

            DustParticle dusty = new DustParticle(emitter.Location, 3, 5, usecolor);

            return dusty;
        }

        protected new Particle AddSprayParticle_Default(BCBlockGameState parentstate, cBall ballhit)
        {
            PointF middlespot;
            middlespot = ballhit.Location;

            //return ballhit != null ? new DustParticle(ballhit) : new DustParticle(middlespot, 3);
            EmitterParticle CreatedEmitter = ballhit != null
                                                 ? new EmitterParticle(ballhit, paddleparticleroutine)
                                                 : new EmitterParticle(middlespot, paddleparticleroutine);
            CreatedEmitter.Tag = ballhit;

            CreatedEmitter.VelocityDecay = new PointF(0.99f, 0.99f);
            CreatedEmitter.Velocity = BCBlockGameState.GetRandomVelocity(1, 3);

            return CreatedEmitter;
        }

        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            return this; //this could cause problems later...
        }

        private Particle[] GeneratePaddleParticles(BCBlockGameState gamestate, cBall withBall)
        {
            //gamestate.Particles.AddRange(new Particle[] { new DustParticle(withBall.Location), new DustParticle(withBall.Location), new DustParticle(withBall.Location) });
            Particle[] CreateParticles = new Particle[5];
            for (int i = 0; i < 5; i++)
            {
                EmitterParticle ep = new EmitterParticle(withBall, paddleparticleroutine);

                CreateParticles[i] = ep;
            }
            return CreateParticles;
        }

        /// <summary>
        /// Procedure used to change the direction of the ball.
        /// </summary>
        /// <param name="withball"></param>
        protected virtual void calcball(cBall withball)
        {
            //should re-traject the ball along a new path.

            float currspeed = withball.getMagnitude();
            //add a smidgen...
            currspeed += 0.01f;
            float distance = (withball.Location.X) - (Position.X);
            float percent = distance/(PaddleSize.Width/2);

            PointF anglecalcorigin = new PointF(Position.X, Position.Y + PaddleSize.Width + PaddleSize.Height);

            //calculate angle between anglecalcorigin and the ball.
            double calculatedangle = BCBlockGameState.GetAngle(anglecalcorigin, withball.Location);


            //make it go in that direction.

            withball.Velocity = new PointF((float) (Math.Cos(calculatedangle)*currspeed),
                                           (float) (Math.Sin(calculatedangle)*currspeed));


            //MessageBox.Show("Position.X=" + Position.X.ToString() + "paddlesize.Width=" + PaddleSize.Width.ToString() + " withball.Location.X=" + withball.Location.X.ToString() + " distance:" + distance.ToString() + " percent:" + percent.ToString());

            //withball.Velocity = new PointF(withball.Velocity.X+(percent),withball.Velocity.Y);


            Callcalcball(withball);
        }
    }
}