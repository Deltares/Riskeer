﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using Assembly.Kernel.Old.Exceptions;
using Assembly.Kernel.Old.Interfaces;
using Assembly.Kernel.Old.Model;
using Assembly.Kernel.Old.Model.AssessmentResultTypes;
using Assembly.Kernel.Old.Model.CategoryLimits;
using Assembly.Kernel.Old.Model.FmSectionTypes;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators.Exceptions;
using Riskeer.AssemblyTool.KernelWrapper.Creators;
using Riskeer.AssemblyTool.KernelWrapper.Kernels;
using Riskeer.Common.Primitives;
using IFailureMechanismSectionAssemblyCalculatorKernel = Assembly.Kernel.Old.Interfaces.IAssessmentResultsTranslator;

namespace Riskeer.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Class representing a failure mechanism section assembly calculator.
    /// </summary>
    public class FailureMechanismSectionAssemblyCalculator : IFailureMechanismSectionAssemblyCalculator
    {
        private readonly IAssemblyToolKernelFactoryOld factory;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactoryOld factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }

            this.factory = factory;
        }

        #region Simple Assessment

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultType input)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output = kernel.TranslateAssessmentResultWbi0E1(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE1(input));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType input)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output = kernel.TranslateAssessmentResultWbi0E3(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE2(input));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        #endregion

        #region Detailed Assessment

        public FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(DetailedAssessmentResultType detailedAssessmentResult)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0G1(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG1(detailedAssessmentResult));

                return FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(output.Result);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          AssemblyCategoriesInput assemblyCategoriesInput)
        {
            return GetDetailedAssembly(detailedAssessmentResult, probability, assemblyCategoriesInput);
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          double failureMechanismSectionN,
                                                                          AssemblyCategoriesInput assemblyCategoriesInput)
        {
            return GetDetailedAssembly(detailedAssessmentResult, probability, failureMechanismSectionN, assemblyCategoriesInput);
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          double normativeNorm,
                                                                          double failureMechanismN,
                                                                          double failureMechanismContribution)
        {
            try
            {
                ICategoryLimitsCalculator categoriesKernel = factory.CreateAssemblyCategoriesKernel();
                CategoriesList<FmSectionCategory> categories = categoriesKernel.CalculateFmSectionCategoryLimitsWbi02(
                    normativeNorm,
                    new FailureMechanism(failureMechanismN, failureMechanismContribution));

                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output =
                    kernel.TranslateAssessmentResultWbi0G3(GetAssessmentResultTypeG2(detailedAssessmentResult, probability),
                                                           probability,
                                                           categories);

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedLowerLimitNorm)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0G6(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateCategoryCompliancyResults(
                        detailedAssessmentResultForFactorizedSignalingNorm,
                        detailedAssessmentResultForSignalingNorm,
                        detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                        detailedAssessmentResultForLowerLimitNorm,
                        detailedAssessmentResultForFactorizedLowerLimitNorm));

                return FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(output.Result);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        private FailureMechanismSectionAssembly GetDetailedAssembly(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                    double probability,
                                                                    AssemblyCategoriesInput assemblyCategoriesInput)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output =
                    kernel.TranslateAssessmentResultWbi0G3(GetAssessmentResultTypeG2(detailedAssessmentResult, probability),
                                                           probability,
                                                           GetWbi01Categories(assemblyCategoriesInput));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        private FailureMechanismSectionAssembly GetDetailedAssembly(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                    double probability,
                                                                    double failureMechanismSectionN,
                                                                    AssemblyCategoriesInput assemblyCategoriesInput)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output = kernel.TranslateAssessmentResultWbi0G5(failureMechanismSectionN,
                                                                                                             GetAssessmentResultTypeG2(detailedAssessmentResult, probability),
                                                                                                             probability,
                                                                                                             GetWbi01Categories(assemblyCategoriesInput));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        private static EAssessmentResultTypeG2 GetAssessmentResultTypeG2(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult, double probability)
        {
            return double.IsNaN(probability) && detailedAssessmentResult == DetailedAssessmentProbabilityOnlyResultType.Probability
                       ? EAssessmentResultTypeG2.Gr
                       : FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(detailedAssessmentResult);
        }

        #endregion

        #region Tailor Made Assessment

        public FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(TailorMadeAssessmentResultType tailorMadeAssessmentResult)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0T1(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT1(tailorMadeAssessmentResult));

                return FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(output.Result);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            double normativeNorm,
                                                                            double failureMechanismN,
                                                                            double failureMechanismContribution)
        {
            try
            {
                ICategoryLimitsCalculator categoriesKernel = factory.CreateAssemblyCategoriesKernel();
                CategoriesList<FmSectionCategory> categories = categoriesKernel.CalculateFmSectionCategoryLimitsWbi02(
                    normativeNorm,
                    new FailureMechanism(failureMechanismN, failureMechanismContribution));

                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0T7(
                    double.IsNaN(probability) && tailorMadeAssessmentResult == TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability
                        ? EAssessmentResultTypeT4.Gr
                        : FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT4(tailorMadeAssessmentResult),
                    probability,
                    categories);

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            AssemblyCategoriesInput assemblyCategoriesInput)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output = kernel.TranslateAssessmentResultWbi0T3(
                    double.IsNaN(probability) && tailorMadeAssessmentResult == TailorMadeAssessmentProbabilityCalculationResultType.Probability
                        ? EAssessmentResultTypeT3.Gr
                        : FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3(tailorMadeAssessmentResult),
                    probability,
                    GetWbi01Categories(assemblyCategoriesInput));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            double failureMechanismSectionN,
                                                                            AssemblyCategoriesInput assemblyCategoriesInput)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output = kernel.TranslateAssessmentResultWbi0T5(
                    failureMechanismSectionN,
                    double.IsNaN(probability) && tailorMadeAssessmentResult == TailorMadeAssessmentProbabilityCalculationResultType.Probability
                        ? EAssessmentResultTypeT3.Gr
                        : FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3(tailorMadeAssessmentResult),
                    probability,
                    GetWbi01Categories(assemblyCategoriesInput));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(
            TailorMadeAssessmentCategoryGroupResultType tailorMadeAssessmentResult)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                Tuple<EAssessmentResultTypeT3, EFmSectionCategory?> input =
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeT3WithCategoryGroupResult(
                        tailorMadeAssessmentResult);

                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0T4(input.Item1, input.Item2);

                return FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(output.Result);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        private CategoriesList<FmSectionCategory> GetWbi01Categories(AssemblyCategoriesInput assemblyCategoriesInput)
        {
            ICategoryLimitsCalculator categoriesKernel = factory.CreateAssemblyCategoriesKernel();
            return categoriesKernel.CalculateFmSectionCategoryLimitsWbi01(
                new AssessmentSection(1, assemblyCategoriesInput.SignalingNorm, assemblyCategoriesInput.LowerLimitNorm),
                new FailureMechanism(assemblyCategoriesInput.N, assemblyCategoriesInput.FailureMechanismContribution));
        }

        #endregion

        #region Combined Assembly

        public FailureMechanismSectionAssembly AssembleCombined(FailureMechanismSectionAssembly simpleAssembly)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output = kernel.TranslateAssessmentResultWbi0A1(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(simpleAssembly),
                    null,
                    null);

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssembly AssembleCombined(FailureMechanismSectionAssembly simpleAssembly,
                                                                FailureMechanismSectionAssembly detailedAssembly,
                                                                FailureMechanismSectionAssembly tailorMadeAssembly)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResultWithProbability output = kernel.TranslateAssessmentResultWbi0A1(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(simpleAssembly),
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(detailedAssembly),
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(tailorMadeAssembly));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FailureMechanismSectionAssemblyCategoryGroup simpleAssembly)
        {
            try
            {
                return AssembleCombined(FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(simpleAssembly),
                                        null,
                                        null);
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FailureMechanismSectionAssemblyCategoryGroup simpleAssembly,
                                                                             FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly)
        {
            try
            {
                return AssembleCombined(FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(simpleAssembly),
                                        null,
                                        FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(tailorMadeAssembly));
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FailureMechanismSectionAssemblyCategoryGroup simpleAssembly,
                                                                             FailureMechanismSectionAssemblyCategoryGroup detailedAssembly,
                                                                             FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly)
        {
            try
            {
                return AssembleCombined(FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(simpleAssembly),
                                        FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(detailedAssembly),
                                        FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyDirectResult(tailorMadeAssembly));
            }
            catch (AssemblyException e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateErrorMessage(e.Errors), e);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(AssemblyErrorMessageCreator.CreateGenericErrorMessage(), e);
            }
        }

        /// <summary>
        /// Assembles the combined assembly based on the input parameters.
        /// </summary>
        /// <param name="simpleAssemblyResult">The simple assembly result.</param>
        /// <param name="detailedAssemblyResult">The simple assembly result.</param>
        /// <param name="tailorMadeAssemblyResult">The tailor made assembly result.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="AssemblyException">Thrown when an error occurs while assembling.</exception>
        private FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FmSectionAssemblyDirectResult simpleAssemblyResult,
                                                                              FmSectionAssemblyDirectResult detailedAssemblyResult,
                                                                              FmSectionAssemblyDirectResult tailorMadeAssemblyResult)
        {
            IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
            FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0A1(
                simpleAssemblyResult,
                detailedAssemblyResult,
                tailorMadeAssemblyResult);

            return FailureMechanismSectionAssemblyCreator.CreateFailureMechanismSectionAssemblyCategoryGroup(output.Result);
        }

        #endregion

        #region Manual Assembly

        public FailureMechanismSectionAssembly AssembleManual(double probability,
                                                              AssemblyCategoriesInput assemblyCategoriesInput)
        {
            return GetDetailedAssembly(DetailedAssessmentProbabilityOnlyResultType.Probability, probability, assemblyCategoriesInput);
        }

        public FailureMechanismSectionAssembly AssembleManual(double probability,
                                                              double failureMechanismSectionN,
                                                              AssemblyCategoriesInput assemblyCategoriesInput)
        {
            return GetDetailedAssembly(DetailedAssessmentProbabilityOnlyResultType.Probability, probability, failureMechanismSectionN, assemblyCategoriesInput);
        }

        #endregion
    }
}