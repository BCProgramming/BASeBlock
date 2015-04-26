using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using System.Xml.Linq;
using BASeBlock.Blocks;
using Ionic.Zlib;
using bcHighScores;
using BASeCamp.XMLSerialization;

namespace BASeBlock
{
    [Serializable]
    public class BBImageSerializationData
    {
        private float [][] _ColorMatrixValues = null;

        public ColorMatrix ColorMatrixValues
        {
            get {
                if (_ColorMatrixValues == null) _ColorMatrixValues = ColorMatrices.GetIdentity();
            
                return new ColorMatrix(_ColorMatrixValues);
            }
            

        }

        public String ImageKey { get; set; }
        


    }
    /// <summary>
    /// Holds the level template data.
    /// These are (or rather, will be) loaded for LTF (Level Template File's) files in a Templates folder.
    /// </summary>
    /// 
    /*[Serializable]
    public class LevelTemplateData
    {
        private Level _LevelData;
        public Level LevelData { get { return _LevelData; } set { _LevelData = value; } }


        /// <summary>
        /// assigns/clones the template data to the given level.
        /// </summary>
        /// <param name="leveldata"></param>
        public void AssignTo(Level leveldata)
        {






        }

        public static LevelTemplateData FromFile(String pFilename)
        {
            LevelTemplateData retrievedata = null;

            FileStream fs = new FileStream(pFilename,FileMode.Open);
            DeflateStream dstream = new DeflateStream(fs, CompressionMode.Decompress);
            retrievedata = FromStream(dstream);
            return retrievedata;


        }
        public static LevelTemplateData FromStream(Stream datastream)
        {
            LevelTemplateData returnobject = new LevelTemplateData();
            //read from a stream.
            BinaryFormatter bf = new BinaryFormatter();
            returnobject.LevelData = (Level)bf.Deserialize(datastream);

            return returnobject;

        }
        public static void ToStream(LevelTemplateData filedata, Stream data)
        {
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(data, filedata);


        }
        public static void ToFile(LevelTemplateData filedata, String pFilename)
        {
            FileStream fs = new FileStream(pFilename, FileMode.Create);
            DeflateStream ds = new DeflateStream(fs, CompressionMode.Compress);
            ToStream(filedata, ds);



        }

    }
    */
    [Serializable]
    public class Level : ISerializable ,IDeserializationCallback,IXmlPersistable
    {
        //holds level data. 
        private static Font IntroDefaultFont = new Font(BCBlockGameState.GetMonospaceFont(), 24);
        public bool NoPaddle=false;

        
        public String PreviewImageKey=null;
        private List<TriggerEvent> _Events=new List <TriggerEvent>();
       
        [Editor(typeof(EventCollectionEditor), typeof(UITypeEditor))]
        public List<TriggerEvent> LevelEvents { get { return _Events; } set { _Events = value; } }
        public int StartTrigger = 0;
        public List<Block> levelblocks = new List<Block>();
        public List<cBall> levelballs = new List<cBall>();
        private float[][] _SidebarColorMatrixValues=null;
        private float[][] _PauseColorMatrixValues = null;
        private String _ClearTitle = "       SCORE     \n";
        public String ClearTitle { get { return _ClearTitle; } set { _ClearTitle = value; } }
        public ColorMatrix PauseColorMatrixValues
        {
            get
            {
                if (_PauseColorMatrixValues == null) _PauseColorMatrixValues = ColorMatrices.GetIdentity();

                return new ColorMatrix(_PauseColorMatrixValues);


            }
        }
        private SizeF _PauseImageScale = new SizeF(4f, 4f);


        public SizeF PauseImageScale
        {
            get { return _PauseImageScale; }
            set { _PauseImageScale = value; }
        }

        private String _PauseImageKey = "paused";

        public String PauseImageKey
        {
            get { return _PauseImageKey; }
            set { _PauseImageKey = value; }
        }

        private String _GameOverPicKey = "TALLYPIC";
        public String GameOverPicKey { get { return _GameOverPicKey;} set {_GameOverPicKey = value;}}

        private Font _PauseFont = new Font("Arial", 24);

        public Font PauseFont
        {
            get { return _PauseFont; }
            set { _PauseFont = value; }
        }


        private Color _PauseTextColor = Color.Black;
        
        public Color PauseTextColor
        {
            get { return _PauseTextColor; }
            set { _PauseTextColor = value; }
        }

        public ColorMatrix SidebarColorMatrixValues
        {
            get
            {
                if (_SidebarColorMatrixValues == null)
                {//identity...
                    _SidebarColorMatrixValues = ColorMatrices.GetIdentity();


                        ;
                   

                }

            return new ColorMatrix(_SidebarColorMatrixValues);

            }
        }
        private String _SidebarImageKey = "";
        public String SidebarImageKey
        {
            get { return _SidebarImageKey; }
            set { _SidebarImageKey = value; }

        }
        private Color _SidebarTextColor=Color.Black;
        public Color SidebarTextColor
        {
            get { return _SidebarTextColor; }
            set { _SidebarTextColor = value; }
        }
            public TimeSpan _ShowNameLength = new TimeSpan(0, 0, 0, 0); //default to the intro sound.
        
            private BackgroundColourImageDrawer _Background;
        [Editor(typeof(BASeBlock.ObjectTypeEditor), typeof(UITypeEditor))]

            public BackgroundColourImageDrawer Background { get { EnsureLoaded(); return _Background; } set { EnsureLoaded(); _Background = value; } }
            [TypeConverter(typeof(TimeSpanConverter))]
        public TimeSpan ShowNameLength { 
                get { EnsureLoaded(); 
                    
                    
                    return _ShowNameLength; } 
                set { EnsureLoaded(); _ShowNameLength = value; } }

            private String _PauseSound = "PAUSE";
            private String _IntroMusicName = "INTRO";
            private String _MusicName = "BASESTOMP";
            public String PauseSound { get { EnsureLoaded(); return _PauseSound; } set { EnsureLoaded(); _PauseSound = value; } }
            public string IntroMusicName { get { EnsureLoaded(); return _IntroMusicName; } set { EnsureLoaded(); _IntroMusicName = value; } }
            public String MusicName { get { EnsureLoaded(); return _MusicName; } set { EnsureLoaded(); _MusicName = value; } }

        private String _DeathSound = "MMDEATH";

        public String DeathSound
        {
            get { EnsureLoaded(); if (_DeathSound == null) _DeathSound = "MMDEATH"; //if null, use a default.
                    return _DeathSound;
        }
            set { EnsureLoaded(); _DeathSound = value; }
        }
        private String _LevelCompleteString="Level Complete!";
        public String EndLevelString { get { return _LevelCompleteString; } set { _LevelCompleteString = value; } }
            public Color LevelNameIntroTextPenColor { get; set; }
        public Color LevelNameIntroTextFillColor { get; set; }
        private Font _LevelNameIntroFont=IntroDefaultFont;
        private List<PlayMessageData> _MessageData = new List<PlayMessageData>();
        public Queue<PlayMessageData> MessageQueue;
        

        public static void PopulateDropDown(List<Level> levels, ToolStripDropDown ts, Func<Level,ToolStripMenuItem,bool> AddFunction=null,Action<Level,ToolStripItem> LevelClicked=null,bool doClear=true)
        {
            if (levels.Count == 0) return;
            ts.Items.Clear();
            foreach (var iterlevel in levels)
            {
                Image drawresult = BCBlockGameState.DrawLevelToImage(iterlevel, new Size(32, 32));
                ToolStripMenuItem createditem = new ToolStripMenuItem(iterlevel.LevelName +"(" + iterlevel.levelblocks.Count + " Blocks)", drawresult)
                                                    {
                                                        Tag
                                                            =
                                                            iterlevel
                                                    };


                if (AddFunction == null || AddFunction(iterlevel, createditem))
                {
                    if (LevelClicked != null)
                    {
                        Level closedLevel = iterlevel;
                        createditem.Click += (obj, ev) => LevelClicked(closedLevel, createditem);
                    }
                }
                

            }



        }


        private void InitMessageStack()
        {
            MessageQueue = new Queue<PlayMessageData>();
            //for...each type loop will go through the sorted list in ascending order.
            SortedList<float, PlayMessageData> tempsort = new SortedList<float, PlayMessageData>();

            foreach (var sortitem in _MessageData)
            {
                 MessageQueue.Enqueue(sortitem);

            }


            

            



        }

        public Image GetLevelThumbnail()
        {

            return BCBlockGameState.DrawLevelToImage(this);
        }
        public Image GetLevelThumbnail(Size imagesize)
        {
            return BCBlockGameState.DrawLevelToImage(this, imagesize);


        }


        public Queue<PlayMessageData> CreateMessageQueue()
        {
            Queue<PlayMessageData> returnresult = new Queue<PlayMessageData>();
            //loop through list in sorted order.
            SortedList<float, PlayMessageData> tempsort = new SortedList<float, PlayMessageData>();
            foreach (var sortitem in _MessageData)
            {
                tempsort.Add(sortitem.TimeIndex, sortitem);

            }

            foreach (var sorteditem in tempsort)
            {
                returnresult.Enqueue((PlayMessageData)sorteditem.Value.Clone());

            }

            return returnresult;

        }
        public List<PlayMessageData> MessageData
        {
            get { EnsureLoaded(); return _MessageData; }
            set
        {
            EnsureLoaded();
            _MessageData = value;
            InitMessageStack();
        
        } }
            public Font LevelnameIntroFont
        {
            get
            {
                EnsureLoaded();
                if (_LevelNameIntroFont == null) _LevelNameIntroFont = IntroDefaultFont;
                return _LevelNameIntroFont;
                
            }
            set { _LevelNameIntroFont = value; }
        }


            private List<Type> _AvailablePowerups = GamePowerUp.GetPowerUpTypes().ToList();


            [Editor(typeof(TypeCheckboxItem<GamePowerUp>), typeof(UITypeEditor))]
            public List<Type> AvailablePowerups { get { return _AvailablePowerups; } set { _AvailablePowerups = value; } } 

            private String _LevelName;
            private String _Description = "";
            private String _TallyMusicName;
            private String _TallyTickSound;
            private String _TallyPicKey="TALLYPIC";
            private String _GameOverMusic;
            private int _NextLevel;
            private int _MaxBalls; //maximum number of balls allowed in play.
        
            public String LevelName { get { EnsureLoaded(); return _LevelName; } set { EnsureLoaded(); _LevelName = value; } }
            public String Description { get { EnsureLoaded(); return _Description; } set { EnsureLoaded(); _Description = value; } }
            public String TallyMusicName { get { EnsureLoaded(); return _TallyMusicName; } set { EnsureLoaded(); _TallyMusicName = value; } }
            public String TallyTickSound { get { EnsureLoaded(); return _TallyTickSound; } set { EnsureLoaded(); _TallyTickSound = value; } }
            public String TallyPicKey { get { EnsureLoaded(); return _TallyPicKey; } set { EnsureLoaded(); _TallyPicKey = value; } }
            public string GameOverMusic { get { EnsureLoaded(); return _GameOverMusic; } set { EnsureLoaded(); _GameOverMusic = value; } }
            public int NextLevel { get { EnsureLoaded(); return _NextLevel; } set { EnsureLoaded(); _NextLevel = value; } }
            public int MaxBalls { get { EnsureLoaded(); return _MaxBalls; } set { EnsureLoaded(); _MaxBalls = value; } }
       
       
            /// <summary>
        /// Creates a bitmap containing the Introductory text, or other elements
        /// </summary>
        /// <param name="forlevel">level whose introductory textbitmap is to be rendered </param>
        /// <param name="thebitmap">resulting bitmap</param>
        /// <param name="thegraphics">a Graphics object created from said bitmap</param>
        public void CreateIntroBitmap(out Bitmap thebitmap, out Graphics thegraphics)
        {
            EnsureLoaded();
            Bitmap createme = new Bitmap(1, 1);
            Level forlevel = this;
            Font intfont = forlevel.LevelnameIntroFont;

            Graphics gotgraphics = Graphics.FromImage(createme);
            gotgraphics.PageUnit = GraphicsUnit.Point;
            SizeF measureit = gotgraphics.MeasureString(forlevel.LevelName, intfont,PointF.Empty,StringFormat.GenericTypographic);
            
            gotgraphics.Dispose();
            measureit = new SizeF(measureit.Width, measureit.Height);
            //destroy the graphics and recreate it..
            //add height and width for stroke.

            

            if (measureit.Width != 0 && measureit.Height != 0)
            {

                createme = new Bitmap((int)Math.Ceiling(measureit.Width), (int)Math.Ceiling(measureit.Height),PixelFormat.Format32bppArgb);
                //adds ten pixels... for... well no reason, really.
                gotgraphics = Graphics.FromImage(createme);
                gotgraphics.CompositingQuality = CompositingQuality.HighQuality;
                gotgraphics.SmoothingMode = SmoothingMode.HighQuality;
                //Excellent, we now have a graphics context on which to draw... stuff...
                //create a new graphics path.
                GraphicsPath drawtextpath = new GraphicsPath();
                //add the text 
                drawtextpath.AddString(forlevel.LevelName, intfont, new Point(0, 0),
                                       StringFormat.GenericTypographic);
                gotgraphics.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
               
                gotgraphics.Clear(Color.Transparent);
                //gotgraphics.DrawRectangle(new Pen(Color.Black, 5), new Rectangle(0, 0, (int)measureit.Width, (int)measureit.Height));
                //gotgraphics.FillRectangle(new SolidBrush(Color.Red), 0, 0, createme.Width, createme.Height);
                gotgraphics.FillPath(new SolidBrush(forlevel.LevelNameIntroTextFillColor), drawtextpath);
                gotgraphics.DrawPath(new Pen(forlevel.LevelNameIntroTextPenColor, 1), drawtextpath);

            }
            else
            {
                //create blank 1x1 intro bitmap.
                createme = new Bitmap(1, 1);
                gotgraphics = Graphics.FromImage(createme);

            }
            thebitmap = createme;
            
            thegraphics = gotgraphics;

        }



        //public Type[] PowerUps = new Type[] {typeof(PaddlePlusPowerup),typeof(PaddleMinusPowerup)};
        //public float[] PowerUpsChance = new float[] {1,1}; //currently, the chance for all powerups are equal.
 


        /// <summary>
        /// Calculates the "par" time that would be about average for beating the level. uses the TimeScore value of all visible blocks.
        /// </summary>
        /// <returns></returns>
        public TimeSpan CalculateParTime()
        {

            TimeSpan TotalSum=new TimeSpan();


            foreach (Block iterateblock in levelblocks)
            {

                TotalSum += iterateblock.TimeScore;


            }

            return TotalSum;



        }


        
        private BCBlockGameState.DeferredLevelLoadProc<Level> DeferredLoader = null;
        private bool isloaded = false;
        private void EnsureLoaded()
        {
            if (DeferredLoader != null)
            {
                DeferredLoader(this);
                isloaded = true;
            }


        }


        #region ISerializable Members
        public Level()
        {
            Background = new BackgroundColourImageDrawer(Color.Gray);
            TallyMusicName = "TALLYMUSIC";
            TallyTickSound = "TALLYTICK";
            GameOverMusic = "GAMEOVER";
            MusicName = "BASESTOMP";
            IntroMusicName = "ARKVC";
            PauseSound = "PAUSE";
            _MaxBalls = 250;
            LevelNameIntroTextFillColor=Color.White;
            LevelNameIntroTextPenColor=Color.Black;
            LevelnameIntroFont = new Font(BCBlockGameState.GetMonospaceFont(), 48);
            isloaded = true; //we aren't deferred....
        }
       
        public Level(BCBlockGameState.DeferredLevelLoadProc<Level> loadingproc)
        {
            isloaded = false;
            DeferredLoader = loadingproc;


        }
        public XElement GetXmlData(String pNodeName)
        {
            XElement result = new XElement(pNodeName);
            result.Add(new XAttribute("MusicName",MusicName));
            result.Add(new XAttribute("Levelname",LevelName));
            result.Add(new XAttribute("MaxBalls",MaxBalls));
            result.Add(StandardHelper.SaveElement<Color>(LevelNameIntroTextFillColor,"LevelNameIntroTextFillColor"));
            result.Add(StandardHelper.SaveElement<Color>(LevelNameIntroTextPenColor, "LevelNameIntroTextPenColor"));
            result.Add(StandardHelper.SaveElement<Font>(LevelnameIntroFont, "LevelNameIntroFont"));
            result.Add(new XAttribute("TallyMusic",TallyMusicName));
            result.Add(new XAttribute("TallyTickSound", TallyTickSound));
            result.Add(new XAttribute("TallyPicKey", TallyPicKey));
            result.Add(new XAttribute("GameOverMusic", GameOverMusic));
            result.Add(new XAttribute("IntroMusicName", IntroMusicName));
            result.Add(new XAttribute("NextLevel", NextLevel));
            result.Add(StandardHelper.SaveElement<TimeSpan>(ShowNameLength,"ShowNameLength"));
            result.Add(StandardHelper.SaveList(levelblocks,"Blocks"));
            result.Add(StandardHelper.SaveList(levelballs, "Balls"));
            result.Add(new XAttribute("NoPaddle",NoPaddle));
            result.Add(new XAttribute("PauseSound", PauseSound));
            result.Add(new XAttribute("DeathSound", DeathSound));
            result.Add(StandardHelper.SaveArray(_SidebarColorMatrixValues,"SideBarColorMatrix"));
            result.Add(new XAttribute("SidebarImageKey",_SidebarImageKey));
            result.Add(StandardHelper.SaveElement(SidebarTextColor,"SidebarTextColor") );
            result.Add(new XAttribute("PauseImageKey",_PauseImageKey));
            result.Add(StandardHelper.SaveArray(_PauseColorMatrixValues,"PauseColorMatrix"));
            result.Add(StandardHelper.SaveElement(_PauseTextColor,"PauseTextColor"));
            result.Add(StandardHelper.SaveElement(PauseFont,"PauseFont"));
            result.Add(StandardHelper.SaveList(LevelEvents,"LevelEvents"));
            List<String> AvailablePowers = BCBlockGameState.TypesToString(_AvailablePowerups).ToList();
            result.Add(StandardHelper.SaveList(AvailablePowers,"AvailablePowerups"));
            result.Add(StandardHelper.SaveElement(MessageData,"MessageData"));
            result.Add(StandardHelper.SaveElement(Background,"Background"));
            result.Add(new XAttribute("ClearTitle",ClearTitle));

            return result;
        }
        public Level(XElement SourceNode)
        {
            isloaded = true;
            Debug.Print("cLevel XML constructor");
            MusicName = SourceNode.GetAttributeString("MusicName",null);
            LevelName = SourceNode.GetAttributeString("LevelName", "Default");
            Description = SourceNode.GetAttributeString("Description", "Default");
            MaxBalls = SourceNode.GetAttributeInt("MaxBalls",-1);
            LevelNameIntroTextFillColor = SourceNode.ReadElement("LevelNameIntroTextFillColor", Color.Black);
            LevelNameIntroTextPenColor = SourceNode.ReadElement("LevelnameIntroTextPenColor", Color.DodgerBlue);
            _LevelNameIntroFont = SourceNode.ReadElement("LevelNameIntroFont", new Font(BCBlockGameState.GetMonospaceFont(),18));
            TallyMusicName = SourceNode.GetAttributeString("TallyMusic");
            TallyTickSound = SourceNode.GetAttributeString("TallyTickSound");
            TallyPicKey = SourceNode.GetAttributeString("TallyPicKey");
            GameOverMusic = SourceNode.GetAttributeString("GameOverMusic");
            GameOverPicKey = SourceNode.GetAttributeString("GameOverPicKey");
            IntroMusicName = SourceNode.GetAttributeString("IntroMusicName");
            NextLevel = SourceNode.GetAttributeInt("NextLevel", -1);
            ShowNameLength = SourceNode.ReadElement<TimeSpan>("ShowNameLength");
            levelblocks = StandardHelper.ReadList<Block>(SourceNode.Element("Blocks"));
            levelballs = StandardHelper.ReadList<cBall>(SourceNode.Element("Balls"));
            NoPaddle = SourceNode.GetAttributeBool("NoPaddle", false);
            PauseSound = SourceNode.GetAttributeString("PauseSound");
            StartTrigger = SourceNode.GetAttributeInt("StartTrigger");
            DeathSound = SourceNode.GetAttributeString("DeathSound","mmdeath");
            _SidebarColorMatrixValues = (float[][])SourceNode.ReadArray<float>("SideBarColorMatrix", null);
            SidebarImageKey = SourceNode.GetAttributeString("SidebarImageKey");
            SidebarTextColor = SourceNode.ReadElement<Color>("SidebarTextColor");
            PauseImageKey = SourceNode.GetAttributeString("PauseImageKey");
            _PauseColorMatrixValues = (float[][])SourceNode.ReadArray<float>("PauseColorMatrix", null);
            PauseTextColor = SourceNode.ReadElement<Color>("PauseTextColor");
            PauseFont = SourceNode.ReadElement<Font>("PauseFont");
            LevelEvents = SourceNode.ReadList<TriggerEvent>("LevelEvents", null);
            try
            {
                _AvailablePowerups = BCBlockGameState.StringToTypes(SourceNode.ReadList<String>("AvailablePowerups")).ToList();
            }
            catch { _AvailablePowerups = GamePowerUp.GetPowerUpTypes().ToList(); }

            MessageData = SourceNode.ReadList<PlayMessageData>("MessageData");

            try
            {
                Background = SourceNode.ReadElement<BackgroundColourImageDrawer>("Background", new BackgroundColourImageDrawer(Color.White));
            }
            catch
            {
                Background = new BackgroundColourImageDrawer(Color.Gray);
            }

            ClearTitle = SourceNode.GetAttributeString("ClearTitle", "       SCORE     \n");

        }
      
        public Level(SerializationInfo info, StreamingContext context):this()
        {
            FromSerializer(info, context);
        }
        internal void FromSerializer(SerializationInfo info, StreamingContext context)
        {
            isloaded = true;
            Debug.Print("cLevel Serialization constructor");
            //each is wrapped with a try; any element causing an exception is ignored. Usually, an exception would occur if an item was missing.
            //SerializableImage grabimage = (SerializableImage)info.GetValue("BackgroundPic", typeof(SerializableImage));
            try { MusicName = info.GetString("MusicName"); } catch { }
            try { LevelName = info.GetString("LevelName"); } catch { }
            try { Description = info.GetString("Description"); }catch { }
            try { MaxBalls = info.GetInt32("MaxBalls"); }catch { }
            try { LevelNameIntroTextFillColor = (Color)info.GetValue("LevelNameIntroTextFillColor", typeof(Color)); } catch { }
            try { LevelNameIntroTextPenColor = (Color)info.GetValue("LevelNameIntroTextPenColor", typeof(Color)); } catch { }
            try { LevelnameIntroFont = (Font)info.GetValue("LevelNameIntroFont", typeof(Font)); } catch { }
            try { TallyMusicName = info.GetString("TallyMusic"); } catch { }
            try { TallyTickSound = info.GetString("TallyTickSound"); } catch { }
            try { TallyPicKey = info.GetString("TallyPicKey"); } catch { }
            try { GameOverMusic = info.GetString("GameOverMusic"); } catch { }
            try { GameOverPicKey = info.GetString("GameOverPicKey");}catch{ }
            try { IntroMusicName = info.GetString("IntroMusicName"); } catch { }
            try { NextLevel = info.GetInt32("NextLevel"); } catch { }
            try { ShowNameLength = (TimeSpan)info.GetValue("ShowNameLength", typeof(TimeSpan)); } catch { }
            try { levelblocks = (List<Block>)info.GetValue("Blocks", typeof(List<Block>)); } catch { }
            try { levelballs = (List<cBall>)info.GetValue("Balls", typeof(List<cBall>)); } catch { }
            try { NoPaddle = info.GetBoolean("NoPaddle"); } catch { }
            try { PauseSound = info.GetString("PauseSound"); } catch { }
            try { StartTrigger = info.GetInt32("StartTrigger"); } catch { }
            try { DeathSound = info.GetString("DeathSound"); } catch { DeathSound = "mmdeath"; }
            try { _SidebarColorMatrixValues = (float[][])info.GetValue("SideBarColorMatrix", typeof(float[][])); } catch { }
            try { _SidebarImageKey = info.GetString("SidebarImageKey"); } catch { }
            try { _SidebarTextColor = (Color)info.GetValue("SidebarTextColor", typeof(Color)); } catch { }
            try { _PauseImageKey = info.GetString("PauseImageKey"); } catch { }
            try { _PauseColorMatrixValues = (float[][])info.GetValue("PauseColorMatrix", typeof(float[][])); } catch { }
            try { _PauseTextColor = (Color)info.GetValue("PauseTextColor", typeof(Color)); } catch { }
            try { _PauseFont = (Font)info.GetValue("PauseFont", typeof(Font)); } catch { }
            try { LevelEvents = (List<TriggerEvent>)info.GetValue("LevelEvents", typeof(List<TriggerEvent>)); }catch { }
            try
            {
                //info.AddValue("AvailablePowerups", BCBlockGameState.TypesToString(_AvailablePowerups).ToList());
                _AvailablePowerups = BCBlockGameState.StringToTypes((List<String>)info.GetValue("AvailablePowerups", typeof(List<String>))).ToList();

            }
            catch { _AvailablePowerups = GamePowerUp.GetPowerUpTypes().ToList(); }
            try
            {
                MessageData =
                    (List<PlayMessageData>)
                    info.GetValue("MessageData", typeof(List<PlayMessageData>));
            }
            catch
            {
            } //empty catch...
            
            //try{LevelImage = info.getImage("LevelImage");}catch{}
            //try { BackgroundColor = (Color)info.GetValue("BackgroundColor", typeof(Color)); }catch{}
            //try { BackgroundPicKey = info.GetString("BackgroundPic"); }catch {}
            try
            {

                Background = (BackgroundColourImageDrawer)info.GetValue("Background", typeof(BackgroundColourImageDrawer));
            }
            catch
            {
                Background = new BackgroundColourImageDrawer(Color.Gray);
            }
            try { _ClearTitle = info.GetString("ClearTitle"); }
            catch { _ClearTitle = "       SCORE     \n"; }


        }
        public byte[] getserialbytes()
        {
            //simply serializes this object and returns the byte from that array.
            MemoryStream ms = new MemoryStream();
            IFormatter bf = BCBlockGameState.getFormatter<Level>(BCBlockGameState.DataSaveFormats.Format_Binary);
            bf.Serialize(ms, this);
            //long locate = ms.Position;
            ms.Seek(0, SeekOrigin.Begin);
            byte[] returnit = new byte[(int)ms.Length];

            ms.Read(returnit, 0, (int)ms.Length);

            return returnit;

        }

        public String MD5Hash()
        {
            MD5CryptoServiceProvider MD5obj = new MD5CryptoServiceProvider();


            byte[] bytes = getserialbytes();
            byte[] bs = MD5obj.ComputeHash(bytes);
            StringBuilder sb = new StringBuilder();
            foreach (byte b in bs)
            {
                sb.Append(b.ToString("x2").ToLower());
            }

            return sb.ToString();


        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
            info.AddValue("MusicName", MusicName);
            info.AddValue("LevelNameIntroTextFillColor", LevelNameIntroTextFillColor);
            info.AddValue("LevelNameIntroTextPenColor", LevelNameIntroTextPenColor);
            info.AddValue("LevelNameIntroFont", LevelnameIntroFont);
            info.AddValue("LevelName", LevelName);
            info.AddValue("Description", Description);
            info.AddValue("ShowNameLength", ShowNameLength);
            info.AddValue("TallyMusic", TallyMusicName);
            info.AddValue("TallyTickSound", TallyTickSound);
            info.AddValue("TallyPicKey", TallyPicKey);
            info.AddValue("PauseSound", PauseSound);
            info.AddValue("GameOverMusic", GameOverMusic);
            info.AddValue("IntroMusicName", IntroMusicName);
            info.AddValue("MaxBalls", MaxBalls);
            info.AddValue("NextLevel", NextLevel);
            info.AddValue("Blocks",levelblocks);
            info.AddValue("Balls", levelballs);
            info.AddValue("NoPaddle", NoPaddle);
            //info.AddValue("BackgroundDrawType", Background.GetType().FullName);
            info.AddValue("Background", Background);
            info.AddValue("StartTrigger", StartTrigger);
            info.AddValue("DeathSound", DeathSound);
            info.AddValue("SideBarColorMatrix", _SidebarColorMatrixValues);
            info.AddValue("SidebarImageKey", SidebarImageKey);
            info.AddValue("SidebarTextColor", _SidebarTextColor);
            info.AddValue("PauseColorMatrix", _PauseColorMatrixValues);
            info.AddValue("PauseImageKey", _PauseImageKey);
            info.AddValue("GameOverPicKey", _GameOverPicKey);
            info.AddValue("PauseTextColor", _PauseTextColor);
            info.AddValue("PauseFont", _PauseFont);
            info.AddValue("MessageData", _MessageData);
            info.AddValue("LevelEvents", LevelEvents);
            info.AddValue("AvailablePowerups", BCBlockGameState.TypesToString(_AvailablePowerups).ToList());
            info.AddValue("ClearTitle", _ClearTitle);
            //info.AddValue("BackgroundColor", BackgroundColor);
   
            Image levelimage = BCBlockGameState.DrawLevelToImage(this);
            //use the extension method.
            //info.AddValue("LevelImage", levelimage);

            

            
        }

        #endregion

        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            InitMessageStack();
            
        }

        #endregion
    }
}
