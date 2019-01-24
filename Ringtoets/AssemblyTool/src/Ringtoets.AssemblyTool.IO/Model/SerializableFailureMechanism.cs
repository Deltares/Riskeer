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
using Riskeer.AssemblyTool.IO.Model.DataTypes;
using Riskeer.AssemblyTool.IO.Model.Enums;
using Riskeer.AssemblyTool.IO.Model.Helpers;
using Riskeer.AssemblyTool.IO.Properties;

namespace Riskeer.AssemblyTool.IO.Model
{
    /// <summary>
    /// Class describing a serializable failure mechanism.
    /// </summary>
    [XmlType(AssemblyXmlIdentifiers.FailureMechanism)]
    public class SerializableFailureMechanism : SerializableFeatureMember
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanism"/>.
        /// </summary>
        public SerializableFailureMechanism()
        {
            DirectFailureMechanism = Resources.DirectFailureMechanism;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanism"/>.
        /// </summary>
        /// <param name="id">The unique ID of the assembly result.</param>
        /// <param name="totalAssemblyResult">The total assembly result the failure mechanism belongs to.</param>
        /// <param name="failureMechanismType">The type of the failure mechanism.</param>
        /// <param name="failureMechanismGroup">The group of the failure mechanism.</param>
        /// <param name="failureMechanismAssemblyResult">The total failure mechanism assembly result.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="id"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableFailureMechanism(string id,
                                            SerializableTotalAssemblyResult totalAssemblyResult,
                                            SerializableFailureMechanismType failureMechanismType,
                                            SerializableFailureMechanismGroup failureMechanismGroup,
                                            SerializableFailureMechanismAssemblyResult failureMechanismAssemblyResult) : this()
        {
            if (!SerializableIdValidator.Validate(id))
            {
                throw new ArgumentException($@"'{nameof(id)}' must have a value and consist only of alphanumerical characters, '-', '_' or '.'.");
            }

            if (totalAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(totalAssemblyResult));
            }

            if (failureMechanismAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismAssemblyResult));
            }

            Id = id;
            TotalAssemblyResultId = totalAssemblyResult.Id;
            FailureMechanismType = failureMechanismType;
            FailureMechanismGroup = failureMechanismGroup;
            FailureMechanismAssemblyResult = failureMechanismAssemblyResult;
        }

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.FailureMechanismId)]
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the parent total assembly result ID.
        /// </summary>
        [XmlAttribute(AssemblyXmlIdentifiers.TotalAssemblyResultIdRef)]
        public string TotalAssemblyResultId { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism type.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.FailureMechanismType)]
        public SerializableFailureMechanismType FailureMechanismType { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism group.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.FailureMechanismGroup)]
        public SerializableFailureMechanismGroup FailureMechanismGroup { get; set; }

        /// <summary>
        /// Gets or sets the direct failure mechanism indicator.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.DirectFailureMechanism)]
        public string DirectFailureMechanism { get; set; }

        /// <summary>
        /// Gets or sets the total failure mechanism assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.FailureMechanismAssemblyResult)]
        public SerializableFailureMechanismAssemblyResult FailureMechanismAssemblyResult { get; set; }
    }
}