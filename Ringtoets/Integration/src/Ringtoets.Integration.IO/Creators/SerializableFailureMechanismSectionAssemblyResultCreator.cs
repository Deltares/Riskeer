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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.IO.Model.DataTypes;
using Ringtoets.AssemblyTool.IO.Model.Enums;
using Ringtoets.Integration.IO.Assembly;

namespace Ringtoets.Integration.IO.Creators
{
    /// <summary>
    /// Creator to create instances of <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
    /// </summary>
    public static class SerializableFailureMechanismSectionAssemblyResultCreator
    {
        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
        /// based on a <see cref="ExportableSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="assessmentType">The type of assessment the
        /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/> represents.</param>
        /// <param name="sectionResult">The <see cref="ExportableSectionAssemblyResult"/> to create a
        /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public static SerializableFailureMechanismSectionAssemblyResult Create(SerializableAssessmentType assessmentType, ExportableSectionAssemblyResult sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (sectionResult.AssemblyCategory == FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                return new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethodCreator.Create(sectionResult.AssemblyMethod),
                                                                             assessmentType);
            }

            return new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethodCreator.Create(sectionResult.AssemblyMethod),
                                                                         assessmentType,
                                                                         SerializableFailureMechanismSectionCategoryGroupCreator.Create(sectionResult.AssemblyCategory));
        }

        /// <summary>
        /// Creates an instance of <see cref="SerializableFailureMechanismSectionAssemblyResult"/>
        /// based on a <see cref="ExportableSectionAssemblyResultWithProbability"/>.
        /// </summary>
        /// <param name="assessmentType">The type of assessment the
        /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/> represents.</param>
        /// <param name="sectionResult">The <see cref="ExportableSectionAssemblyResultWithProbability"/> to create a
        /// <see cref="SerializableFailureMechanismSectionAssemblyResult"/> for.</param>
        /// <returns>A <see cref="SerializableFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="sectionResult"/> is <c>null</c>.</exception>
        public static SerializableFailureMechanismSectionAssemblyResult Create(SerializableAssessmentType assessmentType, ExportableSectionAssemblyResultWithProbability sectionResult)
        {
            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (sectionResult.AssemblyCategory == FailureMechanismSectionAssemblyCategoryGroup.None)
            {
                return new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethodCreator.Create(sectionResult.AssemblyMethod),
                                                                             assessmentType);
            }

            return new SerializableFailureMechanismSectionAssemblyResult(SerializableAssemblyMethodCreator.Create(sectionResult.AssemblyMethod),
                                                                         assessmentType,
                                                                         SerializableFailureMechanismSectionCategoryGroupCreator.Create(sectionResult.AssemblyCategory),
                                                                         sectionResult.Probability);
        }
    }
}