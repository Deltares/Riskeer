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

namespace Core.Components.Gis.IO
{
    /// <summary>
    /// Defines the element names of the WMTS connection configuration file.
    /// </summary>
    public static class WmtsConnectionInfoXmlDefinitions
    {
        /// <summary>
        /// Gets the name of the root element.
        /// </summary>
        public const string RootElement = "WmtsConnections";

        /// <summary>
        /// Gets the WMTS connection element.
        /// </summary>
        public const string WmtsConnectionElement = "WmtsConnection";

        /// <summary>
        /// Gets the WMTS connection name element.
        /// </summary>
        public const string WmtsConnectionNameElement = "Name";

        /// <summary>
        /// Gets the WMTS connection URL element.
        /// </summary>
        public const string WmtsConnectionUrlElement = "URL";
    }
}