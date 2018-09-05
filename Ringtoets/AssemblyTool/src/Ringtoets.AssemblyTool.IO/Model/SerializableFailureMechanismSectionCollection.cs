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
using Ringtoets.AssemblyTool.IO.Model.Helpers;

namespace Ringtoets.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing a serializable failure mechanism section collection object.
    /// </summary>
    [XmlType(AssemblyXmlIdentifiers.FailureMechanismSectionCollection)]
    public class SerializableFailureMechanismSectionCollection : SerializableFeatureMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSectionCollection"/>.
        /// </summary>
        public SerializableFailureMechanismSectionCollection() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSectionCollection"/>.
        /// </summary>
        /// <param name="id">The unique ID of the sections.</param>
        /// <param name="failureMechanism">The failure mechanism the sections belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableFailureMechanismSectionCollection(string id,
                                                             SerializableFailureMechanism failureMechanism)
            : this(id)
        {
            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            FailureMechanismId = failureMechanism.Id;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSectionCollection"/>.
        /// </summary>
        /// <param name="id">The unique ID of the sections.</param>
        /// <param name="totalAssemblyResult">The total assembly result the sections belong to.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableFailureMechanismSectionCollection(string id,
                                                             SerializableTotalAssemblyResult totalAssemblyResult)
            : this(id)
        {
            if (totalAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(totalAssemblyResult));
            }

            TotalAssemblyResultId = totalAssemblyResult.Id;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismSectionCollection"/>.
        /// </summary>
        /// <param name="id">The unique ID of the sections.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="id"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        private SerializableFailureMechanismSectionCollection(string id)
        {
            if (!SerializableIdValidator.Validate(id))
            {
                throw new ArgumentException($@"'{nameof(id)}' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.");
            }

            Id = id;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.FailureMechanismSectionCollectionId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent failure mechanism.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.FailureMechanismIdRef)]
        public string FailureMechanismId { get; set; }

        /// <summary>
        /// Gets or sets the ID of the parent total assembly result.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.TotalAssemblyResultIdRef)]
        public string TotalAssemblyResultId { get; set; }
    }
}