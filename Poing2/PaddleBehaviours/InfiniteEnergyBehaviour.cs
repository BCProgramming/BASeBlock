using System.Drawing;

namespace BASeBlock.PaddleBehaviours
{
    public class InfiniteEnergyBehaviour:BasePaddleBehaviour
    {
        public override Image GetIcon()
        {
            return null;
            //throw new NotImplementedException();
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            //throw new NotImplementedException();
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            return false;
            //throw new NotImplementedException();
        }
        public override void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            toPaddle.OnEnergyChange += toPaddle_OnEnergyChange;
        }

        void toPaddle_OnEnergyChange(object sender, PaddleElementChangeEventArgs<float> e)
        {
            //throw new NotImplementedException();
            if (e.NewValue < e.OldValue) e.Cancel = true;
        }

          
        public override void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            fromPaddle.OnEnergyChange -= toPaddle_OnEnergyChange;
        }
    }
}