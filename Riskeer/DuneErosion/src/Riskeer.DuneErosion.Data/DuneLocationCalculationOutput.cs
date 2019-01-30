// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.Base.Data;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.Probability;

namespace Riskeer.DuneErosion.Data
{
    /// <summary>
    /// Class that holds all dune location calculation specific output parameters.
    /// </summary>
    public class DuneLocationCalculationOutput
    {
        /// <summary>
        /// Creates a new instance of <see cref="DuneLocationCalculationOutput"/>.
        /// </summary>
        /// <param name="calculationConvergence">The convergence status of the calculation.</param>
        /// <param name="constructionProperties">The container for the properties for the <see cref="DuneLocationCalculationOutput"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="constructionProperties"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="ConstructionProperties.TargetProbability"/> 
        /// or <see cref="ConstructionProperties.CalculatedProbability"/> falls outside the [0.0, 1.0] range.</exception>
        public DuneLocationCalculationOutput(CalculationConvergence calculationConvergence, ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            WaterLevel = new RoundedDouble(2, constructionProperties.WaterLevel);
            WaveHeight = new RoundedDouble(2, constructionProperties.WaveHeight);
            WavePeriod = new RoundedDouble(2, constructionProperties.WavePeriod);

            ProbabilityHelper.ValidateProbability(constructionProperties.TargetProbability, nameof(TargetProbability), true);
            ProbabilityHelper.ValidateProbability(constructionProperties.CalculatedProbability, nameof(CalculatedProbability), true);

            TargetProbability = constructionProperties.TargetProbability;
            TargetReliability = new RoundedDouble(5, constructionProperties.TargetReliability);
            CalculatedProbability = constructionProperties.CalculatedProbability;
            CalculatedReliability = new RoundedDouble(5, constructionProperties.CalculatedReliability);
            CalculationConvergence = calculationConvergence;
        }

        /// <summary>
        /// Gets the water level of the calculation.
        /// [m+NAP]
        /// </summary>
        public RoundedDouble WaterLevel { get; }

        /// <summary>
        /// Gets the wave height of the calculation.
        /// [m]
        /// </summary>
        public RoundedDouble WaveHeight { get; }

        /// <summary>
        /// Gets the wave period of the calculation.
        /// [s]
        /// </summary>
        public RoundedDouble WavePeriod { get; }

        /// <summary>
        /// Gets the target probability.
        /// [1/year]
        /// </summary>
        public double TargetProbability { get; }

        /// <summary>
        /// Gets the target beta.
        /// [-]
        /// </summary>
        public RoundedDouble TargetReliability { get; }

        /// <summary>
        /// Gets the calculated probability.
        /// [1/year]
        /// </summary>
        public double CalculatedProbability { get; }

        /// <summary>
        /// Gets the calculated reliability.
        /// [-]
        /// </summary>
        public RoundedDouble CalculatedReliability { get; }

        /// <summary>
        /// Gets the convergence status of the calculation.
        /// [-]
        /// </summary>
        public CalculationConvergence CalculationConvergence { get; }

        /// <summary>
        /// Container for properties for constructing a <see cref="DuneLocationCalculationOutput"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Creates a new instance of <see cref="ConstructionProperties"/>.
            /// </summary>
            public ConstructionProperties()
            {
                WaterLevel = double.NaN;
                WaveHeight = double.NaN;
                WavePeriod = double.NaN;
                TargetProbability = double.NaN;
                TargetReliability = double.NaN;
                CalculatedProbability = double.NaN;
                CalculatedReliability = double.NaN;
            }

            /// <summary>
            /// Gets the water level of the calculation.
            /// </summary>
            public double WaterLevel { internal get; set; }

            /// <summary>
            /// Gets the wave height of the calculation.
            /// </summary>
            public double WaveHeight { internal get; set; }

            /// <summary>
            /// Gets the wave period of the calculation.
            /// </summary>
            public double WavePeriod { internal get; set; }

            /// <summary>
            /// Gets the target probability.
            /// </summary>
            public double TargetProbability { internal get; set; }

            /// <summary>
            /// Gets the target beta.
            /// </summary>
            public double TargetReliability { internal get; set; }

            /// <summary>
            /// Gets the calculated probability.
            /// </summary>
            public double CalculatedProbability { internal get; set; }

            /// <summary>
            /// Gets the calculated reliability.
            /// </summary>
            public double CalculatedReliability { internal get; set; }
        }
    }
}