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
            var soilProfiles = new[]
            {
                new TestPipingSoilProfile()
            };

            var mocks = new MockRepository();
            var assessmentSectionBase = mocks.StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            var groupContext = new PipingCalculationGroupContext(group, surfaceLines, soilProfiles, pipingFailureMechanismMock, assessmentSectionBase);

            // Assert
            Assert.IsInstanceOf<IObservable>(groupContext);
            Assert.IsInstanceOf<PipingContext<PipingCalculationGroup>>(groupContext);
            Assert.AreSame(group, groupContext.WrappedData);
            Assert.AreSame(surfaceLines, groupContext.AvailablePipingSurfaceLines);
            Assert.AreSame(soilProfiles, groupContext.AvailablePipingSoilProfiles);
            Assert.AreSame(pipingFailureMechanismMock, groupContext.PipingFailureMechanism);
            Assert.AreSame(assessmentSectionBase, groupContext.AssessmentSection);
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
            var soilProfiles = new[]
            {
                new TestPipingSoilProfile()
            };

            var mocks = new MockRepository();
            var assessmentSectionBase = mocks.StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingCalculationGroupContext(group, surfaceLines, soilProfiles, null, assessmentSectionBase);

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
            var soilProfiles = new[]
            {
                new TestPipingSoilProfile()
            };

            var mocks = new MockRepository();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            // Call
            TestDelegate call = () => new PipingCalculationGroupContext(group, surfaceLines, soilProfiles, pipingFailureMechanismMock, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Het traject mag niet 'null' zijn.");

            mocks.VerifyAll();
        }

        [Test]
        public void NotifyObservers_HasPipingCalculationAndObserverAttached_NotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var assessmentSectionBase = new MockRepository().StrictMock<AssessmentSectionBase>();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationGroupContext(new PipingCalculationGroup(),
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingSoilProfile>(),
                                                                       pipingFailureMechanismMock, assessmentSectionBase);
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
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBase = new MockRepository().StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var presentationObject = new PipingCalculationGroupContext(new PipingCalculationGroup(),
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingSoilProfile>(),
                                                                       pipingFailureMechanismMock, assessmentSectionBase);
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
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var assessmentSectionBase = new MockRepository().StrictMock<AssessmentSectionBase>();
            mocks.ReplayAll();

            var group = new PipingCalculationGroup();
            var presentationObject = new PipingCalculationGroupContext(group,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingSoilProfile>(),
                                                                       pipingFailureMechanismMock, assessmentSectionBase);
            presentationObject.Attach(observer);

            // Call
            group.NotifyObservers();

            // Assert
            mocks.VerifyAll();
        }
    }
}