using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using BASeBlock.Events;

namespace BASeBlock.PaddleBehaviours
{
    public class StickyBehaviour : BasePaddleBehaviour
    {
        protected Stack<StuckBallData> ballstack = new Stack<StuckBallData>();
        private bool buttonApressed = false;
        public List<cBall> stuckballs = new List<cBall>();


        public StickyBehaviour(BCBlockGameState stateobject)
        {
            //add a hook to the stateobject's targetarea...
            //((PictureBox) stateobject.TargetObject).MouseClick += new MouseEventHandler(StickyBehaviour_MouseClick);
            //((PictureBox)stateobject.TargetObject).MouseClick += new MouseEventHandler(StickyBehaviour_MouseClick);
            stateobject.ClientObject.ButtonDown += ClientObject_ButtonDown;
            stateobject.ClientObject.ButtonUp += ClientObject_ButtonUp;
        }
            
        public override string getName()
        {
            return "Sticky";
        }

        public override Image GetIcon()
        {
            return BCBlockGameState.Imageman.getLoadedImage("Slimepower");
        }

        private void ClientObject_ButtonUp(Object sender, ButtonEventArgs<bool> e)
        {
            //throw new NotImplementedException();
            if (e.Button == ButtonConstants.Button_A)
            {
                buttonApressed = false;
            }
        }

        private void ClientObject_ButtonDown(Object sender, ButtonEventArgs<bool> e)
        {
                
                
                
            if (e.Button==ButtonConstants.Button_A)
            {
                buttonApressed = true;
                Debug.Print("StickyPaddle detected a Button Press");
                //ReleaseAllBalls();
                //only release the top ball... unless shift is pressed...
                if(attachedstate.ClientObject.getPressedButtons().HasFlag(ButtonConstants.Button_Shift))
                    ReleaseAllBalls();
                else
                    ReleaseBall();
            }
        }

        private void ReleaseBall()
        {
            if (ballstack.Count > 0)
            {
                StuckBallData popentry = ballstack.Pop();
                popentry.TheBall.Velocity = popentry.UseVelocity;
                Debug.Print("giving ball back " + popentry.BallsBehaviours.Count.ToString() + " Behaviours");
                //popentry.TheBall.Behaviours = popentry.BallsBehaviours;
                //add back all the behaviours, except for those already present.
                popentry.TheBall.Behaviours.AddRange(from n in popentry.BallsBehaviours
                                                     where !popentry.TheBall.hasBehaviour(n.GetType())
                                                     select n);
                stuckballs.Remove(popentry.TheBall);
            }
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            return false;
            //throw new NotImplementedException();
        }

        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            base.BehaviourRemoved(fromPaddle, gamestate);
            ReleaseAllBalls();
        }


        public void ReleaseAllBalls()
        {
            while (ballstack.Count > 0)
            {
                StuckBallData popentry = ballstack.Pop();
                popentry.TheBall.Velocity = popentry.UseVelocity;
                popentry.TheBall.Behaviours = popentry.BallsBehaviours;
                stuckballs.Remove(popentry.TheBall);
            }
        }

        private void stickball(Paddle onPaddle, cBall stickit)
        {
            //"stick" the given ball to the paddle.
            //how? create a new struct instance representing it's various attributes...
            StuckBallData newentry = new StuckBallData();
            newentry.UseVelocity = stickit.Velocity;
            newentry.TheBall = stickit;
            newentry.MiddleOffset = (stickit.Location.X - onPaddle.Position.X);
            stickit.Velocity = new PointF(0, 0);
            Debug.Print("Taking " + stickit.Behaviours.Count.ToString() + " behaviours from ball");
            newentry.BallsBehaviours = stickit.Behaviours;
            stickit.Behaviours = new List<iBallBehaviour>();

            foreach (iBallBehaviour ballbehave in newentry.BallsBehaviours)
            {
                if (
                    System.Attribute.GetCustomAttributes(ballbehave.GetType())
                          .Any((p) => p is StickyNonRemovableAttribute))
                {
                    //contains the StickyNonRemovableAttribute, and thus should be added to the "temporary" behaviours that it is being given.
                    stickit.Behaviours.Add(ballbehave);
                }
            }

            ballstack.Push(newentry);
            stuckballs.Add(stickit);
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            //base.Draw(g);
            //this is overridden for one purpose: to update the position of the balls that are stuck to the paddle.
            //it would be doable to simply hook the MouseMove routines, but that... well, that's a bit involved, to be fair.
            BCBlockGameState gstate = onPaddle.wGameState.Target as BCBlockGameState;
            foreach (StuckBallData loopentry in ballstack)
            {
                PointF newpos = new PointF(onPaddle.Position.X + loopentry.MiddleOffset,
                                           onPaddle.Position.Y - loopentry.TheBall.Radius*2 -
                                           onPaddle.PaddleSize.Height/2);
                loopentry.TheBall.Location = newpos;
            }
            PointF Position = onPaddle.Position;
            SizeF PaddleSize = onPaddle.PaddleSize;
            RectangleF drawrect = new RectangleF(Position.X - PaddleSize.Width/2, Position.Y - PaddleSize.Height/2,
                                                 PaddleSize.Width, PaddleSize.Height);
            //g.FillRectangle(new SolidBrush(Color.FromArgb(100, Color.Green)), drawrect);
            //draw the "sticky" overlay...
            g.DrawImage(BCBlockGameState.Imageman.getLoadedImage("Sticky"), drawrect);


            //new: now draws the speed vector of the ball that will be released next.
            if (ballstack.Count > 0)
            {
                List<StuckBallData> stuckdraw = new List<StuckBallData>();
                if(gstate!=null && !gstate.ClientObject.getPressedButtons().HasFlag(ButtonConstants.Button_Shift))
                {
                    StuckBallData topstack = ballstack.Peek();
                    stuckdraw.Add(topstack);
                }
                else
                {
                    //if shift is pressed, show the vectors for all of them...
                    stuckdraw.AddRange(ballstack);
                }

                foreach (StuckBallData draweach in stuckdraw)
                {
                    PointF LinePointA = draweach.TheBall.Location;
                    PointF LinePointB = new PointF(LinePointA.X + draweach.UseVelocity.X*3,
                                                   LinePointA.Y + draweach.UseVelocity.Y*3);
                    //create the two points
                    g.DrawLine(new Pen(Color.Red, 3), LinePointA, LinePointB);
                }
            }
        }

        private bool canbestuck(Paddle OnPaddle, cBall checkball)
        {
            return !(checkball.isTempBall) && !(stuckballs.Exists((w) => w == checkball) || buttonApressed);
        }

        public override void calcball(Paddle onPaddle, cBall withball)
        {
            //technique:
            //the assumption I can make pretty easily based on my knowledge of how the game works
            //is that calcball will be called for each ball that impacts the paddle.
            //this is the perfect time to do the following (for the "sticky" effect:
            BCBlockGameState gstate = (BCBlockGameState) onPaddle.wGameState.Target;
            if (gstate != null)
            {
                if (gstate.ClientObject.DemoMode)
                    return;
                //ignore this behaviour if in demo mode...
            }


            //base.calcball(withball);
            //add this ball to our "stuckballs" collection; make sure it's "eligible" first...
            if (canbestuck(onPaddle, withball))
                stickball(onPaddle, withball);
        }

        public struct StuckBallData
        {
            public List<iBallBehaviour> BallsBehaviours;

            /// <summary>
            /// 
            /// </summary>
            public float MiddleOffset;

            /// <summary>
            /// reference to the ball itself. This will be moved with the paddle.
            /// </summary>
            public cBall TheBall;

            /// <summary>
            /// velocity to set to the ball when balls are released.
            /// </summary>
            public PointF UseVelocity;
        }
    }
}