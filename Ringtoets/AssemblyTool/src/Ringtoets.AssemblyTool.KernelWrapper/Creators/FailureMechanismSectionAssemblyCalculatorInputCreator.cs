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
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input instances that can be used in <see cref="IAssessmentResultsTranslator"/>.
    /// </summary>
    public static class FailureMechanismSectionAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates <see cref="EAssessmentResultTypeE1"/> based on the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultType"/> to create the result for.</param>
        /// <returns>The created <see cref="EAssessmentResultTypeE1"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SimpleAssessmentResultType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SimpleAssessmentResultType"/>
        /// is a valid value, but unsupported.</exception>
        public static EAssessmentResultTypeE1 CreateAssessmentResultE1(SimpleAssessmentResultType input)
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
                    return EAssessmentResultTypeE1.Gr;
                case SimpleAssessmentResultType.NotApplicable:
                    return EAssessmentResultTypeE1.Nvt;
                case SimpleAssessmentResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeE1.Fv;
                case SimpleAssessmentResultType.AssessFurther:
                    return EAssessmentResultTypeE1.Vb;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates <see cref="EAssessmentResultTypeE2"/> based on the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentValidityOnlyResultType"/> to create the result for.</param>
        /// <returns>The created <see cref="EAssessmentResultTypeE2"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SimpleAssessmentValidityOnlyResultType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SimpleAssessmentValidityOnlyResultType"/>
        /// is a valid value, but unsupported.</exception>
        public static EAssessmentResultTypeE2 CreateAssessmentResultTypeE2(SimpleAssessmentValidityOnlyResultType input)
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
                    return EAssessmentResultTypeE2.Gr;
                case SimpleAssessmentValidityOnlyResultType.NotApplicable:
                    return EAssessmentResultTypeE2.Nvt;
                case SimpleAssessmentValidityOnlyResultType.Applicable:
                    return EAssessmentResultTypeE2.Wvt;
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
        /// <param name="detailedAssessmentResultForFactorizedSignalingNorm">The detailed assessment result
        /// for category Iv - IIv.</param>
        /// <param name="detailedAssessmentResultForSignalingNorm">The detailed assessment result for category
        /// IIv - IIIv.</param>
        /// <param name="detailedAssessmentResultForMechanismSpecificLowerLimitNorm">The detailed assessment
        /// result  for category IIIv - IVv.</param>
        /// <param name="detailedAssessmentResultForLowerLimitNorm">The detailed assessment result for category
        /// IVv - Vv.</param>
        /// <param name="detailedAssessmentResultForFactorizedLowerLimitNorm">The detailed assessment result
        /// for category Vv - VIv.</param>
        /// <returns>The created <see cref="DetailedCategoryBoundariesCalculationResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when any parameter is an invalid
        /// <see cref="DetailedAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when any parameter is a valid but unsupported
        /// <see cref="DetailedAssessmentResultType"/>.</exception>
        public static DetailedCategoryBoundariesCalculationResult CreateDetailedCalculationInputFromCategoryResults(
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedLowerLimitNorm)
        {
            return new DetailedCategoryBoundariesCalculationResult(
                CreateAssessmentResultTypeG1(detailedAssessmentResultForFactorizedSignalingNorm),
                CreateAssessmentResultTypeG1(detailedAssessmentResultForSignalingNorm),
                CreateAssessmentResultTypeG1(detailedAssessmentResultForMechanismSpecificLowerLimitNorm),
                CreateAssessmentResultTypeG1(detailedAssessmentResultForLowerLimitNorm),
                CreateAssessmentResultTypeG1(detailedAssessmentResultForFactorizedLowerLimitNorm));
        }

        /// <summary>
        /// Creates a <see cref="EAssessmentResultTypeG1"/> based on the given <see cref="DetailedAssessmentResultType"/>.
        /// </summary>
        /// <param name="detailedAssessmentResult">The detailed assessment result to create the result for.</param>
        /// <returns>The created detailed calculation result.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is an invalid <see cref="DetailedAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is a valid but unsupported <see cref="DetailedAssessmentResultType"/>.</exception>
        public static EAssessmentResultTypeG1 CreateAssessmentResultTypeG1(DetailedAssessmentResultType detailedAssessmentResult)
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
                    return EAssessmentResultTypeG1.Gr;
                case DetailedAssessmentResultType.Sufficient:
                    return EAssessmentResultTypeG1.V;
                case DetailedAssessmentResultType.Insufficient:
                    return EAssessmentResultTypeG1.Vn;
                case DetailedAssessmentResultType.NotAssessed:
                    return EAssessmentResultTypeG1.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates a <see cref="EAssessmentResultTypeG2"/> based on the given <see cref="DetailedAssessmentProbabilityOnlyResultType"/>.
        /// </summary>
        /// <param name="detailedAssessmentResult">The detailed assessment result to create the result for.</param>
        /// <returns>The created detailed calculation result.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is an invalid <see cref="DetailedAssessmentProbabilityOnlyResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is a valid but unsupported <see cref="DetailedAssessmentProbabilityOnlyResultType"/>.</exception>
        public static EAssessmentResultTypeG2 CreateAssessmentResultTypeG2(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult)
        {
            if (!Enum.IsDefined(typeof(DetailedAssessmentProbabilityOnlyResultType), detailedAssessmentResult))
            {
                throw new InvalidEnumArgumentException(nameof(detailedAssessmentResult),
                                                       (int) detailedAssessmentResult,
                                                       typeof(DetailedAssessmentProbabilityOnlyResultType));
            }

            switch (detailedAssessmentResult)
            {
                case DetailedAssessmentProbabilityOnlyResultType.Probability:
                    return EAssessmentResultTypeG2.ResultSpecified;
                case DetailedAssessmentProbabilityOnlyResultType.NotAssessed:
                    return EAssessmentResultTypeG2.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates a <see cref="EAssessmentResultTypeT1"/> based on the given <see cref="TailorMadeAssessmentResultType"/>.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The tailor made assessment result to create the result for.</param>
        /// <returns>The created tailor made calculation result.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is an invalid <see cref="TailorMadeAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is a valid but unsupported <see cref="TailorMadeAssessmentResultType"/>.</exception>
        public static EAssessmentResultTypeT1 CreateAssessmentResultTypeT1(TailorMadeAssessmentResultType tailorMadeAssessmentResult)
        {
            if (!Enum.IsDefined(typeof(TailorMadeAssessmentResultType), tailorMadeAssessmentResult))
            {
                throw new InvalidEnumArgumentException(nameof(tailorMadeAssessmentResult),
                                                       (int) tailorMadeAssessmentResult,
                                                       typeof(TailorMadeAssessmentResultType));
            }

            switch (tailorMadeAssessmentResult)
            {
                case TailorMadeAssessmentResultType.None:
                    return EAssessmentResultTypeT1.Gr;
                case TailorMadeAssessmentResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT1.Fv;
                case TailorMadeAssessmentResultType.Sufficient:
                    return EAssessmentResultTypeT1.V;
                case TailorMadeAssessmentResultType.Insufficient:
                    return EAssessmentResultTypeT1.Vn;
                case TailorMadeAssessmentResultType.NotAssessed:
                    return EAssessmentResultTypeT1.Ngo;
                default:
                    throw new NotSupportedException();
            }
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
    }
}