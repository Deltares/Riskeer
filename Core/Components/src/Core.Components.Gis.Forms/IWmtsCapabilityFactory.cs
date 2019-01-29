// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using System.Collections.Generic;

namespace Core.Components.Gis.Forms
{
    /// <summary>
    /// Interface responsible for creating<see cref="WmtsCapability"/> instances for a given
    /// source.
    /// </summary>
    public interface IWmtsCapabilityFactory
    {
        /// <summary>
        /// Returns all <see cref="WmtsCapability"/> based on the capabilities of a Web Map Tile Service.
        /// </summary>
        /// <param name="capabilitiesUrl">The URL to the 'GetCapabilities' part of the service.</param>
        /// <returns>The WMTS capabilities.</returns>
        IEnumerable<WmtsCapability> GetWmtsCapabilities(string capabilitiesUrl);
    }
}