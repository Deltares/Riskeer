// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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

using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.HeightStructures.Data;
using Riskeer.HeightStructures.Forms.PresentationObjects;

namespace Riskeer.HeightStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HeightStructuresCalculationScenarioContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new StructuresCalculationScenario<HeightStructuresInput>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new HeightStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<StructuresCalculationScenarioContext<HeightStructuresInput, HeightStructuresFailureMechanism>>(context);
            Assert.AreSame(calculation, context.WrappedData);
            Assert.AreSame(parent, context.Parent);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocksRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToDerivedObject_ReturnsFalse()
        {
            // Setup  
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new StructuresCalculationScenario<HeightStructuresInput>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var context = new HeightStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);
            var derivedContext = new DerivedHeightStructuresCalculationScenarioContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(derivedContext);

            // Assert
            Assert.IsFalse(isEqual);
            mocksRepository.VerifyAll();
        }

        private class DerivedHeightStructuresCalculationScenarioContext : HeightStructuresCalculationScenarioContext
        {
            public DerivedHeightStructuresCalculationScenarioContext(StructuresCalculationScenario<HeightStructuresInput> calculation,
                                                                     CalculationGroup parent,
                                                                     HeightStructuresFailureMechanism failureMechanism,
                                                                     IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }
    }
}