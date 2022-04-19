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
using System.Collections.Generic;
using Riskeer.AssemblyTool.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Exceptions;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Integration.IO.Assembly;
using Riskeer.Integration.IO.Helpers;

namespace Riskeer.Integration.IO.Factories
{
    /// <summary>
    /// Factory to create instances of <see cref="ExportableFailureMechanism"/>
    /// with assembly results.
    /// </summary>
    public static class ExportableFailureMechanismFactory
    {
        /// <summary>
        /// Creates an <see cref="ExportableFailureMechanism"/>
        /// with assembly results based on the input parameters.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to create an <see cref="ExportableFailureMechanism"/> for.</param>
        /// <param name="assessmentSection">The assessment section the failure mechanism belongs to.</param>
        /// <param name="assembleFailureMechanismFunc">The <see cref="Func{T1,T2,TResult}"/> to perform
        /// the failure mechanism assembly.</param>
        /// <param name="assembleFailureMechanismSectionFunc">The <see cref="Func{T1,T2,T3,TResult}"/>
        /// to perform the failure mechanism section assembly.</param>
        /// <param name="failureMechanismType">The type of the failure mechanism.</param>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
        /// <typeparam name="TSectionResult">The type of the section result.</typeparam>
        /// <returns>An <see cref="ExportableFailureMechanism"/> with assembly results.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="failureMechanism"/>,
        /// <paramref name="assessmentSection"/>, <paramref name="assembleFailureMechanismFunc"/> or
        /// <paramref name="assembleFailureMechanismSectionFunc"/> is <c>null</c>.</exception>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        public static ExportableFailureMechanism CreateExportableFailureMechanism<TFailureMechanism, TSectionResult>(
            TFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
            Func<TFailureMechanism, IAssessmentSection, FailureMechanismAssemblyResultWrapper> assembleFailureMechanismFunc,
            Func<TSectionResult, TFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> assembleFailureMechanismSectionFunc,
            ExportableFailureMechanismType failureMechanismType)
            where TFailureMechanism : IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
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

            return new ExportableFailureMechanism(
                new ExportableFailureMechanismAssemblyResult(
                    assembleFailureMechanismFunc(failureMechanism, assessmentSection).AssemblyResult,
                    failureMechanism.AssemblyResult.IsManualProbability()),
                CreateExportableFailureMechanismSectionResults(
                    failureMechanism, assessmentSection, assembleFailureMechanismSectionFunc),
                failureMechanismType, failureMechanism.Code, failureMechanism.Name);
        }

        /// <summary>
        /// Creates a collection of <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/>
        /// with assembly results based on <paramref name="failureMechanism"/>.
        /// </summary>
        /// <param name="failureMechanism">The failure mechanism to create a collection of
        /// <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/> for.</param>
        /// <param name="assessmentSection">The <see cref="IAssessmentSection"/> to use in the assembly.</param>
        /// <param name="assembleFailureMechanismSectionFunc">The <see cref="Func{T1,T2,T3,TResult}"/>
        /// to perform the failure mechanism section assembly.</param>
        /// <typeparam name="TFailureMechanism">The type of the failure mechanism.</typeparam>
        /// <typeparam name="TSectionResult">The type of the section result.</typeparam>
        /// <returns>A collection of <see cref="ExportableFailureMechanismSectionAssemblyWithProbabilityResult"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when assembly results cannot be created.</exception>
        private static IEnumerable<ExportableFailureMechanismSectionAssemblyWithProbabilityResult> CreateExportableFailureMechanismSectionResults
            <TFailureMechanism, TSectionResult>(
                TFailureMechanism failureMechanism, IAssessmentSection assessmentSection,
                Func<TSectionResult, TFailureMechanism, IAssessmentSection, FailureMechanismSectionAssemblyResultWrapper> assembleFailureMechanismSectionFunc)
            where TFailureMechanism : IFailureMechanism<TSectionResult>
            where TSectionResult : FailureMechanismSectionResult
        {
            IDictionary<TSectionResult, ExportableFailureMechanismSection> failureMechanismSectionsLookup =
                ExportableFailureMechanismSectionHelper.CreateFailureMechanismSectionResultLookup(failureMechanism.SectionResults);

            var exportableResults = new List<ExportableFailureMechanismSectionAssemblyWithProbabilityResult>();
            foreach (KeyValuePair<TSectionResult, ExportableFailureMechanismSection> failureMechanismSectionPair in failureMechanismSectionsLookup)
            {
                FailureMechanismSectionAssemblyResultWrapper assemblyResultWrapper = assembleFailureMechanismSectionFunc(
                    failureMechanismSectionPair.Key, failureMechanism, assessmentSection);
                FailureMechanismSectionAssemblyResult assemblyResult = assemblyResultWrapper.AssemblyResult;

                exportableResults.Add(
                    new ExportableFailureMechanismSectionAssemblyWithProbabilityResult(
                        failureMechanismSectionPair.Value, assemblyResult.FailureMechanismSectionAssemblyGroup,
                        assemblyResult.SectionProbability, ExportableAssemblyMethod.WBI0A2));
            }

            return exportableResults;
        }
    }
}