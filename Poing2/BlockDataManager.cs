using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection.BASeCamp;

namespace BASeBlock.Blocks
{
    public class BlockDataManager
    {
        public LoadedTypeManager BlockTypeManager = BCBlockGameState.BlockTypes;
        public List<BlockData> BlockInfo = new List<BlockData>();

        public BlockData? this[Type blocktype]
        {
            get
            {
                foreach (BlockData loopdata in BlockInfo)
                {

                    if (loopdata.BlockType == blocktype)
                        return loopdata;

                }
                return null;

            }

        }


        public BlockDataManager()
        {
            Bitmap drawBitmap = new Bitmap(128, 64);
            Graphics drawbuffer = Graphics.FromImage(drawBitmap);


            //now, we need to create the images...
            foreach (Type loopvalue in BlockTypeManager.ManagedTypes)
            {

           
                // if (((!(BCBlockGameState.HasAttribute(loopvalue, typeof(BBEditorInvisibleAttribute))) || BCBlockGameState.HasAttribute(loopvalue,typeof(BBEditorVisibleAttribute)))|| KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey) < 0))
                //if (((!invisattrib) || vistrib) || shiftpressed)
                //{
                drawbuffer.Clear(Color.White);
                Block createdblock = null;
                try
                {




                    createdblock =
                        (Block)Activator.CreateInstance(loopvalue, (Object)new RectangleF(0, 0, 128, 64));
                }
                catch (Exception except)
                {
                    Debug.Print("Exception " + except.Message + " instantiating " + loopvalue.Name);
                    continue;
                }
                try
                {
                    createdblock.Draw(drawbuffer);
                    BlockData newdata = new BlockData();
                    newdata.BlockType = loopvalue;
                    newdata.useBlockImage = (Image)drawBitmap.Clone();
                    newdata.Usename = loopvalue.Name;
                    BlockInfo.Add(newdata);
                }
                catch (Exception anyexception)
                {
                    // Exception when we tried to draw, probably. (issue first encountered with replacerblock).
                    //Fix: when an error occurs, set the useBlockImage to null. Also, made a change to the BlockData so that
                    //it will redraw the image itself if it is null.
                    BlockData adddata = new BlockData();
                    adddata.BlockType = loopvalue;
                    adddata.useBlockImage = null;
                    adddata.Usename = loopvalue.Name;
                    BlockInfo.Add(adddata);



                }
                //  }
            }
        }
    }
}