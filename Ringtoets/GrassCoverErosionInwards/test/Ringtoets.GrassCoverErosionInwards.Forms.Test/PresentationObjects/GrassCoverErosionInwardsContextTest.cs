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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class GrassCoverErosionInwardsContextTest
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
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var target = new ObservableObject();

            // Call
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(target, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);

            Assert.AreSame(target, context.WrappedData);

            mockRepository.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            // Call
            TestDelegate call = () => new SimpleGrassCoverErosionInwardsContext<ObservableObject>(null,
                                                                                                  assessmentSectionMock);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("Wrapped data of context cannot be null.", customMessage);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToWrappedObject()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);

            // Call
            context.Attach(observerMock);

            // Assert
            observableObject.NotifyObservers(); // Notification on wrapped object
            mockRepository.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromWrappedObject()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var observerMock = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);
            // Precondition
            context.Attach(observerMock);

            // Call
            context.Detach(observerMock);

            // Assert
            observableObject.NotifyObservers(); // Notification on wrapped object
            mockRepository.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToWrappedObject_NotificationCorrectlyPropagated()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);

            // Precondition
            observableObject.Attach(observerMock); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mockRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);

            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToEqualOtherInstance_ReturnTrue()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);

            var otherContext = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                           assessmentSectionMock);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            Assert.IsTrue(isEqual2);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToInequalOtherInstance_ReturnFalse()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();
            var otherObservableObject = new ObservableObject();
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);

            var otherContext = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(otherObservableObject,
                                                                                           assessmentSectionMock);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsFalse(isEqual);
            Assert.IsFalse(isEqual2);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherGenericType_ReturnFalse()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            var observableStub = mockRepository.Stub<IObservable>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);

            var otherContext = new SimpleGrassCoverErosionInwardsContext<IObservable>(observableStub,
                                                                                      assessmentSectionMock);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsFalse(isEqual);
            Assert.IsFalse(isEqual2);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetHashCode_TwoContextInstancesEqualToEachOther_ReturnIdenticalHashes()
        {
            // Setup
            var assessmentSectionMock = mockRepository.StrictMock<IAssessmentSection>();
            mockRepository.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                      assessmentSectionMock);

            var otherContext = new SimpleGrassCoverErosionInwardsContext<ObservableObject>(observableObject,
                                                                                           assessmentSectionMock);
            // Precondition
            Assert.True(context.Equals(otherContext));

            // Call
            int contextHashCode = context.GetHashCode();
            int otherContextHashCode = otherContext.GetHashCode();

            // Assert
            Assert.AreEqual(contextHashCode, otherContextHashCode);
            mockRepository.VerifyAll();
        }

        private class ObservableObject : Observable {}

        private class SimpleGrassCoverErosionInwardsContext<T> : GrassCoverErosionInwardsContext<T> where T : IObservable
        {
            public SimpleGrassCoverErosionInwardsContext(T target, IAssessmentSection assessmentSection)
                : base(target, assessmentSection) {}
        }
    }
}