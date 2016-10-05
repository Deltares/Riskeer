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
        private readonly Func<bool> useBreakWaterEnabled;

        /// <summary>
        /// Creates a new instance of <see cref="UseBreakWaterProperties"/>.
        /// </summary>
        /// <param name="useBreakWaterData">The data to use for the properties.</param>
        /// <param name="useBreakWaterEnabled">Function to check if <see cref="UseBreakWater"/> should be read-only or not.</param>
        /// <exception cref="ArgumentNullException">Thrown if any of the input parameters is <c>null</c>.</exception>
        public UseBreakWaterProperties(IUseBreakWater useBreakWaterData, Func<bool> useBreakWaterEnabled)
        {
            if (useBreakWaterData == null)
            {
                throw new ArgumentNullException("useBreakWaterData");
            }
            if (useBreakWaterEnabled == null)
            {
                throw new ArgumentNullException("useBreakWaterEnabled");
            }
            data = useBreakWaterData;
            this.useBreakWaterEnabled = useBreakWaterEnabled;
        }

        [DynamicReadOnly]
        [PropertyOrder(useBreakWaterPropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "BreakWater_UseBreakWater_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWater_UseBreakWater_Description")]
        public bool UseBreakWater
        {
            get
            {
                return data.UseBreakWater;
            }
            set
            {
                data.UseBreakWater = value;
                data.NotifyObservers();
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(breakWaterTypePropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterType_Description")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public BreakWaterType BreakWaterType
        {
            get
            {
                return data.BreakWater == null ? BreakWaterType.Dam : data.BreakWater.Type;
            }
            set
            {
                data.BreakWater.Type = value;
                data.NotifyObservers();
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(breakWaterHeightPropertyIndex)]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterHeight_Description")]
        public RoundedDouble BreakWaterHeight
        {
            get
            {
                return data.BreakWater == null ? (RoundedDouble) double.NaN
                           : data.BreakWater.Height;
            }
            set
            {
                data.BreakWater.Height = value;
                data.NotifyObservers();
            }
        }

        [DynamicReadOnlyValidationMethod]
        public bool DynamicReadOnlyValidationMethod(string propertyName)
        {
            return !useBreakWaterEnabled()
                   || !propertyName.Equals(TypeUtils.GetMemberName<UseBreakWaterProperties>(i => i.UseBreakWater))
                   && !UseBreakWater;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}