using System;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Base;

using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            RingtoetsPipingSurfaceLine[] surfaceLines = {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine(),
            };

            PipingSoilProfile[] soilProfiles = {
                new TestPipingSoilProfile(),
                new TestPipingSoilProfile()
            };

            var target = new ObserveableObject();

            // Call
            var context = new SimplePipingContext<ObserveableObject>(target, surfaceLines, soilProfiles);

            // Assert
            Assert.IsInstanceOf<IObservable>(context);
            Assert.AreSame(surfaceLines, context.AvailablePipingSurfaceLines,
                "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(soilProfiles, context.AvailablePipingSoilProfiles,
                "It is vital that the iterator should be identical to the collection, in order to stay in sync when items are added or removed.");
            Assert.AreSame(target, context.WrappedData);
        }

        [Test]
        public void ParameteredConstructor_DataIsNull_ThrowArgumentNullException()
        {
            // Setup

            // Call
            TestDelegate call = () => new SimplePipingContext<ObserveableObject>(null,
                                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                 Enumerable.Empty<PipingSoilProfile>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new [] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("Het piping data object mag niet 'null' zijn.", customMessage);
        }

        [Test]
        public void ParameteredConstructor_SurfaceLinesIsNull_ThrowArgumentNullException()
        {
            // Setup

            // Call
            TestDelegate call = () => new SimplePipingContext<ObserveableObject>(new ObserveableObject(), 
                                                                                 null,
                                                                                 Enumerable.Empty<PipingSoilProfile>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("De verzameling van profielmetingen mag niet 'null' zijn.", customMessage);
        }

        [Test]
        public void ParameteredConstructor_ProfilesIsNull_ThrowArgumentNullException()
        {
            // Setup

            // Call
            TestDelegate call = () => new SimplePipingContext<ObserveableObject>(new ObserveableObject(), 
                                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                 null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            string customMessage = exception.Message.Split(new[] { Environment.NewLine }, StringSplitOptions.None)[0];
            Assert.AreEqual("De verzameling van ondergrondschematiseringen mag niet 'null' zijn.", customMessage);
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var target = new ObserveableObject();

            var presentationObject = new SimplePipingContext<ObserveableObject>(target,
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<PipingSoilProfile>());

            presentationObject.Attach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverDetached_NoCallsOnObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var target = new ObserveableObject();

            var presentationObject = new SimplePipingContext<ObserveableObject>(target,
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<PipingSoilProfile>());
            presentationObject.Attach(observer);
            presentationObject.Detach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect not calls on 'observer'
        }

        [Test]
        public void PipingCalculationNotifyObservers_AttachedOnPipingCalculationContext_ObserverNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var target = new ObserveableObject();

            var presentationObject = new SimplePipingContext<ObserveableObject>(target,
                                                                                Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                Enumerable.Empty<PipingSoilProfile>());
            presentationObject.Attach(observer);

            // Call
            target.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Equals_ToItself_ReturnTrue()
        {
            // Setup
            var observableObject = new ObserveableObject();
            var context = new SimplePipingContext<ObserveableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>());

            // Call
            bool isEqual = context.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
        }

        [Test]
        public void Equals_ToNull_ReturnFalse()
        {
            // Setup
            var observableObject = new ObserveableObject();
            var context = new SimplePipingContext<ObserveableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>());

            // Call
            bool isEqual = context.Equals(null);

            // Assert
            Assert.IsFalse(isEqual);
        }

        [Test]
        public void Equals_ToEqualOtherInstance_ReturnTrue()
        {
            // Setup
            var observableObject = new ObserveableObject();
            var context = new SimplePipingContext<ObserveableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>());

            var otherContext = new SimplePipingContext<ObserveableObject>(observableObject,
                                                                          new[]
                                                                          {
                                                                              new RingtoetsPipingSurfaceLine()
                                                                          },
                                                                          new[]
                                                                          {
                                                                              new TestPipingSoilProfile()
                                                                          });

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsTrue(isEqual);
            Assert.IsTrue(isEqual2);
        }

        [Test]
        public void Equals_ToInequalOtherInstance_ReturnFalse()
        {
            // Setup
            var observableObject = new ObserveableObject();
            var otherObservableObject = new ObserveableObject();
            var context = new SimplePipingContext<ObserveableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>());

            var otherContext = new SimplePipingContext<ObserveableObject>(otherObservableObject,
                                                                          new[]
                                                                          {
                                                                              new RingtoetsPipingSurfaceLine()
                                                                          },
                                                                          new[]
                                                                          {
                                                                              new TestPipingSoilProfile()
                                                                          });

            // Call
            bool isEqual = context.Equals(otherContext);
            bool isEqual2 = otherContext.Equals(context);

            // Assert
            Assert.IsFalse(isEqual);
            Assert.IsFalse(isEqual2);
        }

        [Test]
        public void Equals_ToOtherGenericType_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var observableStub = mocks.Stub<IObservable>();
            mocks.ReplayAll();

            var observableObject = new ObserveableObject();
            var context = new SimplePipingContext<ObserveableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>());

            var otherContext = new SimplePipingContext<IObservable>(observableStub,
                                                                    new[]
                                                                    {
                                                                        new RingtoetsPipingSurfaceLine()
                                                                    },
                                                                    new[]
                                                                    {
                                                                        new TestPipingSoilProfile()
                                                                    });

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
            var observableObject = new ObserveableObject();
            var context = new SimplePipingContext<ObserveableObject>(observableObject,
                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                     Enumerable.Empty<PipingSoilProfile>());

            var otherContext = new SimplePipingContext<ObserveableObject>(observableObject,
                                                                          new[]
                                                                          {
                                                                              new RingtoetsPipingSurfaceLine()
                                                                          },
                                                                          new[]
                                                                          {
                                                                              new TestPipingSoilProfile()
                                                                          });
            // Precondition
            Assert.True(context.Equals(otherContext));

            // Call
            int contextHashCode = context.GetHashCode();
            int otherContextHashCode = otherContext.GetHashCode();

            // Assert
            Assert.AreEqual(contextHashCode, otherContextHashCode);
        }

        private class SimplePipingContext<T> : PipingContext<T> where T : IObservable
        {
            public SimplePipingContext(T target, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<PipingSoilProfile> soilProfiles)
                : base(target, surfaceLines, soilProfiles)
            {
                
            }
        }

        private class ObserveableObject : Observable
        {
            
        }
    }
}