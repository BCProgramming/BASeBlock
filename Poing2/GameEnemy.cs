using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Imaging;
using System.Linq;
using System.Net.Sockets;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.Serialization;
using System.ComponentModel;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Events;
using BASeCamp.BASeBlock.Particles;
using BASeCamp.BASeBlock.Projectiles;

namespace BASeCamp.BASeBlock
{

    public class NoKillIncrementAttribute : System.Attribute
    {


    }
    /// <summary>
    /// Attribute that forces a class deriving from GameEnemy to Increment the Statistics kill counter when it dies.
    /// this cannot be overridden (to prevent kill incrementing)
    /// </summary>
    public class KillIncrementAttribute : System.Attribute
    {



    }
    /// <summary>
    /// GameEnemy: derives from GameObject, and provides the core logic for all Enemies that will be created.
    /// </summary>
    
    public abstract class GameEnemy : GameObject,iLocatable ,IExplodable
    {
        /*many enemies will be implemented using different "states"; that is, an enemy may have one set of animation frames
        for when it is "idle", and other animation frames for Attacking and so forth. We keep track of several things to help in this:
        
         * 1. A Dictionary that stores Arrays of Image KEYS (we don't want to duplicate image data across multiple instances) which can be indexed
         * based on the "EnemyState" string value, which, ideally, could be an enumeration, but different enemies may have any number of states and artificially limiting them seems
         * a bit premature- so we use a string.

        */
        protected int _HitPoints = 50;
        protected int _MaxHitPoints = 50;
        public int HitPoints { get { return _HitPoints; }
            set { _HitPoints = value; if (_HitPoints > _MaxHitPoints) _MaxHitPoints = value; }
        }
        public int MaxHitPoints { get { return _MaxHitPoints; } set { _MaxHitPoints = value; } }
        [Flags]
        public enum AutoSizeModeConstants
        {
            AutoSize_None, //default, no sizing will be done as frames change.
            AutoSize_ExpandTranslate, //expand to the left or up to preserve frame X scale.
            AutoSize_ExpandUp, //expand width or height to preserve frame Y scale.
            AutoSize_ExpandCenter //expansion will center the axis in the same location the center sits in the current frame.

        }

        public class FrameSizeMode
        {
            public AutoSizeModeConstants XSizeMode { get; set; }
            public AutoSizeModeConstants YSizeMode { get; set; }
            public FrameSizeMode(AutoSizeModeConstants pXSizeMode, AutoSizeModeConstants pYSizeMode)
            {
                XSizeMode = pXSizeMode;
                YSizeMode = pYSizeMode;

            }
            /// <summary>
            /// given our scalemode, location, and the current and new size, changes Location and currsize
            /// to scale appropriate to newsize.
            /// </summary>
            /// <param name="sizemode"></param>
            /// <param name="Location"></param>
            /// <param name="currsize"></param>
            /// <param name="newsize"></param>
            
            public void Rescale(ref PointF Location, ref SizeF currsize, SizeF newsize)
            {
                
                



            }
            private void RescaleAxis(AutoSizeModeConstants smode,ref float value, ref float Currsize, ref float newsize)
            {
                switch (smode)
                {

                    case AutoSizeModeConstants.AutoSize_None:
                        break; //nothing...
                    case AutoSizeModeConstants.AutoSize_ExpandTranslate:
                        //move value so that the the "end" of the size will be the same. (so the right or bottom remains unchanged)
                        value = (value + Currsize) - newsize;
                        Currsize = newsize;
                        break;
                    case AutoSizeModeConstants.AutoSize_ExpandUp:
                        //expand outwards...
                        Currsize = newsize;
                        break;
                    case AutoSizeModeConstants.AutoSize_ExpandCenter:
                        value = value + ((value + Currsize / 2) - (value + newsize / 2));
                        break;
                }
                    




            }


        }
        private FrameSizeMode _FramingSizeMode = new FrameSizeMode(AutoSizeModeConstants.AutoSize_None, AutoSizeModeConstants.AutoSize_None);

        public FrameSizeMode FramingSizeMode { get { return _FramingSizeMode; } set { _FramingSizeMode = value; } }


        private List<EnemyTrigger> _Triggers = new List<EnemyTrigger>();

        /// <summary>
        /// Trigger that get's invoked for certain circumstances
        /// </summary>
        public List<EnemyTrigger> Triggers {get { return _Triggers; }



            set { 
                _Triggers = value;
                if (_Triggers != null)
                {
                    foreach (var looptrigger in _Triggers)
                    {
                        looptrigger.OurEnemy=this;


                    }


                }




            }



        } //Trigger to invoke when this enemy dies.
        //public Trigger TriggerInvoke { get; set; }

        public ImageAttributes DrawAttributes=null; 
        //Enemy State:
        protected SizeF? _DrawSize=null; //if null, will use default. This is a float, and represents the % size we should use of the 

        public virtual SizeF DrawSize { 
            get
            {
                if (_DrawSize == null) return this.GetCurrentImage().Size;
                else return _DrawSize.Value;
            }
            set { _DrawSize = value; }}

        //current frame image.

        
        public event EventHandler<EnemyDeathEventArgs> OnDeath;
        
        private struct deathtriggerdata
        {
            public GameEnemy theenemy;
            public BCBlockGameState stateobj;
            public String SoundPlay;
            public List<EnemyTrigger> Triggers;

        }

        public override string ToString()
        {
            String buildit = "GameEnemy { \n" +
                "Location:" + this.Location.ToString() + "\n" +
                "Size:" + this.DrawSize.ToString() + "\n" +
                "State:" + this.EnemyState.ToString() + "\n" +
                "Hitpoints:" + this.HitPoints.ToString() + "\n" +
                "maxHP:" + this.MaxHitPoints.ToString() + "\n";
                
                buildit+="Triggers {\n";
                foreach(var iterate in Triggers)
                {
                    buildit+=iterate.ToString() + "\n";

                    buildit+="----\n";
                }
                buildit+="\n";

                return buildit;
        }

        public virtual void ExplosionInteract(object sender, PointF Origin, PointF Vector)
        {
            HitPoints -= (int)Vector.Magnitude();
        }

        //System.Threading.Timer usetimer;
        System.Threading.Thread deaththread;
        DateTime InitialDeath;
        private void CalledOnDeath(Object sender, EnemyDeathEventArgs e)
        {
            if (e.EnemyDied is ChomperEnemy)
            {
                Debug.Print("Chomper...");

            }
            if (Triggers != null)
            {
                Debug.Print("Enemy killed; invoking triggers.");
                foreach (EnemyTrigger looptrigger in Triggers)
                {
                    looptrigger.InvokeTriggerWithDelay(e.StateObject);


                }


            }


            /*if (Triggers != null)
                {
                    Debug.Print("OH NOES an enemy died, -invoking triggers");
                    foreach (EnemyTrigger looptrigger in Triggers)
                    {
                        looptrigger.InvokeTriggerWithDelay(stateobject);


                    }
                    /*
                    deathtriggerdata datatriggers = new deathtriggerdata();
                    datatriggers.theenemy = enemydied;
                    datatriggers.stateobj = stateobject;
                    datatriggers.SoundPlay = "puzzle";
                    //Debug.Print("assigning datatriggers TriggerID of " + Trigger);
                    datatriggers.Triggers= Triggers;
                    
                    //usetimer = new Timer(deathtriggertimeout,(object)datatriggers,0,500);
                    deaththread = new Thread(deathtriggertimeout);
                    InitialDeath = DateTime.Now;
                    deaththread.Start((object) datatriggers);*/
            //}
                
            //assuming that works, we would now destroy all blocks in the level that have the same TriggerID as us.

        }
        /*
            private void deathtriggertimeout(object parameter)
            {
                Debug.Print("deathtriggertimeout");
                while ((DateTime.Now - InitialDeath).Seconds < 2)
                {
                    Thread.Sleep(100);

                }
                Debug.Print("timeout expired...");
                deathtriggerdata dtd = (deathtriggerdata)parameter;
                //destroy all blocks with the same ID.
               // Debug.Print("TriggerID=" + dtd.TriggerID);
                List<Block> removethese = new List<Block>();
                if(!String.IsNullOrEmpty(dtd.SoundPlay))
                    BCBlockGameState.Soundman.PlaySound(dtd.SoundPlay,3.0f);
                lock (dtd.stateobj )
                {
                    if (dtd.TriggerObj != null)
                    {
                        //Debug.Print("Total number of blocks with triggerID of " + dtd.TriggerID + (from b in dtd.stateobj.Blocks where b.TriggerID == dtd.TriggerID select b).Count().ToString());



                        //foreach (
                        //    Block loopblock in
                        //        (from b in dtd.stateobj.Blocks where b.TriggerID == dtd.TriggerID select b))
                        /*
                        foreach (Block b in dtd.stateobj.Blocks)
                        {
                            if (b.TriggerID == TriggerID)
                            {
                                Debug.Print("kerploding a block...");
                                b.StandardSpray(dtd.stateobj);
                                removethese.Add(b);


                            }
                        }
                        foreach (Block removeblock in removethese)
                        {
                            dtd.stateobj.Blocks.Remove(removeblock);


                        }
                         * 

                        dtd.TriggerObj.InvokeTrigger(dtd.stateobj);

                        dtd.stateobj.Forcerefresh = true;
                    }
                }

                
            }
*/
        public virtual int GetScoreValue()
        {

            return 100;
        }
        private bool AllowKillIncrement()
        {

            return !(BCBlockGameState.HasAttribute(this.GetType(), typeof(NoKillIncrementAttribute))) ||
                BCBlockGameState.HasAttribute(this.GetType(),typeof(NoKillIncrementAttribute));


        }
        internal void InvokeOnDeath(BCBlockGameState stateobject,ref List<GameObject> AddObjects,ref List<GameObject> removeobjects)
        {
            var gotset = stateobject.ClientObject.GetPlayingSet();
            if (gotset !=null && AllowKillIncrement())
            {
                gotset.Statistics.EnemyKills++;
                
            }


            int gotvalue = GetScoreValue();
            if(gotvalue != 0)
                Block.AddScore(stateobject, GetScoreValue(), Location);
            
            var deathproc = OnDeath;
            if (deathproc != null)
            {
                deathproc(this, new EnemyDeathEventArgs(this, stateobject));
                

            }

        }

        public SizeF GetSize()
        {
            return DrawSize;

        }
        //public event Action<String, String> EnemyStateChanged;
        public event EventHandler<EnemyStateChangeEventArgs> EnemyStateChanged;
        private void RaiseEnemyStateChange(String oldstate, String newstate)
        {
            Debug.Print("Enemy State changed:" + oldstate + " to " + newstate);
            var invokethis = EnemyStateChanged;
            if (invokethis != null)
            {
                EnemyStateChangeEventArgs esc = new EnemyStateChangeEventArgs(oldstate, newstate);
                invokethis(this, esc);



            }

        }



        public String EnemyAction
        {
            set
            {
                if (_ActionIndex == null) InitActions();
                //map backwards...
                foreach (var iterate in _ActionIndex)
                {

                    if (iterate.Value.Equals(value, StringComparison.OrdinalIgnoreCase))
                        EnemyState = iterate.Key;
                }

            }
            get
            {
                if (_ActionIndex == null) InitActions();
                return GetAction(_EnemyState);
            }


        }
        protected void InitActions()
        {
            _ActionIndex = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            foreach (var kvp in StateFrameImageKeys)
            {
                _ActionIndex.Add(kvp.Key, kvp.Key);

            }

        }
        protected String _EnemyState = "idle";
        public String EnemyState
        {
            get { return _EnemyState; }
            set
            {
                if (_EnemyState != value)
                {
                    RaiseEnemyStateChange(_EnemyState, value);
                    _EnemyState = value;

                }
            }


        }
        protected int mDefaultFrameDelay=50;
        protected PointF _Location;

        public virtual PointF Location { get { return _Location; } set { _Location = value; } }

        protected float DrawRotation; //rotation to draw...
        //dictionary of Image keys...
        protected Dictionary<String, String[]> _StateFrameImageKeys = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);

        //a similar dictionary, which maps a state to an action. By default, each state is it's own action.
        //this is added to allow for different states for the images (such as jumping up and down) while having the
        //logic be able to deal with it in the same manner.
        protected Dictionary<String, String> _ActionIndex = null;

        public static GameEnemy CreateBoss<T>(PointF location, BCBlockGameState gstate) where T : GameEnemy
        {
            var resultvalue = CreateBoss<T>(location, gstate, (g) => g.OnDeath += bossdeath,new TimeSpan(0,0,0,3));
            return resultvalue;
        }
        public static void bossdeath(Object sender,EnemyDeathEventArgs e)
        {
            createenemy_OnDeath(sender, e);
        }
        public static GameEnemy CreateBoss<T>(PointF location, BCBlockGameState gstate,Action<T> onDeath,TimeSpan DeathDelay) where T:GameEnemy
        {
            
            GameEnemy createenemy = null;
            Type createtype = typeof(T);
            MethodInfo usemethod = null;
            foreach (var iterate in createtype.GetMethods())
            {
                if (iterate.Name.Equals("CreateBoss", StringComparison.OrdinalIgnoreCase) && iterate.IsStatic)
                {
                    usemethod = iterate;


                }

            }
            try
            {
                if (usemethod != null)
                {
                    createenemy= (GameEnemy)usemethod.Invoke(null,new object[] {location,gstate});

                }
            }
            catch(Exception exx)
            {
                Debug.Print(exx.ToString());

            }
            if (createenemy != null)
            {
                //int useChannel = Trigger.GetAvailableID(gstate);
                //createenemy.Triggers.Add(new EnemyTrigger(createenemy, useChannel, new TimeSpan(0, 0, 0, 0, 500)));
                gstate.Blocks.AddLast(new NormalBlock(new RectangleF(-500, -500, 32, 16)));
                BCBlockGameState.Soundman.PlayTemporaryMusic("smb2boss",1.0f,true);
                //gstate.PlayingLevel.LevelEvents.Add(new FinishLevelEvent(useChannel));
                gstate.PlayingLevel.TallyMusicName = "bossclear";
                gstate.PlayingLevel.ClearTitle = "   BOSS CLEAR     \n";
                gstate.BossCounter++; //add one to the boss counter.
                //show a message expressing the gravity of the situation.
                gstate.EnqueueMessage(createenemy.GetType().Name + "Boss Has awoken!");


                createenemy.OnDeath += createenemy_OnDeath;
                return createenemy;
            }
            return null;


        }

        static void createenemy_OnDeath(Object sender, EnemyDeathEventArgs e)
        {
            e.StateObject.EnqueueMessage(e.EnemyDied.GetType().Name + " Boss defeated! Well Done!");
            BCBlockGameState.Soundman.StopMusic();
            BCBlockGameState.Soundman.PlaySound("BOSSKILL");
            var currset = e.StateObject.ClientObject.GetPlayingSet();
            if (currset != null)
            {
                currset.Statistics.BossKills++;

            }
            e.StateObject.BossCounter--;
        }

       
        //protected int FrameDelay = 35; //delay between frame changes.

        protected Dictionary<String, int[]> FrameDelayTimes = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
        private T[] CreateArray<T>(T valueassign,int number)
        {
            T[] createarray = new T[number];
            for (int i = 0; i < createarray.Length;i++ )
            {
                createarray[i] = valueassign;
            }

            return createarray;
        }
        
        public Dictionary<String, String[]> StateFrameImageKeys
        {
            get {
                return _StateFrameImageKeys;
            
            }
            set {
                _StateFrameImageKeys=value;
                if (_StateFrameImageKeys == null) return;
                StateFrameIndex.Clear();
                FrameDelayTimes.Clear();
                foreach (KeyValuePair<String, String[]> kvp in _StateFrameImageKeys)
                {
                    StateFrameIndex.Add(kvp.Key, 0);

                    FrameDelayTimes.Add(kvp.Key, CreateArray<int>(50, kvp.Value.Length));

                }
            
            }


        }
        protected Dictionary<String, int> StateFrameIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        protected GameEnemy(PointF Position, Dictionary<String, String[]> StateFrameData, int FrameDelay)
            : this(Position, StateFrameData, FrameDelay, null)
        {

            OnDeath = CalledOnDeath;

        }
        protected String GetAction(String state)
        {

            return _ActionIndex[state];

        }
        protected GameEnemy(PointF Position, Dictionary<String, String[]> StateFrameData,int FrameDelay,ImageAttributes useattributes)
        {
            OnDeath = CalledOnDeath;
            DrawAttributes=useattributes;
            Location=Position;
            mDefaultFrameDelay=FrameDelay;
            StateFrameImageKeys = StateFrameData;
            


            if(StateFrameData!=null)
            {
                _ActionIndex = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (KeyValuePair<String, String[]> kvp in StateFrameData)
                {
                    if (StateFrameIndex.ContainsKey(kvp.Key))
                    {
                        StateFrameIndex[kvp.Key] = 0;
                        FrameDelayTimes[kvp.Key] = CreateArray(FrameDelay, kvp.Value.Length);
                    }
                    else
                    {
                        StateFrameIndex.Add(kvp.Key, 0);
                        FrameDelayTimes.Add(kvp.Key, CreateArray(FrameDelay, kvp.Value.Length));
                    }
                    _ActionIndex.Add(kvp.Key, kvp.Key);
                }
            }
            
        }



        public virtual Image GetCurrentImage()
        {
            if (StateFrameImageKeys == null) return null;
            try
            {
                return BCBlockGameState.Imageman.getLoadedImage(StateFrameImageKeys[_EnemyState][StateFrameIndex[_EnemyState]]);
            }
            catch
            {
                //do nothin
                return null;
            }

        }
        public RectangleF GetRectangleF()
        {
            if (DrawSize != null)
                return new RectangleF(Location.X, Location.Y, DrawSize.Width, DrawSize.Height);
            else
            {
                Image grabimage = GetCurrentImage();
                return new RectangleF(Location.X,Location.Y,grabimage.Width,grabimage.Height);



            }



        }
        public void setRectangleF(RectangleF newrect)
        {
            Location = new PointF(newrect.Left, newrect.Top);
            DrawSize = new SizeF(newrect.Width, newrect.Height);


        }
        public void setRectangle(Rectangle newrect)
        {
            RectangleF convrect = newrect.ToRectangleF();
            setRectangleF(convrect);


        }
        public Rectangle GetRectangle()
        {
            RectangleF convrect = GetRectangleF();
            return new Rectangle((int)convrect.Left,(int)convrect.Top,(int)convrect.Width,(int)convrect.Height);


        }
        /// <summary>
        /// called when a set of frames is about to loop around.
        /// </summary>

        public virtual void OnFramesetComplete(BCBlockGameState gamestate)
        {




        } //called when a set of frames loops back to 0.
        
        

       

        /// <summary>
        /// increments the frame number of the currently selected list of state images.
        /// </summary>
        /// 

        private int GetFrameDelayTime()
        {
            //if the FrameDelayTimes Dictionary is null- or if it doesn't extend far enough to cover the current frame...
            
            /*if (FrameDelayTimes == null || FrameDelayTimes.Count < StateFrameIndex[_EnemyState])
            {
                //return a default value.
                return mDefaultFrameDelay;
            }*/
            //otherwise, retrieve the appropriate delay time based on this character/enemies current state and current frame.
            try
            {
                return FrameDelayTimes[_EnemyState][StateFrameIndex[_EnemyState]];
            }
            catch
            {

                return mDefaultFrameDelay;
            }

        }

        int FrameDelayCounter = 0;
        private void IncrementImageFrame(BCBlockGameState gamestate)
        {
            FrameDelayCounter++;
            if (FrameDelayCounter > GetFrameDelayTime())
            {
                Trace.WriteLine("FrameSet:" + _EnemyState);
                OnFramesetComplete(gamestate);
                FrameDelayCounter = 0;
                int currframe = StateFrameIndex[_EnemyState];
                int numframes = StateFrameImageKeys[_EnemyState].Length - 1;
                if (currframe >= numframes)
                    currframe = 0;
                else
                    currframe++;

                StateFrameIndex[_EnemyState] = currframe;
            }
        }
        
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            IncrementImageFrame(gamestate);
            //throw new NotImplementedException();
            //return _HitPoints <= 0;
            return false;
        }
        public virtual Image getIcon()
        {
            Bitmap buildbit = new Bitmap((int)DrawSize.Width, (int)DrawSize.Height);
            
                
                Graphics g = Graphics.FromImage(buildbit);
                //translate to 0,0.
                g.TranslateTransform(-(Location.X), -(Location.Y));
                Draw(g);
            return buildbit;    
            
        }
        public override void Draw(Graphics g)
        {
            Image currimage = GetCurrentImage();
            if (currimage == null)
            {
                Debug.Print("BREAK");

            }
            var copyt = g.Transform.Clone();
            PointF DrawLocation = Location;
            if (Math.Abs(DrawRotation) > 0.1f)
            {
                 

                g.TranslateTransform(DrawSize.Width / 2 + DrawLocation.X, DrawSize.Height / 2 + DrawLocation.Y);
                g.RotateTransform(DrawRotation);
                g.TranslateTransform(-DrawSize.Width / 2, -DrawSize.Height / 2);
                DrawLocation = new PointF(0, 0);
            }
            

            if (DrawSize != null)
            {
                if (DrawAttributes == null)
                {
                    //g.DrawImage(GetCurrentImage(), Location.X, Location.Y, DrawSize.Value.Width, DrawSize.Value.Height);
                    g.DrawImage(GetCurrentImage(), DrawLocation.X, DrawLocation.Y, DrawSize.Width, DrawSize.Height);
                }
                else
                {
                 
                    //g.DrawImage(BlockImage, new Rectangle((int)BlockRectangle.Left, (int)BlockRectangle.Top, (int)BlockRectangle.Width, (int)BlockRectangle.Height), 0f, 0f, BlockImage.Width, BlockImage.Height, GraphicsUnit.Pixel, DrawAttributes);
                    g.DrawImage(currimage, new Rectangle((int)DrawLocation.X, (int)DrawLocation.Y, (int)DrawSize.Width, (int)DrawSize.Height), 0f, 0f, currimage.Width, currimage.Height, GraphicsUnit.Pixel, DrawAttributes);
                    //g.DrawImage(GetCurrentImage(), Location.X, DrawSize.Value.Width, DrawSize.Value.Height, DrawAttributes);
                }
            }
            else
            {

                if(DrawAttributes==null)
                    g.DrawImage(GetCurrentImage(), DrawLocation);
                else
                    g.DrawImage(currimage, new Rectangle((int)DrawLocation.X, (int)DrawLocation.Y, (int)currimage.Width, (int)currimage.Height), 0f, 0f, currimage.Width, currimage.Height, GraphicsUnit.Pixel, DrawAttributes);
                
            }
            g.Transform=copyt;
            if (this is iImagable)
            {
                //some Enemies implement interfaces that provide location, size, and speed information.
                //we will use this to draw a HP bar above those enemies.
                //this is just a testing feature...
                

                //first, decide where to draw it.
                iImagable refprojectile = this as iImagable;
                var ourrect = refprojectile.getRectangle();
                //3 pixels high.
                Rectangle HPBar = new Rectangle(ourrect.Left - 2, ourrect.Top - 3, ourrect.Width + 4, 2);
                if (!g.ClipBounds.Contains(HPBar))
                    HPBar.Offset(0, ourrect.Height + 3);
                
                float healthpercent = (float)HitPoints / (float)MaxHitPoints;
                Rectangle fullbar = new Rectangle(HPBar.Left, HPBar.Top, (int)(Math.Ceiling((float)HPBar.Width * healthpercent)), HPBar.Height);
                g.FillRectangle(Brushes.White, HPBar);
                using (SolidBrush usebrush = new SolidBrush(BCBlockGameState.MixColor(Color.Red, Color.Lime, (float)HitPoints / (float)MaxHitPoints)))
                {
                    g.FillRectangle(usebrush, fullbar);
                }
                g.DrawRectangle(Pens.Black, HPBar);




            }
           

            //throw new NotImplementedException();
        }
    }
    [Bossable]
    public class SnakeEnemy : GameEnemy, IDeserializationCallback
    {
        public enum SnakeMoveDirection
        {
            SnakeMove_Left,
            SnakeMove_Up,
            SnakeMove_Right,
            SnakeMove_Down





        }
        private SizeF createblockSize = new SizeF(8, 8);
        private DateTime lastcall = DateTime.Now;
        private int FrameDelay = 500;
        private bool killed = false;
        public PointF BlockSpeed = new PointF(0, 1);
        public Type BlockTypeCreate = typeof(DemonBlock);
        public int MaxLength = 25;
        private Block SnakeHead = null;
        private const int gridsize = 15;
        private Queue<Block> snakecomponents = new Queue<Block>();
        public SnakeEnemy(Block[] usesegments, int Milliframedelay)
            : base(usesegments[usesegments.Length - 1].BlockLocation, null, Milliframedelay)
        {
            snakecomponents = new Queue<Block>();
            //element 0 is the tail; the last element is our new "head". as such ,to make our queue, we need to enqueue from the "last" item (the head) to the first item.
            for (int i = 0; i < usesegments.Length; i++)
            {
                snakecomponents.Enqueue(usesegments[i]);
                if (i == usesegments.Length)
                    usesegments[i].OnBlockHit += SnakeHead_OnBlockHit;
                else
                    usesegments[i].OnBlockDestroy += SnakeBody_OnBlockDestroy;








            }
            SnakeHead = usesegments[usesegments.Length - 1];
            BlockTypeCreate = SnakeHead.GetType();



        }
        public SnakeEnemy(PointF pPosition)
            : this(pPosition, 150)
        {
        }

        public SnakeEnemy(PointF pPosition, int MilliFrameDelay)
            : this(pPosition, MilliFrameDelay, typeof(NormalBlock))
        {

        }
        public override int GetScoreValue()
        {
            return 4000;
        }
        public SnakeEnemy(PointF pPosition, int MilliFrameDelay, Type pBlockTypeCreate)
            : base(pPosition, null, MilliFrameDelay)
        {
            if (!pBlockTypeCreate.IsSubclassOf(typeof(Block)))
            {

                throw new ArgumentException("Snakes can only be made of blocks", "pBlockTypeCreate");

            }
            BlockTypeCreate = pBlockTypeCreate;
            FrameDelay = MilliFrameDelay;
            Location = pPosition;


        }
        private PointF[] prospectivedirections = new PointF[] { new PointF(0, 1), new PointF(1, 0), new PointF(0, -1), new PointF(-1, 0) };

        private SnakeMoveDirection GetCurrentDirection()
        {
            if (BlockSpeed.X == 0 && BlockSpeed.Y == 1)
            {
                return SnakeMoveDirection.SnakeMove_Down;


            }
            else if (BlockSpeed.X == 0 && BlockSpeed.Y == -1)
            {
                return SnakeMoveDirection.SnakeMove_Up;
            }
            else if (BlockSpeed.X == -1 && BlockSpeed.Y == 0)
            {
                return SnakeMoveDirection.SnakeMove_Left;
            }
            else if (BlockSpeed.X == 1 && BlockSpeed.Y == 0)
            {
                return SnakeMoveDirection.SnakeMove_Right;
            }
            return SnakeMoveDirection.SnakeMove_Right;

        }
        private int GetWeightForDirection(int[,] gridweights, SnakeMoveDirection direction)
        {
            int midspot = (gridsize / 2);
            int startx = 0, starty = 0, endx = 0, endy = 0;

            switch (direction)
            {
                case SnakeMoveDirection.SnakeMove_Up:
                    startx = 0;
                    endx = gridsize;
                    starty = 0;
                    endy = midspot;
                    break;
                case SnakeMoveDirection.SnakeMove_Left:
                    startx = 0;
                    endx = midspot;
                    starty = 0;
                    endy = gridsize;
                    break;
                case SnakeMoveDirection.SnakeMove_Down:
                    startx = 0;
                    endx = gridsize;
                    starty = gridsize - midspot;
                    endy = gridsize;

                    break;
                case SnakeMoveDirection.SnakeMove_Right:
                    startx = gridsize - midspot;
                    endx = gridsize;
                    starty = 0;
                    endy = gridsize;
                    break;



            }
            int accumtotal = 0;

            for (int loopx = startx; loopx < endx; loopx++)
            {
                for (int loopy = starty; loopy < endy; loopy++)
                {
                    accumtotal += gridweights[loopx, loopy];


                }



            }


            return accumtotal;

        }
        private PointF directionToSpeed(SnakeMoveDirection convertdirection)
        {
            switch (convertdirection)
            {
                case SnakeMoveDirection.SnakeMove_Up:
                    return new PointF(0, -1);

                case SnakeMoveDirection.SnakeMove_Left:
                    return new PointF(-1, 0);
                case SnakeMoveDirection.SnakeMove_Down:
                    return new PointF(0, 1);
                    break;
                case SnakeMoveDirection.SnakeMove_Right:
                    return new PointF(1, 0);
                    break;



            }
            return new PointF(0, 1);



        }

        private SnakeMoveDirection[] GetProspectiveDirections(SnakeMoveDirection currentdirection)
        {
            switch (currentdirection)
            {
                case SnakeMoveDirection.SnakeMove_Up:
                    return new SnakeMoveDirection[] { SnakeMoveDirection.SnakeMove_Left, SnakeMoveDirection.SnakeMove_Up, SnakeMoveDirection.SnakeMove_Right };
                    break;
                case SnakeMoveDirection.SnakeMove_Down:
                    return new SnakeMoveDirection[] { SnakeMoveDirection.SnakeMove_Right, SnakeMoveDirection.SnakeMove_Down, SnakeMoveDirection.SnakeMove_Left };
                    break;
                case SnakeMoveDirection.SnakeMove_Left:
                    return new SnakeMoveDirection[] { SnakeMoveDirection.SnakeMove_Down, SnakeMoveDirection.SnakeMove_Left, SnakeMoveDirection.SnakeMove_Up };
                    break;
                case SnakeMoveDirection.SnakeMove_Right:
                    return new SnakeMoveDirection[] { SnakeMoveDirection.SnakeMove_Up, SnakeMoveDirection.SnakeMove_Right, SnakeMoveDirection.SnakeMove_Down };
                    break;
            }

            return null;
        }





        private int IsBlockAtPos(BCBlockGameState statein, PointF poscheck)
        {
            return (from n in statein.Blocks where n.BlockRectangle.Contains(poscheck) select n).Count();



        }
        private int getPosWeight(BCBlockGameState statein, PointF poscheck)
        {
            //the way the simple AI works is that it basically interprets the world in units of it's size.
            //the AI itself basically has to choose to either turn left, go straight, or turn right.
            //too simplify this I currently use a small 9x9 grid; if a block occupies a given location or that location is outside the bounds of the level,
            //the grid value will be false. Otherwise, it will be true. the decision of which direction to turn is based on the values in the grid. (whichever prospective direction has the most true values).
            //*changed right now to ints, whichever direction has the lowest sum).

            if (!statein.GameArea.Contains((int)poscheck.X, (int)poscheck.Y))
            {
                return 3;


            }
            else 
                return 2 * IsBlockAtPos(statein,poscheck);
         





        }
        private int[,] getAIGridData(BCBlockGameState gamestate)
        {
            if (SnakeHead == null) return null;

            int middlespot = (int)Math.Ceiling((float)gridsize / 2); //should be five for gridsize of 9.
            SizeF totalgridsize = new SizeF(gridsize * createblockSize.Width, gridsize * createblockSize.Height);
            SizeF halfgridsize = new SizeF(totalgridsize.Width / 2, totalgridsize.Height / 2);
            int[,] returngrid = new int[gridsize, gridsize];

            PointF centralPoint = SnakeHead.CenterPoint();

            for (int x = 0; x < gridsize; x++)
            {
                float Xuse = ((centralPoint.X - halfgridsize.Width) + (createblockSize.Width * x));
                for (int y = 0; y < gridsize; y++)
                {
                    float Yuse = ((centralPoint.Y - halfgridsize.Height) + (createblockSize.Height * y));
                    //at 0, we want to be at centerpoint-(blockwidth*middlespot)
                    returngrid[x, y] = getPosWeight(gamestate, new PointF(Xuse, Yuse));






                }


            }



            return returngrid;




        }

        //private PointF[] getprospectiveDirections(BCBlockGameState gamestate, PointF currentdirection)
        //{
        //    float xpart = currentdirection.X;
        //    float ypart = currentdirection.Y;




        //}

        private void CalcAI(BCBlockGameState gamestate)
        {
            Random rg = BCBlockGameState.rgen;
            //currently changes the snake direction randomly.
            //if (rg.NextDouble() < 0.1)
            //{
            //BlockSpeed = prospectivedirections[rg.Next(0,prospectivedirections.Length)];

            //}
            sincelastchange++;
            if (sincelastchange > 2)
            {

                SnakeMoveDirection[] snakedirections = GetProspectiveDirections(GetCurrentDirection());
                //get the list of possible directions to go.

                //calculate the grid weights around the snakes head.
                int[,] gridweight = getAIGridData(gamestate);
                if (gridweight != null)
                {
                    //corrdirections: corresponds to SnakeMoveDirection Array, containing the accumulated value of that direction from the grid.
                    int[] corrdirections = new int[snakedirections.Length];
                    int currmin = int.MaxValue, minindex = -1;

                    for (int i = 0; i < snakedirections.Length; i++)
                    {
                        corrdirections[i] = GetWeightForDirection(gridweight, snakedirections[i]);
                        if (corrdirections[i] < currmin)
                        {
                            currmin = corrdirections[i];
                            minindex = i;


                        }

                    }

                    Debug.Print("Current Direction is " + Enum.GetName(typeof(SnakeMoveDirection), GetCurrentDirection()) +
                        " Chosen direction is " + Enum.GetName(typeof(SnakeMoveDirection), snakedirections[minindex]));

                    //we have our direction now; the snakedirection corresponding to the minimum.
                    if (snakedirections[minindex] != GetCurrentDirection()) sincelastchange = 0;
                    BlockSpeed = directionToSpeed(snakedirections[minindex]);


                }
            }




        }

        private Block CreateNewBlock(BCBlockGameState gamestate, PointF poscreate)
        {

            //Block createdblock = new NormalBlock(new RectangleF(poscreate, createblockSize), new SolidBrush(Color.Red), new Pen(Color.Black));
            Block createdblock = (Block)Activator.CreateInstance(BlockTypeCreate, new object[] { new RectangleF(poscreate, createblockSize) });
            if (createdblock is NormalBlock)
            {
                NormalBlock nb = createdblock as NormalBlock;
                nb.BlockColor = Color.Red;
                nb.PenColor = Color.Black;


            }
            lock (gamestate.Blocks)
                gamestate.Blocks.AddLast(createdblock);
            gamestate.Forcerefresh = true;
            return createdblock;
        }
        private int sincelastchange = 0; //number of frames since the last direction change.
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //return base.PerformFrame(gamestate, ref AddObjects, ref removeobjects);
            //DON'T call the base; the snake isn't implemented like some other enemies, in that most of it's "matter" is really just a line of blocks.
            //Debug.Print("Snake performframe");
            //Debug.Print("Millis:" + (DateTime.Now - lastcall).TotalMilliseconds.ToString());
            if ((DateTime.Now - lastcall).TotalMilliseconds > FrameDelay)
            {
                if (!killed)
                {
                    //perform a Frame. duh
                    //calculated AI.
                    CalcAI(gamestate);

                    //AI has been calculated, so now we want to add a new block, and if our queue is longer then our maximum length, remove the tail.

                    //for the sake of brevity, the "Position" value will indicate the upper left corner of the "head" piece.
                    PointF useoffset = new PointF(createblockSize.Width * BlockSpeed.X,
                                                  createblockSize.Height * BlockSpeed.Y);
                    //retrieve the new coordinates to add the newest entry...
                    PointF newheadposition = new PointF(Location.X + useoffset.X, Location.Y + useoffset.Y);

                    //create the new block.
                    Block createdblock = CreateNewBlock(gamestate, newheadposition);
                    //stop hooking events from the previous snake head.
                    if (SnakeHead != null)
                    {
                        SnakeHead.OnBlockHit -= SnakeHead_OnBlockHit;
                        SnakeHead.OnBlockDestroy += SnakeBody_OnBlockDestroy;
                    }

                    SnakeHead = createdblock;
                    snakecomponents.Enqueue(SnakeHead);
                    SnakeHead.OnBlockHit += SnakeHead_OnBlockHit;
                    Location = SnakeHead.BlockLocation;
                    while (snakecomponents.Count > MaxLength)
                    {
                        //remove tail bits until our length is appropriate.
                        Block deq = snakecomponents.Dequeue();
                        //remove this block from the game.
                        gamestate.Blocks.Remove(deq);

                    }

                }
                else
                {
                    //killed is true; so animate the snakes GRISLY DEMISE.
                    //dequeue the block...
                    if (snakecomponents.Count == 0) return true;
                    Block deq = snakecomponents.Dequeue();
                    while (!gamestate.Blocks.Contains(deq) && deq != null && snakecomponents.Count > 0)
                    {
                        deq = snakecomponents.Dequeue();
                    }
                    //skip blocks that aren't part of the snake anymore for whatever reason (or that are part of the snake but were destroyed in the meantime)


                    //remove it from the game as well.
                    if (snakecomponents.Count > 0)
                    {
                        if (deq != SnakeHead)
                        {
                            gamestate.Blocks.Remove(deq);
                            BCBlockGameState.Soundman.PlaySound("gren");
                            //   PointF eo = deq.CenterPoint();
                            deq.StandardSpray(gamestate);
                            deq.AddScore(gamestate, 5);
                        }
                        else
                        {
                            gamestate.Blocks.Remove(deq);
                            BCBlockGameState.Soundman.PlaySound("bomb");
                            deq.StandardSpray(gamestate);
                            deq.AddScore(gamestate, 25);
                            gamestate.GameObjects.AddLast(new ExplosionEffect(deq.CenterPoint(), 48));


                        }
                    }


                    /*for (int i = 0; i < 50; i++)
                    {
                        Random rg = BCBlockGameState.rgen;
                        PointF genpoint = new PointF((float)(eo.X + (rg.NextDouble() * createblockSize.Width)), (float)(eo.Y + (rg.NextDouble() * createblockSize.Height)));
                        DustParticle dp = new DustParticle(genpoint, 5f);
                        gamestate.Particles.Add(dp);


                    }*/



                }


                lastcall = DateTime.Now;
                gamestate.Forcerefresh = true;
            }




            return snakecomponents.Count ==0;
        }
        void SnakeBody_OnBlockDestroy(Object Sender,BlockHitEventArgs<bool> e )
        {
            if (e.TheBlock == SnakeHead || killed)
            {
                e.Result = true;
                //return;  //when already dying, ignore.
            }
            Debug.Print("Snake's body was Destroyed...");
            //first, convert our queue into an LinkedList, to make this work a little easier.

            Queue<Block> newsnake = new Queue<Block>();
            //Queue<Block> refreshus = new Queue<Block>();
            //Block[] usearray = snakecomponents.ToArray();
            //the "tail" will be the first element, the last element will be the "head" block.
            
            //PSUEDOCODE:
            //use dequeue on OUR snake segments, adding each one to another queue, and unhooking out event handlers.
            Block removeit = null;
            while (removeit != e.TheBlock)
            {
                removeit = snakecomponents.Dequeue();
                removeit.OnBlockDestroy -= SnakeBody_OnBlockDestroy;
                //remove from game as well.
                removeit.AddScore(e.GameState, 5);
                removeit.StandardSpray(e.GameState);
                BCBlockGameState.Soundman.PlaySound("gren");
                e.GameState.Blocks.Remove(removeit);
                //newsnake.Enqueue(removeit);

            }
            //newsnake is the proper size.
            //commented out: was adding snakes instead :P but found it to be a huge pain in the rear.
            //Block[] snakesegments = newsnake.ToArray();
            //SnakeEnemy se = new SnakeEnemy(snakesegments, FrameDelay);
            //gamestate.GameObjects.AddLast(se);






            e.Result = true;
//            return true; //extra work should be done here to "split" the snake in half at the block.

        }

        void SnakeHead_OnBlockHit(Object sender,BlockHitEventArgs<bool> e )
        {
            //Note: this is Onblock <HIT> because we want the snake to be "defeatable" regardless of the behaviour of the block; so if the head block
            //is hit at all the snake dies.
            if (!killed)
            {
                Debug.Print("The snake has been killed. The horror!");
                FrameDelay *= 3;
                killed = true;

            }
            e.Result = true;
        }
        public override void Draw(Graphics g)
        {
            //amazingly- Do nothing! All the drawing will be managed by the Levelset and blocks and whatnot.

        }


        #region IDeserializationCallback Members

        public void OnDeserialization(object sender)
        {
            //throw new NotImplementedException();
            foreach (EnemyTrigger looptrigger in base.Triggers)
            {

                looptrigger.OurEnemy = this;

            }
        }

        #endregion

        #region iLocatable Members

        /*
        public new PointF Location
        {
            get
            {
                if(SnakeHead!=null)
                    return SnakeHead.CenterPoint();

                return new PointF(0, 0);
            }
            set
            {
                //not truly supported...
                //throw new NotImplementedException();

            }
        }
        */
        #endregion
    }
   


    public class ShootSpinner : SpinnerGuy
    {

       

        [Editor(typeof(ItemTypeEditor<iProjectile>),typeof(UITypeEditor))]
        private Type _ShootType = typeof(LaserShot);

        public Type ShootType { get { return _ShootType; } set { _ShootType = value; } }
        

        public ShootSpinner(PointF pPosition)
            : this(pPosition, new SizeF(16, 16))
        {

        }
        public ShootSpinner(PointF pPosition, SizeF usesize)
            : base(pPosition,usesize)
        {


        }
        public override int GetScoreValue()
        {
            return 400;
        }
        protected override void Shoot(BCBlockGameState gamestate)
        {
            float UseVelocity = 4;
            float useangle = (float)((DrawRotation / 360) * (2 * Math.PI));
            //calculate the velocity....
            PointF projectilespeed = new PointF((float)(Math.Cos(useangle) * UseVelocity), (float)(Math.Sin(useangle) * UseVelocity));
            projectilespeed = new PointF(projectilespeed.X + base.VelocityUse.X, projectilespeed.Y + base.VelocityUse.Y);
            //type has to have a constructor that accepts a Location and Speed.
            if (_ShootType == typeof(LaserShot))
            {
                LaserShot shootbullet = new LaserShot(GetRectangle().CenterPoint(), projectilespeed);
                shootbullet.DamagesPaddle = true;
                shootbullet.Weak = true;
                gamestate.Defer(() =>
                {
                    gamestate.GameObjects.AddLast(shootbullet);
                    BCBlockGameState.Soundman.PlaySound("FIRELASER", 1.0f);
                });
            }
            else
            {

                Projectile shootit = (Projectile)Activator.CreateInstance(_ShootType, GetRectangle().CenterPoint(), projectilespeed);
                gamestate.Defer(() => gamestate.GameObjects.AddLast(shootit));

                
                BCBlockGameState.Soundman.PlaySound("FIRELASER", 1.0f);
            }
        }
    }
    //Similar to Eyeguy, but it bounces around the level.
    public class BouncerGuy : EyeGuy
    {
        public BouncerGuy(PointF pPosition)
            : this(pPosition, new SizeF(16, 16))
        {
            
        }
        public BouncerGuy(PointF pPosition, SizeF pSize)
            : this(pPosition, pSize, new PointF(2.5f, 2.5f))
        {

        }

        public BouncerGuy(PointF pPosition, SizeF pSize, PointF pVelocity):base()
        {
            HitPoints *= 2;
            //setup code is similar to the EyeGuy...
            DoBlockChecks = false;
            counterdelay = 8;
            //choose a Colour
            ColorMatrix[] possiblecolours = new ColorMatrix[] { BASeBlock.ColorMatrices.GetColourizer(3, 1, 1), BASeBlock.ColorMatrices.GetColourizer(1, 1, 3), ColorMatrices.GetColourizer(1, 3, 1) };
            Location = pPosition;
            DrawSize = pSize;
            VelocityUse = pVelocity;
            counterdelay = 5;

            if (BCBlockGameState.rgen.NextDouble() > 0.25)
            {
                DrawAttributes = new ImageAttributes();
                ColorMatrix rndchosen = possiblecolours[BCBlockGameState.rgen.Next(0, possiblecolours.Length)];
                DrawAttributes.SetColorMatrix(rndchosen);
            }




            StateFrameImageKeys = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            StateFrameImageKeys.Add("idle", new string[] { "bouncer_closed" });
            StateFrameImageKeys.Add("opening", new string[] { "bouncer_closed", "bouncer_mid", "bouncer_open" });
            StateFrameImageKeys.Add("active", new string[] { "bouncer_open" });
            StateFrameImageKeys.Add("dying", new string[] { "bouncer_open" });
            StateFrameImageKeys.Add("closing", new string[] { "bouncer_open", "bouncer_mid", "bouncer_closed" });
            StateFrameIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            StateFrameIndex.Add("idle", 0);
            StateFrameIndex.Add("active", 0);
            StateFrameIndex.Add("opening", 0);
            StateFrameIndex.Add("closing", 0);
            StateFrameIndex.Add("dying", 0);
            FrameDelayTimes = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
            FrameDelayTimes.Add("idle", new int[] { 25 });
            FrameDelayTimes.Add("opening", new int[] { 15, 15, 15 });
            FrameDelayTimes.Add("active", new int[] { 5 });
            FrameDelayTimes.Add("closing", new int[] { 15, 15, 15 });
            FrameDelayTimes.Add("dying", new int[] { 25 });



        }
        public override string ToString()
        {
            return "@" + Location.ToString() + " Width:" + DrawSize + " Velocity:" + VelocityUse;
        }
        protected override RectangleF getBoundary(BCBlockGameState mgamestate)
        {
            return mgamestate.GameArea;
        }
        public override int GetScoreValue()
        {
            return 720;
        }
        protected override void BallImpact(cBall ballcheck, Block.BallRelativeConstants ballrel)
        {
            if (((ballrel & Block.BallRelativeConstants.Relative_Right) == Block.BallRelativeConstants.Relative_Right) ||
                (ballrel & Block.BallRelativeConstants.Relative_Left) == Block.BallRelativeConstants.Relative_Left)
            {
                //horizontal; flip x velocity.
                VelocityUse = new PointF(-VelocityUse.X, VelocityUse.Y);

            }

            if (((ballrel & Block.BallRelativeConstants.Relative_Down) == Block.BallRelativeConstants.Relative_Down) ||
                (ballrel & Block.BallRelativeConstants.Relative_Up) == Block.BallRelativeConstants.Relative_Up)
            {
                VelocityUse = new PointF(VelocityUse.X, -VelocityUse.Y);

            }
        }
        protected override void ChangeVelocity(BCBlockGameState gstate)
        {
            int xSign = Math.Sign(VelocityUse.X), ySign = Math.Sign(VelocityUse.Y);
            float distanceRight, distanceBottom;
            float distanceTop = Math.Abs(Location.Y - gstate.GameArea.Top);
            distanceBottom = Math.Abs(Location.Y - gstate.GameArea.Bottom);
            float distanceLeft = Math.Abs(Location.X - gstate.GameArea.Left);
            distanceRight = Math.Abs(Location.X - gstate.GameArea.Right);

            float minval = (new[] { distanceTop, distanceLeft, distanceRight, distanceBottom }).Min();
            if (distanceTop == minval)
            {

                ySign = 1;

            }
            else if (distanceBottom == minval)
                ySign = -1;
            else if (distanceLeft == minval)
            {
                xSign = 1;

            }
            else if (distanceRight == minval)
                xSign = -1;


            VelocityUse = new PointF(Math.Abs(VelocityUse.X) * xSign, Math.Abs(VelocityUse.Y) * ySign);

        }
        protected override float getRotationAmount()
        {
            return 0;
        }

    }
    [Bossable]
    public class QuadSpinner : EyeGuy
    {
        protected override void Shoot(BCBlockGameState gamestate)
        {
            //we don't shoot a fireball, instead we shoot a "bullet".
            counterdelay = BCBlockGameState.rgen.Next(5, 50); //change counter, we shoot at random intervals.
            var coreangle = (float)(((DrawRotation / 360) * (2 * Math.PI)));
            for (int i = 0; i < 3; i++)
            {
                float useangle = (float)(coreangle + ((Math.PI / 2) * (float)i));

                float UseVelocity = 4;

                //calculate the velocity....
                PointF projectilespeed = new PointF((float)(Math.Cos(useangle) * UseVelocity), (float)(Math.Sin(useangle) * UseVelocity));
                projectilespeed = new PointF(projectilespeed.X + base.VelocityUse.X, projectilespeed.Y + base.VelocityUse.Y);
                Bullet shootbullet = new Bullet(GetRectangle().CenterPoint(), projectilespeed);
                shootbullet.Owner = this;
                shootbullet.DamagePaddle = true;
                gamestate.Defer(() => gamestate.GameObjects.AddLast(shootbullet));
            }
            BCBlockGameState.Soundman.PlaySound("SPINSHOOT", 1.0f);

        }


        void QuadSpinnerGuy_OnDeath(Object sender, EnemyDeathEventArgs e)
        {
            var enemydied = e.EnemyDied;
            var stateobject = e.StateObject;
            //spawn a EyeGuy in the same location.
            EyeGuy neweg = new EyeGuy(enemydied.Location, enemydied.DrawSize);
            stateobject.QueueFrameEvent((n, q) => { stateobject.GameObjects.AddLast(neweg); return false; }, null);
        }
        public QuadSpinner(PointF pPosition,SizeF pSize)
        {
            //setup code is similar to the EyeGuy...

            //choose a Colour
            ColorMatrix[] possiblecolours = new ColorMatrix[] { BASeBlock.ColorMatrices.GetColourizer(3, 1, 1), BASeBlock.ColorMatrices.GetColourizer(1, 1, 3), ColorMatrices.GetColourizer(1, 3, 1) };
            Location = pPosition;
            DrawSize = pSize;
            counterdelay = BCBlockGameState.rgen.Next(5,50);

            if (BCBlockGameState.rgen.NextDouble() > 0.25)
            {
                DrawAttributes = new ImageAttributes();
                ColorMatrix rndchosen = possiblecolours[BCBlockGameState.rgen.Next(0, possiblecolours.Length)];
                DrawAttributes.SetColorMatrix(rndchosen);
            }




            StateFrameImageKeys = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            StateFrameImageKeys.Add("idle", new string[] { "quadshooter_closed" });
            StateFrameImageKeys.Add("opening", new string[] { "quadshooter_closed", "quadshooter_mid", "quadshooter_open" });
            StateFrameImageKeys.Add("active", new string[] { "quadshooter_open" });
            StateFrameImageKeys.Add("dying", new string[] { "quadshooter_open" });
            StateFrameImageKeys.Add("closing", new string[] { "quadshooter_open", "quadshooter_mid", "quadshooter_closed" });
            StateFrameIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            StateFrameIndex.Add("idle", 0);
            StateFrameIndex.Add("active", 0);
            StateFrameIndex.Add("opening", 0);
            StateFrameIndex.Add("closing", 0);
            StateFrameIndex.Add("dying", 0);
            FrameDelayTimes = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
            FrameDelayTimes.Add("idle", new int[] { 25 });
            FrameDelayTimes.Add("opening", new int[] { 15, 15, 15 });
            FrameDelayTimes.Add("active", new int[] { 5 });
            FrameDelayTimes.Add("closing", new int[] { 15, 15, 15 });
            FrameDelayTimes.Add("dying", new int[] { 25 });
            base.VelocityUse = new PointF(3, 0);
            OnDeath += QuadSpinnerGuy_OnDeath;

        }
    }
    [Bossable]
    public class SpinnerGuy : EyeGuy
    {
       
        //protected PointF VelocityUse = new PointF(0, 0);

        public new static EyeGuy CreateBoss(PointF pPosition, BCBlockGameState mGameState)
        {
            SpinnerGuy spinnerboss = new SpinnerGuy(pPosition, new SizeF(64, 64));
            spinnerboss.DoBlockChecks = false;
            spinnerboss.idlewaittime = 20;
            spinnerboss.counterdelay = 10;
            spinnerboss.HitPoints *= 25;

            //BCBlockGameState.Soundman.PlayMusic("D:\\mycovers\\thedecisivebattle.mp3", true);
            spinnerboss.OnShoot += spinnerboss_OnShoot;
            spinnerboss.OnDeath += spinnerboss_OnDeath;


            return spinnerboss;
        }

        static void spinnerboss_OnDeath(Object sender,EnemyDeathEventArgs e)
        {
            //throw new NotImplementedException();
        }
        static int incspawn = 0;
        static void  spinnerboss_OnShoot(Object Sender,EyeGuyShootEventArgs<bool> e )
        {

            incspawn++;
            if (incspawn == 100)
            {
                incspawn = 0;
                PointF usebouncerspeed = e.GameState.AimAtPaddle(e.EnemyGuy.GetRectangleF().CenterPoint(), (float)(BCBlockGameState.rgen.NextDouble() * 3 + 2));
                BouncerGuy bg = new BouncerGuy(e.EnemyGuy.GetRectangleF().CenterPoint(), new SizeF(16, 16), usebouncerspeed);
                bg.HitPoints = 1;
                bg.MaxHitPoints = 1;
                e.GameState.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup((() => e.GameState.GameObjects.AddLast(bg))));
                e.Result = true;

            }
            e.Result = false;

           // throw new NotImplementedException();
        }

        public SpinnerGuy(PointF pPosition):this(pPosition,new SizeF(16,16))
        {



        }
        
        public SpinnerGuy(PointF pPosition, SizeF usesize):base()
        {
            //setup code is similar to the EyeGuy...

            //choose a Colour
            ColorMatrix[] possiblecolours = new ColorMatrix[] { BASeBlock.ColorMatrices.GetColourizer(3, 1, 1), BASeBlock.ColorMatrices.GetColourizer(1, 1, 3), ColorMatrices.GetColourizer(1, 3, 1) };
            Location = pPosition;
            DrawSize = usesize;
            counterdelay = 5;

            if (BCBlockGameState.rgen.NextDouble() > 0.25)
            {
                DrawAttributes = new ImageAttributes();
                ColorMatrix rndchosen = possiblecolours[BCBlockGameState.rgen.Next(0, possiblecolours.Length)];
                DrawAttributes.SetColorMatrix(rndchosen);
            }


            
            
            StateFrameImageKeys = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            StateFrameImageKeys.Add("idle", new string[] { "spinshooter_closed" });
            StateFrameImageKeys.Add("opening", new string[] { "spinshooter_closed", "spinshooter_mid", "spinshooter_open" });
            StateFrameImageKeys.Add("active", new string[] { "spinshooter_open" });
            StateFrameImageKeys.Add("dying", new string[] { "spinshooter_open" });
            StateFrameImageKeys.Add("closing", new string[] { "spinshooter_open", "spinshooter_mid", "spinshooter_closed" });
            StateFrameIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            StateFrameIndex.Add("idle", 0);
            StateFrameIndex.Add("active", 0);
            StateFrameIndex.Add("opening", 0);
            StateFrameIndex.Add("closing", 0);
            StateFrameIndex.Add("dying", 0);
            FrameDelayTimes = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
            FrameDelayTimes.Add("idle", new int[] { 25 });
            FrameDelayTimes.Add("opening", new int[] { 15, 15, 15 });
            FrameDelayTimes.Add("active", new int[] { 5 });
            FrameDelayTimes.Add("closing", new int[] { 15, 15, 15 });
            FrameDelayTimes.Add("dying", new int[] { 25 });
            base.VelocityUse = new PointF(3, 0);
            OnDeath += SpinnerGuy_OnDeath;

        }
        public override int GetScoreValue()
        {
            return 320;
        }
        void SpinnerGuy_OnDeath(Object sender,EnemyDeathEventArgs e)
        {
            var enemydied = e.EnemyDied;
            var stateobject = e.StateObject;
            //spawn a EyeGuy in the same location.
            EyeGuy neweg = new EyeGuy(enemydied.Location, enemydied.DrawSize);
            stateobject.QueueFrameEvent((n, q) => { stateobject.GameObjects.AddLast(neweg); return false; },null);
        }
        protected override void Shoot(BCBlockGameState gamestate)
        {
            //we don't shoot a fireball, instead we shoot a "bullet".

            float UseVelocity = 4;
            float useangle = (float)((DrawRotation/360)*(2*Math.PI));
            //calculate the velocity....
            PointF projectilespeed = new PointF((float)(Math.Cos(useangle) * UseVelocity), (float)(Math.Sin(useangle) * UseVelocity));
            projectilespeed = new PointF(projectilespeed.X + base.VelocityUse.X, projectilespeed.Y + base.VelocityUse.Y);
            Bullet shootbullet = new Bullet(GetRectangle().CenterPoint(), projectilespeed);
            shootbullet.Owner = this;
            shootbullet.DamagePaddle = true;
            gamestate.Defer(() => gamestate.GameObjects.AddLast(shootbullet));
            
            BCBlockGameState.Soundman.PlaySound("SPINSHOOT", 1.0f);

        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            return base.PerformFrame(gamestate);
        }
    }
    public class EyeGuyShootEventArgs<T> : EventArgs
    {
        //Func<BCBlockGameState, EyeGuy, bool>
        private BCBlockGameState _gstate;
        private EyeGuy _eyeguy;
        private T _Result;
        public BCBlockGameState GameState { get { return _gstate; } set { _gstate = value; } }
        public EyeGuy EnemyGuy { get { return _eyeguy; } set { _eyeguy = value; } }
        public T Result { get { return _Result; } set { _Result = value; } }
        public EyeGuyShootEventArgs(BCBlockGameState pGameState,EyeGuy pEnemy)
        {
            _gstate = pGameState;
            EnemyGuy=pEnemy;


        }
    }
    [Bossable]
    public class EyeGuy : GameEnemy,iImagable,iSizedProjectile
    {

        public event EventHandler<EyeGuyShootEventArgs<bool>>  OnShoot = null;


        public static EyeGuy CreateBoss(PointF pPosition,BCBlockGameState mGameState)
        {
            EyeGuy eyeboss = new EyeGuy(pPosition, new SizeF(64, 64));
            eyeboss.DoBlockChecks = false;
            eyeboss.idlewaittime = 20;
            eyeboss.counterdelay = 25;
            eyeboss.HitPoints *= 15;
            
            //BCBlockGameState.Soundman.PlayMusic("D:\\mycovers\\thedecisivebattle.mp3", true);
            eyeboss.OnShoot += eyeboss_OnShoot;
            eyeboss.OnDeath += eyeboss_OnDeath;
           

            return eyeboss;
        }
        static int incspawn = 0;
        static void eyeboss_OnShoot(Object Sender,EyeGuyShootEventArgs<bool> e )
        {
            incspawn++;
            if (incspawn == 100)
            {
                incspawn = 0;
                PointF usebouncerspeed = e.GameState.AimAtPaddle(e.EnemyGuy.GetRectangleF().CenterPoint(), (float)(BCBlockGameState.rgen.NextDouble() * 3 + 2));
                BouncerGuy bg = new BouncerGuy(e.EnemyGuy.GetRectangleF().CenterPoint(), new SizeF(16, 16), usebouncerspeed);
                bg.HitPoints = 1;
                bg.MaxHitPoints = 1;
                e.GameState.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup((() => e.GameState.GameObjects.AddLast(bg))));
                e.Result = true;

            }
            e.Result = false;
        }
        iActiveSoundObject revertmusic = null;
        static void eyeboss_OnDeath(Object sender, EnemyDeathEventArgs e)
        {
            

        }
        private bool FireOnShoot(BCBlockGameState gstate, EyeGuy Sender)
        {
            var temp = OnShoot;
            if (temp == null) return false;
            {
                EyeGuyShootEventArgs<bool> e = new EyeGuyShootEventArgs<bool>(gstate, Sender);
                temp(this, e);
                //return temp(gstate, Sender);
                return e.Result;
            }

        }

        public EyeGuy(PointF Position)
            : this(Position, new SizeF(16, 16))
        {

        }
        protected EyeGuy():base(PointF.Empty,null,5)
        {





        }
        public EyeGuy(PointF Position, SizeF usesize)
            : base(Position, null, 50)
        {


            //choose a Colour
            ColorMatrix[] possiblecolours = new ColorMatrix[] { BASeBlock.ColorMatrices.GetColourizer(3, 1, 1), BASeBlock.ColorMatrices.GetColourizer(1, 1, 3), ColorMatrices.GetColourizer(1, 3, 1) };



            if (BCBlockGameState.rgen.NextDouble() > 0.25)
            {
                DrawAttributes = new ImageAttributes();
                ColorMatrix rndchosen = possiblecolours[BCBlockGameState.rgen.Next(0, possiblecolours.Length)];
                DrawAttributes.SetColorMatrix(rndchosen);
            }


            VelocityUse = new PointF(3, 0);
            DrawSize = usesize;
            StateFrameImageKeys = new Dictionary<string, string[]>(StringComparer.OrdinalIgnoreCase);
            StateFrameImageKeys.Add("idle", new string[] { "eyeclose" });
            StateFrameImageKeys.Add("opening", new string[] { "eyeclose", "eyemid", "eyeopen" });
            StateFrameImageKeys.Add("active", new string[] { "eyeopen" });
            StateFrameImageKeys.Add("dying", new string[] { "eyeopen" });
            StateFrameImageKeys.Add("closing", new string[] { "eyeopen", "eyemid", "eyeclose" });
            StateFrameIndex = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
            StateFrameIndex.Add("idle", 0);
            StateFrameIndex.Add("active", 0);
            StateFrameIndex.Add("opening", 0);
            StateFrameIndex.Add("closing", 0);
            StateFrameIndex.Add("dying", 0);
            FrameDelayTimes = new Dictionary<string, int[]>(StringComparer.OrdinalIgnoreCase);
            FrameDelayTimes.Add("idle", new int[] { 25 });
            FrameDelayTimes.Add("opening", new int[] { 15, 15, 15 });
            FrameDelayTimes.Add("active", new int[] { 5 });
            FrameDelayTimes.Add("closing", new int[] { 15, 15, 15 });
            FrameDelayTimes.Add("dying", new int[] { 25 });

        }
        protected int _idlewaittime = 150;
        protected PointF VelocityUse = new PointF(0, 0);
        protected int idlecounter = 0;
        protected int shootcounter = 0;
        protected int _counterdelay = 30;
        public int counterdelay { get { return _counterdelay; } set { _counterdelay = value; } }
        public int idlewaittime { get { return _idlewaittime; } set { _idlewaittime = value; } }
        private PointF GetCenter()
        {
            return new PointF(Location.X + DrawSize.Width / 2, Location.Y + DrawSize.Height / 2);


        }
        public override int GetScoreValue()
        {
            return 300;
        }
        private Type[] _ShootTypes = new Type[]{typeof(Fireball),typeof(BlueFireball)};
        private float[] _ShootProbability = new float[] { 8, 2 };


        public Type[] ShootTypes
        {
            get { return _ShootTypes; }

            set
            {
                if (!value.All((w) => w is iProjectile))
                    throw new ArgumentException("All types in provided array must implement iProjectile.");
                
                _ShootTypes = value;
            }
        }
        public float[] ShootProbability
        {
            get
            {
                if (_ShootProbability.Length != _ShootTypes.Length)
                    Array.Resize(ref _ShootProbability, _ShootTypes.Length);
                return _ShootProbability;

            }

            set
            {
                _ShootProbability = value;
            }
        }
        protected Type ChooseShootType()
        {

            return BCBlockGameState.Select(_ShootTypes, ShootProbability);

        }
        private iProjectile SpawnProjectile(PointF pPosition, SizeF psize, PointF pVelocity)
        {
            Type usetype = ChooseShootType();
            //valid constructor: PointF, PointF,SizeF (location,speed,size) and also lacking Size parameter.
            var ip = (iProjectile)Activator.CreateInstance(usetype, pPosition, pVelocity);
            //set size if it is a sized projectile.
            if (ip is iSizedProjectile) ((iSizedProjectile)ip).Size = psize;
            return ip;
        }
        /// <summary>
        /// called to, uh... shoot. EyeGuy default implementation shoots a fireball towards the paddle.
        /// </summary>
        /// <param name="gamestate"></param>
        protected virtual void Shoot(BCBlockGameState gamestate)
        {

            if (FireOnShoot(gamestate, this)) return;
            float randomspeed = 2;
            randomspeed += (float)(BCBlockGameState.rgen.NextDouble() * 4);
            PointF choosevelocity = gamestate.AimAtPaddle(Location, randomspeed);
            iProjectile Shotitem = SpawnProjectile(Location, new SizeF(8,8), choosevelocity);
            /*
            if (BCBlockGameState.rgen.NextDouble() > 0.2)
            {
                addobjects.Add(new Fireball(gamestate, GetCenter(), new SizeF(8, 8), randomspeed));
            }
            else
            {
                addobjects.Add(new BlueFireball(gamestate, GetCenter(), new SizeF(8, 8), randomspeed));
            }
             * */
            gamestate.Defer(() =>
            {
                gamestate.GameObjects.AddLast(Shotitem as GameObject);
                BCBlockGameState.Soundman.PlaySound("spitfire", 1.0f);
            });
        }

        private bool dodestroyblocks = false;
        protected virtual void ChangeVelocity(BCBlockGameState gamestate)
        {
            VelocityUse = new PointF(-VelocityUse.X, -VelocityUse.Y);

        }
        protected virtual RectangleF getBoundary(BCBlockGameState mgamestate)
        {
            if (mgamestate.PlayerPaddle == null) return mgamestate.GameArea;
            return new RectangleF(mgamestate.GameArea.Left, mgamestate.GameArea.Top, mgamestate.GameArea.Width, mgamestate.PlayerPaddle.Getrect().ToRectangleF().Top);

        }
        public bool DoBlockChecks = true;
        protected virtual void BallImpact(cBall ballcheck,Block.BallRelativeConstants ballrel)
        {
            float minmagnitude = ballcheck.getMagnitude();
            //new velocity should be set....
            //if we are moving, add our velocity.
            //needs more work, velocity of the ball needs
            //to be changed more "realistically"...
            if ((ballrel & Block.BallRelativeConstants.Relative_Left) == Block.BallRelativeConstants.Relative_Left)
            {
                VelocityUse = new PointF(minmagnitude, 0);


            }
            else if ((ballrel & Block.BallRelativeConstants.Relative_Right) == Block.BallRelativeConstants.Relative_Right)
            {
                VelocityUse = new PointF(-minmagnitude, 0);


            }
            else if ((ballrel & Block.BallRelativeConstants.Relative_Up) == Block.BallRelativeConstants.Relative_Up)
            {
                VelocityUse = new PointF(0, minmagnitude);


            }
            else if ((ballrel & Block.BallRelativeConstants.Relative_Down) == Block.BallRelativeConstants.Relative_Down)
            {
                VelocityUse = new PointF(0, -minmagnitude);


            }
            if (EnemyAction == "active")
            {
                ballcheck.Velocity = new PointF(ballcheck.Velocity.X + VelocityUse.X, ballcheck.Velocity.Y + VelocityUse.Y);
                //also, depending on the direction we were hit, modify our direction.

                //make sure we are going <at LEAST> minmagnitude...
                if (ballcheck.getMagnitude() <= minmagnitude)
                {
                    double gotangle = BCBlockGameState.GetAngle(new PointF(0, 0), ballcheck.Velocity);
                    ballcheck.Velocity = BCBlockGameState.GetVelocity(minmagnitude, gotangle);


                }


                ballcheck.invokeballimpact(ballcheck);


            }
        }
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            //very first: check for ball impacts.

            #region ball checks
            //first, create a rectangle that extends out from this "eyeguy"...
            RectangleF currrect = GetRectangleF();
            Rectangle extendedRectangle = currrect.ToRectangle();
            //RectangleF extendedRectangle = new RectangleF(Location.X - currrect.Width, Location.Y - currrect.Height / 2, currrect.Width * 2, currrect.Height * 2);
            foreach (cBall Checkball in (from n in gamestate.Balls where extendedRectangle.Contains(n.Location.ToPoint()) select n))
            {
                //determine if Checkball touches us.
                Block.BallRelativeConstants ballrel;
                
                if (BCBlockGameState.CheckImpact(gamestate, currrect, Checkball, out ballrel))
                {
                    HitPoints -= (int)(Math.Ceiling(Checkball.getMagnitude()));
                    if (HitPoints < 0)
                    {

                        EnemyAction = "dying";


                    }

                    BallImpact(Checkball, ballrel);

                }


            }

            #endregion
            if (DoBlockChecks)
            {
                #region block checks
                List<Block> removethese = new List<Block>();
                foreach (Block checkblock in (from n in gamestate.Blocks where n.HitTest(currrect) select n))
                {
                    //destroy them...


                    //spawn a shiteload of particles.

                    PointF middlespot = checkblock.CenterPoint();
                    var closed = checkblock;
                    lock (gamestate.Blocks)
                    {
                        //checkblock.StandardSpray(gamestate);
                        //removethese.Add(checkblock);
                        gamestate.Defer(() =>
                        {
                            BCBlockGameState.Block_Hit(gamestate, closed);
                            gamestate.Blocks.Remove(closed);
                        });
                    }

                    gamestate.Forcerefresh = true;


                }
                //iterate and remove all 'removethese' ...
                gamestate.Defer(() =>
                {
                    foreach (Block removethis in removethese)
                    {
                        var copied = removethis;

                        gamestate.Blocks.Remove(copied);


                    }
                });
                #endregion

            }



            switch (EnemyAction)
            {
                case "idle":

                    idlecounter++;
                    if (idlecounter >= idlewaittime)
                    {
                        //if(Location.X > (gamestate.GameArea.Width/2))
                        ChangeVelocity(gamestate);
                        
                        idlecounter = 0;
                        EnemyAction = "opening";

                    }
                    break;
                case "opening":

                    //no special handling, we'll move forward as needed in the framesetcomplete routine.
                    break;
                case "active":
                    //are we touching the edge of the gamearea?
                    DrawRotation += getRotationAmount();
                    shootcounter++;
                    if (shootcounter >= counterdelay)
                    {
                        shootcounter = 0;
                        Shoot(gamestate);

                    }
                    //Location = new PointF(Location.X + VelocityUse.X, Location.Y + VelocityUse.Y);
                    RectangleF useboundary = getBoundary(gamestate);
                    BCBlockGameState.IncrementLocation(gamestate, ref _Location, VelocityUse);
                    #region X boundary check
                    if (Math.Sign(VelocityUse.X) == 1)
                    {
                        if (((Location.X + VelocityUse.X + GetRectangleF().Width) > useboundary.Right))
                        {
                            EnemyAction = "closing";
                            Location = new PointF(useboundary.Right - GetRectangleF().Width, Location.Y);
                            //set to right edge.
                        }
                    }
                    else if (Math.Sign(VelocityUse.X) == -1)
                    {
                        if ((Location.X + VelocityUse.X < useboundary.Left))
                        {
                            EnemyAction = "closing";
                            //set to left edge
                            Location = new PointF(useboundary.Left, Location.Y);
                        }



                    }
                    #endregion
                    #region Y boundary check

                    float Ymax = useboundary.Bottom;
                    if (Math.Sign(VelocityUse.Y) == 1)
                    {
                        if (((Location.Y + VelocityUse.Y + GetRectangleF().Height) > Ymax))
                        {
                            EnemyAction = "closing";

                            Location = new PointF(Location.X, Ymax - GetRectangleF().Height);
                            //set to Bottom edge.
                        }
                    }
                    else if (Math.Sign(VelocityUse.Y) == -1)
                    {
                        if ((Location.Y + VelocityUse.Y < useboundary.Top))
                        {
                            EnemyAction = "closing";
                            //set to top edge
                            Location = new PointF(Location.X, useboundary.Top);
                        }



                    }




                    #endregion

                    break;
                case "closing":

                    break; //no special handling
            }




            return base.PerformFrame(gamestate);




        }
        
        protected virtual float getRotationAmount()
        {
            return 5;
        }

        public override void OnFramesetComplete(BCBlockGameState gamestate)
        {
            Trace.WriteLine("Frameset Complete:" + EnemyAction);
            if (EnemyAction == "dying")
            {
                Debug.Print("Break");
            }
            if (EnemyAction == "opening")
            {
                EnemyAction = "active";


            }
            else if (EnemyAction == "closing")
                EnemyAction = "idle";

            else if (EnemyAction == "dying")
            {
                //removeobjects.Add(this);
                gamestate.Defer(()=>gamestate.GameObjects.Remove(this));
                //also... add lots of particles.
                //AND... and... and explosion sound.

                BCBlockGameState.Soundman.PlaySound("gren");
                for (int i = 0; i < 50; i++)
                {
                    Random rg = BCBlockGameState.rgen;
                    PointF genpoint = new PointF((float)(Location.X + (rg.NextDouble() * GetSize().Width)), (float)(Location.Y + (rg.NextDouble() * GetSize().Height)));
                    DustParticle dp = new DustParticle(genpoint, 5f);
                    gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() => gamestate.Particles.Add(dp)));
                    gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                    {
                        var iteratethis = PolyDebris.Fragment(this, 5, null);

                        foreach (var iterate in iteratethis)
                        {
                            gamestate.Particles.Add(iterate);

                        }
                    }));
                }
                //tweak: Feb 19th 2012: make a small explosioneffect now too.
                ExplosionEffect kerplosion = new ExplosionEffect(Location, 40);
                //remember to add it to the AddObject ref parameter...
                gamestate.Defer(() => gamestate.GameObjects.AddLast(kerplosion));
                

            }
        }




        #region iImagable Members

        void iImagable.Draw(Graphics g)
        {
            this.Draw(g);
        }

        Size iImagable.Size
        {
            get
            {
                return new Size((int)DrawSize.Width, (int)DrawSize.Height);
            }
            set
            {
                DrawSize = new SizeF(value.Width, value.Height);
            }
        }

        Point iImagable.Location
        {
            get
            {
                return new Point((int)Location.X, (int)Location.Y);
            }
            set
            {
                Location = new PointF(value.X, value.Y);
            }
        }

        Rectangle iImagable.getRectangle()
        {
            return GetRectangle();
        }

        #endregion

        public PointF Velocity { get { return VelocityUse; } set { VelocityUse = value; } }
        public SizeF Size { get { return DrawSize; } set{_DrawSize=value;} }
    }

    public class ChomperEnemy : GameEnemy
    {
        protected enum MouthAnimationDirectionConstants
        {
            Mouth_Opening,
            Mouth_Closing


        }
        const float MaxMouthAngle = 90f;
        const float MinMouthAngle = 0;
        private float ChomperRadius;
        private float Mouthmovespeed = 10f;
        private float _MouthDirectionAngle = 0f;
        private float Speed = 2;
        private Brush _Fill = Brushes.Yellow;
        private Pen _Draw = Pens.Black;
        //protected MouthAnimationDirectionConstants MouthAnimationDirection=MouthAnimationDirectionConstants.Mouth_Closing;
        protected float CurrentMouthAngle = 45f;

        public float MouthDirectionAngle { get { return _MouthDirectionAngle; } set { _MouthDirectionAngle = value; refreshVelocity(); } }


        private void refreshVelocity()
        {

            _Velocity = new PointF((float)Math.Cos(ToRadians(MouthDirectionAngle)) * Speed,
                    (float)Math.Sin(ToRadians(MouthDirectionAngle)) * Speed);

        }
        protected MouthAnimationDirectionConstants MouthAnimationDirection
        {
            get
            {
                if (Math.Sign(Mouthmovespeed) == 1)
                    return MouthAnimationDirectionConstants.Mouth_Opening;
                else
                    return MouthAnimationDirectionConstants.Mouth_Closing;

            }



        }

        public ChomperEnemy(PointF pLocation, float Radius)
            : base(pLocation, null, 0)
        {
            Location = pLocation;
            ChomperRadius = Radius;
            Speed = 2;
            MouthDirectionAngle = 0;

        }
        private void EmitPacSound()
        {
            BCBlockGameState.Soundman.PlaySound("wakka", 0.5f);

        }
        private void MoveChomper(BCBlockGameState gstate)
        {
          if(gstate.Balls.Count > 1)
            BCBlockGameState.IncrementLocation(gstate, ref _Location, Velocity);


        }
        private PointF _Velocity;
        public PointF Velocity
        {
            get
            {
                return _Velocity;
            }
            set
            {
                _Velocity = value;
            }
        }
        private static float ToRadians(float degrees)
        {

            return (float)(degrees * (Math.PI / 180));

        }
        private static float ToDegrees(float Radians)
        {
            return (float)(Radians / (Math.PI / 180));


        }

        public Rectangle ChomperRectangle()
        {
            return new Rectangle((int)(Location.X - ChomperRadius), (int)(Location.Y - ChomperRadius), (int)(ChomperRadius * 2), (int)(ChomperRadius * 2));


        }
        int framecounter = 0;
        cBall closeball = null;
        public override bool PerformFrame(BCBlockGameState gamestate)
        {
            framecounter++;
            if (framecounter == 5)
            {
                framecounter = 0;


                //determine the closest ball to the Chomper...

                cBall gotmin = null;
                float currentmin = 32768;
                float getmin = 0;
                
                foreach (cBall loopball in gamestate.Balls)
                {
                    getmin = BCBlockGameState.Distance(Location.X, Location.Y, loopball.Location.X, loopball.Location.Y);
                    if (gotmin == null || (getmin < currentmin))
                    {
                        gotmin = loopball;

                        currentmin = getmin;


                    }



                }
             
                if (gotmin != null && !Dying)
                {
                    //gotmin is the closest;
                    //move towards it.
                    closeball = gotmin;

                    //if it is closer then our radius, AND the angle between it and the chomper are less them then the maximum mouth opening stance then "eat" the ball.

                    float baseradians = (float)BCBlockGameState.GetAngle(Location, gotmin.Location);
                    double AngleDifference = BCBlockGameState.AngleDifference(MouthDirectionAngle, baseradians);
                    PointF result = BCBlockGameState.NudgeTowards(Location,Velocity,gotmin.Location,(float)((Math.PI)/10));
                 
                    float fixradians = (float)BCBlockGameState.GetAngle(new PointF(0, 0), result);
                    if (Single.IsNaN(fixradians))
                    {
                        fixradians = ToRadians(MouthDirectionAngle);

                    }
                    float useanglex = fixradians;
                    MouthDirectionAngle = ToDegrees(fixradians);
                    
                    /*


                    float fixradians = baseradians;
                    float useangle = ToDegrees(baseradians);
                    float useanglex = fixradians;
                   
                    //double anglevary = Math.Sign(anglebetween - movementangle) * (Math.PI / 90) * 0.25f;
                    MouthDirectionAngle += Math.Sign(useangle - MouthDirectionAngle) * 10;
                    //int signuse = Math.Sign(MouthDirectionAngle - useangle);
                    //MouthDirectionAngle -= (signuse * 10);
                    */
                    if (currentmin < ChomperRadius && currentmin > 0)
                    {

                        if ((MouthDirectionAngle - useanglex) < (MouthDirectionAngle + (MaxMouthAngle / 2)))
                        {
                            //eat the ball.
                            if (!EatBall(gamestate, gotmin))
                                ElasticWith(gotmin);

                            return false;
                        }
                        else
                        {
                            //reflect the ball "away" from the chomper.
                            ElasticWith(gotmin);

                        }



                    }

                }



            }

            if (Dying) Mouthmovespeed = Math.Abs(Mouthmovespeed);
            bool returnvalue = false;
            CurrentMouthAngle += Mouthmovespeed;

            if (!Dying)
            {
                if (CurrentMouthAngle < MinMouthAngle)
                {
                    EmitPacSound();
                    Mouthmovespeed *= -1;
                    CurrentMouthAngle = MinMouthAngle;
                }


                if (CurrentMouthAngle > MaxMouthAngle)
                {
                    EmitPacSound();
                    Mouthmovespeed *= -1;
                    CurrentMouthAngle = MaxMouthAngle;
                }
            
                MoveChomper(gamestate);
                //return base.PerformFrame(gamestate, ref AddObjects, ref removeobjects);
                //change the current Angle

               
                returnvalue = !gamestate.GameArea.IntersectsWith(ChomperRectangle());
                
                
                
            }

            else
            {
                Mouthmovespeed = Math.Abs(Mouthmovespeed);
                if (CurrentMouthAngle >= (Math.PI *.2f))
                {
                    gamestate.Defer(() =>
                    {
                        for (int i = 0; i < 10; i++)
                        {
                            gamestate.Particles.Add(new PolyDebris(Location, 6, Color.Yellow, 3, 8, 3, 8));
                        }
                    });

                    return true;
                }
            }

            return returnvalue;



        }
        private void ElasticWith(cBall ballbounce)
        {
           
            Polygon ballpoly = ballbounce.GetBallPoly();
            Vector Adjustment = new Vector();
            //create the poly from our circle.
            EllipseBlock eb = new EllipseBlock(new RectangleF(Location.X-ChomperRadius,Location.Y-ChomperRadius,ChomperRadius*2,ChomperRadius*2));
            Polygon ChomperPoly = eb.GetPoly();


            GeometryHelper.PolygonCollisionResult pcr = GeometryHelper.PolygonCollision(ChomperPoly, ballpoly, new Vector(ballbounce.Velocity.X, ballbounce.Velocity.Y));
            Adjustment = pcr.MinimumTranslationVector;
            ballbounce.Velocity = ballbounce.Velocity.Mirror(pcr.MinimumTranslationVector);
            ballbounce.Velocity = new PointF(ballbounce.Velocity.X, ballbounce.Velocity.Y);
            ballbounce.Location = new PointF(ballbounce.Location.X - Adjustment.X, ballbounce.Location.Y - Adjustment.Y);



         


        }
        private bool Dying = false;

        private bool EatBall(BCBlockGameState gamestate, cBall gotmin)
        {
            if (Dying) return false;
            // throw new NotImplementedException();
            if (gamestate.Balls.Count > 1)
            {
                gamestate.RemoveBalls.Add(gotmin);
                ChomperRadius += gotmin.Radius*2;
                if (ChomperRadius > 32)
                {
                    Dying = true;
                    BCBlockGameState.Soundman.PlaySound("pacdie");
                }
                else
                {
                    BCBlockGameState.Soundman.PlaySound("emerge");
                }
                return true;
            }
            return false;
        }

        public override void Draw(Graphics g)
        {
            //base.Draw(g);

            float useRadius = ChomperRadius;
         
            
            

                g.FillPie(_Fill, Location.X - useRadius, Location.Y - useRadius, useRadius * 2, useRadius * 2, MouthDirectionAngle + (CurrentMouthAngle / 2), 360 - (CurrentMouthAngle/2));

                g.DrawPie(_Draw, Location.X - useRadius, Location.Y - useRadius, useRadius * 2, useRadius * 2, MouthDirectionAngle + (CurrentMouthAngle / 2), 360 - (CurrentMouthAngle/2));
            
            //if (closeball != null) 
            //g.DrawLine(new Pen(Color.Black, 2), Location.X, Location.Y, closeball.Location.X, closeball.Location.Y);


        }
    }

}