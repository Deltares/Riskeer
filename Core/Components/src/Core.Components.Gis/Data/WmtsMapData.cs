// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Components.Gis.Properties;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Class representing a mapdata coming from a Web Map Tile Service (WMTS).
    /// </summary>
    public class WmtsMapData : ImageBasedMapData
    {
        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/>.
        /// </summary>
        /// <param name="name">The name of the map data.</param>
        /// <param name="sourceCapabilitiesUrl">The URL to the capabilities of the WMTS.</param>
        /// <param name="selectedCapabilityName">The name of the capability to use.</param>
        /// <param name="preferredFormat">The type of image format. It should be for formatted
        /// in MIME.</param>
        /// <exception cref="ArgumentException">Thrown when 
        /// <list type="bullet">
        /// <item><paramref name="name"/> is <c>null</c> or only whitespace.</item>
        /// <item><paramref name="preferredFormat"/> is not stated as a MIME-type.</item>
        /// </list></exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceCapabilitiesUrl"/>, 
        /// <paramref name="selectedCapabilityName"/> or <paramref name="preferredFormat"/> is <c>null</c>.</exception>
        public WmtsMapData(string name, string sourceCapabilitiesUrl, string selectedCapabilityName,
                           string preferredFormat) : this(name)
        {
            Configure(sourceCapabilitiesUrl, selectedCapabilityName, preferredFormat);
        }

        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/> that hasn't been configured.
        /// </summary>
        /// <param name="name">The name of the map data.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is <c>null</c> or only whitespace.</exception>
        public WmtsMapData(string name) : base(name)
        {
            IsVisible = false;
        }

        /// <summary>
        /// Gets the URL of the source.
        /// </summary>
        public string SourceCapabilitiesUrl { get; private set; }

        /// <summary>
        /// Gets the name of the specific capability that is exposed by <see cref="SourceCapabilitiesUrl"/>
        /// that has been connected to for this map data.
        /// </summary>
        public string SelectedCapabilityIdentifier { get; private set; }

        /// <summary>
        /// Gets the MIME-type specification of the preferred tile image format.
        /// </summary>
        public string PreferredFormat { get; private set; }

        /// <summary>
        /// Configures this instance to use a particular WMTS.
        /// </summary>
        /// <param name="sourceCapabilitiesUrl">The URL to the capabilities of the WMTS.</param>
        /// <param name="selectedCapabilityName">The name of the capability to use.</param>
        /// <param name="preferredFormat">The type of image format. It should be for formatted
        /// in MIME.</param>
        /// <exception cref="ArgumentNullException">Thrown when any input argument is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="preferredFormat"/>
        /// is not stated as a MIME-type.</exception>
        public void Configure(string sourceCapabilitiesUrl, string selectedCapabilityName,
                              string preferredFormat)
        {
            if (sourceCapabilitiesUrl == null)
            {
                throw new ArgumentNullException(nameof(sourceCapabilitiesUrl));
            }

            if (selectedCapabilityName == null)
            {
                throw new ArgumentNullException(nameof(selectedCapabilityName));
            }

            if (preferredFormat == null)
            {
                throw new ArgumentNullException(nameof(preferredFormat));
            }

            if (!preferredFormat.StartsWith("image/"))
            {
                throw new ArgumentException(@"Specified image format is not a MIME type.", nameof(preferredFormat));
            }

            SourceCapabilitiesUrl = sourceCapabilitiesUrl;
            SelectedCapabilityIdentifier = selectedCapabilityName;
            PreferredFormat = preferredFormat;

            IsConfigured = true;
            IsVisible = true;
        }

        /// <summary>
        /// Removes the configuration to the connected WMTS.
        /// </summary>
        public void RemoveConfiguration()
        {
            SourceCapabilitiesUrl = null;
            SelectedCapabilityIdentifier = null;
            PreferredFormat = null;

            IsConfigured = false;
            IsVisible = false;

            Name = Resources.WmtsMapData_Unconfigured_name;
        }
    }
}