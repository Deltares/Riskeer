using System;

namespace Core.Common.Gui.PropertyBag
{
    /// <summary>
    /// Provides data for the GetValue and SetValue events of the PropertyBag class.
    /// </summary>
    public class PropertySpecEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the PropertySpecEventArgs class.
        /// </summary>
        /// <param name="property">The PropertySpec that represents the property whose
        /// value is being requested or set.</param>
        /// <param name="val">The current value of the property.</param>
        public PropertySpecEventArgs(PropertySpec property, object val)
        {
            Property = property;
            Value = val;
        }

        /// <summary>
        /// Gets the PropertySpec that represents the property whose value is being
        /// requested or set.
        /// </summary>
        public PropertySpec Property { get; private set; }

        /// <summary>
        /// Gets or sets the current value of the property.
        /// </summary>
        public object Value { get; set; }
    }
}