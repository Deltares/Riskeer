using System;
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
    public class PipingCalculationContextTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Setup
            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<IObservable>(presentationObject);
            Assert.IsInstanceOf<PipingContext<PipingCalculation>>(presentationObject);
            Assert.AreSame(calculation, presentationObject.WrappedData);
            Assert.AreSame(surfacelines, presentationObject.AvailablePipingSurfaceLines);
            Assert.AreSame(profiles, presentationObject.AvailablePipingSoilProfiles);
            Assert.AreSame(pipingFailureMechanismMock, presentationObject.PipingFailureMechanism);
            Assert.AreSame(assessmentSectionMock, presentationObject.AssessmentSection);
        }

        [Test]
        public void ParameteredConstructor_PipingFailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingCalculationContext(calculation, surfacelines, profiles, null, assessmentSection);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het piping faalmechanisme mag niet 'null' zijn.");
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();
            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het dijktraject mag niet 'null' zijn.");
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock, assessmentSectionBaseMock);
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

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var profiles = new[]
            {
                new TestPipingSoilProfile()
            };
            var calculation = new PipingCalculation();

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationContext(calculation, surfacelines, profiles, pipingFailureMechanismMock, assessmentSectionBaseMock);
            presentationObject.Attach(observer);
            presentationObject.Detach(observer);

            // Call
            presentationObject.NotifyObservers();

            // Assert
            mocks.VerifyAll(); // Expect not calls on 'observer'
        }
    }
}