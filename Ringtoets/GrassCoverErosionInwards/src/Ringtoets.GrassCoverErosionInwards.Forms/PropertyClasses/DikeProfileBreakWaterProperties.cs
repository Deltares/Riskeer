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
using Core.Common.Utils.Attributes;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.Properties;
using Ringtoets.GrassCoverErosionInwards.Forms.TypeConverters;

namespace Ringtoets.GrassCoverErosionInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="DikeProfile.BreakWater"/> for properties panel.
    /// </summary>
    public class DikeProfileBreakWaterProperties : ObjectProperties<DikeProfile>
    {
        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(Resources), "BreakWater_HasBreakWater_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWater_HasBreakWater_Description")]
        public bool HasBreakWater
        {
            get
            {
                return data.HasBreakWater;
            }
        }

        [DynamicVisible]
        [PropertyOrder(2)]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterType_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterType_Description")]
        [TypeConverter(typeof(EnumTypeConverter))]
        public BreakWaterType BreakWaterType
        {
            get
            {
                return data.BreakWater.Type;
            }
        }

        [DynamicVisible]
        [PropertyOrder(3)]
        [ResourcesDisplayName(typeof(Resources), "BreakWaterHeight_DisplayName")]
        [ResourcesDescription(typeof(Resources), "BreakWaterHeight_Description")]
        public RoundedDouble BreakWaterHeight
        {
            get
            {
                return data.BreakWater.Height;
            }
        }

        [DynamicVisibleValidationMethod]
        public bool DynamicVisibleValidationMethod(string propertyName)
        {
            return data.HasBreakWater;
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}