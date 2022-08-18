// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
using System.Xml.Serialization;
using Riskeer.AssemblyTool.IO.ModelOld.DataTypes;
using Riskeer.AssemblyTool.IO.ModelOld.Enums;
using Riskeer.AssemblyTool.IO.ModelOld.Helpers;

namespace Riskeer.AssemblyTool.IO.ModelOld
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
        public SerializableFailureMechanism() {}

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanism"/>.
        /// </summary>
        /// <param name="id">The unique assembly ID.</param>
        /// <param name="failureMechanismType">The type of the failure mechanism.</param>
        /// <param name="code">The code of the failure mechanism.</param>
        /// <param name="name">The name of the failure mechanism.</param>
        /// <param name="totalAssemblyResult">The total assembly result the failure mechanism belongs to.</param>
        /// <param name="failureMechanismAssemblyResult">The total failure mechanism assembly result.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter except <paramref name="id"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="id"/> is invalid.</exception>
        public SerializableFailureMechanism(string id, SerializableFailureMechanismType failureMechanismType, string code,
                                            string name, SerializableTotalAssemblyResult totalAssemblyResult,
                                            SerializableFailureMechanismAssemblyResult failureMechanismAssemblyResult)
            : this()
        {
            SerializableIdValidator.ThrowIfInvalid(id);

            if (totalAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(totalAssemblyResult));
            }

            if (failureMechanismAssemblyResult == null)
            {
                throw new ArgumentNullException(nameof(failureMechanismAssemblyResult));
            }

            Id = id;
            FailureMechanismType = failureMechanismType;
            TotalAssemblyResultId = totalAssemblyResult.Id;
            GenericFailureMechanismCode = code;
            SpecificFailureMechanismName = name;
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
        /// Gets or sets the code of the failure mechanism.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.GenericFailureMechanism)]
        public string GenericFailureMechanismCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the failure mechanism.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.SpecificFailureMechanism)]
        public string SpecificFailureMechanismName { get; set; }

        /// <summary>
        /// Gets or sets the total failure mechanism assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.FailureMechanismAssemblyResult)]
        public SerializableFailureMechanismAssemblyResult FailureMechanismAssemblyResult { get; set; }

        /// <summary>
        /// Determines whether <see cref="GenericFailureMechanismCode"/> should be serialized.
        /// </summary>
        /// <returns><c>true</c> if <see cref="FailureMechanismType"/> is <see cref="SerializableFailureMechanismType.Generic"/>;
        /// <c>false</c> otherwise.</returns>
        public bool ShouldSerializeGenericFailureMechanismCode()
        {
            return FailureMechanismType == SerializableFailureMechanismType.Generic;
        }

        /// <summary>
        /// Determines whether <see cref="SpecificFailureMechanismName"/> should be serialized.
        /// </summary>
        /// <returns><c>true</c> if <see cref="FailureMechanismType"/> is <see cref="SerializableFailureMechanismType.Specific"/>;
        /// <c>false</c> otherwise.</returns>
        public bool ShouldSerializeSpecificFailureMechanismName()
        {
            return FailureMechanismType == SerializableFailureMechanismType.Specific;
        }
    }
}