using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using BASeCamp.BASeBlock.Blocks;


namespace BASeCamp.BASeBlock
{
    public abstract class UndoStackItem:ICloneable 
    {
        private readonly DateTime _DateTime;
        public String Description { get; set; }
        public DateTime DataTime { get { return _DateTime; } }
        protected UndoStackItem(String pDescription, DateTime pDataTime)
        {
            _DateTime = pDataTime;
            Description = pDescription;

        }
        protected UndoStackItem(UndoStackItem clonethis)
        {
            _DateTime = clonethis.DataTime;
            Description = clonethis.Description;

        }
        public abstract object Clone();
    }
    public class EditorUndoStackItem : UndoStackItem
    {
        private List<Block> _storedblocks;
        private List<cBall> _storedballs;

        public List<Block> StoredBlocks { get { return _storedblocks; } set { _storedblocks = value; } }
        public List<cBall> StoredBalls { get { return _storedballs; } set { _storedballs = value; } }

        public EditorUndoStackItem(EditorUndoStackItem source):base(source)
        {
            _storedblocks = source.StoredBlocks.Clone();
            _storedballs = source.StoredBalls.Clone();


        }
        /// <summary>
        /// constructs an UndoStackitem.
        /// </summary>
        /// <param name="storeblocks">Blocks to store</param>
        /// <param name="storeballs">balls to store</param>
        public EditorUndoStackItem(List<Block> storeblocks, List<cBall> storeballs, String pDescription)
            : base(pDescription, DateTime.Now)
        {
            _storedblocks = storeblocks.Clone();
            _storedballs = storeballs.Clone();
            Description = pDescription;


        }
        public override string ToString()
        {
            return Description + " (" + (DateTime.Now - base.DataTime).FriendlyString() + " Ago)";
        }

        public override object Clone()
        {
            return new EditorUndoStackItem(this);
        }
    }


    //Editor Undo
    public class BBEditorUndo<T> where T:UndoStackItem 
    {
       
        private int _maxsize = 10;
        private readonly Stack<T> _UndoStack= new Stack<T>();
        private readonly Stack<T> _RedoStack= new Stack<T>(); 

        public BBEditorUndo(int maxsize)
        {
            
            _maxsize = maxsize;

        }
        public Stack<T> GetUndoStack()
        {
            return _UndoStack;

        }
        public Stack<T> GetRedoStack()
        {
            return _RedoStack;

        }
        public bool CanUndo { get { return _UndoStack.Count > 0; } }
        public bool CanRedo { get { return _RedoStack.Count > 0; } }
        public void PushChange(T changeItem)
        {

            T additem = (T)((changeItem as ICloneable).Clone());
            
            _UndoStack.Push(additem);
            _UndoStack.TrimTo(_maxsize);
            _RedoStack.Clear();
        }
        /// <summary>
        /// Pops the top item off the Undo stack, pushes it onto the Redo stack, and returns it.
        /// </summary>

        /// <returns></returns>
        public T Undo()
        {
            if (CanUndo)
            {
                T usi = _UndoStack.Pop();
                //push it onto the redo stack...
                _RedoStack.Push(usi);
                return usi;
            }
            else
            {
                return null;
            }
        }
        public T Redo()
        {
            if (CanRedo)
            {

                T usi = _RedoStack.Pop();
                //push to the undo stack...
                _UndoStack.Push(usi);
                return usi;

            }
            else
            {
                return null;
            }


        }
        
        /// <summary>
        /// returns the item that would be undone if an Undo operation was performed.
        /// does not pop the item from the Undo stack.
        /// </summary>
        /// <returns>The UndoStackItem that is at the top of the Undo stack; null of the Undo stack is empty.</returns>
        public T PeekUndo()
        {
            if (!CanUndo) return null;
            return _UndoStack.Peek();

        }
        public T PeekRedo()
        {
            if (!CanRedo) return null;
            return _RedoStack.Peek();

        }
    }
}
