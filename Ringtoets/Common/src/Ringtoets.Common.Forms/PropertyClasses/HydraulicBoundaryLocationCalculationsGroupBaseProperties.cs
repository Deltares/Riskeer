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
using System.Linq;
using Core.Common.Gui.PropertyBag;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Forms.PropertyClasses
{
    /// <summary>
    /// Base ViewModel of a collection of <see cref="HydraulicBoundaryLocation"/> with calculations per category boundary for properties panel.
    /// </summary>
    public abstract class HydraulicBoundaryLocationCalculationsGroupBaseProperties : ObjectProperties<IEnumerable<HydraulicBoundaryLocation>>
    {
        private readonly IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary;

        /// <summary>
        /// Creates a new instance of <see cref="HydraulicBoundaryLocationCalculationsGroupBaseProperties"/>.
        /// </summary>
        /// <param name="locations">The collection of hydraulic boundary locations to create properties for.</param>
        /// <param name="calculationsPerCategoryBoundary">A collection of tuples containing the category boundary name and
        /// its corresponding collection of <see cref="HydraulicBoundaryLocationCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        protected HydraulicBoundaryLocationCalculationsGroupBaseProperties(
            IEnumerable<HydraulicBoundaryLocation> locations,
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

        protected IEnumerable<Tuple<string, HydraulicBoundaryLocationCalculation>> GetHydraulicBoundaryLocationCalculationsForLocation(HydraulicBoundaryLocation location)
        {
            return calculationsPerCategoryBoundary.Select(
                boundaryCalculations =>
                    new Tuple<string, HydraulicBoundaryLocationCalculation>(
                        boundaryCalculations.Item1,
                        boundaryCalculations.Item2.Single(calculation => ReferenceEquals(calculation.HydraulicBoundaryLocation, location)))).ToArray();
        }
    }
}