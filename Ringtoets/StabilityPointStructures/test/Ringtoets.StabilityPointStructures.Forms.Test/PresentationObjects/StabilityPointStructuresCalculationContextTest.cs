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

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;

namespace Ringtoets.StabilityPointStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityPointStructuresCalculationContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocksRepository = new MockRepository();
            var assessmentSection = mocksRepository.Stub<IAssessmentSection>();
            mocksRepository.ReplayAll();

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new StabilityPointStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<StructuresCalculationContext<StabilityPointStructuresInput, StabilityPointStructuresFailureMechanism>>(context);
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

            var calculation = new StructuresCalculation<StabilityPointStructuresInput>();
            var failureMechanism = new StabilityPointStructuresFailureMechanism();
            var parent = new CalculationGroup();
            var context = new StabilityPointStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var derivedContext = new DerivedStabilityPointStructuresCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(derivedContext);

            // Assert
            Assert.IsFalse(isEqual);
            mocksRepository.VerifyAll();
        }

        private class DerivedStabilityPointStructuresCalculationContext : StabilityPointStructuresCalculationContext
        {
            public DerivedStabilityPointStructuresCalculationContext(StructuresCalculation<StabilityPointStructuresInput> calculation,
                                                                     CalculationGroup parent,
                                                                     StabilityPointStructuresFailureMechanism failureMechanism,
                                                                     IAssessmentSection assessmentSection)
                : base(calculation, parent, failureMechanism, assessmentSection) {}
        }
    }
}