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
using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Categories;
using AssemblyTool.Kernel.Categories.CalculatorInput;
using AssemblyTool.Kernel.Data.AssemblyCategories;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Categories
{
    /// <summary>
    /// Assembly categories kernel stub for testing purposes.
    /// </summary>
    public class AssemblyCategoriesKernelStub : ICategoriesCalculator
    {
        /// <summary>
        /// Gets a value indicating whether <see cref="CalculateAssessmentSectionCategories"/> was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Gets the lower limit norm.
        /// </summary>
        public double LowerLimitNorm { get; private set; }

        /// <summary>
        /// Gets the upper boundary norm.
        /// </summary>
        public double SignalingNorm { get; private set; }

        /// <summary>
        /// Gets the probability distribution factor.
        /// </summary>
        public double ProbabilityDistributionFactor { get; private set; }

        /// <summary>
        /// Gets the n.
        /// </summary>
        public double N { get; private set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing the calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Gets or sets the assessment section categories output.
        /// </summary>
        public CalculationOutput<AssessmentSectionCategory[]> AssessmentSectionCategoriesOutput { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism section categories output.
        /// </summary>
        public CalculationOutput<FailureMechanismSectionCategory[]> FailureMechanismSectionCategoriesOutput { get; set; }

        public CalculationOutput<AssessmentSectionCategory[]> CalculateAssessmentSectionCategories(CalculateAssessmentSectionCategoriesInput input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            SignalingNorm = input.SignalingStandard;
            LowerLimitNorm = input.LowerBoundaryStandard;

            Calculated = true;

            return AssessmentSectionCategoriesOutput;
        }

        public CalculationOutput<FailureMechanismCategory[]> CalculateFailureMechanismCategories(CalculateFailureMechanismCategoriesInput input)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategory[]> CalculateFailureMechanismSectionCategories(CalculateFailureMechanismSectionCategoriesInput input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            SignalingNorm = input.SignalingStandard;
            LowerLimitNorm = input.LowerBoundaryStandard;
            ProbabilityDistributionFactor = input.ProbabilityDistributionFactor;
            N = input.NValue;

            Calculated = true;

            return FailureMechanismSectionCategoriesOutput;
        }

        public CalculationOutput<FailureMechanismSectionCategory[]> CalculateGeotechnicFailureMechanismSectionCategories(CalculateFailureMechanismSectionCategoriesInput input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            SignalingNorm = input.SignalingStandard;
            LowerLimitNorm = input.LowerBoundaryStandard;
            ProbabilityDistributionFactor = input.ProbabilityDistributionFactor;
            N = input.NValue;

            Calculated = true;

            return FailureMechanismSectionCategoriesOutput;
        }
    }
}