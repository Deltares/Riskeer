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
using Core.Common.Base;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.MacroStabilityInwards.Data
{
    /// <summary>
    /// Simple class containing the results of a macro stability inwards calculation.
    /// </summary>
    public class MacroStabilityInwardsOutput : Observable, ICalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsOutput"/>.
        /// </summary>
        /// <param name="properties">The container of the properties for the
        /// <see cref="MacroStabilityInwardsOutput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when the <paramref name="properties"/>
        /// is <c>null</c>.</exception>
        public MacroStabilityInwardsOutput(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            FactorOfStability = properties.FactorOfStability;
        }

        #region properties

        /// <summary>
        /// Gets the factory of stability of the upliftVan calculation.
        /// </summary>
        public double FactorOfStability { get; }

        #endregion

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
            }

            /// <summary>
            /// Gets or sets the factory of stability of the upliftVan calculation.
            /// </summary>
            public double FactorOfStability { internal get; set; }
        }
    }
}