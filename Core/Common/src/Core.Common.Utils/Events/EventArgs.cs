using System;

namespace Core.Common.Utils.Events
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