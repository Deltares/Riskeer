// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Assembly.Kernel.Exceptions;
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.CategoryLimits;
using Assembly.Kernel.Model.FmSectionTypes;

namespace Riskeer.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Failure mechanism section assembly kernel stub for testing purposes.
    /// </summary>
    public class FailureMechanismSectionAssemblyKernelStub : IAssessmentResultsTranslator
    {
        /// <summary>
        /// Creates a new instance of <see cref="FailureMechanismSectionAssemblyKernelStub"/>.
        /// </summary>
        public FailureMechanismSectionAssemblyKernelStub()
        {
            FailureProbabilityInput = double.NaN;
            LengthEffectFactorInput = double.NaN;
        }

        /// <summary>
        /// Gets the input used in <see cref="TranslateAssessmentResultWbi0E1"/>.
        /// </summary>
        public EAssessmentResultTypeE1? AssessmentResultTypeE1Input { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="TranslateAssessmentResultWbi0E3"/>.
        /// </summary>
        public EAssessmentResultTypeE2? AssessmentResultTypeE2Input { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="TranslateAssessmentResultWbi0G1"/>.
        /// </summary>
        public EAssessmentResultTypeG1? AssessmentResultTypeG1Input { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="TranslateAssessmentResultWbi0G3"/>.
        /// </summary>
        public EAssessmentResultTypeG2? AssessmentResultTypeG2Input { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="TranslateAssessmentResultWbi0T1"/>.
        /// </summary>
        public EAssessmentResultTypeT1? AssessmentResultTypeT1Input { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="TranslateAssessmentResultWbi0T3"/>.
        /// </summary>
        public EAssessmentResultTypeT3? AssessmentResultTypeT3Input { get; private set; }

        /// <summary>
        /// Gets the input used in <see cref="TranslateAssessmentResultWbi0T7"/>.
        /// </summary>
        public EAssessmentResultTypeT4? AssessmentResultTypeT4Input { get; private set; }

        /// <summary>
        /// Gets the section category input used as input parameter for assembly methods.
        /// </summary>
        public EFmSectionCategory? SectionCategoryInput { get; private set; }

        /// <summary>
        /// Gets the simple assessment result used as input parameter for the combined assembly methods.
        /// </summary>
        public FmSectionAssemblyDirectResult SimpleAssessmentResultInput { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result used as input parameter for the combined assembly methods.
        /// </summary>
        public FmSectionAssemblyDirectResult DetailedAssessmentResultInput { get; private set; }

        /// <summary>
        /// Gets the tailor made assessment result used as input parameter for the combined assembly methods.
        /// </summary>
        public FmSectionAssemblyDirectResult TailorMadeAssessmentResultInput { get; private set; }

        /// <summary>
        /// Gets the simple assessment result with probability used as input parameter for the combined assembly methods.
        /// </summary>
        public FmSectionAssemblyDirectResultWithProbability SimpleAssessmentResultInputWithProbability { get; private set; }

        /// <summary>
        /// Gets the detailed assessment result with probability used as input parameter for the combined assembly methods.
        /// </summary>
        public FmSectionAssemblyDirectResultWithProbability DetailedAssessmentResultInputWithProbability { get; private set; }

        /// <summary>
        /// Gets the tailor made assessment result with probability used as input parameter for the combined assembly methods.
        /// </summary>
        public FmSectionAssemblyDirectResultWithProbability TailorMadeAssessmentResultInputWithProbability { get; private set; }

        /// <summary>
        /// Gets the <see cref="AssessmentSection"/> used as input parameter for assembly methods.
        /// </summary>
        public AssessmentSection AssessmentSectionInput { get; private set; }

        /// <summary>
        /// Gets the <see cref="FailureMechanism"/> used as input parameter for assembly methods.
        /// </summary>
        public FailureMechanism FailureMechanismInput { get; private set; }

        /// <summary>
        /// Gets the probability of failure used as input parameter for assembly methods.
        /// </summary>
        public double FailureProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect' which is 
        /// used as input parameter for assembly methods.
        /// </summary>
        public double LengthEffectFactorInput { get; private set; }

        /// <summary>
        /// Gets the <see cref="FmSectionCategoryCompliancyResults"/> used as input parameter for assembly methods.
        /// </summary>
        public FmSectionCategoryCompliancyResults CategoryCompliancyResultsInput { get; private set; }

        /// <summary>
        /// Gets the <see cref="CategoriesList{TCategory}"/> with <see cref="FmSectionCategory"/> used
        /// as input for assembly methods.
        /// </summary>
        public CategoriesList<FmSectionCategory> FailureMechanismSectionCategories { get; private set; }

        /// <summary>
        /// Gets or sets the failure mechanism section assembly result.
        /// </summary>
        public FmSectionAssemblyDirectResult FailureMechanismSectionDirectResult { get; set; }

        /// <summary>
        /// Gets or sets the failure mechanism section assembly result with probability.
        /// </summary>
        public FmSectionAssemblyDirectResultWithProbability FailureMechanismSectionDirectResultWithProbability { get; set; }

        /// <summary>
        /// Gets a value indicating whether a calculation was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="Exception"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        /// <summary>
        /// Sets an indicator whether an <see cref="AssemblyException"/> must be thrown while performing a calculation.
        /// </summary>
        public bool ThrowAssemblyExceptionOnCalculate { private get; set; }

        public FmSectionAssemblyDirectResultWithProbability TranslateAssessmentResultWbi0E1(EAssessmentResultTypeE1 assessment)
        {
            ThrowException();

            AssessmentResultTypeE1Input = assessment;

            Calculated = true;
            return FailureMechanismSectionDirectResultWithProbability;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0E2(EAssessmentResultTypeE1 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResultWithProbability TranslateAssessmentResultWbi0E3(EAssessmentResultTypeE2 assessment)
        {
            ThrowException();

            AssessmentResultTypeE2Input = assessment;

            Calculated = true;
            return FailureMechanismSectionDirectResultWithProbability;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0E4(EAssessmentResultTypeE2 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0G1(EAssessmentResultTypeG1 assessment)
        {
            ThrowException();

            AssessmentResultTypeG1Input = assessment;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0G2(EAssessmentResultTypeG1 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResultWithProbability TranslateAssessmentResultWbi0G3(EAssessmentResultTypeG2 assessment,
                                                                                            double failureProbability,
                                                                                            CategoriesList<FmSectionCategory> categories)
        {
            ThrowException();

            AssessmentResultTypeG2Input = assessment;
            FailureProbabilityInput = failureProbability;
            FailureMechanismSectionCategories = categories;

            Calculated = true;
            return FailureMechanismSectionDirectResultWithProbability;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0G4(EAssessmentResultTypeG2 assessment, EFmSectionCategory? category)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResultWithProbability TranslateAssessmentResultWbi0G5(double fmSectionLengthEffectFactor,
                                                                                            EAssessmentResultTypeG2 assessment,
                                                                                            double failureProbability,
                                                                                            CategoriesList<FmSectionCategory> categories)
        {
            ThrowException();

            AssessmentResultTypeG2Input = assessment;
            FailureProbabilityInput = failureProbability;
            LengthEffectFactorInput = fmSectionLengthEffectFactor;
            FailureMechanismSectionCategories = categories;

            Calculated = true;
            return FailureMechanismSectionDirectResultWithProbability;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0G6(FmSectionCategoryCompliancyResults compliancyResults)
        {
            ThrowException();

            CategoryCompliancyResultsInput = compliancyResults;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T1(EAssessmentResultTypeT1 assessment)
        {
            ThrowException();

            AssessmentResultTypeT1Input = assessment;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0T2(EAssessmentResultTypeT2 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResultWithProbability TranslateAssessmentResultWbi0T3(EAssessmentResultTypeT3 assessment,
                                                                                            double failureProbability,
                                                                                            CategoriesList<FmSectionCategory> categories)
        {
            ThrowException();

            AssessmentResultTypeT3Input = assessment;
            FailureProbabilityInput = failureProbability;
            FailureMechanismSectionCategories = categories;

            Calculated = true;
            return FailureMechanismSectionDirectResultWithProbability;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T4(EAssessmentResultTypeT3 assessment, EFmSectionCategory? category)
        {
            ThrowException();

            AssessmentResultTypeT3Input = assessment;
            SectionCategoryInput = category;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResultWithProbability TranslateAssessmentResultWbi0T5(double fmSectionLengthEffectFactor,
                                                                                            EAssessmentResultTypeT3 assessment,
                                                                                            double failureProbability,
                                                                                            CategoriesList<FmSectionCategory> categories)
        {
            ThrowException();

            AssessmentResultTypeT3Input = assessment;
            FailureProbabilityInput = failureProbability;
            LengthEffectFactorInput = fmSectionLengthEffectFactor;
            FailureMechanismSectionCategories = categories;

            Calculated = true;
            return FailureMechanismSectionDirectResultWithProbability;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T6(FmSectionCategoryCompliancyResults compliancyResults, EAssessmentResultTypeT3 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T7(EAssessmentResultTypeT4 assessment,
                                                                             double failureProbability,
                                                                             CategoriesList<FmSectionCategory> categories)
        {
            ThrowException();

            AssessmentResultTypeT4Input = assessment;
            FailureProbabilityInput = failureProbability;
            FailureMechanismSectionCategories = categories;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0A1(FmSectionAssemblyDirectResult simpleAssessmentResult,
                                                                             FmSectionAssemblyDirectResult detailedAssessmentResult,
                                                                             FmSectionAssemblyDirectResult customAssessmentResult)
        {
            ThrowException();

            SimpleAssessmentResultInput = simpleAssessmentResult;
            DetailedAssessmentResultInput = detailedAssessmentResult;
            TailorMadeAssessmentResultInput = customAssessmentResult;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResultWithProbability TranslateAssessmentResultWbi0A1(FmSectionAssemblyDirectResultWithProbability simpleAssessmentResult,
                                                                                            FmSectionAssemblyDirectResultWithProbability detailedAssessmentResult,
                                                                                            FmSectionAssemblyDirectResultWithProbability customAssessmentResult)
        {
            ThrowException();

            SimpleAssessmentResultInputWithProbability = simpleAssessmentResult;
            DetailedAssessmentResultInputWithProbability = detailedAssessmentResult;
            TailorMadeAssessmentResultInputWithProbability = customAssessmentResult;

            Calculated = true;
            return FailureMechanismSectionDirectResultWithProbability;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0A1(FmSectionAssemblyIndirectResult simpleAssessmentResult,
                                                                               FmSectionAssemblyIndirectResult detailedAssessmentResult,
                                                                               FmSectionAssemblyIndirectResult customAssessmentResult)
        {
            throw new NotImplementedException();
        }

        private void ThrowException()
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            if (ThrowAssemblyExceptionOnCalculate)
            {
                throw new AssemblyException("entity", EAssemblyErrors.CategoryLowerLimitOutOfRange);
            }
        }
    }
}