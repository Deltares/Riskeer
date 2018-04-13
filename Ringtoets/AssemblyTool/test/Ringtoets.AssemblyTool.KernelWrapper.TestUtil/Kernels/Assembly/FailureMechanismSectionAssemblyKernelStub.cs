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
using Assembly.Kernel.Interfaces;
using Assembly.Kernel.Model;
using Assembly.Kernel.Model.AssessmentResultTypes;
using Assembly.Kernel.Model.FmSectionTypes;

namespace Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Kernels.Assembly
{
    /// <summary>
    /// Failure mechanism section assembly kernel stub for testing purposes.
    /// </summary>
    public class FailureMechanismSectionAssemblyKernelStub : IAssessmentResultsTranslator
    {
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
        public double? FailureProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the 'N' parameter used to factor in the 'length effect' which is 
        /// used as input parameter for assembly methods.
        /// </summary>
        public double? LengthEffectFactorInput { get; private set; }

        /// <summary>
        /// Gets the <see cref="FmSectionCategoryCompliancyResults"/> used as input parameter for assembly methods.
        /// </summary>
        public FmSectionCategoryCompliancyResults CategoryCompliancyResultsInput { get; private set; }

        /// <summary>
        /// Gets or sets the failure mechanism section assembly result.
        /// </summary>
        public FmSectionAssemblyDirectResult FailureMechanismSectionDirectResult { get; set; }

        /// <summary>
        /// Gets a value indicating whether a calculation was called or not.
        /// </summary>
        public bool Calculated { get; private set; }

        /// <summary>
        /// Indicator whether an exception must be thrown when performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0E1(EAssessmentResultTypeE1 assessment)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentResultTypeE1Input = assessment;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0E2(EAssessmentResultTypeE1 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0E3(EAssessmentResultTypeE2 assessment)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentResultTypeE2Input = assessment;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0E4(EAssessmentResultTypeE2 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0G1(EAssessmentResultTypeG1 assessment)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentResultTypeG1Input = assessment;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0G2(EAssessmentResultTypeG1 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0G3(AssessmentSection section,
                                                                             FailureMechanism failureMechanism,
                                                                             EAssessmentResultTypeG2 assessment,
                                                                             double? failureProbability)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentSectionInput = section;
            FailureMechanismInput = failureMechanism;
            AssessmentResultTypeG2Input = assessment;
            FailureProbabilityInput = failureProbability;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0G4(EAssessmentResultTypeG2 assessment, EFmSectionCategory? category)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0G5(AssessmentSection section,
                                                                             FailureMechanism failureMechanism,
                                                                             double fmSectionLengthEffectFactor,
                                                                             EAssessmentResultTypeG2 assessment,
                                                                             double? failureProbability)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentSectionInput = section;
            FailureMechanismInput = failureMechanism;
            AssessmentResultTypeG2Input = assessment;
            FailureProbabilityInput = failureProbability;
            LengthEffectFactorInput = fmSectionLengthEffectFactor;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0G6(FmSectionCategoryCompliancyResults compliancyResults)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            CategoryCompliancyResultsInput = compliancyResults;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T1(EAssessmentResultTypeT1 assessment)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentResultTypeT1Input = assessment;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyIndirectResult TranslateAssessmentResultWbi0T2(EAssessmentResultTypeT2 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T3(AssessmentSection section,
                                                                             FailureMechanism failureMechanism,
                                                                             EAssessmentResultTypeT3 assessment,
                                                                             double? failureProbability)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentSectionInput = section;
            FailureMechanismInput = failureMechanism;
            AssessmentResultTypeT3Input = assessment;
            FailureProbabilityInput = failureProbability;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T4(EAssessmentResultTypeT3 assessment, EFmSectionCategory? category)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentResultTypeT3Input = assessment;
            SectionCategoryInput = category;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T5(AssessmentSection section,
                                                                             FailureMechanism failureMechanism,
                                                                             double fmSectionLengthEffectFactor,
                                                                             EAssessmentResultTypeT3 assessment,
                                                                             double? failureProbability)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            AssessmentSectionInput = section;
            FailureMechanismInput = failureMechanism;
            AssessmentResultTypeT3Input = assessment;
            FailureProbabilityInput = failureProbability;
            LengthEffectFactorInput = LengthEffectFactorInput;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T6(FmSectionCategoryCompliancyResults compliancyResults, EAssessmentResultTypeT3 assessment)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyDirectResult TranslateAssessmentResultWbi0T7(AssessmentSection section, FailureMechanism failureMechanism, EAssessmentResultTypeT4 assessment, double? failureProbability)
        {
            throw new NotImplementedException();
        }

        public FmSectionAssemblyResult TranslateAssessmentResultWbi0A1(FmSectionAssemblyDirectResult simpleAssessmentResult,
                                                                       FmSectionAssemblyDirectResult detailedAssessmentResult,
                                                                       FmSectionAssemblyDirectResult tailorMadeAssessmentResult)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new Exception("Message", new Exception());
            }

            SimpleAssessmentResultInput = simpleAssessmentResult;
            DetailedAssessmentResultInput = detailedAssessmentResult;
            TailorMadeAssessmentResultInput = tailorMadeAssessmentResult;

            Calculated = true;
            return FailureMechanismSectionDirectResult;
        }

        public FmSectionAssemblyResult TranslateAssessmentResultWbi0A1(FmSectionAssemblyIndirectResult simpleAssessmentResult,
                                                                       FmSectionAssemblyIndirectResult detailedAssessmentResult,
                                                                       FmSectionAssemblyIndirectResult customAssessmentResult)
        {
            throw new NotImplementedException();
        }
    }
}