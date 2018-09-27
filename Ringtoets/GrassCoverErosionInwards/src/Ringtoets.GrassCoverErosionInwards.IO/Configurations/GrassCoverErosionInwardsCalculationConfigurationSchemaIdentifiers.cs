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

namespace Ringtoets.GrassCoverErosionInwards.IO.Configurations
{
    /// <summary>
    /// Container of identifiers related to the grass cover erosion inwards calculation configuration schema definition.
    /// </summary>
    internal static class GrassCoverErosionInwardsCalculationConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The tag of elements containing the name of the dike profile.
        /// </summary>
        public const string DikeProfileElement = "dijkprofiel";

        /// <summary>
        /// The name for the critical flow rate stochast.
        /// </summary>
        public const string CriticalFlowRateStochastName = "overslagdebiet";

        /// <summary>
        /// The identifier for the dike height elements.
        /// </summary>
        public const string DikeHeightElement = "dijkhoogte";

        /// <summary>
        /// The identifier for the dike height calculation type elements.
        /// </summary>
        public const string DikeHeightCalculationTypeElement = "hbnberekenen";

        /// <summary>
        /// The identifier for the overtopping rate calculation type elements.
        /// </summary>
        public const string OvertoppingRateCalculationTypeElement = "overslagdebietberekenen";

        /// <summary>
        /// The tag of the element containing the value whether illustration points should be read for the overtopping calculation.
        /// </summary>
        public const string ShouldOvertoppingOutputIllustrationPointsBeCalculatedElement = "illustratiepunteninlezen";

        /// <summary>
        /// The tag of the element containing the value whether illustration points should be read for the dike height calculation.
        /// </summary>
        public const string ShouldDikeHeightIllustrationPointsBeCalculatedElementElement = "hbnillustratiepunteninlezen";

        /// <summary>
        /// The tag of the element containing the value whether illustration points should be read for the overtopping rate calculation.
        /// </summary>
        public const string ShouldOvertoppingRateIllustrationPointsBeCalculatedElement = "overslagdebietillustratiepunteninlezen";
    }
}