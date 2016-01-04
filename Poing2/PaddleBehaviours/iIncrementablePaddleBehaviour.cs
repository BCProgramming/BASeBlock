namespace BASeCamp.BASeBlock.PaddleBehaviours
{
    /// <summary>
    /// interface implemented by PaddleBehaviours that want to be notified when another Behaviour of their kind
    /// is applied.
    /// </summary>
    public interface iIncrementablePaddleBehaviour{

        /// <summary>
        /// Called when another Behaviour of the same type is being applied to the paddle.
        /// </summary>
        /// <param name="pBehaviour">Behaviour being added</param>
        /// <param name="pPaddle">Paddle being applied to</param>
        /// <returns>true to prevent this behaviour from being added. False otherwise.</returns>
        bool Applied(iPaddleBehaviour pBehaviour,Paddle pPaddle);

    }
}