using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using BASeCamp.BASeBlock.GameObjects.Orbs;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.PaddleBehaviours
{
    /// <summary>
    /// This behaviour forces Powerups and Macguffins to deflect off the paddle rather than being collected.
    /// </summary>
    public class DeflectorBehaviour:BasePaddleBehaviour
    {
        private Image drawimage = BCBlockGameState.Imageman.getLoadedImage("deflector");
        public override Image GetIcon()
        {
            return null;
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            Rectangle rct = new Rectangle(Point.Empty, onPaddle.Getrect().Size);
            using (TextureBrush tb = new TextureBrush(drawimage, WrapMode.Tile,rct))
            {

                //calculate length to translate.
                int translateamount = (int) (((float)drawimage.Width / 1000f) * DateTime.Now.Millisecond);
                tb.TranslateTransform(translateamount, 0);
                g.FillRectangle(tb, onPaddle.Getrect());
            }
                
                
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            return false;
            //throw new NotImplementedException();
        }
        GameObject addedobject = null;
        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            base.BehaviourAdded(toPaddle, gamestate);
            gamestate.Defer(()=>{
                                    addedobject = new ProxyObject((po, gs) =>
                                                                      {

                                                                          PerformFrame(toPaddle, gamestate);
                                                                          return false;

                                                                      }, null);
                                    gamestate.GameObjects.AddLast(addedobject);
            });
                
        }
        int offsetcall = 0;
        private void PerformFrame(Paddle toPaddle,BCBlockGameState gstate)
        {
            //add a random lightorb.
            if (offsetcall++ > 10)
            {
                offsetcall = 0;
                Color chosencolor = new HSLColor(BCBlockGameState.rgen.NextDouble() * 240, 240, 128);
                LightOrb lo = new LightOrb(toPaddle.BlockRectangle.RandomSpot(BCBlockGameState.rgen), chosencolor, 32);
                lo.Velocity = BCBlockGameState.GetRandomVelocity(0, 3);
                gstate.Defer(() => gstate.Particles.Add(lo));


            }


        }
        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            base.BehaviourRemoved(fromPaddle, gamestate);
            if(addedobject!=null)
                gamestate.GameObjects.Remove(addedobject);
        }
        public override bool getPowerup(BCBlockGameState gstate,Paddle onPaddle, GamePowerUp gpower)
        {
            if(onPaddle==null) return false;
            //don't accept it.
            //reject it with great prejudice.
            //move the location temporarily.
            PointF oldposition = gpower.Location;
            SizeF oldsize = gpower.Size;
            gpower.Location = PointF.Empty;
            //draw to a temporary bitmap.
            Bitmap drawtothis = new Bitmap(16, 16);
            Graphics useg = Graphics.FromImage(drawtothis);
            useg.Clear(Color.Transparent);
            gpower.Draw(useg);
            //reset position.
            gpower.Location = oldposition;
            gpower.Size = oldsize;
            //get average.
            var averagedpixel = Color.FromArgb((int)((from p in drawtothis.getPixels() select p.ToArgb()).Average()));
            ExplosionEffect ee = new ExplosionEffect(gpower.Location, 72);
            ee.ExplosionColor = averagedpixel;
            ee.DamageBlocks = false;
            ee.DamagePaddle = false;
            gstate.Defer(() => gstate.GameObjects.AddLast(ee));
            //move  gpower to above the paddle, and invert the Y speed.
            gpower.Location = new PointF(gpower.Location.X, onPaddle.BlockRectangle.Top - gpower.getRectangle().Height - 1);
            gpower.Velocity = new PointF(gpower.Velocity.X, -Math.Abs(gpower.Velocity.Y)*1.1f);
            return true;
        }
        public override bool getMacGuffin(BCBlockGameState gstate,Paddle onPaddle, CollectibleOrb collected)
        {
            collected.Velocity = new PointF(collected.Velocity.X, -Math.Abs(collected.Velocity.Y)*1.2f);
            return true;
        }
    }
}