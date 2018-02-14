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
using AssemblyTool.Kernel.Assembly;
using AssemblyTool.Kernel.Assembly.CalculatorInput;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using AssemblyTool.Kernel.Data.CalculationResults;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Failure mechanism section assembly kernel stub for testing purposes.
    /// </summary>
    public class FailureMechanismSectionAssemblyKernelStub : IFailureMechanismSectionAssemblyCalculator
    {
        /// <summary>
        /// Gets the input used in <see cref="SimpleAssessmentDirectFailureMechanisms(SimpleCalculationResult)"/>.
        /// </summary>
        public SimpleCalculationResult? SimpleAssessmentFailureMechanismsInput { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="SimpleAssessmentDirectFailureMechanisms(SimpleCalculationResultValidityOnly)"/>.
        /// </summary>
        public SimpleCalculationResultValidityOnly? SimpleAssessmentFailureMechanismsValidityOnlyInput { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="DetailedAssessmentDirectFailureMechanisms(DetailedCalculationInputFromProbability)"/>.
        /// </summary>
        public DetailedCalculationInputFromProbability DetailedAssessmentFailureMechanismFromProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="DetailedAssessmentDirectFailureMechanisms(DetailedCalculationInputFromProbabilityWithLengthEffect)"/>.
        /// </summary>
        public DetailedCalculationInputFromProbability DetailedAssessmentFailureMechanismFromProbabilityWithLengthEffectInput { get; private set; }

        /// <summary>
        /// Gets a value indicating whether a calculation was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism section assembly category result.
        /// </summary>
        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> FailureMechanismSectionAssemblyCategoryResult { get; set; }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> SimpleAssessmentDirectFailureMechanisms(SimpleCalculationResult result)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            SimpleAssessmentFailureMechanismsInput = result;
            Calculated = true;
            return FailureMechanismSectionAssemblyCategoryResult;
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> SimpleAssessmentIndirectFailureMechanisms(SimpleCalculationResult result)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> SimpleAssessmentDirectFailureMechanisms(SimpleCalculationResultValidityOnly result)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            SimpleAssessmentFailureMechanismsValidityOnlyInput = result;
            Calculated = true;
            return FailureMechanismSectionAssemblyCategoryResult;
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> DetailedAssessmentDirectFailureMechanisms(DetailedCalculationResult result)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> DetailedAssessmentIndirectFailureMechanisms(DetailedCalculationResult result)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> DetailedAssessmentDirectFailureMechanisms(DetailedCalculationInputFromProbability input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            DetailedAssessmentFailureMechanismFromProbabilityInput = input;
            Calculated = true;
            return FailureMechanismSectionAssemblyCategoryResult;

        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> DetailedAssessmentDirectFailureMechanisms(DetailedCategoryBoundariesCalculationResult calculationResults)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> DetailedAssessmentDirectFailureMechanisms(DetailedCalculationInputFromProbabilityWithLengthEffect input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            DetailedAssessmentFailureMechanismFromProbabilityWithLengthEffectInput = input;
            Calculated = true;
            return FailureMechanismSectionAssemblyCategoryResult;
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> TailorMadeAssessmentDirectFailureMechanisms(TailorMadeCalculationResult result)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> TailorMadeAssessmentIndirectFailureMechanisms(TailorMadeCalculationResult result)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> TailorMadeAssessmentDirectFailureMechanisms(TailorMadeCalculationInputFromProbability input)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> TailorMadeAssessmentDirectFailureMechanisms(TailorMadeCategoryCalculationResult result)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> TailorMadeAssessmentDirectFailureMechanisms(TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor input)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> CombinedAssessmentFromFailureMechanismSectionResults(FailureMechanismSectionCategoryGroup resultSimpleAssessment, FailureMechanismSectionCategoryGroup resultDetailedAssessment, FailureMechanismSectionCategoryGroup resultTailorMadeAssessment)
        {
            throw new NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> CombinedAssessmentFromFailureMechanismSectionResults(FailureMechanismSectionAssemblyCategoryResult resultSimpleAssessment, FailureMechanismSectionAssemblyCategoryResult resultDetailedAssessment, FailureMechanismSectionAssemblyCategoryResult resultTailorMadeAssessment)
        {
            throw new NotImplementedException();
        }
    }
}