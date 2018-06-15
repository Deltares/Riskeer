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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.AssemblyTool.Data;
using Ringtoets.AssemblyTool.KernelWrapper.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Ringtoets.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;

namespace Ringtoets.Common.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GeotechnicalFailureMechanismAssemblyCategoriesContextTest
    {
        private const double tolerance = 1e-5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble();

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Contribution = random.NextDouble();
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                var context = new GeotechnicalFailureMechanismAssemblyCategoriesContext(failureMechanism,
                                                                                        assessmentSection,
                                                                                        () => n);

                // Assert
                Assert.IsInstanceOf<FailureMechanismAssemblyCategoriesContextBase>(context);
                Assert.AreSame(failureMechanism, context.WrappedData);
                Assert.AreSame(assessmentSection, context.AssessmentSection);

                IEnumerable<FailureMechanismSectionAssemblyCategory> output = context.GetFailureMechanismSectionAssemblyCategoriesFunc();
                IEnumerable<FailureMechanismSectionAssemblyCategory> calculatorOutput = calculator.GeoTechnicalFailureMechanismSectionCategoriesOutput;
                Assert.AreSame(calculatorOutput, output);

                AssemblyCategoriesInput actualCalculatorInput = calculator.AssemblyCategoriesInput;
                FailureMechanismContribution expectedContribution = assessmentSection.FailureMechanismContribution;
                Assert.AreEqual(failureMechanism.Contribution / 100, actualCalculatorInput.FailureMechanismContribution, tolerance);
                Assert.AreEqual(expectedContribution.LowerLimitNorm, actualCalculatorInput.LowerLimitNorm, tolerance);
                Assert.AreEqual(expectedContribution.SignalingNorm, actualCalculatorInput.SignalingNorm, tolerance);
                Assert.AreEqual(n, actualCalculatorInput.N);
            }

            mocks.VerifyAll();
        }
    }
}