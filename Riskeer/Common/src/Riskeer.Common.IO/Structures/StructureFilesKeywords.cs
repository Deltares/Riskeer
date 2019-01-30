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

namespace Riskeer.Common.IO.Structures
{
    /// <summary>
    /// The keywords as used in structure files.
    /// </summary>
    public static class StructureFilesKeywords
    {
        #region Height Structure Keywords

        /// <summary>
        /// The orientation of the structure, relative to north.
        /// [degrees]
        /// </summary>
        public const string HeightStructureParameterKeyword1 = "KW_HOOGTE1";

        /// <summary>
        /// The crest level of the height structure.
        /// [m+NAP]
        /// </summary>
        public const string HeightStructureParameterKeyword2 = "KW_HOOGTE2";

        /// <summary>
        /// The flow width of the height structure at the bottom protection.
        /// [m]
        /// </summary>
        public const string HeightStructureParameterKeyword3 = "KW_HOOGTE3";

        /// <summary>
        /// The critical overtopping discharge per meter of the height structure.
        /// [m^3/s/m]
        /// </summary>
        public const string HeightStructureParameterKeyword4 = "KW_HOOGTE4";

        /// <summary>
        /// The flow apertures width of the height structure.
        /// [m]
        /// </summary>
        public const string HeightStructureParameterKeyword5 = "KW_HOOGTE5";

        /// <summary>
        /// The failure probability of the height structure, given erosion.
        /// [1/year]
        /// </summary>
        public const string HeightStructureParameterKeyword6 = "KW_HOOGTE6";

        /// <summary>
        /// The storage area of the height structure.
        /// [m^2]
        /// </summary>
        public const string HeightStructureParameterKeyword7 = "KW_HOOGTE7";

        /// <summary>
        /// The allowed increase of level for storage of the height structure.
        /// [m]
        /// </summary>
        public const string HeightStructureParameterKeyword8 = "KW_HOOGTE8";

        #endregion

        #region Closing Structure Keywords

        /// <summary>
        /// The storage area of the closing structure.
        /// [m^2]
        /// </summary>
        public const string ClosingStructureParameterKeyword1 = "KW_BETSLUIT1";

        /// <summary>
        /// The allowed increase of level for storage of the closing structure.
        /// [m]
        /// </summary>
        public const string ClosingStructureParameterKeyword2 = "KW_BETSLUIT2";

        /// <summary>
        /// The orientation of the structure, relative to north.
        /// [degrees]
        /// </summary>
        public const string ClosingStructureParameterKeyword3 = "KW_BETSLUIT3";

        /// <summary>
        /// The width of the flow apertures of the closing structure.
        /// [m]
        /// </summary>
        public const string ClosingStructureParameterKeyword4 = "KW_BETSLUIT4";

        /// <summary>
        /// The crest level of the opened closing structure.
        /// [m+NAP]
        /// </summary>
        public const string ClosingStructureParameterKeyword5 = "KW_BETSLUIT5";

        /// <summary>
        /// The interior water level of the closing structure.
        /// [m+NAP]
        /// </summary>
        public const string ClosingStructureParameterKeyword6 = "KW_BETSLUIT6";

        /// <summary>
        /// The threshold height of the opened closing structure.
        /// [m+NAP]
        /// </summary>
        public const string ClosingStructureParameterKeyword7 = "KW_BETSLUIT7";

        /// <summary>
        /// The area of the flow aperture of the closing structure.
        /// [m^2]
        /// </summary>
        public const string ClosingStructureParameterKeyword8 = "KW_BETSLUIT8";

        /// <summary>
        /// The critical overtopping discharge per meter of the closing structure.
        /// [m^3/s/m]
        /// </summary>
        public const string ClosingStructureParameterKeyword9 = "KW_BETSLUIT9";

        /// <summary>
        /// The flow width of the closing structure at the bottom protection.
        /// [m]
        /// </summary>
        public const string ClosingStructureParameterKeyword10 = "KW_BETSLUIT10";

        /// <summary>
        /// The probability or frequency of the closing structure being open before flooding.
        /// [1/year]
        /// </summary>
        public const string ClosingStructureParameterKeyword11 = "KW_BETSLUIT11";

        /// <summary>
        /// The probability of failing to close the closing structure.
        /// [1/year]
        /// </summary>
        public const string ClosingStructureParameterKeyword12 = "KW_BETSLUIT12";

        /// <summary>
        /// The number of identical apertures of the closing structure.
        /// </summary>
        public const string ClosingStructureParameterKeyword13 = "KW_BETSLUIT13";

        /// <summary>
        /// The probability of failing to repair a failed closure of the closing structure.
        /// [1/year]
        /// </summary>
        public const string ClosingStructureParameterKeyword14 = "KW_BETSLUIT14";

        /// <summary>
        /// The type of closing structure inflow model.
        /// </summary>
        public const string ClosingStructureParameterKeyword15 = "KW_BETSLUIT15";

        #endregion

        #region Stability Point Structure Keywords

        /// <summary>
        /// The orientation of the structure, relative to north.
        /// [degrees]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword1 = "KW_STERSTAB1";

        /// <summary>
        /// The storage area of the stability point structure.
        /// [m^2]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword2 = "KW_STERSTAB2";

        /// <summary>
        /// The allowed increase of level for storage of the stability point structure.
        /// [m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword3 = "KW_STERSTAB3";

        /// <summary>
        /// The width of the flow apertures of the stability point structure.
        /// [m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword4 = "KW_STERSTAB4";

        /// <summary>
        /// The interior water level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword5 = "KW_STERSTAB5";

        /// <summary>
        /// The threshold height of the opened stability point structure.
        /// [m+NAP]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword6 = "KW_STERSTAB6";

        /// <summary>
        /// The critical overtopping discharge per meter of the stability point structure.
        /// [m^3/s/m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword7 = "KW_STERSTAB7";

        /// <summary>
        /// The flow width of the stability point structure at the bottom protection.
        /// [m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword8 = "KW_STERSTAB8";

        /// <summary>
        /// The constructive strength of the linear load model of the stability point structure.
        /// [kN/m^2]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword9 = "KW_STERSTAB9";

        /// <summary>
        /// The constructive strength of the quadratic load model of the stability point structure.
        /// [kN/m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword10 = "KW_STERSTAB10";

        /// <summary>
        /// The bank width of the stability point structure.
        /// [m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword11 = "KW_STERSTAB11";

        /// <summary>
        /// The inside water level failure construction of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword12 = "KW_STERSTAB12";

        /// <summary>
        /// The the evaluation level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword13 = "KW_STERSTAB13";

        /// <summary>
        /// The crest level of the stability point structure.
        /// [m+NAP]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword14 = "KW_STERSTAB14";

        /// <summary>
        /// The vertical distance of the stability point structure.
        /// [m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword15 = "KW_STERSTAB15";

        /// <summary>
        /// The probability of failing to repair a failed closure of the stability point structure.
        /// [1/year]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword16 = "KW_STERSTAB16";

        /// <summary>
        /// The failure collision energy of the stability point structure.
        /// [kN m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword17 = "KW_STERSTAB17";

        /// <summary>
        /// The mass of the ship.
        /// [ton]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword18 = "KW_STERSTAB18";

        /// <summary>
        /// The velocity of the ship.
        /// [m/s]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword19 = "KW_STERSTAB19";

        /// <summary>
        /// The the levelling count.
        /// [1/year]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword20 = "KW_STERSTAB20";

        /// <summary>
        /// The probability of a secondary collision on the structure per levelling.
        /// [1/year/levelling]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword21 = "KW_STERSTAB21";

        /// <summary>
        /// The maximum flow velocity at which the structure is closable.
        /// [m/s]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword22 = "KW_STERSTAB22";

        /// <summary>
        /// The stability properties of the linear load model of the stability point structure.
        /// [kN/m^2]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword23 = "KW_STERSTAB23";

        /// <summary>
        /// The stability properties of the quadratic load model of the stability point structure.
        /// [kN/m]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword24 = "KW_STERSTAB24";

        /// <summary>
        /// The area of the flow aperture of the stability point structure.
        /// [m^2]
        /// </summary>
        public const string StabilityPointStructureParameterKeyword25 = "KW_STERSTAB25";

        /// <summary>
        /// The type of stability point structure inflow model.
        /// </summary>
        public const string StabilityPointStructureParameterKeyword26 = "KW_STERSTAB26";

        #endregion

        #region Required Structure *.csv header column names

        /// <summary>
        /// The column name for the identifier of a structure.
        /// </summary>
        public const string IdentificationColumnName = "identificatie";

        /// <summary>
        /// The column name for the identifier of a structure parameter.
        /// </summary>
        public const string StructureIdentificationColumnName = "kunstwerken.identificatie";

        /// <summary>
        /// The column name for the alphanumerical value associated with the structure parameter.
        /// </summary>
        public const string AlphanumericalValueColumnName = "alfanumeriekewaarde";

        /// <summary>
        /// The column name for the numerical value associated with the structure parameter.
        /// </summary>
        public const string NumericalValueColumnName = "numeriekewaarde";

        /// <summary>
        /// The column name for the variation value (standard deviation or coefficient of variation)
        /// associated with the structure parameter.
        /// </summary>
        public const string VariationValueColumnName = "standaardafwijking.variatie";

        /// <summary>
        /// The column name for the descriptor on how to interpret the value in the column
        /// named <see cref="VariationValueColumnName"/>.
        /// </summary>
        public const string VariationTypeColumnName = "boolean";

        #endregion

        #region Inflow model types

        /// <summary>
        /// Defines the value for the 'Inflow Model Type' parameter of a structure corresponding
        /// to a vertical wall.
        /// </summary>
        public const string InflowModelTypeVerticalWall = "verticalewand";

        /// <summary>
        /// Defines the value for the 'Inflow Model Type' parameter of a structure corresponding
        /// to a low sill structure.
        /// </summary>
        public const string InflowModelTypeLowSill = "lagedrempel";

        /// <summary>
        /// Defines the value for the 'Inflow Model Type' parameter of a structure corresponding
        /// to a flooded culvert structure.
        /// </summary>
        public const string InflowModelTypeFloodedCulvert = "verdronkenkoker";

        #endregion
    }
}