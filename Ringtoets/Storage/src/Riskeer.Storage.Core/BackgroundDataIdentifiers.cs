// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

namespace Riskeer.Storage.Core
{
    /// <summary>
    /// Container of general identifiers related to background data.
    /// </summary>
    public static class BackgroundDataIdentifiers
    {
        /// <summary>
        /// The identifier for the configured state.
        /// </summary>
        public const string IsConfigured = "IsConfigured";

        /// <summary>
        /// The identifier for the source capabilities url.
        /// </summary>
        public const string SourceCapabilitiesUrl = "SourceCapabilitiesUrl";

        /// <summary>
        /// The identifier for the selected capability identifier.
        /// </summary>
        public const string SelectedCapabilityIdentifier = "SelectedCapabilityIdentifier";

        /// <summary>
        /// The identifier for the preferred format.
        /// </summary>
        public const string PreferredFormat = "PreferredFormat";

        /// <summary>
        /// The identifier for the well known tile source.
        /// </summary>
        public const string WellKnownTileSource = "WellKnownTileSource";
    }
}