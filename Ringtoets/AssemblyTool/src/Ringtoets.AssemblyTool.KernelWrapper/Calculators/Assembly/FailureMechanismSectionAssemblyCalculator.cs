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
using System.ComponentModel;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.FmSectionTypes;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Creators;
using Ringtoets.AssemblyTool.KernelWrapper.Kernels;
using Ringtoets.Common.Primitives;
using IFailureMechanismSectionAssemblyCalculatorKernel = Assembly.Kernel.Interfaces.IAssessmentResultsTranslator;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Class representing a failure mechanism section assembly calculator.
    /// </summary>
    public class FailureMechanismSectionAssemblyCalculator : IFailureMechanismSectionAssemblyCalculator
    {
        private readonly IAssemblyToolKernelFactory factory;

        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyCalculator"/>.
        /// </summary>
        /// <param name="factory">The factory responsible for creating the assembly kernel.</param>
        /// <exception cref="ArgumentNullException">Thrown when any parameter is <c>null</c>.</exception>
        public FailureMechanismSectionAssemblyCalculator(IAssemblyToolKernelFactory factory)
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
                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0E1(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultE1(input));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType input)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0E3(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeE2(input));

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
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

                return FailureMechanismSectionAssemblyCreator.ConvertFailureMechanismSectionCategory(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          AssessmentSection assessmentSection,
                                                                          FailureMechanism failureMechanism)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0G3(
                    assessmentSection,
                    failureMechanism,
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(detailedAssessmentResult),
                    probability);

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          double n,
                                                                          AssessmentSection assessmentSection,
                                                                          FailureMechanism failureMechanism)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResult output = kernel.TranslateAssessmentResultWbi0G5(
                    assessmentSection,
                    failureMechanism,
                    n,
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateAssessmentResultTypeG2(detailedAssessmentResult),
                    probability);

                return FailureMechanismSectionAssemblyCreator.Create(output);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
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
                CalculationOutput<FailureMechanismSectionCategoryGroup> output = kernel.DetailedAssessmentDirectFailureMechanisms(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateDetailedCalculationInputFromCategoryResults(
                        detailedAssessmentResultForFactorizedSignalingNorm,
                        detailedAssessmentResultForSignalingNorm,
                        detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                        detailedAssessmentResultForLowerLimitNorm,
                        detailedAssessmentResultForFactorizedLowerLimitNorm));

                return FailureMechanismSectionAssemblyCreator.ConvertFailureMechanismSectionCategory(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        #endregion

        #region Tailor Made Assessment

        public FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(TailorMadeAssessmentResultType tailorMadeAssessmentResult)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                FmSectionAssemblyDirectResult output = kernel.TailorMadeAssessmentDirectFailureMechanisms(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationResult(tailorMadeAssessmentResult));

                return FailureMechanismSectionAssemblyCreator.ConvertFailureMechanismSectionCategory(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            return FailureMechanismSectionAssemblyCreator.Create(new FailureMechanismSectionAssemblyCategoryResult(
                                                                     FailureMechanismSectionCategoryGroup.VIIv,
                                                                     new Probability(probability)));
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> output = kernel.TailorMadeAssessmentDirectFailureMechanisms(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbability(tailorMadeAssessmentResult,
                                                                                                                          probability,
                                                                                                                          categories));

                return FailureMechanismSectionAssemblyCreator.Create(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
                                                                            double n)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> output = kernel.TailorMadeAssessmentDirectFailureMechanisms(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                        tailorMadeAssessmentResult,
                        probability,
                        categories,
                        n));

                return FailureMechanismSectionAssemblyCreator.Create(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssessmentResult)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                CalculationOutput<FailureMechanismSectionCategoryGroup> output = kernel.TailorMadeAssessmentDirectFailureMechanisms(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup(
                        tailorMadeAssessmentResult));
                return FailureMechanismSectionAssemblyCreator.ConvertFailureMechanismSectionCategory(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        #endregion

        #region Combined Assembly

        public FailureMechanismSectionAssembly AssembleCombined(FailureMechanismSectionAssembly simpleAssembly,
                                                                FailureMechanismSectionAssembly detailedAssembly,
                                                                FailureMechanismSectionAssembly tailorMadeAssembly)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                CalculationOutput<FailureMechanismSectionAssemblyCategoryResult> output = kernel.CombinedAssessmentFromFailureMechanismSectionResults(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyCategoryResult(simpleAssembly),
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyCategoryResult(detailedAssembly),
                    FailureMechanismSectionAssemblyCalculatorInputCreator.CreateFailureMechanismSectionAssemblyCategoryResult(tailorMadeAssembly));

                return FailureMechanismSectionAssemblyCreator.Create(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        public FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FailureMechanismSectionAssemblyCategoryGroup simpleAssembly,
                                                                             FailureMechanismSectionAssemblyCategoryGroup detailedAssembly,
                                                                             FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly)
        {
            try
            {
                IFailureMechanismSectionAssemblyCalculatorKernel kernel = factory.CreateFailureMechanismSectionAssemblyKernel();
                CalculationOutput<FailureMechanismSectionCategoryGroup> output = kernel.CombinedAssessmentFromFailureMechanismSectionResults(
                    FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionAssemblyCategoryGroup(simpleAssembly),
                    FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionAssemblyCategoryGroup(detailedAssembly),
                    FailureMechanismSectionAssemblyCalculatorInputCreator.ConvertFailureMechanismSectionAssemblyCategoryGroup(tailorMadeAssembly));

                return FailureMechanismSectionAssemblyCreator.ConvertFailureMechanismSectionCategory(output.Result);
            }
            catch (Exception e)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException(e.Message, e);
            }
        }

        #endregion
    }
}