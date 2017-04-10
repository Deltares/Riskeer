// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Piping.IO.Readers
{
    /// <summary>
    /// Class that represents a piping calculation that is read via <see cref="PipingCalculationConfigurationReader"/>.
    /// </summary>
    public class ReadPipingCalculation : IConfigurationItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="ReadPipingCalculation"/>.
        /// </summary>
        /// <param name="constructionProperties">The container of the properties for the <see cref="ReadPipingCalculation"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="constructionProperties"/> is <c>null</c>.</exception>
        public ReadPipingCalculation(ConstructionProperties constructionProperties)
        {
            if (constructionProperties == null)
            {
                throw new ArgumentNullException(nameof(constructionProperties));
            }

            Name = constructionProperties.Name;
            AssessmentLevel = constructionProperties.AssessmentLevel;
            HydraulicBoundaryLocation = constructionProperties.HydraulicBoundaryLocation;
            SurfaceLine = constructionProperties.SurfaceLine;
            EntryPointL = constructionProperties.EntryPointL;
            ExitPointL = constructionProperties.ExitPointL;
            StochasticSoilModel = constructionProperties.StochasticSoilModel;
            StochasticSoilProfile = constructionProperties.StochasticSoilProfile;
            PhreaticLevelExitMean = constructionProperties.PhreaticLevelExitMean;
            PhreaticLevelExitStandardDeviation = constructionProperties.PhreaticLevelExitStandardDeviation;
            DampingFactorExitMean = constructionProperties.DampingFactorExitMean;
            DampingFactorExitStandardDeviation = constructionProperties.DampingFactorExitStandardDeviation;
        }

        /// <summary>
        /// Gets the assessment level of the read piping calculation.
        /// </summary>
        public double? AssessmentLevel { get; }

        /// <summary>
        /// Gets the name of the hydraulic boundary location of the read piping calculation.
        /// </summary>
        public string HydraulicBoundaryLocation { get; }

        /// <summary>
        /// Gets the name of the surface line of the read piping calculation.
        /// </summary>
        public string SurfaceLine { get; }

        /// <summary>
        /// Gets the l-coordinate of the entry point of the read piping calculation.
        /// </summary>
        public double? EntryPointL { get; }

        /// <summary>
        /// Gets the l-coordinate of the exit point of the read piping calculation.
        /// </summary>
        public double? ExitPointL { get; }

        /// <summary>
        /// Gets the name of the stochastic soil model of the read piping calculation.
        /// </summary>
        public string StochasticSoilModel { get; }

        /// <summary>
        /// Gets the name of the stochastic soil profile of the read piping calculation.
        /// </summary>
        public string StochasticSoilProfile { get; }

        /// <summary>
        /// Gets the mean of the phreatic level exit of the read piping calculation.
        /// </summary>
        public double? PhreaticLevelExitMean { get; }

        /// <summary>
        /// Gets the standard deviation of the phreatic level exit of the read piping calculation.
        /// </summary>
        public double? PhreaticLevelExitStandardDeviation { get; }

        /// <summary>
        /// Gets the mean of the damping factor exit of the read piping calculation.
        /// </summary>
        public double? DampingFactorExitMean { get; }

        /// <summary>
        /// Gets the standard deviation of the damping factor exit of the read piping calculation.
        /// </summary>
        public double? DampingFactorExitStandardDeviation { get; }

        public string Name { get; }

        /// <summary>
        /// Class holding the various construction parameters for <see cref="ReadPipingCalculation"/>.
        /// </summary>
        public class ConstructionProperties
        {
            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.Name"/>.
            /// </summary>
            public string Name { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.AssessmentLevel"/>.
            /// </summary>
            public double? AssessmentLevel { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.HydraulicBoundaryLocation"/>.
            /// </summary>
            public string HydraulicBoundaryLocation { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.SurfaceLine"/>.
            /// </summary>
            public string SurfaceLine { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.EntryPointL"/>.
            /// </summary>
            public double? EntryPointL { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.ExitPointL"/>.
            /// </summary>
            public double? ExitPointL { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.StochasticSoilModel"/>.
            /// </summary>
            public string StochasticSoilModel { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.StochasticSoilProfile"/>.
            /// </summary>
            public string StochasticSoilProfile { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.PhreaticLevelExitMean"/>.
            /// </summary>
            public double? PhreaticLevelExitMean { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.PhreaticLevelExitStandardDeviation"/>.
            /// </summary>
            public double? PhreaticLevelExitStandardDeviation { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.DampingFactorExitMean"/>.
            /// </summary>
            public double? DampingFactorExitMean { internal get; set; }

            /// <summary>
            /// Gets or sets the value for <see cref="ReadPipingCalculation.DampingFactorExitStandardDeviation"/>.
            /// </summary>
            public double? DampingFactorExitStandardDeviation { internal get; set; }
        }
    }
}