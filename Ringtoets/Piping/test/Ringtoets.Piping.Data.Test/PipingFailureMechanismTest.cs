using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingFailureMechanismTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var piping = new PipingFailureMechanism();

            // assert
            Assert.IsInstanceOf<BaseFailureMechanism>(piping);
            Assert.IsInstanceOf<GeneralPipingInput>(piping.GeneralInput);
            CollectionAssert.IsEmpty(piping.Sections);
            CollectionAssert.IsEmpty(piping.SurfaceLines);
            Assert.IsInstanceOf<ObservableList<RingtoetsPipingSurfaceLine>>(piping.SurfaceLines);
            CollectionAssert.IsEmpty(piping.SoilProfiles);
            Assert.IsInstanceOf<ObservableList<StochasticSoilModel>>(piping.StochasticSoilModels);
            Assert.AreEqual("Berekeningen", piping.CalculationsGroup.Name);
            Assert.AreEqual(1, piping.CalculationsGroup.Children.Count);
            Assert.IsInstanceOf<PipingCalculation>(piping.CalculationsGroup.Children.First());

            Assert.AreEqual("Oordeel", piping.AssessmentResult.Name);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.Attach(observer);

            // Call & Assert
            pipingFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mockRepository.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.Attach(observer);
            pipingFailureMechanism.Detach(observer);

            // Call & Assert
            pipingFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Notify_TwoListenersAttached_BothAreNotified()
        {
            // Setup
            var observerA = mockRepository.StrictMock<IObserver>();
            observerA.Expect(o => o.UpdateObserver());

            var observerB = mockRepository.StrictMock<IObserver>();
            observerB.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.Attach(observerA);
            pipingFailureMechanism.Attach(observerB);

            // Call & Assert
            pipingFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Notify_TwoListenersAttachedOneDetached_InvokedOnce()
        {
            // Setup
            var observerA = mockRepository.StrictMock<IObserver>();
            observerA.Expect(o => o.UpdateObserver()).Repeat.Never();

            var observerB = mockRepository.StrictMock<IObserver>();
            observerB.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var pipingFailureMechanism = new PipingFailureMechanism();

            pipingFailureMechanism.Attach(observerA);
            pipingFailureMechanism.Attach(observerB);
            pipingFailureMechanism.Detach(observerA);

            // Call & Assert
            pipingFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_ThrowsNoException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();

            var pipingFailureMechanism = new PipingFailureMechanism();

            // Call & Assert
            pipingFailureMechanism.Detach(observer);
        }

        [Test]
        public void Calculations_AddPipingCalculation_ItemIsAddedToCollection()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var failureMechanism = new PipingFailureMechanism();

            // Call
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Assert
            CollectionAssert.Contains(failureMechanism.CalculationsGroup.Children, calculation);
        }

        [Test]
        public void Calculations_RemovePipingCalculation_ItemIsRemovedFromCollection()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Call
            failureMechanism.CalculationsGroup.Children.Remove(calculation);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, calculation);
        }

        [Test]
        public void Calculations_AddPipingCalculationGroup_ItemIsAddedToCollection()
        {
            // Setup
            var folder = new PipingCalculationGroup();

            var failureMechanism = new PipingFailureMechanism();

            // Call
            failureMechanism.CalculationsGroup.Children.Add(folder);

            // Assert
            CollectionAssert.Contains(failureMechanism.CalculationsGroup.Children, folder);
        }

        [Test]
        public void Calculations_RemovePipingCalculationGroup_ItemIsRemovedFromCollection()
        {
            // Setup
            var folder = new PipingCalculationGroup();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(folder);

            // Call
            failureMechanism.CalculationsGroup.Children.Remove(folder);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, folder);
        }

        [Test]
        public void SoilProfiles_StochasticSoilModelsWithDuplicateProfiles_ReturnsDistinctProfiles()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            PipingSoilProfile profile = new TestPipingSoilProfile();
            StochasticSoilProfile stochasticProfile = new StochasticSoilProfile(0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = profile
            };
            failureMechanism.StochasticSoilModels.AddRange(new[]
            {
                new StochasticSoilModel(0, string.Empty, string.Empty)
                {
                    StochasticSoilProfiles =
                    {
                        stochasticProfile
                    }
                }, 
                new StochasticSoilModel(0, string.Empty, string.Empty)
                {
                    StochasticSoilProfiles =
                    {
                        stochasticProfile
                    }
                }
            });

            // Call
            var profiles = failureMechanism.SoilProfiles;

            // Assert
            Assert.AreEqual(1, profiles.Length);
            Assert.AreSame(profile, profiles.First());
        }
    }
}