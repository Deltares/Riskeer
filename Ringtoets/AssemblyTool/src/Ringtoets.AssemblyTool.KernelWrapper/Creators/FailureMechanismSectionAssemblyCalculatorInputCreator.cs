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
using System.ComponentModel;
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.FmSectionTypes;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Creators
{
    /// <summary>
    /// Creates input instances that can be used in <see cref="IAssessmentResultsTranslator"/>.
    /// </summary>
    internal static class FailureMechanismSectionAssemblyCalculatorInputCreator
    {
        /// <summary>
        /// Creates <see cref="EAssessmentResultTypeE1"/> based on the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultType"/> to create an
        /// <see cref="EAssessmentResultTypeE1"/> for.</param>
        /// <returns>The created <see cref="EAssessmentResultTypeE1"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="SimpleAssessmentResultType"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="SimpleAssessmentResultType"/>
        /// is a valid value, but unsupported.</exception>
        public static EAssessmentResultTypeE1 CreateAssessmentResultTypeE1(SimpleAssessmentResultType input)
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
        /// <param name="input">The <see cref="SimpleAssessmentValidityOnlyResultType"/> to create an
        /// <see cref="EAssessmentResultTypeE2"/> for.</param>
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
        /// Creates <see cref="FmSectionAssemblyDirectResultWithProbability"/> based on the given parameters.
        /// </summary>
        /// <param name="assembly">The <see cref="FailureMechanismSectionAssembly"/> to create
        /// a <see cref="FmSectionAssemblyDirectResultWithProbability"/> for.</param>
        /// <returns>The created <see cref="FmSectionAssemblyDirectResultWithProbability"/>.</returns>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="assembly"/>
        /// is <c>null</c>.</exception>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="assembly"/> contains
        /// an invalid <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="assembly"/> contains
        /// a valid but unsupported <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="AssemblyException">Thrown when <paramref name="assembly"/> has an
        /// invalid value.</exception>
        public static FmSectionAssemblyDirectResultWithProbability CreateFailureMechanismSectionAssemblyDirectResult(FailureMechanismSectionAssembly assembly)
        {
            if (assembly == null)
            {
                throw new ArgumentNullException(nameof(assembly));
            }

            return new FmSectionAssemblyDirectResultWithProbability(AssemblyCalculatorInputCreator.CreateFailureMechanismSectionCategory(assembly.Group),
                                                                    assembly.Probability);
        }

        /// <summary>
        /// Creates <see cref="FmSectionAssemblyDirectResult"/> based on the given parameters.
        /// </summary>
        /// <param name="categoryGroup">The <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// to create a <see cref="FmSectionAssemblyDirectResult"/> for.</param>
        /// <returns>The created <see cref="FmSectionAssemblyDirectResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="categoryGroup"/> is
        /// an invalid <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="categoryGroup"/> is
        /// a valid but unsupported <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</exception>
        /// <exception cref="AssemblyException">Thrown when <paramref name="categoryGroup"/> has an
        /// invalid value.</exception>
        public static FmSectionAssemblyDirectResult CreateFailureMechanismSectionAssemblyDirectResult(
            FailureMechanismSectionAssemblyCategoryGroup categoryGroup)
        {
            return new FmSectionAssemblyDirectResult(AssemblyCalculatorInputCreator.CreateFailureMechanismSectionCategory(categoryGroup));
        }

        /// <summary>
        /// Creates a <see cref="FmSectionCategoryCompliancyResults"/> based on the given parameters.
        /// </summary>
        /// <param name="detailedAssessmentResultForFactorizedSignalingNorm">The <see cref="DetailedAssessmentResultType"/>
        /// for category boundary Iv.</param>
        /// <param name="detailedAssessmentResultForSignalingNorm">The <see cref="DetailedAssessmentResultType"/> for category
        /// boundary IIv.</param>
        /// <param name="detailedAssessmentResultForMechanismSpecificLowerLimitNorm">The <see cref="DetailedAssessmentResultType"/>
        /// result  for category boundary IIIv.</param>
        /// <param name="detailedAssessmentResultForLowerLimitNorm">The <see cref="DetailedAssessmentResultType"/> for category
        /// boundary IVv.</param>
        /// <param name="detailedAssessmentResultForFactorizedLowerLimitNorm">The <see cref="DetailedAssessmentResultType"/>
        /// for category boundary Vv.</param>
        /// <returns>The created <see cref="FmSectionCategoryCompliancyResults"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when any parameter is an invalid
        /// <see cref="DetailedAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when any parameter is a valid, but unsupported
        /// <see cref="DetailedAssessmentResultType"/>.</exception>
        public static FmSectionCategoryCompliancyResults CreateCategoryCompliancyResults(
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForSignalingNorm,
            DetailedAssessmentResultType detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForLowerLimitNorm,
            DetailedAssessmentResultType detailedAssessmentResultForFactorizedLowerLimitNorm)
        {
            var compliancyResults = new FmSectionCategoryCompliancyResults();
            compliancyResults.Set(EFmSectionCategory.Iv, CreateCategoryCompliancy(detailedAssessmentResultForFactorizedSignalingNorm));
            compliancyResults.Set(EFmSectionCategory.IIv, CreateCategoryCompliancy(detailedAssessmentResultForSignalingNorm));
            compliancyResults.Set(EFmSectionCategory.IIIv, CreateCategoryCompliancy(detailedAssessmentResultForMechanismSpecificLowerLimitNorm));
            compliancyResults.Set(EFmSectionCategory.IVv, CreateCategoryCompliancy(detailedAssessmentResultForLowerLimitNorm));
            compliancyResults.Set(EFmSectionCategory.Vv, CreateCategoryCompliancy(detailedAssessmentResultForFactorizedLowerLimitNorm));
            return compliancyResults;
        }

        /// <summary>
        /// Creates a <see cref="EAssessmentResultTypeG1"/> based on the given <see cref="DetailedAssessmentResultType"/>.
        /// </summary>
        /// <param name="detailedAssessmentResult">The <see cref="DetailedAssessmentResultType"/> to create an
        /// <see cref="EAssessmentResultTypeG1"/> for.</param>
        /// <returns>The created <see cref="EAssessmentResultTypeG1"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is an invalid <see cref="DetailedAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is a valid, but unsupported <see cref="DetailedAssessmentResultType"/>.</exception>
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
        /// <param name="detailedAssessmentResult">The <see cref="DetailedAssessmentProbabilityOnlyResultType"/> to create an
        /// <see cref="EAssessmentResultTypeG2"/> for.</param>
        /// <returns>The created <see cref="EAssessmentResultTypeG2"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is an invalid <see cref="DetailedAssessmentProbabilityOnlyResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is a valid, but unsupported <see cref="DetailedAssessmentProbabilityOnlyResultType"/>.</exception>
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
        /// <param name="tailorMadeAssessmentResult">The <see cref="TailorMadeAssessmentResultType"/> result to create 
        /// an <see cref="EAssessmentResultTypeT1"/> for.</param>
        /// <returns>The created <see cref="EAssessmentResultTypeT1"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is an invalid <see cref="TailorMadeAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is a valid, but unsupported <see cref="TailorMadeAssessmentResultType"/>.</exception>
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
        /// Creates a <see cref="EAssessmentResultTypeT3"/> based on the given <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/> to create an 
        /// <see cref="EAssessmentResultTypeT3"/> for.</param>
        /// <returns>The created <see cref="EAssessmentResultTypeT3"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is an invalid <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is a valid, but unsupported <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</exception>
        public static EAssessmentResultTypeT3 CreateAssessmentResultTypeT3(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult)
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
                    return EAssessmentResultTypeT3.Gr;
                case TailorMadeAssessmentProbabilityCalculationResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT3.Fv;
                case TailorMadeAssessmentProbabilityCalculationResultType.Probability:
                    return EAssessmentResultTypeT3.ResultSpecified;
                case TailorMadeAssessmentProbabilityCalculationResultType.NotAssessed:
                    return EAssessmentResultTypeT3.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates a <see cref="EAssessmentResultTypeT4"/> based on the
        /// given <see cref="TailorMadeAssessmentProbabilityAndDetailedCalculationResultType"/>.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The <see cref="TailorMadeAssessmentProbabilityAndDetailedCalculationResultType"/> to create an 
        /// <see cref="EAssessmentResultTypeT4"/> for.</param>
        /// <returns>The created <see cref="EAssessmentResultTypeT4"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is an invalid <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is a valid, but unsupported <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>.</exception>
        public static EAssessmentResultTypeT4 CreateAssessmentResultTypeT4(
            TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult)
        {
            if (!Enum.IsDefined(typeof(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType), tailorMadeAssessmentResult))
            {
                throw new InvalidEnumArgumentException(nameof(tailorMadeAssessmentResult),
                                                       (int) tailorMadeAssessmentResult,
                                                       typeof(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType));
            }

            switch (tailorMadeAssessmentResult)
            {
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.None:
                    return EAssessmentResultTypeT4.Gr;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.ProbabilityNegligible:
                    return EAssessmentResultTypeT4.Fv;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Probability:
                    return EAssessmentResultTypeT4.ResultSpecified;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Sufficient:
                    return EAssessmentResultTypeT4.V;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.Insufficient:
                    return EAssessmentResultTypeT4.Vn;
                case TailorMadeAssessmentProbabilityAndDetailedCalculationResultType.NotAssessed:
                    return EAssessmentResultTypeT4.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates a <see cref="Tuple{EAssessmentResultTypeT3, EFmSectionCategory}"/> from a <see cref="TailorMadeAssessmentCategoryGroupResultType"/>.
        /// </summary>
        /// <param name="category">The <see cref="TailorMadeAssessmentCategoryGroupResultType"/> to create a
        /// <see cref="Tuple{EAssessmentResultTypeT3, EFmSectionCategory}"/> for.</param>
        /// <returns>A <see cref="Tuple{EAssessmentResultTypeT3, EFmSectionCategory}"/> based on <paramref name="category"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="category"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="category"/>
        /// is a valid value, but unsupported.</exception>
        public static Tuple<EAssessmentResultTypeT3, EFmSectionCategory?> CreateAssessmentResultTypeT3WithCategoryGroupResult(
            TailorMadeAssessmentCategoryGroupResultType category)
        {
            EFmSectionCategory result = CreateFailureMechanismSectionCategory(category);
            return result == EFmSectionCategory.Gr
                       ? new Tuple<EAssessmentResultTypeT3, EFmSectionCategory?>(EAssessmentResultTypeT3.Gr, null)
                       : new Tuple<EAssessmentResultTypeT3, EFmSectionCategory?>(EAssessmentResultTypeT3.ResultSpecified, result);
        }

        /// <summary>
        /// Creates a <see cref="EFmSectionCategory"/> from a <see cref="TailorMadeAssessmentCategoryGroupResultType"/>.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The <see cref="TailorMadeAssessmentCategoryGroupResultType"/> to create
        /// an <see cref="EFmSectionCategory"/>.</param>
        /// <returns>A <see cref="EFmSectionCategory"/> based on <paramref name="tailorMadeAssessmentResult"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is an invalid value.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="tailorMadeAssessmentResult"/>
        /// is a valid value, but unsupported.</exception>
        private static EFmSectionCategory CreateFailureMechanismSectionCategory(
            TailorMadeAssessmentCategoryGroupResultType tailorMadeAssessmentResult)
        {
            if (!Enum.IsDefined(typeof(TailorMadeAssessmentCategoryGroupResultType), tailorMadeAssessmentResult))
            {
                throw new InvalidEnumArgumentException(nameof(tailorMadeAssessmentResult),
                                                       (int) tailorMadeAssessmentResult,
                                                       typeof(TailorMadeAssessmentCategoryGroupResultType));
            }

            switch (tailorMadeAssessmentResult)
            {
                case TailorMadeAssessmentCategoryGroupResultType.Iv:
                    return EFmSectionCategory.Iv;
                case TailorMadeAssessmentCategoryGroupResultType.IIv:
                    return EFmSectionCategory.IIv;
                case TailorMadeAssessmentCategoryGroupResultType.IIIv:
                    return EFmSectionCategory.IIIv;
                case TailorMadeAssessmentCategoryGroupResultType.IVv:
                    return EFmSectionCategory.IVv;
                case TailorMadeAssessmentCategoryGroupResultType.Vv:
                    return EFmSectionCategory.Vv;
                case TailorMadeAssessmentCategoryGroupResultType.VIv:
                    return EFmSectionCategory.VIv;
                case TailorMadeAssessmentCategoryGroupResultType.VIIv:
                    return EFmSectionCategory.VIIv;
                case TailorMadeAssessmentCategoryGroupResultType.None:
                    return EFmSectionCategory.Gr;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Creates a <see cref="ECategoryCompliancy"/> based on the given <see cref="DetailedAssessmentResultType"/>.
        /// </summary>
        /// <param name="detailedAssessmentResult">The <see cref="DetailedAssessmentResultType"/> to create an 
        /// <see cref="ECategoryCompliancy"/> for.</param>
        /// <returns>The created <see cref="ECategoryCompliancy"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is an invalid <see cref="DetailedAssessmentResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="detailedAssessmentResult"/>
        /// is a valid, but unsupported <see cref="DetailedAssessmentResultType"/>.</exception>
        private static ECategoryCompliancy CreateCategoryCompliancy(DetailedAssessmentResultType detailedAssessmentResult)
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
                    return ECategoryCompliancy.NoResult;
                case DetailedAssessmentResultType.Sufficient:
                    return ECategoryCompliancy.Complies;
                case DetailedAssessmentResultType.Insufficient:
                    return ECategoryCompliancy.DoesNotComply;
                case DetailedAssessmentResultType.NotAssessed:
                    return ECategoryCompliancy.Ngo;
                default:
                    throw new NotSupportedException();
            }
        }
    }
}