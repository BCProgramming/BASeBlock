using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace BASeBlock
{
    public partial class frmEditor : Form
    {

      
        public enum EditDragModeConstants
        {
            Drag_None,
            Drag_Move,
            Drag_Select //Draw selection box.


        }
        [Flags]
        public enum EditObjectConstants
        {
            Edit_Blocks=1,
            Edit_Balls=2

        }
        private struct DragItemData<T>
        {
            public T DragItem;
            public PointF OriginOffset;


        }
        //private Thread RefreshThread;
        private struct BlockData
        {
            public Type BlockType;
            public Image useBlockImage; //rendered to 128x64 buffer
            public String Usename;

        }
        private String LevelFileName = "";
        private bool _IsDirty;
        private Brush SelectionBrush = new HatchBrush(HatchStyle.Trellis, Color.Black);
        /// <summary>
        /// Sets/returns the dirty bit of the loaded file. This is the "global" dirty- that is, wether the file is changed.
        /// </summary>
        public bool IsDirty
        {

            get { return _IsDirty; }
            set { _IsDirty=value;
            if (_IsDirty)
            {
                if (!Text.EndsWith("*")) Text += "*";

            }
            else
            {
                if (Text.EndsWith("*")) Text = Text.Substring(0, Text.Length - 1);

            }


            }

        }
            
        private List<DragItemData<Block>> DraggingBlocks = new List<DragItemData<Block>>();
        private List<DragItemData<cBall>> DraggingBalls = new List<DragItemData<cBall>>();
        
        private class BlockDataManager
        
        {
            public LoadedTypeManager BlockTypeManager = null;
            public List<BlockData> BlockInfo = new List<BlockData>();
            public BlockDataManager()
            {
                Bitmap DrawBitmap = new Bitmap(128, 64);
                Graphics drawbuffer = Graphics.FromImage(DrawBitmap);
                BlockTypeManager = new LoadedTypeManager(new Assembly[] { Assembly.GetExecutingAssembly() }, typeof(Block), new Nullcallback());

                //now, we need to create the images...
                foreach (Type loopvalue in BlockTypeManager.ManagedTypes)
                {
                    drawbuffer.Clear(Color.White);
                    Block createdblock=null;
                    try
                    {
                        createdblock = (Block)Activator.CreateInstance(loopvalue, (Object)new RectangleF(0, 0, 128, 64));
                    }
                    catch
                    {
                        continue;
                    }
                    createdblock.Draw(drawbuffer);
                    
                    BlockData newdata = new BlockData();
                    newdata.BlockType=loopvalue;
                    newdata.useBlockImage = (Image)DrawBitmap.Clone();
                    newdata.Usename = loopvalue.Name;
                    BlockInfo.Add(newdata);
                }


            }





        }
        
        private  BlockDataManager Blockman = new BlockDataManager();
        public List<Block> SelectedBlocks = new List<Block>();
        public List<cBall> SelectedBalls = new List<cBall>();
        public List<Block> EditBlocks = new List<Block>(); //blocks to edit, the current set of blocks.
        public List<cBall> EditBalls = new List<cBall>();
        public LevelSet EditSet = new LevelSet();
        public int EditingLevel = 1;
        public cLevel EditLevel;
        private Thread RepaintThread;  //used currently to attempt to draw blinking selections
        public frmEditor()
        {
            InitializeComponent();
        }
        private void InitBlocks()
        {
            EditLevel = new cLevel();
            EditSet.Levels.Add(EditLevel);
            for(int X=0;X<128;X+=32)
                EditBlocks.Add(new InvincibleBlock(new RectangleF(X, 0, 32, 16)));
            EditBalls.Add(new cBall(new PointF(PicEditor.Width / 2, PicEditor.Height / 2), new PointF(2, -2)));

        }
        /// <summary>
        /// copies EditBlocks to the current Level
        /// </summary>


        private void ApplyBlocks()
        {
            EditLevel.levelblocks = EditBlocks;
            EditLevel.levelballs = EditBalls;

        }
        private void ApplyLevel()
        {
            EditSet.Levels[EditingLevel-1]= EditLevel;


        }
        private Dictionary<TabPage, Panel> TabPanels = new Dictionary<TabPage, Panel>();
        private ImageList TabSideBarImages;
        private void InitImageList()
        {
            TabSideBarImages = new ImageList();
            TabSideBarImages.ImageSize = new Size(16, 16);
            foreach (KeyValuePair<String,Image> loopimage in BCBlockGameState.Imageman.GetImages())
            {
                
                TabSideBarImages.Images.Add(loopimage.Key, loopimage.Value);


            }



        }

        

        private void frmEditor_Load(object sender, EventArgs e)
        {
            InitImageList();
            
            InitBlocks();
            RepaintThread = new Thread(PaintThread);

            RepaintThread.Start();

        }
        const int BlendMax = 168, BlendMin = 24;
        private int SelectionBlend = 0;
        private int SelectionBlendDirection = 15;
        private void PaintThread()
        {
            //change the "SelectionBlend" amount


            while (Visible)
            {
                try
                {
                    SelectionBlend += SelectionBlendDirection;
                    if (SelectionBlend > BlendMax)
                    {
                        SelectionBlend = BlendMax;
                        SelectionBlendDirection = Math.Sign(BlendMax - BlendMin)*-15;

                    }
                    else if (SelectionBlend < BlendMin)
                    {
                        SelectionBlend = BlendMin;
                        SelectionBlendDirection = Math.Sign(BlendMax - BlendMin)*15;
                    }

                    Thread.Sleep(50);

                    PicEditor.Invoke((MethodInvoker) (() =>
                                                          {

                                                              PicEditor.Invalidate();
                                                              PicEditor.Update();
                                                          }
                                                     )
                        );
                }
                catch
                {
                    //no code

                }

            }


        }

        private void InitDrag(Point DragStart)
        {
            if (SelectedBlocks == null || SelectedBlocks.Count == 0)
                return;
            //initialize the drag operation data.
            CurrentDragMode = EditDragModeConstants.Drag_Move;
            //get the appropriate offset from the lowest X/Y block; the one with the lowest Sum X+Y...
            if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
            {

                DragOriginBlock =
                    (from n in SelectedBlocks orderby n.BlockRectangle.Left + n.BlockRectangle.Right ascending select n)
                        .First();
                DragOffset = new PointF(DragStart.X - DragOriginBlock.BlockRectangle.Left, DragStart.Y - DragOriginBlock.BlockRectangle.Top);
                DraggingBlocks = new List<DragItemData<Block>>();
                foreach (Block loopblock in SelectedBlocks)
                {
                    DragItemData<Block> newdragdata = new DragItemData<Block>();
                    newdragdata.DragItem = loopblock;
                    newdragdata.OriginOffset = new PointF(loopblock.BlockRectangle.Left - DragStart.X, loopblock.BlockRectangle.Top - DragStart.Y);
                    DraggingBlocks.Add(newdragdata);



                }
            }
            else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
            {
                DragOriginBall =
                    (from n in SelectedBalls orderby n.Location.X + n.Location.Y ascending select n).First();
                DragOffset = new PointF(DragStart.X - DragOriginBall.Location.X, DragStart.Y - DragOriginBall.Location.Y);
                DraggingBalls = new List<DragItemData<cBall>>();
                foreach (cBall loopball in SelectedBalls)
                {
                    DragItemData<cBall> newballdata = new DragItemData<cBall>();
                    newballdata.DragItem = loopball;
                    newballdata.OriginOffset = new PointF(loopball.Location.X-DragStart.X,loopball.Location.Y-DragStart.Y);
                    DraggingBalls.Add(newballdata);


                }
            }
            //if that worked... LINQ rules.
            

         



        }
        private SizeF gridSize = new SizeF(32,16);
        private void PicEditor_Paint(object sender, PaintEventArgs e)
        {
            //redraw all the editblocks...
            Graphics g=e.Graphics;
            g.Clear(Color.White);
            if (gridToolStripMenuItem.Checked)
            {
                //draw the grid.
                for (float X = 0; X < PicEditor.Width; X += gridSize.Width)
                {
                    for (float Y = 0; Y < PicEditor.Height; Y += gridSize.Height)
                    {
                        g.FillRectangle(new SolidBrush(Color.Black), X, Y, 1, 1);


                    }





                }



            }
            SelectionBrush = new SolidBrush(Color.FromArgb(SelectionBlend, Color.Blue));
            Brush SelectionBallBrush = new SolidBrush(Color.FromArgb(SelectionBlend, Color.Red));
            foreach (Block loopblock in EditBlocks)
            {
                if (SelectedBlocks.Contains(loopblock))
                {
                    Debug.Print("Test");

                }

                loopblock.Draw(g);
                if (SelectedBlocks.Contains(loopblock))
                {
                    //draw a bit over top to show it's selected.
                    g.FillRectangle(SelectionBrush, loopblock.BlockRectangle);

                }

            }
            foreach (cBall loopBall in EditBalls)
            {
                loopBall.Draw(g);
                
                if (SelectedBalls.Contains(loopBall))
                {
                    RectangleF userect = new RectangleF(loopBall.Location.X-loopBall.Radius,loopBall.Location.Y-loopBall.Radius,loopBall.Radius*2,loopBall.Radius*2);
                    g.FillEllipse(SelectionBrush, loopBall.getRect());
                    PointF VelocityPoint = new PointF(loopBall.Location.X + (loopBall.Velocity.X * 5), loopBall.Location.Y + (loopBall.Velocity.Y * 5));

                    g.DrawLine(new Pen(Color.Red, 2), loopBall.Location, VelocityPoint);    

                }


            }
            if (CurrentDragMode == EditDragModeConstants.Drag_Select)
            {
                PointF DragPoint1 = new PointF(Math.Min(SelectDragStart.X, SelectDragEnd.X), Math.Min(SelectDragStart.Y, SelectDragEnd.Y));
                PointF DragPoint2 = new PointF(Math.Max(SelectDragStart.X, SelectDragEnd.X), Math.Max(SelectDragStart.Y, SelectDragEnd.Y));
               
               Rectangle DragRectangle = new Rectangle(new Point((int)DragPoint1.X,(int)DragPoint1.Y), new Size((int)(DragPoint2.X - DragPoint1.X),(int)( DragPoint2.Y - DragPoint1.Y)));
                g.DrawRectangle(new Pen(Color.Black) {DashStyle=DashStyle.Dash},DragRectangle);



            }
        }
       
       // private bool DragSelected=false;
        private EditDragModeConstants CurrentDragMode = EditDragModeConstants.Drag_None;
        private EditObjectConstants CurrentEditMode = EditObjectConstants.Edit_Blocks;
        private Block DragOriginBlock;
        private cBall DragOriginBall;
        private PointF DragOffset;
        private PointF SelectDragStart; private PointF SelectDragEnd;
        private void PicEditor_MouseDown(object sender, MouseEventArgs e)
        {
            PointF FloatPoint = new PointF((float)e.X, (float)e.Y);
           Block touchblock = BCBlockGameState.Block_HitTestOne(EditBlocks, FloatPoint);
           cBall touchball = BCBlockGameState.Ball_HitTestOne(EditBalls, FloatPoint);

           if (touchblock == null && touchball != null)
           {
               CurrentEditMode = EditObjectConstants.Edit_Balls;
           }
           else if (touchblock != null && touchball == null)
           {
               CurrentEditMode = EditObjectConstants.Edit_Blocks;
           }

            if (touchblock == null && touchball==null)
           {
               CurrentDragMode = EditDragModeConstants.Drag_Select;
               SelectDragStart = new PointF(e.X,e.Y);
               return;
           }
           else if (ControlPressed)
           {
               //if control is pressed, remove the block if it exists in the selected list, add it otherwise.
               if (SelectedBlocks.Contains(touchblock))
                   SelectedBlocks.Remove(touchblock);
               else
                   SelectedBlocks.Add(touchblock);


               
           }
           else if(((!SelectedBlocks.Contains(touchblock))&&touchblock!=null) ||
              ( SelectedBlocks.Contains(touchblock) && CurrentEditMode==EditObjectConstants.Edit_Balls))
           {
               CurrentEditMode = EditObjectConstants.Edit_Blocks;
               SelectedBlocks.Clear();
               SelectedBlocks.Add(touchblock);
           }

           else if (touchball != null)
           {
               CurrentEditMode = EditObjectConstants.Edit_Balls;
              // if (!(SelectedBalls.Contains(touchball)))
              
              // {
                   SelectedBalls.Clear();
                   SelectedBalls.Add(touchball);
              // }
              
               


           }


            switch (CurrentEditMode)
           {
               case EditObjectConstants.Edit_Blocks:
                   propgridblock.SelectedObjects = SelectedBlocks.ToArray();
                   break;
               case EditObjectConstants.Edit_Balls:
                   propgridblock.SelectedObjects = SelectedBalls.ToArray();
                   break;
           }
            InitDrag(e.Location);
            //initialize some of the drag info; such as the correct offset.
            



           PicEditor.Invalidate();
           PicEditor.Update();
        }

        
        private bool AltPressed, ControlPressed, ShiftPressed;
        private void frmEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            AltPressed = e.Alt;
            ControlPressed = e.Control;
            ShiftPressed = e.Shift;
        }

        private void PicEditor_Click(object sender, EventArgs e)
        {

        }

        private void levelSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propgridblock.SelectedObject=EditSet;
        }

        private void levelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propgridblock.SelectedObject = EditLevel;
        }

        private void propgridblock_Click(object sender, EventArgs e)
        {

        }

        private void propgridblock_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            //redraw...
            IsDirty=true;
            PicEditor.Invalidate();
            PicEditor.Update();

        }
        private void PopulateDropDownWithBlocks(ToolStripDropDownButton addto,EventHandler eventhandle)
        {
            addto.DropDown.Items.Clear();
            
            foreach (BlockData loopdat in Blockman.BlockInfo)
            {
                //ToolStripItem createitem = new ToolStripItem(loopdat.Usename, loopdat.useBlockImage, eventhandle);
                ToolStripItem createitem = addto.DropDown.Items.Add(loopdat.Usename, loopdat.useBlockImage, eventhandle);
                createitem.Tag = loopdat;
                
            }


        }
        
        private void AddClick(object sender, EventArgs e)
        {
            //add a new block of the appropriate type.
            BlockData dataitem = (BlockData)((ToolStripItem)sender).Tag;
            //create an instance of the block...
            Block createdblock = (Block)Activator.CreateInstance(dataitem.BlockType, (Object)new RectangleF(0, 0, 32, 16));
            EditBlocks.Add(createdblock);
            SelectedBlocks.Clear();
            SelectedBlocks.Add(createdblock);
            propgridblock.SelectedObject = createdblock;
            IsDirty=true;
            PicEditor.Invalidate();
            PicEditor.Update();

        }

        private void DropDownAddBlocks_DropDownOpening(object sender, EventArgs e)
        {
            PopulateDropDownWithBlocks(DropDownAddBlocks, AddClick);
        }

        private void PicEditor_MouseMove(object sender, MouseEventArgs e)
        {
            Rectangle availablearea = PicEditor.ClientRectangle;
            Point UseLocation = e.Location;
            if(UseLocation.X < availablearea.Left) UseLocation = new Point(availablearea.Left,UseLocation.Y);
            if(UseLocation.X > availablearea.Right) UseLocation= new Point(availablearea.Right,UseLocation.Y);
            if(UseLocation.Y < availablearea.Top) UseLocation = new  Point(UseLocation.X,availablearea.Top);
            if(UseLocation.Y > availablearea.Bottom) UseLocation = new Point(UseLocation.X,availablearea.Bottom);
            if (gridToolStripMenuItem.Checked)
            {

                int UseX = ((int)UseLocation.X / (int)gridSize.Width)*(int)gridSize.Width+(int)gridSize.Width;
                int UseY = ((int)UseLocation.Y / (int)gridSize.Height) * (int)gridSize.Height+(int)gridSize.Height;

                UseLocation = new Point(UseX, UseY);

                //UseLocation = new Point(UseLocation.X%(int)gridSize.Width,UseLocation.Y%(int)gridSize.Height);
              


            }
            switch (CurrentDragMode)
            {
                case EditDragModeConstants.Drag_Move:
                    if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                    {
                        if (DraggingBlocks == null || DraggingBlocks.Count == 0)
                            return;
                        //Drag all selected blocks.
                        foreach (DragItemData<Block> dragdata in DraggingBlocks)
                        {
                            float newX = (float) dragdata.OriginOffset.X + UseLocation.X;
                            float newY = (float) dragdata.OriginOffset.Y + UseLocation.Y;
                            if (gridToolStripMenuItem.Checked)
                            {
                                newX = ((int) newX/(int) gridSize.Width)*(int) gridSize.Width;
                                newY = ((int) newY/(int) gridSize.Height)*(int) gridSize.Height;
                            }

                            dragdata.DragItem.BlockRectangle =
                                new RectangleF(newX,
                                               newY,
                                               dragdata.DragItem.BlockRectangle.Width,
                                               dragdata.DragItem.BlockRectangle.Height);

                            IsDirty=true;
                        }
                        PicEditor.Invalidate();
                        PicEditor.Update();
                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                    {
                        if(DraggingBalls==null || DraggingBalls.Count==0)
                            return;
                        //Drag all selected balls
                        foreach (DragItemData<cBall> dragdata in DraggingBalls)
                        {
                            float newX = (float) dragdata.OriginOffset.X+ UseLocation.X;
                            float newY = (float) dragdata.OriginOffset.Y + UseLocation.Y;
                            if (gridToolStripMenuItem.Checked)
                            {
                                newX = ((int)newX / (int)gridSize.Width) * (int)gridSize.Width;
                                newY = ((int)newY / (int)gridSize.Height) * (int)gridSize.Height;

                            }
                            if (gridToolStripMenuItem.Checked) 
                                    dragdata.DragItem.Location = new PointF(newX + dragdata.DragItem.Radius, newY + dragdata.DragItem.Radius);
                            else
                                    dragdata.DragItem.Location = new PointF(newX,newY);
                                
                            


                        }
                        PicEditor.Invalidate();
                        PicEditor.Update();


                    }
                    break;
                case EditDragModeConstants.Drag_Select:
                    propgridblock.Refresh();
                    SelectDragEnd = new PointF(e.X,e.Y);
                    PicEditor.Invalidate();
                    PicEditor.Update();
                    break;
            }
        }

        private void PicEditor_MouseUp(object sender, MouseEventArgs e)
        {
            switch (CurrentDragMode)
            {
                case EditDragModeConstants.Drag_Move:
                    if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                    {
                        CurrentDragMode = EditDragModeConstants.Drag_None;
                        DraggingBlocks = null;
                        
                     
                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                    {
                        CurrentDragMode = EditDragModeConstants.Drag_None;
                        DraggingBalls=null;
                        


                    }
                    PicEditor.Invalidate();
                    PicEditor.Update();
                    break;
                case EditDragModeConstants.Drag_Select:
                      CurrentDragMode = EditDragModeConstants.Drag_None;
                        RectangleF DragRectangle;

                        PointF DragPoint1 = new PointF(Math.Min(SelectDragStart.X, SelectDragEnd.X),
                                                    Math.Min(SelectDragStart.Y, SelectDragEnd.Y));
                        PointF DragPoint2 = new PointF(Math.Max(SelectDragStart.X, SelectDragEnd.X),
                                                       Math.Max(SelectDragStart.Y, SelectDragEnd.Y));
                        DragRectangle = new RectangleF(DragPoint1,
                                                       new SizeF(DragPoint2.X - DragPoint1.X,
                                                                 DragPoint2.Y - DragPoint1.Y));
                        SelectedBlocks.Clear();
                        SelectedBalls.Clear();
                    if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                    {
                        
                        SelectedBlocks = BCBlockGameState.Block_HitTest(EditBlocks, DragRectangle, false);
                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                    {
                        
                        
                        SelectedBalls = BCBlockGameState.Ball_HitTest(EditBalls, DragRectangle);
                        
                    }
                        PicEditor.Invalidate();
                    PicEditor.Update();
                    break;

                case EditDragModeConstants.Drag_None:
                    if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                    {
                        Block touchblock = BCBlockGameState.Block_HitTestOne(EditBlocks,
                                                                             new PointF((float) e.X, (float) e.Y));
                        SelectedBlocks.Clear();
                        SelectedBlocks.Add(touchblock);
                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                    {
                        cBall touchball = BCBlockGameState.Ball_HitTestOne(EditBalls, new PointF(e.X, e.Y));
                        SelectedBalls.Clear();
                        SelectedBalls.Add(touchball);


                    }
                    break;
            }
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridToolStripMenuItem.Checked = !gridToolStripMenuItem.Checked;
            PicEditor.Invalidate();
            PicEditor.Update();
        }
        private void DoSave()
        {
            ApplyBlocks();
            ApplyLevel();
            if (LevelFileName == "")
            {
                DoSaveAs();
                return;
            }
            IsDirty=false;
            EditSet.Save(LevelFileName);
            this.Text = "BASeBlock Editor - " + Path.GetFileName(LevelFileName);


        }
        private void DoSaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = BCBlockGameState.GetFileFilter();
            sfd.Title = "Save Levelset";
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.Cancel)
            {
                LevelFileName = sfd.FileName;
                DoSave();


            }




        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void behavioursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(SelectedBalls==null&& SelectedBalls.Count==0) return;
            BallBehave usebehave = new BallBehave(SelectedBalls[0]);
            usebehave.ShowDialog();
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
            
        }

        

       

        private void AddBall_Click(object sender, EventArgs e)
        {
            EditBalls.Add(new cBall(new PointF(PicEditor.ClientSize.Width / 2, PicEditor.ClientSize.Height / 2), new PointF(2, 2)));
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Delete the currently selected blocks if blockedit mode is enabled;
            //otherwise delete the currently selected balls (if balledit mode is enabled)
            if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
            {
                //remove currently selected blocks from editblocks collection
                foreach (Block removeblock in SelectedBlocks)
                {
                    EditBlocks.Remove(removeblock);

                }
                SelectedBlocks.Clear();

            }
            else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
            {
                foreach (cBall removeball in SelectedBalls)
                {
                    EditBalls.Remove(removeball);

                }
                SelectedBalls.Clear();
            }
        }

        private void cboGenBlockType_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void cboGenBlockType_DropDown(object sender, EventArgs e)
        {
            //populate with all available block types
            if (cboGenBlockType.Items.Count == 0)
            {
                foreach (BlockData loopdat in Blockman.BlockInfo)
                {
                    //ToolStripItem createitem = new ToolStripItem(loopdat.Usename, loopdat.useBlockImage, eventhandle);
                    cboGenBlockType.Items.Add(new BlockTypeContainer(loopdat));



                }
            }
        }

      

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //remove all Blocks
            SelectedBlocks.Clear();
            EditBlocks.Clear();

            //get the appropriate Type...
            Type GenerateBlock = ((BlockTypeContainer)cboGenBlockType.SelectedItem).Data.BlockType;
            int useWidth = Int32.Parse(UpDownWidth.Text);
            int useHeight = Int32.Parse(UpDownHeight.Text);
            int numblocks = Int32.Parse(GenBlockNumber.Text);
            int CurrX=0, CurrY=0;

            for (int cblock = 1; cblock < numblocks; cblock++)
            {
                RectangleF newBlockRect = new RectangleF(CurrX,CurrY,useWidth,useHeight);

                Block AddBlock = (Block)Activator.CreateInstance(GenerateBlock, newBlockRect);
                EditBlocks.Add(AddBlock);
                CurrX+=useWidth;
                if (CurrX >= PicEditor.ClientSize.Width)
                {
                    CurrY += useHeight;
                    CurrX = 0;
                }
                PicEditor.Invalidate();
                PicEditor.Update();
            }








        }




        private class BlockTypeContainer
        {
            public BlockData Data;
            public BlockTypeContainer(BlockData loopdat)
            {
                Data = loopdat;
            }
            public override string ToString()
            {
                return Data.Usename;
            }
        }

      
    }
}
