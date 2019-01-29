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

using System;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.MacroStabilityInwards.Forms.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of water stresses properties in <see cref="MacroStabilityInwardsWaternet"/> for properties panel.
    /// </summary>
    public class MacroStabilityInwardsWaternetProperties : ObjectProperties<MacroStabilityInwardsWaternet>
    {
        private const int phreaticLinesPropertyIndex = 1;
        private const int waternetLinesPropertyIndex = 2;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsWaternetProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsWaternetProperties(MacroStabilityInwardsWaternet data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.data = data;
        }

        [PropertyOrder(phreaticLinesPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Waternet_PiezometricLines_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Waternet_PiezometricLines_Description))]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        public MacroStabilityInwardsPhreaticLineProperties[] PhreaticLines
        {
            get
            {
                return data.PhreaticLines.Select(pl => new MacroStabilityInwardsPhreaticLineProperties(pl)).ToArray();
            }
        }

        [PropertyOrder(waternetLinesPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Waternet_WaternetLines_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.Waternet_WaternetLines_Description))]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        public MacroStabilityInwardsWaternetLineProperties[] WaternetLines
        {
            get
            {
                return data.WaternetLines.Select(wnl => new MacroStabilityInwardsWaternetLineProperties(wnl)).ToArray();
            }
        }

        public override string ToString()
        {
            return string.Empty;
        }
    }
}