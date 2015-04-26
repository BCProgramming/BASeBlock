using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using BASeBlock;
using BASeBlock.Blocks;

namespace BASeBlock.Controls
{
     [DefaultEvent("BlockSelected"),
    Description("ToolStripItem that allows selecting a color from a color picker control."),
    ToolboxItem(false),
    ToolboxBitmap(typeof(BASeBlock.Controls.BlockToolStripButton), "BlockToolStripItem")]
    class BlockToolStripButton : ToolStripSplitButton
    {

        public class BlockToolStripButtonBlockSelectedArgs:EventArgs
        {
            private Block _SelectedBlock=null;
            private ManyToOneBlockData _ManyToOneData;
            private BlockData _bdata;
            
            //properties.
            public Block SelectedBlock { get { return _SelectedBlock;} set {_SelectedBlock = value;}}
            public ManyToOneBlockData ManyToOneData{ get{ return _ManyToOneData;} set {_ManyToOneData = value;}}
            public BlockData bdata { get { return _bdata;} set {_bdata=value;}}
            
            public BlockToolStripButtonBlockSelectedArgs(Block pSelectedBlock,ManyToOneBlockData pManyToOneBlockData, BlockData pbdata)
            {
                _SelectedBlock = pSelectedBlock;
                _ManyToOneData= pManyToOneBlockData;
                _bdata = pbdata;



            }


        }
       
        public event EventHandler<BlockToolStripButtonBlockSelectedArgs> BlockSelected;


        protected void InvokeBlockSelected(BlockToolStripButtonBlockSelectedArgs args)
        {
            var copied = BlockSelected;
            if (copied != null) copied(this, args);


        }


        private BlockToolStripButtonBlockSelectedArgs _SelectedItem = null;
        /// <summary>
        /// sets/returns the selected item in this control.
        /// </summary>
        public BlockToolStripButtonBlockSelectedArgs SelectedItem { get { return _SelectedItem; } set { _SelectedItem = value; Invalidate(); } }

        private bool _ShowNoneEntry = true;

        public bool ShowNoneEntry { get { return _ShowNoneEntry;} set {_ShowNoneEntry = value;}}
        private bool _ExpandManyToOneBlocks = true;
        public bool ExpandManyToOneBlocks { get { return _ExpandManyToOneBlocks; } set { _ExpandManyToOneBlocks = value; } }
        public BlockToolStripButton()
            : base("")
        {

            DropDownOpening += new EventHandler(BlockToolStripButton_DropDownOpening);
            InitControl();
        }
        private void InitControl()
        {
            
            //this.AutoSize = false;
            //this.Width = 30;
            this.DropDownItems.Add("GHOST");
            this.Text = "(None)";
            this.ToolTipText = "No Block Selected.";

        }


        void ItemClicked(ToolStripMenuItem tsclicked, BlockData dataobject)
        {


        

            





        }
        void DirectClicked(ToolStripMenuItem clickeditem, Block blockobject,BlockData bdata,ManyToOneBlockData mtodata)
        {
            BlockData getbd;
            ManyToOneBlockData getmanytoone;
            Block acquireBlock;

            getbd = bdata;
            getmanytoone = mtodata;
            acquireBlock = blockobject;

            

            //set some defaults.
            if (getmanytoone != null)
            {
                //manyto one block. Should show name of "actual" type.
                Text = getmanytoone.DisplayText;
                ToolTipText = blockobject.GetType().Name + " - " + getmanytoone.DisplayText;
                //we need to draw it speshul.
                Bitmap resultimage = null;
                Graphics resultg = null;
                Block.DrawBlock(blockobject, out resultimage, out resultg);
                Image = resultimage;
                

            }
            else
            {
                Text = bdata.BlockType.Name;
                ToolTipText = Text;
                Image = bdata.useBlockImage;
            }


            var gotargs = new BlockToolStripButtonBlockSelectedArgs(blockobject, getmanytoone, bdata);
            var copied = BlockSelected;
            if (copied != null) copied(this, gotargs);
            SelectedItem = gotargs;





        }
        
        void BlockToolStripButton_DropDownOpening(object sender, EventArgs e)
        {
            //populate with blocks.
            ToolStripDropDown tsdropdown = ((ToolStripSplitButton)sender).DropDown;
            
            if (ShowNoneEntry)
            {
                tsdropdown.Items.Clear();
                ToolStripButton noneentry = new ToolStripButton("(None)");
                tsdropdown.Items.Add(noneentry);
                tsdropdown.Items.Add(new ToolStripSeparator());
            }
            else
            {
                tsdropdown.Items.Clear();
            }
            Block.PopulateDropDownWithBlocksCategorized(tsdropdown, false, null, null, ItemClicked, DirectClicked,ExpandManyToOneBlocks);
        }
        
        



    }
}
