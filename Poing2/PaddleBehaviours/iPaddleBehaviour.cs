using System;
using System.Drawing;
using BASeCamp.BASeBlock.GameObjects.Orbs;

namespace BASeCamp.BASeBlock.PaddleBehaviours
{
    public interface iPaddleBehaviour
    {
        /// <summary>
        /// acquires the icon for this behaviour.
        /// </summary>
        /// <returns>an image to represent the behaviour.</returns>
        Image GetIcon();

        void Draw(Paddle onPaddle, Graphics g);
        bool RequiresPerformFrame(BCBlockGameState gamestate, Paddle withpaddle);
        bool Impact(Paddle onPaddle, cBall withBall);
        void calcball(Paddle onPaddle, cBall withball);
        /// <summary>
        /// called when a MacGuffin touches the paddle.
        /// </summary>
        /// <param name="onPaddle"></param>
        /// <param name="collected"></param>
        /// <returns>true to ignore the macguffin. false otherwise.</returns>
        bool getMacGuffin(BCBlockGameState gstate,Paddle onPaddle, CollectibleOrb collected);
        /// <summary>
        /// called when a powerup falls on the paddle.
        /// </summary>
        /// <param name="onPaddle"></param>
        /// <param name="gpower"></param>
        /// <returns>true to ignore the powerup, false otherwise.</returns>
        bool getPowerup(BCBlockGameState gstate,Paddle onPaddle, GamePowerUp gpower);
        /// <summary>
        /// return True to flag this Behaviour for removal.
        /// </summary>
        /// <param name="gamestate"></param>
        /// <param name="ourpaddle"></param>
        /// <returns></returns>
        void PerformFrame(BCBlockGameState gamestate, Paddle ourpaddle);

        String getName();

        /// <summary>
        /// unhooks the behaviour from the paddle. 
        /// 
        /// </summary>
        void UnHook();

        //should also implement following constructor: (single arg).
        //(BCBlockGameState stateobject)
    }
}