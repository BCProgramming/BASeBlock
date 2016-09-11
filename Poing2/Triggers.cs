using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Xml.Linq;
using BASeCamp.BASeBlock;
using BASeCamp.BASeBlock.Blocks;
using BASeCamp.BASeBlock.Events;
using BASeCamp.Elementizer;
using Timer = System.Threading.Timer;

namespace BASeCamp.BASeBlock
{
    /*the BASeBlock Trigger and Event framework!

    * A Trigger is something that 'invokes' an event. Usually, this will be used by other classes to supply the capacity to trigger things.
    * Example being the BlockTrigger class which invokes a given Trigger ID when the block to which it is attached is destroyed or hit (depending on the class options).
    * 
    * Events
     * 
     * Events are things that happen. There are events to force the player to lose a life, to kill all objects in the game field, and to finish the level and various other events. This is the meat of the framework. More events means more possibilities to use
     * the framework for things that I hadn't even foreseen, by connecting triggers to Events; Add to this the ability to even create your own events and triggers via script and you have a veritable cornucopia of capabilities.
    * Trigger IDs
    * All triggers and Events have a Trigger or EventID. These can be considered a "Channel"; when a Trigger with a given ID is triggered, all events that have that ID are invoked.
     * 
     * 
     
   
    


    */

    public abstract class BaseMultiEditor : CustomCollectionForm
    {

        public delegate void OnEditCompleteFunc(Object[] completededit);
        public static event OnEditCompleteFunc OnEditComplete;

        protected BaseMultiEditor(Type ptypeof):base(ptypeof)
        {


        }
        
        public void InvokeEditComplete(Object[] finishwith)
        {
            OnEditCompleteFunc copyto = OnEditComplete;
            if(copyto!=null)
            {
                copyto.Invoke(finishwith);
                


            }
        }

        protected override bool CanSelectMultipleInstances()
        {
            return true;
        }
        protected override object SetItems(object editValue, object[] value)
        {
            if (editValue != null)
            {
                Debug.Print("in TriggerCollectionEditor::SetItems- editvalue type=" + editValue.GetType().FullName +
                            " count of values:" + value.Length.ToString());
                if (value.Length > 3)
                {
                    Debug.Print("greater than 3");

                }
            }
            InvokeEditComplete(value);
            return base.SetItems(editValue, value);

        }

    }
    public class BallBehaviourCollectionEditor : BaseMultiEditor
    {
        public BallBehaviourCollectionEditor(Type ptypeof)
            : base(ptypeof)
        {


        }
        protected override Type[] CreateNewItemTypes()
        {
            return BCBlockGameState.BallBehaviourManager.ManagedTypes.ToArray();
        }

        protected override string GetDisplayText(object value)
        {
            return value.GetType().FullName;
        }

    }
 
    //some collection editors for them...
    public class BlockEventCollectionEditor : BaseMultiEditor 
    {



        public BlockEventCollectionEditor(Type ptypeof)
            : base(ptypeof)
        {
            //this.ControlAdded += new System.Windows.Forms.ControlEventHandler(TriggerEventCollectionEditor_ControlAdded);


        }
        protected override Type[] CreateNewItemTypes()
        {
            //forcibly add the blockevent to the list before returning it. The code In LoadedTypeManager() and the multi variant Assumes that
            //the type given is not instantiatible.
            var returnval = BCBlockGameState.BlockEventTypes.ManagedTypes;
            returnval.Add(typeof(BlockEvent));
            return returnval.ToArray();
        }
        protected override string GetDisplayText(object value)
        {
            return value.GetType().FullName;
        }
    }
    public class BlockTriggerEventCollectionEditor : BlockEventCollectionEditor
    {
        public BlockTriggerEventCollectionEditor(Type ptypeof):base(ptypeof)
        {


        }
        protected override Type[] CreateNewItemTypes()
        {
            //only allow classes derived from BlockEvent, rather then from TriggerEvent...
            Type[] returnitems = (from p in base.CreateNewItemTypes() where p== typeof(BlockEvent) ||  p.IsSubclassOf(typeof(BlockEvent)) select p).ToArray();
            return returnitems;
        }

        protected override string GetDisplayText(object value)
        {
            return value.ToString();
        }
    }
    public class TriggerCollectionEditor : BaseMultiEditor 
    {
        public TriggerCollectionEditor(Type ptype):base(ptype)
        {
            

        }

        protected override Type[] CreateNewItemTypes()
        {
            
            return BCBlockGameState.TriggerTypes.ManagedTypes.ToArray();
            //return base.CreateNewItemTypes();
        }
        
    }
    public class TimedTriggerDataCollectionEditor : BaseMultiEditor
    {
        public TimedTriggerDataCollectionEditor(Type ptype)
            : base(ptype)
        {

        }
        protected override Type[] CreateNewItemTypes()
        {
            return new Type[] {typeof(TimedTriggers.TimedTriggerData)};
        

            }

    }
    public class EventCollectionEditor : BaseMultiEditor
    {
        public EventCollectionEditor(Type ptype)
            : base(ptype)
        {


        }
        protected override Type[] CreateNewItemTypes()
        {
            return BCBlockGameState.EventTypes.ManagedTypes.ToArray();
        }



    }
    public class EnemyTriggerCollectionEditor : BaseMultiEditor
    {
        public EnemyTriggerCollectionEditor(Type ptype)
            : base(ptype)
        {
        }

        protected override Type[]  CreateNewItemTypes()
{



 	            var returnval =BCBlockGameState.EnemyTriggerTypes.ManagedTypes;
                returnval.Add(typeof(EnemyTrigger));
                return returnval.ToArray() ;
}


       }




    public class BlockTriggerCollectionEditor : BaseMultiEditor
    {

        public BlockTriggerCollectionEditor(Type ptype):base(ptype)
        {


        }
        protected override Type[]  CreateNewItemTypes()
        {
            //Type[] returnarray = (from p in base.CreateNewItemTypes() where p==typeof(BlockTrigger) || p.IsSubclassOf(typeof(BlockTrigger)) select p).ToArray(); 
            return BCBlockGameState.BlockTriggerTypes.ManagedTypes.ToArray();
 	         
        }
        protected override string GetDisplayText(object value)
        {
            BlockTrigger bt = value as BlockTrigger;
            return base.GetDisplayText(value);
        }
        
    }
    /// <summary>
    /// event that kills all enemies.
    /// </summary>
    [Serializable]
    public class KillAllObjectsEvent : TriggerEvent
    {

        private bool _TriggerLess = false;
        /// <summary>
        /// If true, will forcibly remove all GameObjects
        /// </summary>
        public bool TriggerLess { get { return _TriggerLess; } set { _TriggerLess = value; } }
        [EditorBrowsable(EditorBrowsableState.Never)]
        private String DestroyTypeString = "";
        private Type _DestroyType = null;
        [Editor(typeof(GameObjectTypeEditor), typeof(UITypeEditor))]
        public Type DestroyType
        {
            get { return _DestroyType; }
            set
            {
                _DestroyType = value;
                DestroyTypeString = _DestroyType==null?"":_DestroyType.Name;


            }
        }


        public KillAllObjectsEvent()
        {


        }
        public KillAllObjectsEvent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            try { TriggerLess = info.GetBoolean("Triggerless"); }
            catch { }
            try { DestroyTypeString = info.GetString("DestroyType"); }catch { }
            
            DestroyType = String.IsNullOrEmpty(DestroyTypeString)?null: BCBlockGameState.FindClass(DestroyTypeString);

        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Triggerless", _TriggerLess);
            info.AddValue("DestroyType", DestroyType.Name);
        }
        
        public KillAllObjectsEvent(KillAllObjectsEvent clonethis)
        {

            _TriggerLess = clonethis.TriggerLess;
            DestroyTypeString = clonethis.DestroyTypeString;
            DestroyType = String.IsNullOrEmpty(DestroyTypeString) ? null : BCBlockGameState.FindClass(DestroyTypeString);

        }
        public override void InvokeEvent(BCBlockGameState gamestate)
        {
            List<GameObject> Removeem = new List<GameObject>();
            List<GameObject> Addem = new List<GameObject>();
            foreach (GameObject iterateobject in gamestate.GameObjects)
            {

                List<GameObject> discard = new List<GameObject>();
                if (iterateobject.GetType() == this.DestroyType)
                {
                    if (!TriggerLess)
                    {
                        if (iterateobject is GameEnemy)
                        {
                            ((GameEnemy)iterateobject).InvokeOnDeath(gamestate, ref Removeem, ref Addem);

                        }

                    }
                    Removeem.Add(iterateobject);
                }


            }


            foreach (var removeit in Removeem)
            {

                gamestate.GameObjects.Remove(removeit);
            }
            foreach (var addit in Addem)
            {
                gamestate.GameObjects.AddLast(addit);

            }



        }
        public override object Clone()
        {
            return new KillAllObjectsEvent(this);
        }
    }




    [Serializable]
    //event that forces the finishing of the level.
    public class FinishLevelEvent : TriggerEvent
    {
        public FinishLevelEvent(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        public FinishLevelEvent(int pID):base(pID)
        {
        }
        public FinishLevelEvent(FinishLevelEvent copythis)
            : base(copythis)
        {
        }
        public override object Clone()
        {
            return new FinishLevelEvent(this);
        }
        public override void InvokeEvent(BCBlockGameState gamestate)
        {
            gamestate.NextFrameCalls.Enqueue(new BCBlockGameState.NextFrameStartup(() =>
                {
                    List<Block> removethem = new List<Block>(gamestate.Blocks);
                    foreach (var iterate in removethem)
                        gamestate.Blocks.Remove(iterate);

                }
            ));
        }
    }

    [Serializable]
    public class LoseLifeEvent : TriggerEvent
    {
        public LoseLifeEvent(SerializationInfo info, StreamingContext context):base(info,context)
        {


        }
        public LoseLifeEvent(LoseLifeEvent copythis):base(copythis)
        {

            //nuttin.
        }
        public LoseLifeEvent()
        {


        }
        public override void InvokeEvent(BCBlockGameState gamestate)
        {
            //Die. 
            gamestate.ClientObject.ForceDeath();

        }
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        /// <returns>
        /// A new object that is a copy of this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override object Clone()
        {
            return new LoseLifeEvent(this);
        }
    }


    /// <summary>
    /// Triggers 
    /// Triggers and Events are something I created to make it easier to program in and make things in levels like say doors that only open when you hit a certain block or kill a certain enemy.
    /// Trigger is the abstract base class; currently there is one derived class, BlockTrigger, which is a trigger that get's attached to a block. Note that each block
    /// has a List of both triggers and events, so multiple triggers could cause the same event or multiple events could be triggered by a single trigger.
    

    ///BlockTrigger: BlockTrigger get's "attached" to a block (this is done in the OnDeserializationComplete handler for the base "Block" class and in the constructor).
    ///this is done by simply giving the trigger object a blockowner. It is referenced with a weakreference (because the BlockTrigger class will almost always be an aggregate of the Block class).
    ///When set, it hooks the Block's destroy and hit events and will trigger when the apporpriate action occurs matching its BlockTriggerType.
    ///
    /// EnemyTrigger: EnemyTrigger is similar to BlockTrigger, but it attaches itself to an Enemy instead. This is interesting because many enemies are just comprised of blocks and so this is sort of like
    /// an additional layer of indirection, but no matter. It works similarly to BlockTrigger, but with enemies.
    /// </summary>

    

    [Serializable]
    public abstract class TriggerEvent:ISerializable,IComparable<TriggerEvent>,ICloneable ,IEditorExtensions,IXmlPersistable
    {
        //TriggerEvent represents an event that is performed 
        
        /// <summary>
        /// ID if this TriggerEvent
        /// </summary>
        /// 
        //public event Action<TriggerEvent,BCBlockGameState> EventInvoked;
        public event EventHandler<TriggerEventArgs> EventInvoked;
        public int ID { get; set; }
        public virtual void InvokeEvent(BCBlockGameState gamestate)
        {
            if (EventInvoked != null)
            {

                EventInvoked(this, new TriggerEventArgs(gamestate,this));

            }

        }
        public abstract Object Clone();
        protected TriggerEvent()
        {


        }

        protected TriggerEvent(int pID)
        {
            ID=pID;

        }
        protected TriggerEvent(TriggerEvent copyfrom)
        {

            ID = copyfrom.ID;

        }
        #region ISerializable Members

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("ID", ID);   
        }

        protected TriggerEvent(SerializationInfo info, StreamingContext context)
        {
            try{ID = info.GetInt32("ID");} catch{ID = -1;}


        }

        #endregion
        public virtual XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            XElement result = new XElement(pNodeName);
            result.Add(new XAttribute("ID", this.ID));
            return result;
        }
        protected TriggerEvent(XElement Source, Object pPersistenceData)
        {
            this.ID = Source.GetAttributeInt("ID", -1);
        }
        public int CompareTo(TriggerEvent other)
        {
            return ID.CompareTo(other.ID);
        }
        public static IEnumerable<TriggerEvent> getEvents(BCBlockGameState fromstate)
        {
            LevelSet lset = fromstate.ClientObject.GetPlayingSet();
            foreach (var looplevel in lset.Levels)
            {
                foreach (var eventloop in looplevel.LevelEvents)
                {
                    yield return eventloop;
                }
                foreach (var blockloop in looplevel.levelblocks)
                {
                    foreach (var eventloop in blockloop.BlockEvents)
                    {
                        yield return eventloop;

                    }

                }
            }



        }

        public string GetToolTipInfo(IEditorClient Client)
        {
            return this.GetType().Name + ":" + this.ID;
        }

      
    }
    /// <summary>
    /// Event that provides a thin wrapper to allow for detection of Events being fired.
    /// </summary>
    public class EventDetector : TriggerEvent
    {

        public override void InvokeEvent(BCBlockGameState gamestate)
        {
            base.InvokeEvent(gamestate);
        }
        public EventDetector(EventDetector clonethis):base(clonethis)
        {


        }
        public override object Clone()
        {
            return new EventDetector(this);
        }
        public EventDetector(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
        {

        }

    }


    [Serializable]
    public class SpawnEnemyEvent : TriggerEvent
    {
        //Trigger that spawns an enemy.

        [Editor(typeof(ItemTypeEditor<GameEnemy>), typeof(UITypeEditor))]
        public Type EnemyType { get; set; }

        [TypeConverter(typeof(FloatFConverter))]
        public PointF SpawnLocation { get; set; }

        [TypeConverter(typeof(FloatFConverter))]
        public SizeF SpawnSize {get;set;}

        public SpawnEnemyEvent()
        {
            EnemyType = null;
            
            


        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            var Result = base.GetXmlData(pNodeName,pPersistenceData);
            Result.Add(new XAttribute("EnemyType",EnemyType.Name));
            Result.Add(StandardHelper.SaveElement(SpawnLocation,"SpawnLocation",pPersistenceData));
            Result.Add(StandardHelper.SaveElement(SpawnSize,"SpawnSize",pPersistenceData));
            return Result;
        }
        public SpawnEnemyEvent(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
        {
            String EnemyTypeStr = Source.GetAttributeString("EnemyType", null);
            if(EnemyTypeStr!=null)
            {
                EnemyType = BCBlockGameState.FindClass(EnemyTypeStr);
            }
            SpawnLocation = (PointF)Source.ReadElement<PointF>("SpawnLocation");
            SpawnSize = (SizeF)Source.ReadElement<SizeF>("SpawnSize");
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("EnemyType", EnemyType.Name);
            info.AddValue("SpawnLocation", SpawnLocation);
            info.AddValue("SpawnSize", SpawnSize);
        }
        public SpawnEnemyEvent(SerializationInfo info, StreamingContext context)
        {

            String enemytypename = info.GetString("EnemyType");
            EnemyType = BCBlockGameState.FindClass(enemytypename);
            SpawnLocation = (PointF)info.GetValue("SpawnLocation", typeof(PointF));
            SpawnSize = (SizeF)info.GetValue("SpawnSize", typeof(SizeF));


        }
        public SpawnEnemyEvent(Type pEnemyType, PointF pSpawnLocation, SizeF pSpawnSize):this()
        {
            EnemyType = pEnemyType;
            SpawnLocation = pSpawnLocation;
            SpawnSize = pSpawnSize;

        }
        public SpawnEnemyEvent(SpawnEnemyEvent clonethis):base(clonethis)
        {
            EnemyType = clonethis.EnemyType;
            SpawnLocation = clonethis.SpawnLocation;
            SpawnSize = clonethis.SpawnSize;


        }





        public override void InvokeEvent(BCBlockGameState gamestate)
        {
            base.InvokeEvent(gamestate);
            //When invoked... we spawn that enemy at the appropriate position.
            //constructor (PointF,SizeF)
            try
            {
                GameEnemy Spawnedenemy = (GameEnemy)Activator.CreateInstance(EnemyType, new object[] { SpawnLocation, SpawnSize });
                lock (gamestate.GameObjects)
                {

                    gamestate.GameObjects.AddLast(Spawnedenemy);



                }



            }
            catch
            {

                //meh

            }

        }

        public override object Clone()
        {
            return new SpawnEnemyEvent(this);
        }
    }

    [Serializable]
    public class PlaySoundEvent : TriggerEvent
    {
        private String _SoundPlay = "wart";
        private bool _IsMusic=false;
        public String SoundPlay { get { return _SoundPlay; } set { _SoundPlay = value; } }
        public bool IsMusic { get { return _IsMusic; } set { _IsMusic = value; } }



            public override void InvokeEvent(BCBlockGameState gamestate)
        {
            base.InvokeEvent(gamestate);
            if (IsMusic)
                BCBlockGameState.Soundman.PlayMusic(_SoundPlay, true);
            else
                BCBlockGameState.Soundman.PlaySound(_SoundPlay);
                    
                
        }
            public PlaySoundEvent(PlaySoundEvent Clonethis)
                : base(Clonethis)
            {

                _SoundPlay = (string)Clonethis.SoundPlay.Clone();
                _IsMusic = Clonethis.IsMusic;
               



            }
            public override object Clone()
            {
                return new PlaySoundEvent(this);
            }
            public PlaySoundEvent()
            {

            }
            public PlaySoundEvent(SerializationInfo info, StreamingContext context)
                : base(info, context)
            {
                _SoundPlay = info.GetString("SoundPlay");
                _IsMusic = info.GetBoolean("IsMusic");

            }
            public override void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                base.GetObjectData(info, context);
                info.AddValue("SoundPlay", _SoundPlay);
                info.AddValue("IsMusic", _IsMusic);
            }
            public PlaySoundEvent(XElement Source, Object pPersistenceData) :base(Source,pPersistenceData)
            {
                _SoundPlay = Source.GetAttributeString("SoundPlay");
                _IsMusic = Source.GetAttributeBool("IsMusic");
            }

            public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
            {
                var Result = base.GetXmlData(pNodeName,pPersistenceData);
                Result.Add(new XAttribute("SoundPlay",_SoundPlay));
                Result.Add(new XAttribute("IsMusic",_IsMusic));
                return Result;
            }
    }

    [Serializable]
    public class BlockEvent:TriggerEvent
    {
        public enum BlockEventTypeConstants
        {
            /// <summary>
            /// Destroys the block.
            /// </summary>
            BlockEvent_Destroy


        }
        public BlockEventTypeConstants BlockEventType;
        private WeakReference _OwnerBlock;
        public Block OwnerBlock { 
            
            
            get {
                if(_OwnerBlock==null) return null;
            if(_OwnerBlock.IsAlive)
                return (Block)_OwnerBlock.Target;
            

                return null;
            }
            set { _OwnerBlock = new WeakReference(value); }

        }

            public override void GetObjectData(SerializationInfo info,StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("BlockEventType", BlockEventType);
        }
        public BlockEvent(int id, Block ownerblock)
            : base(id)
        {
            _OwnerBlock = new WeakReference(ownerblock);

        }
        public BlockEvent(BlockEvent clonethis)
            : base(clonethis)
        {

            BlockEventType = clonethis.BlockEventType;
            _OwnerBlock = new WeakReference(clonethis.OwnerBlock);


        }
        public override object Clone()
        {
            return new BlockEvent(this);
        }
        public BlockEvent()
        {


        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            var Result = base.GetXmlData(pNodeName,pPersistenceData);
            Result.Add(new XAttribute("EventType",(int)BlockEventType));
            return Result;
        }
        public BlockEvent(XElement Source, Object pPersistenceData) :base(Source, pPersistenceData)
        {
            BlockEventType = (BlockEventTypeConstants)Source.GetAttributeInt("EventType");
        }
        public BlockEvent(SerializationInfo info, StreamingContext context):base(info,context)
        {
            BlockEventType = (BlockEventTypeConstants)info.GetValue("BlockEventType", typeof(BlockEventTypeConstants));


        }

        public override void InvokeEvent(BCBlockGameState gamestate)
        {
            base.InvokeEvent(gamestate);
            //throw new NotImplementedException();
            switch (BlockEventType)
            {
            case BlockEventTypeConstants.BlockEvent_Destroy:
            if (_OwnerBlock == null || !(_OwnerBlock.IsAlive)) return;
            Block destroythis = OwnerBlock;
            if (gamestate.Blocks.Contains(destroythis))
            {
                destroythis.StandardSpray(gamestate);
                gamestate.Blocks.Remove(destroythis);
                gamestate.Forcerefresh = true; //force it to redraw, too.
            }
            else
                {
             Debug.Print("what the heck...");       
                }
                    break;
            }
            
        }



    }
    
    
    



    /// <summary>
    /// TextScrollEvent: Creates a Text GameObject at a given location.
    /// </summary>
    [Serializable]
    public class TextScrollEvent : TriggerEvent
    {
        private String _Text="";
        private bool UseSpawnLocation=false; //flag so we know whether to default to the bottom.
        private PointF _SpawnLocation;
        public String Text { get { return _Text; } set { _Text = value; } }
        [Editor(typeof(FloatFConverter), typeof(UITypeEditor))]
        public PointF SpawnLocation { get { return _SpawnLocation; } set { UseSpawnLocation=true; _SpawnLocation = value; } }

        public TextScrollEvent(TextScrollEvent clonethis):base(clonethis)
        {
            _Text = clonethis.Text;
            UseSpawnLocation = clonethis.UseSpawnLocation;
            SpawnLocation = clonethis.SpawnLocation;



        }
        public override object Clone()
        {
            return new TextScrollEvent(this);
        }
            public override void InvokeEvent(BCBlockGameState gamestate)
        {
//            throw new NotImplementedException();
            PointF SpawnSpot=_SpawnLocation;
            if (!UseSpawnLocation)
            {
                SpawnSpot = new PointF(gamestate.GameArea.Width/2,gamestate.GameArea.Height);


            }
                PointF Velocity = new PointF(0,-1);
                String textshow = _Text;
                Font usefont = new Font("Arial", 14);
                Pen usepen = new Pen(Color.Black,2);
                Brush usebrush = new SolidBrush(Color.White);
                int timetolive = 7500;
            var addedobject = new BasicFadingText(textshow, SpawnSpot, Velocity, usefont, usepen, usebrush, timetolive);
            gamestate.GameObjects.AddLast(addedobject);
            //return addedobject;



        }
    }
    [Serializable]
    public class ToggledRepeatingTrigger : TriggerEvent
    {
        //something of an interesting aggregate Trigger.





        public ToggledRepeatingTrigger(ToggledRepeatingTrigger clonethis):base(clonethis)
        {
            _Interval = clonethis.Interval;
            FiredID = clonethis.FiredID;

        }

        public override object Clone()
        {
            return new ToggledRepeatingTrigger(this);
        }
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info,context);
            info.AddValue("Interval", _Interval);
            info.AddValue("FiredID", FiredID);
        }
        public ToggledRepeatingTrigger(SerializationInfo info,StreamingContext context)
        {
            _Interval = (TimeSpan)info.GetValue("Interval", typeof(TimeSpan));
            FiredID = info.GetInt32("FiredID");

        }

        private TimeSpan _Interval = new TimeSpan(0, 0, 0, 1);
        /// <summary>
        /// interval between firings when this RepeatingTrigger is Enabled.
        /// </summary>
        public TimeSpan Interval { get { return _Interval; } set { _Interval = value; } }
        /// <summary>
        /// ID to fire
        /// </summary>
        public int FiredID { get; set; }
        private bool _Enabled = false;
        /// <summary>
        /// whether this RepeatingTrigger is on or off.
        /// </summary>

        public bool Enabled
        {
            get { return _Enabled; }
            set
            {
        
        _Enabled = true;
        
        } }
        System.Threading.Timer delaytimer = null;
        private void TimerProcedure(Object parameter)
        {
            BCBlockGameState gstate = (BCBlockGameState)parameter;
            //try to do it on the UI thread....
            Form grabform = (Form)gstate.ClientObject;

            grabform.Invoke((MethodInvoker)(()=>{


            Trigger.InvokeTriggerID(FiredID,gstate );

            }));

        }
        public override void InvokeEvent(BCBlockGameState gamestate)
        {
            _Enabled = !_Enabled;

            if (_Enabled)
            {
                //enabled  the timer...
                if (delaytimer != null)
                {
                    delaytimer.Dispose();
                    delaytimer = null;
                    delaytimer = new Timer(TimerProcedure, gamestate, Interval, Interval);
                }
                
            }
            else
            {
                //disable the timer...
                delaytimer.Dispose();
                delaytimer = null;

            }

            //toggle the current state (on or off)... and enabled/disable as needed.

        }
       




    }
    /// <summary>
    /// Manages a list of Time delays and TriggerIDs; when this trigger is invoked, it will subsequently
    /// fire the aggregate triggers in order after the given delay.
    /// </summary>
    [Serializable]
    public class TimedTriggers : Trigger ,ISerializable
    {
        #region inner TimedTriggerData Class
        /// <summary>
        /// Class used for storing the TimedTrigger data.
        /// </summary>
        [Serializable]
        public class TimedTriggerData : ISerializable ,IComparable<TimedTriggerData>,IComparable<TimeSpan>,IXmlPersistable
        {
            private TimeSpan _timedelay;
            private int _TriggerID;

            public TimeSpan TimeDelay { get { return _timedelay; } set { _timedelay = value; } }
            public int TriggerID { get { return _TriggerID; } set { _TriggerID = value; } }

                public TimedTriggerData()
            {
                

            }
            public TimedTriggerData(SerializationInfo info, StreamingContext context)
            {
                _timedelay = (TimeSpan)info.GetValue("TimeDelay", typeof(TimeSpan));
                _TriggerID = info.GetInt32("TriggerID");


            }
            public void GetObjectData(SerializationInfo info, StreamingContext context)
            {
                info.AddValue("TimeDelay", _timedelay);
                info.AddValue("TriggerID", _TriggerID);


            }
            public XElement GetXmlData(String pNodeName,Object pPersistenceData)
            {
                return new XElement(pNodeName,new XAttribute("TimeDelay",_timedelay.Ticks),new XAttribute("TriggerID",TriggerID));
            }
            public TimedTriggerData(XElement Source)
            {
                _timedelay = new TimeSpan(int.Parse(Source.Attribute("TimeDelay").Value));;
                TriggerID = int.Parse(Source.Attribute("TriggerID").Value);

            }
            public TimedTriggerData(TimeSpan Timedelay, int TriggerID)
            {
                _timedelay=Timedelay;
                _TriggerID= TriggerID;


            }


            public int CompareTo(TimedTriggerData other)
            {
                return this.TimeDelay.CompareTo(other.TimeDelay);
            }
            public int CompareTo(TimeSpan other)
            {
                return this.TimeDelay.CompareTo(other);


            }
        }
        #endregion
        private List<TimedTriggerData> _TriggerData = new List<TimedTriggerData>();
        private Stack<TimedTriggerData> TriggerStack;
        private DateTime EventStartTime; //DateTime our 'events' started.

        [Editor(typeof(TimedTriggerDataCollectionEditor), typeof(UITypeEditor))]
        public List<TimedTriggerData> TriggerData { get { return _TriggerData; } set { _TriggerData = value; RefreshStack(); } }


        private void RefreshStack()
        {

            //YAY LINQ... Create a Stack by ordering all the triggers in descending order. The top of the stack will always be the next timed trigger that should fire.
            TriggerStack = new Stack<TimedTriggerData>((from n in _TriggerData orderby n.TimeDelay descending select n).ToList());



        }
        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("TriggerData", _TriggerData);
            
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            XElement baseNode = base.GetXmlData(pNodeName,pPersistenceData);
            baseNode.Add(StandardHelper.SaveList(_TriggerData,"TriggerData",pPersistenceData));
            return baseNode;
        }
        public TimedTriggers(XElement Source,Object pPersistenceData) :base(Source,pPersistenceData)
        {
            _TriggerData = StandardHelper.ReadList<TimedTriggerData>(Source,pPersistenceData);
        }
        public TimedTriggers(SerializationInfo info, StreamingContext context):base(info,context)
        {
            _TriggerData = (List<TimedTriggerData>)info.GetValue("TriggerData", typeof(List<TimedTriggerData>));
            RefreshStack();

        }
        public TimedTriggers()
        {
            RefreshStack();


        }
        public TimedTriggers(TimedTriggers cloneitem):base(cloneitem)
        {

            TriggerData = new List<TimedTriggerData>(cloneitem.TriggerData);


        }

        public override void InvokeTrigger(BCBlockGameState gamestate)
        {
            //throw new NotImplementedException();
            //when invoked, we start "counting". We don't actually trigger anything until the delegate routine get's us too, though.
            //refresh our stack, as well. This way the trigger can be invoked multiple times.
            RefreshStack();
            
            gamestate.Defer(()=>  gamestate.GameObjects.AddLast(new ProxyObject(proxyframe, null)));
            EventStartTime = DateTime.Now;
            
        }
        private bool proxyframe(ProxyObject sourceobject,BCBlockGameState gamestate)
        {

            //peek and grab the topmost item.
            if(!TriggerStack.Any()) return true;
            var topmost = TriggerStack.Peek();

            //should we trigger this Item?
            if ((DateTime.Now - EventStartTime) > topmost.TimeDelay)
            {
                //yep! pop it off...
                TriggerStack.Pop();
                //and invoke that triggerID...
                //InvokeTriggerDirect(gamestatetopmost.TriggerID);
                InvokeTriggerID(topmost.TriggerID, gamestate);


            }


            return false;
        }

        public override object Clone()
        {
            return new TimedTriggers(this);
        }
    }
    
    [Serializable]
    public class SoundStopTrigger : Trigger,ISerializable 
    {

        public String SoundWatch = "";

        public SoundStopTrigger()
        {


        }
        private void inithook()
        {
            BCBlockGameState.MainGameState.GameSoundStop += new BCBlockGameState.GameSoundStopEventFunction(MainGameState_GameSoundStop);

        }

        void MainGameState_GameSoundStop(BCBlockGameState gstate, iSoundSourceObject objstop)
        {
            iSoundSourceObject sourceobj = objstop;
            String usekey = "";
            if (sourceobj != null)
            {
                foreach (var iteratesound in BCBlockGameState.Soundman.SoundSources)
                {
                    if (iteratesound.Value == sourceobj)
                    {
                        usekey = iteratesound.Key;
                        break;
                    }


                }
            }
            if (!String.IsNullOrEmpty(usekey) && sourceobj != null)
            {
                if (usekey == SoundWatch)
                {
                    InvokeTrigger(gstate);
                }
                


            }
        }
        public SoundStopTrigger(XElement Source,Object pPersistenceData) :base(Source,pPersistenceData)
        {
            SoundWatch = Source.Attribute("SoundWatch").Value;
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            XElement baseNode = base.GetXmlData(pNodeName,pPersistenceData);
            baseNode.Add(new XAttribute("SoundWatch",SoundWatch));
            return baseNode;
        }

        public SoundStopTrigger(int pID,String pSoundWatch):this()
        {
           // BCBlockGameState.Soundman.Driver.OnSoundStop += Driver_OnSoundStop;
            SoundWatch = pSoundWatch;
            base.ID = pID;
            inithook();
        }
        public SoundStopTrigger(SerializationInfo info, StreamingContext context):base(info,context)
        {
            SoundWatch = info.GetString("SoundWatch");

            inithook();
        }
        ~SoundStopTrigger()
        {
            //BCBlockGameState.MainGameState.GameSoundStop-=

        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("SoundWatch", SoundWatch);
            

        }
        
 


        public override void InvokeTrigger(BCBlockGameState gamestate)
        {
            base.InvokeTriggerDirect(gamestate);
        }

        public override object Clone()
        {
            return new SoundStopTrigger(ID,SoundWatch);
        }
    }
   
    [Serializable]
    public abstract class Trigger:ISerializable ,ICloneable,IXmlPersistable
    {
        /// <summary>
        /// ID of this trigger. Events with the same ID will be invoked when the condition for this trigger is satisfied.
        /// </summary>
        public int ID { get; set; }
        public abstract void InvokeTrigger(BCBlockGameState gamestate);
        private TimeSpan _TriggerDelay = new TimeSpan(0,0,0,3);
        public TimeSpan TriggerDelay
        {
            get { return _TriggerDelay; }
            set { _TriggerDelay = value; }
        }
        #region ISerializable Members

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Debug.Print("Trigger::GetObjectData");
            info.AddValue("ID", ID);
            info.AddValue("TriggerDelay", TriggerDelay);
        }
        public virtual XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            return new XElement(pNodeName,
                new XAttribute("ID", ID),
                new XAttribute("TriggerDelay", TriggerDelay.Ticks));
        }
        protected Trigger(XElement Source,Object pPersistenceData)
        {
            foreach(XAttribute lookattribute in Source.Attributes())
            {
                if(lookattribute.Name=="ID")
                {
                    ID = int.Parse(lookattribute.Value);
                }
                else if(lookattribute.Name=="TriggerDelay")
                {
                    TriggerDelay = TimeSpan.FromTicks(int.Parse(lookattribute.Value));
                }
            }
        }
        protected Trigger()
        {


        }
        

        protected Trigger(SerializationInfo info, StreamingContext context)
        {
            Debug.Print("Trigger::Serialization Constructor");
            ID = info.GetInt32("ID");
            TriggerDelay = (TimeSpan)info.GetValue("TriggerDelay", typeof(TimeSpan));

        }
        protected void InvokeTriggerDirect(BCBlockGameState gamestate)
        {
            
            //we need the GUI thread... so we create a quick form and Invoke on it. WinForms deals on one thread,
            //so the invoked code should be running there.

           
            
            //invoke trigger directly, with no delays at all.
            List<Block> affectedblocks = new List<Block>();
            lock (gamestate.Blocks)
            {
                foreach (Block loopblock in (from q in gamestate.Blocks where q.BlockEvents.Any((i) => i.ID == ID) select q))
                {
                    affectedblocks.Add(loopblock);
                }
            }

            foreach(Block loopblock in affectedblocks)
            {
            foreach (BlockEvent loopevent in loopblock.BlockEvents)
                {
                    if (loopevent.ID == ID)
                    {
                        loopevent.InvokeEvent(gamestate);

                    }


                }



            }

            //also invoke any relevant level events.
            foreach(var loopvalue in gamestate.PlayingLevel.LevelEvents )
            {
                if (loopvalue.ID == ID)
                {
                    loopvalue.InvokeEvent(gamestate);


                }




            }




            }
        #endregion
        public delegate void PerformTriggerFunc(BCBlockGameState gstate);

        public struct DataTriggerDelay
        {
            public DateTime TriggerInitTime;
            public TimeSpan TriggerDelayLength;
            public PerformTriggerFunc delayfunc;
            public BCBlockGameState gstate;

        }
        public void TriggerDelayRoutine(object dtdobj)
        {
            Object[] casted = (Object[])dtdobj;
            DataTriggerDelay dtd = (DataTriggerDelay)(casted[0]);
            iGameClient igc = (iGameClient)(casted[1]);

          
            

            while(DateTime.Now-dtd.TriggerInitTime < dtd.TriggerDelayLength)
            {
                Thread.Sleep(50);

            }
            //HACK: we know iGameClient really refers to frmBaseBlock, so we cast to a Form and call Invoke...
            Form casttoform = (Form)igc;
            //call it using the Form Invoke, so it happens on that thread... rather than this one.
            Thread t;




            try
            {
                casttoform.Invoke((MethodInvoker)(() => dtd.delayfunc(dtd.gstate)));
            }
            catch (InvalidOperationException exx)
            {
                //occurs if game is closed while trigger is waiting to fire.
                //since the game is closing, doesn't really matter. swallow the exception in ignorant bliss.

            }
            
        }
        private Thread TriggerDelayThread;
        public void InvokeTriggerWithDelay(BCBlockGameState withstate)
        {
            DataTriggerDelay usedtd = new DataTriggerDelay();
            usedtd.TriggerInitTime=DateTime.Now;
            usedtd.gstate=withstate;
            usedtd.delayfunc = InvokeTriggerDirect;
            usedtd.TriggerDelayLength = this.TriggerDelay;
            TriggerDelayThread = new Thread(TriggerDelayRoutine);
            TriggerDelayThread.Start(new Object[] {usedtd,withstate.ClientObject});
            
        }
        private static IEnumerable<int> IntNumbers()
        {
            int x = 0;
            while (true)
                yield return x++;


        }
        //retrieves the first available trigger/event channel ID. This means it is not used by any objects at all.
        public static int GetAvailableID(BCBlockGameState gstate)
        {
            int x;
            IEnumerable<int> usedIDs = from p in getAllTriggers(gstate) select p.ID;
            usedIDs = usedIDs.Concat(from p in TriggerEvent.getEvents(gstate) select p.ID);

            return IntNumbers().FirstOrDefault((e) => !usedIDs.Contains(e));


        }
        public static IEnumerable<Trigger> getAllTriggers(BCBlockGameState gstate)
        {
            //leveltriggers.
            var lset = gstate.ClientObject.GetPlayingSet();
            foreach (var iterate in lset.Levels)
            {
                foreach (Block iterateblock in iterate.levelblocks)
                {
                    foreach (var looptrigger in iterateblock.BlockTriggers)
                        yield return looptrigger;

                }


            }



        }
        public static List<TriggerEvent> InvokeTriggerID(int IDInvoke,BCBlockGameState stateuse)
        {
            List<TriggerEvent> returnlist = new List<TriggerEvent>();
            //use stateuse.PlayingLevel, invoke any matching triggers there as well.
            foreach (TriggerEvent loopevent in stateuse.PlayingLevel.LevelEvents)
            {
                if (loopevent.ID == IDInvoke)
                {
                    
                    loopevent.InvokeEvent(stateuse);
                    returnlist.Add(loopevent);

                }



            }
            var TriggeredBlocks = (from q in stateuse.Blocks where q.BlockEvents.Any((p) => p.ID == IDInvoke) select q).ToList();

            foreach(Block loopblock in TriggeredBlocks)
            {
                foreach (BlockEvent loopevent in loopblock.BlockEvents)
                {
                    if (loopevent.ID == IDInvoke)
                    {
                        loopevent.InvokeEvent(stateuse);
                        returnlist.Add(loopevent);

                    }


                }


            }


            return returnlist;

        }


        #region ICloneable Members

        protected Trigger(Trigger clonethis)
        {
            ID = clonethis.ID;
            TriggerDelay = clonethis.TriggerDelay;
            


        }

        public abstract object Clone();
  


        #endregion
    }
    [Serializable]
    public class EnemyTrigger : Trigger
    {
        private WeakReference _ourenemy;

        public GameEnemy OurEnemy
        {
            get
            {
                if (_ourenemy == null) return null;
                if (!_ourenemy.IsAlive) return null;
                return (GameEnemy) _ourenemy.Target;
            }
            set
                {
                
                    if (value == null) return;
                    if (_ourenemy != null)
                    {
                        if (_ourenemy.IsAlive)
                        {
                            if (_ourenemy.Target == value) return; //same value...
                            GameEnemy casted = (GameEnemy)_ourenemy.Target;
                            casted.OnDeath -= enemy_OnDeath;
                            

                        }


                    }
                    _ourenemy = new WeakReference(value);
                    value.OnDeath += enemy_OnDeath;



                }
            }
        public override object Clone()
        {
            return new EnemyTrigger(this);
        }
        void enemy_OnDeath(Object sender, EnemyDeathEventArgs e)
        {
            Debug.Print("enemy death detected...");
            InvokeTriggerWithDelay(e.StateObject);
        }



        public EnemyTrigger(EnemyTrigger clonethis):base(clonethis)
        {
            OurEnemy = clonethis.OurEnemy;


        }
        public EnemyTrigger()
        {


        }
        public EnemyTrigger(GameEnemy pOurEnemy,int pID, TimeSpan pDelay)
        {
            OurEnemy = pOurEnemy;
            this.ID = pID;
            this.TriggerDelay = pDelay;
        }
        public override string ToString()
        {
            return "EnemyTrigger {\n" +
            "ID = " + this.ID.ToString() + "\n" +
            "Delay = " + this.TriggerDelay.ToString() + "\n";

            


        }
        public override void InvokeTrigger(BCBlockGameState gamestate)
        {
            //throw new NotImplementedException();
        }
        #region serialization
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);

        }
        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            return base.GetXmlData(pNodeName,pPersistenceData);
        }
        public EnemyTrigger(XElement Source,Object pPersistenceData) :base(Source,pPersistenceData)
        {

        }
        public EnemyTrigger(SerializationInfo info, StreamingContext context):base(info,context)
        {



        }

        #endregion
    }
   

    [Serializable]
    public class BlockTrigger : Trigger,ICloneable ,IEditorExtensions 
    {
        public enum BlockTriggerTypeConstants
        {
            BlockTriggerType_Destroy,
            BlockTriggerType_Hit,
            BlockTriggerType_Last_Destroyed

        }
        public virtual BlockTriggerTypeConstants BlockTriggerType { get; set; }
        private WeakReference _OwnerBlock;
        private DateTime InitialTriggerTime;
        //public TimeSpan TriggerDelay { get; set; }
        private Thread DelayTriggerThread;
        
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            Debug.Print("BlockTrigger::GetObjectData");
            base.GetObjectData(info, context);
            info.AddValue("BlockTriggerType", BlockTriggerType);
            
        }

        public override XElement GetXmlData(String pNodeName,Object pPersistenceData)
        {
            XElement basenode = base.GetXmlData(pNodeName,pPersistenceData);
            basenode.Add(new XAttribute("BlockTriggerType",(int)BlockTriggerType));

            return basenode;
        }

        public BlockTrigger(SerializationInfo info, StreamingContext context):base(info,context)
        {
            Debug.Print("BlockTrigger::Serialization Constructor");
            BlockTriggerType = (BlockTriggerTypeConstants)info.GetValue("BlockTriggerType", typeof(BlockTriggerTypeConstants));
        }
        public BlockTrigger(XElement Source,Object pPersistenceData) :base(Source,pPersistenceData)
        {
            BlockTriggerType = (BlockTriggerTypeConstants) Source.GetAttributeInt("BlockTriggerType");
        }
        public BlockTrigger(int pID, TimeSpan pTriggerDelay,Block blockowner)
        {
            ID=pID;
            TriggerDelay = pTriggerDelay;
            if(blockowner!=null) OwnerBlock=blockowner;

        }
        public BlockTrigger()
        {


        }
      
        public Block OwnerBlock
        {


            get
            {
                if (_OwnerBlock == null) return null;
                if (_OwnerBlock.IsAlive)
                    return (Block)_OwnerBlock.Target;


                return null;
            }
            set {
                if ((_OwnerBlock!=null)&&_OwnerBlock.IsAlive)
                {
                    Block casted = (Block)_OwnerBlock.Target;
                    casted.OnBlockHit -= TriggerBlock_OnBlockHit;
                    casted.OnBlockDestroy -= TriggerBlock_OnBlockDestroy;


                }
                else if (value != null)
                {
                    _OwnerBlock = new WeakReference(value);

                    value.OnBlockHit += TriggerBlock_OnBlockHit;
                    value.OnBlockDestroy += TriggerBlock_OnBlockDestroy;
                }

            }

        }

        void TriggerBlock_OnBlockDestroy(Object Sender,BlockHitEventArgs<bool> e)
        {
            //throw new NotImplementedException();
            
            Debug.Print("TriggerBlock detected OnBlockDestroy...");

            if (BlockTriggerType == BlockTriggerTypeConstants.BlockTriggerType_Destroy)
            {
                InvokeTriggerWithDelay(e.GameState);


            }
            else if (BlockTriggerType == BlockTriggerTypeConstants.BlockTriggerType_Last_Destroyed)
            {
                //if there are no blocks in the gamestate that aren't this block, then trigger the event.
                if (!e.GameState.Blocks.Any((w) => w.GetType() == e.TheBlock.GetType() && w != e.TheBlock))
                {

                    InvokeTriggerWithDelay(e.GameState);

                }


            }

            e.Result = false;
        }

        void TriggerBlock_OnBlockHit(Object sender,BlockHitEventArgs<bool> e)
        {
            //throw new NotImplementedException();
            Debug.Print("TriggerBlock Detected OnBlockHit");
            if (BlockTriggerType == BlockTriggerTypeConstants.BlockTriggerType_Hit)
            {
                InvokeTriggerWithDelay(e.GameState);

            }
            e.Result = false;
        }



        private void PerformTrigger(BCBlockGameState gamestate)
        {


            Debug.Print("BlockTrigger 'Performing Trigger...'");
        }



        public override void InvokeTrigger(BCBlockGameState gamestate)
        {
            //
            Debug.Print("InvokeTrigger called in BlockTrigger ID=" + ID);
            /*
            InitialTriggerTime=DateTime.Now;
            DelayTriggerThread = new Thread(TriggerDelayRoutine);
            DelayTriggerThread.Start(gamestate);
             * */
            base.InvokeTriggerWithDelay(gamestate);
        }

        #region IDeserializationCallback Members

       

        #endregion

        #region ICloneable Members
        public BlockTrigger(BlockTrigger clonethis)
            : base(clonethis)
        {
            OwnerBlock = clonethis.OwnerBlock;
            this.BlockTriggerType = clonethis.BlockTriggerType;
        }

        public override object Clone()
        {
            return new BlockTrigger(this);
        }

        #endregion

        public string GetToolTipInfo(IEditorClient Client)
        {
            return this.GetType().Name + ":" + this.ID;
        }
    }

    

}
