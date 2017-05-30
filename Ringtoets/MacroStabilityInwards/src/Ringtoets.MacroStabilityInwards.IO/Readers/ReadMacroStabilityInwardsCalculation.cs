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
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.MacroStabilityInwards.IO.Readers
{
    /// <summary>
    /// Class that represents a macro stability inwards calculation that is read via <see cref="MacroStabilityInwardsCalculationConfigurationReader"/>.
    /// </summary>
    public class ReadMacroStabilityInwardsCalculation : IConfigurationItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadMacroStabilityInwardsCalculation"/>.
        /// </summary>
        /// <param name="constructionProperties">The container of the properties for the <see cref="ReadMacroStabilityInwardsCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="constructionProperties"/> is <c>null</c>.</exception>
        public ReadMacroStabilityInwardsCalculation(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            Name = constructionProperties.Name;
            AssessmentLevel = constructionProperties.AssessmentLevel;
            HydraulicBoundaryLocation = constructionProperties.HydraulicBoundaryLocation;
            SurfaceLine = constructionProperties.SurfaceLine;
            StochasticSoilModel = constructionProperties.StochasticSoilModel;
            StochasticSoilProfile = constructionProperties.StochasticSoilProfile;
        }

        /// <summary>
        /// Gets the assessment level of the read calculation.
        /// </summary>
        public double? AssessmentLevel { get; }

        /// <summary>
        /// Gets the name of the hydraulic boundary location of the read calculation.
        /// </summary>
        public string HydraulicBoundaryLocation { get; }

        /// <summary>
        /// Gets the name of the surface line of the read calculation.
        /// </summary>
        public string SurfaceLine { get; }

        /// <summary>
        /// Gets the name of the stochastic soil model of the read calculation.
        /// </summary>
        public string StochasticSoilModel { get; }

        /// <summary>
        /// Gets the name of the stochastic soil profile of the read calculation.
        /// </summary>
        public string StochasticSoilProfile { get; }

        public string Name { get; }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="ReadMacroStabilityInwardsCalculation"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value for <see cref="ReadMacroStabilityInwardsCalculation.Name"/>.
            /// </summary>
            public string Name { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadMacroStabilityInwardsCalculation.AssessmentLevel"/>.
            /// </summary>
            public double? AssessmentLevel { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadMacroStabilityInwardsCalculation.HydraulicBoundaryLocation"/>.
            /// </summary>
            public string HydraulicBoundaryLocation { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadMacroStabilityInwardsCalculation.SurfaceLine"/>.
            /// </summary>
            public string SurfaceLine { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadMacroStabilityInwardsCalculation.StochasticSoilModel"/>.
            /// </summary>
            public string StochasticSoilModel { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadMacroStabilityInwardsCalculation.StochasticSoilProfile"/>.
            /// </summary>
            public string StochasticSoilProfile { internal get; set; }
        }
    }
}