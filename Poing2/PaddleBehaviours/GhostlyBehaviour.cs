using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;

namespace BASeBlock.PaddleBehaviours
{
    class GhostlyBehaviour : BasePaddleBehaviour
    {
        private bool GhostlyMode = false;
        private BCBlockGameState _gamestate;
        private RectangleF storedPosition;
        private static TextureBrush useTextureBrush;
        Paddle ApplicablePaddle = null;
        public GhostlyBehaviour(BCBlockGameState sourcestate)
        {
            _gamestate = sourcestate;
            
        }
        private bool GhostlyFunc(BCBlockGameState.NextFrameStartup self,BCBlockGameState gstate)
        {
            //while "ghostly" mode is enabled, we need to store the current position of the paddle
            //and move it each frame, so that stuff will go through the paddle.
            if (GhostlyMode)
            {
                if (gstate.PlayerPaddle.Location.Y < gstate.GameArea.Height)
                {
                    storedPosition = ApplicablePaddle.BlockRectangle;
                    //move to bottom.
                    ApplicablePaddle.BlockRectangle = new RectangleF(storedPosition.X, storedPosition.Width, gstate.GameArea.Height + 50, storedPosition.Height);
                }
            }
            return GhostlyMode;
        }
        
        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            ApplicablePaddle = toPaddle;
            _gamestate = gamestate;
            gamestate.ClientObject.ButtonDown += ClientObject_ButtonDown;
            gamestate.ClientObject.ButtonUp += ClientObject_ButtonUp;
            base.BehaviourAdded(toPaddle, gamestate);
        }
        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            gamestate.ClientObject.ButtonDown -= ClientObject_ButtonDown;
            gamestate.ClientObject.ButtonUp -= ClientObject_ButtonUp;
            base.BehaviourRemoved(fromPaddle, gamestate);
        }
        void ClientObject_ButtonUp(object sender, Events.ButtonEventArgs<bool> e)
        {
            if ((e.Button & ButtonConstants.Button_B) == ButtonConstants.Button_B)
            {
                GhostlyMode = false;

            }
        }

        void ClientObject_ButtonDown(object sender, Events.ButtonEventArgs<bool> e)
        {
            if ((e.Button & ButtonConstants.Button_B) == ButtonConstants.Button_B)
            {
                GhostlyMode = true;
                //if ghostly mode, attach our per-frame handler.
               // if (_gamestate == null) return;
               // _gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup( GhostlyFunc));
            }
        }
        private TextureBrush getTextureBrush()
        {
            Image clouds = BCBlockGameState.Imageman.getLoadedImage("GHOSTPADDLE");
            //transform...
            Bitmap newclouds = new Bitmap(clouds.Width, clouds.Height);
            ImageAttributes ia = new ImageAttributes();
            ia.SetColorMatrix(ColorMatrices.GetFader(0.75f));
            using (Graphics fadeit = Graphics.FromImage(newclouds))
            {
                fadeit.DrawImage(clouds, new Rectangle(0, 0, clouds.Width, clouds.Height), 0, 0, clouds.Width, clouds.Height, GraphicsUnit.Pixel, ia);

            }
            return new TextureBrush(newclouds, WrapMode.TileFlipXY);

        }
        public override Image GetIcon()
        {
            return BCBlockGameState.Imageman.getLoadedImage("GHOSTLY1");
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            if (useTextureBrush == null) useTextureBrush = getTextureBrush();
            if (!GhostlyMode) return;
            useTextureBrush.ResetTransform();
            float percentage = ((float)DateTime.Now.Millisecond)/1000;
            int numpixels = (int)(useTextureBrush.Image.Width*percentage);

            useTextureBrush.TranslateTransform(numpixels, 0);
            //draw in the old location, just a box.
            g.FillRectangle(useTextureBrush, onPaddle.Getrect());

        }
        public override bool getMacGuffin(BCBlockGameState gstate, Paddle onPaddle, GameObjects.Orbs.CollectibleOrb collected)
        {
            return GhostlyMode;
        }
        public override bool getPowerup(BCBlockGameState gstate, Paddle onPaddle, GamePowerUp gpower)
        {
            return GhostlyMode;
        }
        public override bool Impact(Paddle onPaddle, cBall withBall)
        {

            return GhostlyMode; //cancel!
            //throw new NotImplementedException();
        }
    }
}
