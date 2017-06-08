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

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Class that represents a macro stability inwards calculation configuration.
    /// </summary>
    public class MacroStabilityInwardsCalculationConfiguration : IConfigurationItem
    {
        private string name;

        /// <summary>
        /// Creates a new instance of <see cref="MacroStabilityInwardsCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="MacroStabilityInwardsCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public MacroStabilityInwardsCalculationConfiguration(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the assessment level of the calculation.
        /// </summary>
        public double? AssessmentLevel { get; set; }

        /// <summary>
        /// Gets or sets the name of the hydraulic boundary location of the calculation.
        /// </summary>
        public string HydraulicBoundaryLocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the surface line of the calculation.
        /// </summary>
        public string SurfaceLineName { get; set; }

        /// <summary>
        /// Gets or sets the name of the stochastic soil model of the calculation.
        /// </summary>
        public string StochasticSoilModelName { get; set; }

        /// <summary>
        /// Gets or sets the name of the stochastic soil profile of the calculation.
        /// </summary>
        public string StochasticSoilProfileName { get; set; }

        /// <summary>
        /// Gets or sets the name for the calculation.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="value"/> is <c>null</c>.</exception>
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value), @"Name is required for a calculation configuration.");
                }
                name = value;
            }
        }
    }
}