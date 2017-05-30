// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsFailureMechanismTest
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
            // Call
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Assert
            Assert.IsInstanceOf<FailureMechanismBase>(failureMechanism);
            Assert.IsInstanceOf<ICalculatableFailureMechanism>(failureMechanism);
            Assert.IsInstanceOf<IHasSectionResults<MacroStabilityInwardsFailureMechanismSectionResult>>(failureMechanism);
            Assert.AreEqual("Dijken en dammen - Macrostabiliteit binnenwaarts", failureMechanism.Name);
            Assert.AreEqual("STBI", failureMechanism.Code);

            Assert.IsInstanceOf<GeneralMacroStabilityInwardsInput>(failureMechanism.GeneralInput);

            Assert.AreEqual("Berekeningen", failureMechanism.CalculationsGroup.Name);
            Assert.IsFalse(failureMechanism.CalculationsGroup.IsNameEditable);
            CollectionAssert.IsEmpty(failureMechanism.CalculationsGroup.Children);
            CollectionAssert.IsEmpty(failureMechanism.Sections);
            CollectionAssert.IsEmpty(failureMechanism.SurfaceLines);
            CollectionAssert.IsEmpty(failureMechanism.StochasticSoilModels);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            failureMechanism.Attach(observer);

            // Call & Assert
            failureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mockRepository.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            failureMechanism.Attach(observer);
            failureMechanism.Detach(observer);

            // Call & Assert
            failureMechanism.NotifyObservers();
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            failureMechanism.Attach(observerA);
            failureMechanism.Attach(observerB);

            // Call & Assert
            failureMechanism.NotifyObservers();
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

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            failureMechanism.Attach(observerA);
            failureMechanism.Attach(observerB);
            failureMechanism.Detach(observerA);

            // Call & Assert
            failureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_ThrowsNoException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call & Assert
            failureMechanism.Detach(observer);
            mockRepository.VerifyAll();
        }

        [Test]
        public void Calculations_AddCalculation_ItemIsAddedToCollection()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Assert
            CollectionAssert.Contains(failureMechanism.CalculationsGroup.Children, calculation);
        }

        [Test]
        public void Calculations_RemoveCalculation_ItemIsRemovedFromCollection()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation(new GeneralMacroStabilityInwardsInput());
            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            failureMechanism.CalculationsGroup.Children.Add(calculation);

            // Call
            failureMechanism.CalculationsGroup.Children.Remove(calculation);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, calculation);
        }

        [Test]
        public void Calculations_AddCalculationGroup_ItemIsAddedToCollection()
        {
            // Setup
            var folder = new CalculationGroup();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();

            // Call
            failureMechanism.CalculationsGroup.Children.Add(folder);

            // Assert
            CollectionAssert.Contains(failureMechanism.CalculationsGroup.Children, folder);
        }

        [Test]
        public void Calculations_RemoveCalculationGroup_ItemIsRemovedFromCollection()
        {
            // Setup
            var folder = new CalculationGroup();

            var failureMechanism = new MacroStabilityInwardsFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(folder);

            // Call
            failureMechanism.CalculationsGroup.Children.Remove(folder);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, folder);
        }
    }
}