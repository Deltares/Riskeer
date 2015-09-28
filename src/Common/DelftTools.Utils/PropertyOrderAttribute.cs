using System;

namespace DelftTools.Utils
{
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertyOrderAttribute : Attribute
    {
        //
        // Simple attribute to allow the order of a property to be specified
        //
        private readonly int order;

        public PropertyOrderAttribute(int order)
        {
            this.order = order;
        }

        public int Order
        {
            get { return order; }
        }
    }
}