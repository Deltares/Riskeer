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
using System.Collections.Generic;
using System.Xml.Serialization;
using Core.Common.Base.Geometry;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.AssemblyTool.IO.Properties;

namespace Ringtoets.AssemblyTool.IO.Model
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
        public SerializableFailureMechanismSection()
        {
            FailureMechanismSectionType = Resources.FailureMechanismSectionType_FailureMechanism;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSection"/>.
        /// </summary>
        /// <param name="id">The unique ID of the section.</param>
        /// <param name="failureMechanismSections">The failure mechanism sections object the section belong to.</param>
        /// <param name="startDistance">The distance over the reference line where this section starts in meters.</param>
        /// <param name="endDistance">The distance over the reference line where this section ends in meters.</param>
        /// <param name="geometry">The geometry of the section.</param>
        /// <param name="assemblyMethod">The assembly method used to create this section.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public SerializableFailureMechanismSection(string id,
                                                   SerializableFailureMechanismSections failureMechanismSections,
                                                   double startDistance,
                                                   double endDistance,
                                                   IEnumerable<Point2D> geometry,
                                                   SerializableAssemblyMethod? assemblyMethod = null) : this()
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (failureMechanismSections == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismSections));
            }

            if (geometry == null)
            {
                throw new ArgumentNullException(nameof(geometry));
            }

            Id = id;
            StartDistance = new SerializableMeasure("m", startDistance);
            EndDistance = new SerializableMeasure("m", endDistance);
            FailureMechanismSectionsId = failureMechanismSections.Id;
            Geometry = new SerializableLine(geometry);
            Length = new SerializableMeasure("m", Math2D.Length(geometry));
            AssemblyMethod = assemblyMethod;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.Id, Namespace = AssemblyXmlIdentifiers.GmlNamespace)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent failure mechanism sections.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.FailureMechanismSectionsIdRef)]
        public string FailureMechanismSectionsId { get; set; }

        /// <summary>
        /// Gets or sets the section starting distance.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.StartDistance)]
        public SerializableMeasure StartDistance { get; set; }

        /// <summary>
        /// Gets or sets the section ending distance.
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
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Length)]
        public SerializableMeasure Length { get; set; }

        /// <summary>
        /// Gets or sets the section type.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.FailureMechanismSectionType)]
        public string FailureMechanismSectionType { get; set; }

        /// <summary>
        /// Gets or sets the assembly method used to create this section.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssemblyMethod)]
        public SerializableAssemblyMethod? AssemblyMethod { get; set; }

        /// <summary>
        /// Determines whether <see cref="AssemblyMethod"/> should be serialized.
        /// </summary>
        /// <returns><c>true</c> if <see cref="AssemblyMethod"/> has a value, <c>false</c> otherwise.</returns>
        public bool ShouldSerializeAssemblyMethod()
        {
            return AssemblyMethod.HasValue;
        }
    }
}