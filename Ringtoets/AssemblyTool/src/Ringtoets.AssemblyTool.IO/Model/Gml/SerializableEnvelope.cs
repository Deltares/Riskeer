// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

namespace Ringtoets.AssemblyTool.IO.Model.Gml
{
    /// <summary>
    /// Class containing the data describing a GML Envelope object
    /// </summary>
    public class SerializableEnvelope
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableEnvelope"/>.
        /// </summary>
        public SerializableEnvelope() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableEnvelope"/>.
        /// </summary>
        /// <param name="lowerCorner">The lower corner of the envelope.</param>
        /// <param name="upperCorner">The upper corner of the envelope.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SerializableEnvelope(Point2D lowerCorner, Point2D upperCorner)
        {
            if (lowerCorner == null)
            {
                throw new ArgumentNullException(nameof(lowerCorner));
            }

            if (upperCorner == null)
            {
                throw new ArgumentNullException(nameof(upperCorner));
            }

            LowerCorner = lowerCorner.X + " " + lowerCorner.Y;
            UpperCorner = upperCorner.X + " " + upperCorner.Y;
        }

        /// <summary>
        /// The coordinates of the lower corner of the envelope.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.LowerCorner, Namespace = AssemblyXmlIdentifiers.GmlNamespace)]
        public string LowerCorner { get; set; }

        /// <summary>
        /// The coordinates of the upper corner of the envelope.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.UpperCorner, Namespace = AssemblyXmlIdentifiers.GmlNamespace)]
        public string UpperCorner { get; set; }
    }
}