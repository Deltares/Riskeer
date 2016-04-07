using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.TestUtil;
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
    public class PipingCalculationGroupContextTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilModels = new StochasticSoilModel[]
            {
                new TestStochasticSoilModel()
            };

            var mocks = new MockRepository();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var groupContext = new PipingCalculationGroupContext(group, surfaceLines, soilModels, pipingFailureMechanismMock, assessmentSection);

            // Assert
            Assert.IsInstanceOf<IObservable>(groupContext);
            Assert.IsInstanceOf<PipingContext<PipingCalculationGroup>>(groupContext);
            Assert.AreSame(group, groupContext.WrappedData);
            Assert.AreSame(surfaceLines, groupContext.AvailablePipingSurfaceLines);
            Assert.AreSame(soilModels, groupContext.AvailableStochasticSoilModels);
            Assert.AreSame(pipingFailureMechanismMock, groupContext.PipingFailureMechanism);
            Assert.AreSame(assessmentSection, groupContext.AssessmentSection);
        }

        [Test]
        public void ParameteredConstructor_PipingFailureMechanismIsNull_ThrowArgumentNullException()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingCalculationGroupContext(group, surfaceLines, soilModels, null, assessmentSectionMock);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het piping faalmechanisme mag niet 'null' zijn.");

            mocks.VerifyAll();
        }

        [Test]
        public void ParameteredConstructor_AssessmentSectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var surfaceLines = new[]
            {
                new RingtoetsPipingSurfaceLine()
            };
            var soilModels = new[]
            {
                new TestStochasticSoilModel()
            };

            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingCalculationGroupContext(group, surfaceLines, soilModels, pipingFailureMechanismMock, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het traject mag niet 'null' zijn.");

            mocks.VerifyAll();
        }

        [Test]
        public void Attach_Observer_ObserverAttachedToPipingCalculationGroup()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = new MockRepository().StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingCalculationGroup = new PipingCalculationGroup();

            var context = new PipingCalculationGroupContext(pipingCalculationGroup,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            pipingFailureMechanismMock, assessmentSectionMock);

            // Call
            context.Attach(observer);

            // Assert
            pipingCalculationGroup.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected UpdateObserver call
        }

        [Test]
        public void Detach_Observer_ObserverDetachedFromPipingCalculationGroup()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = new MockRepository().StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var observer = mocks.StrictMock<IObserver>();
            mocks.ReplayAll();

            var pipingCalculationGroup = new PipingCalculationGroup();

            var context = new PipingCalculationGroupContext(pipingCalculationGroup,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            pipingFailureMechanismMock, assessmentSectionMock);

            context.Attach(observer);

            // Call
            context.Detach(observer);

            // Assert
            pipingCalculationGroup.NotifyObservers(); // Notification on wrapped object
            mocks.VerifyAll(); // Expected no UpdateObserver call
        }

        [Test]
        public void NotifyObservers_ObserverAttachedToPipingCalculationGroup_NotificationCorrectlyPropagated()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = new MockRepository().StrictMock<IAssessmentSection>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var pipingCalculationGroup = new PipingCalculationGroup();

            var context = new PipingCalculationGroupContext(pipingCalculationGroup,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<StochasticSoilModel>(),
                                                            pipingFailureMechanismMock, assessmentSectionMock);

            pipingCalculationGroup.Attach(observer); // Attach to wrapped object

            // Call
            context.NotifyObservers(); // Notification on context

            // Assert
            mocks.VerifyAll();
        }
    }
}