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
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Properties;
using Ringtoets.Common.Forms.TypeConverters;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="IUseBreakWater"/>.
    /// </summary>
    public class UseBreakWaterProperties
    {
        private const int useBreakWaterPropertyIndex = 1;
        private const int breakWaterTypePropertyIndex = 2;
        private const int breakWaterHeightPropertyIndex = 3;
        private readonly IUseBreakWater data;
        private readonly IPropertyChangeHandler changeHandler;

        /// <summary>
        /// Creates a new instance of <see cref="UseBreakWaterProperties"/>, in which
        /// all the properties are read only.
        /// </summary>
        public UseBreakWaterProperties() { }

        /// <summary>
        /// Creates a new instance of <see cref="UseBreakWaterProperties"/>in which the 
        /// properties are editable.
        /// </summary>
        /// <param name="useBreakWaterData">The data to use for the properties.</param>
        /// <param name="handler">Optional handler that is used to handle property changes.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input parameter is <c>null</c>.</exception>
        public UseBreakWaterProperties(IUseBreakWater useBreakWaterData, IPropertyChangeHandler handler)
        {
            if (useBreakWaterData == null)
            {
                throw new ArgumentNullException(nameof(useBreakWaterData));
            }
            data = useBreakWaterData;
            changeHandler = handler;
        }

        [DynamicReadOnly]
        [PropertyOrder(useBreakWaterPropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "BreakWater_UseBreakWater_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWater_UseBreakWater_Description")]
        public bool UseBreakWater
        {
            get
            {
                return data != null && data.UseBreakWater;
            }
            set
            {
                data.UseBreakWater = value;
                NotifyPropertyChanged();
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(breakWaterTypePropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterType_Description")]
        [TypeConverter(typeof(NullableEnumTypeConverter))]
        public BreakWaterType? BreakWaterType
        {
            get
            {
                if (data != null && data.BreakWater != null)
                {
                    return data.BreakWater.Type;
                }
                return null;
            }
            set
            {
                if (value.HasValue)
                {
                    data.BreakWater.Type = value.Value;
                    NotifyPropertyChanged();
                }
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(breakWaterHeightPropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterHeight_Description")]
        [TypeConverter(typeof(NoValueRoundedDoubleConverter))]
        public RoundedDouble BreakWaterHeight
        {
            get
            {
                if (data != null && data.BreakWater != null)
                {
                    return data.BreakWater.Height;
                }
                return RoundedDouble.NaN;
            }
            set
            {
                data.BreakWater.Height = value;
                NotifyPropertyChanged();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return data == null ||
                   !propertyName.Equals(TypeUtils.GetMemberName<UseBreakWaterProperties>(i => i.UseBreakWater)) &&
                   !UseBreakWater;
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