﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
        /// Gets the output of the simple assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssembly SimpleAssessmentAssemblyOutput { get; private set; }

        /// <summary>
        /// Gets the input of the simple assessment calculation.
        /// </summary>
        public SimpleAssessmentResultType SimpleAssessmentInput { get; private set; }

        /// <summary>
        /// Gets the input of the simple assessment validity only calculation.
        /// </summary>
        public SimpleAssessmentResultValidityOnlyType SimpleAssessmentValidityOnlyInput { get; private set; }

        /// <summary>
        /// Gets the output of the detailed assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssembly DetailedAssessmentAssemblyOutput { get; private set; }

        /// <summary>
        /// Gets the result type of the detailed assessment calculation.
        /// </summary>
        public DetailedAssessmentResultType DetailedAssessmentResultInput { get; private set; }

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
        /// Gets the output of the tailor made assessment calculation.
        /// </summary>
        public FailureMechanismSectionAssembly TailorMadeAssessmentAssemblyOutput { get; private set; }

        /// <summary>
        /// Gets the result type of the tailor made assessment calculation.
        /// </summary>
        public TailorMadeAssessmentResultType TailorMadeAssessmentResultInput { get; private set; }

        /// <summary>
        /// Gets the probability input of the tailor made assessment calculation.
        /// </summary>
        public double TailorMadeAssessmentProbabilityInput { get; private set; }

        /// <summary>
        /// Gets the categories input of the tailor made assessment calculation.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssemblyCategory> TailorMadeAssessmentCategoriesInput { get; private set; }

        /// <summary>
        /// Gets the output of the combined assembly calculation.
        /// </summary>
        public FailureMechanismSectionAssembly CombinedAssemblyOutput { get; private set; }

        /// <summary>
        /// Gets the input of the combined assembly calculation.
        /// </summary>
        public IEnumerable<FailureMechanismSectionAssembly> CombinedAssemblyInput { get; private set; }

        /// <summary>
        /// Sets an indicator whether an exception must be thrown when performing a calculation.
        /// </summary>
        public bool ThrowExceptionOnCalculate { private get; set; }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultType input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            SimpleAssessmentInput = input;

            return SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.Iv);
        }

        public FailureMechanismSectionAssembly AssembleSimpleAssessment(SimpleAssessmentResultValidityOnlyType input)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            SimpleAssessmentValidityOnlyInput = input;

            return SimpleAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIIv);
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(DetailedAssessmentResultType detailedAssessmentResult,
                                                                          double probability,
                                                                          IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentResultInput = detailedAssessmentResult;
            DetailedAssessmentProbabilityInput = probability;
            DetailedAssessmentCategoriesInput = categories;

            return DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv);
        }

        public FailureMechanismSectionAssembly AssembleDetailedAssessment(double probability,
                                                                          IEnumerable<FailureMechanismSectionAssemblyCategory> categories,
                                                                          double n)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            DetailedAssessmentProbabilityInput = probability;
            DetailedAssessmentCategoriesInput = categories;
            DetailedAssessmentNInput = n;

            return DetailedAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(0, FailureMechanismSectionAssemblyCategoryGroup.VIv);
        }

        public FailureMechanismSectionAssembly AssembleTailorMadeAssessment(TailorMadeAssessmentResultType tailorMadeAssessmentResult,
                                                                            double probability,
                                                                            IEnumerable<FailureMechanismSectionAssemblyCategory> categories)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            TailorMadeAssessmentResultInput = tailorMadeAssessmentResult;
            TailorMadeAssessmentProbabilityInput = probability;
            TailorMadeAssessmentCategoriesInput = categories;

            return TailorMadeAssessmentAssemblyOutput = new FailureMechanismSectionAssembly(1, FailureMechanismSectionAssemblyCategoryGroup.VIv);
        }

        public FailureMechanismSectionAssembly AssembleCombined(FailureMechanismSectionAssembly simpleAssembly,
                                                                FailureMechanismSectionAssembly detailedAssembly,
                                                                FailureMechanismSectionAssembly tailorMadeAssembly)
        {
            if (ThrowExceptionOnCalculate)
            {
                throw new FailureMechanismSectionAssemblyCalculatorException("Message", new Exception());
            }

            CombinedAssemblyInput = new List<FailureMechanismSectionAssembly>
            {
                simpleAssembly,
                detailedAssembly,
                tailorMadeAssembly
            };

            return CombinedAssemblyOutput = tailorMadeAssembly;
        }
    }
}