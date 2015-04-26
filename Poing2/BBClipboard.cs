using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BASeBlock.Blocks;

namespace BASeBlock
{
    class BBClipboard
    {
        public const String BlockFormatName = "BASeBlocks";
        public const String BallsFormatName = "BASeBalls";
        public const String LevelFormatName = "BASeLevel";



        public static void CopyLevel(Level copythis)
        {
            Debug.Print("Copying Level '" + copythis.LevelName);
            //register custom data format...
            DataFormats.Format format = DataFormats.GetFormat(LevelFormatName);
            //copy to clipboard.
            IDataObject dataObj = new DataObject();
            dataObj.SetData(format.Name, false, copythis);
            Clipboard.SetDataObject(dataObj, true);



        }
        public static void CopyBlocks(List<Block> blockscopy)
        {
            Debug.Print("Copying " + blockscopy.Count);
            //register custom data format with Windows..
            DataFormats.Format format =
                 DataFormats.GetFormat(BlockFormatName);

            //now copy to clipboard

            IDataObject dataObj = new DataObject();
            try
            {
                dataObj.SetData(format.Name, false, blockscopy.ToArray());
            }
            catch (Exception exx)
            {
                Debug.Print(exx.ToString());
            }
            Clipboard.SetDataObject(dataObj, true);
            
            var dataObj2 = Clipboard.GetDataObject();

            if (dataObj2.GetData(BBClipboard.BlockFormatName) == null)
            {
                Debug.Assert(false);

            }
            //COPIED.




        }
        public static void CopyBalls(List<cBall> BallsCopy)
        {
            DataFormats.Format format = DataFormats.GetFormat(BallsFormatName);
            IDataObject dataObj = new DataObject();
            dataObj.SetData(format.Name, false, BallsCopy.ToArray());
            Clipboard.SetDataObject(dataObj);

        }
        public static bool CanPerformPaste()
        {
            //if it has a supported data format, allow the paste.
            //otherwise, return false.

            IDataObject dataObj = Clipboard.GetDataObject();
            return dataObj.GetDataPresent(BlockFormatName) || dataObj.GetDataPresent(BallsFormatName);

            /*
             * 
            IDataObject dataObj = Clipboard.GetDataObject();
            string format = typeof(Document).FullName;

            if (dataObj.GetDataPresent(format))
            {
                doc = dataObj.GetData(format) as Document;
            }
            return doc;
            */

        }
    }
}
