using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock
{
    /// <summary>
    /// Similar in purpose to the iGameClient interface. Implemented by the Editor form.
    /// </summary>
    public interface IEditorClient
    {
        List<Block> GetSelectedBlocks();
        List<cBall> GetSelectedBalls();
        List<ObjectPathData> GetSelectedPaths();
        List<Block> GetBlocks();
        List<cBall> GetBalls();
        ObjectPathDataManager GetPaths();
        EditorSet GetSet();
 


    }
    /// <summary>
    /// Implemented by any class who's data can be inspected by the editor.
    /// </summary>
    public interface IEditorExtensions
    {
        /// <summary>
        /// retrieves additional tooltip information for a block.
        /// </summary>
        /// <returns></returns>
        String GetToolTipInfo(IEditorClient Client);


    }


    //Interface for Editor Capabilities.
    /// <summary>
    /// implemented by blocks that want to "know" when they are being edited.
    /// The main purpose for this is so they can do things like
    /// draw more information about themselves; the powerupblock for example can draw it's contents on top of itself.(the powerup it would reveal, that is)
    /// </summary>
    public interface IEditorBlockExtensions : IEditorExtensions
    {
        /// <summary>
        /// called when this block is drawn in the editor. the "normal" implementation of Draw will not be called afterwards.
        /// </summary>
        /// <param name="g"></param>
        void EditorDraw(Graphics g, IEditorClient Client);


        //method used to indicate selections.
        //passes in a brush, which should be used to paint the visible area of the block.
        void DrawSelection(Brush selectionbrush, Graphics g,IEditorClient Client);

    }



}
