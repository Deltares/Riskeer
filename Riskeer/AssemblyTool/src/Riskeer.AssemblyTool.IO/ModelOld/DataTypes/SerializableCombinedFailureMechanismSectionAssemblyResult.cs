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

using System.Xml.Serialization;
using Riskeer.AssemblyTool.IO.ModelOld.Enums;
using Riskeer.AssemblyTool.IO.Properties;

namespace Riskeer.AssemblyTool.IO.ModelOld.DataTypes
{
    /// <summary>
    /// Class describing a serializable combined failure mechanism section assembly result.
    /// </summary>
    public class SerializableCombinedFailureMechanismSectionAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableCombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        public SerializableCombinedFailureMechanismSectionAssemblyResult()
        {
            Status = Resources.FullAssembly;
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableCombinedFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method used to assemble this result.</param>
        /// <param name="failureMechanismType">The type of the failure mechanism.</param>
        /// <param name="code">The code of the failure mechanism this result is assembled for.</param>
        /// <param name="name">The name of the failure mechanism this result is assembled for.</param>
        /// <param name="assemblyGroup">The group of this assembly result.</param>
        public SerializableCombinedFailureMechanismSectionAssemblyResult(
            SerializableAssemblyMethod assemblyMethod, SerializableFailureMechanismType failureMechanismType,
            string code, string name, SerializableFailureMechanismSectionAssemblyGroup assemblyGroup)
            : this()
        {
            AssemblyMethod = assemblyMethod;
            FailureMechanismType = failureMechanismType;
            GenericFailureMechanismCode = code;
            SpecificFailureMechanismName = name;
            AssemblyGroup = assemblyGroup;
        }

        /// <summary>
        /// Gets or sets the name of the method used to assemble this result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssemblyMethod)]
        public SerializableAssemblyMethod AssemblyMethod { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism type of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.FailureMechanismType)]
        public SerializableFailureMechanismType FailureMechanismType { get; set; }

        /// <summary>
        /// Gets or sets the code of the failure mechanism this result is assembled for.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.GenericFailureMechanism)]
        public string GenericFailureMechanismCode { get; set; }

        /// <summary>
        /// Gets or sets the name of the failure mechanism this result is assembled for.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.SpecificFailureMechanism)]
        public string SpecificFailureMechanismName { get; set; }

        /// <summary>
        /// Gets or sets the group of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.FailureMechanismSectionAssemblyGroup)]
        public SerializableFailureMechanismSectionAssemblyGroup AssemblyGroup { get; set; }

        /// <summary>
        /// Gets or sets the status of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Status)]
        public string Status { get; set; }

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