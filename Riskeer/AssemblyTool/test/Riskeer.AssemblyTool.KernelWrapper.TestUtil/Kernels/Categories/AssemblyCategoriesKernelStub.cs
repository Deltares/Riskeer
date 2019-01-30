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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.CategoryLimits;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories
{
    /// <summary>
    /// Assembly categories kernel stub for testing purposes.
    /// </summary>
    public class AssemblyCategoriesKernelStub : ICategoryLimitsCalculator
    {
        /// <summary>
        /// Gets a value indicating whether a calculation was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets the assessment section norm.
        /// </summary>
        public double AssessmentSectionNorm { get; private set; }

        /// <summary>
        /// Gets the lower limit norm.
        /// </summary>
        public double LowerLimitNorm { get; private set; }

        /// <summary>
        /// Gets the upper boundary norm.
        /// </summary>
        public double SignalingNorm { get; private set; }

        /// <summary>
        /// Gets the failure mechanism contribution.
        /// </summary>
        public double FailureMechanismContribution { get; private set; }

        /// <summary>
        /// Gets the n.
        /// </summary>
        public double N { get; private set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="Exception"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="AssemblyException"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowAssemblyExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Gets or sets the assessment section categories output.
        /// </summary>
        public CategoriesList<AssessmentSectionCategory> AssessmentSectionCategoriesOutput { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism categories output.
        /// </summary>
        public CategoriesList<FailureMechanismCategory> FailureMechanismCategoriesOutput { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism section categories output.
        /// </summary>
        public CategoriesList<FmSectionCategory> FailureMechanismSectionCategoriesOutput { get; set; }

        public CategoriesList<AssessmentSectionCategory> CalculateAssessmentSectionCategoryLimitsWbi21(AssessmentSection section)
        {
            ThrowException();

            SignalingNorm = section.FailureProbabilitySignallingLimit;
            LowerLimitNorm = section.FailureProbabilityLowerLimit;

            Calculated = true;

            return AssessmentSectionCategoriesOutput;
        }

        public CategoriesList<FailureMechanismCategory> CalculateFailureMechanismCategoryLimitsWbi11(AssessmentSection section, FailureMechanism failureMechanism)
        {
            ThrowException();

            SignalingNorm = section.FailureProbabilitySignallingLimit;
            LowerLimitNorm = section.FailureProbabilityLowerLimit;
            FailureMechanismContribution = failureMechanism.FailureProbabilityMarginFactor;
            N = failureMechanism.LengthEffectFactor;

            Calculated = true;

            return FailureMechanismCategoriesOutput;
        }

        public CategoriesList<FmSectionCategory> CalculateFmSectionCategoryLimitsWbi01(AssessmentSection section, FailureMechanism failureMechanism)
        {
            ThrowException();

            SignalingNorm = section.FailureProbabilitySignallingLimit;
            LowerLimitNorm = section.FailureProbabilityLowerLimit;
            FailureMechanismContribution = failureMechanism.FailureProbabilityMarginFactor;
            N = failureMechanism.LengthEffectFactor;

            Calculated = true;

            return FailureMechanismSectionCategoriesOutput;
        }

        public CategoriesList<FmSectionCategory> CalculateFmSectionCategoryLimitsWbi02(double assessmentSectionNorm, FailureMechanism failureMechanism)
        {
            ThrowException();

            AssessmentSectionNorm = assessmentSectionNorm;
            FailureMechanismContribution = failureMechanism.FailureProbabilityMarginFactor;
            N = failureMechanism.LengthEffectFactor;

            Calculated = true;

            return FailureMechanismSectionCategoriesOutput;
        }

        private void ThrowException()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            if (ThrowAssemblyExceptionOnCalculate)
            {
                throw new AssemblyException("entity", EAssemblyErrors.CategoryLowerLimitOutOfRange);
            }
        }
    }
}