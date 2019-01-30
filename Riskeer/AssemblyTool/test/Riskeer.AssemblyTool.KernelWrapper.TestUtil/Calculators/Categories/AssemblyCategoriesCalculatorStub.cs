// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Categories;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories
{
    /// <summary>
    /// Assembly categories calculator stub for testing purposes.
    /// </summary>
    public class AssemblyCategoriesCalculatorStub : IAssemblyCategoriesCalculator
    {
        /// <summary>
        /// Gets the signaling norm that is used in the calculation.
        /// </summary>
        public double SignalingNorm { get; private set; }

        /// <summary>
        /// Gets the lower limit norm that is used in the calculation.
        /// </summary>
        public double LowerLimitNorm { get; private set; }

        /// <summary>
        /// Gets the normative norm that is used in the calculation.
        /// </summary>
        public double NormativeNorm { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect'
        /// that is used in the calculation.
        /// </summary>
        public double FailureMechanismN { get; private set; }

        /// <summary>
        /// Gets the failure mechanism contribution that is used in the calculation.
        /// </summary>
        public double FailureMechanismContribution { get; private set; }

        /// <summary>
        /// Gets the assembly categories input used in the assembly calculation methods.
        /// </summary>
        public AssemblyCategoriesInput AssemblyCategoriesInput { get; private set; }

        /// <summary>
        /// Gets or sets the output of the <see cref="AssessmentSectionAssemblyCategory"/> calculation.
        /// </summary>
        public IEnumerable<AssessmentSectionAssemblyCategory> AssessmentSectionCategoriesOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the <see cref="FailureMechanismAssemblyCategory"/> calculation.
        /// </summary>
        public IEnumerable<FailureMechanismAssemblyCategory> FailureMechanismCategoriesOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the <see cref="CalculateFailureMechanismSectionCategories"/> calculation.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategory> FailureMechanismSectionCategoriesOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the <see cref="CalculateFailureMechanismSectionCategories"/> calculation.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategory> GeotechnicalFailureMechanismSectionCategoriesOutput { get; set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public IEnumerable<AssessmentSectionAssemblyCategory> CalculateAssessmentSectionCategories(double signalingNorm, double lowerLimitNorm)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssemblyCategoriesCalculatorException("Message", new Exception());
            }

            SignalingNorm = signalingNorm;
            LowerLimitNorm = lowerLimitNorm;

            return AssessmentSectionCategoriesOutput
                   ?? (AssessmentSectionCategoriesOutput = new[]
                          {
                              new AssessmentSectionAssemblyCategory(1, 2, AssessmentSectionAssemblyCategoryGroup.A),
                              new AssessmentSectionAssemblyCategory(2.01, 3, AssessmentSectionAssemblyCategoryGroup.B),
                              new AssessmentSectionAssemblyCategory(3.01, 4, AssessmentSectionAssemblyCategoryGroup.C)
                          });
        }

        public IEnumerable<FailureMechanismAssemblyCategory> CalculateFailureMechanismCategories(AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssemblyCategoriesCalculatorException("Message", new Exception());
            }

            AssemblyCategoriesInput = assemblyCategoriesInput;

            return FailureMechanismCategoriesOutput
                   ?? (FailureMechanismCategoriesOutput = new[]
                          {
                              new FailureMechanismAssemblyCategory(1, 2, FailureMechanismAssemblyCategoryGroup.It),
                              new FailureMechanismAssemblyCategory(2.01, 3, FailureMechanismAssemblyCategoryGroup.IIt),
                              new FailureMechanismAssemblyCategory(3.01, 4, FailureMechanismAssemblyCategoryGroup.IIIt)
                          });
        }

        public IEnumerable<FailureMechanismSectionAssemblyCategory> CalculateFailureMechanismSectionCategories(
            AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssemblyCategoriesCalculatorException("Message", new Exception());
            }

            AssemblyCategoriesInput = assemblyCategoriesInput;

            return FailureMechanismSectionCategoriesOutput
                   ?? (FailureMechanismSectionCategoriesOutput = new[]
                          {
                              new FailureMechanismSectionAssemblyCategory(1, 2, FailureMechanismSectionAssemblyCategoryGroup.Iv),
                              new FailureMechanismSectionAssemblyCategory(2.01, 3, FailureMechanismSectionAssemblyCategoryGroup.IIv),
                              new FailureMechanismSectionAssemblyCategory(3.01, 4, FailureMechanismSectionAssemblyCategoryGroup.IIIv)
                          });
        }

        public IEnumerable<FailureMechanismSectionAssemblyCategory> CalculateGeotechnicalFailureMechanismSectionCategories(
            double normativeNorm,
            double failureMechanismN,
            double failureMechanismContribution)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssemblyCategoriesCalculatorException("Message", new Exception());
            }

            NormativeNorm = normativeNorm;
            FailureMechanismN = failureMechanismN;
            FailureMechanismContribution = failureMechanismContribution;

            return GeotechnicalFailureMechanismSectionCategoriesOutput
                   ?? (GeotechnicalFailureMechanismSectionCategoriesOutput = new[]
                          {
                              new FailureMechanismSectionAssemblyCategory(1, 2.1, FailureMechanismSectionAssemblyCategoryGroup.IIIv),
                              new FailureMechanismSectionAssemblyCategory(2.2, 3.1, FailureMechanismSectionAssemblyCategoryGroup.IVv),
                              new FailureMechanismSectionAssemblyCategory(3.2, 4, FailureMechanismSectionAssemblyCategoryGroup.Vv)
                          });
        }
    }
}