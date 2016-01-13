using System;
using System.ComponentModel;
using System.Linq;

namespace Core.Common.Gui.PropertyBag
{
    /// <summary>
    /// This class represents a single property.
    /// </summary>
    public class PropertySpec
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySpec"/> class for a given
        /// property meta-data object.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        public PropertySpec(System.Reflection.PropertyInfo propertyInfo)
        {
            Name = propertyInfo.Name;
            TypeName = propertyInfo.PropertyType.AssemblyQualifiedName;

            var attributeList = propertyInfo.GetCustomAttributes(true).OfType<Attribute>().Select(attrib => attrib).ToList();
            if (propertyInfo.GetSetMethod() == null)
            {
                attributeList.Add(new ReadOnlyAttribute(true));
            }
            Attributes = attributeList.ToArray();
        }

        /// <summary>
        /// Gets or sets a collection of additional <see cref="Attribute"/>s for this property. 
        /// This can be used to specify attributes beyond those supported intrinsically by the
        /// <see cref="PropertySpec"/> class, such as <see cref="System.ComponentModel.ReadOnlyAttribute"/> 
        /// and <see cref="BrowsableAttribute"/>.
        /// </summary>
        public Attribute[] Attributes { get; set; }

        /// <summary>
        /// Gets the name of the property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the fully qualified name of the type of this property.
        /// </summary>
        public string TypeName { get; private set; }
    }
}