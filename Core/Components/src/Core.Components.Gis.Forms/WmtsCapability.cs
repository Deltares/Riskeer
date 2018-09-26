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
using Core.Components.Gis.Data;

namespace Core.Components.Gis.Forms
{
    /// <summary>
    /// Class representing a capability coming from a Web Map Tile Service (WMTS).
    /// </summary>
    public class WmtsCapability
    {
        /// <summary>
        /// Creates a new instance of <see cref="WmtsCapability"/>.
        /// </summary>
        /// <param name="id">The id of the WMTS capability.</param>
        /// <param name="format">The type of image format of the WMTS capability.</param>
        /// <param name="title">The title of the WMTS capability.</param>
        /// <param name="coordinateSystem">The coordinate system of the WMTS capability.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the input parameters is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="format"/> is not stated as a MIME-type.</exception>
        public WmtsCapability(string id, string format, string title, string coordinateSystem)
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (format == null)
            {
                throw new ArgumentNullException(nameof(format));
            }

            if (title == null)
            {
                throw new ArgumentNullException(nameof(title));
            }

            if (coordinateSystem == null)
            {
                throw new ArgumentNullException(nameof(coordinateSystem));
            }

            if (!format.StartsWith("image/"))
            {
                throw new ArgumentException(@"Specified image format is not a MIME type.", nameof(format));
            }

            Id = id;
            Format = format;
            Title = title;
            CoordinateSystem = coordinateSystem;
        }

        /// <summary>
        /// Gets the id of the WMTS capability.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the image format of the WMTS capability.
        /// </summary>
        public string Format { get; }

        /// <summary>
        /// Gets the title of the WMTS capability.
        /// </summary>
        public string Title { get; }

        /// <summary>
        /// Gets the coordinate system of the WMTS capability.
        /// </summary>
        public string CoordinateSystem { get; }

        /// <summary>
        /// Creates a new instance of <see cref="WmtsMapData"/> based upon the local properties.
        /// </summary>
        /// <param name="displayName">The name of the source (for visualization purposes only).</param>
        /// <param name="sourceCapabilitiesUrl">The URL to the capabilities of the WMTS.</param>
        /// <returns>The newly created <see cref="WmtsMapData"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="displayName"/>
        /// or <paramref name="sourceCapabilitiesUrl"/> is <c>null</c>.</exception>
        public WmtsMapData ToWmtsMapdata(string displayName, string sourceCapabilitiesUrl)
        {
            return new WmtsMapData(displayName, sourceCapabilitiesUrl, Id, Format);
        }
    }
}