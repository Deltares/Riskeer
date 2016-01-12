using System;

namespace Core.Common.Utils.Events
{
    /// <summary>
    /// Event arguments with a particular value.
    /// </summary>
    /// <typeparam name="T">Type of the attached class instance.</typeparam>
    public class EventArgs<T> : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EventArgs{T}"/> class.
        /// </summary>
        /// <param name="t">The attached instance.</param>
        public EventArgs(T t)
        {
            Value = t;
        }

        /// <summary>
        /// Gets or sets the attached instance.
        /// </summary>
        public T Value { get; private set; }
    }
}