using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using BASeBlock.Events;

namespace BASeBlock.PaddleBehaviours
{
    public class MagnetBehaviour : BasePaddleBehaviour
    {
        private DateTime lastreducetime;
        private bool magnetactivated = false;
        private TimeSpan reducetimeinterval = new TimeSpan(0, 0, 0, 0, 250);

        private BCBlockGameState stateobj = null;

        public MagnetBehaviour(BCBlockGameState stateobject)
        {
            stateobject.ClientObject.ButtonUp += ClientObject_ButtonUp;
            stateobject.ClientObject.ButtonDown += ClientObject_ButtonDown;
        }

        public override string getName()
        {
            return "Magnet";
        }

        public override Image GetIcon()
        {
            return BCBlockGameState.Imageman.getLoadedImage("magnet");
        }

        private void MagnetBalls(BCBlockGameState gstate, Paddle padd, float power)
        {
            var query = from b in padd.Behaviours where b.GetType() == typeof (StickyBehaviour) select b;
            StickyBehaviour tb = null;
            if (query.Count() > 0)
            {
                tb = query.First() as StickyBehaviour;
            }


            //iterate through all balls...
            foreach (cBall loopball in (from b in gstate.Balls
                                        where
                                            b.Location.X > padd.Getrect().Left &&
                                            b.Location.X < padd.Getrect().Right
                                        select b))
            {
                if (tb == null || !tb.stuckballs.Contains(loopball))
                {
                    //if it's null, or the ball is not a stuck ball in the sticky behaviour...
                    //add "power" to it's speed...
                    //loopball.Velocity.Y = loopball.Velocity.Y + power;

                    float currentspeed = loopball.getMagnitude();

                    loopball.Velocity = new PointF(loopball.Velocity.X, loopball.Velocity.Y + power);


                    /*if (loopball.getMagnitude() < currentspeed)
                        {
                            //magnet power can't actually slow down blocks, only direct their movement.
                            double useangle = BCBlockGameState.GetAngle(new PointF(0, 0), loopball.Velocity);
                            loopball.Velocity = BCBlockGameState.GetVelocity(currentspeed, useangle);

                        }*/
                }
            }
        }

        public override void PerformFrame(BCBlockGameState gamestate, Paddle pPaddle)
        {
            stateobj = gamestate;
            base.PerformFrame(gamestate, pPaddle);
            if (magnetactivated)
            {
                //
                MagnetBalls(gamestate, pPaddle, 0.1f);
                if (DateTime.Now - lastreducetime > reducetimeinterval)
                {
                    pPaddle.Energy -= 1;
                }
            }
            if (pPaddle.Energy <= 0)
            {
                gamestate.GameObjects.AddLast(new BehaviourRemoverProxy(pPaddle, this));
            }
        }

        public override bool RequiresPerformFrame(BCBlockGameState gamestate, Paddle withpaddle)
        {
            return true;
        }

        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            toPaddle.Energy = Math.Max(100, toPaddle.Energy); //recharge
        }

        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            gamestate.ClientObject.ButtonUp -= ClientObject_ButtonUp;
            gamestate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
        }

        private void ClientObject_ButtonDown(Object sender, ButtonEventArgs<bool> e)
        {
            //throw new NotImplementedException();
            if (e.Button == ButtonConstants.Button_C)
            {
                magnetactivated = true;
            }
        }

        private void ClientObject_ButtonUp(Object sender, ButtonEventArgs<bool> e)
        {
            if (e.Button == ButtonConstants.Button_C)
            {
                magnetactivated = false;
            }
            //throw new NotImplementedException();
        }

        private ImageAttributes alphada = null;
        public override void Draw(Paddle onPaddle, Graphics g)
        {
            if (onPaddle == null) return;
            RectangleF prect = onPaddle.Getrect();
            if (magnetactivated)
            {
                //"magnetfield.png"
                //change: switch from boring gradient to exciting pathed magnet effect.
                var magnetfield = BCBlockGameState.Imageman.getLoadedImage("magnetfield");

                using (GraphicsPath CreatedPath = new GraphicsPath())
                {
                    CreatedPath.AddRectangle(new RectangleF(prect.Left, 0, prect.Width,
                                                            (float) stateobj.GameArea.Height));
                       
                    using (TextureBrush tb = new TextureBrush(magnetfield, new RectangleF(PointF.Empty,magnetfield.Size)))
                    { 
                            
                        //set offset...
                        var useoffset = (magnetfield.Height / 1000f) * DateTime.Now.Millisecond;
                        //set the offset.
                        //scale...
                        //get the X scale, width of image divided by width of paddle.
                        float xs = (float)onPaddle.Getrect().Width / (float)magnetfield.Width;
                            
                        tb.TranslateTransform(onPaddle.Getrect().Left, useoffset);
                        tb.ScaleTransform(xs, 1, MatrixOrder.Prepend);
                            
                        //draw into the path...
                        g.FillPath(tb,CreatedPath);
                    }
                    
                    //old code below this...
                    //CreatedPath.AddEllipse(new RectangleF(prect.Left, prect.Bottom, prect.Width, prect.Height));
                    /* PathGradientBrush ppg = new PathGradientBrush(CreatedPath);

                        ppg.CenterColor = Color.FromArgb(128, Color.Red);
                        
                        ppg.CenterPoint = onPaddle.Getrect().CenterPoint();
                        ppg.SurroundColors =
                            new Color[]
                                {
                                    Color.FromArgb(64, Color.Red),
                                    Color.FromArgb(64, Color.Pink),
                                    Color.FromArgb(64, Color.Yellow)
                                };
                        g.FillPath(ppg, CreatedPath);*/
                }
            }
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            //throw new NotImplementedException();
            return false;
        }
    }
}