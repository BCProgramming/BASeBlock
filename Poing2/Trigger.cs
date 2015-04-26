using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BASeBlock
{

    /*
     * how triggers work:
     * Each block will have a Trigger and a TriggerEvent class; The trigger class hooks the block it is connected to and 
     * intercepts it's events for hitting and being destroyed, and when the condition is satisfied, it iterates through all the blocks in the level
     * and invokes the TriggerEvent for all blocks whose triggerEvent id matches the TriggerID of the triggered class.
     * */


    /// <summary>
    /// Class used to represent a trigger. A Trigger is connected to a block, and it is "invoked" when the appropriate conditions are met and performs
    /// the action it has been assigned.
    /// </summary>
    [Serializable]
    public class Trigger :ISerializable 
    {
        public enum TriggerTypeConstants
        {
            TriggerType_Hit,
            TriggerType_Destroy
            


        }
        public TriggerTypeConstants TriggerType {get;set;}
        public int TriggerID{get;set;}
        private WeakReference stateobject;
        private WeakReference _blockobject;
        
        /// <summary>
        /// gamestate cached by the PreInit() method.
        /// </summary>
        //public BCBlockGameState gamestate { get { return (BCBlockGameState)stateobject.Target; } set { stateobject = new WeakReference(value); } }
        //blockobject, also cached in the PreInit() method.
        public Block BlockObject { get { return (Block)_blockobject.Target; } set { _blockobject = new WeakReference(value); } }
        public bool IsInitialized()
        {
            return BlockObject != null ;
        }

        #region ISerializable Members
        public void PreInit(Block initblock)
        {
            BlockObject=initblock;
            BlockObject.OnBlockHit += new Block.OnBlockHitFunc(BlockObject_OnBlockHit);
            BlockObject.OnBlockDestroy += new Block.OnBlockHitFunc(BlockObject_OnBlockDestroy);
        }

        bool BlockObject_OnBlockDestroy(Block block, BCBlockGameState gamestate, cBall ballparam, ref List<cBall> ballsadded, ref bool nodefault)
        {

            if (TriggerType == TriggerTypeConstants.TriggerType_Destroy)
            {
                InvokeTrigger(block,gamestate);


            }
            return true;
        }

        public void InvokeTrigger(Block block, BCBlockGameState gamestate)
        {


        }

        bool BlockObject_OnBlockHit(Block block, BCBlockGameState gamestate, cBall ballparam, ref List<cBall> ballsadded, ref bool nodefault)
        {
            if (TriggerType == TriggerTypeConstants.TriggerType_Hit)
            {
                InvokeTrigger(block, gamestate);

            }
        }

        public Trigger(SerializationInfo info, StreamingContext context)
        {


        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }

        #endregion
    }


    [Serializable]
    public class TriggerEvent:ISerializable
    {
        public enum TriggerEventType
        {
            Event_Destroy,
            Event_ChangeType


        }
        //ID used to trigger this event.
        public enum TriggerEventTypeConstants
        {
            /// <summary>
            /// Destroys the event block.
            /// </summary>
            TriggerEvent_Destroy


        }
        public TriggerEventTypeConstants EventType { get; set; }
        public int EventID { get; set; }


        public TriggerEvent()
        {


        }

        public void DoEvent(Block ourblock,BCBlockGameState gamestate)
        {
            switch(EventType)
                    {
                case TriggerEventTypeConstants.TriggerEvent_Destroy:

                    break;

        }


        #region ISerializable Members
        public TriggerEvent(SerializationInfo info, StreamingContext context)
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //throw new NotImplementedException();
        }

        #endregion
    }

}
