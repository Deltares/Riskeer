﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Properties;

namespace Ringtoets.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing a serializable assessment section.
    /// </summary>
    [XmlType(AssemblyXmlIdentifiers.AssessmentSection)]
    public class SerializableAssessmentSection : SerializableFeatureMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssessmentSection"/>.
        /// </summary>
        public SerializableAssessmentSection()
        {
            AssessmentSectionType = Resources.AssessmentSectionType;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableAssessmentSection"/>.
        /// </summary>
        /// <param name="id">The unique ID of the assessment section.</param>
        /// <param name="name">The name of the assessment section.</param>
        /// <param name="geometry">The geometry of the surface line.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SerializableAssessmentSection(string id,
                                             string name,
                                             IEnumerable<Point2D> geometry) : this()
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            Id = id;
            Name = name;
            SurfaceLineLength = new SerializableMeasure(Math2D.Length(geometry));
            SurfaceLineGeometry = new SerializableLine(geometry);
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.Id, Namespace = AssemblyXmlIdentifiers.GmlNamespace)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name of the assessment section.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Name)]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the geometry of the surface line.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Geometry2D)]
        public SerializableLine SurfaceLineGeometry { get; set; }

        /// <summary>
        /// Gets or sets the length of the surface line.
        /// [m]
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Length)]
        public SerializableMeasure SurfaceLineLength { get; set; }

        /// <summary>
        /// Gets or sets the type of the assessment section.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssessmentSectionType)]
        public string AssessmentSectionType { get; set; }
    }
}