using System.Drawing;
using BASeBlock.Particles;

namespace BASeBlock.PaddleBehaviours
{
    /// <summary>
    /// Makes the paddle drip "blood"... well, really, water that is red, same diff though.
    /// </summary>
    public class BloodDripBehaviour : BasePaddleBehaviour
    {
        private readonly int HPCheck = 35;
        private GameObject proxiedObject;

        public override Image GetIcon()
        {
            return null; //doesn't have an icon...
        }

        public override bool RequiresPerformFrame(BCBlockGameState gamestate, Paddle withpaddle)
        {
            return true;
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            //nothing
        }

        private bool PerformFrameProxy(ProxyObject sourceobject, BCBlockGameState gamestate)
        {
            var pPaddle = gamestate.PlayerPaddle;
            if (pPaddle == null) return true;
            //"bleed"
            RectangleF paddlerect = pPaddle.Getrect();
            for (int i = 0; i < (int) (20f*BCBlockGameState.ParticleGenerationFactor); i++)
            {
                //add a random blood particle...
                PointF randomspot =
                    new PointF((float) (paddlerect.Width*BCBlockGameState.rgen.NextDouble() + paddlerect.Left),
                               (float) (paddlerect.Bottom));

                WaterParticle Bloodparticle = new WaterParticle(randomspot, Color.Red);
                Bloodparticle.Velocity = new PointF((float) BCBlockGameState.rgen.NextDouble()*2 - 1,
                                                    (float) BCBlockGameState.rgen.NextDouble() - 0.5f);

                gamestate.Particles.Add(Bloodparticle);
            }

            return false;
        }

        public override void PerformFrame(BCBlockGameState gamestate, Paddle pPaddle)
        {
            base.PerformFrame(gamestate, pPaddle);


            if (pPaddle.HP > HPCheck)
            {
                // 
                if (proxiedObject != null)
                {
                    gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                                                                                               {
                                                                                                   gamestate
                                                                                                       .GameObjects
                                                                                                       .Remove(
                                                                                                           proxiedObject);
                                                                                                   gamestate
                                                                                                       .PlayerPaddle
                                                                                                       .Behaviours
                                                                                                       .Remove(this);
                                                                                               }));
                }
            }
        }

        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            proxiedObject = new ProxyObject(PerformFrameProxy, null);
            gamestate.GameObjects.AddLast(proxiedObject);
        }

        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            if (gamestate != null)
                if (gamestate.GameObjects != null)
                    gamestate.GameObjects.Remove(proxiedObject);
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            //maybe splash blood around...
            return false;
        }
    }
}