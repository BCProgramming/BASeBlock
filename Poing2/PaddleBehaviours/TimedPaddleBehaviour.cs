using System;
using System.Linq;

namespace BASeBlock.PaddleBehaviours
{
    /// <summary>
    /// Abstract class implemented by PaddleBehaviours that only last a limited time.
    /// </summary>
    public abstract class TimedPaddleBehaviour : BasePaddleBehaviour
    {
        public String DelayIdentifier = "";
        protected String _AbilityMusic = "INVINCIBLE"; //music to play for this ability, or "" for no change.
        protected TimeSpan _BehaviourTime = new TimeSpan(0, 0, 0, 10);

        //routine invoked at end of TimeDelay. Tasked with Stopping any music we played, and queueing up the removal of this behaviour.

        private void TimeDelayRoutine(object[] parameters)
        {
            BCBlockGameState ggs = (BCBlockGameState) (parameters[0]);
            if (ggs.PlayerPaddle == null || !ggs.PlayerPaddle.Behaviours.Contains(this)) return;
            //queue up the removal of this Behaviour.
            ggs.QueueFrameEvent((nfs, bstate) =>
                                    {
                                        bstate.Forcerefresh = true;
                                        return bstate.PlayerPaddle.Behaviours.Remove(this);
                                    }, null);
            if (!String.IsNullOrEmpty(_AbilityMusic))
                BCBlockGameState.Soundman.StopTemporaryMusic(_AbilityMusic);
        }

        /// <summary>
        /// Whether this timedbehaviour is SingleInstance or not. If so, we will simply reset the time delay to remove the behaviour.
        /// </summary>
        /// <returns></returns>
        protected bool SingleInstance()
        {
            return true;
        }

        /// <summary>
        /// called as this Behaviour Initializes.
        /// </summary>
        /// <param name="toPaddle"></param>
        /// <param name="gamestate"></param>
        public abstract void TimedBehaviourInitialize(Paddle toPaddle, BCBlockGameState gamestate);

        /// <summary>
        /// called when this Behaviour is being removed or otherwise detached from the Paddle.
        /// </summary>
        /// <param name="frompaddle"></param>
        /// <param name="gamestate"></param>
        public abstract void TimedBehaviourRemove(Paddle frompaddle, BCBlockGameState gamestate);

        /// <summary>
        /// sealed override- responsible for calling the TimedBehaviourRemove abstract method as well as the base
        /// implementation.
        /// </summary>
        /// <param name="fromPaddle"></param>
        /// <param name="gamestate"></param>
        public override sealed void BehaviourRemoved(Paddle fromPaddle, BCBlockGameState gamestate)
        {
            base.BehaviourRemoved(fromPaddle, gamestate);
            TimedBehaviourRemove(fromPaddle, gamestate);
        }

        /// <summary>
        /// sealed override, responsible for most of the bulk of the work of the TimedBehaviour.
        /// </summary>
        /// <param name="toPaddle"></param>
        /// <param name="gamestate"></param>
        public override sealed void BehaviourAdded(Paddle toPaddle, BCBlockGameState gamestate)
        {
            //call base implementation
            base.BehaviourAdded(toPaddle, gamestate);
            //check if we are SingleInstance. If so, that means that there can only be one instance at a time, which may not be surprising given the name.
            if (SingleInstance())
            {
                //check for existing instances of our class. If found, add our delay time to them.
                foreach (var iterate in (from p in toPaddle.Behaviours where p != this select p))
                {
                    if (iterate.GetType() == GetType())
                    {
                        //if same type, change delay, and break from routine.
                        (iterate as TimedPaddleBehaviour).ChangeDelayTime(gamestate, _BehaviourTime, true);
                        //make sure this instance get's removed, too! we are redundant now.
                        gamestate.NextFrameCalls.Enqueue
                            (new BCBlockGameState.NextFrameStartup(() => toPaddle.Behaviours.Remove(this)));

                        return;
                    }
                }
            }


            //if we are either not singleinstance or we are single instance but there are no existing  behaviours of our type attached,
            //do the stuff to add us.
            DelayIdentifier = gamestate.DelayInvoke(_BehaviourTime, TimeDelayRoutine, new object[] {gamestate});
            //if we have ability music, we play it now. Use the SoundManager's capacity to handle temporary music, which works rather well.
            if (_AbilityMusic != "") BCBlockGameState.Soundman.PlayTemporaryMusic(_AbilityMusic, 1.0f, true);
            //hook Death function. If the paddle dies, obviously the time delay will break out early, so we will need to stop the temporary music ourself.
            toPaddle.OnDeath += new Func<Paddle, bool>(toPaddle_OnDeath);
            //call abstract method for initialization.
            TimedBehaviourInitialize(toPaddle, gamestate);
        }

        public bool ChangeDelayTime(BCBlockGameState gstate, TimeSpan newdelay, bool resetstart)
        {
            return gstate.ChangeDelayData(DelayIdentifier, newdelay, null, resetstart);
        }

        private bool toPaddle_OnDeath(Paddle arg)
        {
            if (!String.IsNullOrEmpty(_AbilityMusic))
                BCBlockGameState.Soundman.StopTemporaryMusic(_AbilityMusic);
            return true;
        }
    }
}