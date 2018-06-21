﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Core.Common.Gui.Converters;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of an enumeration of <see cref="HydraulicBoundaryLocation"/> with design water level results for properties panel.
    /// </summary>
    public class DesignWaterLevelCalculationsGroupProperties : HydraulicBoundaryLocationCalculationsGroupBaseProperties
    {
        /// <inheritdoc />
        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationsGroupProperties"/>.
        /// </summary>
        public DesignWaterLevelCalculationsGroupProperties(IEnumerable<HydraulicBoundaryLocation> locations,
                                                           IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary) 
            : base(locations, calculationsPerCategoryBoundary) {}

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Locations_Description))]
        public DesignWaterLevelHydraulicBoundaryLocationProperties[] Locations
        {
            get
            {
                return data.Select(location => new DesignWaterLevelHydraulicBoundaryLocationProperties(
                                       location, GetHydraulicBoundaryLocationCalculationsForLocation(location))).ToArray();
            }
        }
    }
}