// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using System.Linq;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.IO.Model;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.IO.Converters;
using Riskeer.Integration.IO.Exceptions;
using Riskeer.Integration.IO.Helpers;
using Riskeer.Integration.IO.Properties;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism"/>
    /// with assembly results.
    /// </summary>
    public static class ExportableFailureMechanismFactory
    {
        /// <summary>
        /// Creates an <see cref="ExportableGenericFailureMechanism"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created items.</param>
        /// <param name="failureMechanism">The failure mechanism to create an <see cref="ExportableFailureMechanism"/> for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <param name="assembleFailureMechanismFunc">The <see cref="Func{T1,T2,TResult}"/> to perform
        /// the failure mechanism assembly.</param>
        /// <param name="assembleFailureMechanismSectionFunc">The <see cref="Func{T1,T2,T3,TResult}"/>
        /// to perform the failure mechanism section assembly.</param>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
        /// <typeparam name="TSectionResult">The type of the section result.</typeparam>
        /// <returns>An <see cref="ExportableFailureMechanism"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when <paramref name="assembleFailureMechanismSectionFunc"/>
        /// returns an invalid result that cannot be exported.</exception>
        public static ExportableGenericFailureMechanism CreateExportableGenericFailureMechanism<TFailureMechanism, TSectionResult>(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry, TFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
            Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> assembleFailureMechanismFunc,
            Func<TSectionResult, TFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> assembleFailureMechanismSectionFunc)
            where TFailureMechanism : class, IFailureMechanism<TSectionResult>
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

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (assembleFailureMechanismFunc == null)
            {
                throw new ArgumentNullException(nameof(assembleFailureMechanismFunc));
            }

            if (assembleFailureMechanismSectionFunc == null)
            {
                throw new ArgumentNullException(nameof(assembleFailureMechanismSectionFunc));
            }

            FailureMechanismAssemblyResultWrapper assemblyResultWrapper = assembleFailureMechanismFunc(failureMechanism, assessmentSection);
            return new ExportableGenericFailureMechanism(idGenerator.GetUniqueId(Resources.ExportableFailureMechanism_IdPrefix),
                                                         new ExportableFailureMechanismAssemblyResult(
                                                             assemblyResultWrapper.AssemblyResult,
                                                             ExportableAssemblyMethodConverter.ConvertTo(assemblyResultWrapper.AssemblyMethod)),
                                                         CreateExportableFailureMechanismSectionResults(
                                                             idGenerator, registry, failureMechanism, assessmentSection, assembleFailureMechanismSectionFunc),
                                                         failureMechanism.Code);
        }

        /// <summary>
        /// Creates an <see cref="ExportableSpecificFailureMechanism"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created items.</param>
        /// <param name="failureMechanism">The failure mechanism to create an <see cref="ExportableSpecificFailureMechanism"/> for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <param name="assembleFailureMechanismFunc">The <see cref="Func{T1,T2,TResult}"/> to perform
        /// the failure mechanism assembly.</param>
        /// <param name="assembleFailureMechanismSectionFunc">The <see cref="Func{T1,T2,T3,TResult}"/>
        /// to perform the failure mechanism section assembly.</param>
        /// <returns>An <see cref="ExportableSpecificFailureMechanism"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when <paramref name="assembleFailureMechanismSectionFunc"/>
        /// returns an invalid result that cannot be exported.</exception>
        public static ExportableSpecificFailureMechanism CreateExportableSpecificFailureMechanism(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry, SpecificFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
            Func<SpecificFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> assembleFailureMechanismFunc,
            Func<NonAdoptableFailureMechanismSectionResult, SpecificFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> assembleFailureMechanismSectionFunc)
        {
            if (idGenerator == null)
            {
                throw new ArgumentNullException(nameof(idGenerator));
            }

            if (registry == null)
            {
                throw new ArgumentNullException(nameof(registry));
            }

            if (failureMechanism == null)
            {
                throw new ArgumentNullException(nameof(failureMechanism));
            }

            if (assessmentSection == null)
            {
                throw new ArgumentNullException(nameof(assessmentSection));
            }

            if (assembleFailureMechanismFunc == null)
            {
                throw new ArgumentNullException(nameof(assembleFailureMechanismFunc));
            }

            if (assembleFailureMechanismSectionFunc == null)
            {
                throw new ArgumentNullException(nameof(assembleFailureMechanismSectionFunc));
            }

            FailureMechanismAssemblyResultWrapper assemblyResultWrapper = assembleFailureMechanismFunc(failureMechanism, assessmentSection);
            return new ExportableSpecificFailureMechanism(idGenerator.GetUniqueId(Resources.ExportableFailureMechanism_IdPrefix),
                                                          new ExportableFailureMechanismAssemblyResult(
                                                              assemblyResultWrapper.AssemblyResult,
                                                              ExportableAssemblyMethodConverter.ConvertTo(assemblyResultWrapper.AssemblyMethod)),
                                                          CreateExportableFailureMechanismSectionResults(
                                                              idGenerator, registry, failureMechanism, assessmentSection, assembleFailureMechanismSectionFunc),
                                                          failureMechanism.Name);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanismSectionAssemblyResult"/>
        /// with assembly results based on <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="idGenerator">The generator to generate ids for the exportable components.</param>
        /// <param name="registry">The <see cref="ExportableModelRegistry"/> to keep track of the created items.</param>
        /// <param name="failureMechanism">The failure mechanism to create a collection of
        /// <see cref="ExportableFailureMechanismSectionAssemblyResult"/> for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to use in the assembly.</param>
        /// <param name="assembleFailureMechanismSectionFunc">The <see cref="Func{T1,T2,T3,TResult}"/>
        /// to perform the failure mechanism section assembly.</param>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
        /// <typeparam name="TSectionResult">The type of the section result.</typeparam>
        /// <returns>A collection of <see cref="ExportableFailureMechanismSectionAssemblyResult"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        /// <exception cref="AssemblyFactoryException">Thrown when <paramref name="assembleFailureMechanismSectionFunc"/>
        /// returns an invalid result that cannot be exported.</exception>
        private static IEnumerable<ExportableFailureMechanismSectionAssemblyResult> CreateExportableFailureMechanismSectionResults<TFailureMechanism, TSectionResult>(
            IdentifierGenerator idGenerator, ExportableModelRegistry registry, TFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
            Func<TSectionResult, TFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> assembleFailureMechanismSectionFunc)
            where TFailureMechanism : class, IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            return failureMechanism.SectionResults.Select(
                                       sr => ExportableFailureMechanismSectionAssemblyResultFactory.Create(
                                           idGenerator, registry, sr, failureMechanism, assessmentSection,
                                           assembleFailureMechanismSectionFunc))
                                   .ToList();
        }
    }
}