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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System;

namespace Core.Common.Gui
{
    /// <summary>
    /// Information for creating object properties
    /// </summary>
    public class PropertyInfo
    {
        /// <summary>
        /// The type of the object to create properties for
        /// </summary>
        public Type ObjectType { get; set; }

        /// <summary>
        /// The type of object properties to create
        /// </summary>
        public Type PropertyType { get; set; }

        /// <summary>
        /// Function for determining whether or not the property information is relevant in a specfic context 
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt; Folder, ModelImplementationFolderProperties} { AdditionalDataCheck = o =&gt; GetParent(o) is ModelImplementation };</code>
        /// </example>
        /// <remarks>
        /// This property breaks the single responsibility principle; besides <see cref="ObjectType"/> and <see cref="PropertyType"/> an additional method is 
        /// introduced to determine whether or not property information is relevant in a specfic context.
        /// </remarks>
        public Func<object, bool> AdditionalDataCheck { get; set; }

        /// <summary>
        /// Function for obtaining the data that should be set while creating object properties
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt;ModelImplementation, ModelImplementationProperties&gt; { GetObjectPropertiesData = o =&gt; o.RunParameters };</code>
        /// </example>
        public Func<object, object> GetObjectPropertiesData { get; set; }

        /// <summary>
        /// Action that must be performed after creating object properties
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo &lt; ModelImplementation, ModelImplementationProperties &gt; { AfterCreate = op =&gt; op.AdditionalBooleanProperty = true };</code>
        /// </example>
        public Action<object> AfterCreate { get; set; }
    }

    /// <summary>
    /// Information for creating object properties
    /// </summary>
    /// <typeparam name="TObject">The type of the object to create object properties for</typeparam>
    /// <typeparam name="TProperty">The type of the object properties to create</typeparam>
    public class PropertyInfo<TObject, TProperty> where TProperty : IObjectProperties
    {
        /// <summary>
        /// The type of the object to create properties for
        /// </summary>
        public Type ObjectType
        {
            get
            {
                return typeof(TObject);
            }
        }

        /// <summary>
        /// The type of object properties to create
        /// </summary>
        public Type PropertyType
        {
            get
            {
                return typeof(TProperty);
            }
        }

        /// <summary>
        /// Function for determining whether or not the property information is relevant in a specfic context 
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>
        /// var propertyInfo = new PropertyInfo &lt; Folder, ModelImplementationFolderProperties&gt; { AdditionalDataCheck = o =&gt; GetParent(o) is ModelImplementation };
        /// </code>
        /// </example>
        /// <remarks>
        /// This property breaks the single responsibility principle; besides <see cref="ObjectType"/> and <see cref="PropertyType"/> an additional method is 
        /// introduced to determine whether or not property information is relevant in a specfic context.
        /// </remarks>
        public Func<TObject, bool> AdditionalDataCheck { get; set; }

        /// <summary>
        /// Function for obtaining the data that should be set while creating object properties
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>
        /// var propertyInfo = new PropertyInfo &lt;ModelImplementation, ModelImplementationProperties&gt; { GetObjectPropertiesData = o =&gt; o.RunParameters };
        /// </code></example>
        public Func<TObject, object> GetObjectPropertiesData { get; set; }

        /// <summary>
        /// Action that must be performed after creating object properties
        /// </summary>
        /// <example>
        /// As an example, you could implement this as follows:
        /// <code>var propertyInfo = new PropertyInfo&lt; ModelImplementation, ModelImplementationProperties&gt; { AfterCreate = op =&gt; op.AdditionalBooleanProperty = true };</code>
        /// </example>
        public Action<TProperty> AfterCreate { get; set; }

        public static implicit operator PropertyInfo(PropertyInfo<TObject, TProperty> pi)
        {
            return new PropertyInfo
            {
                ObjectType = typeof(TObject),
                PropertyType = typeof(TProperty),
                AdditionalDataCheck = pi.AdditionalDataCheck != null
                                          ? o => pi.AdditionalDataCheck((TObject) o)
                                          : (Func<object, bool>) null,
                GetObjectPropertiesData = pi.GetObjectPropertiesData != null
                                              ? o => pi.GetObjectPropertiesData((TObject) o)
                                              : (Func<object, object>) null,
                AfterCreate = pi.AfterCreate != null
                                  ? op => pi.AfterCreate((TProperty) op)
                                  : (Action<object>) null
            };
        }
    }
}