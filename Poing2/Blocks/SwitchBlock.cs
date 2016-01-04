using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Windows.Forms;
using BASeCamp.BASeBlock.Particles;

namespace BASeCamp.BASeBlock.Blocks
{
    
    [Serializable]
    public class SwitchBlockMulti : Block
    {

        //very much the same as SwitchBlock, but it has multiple states.
        
        private List<SwitchStateData> _SwitchData = new List<SwitchStateData>();
        [Editor(typeof(ItemTypeEditor<SwitchStateData>), typeof(UITypeEditor))]
        public List<SwitchStateData> SwitchData { get { return _SwitchData; } set { _SwitchData = value; RebuildImages(); } }
         
        private int _State = 0; //current state. Also doubles as index into various arrays.
        private bool _Locked;
        public override RectangleF BlockRectangle
        {
            get
            {
                return base.BlockRectangle;
            }
            set
            {
                //rebuild the images when we resize.
                base.BlockRectangle = value;
                RebuildImages();
            }
        }
        private bool MultiTrigger = false; //whether this SwitchSet can be set
        public int State { get { return _State; } set { if (_Locked) return;  _State = value; } }
        public bool Locked { get { return _Locked; } set { _Locked = value; } }
        
        private void RebuildImages()
        {
            if (!flInitialized) return;
            if (BlockRectangle_int.IsEmpty) return;
            Image overlaypic = BCBlockGameState.Imageman.getLoadedImage("switchoverlay");
            foreach (var iterate in _SwitchData)
            {
                //rebuild each image.
                Bitmap usebitmap = new Bitmap(BlockRectangle_int.Width, BlockRectangle_int.Height);
                Image Gummypic = BCBlockGameState.GetGummyImage(iterate.StateColor);
                
                using (Graphics useg = Graphics.FromImage(usebitmap))
                {
                    //Draw our 'gummy' image, and then overlay overlaypic.
                    useg.DrawImageUnscaled(Gummypic, 0, 0);
                    useg.DrawImage(overlaypic, 0, 0, usebitmap.Width, usebitmap.Height);
                    iterate.StateImage = usebitmap;


                }



            }


        }
        protected override void StandardSpray(BCBlockGameState parentstate, cBall ballhit)
        {

            //add lightorbs, too!
            //one in each cardinal direction.
            double usespeed = 4;
            double currangle = 0;
            double angleincrement = Math.PI / 4;
            while (currangle < Math.PI * 2)
            {
                PointF velocityuse = new PointF((float)(Math.Cos(currangle) * usespeed), (float)(Math.Sin(currangle) * usespeed));
                LightOrb makeorb = new LightOrb(BlockRectangle.CenterPoint(), SwitchData[State].StateColor, 24);
                makeorb.Velocity = velocityuse;
                parentstate.Particles.Add(makeorb);

                currangle += angleincrement;
            }

        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            //tricky business.
            //first, if we are locked, we cannot be affected.
            if (_Locked) return false;

            //otherwise, add to our current state.
            State = (State + 1) % _SwitchData.Count;
            
            
            //if enabled, see if all other SwitchBlocks that have the same triggerID at this state index
            //are set to this state.
            var applicableblocks = from b in parentstate.Blocks
                                   where b is SwitchBlockMulti && (b as SwitchBlockMulti).SwitchData.Count >= SwitchData.Count &&
                                       (b as SwitchBlockMulti).SwitchData[State].InvokeID == SwitchData[State].InvokeID
                                   select b as SwitchBlockMulti;

            if (applicableblocks.All((r) => r.State == State))
            {
                //if they are all in this state, fire the trigger.
                Trigger.InvokeTriggerID(SwitchData[State].InvokeID, parentstate);


                //now, destroy the applicable blocks if necessary.
                if (!MultiTrigger)
                {
                    List<Block> removethem = new List<Block>(applicableblocks);
                    parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                        {
                            foreach (var removeit in removethem)
                            {
                                removeit.StandardSpray(parentstate);
                                parentstate.Blocks.Remove(removeit);

                            }

                        }));


                }

            }

            return true;

        }
        public override void Draw(Graphics g)
        {


            g.DrawImageUnscaled(SwitchData[_State].StateImage, BlockLocation.ToPoint());
        }
        private bool _Init = false;
        private bool flInitialized { get { return _Init; } set { _Init = value; if (_Init) RebuildImages(); } }
        public SwitchBlockMulti(SerializationInfo info, StreamingContext context):base(info,context)
        {
            State = info.GetInt32("State");
            SwitchData = (List<SwitchStateData>)info.GetValue("SwitchData",typeof(List<SwitchStateData>));


            
        }
        public override void OnDeserialization(object sender)
        {
            base.OnDeserialization(sender);
            flInitialized = true;
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
            base.GetObjectData(info, context);
            info.AddValue("State", State);
            info.AddValue("SwitchData", SwitchData);



        }
        public SwitchBlockMulti(RectangleF blockrect)
        {
            base.BlockRectangle = blockrect;
            
            SwitchData = new List<SwitchStateData>() {
                new SwitchStateData {HitSound = "Switch", 
                    InvokeID=0,
                    StateColor=Color.Red,
                    StateImage=null},
                    new SwitchStateData {HitSound="Switch",
                        InvokeID = 0,
                        StateColor = Color.Blue,
                        StateImage=null}

                };
            flInitialized = true;

        }

        

        public SwitchBlockMulti(SwitchBlockMulti clonethis):base(clonethis)
        {
            _SwitchData = clonethis.SwitchData;
            _State = clonethis.State;
            _Locked = clonethis.Locked;
            flInitialized = true;

        }
        public override object Clone()
        {
            return new SwitchBlockMulti(this);
        }
    }
    


    //SwitchBlock is a block that... uh... switches.
    //it switches on and off.
    //when hit, it switches on and off. When the switchmode is set to GroupMode, the given TriggerID is only fired
    //when all other SwitchBlocks in the set are in the On state.

    /// <summary>
    /// A Block that fires triggers when switching states. TriggerID dictates the Trigger to invoke.
    /// </summary>

    [Serializable]
    [ImpactEffectBlockCategory]
    public class SwitchBlock : Block 
    {
        public override bool Destructable
        {
            get
            {
                return false;
            }
            set
            {
                base.Destructable = value;
            }
        }
        [Flags]
        public enum SwitchModeConstants
        {
            /*
            /// <summary>
            /// This block will fire it's trigger when it is activated.
            /// </summary>
            SwitchMode_SingleBlock=2,
            /// <summary>
            /// Trigger will be fired once when all SwitchBlocks with the same triggerID in the game are set to On.
            /// </summary>
             * */
            SwitchMode_MultipleBlock=4,
            /*/// <summary>
            /// Trigger will only fire once. Switch blocks with this ID will be destroyed.
            /// </summary>
            SwitchMode_SingleTrigger=8, */
            /// <summary>
            /// Trigger can fire multiple times.
            /// </summary>
            SwitchMode_MultiTrigger=16
        }

        private Image ActiveImage = null;
        private Image InactiveImage = null;

        protected Color _ActiveColor = Color.Lime;
        protected Color _InactiveColor = Color.DimGray;
        protected bool _Active = false;
        protected bool _Locked = false;
        protected int _AllActiveID = 0;
        const String cActiveSound = "Switch_Active";
        const String cInactiveSound = "Switch_Inactive";

        protected String _ActiveSound = cActiveSound;
        protected String _InactiveSound = cInactiveSound;
        public String ActiveSound { get { return _ActiveSound; } set { _ActiveSound = value; } }
        public String InactiveSound { get { return _InactiveSound; } set { _InactiveSound = value; } }

        /// <summary>
        /// TriggerID to fire when all applicable blocks are activated.
        /// </summary>
        public int AllActiveID { get { return _AllActiveID; } set { _AllActiveID = value; } }
        protected int _AllInactiveID = 0;
        public int AllInactiveID { get { return _AllInactiveID; } set { _AllInactiveID = value; } }
        public int ID(bool Active)
        {

            return Active ? AllActiveID : AllInactiveID;

        }
        public bool Locked { get { return _Locked; } set { _Locked = value; } }
        private SwitchModeConstants _SwitchMode = 0;
        public Color ActiveColor { get { return _ActiveColor;} set {_ActiveColor =value; RebuildImages();}}
        public Color InactiveColor { get { return _InactiveColor; } set {_InactiveColor = value; RebuildImages();}}
        public bool Active { get { return _Active; } set { if (_Locked) return;  _Active = value; } }
        
        public SwitchModeConstants SwitchMode { get { return _SwitchMode; } set { _SwitchMode = value; } }

     
     
        



        public SwitchBlock(RectangleF blockrect)
        {
            base.BlockRectangle = blockrect;
            OnBlockRectangleChange+=new Action<RectangleF>(SwitchBlock_OnBlockRectangleChange);    
        }
        public SwitchBlock(SwitchBlock clonethis)
            : base(clonethis)
        {
            base.BlockRectangle = clonethis.BlockRectangle;
            ActiveSound = clonethis.ActiveSound;
            InactiveSound = clonethis.InactiveSound;
            _ActiveColor = clonethis.ActiveColor;
            _InactiveColor = clonethis.InactiveColor;
            _Locked = clonethis.Locked;
            _Active = clonethis.Active;
            _SwitchMode = clonethis.SwitchMode;
            _AllActiveID = clonethis.AllActiveID;
            _AllInactiveID = clonethis.AllInactiveID;
            

            Active = clonethis.Active;

            OnBlockRectangleChange+=new Action<RectangleF>(SwitchBlock_OnBlockRectangleChange);    
        }
        public SwitchBlock(SerializationInfo info, StreamingContext context):base(info,context)
        {
            ActiveSound = info.GetString("ActiveSound");
            InactiveSound = info.GetString("InactiveSound");
            ActiveColor = info.GetValue<Color>("ActiveColor");
            InactiveColor = info.GetValue<Color>("InactiveColor");
            Active = info.GetBoolean("Active");
            SwitchMode = info.GetValue<SwitchModeConstants>("SwitchMode");
            AllActiveID = info.GetInt32("AllActiveID");
            AllInactiveID = info.GetInt32("AllInactiveID");



            OnBlockRectangleChange += new Action<RectangleF>(SwitchBlock_OnBlockRectangleChange); 
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info,context);
            info.AddValue("ActiveSound", ActiveSound);
            info.AddValue("InactiveSound", InactiveSound);
            info.AddValue("ActiveColor", ActiveColor);
            info.AddValue("InactiveColor", InactiveColor);
            info.AddValue("Active", Active);
            info.AddValue("SwitchMode", SwitchMode);
            info.AddValue("AllActiveID", _AllActiveID);
            info.AddValue("AllInactiveID", _AllInactiveID);

            

        }
    void  SwitchBlock_OnBlockRectangleChange(RectangleF obj)
    {
 	    RebuildImages();
    }
        private void RebuildImages()
        {

            Image tActiveImage = new Bitmap((int)BlockRectangle.Width, (int)BlockRectangle.Height);
            Image tInactiveImage = new Bitmap((int)BlockRectangle.Width, (int)BlockRectangle.Height);
            using (Graphics A = Graphics.FromImage(tActiveImage))
            {
                using (Graphics I = Graphics.FromImage(tInactiveImage))
                {

                Image gummyActive = BCBlockGameState.GetGummyImage(ActiveColor, tActiveImage.Size);
                Image gummyInactive = BCBlockGameState.GetGummyImage(InactiveColor, tInactiveImage.Size);
                Image SwitchOverlay = BCBlockGameState.Imageman.getLoadedImage("SwitchOverlay");

                A.DrawImageUnscaled(gummyActive, 0, 0);
                I.DrawImageUnscaled(gummyInactive, 0, 0);
                A.DrawImage(SwitchOverlay, 0, 0,tActiveImage.Width,tActiveImage.Height);
                I.DrawImage(SwitchOverlay, 0, 0,tInactiveImage.Width,tInactiveImage.Height);

                ActiveImage = tActiveImage;
                InactiveImage = tInactiveImage;
                }
            }

        }
        public override void EditorDraw(Graphics g, IEditorClient Client)
        {
            base.EditorDraw(g,Client);
            String drawstring="";
            if(_AllActiveID!=0) drawstring += "A:" + _AllActiveID + "\n";
            if(_AllInactiveID!=0) drawstring += "I:" + _AllInactiveID + _AllInactiveID + "\n";
            //draw our trigger number.
            Font usefont = BCBlockGameState.GetScaledFont(new Font(BCBlockGameState.GetMonospaceFont(), 12), (int)(BlockRectangle.Height / 2));
            var textsize = BCBlockGameState.MeasureString(drawstring, usefont);

            PointF DrawLocation = new PointF(CenterPoint().X - textsize.Width / 2,
                CenterPoint().Y - textsize.Height / 2);



        }

        public override string GetToolTipInfo(IEditorClient Client)
        {
            String currbuild = base.GetToolTipInfo(Client);

            //add some information about this switch.
            //(In group with X others)
            //find the others in our group.
            //we can group based on AllActive and AllInActive.

            //first find Active group..

            var ActiveGroup = from m in Client.GetBlocks() where m != this 
                                  && m is SwitchBlock && (m as SwitchBlock).AllActiveID == AllActiveID select m;
            var InActiveGroup = from m in Client.GetBlocks() where m != this &&
                                    m is SwitchBlock && (m as SwitchBlock).AllInactiveID == AllInactiveID select m;
            StringBuilder buildstr = new StringBuilder();
            if (ActiveGroup.Any())
            {
                buildstr.AppendLine("ActiveID #" + AllActiveID + " With " + ActiveGroup.Count() + " Others.");


            }


            if (InActiveGroup.Any())
            {
                buildstr.AppendLine("ActiveID #" + AllInactiveID + " With " + InActiveGroup.Count() + " Others.");


            }
            




            return buildstr.ToString() + currbuild;
        }
        public override object Clone()
        {
            return new SwitchBlock(this);
        }
        public override void Draw(Graphics g)
        {
            if(ActiveImage ==null || InactiveImage==null)
            {
                RebuildImages();

            }
            Image useimage = Active ? ActiveImage : InactiveImage;
            g.DrawImageUnscaled(useimage, BlockRectangle_int);
        }
        protected override void StandardSpray(BCBlockGameState parentstate, cBall ballhit)
        {
            
            //add lightorbs, too!
            //one in each cardinal direction.
            double usespeed = 4;
            double currangle = 0;
            double angleincrement = Math.PI / 4;
            while (currangle < Math.PI * 2)
            {
                PointF velocityuse = new PointF((float)(Math.Cos(currangle) * usespeed), (float)(Math.Sin(currangle) * usespeed));
                LightOrb makeorb = new LightOrb(BlockRectangle.CenterPoint(), ActiveColor, 24);
                makeorb.Velocity = velocityuse;
                parentstate.Particles.Add(makeorb);
              
                currangle += angleincrement;
            }

        }
        public override bool PerformBlockHit(BCBlockGameState parentstate, cBall ballhit)
        {
            
            if (_Locked) return false;
            Active = !Active;
            _hasChanged = true;

            BCBlockGameState.Soundman.PlaySound(Active ? ActiveSound : InactiveSound);
            int IDcheck = Active ? AllActiveID : AllInactiveID;

            IEnumerable<SwitchBlock> dealblocks=null;
            bool triggered = false;
            bool? triggerbool = null;
            
            if ((SwitchMode & SwitchModeConstants.SwitchMode_MultipleBlock) != SwitchModeConstants.SwitchMode_MultipleBlock)
            {
                //we need to deal with only this block.
                dealblocks = new SwitchBlock[] { this};
            }
            else if ((SwitchMode & SwitchModeConstants.SwitchMode_MultipleBlock)==SwitchModeConstants.SwitchMode_MultipleBlock)
            {
                //we need to work with all blocks in the set that are SwitchBlocks and have the same ID.
                

                dealblocks = (from t in parentstate.Blocks where t is SwitchBlock && (t as SwitchBlock).ID(Active) == IDcheck select (SwitchBlock)t);
            }
            if (dealblocks == null || !dealblocks.Any())
            {
                //uhhh...
                return false;

            }
            // are all dealblocks activated?
            if (AllActiveID != 0)
            {
                if (dealblocks.All((t) => t.Active))
                {
                    triggered = true;
                    triggerbool = true; //all active.
                    BCBlockGameState.Soundman.PlaySound("laser2");
                    Trigger.InvokeTriggerID(AllActiveID, parentstate);

                }
            }
            if (AllInactiveID != 0)
            {
                if (dealblocks.All((t) => !t.Active))
                {
                    triggered = true;
                    triggerbool = false;
                    BCBlockGameState.Soundman.PlaySound("laser2");
                    Trigger.InvokeTriggerID(AllInactiveID, parentstate);
                }

            }
            if (triggered) //only destroy or reset if we were triggered.
            {
                if ((SwitchMode & SwitchModeConstants.SwitchMode_MultiTrigger) != SwitchModeConstants.SwitchMode_MultiTrigger)
                {
                    //we fire once. if activated, destroy all the blocks we worked with.

                    parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                    {
                        var removethese = dealblocks.ToList();
                        foreach (var destroythis in removethese)
                        {
                            destroythis.StandardSpray(parentstate,null);
                            parentstate.Blocks.Remove(destroythis);
                            
                        }
                        parentstate.Forcerefresh = true;
                    }));

                }
                else if ((SwitchMode & SwitchModeConstants.SwitchMode_MultiTrigger) == SwitchModeConstants.SwitchMode_MultiTrigger)
                {
                    //can fire multiple times. Set the state of all affected blocks to false. 
                    //We will do this on a time delay, however, and set their "Locked" property to true, preventing
                    //this routine from changing their active state in the future until the delay expires.
                    foreach (var anullthis in dealblocks)
                    {
                        anullthis.Locked = true;
                    }
                    parentstate.DelayInvoke(new TimeSpan(0, 0, 0, 1), (a) =>
                    {
                        parentstate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                        {
                            foreach (var unlockit in dealblocks)
                            {
                                unlockit.Active = !(triggerbool.Value);  //invert to old value.
                                unlockit.Locked = false;


                            }
                            parentstate.Forcerefresh = true;
                        }
                        ));
                    });
                }
            }
            return false; //don't destroy.
        }
    }
}
