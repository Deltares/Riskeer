// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.Model.Helpers;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing a serializable failure mechanism section object.
    /// </summary>
    [XmlType(AssemblyXmlIdentifiers.FailureMechanismSection)]
    public class SerializableFailureMechanismSection : SerializableFeatureMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSection"/>.
        /// </summary>
        public SerializableFailureMechanismSection() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSection"/>.
        /// </summary>
        /// <param name="id">The unique ID of the section.</param>
        /// <param name="failureMechanismSectionCollection">The failure mechanism sections object the section belong to.</param>
        /// <param name="startDistance">The distance over the reference line where this section starts in meters.</param>
        /// <param name="endDistance">The distance over the reference line where this section ends in meters.</param>
        /// <param name="geometry">The geometry of the section.</param>
        /// <param name="sectionType">The type of the section.</param>
        /// <param name="assemblyMethod">The assembly method used to create this section.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanismSectionCollection"/>,
        /// or <paramref name="geometry"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><paramref name="geometry"/> contains no elements.</item>
        /// <item><paramref name="id"/> is invalid.</item>
        /// </list>
        /// </exception>
        public SerializableFailureMechanismSection(string id,
                                                   SerializableFailureMechanismSectionCollection failureMechanismSectionCollection,
                                                   double startDistance,
                                                   double endDistance,
                                                   IEnumerable<Point2D> geometry,
                                                   SerializableFailureMechanismSectionType sectionType,
                                                   SerializableAssemblyMethod? assemblyMethod = null)
        {
            if (!SerializableIdValidator.Validate(id))
            {
                throw new ArgumentException($@"'{nameof(id)}' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.");
            }

            if (failureMechanismSectionCollection == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSectionCollection));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            Id = id;
            StartDistance = new SerializableMeasure(startDistance);
            EndDistance = new SerializableMeasure(endDistance);
            FailureMechanismSectionCollectionId = failureMechanismSectionCollection.Id;
            Geometry = new SerializableLine(geometry);
            Length = new SerializableMeasure(Math2D.Length(geometry));
            AssemblyMethod = assemblyMethod;
            FailureMechanismSectionType = sectionType;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.Id, Namespace = AssemblyXmlIdentifiers.GmlNamespace)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent failure mechanism section collection.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.FailureMechanismSectionCollectionIdRef)]
        public string FailureMechanismSectionCollectionId { get; set; }

        /// <summary>
        /// Gets or sets the section starting distance.
        /// [m]
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.StartDistance)]
        public SerializableMeasure StartDistance { get; set; }

        /// <summary>
        /// Gets or sets the section ending distance.
        /// [m]
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.EndDistance)]
        public SerializableMeasure EndDistance { get; set; }

        /// <summary>
        /// Gets or sets the section geometry.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.GeometryLine2D)]
        public SerializableLine Geometry { get; set; }

        /// <summary>
        /// Gets or sets the section length.
        /// [m]
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Length)]
        public SerializableMeasure Length { get; set; }

        /// <summary>
        /// Gets or sets the section type.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.FailureMechanismSectionType)]
        public SerializableFailureMechanismSectionType FailureMechanismSectionType { get; set; }

        /// <summary>
        /// Gets or sets the assembly method used to create this section.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssemblyMethod)]
        public SerializableAssemblyMethod? AssemblyMethod { get; set; }

        /// <summary>
        /// Determines whether <see cref="AssemblyMethod"/> should be serialized.
        /// </summary>
        /// <returns><c>true</c> if <see cref="AssemblyMethod"/> should be serialized, <c>false</c> otherwise.</returns>
        public bool ShouldSerializeAssemblyMethod()
        {
            return AssemblyMethod.HasValue;
        }
    }
}