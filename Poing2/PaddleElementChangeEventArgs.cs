using System;

namespace BASeBlock
{
    public class PaddleElementChangeEventArgs<T> : EventArgs
    {
        private T _OldValue;
        private T _NewValue;
        private bool _Cancel;
        private Paddle _Source;
        public T OldValue { get { return _OldValue; } set { _OldValue = value; } }
        public T NewValue { get { return _NewValue; } set { _NewValue = value; } }
        public bool Cancel { get { return _Cancel; } set { _Cancel = value; } }
        public Paddle Source { get { return _Source; } set { _Source = value; } }
        public PaddleElementChangeEventArgs(Paddle pSource, T oldvalue,T newvalue)
        {
            _OldValue = oldvalue;
            _NewValue = newvalue;
            _Source = pSource;
        }


    }
}