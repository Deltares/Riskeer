using System;
using System.Linq;

using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;

using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.PresentationObjects
{
    [TestFixture]
    public class PipingInputContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());
            var surfaceLines = new[] { new RingtoetsPipingSurfaceLine() };
            var profiles = new[] { new TestPipingSoilProfile() };

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();

            mocks.ReplayAll();
            
            // Call
            var context = new PipingInputContext(pipingInput, surfaceLines, profiles, assessmentSection);

            // Assert
            Assert.IsInstanceOf<PipingContext<PipingInput>>(context);
            Assert.AreSame(pipingInput, context.WrappedData);
            Assert.AreSame(assessmentSection, context.AssessmentSection);
            CollectionAssert.AreEqual(surfaceLines, context.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(profiles, context.AvailablePipingSoilProfiles);

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilProfiles = new[]
            {
                new TestPipingSoilProfile()
            };

            // Call
            TestDelegate call = () => new PipingInputContext(input, surfaceLines, soilProfiles, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het traject mag niet 'null' zijn.");
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToPipingInput()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingInput = new PipingInput(new GeneralPipingInput());
            var context = new PipingInputContext(pipingInput,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                 assessmentSectionMock);

            // Call
            context.Attach(observer);

            // Assert
            pipingInput.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromPipingInput()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var pipingInput = new PipingInput(new GeneralPipingInput());
            var context = new PipingInputContext(pipingInput,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                 assessmentSectionMock);

            context.Attach(observer);

            // Call
            context.Detach(observer);

            // Assert
            pipingInput.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToPipingInput_NotificationCorrectlyPropagated()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingInput = new PipingInput(new GeneralPipingInput());
            var context = new PipingInputContext(pipingInput,
                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                 assessmentSectionMock);

            pipingInput.Attach(observer); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mocks.VerifyAll();
        }
    }
}