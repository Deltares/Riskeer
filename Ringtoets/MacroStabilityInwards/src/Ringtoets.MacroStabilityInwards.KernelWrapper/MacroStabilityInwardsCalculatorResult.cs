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
using Ringtoets.MacroStabilityInwards.KernelWrapper.Result;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// This class contains all the results of a complete macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsCalculatorResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculatorResult"/>. 
        /// The result will hold all the values which were given.
        /// </summary>
        /// <param name="slidingCurve">The sliding curve result.</param>
        /// <param name="upliftVanCalculationGrid">The Uplift Van calculation grid result.</param>
        /// <param name="properties">The container of the properties for the
        /// <see cref="MacroStabilityInwardsCalculatorResult"/></param>
        /// <exception cref="ArgumentNullException">Thrown when any
        /// parameter is <c>null</c>.</exception>
        internal MacroStabilityInwardsCalculatorResult(MacroStabilityInwardsSlidingCurveResult slidingCurve,
                                                       MacroStabilityInwardsUpliftVanCalculationGridResult upliftVanCalculationGrid,
                                                       ConstructionProperties properties)
        {
            if (slidingCurve == null)
            {
                throw new ArgumentNullException(nameof(slidingCurve));
            }
            if (upliftVanCalculationGrid == null)
            {
                throw new ArgumentNullException(nameof(upliftVanCalculationGrid));
            }
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            SlidingCurve = slidingCurve;
            UpliftVanCalculationGrid = upliftVanCalculationGrid;

            FactorOfStability = properties.FactorOfStability;
            ZValue = properties.ZValue;
            ForbiddenZonesXEntryMin = properties.ForbiddenZonesXEntryMin;
            ForbiddenZonesXEntryMax = properties.ForbiddenZonesXEntryMax;
            ForbiddenZonesAutomaticallyCalculated = properties.ForbiddenZonesAutomaticallyCalculated;
            GridAutomaticallyCalculated = properties.GridAutomaticallyCalculated;
        }

        /// <summary>
        /// Container for properties for constructing a <see cref="MacroStabilityInwardsCalculatorResult"/>.
        /// </summary>s
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                FactorOfStability = double.NaN;
                ZValue = double.NaN;
                ForbiddenZonesXEntryMin = double.NaN;
                ForbiddenZonesXEntryMax = double.NaN;
            }

            /// <summary>
            /// Gets or sets the factor of stability of the Uplift Van calculation.
            /// </summary>
            public double FactorOfStability { internal get; set; }

            /// <summary>
            /// Gets or sets the z value.
            /// </summary>
            public double ZValue { internal get; set; }

            /// <summary>
            /// Gets or sets the forbidden zones x entry min.
            /// </summary>
            public double ForbiddenZonesXEntryMin { internal get; set; }

            /// <summary>
            /// Gets or sets the forbidden zones x entry max.
            /// </summary>
            public double ForbiddenZonesXEntryMax { internal get; set; }

            /// <summary>
            /// Gets or sets whether the forbidden zones are automatically calculated.
            /// </summary>
            public bool ForbiddenZonesAutomaticallyCalculated { internal get; set; }

            /// <summary>
            /// Gets or sets whether the grid is automatically calculated.
            /// </summary>
            public bool GridAutomaticallyCalculated { internal get; set; }
        }

        #region Properties

        /// <summary>
        /// Gets the sliding curve result.
        /// </summary>
        public MacroStabilityInwardsSlidingCurveResult SlidingCurve { get; }

        /// <summary>
        /// Gets the Uplift Van calculation grid result.
        /// </summary>
        public MacroStabilityInwardsUpliftVanCalculationGridResult UpliftVanCalculationGrid { get; }

        /// <summary>
        /// Gets the factor of stability of the Uplift Van calculation.
        /// </summary>
        public double FactorOfStability { get; }

        /// <summary>
        /// Gets the z value.
        /// </summary>
        public double ZValue { get; }

        /// <summary>
        /// Gets the forbidden zones x entry min.
        /// </summary>
        public double ForbiddenZonesXEntryMin { get; }

        /// <summary>
        /// Gets the forbidden zones x entry max.
        /// </summary>
        public double ForbiddenZonesXEntryMax { get; }

        /// <summary>
        /// Gets whether the forbidden zones are automatically calculated.
        /// </summary>
        public bool ForbiddenZonesAutomaticallyCalculated { get; }

        /// <summary>
        /// Gets whether the grid is automatically calculated.
        /// </summary>
        public bool GridAutomaticallyCalculated { get; }

        #endregion
    }
}