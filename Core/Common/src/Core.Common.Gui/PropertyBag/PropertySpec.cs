using System;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace Core.Common.Gui.PropertyBag
{
    /// <summary>
    /// This class represents a single property.
    /// </summary>
    public class PropertySpec
    {
        private readonly System.Reflection.PropertyInfo propertyInfo;

        /// <summary>
        /// Initializes a new instance of the <see cref="PropertySpec"/> class for a given
        /// property meta-data object.
        /// </summary>
        /// <param name="propertyInfo">The property information.</param>
        /// <exception cref="ArgumentException">When <paramref name="propertyInfo"/> is 
        /// an index property.</exception>
        public PropertySpec(System.Reflection.PropertyInfo propertyInfo)
        {
            if (propertyInfo.GetIndexParameters().Length > 0)
            {
                throw new ArgumentException("Index properties are not allowed.", "propertyInfo");
            }

            this.propertyInfo = propertyInfo;
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

        /// <summary>
        /// Sets the property represented by this instance of some object instance.
        /// </summary>
        /// <param name="instance">The instance to be updated.</param>
        /// <param name="newValue">The new value for the property.</param>
        /// <exception cref="ArgumentException">When
        /// <list type="bullet">
        /// <item>Represented property is an index-property.</item>
        /// <item>Property setter is not available.</item>
        /// <item><paramref name="instance"/> does not match the target type.</item>
        /// <item>Property is an instance property but <paramref name="instance"/> is null.</item>
        /// <item><paramref name="newValue"/> is of incorrect type.</item>
        /// <item>An error occurred while setting 
        /// the property value. The <see cref="Exception.InnerException"/> property 
        /// indicates the reason for the error.</item>
        /// </list></exception>
        /// <exception cref="InvalidOperationException">Calling this method while property
        /// has no setter.</exception>
        public void SetValue(object instance, object newValue)
        {
            var setMethodInfo = propertyInfo.GetSetMethod();
            if (setMethodInfo == null)
            {
                throw new InvalidOperationException("Property lacks public setter!");
            }

            try
            {
                setMethodInfo.Invoke(instance, new[]
                {
                    newValue
                });
            }
            catch (TargetException e)
            {
                object type = instance == null ? null : instance.GetType();
                var message = string.Format("Are you calling SetValue on the correct instance? Expected '{0}', but was '{1}'",
                                            propertyInfo.DeclaringType, type);
                throw new ArgumentException(message, "instance", e);
            }
            catch (TargetInvocationException e)
            {
                var message = string.Format("Something when wrong while setting property with value '{0}'; Check InnerException for more information.",
                                            newValue);
                throw new ArgumentException(message, "newValue", e);
            }
        }
    }
}