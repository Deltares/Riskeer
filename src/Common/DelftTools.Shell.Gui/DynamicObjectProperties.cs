using System.Linq;
using DelftTools.Utils.ComponentModel;
using DelftTools.Utils.PropertyBag.Dynamic;

namespace DelftTools.Shell.Gui
{
    /// <summary>
    /// Provides a dynamic implementation of IObjectProperties (a property object for a domain object), 
    /// showing only properties which have the 'PropertyGrid' Attribute.
    /// </summary>
    public class DynamicObjectProperties : DynamicPropertyBag, IObjectProperties
    {
        private object data;

        public object Data
        {
            get { return data; }
            set 
            { 
                data = value;

                Initialize(value, value.GetType().GetProperties()
                                       .Where(pi => pi.GetCustomAttributes(typeof(PropertyGridAttribute), false).Any())
                                       .ToArray());
            }
        }
    }
}