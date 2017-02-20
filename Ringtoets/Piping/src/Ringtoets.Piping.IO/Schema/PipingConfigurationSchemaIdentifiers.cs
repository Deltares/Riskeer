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

namespace Ringtoets.Piping.IO.Schema
{
    /// <summary>
    /// Container of identifiers related to the piping configuration schema definition.
    /// </summary>
    public static class PipingConfigurationSchemaIdentifiers
    {
        /// <summary>
        /// Gets the identifier for root elements.
        /// </summary>
        public static string RootElement => "root";

        /// <summary>
        /// Gets the identifier for calculation elements.
        /// </summary>
        public static string CalculationElement => "berekening";

        /// <summary>
        /// Gets the identifier for folder elements.
        /// </summary>
        public static string FolderElement => "folder";

        /// <summary>
        /// Gets the identifier for name attributes.
        /// </summary>
        public static string NameAttribute => "naam";

        /// <summary>
        /// Gets the identifier for assessment level elements.
        /// </summary>
        public static string AssessmentLevelElement => "toetspeil";

        /// <summary>
        /// Gets the identifier for hydraulic boundary location elements.
        /// </summary>
        public static string HydraulicBoundaryLocationElement => "hrlocatie";

        /// <summary>
        /// Gets the identifier for surface line elements.
        /// </summary>
        public static string SurfaceLineElement => "profielschematisatie";

        /// <summary>
        /// Gets the identifier for entry point elements.
        /// </summary>
        public static string EntryPointElement => "intredepunt";

        /// <summary>
        /// Gets the identifier for exit point elements.
        /// </summary>
        public static string ExitPointElement => "uittredepunt";

        /// <summary>
        /// Gets the identifier for stochastic soil model elements.
        /// </summary>
        public static string StochasticSoilModelElement => "ondergrondmodel";

        /// <summary>
        /// Gets the identifier for stochastic soil profile elements.
        /// </summary>
        public static string StochasticSoilProfileElement => "ondergrondschematisatie";

        /// <summary>
        /// Gets the identifier for stochast elements.
        /// </summary>
        public static string StochastElement => "stochast";

        /// <summary>
        /// Gets the identifier for mean elements.
        /// </summary>
        public static string MeanElement => "verwachtingswaarde";

        /// <summary>
        /// Gets the identifier for standard deviation elements.
        /// </summary>
        public static string StandardDeviationElement => "standaardafwijking";

        /// <summary>
        /// Gets the identifier for the phreatic level exit stochast names.
        /// </summary>
        public static string PhreaticLevelExitStochastName => "polderpeil";

        /// <summary>
        /// Gets the identifier for the damping factor exit stochast names.
        /// </summary>
        public static string DampingFactorExitStochastName => "dempingsfactor";
    }
}