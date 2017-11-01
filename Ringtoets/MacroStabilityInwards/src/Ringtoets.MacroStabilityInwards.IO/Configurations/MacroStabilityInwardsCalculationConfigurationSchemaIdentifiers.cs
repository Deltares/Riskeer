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

namespace Ringtoets.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Container of identifiers related to the macro stability inwards calculation configuration schema definition.
    /// </summary>
    internal static class MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The identifier for assessment level elements.
        /// </summary>
        public const string AssessmentLevelElement = "toetspeil";

        /// <summary>
        /// The identifier for surface line elements.
        /// </summary>
        public const string SurfaceLineElement = "profielschematisatie";

        /// <summary>
        /// The identifier for stochastic soil model elements.
        /// </summary>
        public const string StochasticSoilModelElement = "ondergrondmodel";

        /// <summary>
        /// The identifier for stochastic soil profile elements.
        /// </summary>
        public const string StochasticSoilProfileElement = "ondergrondschematisatie";

        /// <summary>
        /// The tag of the element containing the value indicating the minimum depth of the slip plane.
        /// </summary>
        public const string SlipPlaneMinimumDepthElement = "minimaleglijvlakdiepte";

        /// <summary>
        /// The tag of the element containing the value indicating the minimum length of the slip plane.
        /// </summary>
        public const string SlipPlaneMinimumLengthElement = "minimaleglijvlaklengte";

        /// <summary>
        /// The tag of the element containing the value indicating the maximum slice width.
        /// </summary>
        public const string MaximumSliceWidthElement = "maximalelamelbreedte";

        /// <summary>
        /// The identifier for zones elements.
        /// </summary>
        public const string ZonesElement = "zonering";

        /// <summary>
        /// The tag of the element containing the value indicating if zones should be created automatically.
        /// </summary>
        public const string CreateZones = "bepaling";

        #region Water stresses

        /// <summary>
        /// The identifier for water stresses elements.
        /// </summary>
        public const string WaterStressesElement = "waterspanningen";

        /// <summary>
        /// The tag of the element containing the average river water level.
        /// </summary>
        public const string WaterLevelRiverAverageElement = "ghw";

        /// <summary>
        /// The tag of the element containing the value indicating whether phreatic line 3 and 4 should be adjusted for Uplift.
        /// </summary>
        public const string AdjustPhreaticLine3And4ForUpliftElement = "corrigeervooropbarsten";

        #region Drainage

        /// <summary>
        /// The identifier for drainage construction elements.
        /// </summary>
        public const string DrainageConstructionElement = "drainage";

        /// <summary>
        /// The tag of the element containing the value whether a drainage construction is present.
        /// </summary>
        public const string DrainageConstructionPresentElement = "aanwezig";

        /// <summary>
        /// The tag of the element containing the x coordinate of the drainage construction.
        /// </summary>
        public const string XCoordinateDrainageConstructionElement = "x";

        /// <summary>
        /// The tag of the element containing the z coordinate of the drainage construction.
        /// </summary>
        public const string ZCoordinateDrainageConstructionElement = "z";

        #endregion

        #endregion

        #region Dike soil scenario

        /// <summary>
        /// The identifier for the dike soil scenario type elements.
        /// </summary>
        public const string DikeSoilScenarioElement = "dijktype";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationDikeSoilScenario"/> 
        /// element indicating a clay dike on clay.
        /// </summary>
        public const string DikeSoilScenarioClayDikeOnClay = "1A";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationDikeSoilScenario"/> 
        /// element indicating a sand dike on clay.
        /// </summary>
        public const string DikeSoilScenarioSandDikeOnClay = "2A";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationDikeSoilScenario"/> 
        /// element indicating a clay dike on sand
        /// </summary>
        public const string DikeSoilScenarioClayDikeOnSand = "1B";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationDikeSoilScenario"/> 
        /// element indicating a sand dike on sand.
        /// </summary>
        public const string DikeSoilScenarioSandDikeOnSand = "2B";

        #endregion

        #region Grid

        /// <summary>
        /// The identifier for grid elements.
        /// </summary>
        public const string GridElement = "grids";

        /// <summary>
        /// The tag of the element containing the value whether the grid should be moved.
        /// </summary>
        public const string MoveGrid = "verplaatsgrid";

        /// <summary>
        /// The identifier for the grid determination type elements.
        /// </summary>
        public const string GridDeterminationTypeElement = "bepaling";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationGridDeterminationType"/> 
        /// element indicating automatic grid determination.
        /// </summary>
        public const string GridDeterminationTypeAutomatic = "automatisch";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationGridDeterminationType"/> 
        /// element indicating manual grid determination.
        /// </summary>
        public const string GridDeterminationTypeManual = "handmatig";

        #region Tangent lines

        /// <summary>
        /// The identifier for tangent line elements.
        /// </summary>
        public const string TangentLineElement = "tangentlijnen";

        /// <summary>
        /// The identifier for the tangent line determination type elements.
        /// </summary>
        public const string TangentLineDeterminationTypeElement = "bepalingtangentlijnen";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationTangentLineDeterminationType"/> 
        /// element indicating layer separated.
        /// </summary>
        public const string TangentLineDeterminationTypeLayerSeparated = "laagscheiding";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationTangentLineDeterminationType"/> 
        /// element indicating specified.
        /// </summary>
        public const string TangentLineDeterminationTypeSpecified = "gespecificeerd";

        /// <summary>
        /// The tag of the element containing the value indicating the top z of the tangent line.
        /// </summary>
        public const string TangentLineZTop = "zboven";

        /// <summary>
        /// The tag of the element containing the value indicating the bottom z of the tangent line.
        /// </summary>
        public const string TangentLineZBottom = "zonder";

        /// <summary>
        /// The tag of the element containing the value indicating the number of tangent lines.
        /// </summary>
        public const string TangentLineNumber = "aantal";

        #endregion

        /// <summary>
        /// The identifier for left grid elements.
        /// </summary>
        public const string LeftGridElement = "linkergrid";

        /// <summary>
        /// The identifier for right grid elements.
        /// </summary>
        public const string RightGridElement = "rechtergrid";

        /// <summary>
        /// The tag of the element containing the value indicating the left x of the grid.
        /// </summary>
        public const string GridXLeft = "links";

        /// <summary>
        /// The tag of the element containing the value indicating the right x of the grid.
        /// </summary>
        public const string GridXRight = "rechts";

        /// <summary>
        /// The tag of the element containing the value indicating the top z of the grid.
        /// </summary>
        public const string GridZTop = "boven";

        /// <summary>
        /// The tag of the element containing the value indicating the bottom z of the grid.
        /// </summary>
        public const string GridZBottom = "onder";

        /// <summary>
        /// The tag of the element containing the value indicating the number of horizontal points of the grid.
        /// </summary>
        public const string GridNumberOfHorizontalPoints = "aantalpuntenhorizontaal";

        /// <summary>
        /// The tag of the element containing the value indicating the number of vertical points of the grid.
        /// </summary>
        public const string GridNumberOfVerticalPoints = "aantalpuntenverticaal";

        #endregion
    }
}