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

namespace Riskeer.MacroStabilityInwards.IO.Configurations
{
    /// <summary>
    /// Container of identifiers related to the macro stability inwards calculation configuration schema definition.
    /// </summary>
    internal static class MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The identifier for water level elements.
        /// </summary>
        public const string WaterLevelElement = "waterstand";

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

        #region Zones

        /// <summary>
        /// The identifier for zones elements.
        /// </summary>
        public const string ZonesElement = "zonering";

        /// <summary>
        /// The tag of the element containing the value indicating if zones should be created.
        /// </summary>
        public const string CreateZonesElement = "bepaling";

        /// <summary>
        /// The tag of the element containing the value indicating the method to determine the zones.
        /// </summary>
        public const string ZoningBoundariesDeterminationTypeElement = "methode";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationZoningBoundariesDeterminationType"/> 
        /// element indicating zoning boundary determination.
        /// </summary>
        public const string ZoningBoundariesDeterminationTypeAutomatic = "automatisch";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationZoningBoundariesDeterminationType"/> 
        /// element indicating manual zoning boundary determination.
        /// </summary>
        public const string ZoningBoundariesDeterminationTypeManual = "handmatig";

        /// <summary>
        /// The tag of the element containing the value indicating the left boundary of the zone.
        /// </summary>
        public const string ZoneBoundaryLeft = "zoneringsgrenslinks";

        /// <summary>
        /// The tag of the element containing the value indicating the right boundary of the zone.
        /// </summary>
        public const string ZoneBoundaryRight = "zoneringsgrensrechts";

        #endregion

        #region Water stresses

        /// <summary>
        /// The identifier for water stresses elements.
        /// </summary>
        public const string WaterStressesElement = "waterspanningen";

        /// <summary>
        /// The tag of the element containing the average river water level.
        /// </summary>
        public const string WaterLevelRiverAverageElement = "gemiddeldhoogwater";

        #region Phreatic lines

        /// <summary>
        /// The identifier for minimum level phreatic line 1 elements.
        /// </summary>
        public const string PhreaticLine1MinimumLevelElement = "initielehoogtepl1";

        /// <summary>
        /// The tag of the element containing the value indicating the minimum level phreatic line at dike top river.
        /// </summary>
        public const string MinimumLevelPhreaticLineAtDikeTopRiverElement = "buitenkruin";

        /// <summary>
        /// The tag of the element containing the value indicating the minimum level phreatic line at dike top polder.
        /// </summary>
        public const string MinimumLevelPhreaticLineAtDikeTopPolderElement = "binnenkruin";

        /// <summary>
        /// The identifier for leakage length phreatic line 3 elements.
        /// </summary>
        public const string PhreaticLine3LeakageLengthElement = "leklengtespl3";

        /// <summary>
        /// The identifier for leakage length phreatic line 4 elements.
        /// </summary>
        public const string PhreaticLine4LeakageLengthElement = "leklengtespl4";

        /// <summary>
        /// The identifier for piezometric head phreatic line 2 elements.
        /// </summary>
        public const string PhreaticLine2PiezometricHeadElement = "stijghoogtespl2";

        /// <summary>
        /// The tag of the element containing the value indicating the phreatic line inwards leakage length or piezometric head.
        /// </summary>
        public const string PhreaticLineInwardsElement = "binnenwaarts";

        /// <summary>
        /// he tag of the element containing the value indicating the phreatic line outwards leakage length or piezometric head.
        /// </summary>
        public const string PhreaticLineOutwardsElement = "buitenwaarts";

        #endregion

        /// <summary>
        /// The tag of the element containing the value indicating whether phreatic line 3 and 4 should be adjusted for Uplift.
        /// </summary>
        public const string AdjustPhreaticLine3And4ForUpliftElement = "corrigeervooropbarsten";

        #region Location input

        /// <summary>
        /// The identifier for all location input elements for daily conditions.
        /// </summary>
        public const string LocationInputDailyElement = "dagelijks";

        /// <summary>
        /// The identifier for all location input elements for extreme conditions.
        /// </summary>
        public const string LocationInputExtremeElement = "extreem";

        /// <summary>
        /// The tag of the element containing the polder water level.
        /// </summary>
        public const string WaterLevelPolderElement = "polderpeil";

        /// <summary>
        /// The tag of the element containing the penetration length.
        /// </summary>
        public const string PenetrationLengthElement = "indringingslengte";

        /// <summary>
        /// The identifier for the offset of the location input elements.
        /// </summary>
        public const string LocationInputOffsetElement = "offsets";

        /// <summary>
        /// The tag of the element containing the value whether the default offsets should be used.
        /// </summary>
        public const string UseDefaultOffsetsElement = "gebruikdefaults";

        /// <summary>
        /// The tag of the element containing the offset of the phreatic line below dike top at river.
        /// </summary>
        public const string PhreaticLineOffsetBelowDikeTopAtRiverElement = "buitenkruin";

        /// <summary>
        /// The tag of the element containing the  offset of the phreatic line below dike top at polder.
        /// </summary>
        public const string PhreaticLineOffsetBelowDikeTopAtPolderElement = "binnenkruin";

        /// <summary>
        /// The tag of the element containing the offset of the phreatic line below shoulder base inside.
        /// </summary>
        public const string PhreaticLineOffsetBelowShoulderBaseInsideElement = "insteekbinnenberm";

        /// <summary>
        /// The tag of the element containing the offset of the phreatic line below dike toe at polder.
        /// </summary>
        public const string PhreaticLineOffsetBelowDikeToeAtPolderElement = "teendijkbinnenwaarts";

        #endregion

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
        public const string DikeSoilScenarioClayDikeOnClay = "kleidijkopklei";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationDikeSoilScenario"/> 
        /// element indicating a sand dike on clay.
        /// </summary>
        public const string DikeSoilScenarioSandDikeOnClay = "zanddijkopklei";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationDikeSoilScenario"/> 
        /// element indicating a clay dike on sand
        /// </summary>
        public const string DikeSoilScenarioClayDikeOnSand = "kleidijkopzand";

        /// <summary>
        /// The possible content of the <see cref="ConfigurationDikeSoilScenario"/> 
        /// element indicating a sand dike on sand.
        /// </summary>
        public const string DikeSoilScenarioSandDikeOnSand = "zanddijkopzand";

        #endregion

        #region Grid

        /// <summary>
        /// The identifier for grids elements.
        /// </summary>
        public const string GridsElement = "grids";

        /// <summary>
        /// The tag of the element containing the value whether the grid should be moved.
        /// </summary>
        public const string MoveGridElement = "verplaatsgrid";

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
        /// The tag of the element containing the value indicating the top boundary of the tangent line.
        /// </summary>
        public const string TangentLineZTopElement = "zboven";

        /// <summary>
        /// The tag of the element containing the value indicating the bottom boundary of the tangent line.
        /// </summary>
        public const string TangentLineZBottomElement = "zonder";

        /// <summary>
        /// The tag of the element containing the value indicating the number of tangent lines.
        /// </summary>
        public const string TangentLineNumberElement = "aantal";

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
        /// The tag of the element containing the value indicating the left boundary of the grid.
        /// </summary>
        public const string GridXLeftElement = "xlinks";

        /// <summary>
        /// The tag of the element containing the value indicating the right boundary of the grid.
        /// </summary>
        public const string GridXRightElement = "xrechts";

        /// <summary>
        /// The tag of the element containing the value indicating the top boundary of the grid.
        /// </summary>
        public const string GridZTopElement = "zboven";

        /// <summary>
        /// The tag of the element containing the value indicating the bottom boundary of the grid.
        /// </summary>
        public const string GridZBottomElement = "zonder";

        /// <summary>
        /// The tag of the element containing the value indicating the number of horizontal points of the grid.
        /// </summary>
        public const string GridNumberOfHorizontalPointsElement = "aantalpuntenhorizontaal";

        /// <summary>
        /// The tag of the element containing the value indicating the number of vertical points of the grid.
        /// </summary>
        public const string GridNumberOfVerticalPointsElement = "aantalpuntenverticaal";

        #endregion
    }
}