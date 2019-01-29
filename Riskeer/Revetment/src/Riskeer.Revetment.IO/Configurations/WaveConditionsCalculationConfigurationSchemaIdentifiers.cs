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

namespace Riskeer.Revetment.IO.Configurations
{
    /// <summary>
    /// Container of identifiers related to the wave conditions calculation configuration schema definition.
    /// </summary>
    public static class WaveConditionsCalculationConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The tag of elements containing the category type of revetment.
        /// </summary>
        public const string CategoryType = "categoriegrens";

        /// <summary>
        /// The tag of elements containing the upper boundary of revetment.
        /// </summary>
        public const string UpperBoundaryRevetment = "bovengrensbekleding";

        /// <summary>
        /// The tag of elements containing the lower boundary of revetment.
        /// </summary>
        public const string LowerBoundaryRevetment = "ondergrensbekleding";

        /// <summary>
        /// The tag of elements containing the upper boundary of water level.
        /// </summary>
        public const string UpperBoundaryWaterLevels = "bovengrenswaterstanden";

        /// <summary>
        /// The tag of elements containing the lower boundary of water level.
        /// </summary>
        public const string LowerBoundaryWaterLevels = "ondergrenswaterstanden";

        /// <summary>
        /// The tag of elements containing the step size of the water levels.
        /// </summary>
        public const string StepSize = "stapgrootte";

        /// <summary>
        /// The tag of elements containing the name of the foreshore profile.
        /// </summary>
        public const string ForeshoreProfile = "voorlandprofiel";
    }
}