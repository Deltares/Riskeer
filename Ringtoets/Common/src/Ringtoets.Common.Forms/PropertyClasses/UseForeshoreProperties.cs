// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Utils.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="IUseForeshore"/>.
    /// </summary>
    public class UseForeshoreProperties
    {
        private const int useForeshorePropertyIndex = 1;
        private const int coordinatesPropertyIndex = 2;
        private readonly IUseForeshore data;
        private readonly IPropertyChangeHandler changeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="UseForeshoreProperties"/>, in which
        /// all the properties are read only.
        /// </summary>
        public UseForeshoreProperties() { }

        /// <summary>
        /// Creates a new instance of <see cref="UseForeshoreProperties"/>.
        /// </summary>
        /// <param name="useForeshoreData">The data to use for the properties. </param>
        /// <param name="handler">Optional handler that is used to handle property changes.</param>
        /// <remarks>If <paramref name="useForeshoreData"/> is <c>null</c>, all properties will 
        /// be set to <see cref="ReadOnlyAttribute"/>.</remarks>
        public UseForeshoreProperties(IUseForeshore useForeshoreData, IPropertyChangeHandler handler)
        {
            if (useForeshoreData == null)
            {
                throw new ArgumentNullException(nameof(useForeshoreData));
            }
            data = useForeshoreData;
            changeHandler = handler;
        }

        [DynamicReadOnly]
        [PropertyOrder(useForeshorePropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "Foreshore_UseForeshore_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Foreshore_UseForeshore_Description")]
        public bool UseForeshore
        {
            get
            {
                return data != null && data.UseForeshore;
            }
            set
            {
                data.UseForeshore = value;
                NotifyPropertyChanged();
            }
        }

        [PropertyOrder(coordinatesPropertyIndex)]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        [ResourcesDisplayName(typeof(Resources), "Geometry_Coordinates_DisplayName")]
        [ResourcesDescription(typeof(Resources), "Geometry_Coordinates_Description")]
        public Point2D[] Coordinates
        {
            get
            {
                return data != null && data.ForeshoreGeometry != null ?
                           data.ForeshoreGeometry.ToArray() :
                           null;
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return data == null || data.ForeshoreGeometry == null || data.ForeshoreGeometry.Count() < 2;
        }

        public override string ToString()
        {
            return string.Empty;
        }

        private void NotifyPropertyChanged()
        {
            if (changeHandler != null)
            {
                changeHandler.PropertyChanged();
            }
            data.NotifyObservers();
        }
    }
}