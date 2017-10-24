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

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;

namespace Ringtoets.HeightStructures.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class HeightStructuresCalculationGroupContextTest
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ParameteredConstructor_ExpectedValues(bool withParent)
        {
            // Setup
            var mockRepository = new MockRepository();
            var assessmentSection = mockRepository.Stub<IAssessmentSection>();
            mockRepository.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();

            CalculationGroup parent = withParent ? new CalculationGroup() : null;

            // Call
            var groupContext = new HeightStructuresCalculationGroupContext(calculationGroup, parent, failureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<FailureMechanismItemContextBase<CalculationGroup, HeightStructuresFailureMechanism>>(groupContext);
            Assert.IsInstanceOf<ICalculationContext<CalculationGroup, HeightStructuresFailureMechanism>>(groupContext);
            Assert.AreSame(calculationGroup, groupContext.WrappedData);
            Assert.AreSame(parent, groupContext.Parent);
            Assert.AreSame(failureMechanism, groupContext.FailureMechanism);
            Assert.AreSame(assessmentSection, groupContext.AssessmentSection);
            Assert.AreSame(failureMechanism.ForeshoreProfiles, groupContext.AvailableForeshoreProfiles);
            Assert.AreSame(failureMechanism.HeightStructures, groupContext.AvailableStructures);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                      parent,
                                                                      failureMechanism,
                                                                      assessmentSection);

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

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                      parent,
                                                                      failureMechanism,
                                                                      assessmentSection);

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

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                      parent,
                                                                      failureMechanism,
                                                                      assessmentSection);

            // Call
            bool isEqual = context.Equals(new object());

            // Assert
            Assert.IsFalse(isEqual);

            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToDerivedType_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                      parent,
                                                                      failureMechanism,
                                                                      assessmentSection);
            var derivedContext = new DerivedHeightStructuresCalculationGroupContext(calculationGroup,
                                                                                    parent,
                                                                                    failureMechanism,
                                                                                    assessmentSection);

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

            var calculationGroup1 = new CalculationGroup();
            var calculationGroup2 = new CalculationGroup();
            var parent = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context1 = new HeightStructuresCalculationGroupContext(calculationGroup1,
                                                                       parent,
                                                                       failureMechanism,
                                                                       assessmentSection);
            var context2 = new HeightStructuresCalculationGroupContext(calculationGroup2,
                                                                       parent,
                                                                       failureMechanism,
                                                                       assessmentSection);

            // Precondition
            Assert.IsFalse(calculationGroup1.Equals(calculationGroup2));

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

            var calculationGroup = new CalculationGroup();
            var parent1 = new CalculationGroup();
            var parent2 = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context1 = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                       parent1,
                                                                       failureMechanism,
                                                                       assessmentSection);
            var context2 = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                       parent2,
                                                                       failureMechanism,
                                                                       assessmentSection);

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

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context1 = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                       parent,
                                                                       failureMechanism,
                                                                       assessmentSection);
            var context2 = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                       parent,
                                                                       failureMechanism,
                                                                       assessmentSection);

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

            var calculationGroup = new CalculationGroup();
            var parent = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var context1 = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                       parent,
                                                                       failureMechanism,
                                                                       assessmentSection);
            var context2 = new HeightStructuresCalculationGroupContext(calculationGroup,
                                                                       parent,
                                                                       failureMechanism,
                                                                       assessmentSection);
            // Precondition
            Assert.AreEqual(context1, context2);

            // Call
            int hashCode1 = context1.GetHashCode();
            int hashCode2 = context2.GetHashCode();

            // Assert
            Assert.AreEqual(hashCode1, hashCode2);

            mocks.VerifyAll();
        }

        private class DerivedHeightStructuresCalculationGroupContext : HeightStructuresCalculationGroupContext
        {
            public DerivedHeightStructuresCalculationGroupContext(CalculationGroup calculationsGroup,
                                                                  CalculationGroup parent,
                                                                  HeightStructuresFailureMechanism failureMechanism,
                                                                  IAssessmentSection assessmentSection)
                : base(calculationsGroup, parent, failureMechanism, assessmentSection) {}
        }
    }
}