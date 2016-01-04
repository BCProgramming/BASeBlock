using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BASeCamp.BASeBlock.HighScores;

namespace BASeCamp.BASeBlock
{

   

    //iLevelBuilder interface
    /// <summary>
    /// Interface implemented by level builder classes for the creation of Levels.
    /// </summary>
    public interface iLevelSetBuilder
    {
        String getName();
        String getDescription();
        LevelSet BuildLevelSet(RectangleF targetrect,IWin32Window Owner);
        IHighScoreList HighScores { get; set; }
        /// <summary>
        /// determine wether this implementation requires the display of config pages, or otherwise a call to "configure" before Building.
        /// </summary>
        /// <returns></returns>
        bool RequiresConfig();

        void Configure();


    }
}
