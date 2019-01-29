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
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;

namespace Riskeer.MacroStabilityInwards.Data
{
    /// <summary>
    /// Simple class containing the results of a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsOutput : CloneableObservable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="slidingCurve">The sliding curve result.</param>
        /// <param name="slipPlane">The slip plane Uplift Van result.</param>
        /// <param name="properties">The container of the properties for the
        /// <see cref="MacroStabilityInwardsOutput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsOutput(MacroStabilityInwardsSlidingCurve slidingCurve,
                                           MacroStabilityInwardsSlipPlaneUpliftVan slipPlane,
                                           ConstructionProperties properties)
        {
            if (slidingCurve == null)
            {
                throw new ArgumentNullException(nameof(slidingCurve));
            }

            if (slipPlane == null)
            {
                throw new ArgumentNullException(nameof(slipPlane));
            }

            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            SlidingCurve = slidingCurve;
            SlipPlane = slipPlane;

            FactorOfStability = properties.FactorOfStability;
            ZValue = properties.ZValue;
            ForbiddenZonesXEntryMin = properties.ForbiddenZonesXEntryMin;
            ForbiddenZonesXEntryMax = properties.ForbiddenZonesXEntryMax;
        }

        public override object Clone()
        {
            var clone = (MacroStabilityInwardsOutput) base.Clone();
            clone.SlidingCurve = (MacroStabilityInwardsSlidingCurve) SlidingCurve.Clone();
            clone.SlipPlane = (MacroStabilityInwardsSlipPlaneUpliftVan) SlipPlane.Clone();
            return clone;
        }

        /// <summary>
        /// Container for properties for constructing a <see cref="MacroStabilityInwardsOutput"/>.
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
        }

        #region Properties

        /// <summary>
        /// Gets the sliding curve.
        /// </summary>
        public MacroStabilityInwardsSlidingCurve SlidingCurve { get; private set; }

        /// <summary>
        /// Gets the slip plane.
        /// </summary>
        public MacroStabilityInwardsSlipPlaneUpliftVan SlipPlane { get; private set; }

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

        #endregion
    }
}