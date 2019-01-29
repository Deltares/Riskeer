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
using System.Collections.Generic;
using NUnit.Framework;
using Riskeer.AssemblyTool.Data;
using Riskeer.AssemblyTool.KernelWrapper.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators;
using Riskeer.AssemblyTool.KernelWrapper.TestUtil.Calculators.Categories;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Forms.PresentationObjects;

namespace Riskeer.Integration.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class MacroStabilityOutwardsAssemblyCategoriesContextTest
    {
        private const double tolerance = 1e-5;

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var random = new Random(21);
            double n = random.NextDouble();

            var failureMechanism = new MacroStabilityOutwardsFailureMechanism
            {
                Contribution = random.NextDouble()
            };

            var assessmentSection = new AssessmentSectionStub();

            using (new AssemblyToolCalculatorFactoryConfig())
            {
                var calculatorFactory = (TestAssemblyToolCalculatorFactory) AssemblyToolCalculatorFactory.Instance;
                AssemblyCategoriesCalculatorStub calculator = calculatorFactory.LastCreatedAssemblyCategoriesCalculator;

                // Call
                var context = new MacroStabilityOutwardsAssemblyCategoriesContext(failureMechanism,
                                                                                  assessmentSection,
                                                                                  () => n);

                // Assert
                Assert.IsInstanceOf<FailureMechanismAssemblyCategoriesContextBase>(context);
                Assert.AreSame(failureMechanism, context.WrappedData);
                Assert.AreSame(assessmentSection, context.AssessmentSection);

                IEnumerable<FailureMechanismSectionAssemblyCategory> output = context.GetFailureMechanismSectionAssemblyCategoriesFunc();
                IEnumerable<FailureMechanismSectionAssemblyCategory> calculatorOutput = calculator.GeotechnicalFailureMechanismSectionCategoriesOutput;
                Assert.AreSame(calculatorOutput, output);

                double normativeNorm = assessmentSection.FailureMechanismContribution.Norm;
                Assert.AreEqual(failureMechanism.Contribution / 100, calculator.FailureMechanismContribution, tolerance);
                Assert.AreEqual(normativeNorm, calculator.NormativeNorm, tolerance);
                Assert.AreEqual(n, calculator.FailureMechanismN);
            }
        }
    }
}