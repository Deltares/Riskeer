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
using Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Assembly
{
    /// <summary>
    /// Failure mechanism section assembly calculator stub for testing purposes.
    /// </summary>
    public class FailureMechanismSectionAssemblyCalculatorStub : IFailureMechanismSectionAssemblyCalculator
    {
        /// <summary>
        /// Gets or sets the output of the simple assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssembly SimpleAssessmentAssemblyOutput { get; set; }

        /// <summary>
        /// Gets the input of the simple assessment calculation.
        /// </summary>
        public SimpleAssessmentResultType SimpleAssessmentInput { get; private set; }

        /// <summary>
        /// Gets the input of the simple assessment validity only calculation.
        /// </summary>
        public SimpleAssessmentValidityOnlyResultType SimpleAssessmentValidityOnlyInput { get; private set; }

        /// <summary>
        /// Gets or sets the output of the detailed assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssembly DetailedAssessmentAssemblyOutput { get; set; }

        /// <summary>
        /// Gets or sets the group output of the detailed assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup? DetailedAssessmentAssemblyGroupOutput { get; set; }

        /// <summary>
        /// Gets the result type of the detailed assessment calculation.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultInput { get; private set; }

        /// <summary>
        /// Gets the result type of the detailed assessment calculation.
        /// </summary>
        public DetailedAssessmentProbabilityOnlyResultType DetailedAssessmentProbabilityOnlyResultInput { get; private set; }

        /// <summary>
        /// Gets the probability input of the detailed assessment calculation.
        /// </summary>
        public double DetailedAssessmentProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the normative norm input of the detailed assessment calculation.
        /// </summary>
        public double DetailedAssessmentNormativeNormInput { get; private set; }

        /// <summary>
        /// Gets the failure mechanism 'N' parameter input of the detailed assessment calculation.
        /// </summary>
        public double DetailedAssessmentFailureMechanismNInput { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter failure mechanism section input of the detailed assessment calculation.
        /// </summary>
        public double DetailedAssessmentFailureMechanismSectionNInput { get; private set; }

        /// <summary>
        /// Gets the failure mechanism contribution input of the detailed assessment calculation.
        /// </summary>
        public double DetailedAssessmentFailureMechanismContribution { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result input for category boundary Iv.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForFactorizedSignalingNormInput { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result input for category boundary IIv.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForSignalingNormInput { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result input for category boundary IIIv.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForMechanismSpecificLowerLimitNormInput { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result input for category boundary IVv.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForLowerLimitNormInput { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result input for category boundary Vv.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultForFactorizedLowerLimitNormInput { get; private set; }

        /// <summary>
        /// Gets or sets the output of the tailor made assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssembly TailorMadeAssessmentAssemblyOutput { get; set; }

        /// <summary>
        /// Gets the result type of the tailor made assessment calculation.
        /// </summary>
        public TailorMadeAssessmentResultType TailorMadeAssessmentResultInput { get; private set; }

        /// <summary>
        /// Gets the result type of the tailor made assessment calculation with probability or detailed calculation result.
        /// </summary>
        public TailorMadeAssessmentProbabilityAndDetailedCalculationResultType TailorMadeAssessmentProbabilityAndDetailedCalculationResultInput { get; private set; }

        /// <summary>
        /// Gets the result type of the tailor made assessment calculation with a probability calculation.
        /// </summary>
        public TailorMadeAssessmentProbabilityCalculationResultType TailorMadeAssessmentProbabilityCalculationResultInput { get; private set; }

        /// <summary>
        /// Gets the probability input of the tailor made assessment calculation.
        /// </summary>
        public double TailorMadeAssessmentProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter of a failure mechanism section input of the tailor made assessment calculation.
        /// </summary>
        public double TailorMadeAssessmentFailureMechanismSectionNInput { get; private set; }

        /// <summary>
        /// Gets the normative norm input of the tailor made assessment calculation.
        /// </summary>
        public double TailorMadeAssessmentNormativeNormInput { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter of a failure mechanism input of the tailor made assessment calculation.
        /// </summary>
        public double TailorMadeAssessmentFailureMechanismNInput { get; private set; }

        /// <summary>
        /// Gets the failure mechanism contribution input of the tailor made assessment calculation.
        /// </summary>
        public double TailorMadeAssessmentFailureMechanismContributionInput { get; private set; }

        /// <summary>
        /// Gets the result type of the tailor made assessment calculation with a category group.
        /// </summary>
        public TailorMadeAssessmentCategoryGroupResultType TailorMadeAssessmentCategoryGroupResultInput { get; private set; }

        /// <summary>
        /// Gets or sets the output of the tailor made assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup? TailorMadeAssemblyCategoryOutput { get; set; }

        /// <summary>
        /// Gets or sets the output of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssembly CombinedAssemblyOutput { get; set; }

        /// <summary>
        /// Gets the simple assembly input of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssembly CombinedSimpleAssemblyInput { get; private set; }

        /// <summary>
        /// Gets the detailed assembly input of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssembly CombinedDetailedAssemblyInput { get; private set; }

        /// <summary>
        /// Gets the tailor made assembly input of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssembly CombinedTailorMadeAssemblyInput { get; private set; }

        /// <summary>
        /// Gets or sets the output of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup? CombinedAssemblyCategoryOutput { get; set; }

        /// <summary>
        /// Gets the simple assembly input of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup CombinedSimpleAssemblyGroupInput { get; private set; }

        /// <summary>
        /// Gets the detailed assembly input of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup CombinedDetailedAssemblyGroupInput { get; private set; }

        /// <summary>
        /// Gets the tailor made assembly input of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup CombinedTailorMadeAssemblyGroupInput { get; private set; }

        /// <summary>
        /// Gets or sets the output of the manual assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssembly ManualAssemblyAssemblyOutput { get; set; }

        /// <summary>
        /// Gets the probability input of the manual assembly calculation.
        /// </summary>
        public double ManualAssemblyProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter input of the manual assembly calculation.
        /// </summary>
        public double ManualAssemblyNInput { get; private set; }

        /// <summary>
        /// Gets the assembly categories input of the manual assembly calculation.
        /// </summary>
        public AssemblyCategoriesInput ManualAssemblyCategoriesInput { get; private set; }

        /// <summary>
        /// Gets the assembly categories input used in the assembly calculation methods.
        /// </summary>
        public AssemblyCategoriesInput AssemblyCategoriesInput { get; private set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown while performing a combined assembly calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculateCombinedAssembly { private get; set; }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultType input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            SimpleAssessmentInput = input;

            return SimpleAssessmentAssemblyOutput ??
                   (SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.Iv));
        }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            SimpleAssessmentValidityOnlyInput = input;

            return SimpleAssessmentAssemblyOutput ??
                   (SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIIv));
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(DetailedAssessmentResultType detailedAssessmentResult)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentResultInput = detailedAssessmentResult;

            if (DetailedAssessmentAssemblyGroupOutput == null)
            {
                DetailedAssessmentAssemblyGroupOutput = FailureMechanismSectionAssemblyCategoryGroup.IIv;
            }

            return DetailedAssessmentAssemblyGroupOutput.Value;
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentProbabilityOnlyResultInput = detailedAssessmentResult;
            DetailedAssessmentProbabilityInput = probability;
            AssemblyCategoriesInput = assemblyCategoriesInput;

            return DetailedAssessmentAssemblyOutput ??
                   (DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          double failureMechanismSectionN,
                                                                          AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentProbabilityOnlyResultInput = detailedAssessmentResult;
            DetailedAssessmentProbabilityInput = probability;
            AssemblyCategoriesInput = assemblyCategoriesInput;
            DetailedAssessmentFailureMechanismSectionNInput = failureMechanismSectionN;

            return DetailedAssessmentAssemblyOutput ??
                   (DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          double normativeNorm,
                                                                          double failureMechanismN,
                                                                          double failureMechanismContribution)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentProbabilityOnlyResultInput = detailedAssessmentResult;
            DetailedAssessmentProbabilityInput = probability;
            DetailedAssessmentNormativeNormInput = normativeNorm;
            DetailedAssessmentFailureMechanismNInput = failureMechanismN;
            DetailedAssessmentFailureMechanismContribution = failureMechanismContribution;

            return DetailedAssessmentAssemblyOutput ??
                   (DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(0.25, FailureMechanismSectionAssemblyCategoryGroup.IVv));
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedLowerLimitNorm)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentResultForFactorizedSignalingNormInput = detailedAssessmentResultForFactorizedSignalingNorm;
            DetailedAssessmentResultForSignalingNormInput = detailedAssessmentResultForSignalingNorm;
            DetailedAssessmentResultForMechanismSpecificLowerLimitNormInput = detailedAssessmentResultForMechanismSpecificLowerLimitNorm;
            DetailedAssessmentResultForLowerLimitNormInput = detailedAssessmentResultForLowerLimitNorm;
            DetailedAssessmentResultForFactorizedLowerLimitNormInput = detailedAssessmentResultForFactorizedLowerLimitNorm;

            if (DetailedAssessmentAssemblyGroupOutput == null)
            {
                DetailedAssessmentAssemblyGroupOutput = FailureMechanismSectionAssemblyCategoryGroup.IIv;
            }

            return DetailedAssessmentAssemblyGroupOutput.Value;
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(TailorMadeAssessmentResultType tailorMadeAssessmentResult)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentResultInput = tailorMadeAssessmentResult;

            if (TailorMadeAssemblyCategoryOutput == null)
            {
                TailorMadeAssemblyCategoryOutput = FailureMechanismSectionAssemblyCategoryGroup.IIv;
            }

            return TailorMadeAssemblyCategoryOutput.Value;
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            double normativeNorm,
                                                                            double failureMechanismN,
                                                                            double failureMechanismContribution)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentProbabilityAndDetailedCalculationResultInput = tailorMadeAssessmentResult;
            TailorMadeAssessmentProbabilityInput = probability;
            TailorMadeAssessmentNormativeNormInput = normativeNorm;
            TailorMadeAssessmentFailureMechanismNInput = failureMechanismN;
            TailorMadeAssessmentFailureMechanismContributionInput = failureMechanismContribution;

            return TailorMadeAssessmentAssemblyOutput ??
                   (TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentProbabilityCalculationResultInput = tailorMadeAssessmentResult;
            TailorMadeAssessmentProbabilityInput = probability;
            AssemblyCategoriesInput = assemblyCategoriesInput;

            return TailorMadeAssessmentAssemblyOutput ??
                   (TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            double failureMechanismSectionN,
                                                                            AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentProbabilityCalculationResultInput = tailorMadeAssessmentResult;
            TailorMadeAssessmentProbabilityInput = probability;
            AssemblyCategoriesInput = assemblyCategoriesInput;
            TailorMadeAssessmentFailureMechanismSectionNInput = failureMechanismSectionN;

            return TailorMadeAssessmentAssemblyOutput ??
                   (TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(TailorMadeAssessmentCategoryGroupResultType tailorMadeAssessmentResult)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentCategoryGroupResultInput = tailorMadeAssessmentResult;

            if (TailorMadeAssemblyCategoryOutput == null)
            {
                TailorMadeAssemblyCategoryOutput = FailureMechanismSectionAssemblyCategoryGroup.Iv;
            }

            return TailorMadeAssemblyCategoryOutput.Value;
        }

        public FailureMechanismSectionAssembly AssembleCombined(FailureMechanismSectionAssembly simpleAssembly)
        {
            if (ThrowExceptionOnCalculateCombinedAssembly)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            CombinedSimpleAssemblyInput = simpleAssembly;

            return CombinedAssemblyOutput ?? (CombinedAssemblyOutput = simpleAssembly);
        }

        public FailureMechanismSectionAssembly AssembleCombined(FailureMechanismSectionAssembly simpleAssembly,
                                                                FailureMechanismSectionAssembly detailedAssembly,
                                                                FailureMechanismSectionAssembly tailorMadeAssembly)
        {
            if (ThrowExceptionOnCalculateCombinedAssembly)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            CombinedSimpleAssemblyInput = simpleAssembly;
            CombinedDetailedAssemblyInput = detailedAssembly;
            CombinedTailorMadeAssemblyInput = tailorMadeAssembly;

            return CombinedAssemblyOutput ?? (CombinedAssemblyOutput = tailorMadeAssembly);
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FailureMechanismSectionAssemblyCategoryGroup simpleAssembly)
        {
            if (ThrowExceptionOnCalculateCombinedAssembly)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            CombinedSimpleAssemblyGroupInput = simpleAssembly;

            if (CombinedAssemblyCategoryOutput == null)
            {
                CombinedAssemblyCategoryOutput = simpleAssembly;
            }

            return CombinedAssemblyCategoryOutput.Value;
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FailureMechanismSectionAssemblyCategoryGroup simpleAssembly,
                                                                             FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly)
        {
            if (ThrowExceptionOnCalculateCombinedAssembly)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            CombinedSimpleAssemblyGroupInput = simpleAssembly;
            CombinedTailorMadeAssemblyGroupInput = tailorMadeAssembly;

            if (CombinedAssemblyCategoryOutput == null)
            {
                CombinedAssemblyCategoryOutput = tailorMadeAssembly;
            }

            return CombinedAssemblyCategoryOutput.Value;
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FailureMechanismSectionAssemblyCategoryGroup simpleAssembly,
                                                                             FailureMechanismSectionAssemblyCategoryGroup detailedAssembly,
                                                                             FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly)
        {
            if (ThrowExceptionOnCalculateCombinedAssembly)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            CombinedSimpleAssemblyGroupInput = simpleAssembly;
            CombinedDetailedAssemblyGroupInput = detailedAssembly;
            CombinedTailorMadeAssemblyGroupInput = tailorMadeAssembly;

            if (CombinedAssemblyCategoryOutput == null)
            {
                CombinedAssemblyCategoryOutput = tailorMadeAssembly;
            }

            return CombinedAssemblyCategoryOutput.Value;
        }

        public FailureMechanismSectionAssembly AssembleManual(double probability, AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            ManualAssemblyProbabilityInput = probability;
            ManualAssemblyCategoriesInput = assemblyCategoriesInput;

            return ManualAssemblyAssemblyOutput ??
                   (ManualAssemblyAssemblyOutput = new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.IIv));
        }

        public FailureMechanismSectionAssembly AssembleManual(double probability, double failureMechanismSectionN, AssemblyCategoriesInput assemblyCategoriesInput)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            ManualAssemblyProbabilityInput = probability;
            ManualAssemblyCategoriesInput = assemblyCategoriesInput;
            ManualAssemblyNInput = failureMechanismSectionN;

            return ManualAssemblyAssemblyOutput ??
                   (ManualAssemblyAssemblyOutput = new FailureMechanismSectionAssembly(1.0, FailureMechanismSectionAssemblyCategoryGroup.VIIv));
        }
    }
}