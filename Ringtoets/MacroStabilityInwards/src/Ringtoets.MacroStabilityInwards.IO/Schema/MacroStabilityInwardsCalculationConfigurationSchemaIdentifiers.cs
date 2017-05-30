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

namespace Ringtoets.MacroStabilityInwards.IO.Schema
{
    /// <summary>
    /// Container of identifiers related to the macro stability inwards calculation configuration schema definition.
    /// </summary>
    internal static class MacroStabilityInwardsCalculationConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The identifier for assessment level elements.
        /// </summary>
        internal const string AssessmentLevelElement = "toetspeil";

        /// <summary>
        /// The identifier for surface line elements.
        /// </summary>
        internal const string SurfaceLineElement = "profielschematisatie";

        /// <summary>
        /// The identifier for stochastic soil model elements.
        /// </summary>
        internal const string StochasticSoilModelElement = "ondergrondmodel";

        /// <summary>
        /// The identifier for stochastic soil profile elements.
        /// </summary>
        internal const string StochasticSoilProfileElement = "ondergrondschematisatie";
    }
}