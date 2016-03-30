using System;
using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;

using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

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
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            var presentationObject = new PipingCalculationContext(calculation, surfacelines, soilModels, pipingFailureMechanismMock, assessmentSectionMock);

            // Assert
            Assert.IsInstanceOf<IObservable>(presentationObject);
            Assert.IsInstanceOf<PipingContext<PipingCalculation>>(presentationObject);
            Assert.AreSame(calculation, presentationObject.WrappedData);
            Assert.AreSame(surfacelines, presentationObject.AvailablePipingSurfaceLines);
            Assert.AreSame(soilModels, presentationObject.AvailableStochasticSoilModels);
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
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingCalculationContext(calculation, surfacelines, soilModels, null, assessmentSection);

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
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingCalculationContext(calculation, surfacelines, soilModels, pipingFailureMechanismMock, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het traject mag niet 'null' zijn.");
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToPipingCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var context = new PipingCalculationContext(calculation, surfacelines, soilModels, pipingFailureMechanismMock, assessmentSectionBaseMock);

            // Call
            context.Attach(observer);

            // Assert
            calculation.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromPipingCalculation()
        {
            // Setup
            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var context = new PipingCalculationContext(calculation, surfacelines, soilModels, pipingFailureMechanismMock, assessmentSectionBaseMock);

            context.Attach(observer);

            // Call
            context.Detach(observer);

            // Assert
            calculation.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToPipingCalculation_NotificationCorrectlyPropagated()
        {
            // Setup
            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBaseMock = mocks.StrictMock<AssessmentSectionBase>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var surfacelines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            var context = new PipingCalculationContext(calculation, surfacelines, soilModels, pipingFailureMechanismMock, assessmentSectionBaseMock);

            calculation.Attach(observer); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mocks.VerifyAll();
        }
    }
}