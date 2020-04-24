// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using MacroStabilityInwardsDataResources = Riskeer.MacroStabilityInwards.Data.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of water stress lines properties in <see cref="MacroStabilityInwardsInput"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsWaterStressLinesProperties : ObjectProperties<MacroStabilityInwardsInput>
    {
        private const int waternetExtremePropertyIndex = 1;
        private const int waternetDailyPropertyIndex = 2;

        private readonly RoundedDouble assessmentLevel;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsWaterStressLinesProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <param name="assessmentLevel">The assessment level at stake.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsWaterStressLinesProperties(MacroStabilityInwardsInput data, RoundedDouble assessmentLevel)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.data = data;
            this.assessmentLevel = assessmentLevel;
        }

        [PropertyOrder(waternetExtremePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(MacroStabilityInwardsDataResources.Extreme_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaterStressLines_Extreme_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsWaternetProperties WaternetExtreme
        {
            get
            {
                return new MacroStabilityInwardsWaternetProperties(DerivedMacroStabilityInwardsInput.GetWaternetExtreme(data, assessmentLevel));
            }
        }

        [PropertyOrder(waternetDailyPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(MacroStabilityInwardsDataResources.Daily_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaterStressLines_Daily_Description))]
        [TypeConverter(typeof(ExpandableObjectConverter))]
        public MacroStabilityInwardsWaternetProperties WaternetDaily
        {
            get
            {
                return new MacroStabilityInwardsWaternetProperties(DerivedMacroStabilityInwardsInput.GetWaternetDaily(data));
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}