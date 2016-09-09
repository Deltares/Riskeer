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

using System.ComponentModel;
using Core.Common.Base.Data;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.PropertyBag;
using Core.Common.Utils;
using Core.Common.Utils.Attributes;
using Core.Common.Utils.Reflection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Revetment.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Revetment.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="WaveConditionsInput.BreakWater"/>.
    /// </summary>
    public class WaveConditionsInputBreakWaterProperties : ObjectProperties<WaveConditionsInput>
    {
        [DynamicReadOnly]
        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "BreakWater_UseBreakWater_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "BreakWater_UseBreakWater_Description")]
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
        [PropertyOrder(2)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "BreakWaterType_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "BreakWaterType_Description")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public BreakWaterType BreakWaterType
        {
            get
            {
                return data.BreakWater.Type;
            }
            set
            {
                data.BreakWater.Type = value;
                data.NotifyObservers();
            }
        }

        [DynamicReadOnly]
        [PropertyOrder(3)]
        [ResourcesDisplayName(typeof(RingtoetsCommonFormsResources), "BreakWaterHeight_DisplayName")]
        [ResourcesDescription(typeof(RingtoetsCommonFormsResources), "BreakWaterHeight_Description")]
        public RoundedDouble BreakWaterHeight
        {
            get
            {
                return data.BreakWater.Height;
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
            if (data.ForeshoreProfile == null)
            {
                return true;
            }

            if (!propertyName.Equals(TypeUtils.GetMemberName<WaveConditionsInputBreakWaterProperties>(i => i.UseBreakWater)))
            {
                return !UseBreakWater;
            }

            return false;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}