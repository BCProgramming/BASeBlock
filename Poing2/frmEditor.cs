/*
 * BASeCamp BASeBlock
Copyright (c) 2011, Michael Burgwin
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:

    Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
    Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in the documentation and/or other materials provided with the distribution.
    Neither the name of BASeCamp Corporation nor the names of its contributors may be used to endorse or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * */
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Html;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Xml.Linq;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Templates;
using BASeCamp.Elementizer;
using OfficePickers.ColorPicker;
using vbAccelerator.Components.Controls;

namespace BASeCamp.BASeBlock
{

    

    public partial class frmEditor : Form,IEditorClient 
    {

        public enum DrawGridModeConstants
        {
            DrawGrid_None,
            DrawGrid_Dots,
            DrawGrid_Lines




        }

        public enum EditorStateConstants
        {
            EditState_Ready, //ready for action.
            EditState_Loading, //loading a file.
            EditState_Writing //writing a file


        }

        [Serializable]
        public class DrawGridData : IXmlPersistable
        {
            private DrawGridModeConstants _GridMode = DrawGridModeConstants.DrawGrid_Dots;
            private SizeF _GridSize= new SizeF(33,16);
            private PointF _Offset = new PointF(0, 0);
            private Pen _GridPen = new Pen(Color.Black);
            
            private bool _LockToGrid = false;
            public event Action Changed;
                    private void InvokeChanged()
            {
                var copied = Changed;
                if (copied != null) copied();

                DateTimeOffset f;
                TimeZoneInfo tzi;
                TimeZone tz;
                        
                DateTime ytt= new DateTime();
                
                
                
                

            }
            public XElement GetXmlData(string pNodeName)
            {
                XElement resultNode = new XElement(pNodeName);
                resultNode.Add(new XAttribute("GridMode",(int)GridMode));
                resultNode.Add(StandardHelper.SaveElement(GridSize,"Size"));
                resultNode.Add(StandardHelper.SaveElement(Offset,"Offset"));
                resultNode.Add(new XAttribute("LockToGrid",LockToGrid));
                return resultNode;
            }
            public DrawGridData(XElement source)
            {
                GridMode = (DrawGridModeConstants)source.GetAttributeInt("GridMode");
                GridSize = source.ReadElement<SizeF>("Size");
                Offset = source.ReadElement<PointF>("Offset");
                LockToGrid = source.GetAttributeBool("LockToGrid", false);
            }

            public DrawGridModeConstants GridMode { set { _GridMode = value; InvokeChanged(); } get { return _GridMode; } }

            public SizeF GridSize { set { _GridSize = value; InvokeChanged(); } get { return _GridSize; } }

            [TypeConverter(typeof(FloatFConverter))]
            public PointF Offset { set { _Offset = value; InvokeChanged(); } get { return _Offset; } }

            public bool LockToGrid { get { return _LockToGrid; } set { _LockToGrid = value; } }
                [EditorBrowsable(EditorBrowsableState.Advanced)]
            public Pen GridPen { set { _GridPen = value; InvokeChanged(); } get { return _GridPen; } }

                public DrawGridData()
            {
                //empty...

            }
                public void GetObjectData(SerializationInfo info, StreamingContext context)
                {
                    info.AddValue("GridSize", GridSize);
                    info.AddValue("Offset", Offset);
                    info.AddValue("GridMode", _GridMode);


                }
                public DrawGridData(SerializationInfo info, StreamingContext context)
                {
                    GridSize = (SizeF)info.GetValue("GridSize", typeof(SizeF));
                    Offset = (PointF)info.GetValue("Offset", typeof(PointF));
                    GridMode = (DrawGridModeConstants)info.GetValue("GridMode", typeof(DrawGridModeConstants));


                }
            public DrawGridData(DrawGridModeConstants pGridMode,
                                   SizeF pGridSize,
                PointF pOffset)
            {

                _GridMode=pGridMode;
                _GridSize = pGridSize;
                _Offset = pOffset;
                


            }
            private void Validate()
            {
                if(GridSize.Width <=0 || GridSize.Height<= 0)
                {
                    GridSize = new SizeF(33, 16);
                }
            }
            public void Draw(Size totalsize,Graphics g)
            {
                Validate();
                //draw the grid.
                switch(_GridMode)
                {
                    case DrawGridModeConstants.DrawGrid_Dots:

                        for (float X = Offset.X; X < totalsize.Width; X += GridSize.Width)
                        {
                            for (float Y = Offset.Y; Y < totalsize.Height; Y += GridSize.Height)
                            {
                                g.FillRectangle(new SolidBrush(Color.Black), X, Y, 1, 1);
                            }
                        }
                        break;
                    case DrawGridModeConstants.DrawGrid_Lines:
                        //this can be made a lot faster...
                        for (float X = Offset.X; X < totalsize.Width+GridSize.Width; X += GridSize.Width)
                        {
                            for (float Y = Offset.Y; Y < totalsize.Height+GridSize.Height; Y += GridSize.Height)
                            {
                                g.DrawRectangle(_GridPen, X, Y, GridSize.Width, GridSize.Height);
                            }
                        }
                        break;



                        break;
                    case DrawGridModeConstants.DrawGrid_None:
                        //do nothing...
                        break;
                }


            }


        
        }
        private EditorStateConstants _editorState;

        protected EditorStateConstants editorState
        {
            get { return _editorState; }
            set { _editorState = value;
            Enabled = _editorState != EditorStateConstants.EditState_Ready;



            
            }
        }
        public BBEditorUndo<EditorUndoStackItem> UndoObject = new BBEditorUndo<EditorUndoStackItem>(BCBlockGameState.Settings.EditorUndoStackSize);
        private DrawGridData ddgriddata = new DrawGridData();
        private class PicEditorMessageHook : IMessageFilter
        {

            private frmEditor Owner=null;
            private PictureBox picEditorsubclassed;
            public PicEditorMessageHook(frmEditor pOwner)
            {
                Owner = pOwner;
                picEditorsubclassed=pOwner.PicEditor;
                Application.AddMessageFilter(this);

            }
            ~PicEditorMessageHook()
            {
                Application.RemoveMessageFilter(this);

            }

            #region IMessageFilter Members

            public bool PreFilterMessage(ref Message m)
            {
                Debug.Print("Prefilter:" + m.Msg);
                if (ActiveForm==Owner && Owner.mouseinPic)
                {
                    Owner.WndProc(ref m);
                    return true;
                }
                else
                {
                    return true;
                }
            }

            #endregion
        }
        private FloatControl TipFloater;
        internal bool mouseinPic=false;
        //private PicEditorMessageHook PicHook;
        public enum EditDragModeConstants
        {
            Drag_None,
            Drag_Move,
            Drag_Select, //Draw selection box.
            Drag_SelectCircle, //Draw Selection Circle
            Drag_Paint //paint in all blocks with the selected block
        }
        public enum DragTypeConstants
        {
            Drag_SelectMove=0,
            Drag_Paint=1


        }
        [Flags]
        public enum EditObjectConstants
        {
            Edit_Blocks = 1,
            Edit_Balls = 2,
            Edit_Paths=3, //Edits paths. This allows the selection and movement of points in the selected path.
            
            Edit_DrawPath=4 //draws a path, this adds points for each click to the active Path (SelectedPath)

        }
        
        private struct DragItemData<T>
        {
            public T DragItem;
            public PointF OriginOffset;
        }

        //private Thread RefreshThread;
        
        private ImageList imlLevelThumbs = new ImageList(); //level thumbnails imagelist.
        private String LevelFileName = "";
        private bool _IsDirty;
        private BlockData? SelectedBlockType;
       // private Color SelectedFillColor = Color.Blue;
       // private Color SelectedPenColor = Color.Black;
        private Brush SelectionBrush = new HatchBrush(HatchStyle.Trellis, Color.Black);

        public Color SelectedFillColor
        {
            get {
                if(ToolStripFillColor.Tag==null)
                    return Color.Red;
                else
                return (Color)ToolStripFillColor.Tag; 
            
            
            
            }

            set {
            ToolStripFillColor.Tag = value;
                ((ToolStripColorPicker)ToolStripFillColor).Color=value;
            //ToolStripFillColor.Image = ColorToBitmap(ToolStripFillColor);
            
            }

        }
        public Color SelectedPenColor
        {
            get { 
                if(ToolStripPenColor.Tag==null)
                    return Color.Black;
                else
                    return (Color) ToolStripPenColor.Tag; 
            }
            set { ToolStripPenColor.Tag = value;
            ((ToolStripColorPicker)ToolStripPenColor).Color = value;
            //ToolStripPenColor.Image = ColorToBitmap(ToolStripFillColor);
            }




        }


            /// <summary>
        /// Sets/returns the dirty bit of the loaded file. This is the "global" dirty- that is, wether the file is changed.
        /// </summary>
        
        public bool IsDirty
        {
            get { return _IsDirty; }
            set
            {
                _IsDirty = value;
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
        private List<DragItemData<ObjectPathDataPoint>> DraggingPathPoints = new List<DragItemData<ObjectPathDataPoint>>();
       
        private struct BlockDrawEffectData
        {
            Block blocktoeffect;
            Color ColorMix; 


        }
        private List<BlockDrawEffectData> EffectBlocks = new List<BlockDrawEffectData>();
        private BlockDataManager Blockman = BCBlockGameState.BlockDataMan;
        public List<Block> SelectedBlocks = new List<Block>();
        public List<cBall> SelectedBalls = new List<cBall>();
        public ObjectPathData SelectedPath=null; //there can only be one selected path at a time.
        public List<Block> EditBlocks = new List<Block>(); //blocks to edit, the current set of blocks.
        public List<cBall> EditBalls = new List<cBall>();
        
        public EditorSet EditContext = new EditorSet();
        public LevelSet EditSet = null;
        public int EditingLevel = 1;
        public Level EditLevel;
        private Thread RepaintThread; //used currently to attempt to draw blinking selections

        public frmEditor()
        {
            InitializeComponent();
        }

        private void InitBlocks()
        {
            if (EditSet == null)
            {
                EditSet = EditContext.LevelData;

            }
            EditLevel = new Level();
            EditSet.Levels.Add(EditLevel);
            for (int X = 0; X < 128; X += 32)
                EditBlocks.Add(new InvincibleBlock(new RectangleF(X, 0, 32, 16)));
            EditBalls.Add(new cBall(new PointF(PicEditor.Width/2, PicEditor.Height/2), new PointF(2, -2)));
        }
        private void PerformUndo()
        {
            if (!UndoObject.CanUndo) return;
            var undovalue = UndoObject.Undo();
            EditBlocks = undovalue.StoredBlocks;
            EditBalls = undovalue.StoredBalls;


        }
        private void PerformRedo()
        {

            if (!UndoObject.CanRedo) return;
            var redovalue = UndoObject.Redo();
            EditBlocks = redovalue.StoredBlocks;
            EditBalls = redovalue.StoredBalls;
            
                

        }
        private void PushUndo(String ActionDesc)
        {
            //Pushes the current state of blocks and balls to the undo stack.
            UndoObject.PushChange(new EditorUndoStackItem(EditBlocks, EditBalls,ActionDesc));


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
            EditSet.Levels[EditingLevel - 1] = EditLevel;
            EditContext.LevelData=EditSet;
        }

        private Dictionary<TabPage, Panel> TabPanels = new Dictionary<TabPage, Panel>();
        private ImageList TabSideBarImages;

        private void InitImageList()
        {
            TabSideBarImages = new ImageList();
            TabSideBarImages.ImageSize = new Size(16, 16);
            foreach (KeyValuePair<String, Image> loopimage in BCBlockGameState.Imageman.GetImages())
            {
                TabSideBarImages.Images.Add(loopimage.Key, loopimage.Value);
            }
        }
        private const int WM_KEYDOWN = 0x100;
        private const int WM_KEYUP = 0x101;
        HashSet<Keys> pressedkeys = new HashSet<Keys>();
        /*
        protected override void WndProc(ref Message m)
        {
            Debug.Print("message:" + m.Msg);
            if (m.Msg == WM_KEYUP)
            {
                Debug.Print("Key Released:" + Enum.GetName(typeof(Keys), m.WParam));
                pressedkeys.Remove((Keys) (m.WParam));
                
            }
            else if (m.Msg == WM_KEYDOWN)
            {
                Keys kpressed = (Keys) (m.WParam);
                pressedkeys.Add(kpressed);
                Debug.Print("Key Pressed:" + Enum.GetName(typeof(Keys), m.WParam));
                //AltPressed = e.Alt;
                //ControlPressed = e.Control;
                //ShiftPressed = e.Shift;


                

            }






            base.WndProc(ref m);
        
    }
         * */
        private string getEditorConfigFile()
        {
            String folderuse = BCBlockGameState.AppDataFolder;
            return Path.Combine(folderuse, "editor.config");




        }
        private void LoadEditorConfig()
        {
            String readfrom = getEditorConfigFile();
            if (File.Exists(readfrom))
            {
                using (FileStream readconfig = new FileStream(readfrom, FileMode.Open))
                {
                    try
                    {
                        XDocument loaddoc = XDocument.Load(readconfig);
                        ddgriddata = new DrawGridData(loaddoc.Root);
                    }
                    catch(Exception exx)
                    {
                        Debug.Print("failed to load Editor configuration.");
                        ddgriddata = new DrawGridData(DrawGridModeConstants.DrawGrid_Dots,new SizeF(33,16),PointF.Empty);
                    }

                }


            }

        }
        private void SaveEditorConfig()
        {
            String saveto = getEditorConfigFile();
            using (FileStream outstream = new FileStream(saveto, FileMode.Create))
            {
                XDocument savedoc = new XDocument(ddgriddata.GetXmlData("EditSettings"));
                savedoc.Save(outstream);
            }


        }

        private void frmEditor_Load(object sender, EventArgs e)
        {
            //TipFloater = new FloatControl();

            


            //Force Editor control to be a specific pixel size. Everything else should scale with DPI though.
            PicEditor.Size = new Size(493, 427);
            picLeftPane.Location = new Point(0, toolStrip1.Top);
            picLeftPane.Height = ClientSize.Height-sstripEditor.Height-toolStrip1.Height;
            PicEditor.Location = new Point(picLeftPane.Right, toolStrip1.Bottom);
            tabProperties.Location = new Point(PicEditor.Left, toolStrip1.Bottom);
            sstripEditor.Dock = DockStyle.Bottom;
            ClientSize = new Size(PicEditor.Size.Width + tabProperties.Size.Width, PicEditor.Bottom + sstripEditor.Height);
            tabProperties.Height = ClientSize.Height - toolStrip1.Bottom - sstripEditor.Height;

            //this is now our minimum size.
            MinimumSize = Size;

            //check for Editor.dat in appdata...
            LoadEditorConfig();



            ddgriddata.Changed += ddgriddata_Changed;
            PaintMode.Click += ModeButton_Click;
            //cboGenBlockType.RePopulate();
            selectMoveToolStripMenuItem.Click += new EventHandler(ModeButton_Click);
            InitImageList();

            String defaulteditorfile = BCBlockGameState.Settings.DefaultEditorSet;
            if(defaulteditorfile != "" && File.Exists(BCBlockGameState.Settings.DefaultEditorFilename))
                defaulteditorfile = BCBlockGameState.Settings.DefaultEditorFilename;
            if (defaulteditorfile != "")
            {
                //load EditContext from defaulteditorfile.
                try
                {

                    EditContext = EditorSet.FromFile(defaulteditorfile);

                }
                catch(Exception oo)
                {
                    Debug.Print("Exception:" + oo.Message + " in frmEditor.Load while trying to load defaulteditorfile.");
                    defaulteditorfile = ""; //reset to empty string for the succeeding if() block.
                }

            }
            if(defaulteditorfile=="")
            {
                


                InitBlocks();
            }
            RepaintThread = new Thread(PaintThread);

            RepaintThread.Start();
            BaseMultiEditor.OnEditComplete -= BaseMultiEditor_OnEditComplete;
            BaseMultiEditor.OnEditComplete += new BaseMultiEditor.OnEditCompleteFunc(BaseMultiEditor_OnEditComplete);
            
            loadOfficepickers();
           // PicHook = new PicEditorMessageHook(this);
            ToolStripFillColor.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
            ToolStripFillColor.Image = BCBlockGameState.ToolbarImages["FILL"];
            ToolStripPenColor.Image = BCBlockGameState.ToolbarImages["PEN"];
            chkClearExist.Image = BCBlockGameState.ToolbarImages["unselectall"];
            chkSelectNew.Image = BCBlockGameState.ToolbarImages["selectgenerated"];
            openContainingFolderToolStripMenuItem.Image = BCBlockGameState.ToolbarImages["Folder"];
            saveAsToolStripMenuItem.Image = BCBlockGameState.ToolbarImages["savecopy"];
            saveToolStripMenuItem.Image = BCBlockGameState.ToolbarImages["save"];
            reopenToolStripMenuItem.Image = BCBlockGameState.ToolbarImages["reopen"];
            closeToolStripMenuItem.Image = BCBlockGameState.ToolbarImages["error"];
            undoToolStripMenuItem.Image = BCBlockGameState.ToolbarImages["undo"];
            redoToolStripMenuItem.Image = BCBlockGameState.ToolbarImages["redo"];
            
        }

        void ddgriddata_Changed()
        {
            PicEditor.Invalidate();
            PicEditor.Update();
        }
        private void loadOfficepickers()
        {

            



        }

        void BaseMultiEditor_OnEditComplete(Object[] completededit)
        {
            Debug.Print("BaseMultiEditor event fired in frmeditor...");
            Debug.Print("we were given " + completededit.Length + " items.");
            string propname;
            if(selectedpropgriditem!=null)
            {
                propname=selectedpropgriditem.PropertyDescriptor.Name;
                Debug.Print("Selected property is " + propname);
                //this event is fired by the BaseMultiEditor, as a hack to get the propertygrid to work with editing collection properties on multiple objects at once.
                //the "object[]" array we receive will be 


                //task:iterate through propgridblock.SelectedObjects.
                if (propgridblock.SelectedObjects.Length > 1)
                {
                    foreach (Object selectedcontrol in propgridblock.SelectedObjects)
                    {
                        //with each object:
                        //get a ref to the changed property...
                        PropertyInfo changedproperty = selectedcontrol.GetType().GetProperty(propname);
                        if (changedproperty.PropertyType.GetInterfaces().Contains(typeof (IEnumerable)))
                        {
                            //it's an enumerable type.
                            Debug.Print("type named " + changedproperty.PropertyType.FullName + " is IEnumerable...");
                            //apply completededit to the list. First clear the current values.
                            IList castlist = (IList)selectedcontrol.GetType().GetProperty(propname).GetValue(selectedcontrol,null);
                            //clear the current list...
                            castlist.Clear();
                            //and add all the items to this list; they ought to be the proper type already.
                            foreach (Object addobject in completededit)
                            {
                                castlist.Add(addobject);
                            }
                        }
                        else
                        {
                            Debug.Print("type named " + changedproperty.PropertyType.FullName + " is not IEnumerable...");
                        }

                        


                    }
                }
            }
            //propgridblock.SelectedObjects 

        }

        void ModeButton_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            ToolStripMenuItem clickedbutton = sender as ToolStripMenuItem;
            if(clickedbutton==null) return; //no clue....

            //now, the fun bit; uncheck all the dropdownitems of tstripmode...
            foreach (ToolStripMenuItem loopitem in tstripmode.DropDown.Items)
            {
                loopitem.Checked=false; //uncheck it.


            }
            //check the selected item...
            clickedbutton.Checked=true;
            //and change the caption and tag of tstripmode...
            tstripmode.Tag=clickedbutton.Tag;
            tstripmode.Text=clickedbutton.Text;





        }

        private const int BlendMax = 168, BlendMin = 24;
        private int SelectionBlend = 0;
        private int SelectionBlendDirection = 15;


        private Point? cachedmousepos;
        private TimeSpan tiptimerdelay = new TimeSpan(0,0,0,0,250);
        private DateTime tiptimer;
        private void PerformEditorKeyPress()
        {

            if (!mouseinPic) return;


            pressedkeys.Clear();
            foreach (Keys loopkey in Enum.GetValues(typeof(Keys)))
            {
                if (KeyboardInfo.GetAsyncKeyState((int)loopkey)<0)
                    pressedkeys.Add(loopkey);



            }
            if (pressedkeys.Contains(Keys.Home))
            {
                //get the leftmost block
                
                
            }
            PointF? absolutepos = null;
            PointF? movespeed = null; 
            float XMoveSpeed =0;
            float YMoveSpeed=0;
            const float speeddiff=2;
            if(pressedkeys.Contains(Keys.Up)) YMoveSpeed-=speeddiff;
            if(pressedkeys.Contains(Keys.Down)) YMoveSpeed+=speeddiff;
            if(pressedkeys.Contains(Keys.Left)) XMoveSpeed-=speeddiff;
            if(pressedkeys.Contains(Keys.Right)) XMoveSpeed+=speeddiff;
            if (pressedkeys.Contains(Keys.Delete))
            {
                DoDelete();

            }
            else
            {
                

            



            PointF useOffset = new  PointF(XMoveSpeed,YMoveSpeed);

                if (XMoveSpeed != 0 || YMoveSpeed != 0)
                {
                    movespeed = new PointF(XMoveSpeed, YMoveSpeed);


                }
                if (movespeed != null)
                {
                    useOffset = new PointF(useOffset.X*(ControlPressed ? 5 : 1), useOffset.Y*(ControlPressed ? 5 : 1));
                    if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                    {
                        if (SelectedBlocks == null || SelectedBlocks.Count == 0) return;
                        foreach (Block nudgeblock in SelectedBlocks)
                        {
                            nudgeblock.BlockRectangle = new RectangleF(nudgeblock.BlockRectangle.Left + useOffset.X,
                                                                       nudgeblock.BlockRectangle.Top + useOffset.Y,
                                                                       nudgeblock.BlockRectangle.Width,
                                                                       nudgeblock.BlockRectangle.Height);





                        }



                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                    {
                        if (SelectedBalls == null) return;
                        foreach (cBall nudgeball in SelectedBalls)
                        {
                            nudgeball.Location = new PointF(nudgeball.Location.X + useOffset.X,
                                                            nudgeball.Location.Y + useOffset.Y);


                        }



                    }
                }

            }

            PicEditor.Invoke((MethodInvoker)(() =>
                                                         {
                                                             PicEditor.Invalidate();
                                                             PicEditor.Update();
                                                         }
                ));




        }

        private void PaintThread()
        {
            //change the "SelectionBlend" amount


            while (Visible)
            {
                //added May 20th 2011
                //compare MouseLocation to the cached previous mouse location. If it's different reset the "tiptimer" dateTime. Otherwise
                //if tiptimer exceeds tiptimerdelay call the hover routine.
                if (mouseinPic)
                {

                    //also perform movement of selected items here.
                    PerformEditorKeyPress();



                    if (cachedmousepos == null || (cachedmousepos.Value.X != Mouselocation.X || cachedmousepos.Value.Y!=Mouselocation.Y))
                    {
                        if(cachedmousepos!=null) Debug.Print("Tip: " + cachedmousepos.Value.ToString() + " != " + Mouselocation.ToString());
                        tiptimer = DateTime.Now;

                        cachedmousepos = Mouselocation;

                    }
                    else if (DateTime.Now - tiptimer > tiptimerdelay)
                    {
                        //Hovered();

                    }
                }
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
                    try
                    {
                        PicEditor.Invoke((MethodInvoker)(() =>
                                                              {
                                                                  PicEditor.Invalidate();
                                                                  PicEditor.Update();
                                                              }
                                                         )
                            );
                    }
                    catch(Exception exx)
                    {
                        Trace.WriteLine("PaintThread Error:" + exx.ToString());

                    }
                }
                catch
                {
                    //no code
                }
            }
        }

        private void InitDrag(Point DragStart)
        {
            if ((SelectedBlocks == null || SelectedBlocks.Count == 0) &&
                (SelectedBalls == null || SelectedBalls.Count == 0) && (SelectedPath==null))
                return;
            //initialize the drag operation data.
            CurrentDragMode = EditDragModeConstants.Drag_Move;
            //get the appropriate offset from the lowest X/Y block; the one with the lowest Sum X+Y...

            if (CurrentEditMode == EditObjectConstants.Edit_Paths)
            {
                var foundpaths = (from n in SelectedPath.PathPoints where n.Selected orderby n.Location.X ascending select n);
                if (foundpaths.Any())
                {
                    DragOriginPoint = foundpaths.First();
                    DragOffset = new PointF(DragStart.X - DragOriginPoint.Location.X,
                                            DragStart.Y - DragOriginPoint.Location.Y);
                    DraggingPathPoints = new List<DragItemData<ObjectPathDataPoint>>();
                    foreach (var looppoint in (from n in SelectedPath.PathPoints where n.Selected select n))
                    {
                        DragItemData<ObjectPathDataPoint> newdragdata = new DragItemData<ObjectPathDataPoint>();
                        newdragdata.DragItem = looppoint;
                        newdragdata.OriginOffset = new PointF(looppoint.Location.X - DragStart.X,
                                                              looppoint.Location.Y - DragStart.Y);
                        DraggingPathPoints.Add(newdragdata);


                    }
                }

            }
            if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
            {
                DragOriginBlock =
                    (from n in SelectedBlocks orderby n.BlockRectangle.Left + n.BlockRectangle.Right ascending select n)
                        .First();
                DragOffset = new PointF(DragStart.X - DragOriginBlock.BlockRectangle.Left,
                                        DragStart.Y - DragOriginBlock.BlockRectangle.Top);
                DraggingBlocks = new List<DragItemData<Block>>();
                foreach (Block loopblock in SelectedBlocks)
                {
                    DragItemData<Block> newdragdata = new DragItemData<Block>();
                    newdragdata.DragItem = loopblock;
                    newdragdata.OriginOffset = new PointF(loopblock.BlockRectangle.Left - DragStart.X,
                                                          loopblock.BlockRectangle.Top - DragStart.Y);
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
                    newballdata.OriginOffset = new PointF(loopball.Location.X - DragStart.X,
                                                          loopball.Location.Y - DragStart.Y);
                    DraggingBalls.Add(newballdata);
                }
            }
            //if that worked... LINQ rules.
        }

        private SizeF gridSize = new SizeF(32, 16);
        private bool DoDrawIntroText = false; //will be set to true when the introsequence text is changed...
        private bool mustredrawintro = true;
        private Graphics drawnintro=null;
        private Bitmap drawnintrobitmap = null;
        private PointF ScrollOffset = new PointF(0, 0); //scrolled offset
        private void DrawIntrobitmap(Graphics g)
        {

            if (mustredrawintro || drawnintrobitmap==null)
            {
                mustredrawintro=false;
                EditLevel.CreateIntroBitmap(out drawnintrobitmap, out drawnintro);


            }
            if (DoDrawIntroText)
            {
                Rectangle drawintrorect = BCBlockGameState.CenterRect(PicEditor.ClientRectangle, drawnintrobitmap.Size);

                //Debug.Print("Drawing intro rect:" + drawintrorect.ToString() + " within available area:" + AvailableClientArea.ToString() + " bitmap size is:" + IntroSequenceBitmap.Size.ToString());

                g.DrawImageUnscaled(drawnintrobitmap, drawintrorect.Left, drawintrorect.Top);

            }


        }

        private void PicEditor_Paint(object sender, PaintEventArgs e)
        {
            //redraw all the editblocks...
            Graphics g = e.Graphics;
            g.TranslateTransform(ScrollOffset.X, ScrollOffset.Y);
            g.Clear(Color.White);
            EditLevel.Background.DrawBackground(null, g, PicEditor.ClientRectangle, true);
            ddgriddata.Draw(PicEditor.ClientSize,  g);
            //create the selection colour based on the current number of milliseconds.
            float useHue = ((DateTime.Now.Ticks/55000) % 240);
            Color useselectioncolor = new HSLColor(useHue, 240, 120);

            SelectionBrush = new SolidBrush(Color.FromArgb(SelectionBlend, useselectioncolor));
            Brush SelectionBallBrush = new SolidBrush(Color.FromArgb(SelectionBlend, Color.Red));
            foreach (Block loopblock in EditBlocks)
            {
                if (SelectedBlocks.Contains(loopblock))
                {
                    //Debug.Print("Test");
                }
                if (loopblock is IEditorBlockExtensions)
                {
                    ((IEditorBlockExtensions)loopblock).EditorDraw(g,this);
                }
                else
                {


                    loopblock.Draw(g);
                }
                if (SelectedBlocks.Contains(loopblock) && CurrentEditMode == EditObjectConstants.Edit_Blocks)
                {
                    //draw a bit over top to show it's selected.

                    (loopblock as IEditorBlockExtensions).DrawSelection(SelectionBrush, g,this);
                    

                }
            }
            foreach (cBall loopBall in EditBalls)
            {
                loopBall.Draw(g);

                if (SelectedBalls.Contains(loopBall) && CurrentEditMode == EditObjectConstants.Edit_Balls)
                {
                    RectangleF userect = new RectangleF(loopBall.Location.X - loopBall.Radius,
                                                        loopBall.Location.Y - loopBall.Radius, loopBall.Radius*2,
                                                        loopBall.Radius*2);
                    g.FillEllipse(SelectionBallBrush, loopBall.getRect());
                    PointF VelocityPoint = new PointF(loopBall.Location.X + (loopBall.Velocity.X*5),
                                                      loopBall.Location.Y + (loopBall.Velocity.Y*5));

                    g.DrawLine(new Pen(Color.Red, 2), loopBall.Location, VelocityPoint);
                }
            }
            foreach (var loop in EditSet.PathData)
            {
                loop.Value.Draw(g);

            }

            //paths are "per levelset". This allows complex paths to be used a number of times throughout without any ill effects in the way of redundancy. 
            foreach (var loopit in EditSet.PathData)
            {
                //draw if visible.
                if(loopit.Value.Visible)
                    loopit.Value.Draw(g);

            }



            if (CurrentDragMode == EditDragModeConstants.Drag_Select)
            {
                PointF DragPoint1 = new PointF(Math.Min(SelectDragStart.X, SelectDragEnd.X),
                                               Math.Min(SelectDragStart.Y, SelectDragEnd.Y));
                PointF DragPoint2 = new PointF(Math.Max(SelectDragStart.X, SelectDragEnd.X),
                                               Math.Max(SelectDragStart.Y, SelectDragEnd.Y));

                Rectangle DragRectangle = new Rectangle(new Point((int) DragPoint1.X, (int) DragPoint1.Y),
                                                        new Size((int) (DragPoint2.X - DragPoint1.X),
                                                                 (int) (DragPoint2.Y - DragPoint1.Y)));
                g.DrawRectangle(new Pen(Color.Black) {DashStyle = DashStyle.Dash}, DragRectangle);
            }
            else if (CurrentDragMode == EditDragModeConstants.Drag_SelectCircle)
            {
                PointF CenterPoint = SelectDragStart;
                float Radius = BCBlockGameState.Distance(CenterPoint.X, CenterPoint.Y, SelectDragEnd.X, SelectDragEnd.Y);
                Rectangle DragCircle = new Rectangle((int)(CenterPoint.X - Radius), (int)(CenterPoint.Y - Radius), (int)(Radius * 2), (int)(Radius * 2));

                g.DrawEllipse(new Pen(Color.Black) { DashStyle = DashStyle.Dash }, DragCircle);

            }
            if (DoDrawIntroText)
            {
                DrawIntrobitmap(g); 


            }


            if ((DateTime.Now - LastTipUpdate) < (tipvisibletime + tipfadeouttime + tipfadeintime))
            {
                PaintTip(g);
                


            }
            else
            {
                Tipisvisible = false;
            }

        }
        private enum tipfademode
        {
            tipfadein,
            tipfadeout,
            tipvisible

        }
        private bool Tipisvisible=false;
        private void PaintTip(Graphics g)
        {
            tipfademode tipfade;
            float percent;
            const float maxalphatip = 0.6f;
            TimeSpan currdiff=DateTime.Now - LastTipUpdate ;
            //first step: determine wether we are fading in or out or 100% visible. (100% visible is only 80, btw).
            ImageAttributes Drawattributes = new ImageAttributes();
            if (currdiff < tipfadeintime && !Tipisvisible)
            {
                
                tipfade = tipfademode.tipfadein;
                percent = ((float)currdiff.TotalMilliseconds)/((float)tipfadeintime.TotalMilliseconds);
                //percent *= 0.6f;
                //Debug.Print("Tip fadein," + currdiff.ToString() + " percent:" + percent);
                Drawattributes.SetColorMatrix(ColorMatrices.GetColourizer(1,1,1,percent*maxalphatip));
            }
            else if (currdiff < tipfadeintime + tipvisibletime || (Tipisvisible&&currdiff < tipfadeintime+tipvisibletime))
            {
                //visible...
                //Debug.Print("Tip Visible," + currdiff.ToString());
                tipfade = tipfademode.tipvisible ;
                Tipisvisible=true;
                percent = 1;
                Drawattributes.SetColorMatrix(ColorMatrices.GetColourizer(1, 1, 1, maxalphatip));
            }
            else //fadeout
            {
                Tipisvisible = false;
                tipfade = tipfademode.tipfadeout;
                percent = 1-(float)((currdiff - (tipfadeintime + tipvisibletime)).TotalMilliseconds)/((float)tipfadeouttime.TotalMilliseconds);

                Drawattributes.SetColorMatrix(ColorMatrices.GetColourizer(1, 1, 1, percent * maxalphatip));
               // Debug.Print("Tip fadeout," + currdiff.ToString() + " percent:" + (1-percent));
            }

            int percentheight = (int)(percent * (float)drawoverlayimage.Height);
            CompositingQuality cache = g.CompositingQuality;
            g.CompositingQuality = CompositingQuality.HighQuality;
            g.DrawImage(drawoverlayimage, new Rectangle(PicEditor.Width - drawoverlayimage.Width, PicEditor.Height - percentheight,
                    drawoverlayimage.Width, drawoverlayimage.Height), 0, 0, drawoverlayimage.Width, drawoverlayimage.Height, GraphicsUnit.Pixel,Drawattributes);
            g.CompositingQuality=cache;

        }

        // private bool DragSelected=false;
        private EditDragModeConstants _CurrentDragMode = EditDragModeConstants.Drag_None;

        public EditDragModeConstants CurrentDragMode
        {
            get { return _CurrentDragMode; }

            set
            {
                _CurrentDragMode = value;
                PaintMode.Checked = _CurrentDragMode == EditDragModeConstants.Drag_Paint;
            }
        }


        private EditObjectConstants CurrentEditMode = EditObjectConstants.Edit_Blocks;

        public enum RightClickMenuType
        {
            None,
            Block,
            Ball


        }
        private Block DragOriginBlock;
        private cBall DragOriginBall;
        private ObjectPathDataPoint DragOriginPoint;
        private PointF DragOffset;
        private PointF SelectDragStart;
        private PointF SelectDragEnd;
        private RightClickMenuType contextmenu = RightClickMenuType.None;
        /// <summary>
        /// when true, next mousedown in piceditor will start a custom drag.
        /// </summary>
        private bool EngageCustom = false;
        private Action<RectangleF> CustomRectAction;
        private Action<PointF, float> CustomCircleAction;
        private void PicEditor_MouseDown(object sender, MouseEventArgs e)
        {
            ObjectPathDataPoint touchpoint =null;
            SelectDragStart = new PointF(e.X, e.Y);
            SelectDragEnd = new PointF(e.X, e.Y);
            
            PointF FloatPoint = new PointF((float) e.X, (float) e.Y);
            Block touchblock = BCBlockGameState.Block_HitTestOne(EditBlocks, FloatPoint);
            if (touchblock is EllipseBlock)
            {

                Debug.Print("Debug");

            }
            cBall touchball = BCBlockGameState.Ball_HitTestOne(EditBalls, FloatPoint);
            if (SelectedPath == null)
            {
                var SelectedPathList = EditSet.PathData.HitTest(FloatPoint, 4);
                if (SelectedPathList.Count > 0) SelectedPath = EditSet.PathData.FindDataForPoint(SelectedPathList.First());



            }
            if(SelectedPath!=null) touchpoint = SelectedPath.HitTest(FloatPoint, 4);
            
          


            if (PaintMode.Checked)
            {
                CurrentDragMode = EditDragModeConstants.Drag_Paint;
                PicEditor_MouseMove(sender, e);
                return;
            }

            if (EngageCustom)
            {
                
                if (KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey) < 0)
                {
                    CurrentDragMode = EditDragModeConstants.Drag_SelectCircle;

                }
                else
                    CurrentDragMode = EditDragModeConstants.Drag_Select;
                SelectDragStart = new PointF(e.X, e.Y);
                return;


            }
            //on right-click, we do not want to change the selection. However, for right-clicking balls and blocks we need to remember if we right-clicked a ball or block
            //so we can show the appropriate menu on mouseup for Blocks or balls.
            if (e.Button  == MouseButtons.Right)
            {
                
                if (touchblock != null)
                {
                    CurrentEditMode = EditObjectConstants.Edit_Blocks;
                    CurrentDragMode = EditDragModeConstants.Drag_None;
                    //if the touchedblock is not in the current selection, clear it and set the current selection to the right-clicked block.
                    if(!SelectedBlocks.Contains(touchblock))
                    {
                        SelectedBlocks.Clear();
                        SelectedBlocks.Add(touchblock);
                        
                    }

                    contextmenu = RightClickMenuType.Block; 
                }
                if(touchball!=null)
                {
                    CurrentEditMode = EditObjectConstants.Edit_Balls;
                    CurrentDragMode = EditDragModeConstants.Drag_None;
                    if(!SelectedBalls.Contains(touchball))
                    {
                        SelectedBalls.Clear();
                        SelectedBalls.Add(touchball);

                    }

                    contextmenu = RightClickMenuType.Ball; 
                }
                PicEditor.Invalidate();
                PicEditor.Update();
                return;
            }
            if (touchblock == null && touchball != null)
            {
                PushUndo("Ball Move"); 
                CurrentEditMode = EditObjectConstants.Edit_Balls;
            }
            else if (touchblock != null && touchball == null)
            {
                PushUndo("Block Move"); 
                CurrentEditMode = EditObjectConstants.Edit_Blocks;
            }
            else if (touchpoint != null)
            {
                CurrentEditMode = EditObjectConstants.Edit_Paths;
                if (KeyboardInfo.IsPressed(Keys.ControlKey))
                {
                    touchpoint.Selected = !touchpoint.Selected;

                }
                else
                {

                    /*
                    foreach (var looppoint in SelectedPath.PathPoints)
                    {
                        looppoint.Selected=false;

                    }*/
                    //don't want to unselect them all, we could be starting a drag operation...
                    touchpoint.Selected = true;
                    pointselonmousedown=true;
                    propgridblock.SelectedObject = touchpoint;
                }
                
            }
            else if (touchpoint == null && CurrentEditMode == EditObjectConstants.Edit_Paths)
            {
                //didn't click on a point, but we are in path editing mode. If the right mouse button is pressed, create a new point here.
                if (e.Button == MouseButtons.Right)
                {

                    if (SelectedPath == null)
                    {
                        //no selected path, so create a new one.
                        EditSet.PathData.Add(SelectedPath = new ObjectPathData());


                    }
                    ObjectPathDataPoint createdpoint=null;
                    //create a new point at the given location and add it to the path.
                    SelectedPath.PathPoints.Add(createdpoint=new ObjectPathDataPoint(new PointF(e.X, e.Y)));
                    createdpoint.Selected=true;
                    pointselonmousedown = true;

                }



            }
            if (touchblock == null && touchball == null && touchpoint==null)
            {
                //INFO: we callGetAsyncKeyState twice: the first time will tell us if it was pressed since the last call, the second will tell us wether it is <currently> pressed.
                //we don't care if it was pressed/released since the last call, we want to know what it is doing NOW.

                if (KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey) < 0)
                {
                    CurrentDragMode = EditDragModeConstants.Drag_SelectCircle;
                    
                }
                else
                    CurrentDragMode = EditDragModeConstants.Drag_Select;
                SelectDragStart = new PointF(e.X, e.Y);
                return;
            }
           
            else if (KeyboardInfo.IsPressed(Keys.ControlKey))
            {
                //if control is pressed, remove the block if it exists in the selected list, add it otherwise.
                if (SelectedBlocks.Contains(touchblock))
                    SelectedBlocks.Remove(touchblock);
                else
                    SelectedBlocks.Add(touchblock);

            }

            else if (((!SelectedBlocks.Contains(touchblock)) && touchblock != null) ||
                     (SelectedBlocks.Contains(touchblock) && CurrentEditMode == EditObjectConstants.Edit_Balls &&
                      touchblock != null))
            {
                
                CurrentEditMode = EditObjectConstants.Edit_Blocks;
                SelectedBlocks.Clear();
                SelectedBlocks.Add(touchblock);
            }

            else if (((!SelectedBalls.Contains(touchball) && touchball != null) ||
                      (SelectedBalls.Contains(touchball) && CurrentEditMode == EditObjectConstants.Edit_Blocks &&
                       touchball != null)))
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
                case EditObjectConstants.Edit_Paths:
                    propgridblock.SelectedObject = SelectedPath;
                    break;
            }
            SelectedItemsChanged();
            InitDrag(e.Location);
            //initialize some of the drag info; such as the correct offset.


            PicEditor.Invalidate();
            PicEditor.Update();
        }

        private void SelectedItemsChanged()
        {
            if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
            {
                propgridblock.SelectedObjects = SelectedBlocks.ToArray();
                //update overview to select these blocks.
                RefreshLevelView();
            }
            else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
            {
                propgridblock.SelectedObjects = SelectedBalls.ToArray();
                RefreshLevelView();
            }
            else if (CurrentEditMode==EditObjectConstants.Edit_Paths)
                propgridblock.SelectedObject = SelectedPath;
            RefreshLevelView();

        }

       // private bool AltPressed, ControlPressed, ShiftPressed;

        private bool AltPressed { get { return pressedkeys.Contains(Keys.Alt); } }
        private bool ControlPressed { get { return pressedkeys.Contains(Keys.Control); } }
        private bool ShiftPressed { get { return pressedkeys.Contains(Keys.Shift); } }
            private void PicEditor_Click(object sender, EventArgs e)
        {
            
        }

        private void levelSetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void propgridblock_Click(object sender, EventArgs e)
        {
        }

        private void propgridblock_PropertyValueChanged(object s, PropertyValueChangedEventArgs e)
        {
            StringBuilder messageBoxCS = new System.Text.StringBuilder();
            messageBoxCS.AppendFormat("{0} = {1}", "ChangedItem", e.ChangedItem);
            messageBoxCS.AppendLine();
            messageBoxCS.AppendFormat("{0} = {1}", "OldValue", e.OldValue);
            messageBoxCS.AppendLine();
            Debug.Print(messageBoxCS.ToString() + " PropertyValueChanged Event");


            //redraw...
            if(propgridblock.SelectedObject is Block)
            {
                PicEditor.Invalidate();
                PicEditor.Update();
                Block gotblock = (Block)propgridblock.SelectedObject;
                foreach (Block loopblock in SelectedBlocks)
                {
                    


                }
            }
            else if (propgridblock.SelectedObject is Level)
            {

                mustredrawintro = true;

            }
            IsDirty = true;
            PicEditor.Invalidate();
            PicEditor.Update();
        }

       


        
        private void clickmanyone(Object sender, EventArgs args)
        {
         /*   foreach (var iterateitem in loopdat.getManyToOneAttributeData().AcquiredData)
            {
                ToolStripItem createsubitem = addto.Items.Add(iterateitem.DisplayText, iterateitem.DisplayImage, eventhandle);
                createsubitem.Tag = new Object[] { iterateitem, loopdat,eventhandle };



            }
            */
            //first, get the data from the tag.
            ToolStripItem clickeditem = (ToolStripItem)sender;
            Object[] tagvalue = (Object[])clickeditem.Tag;

            ManyToOneBlockData MultiData = (ManyToOneBlockData)tagvalue[0];
            BlockData capturedata = (BlockData)tagvalue[1];
            EventHandler realclick = (EventHandler)tagvalue[2];


            BlockData bdd = capturedata;
            //duplicate BlockData but set the ManyInstanceData field to the proper ManyToOneAttributeData Item.
            bdd.CreateData = MultiData;

            //HACK!!!! (only works as long as we get one click event per menu open...)
            clickeditem.Tag = bdd;

            realclick(clickeditem,args);
            




        }


   

        private void SelPaint(object sender, EventArgs e)
        {
            if (((ToolStripItem) sender).Tag == null)
            {
                DropDownSelBlock.Text = "None (Clear)";
                DropDownSelBlock.Image = null;
                SelectedBlockType = null;
            }
            else
            {
                BlockData bdata = ((BlockData) ((ToolStripItem) sender).Tag);
                SelectedBlockType = bdata;
                DropDownSelBlock.Text = bdata.Usename;
                DropDownSelBlock.Image = bdata.useBlockImage;
            }
        }
        private void selNull(Object sender, EventArgs e)
        {
            tstripgenerate.Text = "None (Clear)";
            tstripgenerate.Image = null;
            generateblockdata = null;

        }
        private void SelGenerate(ToolStripMenuItem clickeditem, Block blockobject, BlockData blockData, ManyToOneBlockData mtodata)
        {
            
                BlockData bdata = blockData;
            
                tstripgenerate.Text = bdata.Usename;
                tstripgenerate.Image = bdata.useBlockImage;
                generateblockdata=bdata;
            
        }
        private void BlockMenuMouseOver(ToolStripMenuItem clickeditem,Block blockobject,BlockData bdata,ManyToOneBlockData mtodata)
        {

        }
        private void AddClick(ToolStripMenuItem clickeditem, Block blockobject, BlockData bdata, ManyToOneBlockData mtodata)
        {
            //add a new block of the appropriate type.
            
            BlockData dataitem = bdata;
            //create an instance of the block...
            //Block createdblock =
            //    (Block) Activator.CreateInstance(dataitem.BlockType, (Object) new RectangleF(0, 0, 32, 16));
            Block createdblock = dataitem.Instantiate(new RectangleF(0, 0, 32, 16));
            EditBlocks.Add(createdblock);
            SelectedBlocks.Clear();
            SelectedBlocks.Add(createdblock);
            propgridblock.SelectedObject = createdblock;
            IsDirty = true;
            PicEditor.Invalidate();
            PicEditor.Update();
            bringToFrontToolStripMenuItem_Click(bringToFrontToolStripMenuItem, new EventArgs());
        }
        private void BlockMenuitemCallback(ToolStripItem item, BlockData bd)
        {
            //extra code to draw a number on the image.

        }
        private void CategoryMenuCallback(ToolStripMenuItem item, Type t)
        {


        }
        private void hookadded(ToolStripMenuItem addeditem, BlockData addeddata)
        {
            //hook mouseover...
            addeditem.MouseHover += addeditem_MouseHover;
        }

        void addeditem_MouseHover(object sender, EventArgs e)
        {
            ToolStripMenuItem source = sender as ToolStripMenuItem;
            if(source==null || source.Tag==null || (source.Tag as Object[]) == null) return;
            String acquireDescription = source.Text;
            //Debug.Print("MouseOver Item:" + source.Text + " - " + source.Tag.GetType().Name);
            BlockData casted;
            try
            {
                casted = (BlockData)((source.Tag as Object[])[1]);
            }
            catch (NullReferenceException exx) { return; }
            Type acquiredtype = casted.BlockType;
            //retrieve description...
            if (acquiredtype.IsDefined(typeof(BlockDescriptionAttribute),true))
            {
                BlockDescriptionAttribute getdesc = (BlockDescriptionAttribute) acquiredtype.GetCustomAttributes(typeof(BlockDescriptionAttribute), true).First();
                acquireDescription = getdesc.Description;


            }
            


            HTMLTip.Show(acquireDescription, this, source.Owner.Left + source.Bounds.Left + source.Bounds.Width / 2,source.Owner.Top+ source.Bounds.Bottom, 15000);
            
            //tag is MenuItem,BlockData,clickcallback, and ClickDirect.
            // new Object[] { iterateitem, bdata, clickcallback,ClickDirect }
        }
        private void DropDownAddBlocks_DropDownOpening(object sender, EventArgs e)
        {
            if (KeyboardInfo.IsPressed(Keys.ShiftKey))
            {
                //Block.PopulateDropDownWithBlocks(DropDownAddBlocks.DropDown, true, null, AddClick, null, null, true);
                Block.PopulateDropDownWithBlocks(DropDownAddBlocks.DropDown, true, null, hookadded, null, AddClick, true);
            }
            else
            {
                //we need to call a specific overload. the one that accepts a Type doesn't have the extra logic we need.
                //PopulateDropDownWithBlocks(DropDownAddBlocks.DropDown, AddClick, true, CategoryMenuCallback, BlockMenuitemCallback);
                Block.PopulateDropDownWithBlocksCategorized(DropDownAddBlocks.DropDown, true, null, hookadded, null, AddClick, true);
                //DropDownAddBlocks.ShowDropDown();
            }
        }
        
        private Point Mouselocation;
        private void PicEditor_MouseMove(object sender, MouseEventArgs e)
        {
            //ttip.Hide(PicEditor); //hide the tip after <any> mouse movement.
            Mouselocation = e.Location;

            Rectangle availablearea = PicEditor.ClientRectangle;
            Point UseLocation = e.Location;
            if (UseLocation.X < availablearea.Left) UseLocation = new Point(availablearea.Left, UseLocation.Y);
            if (UseLocation.X > availablearea.Right) UseLocation = new Point(availablearea.Right, UseLocation.Y);
            if (UseLocation.Y < availablearea.Top) UseLocation = new Point(UseLocation.X, availablearea.Top);
            if (UseLocation.Y > availablearea.Bottom) UseLocation = new Point(UseLocation.X, availablearea.Bottom);


            UseLocation = new Point(UseLocation.X + (int)ScrollOffset.X, UseLocation.Y + (int)ScrollOffset.Y);
            /*if (ddgriddata.LockToGrid)
            {
                int UseX = ((int)UseLocation.X / (int)ddgriddata.GridSize.Width) * (int)ddgriddata.GridSize.Width + ((int)ddgriddata.GridSize.Width/2);
                int UseY = ((int)UseLocation.Y / (int)ddgriddata.GridSize.Height) * (int)ddgriddata.GridSize.Height + ((int)ddgriddata.GridSize.Height/2);

                UseLocation = new Point(UseX, UseY);

                //UseLocation = new Point(UseLocation.X%(int)gridSize.Width,UseLocation.Y%(int)gridSize.Height);
            }*/
            switch (CurrentDragMode)
            {
                case EditDragModeConstants.Drag_Move:
                    if(CurrentEditMode == EditObjectConstants.Edit_Paths)
                    {
                        if (DraggingPathPoints == null || DraggingPathPoints.Count == 0)
                        {
                            return;


                        }
                        foreach (DragItemData<ObjectPathDataPoint> dragdata in DraggingPathPoints)
                        {
                            float newX = dragdata.OriginOffset.X - UseLocation.X;
                            float newY = dragdata.OriginOffset.Y - UseLocation.Y;
                            if (ddgriddata.LockToGrid)
                            {
                                //newX = ((int)newX / (int)gridSize.Width) * (int)gridSize.Width;
                                //newY = ((int)newY / (int)gridSize.Height) * (int)gridSize.Height;

                                //newX = (int)((Math.Round(newX) / gridSize.Width) * Math.Floor(gridSize.Width));
                                //newY = (int)((Math.Round(newY) / gridSize.Height) * Math.Floor(gridSize.Height));
                                newX = ((newX - ddgriddata.Offset.X) % ddgriddata.GridSize.Width);// +ddgriddata.Offset.X;
                                newY = ((newY - ddgriddata.Offset.Y) % ddgriddata.GridSize.Height);// +ddgriddata.Offset.Y;
                            }
                            dragdata.DragItem.Location = new PointF(newX, newY);
                            IsDirty = true;
                        }
                        
                        PicEditor.Invalidate();
                        PicEditor.Update();
                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                    {
                        if (DraggingBlocks == null || DraggingBlocks.Count == 0)
                            return;
                        //Drag all selected blocks.
                        foreach (DragItemData<Block> dragdata in DraggingBlocks)
                        {
                          float newX,newY;
                          if (ddgriddata.LockToGrid)
                          {
                              //newX = ((int) newX/(int) gridSize.Width)*(int) gridSize.Width;
                              //newY = ((int) newY/(int) gridSize.Height)*(int) gridSize.Height;
                              int xuse = UseLocation.X + (int)dragdata.OriginOffset.X;
                              int yuse = UseLocation.Y + (int)dragdata.OriginOffset.Y;
                              Point gotpoint = BCBlockGameState.ClampToGrid(new Point((int)xuse, (int)yuse),
                                  gridSize.ToSize(),new Point((int)ddgriddata.Offset.X,(int)ddgriddata.Offset.Y));
                              newX = gotpoint.X;
                              newY = gotpoint.Y;
                          }
                          else
                          {
                              
                              newX = (float)dragdata.OriginOffset.X + UseLocation.X;
                              newY = (float)dragdata.OriginOffset.Y + UseLocation.Y;
                          }

                            if (newX != 0 && newY != 0)
                            {
                                Debug.Print("break");

                            }
                            dragdata.DragItem.BlockRectangle =
                                new RectangleF(newX,
                                               newY,
                                               dragdata.DragItem.BlockRectangle.Width,
                                               dragdata.DragItem.BlockRectangle.Height);

                            IsDirty = true;
                        }
                        PicEditor.Invalidate();
                        PicEditor.Update();
                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                    {
                        if (DraggingBalls == null || DraggingBalls.Count == 0)
                            return;
                        //Drag all selected balls
                        foreach (DragItemData<cBall> dragdata in DraggingBalls)
                        {
                            float newX = (float) dragdata.OriginOffset.X + UseLocation.X;
                            float newY = (float) dragdata.OriginOffset.Y + UseLocation.Y;
                            if (gridToolStripMenuItem.Checked)
                            {
                                newX = ((int) newX/(int) gridSize.Width)*(int) gridSize.Width;
                                newY = ((int) newY/(int) gridSize.Height)*(int) gridSize.Height;
                            }
                            if (gridToolStripMenuItem.Checked)
                                dragdata.DragItem.Location = new PointF(newX + dragdata.DragItem.Radius,
                                                                        newY + dragdata.DragItem.Radius);
                            else
                                dragdata.DragItem.Location = new PointF(newX, newY);
                        }
                        PicEditor.Invalidate();
                        PicEditor.Update();
                    }
                    break;
                case EditDragModeConstants.Drag_Paint:
                    //if we are in Path editing "mode" ignore everything else, and draw paths.
                    if (CurrentEditMode == EditObjectConstants.Edit_DrawPath)
                    {
                        //if SelectedPath is null, create a new path.
                        if (SelectedPath == null)
                        {
                            SelectedPath = new ObjectPathData();


                        }
                        SelectedPath.AppendPoint(UseLocation).Selected = true;


                    }
                    else
                    {

                        //are we over top of a block?
                        if (e.Button == System.Windows.Forms.MouseButtons.Left)
                        {
                            List<Block> HitBlocks = BCBlockGameState.Block_HitTest(EditBlocks, e.Location);
                            if (HitBlocks == null || HitBlocks.Count > 0)
                            {
                                foreach (Block loopblock in HitBlocks)
                                {
                                    //if selmode is null, merely remove this block
                                    if (SelectedBlockType == null)
                                    {
                                        EditBlocks.Remove(loopblock);
                                    }
                                    else
                                    {
                                        // if (loopblock.GetType() != SelectedBlockType)
                                        // {
                                        RectangleF grabrect = loopblock.BlockRectangle;
                                        EditBlocks.Remove(loopblock);
                                        //Block acquiredblock = (Block)Activator.CreateInstance(SelectedBlockType, grabrect);
                                        Block acquiredblock = SelectedBlockType.Value.Instantiate(grabrect);
                                        if (acquiredblock is NormalBlock)
                                        {
                                            ((NormalBlock)acquiredblock).BlockColor = SelectedFillColor;
                                            ((NormalBlock)acquiredblock).PenColor = SelectedPenColor;


                                        }
                                        EditBlocks.Add(acquiredblock);
                                        // }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case EditDragModeConstants.Drag_Select:
                case EditDragModeConstants.Drag_SelectCircle:
                    propgridblock.Refresh();
                    SelectDragEnd = new PointF(e.X, e.Y);
                    if (EngageCustom)
                    {
                        PerformCustomAction(new RectangleF(SelectDragStart,new SizeF(SelectDragEnd.X-SelectDragStart.X,SelectDragEnd.Y-SelectDragStart.Y)));

                    }
                    
                    PicEditor.Invalidate();
                    PicEditor.Update();
                    break;

            }


            

        }
        /// <summary>
        /// redraws  the tip bitmap 
        /// </summary>

        private void UpdateTooltipImage()
        {
            String tiptext = "";
            if (CurrentEditMode == EditObjectConstants.Edit_Paths)
            {
                tiptext += "Editing Path.";
                if (SelectedPath != null)
                {
                    tiptext += "Path '" + SelectedPath.Name + "'\n";
                    var selectedpoints = from pp in SelectedPath.PathPoints where pp.Selected select pp;
                    if (selectedpoints.Count() == 0)
                    {
                        tiptext += "No Points selected.\n";
                    }
                    else if (selectedpoints.Count() == 1)
                    {
                        var firstpoint = selectedpoints.First();
                        tiptext += "Point Selected:\n";
                        tiptext += "Location (" + String.Format("({0:00},{1:00})\n", firstpoint.Location.X, firstpoint.Location.Y);

                    }
                    tiptext += SelectedPath.PathPoints.Count.ToString() + " Points selected.\n";

                }




            }
            else if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
            {
                if (SelectedBlocks.Count == 1)
                {
                    Block blockhittest = SelectedBlocks[0];
                    if (blockhittest != null)
                    {
                        tiptext += "Type:" + blockhittest.GetType().Name + "\n" +
                                   "Location:" + blockhittest.BlockLocation.ToString() + "\n" +
                                   "Size:" + blockhittest.BlockSize.ToString() ;


                        if (blockhittest is IEditorBlockExtensions)
                        {

                            tiptext += "\n" + ((IEditorBlockExtensions)blockhittest).GetToolTipInfo(this);

                        }

                    }
                 
                }
                
                       else
                    {
                        tiptext += SelectedBlocks.Count + " Blocks Selected.";
                    }
                }
            else if (CurrentEditMode ==EditObjectConstants.Edit_Balls)
            {
                if(SelectedBalls.Count == 1)
                {
                    
                    cBall ballhit = SelectedBalls[0];
                    if (ballhit != null)
                    {
                        tiptext += "Location:" + ballhit.Location.ToString();
                        tiptext += "\nRadius:" + ballhit.Radius;
                        tiptext += "\nVelocity:" + ballhit.Velocity;
                    }
                }
                else
                {
                    tiptext += SelectedBalls.Count + " Balls Selected.";
                }
            }
            //texturebrush for background.
            ImageAttributes painttextureattributes = new ImageAttributes();
            painttextureattributes.SetColorMatrix(ColorMatrices.GetColourizer(0.5f,2.0f,0.9f,0.9f));
            //TextureBrush bgbrush = new TextureBrush(tallypicimage, new Rectangle(new Point(0, 0), tallypicimage.Size), painttextureattributes);
            //bgbrush.WrapMode = WrapMode.Tile;
            
            
            drawoverlayimage = new Bitmap(1, 1);
            drawoverlay = Graphics.FromImage(drawoverlayimage);
            SizeF usesize = BCBlockGameState.MeasureString(tiptext,useoverlayfont);
            drawoverlayimage = new Bitmap((int)usesize.Width+20, (int)usesize.Height+20);
            drawoverlay = Graphics.FromImage(drawoverlayimage);
            Brush bgbrush = new LinearGradientBrush(new Point(0,0),new Point(drawoverlayimage.Width,drawoverlayimage.Height),SystemColors.Info,Color.Yellow); 
            drawoverlay.CompositingQuality = CompositingQuality.HighQuality; 
            drawoverlay.Clear(Color.FromArgb(128, Color.Transparent));
            Rectangle overlayrect=new Rectangle(new Point(0, 0), drawoverlayimage.Size);
            drawoverlay.FillRectangle(bgbrush, overlayrect);
            drawoverlay.FillRectangle(new SolidBrush(SystemColors.Info), overlayrect);
            drawoverlay.DrawRectangle(new Pen(SystemColors.InfoText), overlayrect);
            GraphicsPath usepath = new GraphicsPath();
            
            usepath.AddString(tiptext, useoverlayfont.FontFamily, (int)useoverlayfont.Style, useoverlayfont.Size, new Point(10, 10), StringFormat.GenericDefault);
            drawoverlay.FillPath(new SolidBrush(SystemColors.InfoText), usepath);
            //drawoverlay.DrawPath(new Pen(SystemColors.InfoText), usepath);
            LastTipUpdate=DateTime.Now;
            if (TipUpdateThread!=null&&!TipUpdateThread.IsAlive)
            {
                TipUpdateThread = new Thread(TipUpdateRoutine);
                TipUpdateThread.Start();
            }

        }
        private void TipUpdateRoutine()
        {
            while (DateTime.Now - LastTipUpdate < tipfadeintime + tipfadeouttime + tipvisibletime)
            {
                Thread.Sleep(20);
                PicEditor.Invoke((MethodInvoker)(() => { PicEditor.Invalidate();PicEditor.Update(); }));


            }
            Thread.Sleep(5);
            PicEditor.Invoke((MethodInvoker)(() => { PicEditor.Invalidate();PicEditor.Update(); })); //one final call, to make sure the tip is "deleted".

        }
        private void PerformCustomAction(RectangleF borderrect)
        {
            Debug.Print("Custom Action:" + borderrect.ToString());
            if (CustomRectAction != null)
                CustomRectAction(borderrect);

        }

        private void PerformCustomAction(PointF CenterPoint, float Radius)
        {
            if (CustomCircleAction != null)
                CustomCircleAction(CenterPoint,Radius);

        }
        private bool pointselonmousedown = false;
        private Thread TipUpdateThread;
        private DateTime LastTipUpdate;
        private TimeSpan tipfadeintime = new TimeSpan(0, 0, 0, 0,750);
        private TimeSpan tipfadeouttime = new TimeSpan(0, 0, 0, 0,500);
        private TimeSpan tipvisibletime = new TimeSpan(0, 0, 0,2);
        private Font useoverlayfont = new Font("Arial", 14);
        private Bitmap drawoverlayimage;
        private Graphics drawoverlay;
        private void PicEditor_MouseUp(object sender, MouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && contextmenu != RightClickMenuType.None)
            {
                ContextMenuStrip cm = new ContextMenuStrip();
                
                if (contextmenu == RightClickMenuType.Block)
                {
                    cm.Items.Add("Blocks ( " + SelectedBlocks.Count + ") Dropdown.");

                }
                else if (contextmenu == RightClickMenuType.Ball)
                {
                    cm.Items.Add("Balls ( " + SelectedBalls.Count + ") Dropdown.");
                }
                cm.Show(System.Windows.Forms.Cursor.Position);
                return;
            }

            Point uppoint = new Point(e.X+(int)ScrollOffset.X, e.Y+(int)ScrollOffset.Y);

            switch (CurrentDragMode)
            {
                case EditDragModeConstants.Drag_Move:
                    if (CurrentEditMode == EditObjectConstants.Edit_Paths)
                    {
                        CurrentDragMode = EditDragModeConstants.Drag_None;
                        DraggingPathPoints = null;

                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                    {
                        CurrentDragMode = EditDragModeConstants.Drag_None;
                        PushUndo("Move " + DraggingBlocks.Count + " Blocks.");
                        DraggingBlocks = null;
                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                    {
                        CurrentDragMode = EditDragModeConstants.Drag_None;
                        PushUndo("Move " + DraggingBalls.Count + " Balls.");
                        DraggingBalls = null;
                    }
                    else
                    {

                        CurrentDragMode = EditDragModeConstants.Drag_None;

                    }






                    PicEditor.Invalidate();
                    PicEditor.Update();
                    break;
                case EditDragModeConstants.Drag_SelectCircle:
                case EditDragModeConstants.Drag_Select:
                    bool wascirc = (CurrentDragMode == EditDragModeConstants.Drag_SelectCircle);
                    CurrentDragMode = EditDragModeConstants.Drag_None;
                    RectangleF DragRectangle;

                    if (wascirc)
                    {
                        PointF CenterPoint = SelectDragStart;
                        float Radius = BCBlockGameState.Distance(CenterPoint.X, CenterPoint.Y, SelectDragEnd.X, SelectDragEnd.Y);
                        if (EngageCustom)
                        {
                            PerformCustomAction(CenterPoint, Radius);
                            EngageCustom=false;
                        }
                        else
                        {
                            SelectedBlocks.Clear();
                            SelectedBalls.Clear();
                            SelectedBlocks = BCBlockGameState.Block_HitTestCircle(EditBlocks, CenterPoint, Radius);
                            SelectedBalls = BCBlockGameState.Ball_HitTestCircle(EditBalls, CenterPoint, Radius);
                            
                            SelectedItemsChanged();
                        }
                    }
                    else
                    {


                        PointF DragPoint1 = new PointF(Math.Min(SelectDragStart.X, SelectDragEnd.X),
                                                       Math.Min(SelectDragStart.Y, SelectDragEnd.Y));
                        PointF DragPoint2 = new PointF(Math.Max(SelectDragStart.X, SelectDragEnd.X),
                                                       Math.Max(SelectDragStart.Y, SelectDragEnd.Y));
                        DragRectangle = new RectangleF(DragPoint1,
                                                       new SizeF(DragPoint2.X - DragPoint1.X,
                                                                 DragPoint2.Y - DragPoint1.Y));

                        if (EngageCustom)
                        {
                            PerformCustomAction(DragRectangle);
                            EngageCustom=false;
                        }
                        else
                        {


                            SelectedBlocks.Clear();
                            SelectedBalls.Clear();

                            SelectedBlocks = BCBlockGameState.Block_HitTest(EditBlocks, DragRectangle, false);

                            List<ObjectPathDataPoint> allpoints = EditSet.PathData.HitTest(DragRectangle);



                            //unselected all selected points.
                            foreach (var pathloop in EditSet.PathData)
                            {
                                foreach (var pointloop in pathloop.Value.PathPoints)
                                {
                                    pointloop.Selected=false;

                                }

                            }


                            if (allpoints.Count() > 0)
                            {
                                //go with the path as the path of the first item selected.
                                SelectedPath = EditSet.PathData.FindDataForPoint(allpoints.First());
                                //now only allow points that are part of that path to be selected.
                                allpoints =
                                    (from n in allpoints
                                     where EditSet.PathData.FindDataForPoint(n) == SelectedPath
                                     select n).ToList();
                                foreach (var loopit in allpoints)
                                {
                                    loopit.Selected = false;

                                }
                                foreach (var looppoint in allpoints)
                                {
                                    looppoint.Selected = true;

                                }
                                CurrentEditMode = EditObjectConstants.Edit_Paths;
                            }
                            SelectedBalls = BCBlockGameState.Ball_HitTest(EditBalls, DragRectangle);
                            SelectedItemsChanged();
                        }
                    }

                    
                    if (SelectedBlocks.Count > SelectedBalls.Count)
                    {
                        CurrentEditMode = EditObjectConstants.Edit_Blocks;
                    }
                    else
                        {
                            CurrentEditMode=EditObjectConstants.Edit_Balls;
                        }

                    Block touchblock = SelectedBlocks.FirstOrDefault();
                    if (KeyboardInfo.GetAsyncKeyState((int)Keys.ControlKey) != 0 && touchblock!=null)
                    {
                        Debug.Print("Control is pressed...");
                        //select all objects of the same type and color.
                        SelectAllSimilar(touchblock);
                    }

                    if ((SelectedBlocks == null || SelectedBlocks.Count == 0) &&
                        (SelectedBalls == null || SelectedBlocks.Count == 0) &&
                        !((CurrentEditMode == EditObjectConstants.Edit_Paths || CurrentEditMode == EditObjectConstants.Edit_DrawPath)))
                    {
                        //select the level.
                        propgridblock.SelectedObject = EditLevel;

                    }


                    PicEditor.Invalidate();
                    PicEditor.Update();
                    break;
                case EditDragModeConstants.Drag_Paint:
                    //CurrentDragMode = EditDragModeConstants.Drag_None;
                    break;
                case EditDragModeConstants.Drag_None:

                    if (CurrentEditMode == EditObjectConstants.Edit_Paths)
                    {
                        //unselect all, if control is not pressed.
                        if (!KeyboardInfo.IsPressed(Keys.Control) && !pointselonmousedown)
                        {
                            EditSet.PathData.ToAllPoints((q) => { q.Selected = false; });

                        }
                        if (pointselonmousedown) pointselonmousedown = false;


                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                    {
                        Block touchblocke = BCBlockGameState.Block_HitTestOne(EditBlocks,
                                                                             new PointF((float)uppoint.X, (float)uppoint.Y));


                        if (KeyboardInfo.GetAsyncKeyState((int)Keys.ControlKey) != 0 && touchblocke != null)
                        {
                            Debug.Print("Control is pressed...");
                            //select all objects of the same type and color.
                            SelectAllSimilar(touchblocke);
                        }

                        SelectedBlocks.Clear();
                        SelectedBlocks.Add(touchblocke);




                    }
                    else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                    {

                        cBall touchball = BCBlockGameState.Ball_HitTestOne(EditBalls, new PointF(uppoint.X, uppoint.Y));
                        if (touchball != null)
                        {
                            SelectedBalls.Clear();
                            SelectedBalls.Add(touchball);
                        }
                    }
                    break;
            }
            UpdateTooltipImage();
        }

        private void SelectAllSimilar(Block touchblock)
        {
            foreach (Block loopblock in EditBlocks)
            {
                if (loopblock != touchblock)
                {

                    if (touchblock.GetType() == loopblock.GetType())
                    {
                        if (touchblock.GetType() == typeof(NormalBlock))
                        {
                            if (((NormalBlock)touchblock).BlockColor == (((NormalBlock)loopblock).BlockColor))
                            {
                                SelectedBlocks.Add(loopblock);

                            }

                        }
                        else
                        {
                            SelectedBlocks.Add(loopblock);

                        }



                    }




                }



            }
        }
        

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //gridToolStripMenuItem.Checked = !gridToolStripMenuItem.Checked;
            PicEditor.Invalidate();
            PicEditor.Update();
        }
        private void DoSave()
        {
            //call sync routine in a separate thread.
            Thread SaveThread = new Thread(DoSave_Sync);
            SaveThread.Start();


        }
        private void DoSave_Sync()
        {
            ApplyBlocks();
            ApplyLevel();
            if (LevelFileName == "")
            {
                //needs to be invoked (we might be on a separate thread)
                try
                {
                    Invoke((MethodInvoker)(DoSaveAs));
                    return;
                }
                catch (ObjectDisposedException)
                {
                    //shame...
                    return;

                }
            }
            try
            {
                Invoke((MethodInvoker)(() => IsDirty = false));
            }
            catch (InvalidOperationException ioe)
            {
                //don't care.. ignore.

            }
            try
            {
                File.Delete(LevelFileName);
            }
            catch (IOException ioe)
            {
                Invoke((MethodInvoker)(()=>
                MessageBox.Show("Failed to delete Existing File, " + LevelFileName + " (" + ioe.Message + ")"))
                );

            }

            try
            {



                //Save to the "temporary" memorystream, as well.
                //BCBlockGameState.CurrentLevelStream = new MemoryStream();
                //EditContext.Save(BCBlockGameState.CurrentLevelStream);
                BCBlockGameState.SetFromEditor = EditContext;
                Invoke((MethodInvoker)(() =>
                                       {
                                           tsProgress.Minimum = 0;
                                           tsProgress.Maximum = 100;
                                           tsProgress.Value = 0;
                                           tsProgress.Visible = true;
                                       

                tstripstatustext.Text = "Saving To " + Path.GetFileName(LevelFileName);
                EditContext.Save(LevelFileName,FileReadProgress);
                tstripstatustext.Text = "File Saved.";
                                       }));
                /*if(Program.isLicensed)
                    EditContext.Save(LevelFileName);
                else
                {
                    MessageBox.Show(this, "Level data cannot be saved to a file in the unregistered version.", "Unregistered!", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                    return;
                }*/
            }
            catch (IOException ioe)
            {
                MessageBox.Show("Exception while saving to " + LevelFileName + " (" + ioe.Message + ")");

                Invoke((MethodInvoker)(() =>
                    {
                        tstripstatustext.Text = "Save Error:" + ioe.Message;

                    }));

            }

            Invoke((MethodInvoker)(() =>
            {
                PicEditor.Invalidate();
                PicEditor.Update();

                BCBlockGameState.MRU["Editor"].AddToList(LevelFileName);
                SetTitle();
                
                tsProgress.Visible = false;
                RefreshLevelView();
            }));


            
        }
        private void SetTitle()
        {
            this.Text = "BASeBlock Editor - " + Path.GetFileName(LevelFileName);

        }

        private void saveCopyAsToolStripMenuItem1_Click(object sender, EventArgs e)
        {

        }

        private void DoSaveAs()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            String DefaultlsPath = Path.Combine(BCBlockGameState.AppDataFolder, "Levelsets");
            String useopenpath = BCBlockGameState.Settings.LevelsFolder.Count > 0 ? BCBlockGameState.Settings.LevelsFolder.First() : DefaultlsPath;


            if (LevelFileName != "")
                useopenpath = Path.GetDirectoryName(LevelFileName);

            sfd.InitialDirectory = useopenpath;
            sfd.Filter = BCBlockGameState.GetFileFilter();
            sfd.Title = "Save Levelset";
            if (sfd.ShowDialog(this) != System.Windows.Forms.DialogResult.Cancel)
            {
                LevelFileName = sfd.FileName;
                //Also: if a levelset name has not been given, set it to the base name of the file being saved.
                //this is important particularly for highscore saving- we don't want
                //multiple sets working with the same score set.
                if (EditSet.SetName.Equals(LevelSet.DefaultSetName))
                {

                    EditSet.SetName = Path.GetFileNameWithoutExtension(LevelFileName);

                }
                DoSave();
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoSave();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoSaveAs();
        }

        private void behavioursToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (SelectedBalls == null || SelectedBalls.Count == 0) return;
            BallBehave usebehave = new BallBehave(SelectedBalls[0]);
            usebehave.ShowDialog();

        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BCBlockGameState.SetFromEditor = EditContext;
            
            Close();
        }


        private void AddBall_Click(object sender, EventArgs e)
        {
            var addedball=new cBall(new PointF(PicEditor.ClientSize.Width/2, PicEditor.ClientSize.Height/2),
                                    new PointF(2, 2));
            EditBalls.Add(addedball);
            SelectedBalls.Add(addedball);
        }

        private void deleteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //Delete the currently selected blocks if blockedit mode is enabled;
            //otherwise delete the currently selected balls (if balledit mode is enabled)
            mouseinPic = true;
            DoDelete();
            mouseinPic = false;
        }

        private void DoDelete()
        {
            if (!mouseinPic) return;
            PushUndo("Delete"); //push current state for undo, first...
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
            else if (CurrentEditMode == EditObjectConstants.Edit_Paths && SelectedPath!=null)
            {
                List<ObjectPathDataPoint> removepoints = new List<ObjectPathDataPoint>();
                foreach (var looppoint in SelectedPath.PathPoints)
                {
                    if (looppoint.Selected)
                    {
                        var pathobj = EditSet.PathData.FindDataForPoint(looppoint);
                        removepoints.Add(looppoint);
                        
                    }
                    
                    
                }
                foreach (var removeit in removepoints)
                {
                    EditSet.PathData.FindDataForPoint(removeit).PathPoints.Remove(removeit);

                }
                //if selectedpath has no points...
                if (!SelectedPath.PathPoints.Any())
                {
                    //remove it from the list.
                    EditSet.PathData.Remove(SelectedPath);

                }



            }

        }

        private void cboGenBlockType_SelectedIndexChanged(object sender, EventArgs e)
        {
        }

       
        private BlockData? generateblockdata;

        private void btnGenerate_Click(object sender, EventArgs e)
        {
            //remove all Blocks
            if (chkClearExist.Checked)
            {
                SelectedBlocks.Clear();
                EditBlocks.Clear();
            }
            if (chkSelectNew.Checked)
            {
                SelectedBlocks.Clear();

            }
            if(generateblockdata==null) return;
            //get the appropriate Type...
            //Type GenerateBlock = cboGenBlockType.GetSelectedBlockData().Value.BlockType;
            Type GenerateBlock = generateblockdata.Value.BlockType;
            int useWidth = Int32.Parse(UpDownWidth.Text);
            int useHeight = Int32.Parse(UpDownHeight.Text);
            int numblocks = Int32.Parse(GenBlockNumber.Text);
            int CurrX = 0, CurrY = 0;

            for (int cblock = 0; cblock < numblocks; cblock++)
            {
                RectangleF newBlockRect = new RectangleF(CurrX, CurrY, useWidth, useHeight);

                Block AddBlock = (Block) Activator.CreateInstance(GenerateBlock, newBlockRect);
                EditBlocks.Add(AddBlock);
                if (chkSelectNew.Checked) SelectedBlocks.Add(AddBlock);
                CurrX += useWidth;
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
        
       
      
        private void DoCut()
        {
            
            DoCopy();
            DoDelete();
            

        }
        

        private void DoCopy()
        {
            Debug.Print("Copying...");
            if (KeyboardInfo.IsPressed(Keys.ShiftKey))
            {
                //copy level...
                BBClipboard.CopyLevel(EditLevel);

            }
            else if (CurrentEditMode == EditObjectConstants.Edit_Blocks && SelectedBlocks.Count > 0)
                BBClipboard.CopyBlocks(SelectedBlocks);
            else if (CurrentEditMode == EditObjectConstants.Edit_Balls && SelectedBalls.Count > 0)
                BBClipboard.CopyBalls(SelectedBalls);
            

        }
        private void DoRevert()
        {
            EditLevel = EditSet.Levels[EditingLevel];
            PicEditor.Invalidate();
            PicEditor.Refresh();


        }

        private void DoPaste()
        {
            IDataObject dataObj = Clipboard.GetDataObject();
            
            if (dataObj.GetDataPresent(BBClipboard.BlockFormatName))
            {
                SelectedBlocks.Clear();
                if (dataObj.GetData(BBClipboard.BlockFormatName) != null)
                {
                    var acquired  =(dataObj.GetData(BBClipboard.BlockFormatName) as Block[]);
                    if (acquired != null)
                    {

                        PushUndo("Paste " + acquired.Length + " Blocks");

                        SelectedBlocks = acquired.ToList();
                        foreach (Block loopblock in SelectedBlocks)
                        {
                            loopblock.BlockLocation = loopblock.BlockLocation;
                            loopblock.BlockLocation = new PointF(loopblock.BlockLocation.X + 15,
                                                                 loopblock.BlockLocation.Y + 15);
                            EditBlocks.Add(loopblock);


                        }
                        bringToFrontToolStripMenuItem_Click(bringToFrontToolStripMenuItem, null);
                    }
                }

            }
            if (dataObj.GetDataPresent(BBClipboard.BallsFormatName))
            {
                cBall[] gotballs = dataObj.GetData(BBClipboard.BallsFormatName) as cBall[];
                PushUndo("Paste " + gotballs.Length + " Balls");
                SelectedBalls.Clear();
                SelectedBalls = (gotballs).ToList();
                foreach (cBall loopball in SelectedBalls)
                {
                    EditBalls.Add(loopball);



                }

            }
            if (dataObj.GetDataPresent(BBClipboard.LevelFormatName))
            {
                //paste a new level.
                //first, Apply changes and all that, as we would for merely changing to another level...
                ApplyLevel();
                //now, acquire the actual level data from the clipboard. It will simply be a Level...
                Level pastelevel = (dataObj.GetData(BBClipboard.LevelFormatName) as Level);
                Debug.Assert(pastelevel != null, "Assertion: pastelevel is null!");
                //if(pastelevel==null) return;
                //insert at EditingLevel in the collection...
                EditSet.Levels.Insert(EditingLevel, pastelevel);
                EditLevel = pastelevel;
                RefreshLevelView();
                PicEditor.Invalidate();
                PicEditor.Refresh();
                
                    
                



            }
            PicEditor.Invalidate();
            PicEditor.Update();


        }




        private void frmEditor_KeyDown(object sender, KeyEventArgs e)
        {

            
                if (e.KeyCode == Keys.ControlKey)
                {
                    if (this.DraggingBalls != null)
                    {
                        //copy all Dragging Balls, place them in the same position.
                        List<cBall> copied = (from p in this.DraggingBalls select (cBall)p.DragItem.Clone()).ToList();
                        foreach (cBall duplicate in copied)
                        {
                            EditLevel.levelballs.Add(duplicate);
                        }
                    }
                    if(this.DraggingBlocks !=null){
                        List<Block> copied = (from b in this.DraggingBlocks select (Block)b.DragItem.Clone()).ToList();
                        foreach(var iterate in copied){
                            EditLevel.levelblocks.Add(iterate);
                        }
                    }
                }
                
            
            


        }

        private void DropDownSelBlock_DropDownOpening(object sender, EventArgs e)
        {

            //clear out the drop down
            DropDownSelBlock.DropDown.Items.Clear();
            //add "none" item.
            DropDownSelBlock.DropDown.Items.Add("None(Clear)").Click += selNull;
            var separatorobj = DropDownSelBlock.DropDown.Items.Add("-");
            Block.PopulateDropDownWithBlocksCategorized(DropDownSelBlock.DropDown, false, null, null, null, PaintMenuCallback, true);

        }
        private void PaintMenuCallback(ToolStripMenuItem clickeditem, Block blockobject, BlockData bdata, ManyToOneBlockData mtodata)
        {

            

                clickeditem.Checked = true;
                DropDownSelBlock.Text = bdata.Usename;
                DropDownSelBlock.Image = bdata.useBlockImage;
                SelectedBlockType = bdata;
            


        }
     
      

        private void RefreshLevelView()
        {
            //responsible for "refreshing" the view of lvwLevels of the current levelset.

            lvwLevels.Items.Clear();
            lvwLevels.Columns.Clear();
            imlLevelThumbs.Images.Clear();
            imlLevelThumbs = new ImageList();
            imlLevelThumbs.ImageSize= new Size(48,48);
            lvwLevels.Columns.Add("NAME", "Name", 128);
            lvwLevels.Columns.Add("NUMBER", "#", 16);
            lvwLevels.Columns.Add("BLOCKS", "Blocks", 128);
            lvwLevels.LargeImageList = imlLevelThumbs;
            lvwLevels.SmallImageList = imlLevelThumbs;
            int i = 0;
            ApplyBlocks();
            foreach (Level currlevel in EditSet.Levels)
            {
                String imagekeyuse = AddImageThumbnail(currlevel, imlLevelThumbs);
                i++;
                ListViewItem addeditem =
                    lvwLevels.Items.Add(
                        new ListViewItem(new string[]
                                             {currlevel.LevelName,i.ToString(), currlevel.levelblocks.Count.ToString()},imagekeyuse));
                addeditem.Tag = currlevel;
            }
            RefreshBlockView();
           // RefreshOverview();
        }

        private ImageList BlockViewiml = new ImageList();
        private void RefreshBlockView()
        {
            //refresh lvwBlocks to contain the information for this levels blocks.
            /*
            //First, clear the listview...
            lvwBlocks.Items.Clear();
            //now clear the imagelist...
            BlockViewiml.Images.Clear();

            //clear columns, add add them...
            lvwBlocks.Columns.Clear();

            
            
            //Type, Location, Triggers and Events columns...
            lvwBlocks.SmallImageList=BlockViewiml;
            lvwBlocks.LargeImageList = BlockViewiml;

            lvwBlocks.Columns.Add("TYPE", "Type");
            lvwBlocks.Columns.Add("RECT", "Rectangle");
            lvwBlocks.Columns.Add("TRIGGERS", "Triggers");
            lvwBlocks.Columns.Add("EVENTS", "Events");


            //columns added, now iterate through all blocks...
            foreach (Block loopblock in EditBlocks)
            {
                Image thisblockimage = DrawBlockToImage(loopblock, Color.White);
                String createkey = loopblock.GetType().Name + EditBlocks.IndexOf(loopblock);
                BlockViewiml.Images.Add(createkey, thisblockimage);

                ListViewItem lvi = lvwBlocks.Items.Add(new ListViewItem(
                    new String[] 
                    {
                        loopblock.GetType().Name,
                        loopblock.BlockRectangle.ToString(),
                        loopblock.BlockTriggers.Count.ToString(),
                        loopblock.BlockEvents.Count.ToString()
                    },createkey));
                lvi.Tag = loopblock;
                if(SelectedBlocks.Contains(loopblock))
                {

                    lvi.Selected=true;

                }
            else
                {
                    lvi.Selected=false;
                }

            }




            */


        }

        /*
        ImageList imlOverView; //overview icons.



        private List<TreeNode> SelectedOverviewNodes = new List<TreeNode>();


        private void RefreshOverview()
        {
            //refresh contents of tvwOverView.
            tvwOverView.Nodes.Clear();
            imlOverView = new ImageList();
            imlOverView.ImageSize = new Size(32, 32);

            //assume ApplyBlocks() was just called.
            tvwOverView.ImageList=imlOverView;
            SelectedOverviewNodes.Clear();
            
            int levelnum=0;
            foreach (Level currlevel in EditSet.Levels)
            {
                levelnum++;
                //add a new node.
                String usekey=currlevel.LevelName + levelnum.ToString();
                TreeNode tn = tvwOverView.Nodes.Add(usekey,currlevel.LevelName);
                tn.Text = currlevel.LevelName;
                String levelimagekey = AddImageThumbnail(currlevel, imlOverView);

                TreeNode blocksnode = tn.Nodes.Add("BLOCKNODE", "Blocks");
                foreach (Block loopblock in currlevel.levelblocks)
                {


                    String blockimage = AddImageThumbnail(loopblock, imlOverView);

                    TreeNode blocknode = blocksnode.Nodes.Add(loopblock.GetType().Name + currlevel.levelblocks.IndexOf(loopblock),loopblock.GetType().Name,blockimage);
                    
                    if(SelectedBlocks.Contains(loopblock))
                    {
                        blocknode.BackColor = SystemColors.Highlight;
                            blocknode.ForeColor=SystemColors.HighlightText;
                        
                    }

                }




            }




        }
        
        void tvwOverView_DrawNode(object sender, DrawTreeNodeEventArgs e)
        {
            //throw new NotImplementedException();
            
        }
         * */
        /// <summary>
        /// 
        /// Adds a thumbnail image of the given Block to the passed in ImageList; the thumbnail will be sized to the size of the imagelist images.
        /// </summary>
        /// <param name="currlevel">Block to draw</param>
        /// <param name="thumbs">ImageList to add the image to</param>
        /// <returns>Key that the image was added as</returns>
        private String AddImageThumbnail(Block useblock, ImageList thumbs)
        {
            String usekey = "";
            using (Image blockthumbnail = DrawBlockToImage(useblock,Color.White))
            {
                Image thumbnailsize = new Bitmap(thumbs.ImageSize.Width, thumbs.ImageSize.Height);
                Graphics drawthumb = Graphics.FromImage(thumbnailsize);
                drawthumb.Clear(Color.White);
                drawthumb.DrawImage(blockthumbnail, new Rectangle(0, 0, thumbnailsize.Width, thumbnailsize.Height));
                //add thumbnail to the imagelist...
                usekey = useblock.GetType().Name + useblock.BlockRectangle.ToString();
                thumbs.Images.Add(usekey, thumbnailsize);


            }

            return usekey;

        }

        
        /// <summary>
        /// 
        /// Adds a thumbnail image of the given Level to the passed in ImageList; the thumbnail will be sized to the size of the imagelist images.
        /// </summary>
        /// <param name="currlevel">Level to draw</param>
        /// <param name="thumbs">ImageList to add the image to</param>
        /// <returns></returns>
        private String AddImageThumbnail(Level currlevel, ImageList thumbs)
        {
            String useKey = "";
            //throw new NotImplementedException();
            using (Image levelthumbnail = BCBlockGameState.DrawLevelToImage(currlevel,PicEditor.Size))
            {

                //now, we need to resize the image to fit the dimensions of the imagelist...
                Image thumbnailsize = new Bitmap(thumbs.ImageSize.Width, thumbs.ImageSize.Height);
                Graphics drawthumb = Graphics.FromImage(thumbnailsize);
                drawthumb.Clear(Color.White);
                drawthumb.DrawImage(levelthumbnail, new Rectangle(0, 0, thumbnailsize.Width, thumbnailsize.Height));
                //now, add thumbnailsize to the imagelist...
                useKey = currlevel.LevelName + EditSet.Levels.IndexOf(currlevel);
                thumbs.Images.Add(useKey,thumbnailsize);
                

                //dispose of the large image to prevent to much memory use before a GC...

            }
            return useKey;


        }
        private Image DrawBlockToImage(Block drawblock,Color usebgcolor)
        {
            //create bitmap...
            Bitmap imgDraw = new Bitmap((int)drawblock.BlockSize.Width,(int) drawblock.BlockSize.Height);
            //create graphics context from that bitmap...
            Graphics drawgraphics = Graphics.FromImage(imgDraw);
            //clear...
            drawgraphics.Clear(usebgcolor);


            Block cloneblock = (Block)drawblock.Clone();
            cloneblock.BlockLocation = new PointF(0, 0);
            //draw to graphics context...
            cloneblock.Draw(drawgraphics);

            //return resulting image.
            return imgDraw;



        }

    

        private void tabSideBar_Selected(object sender, TabControlEventArgs e)
        {
            if (e.TabPage != null && e.TabPage == tabLevels)
            {
                RefreshLevelView();
            }
        }

        private void lvwLevels_SelectedIndexChanged(object sender, EventArgs e)
        {

            ListView varview = sender as ListView;
            if (varview.SelectedIndices.Count == 0) return;
            int selindex = varview.SelectedIndices[0];
           
            lvlMoveUp.Enabled=lvlMoveDown.Enabled=true;

            if(selindex==0) lvlMoveUp.Enabled=false;
            if(selindex>=varview.Items.Count-1) lvlMoveDown.Enabled=false;
        }

        private void AddLevel_Click(object sender, EventArgs e)
        {
            
        }

        private void lvwLevels_ItemSelectionChanged(object sender, ListViewItemSelectionChangedEventArgs e)
        {
            RemoveLevel.Enabled = (lvwLevels.SelectedItems.Count > 0);
        }

        private void lvwLevels_MouseClick(object sender, MouseEventArgs e)
        {
        }
        private void ChangeLevel(Level newChooseLevel)
        {
            ApplyBlocks();
            ApplyLevel();
            EditLevel = newChooseLevel;
            EditingLevel = EditSet.Levels.IndexOf(newChooseLevel) + 1;
            EditBlocks = newChooseLevel.levelblocks;
            EditBalls = newChooseLevel.levelballs;
            PicEditor.Invalidate();
            PicEditor.Update();

        }

        private void lvwLevels_DoubleClick(object sender, EventArgs e)
        {
            Level newChooseLevel;
            if (lvwLevels.SelectedItems.Count > 0)
            {
                newChooseLevel = (Level) lvwLevels.SelectedItems[0].Tag;
                ChangeLevel(newChooseLevel);  
                
            }
        }

        private void frmEditor_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(TipUpdateThread!=null) TipUpdateThread.Abort();
            RepaintThread.Abort();
            RepaintThread = null;
            TipUpdateThread=null;
        }
        /// <summary>
        /// "audits" (checks for possible issues) within the given Editset. Returns and empty string if no issues are found; otherwise, a 
        /// message that cna be shown to the user for the message.
        /// </summary>
        /// <returns></returns>
        private String AuditEditSet()
        {
            String buildmessage = "";
            ApplyBlocks();
            ApplyLevel();
            int levelnum=1;
            foreach (Level currlevel in EditSet.Levels)
            {
                //current main issues: levels with no ball
                if (currlevel.levelballs.Count == 0)
                {
                    buildmessage += "level " + levelnum.ToString() + " (" + currlevel.LevelName + ") Does not contain any Balls.\n";
                    



                }
                else if (currlevel.levelblocks.Count == 0)
                {

                    buildmessage += "level " + levelnum.ToString() + " (" + currlevel.LevelName + ") Does not contain any Blocks.\n";

                }
                else if (currlevel.levelblocks.Count((w) => w.MustDestroy() == true) == 0)
                {

                    buildmessage += "level " + levelnum.ToString() + " (" + currlevel.LevelName + ") Does not contain Blocks that can be destroyed.\n";

                }

            }

            return buildmessage;

        }
       


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            

            String DefaultlsPath = Path.Combine(BCBlockGameState.AppDataFolder, "Levelsets");
            String useopenpath = BCBlockGameState.Settings.LevelsFolder.Count > 0 ? BCBlockGameState.Settings.LevelsFolder.First() : DefaultlsPath;
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.InitialDirectory = useopenpath;
            ofd.Filter = BCBlockGameState.GetFileFilter();
            ofd.Title = "Open LevelSet";
            String filenameopen;
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                filenameopen=ofd.FileName;
                
                //Add to recent list.
                //BCBlockGameState.PopulateRecentList((ToolStripMenuItem)sender, "Editor", (EventHandler)recent_Click);
                
                try
                {
                    //EditFile(filenameopen,ref EditContext);
                    EditFile_Threaded(filenameopen);
                }
                catch
                {


                }
                RefreshLevelView();

                

            }
            
        }
        private void SetEditSet(EditorSet editthis)
        {
            EditContext = editthis;
            EditSet = EditContext.LevelData;
            //cool, now set editblocks and editballs...
            EditLevel = EditSet.Levels[0];
            EditBlocks = EditLevel.levelblocks;
            EditBalls = EditLevel.levelballs;
            PicEditor.Invalidate();
            PicEditor.Update();

            
            
            SetTitle();
            RefreshLevelView();

        }
        private void SaveProgress(int percentage, ProgressStream.ProgressCallbackStream sender)
        {
            Invoke((MethodInvoker)(() => { tsProgress.Value = percentage; }));
        


    }
        public void FileSaveProgress(float Percentage, long BytesRead, long TotalBytes)
        {
            Invoke((MethodInvoker)(() =>
            {
                tsProgress.Style = ProgressBarStyle.Marquee;
                tsProgress.Invalidate();


            }));


        }
        public void FileReadProgress(float Percentage, long BytesRead, long TotalBytes)
        {

            Invoke((MethodInvoker)(() => {
                try
                {

                    tsProgress.Value = (int)((tsProgress.Maximum - tsProgress.Minimum) * Percentage + tsProgress.Minimum);
                    tsProgress.Invalidate();
                }
                catch(Exception exx)
                {

                }
            }));


        }
        private Thread FileReader=null;
        private void EditFile_Threaded(string filenameopen)
        {
            if (FileReader == null) FileReader = new Thread(FileReadThread);
            lock (FileReader)
            {
                if (FileReader.ThreadState == System.Threading.ThreadState.Running)
                {
                    FileReader.Abort();
                    FileReader.Join();
                }
                FileReader = new Thread(FileReadThread);
                editorState = EditorStateConstants.EditState_Loading;
                FileReader.Start(filenameopen);




            }



        }
        private void FileReadThread(Object param)
        {
            String usefilename = (String)param;
            EditFile(usefilename, ref EditContext);





        }

        private void EditFile(string filenameopen,ref EditorSet contextobject)
        {

            DialogResult confirmation=DialogResult.Cancel;
            Invoke((MethodInvoker)(() => { confirmation = confirmdirty(); }));
            if (confirmation == DialogResult.Cancel) return;


            Invoke((MethodInvoker)(() =>
                                       {
                                           tsProgress.Minimum = 0;
                                           tsProgress.Maximum = 100;
                                           tsProgress.Value = 0;
                                           tsProgress.Style = ProgressBarStyle.Continuous;
                                           tsProgress.Visible = true;
               
            tstripstatustext.Text = "Loading \"" + filenameopen + "\"";
                                       }
            ));
            //set editorstate to reading..
            editorState=EditorStateConstants.EditState_Loading;

            try
            {

                contextobject = EditorSet.FromFile(filenameopen, FileReadProgress);

            }
            catch (Exception eex)
            {
                EditSet = null;
                EditLevel = null;
                EditBlocks = null;
                EditBalls = null;
                //wrap in exception, thread could be being aborted...
                try { Invoke((MethodInvoker)(() => { tstripstatustext.Text = "File Load Failed."; })); }
                catch { }

            }
            finally
            {

            }
            EditSet=contextobject.LevelData;
            //cool, now set editblocks and editballs...
            EditLevel = EditSet.Levels[0];
            EditingLevel = 1;
            EditBlocks = EditLevel.levelblocks;
            EditBalls = EditLevel.levelballs;

            Invoke((MethodInvoker)(()=>
                                       {
                                           PicEditor.Invalidate();
                                           PicEditor.Update();

                                           LevelFileName = filenameopen;
                                           BCBlockGameState.MRU["Editor"].AddToList(LevelFileName);
                                           SetTitle();
                                           tstripstatustext.Text = "File loaded.";
                                           tsProgress.Visible = false;
                                           RefreshLevelView();
                                           BCBlockGameState.MRU["Editor"].AddToList(filenameopen);
                                       }));



        }

        private void DropDownSelBlock_Click(object sender, EventArgs e)
        {
            //nothing.
        }
        private void SetPickerColor(ToolStripButton buttonitem, Color colorset)
        {
            buttonitem.Tag=colorset;


        }

        private void ColorButtonClick(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            ToolStripButton modifybutton = (ToolStripButton)sender;
            if (cd.ShowDialog() == DialogResult.OK)
            {
                Color currcolor = cd.Color;
                modifybutton.Tag = currcolor;

                //change toolstripSelColor image to be a small box of the color- 16x16...

                Bitmap usebitmap = ColorToBitmap(modifybutton);
                modifybutton.Image = usebitmap;
                


                    
            }



        }

        private Bitmap ColorToBitmap(ToolStripButton frombutton)
        {
            Color currcolor = (Color)frombutton.Tag;
            Bitmap usebitmap = new Bitmap(16, 16);
            Graphics drawg = Graphics.FromImage(usebitmap);
            drawg.Clear(Color.White);
            drawg.FillRectangle(new SolidBrush(currcolor), 0, 0, 16, 16);
            drawg.DrawRectangle(new Pen(Color.Black,2), 0, 0, 16, 16);
            return usebitmap;
        }

        private void frmEditor_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
           

        }

        private void cutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoCut();
            
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoCopy();
            
        }

        private void pasteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPaste();
            
        }
        private System.Threading.Timer HoverTimer = null;
        private Point PreviousHoverTimerLocation;
        private Point PreviousHoverFireLocation;
        private void HoverTimerCallback(Object parameter)
        {
            Debug.Print("HoverTimerCallback");
            //what is the current position of the mouse?
            Point CurrentLocation = Mouselocation;

            //if CurrentLocation is within SystemInformation.MouseHoverSize of PreviousHoverTimerLocation, call the routine...
            Debug.Print("Hover Size:" + SystemInformation.MouseHoverSize);
            Debug.Print("Current Position=" + CurrentLocation + " Previous Location= " + PreviousHoverTimerLocation);
            Point pDiff = new Point(Math.Abs(CurrentLocation.X - PreviousHoverTimerLocation.X),
                Math.Abs(CurrentLocation.Y - PreviousHoverTimerLocation.Y));

            Debug.Print("pdiff:" + pDiff);
            if (pDiff.X < SystemInformation.MouseHoverSize.Width &&
                pDiff.Y < SystemInformation.MouseHoverSize.Height)
            {
                //set the previous hover position..
                //call the routine.
                if (PreviousHoverFireLocation != CurrentLocation)
                {
                    PreviousHoverFireLocation = CurrentLocation;

                    PicEditorHover();
                }

            }

            PreviousHoverTimerLocation = CurrentLocation;




        }

        private void PicEditor_MouseEnter(object sender, EventArgs e)
        {
            //for the hover routine, start the Timer.
            if (HoverTimer != null)
                HoverTimer.Dispose();


            



            Debug.Print("PicEditor_MouseEnter...");
            mouseinPic=true;
            propgridblock.Enabled=false;

            //set up the Hover Timer...
            Debug.Print("Setting  Up HoverTimer..." + "(Hovertime:" + SystemInformation.MouseHoverTime + ")");
            HoverTimer = new System.Threading.Timer(HoverTimerCallback, (object)null, 0 , SystemInformation.MouseHoverTime);
        }
        //API declarations for the Window/Cursor testing for tooltip...
        [DllImport("user32")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out Point lpPoint);

        [DllImport("user32.dll")]
        static extern IntPtr WindowFromPoint(int xPoint, int yPoint);


        private IntPtr getMouseControl()
        {
            //task: get the Cursor position, get the window at the cursor position...

            return IntPtr.Zero;


        }


        private void PicEditor_MouseLeave(object sender, EventArgs e)
        {



            try
            {
                Debug.Print("PicEditor_MouseLeave...");
                mouseinPic = false;
                //remove the hover timer, too.
                HoverTimer.Dispose();
                HoverTimer = null;
                propgridblock.Enabled = true;
                //force a repaint...
                PicEditor.Invalidate();
                PicEditor.Update();
            }
            catch {


            }
        }

        private void auditToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String Auditmessage = AuditEditSet();
            if (Auditmessage == "")
                MessageBox.Show("No issues found in loaded Set.");
            else
                MessageBox.Show(Auditmessage);
        }

        private void selectNoneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedBalls.Clear();
            SelectedBlocks.Clear();
        }
        private void BringToFront(IEnumerable<Block> theseblocks)
        {


        }
        private void bringToFrontToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //bring to front: make it the last item in the list (the selected Items depending on edit mode)
            if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
            {
                //remove the selected blocks from the editblocks collection...
                foreach (Block selblock in SelectedBlocks)
                {
                    EditBlocks.Remove(selblock);


                }
                //add them to the end.
                EditBlocks.AddRange(SelectedBlocks);


            }
            else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
            {

                //remove the selected balls from the editballs collection...
                foreach (cBall selball in SelectedBalls)
                {
                    EditBalls.Remove(selball);


                }
                //add them to the end.
                EditBalls.AddRange(SelectedBalls);
                


            }

        }

        private void sendToBackToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //send to back; make them the first items in the list (The selected Items depending on the edit mode)

            if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
            {
                //remove the selected blocks from the editblocks collection...
                foreach (Block selblock in SelectedBlocks)
                {
                    EditBlocks.Remove(selblock);


                }
                //add them to the start
                EditBlocks.InsertRange(0,SelectedBlocks);


            }
            else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
            {

                //remove the selected balls from the editballs collection...
                foreach (cBall selball in SelectedBalls)
                {
                    EditBalls.Remove(selball);


                }
                //add them to the start.
                EditBalls.InsertRange(0,SelectedBalls);



            }

        }
        /// <summary>
        /// determines if the clipboard has a recognized format and provides a string such as "paste ball" or "paste block" etc that represents that data.
        /// 
        /// </summary>
        /// <returns></returns>
        private static String GetPasteText()
        {
            IDataObject dataObj = Clipboard.GetDataObject();
            if (dataObj == null)
                return "Paste";
            else
            {
                if (dataObj.GetDataPresent(BBClipboard.BallsFormatName))
                    return "Paste Ball(s)";
                else if (dataObj.GetDataPresent(BBClipboard.BlockFormatName))
                    return "Paste Block(s)";
                else if (dataObj.GetDataPresent(BBClipboard.LevelFormatName))
                    return "Paste Level";
                else
                {
                    return "Paste";
                }


            }
        }

        private void editToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {

            


            //requires selected blocks:
            bool shouldenable = (CurrentEditMode==EditObjectConstants.Edit_Blocks && (SelectedBlocks != null && SelectedBlocks.Count > 0) ||
                (CurrentEditMode ==EditObjectConstants.Edit_Balls && (SelectedBalls !=null && SelectedBalls.Count > 0)) ||
                (CurrentEditMode == EditObjectConstants.Edit_Paths && (SelectedPath !=null && SelectedPath.getSelected().Length > 0))
                
                
                );

            selectNoneToolStripMenuItem.Enabled = deleteToolStripMenuItem.Enabled = cutToolStripMenuItem.Enabled =
                copyToolStripMenuItem.Enabled = bringToFrontToolStripMenuItem.Enabled =
                sendToBackToolStripMenuItem.Enabled = forceToBoxToolStripMenuItem.Enabled= shouldenable;

            pathsToolStripMenuItem1.Checked = CurrentEditMode == EditObjectConstants.Edit_DrawPath || CurrentEditMode == EditObjectConstants.Edit_Paths;
            //selectNoneToolStripMenuItem 
            //deleteToolStripMenuItem 
            //cutToolStripMenuItem 
            //copyToolStripMenuItem 
            //bringToFrontToolStripMenuItem 
            //sendToBackToolStripMenuItem 


            pasteToolStripMenuItem.Enabled = BBClipboard.CanPerformPaste();
            pasteToolStripMenuItem.Text = GetPasteText();

            //check Undo:
#pragma warning disable 665 
            //disable the warning, we know we are assigning...

            if (undoToolStripMenuItem.Enabled = UndoObject.CanUndo)
            {
                var topitem = UndoObject.PeekUndo();
                undoToolStripMenuItem.Text = "Undo " + topitem.Description;


            }
            else
            {
                undoToolStripMenuItem.Text = "(Can't Undo)";

            }



            if (redoToolStripMenuItem.Enabled = UndoObject.CanRedo)
            {
                var topitem = UndoObject.PeekRedo();
                redoToolStripMenuItem.Text = "Redo " + topitem.Description;


            }
            else
            {
                redoToolStripMenuItem.Text = "(Can't Redo)";
            }

            undoToolStripMenuItem.DropDown.Items.Clear();
            redoToolStripMenuItem.DropDown.Items.Clear();
            //check if Shift is pressed, if so, add subitems to the redo and undo items.
            if (KeyboardInfo.IsPressed(Keys.ShiftKey))
            {
                undoToolStripMenuItem.DropDown.Items.Add("Ghost");
                redoToolStripMenuItem.DropDown.Items.Add("Ghost");


            }

#pragma warning restore 665
            //requires data on the clipboard:
            //pasteToolStripMenuItem 


        }
        private int PreviousSessionID = 0; //sessionID of previous object.
        private void ImageImportCallback(ImageImport.ImageImportOptions currentoptions)
        {
            Bitmap Imagedata = new Bitmap(currentoptions.ImageData);

            //if necessary rescale..
            if (currentoptions.PreScale.Width != 1 || currentoptions.PreScale.Height != 1)
            {
                Size newsize = new Size((int)(((float)currentoptions.ImageData.Width)*currentoptions.PreScale.Width),
                    (int)(((float)currentoptions.ImageData.Height)*currentoptions.PreScale.Height));

                Bitmap scaleimage = new Bitmap(newsize.Width,newsize.Height);
                Graphics paintto = Graphics.FromImage(scaleimage);
                paintto.CompositingQuality = CompositingQuality.AssumeLinear;
                paintto.DrawImage(Imagedata, new Rectangle(0, 0, newsize.Width, newsize.Height));
                Imagedata = scaleimage;

            }




            float BlockWidth = currentoptions.BlockSize.Width;
            float BlockHeight = currentoptions.BlockSize.Height;

            //probability that a block will spawn orbs is width+height/33+16 percent
            float probability = (BlockWidth + BlockHeight) / (33 + 16);
            bool dospawnorb = false;



            SizeF RenderedSize = new SizeF(BlockWidth * Imagedata.Width, BlockHeight * Imagedata.Height);
            PointF StartOffset = new PointF(PicEditor.ClientSize.Width / 2 - RenderedSize.Width / 2,
                PicEditor.ClientSize.Height / 2 - RenderedSize.Height / 2);


            EditBlocks.Clear();
            if (currentoptions.UseClipBlock == false)
            {
                for (int x = 0; x < Imagedata.Width; x++)
                {
                    for (int y = 0; y < Imagedata.Height; y++)
                    {
                        
                        float currx = StartOffset.X + (x*BlockWidth);
                        float curry = StartOffset.Y + (y*BlockHeight);


                        //create a new block...
                        NormalBlock newblock = new NormalBlock(new RectangleF(currx, curry, BlockWidth, BlockHeight));
                        newblock.BlockColor = Imagedata.GetPixel(x, y);
                        newblock.PenColor = newblock.BlockColor;
                        dospawnorb = BCBlockGameState.rgen.NextDouble() > probability;
                        newblock.DoSpawnOrbs = dospawnorb;
                        if(newblock.BlockColor!=Color.Transparent) //ignore transparent blocks.
                            EditBlocks.Add(newblock);

                    }


                }
            }
            else if (currentoptions.UseClipBlock)
            {
                //uses a clipblock.
                //we need to manually determine how many blocks will be used in the X and Y directions based on
                //the currentoptions.ClipSize 
                //add to the EditSet, the given image.
                String CreatedKeyName = ("ImageGen" + EditingLevel.ToString()).ToUpper();
                if(EditContext.CreateData.SavedImages.ContainsKey(CreatedKeyName))
                {
                    EditContext.CreateData.SavedImages.Remove(CreatedKeyName);
                    

                }
                EditContext.CreateData.SavedImages.Add(CreatedKeyName,
                                                           new CreatorProperties.ImageDataItem(CreatedKeyName, Imagedata));
                    ;
                BCBlockGameState.Imageman.AddImage(CreatedKeyName,Imagedata);
                //ClipSize indicates the <number> of blocks, not their width...
                int BlocksX = currentoptions.ClipSize.Width;
                int BlocksY = currentoptions.ClipSize.Height;
                float clipW = Imagedata.Width/BlocksX;
                float clipH = Imagedata.Height/BlocksY;

                
                
                StartOffset = new PointF(PicEditor.ClientSize.Width / 2 - (BlocksX*BlockWidth) / 2,
                PicEditor.ClientSize.Height / 2 - (BlocksY*BlockHeight) / 2);
                for (int currX = 0; currX < BlocksX ; currX++)
                {

                    for (int currY = 0; currY < BlocksY; currY++)
                    {

                        //each block is a segment of the image.
                        float oX = StartOffset.X + currX*BlockWidth;
                        float oY = StartOffset.Y+currY*BlockHeight;
                        
                        //create the clip block...

                        //float CliporiginX = currX * currentoptions.ClipSize.Width;
                        //float CliporiginY = currY * currentoptions.ClipSize.Height;

                        float CliporiginX =currX* clipW;
                        float CliporiginY=currY*clipH;

                        RectangleF useclip = new RectangleF(CliporiginX,CliporiginY,clipW,clipH);

                        ImageClipBlock createblock = new ImageClipBlock(new RectangleF(oX, oY, BlockWidth, BlockHeight), CreatedKeyName,
                            useclip);
                        dospawnorb = BCBlockGameState.rgen.NextDouble() > probability;
                        createblock.DoSpawnOrbs = dospawnorb;
                        EditBlocks.Add(createblock);

                    }



                }


            }


            PreviousSessionID = currentoptions.SessionID;
            PicEditor.Invalidate();
            PicEditor.Update();



        }

        private void fromImageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //task: create a level from a specified image.
            //rules: we scale the blocks to match the resolution of the image. This can cause some grotesquely weird things but... oh well.

            
            ImageImport.DoImport(this, new SizeF(PicEditor.Size.Width, PicEditor.Size.Height),ImageImportCallback);






        }

        private void setPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propgridblock.SelectedObject=EditContext;
            tabSideBar.SelectTab(tabProperties);
        }

        private void reopenToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            BCBlockGameState.PopulateRecentList((ToolStripMenuItem)sender, "Editor", (EventHandler)recent_Click);


        }

        void recent_Click(object sender, EventArgs e)
        {



            EditFile((String)((ToolStripDropDownItem)sender).Tag,ref EditContext);
        }
        private bool FilterSelected = false;
        private void blockFiltersToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            var addto = blockFiltersToolStripMenuItem;
            addto.DropDownItems.Clear();
            //add the checkbox item first.
            var tsi = addto.DropDownItems.Add("Filter Selected Only") as ToolStripMenuItem;
            tsi.Checked = FilterSelected;
            tsi.CheckOnClick = true;
            tsi.CheckedChanged+=new EventHandler((obj,ev)=>FilterSelected=((ToolStripMenuItem)obj).Checked);
            
            //add a separator.
            addto.DropDownItems.Add(new ToolStripSeparator());
            foreach (Type looptype in BCBlockGameState.FilterPluginManager.ManagedTypes)
            {
                //instantiate...
                iEditBlockFilter varfilter = (iEditBlockFilter)(Activator.CreateInstance(looptype));
                String usename = varfilter.Name;
                String usedescription = varfilter.Description;

                
                ToolStripItem addeditem = addto.DropDownItems.Add(usename);
                addeditem.Tag = varfilter;
                addeditem.Click += new EventHandler(editfilter_Click);






            }
            //FilterPluginManager
        }

        void editfilter_Click(object sender, EventArgs e)
        {
            Object createddataobject;
            iEditBlockFilter callfilter = (iEditBlockFilter)((ToolStripItem)sender).Tag;
            Debug.Print(callfilter.Name + " would be called here...");

            Type createtype = callfilter.GetFilterDataType();
            IFilterDataObject createFilterData = (IFilterDataObject)Activator.CreateInstance(createtype);
            //yay...now edit it...
            ICloneable cloneobject = createFilterData;
            ObjectPropertyEditor.EditObject(this, ref cloneobject);

            if (FilterSelected)
            {
                callfilter.PerformFilter(PicEditor.ClientRectangle, ref SelectedBlocks, ref SelectedBalls, (IFilterDataObject)cloneobject);
            }
            else
            callfilter.PerformFilter(PicEditor.ClientRectangle, ref EditBlocks, ref EditBalls, (IFilterDataObject)cloneobject);



            //callfilter.PerformFilter(EditBlocks, createddataobject);

        }
            private void MoveListItems(ListView uselvw, int MoveDir)
            {
                int useindex = uselvw.SelectedIndices[0];


                int IndexA, IndexB;
                ListViewItem ItemA, ItemB;
                IndexA = useindex;
                ItemA = uselvw.Items[IndexA];
                IndexB = useindex + MoveDir;
                ItemB = uselvw.Items[IndexB];
                BCBlockGameState.SwapListItems(uselvw.Items, ItemA, ItemB);
            }
        private void lvlMoveUp_Click(object sender, EventArgs e)
        {
            //move selected level up.
            int MoveDir=-1;
            
            MoveListItems(lvwLevels, MoveDir);
        }

       

        private void lvlMoveDown_Click(object sender, EventArgs e)
        {
            int MoveDir = 1;

            MoveListItems(lvwLevels, MoveDir);
            //move selected level down.
        }

        private void RemoveLevel_Click(object sender, EventArgs e)
        {
            if(lvwLevels.SelectedItems.Count==0) return;

            foreach (ListViewItem loopitem in lvwLevels.SelectedItems)
            {
                //remove from EditorSet as well as the list.
                EditSet.Levels.Remove((Level)loopitem.Tag);
                lvwLevels.Items.Remove(loopitem);


            }




        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private bool HasAttribute(Type typecheck, Type checkforattribute)
        {
            return (System.Attribute.GetCustomAttributes(typecheck).Any((p) => p.GetType() == checkforattribute || p.GetType().IsSubclassOf(checkforattribute)));

        }
        
        private void newToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {

            /*the "New" submenu will contain:

            Blank
            ----- (separator)
            an additional entry for each Level builder. Shown, again, based on
             the shift key.
            
            
            */
            ToolStripMenuItem mitem = (ToolStripMenuItem)sender;
            mitem.DropDownItems.Clear();
            var blankitem = mitem.DropDownItems.Add("&Blank");
            blankitem.Click += new EventHandler(blankitem_Click);
            var separator = mitem.DropDownItems.Add("-");
            
            Debug.Print("KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey)=" + KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey));

            foreach (Type iteratetype in BCBlockGameState.Levelman.ManagedTypes)
            {
                //attempt to create an instance...
                if ((!(HasAttribute(iteratetype, typeof(InvisibleBuilderAttribute))) || KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey) < 0))
                {
                    iLevelSetBuilder newinstance;
                    try
                    {
                        newinstance = (iLevelSetBuilder)Activator.CreateInstance(iteratetype);
                    }
                    catch
                    {
                        newinstance = null;
                    }

                    //add a new dropdown...
                    if (newinstance != null)
                    {
                        var newdropdown = mitem.DropDownItems.Add(newinstance.getName());
                        newdropdown.Tag = newinstance;
                        newdropdown.Click += new EventHandler(newdropdown_Click);
                    }
                }

            }




        }
        public DialogResult confirmdirty()
        {
            if (IsDirty)
            {
                String usemessage = "The Loaded LevelSet has changed. Save Changes?";
                if (LevelFileName != "")
                    usemessage = "Save changes to \"" + Path.GetFileName(LevelFileName) + "\"?";

                var returnme = MessageBox.Show(usemessage,"Save",MessageBoxButtons.YesNoCancel);
                try
                {
                    if (returnme == DialogResult.Yes)
                    {
                        DoSave();



                    }
                    return returnme;
                }
                catch (Exception e)
                {
                    return DialogResult.Cancel;


                }
            }
            else
            {
                return DialogResult.Yes;
            }




        }

        void blankitem_Click(object sender, EventArgs e)
        {

            //throw new NotImplementedException();
            if (confirmdirty() !=DialogResult.Cancel)
            {
                LevelFileName = "";
                EditContext = new EditorSet();
                EditContext.CreateData.Author = BCBlockGameState.CurrentUserName;
                EditContext.CreateData.Comment = "New LevelSet created " + DateTime.Now.ToShortDateString() + " at " +
                                                 DateTime.Now.ToShortTimeString();
                EditContext.LevelData.Levels.Add(new Level());
                SetEditSet(EditContext);
            }



        }

        void newdropdown_Click(object sender, EventArgs e)
        {
            //throw new NotImplementedException();
            ToolStripItem casted = sender as ToolStripItem;
            iLevelSetBuilder builderitem = casted.Tag as iLevelSetBuilder;

            LevelFileName = "";
            EditorSet createset = new EditorSet();
            createset.CreateData.Author = BCBlockGameState.CurrentUserName;
            createset.CreateData.Comment = "LevelSet generated by " + builderitem.getDescription();
            if (builderitem.RequiresConfig()) builderitem.Configure();
            tstripstatustext.Text = "Creating LevelSet from " + builderitem.getName();
            tstripstatustext.Invalidate();
            Update();


            Thread newdropthread = new Thread(() =>
            {
                BeginInvoke((MethodInvoker)(() =>
                {
                    var loadeddata = builderitem.BuildLevelSet(new RectangleF(0, 0, PicEditor.ClientSize.Width, PicEditor.ClientSize.Height),this);
                    if(loadeddata!=null)
                    {
                    createset.LevelData = loadeddata;

                    SetEditSet(createset);
                        }
                }));
            }
            );
            newdropthread.Start();


           
        }

        private void propgridblock_SelectedObjectsChanged(object sender, EventArgs e)
        {
            Debug.Print("Selected items changed Count=" + propgridblock.SelectedObjects.Length);
            tabSideBar.SelectTab(tabProperties);
        }
        private GridItem selectedpropgriditem=null;
        private void propgridblock_SelectedGridItemChanged(object sender, SelectedGridItemChangedEventArgs e)
        {
            Debug.Print("SelectedGridItemChanged...");
            if (e.NewSelection.PropertyDescriptor != null)
            {
                Debug.Print("selectedgriditem changed:" + e.NewSelection.PropertyDescriptor.Name);
                selectedpropgriditem = e.NewSelection;
            }
        }
        private Block previousHoveredBlock=null;
        private string GetTipData(Block testblock)
        {
            String blocktype = testblock.GetType().Name;
            String blockrectangle = String.Format("{0:0.##},{1:0.##};{2:0.##},{3:0:##}", testblock.BlockRectangle.Left, testblock.BlockRectangle.Top, testblock.BlockRectangle.Width, testblock.BlockRectangle.Height);

            String buildresult =  String.Format(
@"<b>Type</b>:{0}<br>
<b>Rect:</b>:{1}"
, blocktype, blockrectangle);

            if (testblock is IEditorBlockExtensions)
            {

                buildresult += "<br>" + ((IEditorBlockExtensions)testblock).GetToolTipInfo(this);

            }
            return buildresult;

        }
        private void PicEditorHover()
        {
            Block blockhittest = BCBlockGameState.Block_HitTestOne(EditBlocks, new PointF(Mouselocation.X, Mouselocation.Y));
            Point ShowLocation = new Point(Mouselocation.X + PicEditor.Left + 10, Mouselocation.Y + PicEditor.Top - 10);
            if (blockhittest != null)
            {
                if (blockhittest != previousHoveredBlock)
                {
                    Debug.Print("Showing Tip...");
                    String tipString = GetTipData(blockhittest);
                    PicEditor.Invoke((MethodInvoker)(() =>
                                                         {
                                                             
                                                             if (HTMLTip.Active)
                                                                 HTMLTip.Hide(this);


                                                             HTMLTip.ReshowDelay = 950;

                                                             HTMLTip.Show(
                                                                 tipString, this,
                                                                 ShowLocation, 1200);
                                                         }
                    )
                    );

                }


            }


        }

        private void PicEditor_MouseHover(object sender, EventArgs e)
        {
            
           



        }
        /// <summary>
        
        /// </summary>
        private void Hovered()
        {

            UpdateTooltipImage();
            LastTipUpdate = DateTime.Now;

        }

        private void ToolStripFillColor_SelectedColorChanged(object sender, EventArgs e)
        {
            ToolStripFillColor.Tag = ((ToolStripColorPicker)sender).Color;
            //SetPickerColor(ToolStripFillColor.OwnerItem,((ToolStripColorPicker)sender).Color);
        }

        private void ToolStripPenColor_SelectedColorChanged(object sender, EventArgs e)
        {
            ToolStripPenColor.Tag = ((ToolStripColorPicker)sender).Color;
            //SetPickerColor(ToolStripPenColor.OwnerItem, ((ToolStripColorPicker)sender).Color);
        }

        private void forceToBoxToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //force all selectedblocks into a "box" formation.
            //this is done ByteConverter changing their size and location to to match up with the upper-TopMost block.
            RectangleF userectangle;
            //int maxboxwidth=128;
           // if (CurrentEditMode == EditObjectConstants.Edit_Blocks)

            EngageCustom=true;
            CustomRectAction = PerformBlockify;
            CustomCircleAction = null;


            // }
        }

        private void PerformBlockify(RectangleF userectangle)
        {
            
            Block currminblock=null;
            float currmin = int.MaxValue;
            foreach (Block selblock in SelectedBlocks)
            {
                if ((selblock.BlockLocation.X + selblock.BlockLocation.Y) < currmin)
                {
                    currminblock=selblock;
                    currmin = selblock.BlockLocation.X + selblock.BlockLocation.Y;


                }



            }
                
            //armed with currminblock as the uppertopmost, arrange all the other blocks around it.
            float usewidth=currminblock.BlockRectangle.Width;
            float useheight = currminblock.BlockRectangle.Height;
            float currX=currminblock.BlockLocation.X+usewidth;
            float currY = currminblock.BlockLocation.Y;
            int numcols = 0;
            foreach (Block selblock in SelectedBlocks)
            {
                if (selblock != currminblock)
                {
                    Debug.Print("CurrX:" + currX + " CurrY:" + currY);
                    selblock.BlockRectangle = new RectangleF(new PointF(currX, currY), new SizeF(usewidth, useheight));
                    currX+=usewidth;
                    if ((currX + usewidth > userectangle.Width))
                    {
                        numcols=0;
                        currY += useheight;
                        currX=currminblock.BlockLocation.X;
                    }
                    numcols++;
                        
                }




            }
        }

        private void propgridblock_Click_1(object sender, EventArgs e)
        {

        }

        private void introTextToolStripMenuItem_Click(object sender, EventArgs e)
        {
            introTextToolStripMenuItem.Checked = !introTextToolStripMenuItem.Checked;
            DoDrawIntroText=introTextToolStripMenuItem.Checked;
        }

        private void linesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ddgriddata.GridMode = DrawGridModeConstants.DrawGrid_Lines;
        }

        private void dotsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ddgriddata.GridMode = DrawGridModeConstants.DrawGrid_Dots;
        }



        private void noneToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ddgriddata.GridMode = DrawGridModeConstants.DrawGrid_None;
        }
        private void gridToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
        //    linesToolStripMenuItem.Checked = ddgriddata.GridMode == DrawGridModeConstants.DrawGrid_Lines;
        //    dotsToolStripMenuItem.Checked = ddgriddata.GridMode == DrawGridModeConstants.DrawGrid_Dots;
        //    noneToolStripMenuItem.Checked = ddgriddata.GridMode == DrawGridModeConstants.DrawGrid_None;
        }

        private void gridToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            propgridblock.SelectedObject = ddgriddata;
            tabSideBar.SelectTab(tabProperties);
        }
        private void PathItemClick(object sender, EventArgs e)
        {
            ObjectPathData opd = (sender as ToolStripMenuItem).Tag as ObjectPathData;
            opd.Visible=!opd.Visible;

        }

        private void pathsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //populate our drop down with all the loaded Paths. Or, a disabled (empty) item.
            //first clear current contents...
            ToolStripMenuItem tsm = (ToolStripMenuItem)sender;


            tsm.DropDownItems.Clear();

            foreach (var looppath in EditSet.PathData)
            {
                
                var gotmenu = tsm.DropDownItems.Add(looppath.Value.Name);
                ToolStripMenuItem tsmi = (gotmenu as ToolStripMenuItem);
                gotmenu.Click += PathItemClick;
                tsmi.Checked=looppath.Value.Visible;
                tsmi.Tag = looppath.Value;



            }




        }
        private void PopulateDropDown<T>(ToolStripMenuItem MenuDropdown,IEnumerable<T> enumeration, Func<T,ToolStripMenuItem,ToolStripDropDownItem> menutranslatefunction,Action<ToolStripDropDownItem> clickaction)
        {
            MenuDropdown.DropDownItems.Clear();

            foreach (T loopvar in enumeration)
            {
                ToolStripDropDownItem dditem;
                MenuDropdown.DropDownItems.Add(dditem=menutranslatefunction(loopvar, MenuDropdown));

                dditem.Tag=loopvar;
                dditem.Click+=((q,w)=> clickaction((ToolStripDropDownItem) q));


            }

        }

        private void PopulateMenuDropDown_Paths(ToolStripMenuItem MenuDropdown, Action<ToolStripDropDownItem> clickaction)
        {
            MenuDropdown.DropDownItems.Clear(); //clear all drop down items.


            //iterate all paths.
            foreach (var looppath in EditSet.PathData)
            {
                ToolStripItem dropitem = MenuDropdown.DropDownItems.Add(looppath.Value.Name);
                dropitem.Tag = looppath;
                dropitem.Click+= ((q,w)=>clickaction((ToolStripDropDownItem)q));
                



            }







        }



        private void pathsToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (CurrentEditMode == EditObjectConstants.Edit_Paths)
            {
                CurrentEditMode = EditObjectConstants.Edit_Blocks;
            }
            else
            {
                CurrentEditMode = EditObjectConstants.Edit_Paths;
            }
        }

        private void editToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void editToolStripMenuItem_DropDownClosed(object sender, EventArgs e)
        {
            //Enable every item.

            //this may seem pointless, but it will allow shortcuts to work. There is of course the additional issue that those menu items have some sanity checking...
            deleteToolStripMenuItem.Enabled = true;
        }

        private void pathsToolStripMenuItem1_DropDownOpening(object sender, EventArgs e)
        {
            var pathmenu = (ToolStripMenuItem)sender;
            PopulateMenuDropDown_Paths(pathmenu, PathItemClick);
            //also, add a separator and a New... item, which will create a new path, and set it as the current path.
            pathmenu.DropDownItems.Add("-"); //separator
            ToolStripItem newpathmenu = pathmenu.DropDownItems.Add("New");
            newpathmenu.Click += new EventHandler(newpathmenu_Click);
        }

        void newpathmenu_Click(object sender, EventArgs e)
        {
            //create a new path, and set it as  the selected one.
            ObjectPathData addedpath=null;
            String addedkey = EditSet.PathData.Add(addedpath = new ObjectPathData());
            SelectedPath=addedpath;   
        }

        private void PathItemClick(ToolStripDropDownItem itemclick)
        {
            propgridblock.SelectedObject = itemclick.Tag;
            //also set as selectedpath...
            SelectedPath = (ObjectPathData)(((KeyValuePair<String,ObjectPathData>)(itemclick.Tag)).Value);
            //unselect all other paths...
            foreach (var looppath in EditSet.PathData)
            {
                foreach (var looppoint in looppath.Value.PathPoints)
                {
                    looppoint.Selected=false;


                }

            }

        }

        private void testToolStripMenuItem_Click(object sender, EventArgs e)
        {
            




            



        }

        private void viewlevelmenu_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem showing = (ToolStripMenuItem)sender;
            Level.PopulateDropDown(EditContext.LevelData.Levels, showing.DropDown, null, (o, a) => viewlevel_Click(a, e));
            //clear all subitems.
            

        }

        void viewlevel_Click(object sender, EventArgs e)
        {
            //for the level view menu.
            ChangeLevel((Level)(((ToolStripItem)sender).Tag));
        }
        private void ChooseBlock_Generator(ToolStripItem ts,BlockData bd)
        {
            Debug.Print("ChooseBlock_Generator");
            generateblockdata = bd;


        }

        private void tstripgenerate_DropDownOpening(object sender, EventArgs e)
        {
            //clear out the drop down
            //tstripgenerate.DropDown.Items.Clear();
            //add "none" item.
            tstripgenerate.DropDown.Items.Clear();
            tStripBlockSelection.DropDown.Items.Add("None(Clear)").Click +=selNull;
            Block.PopulateDropDownWithBlocksCategorized(tstripgenerate.DropDown, false, null, null, null, SelGenerate, true);

            //PopulateDropDownWithBlocks(tstripgenerate.DropDown, SelGenerate, false, PaintMenuCategoryCallback, ChooseBlock_Generator);
            //now, add a Separator...
            //var separatorobj = DropDownSelBlock.DropDown.Items.Add("-");
            //no click event for separator...
        }

        private void frmEditor_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (IsDirty)
            {
                //there are unsaved changes.
                //LevelFileName will either be blank (not saved at all) or have a filename.
                //if it's blank, prompt that there are unsaved changes; otherwise, prompt that there are unsaved changes to the filename (only give the basename.extension)
                String SavePromptMessage = "";
                if (LevelFileName == "")
                {
                    SavePromptMessage = "There are unsaved changes. Would you like to Save?";

                }
                else
                {
                    SavePromptMessage = "There are unsaved changes to \"" + Path.GetFileName(LevelFileName) + "\". Would you like to Save?";
                }
                switch(MessageBox.Show(SavePromptMessage,"Save",MessageBoxButtons.YesNoCancel,MessageBoxIcon.Question))
                {

                    case System.Windows.Forms.DialogResult.Yes:
                        DoSave();
                    break;
                    case DialogResult.No:
                        //do nothing.
                    break;
                    case DialogResult.Cancel:
                        //cancel close.
                        e.Cancel=true;
                    break;
                }

            }
            if (!e.Cancel) SaveEditorConfig();


            }

        private void importMessageDataToolStripMenuItem_Click(object sender, EventArgs e)
        {

            //first  get the file.
            OpenFileDialog ofd = new OpenFileDialog();
            List<PlayMessageData> createlist;
            if (ofd.ShowDialog(this) == System.Windows.Forms.DialogResult.OK)
            {

                String strfname = ofd.FileName;
                //clear current messages.
                createlist = BCBlockGameState.ImportMessageData(strfname);





                EditLevel.MessageData = createlist;

                }
                



            }
        private void ShowHideTaskPane(bool pVisible)
        {
            
        }
        private void frmEditor_Resize(object sender, EventArgs e)
        {
            //Resizing:
            //PicEditor stays the same size- we set this in the Load event.
            picLeftPane.Location = new Point(0, toolStrip1.Bottom);
            picLeftPane.Height = sstripEditor.Top - toolStrip1.Bottom;
            PicEditor.Location = new Point(picLeftPane.Right, toolStrip1.Bottom);
            //move tabProperties to besize the Editor...
            tabSideBar.Location = new Point(PicEditor.Right, PicEditor.Top);
            //width takes up rest of the form.
            tabSideBar.Width = ClientSize.Width - tabSideBar.Left;
            //height takes up the entire space between the status bar and the toolstrip.
            tabSideBar.Height = ClientSize.Height - sstripEditor.Height - toolStrip1.Bottom-tabSideBar.Margin.Vertical;



        }

        private void tabSideBar_Resize(object sender, EventArgs e)
        {

        }

        private void tabProperties_Resize(object sender, EventArgs e)
        {
            propgridblock.Location = new Point(tabProperties.ClientRectangle.Left,tabProperties.ClientRectangle.Top);
            propgridblock.Size = tabProperties.ClientSize;
        }

        private void TabBlocks_Resize(object sender, EventArgs e)
        {
            grpGenerate.Location = new Point(TabBlocks.ClientRectangle.Left, TabBlocks.ClientRectangle.Top);
            grpGenerate.Size = new Size(TabBlocks.ClientSize.Width, grpGenerate.Height);
        }

        private void tabLevels_Resize(object sender, EventArgs e)
        {
            lvwLevels.Location = new Point(tabLevels.ClientRectangle.Left, tstriplevels.Bottom);
            lvwLevels.Size = new Size(tabLevels.ClientSize.Width, tabLevels.ClientSize.Height - lvwLevels.Top);
        }

        private void grpGenerate_Resize(object sender, EventArgs e)
        {
            btnGenerate.Location = new Point(grpGenerate.ClientSize.Width - btnGenerate.Width - 10, btnGenerate.Top);
        }

        private void undoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformUndo();
        }

        private void redoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PerformRedo();
        }

        private void editPropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propgridblock.SelectedObject = EditLevel;
            tabSideBar.SelectTab(tabProperties);
        }

        private void editPropertiesToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            propgridblock.SelectedObject = EditSet;
            tabSideBar.SelectTab(tabProperties);
        }

        private void introLengthFromSoundToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void testingToolStripMenuItem_Click(object sender, EventArgs e)
        {
            propgridblock.SelectedObject = new TestCheckObject();
        }
        //new item on the Add Dropdown on the Levels tab.
        private void newToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            Level addthislevel = new Level();
            addthislevel.LevelName = "New Level";
            AddLevelToEditor(addthislevel,false);
        }
        private void AddLevelToEditor(Level newLevel)
        {
            AddLevelToEditor(newLevel,true);

        }
        private void AddLevelToEditor(Level newLevel,bool SetFocus)
        {

            EditSet.Levels.Add(newLevel);
            
            EditingLevel = EditSet.Levels.FindIndex((w) => w == newLevel);
            RefreshLevelView();
            propgridblock.SelectedObject = EditLevel;

        }
        private void fromTemplateToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
        }

        private void fromTemplateToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {

            ToolStripDropDownItem titem = (ToolStripDropDownItem)sender;
            //clear the current items.
            titem.DropDown.Items.Clear();
            //add the "Template Browser" Option first.
            ToolStripItem Templatebrowser = titem.DropDown.Items.Add("Template Browser");
            Templatebrowser.Click += TemplateBrowser_Click;

            //add a separator...
            titem.DropDown.Items.Add(new ToolStripSeparator());
            //and NOW we can add them...
            TemplateCategory.PopulateDropdown(titem.DropDown, false, null, TemplateItemCallback);



        }
        private void TemplateItemCallback(TemplateManager.TemplateLevelData tld, ToolStripMenuItem clickeditem)
        {

            MessageBox.Show("Add template " + tld.Name + " here...");
            //adding a template is done by inserting the level as needed, and also adding the appropriate CreatorProperty data from
            //the set owner to our levelset (so that any sounds or other data associated with the level or block information will be available)
            EditContext.CreateData.MergeWith(tld.SetOwner.CreationData);
            Level levelcopy = tld.CloneLevelData();
            //add the creatorproperties first.
            //now insert the level, but don't set focus to it.
            AddLevelToEditor(levelcopy,false);


        }
        private void TemplateBrowser_Click(object sender, EventArgs e)
        {

            //first we need to get a template.
            frmFromTemplate templatedialog = new frmFromTemplate();
            if (templatedialog.ShowDialog() == DialogResult.OK)
            {
                TemplateManager.TemplateLevelData chosen = templatedialog.SelectedTemplate;
                if (chosen == null) return; //idiot check
                //create a new Level...
                Level addthislevel = chosen.CloneLevelData();
                addthislevel.LevelName = "From " + chosen.LevelData.LevelName;

                EditSet.Levels.Add(addthislevel);
                RefreshLevelView();
                propgridblock.SelectedObject = EditLevel;


            }


        }
        private void testblockmenu(ToolStripMenuItem clickeditem, Block blockobject,BlockData bdata,ManyToOneBlockData mtodata)
        {

            Debug.Print("in testblock menu");
            MessageBox.Show(this, blockobject.BlockRectangle.ToString());
        }
        private void testingToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            
            Block.PopulateDropDownWithBlocksCategorized((sender as ToolStripMenuItem).DropDown, true, null, null, null,testblockmenu,true);
        }

        private void serialToolStripMenuItem_Click(object sender, EventArgs e)
        {

            try
            {
                Block[] savethese = SelectedBlocks.ToArray();

                MemoryStream mstream = new MemoryStream();
                BinaryFormatter bf = new BinaryFormatter();
                //serialize...
                bf.Serialize(mstream, savethese);

                mstream.Seek(0, SeekOrigin.Begin);
                bf = new BinaryFormatter();
                Block[] readback = (Block[])bf.Deserialize(mstream);
                MessageBox.Show("Serialize->Deserialize was successful. (Data size " + mstream.Length + ")");
            }
            catch (Exception exx)
            {
                MessageBox.Show("Serialize->Deserialize Failed :(\n" + exx.ToString());

            }

            //test serializing 



        }

        private void undoToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //
            //populate with all undo items.
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            var undostack = UndoObject.GetUndoStack().ToList();
            undostack.Reverse();
            tsmi.DropDown.Items.Clear();
            foreach (var iterate in undostack)
            {
                ToolStripMenuItem undoitem = new ToolStripMenuItem("Undo to " + iterate.ToString());
                undoitem.Tag = iterate;
                undoitem.Click += new EventHandler(undoitem_Click);
                tsmi.DropDown.Items.Add(undoitem);

            }
            

        }

        

        private void redoToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            ToolStripMenuItem tsmi = sender as ToolStripMenuItem;
            var Redostack = UndoObject.GetRedoStack().ToList();
            Redostack.Reverse();
            tsmi.DropDown.Items.Clear();

            foreach (var iterate in Redostack)
            {
                ToolStripMenuItem redoitem = new ToolStripMenuItem("Redo to " + iterate.ToString());
                redoitem.Tag = iterate;
                redoitem.Click += new EventHandler(redoitem_Click);
                tsmi.DropDown.Items.Add(redoitem);
            }
            
        }

        void redoitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem grabitem = sender as ToolStripMenuItem;
            var usi = grabitem.Tag as EditorUndoStackItem;
            if (UndoObject.PeekRedo() == null) return;
            while (UndoObject.PeekRedo() != usi)
            {

                PerformRedo();

            }
            PerformRedo();
        }
        void undoitem_Click(object sender, EventArgs e)
        {
            ToolStripMenuItem grabitem = sender as ToolStripMenuItem;
            var usi = grabitem.Tag as EditorUndoStackItem;

            //undo up to that point.
            if (UndoObject.PeekUndo() == null) return;
            while (UndoObject.PeekUndo() != usi)
            {


                PerformUndo();

            }
            PerformUndo(); //one more time...


        }

        private void PicEditor_DragOver(object sender, DragEventArgs e)
        {



        }

        private void PicEditor_DragEnter(object sender, DragEventArgs e)
        {

           
        }

        private void PicEditor_DragDrop(object sender, DragEventArgs e)
        {




        }

        private void frmEditor_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;


            }
            else
            {
                e.Effect = DragDropEffects.None;

            }
        }

        private void frmEditor_DragDrop(object sender, DragEventArgs e)
        {

            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    Array a = (Array)e.Data.GetData(DataFormats.FileDrop);

                    if (a != null)
                    {

                        String s = a.GetValue(0).ToString();
                        EditFile_Threaded(s);


                    }





                }
                //other formats (such as dragging blocks or something. Not sure how that would work...



            }
            catch
            {



            }
        }
        private IEnumerable<Block> GetLinear(IEnumerable<Block> fromgroup,bool CenterOnly,bool Horizontal)
        {
            return fromgroup.SelectMany(iterate => GetLinear(iterate, CenterOnly, Horizontal));
        }

        private IEnumerable<Block> GetLinear(Block source, bool CenterOnly,bool Horizontal)
        {
            Rectangle hittestrect;
            if (Horizontal)
            {
                if (CenterOnly)
                    hittestrect = new Rectangle(0, (int)source.CenterPoint().Y - 1, PicEditor.ClientSize.Width, 2);
                else
                    hittestrect = new Rectangle(0, (int)source.BlockRectangle.Top, PicEditor.ClientSize.Width, (int)source.BlockRectangle.Height);
            }
            else
            {
                if (CenterOnly)
                    hittestrect = new Rectangle((int)source.CenterPoint().X, 0, 2, PicEditor.ClientSize.Height);
                else
                    hittestrect = new Rectangle((int)source.BlockRectangle.Left, 0, (int)source.BlockRectangle.Width, PicEditor.ClientSize.Height);
                    
                


            }



            return (from p in EditBlocks where p.HitTest(hittestrect) select p);


        }
        private void expandVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //pass in the inverse of whether the shift key is pressed, so that we select all blocks that a given block "sees".
            foreach (Block loopiterate in
                GetLinear(SelectedBlocks, !KeyboardInfo.IsPressed(Keys.ShiftKey), false).Where(loopiterate => !SelectedBlocks.Contains(loopiterate)))
            {
                SelectedBlocks.Add(loopiterate);
            }
        }

        private void expandHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Block> addselection = new List<Block>();
            foreach (Block loopiterate in GetLinear(SelectedBlocks, !KeyboardInfo.IsPressed(Keys.ShiftKey), true).Where(loopiterate => !SelectedBlocks.Contains(loopiterate)))
            {
                addselection.Add(loopiterate);
                

            }

            foreach (Block loopiterate in addselection)
            {

                SelectedBlocks.Add(loopiterate);

            }
        }

        private void selectionToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            selectNoneToolStripMenuItem.Enabled = expandVerticalToolStripMenuItem.Enabled = expandHorizontalToolStripMenuItem.Enabled = SelectedBlocks.Count > 0;
            selectColinearToolStripMenuItem.Enabled =
                (CurrentEditMode == EditObjectConstants.Edit_Balls && EditBalls.Count == 2) ||
                (CurrentEditMode == EditObjectConstants.Edit_Blocks && EditBlocks.Count == 2);

        }

        private void openContainingFolderToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            String sfilename = Path.GetFileName(LevelFileName);

            Process.Start("explorer.exe", "/select," + LevelFileName);
            


            

        }

        private void fileToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            openContainingFolderToolStripMenuItem.Enabled = !String.IsNullOrEmpty(LevelFileName);
        }


        public List<Block> GetSelectedBlocks()
        {
            return SelectedBlocks;
        }

        public List<cBall> GetSelectedBalls()
        {
            return SelectedBalls;
        }

        public List<ObjectPathData> GetSelectedPaths()
        {
            return new List<ObjectPathData>() { SelectedPath };

        }

        public List<Block> GetBlocks()
        {
            return EditBlocks;
        }

        public List<cBall> GetBalls()
        {
            return EditBalls;
        }

        public ObjectPathDataManager GetPaths()
        {
            return EditSet.PathData;
        }

        public EditorSet GetSet()
        {
            return EditContext;
        }

        private List<ToolStripMenuItem> LevelMenus = null; 

        private void levelToolStripMenuItem1_DropDownOpening(object sender, EventArgs e)
        {

            ToolStripMenuItem LevelMenu = sender as ToolStripMenuItem;
            //if levelMenus is not null, remove all of them.
            if (LevelMenus != null)
            {
                foreach (var iteratemenu in LevelMenus)
                {
                    LevelMenu.DropDownItems.Remove(iteratemenu);


                }
                Level.PopulateDropDown(EditContext.LevelData.Levels, LevelMenu.DropDown, (cl, ts) => { LevelMenus.Add(ts); return true; },
                (o, a) => { ChangeLevel(o); }, false);

            }
            


        }

        private void selectAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SelectedBlocks.Clear();
            SelectedBalls.Clear();
            SelectedBlocks.AddRange(EditBlocks);
            SelectedBalls.AddRange(EditBalls);
        }

        private void selectSameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<Block> currselblocks = SelectedBlocks.Clone();
            List<cBall> currselballs = SelectedBalls.Clone();
            SelectedBlocks.Clear();
            SelectedBalls.Clear();
            SelectedBlocks.AddRange(from b in EditBlocks where currselblocks.Any((y) => y.GetType().Equals(b.GetType())) select b);
            SelectedBalls.AddRange(from b in EditBalls where currselballs.Any((y) => y.GetType().Equals(b.GetType())) select b);
            
        }

        private void selectColinearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PointF FirstPosition = PointF.Empty;
            PointF SecondPosition=PointF.Empty;
            switch(CurrentEditMode)
            {
                case EditObjectConstants.Edit_Balls:
                    Debug.Assert(EditBalls.Count != 2);
                    FirstPosition = EditBalls.First().Location;
                    SecondPosition = EditBalls.Last().Location;

                   

                    break;
                case EditObjectConstants.Edit_Blocks:
                    Debug.Assert(EditBlocks.Count != 2);
                    FirstPosition = EditBlocks.First().CenterPoint();
                    SecondPosition = EditBlocks.Last().CenterPoint();
                    break;

                default:

                    return;
            }

            var firstangle = BCBlockGameState.GetAngle(FirstPosition, SecondPosition);
            PointF offsetA = new PointF((float)(Math.Cos(firstangle) * 3), (float)(Math.Sin(firstangle) * 3));
            var secondangle = (firstangle + Math.PI) % Math.PI;
            PointF offsetB = new PointF((float)(Math.Cos(secondangle) * 3), (float)(Math.Sin(firstangle) * 3));

            //starting from second point, add Firstangle as long as we are within the level area, add a PointF to the List.
            List<PointF> checkPoints = new List<PointF>();
            PointF currentpoint = SecondPosition;
            while (PicEditor.ClientRectangle.Contains(currentpoint.ToPoint()))
            {
                checkPoints.Add(currentpoint);
                currentpoint = new PointF(currentpoint.X + offsetA.X, currentpoint.Y + offsetA.Y);
            }

            currentpoint = FirstPosition;
            while (PicEditor.ClientRectangle.Contains(currentpoint.ToPoint()))
            {
                checkPoints.Add(currentpoint);
                currentpoint = new PointF(currentpoint.X + offsetB.X, currentpoint.Y + offsetB.Y);

            }

            foreach (var currpoint in checkPoints)
            {
                if (CurrentEditMode == EditObjectConstants.Edit_Blocks)
                {
                    foreach (Block iterate in BCBlockGameState.Block_HitTest(EditBlocks, currpoint))
                        if (!SelectedBlocks.Contains(iterate)) SelectedBlocks.Add(iterate);


                }
                else if (CurrentEditMode == EditObjectConstants.Edit_Balls)
                {
                    foreach (cBall iterate in BCBlockGameState.Ball_HitTest(EditBalls, currpoint))
                        if (!SelectedBalls.Contains(iterate)) SelectedBalls.Add(iterate);
                }




            }

        }

        private void DropDownAddBlocks_DropDownClosed(object sender, EventArgs e)
        {
            HTMLTip.Hide(this);
        }
    }
        }



    
