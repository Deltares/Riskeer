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
using Ringtoets.AssemblyTool.IO.Model.Gml;

namespace Ringtoets.AssemblyTool.IO.Model.DataTypes
{
    /// <summary>
    /// Class that describes a serializable line.
    /// </summary>
    public class SerializableLine
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableLine"/>.
        /// </summary>
        public SerializableLine() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableLine"/>.
        /// </summary>
        /// <param name="geometry">The geometry of the line.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="geometry"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="geometry"/> contains no elements.</exception>
        public SerializableLine(IEnumerable<Point2D> geometry)
        {
            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            LineString = new SerializableLineString(geometry);
        }

        /// <summary>
        /// Gets or sets the line string containing the geometry of the line.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.LineString, Namespace = AssemblyXmlIdentifiers.GmlNamespace)]
        public SerializableLineString LineString { get; set; }
    }
}