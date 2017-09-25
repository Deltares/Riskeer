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
    /// This class contains the results of an Uplift Van calculation.
    /// </summary>
    public class UpliftVanCalculatorResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="UpliftVanCalculatorResult"/>.
        /// </summary>
        /// <param name="slidingCurveResult">The sliding curve result.</param>
        /// <param name="calculationGridResult">The calculation grid result.</param>
        /// <param name="properties">The container of the properties for the <see cref="UpliftVanCalculatorResult"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        internal UpliftVanCalculatorResult(MacroStabilityInwardsSlidingCurveResult slidingCurveResult,
                                           MacroStabilityInwardsUpliftVanCalculationGridResult calculationGridResult,
                                           ConstructionProperties properties)
        {
            if (slidingCurveResult == null)
            {
                throw new ArgumentNullException(nameof(slidingCurveResult));
            }
            if (calculationGridResult == null)
            {
                throw new ArgumentNullException(nameof(calculationGridResult));
            }
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            SlidingCurveResult = slidingCurveResult;
            CalculationGridResult = calculationGridResult;

            FactorOfStability = properties.FactorOfStability;
            ZValue = properties.ZValue;
            ForbiddenZonesXEntryMin = properties.ForbiddenZonesXEntryMin;
            ForbiddenZonesXEntryMax = properties.ForbiddenZonesXEntryMax;
            ForbiddenZonesAutomaticallyCalculated = properties.ForbiddenZonesAutomaticallyCalculated;
            GridAutomaticallyCalculated = properties.GridAutomaticallyCalculated;
        }

        /// <summary>
        /// Container for properties for constructing a <see cref="UpliftVanCalculatorResult"/>.
        /// </summary>
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
            /// Gets or sets the factor of stability.
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
        public MacroStabilityInwardsSlidingCurveResult SlidingCurveResult { get; }

        /// <summary>
        /// Gets the calculation grid result.
        /// </summary>
        public MacroStabilityInwardsUpliftVanCalculationGridResult CalculationGridResult { get; }

        /// <summary>
        /// Gets the factor of stability.
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