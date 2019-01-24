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
using System.Xml.Serialization;
using Core.Common.Base.Geometry;

namespace Riskeer.AssemblyTool.IO.Model.Gml
{
    /// <summary>
    /// Class describing a boundary object.
    /// </summary>
    public class SerializableBoundary
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableBoundary"/>.
        /// </summary>
        public SerializableBoundary() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableBoundary"/>.
        /// </summary>
        /// <param name="lowerCorner">The lower corner of the boundary.</param>
        /// <param name="upperCorner">The upper corner of the boundary.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SerializableBoundary(Point2D lowerCorner, Point2D upperCorner)
        {
            if (lowerCorner == null)
            {
                throw new ArgumentNullException(nameof(lowerCorner));
            }

            if (upperCorner == null)
            {
                throw new ArgumentNullException(nameof(upperCorner));
            }

            Envelope = new SerializableEnvelope(lowerCorner, upperCorner);
        }

        /// <summary>
        /// Gets or sets the envelope describing the boundary.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Envelope)]
        public SerializableEnvelope Envelope { get; set; }
    }
}