// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Gui.PropertyBag;
using Core.Common.Util.Attributes;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.Properties;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// ViewModel of an enumeration of <see cref="HydraulicBoundaryLocation"/> for properties panel.
    /// </summary>
    public class DesignWaterLevelCalculationsGroupProperties : ObjectProperties<IEnumerable<HydraulicBoundaryLocation>>
    {
        private readonly IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary;

        /// <summary>
        /// Creates a new instance of <see cref="DesignWaterLevelCalculationsGroupProperties"/>.
        /// </summary>
        /// <param name="locations">The locations to set as data.</param>
        /// <param name="calculationsPerCategoryBoundary">A collection of tuples containing the category boundary name and
        /// its corresponding collection of <see cref="HydraulicBoundaryLocationCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public DesignWaterLevelCalculationsGroupProperties(IEnumerable<HydraulicBoundaryLocation> locations,
                                                           IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary)
        {
            if (locations == null)
            {
                throw new ArgumentNullException(nameof(locations));
            }

            if (calculationsPerCategoryBoundary == null)
            {
                throw new ArgumentNullException(nameof(calculationsPerCategoryBoundary));
            }

            this.calculationsPerCategoryBoundary = calculationsPerCategoryBoundary;

            Data = locations;
        }

        [TypeConverter(typeof(ExpandableArrayConverter))]
        [ResourcesCategory(typeof(Resources), nameof(Resources.Categories_General))]
        [ResourcesDisplayName(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Locations_DisplayName))]
        [ResourcesDescription(typeof(Resources), nameof(Resources.HydraulicBoundaryDatabase_Locations_Description))]
        public HydraulicBoundaryLocationProperties[] Locations
        {
            get
            {
                return data.Select(location => new HydraulicBoundaryLocationProperties(location, GetHydraulicBoundaryLocationCalculationsForLocation(location))).ToArray();
            }
        }

        private IEnumerable<Tuple<string, HydraulicBoundaryLocationCalculation>> GetHydraulicBoundaryLocationCalculationsForLocation(HydraulicBoundaryLocation location)
        {
            return calculationsPerCategoryBoundary.Select(boundaryCalculations =>
                                                              new Tuple<string, HydraulicBoundaryLocationCalculation>(
                                                                  boundaryCalculations.Item1,
                                                                  boundaryCalculations.Item2.Single(calculation => ReferenceEquals(calculation.HydraulicBoundaryLocation, location))));
        }
    }
}