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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Is = Rhino.Mocks.Constraints.Is;

namespace Ringtoets.StabilityStoneCover.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationContextTest
    {
        [Test]
        public void ConstructorWithData_Always_ExpectedPropertiesSet()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();

            // Call
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<StabilityStoneCoverContext<StabilityStoneCoverWaveConditionsCalculation>>(context);
            Assert.IsInstanceOf<ICalculationContext<StabilityStoneCoverWaveConditionsCalculation, StabilityStoneCoverFailureMechanism>>(context);
            Assert.AreSame(calculation, context.WrappedData);
            Assert.AreSame(parent, context.Parent);
            Assert.AreSame(failureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ParentNull_ThrowsArgumentNullException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            // Call
            TestDelegate call = () => new StabilityStoneCoverWaveConditionsCalculationContext(calculation, null, failureMechanism, assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("parent", exception.ParamName);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentType_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(new object());

            // Assert
            Assert.IsFalse(isEqual);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToDerivedObject_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var derivedContext = new DerivedStabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual = context.Equals(derivedContext);

            // Assert
            Assert.IsFalse(isEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentWrappedData_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation1 = new StabilityStoneCoverWaveConditionsCalculation();
            var calculation2 = new StabilityStoneCoverWaveConditionsCalculation();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new StabilityStoneCoverWaveConditionsCalculationContext(calculation1, parent, failureMechanism, assessmentSection);
            var context2 = new StabilityStoneCoverWaveConditionsCalculationContext(calculation2, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithDifferentParent_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation1 = new StabilityStoneCoverWaveConditionsCalculation();
            var calculation2 = new StabilityStoneCoverWaveConditionsCalculation();

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent1 = new CalculationGroup();
            var parent2 = new CalculationGroup();
            var context1 = new StabilityStoneCoverWaveConditionsCalculationContext(calculation1, parent1, failureMechanism, assessmentSection);
            var context2 = new StabilityStoneCoverWaveConditionsCalculationContext(calculation2, parent2, failureMechanism, assessmentSection);

            // Precondition
            Assert.IsFalse(parent1.Equals(parent2));

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsFalse(isEqual1);
            Assert.IsFalse(isEqual2);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherWithSameWrappedDataAndParent_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var context2 = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Call
            bool isEqual1 = context1.Equals(context2);
            bool isEqual2 = context2.Equals(context1);

            // Assert
            Assert.IsTrue(isEqual1);
            Assert.IsTrue(isEqual2);

            mocks.VerifyAll();
        }

        [Test]
        public void GetHashCode_EqualObjects_ReturnSameHashCode()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var context1 = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);
            var context2 = new StabilityStoneCoverWaveConditionsCalculationContext(calculation, parent, failureMechanism, assessmentSection);

            // Precondition
            Assert.AreEqual(context1, context2);

            // Call
            int hashCode1 = context1.GetHashCode();
            int hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);

            mocks.VerifyAll();
        }

        private class DerivedStabilityStoneCoverWaveConditionsCalculationContext : StabilityStoneCoverWaveConditionsCalculationContext
        {
            public DerivedStabilityStoneCoverWaveConditionsCalculationContext(StabilityStoneCoverWaveConditionsCalculation wrappedData,
                                                                              CalculationGroup parent,
                                                                              StabilityStoneCoverFailureMechanism failureMechanism,
                                                                              IAssessmentSection assessmentSection)
                : base(wrappedData, parent, failureMechanism, assessmentSection) {}
        }
    }
}