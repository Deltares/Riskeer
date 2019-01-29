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

using System;

namespace Riskeer.Common.Data.AssessmentSection
{
    /// <summary>
    /// A background data configuration of WMTS tile sources.
    /// </summary>
    public class WmtsBackgroundDataConfiguration : IBackgroundDataConfiguration
    {
        /// <summary>
        /// Creates a new instance of <see cref="WmtsBackgroundDataConfiguration"/> that is not configured.
        /// </summary>
        public WmtsBackgroundDataConfiguration() : this(false, null, null, null) {}

        /// <summary>
        /// Creates a new instance of <see cref="WmtsBackgroundDataConfiguration"/> that is configured.
        /// </summary>
        /// <param name="isConfigured">Indicates if the configuration is configured to use as a tile source.</param>
        /// <param name="sourceCapabilitiesUrl">The URL to the capabilities of the WMTS.</param>
        /// <param name="selectedCapabilityIdentifier">The name of the capability to use.</param>
        /// <param name="preferredFormat">The MIME-type specification of the preferred tile image format.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters are <c>null</c> when 
        /// <paramref name="isConfigured"/> is <c>true</c>.</exception>
        /// /// <exception cref="ArgumentException">Thrown when any of the parameters are not <c>null</c> when 
        /// <paramref name="isConfigured"/> is <c>false</c>.</exception>
        public WmtsBackgroundDataConfiguration(bool isConfigured, string sourceCapabilitiesUrl,
                                               string selectedCapabilityIdentifier, string preferredFormat)
        {
            if (isConfigured)
            {
                InitalizeConfiguredWmtsBackgroundDataConfiguration(sourceCapabilitiesUrl,
                                                                   selectedCapabilityIdentifier,
                                                                   preferredFormat);
            }
            else
            {
                InitalizeUnconfiguredWmtsBackgroundDataConfiguration(sourceCapabilitiesUrl,
                                                                     selectedCapabilityIdentifier,
                                                                     preferredFormat);
            }
        }

        /// <summary>
        /// Gets if the configuration is ready to use as a tile source.
        /// </summary>
        public bool IsConfigured { get; private set; }

        /// <summary>
        /// Gets the URL to the capabilities of the WMTS.
        /// </summary>
        public string SourceCapabilitiesUrl { get; private set; }

        /// <summary>
        /// Gets the name of the capability to use.
        /// </summary>
        public string SelectedCapabilityIdentifier { get; private set; }

        /// <summary>
        /// Gets the MIME-type specification of the preferred tile image format.
        /// </summary>
        public string PreferredFormat { get; private set; }

        /// <summary>
        /// Initializes the properties of the <see cref="WmtsBackgroundDataConfiguration"/> corresponding
        /// to an unconfigured WMTS tile source.
        /// </summary>
        /// <param name="sourceCapabilitiesUrl">The URL to the capabilities of the WMTS.</param>
        /// <param name="selectedCapabilityIdentifier">The name of the capability to use.</param>
        /// <param name="preferredFormat">The MIME-type specification of the preferred tile image format.</param>
        /// <exception cref="ArgumentException">Thrown when any of the parameters are not <c>null</c>.</exception>
        private void InitalizeUnconfiguredWmtsBackgroundDataConfiguration(string sourceCapabilitiesUrl,
                                                                          string selectedCapabilityIdentifier,
                                                                          string preferredFormat)
        {
            const string exceptionMessage = "Value must be null when instantiating an unconfigured configuration.";
            if (sourceCapabilitiesUrl != null)
            {
                throw new ArgumentException(exceptionMessage, nameof(sourceCapabilitiesUrl));
            }

            if (selectedCapabilityIdentifier != null)
            {
                throw new ArgumentException(exceptionMessage, nameof(selectedCapabilityIdentifier));
            }

            if (preferredFormat != null)
            {
                throw new ArgumentException(exceptionMessage, nameof(preferredFormat));
            }

            IsConfigured = false;
        }

        /// <summary>
        /// Initializes the properties of the <see cref="WmtsBackgroundDataConfiguration"/> corresponding
        /// to a configured WMTS tile source.
        /// </summary>
        /// <param name="sourceCapabilitiesUrl">The URL to the capabilities of the WMTS.</param>
        /// <param name="selectedCapabilityIdentifier">The name of the capability to use.</param>
        /// <param name="preferredFormat">The MIME-type specification of the preferred tile image format.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the parameters are <c>null</c>.</exception>
        private void InitalizeConfiguredWmtsBackgroundDataConfiguration(string sourceCapabilitiesUrl,
                                                                        string selectedCapabilityIdentifier,
                                                                        string preferredFormat)
        {
            if (sourceCapabilitiesUrl == null)
            {
                throw new ArgumentNullException(nameof(sourceCapabilitiesUrl));
            }

            if (selectedCapabilityIdentifier == null)
            {
                throw new ArgumentNullException(nameof(selectedCapabilityIdentifier));
            }

            if (preferredFormat == null)
            {
                throw new ArgumentNullException(nameof(preferredFormat));
            }

            IsConfigured = true;
            SourceCapabilitiesUrl = sourceCapabilitiesUrl;
            SelectedCapabilityIdentifier = selectedCapabilityIdentifier;
            PreferredFormat = preferredFormat;
        }
    }
}