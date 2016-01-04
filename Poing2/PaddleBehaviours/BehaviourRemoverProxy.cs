using System.Diagnostics;

namespace BASeCamp.BASeBlock.PaddleBehaviours
{
    public class BehaviourRemoverProxy : ProxyObject
    {
        private Paddle RemoveFrompaddle;
        private iPaddleBehaviour removeobject;

        public BehaviourRemoverProxy(Paddle frompaddle, iPaddleBehaviour removethis) : base(null, null)
        {
            RemoveFrompaddle = frompaddle;
            removeobject = removethis;
        }

        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            if (removeobject is BasePaddleBehaviour)
                ((BasePaddleBehaviour) removeobject).BehaviourRemoved(RemoveFrompaddle, gamestate);

            RemoveFrompaddle.Behaviours.Remove(removeobject);
            Debug.Print("removed...");

            return true;
        }
    }
}