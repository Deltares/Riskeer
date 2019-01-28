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
using Ringtoets.Common.IO.Configurations;

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Class that represents a configuration of a macro stability inwards calculation scenario.
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
        /// Gets or sets the configuration of the scenario of the calculation.
        /// </summary>
        public ScenarioConfiguration Scenario { get; set; }

        /// <summary>
        /// Gets or sets the dike soil scenario of the calculation.
        /// </summary>
        public ConfigurationDikeSoilScenario? DikeSoilScenario { get; set; }

        /// <summary>
        /// Gets or sets the minimum depth of the slip plane.
        /// </summary>
        public double? SlipPlaneMinimumDepth { get; set; }

        /// <summary>
        /// Gets or sets the minimum length of the slip plane.
        /// </summary>
        public double? SlipPlaneMinimumLength { get; set; }

        /// <summary>
        /// Gets or sets the maximum slice width.
        /// </summary>
        public double? MaximumSliceWidth { get; set; }

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

        #region Zoning

        /// <summary>
        /// Gets or sets whether zones should be created.
        /// </summary>
        public bool? CreateZones { get; set; }

        /// <summary>
        /// Gets or sets the zoning boundaries determination type.
        /// </summary>
        public ConfigurationZoningBoundariesDeterminationType? ZoningBoundariesDeterminationType { get; set; }

        /// <summary>
        /// Gets or sets the left zone boundary.
        /// </summary>
        public double? ZoneBoundaryLeft { get; set; }

        /// <summary>
        /// Gets or sets the right zone boundary.
        /// </summary>
        public double? ZoneBoundaryRight { get; set; }

        #endregion

        #region Water stresses

        /// <summary>
        /// Gets or sets the average river water level.
        /// </summary>
        public double? WaterLevelRiverAverage { get; set; }

        #region Drainage

        /// <summary>
        /// Gets or sets whether a drainage construction is present.
        /// </summary>
        public bool? DrainageConstructionPresent { get; set; }

        /// <summary>
        /// Gets or sets the x coordinate of the drainage construction.
        /// </summary>
        public double? XCoordinateDrainageConstruction { get; set; }

        /// <summary>
        /// Gets or sets the z coordinate of the drainage construction.
        /// </summary>
        public double? ZCoordinateDrainageConstruction { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the minimum level phreatic line at dike top river.
        /// </summary>
        public double? MinimumLevelPhreaticLineAtDikeTopRiver { get; set; }

        /// <summary>
        /// Gets or sets the minimum level phreatic line at dike top polder.
        /// </summary>
        public double? MinimumLevelPhreaticLineAtDikeTopPolder { get; set; }

        /// <summary>
        /// Gets or sets whether phreatic line 3 and 4 should be adjusted for Uplift.
        /// </summary>
        public bool? AdjustPhreaticLine3And4ForUplift { get; set; }

        /// <summary>
        /// Gets or sets the piezometric head of the phreatic line 2 inwards.
        /// </summary>
        public double? PiezometricHeadPhreaticLine2Inwards { get; set; }

        /// <summary>
        /// Gets or sets the piezometric head of the phreatic line 2 outwards.
        /// </summary>
        public double? PiezometricHeadPhreaticLine2Outwards { get; set; }

        /// <summary>
        /// Gets or sets the leakage length inwards of phreatic line 3.
        /// </summary>
        public double? LeakageLengthInwardsPhreaticLine3 { get; set; }

        /// <summary>
        /// Gets or sets the leakage length outwards of phreatic line 3.
        /// </summary>
        public double? LeakageLengthOutwardsPhreaticLine3 { get; set; }

        /// <summary>
        /// Gets or sets the leakage length inwards of phreatic line 4.
        /// </summary>
        public double? LeakageLengthInwardsPhreaticLine4 { get; set; }

        /// <summary>
        /// Gets or sets the leakage length outwards of phreatic line 4.
        /// </summary>
        public double? LeakageLengthOutwardsPhreaticLine4 { get; set; }

        /// <summary>
        /// Gets or sets the locations input values for daily conditions.
        /// </summary>
        public MacroStabilityInwardsLocationInputConfiguration LocationInputDaily { get; set; }

        /// <summary>
        /// Gets or sets the locations input values for extreme conditions.
        /// </summary>
        public MacroStabilityInwardsLocationInputExtremeConfiguration LocationInputExtreme { get; set; }

        #endregion

        #region Grid

        /// <summary>
        /// Gets or sets the grid determination type.
        /// </summary>
        public ConfigurationGridDeterminationType? GridDeterminationType { get; set; }

        /// <summary>
        /// Gets or sets the value whether the grid should be moved.
        /// </summary>
        public bool? MoveGrid { get; set; }

        #region Tangent line

        /// <summary>
        /// Gets or sets the tangent line determination type.
        /// </summary>
        public ConfigurationTangentLineDeterminationType? TangentLineDeterminationType { get; set; }

        /// <summary>
        /// Gets or sets the tangent line top boundary.
        /// </summary>
        public double? TangentLineZTop { get; set; }

        /// <summary>
        /// Gets or sets the tangent line bottom boundary.
        /// </summary>
        public double? TangentLineZBottom { get; set; }

        /// <summary>
        /// Gets or sets the number of tangent lines.
        /// </summary>
        public int? TangentLineNumber { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the left grid.
        /// </summary>
        public MacroStabilityInwardsGridConfiguration LeftGrid { get; set; }

        /// <summary>
        /// Gets or sets the right grid.
        /// </summary>
        public MacroStabilityInwardsGridConfiguration RightGrid { get; set; }

        #endregion
    }
}