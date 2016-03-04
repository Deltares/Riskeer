using System;
using System.Linq;

using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;

using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

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
        public void NotifyObservers_HasPipingInputAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var presentationObject = new PipingInputContext(new PipingInput(new GeneralPipingInput()),
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingSoilProfile>(),
                                                            assessmentSectionMock);
            presentationObject.Attach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_HasPipingInputAndObserverDetached_NoCallsOnObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var presentationObject = new PipingInputContext(new PipingInput(new GeneralPipingInput()),
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingSoilProfile>(),
                                                            assessmentSectionMock);
            presentationObject.Attach(observer);
            presentationObject.Detach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect not calls on 'observer'
        }

        [Test]
        public void PipingInputNotifyObservers_AttachedOnPipingCalculationContext_ObserverNotified()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingInput = new PipingInput(new GeneralPipingInput());
            var presentationObject = new PipingInputContext(pipingInput,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingSoilProfile>(),
                                                            assessmentSectionMock);
            presentationObject.Attach(observer);

            // Call
            pipingInput.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}