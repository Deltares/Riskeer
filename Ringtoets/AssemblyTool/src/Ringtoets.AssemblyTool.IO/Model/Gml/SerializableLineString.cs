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
using System.Collections.Generic;
using System.Xml.Serialization;
using Core.Common.Base.Geometry;
using Riskeer.AssemblyTool.IO.Model.Helpers;
using Riskeer.AssemblyTool.IO.Properties;

namespace Riskeer.AssemblyTool.IO.Model.Gml
{
    /// <summary>
    /// Class describing a serializable line string.
    /// </summary>
    public class SerializableLineString
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableLineString"/>.
        /// </summary>
        public SerializableLineString()
        {
            CoordinateSystem = Resources.CoordinateSystemName;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableLineString"/>.
        /// </summary>
        /// <param name="geometry">The geometry of the line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="geometry"/> contains no elements.</exception>
        public SerializableLineString(IEnumerable<Point2D> geometry) : this()
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            Geometry = GeometrySerializationFormatter.Format(geometry);
        }

        /// <summary>
        /// Gets or sets the name of the coordinate system this line is projected on.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.CoordinateSystem)]
        public string CoordinateSystem { get; set; }

        /// <summary>
        /// Gets or sets the list of coordinates representing the
        /// geometry of the line.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Geometry)]
        public string Geometry { get; set; }
    }
}