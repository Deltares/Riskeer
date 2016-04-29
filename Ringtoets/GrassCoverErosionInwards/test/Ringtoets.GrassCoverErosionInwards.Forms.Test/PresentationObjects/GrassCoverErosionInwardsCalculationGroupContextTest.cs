// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationGroupContextTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup, failureMechanismMock, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<IObservable>(groupContext);
            Assert.IsInstanceOf<GrassCoverErosionInwardsContext<CalculationGroup>>(groupContext);
            Assert.AreSame(calculationGroup, groupContext.WrappedData);
            Assert.AreSame(failureMechanismMock, groupContext.GrassCoverErosionInwardsFailureMechanism);
            Assert.AreSame(assessmentSectionMock, groupContext.AssessmentSection);
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_FailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup, null, assessmentSectionMock);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het grasbekleding erosie kruin en binnentalud faalmechanisme mag niet 'null' zijn.");
            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var calculationGroup = new CalculationGroup();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup, failureMechanismMock, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het traject mag niet 'null' zijn.");
            mockRepository.VerifyAll();
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToCalculationGroup()
        {
            // Setup
            var assessmentSectionMock = new MockRepository().StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                              failureMechanismMock, assessmentSectionMock);

            // Call
            context.Attach(observerMock);

            // Assert
            calculationGroup.NotifyObservers(); // Notification on wrapped object
            mockRepository.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromCalculationGroup()
        {
            // Setup
            var assessmentSectionMock = new MockRepository().StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var observer = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                              failureMechanismMock, assessmentSectionMock);

            // Precondition
            context.Attach(observer);

            // Call
            context.Detach(observer);

            // Assert
            calculationGroup.NotifyObservers(); // Notification on wrapped object
            mockRepository.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToCalculationGroup_NotificationCorrectlyPropagated()
        {
            // Setup
            var assessmentSectionMock = new MockRepository().StrictMock<IAssessmentSection>();
            var failureMechanismMock = mockRepository.StrictMock<GrassCoverErosionInwardsFailureMechanism>();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var calculationGroup = new CalculationGroup();

            var context = new GrassCoverErosionInwardsCalculationGroupContext(calculationGroup,
                                                                              failureMechanismMock, assessmentSectionMock);

            calculationGroup.Attach(observer); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mockRepository.VerifyAll();
        }
    }
}