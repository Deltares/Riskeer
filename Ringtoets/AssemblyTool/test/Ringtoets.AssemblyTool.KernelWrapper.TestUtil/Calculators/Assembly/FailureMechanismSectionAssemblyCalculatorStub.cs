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
        public DetailedAssessmentProbabilityOnlyResultType DetailedAssessmentProbabilityOnlyResultInput { get; private set; }

        /// <summary>
        /// Gets the probability input of the detailed assessment calculation.
        /// </summary>
        public double DetailedAssessmentProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the categories input of the detailed assessment calculation.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategory> DetailedAssessmentCategoriesInput { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter input of the detailed assessment calculation.
        /// </summary>
        public double DetailedAssessmentNInput { get; private set; }
        
        /// <summary>
        /// Gets the detailed assessment result input for cat Iv - IIv.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssesmentResultForFactorizedSignalingNormInput { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result input for cat IIv - IIIv.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssesmentResultForSignalingNormInput { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result input for cat IIIv - IVv.
        /// </summary>public DetailedAssessmentResultType DetailedAssesmentResultForMechanismSpecificLowerLimitNormInput { get; private set; }
        public DetailedAssessmentResultType DetailedAssesmentResultForMechanismSpecificLowerLimitNormInput { get; private set; }


        /// <summary>
        /// Gets the detailed assessment result input for cat IVv - Vv.
        /// </summary>public DetailedAssessmentResultType DetailedAssesmentResultForMechanismSpecificLowerLimitNormInput { get; private set; }
        public DetailedAssessmentResultType DetailedAssesmentResultForLowerLimitNormInput { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result input for cat Vv - VIv.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssesmentResultForFactorizedLowerLimitNormInput { get; private set; }
        
        /// <summary>
        /// Gets or sets the output of the tailor made assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssembly TailorMadeAssessmentAssemblyOutput { get; set; }

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
        /// Gets the categories input of the tailor made assessment calculation.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategory> TailorMadeAssessmentCategoriesInput { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter input of the tailor made assessment calculation.
        /// </summary>
        public double TailorMadeAssessmentNInput { get; private set; }

        /// <summary>
        /// Gets the category group input of the tailor made assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssemblyCategoryGroup TailorMadeAssessmentResultInput { get; private set; }

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
        /// Sets an indicator whether an exception must be thrown when performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown when performing a combined assembly calculation.
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

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentProbabilityOnlyResultInput = detailedAssessmentResult;
            DetailedAssessmentProbabilityInput = probability;
            DetailedAssessmentCategoriesInput = categories;

            return DetailedAssessmentAssemblyOutput ??
                   (DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
                                                                          double n)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentProbabilityOnlyResultInput = detailedAssessmentResult;
            DetailedAssessmentProbabilityInput = probability;
            DetailedAssessmentCategoriesInput = categories;
            DetailedAssessmentNInput = n;

            return DetailedAssessmentAssemblyOutput ??
                   (DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(
            DetailedAssessmentResultType detailedAssesmentResultForFactorizedSignalingNorm,
            DetailedAssessmentResultType detailedAssesmentResultForSignalingNorm,
            DetailedAssessmentResultType detailedAssesmentResultForMechanismSpecificLowerLimitNorm,
            DetailedAssessmentResultType detailedAssesmentResultForLowerLimitNorm,
            DetailedAssessmentResultType detailedAssesmentResultForFactorizedLowerLimitNorm)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssesmentResultForFactorizedSignalingNormInput = detailedAssesmentResultForFactorizedSignalingNorm;
            DetailedAssesmentResultForSignalingNormInput = detailedAssesmentResultForSignalingNorm;
            DetailedAssesmentResultForMechanismSpecificLowerLimitNormInput = detailedAssesmentResultForMechanismSpecificLowerLimitNorm;
            DetailedAssesmentResultForLowerLimitNormInput = detailedAssesmentResultForLowerLimitNorm;
            DetailedAssesmentResultForFactorizedLowerLimitNormInput = detailedAssesmentResultForFactorizedLowerLimitNorm;

            if (DetailedAssessmentAssemblyGroupOutput == null)
            {
                DetailedAssessmentAssemblyGroupOutput = FailureMechanismSectionAssemblyCategoryGroup.IIv;
            }

            return DetailedAssessmentAssemblyGroupOutput.Value;
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentProbabilityAndDetailedCalculationResultInput = tailorMadeAssessmentResult;
            TailorMadeAssessmentProbabilityInput = probability;
            TailorMadeAssessmentCategoriesInput = categories;

            return TailorMadeAssessmentAssemblyOutput ??
                   (TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentProbabilityCalculationResultInput = tailorMadeAssessmentResult;
            TailorMadeAssessmentProbabilityInput = probability;
            TailorMadeAssessmentCategoriesInput = categories;

            return TailorMadeAssessmentAssemblyOutput ??
                   (TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability, 
                                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> categories, 
                                                                            double n)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentProbabilityCalculationResultInput = tailorMadeAssessmentResult;
            TailorMadeAssessmentProbabilityInput = probability;
            TailorMadeAssessmentCategoriesInput = categories;
            TailorMadeAssessmentNInput = n;

            return TailorMadeAssessmentAssemblyOutput ??
                   (TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv));
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssessmentResult)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentResultInput = tailorMadeAssessmentResult;

            if (TailorMadeAssemblyCategoryOutput == null)
            {
                TailorMadeAssemblyCategoryOutput = FailureMechanismSectionAssemblyCategoryGroup.Iv;
            }

            return TailorMadeAssemblyCategoryOutput.Value;
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
    }
}