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
using System.Linq;
using AssemblyTool.Kernel.Assembly;
using AssemblyTool.Kernel.Assembly.CalculatorInput;
using AssemblyTool.Kernel.Data;
using AssemblyTool.Kernel.Data.AssemblyCategories;
using AssemblyTool.Kernel.Data.CalculationResults;
using AssemblyTool.Kernel.ErrorHandling;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input instances that can be used in <see cref="IFailureMechanismSectionAssemblyCalculator"/>.
    /// </summary>
    public static class FailureMechanismSectionAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates <see cref="SimpleCalculationResult"/> based on the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultType"/> to create the result for.</param>
        /// <returns>The created <see cref="SimpleCalculationResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SimpleAssessmentResultType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SimpleAssessmentResultType"/>
        /// is a valid value, but unsupported.</exception>
        public static SimpleCalculationResult CreateSimpleCalculationResult(SimpleAssessmentResultType input)
        {
            if (!Enum.IsDefined(typeof(SimpleAssessmentResultType), input))
            {
                throw new InvalidEnumArgumentException(nameof(input),
                                                       (int) input,
                                                       typeof(SimpleAssessmentResultType));
            }

            switch (input)
            {
                case SimpleAssessmentResultType.None:
                    return SimpleCalculationResult.None;
                case SimpleAssessmentResultType.NotApplicable:
                    return SimpleCalculationResult.NVT;
                case SimpleAssessmentResultType.ProbabilityNegligible:
                    return SimpleCalculationResult.FV;
                case SimpleAssessmentResultType.AssessFurther:
                    return SimpleCalculationResult.VB;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates <see cref="SimpleCalculationResultValidityOnly"/> based on the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentValidityOnlyResultType"/> to create the result for.</param>
        /// <returns>The created <see cref="SimpleCalculationResultValidityOnly"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SimpleAssessmentValidityOnlyResultType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SimpleAssessmentValidityOnlyResultType"/>
        /// is a valid value, but unsupported.</exception>
        public static SimpleCalculationResultValidityOnly CreateSimpleCalculationResultValidityOnly(SimpleAssessmentValidityOnlyResultType input)
        {
            if (!Enum.IsDefined(typeof(SimpleAssessmentValidityOnlyResultType), input))
            {
                throw new InvalidEnumArgumentException(nameof(input),
                                                       (int) input,
                                                       typeof(SimpleAssessmentValidityOnlyResultType));
            }

            switch (input)
            {
                case SimpleAssessmentValidityOnlyResultType.None:
                    return SimpleCalculationResultValidityOnly.None;
                case SimpleAssessmentValidityOnlyResultType.NotApplicable:
                    return SimpleCalculationResultValidityOnly.NVT;
                case SimpleAssessmentValidityOnlyResultType.Applicable:
                    return SimpleCalculationResultValidityOnly.WVT;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates <see cref="DetailedCalculationInputFromProbability"/> based on the given parameters.
        /// </summary>
        /// <param name="probability">The calculated probability to create the input for.</param>
        /// <param name="categories">A collection of <see cref="FailureMechanismSectionAssemblyCategory"/> to
        /// create the input for.</param>
        /// <returns>The created <see cref="DetailedCalculationInputFromProbability"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categories"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="categories"/> contains
        /// an invalid <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="categories"/> contains
        /// a valid but unsupported <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="AssemblyToolKernelException">Thrown when any input parameter has an
        /// invalid value.</exception>
        public static DetailedCalculationInputFromProbability CreateDetailedCalculationInputFromProbability(
            double probability,
            IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            return new DetailedCalculationInputFromProbability(new Probability(probability),
                                                               categories.Select(category => new FailureMechanismSectionCategory(
                                                                                     ConvertFailureMechanismSectionAssemblyCategoryGroup(category.Group),
                                                                                     new Probability(category.LowerBoundary),
                                                                                     new Probability(category.UpperBoundary))).ToArray());
        }

        /// <summary>
        /// Creates <see cref="DetailedCalculationInputFromProbabilityWithLengthEffect"/> based on the given parameters.
        /// </summary>
        /// <param name="probability">The calculated probability to create the input for.</param>
        /// <param name="categories">A collection of <see cref="FailureMechanismSectionAssemblyCategory"/> to
        /// create the input for.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>The created <see cref="DetailedCalculationInputFromProbabilityWithLengthEffect"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categories"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="categories"/> contains
        /// an invalid <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="categories"/> contains
        /// a valid but unsupported <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="AssemblyToolKernelException">Thrown when any input parameter has an
        /// invalid value.</exception>
        public static DetailedCalculationInputFromProbabilityWithLengthEffect CreateDetailedCalculationInputFromProbabilityWithLengthEffect(
            double probability,
            IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
            double n)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            return new DetailedCalculationInputFromProbabilityWithLengthEffect(
                new Probability(probability),
                categories.Select(category => new FailureMechanismSectionCategory(
                                      ConvertFailureMechanismSectionAssemblyCategoryGroup(category.Group),
                                      new Probability(category.LowerBoundary),
                                      new Probability(category.UpperBoundary))).ToArray(),
                n);
        }

        /// <summary>
        /// Creates <see cref="FailureMechanismSectionAssemblyCategoryResult"/> based on the given parameters.
        /// </summary>
        /// <param name="assembly">The assembly to convert.</param>
        /// <returns>The created <see cref="FailureMechanismSectionAssemblyCategoryResult"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="assembly"/> contains
        /// an invalid <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="assembly"/> contains
        /// a valid but unsupported <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="AssemblyToolKernelException">Thrown when any input parameter has an
        /// invalid value.</exception>
        public static FailureMechanismSectionAssemblyCategoryResult CreateFailureMechanismSectionAssemblyCategoryResult(FailureMechanismSectionAssembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return new FailureMechanismSectionAssemblyCategoryResult(
                ConvertFailureMechanismSectionAssemblyCategoryGroup(assembly.Group),
                new Probability(assembly.Probability));
        }

        /// <summary>
        /// Creates <see cref="DetailedCategoryBoundariesCalculationResult"/> based on the given parameters.
        /// </summary>
        /// <param name="detailedAssesmentResultForFactorizedSignalingNorm">The detailed assessment result
        /// for category Iv - IIv.</param>
        /// <param name="detailedAssesmentResultForSignalingNorm">The detailed assessment result for category
        /// IIv - IIIv.</param>
        /// <param name="detailedAssesmentResultForMechanismSpecificLowerLimitNorm">The detailed assessment
        /// result  for category IIIv - IVv.</param>
        /// <param name="detailedAssesmentResultForLowerLimitNorm">The detailed assessment result for category
        /// IVv - Vv.</param>
        /// <param name="detailedAssesmentResultForFactorizedLowerLimitNorm">The detailed assessment result
        /// for category Vv - VIv.</param>
        /// <returns>The created <see cref="DetailedCategoryBoundariesCalculationResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when any parameter is an invalid
        /// <see cref="DetailedAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when any parameter is a valid but unsupported
        /// <see cref="DetailedAssessmentResultType"/>.</exception>
        public static DetailedCategoryBoundariesCalculationResult CreateDetailedCalculationInputFromCategoryResults(
            DetailedAssessmentResultType detailedAssesmentResultForFactorizedSignalingNorm,
            DetailedAssessmentResultType detailedAssesmentResultForSignalingNorm,
            DetailedAssessmentResultType detailedAssesmentResultForMechanismSpecificLowerLimitNorm,
            DetailedAssessmentResultType detailedAssesmentResultForLowerLimitNorm,
            DetailedAssessmentResultType detailedAssesmentResultForFactorizedLowerLimitNorm)
        {
            return new DetailedCategoryBoundariesCalculationResult(
                ConvertDetailedAssessmentResultType(detailedAssesmentResultForFactorizedSignalingNorm),
                ConvertDetailedAssessmentResultType(detailedAssesmentResultForSignalingNorm),
                ConvertDetailedAssessmentResultType(detailedAssesmentResultForMechanismSpecificLowerLimitNorm),
                ConvertDetailedAssessmentResultType(detailedAssesmentResultForLowerLimitNorm),
                ConvertDetailedAssessmentResultType(detailedAssesmentResultForFactorizedLowerLimitNorm));
        }

        /// <summary>
        /// Creates <see cref="TailorMadeCalculationInputFromProbability"/> based on the given parameters.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The tailor made assessment result to create
        /// the input for.</param>
        /// <param name="probability">The calculated probability to create the input for.</param>
        /// <param name="categories">A collection of <see cref="FailureMechanismSectionAssemblyCategory"/> to
        /// create the input for.</param>
        /// <returns>The created <see cref="TailorMadeCalculationInputFromProbability"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categories"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="categories"/> contains an invalid <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>;</item>
        /// <item><see cref="tailorMadeAssessmentResult"/> is an invalid <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="NotSupportedException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="categories"/> contains a valid but unsupported <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>;</item>
        /// <item><see cref="tailorMadeAssessmentResult"/> a valid but unsupported <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="AssemblyToolKernelException">Thrown when any input parameter has an
        /// invalid value.</exception>
        public static TailorMadeCalculationInputFromProbability CreateTailorMadeCalculationInputFromProbability(
            TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
            double probability,
            IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            return new TailorMadeCalculationInputFromProbability(ConvertTailorMadeProbabilityCalculationResult(tailorMadeAssessmentResult, probability),
                                                                 categories.Select(category => new FailureMechanismSectionCategory(
                                                                                       ConvertFailureMechanismSectionAssemblyCategoryGroup(category.Group),
                                                                                       new Probability(category.LowerBoundary),
                                                                                       new Probability(category.UpperBoundary))).ToArray());
        }

        /// <summary>
        /// Creates <see cref="TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor"/> based on the given parameters.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The tailor made assessment result to create
        /// the input for.</param>
        /// <param name="probability">The calculated probability to create the input for.</param>
        /// <param name="categories">A collection of <see cref="FailureMechanismSectionAssemblyCategory"/> to
        /// create the input for.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>The created <see cref="TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="categories"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="categories"/> contains an invalid <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>;</item>
        /// <item><see cref="tailorMadeAssessmentResult"/> is an invalid <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="NotSupportedException">Thrown when:
        /// <list type="bullet">
        /// <item><see cref="categories"/> contains a valid but unsupported <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>;</item>
        /// <item><see cref="tailorMadeAssessmentResult"/> a valid but unsupported <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</item>
        /// </list>
        /// </exception>
        /// <exception cref="AssemblyToolKernelException">Thrown when any input parameter has an
        /// invalid value.</exception>
        public static TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor CreateTailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
            TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
            double probability,
            IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
            double n)
        {
            if (categories == null)
            {
                throw new ArgumentNullException(nameof(categories));
            }

            return new TailorMadeCalculationInputFromProbabilityWithLengthEffectFactor(
                ConvertTailorMadeProbabilityCalculationResult(tailorMadeAssessmentResult, probability),
                categories.Select(category => new FailureMechanismSectionCategory(
                                      ConvertFailureMechanismSectionAssemblyCategoryGroup(category.Group),
                                      new Probability(category.LowerBoundary),
                                      new Probability(category.UpperBoundary))).ToArray(),
                n);
        }

        /// <summary>
        /// Converts a <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> into a <see cref="FailureMechanismSectionCategoryGroup"/>.
        /// </summary>
        /// <param name="category">The <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> to convert.</param>
        /// <returns>A <see cref="FailureMechanismSectionCategoryGroup"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static FailureMechanismSectionCategoryGroup ConvertFailureMechanismSectionAssemblyCategoryGroup(
            FailureMechanismSectionAssemblyCategoryGroup category)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyCategoryGroup), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(FailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (category)
            {
                case FailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return FailureMechanismSectionCategoryGroup.Iv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return FailureMechanismSectionCategoryGroup.IIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return FailureMechanismSectionCategoryGroup.IIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return FailureMechanismSectionCategoryGroup.IVv;
                case FailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return FailureMechanismSectionCategoryGroup.Vv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return FailureMechanismSectionCategoryGroup.VIv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return FailureMechanismSectionCategoryGroup.VIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                    return FailureMechanismSectionCategoryGroup.NotApplicable;
                case FailureMechanismSectionAssemblyCategoryGroup.None:
                    return FailureMechanismSectionCategoryGroup.None;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> into a <see cref="TailorMadeCategoryCalculationResult"/>.
        /// </summary>
        /// <param name="category">The <see cref="FailureMechanismSectionAssemblyCategoryGroup"/> to convert.</param>
        /// <returns>A <see cref="TailorMadeCategoryCalculationResult"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static TailorMadeCategoryCalculationResult ConvertTailorMadeFailureMechanismSectionAssemblyCategoryGroup(
            FailureMechanismSectionAssemblyCategoryGroup category)
        {
            if (!Enum.IsDefined(typeof(FailureMechanismSectionAssemblyCategoryGroup), category))
            {
                throw new InvalidEnumArgumentException(nameof(category),
                                                       (int) category,
                                                       typeof(FailureMechanismSectionAssemblyCategoryGroup));
            }

            switch (category)
            {
                case FailureMechanismSectionAssemblyCategoryGroup.Iv:
                    return TailorMadeCategoryCalculationResult.FV;
                case FailureMechanismSectionAssemblyCategoryGroup.IIv:
                    return TailorMadeCategoryCalculationResult.IIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IIIv:
                    return TailorMadeCategoryCalculationResult.IIIv;
                case FailureMechanismSectionAssemblyCategoryGroup.IVv:
                    return TailorMadeCategoryCalculationResult.IVv;
                case FailureMechanismSectionAssemblyCategoryGroup.Vv:
                    return TailorMadeCategoryCalculationResult.Vv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIv:
                    return TailorMadeCategoryCalculationResult.VIv;
                case FailureMechanismSectionAssemblyCategoryGroup.VIIv:
                    return TailorMadeCategoryCalculationResult.NGO;
                case FailureMechanismSectionAssemblyCategoryGroup.NotApplicable:
                case FailureMechanismSectionAssemblyCategoryGroup.None:
                    return TailorMadeCategoryCalculationResult.None;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/> and the given
        /// <paramref name="probability"/> to a <see cref="TailorMadeProbabilityCalculationResult"/>.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The tailor made assessment result to create
        /// the input for.</param>
        /// <param name="probability">The calculated probability to create the input for.</param>
        /// <returns>The converted <see cref="TailorMadeProbabilityCalculationResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is an invalid <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is a valid but unsupported <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</exception>
        private static TailorMadeProbabilityCalculationResult ConvertTailorMadeProbabilityCalculationResult(
            TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
            double probability)
        {
            if (!Enum.IsDefined(typeof(TailorMadeAssessmentProbabilityCalculationResultType), tailorMadeAssessmentResult))
            {
                throw new InvalidEnumArgumentException(nameof(tailorMadeAssessmentResult),
                                                       (int) tailorMadeAssessmentResult,
                                                       typeof(TailorMadeAssessmentProbabilityCalculationResultType));
            }

            switch (tailorMadeAssessmentResult)
            {
                case TailorMadeAssessmentProbabilityCalculationResultType.None:
                    return new TailorMadeProbabilityCalculationResult(TailorMadeProbabilityCalculationResultGroup.None);
                case TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible:
                    return new TailorMadeProbabilityCalculationResult(TailorMadeProbabilityCalculationResultGroup.FV);
                case TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed:
                    return new TailorMadeProbabilityCalculationResult(TailorMadeProbabilityCalculationResultGroup.NGO);
                case TailorMadeAssessmentProbabilityCalculationResultType.Probability:
                    return new TailorMadeProbabilityCalculationResult(new Probability(probability));
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Converts a <see cref="DetailedAssessmentResultType"/> into <see cref="DetailedCalculationResult"/>.
        /// </summary>
        /// <param name="detailedAssessmentResult">The detailed assessment result to convert.</param>
        /// <returns>The converted detailed calculation result.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is an invalid <see cref="DetailedAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is a valid but unsupported <see cref="DetailedAssessmentResultType"/>.</exception>
        private static DetailedCalculationResult ConvertDetailedAssessmentResultType(DetailedAssessmentResultType detailedAssessmentResult)
        {
            if (!Enum.IsDefined(typeof(DetailedAssessmentResultType), detailedAssessmentResult))
            {
                throw new InvalidEnumArgumentException(nameof(detailedAssessmentResult),
                                                       (int) detailedAssessmentResult,
                                                       typeof(DetailedAssessmentResultType));
            }

            switch (detailedAssessmentResult)
            {
                case DetailedAssessmentResultType.None:
                    return DetailedCalculationResult.None;
                case DetailedAssessmentResultType.Sufficient:
                    return DetailedCalculationResult.V;
                case DetailedAssessmentResultType.Insufficient:
                    return DetailedCalculationResult.VN;
                case DetailedAssessmentResultType.NotAssessed:
                    return DetailedCalculationResult.NGO;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}