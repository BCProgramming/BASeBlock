using System;
using System.Drawing;

namespace BASeBlock.PaddleBehaviours
{
    public class GenericTimedBehaviour<T> : TimedPaddleBehaviour where T : class,iPaddleBehaviour
    {
        private T _AddedInstance = default(T);

        public GenericTimedBehaviour():this(new TimeSpan(0,0,0,10))
        {

        }
        public GenericTimedBehaviour(TimeSpan Duration):this((T) Activator.CreateInstance(typeof(T)),Duration)
        {
                

        }
        public GenericTimedBehaviour(T defvalue,TimeSpan Duration)
        {
            _AddedInstance = defvalue;
            _BehaviourTime = Duration;    
        }
        public override Image GetIcon()
        {
            return null;
        }

        public override void Draw(Paddle onPaddle, Graphics g)
        {
            //nuthin.
        }

        public override bool Impact(Paddle onPaddle, cBall withBall)
        {
            //nuthin.
            return false;
        }

        public override void TimedBehaviourInitialize(Paddle toPaddle, BCBlockGameState gamestate)
        {
            //add the behaviour.
            if(_AddedInstance!=null)
                toPaddle.Behaviours.Add(_AddedInstance);
        }

        public override void TimedBehaviourRemove(Paddle frompaddle, BCBlockGameState gamestate)
        {
            if (_AddedInstance != null)
                frompaddle.Behaviours.Remove(_AddedInstance);
        }
    }
}