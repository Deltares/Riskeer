// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;

using log4net;

namespace Core.Common.Gui.PropertyBag
{
    /// <summary>
    /// This class represents a single property.
    /// </summary>
    public class PropertySpec
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(PropertySpec));
        private readonly PropertyInfo propertyInfo;

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

            var attributeList = propertyInfo.GetCustomAttributes(true).OfType<Attribute>().ToList();
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
        public Attribute[] Attributes { get; private set; }

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
        /// <item><paramref name="instance"/> does not match the target type.</item>
        /// <item>Property is an instance property but <paramref name="instance"/> is null.</item>
        /// <item><paramref name="newValue"/> is of incorrect type.</item>
        /// <item>An error occurred while setting the property value. The <see cref="Exception.InnerException"/>
        /// property indicates the reason for the error.</item>
        /// </list></exception>
        /// <exception cref="InvalidOperationException">Calling this method while property
        /// has no setter.</exception>
        /// <exception cref="TargetInvocationException">Calling the method resulted in an exception.</exception>
        /// <exception cref="TargetException"><paramref name="instance"/> is <c>null</c> or the
        /// method is not defined on <paramref name="instance"/>.</exception>
        public void SetValue(object instance, object newValue)
        {
            var setMethodInfo = propertyInfo.GetSetMethod();
            if (setMethodInfo == null)
            {
                throw new InvalidOperationException("Property lacks public setter!");
            }

            setMethodInfo.Invoke(instance, new[]
            {
                newValue
            });
        }

        /// <summary>
        /// Gets the property value represented by this instance of some object instance.
        /// </summary>
        /// <param name="instance">The instance that holds the property to be retrieved.</param>
        /// <returns>The property value on <paramref name="instance"/>.</returns>
        /// <exception cref="ArgumentException">When
        /// <list type="bullet">
        /// <item>Represented property is an index-property.</item>
        /// <item><paramref name="instance"/> does not match the target type.</item>
        /// <item>Property is an instance property but <paramref name="instance"/> is null.</item>
        /// <item>An error occurred while setting the property value. The <see cref="Exception.InnerException"/>
        /// property indicates the reason for the error.</item>
        /// </list></exception>
        /// <exception cref="InvalidOperationException">Calling this method while property
        /// has no getter.</exception>
        public object GetValue(object instance)
        {
            var getMethodInfo = propertyInfo.GetGetMethod();
            if (getMethodInfo == null)
            {
                throw new InvalidOperationException("Property lacks public getter!");
            }

            try
            {
                return getMethodInfo.Invoke(instance, new object[0]);
            }
            catch (TargetException e)
            {
                object type = instance == null ? null : instance.GetType();
                var message = string.Format("Are you calling GetValue on the correct instance? Expected '{0}', but was '{1}'",
                                            propertyInfo.DeclaringType, type);
                throw new ArgumentException(message, "instance", e);
            }
            catch (TargetInvocationException e)
            {
                throw new ArgumentException("Something went wrong while getting property; Check InnerException for more information.", "instance", e);
            }
        }

        /// <summary>
        /// Determines whether the captured property is decorated with <see cref="TypeConverterAttribute"/>
        /// that is configured to use <see cref="ExpandableObjectConverter"/>.
        /// </summary>
        /// <returns>Returns <c>true</c> if a <see cref="TypeConverterAttribute"/> is declared using
        /// <see cref="ExpandableObjectConverter"/>, <c>false</c> if no match has been found or when
        /// the type converter inherits from <see cref="ExpandableObjectConverter"/>.</returns>
        /// <remarks>Custom implementations of <see cref="ExpandableObjectConverter"/> is
        /// likely to have behavior that Core.Common.Gui namespace cannot account for. As
        /// such those properties will be considered not having the expandable object type converter.</remarks>
        public bool IsNonCustomExpandableObjectProperty()
        {
            var typeConverterClassName = propertyInfo.GetCustomAttributes(typeof(TypeConverterAttribute), false)
                                                        .OfType<TypeConverterAttribute>()
                                                        .Select(tca => tca.ConverterTypeName)
                                                        .Where(n => !string.IsNullOrEmpty(n));
            foreach (string typeName in typeConverterClassName)
            {
                try
                {
                    var type = Type.GetType(typeName);
                    if (type != null && typeof(ExpandableObjectConverter) == type)
                    {
                        return true;
                    }
                }
                catch (Exception e)
                {
                    if (e is TargetInvocationException || e is ArgumentException ||
                        e is TypeLoadException || e is FileLoadException || e is BadImageFormatException)
                    {
                        log.DebugFormat("Unable to find TypeConverter of type '{0}", typeConverterClassName);
                    }
                    else
                    {
                        throw; // Not expected exception -> Fail fast
                    }
                }
            }
            return false;
        }
    }
}