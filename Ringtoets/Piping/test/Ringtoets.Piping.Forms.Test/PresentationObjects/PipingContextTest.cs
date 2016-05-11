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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine[] surfaceLines =
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine(),
            };

            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var target = new ObservableObject();

            // Call
            var context = new SimplePipingContext<ObservableObject>(target, surfaceLines, soilModels, pipingFailureMechanism, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.AreSame(surfaceLines, context.AvailablePipingSurfaceLines,
                           "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(soilModels, context.AvailableStochasticSoilModels,
                           "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(target, context.WrappedData);
            Assert.AreSame(pipingFailureMechanism, context.FailureMechanism);
            Assert.AreSame(assessmentSection, context.AssessmentSection);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(null,
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanism,
                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("Wrapped data of context cannot be null.", customMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_SurfaceLinesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                null,
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanism,
                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("De verzameling van profielschematisaties mag niet 'null' zijn.", customMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ProfilesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                null,
                                                                                pipingFailureMechanism,
                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("De verzameling van ondergrondschematisaties mag niet 'null' zijn.", customMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_PipingFailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                null,
                                                                                assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("Het piping toetsspoor mag niet 'null' zijn.", customMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(),
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<StochasticSoilModel>(),
                                                                                pipingFailureMechanism,
                                                                                null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[]
            {
                Environment.NewLine
            }, StringSplitOptions.None)[0];
            Assert.AreEqual("Het traject mag niet 'null' zijn.", customMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToWrappedObject()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            // Call
            context.Attach(observer);

            // Assert
            observableObject.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromWrappedObject()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            context.Attach(observer);

            // Call
            context.Detach(observer);

            // Assert
            observableObject.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToWrappedObject_NotificationCorrectlyPropagated()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            observableObject.Attach(observer); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToEqualOtherInstance_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            var otherContext = new SimplePipingContext<ObservableObject>(observableObject,
                                                                         new[]
                                                                         {
                                                                             new RingtoetsPipingSurfaceLine()
                                                                         },
                                                                         new[]
                                                                         {
                                                                             new StochasticSoilModel(0, string.Empty, string.Empty)
                                                                         },
                                                                         pipingFailureMechanism,
                                                                         assessmentSection);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            Assert.IsTrue(isEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToInequalOtherInstance_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var otherObservableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            var otherContext = new SimplePipingContext<ObservableObject>(otherObservableObject,
                                                                         new[]
                                                                         {
                                                                             new RingtoetsPipingSurfaceLine()
                                                                         },
                                                                         new[]
                                                                         {
                                                                             new StochasticSoilModel(0, string.Empty, string.Empty)
                                                                         },
                                                                         pipingFailureMechanism,
                                                                         assessmentSection);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsFalse(isEqual);
            Assert.IsFalse(isEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToOtherGenericType_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            var observableStub = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            var otherContext = new SimplePipingContext<IObservable>(observableStub,
                                                                    new[]
                                                                    {
                                                                        new RingtoetsPipingSurfaceLine()
                                                                    },
                                                                    new[]
                                                                    {
                                                                        new StochasticSoilModel(0, string.Empty, string.Empty)
                                                                    },
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsFalse(isEqual);
            Assert.IsFalse(isEqual2);
            mocks.VerifyAll();
        }

        [Test]
        public void GetHashCode_TwoContextInstancesEqualToEachOther_ReturnIdenticalHashes()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<StochasticSoilModel>(),
                                                                    pipingFailureMechanism,
                                                                    assessmentSection);

            var otherContext = new SimplePipingContext<ObservableObject>(observableObject,
                                                                         new[]
                                                                         {
                                                                             new RingtoetsPipingSurfaceLine()
                                                                         },
                                                                         new[]
                                                                         {
                                                                             new StochasticSoilModel(0, string.Empty, string.Empty)
                                                                         },
                                                                         pipingFailureMechanism,
                                                                         assessmentSection);
            // Precondition
            Assert.True(context.Equals(otherContext));

            // Call
            int contextHashCode = context.GetHashCode();
            int otherContextHashCode = otherContext.GetHashCode();

            // Assert
            Assert.AreEqual(contextHashCode, otherContextHashCode);
            mocks.VerifyAll();
        }

        private class SimplePipingContext<T> : PipingContext<T> where T : IObservable
        {
            public SimplePipingContext(T target, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> stochasticSoilModels, PipingFailureMechanism pipingFailureMechanism, IAssessmentSection assessmentSection)
                : base(target, surfaceLines, stochasticSoilModels, pipingFailureMechanism, assessmentSection) {}
        }

        private class ObservableObject : Observable {}
    }
}