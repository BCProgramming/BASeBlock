using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BASeBlock.Blocks;

namespace BASeBlock
{

    /// <summary>
    /// BlockCombo control, derives from ImageCombo, and is automatically populated with a list of blocktypes as well as their images.
    /// </summary>
    /// 
    

    class BlockCombo : ImageCombo 
    {
        public BlockCombo()
            : base()
        {
            if (!this.DesignMode)
            {
                
            }
        }
        private Dictionary<ImageComboItem, BlockData> blockdatacache = new Dictionary<ImageComboItem, BlockData>();
        public BlockData? GetSelectedBlockData()
        {
            if(SelectedItem==null) return null;
            return ((BlockData)(((ImageComboItem)SelectedItem).Tag));
            

        }

        public void RePopulate()
        {
            System.Windows.Forms.ImageList createlist = new ImageList();
            this.ImageList=createlist;
            blockdatacache=new Dictionary<ImageComboItem, BlockData>();
            
            foreach (BlockData loopdata in BCBlockGameState.BlockDataMan.BlockInfo)
            {
                createlist.Images.Add(loopdata.BlockType.Name, loopdata.useBlockImage);
                //add the item itself.
                ImageComboItem createditem = new ImageComboItem(loopdata.Usename, createlist.Images.IndexOfKey(loopdata.BlockType.Name));
                blockdatacache.Add(createditem,loopdata);
                createditem.Tag = loopdata;
                int addedindex=this.Items.Add(createditem);
                


            }


        }
        


    }
}
