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
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Security;
//using System.Windows.Media;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Cheats;
using BASeCamp.BASeBlock.Events;
using BASeCamp.BASeBlock.GameObjects;
using BASeCamp.BASeBlock.GameObjects.Orbs;
using BASeCamp.BASeBlock.PaddleBehaviours;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.Configuration;
using BASeCamp.BASeBlock.GameStates;
using BASeCamp.BASeBlock.HighScores;
using BASeCamp.Updating;
using BASeCamp.Licensing;


using Brush=System.Drawing.Brush;
using Color=System.Drawing.Color;
using Pen=System.Drawing.Pen;
using ThreadState=System.Threading.ThreadState;
using Timer = System.Threading.Timer;

namespace BASeCamp.BASeBlock
{
    public partial class frmBaseBlock : Form,iGameClient ,iManagerCallback 
    {
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, Int32 wMsg, bool wParam, Int32 lParam);
        public IGameState ActiveState { get { return gamerunstate;} set { gamerunstate = value;}}
        private const int WM_SETREDRAW = 11;

        





        public int[][] GameOverMatrix = new int[][]
       {
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
           new int[]{0,1,1,1,1,1,1,0,0,0,1,1,0,0,0,1,1,1,0,0,1,1,1,0,1,1,1,1,1,1,},
           new int[]{0,1,1,1,1,1,1,0,0,1,1,1,1,0,0,1,1,1,0,0,1,1,1,0,1,1,1,1,1,1,},
           new int[]{0,1,1,0,0,0,0,0,1,1,1,1,1,1,0,1,1,1,1,1,1,1,1,0,1,1,0,0,0,0,},
           new int[]{0,1,1,0,1,1,1,0,1,1,0,0,1,1,0,1,1,0,1,1,0,1,1,0,1,1,1,1,0,0,},
           new int[]{0,1,1,0,1,1,1,0,1,1,1,1,1,1,0,1,1,0,0,0,0,1,1,0,1,1,1,1,0,0,},
           new int[]{0,1,1,0,0,1,1,0,1,1,0,0,1,1,0,1,1,0,0,0,0,1,1,0,1,1,0,0,0,0,},
           new int[]{0,1,1,1,1,1,1,0,1,1,0,0,1,1,0,1,1,0,0,0,0,1,1,0,1,1,1,1,1,1,},
           new int[]{0,1,1,1,1,1,1,0,1,1,0,0,1,1,0,1,1,0,0,0,0,1,1,0,1,1,1,1,1,1,},
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
           new int[]{0,1,1,1,1,1,1,0,1,1,0,0,1,1,0,0,1,1,1,1,1,1,0,0,1,1,1,1,0,0,},
           new int[]{0,1,1,1,1,1,1,0,1,1,0,0,1,1,0,0,1,1,1,1,1,1,0,0,1,1,1,1,1,0,},
           new int[]{0,1,1,0,0,1,1,0,1,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,1,1,0,0,1,1,},
           new int[]{0,1,1,0,0,1,1,0,1,1,0,0,1,1,0,0,1,1,1,1,0,0,0,0,1,1,0,0,1,1,},
           new int[]{0,1,1,0,0,1,1,0,1,1,0,0,1,1,0,0,1,1,1,1,0,0,0,0,1,1,1,1,1,0,},
           new int[]{0,1,1,0,0,1,1,0,1,1,0,0,1,1,0,0,1,1,0,0,0,0,0,0,1,1,1,1,1,1,},
           new int[]{0,1,1,1,1,1,1,0,0,1,1,1,1,0,0,0,1,1,1,1,1,1,0,0,1,1,0,0,1,1,},
           new int[]{0,1,1,1,1,1,1,0,0,0,1,1,0,0,0,0,1,1,1,1,1,1,0,0,1,1,0,0,1,1,},
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
           new int[]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,},
       };


        private int MaxParticles = 800;

       

        public class MenuModeMenuItem
        {
            public class MenuItemStyleData
            {
                
                //private Color _BackColor = Color.White;
                //private Color _ForeColor = Color.Black;
                
                private Brush _Background = new SolidBrush(Color.White);
                private Brush _Foreground = new SolidBrush(Color.Black);
                private Pen _BoxBorder = new Pen(Color.Black);
                

                public Brush Background { get { return _Background; } set { _Background = value; } }
                public Brush Foreground { get { return _Foreground; } set { _Foreground = value; } }
                public Pen BoxBorder { get { return _BoxBorder; } set { _BoxBorder = value; } }
                    private Color _BackColor, _ForeColor;


                public Color BackColor
                {
                    get { return _BackColor; }
                    set
                    {
                        _BackColor = value;
                        _Background = new SolidBrush(_BackColor);
                    }
                }
                public Color ForeColor
                {
                    get { return _ForeColor; }
                    set
                    {
                        _ForeColor = value;
                        _Foreground = new SolidBrush(_ForeColor);
                    }
                }
                private Font _ItemFont = new Font("Arial", 14);

                public Font ItemFont { get { return _ItemFont; } set { _ItemFont = value; } }
                public MenuItemStyleData()
                {

                }

                public MenuItemStyleData(Color pBackColor, Color pForeColor,Font pItemFont)
                {
                    BackColor = pBackColor;
                    ForeColor = pForeColor;
                    ItemFont = pItemFont;


                }

            }

            public delegate void MenuItemEvent(MenuModeMenuItem sender);
            public delegate void MenuItemPrePaintEvent(MenuModeMenuItem sender, RectangleF drawarea);

            public event MenuItemEvent MenuItemSelect;
            public event MenuItemEvent MenuItemUnselect;
            public event MenuItemEvent MenuItemChosen;
            public event MenuItemPrePaintEvent MenuItemPrePaint;


            public void InvokeItemPrePaint(RectangleF drawarea)
            {
                var temp = MenuItemPrePaint;
                if (temp != null) temp.Invoke(this,drawarea);



            }

            public void InvokeItemSelect()
            {
                var temp = MenuItemSelect;
                if (temp != null) temp.Invoke(this);
                
            }
            public void InvokeItemUnselect()
            {
                var temp = MenuItemUnselect;
                if (temp != null) temp.Invoke(this);

            }
            public void InvokeItemChosen()
            {
                var temp = MenuItemChosen;
                if (temp != null) temp.Invoke(this);

            }

            private Object _Tag = null;
            public Object Tag { get { return _Tag; } set { _Tag = value; } }
            private String _Text;

            

            private MenuModeMenuItem.MenuItemStyleData Style = new MenuModeMenuItem.MenuItemStyleData(Color.White,Color.Black,new Font("Arial",14));
            private MenuModeMenuItem.MenuItemStyleData SelStyle = new MenuModeMenuItem.MenuItemStyleData(Color.Black,Color.Yellow,new Font("Arial",14,FontStyle.Bold))  ;
            private bool _Selected=false;

            

            public bool Selected { get { return _Selected; } set { _Selected = value; } }
            public MenuModeMenuItem.MenuItemStyleData OurStyle { get {
                return Selected?SelStyle:Style;
                



            } }
                public delegate void ItemActionProc(MenuModeMenuItem sender);
            private ItemActionProc _Action;
            public ItemActionProc Action { get { return _Action; } set { _Action = value; } }

            private static Graphics useg = null;
            private static bool ginitialized;
            public SizeF MeasureItem()
            {
                if (!ginitialized)
                {
                    Bitmap useb = new Bitmap(1, 1);
                    useg = Graphics.FromImage(useb);



                }

                SizeF gotsize = useg.MeasureString(_Text, OurStyle.ItemFont);


                return new SizeF(gotsize.Width*1.5f,gotsize.Height);

            }
         public MenuModeMenuItem(String Text,ItemActionProc MenuAction)
        {
            _Text=Text;
            _Action = MenuAction;
            SelStyle.BoxBorder = new Pen(Color.Red, 3);
            Style.BoxBorder = new Pen(Color.White, 1);
        }

        public String Text { get { return _Text; } set { _Text = value; } }


            public void Draw(Graphics g, RectangleF drawArea)
            {
                MenuModeMenuItem.MenuItemStyleData usestyle = OurStyle;
                InvokeItemPrePaint(drawArea);
                g.FillRectangle(usestyle.Background, drawArea);
                StringFormat useformat = new StringFormat();
                useformat.Alignment=StringAlignment.Center;
                g.DrawRectangle(usestyle.BoxBorder, drawArea.ToRectangle());
                g.DrawString(_Text, OurStyle.ItemFont, usestyle.Foreground , drawArea,useformat);
                

            }



        }
        public class MenuModeData
        {
            //list of menu items.
            private String _MenuMusic = "credit";
            private String _MenuTitle = "MENU";
            private Font _TitleFont = new Font("Arabia", 18);
            public String MenuTitle { get { return _MenuTitle; } set { _MenuTitle = value; } }
                public String MenuMusic { get { return _MenuMusic; } set { _MenuMusic = value; } }
                public LinkedList<MenuModeMenuItem> items = new LinkedList<MenuModeMenuItem>();

                private MenuModeMenuItem _SelectedItem = null;

            //accessor which "unselects" (untoggles) the currently selected items selected state when a new one is assigned, and sets the new one.
                public MenuModeMenuItem SelectedItem
                {
                    get {return _SelectedItem; }
                    set {
                        if (_SelectedItem != null)
                        {
                            _SelectedItem.Selected=false;

                        }
                        _SelectedItem=value;
                        _SelectedItem.Selected = true;

                    }

                }
            public MenuModeData(IEnumerable<MenuModeMenuItem> pitems, MenuModeMenuItem.MenuItemEvent SelectEvent, MenuModeMenuItem.MenuItemEvent UnselectEvent, MenuModeMenuItem.MenuItemEvent ChosenEvent)
            {
                
                foreach (var loopit in pitems)
                {
                    var itemadd = loopit;
                    itemadd.MenuItemSelect += SelectEvent;
                    itemadd.MenuItemUnselect += UnselectEvent;
                    itemadd.MenuItemChosen += ChosenEvent;
                    
                    items.AddLast(itemadd);


                }


            }

           
            public void MenuAction(MenuModeMenuItem sender)
            {


            }
            public MenuModeMenuItem HitTest(BCBlockGameState gstate,PointF testpoint)
            {
                //like the draw routine, we need to acquire the calculated sizes. We just don't draw.
                //calculations. First, get a SizeF from each menu item...

                List<SizeF> allsizes = (from n in items select n.MeasureItem()).ToList();

                //get maximum width...
                float totalheight = 0;
                foreach (var loopsize in allsizes)
                {
                    totalheight += loopsize.Height;

                }
                float maxwidth = (from n in allsizes orderby n.Width descending select n).First().Width;

                //center the rectangle maxwidth,totalheight in the gamearea.

                RectangleF EntireSizeRect = BCBlockGameState.CenterRect(gstate.GameArea,
                                                                        new SizeF(maxwidth, totalheight));


                //title rect will be centered above the entiresize rect. First, we need to measure the title...
                SizeF titlesize = BCBlockGameState.MeasureString(_MenuTitle, _TitleFont);

                if (titlesize.Width < EntireSizeRect.Width)
                {
                    titlesize = new SizeF(EntireSizeRect.Width, titlesize.Height);


                }
                PointF TopCenter = new PointF(EntireSizeRect.Left + EntireSizeRect.Width / 2, EntireSizeRect.Top);
                RectangleF TitleRect = new RectangleF(TopCenter.X - titlesize.Width / 2, TopCenter.Y - titlesize.Height,
                                                      titlesize.Width, titlesize.Height);

               


                //EntireSizeRect = new RectangleF(EntireSizeRect.Left - 20, EntireSizeRect.Top - 20, EntireSizeRect.Width + 20, EntireSizeRect.Height + 20);
                float currx = EntireSizeRect.Left;
                float currY = EntireSizeRect.Top;


                //go through each one and calculate it's rectangle, then see if the given position is inside it.
                float selectedY = 0;
                MenuModeMenuItem selecteditemuse = null;
                foreach (var loopitem in items)
                {
                    var loopiheight = loopitem.MeasureItem();
                   
                    loopitem.Selected = loopitem == SelectedItem;
                    {
                   
                   
                        currY += loopiheight.Height;
                        RectangleF testrect = new RectangleF(new PointF(currx, currY),
                                                         new SizeF(maxwidth, loopiheight.Height));
                        if (testrect.Contains(testpoint))
                        {

                            return loopitem;
                        }
                    }


                }

            return null; //no hit...



            }

            public void Draw(Graphics g, BCBlockGameState gstate)
            {
                //calculations. First, get a SizeF from each menu item...

                List<SizeF> allsizes = (from n in items select n.MeasureItem()).ToList();

                //get maximum width...
                float totalheight = 0;
                foreach (var loopsize in allsizes)
                {
                    totalheight += loopsize.Height;

                }
                float maxwidth = (from n in allsizes orderby n.Width descending select n).First().Width;

                //center the rectangle maxwidth,totalheight in the gamearea.

                RectangleF EntireSizeRect = BCBlockGameState.CenterRect(gstate.GameArea,
                                                                        new SizeF(maxwidth, totalheight));


                //title rect will be centered above the entiresize rect. First, we need to measure the title...
                SizeF titlesize = BCBlockGameState.MeasureString(_MenuTitle, _TitleFont);

                if (titlesize.Width < EntireSizeRect.Width)
                {
                    titlesize = new SizeF(EntireSizeRect.Width, titlesize.Height);


                }
                PointF TopCenter = new PointF(EntireSizeRect.Left + EntireSizeRect.Width/2, EntireSizeRect.Top);
                RectangleF TitleRect = new RectangleF(TopCenter.X - titlesize.Width/2, TopCenter.Y - titlesize.Height,
                                                      titlesize.Width, titlesize.Height);

                //draw the title.
                g.FillRectangle(new SolidBrush(Color.Green), TitleRect);
                g.DrawRectangle(new Pen(Color.Yellow, 3), TitleRect.ToRectangle());
                g.DrawString(_MenuTitle, _TitleFont, new SolidBrush(Color.White), TitleRect);



                //EntireSizeRect = new RectangleF(EntireSizeRect.Left - 20, EntireSizeRect.Top - 20, EntireSizeRect.Width + 20, EntireSizeRect.Height + 20);
                float currx = EntireSizeRect.Left;
                float currY = EntireSizeRect.Top;


                //go through each one and draw it.
                float selectedY = 0;
                MenuModeMenuItem selecteditemuse = null;
                foreach (var loopitem in items)
                {
                    var loopiheight = loopitem.MeasureItem();
                    //defer drawing if selected item for last... save info when we encounter it, and then use that info after the loop.
                    loopitem.Selected = loopitem == SelectedItem;
                    {
                        if (loopitem.Selected)
                        {
                            selectedY = currY;
                            selecteditemuse = loopitem;
                        }
                        else
                        {
                            loopitem.Draw(g,
                                          new RectangleF(new PointF(currx, currY),
                                                         new SizeF(maxwidth, loopiheight.Height)));
                        }
                        currY += loopiheight.Height;

                    }


                }

                //draw the selected item...
                if (selecteditemuse != null)
                {

                    selecteditemuse.Draw(g,new RectangleF(new PointF(currx,selectedY),new SizeF(maxwidth,selecteditemuse.MeasureItem().Height)));
                }
            







        }

            public MenuModeData(String[] MenuItems,MenuModeMenuItem.MenuItemEvent SelectEvent,MenuModeMenuItem.MenuItemEvent UnselectEvent,MenuModeMenuItem.MenuItemEvent ChosenEvent,MenuModeMenuItem.MenuItemPrePaintEvent prepaint )
            {
                //constructor that accepts a bunch of strings, and the event assignments.
                
                foreach (String loopit in MenuItems)
                {
                    var itemadd = new MenuModeMenuItem(loopit, MenuAction);
                    itemadd.MenuItemSelect += SelectEvent;
                    itemadd.MenuItemUnselect += UnselectEvent;
                    itemadd.MenuItemChosen += ChosenEvent;
                    itemadd.MenuItemPrePaint += prepaint;
                    items.AddLast(itemadd);


                }

            }


        }


        private String GameOverHiScores = "";
        private DateTime lastscoreshow;
        private int Nextshowhiscore = 1; //next hi score to be shown by the "Game Over" screen...
        
        private Graphics pcgraph;
        //private Image GameAreaImage; //used during level complete tally...
        private Bitmap BackBufferBitmap;
        private Graphics backBufferCanvas;
        private List<String> RecentCheats = new List<string>();
        private int currentcheatsel = 0; //<<index into the list of the currently "selected" cheat; only really valid for a single
        private int currentcheatcharpos = 0; //position within cheat string, will be changable via arrow keys, home, end, etc.
                                        //"session" of the cheat "window".
        public bool DemoMode=false; //if true, refuses player interactions.

        
        public struct Tallyscreendata
        {
            public String TallyScreenString;
            public Image TallyImage;


        }
     
        private MenuModeData menudata;
        private Tallyscreendata tallydata;
        private IGameState statebeforepause;
        public Rectangle AvailableClientArea = new Rectangle(0, 0, 493, 427); //default client area. the HUD is drawn to the right this.
        
        private IGameState _gamerunstate = new StateNotRunning();
        public IGameState gamerunstate { get { 
           
            
            return _gamerunstate; } 
            set {
                _gamerunstate = value; Debug.Print("Game run state set to " + value.GetType().Name +" Stack:" + new StackTrace().ToString());
            }
        }
        public Thread GameThread=null;
        public Random mrandom = BCBlockGameState.rgen;
        public int mScoreatlevelstart = 0;
        public TimeSpan mLevelTime;
        public int LevelDeathCount = 0;
        public DateTime mLevelIntroStartTime;
        public TimeSpan mLevelParTime;
        public BCBlockGameState mGameState;
        public LevelSet mPlayingSet;
        public int mPlayingLevel;
        private Queue<PlayMessageData> LevelMessageQueue = null; 
        public bool ShowDebugInfo=false;
        public LevelSet GetPlayingSet()
        {
            return mPlayingSet;


        }

        public static void SuspendDrawing(Control parent)
        {
            parent.Invoke((MethodInvoker)(()=>SendMessage(parent.Handle, WM_SETREDRAW, false, 0)));
        }

        public static void ResumeDrawing(Control parent)
        {
            parent.Invoke((MethodInvoker)(() => SendMessage(parent.Handle, WM_SETREDRAW, true, 0)));
            //parent.Refresh();
        }

        public Level mPlayingLevelobj
        {
            get
            {
                
                    return mPlayingSet.Levels[mPlayingLevel - 1];
                
            
            
            
            }


        }
            //draws in several layers


        //the background layer...
        private Bitmap backgroundbitmap;
        private Graphics BackgroundBuffer;
        private Random mRandom = new Random();
        
        //and the "block" layer...
        private Bitmap BlockDrawBitmap;
        private Graphics BlockDrawBuffer;

        private bool sidebarbgdrawn=false;

        //decided that the moving blocks will be on a separate layer from the standard blocks.

        private Bitmap MovingBlockBitmap;
        private Graphics MovingBlockBuffer;

        private Bitmap SidebarBitmap;
        private Graphics sidebarGraphics;
        private Image SidebarImage;

        //private Bitmap backgroundpic = new Bitmap("D:\\banner2.png");
        private int drawbackgroundoffset = 0;
        /// <summary>
        /// set in GameProc and queried in the Paint() routine to determine wether nonanimated blocks need to be redrawn.
        /// </summary>
        private bool mredrawblocks=true;
        /// <summary>
        /// set in GameProc and queried in the Paint() routine to determine wether nonanimated blocks need to be redrawn.
        /// </summary>
        /// 
        
        ///<summary>
        ///also set in GameProc for consumption in the paint() routine; previously I was performing the LINQ to create this (the blocks that return true for "requiresPerformFrame" were in one, ones that returned false in another).
        ///although I don't know how LINQ is implemented internally I hazard a guess and say that repetition is not a good thing, so I graduated
        ///some collections to form-level locals.
        /// </summary>
        private List<Block> staticblocks;
        private List<Block> AnimatedBlocks;
        private bool mRedrawAnimated=true; 
        private bool gamethreadsuspended=false;
        private int gamepauseamount=0;
        //the ball and paddle are drawn to the Picturebox graphics object in the Paint routine.

        private Color getrandomcolor()
        {
            Func<int> randomint = new Func<int>(()=>((int)(mrandom.NextDouble()*255)));
            return Color.FromArgb(randomint(),randomint(),randomint(),randomint());




        }
        public void ReplaceBlocks(Func<Block, Block> replacefunc)
        {

            LinkedList<Block> newblocks = new LinkedList<Block>();
            Block createdblock;
            foreach (Block loopblock in mGameState.Blocks)
            {
                if (!loopblock.BlockRectangle.IsEmpty)
                {
                    createdblock = replacefunc(loopblock);
                    createdblock.BlockTriggers = loopblock.BlockTriggers;
                    createdblock.BlockEvents = loopblock.BlockEvents;
                    foreach (BlockTrigger bt in createdblock.BlockTriggers)
                    {
                        bt.OwnerBlock = createdblock;


                    }
                    foreach (BlockEvent be in createdblock.BlockEvents)
                    {
                        be.OwnerBlock = createdblock;

                    }
                    if (createdblock != null)
                        newblocks.AddLast(createdblock);

                }
            }
            mGameState.Blocks.Clear();
            mGameState.Blocks.AddRangeAfter(newblocks);
            staticblocks = (from bsel in mGameState.Blocks where !bsel.RequiresPerformFrame() select bsel).ToList();
            AnimatedBlocks = (from bsel in mGameState.Blocks where bsel.RequiresPerformFrame() select bsel).ToList();

            mredrawblocks = true;
            mRedrawAnimated = true;
            //invoke redraw
            PicGame.Invoke((MethodInvoker)(() => PicGame.Invalidate()));



        }
        public void UpdateBlocks()
        {
            staticblocks = (from bsel in mGameState.Blocks where !bsel.RequiresPerformFrame() select bsel).ToList();
            AnimatedBlocks = (from bsel in mGameState.Blocks where bsel.RequiresPerformFrame() select bsel).ToList();

            mredrawblocks = true;
            mRedrawAnimated = true;
            

        }
        public static int ScoreRevealDelayTime = 500;

        private Color RandomColor()
        {
            return Color.FromArgb((int)(mrandom.NextDouble() * 128 + 128), (int)(mrandom.NextDouble() * 255), (int)(mrandom.NextDouble() * 255), (int)(mrandom.NextDouble() * 255));



        }
        private LevelSet CreateDefaultLevelSet()
        {
            iLevelSetBuilder defaultbuilder = new DefaultLevelBuilder();
            LevelSet returnset =  defaultbuilder.BuildLevelSet(AvailableClientArea,this);

            //returnset.Save(@"D:\textoutxaml.xaml");


            return returnset;


        }

        private Level CreateDefaultLeveltestray(int levelnumber)
        {

            Level returnlevel = new Level();



            //returnlevel.levelballs.Add(new cBall(new PointF(PicGame.ClientSize.Width / 2, PicGame.ClientSize.Height - 50), new PointF(-2f, -2f)));
            returnlevel.LevelName = "Level " + levelnumber.ToString();
            returnlevel.MusicName = "ENDLESSCHALLENGE";
            returnlevel.levelballs.Add(new cBall(new PointF(30, 30), new PointF(2, 2)));
            
            /*
            for (int i = 0; i < PicGame.Width; i += 32)
            {
                returnlevel.levelblocks.Add(new RayBlock(new RectangleF(i, 50, 32, 16)));
            }
            */
            //returnlevel.levelblocks.Add(new RayBlock(new RectangleF(0,2,PicGame.Width,32)));
            for (int i = 0; i < PicGame.Width / 2; i += 32)
            {
                returnlevel.levelblocks.Add(new BoundedMovingBlock(new RayBlock(new RectangleF(PicGame.Width/2-(i/2), i, 32, 16)),
                                                            new PointF(2 + (float)(mRandom.NextDouble() * 3), 0)));
                returnlevel.levelblocks.Add(new BoundedMovingBlock(new RayBlock(new RectangleF(PicGame.Width / 2 + (i / 2), i, 32, 16)),
                                                            new PointF(2+ (float)(mRandom.NextDouble()*3), 0)));
            }
         
            returnlevel.levelblocks.Add(new NormalBlock(new RectangleF(PicGame.Width/2,5,32,16),new SolidBrush(Color.Red),new Pen(Color.Yellow)));



            return returnlevel;

        }
        

        private BCBlockGameState InitGameState(LevelSet useset, int startlevel)
        {
            BCBlockGameState newstate = new BCBlockGameState(this,PicGame,AvailableClientArea);
            BCBlockGameState.MainGameState = newstate;
            if(useset.Levels.Count < startlevel)
                throw new ArgumentException("Specified Level, " + startlevel.ToString() + " not found in given levelset, which contains " + useset.Levels.Count.ToString() + " Levels.");
            Level levelcopy = useset.Levels[startlevel-1];
            foreach(cBall levelball in levelcopy.levelballs)
            {
                newstate.Balls.AddLast(new cBall(levelball));


            }
            //newstate.Balls.Add(new cBall(newstate,new PointF(PicGame.ClientSize.Width/2,PicGame.ClientSize.Height-50),new PointF(-4f,-4f)));

            newstate.OnBallHitBottom += new BCBlockGameState.BallHitBottomProcedure(newstate_OnBallHitBottom);

            foreach(Block levelblock in levelcopy.levelblocks)
            {

                newstate.Blocks.AddLast((Block)levelblock.Clone());

                
            }

            /*
            for (int x = 0; x < PicGame.Width; x += 32)
            {


                for (int y = 0; y < PicGame.Height / 2; y += 16)
                {
                    if (mrandom.NextDouble() > 0.1)
                        newstate.Blocks.Add(new NormalBlock(newstate, new RectangleF(x, y, 32, 16), new SolidBrush(RandomColor()), new Pen(Color.Black, 1)));
                    else
                        newstate.Blocks.Add(new BombBlock(newstate, new RectangleF(x, y, 32, 16)));
                        
                    
                }
            }
             * */
            return newstate;


        }
        private PointF MidPoint(RectangleF ofrect)
        {

            return new PointF(ofrect.Left + (ofrect.Width / 2), ofrect.Top + (ofrect.Height / 2));


        }
       
       
        
        private IEnumerable<RectangleF> BlocksFromMatrix(int[][] Matrixuse)
        {

           
            //15/26, also zero.
            

            float usewidth = AvailableClientArea.Width / Matrixuse[0].Length;
            float useheight = AvailableClientArea.Height / Matrixuse.Length;

            for (int x = 0; x < Matrixuse.Length; x++)
            {
                for (int y = 0; y < Matrixuse[x].Length; y++)
                {
                    //if it is a black pixel..
                    if (Matrixuse[x][y]==1)
                    {
                        //add a new rectangle...
                        yield return new RectangleF(y*usewidth,x*useheight,usewidth,useheight);



                    }

                }




            }

            




        }

        private LevelSet CreateGameOverSet()
        {
            LevelSet returnset = new LevelSet();


            for (int i = 0; i < mPlayingSet.Levels.Count; i++)
            {
                Level golevel = new Level();

                golevel.levelballs.Add(new cBall(new PointF(15, 15), new PointF(2, 2)));
                golevel.SidebarTextColor = Color.Black;

                //now, we add the blocks.
                //a square of blocks, 10 blocks wide and 15 blocks high, centered around the middle.
                SizeF middleblocksize = new SizeF(32*10, 16*15);
                RectangleF middlecentered = GetCenteredRect(middleblocksize, AvailableClientArea);
                GraphicsPath usepath = new GraphicsPath();
              
                IEnumerable<RectangleF> userects = BlocksFromMatrix(GameOverMatrix); //known to work
                foreach (RectangleF createblockrect in userects)
                {
                    Block addthisblock;
                    if (BCBlockGameState.rgen.NextDouble() > 0.1f)
                        addthisblock = new InvincibleBlock(createblockrect);
                    else
                    {
                        addthisblock = new NormalBlock(createblockrect);
                    }
                    golevel.levelblocks.Add(addthisblock);



                }
                golevel.levelblocks.Add(new NormalBlock(new RectangleF(-300, -300, 32, 16)));
                //add the top line...
                /* for (int x = 0; x < middleblocksize.Width; x += 32)
                 {
                     Block newblock = new InvincibleBlock(new RectangleF(middlecentered.Left + x, middlecentered.Top, 32, 16));
                     golevel.levelblocks.Add(newblock);



                 }
                 //add the line on the left...
                 for (int y = 16; y < middleblocksize.Height; y += 16)
                 {

                     Block newblock = new InvincibleBlock(new RectangleF(middlecentered.Left, middlecentered.Top + y, 32, 16));
                     golevel.levelblocks.Add(newblock);


                 }
                 */


                returnset.Levels.Add(golevel);

            }


            return returnset;







        }
        bool HiscoreCheck()
        {


            String PlayerName = BCBlockGameState.Settings.PlayerName;

            int position = 0;
            // gamethread should be aborted. If it isn't- we do it.

            if ((GameThread!=null) && (GameThread.IsAlive) && GameThread != Thread.CurrentThread)
            {
                //abort it.
                GameThread.Abort();
                GameThread=null;

            }


            if ((position = mPlayingSet.HighScores.Eligible(PlayerName,(int)mGameState.GameScore)) > 0)
            {
                //score made! woop!

                HighScoreEntryMode(position,PlayerName);
                
                return true;

            }
            return false;

        }

        void GameOver()
        {
            if (GameThread != null && GameThread!=Thread.CurrentThread)
                GameThread.Abort();



            bool gotscore=false;
            BCBlockGameState.Soundman.StopMusic();
            //TODO: here is where we would "submit" the score to the HighScore management system... but ONLY if demo mode is off!
            //create the game over level.
            if (DemoMode == false)
            {
                gotscore=HiscoreCheck();


            }
            //if they didn't get a hi score, forcegameover().... otherwise, game over will be called after they enter their name.
            if(!gotscore) forcegameover();
        }
        private int HighScoreNamesMaxLength = 0;
        private int HighScoreScoresMaxLength = 0;
        private int HighScoresMaxSeparators = 0;
        private LevelSet restartset = null;
        private void forcegameover()
        {
            gamerunstate = new StateLevelOutroGameOver();

            LevelSet mGoverSet = CreateGameOverSet();
            DemoMode = true;
           
            ActualPlaySet = mPlayingSet;
            Nextshowhiscore = 0;
            GameOverHiScores = "Top - " + ActualPlaySet.SetName;

            //calculate maximum length of names and scores.
            HighScoreNamesMaxLength = Math.Min(ActualPlaySet.HighScores.GetScores().Max((w) => w.Name.Length),20);
            HighScoreScoresMaxLength = ActualPlaySet.HighScores.GetScores().Max((w) => w.Score.ToString().Length);
            HighScoresMaxSeparators= Math.Max(HighScoreNamesMaxLength+1,9);

            //cache the current set...
            restartset = mPlayingSet;
            tallydata.TallyImage = BCBlockGameState.Imageman.getImageRandom(mPlayingLevelobj.GameOverPicKey);
            PlayLevel(mGameState, mGoverSet.Levels[0]);
            
            gamerunstate = new StateLevelOutroGameOver();
        }


        protected iActiveSoundObject ds;
        
        private void ExplodePaddle()
        {
            if(mGameState.PlayerPaddle==null) return;
            Random rg = BCBlockGameState.rgen;
            Rectangle paddlerect = mGameState.PlayerPaddle.Getrect();
            for (int i = 0; i < 120; i++)
            {
                //todo: fix so this isn't called repeatedly because there is a ball outside...
                //(add another ball somewhere else, invisible, maybe?)

                int minvalue = Math.Min(paddlerect.Left, paddlerect.Right);
                int maxvalue = Math.Max(paddlerect.Left, paddlerect.Right);


                Point randomspot = new Point(rg.Next(minvalue, maxvalue), rg.Next(paddlerect.Top, paddlerect.Bottom));
                PointF rspot = randomspot.ToPointF();
                Particle dp = null;


                //Type[] particletypes = new Type[] { typeof(DustParticle),typeof(FireParticle),typeof(PolyDebris)};


                int rnum = rg.Next(1, 3);
                if (rnum == 1)
                    dp = new DustParticle(rspot);
                else
                {
                    PolyDebris pd = new PolyDebris(rspot, (rg.NextDouble()*4)-2, Color.YellowGreen);
                    pd.PenColor=Color.YellowGreen;
                    pd.BrushColor=Color.YellowGreen;
                    dp = pd;


                }
                mGameState.Particles.Add(dp);




            }
            //use Level death sound...

            //BCBlockGameState.Soundman.SetMusicVolume(0.0f);
            
            
            BCBlockGameState.Soundman.PauseMusic(true);
            BCBlockGameState.Soundman.Driver.OnSoundStop += new OnSoundStopDelegate(DeathMusicStop);
            
           
            ds = BCBlockGameState.Soundman.PlaySound(mPlayingLevelobj.DeathSound);
            mGameState.PlayerPaddle.InvokeOnDeath();
            mGameState.PlayerPaddle = null;
        }
        
        void DeathMusicStop(iActiveSoundObject objstop)
        {

            if (objstop == ds)
            {
                //remove this method...
                BCBlockGameState.Soundman.Driver.OnSoundStop -= DeathMusicStop;
                //reset the volume, too.
                try
                {
                    BCBlockGameState.Soundman.PauseMusic(false); //unpause...
                    //BCBlockGameState.Soundman.GetPlayingMusic_Active().setVolume(1.0f);
                }
                catch
                {
                    //ignored...
                }

            }

        }

        private DateTime DeathStart;
        void LifeLost()
        {
            //we cant lose a life if we are already "dying"
            Dorefreshstats = true;
            if(gamerunstate is StateDeath) return;

            //change  state to be death, and put a bunch of explodey particles all over.
            if (mGameState.PlayerPaddle != null)
            {
                Rectangle paddlerect = mGameState.PlayerPaddle.Getrect();
                
            /*    foreach (cBall createball in mGameState.Balls)
                {
                    createball.Velocity = new PointF(0, 0);
                    createball.Location = new PointF(-50, paddlerect.Bottom);
                    createball.Radius = 05f;
                    createball.DrawColor = Color.Blue;
                }
                */
                


                LevelDeathCount++;
                //cut macguffins in half and explode from the paddle.
                mGameState.MacGuffins /= 2;
                PointF SpawnGuffLocation = new PointF((float)mGameState.PlayerPaddle.Getrect().CenterPoint().X, mGameState.PlayerPaddle.Getrect().Top - 16);

                for (int i = 0; i < mGameState.MacGuffins; i++)
                {
                    //choose a random velocity. between PI and PI*2; this will be upwards-only.
                    double useangle = Math.PI + (BCBlockGameState.rgen.NextDouble()*Math.PI);
                    PointF RandomSpeed = BCBlockGameState.GetRandomVelocity(3, 5, useangle);
                    MacGuffinOrb mgo = new MacGuffinOrb(SpawnGuffLocation);
                    mgo.MacGuffinValue = BCBlockGameState.rgen.Next(0, 6);
                    mgo.Velocity = RandomSpeed;
                    mGameState.GameObjects.AddLast(mgo);


                }



                ExplodePaddle();
            }
            //remove all gamecharacter objects.
            List<GameCharacter> removethese = new List<GameCharacter>(from m in mGameState.GameObjects where m is GameCharacter select (GameCharacter)m);
            foreach (GameCharacter gchar in removethese)
            {
                
                mGameState.GameObjects.Remove(gchar);


            }
            gamerunstate = new StateDeath();
            DeathStart=DateTime.Now;
            

        }
        void PostLifeLost()
        {
            Dorefreshstats = true;

            if (mPlayingSet != null)
            {
                mPlayingSet.Statistics.Deaths++;

            }

            mGameState.playerLives--;
            //lblLives.Invoke(((MethodInvoker)(() => lblLives.Text = mGameState.playerLives.ToString())));

            gamethreadsuspended = true;
            //Thread.Sleep(500);
            gamethreadsuspended = false;
            gamerunstate = new StateRunning();
            if (mGameState.playerLives <= 0)
            {

                GameOver();



            }
            else
            {
                //reinitialize the balls from the "stored" levelset...
                
                if (mGameState.PlayerPaddle != null)
                {
                    lock (mGameState.PlayerPaddle) //lock it to prevent race condition.
                    {
                        //added June 24th 2011- Destroy() routine needs to be called when reinstantiating the paddle, otherwise
                        //the "old" paddle, and it's behaviours, will still have active hooks to any events they hooked.
                        foreach (var loopbeh in mGameState.PlayerPaddle.Behaviours)
                        {
                            loopbeh.UnHook();


                        }
                        mGameState.PlayerPaddle.Behaviours.Clear();
                    }
                }
                 
                mGameState.PlayerPaddle = new Paddle(mGameState, new Size(48, 15), new PointF(mGameState.GameArea.Width / 2, mGameState.TargetObject.Height - 35), BCBlockGameState.Imageman.getLoadedImage("paddle"));
                mGameState.PlayerPaddle.OnHPChange+=PlayerPaddle_OnHPChange;
                mGameState.PlayerPaddle.OnEnergyChange +=PlayerPaddle_OnEnergyChange;
                foreach (cBall loopball in mPlayingSet.Levels[mPlayingLevel - 1].levelballs)
                {
                    mGameState.Balls.AddLast((cBall)loopball.Clone());



                }

                //iterate through the levels blocks as well and see which ones we need to respawn...
                foreach (Block iterateblock in mPlayingSet.Levels[mPlayingLevel - 1].levelblocks)
                {
                    if (iterateblock.AutoRespawn)
                    {
                        mGameState.Blocks.AddLast((Block)iterateblock.Clone());

                    }
                }


            }




        }
        public void ForceDeath()
        {

            LifeLost();

        }
        void newstate_OnBallHitBottom(cBall ball)
        {
         
        }
        private Bitmap IntroSequenceBitmap;
        private Graphics IntroSequenceGraphic;
        public float CurrentFPS { get; set; }
        
        public void PlayLevel(BCBlockGameState stateobject, Level playthislevel)
        {
            
            Debug.Print("PlayLevel Entered...");
            //first, terminate the game thread, if it is running...
            bool ongamethread = (GameThread == Thread.CurrentThread);
            if (GameThread != null && GameThread != Thread.CurrentThread)
            {
                Debug.Print("GameThread is running; Aborting...");
                GameThread.Abort();



            }
            Debug.Print("Stopping playing music...");
            BCBlockGameState.Soundman.StopMusic();
            //next, we copy the balls and blocks from playthislevel...
            //clear any existing balls or blocks...
            //stateobject.Balls.Clear();
            //stateobject.Blocks.Clear();
            Debug.Print("Clearing current game state variables...");
            stateobject.GameObjects.Clear();
            stateobject.Particles.Clear();
            stateobject.Balls = new LinkedList<cBall>();
            stateobject.Blocks = new LinkedList<Block>();
            
            stateobject.RemoveBalls.Clear();
            Debug.Print("Cloning level data to current state...");
            foreach (cBall copyball in playthislevel.levelballs)
            {
                stateobject.Balls.AddLast((cBall) copyball.Clone());


            }


            foreach (Block copyblock in playthislevel.levelblocks)
            {

                stateobject.Blocks.AddLast((Block) copyblock.Clone());


            }
            if (stateobject.PlayerPaddle != null)
            {
              
                    lock (stateobject.PlayerPaddle) //lock it to prevent race condition.
                    {
                        //added June 24th 2011- Destroy() routine needs to be called when reinstantiating the paddle, otherwise
                        //the "old" paddle, and it's behaviours, will still have active hooks to any events they hooked.
                        foreach (var loopbeh in stateobject.PlayerPaddle.Behaviours)
                        {
                            loopbeh.UnHook();


                        }
                        stateobject.PlayerPaddle.Behaviours.Clear();
                    }
                

                
                stateobject.PlayerPaddle.OnHPChange += PlayerPaddle_OnHPChange;
                stateobject.PlayerPaddle.OnEnergyChange += PlayerPaddle_OnEnergyChange;
            }
            //LevelMessageQueue = new Queue<PlayMessageData>(playthislevel.MessageQueue.Clone());
            LevelMessageQueue = playthislevel.CreateMessageQueue();
            Debug.Print("Level contains " + stateobject.Balls.Count.ToString() + " balls and " +
                        stateobject.Blocks.Count.ToString() + " blocks.");
            Debug.Print("Playing level music...");
            PlayLevelMusic(playthislevel, true);


            //change sidebarimage if the Level has one to use.
            //the sidebarimage in the Level will be a key. if it's not null or empty, load the Image from the Image manager.
            SideBarImageCanvas = null; //nullify so PaintSideBarStats will redraw it.


            if (!String.IsNullOrEmpty(playthislevel.SidebarImageKey))
            {
                SidebarImage = BCBlockGameState.Imageman.getLoadedImage(playthislevel.SidebarImageKey);


            }
            else
            {
                SidebarImage = BCBlockGameState.Imageman.getLoadedImage("sidebarbg");

            }
            //create the bitmap of the introduction text.
            
            #region create introduction bitmap
            Debug.Print("Creating Intro sequence bitmap...");
            //mGameState.BackgroundDraw = new BackgroundColourImageDrawer(playthislevel);

            mGameState.PlayingLevel = playthislevel;
            mGameState.BackgroundDraw=playthislevel.Background;
            //CreateIntroBitmap(playthislevel, out IntroSequenceBitmap, out IntroSequenceGraphic);
            playthislevel.CreateIntroBitmap(out IntroSequenceBitmap, out IntroSequenceGraphic);
            #endregion
            
            //initialize textanimator.
            //IntroAnimator = new TextAnimationManager(playthislevel.LevelName, getLinearCharAnimator,new PointF(AvailableClientArea.Width/2,AvailableClientArea.Height/2 ));


            if (playthislevel.NoPaddle)
            {
                mGameState.PlayerPaddle=null;

            }
            Dorefreshstats = true;
            gamerunstate = new StateLevelIntro();
            mScoreatlevelstart = (int)mGameState.GameScore;
            mLevelParTime = playthislevel.CalculateParTime();
            mLevelIntroStartTime = DateTime.Now;
            mLevelTime = new TimeSpan(0);
            LevelDeathCount = 0;
            mRedrawAnimated=true;
            mredrawblocks=true;
            //initialize the AnimatedBlocks and StaticBlocks collections, otherwise we still won't
            //get them to draw.
            staticblocks = (from n in mGameState.Blocks where !n.RequiresPerformFrame() select n).ToList();
            AnimatedBlocks = (from n in mGameState.Blocks where n.RequiresPerformFrame() select n).ToList();
            if (!ongamethread)
            {
                Debug.Print("Not on game thread, creating new gamethread...");   
                GameThread = new Thread(gameproc);
                GameThread.Start();
                
            }
            else
            {
                Debug.Print("on game thread, aborting and instantiating anew...");
                //I R IDIOT. I was trying to fix another issue and changed this to use "Invoke" rather then BeginInvoke, which of course broke because it has to terminate <this> thread...

                this.BeginInvoke((MethodInvoker)(()=>
                                                     {
                                                         Debug.Print("creating new thread...");
                                                         Thread GameThreadx = new Thread(gameproc);
                                                         Debug.Print("Starting new thread...");
                                                         GameThreadx.Start();
                                                         Debug.Print("Aborting old thread...");
                                                         GameThread.Abort();
                                                         GameThread=GameThreadx;
                                                     }));
                Debug.Print("new thread is ID " + GameThread.ManagedThreadId);



            }


        }

        void PlayerPaddle_OnEnergyChange(Object sender,PaddleElementChangeEventArgs<float> earg )
        {
            Dorefreshstats = true;
        }

        void PlayerPaddle_OnHPChange(Object Sender,PaddleElementChangeEventArgs<float> earg )
        {
            if (mGameState.PlayerPaddle == null) { earg.Cancel = true; return; } //PlayerPaddle could be destroyed easily by the gameproc thread while the paddle's HP is being reduced.

            var lset = mGameState.ClientObject.GetPlayingSet();
            if (lset != null)
            {
                var difference = earg.NewValue - earg.OldValue;
                //negative==damage, positive is healing.
                if (Math.Sign(difference) == -1)
                    lset.Statistics.TotalDamage += Math.Abs(difference);
                else
                    lset.Statistics.TotalHealed += Math.Abs(difference);
                    
                
                

            }
            


            if (mGameState.PlayerPaddle.HP <= 0)
            {
                ExplodePaddle();
                //mGameState.PlayerPaddle = null;

            }
            Dorefreshstats = true;
           
            
        }
        /// <summary>
        /// determines whether the status bitmap needs to be redrawn and then pasted into the backbuffer.
        /// </summary>
        private bool _Dorefreshstats = true;
        public bool Dorefreshstats { get { return _Dorefreshstats; } set { _Dorefreshstats = value; } }
        private void PlayLevelMusic(Level playthislevel,bool playintromusic)
        {
            String playthissound = "";
            String usesound;
            iSoundSourceObject grabsound;
            if(!playintromusic)
                usesound = playthislevel.MusicName;
            else
            {
                usesound = playthislevel.IntroMusicName;
             
            }
            if (BCBlockGameState.Soundman.HasSound(usesound))
            {
                playthissound = usesound;

            }
            else if ((playthissound=BCBlockGameState.Soundman.getRandomSound(usesound)) != "")
            {
               

            }
            else if (BCBlockGameState.Soundman.HasSound(Path.GetFileNameWithoutExtension(usesound)))
            {
                playthissound = Path.GetFileNameWithoutExtension(usesound);

            }
            else if (File.Exists(playthislevel.MusicName))
            {

                playthissound = BCBlockGameState.Soundman.AddSound(usesound);

            }
            if (BCBlockGameState.Soundman.GetPlayingMusic() != null) BCBlockGameState.Soundman.StopMusic();
            var soundresult = BCBlockGameState.Soundman.PlayMusic(playthissound,true);
            if (playintromusic)
            {
                if (playthislevel.ShowNameLength.Ticks == 0)
                {
                    /*
                     Are you FUCKING KIDDING ME? WHY THE FUCKING HELL IS THIS FUCKING BROKEN? IT WORKED A FUCKIN YEAR AGO...
                     * This is a load of god-damned fucking bullshit is what it is. Commented this piece of shit out and now it just sets it to 5 fucking seconds, because FUCK YOU BASS.NET YOU GIGANTIC PILE OF FUCKING GARBAGE.
                     * 
                     */
                    playthislevel.ShowNameLength = new TimeSpan(0,0,5);
                    //playthislevel.ShowNameLength = BCBlockGameState.TimeSpanFromFloat(soundresult.Source.getLength());

                }
            }
            
         

        }

        void Driver_OnSoundStop(iActiveSoundObject objstop)
        {

            playintrodone = true;
            BCBlockGameState.Soundman.Driver.OnSoundStop -= Driver_OnSoundStop;
        }
        private bool playintrodone = false;
        private iActiveSoundObject PlayingIntroSound = null;
        public void StartGame(LevelSet startwith,int startlevel)
        {
            //additions.... accept level argument, or class representing level, etc.
            gamethreadsuspended=false;
            RightClickRestart = false; //Do not restart on right-click
            restartset=null;
            sidebarbgdrawn = false;
            if (GameThread != null && GameThread != Thread.CurrentThread)
            {
                GameThread.Abort();

            }
            
            
            if(startwith==null)
                mPlayingSet = CreateDefaultLevelSet();
            else
                mPlayingSet = startwith;
                
            
            
            //mGameState.GameScore = 0;
            
            mPlayingLevel = startlevel;
           // mGameState = InitGameState(startwith,startlevel);
            mGameState = new BCBlockGameState(this,PicGame,AvailableClientArea);
            BCBlockGameState.MainGameState = mGameState;
            mGameState.OnBallHitBottom += new BCBlockGameState.BallHitBottomProcedure(mGameState_OnBallHitBottom);
            mGameState.LevelComplete += new BCBlockGameState.LevelCompleteProc(mGameState_LevelComplete);
            mGameState.ScoreUpdate += new BCBlockGameState.ScoreUpdateRoutine(mGameState_ScoreUpdate);
            mGameState.NumCompletionsChanged += new Action(mGameState_NumCompletionsChanged);

            cNewSoundManager.Callback = this;

            PlayLevel(mGameState, mPlayingSet.Levels.First());
            //BCBlockGameState.Soundman.PlayMusic("endlesschallenge",1.0f);
            //GameThread = new Thread(gameproc);
            //GameThread.Start();
        }

        void mGameState_NumCompletionsChanged()
        {
            Dorefreshstats = true;
        }

        void mGameState_ScoreUpdate(ref long oldscore, ref long newscore)
        {
            //lblScore.Invoke((MethodInvoker)(()=>lblScore.Text=newscore.ToString()));
            if (DemoMode)
            {
                //ignore all updates to the score.
                newscore=oldscore;


            }
            if (Math.Sign(newscore - oldscore) == 1)
            {
                //positive, so increment add score statistic.
                mGameState.ClientObject.GetPlayingSet().Statistics.TotalScore += (int)(newscore - oldscore);

            }
            else if (Math.Sign(newscore - oldscore) == -1)
            {
                mGameState.ClientObject.GetPlayingSet().Statistics.TotalNegativeScore += (int)Math.Abs(newscore - oldscore);

            }
            Debug.Print("ScoreUpdate: oldscore=" + oldscore + " newscore=" + newscore);
            Dorefreshstats = true;
        }
        public void DelayInvoke(TimeSpan Delaytime, BCBlockGameState.DelayedInvokeRoutine routinefunc, object[] parameters)
        {

            if (mGameState != null)
            {
                mGameState.DelayInvoke(mLevelTime + Delaytime, routinefunc, parameters);


            }


        }
        void mGameState_LevelComplete()
        {

            Debug.Print("Level Complete");
            if(gamerunstate is StateLevelOutroGameOver) return;

//            lblLevel.Invoke((MethodInvoker)(() => lblLevel.Text = currlevel.ToString()));
//            PlayLevel(mGameState, mPlayingSet.Levels[currlevel-1]);
            mPlayingSet.Statistics.LevelsCompleted++;
            tallydata.TallyImage = BCBlockGameState.Imageman.getImageRandom(mPlayingLevelobj.TallyPicKey);
            gamerunstate = new StateLevelOutro();
            



        }

        void mGameState_OnBallHitBottom(cBall ball)
        {
      
                mGameState.Balls.Remove(ball);
            balldeath(ball);
        }
        int getNonTempBallCount()
        {
            return (mGameState.Balls.Count((w) => !w.isTempBall));

        }

        void balldeath(cBall balldie)
        {
           // mGameState.removeballs.Add(balldie);
            if ((mGameState.Balls.Count )<= 0)
            {
                Debug.Print("Life Lost");
                LifeLost();






            }
            


        }

        private DateTime LastDateTime=DateTime.Now;

        private bool AccelerateCountdown = false;

        private void CountDown(ref int sourcevalue, ref int destvalue, int amount)
        {
            //CountDown: removes amount from sourcevalue and adds it to destvalue.
            if (AccelerateCountdown) amount *= 500;

            if (sourcevalue < amount)
            {
                destvalue += sourcevalue;
                sourcevalue = 0;

            }
            else
            {
                destvalue+=amount;
                sourcevalue-=amount;



            }


        }
        public int GetTimeBonus(TimeSpan LevelTime, TimeSpan ParTime)
        {

            int buildresult = ((int)((ParTime - LevelTime).TotalSeconds)) * (5 * mPlayingLevel);
            int testvalue = (int)Math.Floor(LevelTime.TotalHours*100);
            if (CPrimes.IsPrime(testvalue))
            {
                buildresult += LevelTime.Seconds*25;


            }


            return buildresult;


        }

        //currently unused...

        //private static GameRunStateConstants[] LoopingStates = new GameRunStateConstants[] { GameRunStateConstants.Game_Menu, GameRunStateConstants.Game_Paused, GameRunStateConstants.Game_ValueInput };
        public TimeSpan Deathtime = new TimeSpan(0,0,0,2);
        public TextAnimationManager IntroAnimator;
        private bool CancelGameThread = false;
        private bool RightClickRestart = false; //true to allow rightclick to restart game. false otherwise.
        //more a special-case variable used for the gameover screen.
        public TimeSpan GetLevelTime()
        {
            return mLevelTime;

        }



        public void gameproc()
        {
            const int framedelay = 0;
            List<cBall> ballsadded = new List<cBall>();
            Debug.Print("gameproc entered.");
            CancelGameThread = false;
            while (!CancelGameThread)
            {

                //PaintSideBarstats();
                mGameState.HandleDelayInvoke(mLevelTime);
                mGameState.ProcessMessages();
                while (gamethreadsuspended)
                {
                    Application.DoEvents();
                    Thread.Sleep(100);

                }
                List<BCBlockGameState.NextFrameStartup> reenqueue = new List<BCBlockGameState.NextFrameStartup>();
                bool noerrorloop = false;
                while (!noerrorloop)
                {
                    try
                    {
                        while (mGameState.NextFrameCalls.Any())
                        {
                            //dequeue next item and call it's function
                            var deqobj = mGameState.NextFrameCalls.Dequeue();
                            if(deqobj!=null) //pranksters!
                                if (deqobj.NextFrame(deqobj, mGameState))
                                    reenqueue.Add(deqobj);


                        }
                        noerrorloop = true;
                    }
                    catch (InvalidOperationException exx)
                    {
                        noerrorloop = false;

                    }

                }
                foreach (BCBlockGameState.NextFrameStartup readd in reenqueue)
                {

                    mGameState.NextFrameCalls.Enqueue(readd);

                }

                while (gamepauseamount > 0)
                {
                    gamepauseamount--;
                    Application.DoEvents();


                }
                try
                {
                    if (gamerunstate is StateDeath)
                    {
                        //has the time expired? if so call the post-death routine...
                        if (DateTime.Now - DeathStart > Deathtime)
                            PostLifeLost();


                    }
                    while (gamerunstate.IsLoopingState)
                    {
                       
                        while (gamerunstate is StateMenu)
                        {


                            Application.DoEvents();
                            Thread.Sleep(5);
                            PicGame.Invoke((MethodInvoker) (() =>
                                                                {
                                                                    PicGame.Invalidate();
                                                                    PicGame.Update();
                                                                }))
                                ;


                        }
                        while (gamerunstate is StatePaused)
                        {
                            Thread.Sleep(100);
                            Application.DoEvents();
                            PicGame.Invoke((MethodInvoker)(() =>
                            {
                                PicGame.Invalidate();
                                PicGame.Update();
                            }))
                                ;
                        }

                        #region Cheat Input

                        while (gamerunstate is StateValueInput)
                        {
                            var result = gamerunstate.Run(mGameState);
                            if (result != null) gamerunstate = result;
                            

                        }
                    
                    //debugging code. Seems the Game_Menu setting doesn't play friendly with the other stuff. TSSK TSSK.
                    if (gamerunstate is StateMenu)
                    {
                        Debug.Print("Menu");

                    }

                    #endregion
                    }
                    //const int levelintroticks=500;
                    int elapsedintroticks = 0;
                    int elapsedoutroticks = 0;
                    DateTime startlevelintroloop = DateTime.Now;
                    bool foundanimated = false;
                    #region LevelIntro
                    while (gamerunstate is StateLevelIntro)
                    {
                        //TODO: there is a bug somewhere; AnimatedBlocks are not drawn properly during the intro sequence. (usually, they show up as normal blocks instead for some reason).
                        //addendum to the above: now they no longer draw at all during the intro sequence.
                        //do nothing but "wait" for a specific period. 
                        //elapsedintroticks++;
                        //Debug.Print("elapsedintroticks:" + elapsedintroticks.ToString() + "gamerunstate:" + gamerunstate.ToString());
                        
                        //tweak: if shownamelength is negative, then when we had played the music using PlayLevelMusic the  routine
                        //will have hooked the sound stop event; we use that to determine when to end, since it also sets a flag.
                        if(((DateTime.Now-startlevelintroloop)>mPlayingSet.Levels[mPlayingLevel-1].ShowNameLength)
                            || (mPlayingSet.Levels[mPlayingLevel - 1].ShowNameLength.Ticks == 0 && playintrodone))
                            
                            
                        {


                            elapsedintroticks = 0;
                            //play the Level Music.
                            mLevelTime=new TimeSpan(0);
                            LastDateTime = DateTime.Now;
                            PlayLevelMusic(mPlayingSet.Levels[mPlayingLevel - 1], false);
                            gamerunstate = new StateRunning();
                          

                        }
                        
                    


                        foreach (Block LoopBlock in mGameState.Blocks)
                        {
                            if (LoopBlock is AnimatedBlock)
                            {

                              
                                    foundanimated = true;
                                    if ((LoopBlock).PerformFrame(mGameState)) mredrawblocks = true;

                                



                            }


                        }
                       // IntroAnimator.PerformFrame();
                        PicGame.Invoke((MethodInvoker)(() =>
                        {
                            //if(mredrawblocks) 
                            PicGame.Invalidate();
                            PicGame.Update();
                        }));
                        mredrawblocks=false;

                    }
                    #endregion
                    //int elapsedoutroticks = 0;

                    //otherwise we are in the gameover or level outtro...

                    #region Leveloutro and outro gameover
                    DateTime startleveloutroloop = DateTime.Now;
                    int Countdownlevelscore=(int)mGameState.GameScore-mScoreatlevelstart;
                        //the level score counts down, and is added to our initial level score (mScoreAtlevelstart)
                        int Addupgamescore=mScoreatlevelstart;
                        //game score that goes from scoreatlevelstart to CountDownlevelscore
                        //int countdownbonusscore=250*(mPlayingLevel+1);


                        int countdownbonusscore = GetTimeBonus(mLevelTime, mLevelParTime);
                        
                        //also, bonus for each ball in play. (why the heck not...)
                        countdownbonusscore += (mGameState.Balls.Count * 50);
                    //and add some score for macguffins
                    //1 + (x/500)

                        countdownbonusscore += (int)((countdownbonusscore * (1 + ((float)mGameState.MacGuffins / 500))));
                        countdownbonusscore = (int)(countdownbonusscore * mGameState.ScoreMultiplier);
                        //string tallykey = BCBlockGameState.Soundman.getRandomSound("TALLYTICK");
                    String tallykey="";
                    lock (mPlayingSet)
                    {
                        if (!(mPlayingLevel - 1 > mPlayingSet.Levels.Count))
                            tallykey = mPlayingSet.Levels[0].TallyTickSound;
                        else
                            tallykey = mPlayingSet.Levels[mPlayingLevel - 1].TallyTickSound;
                    }
                    int totalbonus = countdownbonusscore;
                    TimeSpan aSecond=new TimeSpan(0,0,0,2);
                        //bonus score is added after. 
                    String writeString="";
                    bool playingrollsound = false;
                    bool tallyfinished=false;
                    
                    int scoredecrement;
                    scoredecrement = Countdownlevelscore / 100;
                    if (scoredecrement == 0) scoredecrement = 1;

                    DateTime finishtallytime = DateTime.Now.AddDays(1);
                    
                    bool doGameOver = gamerunstate is StateLevelOutroGameOver;
                    if (doGameOver)
                        countdownbonusscore = (int)(Countdownlevelscore * -0.4);
                    while (gamerunstate is StateLevelOutro || gamerunstate is StateLevelOutroGameOver)
                    {
                        
                        
                        //note: we wait a full second before we start the "counting"...
                        if (!playingrollsound)
                        {
                            if (doGameOver)
                            {
                                //bugfix: when we get here, turns out the GameOver LevelSet had already been initialized. Or something.
                                
                                    BCBlockGameState.Soundman.PlayMusic(mPlayingSet.Levels[0].GameOverMusic,1.0f,false);
                            }
                            else
                                //musicobj = BCBlockGameState.Soundman.PlayMusic("TALLY", 1.0f, false);
                                BCBlockGameState.Soundman.PlayMusic(mPlayingSet.Levels[mPlayingLevel - 1].TallyMusicName, 1.0f, false);
                        
                        }
                        
                        playingrollsound = true;
                        if((DateTime.Now - startleveloutroloop) > (aSecond))
                        {
                            //ok, good. perform a single "frame" of countdown.
                            //if our levelcountdown is non-zero, take some off of it, and add it to countdowngamescore.
                         
                            if(Countdownlevelscore>0)
                            {
                                
                                CountDown(ref Countdownlevelscore,ref Addupgamescore,scoredecrement);
                                BCBlockGameState.Soundman.PlaySound(tallykey, 0.9f);


                            }
                                
                            else if(countdownbonusscore!=0)
                            {
                                CountDown(ref countdownbonusscore,ref Addupgamescore,Math.Sign(countdownbonusscore)*50);
                                BCBlockGameState.Soundman.PlaySound(tallykey, 0.9f);

                            }
                            else if(!tallyfinished)
                            {
                                finishtallytime=DateTime.Now;
                                tallyfinished=true;

                            }


                        }
                        if((DateTime.Now-finishtallytime)>=(aSecond+aSecond))
                        {
                            if (doGameOver)
                            {
                               

                            }
                            else
                            {
                                mGameState.GameScore += totalbonus;
                                BlockDrawBuffer.Clear(Color.Transparent);
                                int gotnext = mPlayingSet.Levels[mPlayingLevel - 1].NextLevel;
                                int currlevel;
                                if(gotnext==0)
                                    currlevel= mPlayingLevel + 1;
                                else
                                    currlevel=gotnext;
                                    
                                


                                if (currlevel > mPlayingSet.Levels.Count())
                                {
                                    //MessageBox.Show("You have defeated all levels...");
                                    //a "completion" occurs when any level is finished that tries to go to a level higher then the number of levels.

                                    mGameState.NumCompletions++;
                                    sidebarbgdrawn = false;
                                    gamerunstate= new StateRunning();
                                    
                                    currlevel = 1;
                                }
                                //currlevel = 1;
                                mPlayingLevel = currlevel;
                                long usescore = mGameState.GameScore;
                                mGameState.invokescoreupdate(ref usescore,ref usescore);
                                PlayLevel(mGameState, mPlayingSet.Levels[mPlayingLevel-1]);
                            }
                        }
                        if (doGameOver)
                        {
                            if (KeyboardInfo.IsPressed(Keys.LButton))
                            {
                                lastscoreshow = DateTime.Now - new TimeSpan(0, 0, 0, 0, ScoreRevealDelayTime+1); 
                            }
                           //the game over screen will also show the high scores of that set.
                            //it does this on it's own, but we need to initialize the string...


                            //if the time has elapsed..
                            if ((DateTime.Now - lastscoreshow).TotalMilliseconds > ScoreRevealDelayTime)
                            {
                                //add the next score entry to the string...

                                if (Nextshowhiscore <= 9)
                                {
                                    
                                    var nextentry = ActualPlaySet.HighScores.Scores.ElementAt(ActualPlaySet.HighScores.Scores.Count- Nextshowhiscore-1).Value;
                                    int useseparators = HighScoresMaxSeparators - nextentry.Name.Length;
                                    String usespaces = (Nextshowhiscore+1==HighScorePos)?"**":"  ";
                                    String usename = nextentry.Name;
                                    if (usename.Length > 22) usename = usename.Substring(0, 20) + ">>";
                                    String nextshow = String.Format("{0:00}",Nextshowhiscore+1) + ":" + usespaces + usename + Repeat(".",useseparators+4) + nextentry.Score;
                                    GameOverHiScores += "\n" + nextshow;
                                    BCBlockGameState.Soundman.PlaySound("revel");

                                    //GameOverHiScores 
                                    lastscoreshow = DateTime.Now;
                                    Nextshowhiscore++;
                                }
                                else if (Nextshowhiscore == 10)
                                {
                                    //show a blank...
                                    GameOverHiScores += "\n" + Repeat(".", HighScoresMaxSeparators+1);
                                    BCBlockGameState.Soundman.PlaySound("revel");
                                    lastscoreshow = DateTime.Now;
                                    Nextshowhiscore++;
                                }
                                else if (Nextshowhiscore == 11)
                                {

                                    GameOverHiScores += "\nPress Button B";
                                    BCBlockGameState.Soundman.PlaySound("revel");
                                    lastscoreshow = DateTime.Now;
                                    Nextshowhiscore++;
                                }
                                else if (Nextshowhiscore == 12)
                                {
                                    GameOverHiScores += "\nTo Play Again.";
                                    BCBlockGameState.Soundman.PlaySound("revel");
                                    CancelGameThread = true;
                                    RightClickRestart = true;
                                    //Special code:
                                    //we want to STOP the gameproc, since it's job is essential done at this point.



                                    BeginInvoke((MethodInvoker)(() => { PicGame.Invalidate(); PicGame.Update(); GameThread.Abort(); }));
                                    Nextshowhiscore++;

                                }

                            }

                            //Yield.
                            Thread.Sleep(10);
                            writeString = "   GAME OVER   \n";
                            writeString += "LEVEL:      " + mPlayingLevel.ToString() + "\n" +
                                           "SCORE:      " + mGameState.GameScore.ToString();
                        }
                        else
                        {

                            writeString = mGameState.PlayingLevel.ClearTitle;


                            writeString += "LEVEL:      " + Countdownlevelscore.ToString() + "\n" +
                                           "TIME:       " + FormatLevelTime(mLevelTime) + "\n" +
                                           "PAR TIME:   " + FormatLevelTime(mLevelParTime) + "\n" +
                                           "BONUS:      " + countdownbonusscore + "\n" +
                                           "TOTAL:      " + Addupgamescore + "\n";
                        }
                        tallydata.TallyScreenString=writeString;
                        //Debug.Print("elapsedintroticks:" + elapsedintroticks.ToString() + "gamerunstate:" + gamerunstate.ToString());
                        
                        AnimateParticles();
                        RefreshFrames();




                    }
                    #endregion
                    //Thread.Sleep(framedelay);

                    //while ((DateTime.Now - LastDateTime).Milliseconds < 3)
                    //{
                    //}
                    Thread.Sleep(0);
                    bool blocksaffected = false;
                    List<Block> blocksfromperformframe;
                    //go through all balls...



                    if (mGameState.Balls != null)
                    {
                        //purge balls beyond our limit.
                        if (mPlayingLevelobj.MaxBalls > 0 && mGameState.Balls.Count() > mPlayingLevelobj.MaxBalls)
                        {
                            while (mGameState.Balls.Count() > mPlayingLevelobj.MaxBalls)
                                mGameState.Balls.RemoveLast();


                        }


                        cBall continueball = null;
                        cBall lastball = null;
                        ballsadded.Clear();
                        restartloop:

                        try
                        {
                            //initially start with the assumption that no blocks will need to be redrawn.
                            if (mGameState.Balls.Count == 0)
                            {
                                LifeLost();

                            }
                            mredrawblocks = false;
                            //foreach (cBall loopball in mGameState.Balls)
                            // for(LinkedListNode<cBall> loopballnode = mGameState.Balls.First;
                            //    loopballnode!=null;loopballnode=loopballnode.Next)
                            // for(LinkedListNode<cBall> loopballnode = mGameState.Balls.Last;
                            //    loopballnode!=null;loopballnode=loopballnode.Previous)
                            cBall lowestBall=null;
                            //naturally, we iterate through every ball.

                            //testing: now clones the gamestate balls list...
                            if (mGameState.ShootBalls.Count > 0)
                            {
                                lock (mGameState.Balls)
                                {
                                    mGameState.Balls.AddRangeAfter(mGameState.ShootBalls);

                                }
                                mGameState.ShootBalls.Clear();
                            }
                            LinkedList<cBall> cloneballs = mGameState.Balls;
                            lock (mGameState.Balls)
                            {
                                //cloneballs = new LinkedList<cBall>(mGameState.Balls);


                                foreach (cBall loopball in mGameState.Balls)
                        
                                    {
                                       
                                        //cBall loopball = loopballnode.Value;
                                        if (loopball != null)
                                        {
                                            //cBall loopball = mGameState.Balls[i];

                                            if (lowestBall == null || loopball.Location.Y > lowestBall.Location.Y)
                                            {
                                                if ((Math.Sign(loopball.Velocity.Y)) > 0)
                                                    lowestBall = loopball;
                                                //used by demo mode (and possibly later by other stuff) stores the lowest ball that is coming towards the paddle. (well, actually, it's the lowest ball that 
                                                //is going downwards.
                                            }
                                            if (continueball == null || continueball == loopball)
                                            {
                                                continueball = null;
                                                //make sure the ball is set to call us back if an impact occurs.
                                                if (!loopball.hasBallImpact())
                                                    loopball.BallImpact += loopball_BallImpact;


                                                List<cBall> ballsremove = new List<cBall>();
                                                //Hey ball, can you move a single frame and give us back some info on the blocks
                                                //that you affected? thanks.
                                                blocksfromperformframe = loopball.PerformFrame(mGameState, ref ballsadded,
                                                                                               ref ballsremove);
                                                //Blocksfromperformframe is now a collection of Blocks that were affected by this frame.
                                                
                                                foreach (cBall loopremball in ballsremove)
                                                {

                                                    // mGameState.Balls.Remove(loopremball);
                                                    //kill each ball in the removing collection.
                                                    balldeath(loopremball);

                                                    


                                                    

                                                }
                                                //mredrawblocks = mredrawblocks || (blocksfromperformframe.Count() > 0);

                                                mredrawblocks = mredrawblocks ||
                                                                (blocksfromperformframe.Any((w) => !(w.RequiresPerformFrame())));
                                                mRedrawAnimated = mredrawblocks ||
                                                                  (blocksfromperformframe.Any((w) => (w.RequiresPerformFrame())));
                                                //mredrawblocks=true;  

                                            }
                                            lastball = loopball;
                                        }

                                    }
                            }
                            Func<Block, bool> checkfrozen= ((a) => {
                                if (a is AnimatedBlock)
                                    if (((AnimatedBlock)a).Frozen)
                                        return true; //it's frozen.


                                return false;
                            
                            
                            });
                            //now, use LINQ to grab all Animated blocks Blocks that require animation.
                            #region block animation 
                            var result = from x in mGameState.Blocks where x.RequiresPerformFrame()     select x;
                                AnimatedBlocks = result.ToList();

                                staticblocks =(from x in mGameState.Blocks where !x.RequiresPerformFrame() select x).ToList();

                            foreach (Block loopblock in result)
                            {
                                /*if (loopblock is DemonBlock)
                                {
                                    Debug.Print("break");


                                }*/
                                //FIX: changed order so that performframe is called regardless.
                                bool hc=false;
                                hc = loopblock.PerformFrame(mGameState) || loopblock.hasChanged;
                                
                                if(hc) mRedrawAnimated=true;

                            }
                            #endregion 
                            //combo check.
                            if (mGameState.ComboCount > 1 && mGameState.ComboFinishTime!=null)
                            {
                                Font fontuse = new Font("Arial", 28);
                                //mGameState.ComboCountDown--;
                                //Debug.Print("ComboCountdown:" + mGameState.ComboCountDown);

                                

                                int clamped = BCBlockGameState.ClampValue(mGameState.ComboCount-1, 0, BCBlockGameState.ComboNames.Length - 1);
                                if (mGameState.ComboFinishTime.Value < DateTime.Now)
                                {
                                    mGameState.ComboFinishTime = null;
                                    string endquote = " chained)";
                                    if((mGameState.ComboCount - 1)==1) endquote = " chain)";
                                    string combotext = BCBlockGameState.ComboNames[clamped] + " \nCombo!" + "(" + (mGameState.ComboCount - 1) + endquote;
                                    SizeF calcsize = BCBlockGameState.MeasureString(combotext, fontuse);

                                    PointF initialpos = new PointF(mGameState.GameArea.Width / 2,
                                        mGameState.GameArea.Height);


                                    Brush drawbrush = new SolidBrush(BCBlockGameState.ComboFillColour[clamped]);
                                    Pen drawpen = new Pen(BCBlockGameState.ComboPenColour[clamped]);


                                    Block.PopupText(mGameState, combotext, fontuse, drawbrush, drawpen, 500, new PointF(0, -1.05f), initialpos);
                                    BCBlockGameState.Soundman.PlaySound(BCBlockGameState.ComboNames[clamped],2.0f);
                                    mGameState.ComboCount = 0;

                                }



                            }
                            Block.HandleEffects(mGameState);

                            #region particle animation
                            AnimateParticles();

                            #endregion 


                            //Changeup: now inspect to see if we need to add a message.
                            //the latest message will be 
                            //mPlayingLevelobj.MessageQueue.Peek()

                            //grab topmost...
                            if (LevelMessageQueue!=null && LevelMessageQueue.Count>0)
                            {
                                var topmostmessage = LevelMessageQueue.Peek();


                                if (topmostmessage.TotalMS < (mLevelTime).TotalMilliseconds)
                                {
                                    LevelMessageQueue.Dequeue();
                                    topmostmessage.Invoke(mGameState);
                                    //enqueue a new item, add the length of the playing music to the timeindex...
                                    float accumlength = BCBlockGameState.Soundman.GetPlayingMusic().getLength();
                                    accumlength += topmostmessage.TimeIndex;
                                    //create a new Message...
                                    var clonedmess = (PlayMessageData)topmostmessage.Clone();
                                    clonedmess.TimeIndex = accumlength;
                                    LevelMessageQueue.Enqueue(clonedmess);

                                    




                                }


                            }


                            AnimateGameObjects();

                            if (DemoMode)
                            {
                                //reposition the paddle to be in the path of the lowest ball. we cached that earlier.
                                if (lowestBall != null)
                                {
                                    if(mGameState.PlayerPaddle!=null)
                                        mGameState.PlayerPaddle.Position = new PointF(lowestBall.Location.X,
                                                                                  mGameState.PlayerPaddle.Position.Y);
                                }



                            }
                            int gotcount = mGameState.Blocks.Count((w) => (w.MustDestroy() == true));
                            if (gotcount == 0)
                            {
                                
                                //LifeLost();
                                mGameState.invokeLevelComplete();
                            }
                        }
                        catch (Exception e)
                        {
                            Trace.WriteLine("Exception in loop :(" + e.Message +  " Stack:{" + e.StackTrace + "}");

                            continueball = lastball;
                            //goto restartloop;
                        }

                        foreach (cBall addthis in ballsadded)
                        {
                            mGameState.Balls.AddLast(addthis);



                        }
                        foreach (cBall removethis in mGameState.RemoveBalls)
                        {
                            mGameState.Balls.Remove(removethis);

                            
                        }

                        mGameState.RemoveBalls.Clear();

                        try
                        {

                           // mredrawblocks = mredrawblocks || mGameState.Blocks.Any((w) => (w.hasChanged&&((w is AnimatedBlock))));

                            PicGame.Invalidate();
                            PicGame.Invoke((MethodInvoker)(() => { PicGame.Invalidate(); PicGame.Update(); }));
                            //Application.DoEvents();
                        }
                        catch
                        {


                        }
                    }
                }



                catch (ThreadAbortException e)
                {

                    Debug.Print("GameProc thread aborted");

                }



                if (mGameState.Blocks.Count == 0)
                {
                    mGameState.invokeLevelComplete();
                    

                }
                if ((gamerunstate is StateRunning) && (LastDateTime.Ticks > 0))
                {

                    if (TotalPauseTime != null)
                    {
                        //totalpausetime is initialized when the game is unpaused, and will contain the total time the game was paused.
                        //don't add anything to the leveltime, and reinitialize LastDateTime.
                        TotalPauseTime=null;

                    }
                    else
                    {
                        mLevelTime += (DateTime.Now - LastDateTime);
                    }
                }
                LastDateTime = DateTime.Now;
            }
        
            
        }
        
        private void RefreshFrames()
        {
            PicGame.Invoke((MethodInvoker)(() =>
                                               {
                                                   PicGame.Invalidate();
                                                   PicGame.Update();
                                               }));
        }

        private String FormatLevelTime(TimeSpan LevelTime)
        {
            if(LevelTime.Hours >0)
                return new DateTime(LevelTime.Ticks).ToString("hh:mm:ss.ff"); 
            else
                return new DateTime(LevelTime.Ticks).ToString("mm:ss.ff"); 








        }

        private void AnimateGameObjects()
        {
            List<GameObject> lAddObjects = new List<GameObject>();
            List<GameObject> lremoveObjects = new List<GameObject>();
            //bugfix: seems that issues will be abound if collections are modified during the loop (duh).
            //but mostly because execution leaves this procedure entirely, leaving the gameobjects to add and gameobjects to remove collections untransferred.
            GameObject lastinstance = null;
            try

            {
                
                lock (mGameState.GameObjects)
                {
                    foreach (GameObject loopgobject in mGameState.GameObjects)
                    {
                        if (loopgobject.Frozen) continue; //frozen objects can't move...
                        lastinstance = loopgobject;
                        if (loopgobject is SpinShot)
                        {
                            Debug.Print("test");
                        }
                        if (loopgobject.PerformFrame(mGameState))
                        {
                            
                            lremoveObjects.Add(loopgobject);
                        }

                    }
                    if (mGameState.Forcerefresh)
                    {
                        mredrawblocks = true;
                        mGameState.Forcerefresh = false;


                    }
                }
            }

            catch (InvalidOperationException eex)
            {
                //unused catch handler. 
                Debug.Print("InvalidOperationException Caught in AnimateGameObjects-" + eex.ToString());
            }
            

            //add the added objects...
            lock (mGameState.GameObjects)
            {

                foreach (GameObject addthisobject in lAddObjects)
                    mGameState.GameObjects.AddLast(addthisobject);

                foreach (GameObject removethisobject in lremoveObjects)
                {
                    if (removethisobject is GameEnemy)
                    {
                        GameEnemy caste = removethisobject as GameEnemy;
                        List<GameObject> addtemp= new List<GameObject>();
                        List<GameObject> removetemp = new List<GameObject>();
                        caste.InvokeOnDeath(mGameState, ref addtemp, ref removetemp);

                    }
                    mGameState.GameObjects.Remove(removethisobject);
                }
            }
        }
        private int particlelimit = Int32.Parse(BCBlockGameState.GameSettings["game"]["maxparticles","1500"].Value);
        private List<Particle> mremoveparticles = new List<Particle>();
        private void AnimateParticles()
        {
            
            mremoveparticles.Clear();
            lock (mGameState.Particles)
            {
                /*
                if (mGameState.Particles.Count > particlelimit)
                {
                    //if above the particle limit, remove the "oldest" ones. This is more a guess then anything, we just remove the ones at the start until we are below the limit.
                    int numremove = particlelimit - mGameState.Particles.Count;
                    if (numremove > 0) mGameState.Particles.RemoveRange(0, numremove); //tada...

                }
                */
                int totalparticles = mGameState.Particles.Count((w)=>!w.Important);
                foreach (Particle loopparticle in mGameState.Particles)
                {
                    if(loopparticle!=null)
                        if (loopparticle.PerformFrame(mGameState) || totalparticles > MaxParticles)
                        {
                            mremoveparticles.Add(loopparticle);
                            totalparticles--;
                        }


                }

                foreach (Particle removeparticle in mremoveparticles)
                    mGameState.Particles.Remove(removeparticle);
            }
        }
        public String Repeat(String repeatthis, int count)
        {
            count = Math.Abs(count);
            if (String.IsNullOrEmpty(repeatthis)) return "";
            StringBuilder buildresult = new StringBuilder(repeatthis.Length * count);
            for (int i = 0; i < count; i++)
            {
                buildresult.Append(repeatthis);

            }
            return buildresult.ToString();
        }

        void loopball_BallImpact(cBall ballimpact)
        {

            if ((ballimpact.numImpacts> 0) && ballimpact.isTempBall)
            {


                if (mGameState.Balls.Count() == 1 && mGameState.Balls.Contains(ballimpact))
                {
                    //we are the last ball, promote to a true ball.
                    ballimpact.isTempBall= false;



                }
                try
                {
                   // mGameState.Balls.Remove(ballimpact);
                    
                    mGameState.RemoveBalls.Add(ballimpact);
                }
                catch
                {

                }


            }

            //throw new NotImplementedException();
            
        }

        public frmBaseBlock()
        {
            InitializeComponent();
        }
        const int WM_DWMCOMPOSITIONCHANGED = 0x031E;
        protected override void WndProc(ref Message m)
        {
            if (m.Msg == WM_DWMCOMPOSITIONCHANGED)
            {
                //set the paint handler based on the current Composition state.
                setPaintHandler();


            }


            base.WndProc(ref m);
        }
        GameControllerManager gcm = null;
        private void frmPoing_Load(object sender, EventArgs e)
        {
            
            setPaintHandler();
        SidebarImage = BCBlockGameState.Imageman.getLoadedImage("sidebarbg");

        ButtonDown += frmBaseBlock_ButtonDown;
        ButtonUp += frmBaseBlock_ButtonUp;
            ClientSize = new Size(704, 427+menuStrip1.Height+6);
            StandardSize = new Rectangle(0,0,ClientSize.Width,ClientSize.Height);
            PicGame.Location = new Point(0, menuStrip1.Bottom);
            ClientSize = new Size(PicGame.Size.Width,PicGame.Size.Height+menuStrip1.Height);
            //background is the full size of picgame, so that it can hold the sidebar bmp.
            BackBufferBitmap = new Bitmap(PicGame.Width, PicGame.Height);

            backgroundbitmap = new Bitmap(PicGame.Width, PicGame.Height, PixelFormat.Format32bppArgb);
            BlockDrawBitmap = new Bitmap(AvailableClientArea.Width, AvailableClientArea.Height,PixelFormat.Format32bppArgb);
            MovingBlockBitmap = new Bitmap(AvailableClientArea.Width, AvailableClientArea.Height, PixelFormat.Format32bppArgb);

            backBufferCanvas = Graphics.FromImage(BackBufferBitmap);
            BackgroundBuffer = Graphics.FromImage(backgroundbitmap);
            BlockDrawBuffer = Graphics.FromImage(BlockDrawBitmap);
            MovingBlockBuffer = Graphics.FromImage(MovingBlockBitmap);

            backBufferCanvas.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            backBufferCanvas.CompositingQuality = CompositingQuality.HighSpeed;
            BackgroundBuffer.CompositingQuality = CompositingQuality.HighSpeed;
            BlockDrawBuffer.CompositingQuality = CompositingQuality.AssumeLinear;
            MovingBlockBuffer.CompositingQuality = CompositingQuality.HighSpeed;
//System.Collections.Generic.L


            SidebarBitmap = new Bitmap(SidebarImage);
            //MessageBox.Show(SidebarBitmap.Size.ToString());
            sidebarGraphics = Graphics.FromImage(SidebarBitmap);

            BackgroundBuffer.Clear(Color.Transparent);
            BlockDrawBuffer.Clear(Color.Transparent);

BCBlockGameState.setMenuRenderer(BCBlockGameState.GameSettings);

            //initialize Game Controller Manager.
            gcm = new GameControllerManager(BCBlockGameState.MTypeManager[typeof(iGameInput)].ManagedTypes,this);
            
            DoAutoLoad(Environment.CommandLine);



        }
        void ResizeForm()
        {
            Rectangle WorkingArea = Screen.FromHandle(this.Handle).WorkingArea;
            Size WorkSize = WorkingArea.Size;
            int i = 1;
            Size useSize = Size;
            while(true)
            {
                Size testsize = new Size(Width*i,Height*i);
                //if the testing size is too big, break.
                if (testsize.Width > WorkSize.Width || testsize.Height > WorkSize.Height) break;
                useSize = testsize;
                i++;
            }

            Size = useSize;

        }
        void frmBaseBlock_ButtonUp(object sender, ButtonEventArgs<bool> e)
        {
            //x |= Time.Past;
            currentlypressed &= ~e.Button;
        }

        void frmBaseBlock_ButtonDown(object sender, ButtonEventArgs<bool> e)
        {
            currentlypressed |= e.Button;
        }
        private Graphics SideBarImageCanvas;
        private Bitmap SideBarImageBitmap;
       // private void PaintSideBar(Graphics g)
     //   {
     
            
            
            



    //    }
        private void PaintSideBarstats()
        {

            //TODO: current implementation is somewhat less than optimal.
            //sidebar is painted every frame. ideally, we would only paint a new sidebar image to the backbuffer when necessary, and any other frame would be restricted to only
            //painting within the client area via clipping.

            //rects defined on the sidebar
            //Level name/title rect:
            //(13,12)-(197,57)
            //score/lives etc rect:
            // (13,80)-(197,243)

            //(13,390)-(197,415) //extra info rect (url probably, or copyright, etc.)
           //PaintSideBar(g);

            //task, if necessary, redraw the sidebar.
            //if (!sidebarbgdrawn)
            //{

            //if no sidebar image or canvas is defined, create it.
            //the idea is, the sidebar stuff is stored in it's own "bitmap" which is refreshed when necessary, but otherwise we simply Draw that
            //sidebar image verbatim.
            //SideBarImage is the actual "image"...
            
            if (SideBarImageCanvas == null)
            {
                SideBarImageBitmap = new Bitmap(BackBufferBitmap.Width-AvailableClientArea.Width,BackBufferBitmap.Height);
                SideBarImageCanvas = Graphics.FromImage(SideBarImageBitmap);

            }
            SideBarImageCanvas.Clear(Color.Transparent);


           // if (!sidebarbgdrawn)
          //  {
                //BackgroundBuffer.DrawImageUnscaled(SidebarImage, AvailableClientArea.Width, 0,50,PicGame.Height);
                //MessageBox.Show(SidebarImage.Size.ToString()); 
                //backBufferCanvas.CompositingQuality = CompositingQuality.HighQuality;
                //SideBarImageCanvas.DrawImage(SidebarImage, AvailableClientArea.Width, 0, PicGame.ClientRectangle.Width - AvailableClientArea.Width, PicGame.Height);
            ImageAttributes sidebardrawattr = new ImageAttributes();
            if (mGameState != null)
            {

                ColorMatrix getmatrix = mGameState.PlayingLevel.SidebarColorMatrixValues;
                sidebardrawattr.SetColorMatrix(getmatrix);


            }
           

            //SideBarImageCanvas.DrawImage(SidebarImage, 0, 0, SideBarImageBitmap.Width, SideBarImageBitmap.Height);
            SideBarImageCanvas.DrawImage(SidebarImage, new Rectangle(0, 0, SideBarImageBitmap.Width, SideBarImageBitmap.Height),0,0,
                SidebarImage.Width,SidebarImage.Height,GraphicsUnit.Pixel,sidebardrawattr);

            


            //and here is where we "colourize" the used background. This could be a level setting. Than again, so could the sidebar image
            //itself, come to think of it.

            //SideBarImageCanvas.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Blue)), 0, 0, SideBarImageBitmap.Width, SideBarImageBitmap.Height);
                //backBufferCanvas.CompositingQuality = CompositingQuality.HighSpeed;
                //12,212

          //  }


            //}

            if (mGameState == null) return; //TODO: make it show something different when there is no levelset loaded.


            Rectangle titlerect = new Rectangle(13, 12, 197 - 13, 57 - 12);


            Rectangle scorelivesrect = new Rectangle(13,80,197-13,243-80);

            Rectangle extrainforect = new Rectangle(13,390,197-13,415-390);
            Color usetextcolor = mPlayingSet.Levels[mPlayingLevel - 1].SidebarTextColor;


            //titlerect.Offset(AvailableClientArea.Width, 0);
            //scorelivesrect.Offset(AvailableClientArea.Width, 0);
            //extrainforect.Offset(AvailableClientArea.Width, 0);
            //TODO: use a INI file setting for the font.
            Font usetitlefont = BCBlockGameState.GetScaledFont(new Font("Arabia", 16),28);
            //try
            //{
            if (mPlayingSet != null)
            {
                if (mPlayingLevel - 1 < mPlayingSet.Levels.Count)
                    SideBarImageCanvas.DrawString(
                        mPlayingSet.Levels[mPlayingLevel - 1].LevelName + "(" + mPlayingLevel.ToString() + " of " +
                        (mPlayingSet.Levels.Count).ToString() + ")",
                        usetitlefont, new SolidBrush(usetextcolor), titlerect);
            }
            //}
            //catch (Exception)
            //{
                //



            //}
            String scorelivesstring = "Score:" + mGameState.GameScore.ToString() + "\n" +
                "Lives:" + mGameState.playerLives.ToString() + "\n" +
                "Multiplier:" + mGameState.ScoreMultiplier;
            //+"Time:" + FormatLevelTime(mLevelTime) + "\n";

            SideBarImageCanvas.DrawString(scorelivesstring
                , BCBlockGameState.GetScaledFont(new Font("Arial", 16, FontStyle.Bold),28), new SolidBrush(usetextcolor), scorelivesrect);


            //draw HP:
            //draw in rect:
            //(10,215)-(197,245) of the sidebar.
            #region drawHPbar
            const int MeterX = 10;
            const int MeterY = 171;
            Rectangle HPrect = new Rectangle(MeterX,MeterY,180,32);
            float useHP=0;
            if (mGameState.PlayerPaddle != null)
            {
               useHP = mGameState.PlayerPaddle.HP;
            }
            Rectangle HPmeterrect = new Rectangle(10, MeterY, (int)((useHP * ((float)HPrect.Width / 100))),HPrect.Height);
            //HPrect.Offset(AvailableClientArea.Width, 0);
            //HPmeterrect.Offset(AvailableClientArea.Width, 0);
            //now, create the brushes we will use:
            if (useHP > 0)
            {
                Brush meterbackground = new LinearGradientBrush(HPrect, Color.FromArgb(128, Color.Gray),
                                                                Color.FromArgb(15, Color.White),
                                                                LinearGradientMode.Horizontal);
                Brush meterforeground = new LinearGradientBrush(HPmeterrect, Color.Blue, Color.Green,
                                                                LinearGradientMode.Horizontal);
                String MeterString = useHP.ToString();

                DrawMeter(SideBarImageCanvas, HPrect, HPmeterrect, meterbackground, meterforeground, "HP:" + MeterString);
            }

            #endregion 

            //draw energy Bar. Draw in rect:
            //(10,215)-(197,245)
            #region DrawEnergyBar 
            float useEnergy = 0;
            if (mGameState.PlayerPaddle != null)
            {
                useEnergy = mGameState.PlayerPaddle.Energy;
                if (useEnergy > 0)
                {
                    Rectangle Energyrect = new Rectangle(10, 210, 180, 32);
                    Rectangle Energymeterrect = new Rectangle(10, 210, (int)((useEnergy * ((float)Energyrect.Width / 100))), Energyrect.Height);
                    //Energyrect.Offset(AvailableClientArea.Width, 0);
                    //Energymeterrect.Offset(AvailableClientArea.Width, 0);
                    Brush EnergyMeterBackground = new LinearGradientBrush(Energyrect, Color.FromArgb(128, Color.Gray),
                                                                    Color.FromArgb(15, Color.White),
                                                                    LinearGradientMode.Horizontal);
                    Brush EnergyMeterForeground = new LinearGradientBrush(Energymeterrect, Color.Red, Color.Yellow,
                                                                    LinearGradientMode.Horizontal);


                    // DrawMeter(g, HPrect, HPmeterrect, EnergyMeterBackground, EnergyMeterForeground, "Energy:" + useEnergy.ToString());
                    DrawMeter(SideBarImageCanvas, Energyrect, Energymeterrect, EnergyMeterBackground, EnergyMeterForeground, "Energy:" + useEnergy.ToString());
                }
            }



            #endregion


            //start drawing the stars at position 12,212, in increments of 33 pixels.)
            if (mGameState != null)
             //   Debug.Print("Numcompletions=" + mGameState.NumCompletions.ToString());
            {

                if ((mGameState.NumCompletions < 4))
                {

                    for (int i = 0; i < mGameState.NumCompletions; i++)
                    {
                        //BackgroundBuffer.DrawImage(BCBlockGameState.Imageman.getLoadedImage("STAR"),
                        //    AvailableClientArea.Right + 12 + (33 * i), AvailableClientArea.Top + 212, 32, 32);
                        //Debug.Print("Drawing star at " + AvailableClientArea.Width.ToString() + "+ 12 + (33 *" + i + ")");
                        /*
                           BackgroundBuffer.DrawLine(new Pen(Color.Black),0, 0, AvailableClientArea.Width + 12 + (33 * i), 212);
                           BackgroundBuffer.DrawImage(BCBlockGameState.Imageman.getLoadedImage("STAR"),
                                                      AvailableClientArea.Width + 12 + (33 * i), 212, 32, 32); 
                           */
                        //g.DrawLine(new Pen(Color.Black), 0, 0, AvailableClientArea.Width + 12 + (33 * i), 212);
                        SideBarImageCanvas.DrawImage(BCBlockGameState.Imageman.getLoadedImage("STAR"),
                                    AvailableClientArea.Width + 12 + (33 * i), 212, 32, 32);
                    }
                }
                else
                {
                    //if more than 4 completions, show <star> * <number>
                    //draw the star...
                    SideBarImageCanvas.DrawImage(BCBlockGameState.Imageman.getLoadedImage("STAR"), AvailableClientArea.Width + 12, 212, 32, 32);

                    //SizeF sizeuse = mGameState.CompletionsFontsize;
                    GraphicsPath usegp = new GraphicsPath();
                    Font usefont = mGameState.CompletionsFont;
                    PointF cptextorigin = new PointF(AvailableClientArea.Width+12+35,212);
                    usegp.AddString(mGameState.NumCompletionsText,usefont.FontFamily,(int)usefont.Style,usefont.Size,cptextorigin,StringFormat.GenericTypographic);

                    SideBarImageCanvas.FillPath(new SolidBrush(usetextcolor), usegp);







                }

                //39,335 is where we draw the MacGuffin Count.
                Image MacGuffinPic = BCBlockGameState.GetSphereImage(Color.Blue);

                //paint it at 39,335; 16x16:
                SideBarImageCanvas.DrawImage(MacGuffinPic, 39, 340, 16, 16);
                //paint x <Count>

                const int normalsize = 32;
                const int expandedsize = 48;
                TimeSpan Expandtime = mGameState.MacGuffinExpandTime;
                //choose a size between normalsize and expandedsize based on the last macguffin time.
                int usesize;
                TimeSpan sincelast = DateTime.Now - mGameState.LastMacGuffinTime;
                if (sincelast.TotalMilliseconds > Expandtime.TotalMilliseconds)
                    usesize = normalsize;
                else
                {
                    usesize = expandedsize - (int)((sincelast.TotalMilliseconds / Expandtime.TotalMilliseconds) * (expandedsize - normalsize));

                }
                Point uselocation = new Point(60, 362 - usesize);
                Font macguffinfont= BCBlockGameState.GetScaledFont(new Font("Consolas", DPIHelper.ScaleFontSize(16,SideBarImageCanvas)), usesize);
                SideBarImageCanvas.DrawString("x " + mGameState.MacGuffins.ToString(), macguffinfont, new SolidBrush(usetextcolor), uselocation.X, uselocation.Y);


               // PicGame.Invalidate();
              //  sidebarbgdrawn = true;
            }

            //extra code to draw active power ups. This will be done by seeing first if there is a valid paddle (not null)
            //and then by grabbing the images for all it's behaviours- again, ignoring null values for getIcon().

            //starting at 9,250...
            if (mGameState.PlayerPaddle != null)
            {
                //LINQ ftw again...
                var geticons  =(from qq in mGameState.PlayerPaddle.Behaviours where qq.GetIcon()!=null select qq.GetIcon());
                int initialX = 32;
                int pbX = initialX, pbY = 260;
                Size behdrawsize = new Size(32, 16);

                foreach (Image loopimage in geticons)
                {
                    SideBarImageCanvas.DrawImage(loopimage, new Rectangle(new Point(pbX, pbY), behdrawsize));
                    pbX += behdrawsize.Width + 2;
                    if (pbX > initialX+160)
                    {
                        pbX = initialX;
                        pbY += behdrawsize.Height + 2;
                    }

                }

                //tada....
                



            }

            SideBarImageCanvas.DrawString("Copyright 2009-2011 BASeCamp Corporation. Ver:" + getappversion(),
                new Font(BCBlockGameState.GetMonospaceFont(), 8), new SolidBrush(usetextcolor), extrainforect);


        }

        private void DrawMeter(Graphics g, Rectangle HPrect, Rectangle HPmeterrect, Brush meterbackground, Brush meterforeground, string MeterString)
        {
            g.FillRectangle(meterbackground, HPrect);
            g.FillRectangle(meterforeground, HPmeterrect);
            StringFormat newformat = new StringFormat(StringFormatFlags.NoWrap);
            newformat.Alignment = StringAlignment.Center;

            g.DrawString(MeterString, BCBlockGameState.GetScaledFont(new Font(BCBlockGameState.GetMonospaceFont(), 14, FontStyle.Bold),32), new SolidBrush(Color.Black),
                         HPrect, newformat);
        }

        private string getappversion()
        {

            return Application.ProductVersion;


        }

        private void DrawBackgroundFrame()
        {

            //used to draw a changing background.
            
            if (mGameState.BackgroundDraw.RequiresPerformFrame()) mGameState.BackgroundDraw.PerformFrame(mGameState);
            BackgroundBuffer.SetClip(AvailableClientArea);
            mGameState.BackgroundDraw.DrawBackground(mGameState, BackgroundBuffer,PicGame.ClientRectangle, false);
            

        }
        private Font usefont = new Font(BCBlockGameState.GetMonospaceFont(),24);
        private CharacterAnimator getLinearCharAnimator(PointF position, String fromstring, int index)
        {
            Graphics g = Graphics.FromImage(new Bitmap(1,1));
            String lensubstr = fromstring.Substring(0, index);
            SizeF stringsize = g.MeasureString(lensubstr, usefont);

            PointF useendpos = new PointF(position.X+stringsize.Width,position.Y);
            PointF usestartpos = new PointF(useendpos.X,useendpos.Y+50);

            return new LinearCharacterAnimator(fromstring[index],usestartpos,useendpos,new TimeSpan(0,0,5));


        }
        public Region BlockRegion = null;
        public LevelSet ActualPlaySet;
        public void setPaintHandler()
        {
            PicGame.Paint += PicGame_PaintNonDWM;
            /*
            if (BCBlockGameState.hasDWM())
            {
                PicGame.Paint -= PicGame_PaintNonDWM;
                PicGame.Paint -= PicGame_PaintDWM;
                PicGame.Paint += PicGame_PaintDWM;
            }
            else
            {
                PicGame.Paint -= PicGame_PaintDWM;
                PicGame.Paint -= PicGame_PaintDWM;
                PicGame.Paint += PicGame_PaintNonDWM;

            }
            */

        }
        private Bitmap nonDWMBuffer = null;
        private Graphics nonDWMBufferg = null;
        private void PicGame_PaintNonDWM(object sender, PaintEventArgs e)
        {
            //NON-DWM painting- create a bitmap, call he DWM paint and paint on it, and blit that to the object.
            if (nonDWMBuffer == null)
            {
                nonDWMBuffer = new Bitmap((int)PicGame.ClientSize.Width, (int)PicGame.ClientSize.Height);
                nonDWMBufferg = Graphics.FromImage(nonDWMBuffer);
            }
            nonDWMBufferg.Clear(Color.Transparent);
            //PaintEventArgs passargs = new PaintEventArgs(nonDWMBufferg, e.ClipRectangle);
            PaintEventArgs passargs = new PaintEventArgs(nonDWMBufferg,PicGame.ClientRectangle);
            //delegate to the "DWM" paint routine...
            PicGame_PaintDWM(sender, passargs);
            //now we blit nonDWMBuffer to e.Graphics...
            var PrevMode = e.Graphics.CompositingMode;
            var PrevQuality = e.Graphics.CompositingQuality;
            e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            e.Graphics.CompositingQuality = CompositingQuality.HighSpeed;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            e.Graphics.DrawImage(nonDWMBuffer, 0, 0,PicGame.ClientSize.Width,PicGame.ClientSize.Height);
            e.Graphics.CompositingMode = PrevMode;
            e.Graphics.CompositingQuality = PrevQuality;


        }
        Rectangle StandardSize = new Rectangle(0, 0, 940, 526);
        private static Font LoadingFont = BCBlockGameState.GetScaledFont(new Font("Arial", 72), 80);
        private void PicGame_PaintDWM(object sender, PaintEventArgs e)
        {

            //if (mGameState!=null && mGameState.lastframeimage == null) mGameState.lastframeimage = new Bitmap(PicGame.ClientSize.Width, PicGame.ClientSize.Height, e.Graphics);
            String LoadingText="Please Wait...";
            e.Graphics.PageUnit = GraphicsUnit.Pixel;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighSpeed;
            
            if (gamerunstate is StateLoading)
            {
                Rectangle usearea = StandardSize;
                e.Graphics.SmoothingMode = SmoothingMode.HighQuality;
                Image useimage = BCBlockGameState.Imageman.getLoadedImage("LOADINGBG");
                /*
                LinearGradientBrush lgb = new LinearGradientBrush(usearea, Color.Green, Color.Blue, LinearGradientMode.Horizontal);
                e.Graphics.FillRectangle(lgb, usearea);
                 * */
                e.Graphics.DrawImage(useimage, 0, 0,StandardSize.Width,StandardSize.Height);
                Size gotsize = TextRenderer.MeasureText(LoadingText, LoadingFont);

                Point drawhere = new Point(usearea.Width / 2 - gotsize.Width / 2, usearea.Height / 2 - gotsize.Height / 2);
                GraphicsPath gp = new GraphicsPath();
                gp.AddString(LoadingText, LoadingFont, drawhere, StringFormat.GenericDefault);
                e.Graphics.FillPath(new SolidBrush(Color.White), gp);
                e.Graphics.DrawPath(new Pen(Color.Black),gp);


                return;
            }


            if(mGameState!=null)
                e.Graphics.TranslateTransform(mGameState.Translation.X, mGameState.Translation.Y);
            bool blocksdrawn=false;
            DateTime LastPaint=DateTime.Now;
            //clear...
            bool foundanimated = false;

            try
            {
                //Graphics g = e.Graphics;
                Graphics g = backBufferCanvas;
                if (Dorefreshstats)
                {

                    //Dorefreshstats is set when the score, lives, or level is changed.
                    Dorefreshstats = false; //set to false- the point of this is to prevent having to draw the status info every single frame!
                    g.Clear(Color.White);
                    
                    //clear the entire thing.
                    //redraw the status:
                    PaintSideBarstats();
                    //draw it to the backbuffer. clear the clip first, so we can draw outside any defined clip:
                    g.SetClip(new Rectangle(Point.Empty,BackBufferBitmap.Size)); //use clip that is the entire size.
                    //draw the image of the  sidebar to the backbuffer (g is the backbuffer)
                    g.DrawImageUnscaled(SideBarImageBitmap, AvailableClientArea.Width, 0);
                    //re set the clip,
                    g.SetClip(AvailableClientArea);
                }
                else
                {
                    g.SetClip(AvailableClientArea);
                    g.Clear(Color.Transparent);
                }


                //g.SmoothingMode = SmoothingMode.HighSpeed;
                //Graphics g = backBufferCanvas;
                //g.Clear(Color.White);
               


                if (mGameState == null)
                    return;
               
                // lock (mGameState)
                // {

                //BackgroundBuffer.Clear(Color.Transparent);


                //foundanimated = (from Block w in mGameState.Blocks where (w.RequiresPerformFrame()) select w).Count() > 0;
                //foundanimated = mGameState.Blocks.Any((w) => w.RequiresPerformFrame());
                if(gamerunstate is StateMenu)
                {
                    DrawShade(g, Color.FromArgb(75, Color.Blue));
                    menudata.Draw(g, mGameState);
                    
                  
                    if (mGameState.PlayerPaddle != null)
                        mGameState.PlayerPaddle.Draw(g);





                }
                else if (gamerunstate is StatePaused)
                {
                    DrawShade(g, Color.FromArgb(75, Color.Blue));

                    PaintPause(g);
                   

                    /*
                    String measureme = "PAUSED";
                    //draw a white box near the center...
                    //RectangleF Inputboxcoordsf = new RectangleF(PicGame.ClientRectangle.Width/4,PicGame.ClientRectangle.Height/4,PicGame.ClientRectangle.Width/2,PicGame.ClientRectangle.Height/2);
                    Font pausedinputfont = new Font("Arial Black", 38);
                    SizeF InputboxSize;
                    InputboxSize = g.MeasureString(measureme, pausedinputfont);

                    RectangleF centered = BCBlockGameState.CenterRect(mGameState.GameArea, InputboxSize);
                    g.DrawString("PAUSED", pausedinputfont, new SolidBrush(Color.Yellow), centered);
                  */
                    if (mGameState.PlayerPaddle != null)
                        mGameState.PlayerPaddle.Draw(g);





                }
                #region cheat/value entry

                else if (gamerunstate is StateValueInput)
                {
                    gamerunstate.DrawFrame(mGameState,g,PicGame.ClientSize);
                    
                }
                #endregion
                else
                {
                    if (BlockRegion != null) BlockRegion.Dispose();
                    BlockRegion = new Region();

                    //Debug.Print("redrawblocks=" + mredrawblocks.ToString());
                    blocksdrawn = mredrawblocks;

                    #region draw static blocks
                    if (mredrawblocks)
                    {
                        BlockDrawBuffer.Clear(Color.Transparent);
                        if (staticblocks != null)
                        {
                            foreach (Block drawblock in staticblocks)
                            {
                                BlockRegion.Intersect(drawblock.BlockRectangle);
                                drawblock.Draw(BlockDrawBuffer);


                            }



                        }
                        else
                        {
                            Debug.Print("staticblocks was null");
                        }



                    }

                    #endregion
                    //now the animated blocks.



                    /*
                    if (mredrawblocks)
                    {
                        BlockDrawBuffer.Clear(Color.Transparent);
                        if (mGameState.Blocks != null)
                        {
                            foreach (Block drawblock in mGameState.Blocks)
                            {
                                if (!(drawblock.RequiresPerformFrame()))
                                    drawblock.Draw(BlockDrawBuffer);
                                else
                                    foundanimated = true;



                            }



                        }
                    }*/

                    if (mRedrawAnimated)
                    {
                        MovingBlockBuffer.Clear(Color.Transparent);
                        if (AnimatedBlocks != null)
                        {
                            foreach (Block drawblock in AnimatedBlocks)
                            {
                                BlockRegion.Intersect(drawblock.BlockRectangle);
                                drawblock.Draw(MovingBlockBuffer);
                                
                            }
                        }
                        else
                        {
                            Debug.Print("AnimatedBlocks=null for some reason.");
                        }


                    }
                    if (foundanimated || mRedrawAnimated)
                    {
                        var queryblocks = from Block w in mGameState.Blocks where (w.RequiresPerformFrame()) select w;
                        MovingBlockBuffer.Clear(Color.Transparent);
                        Block lastblock = null;
                    reloopit:
                        try
                        {

                            foreach (Block drawblock in queryblocks)
                            {
                                lastblock = drawblock;
                                drawblock.Draw(MovingBlockBuffer);



                            }
                        }
                        catch (Exception pz)
                        {
                            Trace.WriteLine(pz.StackTrace);
                            //messy... but it works.
                            goto reloopit;


                        }
                    }

                    g.DrawImageUnscaled(backgroundbitmap, 0, 0);
                    //PaintSideBarstats(e.Graphics);

                    g.SetClip(AvailableClientArea);

                    DrawBackgroundFrame();
                    g.DrawImageUnscaled(BlockDrawBitmap, 0, 0);
                    //Debug.Print("foundanimated=" + foundanimated.ToString());
                    if (foundanimated || mRedrawAnimated)
                    {
                        g.DrawImageUnscaled(MovingBlockBitmap, 0, 0);

                    }
                    if (mGameState.Balls != null)
                    {
                        mGameState.Balls.Remove((cBall)null);
                       


                    }


                    lock (mGameState.Particles)
                    {
                        foreach (Particle loopparticle in mGameState.Particles)
                        {
                            if(loopparticle!=null)
                                loopparticle.Draw(g);


                        }
                    }

                    foreach (GameObject loopgameobject in mGameState.GameObjects)
                    {
                        if(loopgameobject!=null)
                        loopgameobject.Draw(g);




                    }
                    foreach (cBall drawball in mGameState.Balls)
                    {

                        if (drawball != null)
                            drawball.Draw(g);




                    }

                    if (mGameState.PlayerPaddle != null)
                        mGameState.PlayerPaddle.Draw(g);
                    if (DemoMode)
                    {
                        #region gameoverdraw
                        if (gamerunstate is StateLevelOutroGameOver)
                        {
                            //Font GameOverFont = new Font("Arial", 28);
                            Font GameOverFont = BCBlockGameState.GetScaledFont(new Font("Arial", 28), 72);
                            SizeF valueSize = g.MeasureString(tallydata.TallyScreenString, GameOverFont);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Gray)), AvailableClientArea);
                            RectangleF middlerect = GetCenteredRect(valueSize, AvailableClientArea);
                            /*if (ActualPlaySet != null)
                            {

                                middlerect = new RectangleF(middlerect.Left, AvailableClientArea.Height - middlerect.Height, AvailableClientArea.Width, AvailableClientArea.Height);

                            }
                            */
                            RectangleF expandedrect = middlerect;
                            expandedrect.Inflate(10, 10);
                            var sf = new StringFormat();
                            sf.Alignment = StringAlignment.Near;
                            /*
                            Color FirstColor = Color.FromArgb(200, Color.White);
                            Color SecondColor = Color.FromArgb(125, Color.Green);
                            Brush fillbrush = new LinearGradientBrush(middlerect, FirstColor, SecondColor, 0f);
                            g.FillRectangle(new SolidBrush(Color.FromArgb(128, Color.Black)), expandedrect);
                            g.FillRectangle(fillbrush, middlerect);

                            GraphicsPath textpath = new GraphicsPath();
                         
                            textpath.AddString(tallydata.TallyScreenString, GameOverFont.FontFamily, (int)GameOverFont.Style, GameOverFont.Size, middlerect, sf);

                            g.FillPath(new TextureBrush(BCBlockGameState.Imageman.getLoadedImage("Invincible"), WrapMode.Tile), textpath);
                            g.DrawPath(new Pen(Color.Black), textpath);
                            // g.DrawString(tallydata.TallyScreenString, GameOverFont, new SolidBrush(Color.Black), middlerect);
                            */
                            

                            //draw the top high scores and their names, as well.
                            //the relevant set was cached as "ActualPlaySet"...
                            if (ActualPlaySet != null)
                            {
                                //start from the top.
                                //draw GameOverHiScores
                                //HighScoreNamesMaxLength is maximum length of name
                                //HighScoreScoresMaxLength is maximum length of score.
                                //HighScoremaxSeparators is max separators to use. (periods, usually)

                                //Font hiscorefont = new Font("Courier New", 18);
                                Font hiscorefont = BCBlockGameState.GetScaledFont(new Font("Courier New", 18), 28);
                                using (GraphicsPath usepath = new GraphicsPath())
                                {
                                    SizeF measuredsize = g.MeasureString(GameOverHiScores, hiscorefont);
                                    PointF topscoresorigin = new PointF(AvailableClientArea.Width / 2 - (measuredsize.Width / 2), 16);
                                    Rectangle scoresbox = new Rectangle(new Point((int)topscoresorigin.X, (int)topscoresorigin.Y), new Size((int)measuredsize.Width, (int)measuredsize.Height));
                                    //draw a box to "surround" the top scores.
                                    g.FillRectangle(new SolidBrush(Color.FromArgb(210, Color.YellowGreen)), scoresbox);
                                    g.DrawRectangle(new Pen(Color.Black, 3), scoresbox);

                                    GraphicsPath shadowed = (GraphicsPath)usepath.Clone();
                                    usepath.AddString(GameOverHiScores, hiscorefont.FontFamily, (int)hiscorefont.Style, hiscorefont.Size, topscoresorigin, sf);
                                    shadowed.AddString(GameOverHiScores, hiscorefont.FontFamily, (int)hiscorefont.Style, hiscorefont.Size, new PointF(topscoresorigin.X+3,topscoresorigin.Y+3), sf);
                                    SmoothingMode cachedmode = g.SmoothingMode;
                                    g.SmoothingMode = SmoothingMode.HighQuality;
                                    g.FillPath(new SolidBrush(Color.FromArgb(128, Color.Black)), shadowed);
                                    
                                    g.DrawPath(new Pen(Color.Black), usepath);
                                    g.FillPath(new SolidBrush(Color.Black), usepath);
                                    
                                    
                                    
                                    g.SmoothingMode = cachedmode;
                                }
                                //g.DrawPath(new Pen(Color.Black), usepath);

                            }



                        }
                        #endregion
                        else if ((DateTime.Now.Second % 2) == 0)
                        {
                            Font DemoModeFont = BCBlockGameState.GetScaledFont(new Font("Arial", 18),24);
                            SizeF valuesize = g.MeasureString("DEMO MODE", DemoModeFont);
                            g.DrawString("DEMO MODE", DemoModeFont, new SolidBrush(Color.Red), AvailableClientArea.Right - valuesize.Width, AvailableClientArea.Bottom - valuesize.Height);


                        }


                    }
                    //draw some info in the lower right corner.
                }
                String InfoString = "";
                TimeSpan timediff = DateTime.Now - LastPaint;
                if (timediff.Milliseconds > 0)
                {
                    CurrentFPS = (1000 / timediff.Milliseconds);
                    InfoString = "FPS:" + CurrentFPS.ToString() + "\n";

                }



                if (ShowDebugInfo)
                {
                    InfoString += "Blocks Drawn:" + (blocksdrawn ? mGameState.Blocks.Count() : 0).ToString() + "\n" +
                                  "Balls:" + mGameState.Balls.Count.ToString() + "\n" +
                                  "Particles:" + mGameState.Particles.Count.ToString() + "\n" +
                                  "GameObjects:" + mGameState.GameObjects.Count.ToString() + "\n" +
                                  "SpeedMult:" + mGameState.SpeedMultiplier.ToString() + "\n" + 
                                  "Lives:" + mGameState.playerLives + "\n" +
                                  "Score:" + mGameState.GameScore + "\n" +
                                  "Level:" + mPlayingLevel + "\n" +
                                  "Elapsed:" + mLevelTime.ToString() + "\n" +
                                  "Mem Alloc:" + System.GC.GetTotalMemory(false);
                  

                }
                if (!String.IsNullOrEmpty(InfoString))
                {
                    Font InfoTextFont = BCBlockGameState.GetScaledFont(new Font(BCBlockGameState.GetMonospaceFont(), DPIHelper.ScaleFontSize(10,e.Graphics)),15,e.Graphics);
                    SizeF stattextsize = g.MeasureString(InfoString, InfoTextFont);

                    g.DrawString(InfoString, InfoTextFont, new SolidBrush(Color.FromArgb(240, Color.Black)), 0,
                                 PicGame.Height - stattextsize.Height);
                }
                if (gamerunstate is StateLevelIntro)
                {
                    Level playinglevel = mPlayingSet.Levels[mPlayingLevel - 1];
                    //also draw the levels name...
                    g.DrawImageUnscaled(backgroundbitmap, 0, 0);
                    g.Clip = new Region();
                   // PaintSideBarstats(e.Graphics);
                    g.SetClip(AvailableClientArea);
                    DrawBackgroundFrame();

                    g.DrawImageUnscaled(BlockDrawBitmap, 0, 0);

                    String drawlevelname = playinglevel.LevelName;
                    Color mFillColor = playinglevel.LevelNameIntroTextFillColor;
                    Color mStrokeColor = playinglevel.LevelNameIntroTextPenColor;
                    Font drawlevelfont = playinglevel.LevelnameIntroFont;



                    


                    GraphicsPath usepath = new GraphicsPath();
                    Rectangle drawintrorect = BCBlockGameState.CenterRect(AvailableClientArea, IntroSequenceBitmap.Size);

                    //Debug.Print("Drawing intro rect:" + drawintrorect.ToString() + " within available area:" + AvailableClientArea.ToString() + " bitmap size is:" + IntroSequenceBitmap.Size.ToString());

                    g.DrawImageUnscaled(IntroSequenceBitmap, drawintrorect.Left, drawintrorect.Top);
                    TimeSpan Timeremaining = playinglevel.ShowNameLength - (DateTime.Now - mLevelIntroStartTime);
                    var oldq = g.CompositingQuality;
                    var olds = g.SmoothingMode;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.SmoothingMode = SmoothingMode.HighQuality;

                    g.DrawStringCenter("Ready in " + Timeremaining.Seconds, 
                        BCBlockGameState.GetScaledFont(new Font("Arial", 24),45,e.Graphics),
                        new SolidBrush(Color.Blue), new Pen(Color.YellowGreen), AvailableClientArea, new Point(0, 48));
                    g.CompositingQuality = oldq;
                    g.SmoothingMode = olds;
                    //g.DrawString(Timeremaining.Seconds,new Font("Arial",24),new SolidBrush
                    //  IntroAnimator.Draw(g);

                    // usepath.AddString(drawlevelname,drawlevelfont.FontFamily,(int)drawlevelfont.Style,drawlevelfont.Size,GetCenteredRect(g.MeasureString(drawlevelname, drawlevelfont),AvailableClientArea),StringFormat.GenericDefault);


                    //g.FillPath(new SolidBrush(mFillColor), usepath);
                    //g.DrawPath(new Pen(mStrokeColor, 1),usepath);

                    //g.DrawString(drawlevelname, drawlevelfont, new SolidBrush(mDrawColor), , AvailableClientArea));


                }
                else if (gamerunstate is StateLevelOutro)
                {
                    //again, might be preferable to use a selectable font?
                    System.Drawing.Font tallyfont = new Font(BCBlockGameState.GetMonospaceFont(), DPIHelper.ScaleFontSize(32,g));
                    RectangleF centered = GetCenteredRect(g.MeasureString(tallydata.TallyScreenString, tallyfont), AvailableClientArea);
                    RectangleF boxdraw = centered;
                    boxdraw.Inflate(5, 5);
                    Color ColorA = Color.FromArgb(200, Color.White), ColorB = Color.FromArgb(128, Color.Gray);
                    //TODO: change This section so that it displays an Alpha Image instead of a boring white box.
                    //g.FillRectangle(new System.Drawing.Drawing2D.LinearGradientBrush(boxdraw,ColorA, ColorB, (float)(Math.PI * 2 * BCBlockGameState.rgen.NextDouble())), boxdraw);

                    //g.DrawRectangle(new Pen(Color.Black, 3), boxdraw.Left,boxdraw.Top,boxdraw.Width,boxdraw.Height);
                    g.DrawImage(tallydata.TallyImage, boxdraw.Left, boxdraw.Top, boxdraw.Width, boxdraw.Height);


                    g.DrawString(tallydata.TallyScreenString, tallyfont, new SolidBrush(Color.Black), centered);


                }
                mGameState.PaintMessages(g,AvailableClientArea);
                //if(mGameState!=null) mGameState.lastframeimage = new Bitmap(PicGame.ClientSize.Width, PicGame.ClientSize.Height, e.Graphics);
                //pcgraph = PicGame.CreateGraphics();


            }
            catch (Exception q)
            {
                Trace.WriteLine("Exception: Type:" + q.GetType().ToString() + " trace:" + q.StackTrace + " message:" + q.Message);

            }
            finally
            {
               //here is where we have any image post-processing.
                PostProcess(ref BackBufferBitmap,ref backBufferCanvas);
                lock (backBufferCanvas)
                {
                   // PaintSideBar(e.Graphics);
                    //if (SideBarImageBitmap!= null)
                    //{
                      

                    //}

                    e.Graphics.DrawImageUnscaled(BackBufferBitmap, 0, 0);
                    LastPaint = DateTime.Now;
                    

                    if (dosaveshot)
                    {
                        dosaveshot = false;
                        
                        SaveScreenshot(BackBufferBitmap);




                    }

                }


            }

        }

        private ImageAttributes ProcessColourizer=null;

        Image _CachedPause = null;
        Level _Cachedforlevel = null;
        private void PaintPause(Graphics g)
        {


                String usepausekey = "paused";

                if (mPlayingLevelobj != null) usepausekey = mPlayingLevelobj.PauseImageKey;


            //if we need to, redraw the CachedPause object. we do this because
            //it could involve ImageAttributes which we can ideally avoid drawing every frame.
                if (_CachedPause == null || _Cachedforlevel != mPlayingLevelobj)
                {
                    Image createimage = BCBlockGameState.Imageman.getLoadedImage(usepausekey);

                    //create the bitmap to cache. Scale it now (this is the "header" image)
                    _CachedPause = new Bitmap((int)(createimage.Width * mPlayingLevelobj.PauseImageScale.Width)  , 

                        (int)( createimage.Height * mPlayingLevelobj.PauseImageScale.Height));
                    Graphics Colorized = Graphics.FromImage(_CachedPause);
                    ImageAttributes pauseattributes = new ImageAttributes();
                    pauseattributes.SetColorMatrix(mPlayingLevelobj.PauseColorMatrixValues);
                    Colorized.DrawImage(createimage, 
                        new Rectangle(0, 0, _CachedPause.Width, _CachedPause.Height), 
                        0, 0, createimage.Width, createimage.Height, GraphicsUnit.Pixel, pauseattributes);


                    Colorized.Dispose(); //finished with the graphics object.
                }

            //_Cachedpause: we want to use that, add on enough height for the pause text we generate, and paint the new image.

                String pausedatastring = "Score:" + mGameState.GameScore + Environment.NewLine +
                                         "Time:" + String.Format("{0:00}:{1:00}.{2:00}", Math.Floor(mLevelTime.TotalMinutes), mLevelTime.Seconds, mLevelTime.Milliseconds);


                SizeF measured = BCBlockGameState.MeasureString(pausedatastring,mPlayingLevelobj.PauseFont);


                Image drawpausedimage = new Bitmap((int)(Math.Max(_CachedPause.Width, measured.Width + 30)), (int)(_CachedPause.Height + measured.Height + 20));
                Graphics DrawPause = Graphics.FromImage(drawpausedimage);
                DrawPause.Clear(Color.Transparent);
                //draw the image. Center it if the width is larger.
            PointF drawimageorigin = PointF.Empty;
                if (drawpausedimage.Width > _CachedPause.Width)
                {

                    drawimageorigin = new PointF(drawpausedimage.Width / 2 - (_CachedPause.Width / 2), 0);




                }
            //draw the image at the proper coordinates...

            DrawPause.DrawImage(_CachedPause,drawimageorigin);

            




            //we'll want to draw the text at offset 15,imageheight+10
            //create the graphics path to use for that...
            GraphicsPath TextDraw = new GraphicsPath();
            TextDraw.AddString(pausedatastring, mPlayingLevelobj.PauseFont, new Point(15, _CachedPause.Height+ 10), StringFormat.GenericDefault);

            //paint in drawPause context...
            DrawPause.FillPath(new SolidBrush(mPlayingLevelobj.PauseTextColor), TextDraw);
            
            //dispose the objects used.
            TextDraw.Dispose();
            DrawPause.Dispose();





                Size useimagesize = new Size((int)(drawpausedimage.Width), 
                    (int)(drawpausedimage.Height));
                 RectangleF centered = BCBlockGameState.CenterRect(mGameState.GameArea, useimagesize);
                 CompositingQuality prevquality = g.CompositingQuality;
                 g.CompositingQuality = CompositingQuality.AssumeLinear;
                 g.DrawImage(drawpausedimage, centered);
                 g.CompositingQuality = prevquality;




        }


        int postprocessframe = 0;

        private void PostProcess(ref Bitmap processit,ref Graphics useg)
        {
            if(mGameState==null) return;
            postprocessframe++;
            if (postprocessframe > 50000) postprocessframe = 0;
            if(mGameState.PlayerPaddle!=null)
            {
                if (mGameState.PlayerPaddle.HP < Paddle.dangerzone)
                {
                    //removed....
                    /*
                    if (ProcessColourizer == null) ProcessColourizer = new ImageAttributes();
                    ProcessColourizer.SetColorMatrix(ColorMatrices.GetColourizer((float)Math.Sin(1.0f + (double)(postprocessframe/5)), 1.0f, 1.0f, 1.0f));
                    useg.DrawImage(processit, new Rectangle(0, 0, processit.Width, processit.Height), 0, 0, processit.Width, processit.Height, GraphicsUnit.Pixel,
                    ProcessColourizer);
                    */
                }



            }
                if(gamerunstate is StateDeath)
            {
                //grayscale...
                if (ProcessColourizer == null)
                {
                    ProcessColourizer = new ImageAttributes();
                    ProcessColourizer.SetColorMatrix(ColorMatrices.GrayScale());
                }   
                useg.DrawImage(processit, new Rectangle(0, 0, processit.Width, processit.Height), 0, 0, processit.Width, processit.Height, GraphicsUnit.Pixel,
                    ProcessColourizer);
                

            }
            



        }

       // private Color UseTitleColour = Color.Blue;
        private void SaveScreenshot(Image shotsave)
        {
            //save location:
            //%APPDATA$\BASeBlock\Screenshots\
            String Screenshotfolder = Path.Combine(BCBlockGameState.AppDataFolder, "Screenshots");
            if (!Directory.Exists(Screenshotfolder))
            {
                Directory.CreateDirectory(Screenshotfolder);


            }
            //build the path name.
           // String screenshotname = String.Format("{0:MM-dd-mmzzz}.png", DateTime.Now);
            String screenshotname = DateTime.Now.Year.ToString() + "-" +
                DateTime.Now.Month.ToString() + "-" +
                DateTime.Now.Day.ToString() + " " +
                DateTime.Now.Hour.ToString() + "-" +
                DateTime.Now.Minute.ToString() + "-" +
                DateTime.Now.Second.ToString() + "-" +
                DateTime.Now.Millisecond.ToString() + ".png";
            String shotfile = Path.Combine(Screenshotfolder, screenshotname);
            shotsave.Save(shotfile);
            mGameState.EnqueueMessage("Saved screenshot to \"" + Path.GetFileName(shotfile) + "\"");

        }

        public void DrawShade(Graphics g,Color usecolor)
        {
            g.DrawImageUnscaled(backgroundbitmap, 0, 0);
           
            g.SetClip(AvailableClientArea);
            DrawBackgroundFrame();
            g.DrawImageUnscaled(BlockDrawBitmap, 0, 0);

            g.DrawImageUnscaled(MovingBlockBitmap, 0, 0);
            g.FillRectangle(new SolidBrush(usecolor), PicGame.ClientRectangle);
        }

        private RectangleF GetCenteredRect(SizeF sizeF, Rectangle rectangle)
        {
            

            return new RectangleF(new PointF((rectangle.Width/2)-sizeF.Width/2,(rectangle.Height/2)-sizeF.Height/2),sizeF);


        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            
        }

        private void PicGame_Click(object sender, EventArgs e)
        {
            

            


        }
        private void frmPoing_Activated(object sender, EventArgs e)
        {
            gamethreadsuspended = false;
            UnpauseGame();
            
            Debug.Print("Form Activated: gamerunstate set to " + gamerunstate.GetType().Name);
        }
        private void frmPoing_Deactivate(object sender, EventArgs e)
        {
            PauseGame();
            Debug.Print("Form Deactivated: gamerunstate set to Paused.");
            gamethreadsuspended = true;
            

        }
        private DateTime PausedStartTime; //DateTime when the game was paused...
        private TimeSpan? TotalPauseTime;
        private void PauseGame_old()
        {
            if (mGameState == null) return;
            if (!(gamerunstate is StateRunning)) return;
            //remove any present PauseBehaviours.
            //if paused behaviours are present, that means that the balls actual speed is "cached" in the paused ballbehaviour,
            //so we force it to revert...
            Debug.Print("Pausing....");
            //first, if there is a stickypaddlebehaviour present in the playerpaddle, release all the balls.
            if (mGameState.PlayerPaddle != null)
            {
                //if(mGameState.PlayerPaddle.Behaviours.Any((w)=>w is PaddleBehaviours.StickyBehaviour
                foreach (PaddleBehaviours.StickyBehaviour releasefrom in (from n in mGameState.PlayerPaddle.Behaviours where n is PaddleBehaviours.StickyBehaviour select (PaddleBehaviours.StickyBehaviour)n))
                {

                    releasefrom.ReleaseAllBalls();


                }


            }

            List<iBallBehaviour> removelist = new List<iBallBehaviour>();

            var clonecol = mGameState.Balls.ShallowClone();
            //clone the collection, so that changes to .Balls don't cause an exception. The actual cBall objects
            //will be the same, however.
            foreach (cBall behaviourball in (from n in clonecol where n.hasBehaviour(typeof(PausedBallBehaviour)) select n))
            {
                removelist.Clear();
                foreach (PausedBallBehaviour pausedbehave in (from n in behaviourball.Behaviours where n is PausedBallBehaviour select (PausedBallBehaviour)n))
                {
                    pausedbehave.Forcerevert();
                    //pausedbehave.SetSpeed();

                }

                //behaviourball.Behaviours.RemoveAll((w) => w is PausedBallBehaviour);


            }

       
            BCBlockGameState.Soundman.PauseMusic(true);
            BCBlockGameState.Soundman.PlaySound(mPlayingLevelobj.PauseSound);
            statebeforepause=gamerunstate;
            PausedStartTime=DateTime.Now;
            gamerunstate = new StatePaused();
            //suspend the game thread...
            
        }
        private void PauseGame()
        {
            if (mGameState == null) return;
            if (!(gamerunstate is StateRunning)) return;
            //remove any present PauseBehaviours.
            //if paused behaviours are present, that means that the balls actual speed is "cached" in the paused ballbehaviour,
            //so we force it to revert...
            Debug.Print("Pausing....");
            //first, if there is a stickypaddlebehaviour present in the playerpaddle, release all the balls.
            if (mGameState.PlayerPaddle != null)
            {
                //if(mGameState.PlayerPaddle.Behaviours.Any((w)=>w is PaddleBehaviours.StickyBehaviour
                foreach (PaddleBehaviours.StickyBehaviour releasefrom in (from n in mGameState.PlayerPaddle.Behaviours where n is PaddleBehaviours.StickyBehaviour select (PaddleBehaviours.StickyBehaviour)n))
                {

                    releasefrom.ReleaseAllBalls();


                }


            }

            List<iBallBehaviour> removelist = new List<iBallBehaviour>();
            LinkedList<cBall> clonecol;
            lock (mGameState.Balls)
            {
                clonecol = mGameState.Balls.ShallowClone();
            }
            //clone the collection, so that changes to .Balls don't cause an exception. The actual cBall objects
            //will be the same, however.
            foreach (cBall behaviourball in (from n in clonecol where n.hasBehaviour(typeof(PausedBallBehaviour)) select n))
            {
                removelist.Clear();
                foreach (PausedBallBehaviour pausedbehave in (from n in behaviourball.Behaviours where n is PausedBallBehaviour select (PausedBallBehaviour)n))
                {
                    if(pausedbehave.cachedSpeed.X!=0 && pausedbehave.cachedSpeed.Y!=0)
                        pausedbehave.Forcerevert();
                    //pausedbehave.SetSpeed();

                }

                //behaviourball.Behaviours.RemoveAll((w) => w is PausedBallBehaviour);


            }
            //    if (unpauseCompletionTimer == null)
            //if the unpauseCompletionTimer is not null, then the game was paused while it was in action.
            //we need to dispose it so that when it's timer goes off it doesn't reactivate the enemies while the ball and whatnot are still frozen.
            if (unpauseCompletionTimer != null)
            {
                unpauseCompletionTimer.Dispose();
                unpauseCompletionTimer = null;
            }
            BCBlockGameState.Soundman.PauseMusic(true);
            BCBlockGameState.Soundman.PlaySound(mPlayingLevelobj.PauseSound);
            statebeforepause = gamerunstate;
            PausedStartTime = DateTime.Now;
            gamerunstate = new StatePaused();
            //suspend the game thread...

        }
        private void UnpauseGame()
        {
            //cBall loopball;
            if (mGameState == null) return;
            if(!(gamerunstate is StatePaused) ) return;

            Debug.Print("Unpausing....");

            //remove all PausedBallBehaviours from all balls, first.
            //first, make a shallow clone of mGameState.Balls.
            var copyballs = mGameState.Balls.ShallowClone();
            foreach(cBall iterate in (from r in mGameState.Balls where r.hasBehaviour(typeof(PausedBallBehaviour)) select r))
            {
                //first, revert...
                foreach (PausedBallBehaviour pbbb in (from beh in iterate.Behaviours where beh is PausedBallBehaviour select beh as PausedBallBehaviour))
                {
                    pbbb.Forcerevert(); //force it to revert the speed.

                    List<String> p;
                    

                    

                }
                

            }


            foreach(cBall loopball in mGameState.Balls)
            {
                loopball.Behaviours.Add(new PausedBallBehaviour(PausedBallBehaviour.GetDefaultTimeout,true));
                //set the ball's speed to zero.
                
            }
            
            //freeze all gameobjects...
            lock (mGameState.GameObjects)
            {
                foreach (GameObject loopobject in mGameState.GameObjects)
                {

                    loopobject.Frozen = true;

                }


            }

            //initialize the timer, as well, for unfreezing.
            if (unpauseCompletionTimer == null) unpauseCompletionTimer 
                = new Timer(UnfreezePauseTimer, null, PausedBallBehaviour.GetDefaultTimeout, new TimeSpan(0, 0, 0, 0, -1));

            BCBlockGameState.Soundman.PauseMusic(false);
           // BCBlockGameState.Soundman.PlaySound("Pause");
            //we want to unpause.
            //however, we will need to change the "time the game started". Since I cannot think of a reason the time would need to be accurate (representing when the game started)
            //this will have the time that the game had been paused added to it.
            TotalPauseTime = DateTime.Now-PausedStartTime;


            gamerunstate = statebeforepause;
        }
        public void FreezeAnimatedBlocks()
        {
            foreach (AnimatedBlock ab in (from m in mGameState.Blocks where m is AnimatedBlock select m))
            {

                ab.Frozen = true;


            }



        }
        public void UnfreezeAnimatedBlocks()
        {
            foreach (AnimatedBlock ab in (from m in mGameState.Blocks where m is AnimatedBlock select m))
                ab.Frozen = false;


        }
        /// <summary>
        /// timer created/set in unpause, used to unfreeze gameobjects when PausedBallBehaviours "release".
        /// </summary>
        private System.Threading.Timer unpauseCompletionTimer = null;
        private void UnfreezePauseTimer(Object parameter)
        {

            //unfreeze all gameobjects.
            lock (mGameState.GameObjects)
            {
                foreach (GameObject iterate in mGameState.GameObjects)
                {

                    iterate.Frozen = false;

                }



            }
            if(unpauseCompletionTimer!=null) unpauseCompletionTimer.Dispose();
            unpauseCompletionTimer = null;

        }


        private void frmPoing_FormClosed(object sender, FormClosedEventArgs e)
        {
            
            if(GameThread!=null&&GameThread.IsAlive)
                GameThread.Abort();
            gamerunstate = new StateNotRunning(); 
            Application.Exit();

        }
        private PointF ScalePosition(PointF DisplayPos)
        {
            float useX = ((float)DisplayPos.X / (float)PicGame.ClientSize.Width) * (float)StandardSize.Width;
            float useY = ((float)DisplayPos.Y / (float)PicGame.ClientSize.Height) * (float)StandardSize.Height;
            return new PointF(useX, useY);
        }
        private void PicGame_MouseMove(object sender, MouseEventArgs e)
        {

            PointF ClickedPos = ScalePosition(new PointF(e.X, e.Y));

            //if we're not running... don't care...
            if (gamerunstate is StateMenu)
            {

                var hitmenu = menudata.HitTest(mGameState, ClickedPos);
                if (hitmenu != null)
                {
                    hitmenu.Selected = true;


                }




            }


            if(!(gamerunstate is StateRunning)) return;
            if (!DemoMode)
            {
                
                FireMoveAbsolute(ClickedPos);
                InvokeOnMove(ClickedPos);
            }
            //add light particles, for testing.
            /*
            Random rg = BCBlockGameState.rgen;
            for (int i = 0; i < 5; i++)
            {
                PointF ourpoint = new PointF((float) (e.X+rg.NextDouble()*3f-1.5f),(float) (e.Y+rg.NextDouble()*3f-1.5f));
                LightOrb lo = new LightOrb(ourpoint, new HSLColor(rg.NextDouble()*255, 240, 120), 15);
                lo.Velocity = BCBlockGameState.GetRandomVelocity(1, 4);
                lo.VelocityDecay = new PointF(0.95f, 0.95f);
                mGameState.Particles.Add(lo);


            }
            */

        }

        private void newGameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //StartGame(CreateDefaultLevelSet(),1);
        }
        private Dictionary<Keys, ButtonConstants> GameKeyLookup = null;

        private Dictionary<Keys,ButtonConstants> InitKeyLookup()
        {
            var gotsection = BCBlockGameState.GameSettings["keys"];
            Dictionary<Keys, ButtonConstants> returndictionary = new Dictionary<Keys, ButtonConstants>();
            returndictionary.Add(Keys.Up, ButtonConstants.Button_Up);
            returndictionary.Add(Keys.Down, ButtonConstants.Button_Down);
            returndictionary.Add(Keys.Left, ButtonConstants.Button_Left);
            returndictionary.Add(Keys.Right, ButtonConstants.Button_Right);
            returndictionary.Add(Keys.Space, ButtonConstants.Button_D);
            returndictionary.Add(Keys.W, ButtonConstants.Button_E);
            returndictionary.Add(Keys.A, ButtonConstants.Button_F);
            returndictionary.Add(Keys.S, ButtonConstants.Button_G);
            returndictionary.Add(Keys.D, ButtonConstants.Button_H);
            returndictionary.Add(Keys.ShiftKey, ButtonConstants.Button_Shift);
        
            foreach (var iteratevalue in gotsection.getValues())
            {
                //ButtonConstant=Keys Constant
                ButtonConstants checkname;
                Debug.Print("Looking for Enumeration value " + iteratevalue.Name);
                if (Enum.TryParse(iteratevalue.Name, true, out checkname))
                {
                    Debug.Print("Enumeration found " + checkname.ToString() + " to match " + iteratevalue.Name);
                    //if the parse succeeds...
                    //we need to do the same for the value with the Keys Enumeration.
                    Keys checkvalue;
                    if (Enum.TryParse(iteratevalue.Value, true, out checkvalue))
                    {
                        //we have a name, and a value. Add it to the Dictionary.
                        returndictionary[checkvalue] = checkname;
                    }


                }



            }

            return returndictionary;
        }
        private ButtonConstants KeyToGameButton(Keys key)
        {
            if (GameKeyLookup == null)
            {
                //look in the keys section.

                GameKeyLookup = InitKeyLookup();



            }
            if(GameKeyLookup.ContainsKey(key))
                return GameKeyLookup[key];

            return ButtonConstants.Button_None;
            /*
            switch (key)
            {
                case Keys.Up:
                    return ButtonConstants.Button_Up;
                case Keys.Down:
                    return ButtonConstants.Button_Down;
                case Keys.Right:
                    return ButtonConstants.Button_Right;
                case Keys.Left:
                    return ButtonConstants.Button_Left;
                case Keys.Space:
                    return ButtonConstants.Button_D;
                case Keys.W:
                    return ButtonConstants.Button_E;
                case Keys.A:
                    return ButtonConstants.Button_F;
                case Keys.S:
                    return ButtonConstants.Button_G;
                case Keys.D:
                    return ButtonConstants.Button_H;
                //Button_E = 256,   //maps to "W" key (up) 
        //Button_F = 512,   //maps to "A" key (Left)
        //Button_G = 1024,  //maps to "S" key (Down)
        //Button_H=2048,    //maps to "D" key (Right)*/




         


        }

        private ButtonConstants MouseButtonToGameButton(MouseButtons mousebut)
        {
            switch (mousebut)
            {
                case MouseButtons.Left:
                    return ButtonConstants.Button_A;
                    
                case MouseButtons.Right:
                    return ButtonConstants.Button_B;
                    
                case MouseButtons.Middle:
                    return ButtonConstants.Button_C;
                    

            }
            return 0;
        }

        private void PicGame_MouseDown(object sender, MouseEventArgs e)
        {
            PointF clickpos = ScalePosition(new PointF(e.X, e.Y));
            AccelerateCountdown = true;
            if (gamerunstate is  StateMenu)
            {

                var hitmenu = menudata.HitTest(mGameState, clickpos);
                if (hitmenu != null)
                {
                    hitmenu.InvokeItemChosen();


                }




            }
            if (!(gamerunstate is StateRunning)) return;

            if (SpawnItemType != null)
            {
                GameObject addobject = (GameObject)Activator.CreateInstance(SpawnItemType, new object[]{clickpos, new SizeF(16, 16)});  //new Shell(e.Location, new SizeF(16, 16));
                
                mGameState.GameObjects.AddLast(addobject);
            }


            FireButtonDown(new ButtonEventArgs<bool>(MouseButtonToGameButton(e.Button)));
            //mGameState.GameObjects.AddLast(new Laser(new PointF(mGameState.PlayerPaddle.Position.X,mGameState.PlayerPaddle.Getrect().Top),mGameState));
        }

        private void PicGame_MouseClick(object sender, MouseEventArgs e)
        {
            PointF clickpos = ScalePosition(new PointF(e.X, e.Y));
            //
            //quick test of the AnimatedSprite class...
            //AnimatedSprite testsprite = new AnimatedSprite(new PointF(e.X-8, e.Y-8), BCBlockGameState.Imageman.getImageFrames("testdebris"), 0, 5,3);
            //add it in there
            if (hitscanner)
            {

                HitscanBullet hsb = new HitscanBullet(clickpos, new PointF(0, -2));
                hsb.Penetrate = false;

                mGameState.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => mGameState.GameObjects.AddLast(hsb)));
            }
          //  mGameState.GameObjects.AddLast(testsprite);
            if (RightClickRestart && e.Button == MouseButtons.Right)
            {
                RightClickRestart = false;
                mPlayingSet = restartset;
                DemoMode = false;
                //PlayLevel(mGameState, restartset.Levels[0]);
                mPlayingLevel = 1;
                StartGame(restartset, 1);
                return;
            }

            if (DemoMode) return;
            if (mGameState == null) return;
            if (AvailableClientArea.Contains(e.Location))
            {
                lock (mGameState)
                {
                    if (BCBlockGameState.CheatSet(BCBlockGameState.BBlockCheatMode.Cheats_Ballability))
                    {
                        cBall addme = new cBall(clickpos, new PointF(1, 1));
                        mGameState.Balls.AddLast(addme);
                    }
                    else
                    {
                        //add our silly test enemy
                       // mGameState.GameObjects.AddLast(new SillyFace(new PointF(e.Location.X, e.Location.Y)));
                        //if(KeyboardInfo.GetKeyState(Keys.ShiftKey).IsPressed)
                         //   mGameState.GameObjects.AddLast(new MegamanTest(new PointF(e.Location.X, e.Location.Y)));
                        //mGameState.GameObjects.AddLast(new ChomperEnemy(new PointF(e.Location.X,e.Location.Y),15));
                        //mGameState.GameObjects.AddLast(new EyeGuy(new PointF(e.Location.X, e.Location.Y),new SizeF(32,32)));
                    }


                }
            }


        }

        private bool AltPressed=false,ControlPressed=false,ShiftPressed=false;

        private void CheatInputRoutine(ref String InputText)
        {

            RecentCheats.Add(InputText);
            if (ProcessCheat(InputText))
                BCBlockGameState.Soundman.PlaySound("right", 1.0f);
            else
                BCBlockGameState.Soundman.PlaySound("wrong", 1.0f);


            if ((!ControlPressed) && gamerunstate is StateValueInput) gamerunstate = new StateRunning();
        }

        private void CheatEntryMode()
        {
            gamerunstate = new StateValueInput(new ValueInputData(CheatInputRoutine, "Enter Cheat Code", null, ValueInputPaintTypeConstants.paint_Pre, null), new ValueInputTextData() { Text = "", SelStart = 0 });
            
            
        }
        private void HiScoreGameProc()
        {
            Debug.Print("HiScoreGameProc");
            if ((DateTime.Now - lastHSProc).TotalMilliseconds >= 150)
            {
                if (!(ActiveState is StateValueInput)) return;
                StateValueInput svi = (StateValueInput)ActiveState;

                currentHue = (currentHue+1)%240;
                lastHSProc = DateTime.Now;
                svi.TitleColour =  new HSLColor(((float)currentHue), 240f, 120f);

            }

        }
        private DateTime lastHSProc=DateTime.Now;
        private int currentHue = 0;
        private void HiScorePaint(Graphics g, ValueInputData inputdata, ValueInputTextData CurrentTextData)
        {
            Debug.Print("HiScorePaint currenthue=" + currentHue);
            Color paintcolor = new HSLColor(((float)currentHue), 240f, 120f);
            //g.FillRectangle(new SolidBrush(Color.FromArgb(128, paintcolor)), 0, 0, mGameState.GameArea.Width, mGameState.GameArea.Height);
            Brush usebrush = new SolidBrush(Color.FromArgb(180, paintcolor));

            var ga = mGameState.GameArea;
            g.FillRectangle(usebrush, 0, 0, 16, ga.Height);
            g.FillRectangle(usebrush, 16, 0,ga.Width, 16);
            g.FillRectangle(usebrush, ga.Width - 16, 16, 16, ga.Height);
            g.FillRectangle(usebrush, 16, ga.Height - 16, ga.Width, 16);


        }
        private void HighScoreEntryMode(int Position,String sDefaultText,int sDefaultPosition = -1)
        {
            if (sDefaultPosition == -1) sDefaultPosition = sDefaultText.Length;
            //stop the music...
            var inputdata = new ValueInputData(HiscoreEntryCompletion, "High Score!(" + Position + ") Enter Your Name:",HiScorePaint,ValueInputPaintTypeConstants.paint_Post,HiScoreGameProc);
            var DefaultSetting = new ValueInputTextData() { Text = sDefaultText, SelStart=sDefaultPosition };
            BCBlockGameState.Soundman.StopMusic();
            BCBlockGameState.Soundman.PlaySound("win", false);
            gamerunstate = new StateValueInput(inputdata,DefaultSetting);
            HighScorePos = Position;
            EnterHiScore=mGameState.GameScore;
            



        }
        private int HighScorePos = 0;

        private long EnterHiScore = 0;
        private void HiscoreEntryCompletion(ref String hiscoreEntered)
        {
            String PlayerName = BCBlockGameState.Settings.PlayerName;
            Debug.Print("HiScoreEntryCompletion: " + hiscoreEntered + " Score:" + EnterHiScore);
            Debug.Print("Highscore applied to LevelSet:" + mPlayingSet.SetName);

            var scoreentry = mPlayingSet.HighScores.Submit(hiscoreEntered, (int)mGameState.GameScore);
            //don't show highscores...
            //frmhighscores hscores = new frmhighscores(mPlayingSet.HighScores, "BASeBlock High Scores (" + mPlayingSet.SetName + ")");
            //this.Invoke((MethodInvoker)(() => { hscores.ShowDialog(this); })); 


            forcegameover();
            gamerunstate = new StateLevelOutroGameOver();

        }

        private void frmBaseBlock_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
               if (gamerunstate is StateLevelOutroGameOver)
            {
                if(ControlPressed && AltPressed && e.KeyCode==Keys.H)
                {
                    //Erase the high scores for ActualPlaySet.
                    //ActualPlaySet 
                    BCBlockGameState.Soundman.PlaySound("GREN");
                    ActualPlaySet.HighScores.FillWithProxyData();
                    mPlayingSet = ActualPlaySet;
                    forcegameover();

                }

            }
            if(DemoMode) return;
            AltPressed = e.Alt;
            ControlPressed = e.Control;
            ShiftPressed=e.Shift;
         
            if (gamerunstate is StateRunning)
            {
                if (e.Alt && e.Control && e.KeyCode == Keys.C)
                {
                    //pause the game thread...
                    
                    currentcheatsel = RecentCheats.Count + 1;
                    CheatEntryMode();
                    




                }
                else if (e.Alt && e.Control && e.KeyCode == Keys.K)
                {
                    //SUICIDE
                    ProcessCheat("kill");



                }
                
                else if (e.KeyCode == Keys.Pause)
                {
                    //pause the game.
                    //gamerunstate=GameRunStateConstants.Game_Paused;
                    PauseGame();


                }
            }
            else if (gamerunstate is StatePaused)
            {
                if (e.KeyCode == Keys.Pause)
                {
                    //Unpause the game.
                    //gamerunstate = GameRunStateConstants.Game_Running;
                    UnpauseGame();

                }


            }
            else if (gamerunstate is StateValueInput)
            {
                //valid keys during cheat input- up and down to scroll through previous entries.
                StateValueInput InputState = (StateValueInput)ActiveState;
           

                if(e.KeyCode == Keys.Up || e.KeyCode== Keys.Down)
                {
                    switch (e.KeyCode)
                    {
                   




                        case Keys.Up:

                            currentcheatsel--;
                            if (currentcheatsel < 0) currentcheatsel = 0;


                            break;

                        case Keys.Down:

                            currentcheatsel++;
                            if (currentcheatsel > RecentCheats.Count) currentcheatsel = RecentCheats.Count;
                            break;


                    }
                    if (RecentCheats.Count > 0)
                    {
                        currentcheatsel = BCBlockGameState.ClampValue(currentcheatsel, 0, RecentCheats.Count - 1);
                        String usetext = RecentCheats[currentcheatsel];
                        
                        InputState.CurrentValue.Text = usetext;
                        InputState.CurrentValue.SelStart = usetext.Length;
                        
                    }


                }
                else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                {
                    switch (e.KeyCode)
                    {
                        case Keys.Left:
                            //left arrow
                            //move back one position, unless the current position is already zero.
                            //play shootfail in that case.
                            if (InputState.CurrentValue.SelStart <= 0)
                                BCBlockGameState.Soundman.PlaySound("SHOOTFAIL", 1.0f);
                            else
                            {
                                InputState.CurrentValue.SelStart--; //move back one.
                            }

                            break;
                        case Keys.Right:
                            //right arrow
                            if (InputState.CurrentValue.SelStart >= InputState.CurrentValue.Text.Length)
                                BCBlockGameState.Soundman.PlaySound("SHOOTFAIL", 1.0f);
                            else
                                InputState.CurrentValue.SelStart++;
                            break;
                    }

                }
            }
        }
       
        private void frmBaseBlock_KeyPress(object sender, KeyPressEventArgs e)
        {

            

            if(DemoMode) return;
            if (gamerunstate is StateValueInput)
            {
                StateValueInput InputState = (StateValueInput)ActiveState;
                if (((int) e.KeyChar) == 8)
                {

                    //backspace...
                    Debug.Print("Backspace pressed.");
                    if (InputState.CurrentValue.Text.Length > 0 && InputState.CurrentValue.SelStart > 0)  //idiot check. (can't remove a char from an empty string...)
                        //delete the character behind the selection.
                        if (InputState.CurrentValue.SelStart < InputState.CurrentValue.Text.Length)
                            InputState.CurrentValue.Text = InputState.CurrentValue.Text.Substring(0, InputState.CurrentValue.SelStart - 1) + InputState.CurrentValue.Text.Substring(InputState.CurrentValue.SelStart + 1);
                        else
                            InputState.CurrentValue.Text = InputState.CurrentValue.Text.Substring(0, InputState.CurrentValue.Text.Length - 1);
                    InputState.CurrentValue.SelStart--; //move selection back one.

                }
                else if ((((int)e.KeyChar) == 13)||(((int)e.KeyChar) == 10))
                {
                    //MessageBox.Show("here we would process " + VInput.Text + " as a cheat.");
                    //VInput.Text = "";
                   /*
                    RecentCheats.Add(VInput.Text);
                    if (ProcessCheat(VInput.Text))
                        BCBlockGameState.Soundman.PlaySound("right",1.0f);
                    else
                        BCBlockGameState.Soundman.PlaySound("wrong", 1.0f);
                    VInput.Text = "";
                    * */
                    InputState.InputData.CompletionRoutine(ref InputState.CurrentValue.Text);

                    //if(!ControlPressed) gamerunstate = GameRunStateConstants.Game_Running;
                    

                }
                else
                {
                    if (InputState.CurrentValue.SelStart < 0) InputState.CurrentValue.SelStart = 0;
                    if (InputState.CurrentValue.SelStart > InputState.CurrentValue.Text.Length) InputState.CurrentValue.SelStart = InputState.CurrentValue.Text.Length; 
                    //insert at appropriate position.
                    InputState.CurrentValue.Text = InputState.CurrentValue.Text.Substring(0, InputState.CurrentValue.SelStart) + e.KeyChar.ToString() + InputState.CurrentValue.Text.Substring(InputState.CurrentValue.SelStart);
                    //VInput.Text += e.KeyChar;
                    Debug.Print("VInput.Text=" + InputState.CurrentValue.Text + " char pressed was code " + ((int)e.KeyChar).ToString());
                    //increment position as well.
                    InputState.CurrentValue.SelStart++;

                }
            }
        }
        private int findstringindex(String[] arraylook, String searchfor,out String Parameter)
        {
            Parameter="";
            for (int i = 0; i < arraylook.Length; i++)
            {
                //if(arraylook[i].Equals(searchfor,StringComparison.OrdinalIgnoreCase))
                if (searchfor.StartsWith(arraylook[i], StringComparison.OrdinalIgnoreCase))
                {
                    Parameter = searchfor.Substring(arraylook[i].Length).Trim();
                    return i;

                }

            }
            return -1;


        }
        private bool ProcessCheat(IEnumerable<string> cheats)
        {
            bool retval=false;
            foreach (String Processme in cheats)
            {
                retval = retval || ProcessCheat(Processme);



            }

            return retval;


        }
        
        
        private bool ProcessCheat(String cheattext)
        {
            //same recursive definition for semicolons as well.
            if (cheattext.Contains(";"))
            {
                return ProcessCheat(cheattext.Split(';'));


            }
            String[] splitcheat = cheattext.Split(' ');
            Cheat acquirecheat = Cheat.GetCheat(splitcheat[0]);
            if (acquirecheat == null)
            {
                return OldProcessCheat(cheattext);

            }
            //remove the first element from the string array.
            splitcheat = new List<String>(splitcheat.Skip(1)).ToArray();
            //call the cheat we acquired.
            return acquirecheat.ApplyCheat(mGameState,splitcheat.Length, splitcheat);
        }
        //the old cheat routine.
        private bool OldProcessCheat(String cheattext)
        {

            if (cheattext.Contains(";"))
            {
                ProcessCheat(cheattext.Split(';'));


            }

            String[] Cheatcodes = new string[] {"The Null Cheat","Thats a paddlin","playmusic","getmewet","bombsaway",
                "imbored","speedmeup","itgrewonme","ifeelthepower","powerhascorruptedme","saveset","openset","lonely",
                "removeblocks","replaceblocks","whatisthepoint","terminated","gimmeastar","flushing meadows","nibbles.bas",
                "testtrigger","astickysituation","warpto","spawnpowerup","message","onetotransport","suzieq","kerplode","makenoise","showscores","points",
                "setspeed","kill","spawner","fbomb","doskey","macguffins","namco","changeblock","settempo",
                "dumpclasses","hitscan","changebg","bouncer","dumpgameobjects","eyeboss","dnkroz","dumpstats","firework"};
            String paramgot;
            int cheatcode = findstringindex(Cheatcodes, cheattext,out paramgot);

            try
            {
                switch (cheatcode)
                {
                    case 0:
                        break;

                    case 1:
                        //mGameState.PlayerPaddle.PaddleSize.Width *= 1.1;
                        mGameState.PlayerPaddle.PaddleSize = new SizeF(mGameState.PlayerPaddle.PaddleSize.Width * 1.5f, mGameState.PlayerPaddle.PaddleSize.Height);
                        break;
                    case 2:
                        try
                        {
                            BCBlockGameState.Soundman.PlayMusic(paramgot, 1.0f, true);
                        }
                        catch
                        {
                            return false;

                        }
                        break;

                    case 3:
                        //getmewet
                        //replace all blocks in the arena with a water block.

                        /*List<Block> newblocks = new List<Block>();

                        foreach (Block loopblock in mGameState.Blocks)
                        {
                            if(!loopblock.BlockRectangle.IsEmpty)
                                newblocks.AddLast(new WaterBlock(loopblock.BlockRectangle));


                        }
                        mGameState.Blocks=newblocks;
                        */
                        ReplaceBlocks((w) => new WaterBlock(w.BlockRectangle));

                        break;
                    case 4:
                        //Bombsaway, replae all blocks in arena with bomb blocks.
                        ReplaceBlocks((w) => new BombBlock(w.BlockRectangle));
                        break;
                    case 5:
                        //imbored
                        mGameState.invokeLevelComplete();
                        break;
                    case 6:
                        //speedmeup
                        ReplaceBlocks((w) => new SpeedBallBlock(w.BlockRectangle));
                        break;
                    case 7:
                        //itgrewonme
                        ReplaceBlocks((w) => new GrowBlock(w.BlockRectangle));
                        break;
                    case 8:
                        //give all balls the powerball behaviour.
                        foreach (cBall loopball in mGameState.Balls)
                        {
                            if (!loopball.hasBehaviour(typeof(PowerBallBehaviour)))
                                loopball.Behaviours.Add(new PowerBallBehaviour());


                        }
                        break;
                    case 9:
                        //remove all powerballbehaviours from all balls.
                        //powerhascorruptedme
                        foreach (cBall loopball in mGameState.Balls)
                        {
                            if (loopball.hasBehaviour(typeof(PowerBallBehaviour)))
                            {
                                loopball.Behaviours.RemoveAll((w) => w.GetType() == typeof(PowerBallBehaviour));

                            }

                        }


                        break;
                    case 10: //saveset
                        //SaveCurrentSet();
                        break;
                    case 11:
                        //openset
                        // OpenCurrentSet();
                        break;
                    case 12:
                        bool returnblock = false;
                        ReplaceBlocks((w) =>
                        {

                            if (returnblock) return null;
                            returnblock = true;
                            mGameState.Balls.AddLast(new cBall(new PointF(w.BlockRectangle.Left, w.BlockRectangle.Top), new PointF(2, 2)));
                            return w;


                        });
                        break;
                    case 13:
                        //removeblocks; use paramgot and remove all blocks that are of that typename.
                        String removetype = paramgot.Trim();
                        if (removetype.Length > 0)
                        {
                            //here is the issue: we want them to be able to simply say "Waterblock" or "StrongBlock" rather then "BASeBlock.WaterBlock"... first, see if they provided a "qualified" name...
                            ReplaceBlocks((w) =>
                                              {

                                                  if (w.GetType().Name.Equals(removetype, StringComparison.OrdinalIgnoreCase))
                                                      return null;
                                                  else
                                                      return w;





                                              });






                        }
                        break;

                    case 14:
                        //replaceblocks
                        String[] splitit = paramgot.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        try
                        {
                            String findtype = splitit[0];
                            string replacetype = splitit[1];
                            Type replaceT = BCBlockGameState.FindClass(replacetype);

                            if (findtype.Length > 0 && replacetype.Length > 0 && replaceT != null)
                            {
                                ReplaceBlocks((w) =>
                                                  {

                                                      if (w.GetType().Name.Equals(findtype,
                                                                                  StringComparison.OrdinalIgnoreCase))
                                                          return
                                                              (Block)
                                                              Activator.CreateInstance(replaceT,
                                                                                       new Object[] { w.BlockRectangle });
                                                      else
                                                          return w;


                                                  });

                            }
                            else
                            {

                                return false;


                            }
                        }
                        catch (Exception q)
                        {
                            Debug.Print("Unexpected exception:" + q.Message);
                        }

                        break;
                    case 15:
                        //whatisthepoint.
                        //mGameState.Balls.Clear();
                        mGameState.playerLives = 0;
                        foreach (cBall loopball in mGameState.Balls)
                        {
                            loopball.Location = new PointF(AvailableClientArea.Width / 2, AvailableClientArea.Height);
                            loopball.Velocity = new PointF(0, 5);



                        }
                        //GameOver();
                        break;
                    case 16:
                        //terminated... add terminator powerup to paddle.
                        if (mGameState.PlayerPaddle != null)
                        {
                            if (mGameState.PlayerPaddle.Behaviours.Any((w) => w.GetType() == typeof(PaddleBehaviours.TerminatorBehaviour)))
                            {
                                ((PaddleBehaviours.TerminatorBehaviour)mGameState.PlayerPaddle.Behaviours.First((w) => w.GetType() == typeof(PaddleBehaviours.TerminatorBehaviour))).PowerLevel++;


                            }
                            mGameState.PlayerPaddle.Behaviours.Add(new PaddleBehaviours.TerminatorBehaviour(mGameState));


                        }
                        break;
                    case 17:
                        //gimmeastar
                        Debug.Print("Star given." + mGameState.NumCompletions++.ToString());
                        mGameState.NumCompletions++;
                        sidebarbgdrawn = false; //make it redraw...
                        break;
                    case 18:
                        BCBlockGameState.Soundman.PlaySound("TFLUSH", 1.5f);
                        break;
                    case 19:
                        //nibbles.bas
                        String snaketype = paramgot.Trim();
                        Type snakeblocktype;
                        if (!String.IsNullOrEmpty(snaketype))
                        {
                            snakeblocktype = BCBlockGameState.FindClass(snaketype);

                            if (snakeblocktype == null)
                            {
                                return false;

                            }


                        }
                        else
                        {
                            snakeblocktype = typeof(NormalBlock);

                        }
                        mGameState.GameObjects.AddLast(new SnakeEnemy(new PointF(AvailableClientArea.Width / 2, AvailableClientArea.Height / 2), 100, snakeblocktype));
                        break;
                    case 20:
                        StringBuilder resstring = new StringBuilder();
                        foreach (Block loopblock in mGameState.Blocks)
                        {
                            if (loopblock.BlockTriggers.Count != 0)
                            {

                                resstring.AppendLine("Block with Trigger, Value=" + loopblock.BlockTriggers.Count());

                            }


                        }
                        MessageBox.Show("Triggers:" + resstring.ToString());
                        break;


                    case 21: //astickysituation
                        if (mGameState.PlayerPaddle != null)
                        {

                            mGameState.PlayerPaddle.Behaviours.Add(new PaddleBehaviours.StickyBehaviour(mGameState));


                        }
                        break;
                    case 22:
                        //warpto
                        int gotlevelindex = -1;
                        paramgot = paramgot.ToUpper();
                        if (!(Int32.TryParse(paramgot, out gotlevelindex)))
                        {
                            foreach (Level uselevel in (from j in mPlayingSet.Levels where j.LevelName.ToUpper() == paramgot select j))
                            {

                                gotlevelindex = mPlayingSet.Levels.IndexOf(uselevel);


                            }



                        }

                        if (gotlevelindex != 0)
                        {
                            gotlevelindex = BCBlockGameState.ClampValue(gotlevelindex, 0, mPlayingSet.Levels.Count);
                            PlayLevel(mGameState, mPlayingSet.Levels[gotlevelindex]);


                        }


                        break;
                    case 23:
                        //spawnpowerup: paramgot will be the name of the powerup to instantiate.
                        //plop it in the middle, also.

                        Type spawntype = BCBlockGameState.FindClass(paramgot.Trim());
                        if (spawntype == null) return false;
                        //use spawntype to create a powerup. Location (PointF) and Size (SizeF)
                        PointF centpoint = mGameState.GameArea.CenterPoint();

                        GamePowerUp createpowerup = Activator.CreateInstance(spawntype, new Object[] { centpoint, new SizeF(16, 8) }) as GamePowerUp;
                        //check for null... again...
                        if (createpowerup == null) return false;
                        //otherwise, Add it
                        mGameState.GameObjects.AddLast(createpowerup);
                        break;
                    case 24:
                        //message
                        mGameState.EnqueueMessage(paramgot);
                        break;
                    case 25:
                        //onetotransport
                        Debug.Print("One To Transport!");
                        String[] Levelnames = (from n in mPlayingSet.Levels select n.LevelName).ToArray();
                        Level[] thelevels = (from n in mPlayingSet.Levels select n).ToArray();


                        EnterMenu(Levelnames,warpchosen, thelevels);


                        break;
                    case 26:
                        //suzieq
                        mGameState.EnqueueMessage("Suzie Q has entered the game.");
                        mGameState.GameObjects.AddLast(new EyeGuy(new PointF(AvailableClientArea.Width / 2, AvailableClientArea.Height / 2)));
                        break;

                    case 27:
                        //kerplode

                        if (!String.IsNullOrEmpty(paramgot))
                        {
                            String[] splitvalues = paramgot.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            Type[] grabentry = (from m in splitvalues select BCBlockGameState.FindClass(m)).ToArray();

                            mGameState.KerploderFind = grabentry[0];
                            if (grabentry.Length > 1)
                                mGameState.KerploderReplace = grabentry[1];
                        }
                        else
                        {
                            mGameState.KerploderReplace = mGameState.KerploderFind = null;
                            
                        }

                        if ((BCBlockGameState.CheatMode & BCBlockGameState.BBlockCheatMode.Cheats_kerploder) == BCBlockGameState.BBlockCheatMode.Cheats_kerploder)
                        {
                            //remove
                            //BCBlockGameState.BBlockCheatMode.Cheats_kerploder
                            BCBlockGameState.CheatMode &= ~BCBlockGameState.BBlockCheatMode.Cheats_kerploder;
                            mGameState.EnqueueMessage("kerploder disabled.");

                        }
                        else
                        {
                            BCBlockGameState.CheatMode |= BCBlockGameState.BBlockCheatMode.Cheats_kerploder;
                            mGameState.EnqueueMessage("kerploder enabled.");
                        }


                        break;
                    case 28:
                        //makenoise
                        String soundemit = paramgot;
                        var gotsound = BCBlockGameState.Soundman.PlaySound(soundemit);
                        if (gotsound == null) return false;

                        break;
                    case 29:
                        //showscores, paramgot is name of set, or use current set if none specified.
                        String showscoreforset = mPlayingSet.SetName;
                        if (paramgot != "")
                        {

                            showscoreforset = paramgot;

                        }

                        //grab it
                        var grabscoreset = BCBlockGameState.Scoreman.getScoreForSetName(showscoreforset);




                        break;


                    case 30:
                        //points
                        float multiplier = 1.0f;

                        float.TryParse(paramgot, out multiplier);
                        mGameState.GameScore += (long)(5000 * multiplier);

                        break;
                    case 31:
                        //setspeed X Y
                        try
                        {
                            String[] splittered = paramgot.Split(' ');
                            float useX = float.Parse(splittered[0]);
                            float useY = float.Parse(splittered[1]);

                            foreach (cBall loopball in mGameState.Balls)
                            {
                                loopball.Velocity = new PointF(useX, useY);

                            }


                            break;



                        }
                        catch
                        {
                            return false;
                        }
                    case 32:
                        //kill
                        //have a little showmanship. make a small explosion at each ball.
                        lock (mGameState.GameObjects)
                        {
                            foreach (cBall loopball in mGameState.Balls)
                            {
                                ExplosionEffect ee = new ExplosionEffect(loopball.Location, 16);
                                mGameState.GameObjects.AddLast(ee);

                            }
                        }
                        mGameState.Balls.Clear();
                        break;
                    case 33:
                        //spawner <classname>

                        String classnamestr = paramgot.Trim();

                        Type getclass = BCBlockGameState.FindClass(classnamestr);
                        SpawnItemType = getclass;
                        break;
                    case 34:
                        //fbomb: spawn a frustratorball.
                        FrustratorBall fb = new FrustratorBall(new PointF(mGameState.GameArea.Width / 2, mGameState.GameArea.Height - 64), new PointF(2, 2));
                        mGameState.Balls.AddLast(fb);
                        break;
                    case 35:
                        //doskey
                        String populatefrom = paramgot.Trim();
                        DirectoryInfo di = new DirectoryInfo(populatefrom);
                        foreach (FileInfo fi in di.GetFiles())
                        {

                            if (fi.Extension.ToUpper() == ".MP3" || fi.Extension.ToUpper() == ".OGG")
                            {
                                RecentCheats.Add("PLAYMUSIC " + fi.FullName);

                            }


                        }



                        break;
                    case 36:
                        //MacGuffins
                        //spawn a LOT of macguffins.
                        //from the middle of the level-
                        PointF SpawnPosition = new PointF(mGameState.GameArea.Width / 2, mGameState.GameArea.Height / 2);
                        for (int i = 0; i < 500; i++)
                        {
                            MacGuffinOrb mgo = new MacGuffinOrb(SpawnPosition);
                            mgo.MacGuffinValue = BCBlockGameState.rgen.Next(0, 6);
                            mGameState.GameObjects.AddLast(mgo);
                            


                        }
                        mGameState.EnqueueMessage("MACGUFFINS FOR ALL!");
                        break;
                    case 37:
                        //namco
                        ChomperEnemy co = new ChomperEnemy(new PointF(mGameState.GameArea.Width / 2, mGameState.GameArea.Height / 2), 16);
                        mGameState.GameObjects.AddLast(co);
                        break;
                    case 38:
                        //changeblock
                        //syntax: changeblock <number> <newtype>
                        //changes the given block into the new type. the number should correspond to the number that DebugHelper gives, which
                        //is it's location in the linkedlist.
                        //first: parse the parameters.
                        lock (mGameState.Blocks)
                        {
                            String[] changeparameters = paramgot.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

                            int blockindex = int.Parse(changeparameters[0]);
                            //we use Parse rather than tryparse, since the exception will do what we would want in that case anyway (return false).
                            //retrieve the block type the replace it with.
                            String newblocktypestring = changeparameters[1].Trim();

                            //get the actual type...
                            Type newblocktype = BCBlockGameState.FindClass(newblocktypestring);
                            if (newblocktype == null)
                            {
                                mGameState.EnqueueMessage("Type \"" + newblocktypestring + "\" not valid.");
                                return false; //if block type was not valid, return false.

                            }
                            //otherwise, we now find the appropriate indexed block.

                            //if the given index exceeds the bounds, show a message and return false.
                            if (blockindex > mGameState.Blocks.Count - 1)
                            {
                                mGameState.EnqueueMessage("Index " + blockindex + " exceeds number of blocks.");

                            }
                            Block replaceblock = mGameState.Blocks.ElementAt(blockindex);
                            //now, construct the replacement block...
                            Block replacewith = (Block)Activator.CreateInstance(newblocktype, replaceblock.BlockRectangle);
                            //out with the old...
                            //and in with the new!
                            mGameState.Blocks.AddAfter(mGameState.Blocks.Find(replaceblock),replacewith);
                            mGameState.Blocks.Remove(replaceblock);
                            //set flag to refresh, as well.
                            mGameState.Forcerefresh = true;
                        }


                        break;
                    case 39:
                 
                        //settempo
                        float useparam = float.Parse(paramgot);
                        var acquire = BCBlockGameState.Soundman.GetPlayingMusic_Active();
                        if (acquire != null)
                            Debug.Print("Set Tempo to " + useparam);
                            acquire.Tempo = useparam;
                        break;
                    case 40:
                        //dumpclasses.
                        dumpclasses(paramgot);
                        break;
                    case 41:
                        //hitscan
                        hitscanner = !hitscanner;
                        break;
                    case 42:
                        //changebg
                        String usename = paramgot;
                        



                        if (BCBlockGameState.Imageman.HasImage(usename))
                        {
                            //if so, change to that as the background image.
                            var newbg = new BackgroundColourImageDrawer(usename);
                            newbg.RotateOrigin=PointF.Empty;
                            newbg.RotateSpeed=0;
                            newbg.MoveVelocity = PointF.Empty;
                            
                            mGameState.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                            {
                                mGameState.BackgroundDraw = newbg;
                                mGameState.Forcerefresh = true;
                                Dorefreshstats = true;
                                mRedrawAnimated = true;
                                mredrawblocks = true;
                            }));

                        }
                        else if (File.Exists(usename))
                        {
                            String newitem = BCBlockGameState.Imageman.AddFromFile(usename);
                            var newbg = new BackgroundColourImageDrawer(newitem);
                            mGameState.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                            {
                                mGameState.BackgroundDraw = newbg;
                                mGameState.Forcerefresh = true;
                                Dorefreshstats = true;
                                mRedrawAnimated = true;
                                mredrawblocks = true;
                            }));

                        }
                        break;
                    case 43:
                        mGameState.GameObjects.AddLast(new BouncerGuy(new PointF(AvailableClientArea.Width / 2, AvailableClientArea.Height / 2)));
                        break;
                    case 44:
                        if (File.Exists(paramgot)) File.Delete(paramgot);
                        StreamWriter sw = new StreamWriter(paramgot);
                        foreach (var iterate in mGameState.GameObjects)
                        {
                            sw.WriteLine("Type:" + iterate.GetType().Name + ". " + iterate);



                        }
                        mGameState.EnqueueMessage("GameObject data dumped to \"" + paramgot + "\"");
                        sw.Close();

                        break;
                    case 45:
                        //eyeboss

                        //EyeGuy eyeboss = EyeGuy.CreateBoss(mGameState.GameArea.CenterPoint(),mGameState);
                        EyeGuy eyeboss = (EyeGuy)GameEnemy.CreateBoss<EyeGuy>(mGameState.GameArea.CenterPoint(), mGameState);

                        mGameState.GameObjects.AddLast(eyeboss);
                        break;
                    case 46:
                        //dnkroz
                        if (mGameState.PlayerPaddle != null)
                            mGameState.PlayerPaddle.Behaviours.Add(new InvinciblePaddleBehaviour());
                        break;
                    case 47:
                        //dumpstats
                        String targetfile = paramgot;
                        if (mPlayingSet != null)
                        {
                            using (sw = new StreamWriter(targetfile))
                            {
                                sw.WriteLine(BCBlockGameState.Statman.ToString());

                            }
                        }
                        mGameState.EnqueueMessage("Dumped statistics to \"" + targetfile + "\"");
                        break;
                    case 48:
                        //firework

                        DustTrail dt = new DustTrail(new PointF(mGameState.GameArea.Width / 2, mGameState.GameArea.Height), new PointF(0, -3), new TimeSpan(0, 0, 0, 2),
                            new FireworkEffect(typeof(MacGuffinOrb), new PointF(0, 0), 25, 20, 50, new SizeF(8, 8)));
                        mGameState.GameObjects.AddLast(dt);


                        break;
                }
            }
            catch (Exception exx)
            {
                //log it.
                Debug.Print(exx.ToString());
                return false;


            }

            return (cheatcode != -1);

        }
        int incspawn = 0;

        private bool hitscanner = false;
        private Type SpawnItemType = null;
        private void gameToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
        
        private void dumpclasses(String targetfile)
        {
            int TypeCount = 0, totalcount = 0;

            if (File.Exists(targetfile))
            {
                mGameState.EnqueueMessage("Target file already exists.");
                return;

            }

            //otherwise, dump the data from out type manager.

            using (StreamWriter sw = new StreamWriter(new FileStream(targetfile,FileMode.CreateNew)))
            {
                sw.WriteLine("BASeBlock Version " + BCBlockGameState.GetExecutingVersion() + " Class Dump\n\n");
            foreach (var iterate in BCBlockGameState.MTypeManager.loadeddata)
            {
                TypeCount++;
                sw.WriteLine("Type:" + iterate.Key.FullName);
                foreach (Type iteratetype in iterate.Value.ManagedTypes)
                {
                    sw.WriteLine("\tImplemented/Derived Class:" + iteratetype.FullName);


                    totalcount++;

                }
                sw.WriteLine();

            }
            sw.WriteLine(TypeCount.ToString() + " Types; Total of " + totalcount + " Implementations/derivations.");
            sw.WriteLine("---END OF FILE--");
            }
            mGameState.EnqueueMessage("Types dumped to " + targetfile);




        }
        private void gameToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            demoModeToolStripMenuItem.Checked=DemoMode;
            stopGameToolStripMenuItem.Enabled = (GameThread != null && GameThread.IsAlive && !(gamerunstate is StateNotRunning));
        }

        private void demoModeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DemoMode=!DemoMode;
        }



        #region iGameClient Members

        public void RefreshDisplay()
        {
            PicGame.Invoke((MethodInvoker)(() =>
            {
                PicGame.Invalidate();
                PicGame.Update();
            }));
        }
        public Bitmap getBackgroundBitmap()
        {
            return backgroundbitmap;
        }

        public Bitmap getCanvasBitmap()
        {
            return mGameState.lastframeimage;
        }

        #endregion

        #region iGameClient Members


       

        #endregion

        #region iGameClient Members


        public Level getcurrentLevel()
        {
            return mPlayingSet.Levels[mPlayingLevel-1];
        }

        #endregion

        #region iGameClient Members


        public event EventHandler<ButtonEventArgs<bool>> ButtonDown;

        public event EventHandler<ButtonEventArgs<bool>> ButtonUp;

        public event EventHandler<MouseEventArgs<bool>> OnMove; 
        public event Func<PointF, bool> MoveAbsolute;



        public void FireButtonDown(ButtonEventArgs<bool> callwith )
        {
            var temp = ButtonDown;
            if (temp != null)
                temp.Invoke(this,callwith);


        }
        public void FireButtonUp(ButtonEventArgs<bool> callwith )
        {
            var temp = ButtonUp;
            if (temp != null)
                temp.Invoke(this,callwith);


        }
        public void FireMoveAbsolute(PointF pointmove)
        {
            Func<PointF, bool> temp = MoveAbsolute;
            if (temp != null)
                temp.Invoke(pointmove);



        }
        public void MoveRelative(float Amount)
        {
            //get current paddle position.
            if(mGameState==null) return;
            if (mGameState.PlayerPaddle == null) return;
            PointF Paddlespot = mGameState.PlayerPaddle.Position;
            PointF newpos = new PointF(Paddlespot.X+Amount,Paddlespot.Y);
            FireMoveAbsolute(newpos);



        }

        #endregion

        private void PicGame_MouseUp(object sender, MouseEventArgs e)
        {
            AccelerateCountdown = false;
            if (!(gamerunstate is StateRunning)) return;
            
            if(((BCBlockGameState.CheatMode & BCBlockGameState.BBlockCheatMode.Cheats_kerploder) == BCBlockGameState.BBlockCheatMode.Cheats_kerploder))
            {

                //spawn a kerplodey thing.
                if (mGameState.KerploderFind != null || mGameState.KerploderReplace != null)
                {
                    BlockChangeEffect bce = new BlockChangeEffect(e.Location, 16, 64, mGameState.KerploderFind, mGameState.KerploderReplace);
                    mGameState.GameObjects.AddLast(bce);

                }
                else if (e.Button == MouseButtons.Left)
                {
                    ExplosionEffect ee = new ExplosionEffect(e.Location);
                    mGameState.GameObjects.AddLast(ee);
                }
                else if (e.Button == MouseButtons.Right)
                {

                    CreationEffect ee = new CreationEffect(new SizeF(33,16), e.Location,64);
                    mGameState.GameObjects.AddLast(ee);

                }
            }


            FireButtonUp(new ButtonEventArgs<bool>(MouseButtonToGameButton(e.Button)));
            
        }
        private cNewSoundManager newmanager;
        private void testNewSoundEngineToolStripMenuItem_Click(object sender, EventArgs e)
        {
           
            /*if(newmanager==null)
                newmanager = new cNewSoundManager(new BASSDriver(),BCBlockGameState.GameSettings["folders"]["sound"].Value);


            //newmanager.PlayMusic("ENDLESSCHALLENGE");
            newmanager.PlayMusic("COMICAL4");
            newmanager.GetSound("BBOUNCE").Play(false);
            */
            BCBlockGameState.Soundman.PlayMusic("COMICAL4");








        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //quit
            //first, if a game is in progress, ask for confirmation.
            if (mGameState != null)
            {
                
                if (!(gamerunstate is StateNotRunning) &&  !(gamerunstate is StateLevelOutroGameOver))
                {
                    //pause the gameproc thread while we ask; do this by changing gamerunstate...
                    statebeforepause = gamerunstate;
                    gamerunstate = new StatePaused();
                    if (MessageBox.Show("There is a Game in Progress. Quit Anyway?", "Exit Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2) == DialogResult.Yes)
                    {
                        //they said yes... abort the thread.
                        //high scores will not be checked when quitting.
                        GameThread.Abort();

                        


                    }
                    else
                    {
                        //they decided not to quit.
                        return;


                    }



                }


                
            }
            Debug.Print(Application.OpenForms.Count + " Open Forms:");
            foreach (var formiterate in Application.OpenForms)
            {
                Debug.Print((formiterate as Form).Name);

            }
            Application.Exit();
        }

        private void highScoresToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //testing: show net highscores for item 3;
            if (mPlayingSet == null)
            {
                frmhighscores scoreform = new frmhighscores("BASeBlock");
                scoreform.Show(this);
            }
            else
            {


                IHighScoreList scoreobject = mPlayingSet.HighScores;
                frmhighscores scoreform = new frmhighscores(scoreobject,
                                                            "BASeBlock");
                scoreform.Show(this);
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // SaveCurrentSet();
        }
        /*
        private void SaveCurrentSet()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Title = "Save LevelSet";
            sfd.Filter = "BASeBlock Level Set File (*.LSF)|*.LSF|All Files(*.*)|*.*";
            DialogResult result = sfd.ShowDialog(this);
            if (result == DialogResult.Cancel)
            {
                return;

            }
            else
            {
                //file was selected to save to.
                mPlayingSet.Save(sfd.FileName);
            }
        }
        */
        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
           // OpenCurrentSet();
        }
        /*
        private void OpenCurrentSet()
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Title = "Open Levelset";
            ofd.Filter = "BASeBlock Level Set File (*.LSF)|*.LSF|All Files(*.*)|*.*";
            DialogResult result = ofd.ShowDialog(this);
            if(result==DialogResult.Cancel)
                return;
            else
            {
                mPlayingSet = BASeBlock.LevelSet.FromFile(ofd.FileName);
            }
        }
*/
        private bool HasAttribute(Type typecheck, Type checkforattribute)
        {
            return (System.Attribute.GetCustomAttributes(typecheck).Any((p) => p.GetType() == checkforattribute));

        }

        private void newGameToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            //add a new entry for each item in BCBlockGameState.Levelman 
            //clear current entries.
            ToolStripMenuItem mitem = (ToolStripMenuItem)sender;
            mitem.DropDownItems.Clear();
            Debug.Print("KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey)=" + KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey));

            foreach (Type iteratetype in BCBlockGameState.Levelman.ManagedTypes)
            {
                //attempt to create an instance...
                if( (!(HasAttribute(iteratetype,typeof(InvisibleBuilderAttribute))) || KeyboardInfo.GetAsyncKeyState((int)Keys.ShiftKey)<0))
                {
                    iLevelSetBuilder newinstance;
                    try
                    {
                        newinstance = (iLevelSetBuilder) Activator.CreateInstance(iteratetype);
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
        
        private void DoAutoLoad(String cmd)
        {
            //BCBlockGameState.Levelman.ManagedTypes
            //check for /load

            


        }

        void newdropdown_Click(object sender, EventArgs e)
        {
            //MessageBox.Show("Create levelset from " + ((iLevelSetBuilder)((ToolStripMenuItem)sender).Tag).getName());


            iLevelSetBuilder usebuilder = ((iLevelSetBuilder)((ToolStripMenuItem)sender).Tag);
            gamerunstate = new StateLoading();
            PicGame.Invalidate();
            PicGame.Update();
            LevelSet lsetuse = usebuilder.BuildLevelSet(AvailableClientArea,this);
            if (lsetuse == null)
            {
                return;


            }
            else
            {
                gamerunstate = new StateNotRunning();
                DemoMode = false;
                StartGame(lsetuse, 1);

            }
            //throw new NotImplementedException();
        }



        #region iGameClient Members

       
        bool iGameClient.DemoMode
        {
            get
            {
                return this.DemoMode;

            }
        }
        private Bitmap measurebitmap= new Bitmap(1,1);
        private Graphics measuregraphics=null;
        public SizeF MeasureString(String stringmeasure, Font fontuse)
        {

            //this can be accessed cross-thread, so we lock it in both locations.
            if (measuregraphics == null)
                measuregraphics = Graphics.FromImage(measurebitmap);
            
            return measuregraphics.MeasureString(stringmeasure, fontuse);
            

        }

        #endregion

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            new frmUpdates().Show();
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (gamerunstate is StateRunning)
                gamerunstate = new StatePaused();


            
            SplashScreen showscreen = new SplashScreen(true);
            showscreen.Show();
            showscreen.Focus();
        }
        private ButtonConstants currentlypressed;
        public ButtonConstants getPressedButtons()
        {
            return currentlypressed;

        }

        public void InvokeButtonDown(ButtonEventArgs<bool> callwith )
        {

            ButtonDown.Invoke(this,callwith);
        }

        public void InvokeButtonUp(ButtonEventArgs<bool> callwith )
        {
            ButtonUp.Invoke(this,callwith);
        }

        public void InvokeMoveAbsolute(PointF newlocation)
        {
            MoveAbsolute.Invoke(newlocation);
        }
        public void InvokeOnMove(PointF pPosition)
        {
            MouseEventArgs<bool> me = new MouseEventArgs<bool>(0, pPosition);
            var copied = OnMove;
            if(copied==null) return;
            copied(this, me);
        }
        private frmEditor editform;
        private void editorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (editform==null||!editform.Visible)
                editform = new frmEditor();

            editform.Show(this);

        }

        private void stopGameToolStripMenuItem_Click(object sender, EventArgs e)
        {




            if(GameThread==null) return;

            gamerunstate = new StatePaused();
            //ask...
            if (MessageBox.Show(this, "Are you sure you want to stop this game?", "Stop Game", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
            {
                return;

            }

            gamerunstate = new StateNotRunning();
            //kill the gamethread
           
            GameThread.Abort();
            BCBlockGameState.Soundman.StopMusic();
            PicGame.Invalidate();
            PicGame.Update();
            OpenFileDialog ofd = new OpenFileDialog();
            
        }

        private void ghostToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }
        private DebugHelper HelperDebugObject = null;
        private bool dosaveshot = false;
        private Dictionary<cBall, PointF> savedF7speeds = null;
        private void frmBaseBlock_KeyDown(object sender, KeyEventArgs e)
        {

            if (gamerunstate is StateValueInput)
            {
                StateValueInput InputState = (StateValueInput)gamerunstate;
                if (e.KeyCode == Keys.Escape)
                {
                    InputState.CurrentValue.SelStart = 0;
                    InputState.CurrentValue.Text = "";
                }
            }

            if (e.KeyCode == Keys.F12)
            {
                if (!(gamerunstate is StateNotRunning))
                {
                    dosaveshot=true;


                }



            }
            else if (e.KeyCode == Keys.F7)
            {
                if (gamerunstate is StateRunning)
                {
                    if (savedF7speeds != null)
                    {
                        mGameState.EnqueueMessage("Ball speeds reverted.");
                        foreach (var iterate in savedF7speeds)
                        {
                            iterate.Key.Velocity = iterate.Value;

                        }
                        
                        savedF7speeds=null;
                    }
                    else
                    {
                        mGameState.EnqueueMessage("Ball speeds stopped.");
                        savedF7speeds=new Dictionary<cBall, PointF>();
                        foreach (cBall stationit in mGameState.Balls)
                        {
                            savedF7speeds.Add(stationit, stationit.Velocity);
                            stationit.Velocity = new PointF(0, 0);

                        }
                    }


                }



            }
            if(gamerunstate is StateMenu)
                {


                    if (menudata.SelectedItem == null)
                    {
                        //if no selection, select the first item.
                        menudata.SelectedItem = menudata.items.First();

                    }
                    else
                    {

                        var currentnode = menudata.items.Find(menudata.SelectedItem);
                        if (e.KeyCode == Keys.Enter)
                        {
                            menudata.SelectedItem.InvokeItemChosen();
                            ExitMenu();
                        }
                        else if (e.KeyCode == Keys.Escape)
                        {

                            ExitMenu();

                        }
                        else if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down || e.KeyCode == Keys.Right ||
                                 e.KeyCode == Keys.Left)
                        {
                            menudata.SelectedItem.InvokeItemUnselect();

                            if (e.KeyCode == Keys.Up)
                            {
                                //select the previous item.
                                BCBlockGameState.Soundman.PlaySound("MENU_SELECT");
                                if (currentnode.Previous == null)
                                {
                                    //BCBlockGameState.Soundman.PlaySound("WRONG");
                                    //no previous item to select. remain unchanged...
                                    menudata.SelectedItem = currentnode.List.Last.Value;
                                }
                                else
                                {
                                    //currentnode = currentnode.Previous.Value;
                                    menudata.SelectedItem = currentnode.Previous.Value;

                                }
                            }
                            else if (e.KeyCode == Keys.Down)
                            {
                                BCBlockGameState.Soundman.PlaySound("RIGHT");
                                if (currentnode.Next == null)
                                    menudata.SelectedItem = currentnode.List.First.Value;
                                else
                                {

                                    menudata.SelectedItem = currentnode.Next.Value;
                                }


                            }
                            menudata.SelectedItem.InvokeItemSelect();

                        }
                    }

                }
                
              
                
                else if (e.KeyCode == Keys.F4)
            {
              if (gamerunstate is StateRunning)
                {


                    menudata = new MenuModeData(new String[] { "Item1", "Item2", "Item3", "Item4" },menuitems_MenuItemSelect,menuitems_MenuItemUnselect,menuitems_MenuItemChosen,menuitems_MenuItemPrePaint);
                    //gamerunstate = GameRunStateConstants.Game_Menu;
                    EnterMenu();
                }

            }
            
            else if (e.KeyCode == Keys.F3)
            {
                if (KeyboardInfo.IsPressed(Keys.ShiftKey))
                {
                    //remove debug helper if present.
                    if (HelperDebugObject != null)
                    {
                        mGameState.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => mGameState.GameObjects.Remove(HelperDebugObject)));
                        //mGameState.GameObjects.Remove(HelperDebugObject);
                        HelperDebugObject = null;


                    }
                    else
                    {
                        HelperDebugObject = new DebugHelper();
                        mGameState.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => mGameState.GameObjects.AddLast(HelperDebugObject)));
                        //mGameState.GameObjects.AddLast(HelperDebugObject);
                    }
                }
                else
                {
                    ShowDebugInfo = !ShowDebugInfo;
                }
            }

            else
            {
                if(gamerunstate is StateRunning)
                    FireButtonDown(new ButtonEventArgs<bool>(KeyToGameButton(e.KeyCode)));

            }
        }

        private void ExitMenu()
        {
            //throw new NotImplementedException();
            //reverse appropriate actions taken in EnterMenu.
            //BCBlockGameState.Soundman.PopMusic();
            if (gamerunstate is StateMenu)
            {
                BCBlockGameState.Soundman.StopTemporaryMusic(menudata.MenuMusic);
                gamerunstate = new StateRunning();
            }
        }
        iActiveSoundObject menumusic=null;
        private void EnterMenu(String[] menuoptions, MenuModeMenuItem.MenuItemEvent chosenroutine, Object[] menutags)
        {
            menudata = new MenuModeData(menuoptions, menuitems_MenuItemSelect, menuitems_MenuItemUnselect, chosenroutine, menuitems_MenuItemPrePaint);
            int currindex = 0;
            foreach (var loopitem in menudata.items)
            {
                loopitem.Tag = menutags[currindex];
                loopitem.MenuItemPrePaint += menuitems_MenuItemPrePaint;
                currindex++;


            }
            EnterMenu();
        }

        private void EnterMenu(String[] menuoptions,Object[] menutags)
        {
            //recreate the menu based on the given options...
            menudata = new MenuModeData(menuoptions, menuitems_MenuItemSelect, menuitems_MenuItemUnselect, menuitems_MenuItemChosen, menuitems_MenuItemPrePaint);
            int currindex = 0;
            foreach (var loopitem in menudata.items)
            {
                loopitem.Tag = menutags[currindex];
                loopitem.MenuItemPrePaint += menuitems_MenuItemPrePaint;
                currindex++;



            }
            //now, call the "base" EnterMenu routine, which will pause the music, play the appropriate menu music, and change the runstate to show the menu.
            EnterMenu();
        }

        private void EnterMenu()
        {
            //Play the menu music.
            //pause, and cache the current music...
            BCBlockGameState.Soundman.PauseMusic(true);
            ////menumusic = BCBlockGameState.Soundman.PushMusic("CREDITS", 1.0f, true);
            menumusic = BCBlockGameState.Soundman.PlayTemporaryMusic("CREDITSONG", 1.0f, true);
            gamerunstate = new StateMenu();
            

            //Enter the menu: play the relevant sound, pause current music, and play menu music. Then set menu mode.
            //throw new NotImplementedException();
        }

        private Brush _DefUnSelBrush = null, _DefSelBrush = null;

      

          


        void menuitems_MenuItemPrePaint(MenuModeMenuItem sender, RectangleF drawarea)
        {
            if (!sender.Selected)
            {
                if (_DefUnSelBrush == null)
                {

                    _DefUnSelBrush = new LinearGradientBrush(new PointF(0, 0), new PointF(0, drawarea.Height), Color.White, Color.Gray);



                }
                sender.OurStyle.Background = _DefUnSelBrush;
            }
            else
            {
                if (_DefSelBrush == null)
                {
                    _DefSelBrush = new LinearGradientBrush(new PointF(0, 0), new PointF(0, drawarea.Height), Color.Yellow, Color.Green); 


                }
                sender.OurStyle.Background = _DefSelBrush;
            }
        }

        //Menu Mode item event handling.
        void menuitems_MenuItemUnselect(MenuModeMenuItem sender)
        {
            Debug.Print("MenuItemUnselected, text=" + sender.Text);
            //throw new NotImplementedException();
        }
        void menuitems_MenuItemSelect(MenuModeMenuItem sender)
        {
            Debug.Print("MenuItemSelected, text=" + sender.Text);
            //throw new NotImplementedException();
        }
        void warpchosen(MenuModeMenuItem sender)
        {
            mPlayingLevel = mPlayingSet.Levels.IndexOf((Level)sender.Tag)+1; //haha...
            PlayLevel(mGameState, (Level)sender.Tag);
        }
        void menuitems_MenuItemChosen(MenuModeMenuItem sender)
        {
            Debug.Print("MenuItemChosen, text=" + sender.Text);
            //throw new NotImplementedException();
            //exit menu mode.
            gamerunstate = new StateRunning();
        }
        private void frmBaseBlock_KeyUp(object sender, KeyEventArgs e)
        {
            if(gamerunstate is StateRunning)
                FireButtonUp(new ButtonEventArgs<bool>(KeyToGameButton(e.KeyCode)));
        }

        private void BrowserToolStripItem_Click(object sender, EventArgs e)
        {
            frmLevelBrowser fbrowser = new frmLevelBrowser();
            fbrowser.ShowDialog();
        }

        private void keyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            String[] Names = new string[] {"Michael Burgwin","BC_Programmer","BC_Programming","John Doe"};
            String[] Organizations = new string[] {"BASeCamp Corporation","BASeCamp Software Solutions","Microsoft","The Free Software Foundation"};
            String[] ApplicationNames = new string[] {"BASeBlock","BCSearch","Gravgame","BCJobClock"};
            using (FileStream writekeydata = new FileStream("D:\\keyout.txt", FileMode.Create))
            {
                StreamWriter writerobj = new StreamWriter(writekeydata);
                foreach (String UserName in Names)
                    foreach (String Organization in Organizations)
                        foreach (String Appname in ApplicationNames)
                        {
                            String generatedkey = LicensedFeatureData.SecureStringstr(LicensedFeatureData.GenKey(UserName, Organization, Appname));

                            writerobj.WriteLine("Name:" + UserName);
                            writerobj.WriteLine("Organization:" + UserName);
                            writerobj.WriteLine("Application:" + UserName);
                            writerobj.WriteLine("GeneratedKey:" + generatedkey);
                            writerobj.WriteLine();

                        }

                writerobj.Close();
            }
        }


        public void ShowMessage(string message)
        {
            if(mGameState!=null)
                mGameState.EnqueueMessage(message);
        }
        public void FlagError(String ErrorDescription, Exception AttachedException)
        {
            //
        }
        public void UpdateProgress(float ProgressPercentage)
        {
            //throw new NotImplementedException();

        }
    }
}
