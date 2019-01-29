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
using Riskeer.Common.IO.Configurations;

namespace Riskeer.Piping.IO.Configurations
{
    /// <summary>
    /// Configuration of a piping calculation scenario.
    /// </summary>
    public class PipingCalculationConfiguration : IConfigurationItem
    {
        private string name;

        /// <summary>
        /// Creates a new instance of <see cref="PipingCalculationConfiguration"/>.
        /// </summary>
        /// <param name="name">The name of the <see cref="PipingCalculationConfiguration"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="name"/> is <c>null</c>.</exception>
        public PipingCalculationConfiguration(string name)
        {
            Name = name;
        }

        /// <summary>
        /// Gets or sets the assessment level of the piping calculation.
        /// </summary>
        public double? AssessmentLevel { get; set; }

        /// <summary>
        /// Gets or sets the name of the hydraulic boundary location of the piping calculation.
        /// </summary>
        public string HydraulicBoundaryLocationName { get; set; }

        /// <summary>
        /// Gets or sets the name of the surface line of the piping calculation.
        /// </summary>
        public string SurfaceLineName { get; set; }

        /// <summary>
        /// Gets or sets the l-coordinate of the entry point of the piping calculation.
        /// </summary>
        public double? EntryPointL { get; set; }

        /// <summary>
        /// Gets or sets the l-coordinate of the exit point of the piping calculation.
        /// </summary>
        public double? ExitPointL { get; set; }

        /// <summary>
        /// Gets or sets the name of the stochastic soil model of the piping calculation.
        /// </summary>
        public string StochasticSoilModelName { get; set; }

        /// <summary>
        /// Gets or sets the name of the stochastic soil profile of the piping calculation.
        /// </summary>
        public string StochasticSoilProfileName { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the phreatic level exit of the piping calculation.
        /// </summary>
        public StochastConfiguration PhreaticLevelExit { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the damping factor exit of the piping calculation.
        /// </summary>
        public StochastConfiguration DampingFactorExit { get; set; }

        /// <summary>
        /// Gets or sets the configuration of the scenario of the piping calculation.
        /// </summary>
        public ScenarioConfiguration Scenario { get; set; }

        /// <summary>
        /// Gets or sets the name of the piping calculation.
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