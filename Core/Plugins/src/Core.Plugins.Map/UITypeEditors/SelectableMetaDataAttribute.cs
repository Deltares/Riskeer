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
using Core.Components.Gis.Data;

namespace Core.Plugins.Map.UITypeEditors
{
    /// <summary>
    /// Class that represents a <see cref="FeatureBasedMapData.MetaData"/> attribute in the drop down list edit control
    /// for the <see cref="FeatureBasedMapData.MetaData"/>.
    /// </summary>
    public class SelectableMetaDataAttribute
    {
        private readonly string metaDataAttribute;

        /// <summary>
        /// Creates a new instance of <see cref="SelectableMetaDataAttribute"/>.
        /// </summary>
        /// <param name="metaDataAttribute">The meta data attribute.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="metaDataAttribute"/> is <c>null</c>.</exception>
        public SelectableMetaDataAttribute(string metaDataAttribute)
        {
            if (metaDataAttribute == null)
            {
                throw new ArgumentNullException(nameof(metaDataAttribute));
            }

            this.metaDataAttribute = metaDataAttribute;
        }

        /// <summary>
        /// Gets the meta data attribute.
        /// </summary>
        public string MetaDataAttribute
        {
            get
            {
                return metaDataAttribute;
            }
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((SelectableMetaDataAttribute) obj);
        }

        public override int GetHashCode()
        {
            return metaDataAttribute.GetHashCode();
        }

        public override string ToString()
        {
            return metaDataAttribute;
        }

        private bool Equals(SelectableMetaDataAttribute other)
        {
            return Equals(metaDataAttribute, other.MetaDataAttribute);
        }
    }
}