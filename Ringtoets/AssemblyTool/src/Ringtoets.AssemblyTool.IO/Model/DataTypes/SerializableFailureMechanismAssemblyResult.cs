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

using System.Xml.Serialization;
using Ringtoets.AssemblyTool.IO.Model.Enums;

namespace Ringtoets.AssemblyTool.IO.Model.DataTypes
{
    /// <summary>
    /// Class describing a serializable failure mechanism assembly result.
    /// </summary>
    public class SerializableFailureMechanismAssemblyResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismAssemblyResult"/>.
        /// </summary>
        public SerializableFailureMechanismAssemblyResult()
        {
            Status = "VOLLDG";
        }

        /// <summary>
        /// Creates a new instance of <see cref="SerializableFailureMechanismAssemblyResult"/>.
        /// </summary>
        /// <param name="assemblyMethod">The method used to assemble this result.</param>
        /// <param name="categoryGroup">The category group of this assembly result.</param>
        /// <param name="probability">The probability of this assembly result.</param>
        public SerializableFailureMechanismAssemblyResult(SerializableAssemblyMethod assemblyMethod,
                                                          SerializableFailureMechanismCategoryGroup categoryGroup,
                                                          double? probability = null) : this()
        {
            CategoryGroup = categoryGroup;
            Probability = probability;
            AssemblyMethod = assemblyMethod;
        }

        /// <summary>
        /// Gets or sets the name of the method used to assemble this result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssemblyMethod)]
        public SerializableAssemblyMethod AssemblyMethod { get; set; }

        /// <summary>
        /// Gets or sets the category group of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.AssessmentSectionCategoryGroup)]
        public SerializableFailureMechanismCategoryGroup CategoryGroup { get; set; }

        /// <summary>
        /// Gets or sets the probability of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Probability)]
        public double? Probability { get; set; }

        /// <summary>
        /// Gets or sets the status of this assembly result.
        /// </summary>
        [XmlElement(AssemblyXmlIdentifiers.Status)]
        public string Status { get; set; }

        /// <summary>
        /// Determines whether <see cref="Probability"/> should be serialized.
        /// </summary>
        /// <returns><c>true</c> if <see cref="Probability"/> has a value, <c>false</c> otherwise.</returns>
        public bool ShouldSerializeProbability()
        {
            return Probability.HasValue;
        }
    }
}