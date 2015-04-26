using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using BASeBlock.GameObjects.Orbs;
using BASeBlock.GameStates;

namespace BASeBlock.PaddleBehaviours
{
    public abstract class BasePaddleBehaviour : iPaddleBehaviour
    {
        /// <summary>
        /// array of Type arrays; each one lists a set of behaviours that are mutually exclusive.
        /// </summary>
        public static Type[][] MutuallyExclusives = new Type[][]
                                                        {
                                                            new Type[]
                                                                {
                                                                    typeof (BaseTerminatorBehaviour),
                                                                    typeof (TerminatorBehaviour),
                                                                    typeof (BuilderShotBehaviour)
                                                                },
                                                            new Type[] {typeof (BaseTerminatorBehaviour)}
                                                        }; //all base terminator behaviours are mutually exclusive.

        //given a type, checks the MutuallyExclusives array and creates
        //a listing of all the types that are mutually exclusive to that type in a single array.

        private Thread WatchDogThread = null;

        protected Paddle attachedPaddle;
        protected BCBlockGameState attachedstate;
        private bool mInitialized = false;

        #region iPaddleBehaviour Members

        private BCBlockGameState usestate = null;
        public abstract Image GetIcon();
        public abstract void Draw(Paddle onPaddle, Graphics g);

        public abstract bool Impact(Paddle onPaddle, cBall withBall);

        public virtual void calcball(Paddle onPaddle, cBall withball)
        {
            //throw new NotImplementedException();
        }

        public virtual bool getMacGuffin(BCBlockGameState gstate,Paddle onPaddle, CollectibleOrb collected)
        {
            return false; //default to nothing.
            //
        }

        public virtual bool getPowerup(BCBlockGameState gstate,Paddle onPaddle, GamePowerUp gpower)
        {
            return false; 
            // default to doing nothing.
        }

        public virtual bool RequiresPerformFrame(BCBlockGameState gamestate, Paddle withpaddle)
        {
            return !mInitialized;
            //default behaviour only needs the first "performframe" (for "firing" the behaviouradded method)
        }

        public virtual void PerformFrame(BCBlockGameState gamestate, Paddle pPaddle)
        {
            if (!mInitialized)
            {
                usestate = gamestate;
                mInitialized = true;
                attachedPaddle = pPaddle;
                attachedstate = usestate;
                BehaviourAdded(pPaddle, usestate);
            }
        }
        public virtual string Name
        {
            get
            {
                //name to use...
                return getName();
            }
        }
        public virtual string getName()
        {
            return "BASeBehaviour";
        }

        public virtual void UnHook()
        {
            BehaviourRemoved(attachedPaddle, usestate);
        }

        ~BasePaddleBehaviour()
        {
            BehaviourRemoved(attachedPaddle, attachedstate);
        }


        public virtual void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            gamestate.Forcerefresh = true;

            //tweak: BasePaddleBehaviour will have a low priority thread that checks periodically to make sure this behaviour is actually valid.
            //we can't use PerformFrame, because once we are removed that won't be called again.
            if (WatchDogThread == null)
            {
                //initialize the watchdog thread. We'll put in inline (the method, that is) for now.
                //if it get's bigger we'll refactor it out.
                WatchDogThread = new Thread(() =>
                                                {
                                                    while (true)
                                                    {
                                                        bool isvisible = (gamestate.ClientObject is Form &&
                                                                          !(gamestate.ClientObject as Form)
                                                                               .IsDisposed);
                                                        Thread.Sleep(250);
                                                        if (gamestate.PlayerPaddle != null)
                                                        {
                                                            if (!gamestate.PlayerPaddle.Behaviours.Contains(this))
                                                            {
                                                                //we aren't there...
                                                                //raise the event and break.
                                                                UnHook();
                                                                break;
                                                            }
                                                        }
                                                        if ((gamestate.ClientObject as Form).IsDisposed || gamestate.ClientObject.ActiveState is StateNotRunning)
                                                        {
                                                                
                                                            break;
                                                        }
                                                    }
                                                });

                WatchDogThread.Priority = ThreadPriority.BelowNormal;
                WatchDogThread.Start();
            }
        }

        public virtual void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            //also... no code.
        }

        #endregion

        public static IEnumerable<Type> GetExclusivityForType(Type TestType)
        {
            List<Type> buildlist = new List<Type>();


            foreach (var iterate in MutuallyExclusives)
            {
                if (iterate.Any((w) => w == TestType || TestType.IsSubclassOf(w)))
                {
                    foreach (var addthis in iterate)
                    {
                        if (!buildlist.Contains(addthis) && addthis != TestType)
                        {
                            buildlist.Add(addthis);
                            yield return addthis;
                        }
                    }
                }
            }
        }
    }
}