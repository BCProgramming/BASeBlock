using System;
using System.Diagnostics;
using System.Drawing;

namespace BASeBlock.PaddleBehaviours
{
    /// <summary>
    /// class used to resize the paddle when a grow or shrink "item" is retrieved.
    /// it does so given two pieces of data: the ending size, and the amount of time it should take to do so. (A TimeSpan).
    /// </summary>
    public class SizeChangeBehaviour : BasePaddleBehaviour
    {
        private static int MinimumWidth = 13;
        public SizeF _EndingSize;
        public TimeSpan _Growlength;
        private DateTime? _Starttime; //performframe initializes this..
        private SizeF initialsize;
        private PaddleSizeChangeBehaviourObject ourproxy = null;
        private Paddle ownerpaddle;

        public SizeChangeBehaviour(SizeF EndingSize, TimeSpan growlength)
        {
            _EndingSize = EndingSize;
            _Growlength = growlength;
        }

        public override Image GetIcon()
        {
            return null; //doesn't have an icon.
        }

        protected bool PerformFrame(BCBlockGameState gamestate)
        {
            Debug.Print("performFrame on SizeChangeBehaviour Called");
            //called by the GameObject we add in BehaviourAdded...
            //if _Starttime is not initialized- initialize it...
            if (_Starttime == null)
            {
                _Starttime = DateTime.Now;
                return false;
            }
            else
            {
                //otherwise, calculate the difference (in a timespan...) between now and the initial time..
                TimeSpan timediff = DateTime.Now - _Starttime.Value;
                //if the timedifference exceeds growlength, 
                if (timediff >= _Growlength)
                {
                    Debug.Print("SizeChangeBehaviour Time exceeded...");
                    //change ownerpaddle's size to the ending size and remove our proxy
                    //object from the gameobjects list, and ourself from the paddle's behaviour list.
                    ownerpaddle.PaddleSize = _EndingSize;
                    gamestate.Defer(() => gamestate.GameObjects.Remove(ourproxy));

                    //remove ourself...
                    ownerpaddle.Behaviours.Remove(this);
                    return false;
                }
                else
                {
                    //what percentage are we?
                    float percent = ((float) timediff.TotalMilliseconds/((float) _Growlength.TotalMilliseconds));
                    //using the percent, create a sizeF value  that is that percentage between the two sizeF's
                    SizeF difference = _EndingSize - initialsize;
                    SizeF diffpercent = new SizeF(difference.Width*percent, difference.Height*percent);
                    //and lastly, add that diff to the existing size...
                    ownerpaddle.PaddleSize = new SizeF(initialsize.Width + diffpercent.Width,
                                                       initialsize.Height + diffpercent.Height);
                    //make sure it is no smaller than the minimum size.
                    if (ownerpaddle.PaddleSize.Width < MinimumWidth)
                        ownerpaddle.PaddleSize = new SizeF(MinimumWidth, ownerpaddle.PaddleSize.Height);
                    return false;
                }
            }


            return false;
        }

        public override void calcball(Paddle onPaddle, cBall withball)
        {
            base.calcball(onPaddle, withball); //call the base.
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            //throw new NotImplementedException(); //nothing for now, might add effects during the shrink/grow?
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            //base.Impact(onPaddle, withBall);
            //throw new NotImplementedException();
            //again, we don't much care about that..
            return false;
        }

        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            //when the behaviour is added, add our "proxy" item to the gameObject listing, so that we can have a performFrame.

            //logic to prevent multiple instances:
            //if more then one size-based powerup is acquired in a short time frame, two of these behaviours might conflict. So, instead, when
            //this behaviour is added, it will check to see if there is already another SizeChangeBehaviour present that isn't itself, and if so,
            //it will add the difference between the paddles size and our own size to the other one, as well as adding out timespan variable to it.
            Debug.Print("BehaviourAdded: SizeChangeBehaviour");
            bool doremove = false;
            lock (toPaddle.Behaviours)
            {
                foreach (iPaddleBehaviour loopbehaviour in toPaddle.Behaviours)
                {
                    if (loopbehaviour is SizeChangeBehaviour && loopbehaviour != this)
                    {
                        SizeChangeBehaviour scb = loopbehaviour as SizeChangeBehaviour;
                        //found another one... first, calc our differences...
                        float diffx = _EndingSize.Width - toPaddle.PaddleSize.Width;
                        float diffy = _EndingSize.Height - toPaddle.PaddleSize.Height;
                        //add the differences to the other behaviours ending size...
                        scb._EndingSize = new SizeF(scb._EndingSize.Width + diffx, scb._EndingSize.Height + diffy);
                        //also add our timespan...
                        scb._Growlength += _Growlength;
                        // we cannot remove ourself in the loop, so set a flag, break out, and then remove ourself.
                        doremove = true;
                        break;
                    }
                }
            }
            if (doremove)
            {
                toPaddle.Behaviours.Remove(this);

                return;
            }
            ownerpaddle = toPaddle;
            initialsize = ownerpaddle.PaddleSize;
            base.BehaviourAdded(toPaddle, gamestate);
            ourproxy = new PaddleSizeChangeBehaviourObject(this);
            lock (gamestate.GameObjects)
            {
                gamestate.GameObjects.AddLast(ourproxy);
            }
        }

        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            if (ourproxy != null)
            {
                lock (gamestate.GameObjects)
                {
                    gamestate.GameObjects.Remove(ourproxy);
                }
            }
        }

        public override string getName()
        {
            return "SizeChange";
        }

        public class PaddleSizeChangeBehaviourObject : GameObject
        {
            //implementation used so SizeChangeBehaviour can have a "PerformFrame" routine...
            private SizeChangeBehaviour mParent = null;

            public PaddleSizeChangeBehaviourObject(SizeChangeBehaviour parent)
            {
                mParent = parent;
            }

            public override bool PerformFrame(BCBlockGameState gamestate)
            {
                return ourPerformFrame(gamestate);
            }

            public bool ourPerformFrame(BCBlockGameState gamestate)
            {
                Debug.Print("Proxy object PerformFrame called...");
                return mParent.PerformFrame(gamestate);
            }

            public override void Draw(Graphics g)
            {
                //throw new NotImplementedException();
            }
        }
    }
}