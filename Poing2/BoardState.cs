using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using BASeCamp.BASeBlock.Blocks;

namespace BASeCamp.BASeBlock
{

    //Blocks in the game typically are arranged in a grid but sometimes are organized in other ways, such as beneath each other and whatnot.
    //This class attempts to make it possible to write algorithms for the blocks based on a grid.

    /// <summary>
    /// BoardState  class: used to create a grid-oriented approximation of a level.
    /// </summary>
    class BoardState
    {

        public class BoardCell
        {
            private int _Row, _Col;
            private Rectangle Rect;
            private List<Block> _BlocksTouched;
            public bool hasBlocks { get; private set; }

            public int Row
            {
                get { return _Row; }
                set { _Row = value; }
            }

            public int Col {
                get { return _Col; }
                set { _Col = value; }
            }

            public int X
            {
                get { return Col; }
                set { Col = value; }
            }

            public int Y
            {
                get { return Row; }
                set { Row = value; }
            }

            public BoardCell(Rectangle userect, BCBlockGameState gstate,int pRow,int pCol)
            {
                Row = pRow;
                Col = pCol;
                List<Block> result = BCBlockGameState.Block_HitTest(gstate.Blocks.ToList(), userect, false);
                _BlocksTouched = result;
                hasBlocks = _BlocksTouched.Count > 0;
            }


        }


        public BoardState(BCBlockGameState currentstate)
            : this(currentstate, 16, 16)
        {

        }

        public BoardState(BCBlockGameState currentstate,int GridX,int GridY)
        {
            

        }


    }
}
