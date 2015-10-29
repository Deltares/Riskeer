using System;

namespace Core.Common.Utils.PropertyBag
{
    /// <summary>
    /// Represents a single property in a PropertySpec.
    /// </summary>
    public class PropertySpec
    {
        /// <summary>
        /// Initializes a new instance of the PropertySpec class.
        /// </summary>
        /// <param name="name">The name of the property displayed in the property grid.</param>
        /// <param name="type">A Type that represents the type of the property.</param>
        public PropertySpec(string name, Type type)
        {
            Name = name;
            TypeName = type.AssemblyQualifiedName;
        }

        /// <summary>
        /// Gets or sets a collection of additional Attributes for this property. This can
        /// be used to specify attributes beyond those supported intrinsically by the
        /// PropertySpec class, such as ReadOnly and Browsable.
        /// </summary>
        public Attribute[] Attributes { get; set; }

        /// <summary>
        /// Gets or sets the name of this property.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Gets or sets the fully qualfied name of the type of this
        /// property.
        /// </summary>
        public string TypeName { get; private set; }
    }
}