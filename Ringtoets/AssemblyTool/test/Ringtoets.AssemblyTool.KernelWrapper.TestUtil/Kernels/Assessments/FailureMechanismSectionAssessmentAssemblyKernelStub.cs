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

using AssemblyTool.Kernel;
using AssemblyTool.Kernel.Assembly;
using AssemblyTool.Kernel.Assembly.CalculatorInput;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using AssemblyTool.Kernel.Data.CalculationResults;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assessments
{
    /// <summary>
    /// Failure mechanism section assessment assembly kernel stub for testing purposes.
    /// </summary>
    public class FailureMechanismSectionAssessmentAssemblyKernelStub : IFailureMechanismSectionAssemblyCalculator
    {
        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> SimpleAssessmentDirectFailureMechanisms(SimpleCalculationResult result)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> SimpleAssessmentIndirectFailureMechanisms(SimpleCalculationResult result)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> SimpleAssessmentDirectFailureMechanisms(SimpleCalculationResultValidityOnly result)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> DetailedAssessmentDirectFailureMechanisms(DetailedCalculationResult result)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> DetailedAssessmentIndirectFailureMechanisms(DetailedCalculationResult result)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> DetailedAssessmentDirectFailureMechanisms(DetailedCalculationInputFromProbability input)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> DetailedAssessmentDirectFailureMechanisms(DetailedCategoryBoundariesCalculationResult calculationResults)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> DetailedAssessmentDirectFailureMechanisms(DetailedCalculationInputFromProbabilityWithLengthEffect input)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> TailorMadeAssessmentDirectFailureMechanisms(TailorMadeCalculationResult result)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> TailorMadeAssessmentIndirectFailureMechanisms(TailorMadeCalculationResult result)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> TailorMadeAssessmentDirectFailureMechanisms(TailorMadeCalculationInputFromProbability input)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> TailorMadeAssessmentDirectFailureMechanisms(TailorMadeCategoryCalculationResult result)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> TailorMadeAssessmentDirectFailureMechanisms(TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor input)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionCategoryGroup> CombinedAssessmentFromFailureMechanismSectionResults(FailureMechanismSectionCategoryGroup resultSimpleAssessment, FailureMechanismSectionCategoryGroup resultDetailedAssessment, FailureMechanismSectionCategoryGroup resultTailorMadeAssessment)
        {
            throw new System.NotImplementedException();
        }

        public CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> CombinedAssessmentFromFailureMechanismSectionResults(FailureMechanismSectionAssemblyCategoryResult resultSimpleAssessment, FailureMechanismSectionAssemblyCategoryResult resultDetailedAssessment, FailureMechanismSectionAssemblyCategoryResult resultTailorMadeAssessment)
        {
            throw new System.NotImplementedException();
        }
    }
}
