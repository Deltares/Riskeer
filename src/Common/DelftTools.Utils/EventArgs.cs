using System;

namespace DelftTools.Utils
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T t)
        {
            this.Value = t;
        }

        public T Value { get; set; }
    }
}