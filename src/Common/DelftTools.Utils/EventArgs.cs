using System;

namespace DelftTools.Utils
{
    public class EventArgs<T> : EventArgs
    {
        public EventArgs(T t)
        {
            Value = t;
        }

        public T Value { get; set; }
    }
}