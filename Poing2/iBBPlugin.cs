using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// interface to be implemented by more "low-level" BASeBlock plugins.
    /// </summary>
    public interface iBBPlugin
    {
        


        /// <summary>
        /// Initializes this BASeBlock plugin. This is where events should be hooked- no other "callbacks" will be occuring unless events are
        /// hooked here.
        /// </summary>
        /// <param name="gamestate"></param>
        /// <returns>true to indicate success; false otherwise.</returns>
        bool Initialize(BCBlockGameState gamestate);
        /// <summary>
        /// Gets the Name of this plugin.
        /// </summary>
        /// <returns>The name of this plugin.</returns>
        string getName();
        /// <summary>
        /// Gets a quick description of this plugin.
        /// </summary>
        /// <returns>Description for this plugin.</returns>
        string getDescription();
    }
}
