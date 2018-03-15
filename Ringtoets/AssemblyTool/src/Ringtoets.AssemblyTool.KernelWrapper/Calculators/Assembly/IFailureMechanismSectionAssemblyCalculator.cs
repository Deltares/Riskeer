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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Primitives;

namespace Ringtoets.AssemblyTool.KernelWrapper.Calculators.Assembly
{
    /// <summary>
    /// Interface representing a failure mechanism section assembly calculator.
    /// </summary>
    public interface IFailureMechanismSectionAssemblyCalculator
    {
        /// <summary>
        /// Assembles the simple assessment for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentResultType"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultType input);

        /// <summary>
        /// Assembles the simple assessment for the given <paramref name="input"/>.
        /// </summary>
        /// <param name="input">The <see cref="SimpleAssessmentValidityOnlyResultType"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentValidityOnlyResultType input);

        /// <summary>
        /// Assembles the detailed assessment based on the input parameter.
        /// </summary>
        /// <param name="detailedAssessmentResult">The <see cref="DetailedAssessmentProbabilityOnlyResultType"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(DetailedAssessmentResultType detailedAssessmentResult);

        /// <summary>
        /// Assembles the detailed assessment based on the input parameters.
        /// </summary>
        /// <param name="detailedAssessmentResult">The <see cref="DetailedAssessmentProbabilityOnlyResultType"/> to assemble for.</param>
        /// <param name="probability">The calculated probability.</param>
        /// <param name="categories">The collection of categories for this failure mechanism section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="InvalidEnumArgumentException">Thrown when <see cref="detailedAssessmentResult"/> is
        /// an invalid <see cref="DetailedAssessmentProbabilityOnlyResultType"/>.</exception>
        /// <exception cref="NotSupportedException">Thrown when <see cref="detailedAssessmentResult"/> contains
        /// a valid but unsupported <see cref="DetailedAssessmentProbabilityOnlyResultType"/>.</exception>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                   double probability,
                                                                   IEnumerable<FailureMechanismSectionAssemblyCategory> categories);

        /// <summary>
        /// Assembles the detailed assessment based on the input parameters.
        /// </summary>
        /// <param name="detailedAssessmentResult">The <see cref="DetailedAssessmentProbabilityOnlyResultType"/> to assemble for.</param>
        /// <param name="probability">The calculated probability.</param>
        /// <param name="categories">The collection of categories for this failure mechanism section.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentProbabilityOnlyResultType detailedAssessmentResult,
                                                                   double probability,
                                                                   IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
                                                                   double n);

        /// <summary>
        /// Assembles the detailed assessment based on the input parameters.
        /// </summary>
        /// <param name="detailedAssessmentResultForFactorizedSignalingNorm">The category Iv - IIv 
        /// <see cref="DetailedAssessmentResultType"/> to assemble for.</param>
        /// <param name="detailedAssessmentResultForSignalingNorm">The category IIv - IIIv 
        /// <see cref="DetailedAssessmentResultType"/> to assemble for.</param>
        /// <param name="detailedAssessmentResultForMechanismSpecificLowerLimitNorm">TThe category IIIv - IVv 
        /// <see cref="DetailedAssessmentResultType"/> to assemble for.</param>
        /// <param name="detailedAssessmentResultForLowerLimitNorm">The category IVv - Vv 
        /// <see cref="DetailedAssessmentResultType"/> to assemble for.</param>
        /// <param name="detailedAssessmentResultForFactorizedLowerLimitNorm">The category Vv - VIv 
        /// <see cref="DetailedAssessmentResultType"/> to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssemblyCategoryGroup AssembleDetailedAssessment(DetailedAssessmentResultType detailedAssessmentResultForFactorizedSignalingNorm,
                                                                                DetailedAssessmentResultType detailedAssessmentResultForSignalingNorm,
                                                                                DetailedAssessmentResultType detailedAssessmentResultForMechanismSpecificLowerLimitNorm,
                                                                                DetailedAssessmentResultType detailedAssessmentResultForLowerLimitNorm,
                                                                                DetailedAssessmentResultType detailedAssessmentResultForFactorizedLowerLimitNorm);

        /// <summary>
        /// Assembles the tailor made assessment based on the input parameters.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The <see cref="TailorMadeAssessmentProbabilityAndDetailedCalculationResultType"/>
        /// to assemble for.</param>
        /// <param name="probability">The calculated probability.</param>
        /// <param name="categories">The collection of categories for this failure mechanism section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityAndDetailedCalculationResultType tailorMadeAssessmentResult,
                                                                     double probability,
                                                                     IEnumerable<FailureMechanismSectionAssemblyCategory> categories);

        /// <summary>
        /// Assembles the tailor made assessment based on the input parameters.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>
        /// to assemble for.</param>
        /// <param name="probability">The calculated probability.</param>
        /// <param name="categories">The collection of categories for this failure mechanism section.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                     double probability,
                                                                     IEnumerable<FailureMechanismSectionAssemblyCategory> categories);

        /// <summary>
        /// Assembles the tailor made assessment based on the input parameters.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The <see cref="TailorMadeAssessmentProbabilityCalculationResultType"/>
        /// to assemble for.</param>
        /// <param name="probability">The calculated probability.</param>
        /// <param name="categories">The collection of categories for this failure mechanism section.</param>
        /// <param name="n">The 'N' parameter used to factor in the 'length effect'.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentProbabilityCalculationResultType tailorMadeAssessmentResult,
                                                                     double probability,
                                                                     IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
                                                                     double n);

        /// <summary>
        /// Assembles the tailor made assessment based on the input parameters.
        /// </summary>
        /// <param name="tailorMadeAssessmentResult">The <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>
        /// to assemble for.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssemblyCategoryGroup AssembleTailorMadeAssessment(FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssessmentResult);

        /// <summary>
        /// Assembles the combined assembly based on the input parameters.
        /// </summary>
        /// <param name="simpleAssembly">The simple assembly.</param>
        /// <param name="detailedAssembly">The detailed assembly.</param>
        /// <param name="tailorMadeAssembly">The tailor made assembly.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssembly"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssembly AssembleCombined(FailureMechanismSectionAssembly simpleAssembly,
                                                         FailureMechanismSectionAssembly detailedAssembly,
                                                         FailureMechanismSectionAssembly tailorMadeAssembly);

        /// <summary>
        /// Assembles the combined assembly based on the input parameters.
        /// </summary>
        /// <param name="simpleAssembly">The simple assembly.</param>
        /// <param name="detailedAssembly">The detailed assembly.</param>
        /// <param name="tailorMadeAssembly">The tailor made assembly.</param>
        /// <returns>A <see cref="FailureMechanismSectionAssemblyCategoryGroup"/>.</returns>
        /// <exception cref="FailureMechanismSectionAssemblyCalculatorException">Thrown when
        /// an error occurs when performing the assembly.</exception>
        FailureMechanismSectionAssemblyCategoryGroup AssembleCombined(FailureMechanismSectionAssemblyCategoryGroup simpleAssembly,
                                                                      FailureMechanismSectionAssemblyCategoryGroup detailedAssembly,
                                                                      FailureMechanismSectionAssemblyCategoryGroup tailorMadeAssembly);
    }
}