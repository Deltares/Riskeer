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
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.IO.Converters;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.
    /// </summary>
    public static class ExportableFailureMechanismSectionAssemblyResultFactory
    {
        /// <summary>
        /// Creates a <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of
        /// the created <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.</param>
        /// <param name="sectionResult">The <see cref="TSectionResult"/> to assemble for.</param>
        /// <param name="failureMechanism">The <see cref="TFailureMechanism"/> the <paramref name="sectionResult"/>
        /// belongs to.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to use in the assembly.</param>
        /// <param name="assemblyFunc">The <see cref="Func{T1,T2,T3,TResult}"/> to perform the assembly.</param>
        /// <typeparam name="TSectionResult">The type of section result.</typeparam>
        /// <typeparam name="TFailureMechanism">The type of failure mechanism.</typeparam>
        /// <returns>An <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when <paramref name="assemblyFunc"/> returns an invalid value
        /// that cannot be exported.</exception>
        public static ExportableFailureMechanismSectionAssemblyResult Create<TSectionResult, TFailureMechanism>(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry,
            TSectionResult sectionResult, TFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
            Func<TSectionResult, TFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> assemblyFunc)
            where TFailureMechanism : IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (sectionResult == null)
            {
                throw new ArgumentNullException(nameof(sectionResult));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (assemblyFunc == null)
            {
                throw new ArgumentNullException(nameof(assemblyFunc));
            }

            if (registry.Contains(sectionResult))
            {
                return registry.Get(sectionResult);
            }

            FailureMechanismSectionAssemblyResultWrapper assemblyResultWrapper = assemblyFunc(
                sectionResult, failureMechanism, assessmentSection);
            FailureMechanismSectionAssemblyResult assemblyResult = assemblyResultWrapper.AssemblyResult;

            if (assemblyResult.FailureMechanismSectionAssemblyGroup == FailureMechanismSectionAssemblyGroup.NoResult
                || assemblyResult.FailureMechanismSectionAssemblyGroup == FailureMechanismSectionAssemblyGroup.Dominant)
            {
                throw new AssemblyFactoryException("The assembly result is invalid and cannot be created.");
            }

            var exportableFailureMechanismSectionAssemblyResult = new ExportableFailureMechanismSectionAssemblyResult(
                idGenerator.GetUniqueId(Resources.ExportableFailureMechanismSectionAssemblyResult_IdPrefix),
                registry.Get(sectionResult.Section),
                assemblyResult.SectionProbability,
                ExportableFailureMechanismSectionAssemblyGroupConverter.ConvertTo(assemblyResult.FailureMechanismSectionAssemblyGroup),
                ExportableAssemblyMethodConverter.ConvertTo(assemblyResultWrapper.AssemblyGroupMethod),
                ExportableAssemblyMethodConverter.ConvertTo(assemblyResultWrapper.ProbabilityMethod));

            registry.Register(sectionResult, exportableFailureMechanismSectionAssemblyResult);
            return exportableFailureMechanismSectionAssemblyResult;
        }
    }
}