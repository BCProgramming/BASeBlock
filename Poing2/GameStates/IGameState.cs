using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// Represents a Game State- Paused, Running, Showing Results screen, showing high scores, etc.
    /// Implementations implement appropriate display rendering and Game Routines via the interface methods.
    /// </summary>
    public interface IGameState
    {
        /// <summary>
        /// Runs a single frame of State logic using the given Game Information.
        /// </summary>
        /// <param name="GameInfo">Current Game Information to process</param>
        /// <returns>IGameState implementation for the next GameState to use. If this is null, no change in the current state is affected.</returns>
        IGameState Run(BCBlockGameState GameInfo);


        void DrawFrame(BCBlockGameState GameInfo, Graphics g, Size AreaSize);

        bool IsLoopingState {get ;}
    }

    public abstract class GameState:IGameState
    {
        public abstract IGameState Run(BCBlockGameState GameInfo);
        public abstract void DrawFrame(BCBlockGameState GameInfo,Graphics g,Size AreaSize);

        public virtual bool IsLoopingState { get{ return false;}}
    }
}
