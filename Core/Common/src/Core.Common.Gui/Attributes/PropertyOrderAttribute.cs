using System;

namespace Core.Common.Gui.Attributes
{
    /// <summary>
    /// Attribute that allows for controlling the order that properties appear in a property grid.
    /// Ordering should occur on ascending order.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PropertyOrderAttribute : Attribute
    {
        private readonly int order;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyOrderAttribute"/> class.
        /// </summary>
        /// <param name="order">The ordering value.</param>
        public PropertyOrderAttribute(int order)
        {
            this.order = order;
        }

        /// <summary>
        /// Gets the ordering value.
        /// </summary>
        public int Order
        {
            get
            {
                return order;
            }
        }
    }
}