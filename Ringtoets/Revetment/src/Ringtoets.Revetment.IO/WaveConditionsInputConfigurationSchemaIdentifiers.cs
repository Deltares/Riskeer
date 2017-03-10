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

namespace Ringtoets.Revetment.IO
{
    /// <summary>
    /// Container of identifiers related to the piping configuration schema definition.
    /// </summary>
    internal static class WaveConditionsInputConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The tag of elements containing the name of the hydraulic boundary location.
        /// </summary>
        internal const string HydraulicBoundaryLocationElement = "hrlocatie";

        /// <summary>
        /// The tag of elements containing the upper boundary of revetment.
        /// </summary>
        internal const string UpperBoundaryRevetment = "bovengrensbekleding";

        /// <summary>
        /// The tag of elements containing the lower boundary of revetment.
        /// </summary>
        internal const string LowerBoundaryRevetment = "ondergrensbekleding";

        /// <summary>
        /// The tag of elements containing the upper boundary of water level.
        /// </summary>
        internal const string UpperBoundaryWaterLevels = "bovengrenswaterstanden";

        /// <summary>
        /// The tag of elements containing the lower boundary of water level.
        /// </summary>
        internal const string LowerBoundaryWaterLevels = "ondergrenswaterstanden";

        /// <summary>
        /// The tag of elements containing the step size of the water levels.
        /// </summary>
        internal const string StepSize = "stapgrootte";

        /// <summary>
        /// The tag of elements containing the name of the foreshore profile.
        /// </summary>
        internal const string ForeshoreProfile = "voorlandprofiel";
    }
}