// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Components.Gis.Properties;

namespace Core.Components.Gis.Data
{
    /// <summary>
    /// Class representing a mapdata coming from a Web Map Tile Service (WMTS).
    /// </summary>
    public class WmtsMapData : MapData
    {
        private RoundedDouble transparency;

        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/>.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <param name="sourceCapabilitiesUrl">The URL to the capabilities of the WMTS.</param>
        /// <param name="selectedCapabilityName">The name of the capability to use.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is <c>null</c> or only whitespace.</exception>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceCapabilitiesUrl"/> 
        /// or <paramref name="selectedCapabilityName"/> is <c>null</c>.</exception>
        public WmtsMapData(string name, string sourceCapabilitiesUrl, string selectedCapabilityName) : this(name)
        {
            Configure(sourceCapabilitiesUrl, selectedCapabilityName);
        }

        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/> that hasn't been configured.
        /// </summary>
        /// <param name="name">The name of the source.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="name"/> is <c>null</c> or only whitespace.</exception>
        private WmtsMapData(string name) : base(name)
        {
            transparency = new RoundedDouble(2, 0.0);
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
        public string SelectedCapabilityName { get; private set; }

        /// <summary>
        /// Gets or sets the transparency of the map data.
        /// </summary>
        public RoundedDouble Transparency
        {
            get
            {
                return transparency;
            }
            set
            {
                var newValue = new RoundedDouble(transparency.NumberOfDecimalPlaces, value);
                if (double.IsNaN(newValue) || newValue < 0.0 || newValue > 1.0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value),
                                                          Resources.WmtsMapData_Transparency_Value_must_be_in_zero_to_one_range);
                }

                transparency = newValue;
            }
        }

        /// <summary>
        /// Gets a value indicating if the map data is configured to use a particular WMTS.
        /// </summary>
        public bool IsConfigured { get; private set; }

        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/> configured to the 'brtachtergrondkaart'
        /// of PDOK.
        /// </summary>
        public static WmtsMapData CreateDefaultPdokMapData()
        {
            return new WmtsMapData(Resources.WmtsMapData_CreateDefaultPdokMapData_Name,
                                   "https://geodata.nationaalgeoregister.nl/tiles/service/wmts/bgtachtergrond?request=GetCapabilities",
                                   "brtachtergrondkaart");
        }

        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/> that hasn't been configured.
        /// </summary>
        /// <returns></returns>
        public static WmtsMapData CreateUnconnectedMapData()
        {
            return new WmtsMapData(Resources.WmtsMapData_Unconfigured_name);
        }

        /// <summary>
        /// Configures this instance to use a particular WMTS.
        /// </summary>
        /// <param name="sourceCapabilitiesUrl">The URL to the capabilities of the WMTS.</param>
        /// <param name="selectedCapabilityName">The name of the capability to use.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sourceCapabilitiesUrl"/> 
        /// or <paramref name="selectedCapabilityName"/> is <c>null</c>.</exception>
        public void Configure(string sourceCapabilitiesUrl, string selectedCapabilityName)
        {
            if (sourceCapabilitiesUrl == null)
            {
                throw new ArgumentNullException(nameof(sourceCapabilitiesUrl));
            }
            if (selectedCapabilityName == null)
            {
                throw new ArgumentNullException(nameof(selectedCapabilityName));
            }

            SourceCapabilitiesUrl = sourceCapabilitiesUrl;
            SelectedCapabilityName = selectedCapabilityName;

            IsConfigured = true;
            IsVisible = true;
        }

        /// <summary>
        /// Removes the configuration to the connected WMTS.
        /// </summary>
        public void RemoveConfiguration()
        {
            SourceCapabilitiesUrl = null;
            SelectedCapabilityName = null;

            IsConfigured = false;
            IsVisible = false;
        }
    }
}