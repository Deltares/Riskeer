// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of <see cref="ForeshoreProfile.BreakWater"/> for properties panel.
    /// </summary>
    public class BreakWaterProperties : ObjectProperties<ForeshoreProfile>
    {
        [PropertyOrder(1)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BreakWater_HasBreakWater_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BreakWater_HasBreakWater_Description))]
        public bool HasBreakWater
        {
            get
            {
                return data.HasBreakWater;
            }
        }

        [DynamicVisible]
        [PropertyOrder(2)]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BreakWaterType_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BreakWaterType_Description))]
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
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.BreakWaterHeight_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.BreakWaterHeight_Description))]
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