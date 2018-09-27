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

namespace Ringtoets.Piping.IO.Configurations
{
    /// <summary>
    /// Container of identifiers related to the piping calculation configuration schema definition.
    /// </summary>
    internal static class PipingCalculationConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// The identifier for assessment level elements.
        /// </summary>
        public const string AssessmentLevelElement = "toetspeil";

        /// <summary>
        /// The identifier for water level elements.
        /// </summary>
        public const string WaterLevelElement = "waterstand";

        /// <summary>
        /// The identifier for surface line elements.
        /// </summary>
        public const string SurfaceLineElement = "profielschematisatie";

        /// <summary>
        /// The identifier for entry point elements.
        /// </summary>
        public const string EntryPointLElement = "intredepunt";

        /// <summary>
        /// The identifier for exit point elements.
        /// </summary>
        public const string ExitPointLElement = "uittredepunt";

        /// <summary>
        /// The identifier for stochastic soil model elements.
        /// </summary>
        public const string StochasticSoilModelElement = "ondergrondmodel";

        /// <summary>
        /// The identifier for stochastic soil profile elements.
        /// </summary>
        public const string StochasticSoilProfileElement = "ondergrondschematisatie";

        /// <summary>
        /// The identifier for the phreatic level exit stochast names.
        /// </summary>
        public const string PhreaticLevelExitStochastName = "polderpeil";

        /// <summary>
        /// The identifier for the damping factor exit stochast names.
        /// </summary>
        public const string DampingFactorExitStochastName = "dempingsfactor";
    }
}