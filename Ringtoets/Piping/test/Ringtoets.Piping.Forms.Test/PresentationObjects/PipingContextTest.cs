using System;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Base;

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            RingtoetsPipingSurfaceLine[] surfaceLines = {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine(),
            };

            PipingSoilProfile[] soilProfiles = {
                new TestPipingSoilProfile(),
                new TestPipingSoilProfile()
            };

            var target = new ObservableObject();

            // Call
            var context = new SimplePipingContext<ObservableObject>(target, surfaceLines, soilProfiles, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.AreSame(surfaceLines, context.AvailablePipingSurfaceLines,
                "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(soilProfiles, context.AvailablePipingSoilProfiles,
                "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(target, context.WrappedData);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(null,
                                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                                                 assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new [] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("Het piping data object mag niet 'null' zijn.", customMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_SurfaceLinesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(), 
                                                                                 null,
                                                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                                                 assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("De verzameling van profielschematisaties mag niet 'null' zijn.", customMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_ProfilesIsNull_ThrowArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new SimplePipingContext<ObservableObject>(new ObservableObject(), 
                                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                 null,
                                                                                 assessmentSection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("De verzameling van ondergrondschematiseringen mag niet 'null' zijn.", customMessage);
            mocks.VerifyAll();
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToWrappedObject()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<PipingSoilProfile>(),
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<PipingSoilProfile>(),
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var observableObject = new ObservableObject();

            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<PipingSoilProfile>(),
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>(),
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>(),
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>(),
                                                                     assessmentSection);

            var otherContext = new SimplePipingContext<ObservableObject>(observableObject,
                                                                          new[]
                                                                          {
                                                                              new RingtoetsPipingSurfaceLine()
                                                                          },
                                                                          new[]
                                                                          {
                                                                              new TestPipingSoilProfile()
                                                                          },
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var otherObservableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>(),
                                                                     assessmentSection);

            var otherContext = new SimplePipingContext<ObservableObject>(otherObservableObject,
                                                                          new[]
                                                                          {
                                                                              new RingtoetsPipingSurfaceLine()
                                                                          },
                                                                          new[]
                                                                          {
                                                                              new TestPipingSoilProfile()
                                                                          },
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var observableStub = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>(),
                                                                     assessmentSection);

            var otherContext = new SimplePipingContext<IObservable>(observableStub,
                                                                    new[]
                                                                    {
                                                                        new RingtoetsPipingSurfaceLine()
                                                                    },
                                                                    new[]
                                                                    {
                                                                        new TestPipingSoilProfile()
                                                                    },
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
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var observableObject = new ObservableObject();
            var context = new SimplePipingContext<ObservableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>(),
                                                                     assessmentSection);

            var otherContext = new SimplePipingContext<ObservableObject>(observableObject,
                                                                          new[]
                                                                          {
                                                                              new RingtoetsPipingSurfaceLine()
                                                                          },
                                                                          new[]
                                                                          {
                                                                              new TestPipingSoilProfile()
                                                                          },
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
            public SimplePipingContext(T target, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<PipingSoilProfile> soilProfiles, AssessmentSectionBase assessmentSection)
                : base(target, surfaceLines, soilProfiles, assessmentSection)
            {
                
            }
        }

        private class ObservableObject : Observable
        {
            
        }
    }
}