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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Gui.Attributes;
using Core.Common.Gui.Converters;
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Riskeer.MacroStabilityInwards.Forms.Properties;
using Riskeer.MacroStabilityInwards.Primitives;

namespace Riskeer.MacroStabilityInwards.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of waternet line properties in <see cref="MacroStabilityInwardsWaternetLine"/> for properties panel.
    /// </summary>
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class MacroStabilityInwardsWaternetLineProperties : ObjectProperties<MacroStabilityInwardsWaternetLine>
    {
        private const int namePropertyIndex = 1;
        private const int geometryPropertyIndex = 2;
        private const int phreaticLineNamePropertyIndex = 3;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsWaternetLineProperties"/>.
        /// </summary>
        /// <param name="data">The data of the properties.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsWaternetLineProperties(MacroStabilityInwardsWaternetLine data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.data = data;
        }

        [PropertyOrder(namePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Line_Name_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaternetLine_Name_Description))]
        public string Name
        {
            get
            {
                return data.Name;
            }
        }

        [PropertyOrder(geometryPropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.Line_Geometry_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaternetLine_Geometry_Description))]
        [TypeConverter(typeof(ExpandableReadOnlyArrayConverter))]
        public Point2D[] Geometry
        {
            get
            {
                return data.Geometry.ToArray();
            }
        }

        [PropertyOrder(phreaticLineNamePropertyIndex)]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Waterstresses_DisplayName))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.WaternetLine_PiezometricLineName_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.WaternetLine_PiezometricLineName_Description))]
        public string PhreaticLineName
        {
            get
            {
                return data.PhreaticLine.Name;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}