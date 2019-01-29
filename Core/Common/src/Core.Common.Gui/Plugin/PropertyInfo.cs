// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.PropertyBag;

namespace Core.Common.Gui.Plugin
{
    /// <summary>
    /// Information for creating object properties for a particular data object.
    /// </summary>
    public class PropertyInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInfo"/> class.
        /// </summary>
        public PropertyInfo()
        {
            CreateInstance = o =>
            {
                var properties = (IObjectProperties) Activator.CreateInstance(PropertyObjectType);
                properties.Data = o;
                return properties;
            };
        }

        /// <summary>
        /// Gets or sets the type of the data to create properties for.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the type of object properties to create.
        /// </summary>
        public Type PropertyObjectType { get; set; }

        /// <summary>
        /// Gets or sets the optional function used to create a new property instance.
        /// The function provided should guarantee that <see cref="IObjectProperties.Data"/> property
        /// for the newly created property instance is set.
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt;Folder, ModelImplementationFolderProperties&gt; 
        /// { 
        ///     CreateInstance = o =&gt; new ModelImplementationFolderProperties 
        ///     { 
        ///          Data = o 
        ///     }
        /// };</code>
        /// </example>
        public Func<object, IObjectProperties> CreateInstance { get; set; }
    }

    /// <summary>
    /// Information for creating object properties for a particular data object.
    /// </summary>
    /// <typeparam name="TObject">The type of the object to create object properties for.</typeparam>
    /// <typeparam name="TProperty">The type of the object properties to create.</typeparam>
    public class PropertyInfo<TObject, TProperty> where TProperty : IObjectProperties
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PropertyInfo{TObject, TProperty}"/> class.
        /// </summary>
        public PropertyInfo()
        {
            CreateInstance = o =>
            {
                var properties = (TProperty) Activator.CreateInstance(PropertyObjectType);
                properties.Data = o;
                return properties;
            };
        }

        /// <summary>
        /// Gets the type of the data to create properties for.
        /// </summary>
        public Type DataType
        {
            get
            {
                return typeof(TObject);
            }
        }

        /// <summary>
        /// Gets the type of object properties to create.
        /// </summary>
        public Type PropertyObjectType
        {
            get
            {
                return typeof(TProperty);
            }
        }

        /// <summary>
        /// Gets or sets the optional function used to create a new property instance.
        /// The function provided should guarantee that <see cref="IObjectProperties.Data"/> property
        /// for the newly created property instance is set.
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt;Folder, ModelImplementationFolderProperties&gt; 
        /// { 
        ///     CreateInstance = o =&gt; new ModelImplementationFolderProperties 
        ///     { 
        ///          Data = o 
        ///     }
        /// };</code>
        /// </example>
        public Func<TObject, TProperty> CreateInstance { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="PropertyInfo{TObject, TProperty}"/> to <see cref="PropertyInfo"/>.
        /// </summary>
        /// <param name="propertyInfo">The property information to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator PropertyInfo(PropertyInfo<TObject, TProperty> propertyInfo)
        {
            return new PropertyInfo
            {
                DataType = typeof(TObject),
                PropertyObjectType = typeof(TProperty),
                CreateInstance = o => propertyInfo.CreateInstance((TObject) o)
            };
        }
    }
}