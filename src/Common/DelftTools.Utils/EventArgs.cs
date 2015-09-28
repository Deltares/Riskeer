using System;

namespace DelftTools.Utils
{
    public class EventArgs<T> : EventArgs
    {
        private T t;
        public EventArgs(T t)
        {
            this.t = t;
        }

        public T Value
        {
            get { return t; }
            set { t = value; }
        }
    }
}
