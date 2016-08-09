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
using Core.Common.Gui.PropertyBag;

namespace Core.Common.Gui.Plugin
{
    /// <summary>
    /// Information for creating object properties for a particular data object.
    /// </summary>
    public class PropertyInfo
    {
        /// <summary>
        /// Gets or sets the type of the data to create properties for.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the type of object properties to create.
        /// </summary>
        public Type PropertyObjectType { get; set; }

        /// <summary>
        /// Gets or sets the optional function used to determine if this instance is relevant 
        /// given a data object.
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt; Folder, ModelImplementationFolderProperties &gt; { AdditionalDataCheck = o =&gt; GetParent(o) is ModelImplementation };</code>
        /// </example>
        /// <remarks>
        /// This property breaks the single responsibility principle; besides <see cref="DataType"/> and <see cref="PropertyObjectType"/> an additional method is 
        /// introduced to determine whether or not property information is relevant in a specfic context.
        /// </remarks>
        public Func<object, bool> AdditionalDataCheck { get; set; }

        /// <summary>
        /// Gets or sets the optional function that allows a data object to be converted 
        /// to another data object that should be used as the property object instead.
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt;ModelImplementation, ModelImplementationProperties&gt; { GetObjectPropertiesData = o =&gt; o.RunParameters };</code>
        /// </example>
        public Func<object, object> GetObjectPropertiesData { get; set; }

        /// <summary>
        /// Gets or sets the optional function that allows for post-creation logic to be 
        /// executed on the newly created object properties.
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt; ModelImplementation, ModelImplementationProperties &gt; { AfterCreate = op =&gt; op.AdditionalBooleanProperty = true };</code>
        /// </example>
        public Action<object> AfterCreate { get; set; }
    }

    /// <summary>
    /// Information for creating object properties for a particular data object.
    /// </summary>
    /// <typeparam name="TObject">The type of the object to create object properties for.</typeparam>
    /// <typeparam name="TProperty">The type of the object properties to create.</typeparam>
    public class PropertyInfo<TObject, TProperty> where TProperty : IObjectProperties
    {
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
        /// Gets or sets the optional function used to determine if this instance is relevant 
        /// given a data object.
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt; Folder, ModelImplementationFolderProperties &gt; { AdditionalDataCheck = o =&gt; GetParent(o) is ModelImplementation };</code>
        /// </example>
        /// <remarks>
        /// This property breaks the single responsibility principle; besides <see cref="DataType"/> and <see cref="PropertyObjectType"/> an additional method is 
        /// introduced to determine whether or not property information is relevant in a specfic context.
        /// </remarks>
        public Func<TObject, bool> AdditionalDataCheck { get; set; }

        /// <summary>
        /// Gets or sets the optional function that allows a data object to be converted 
        /// to another data object that should be used as the property object instead.
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt;ModelImplementation, ModelImplementationProperties&gt; { GetObjectPropertiesData = o =&gt; o.RunParameters };</code>
        /// </example>
        public Func<TObject, object> GetObjectPropertiesData { get; set; }

        /// <summary>
        /// Gets or sets the optional function that allows for post-creation logic to be 
        /// executed on the newly created object properties.
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt; ModelImplementation, ModelImplementationProperties &gt; { AfterCreate = op =&gt; op.AdditionalBooleanProperty = true };</code>
        /// </example>
        public Action<TProperty> AfterCreate { get; set; }

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
                AdditionalDataCheck = propertyInfo.AdditionalDataCheck != null
                                          ? o => propertyInfo.AdditionalDataCheck((TObject) o)
                                          : (Func<object, bool>) null,
                GetObjectPropertiesData = propertyInfo.GetObjectPropertiesData != null
                                              ? o => propertyInfo.GetObjectPropertiesData((TObject) o)
                                              : (Func<object, object>) null,
                AfterCreate = propertyInfo.AfterCreate != null
                                  ? op => propertyInfo.AfterCreate((TProperty) op)
                                  : (Action<object>) null
            };
        }
    }
}