using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using BASeBlock.Events;
using BASeBlock.Particles;

namespace BASeBlock.PaddleBehaviours
{
    /// <summary>
    /// FrozenBehaviour- freezes Paddle in-place
    /// </summary>
    public class FrozenBehaviour : BasePaddleBehaviour
    {
        private const int numpresstobreak = 15;
        private bool DoRemove = false;
        private Paddle OwnerPaddle;
        private List<iPaddleBehaviour> cachebehaviours;
        private int numpressed = 0;
        private BCBlockGameState stateobj;

        public FrozenBehaviour(BCBlockGameState gstate)
        {
            stateobj = gstate;
        }

        public override Image GetIcon()
        {
            return BCBlockGameState.Imageman.getLoadedImage("water1");
        }

        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            //when behaviour is added:
            //Cache current state of the events we nullify...
            OwnerPaddle = toPaddle;
            stateobj = gamestate;
            toPaddle.Interactive = false;
            cachebehaviours = toPaddle.Behaviours;
            cachebehaviours.RemoveAll((w) => (w != this && (w.GetType() == typeof (FrozenBehaviour))));

            //base.BehaviourAdded(toPaddle, gamestate);
            gamestate.ClientObject.ButtonDown += ClientObject_ButtonDown;
            //queue a message...
            //gamestate.ClientObject.QueueMessage(
            //Slow down the music for no reason.
            //BCBlockGameState.Soundman.GetPlayingMusic_Active().Tempo = 0.9f;
            //Debug.Print("Speed/Tempo set to " + BCBlockGameState.Soundman.GetPlayingMusic_Active().Tempo);
            //gamestate.EnqueueMessage("Paddle Frozen! Press Button A repeatedly!");
        }
        private HashSet<ButtonConstants> PressedButtons = new HashSet<ButtonConstants>(); 
        private void ClientObject_ButtonDown(Object sender, ButtonEventArgs<bool> e)
        {
                
            Random rg = BCBlockGameState.rgen;
            if (e.Button == ButtonConstants.Button_A)
            {
                if (!OwnerPaddle.Behaviours.Contains(this))
                {
                    e.Result = true;
                    return;
                }

                else
                {
                    Debug.Print("Ownerpaddle doesn't have us...");
                }

                for (int i = 0; i < 3; i++)
                {
                    RectangleF paddlerect = OwnerPaddle.Getrect();
                    PointF Randomspot = new PointF((float) (paddlerect.Width*rg.NextDouble()) + paddlerect.Left,
                                                   (float) (paddlerect.Height*rg.NextDouble()) + paddlerect.Top);
                    PolyDebris icedebris = new PolyDebris(Randomspot, 3, Color.Blue);
                    stateobj.Particles.Add(icedebris);
                }

                numpressed++;
                if (numpressed >= numpresstobreak)
                {
                    for (int i = 0; i < 50; i++)
                    {
                        RectangleF paddlerect = OwnerPaddle.Getrect();
                        PointF Randomspot = new PointF((float) (paddlerect.Width*rg.NextDouble()) + paddlerect.Left,
                                                       (float) (paddlerect.Height*rg.NextDouble()) + paddlerect.Top);
                        PolyDebris icedebris = new PolyDebris(Randomspot, 3, Color.Blue);
                        stateobj.Particles.Add(icedebris);
                    }
                    Debug.Print("Broken...");
                    //OwnerPaddle.Behaviours.Remove(this);
                    //((BCBlockGameState)OwnerPaddle.wGameState.Target).GameObjects.AddLast(new BehaviourRemoverProxy(OwnerPaddle, this));
                    ((BCBlockGameState) OwnerPaddle.wGameState.Target).Defer(() =>
                                                                             ((BCBlockGameState)
                                                                              OwnerPaddle.wGameState.Target)
                                                                                 .PlayerPaddle.Behaviours.Remove(
                                                                                     this));
                    DoRemove = true;
                }
                e.Result = true;
                return;
            }
            e.Result = false;
        }

        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
                
            //add it all back...
            if (fromPaddle != null)
            {
                fromPaddle.Behaviours = cachebehaviours;

                fromPaddle.Interactive = true;
                gamestate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
                if (BCBlockGameState.Soundman.GetPlayingMusic_Active() != null)
                    BCBlockGameState.Soundman.GetPlayingMusic_Active().Tempo = 1f;
            }
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            //throw new NotImplementedException();
            g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Blue)), onPaddle.Getrect());
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            return false;
            //throw new NotImplementedException();
        }
    }
}