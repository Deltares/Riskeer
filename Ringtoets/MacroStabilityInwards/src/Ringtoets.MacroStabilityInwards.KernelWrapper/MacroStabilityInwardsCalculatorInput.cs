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
using Ringtoets.MacroStabilityInwards.Primitives;

namespace Ringtoets.MacroStabilityInwards.KernelWrapper
{
    /// <summary>
    /// This class contains all the parameters that are required to perform a macro stability inwards assessment.
    /// </summary>
    public class MacroStabilityInwardsCalculatorInput
    {
        /// <summary>
        /// Constructs a new <see cref="MacroStabilityInwardsCalculatorInput"/>, which contains values for the parameters used
        /// in the macro stability inwards sub calculations.
        /// </summary>
        /// <param name="properties">The object containing the values for the properties 
        /// of the new <see cref="MacroStabilityInwardsCalculatorInput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="properties"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculatorInput(ConstructionProperties properties)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }
            AssessmentLevel = properties.AssessmentLevel;
            SurfaceLine = properties.SurfaceLine;
            SoilProfile = properties.SoilProfile;
        }

        public class ConstructionProperties
        {
            /// <summary>
            /// Creates new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                AssessmentLevel = double.NaN;
                SurfaceLine = null;
                SoilProfile = null;
            }

            #region properties

            /// <summary>
            /// Gets the outside high water level.
            /// [m]
            /// </summary>
            public double AssessmentLevel { internal get; set; }

            /// <summary>
            /// Gets the surface line.
            /// </summary>
            public MacroStabilityInwardsSurfaceLine SurfaceLine { internal get; set; }

            /// <summary>
            /// Gets the profile which contains a 1 dimensional definition of soil layers with properties.
            /// </summary>
            public ISoilProfile SoilProfile { internal get; set; }

            #endregion
        }

        #region properties

        /// <summary>
        /// Gets the outside high water level.
        /// [m]
        /// </summary>
        public double AssessmentLevel { get; private set; }

        /// <summary>
        /// Gets the surface line.
        /// </summary>
        public MacroStabilityInwardsSurfaceLine SurfaceLine { get; private set; }

        /// <summary>
        /// Gets the profile which contains a definition of soil layers with properties.
        /// </summary>
        public ISoilProfile SoilProfile { get; private set; }

        #endregion
    }
}