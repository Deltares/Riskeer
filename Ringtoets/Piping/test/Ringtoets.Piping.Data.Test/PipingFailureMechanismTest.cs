using System;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
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
            CollectionAssert.IsEmpty(piping.StochasticSoilModels);
            Assert.IsInstanceOf<ObservableList<StochasticSoilModel>>(piping.StochasticSoilModels);
            Assert.AreEqual("Berekeningen", piping.CalculationsGroup.Name);
            Assert.AreEqual(1, piping.CalculationsGroup.Children.Count);
            Assert.IsInstanceOf<PipingCalculation>(piping.CalculationsGroup.Children.First());
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
        public void AddSection_SectionIsNull_ThrowArgumentNullException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            // Call
            TestDelegate call = () => failureMechanism.AddSection(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void AddSection_SectionValid_SectionAddedSectionResults()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });

            // Precondition
            Assert.AreEqual(0, failureMechanism.PipingFailureMechanismSectionResults.Count);

            // Call
            failureMechanism.AddSection(section);

            // Assert
            Assert.AreEqual(1, failureMechanism.PipingFailureMechanismSectionResults.Count);
        }

        [Test]
        public void AddSection_SecondSectionDoesNotConnectToFirst_ThrowArgumentException()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var section1 = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
                
            });
            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(5, 6),
                new Point2D(7, 8)
            });

            failureMechanism.AddSection(section1);

            // Call
            TestDelegate call = () => failureMechanism.AddSection(section2);

            // Assert
            string expectedMessage = "Vak 'B' sluit niet aan op de al gedefinieerde vakken van het faalmechanisme.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void PipingFailureMechanismSectionResults_Always_ReturnsPipingFailureMechanismSectionResults()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });

            var section2 = new FailureMechanismSection("B", new[]
            {
                new Point2D(3, 4),
                new Point2D(7, 8)
            });

            failureMechanism.AddSection(section);
            failureMechanism.AddSection(section2);

            // Call
            var data = failureMechanism.PipingFailureMechanismSectionResults;

            // Assert
            Assert.AreEqual(2, data.Count);
            Assert.AreEqual(section, data[0].Section);
            Assert.AreEqual(section2, data[1].Section);
        }
    }
}