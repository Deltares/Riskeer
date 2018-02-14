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
using System.Collections.Generic;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Categories;
using Ringtoets.Common.Data.AssemblyTool;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories
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
        /// Gets the probability distribution factor that is used in the calculation.
        /// </summary>
        public double ProbabilityDistributionFactor { get; private set; }

        /// <summary>
        /// Gets the n that is used in the calculation.
        /// </summary>
        public double N { get; private set; }

        /// <summary>
        /// Gets the output of the <see cref="AssessmentSectionAssemblyCategory"/> calculation.
        /// </summary>
        public IEnumerable<AssessmentSectionAssemblyCategory> AssessmentSectionCategoriesOutput { get; private set; }

        /// <summary>
        /// Gets the output of the <see cref="CalculateFailureMechanismSectionCategories"/> calculation.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategory> FailureMechanismSectionCategoriesOutput { get; private set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the calculation.
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

        public IEnumerable<FailureMechanismSectionAssemblyCategory> CalculateFailureMechanismSectionCategories(double signalingNorm, double lowerLimitNorm,
                                                                                                               double probabilityDistributionFactor, double n)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssemblyCategoriesCalculatorException("Message", new Exception());
            }

            SignalingNorm = signalingNorm;
            LowerLimitNorm = lowerLimitNorm;
            ProbabilityDistributionFactor = probabilityDistributionFactor;
            N = n;

            return FailureMechanismSectionCategoriesOutput
                ?? (FailureMechanismSectionCategoriesOutput = new[]
                {
                    new FailureMechanismSectionAssemblyCategory(1, 2, FailureMechanismSectionAssemblyCategoryGroup.Iv),
                    new FailureMechanismSectionAssemblyCategory(2.01, 3, FailureMechanismSectionAssemblyCategoryGroup.IIv),
                    new FailureMechanismSectionAssemblyCategory(3.01, 4, FailureMechanismSectionAssemblyCategoryGroup.IIIv)
                });
        }

        public IEnumerable<FailureMechanismSectionAssemblyCategory> CalculateGeotechnicFailureMechanismSectionCategories(double signalingNorm, double lowerLimitNorm,
                                                                                                                         double probabilityDistributionFactor, double n)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new AssemblyCategoriesCalculatorException("Message", new Exception());
            }

            SignalingNorm = signalingNorm;
            LowerLimitNorm = lowerLimitNorm;
            ProbabilityDistributionFactor = probabilityDistributionFactor;
            N = n;

            return FailureMechanismSectionCategoriesOutput
                   ?? (FailureMechanismSectionCategoriesOutput = new[]
                   {
                       new FailureMechanismSectionAssemblyCategory(1, 2, FailureMechanismSectionAssemblyCategoryGroup.Iv),
                       new FailureMechanismSectionAssemblyCategory(2.01, 3, FailureMechanismSectionAssemblyCategoryGroup.IIv),
                       new FailureMechanismSectionAssemblyCategory(3.01, 4, FailureMechanismSectionAssemblyCategoryGroup.IIIv)
                   });
        }
    }
}