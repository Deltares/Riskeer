﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

namespace Ringtoets.Common.IO.Structures
{
    /// <summary>
    /// The keywords as used in structure files.
    /// </summary>
    public static class StructureFilesKeywords
    {
        /// <summary>
        /// The first height structure parameter keyword.
        /// </summary>
        public const string HeightStructureParameterKeyword1 = "KW_HOOGTE1";

        /// <summary>
        /// The second height structure parameter keyword.
        /// </summary>
        public const string HeightStructureParameterKeyword2 = "KW_HOOGTE2";

        /// <summary>
        /// The third height structure parameter keyword.
        /// </summary>
        public const string HeightStructureParameterKeyword3 = "KW_HOOGTE3";

        /// <summary>
        /// The fourth height structure parameter keyword.
        /// </summary>
        public const string HeightStructureParameterKeyword4 = "KW_HOOGTE4";

        /// <summary>
        /// The fifth height structure parameter keyword.
        /// </summary>
        public const string HeightStructureParameterKeyword5 = "KW_HOOGTE5";

        /// <summary>
        /// The sixth height structure parameter keyword.
        /// </summary>
        public const string HeightStructureParameterKeyword6 = "KW_HOOGTE6";

        /// <summary>
        /// The seventh height structure parameter keyword.
        /// </summary>
        public const string HeightStructureParameterKeyword7 = "KW_HOOGTE7";

        /// <summary>
        /// The eighth height structure parameter keyword.
        /// </summary>
        public const string HeightStructureParameterKeyword8 = "KW_HOOGTE8";

        /// <summary>
        /// The first closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword1 = "KW_BETSLUIT1";

        /// <summary>
        /// The second closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword2 = "KW_BETSLUIT2";

        /// <summary>
        /// The third closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword3 = "KW_BETSLUIT3";

        /// <summary>
        /// The fourth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword4 = "KW_BETSLUIT4";

        /// <summary>
        /// The fifth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword5 = "KW_BETSLUIT5";

        /// <summary>
        /// The sixth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword6 = "KW_BETSLUIT6";

        /// <summary>
        /// The seventh closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword7 = "KW_BETSLUIT7";

        /// <summary>
        /// The eighth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword8 = "KW_BETSLUIT8";

        /// <summary>
        /// The ninth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword9 = "KW_BETSLUIT9";

        /// <summary>
        /// The tenth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword10 = "KW_BETSLUIT10";

        /// <summary>
        /// The eleventh closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword11 = "KW_BETSLUIT11";

        /// <summary>
        /// The twelfth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword12 = "KW_BETSLUIT12";

        /// <summary>
        /// The thirteenth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword13 = "KW_BETSLUIT13";

        /// <summary>
        /// The fourteenth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword14 = "KW_BETSLUIT14";

        /// <summary>
        /// The fifteenth closing structure parameter keyword.
        /// </summary>
        public const string ClosingStructureParameterKeyword15 = "KW_BETSLUIT15";

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
        public const string AlphanumericalValueColumnName = "alphanumeriekewaarde";

        /// <summary>
        /// The column name for the numerical value associated with the structure parameter.
        /// </summary>
        public const string NumericalValueColumnName = "numeriekewaarde";

        /// <summary>
        /// The column name for the variation value (standard deviation or coefficient of variation)
        /// associated with the structure parameter.
        /// </summary>
        public const string VariationValueColumnName = "standarddeviatie.variance";

        /// <summary>
        /// The column name for the descriptor on how to interpret the value in the column
        /// named <see cref="VariationValueColumnName"/>.
        /// </summary>
        public const string VariationTypeColumnName = "boolean";

        #endregion
    }
}