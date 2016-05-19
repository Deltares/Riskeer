// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.GrassCoverErosionInwards.Data.Properties;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsFailureMechanismTest
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
            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // assert
            Assert.IsInstanceOf<FailureMechanismBase<GrassCoverErosionInwardsFailureMechanismSectionResult>>(grassCoverErosionInwardsFailureMechanism);
            Assert.IsInstanceOf<ICalculatableFailureMechanism>(grassCoverErosionInwardsFailureMechanism);
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayName, grassCoverErosionInwardsFailureMechanism.Name);
            Assert.AreEqual(Resources.GrassCoverErosionInwardsFailureMechanism_DisplayCode, grassCoverErosionInwardsFailureMechanism.Code);
            CollectionAssert.IsEmpty(grassCoverErosionInwardsFailureMechanism.Calculations);
            Assert.IsInstanceOf<GeneralGrassCoverErosionInwardsInput>(grassCoverErosionInwardsFailureMechanism.GeneralInput);
            Assert.IsInstanceOf<GeneralNormProbabilityInput>(grassCoverErosionInwardsFailureMechanism.NormProbabilityInput);
            Assert.AreEqual("Berekeningen", grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Name);
            Assert.IsEmpty(grassCoverErosionInwardsFailureMechanism.CalculationsGroup.Children);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            grassCoverErosionInwardsFailureMechanism.Attach(observer);

            // Call & Assert
            grassCoverErosionInwardsFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mockRepository.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            grassCoverErosionInwardsFailureMechanism.Attach(observer);
            grassCoverErosionInwardsFailureMechanism.Detach(observer);

            // Call & Assert
            grassCoverErosionInwardsFailureMechanism.NotifyObservers();
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

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            grassCoverErosionInwardsFailureMechanism.Attach(observerA);
            grassCoverErosionInwardsFailureMechanism.Attach(observerB);

            // Call & Assert
            grassCoverErosionInwardsFailureMechanism.NotifyObservers();
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

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            grassCoverErosionInwardsFailureMechanism.Attach(observerA);
            grassCoverErosionInwardsFailureMechanism.Attach(observerB);
            grassCoverErosionInwardsFailureMechanism.Detach(observerA);

            // Call & Assert
            grassCoverErosionInwardsFailureMechanism.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_DoesNotThrowException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var grassCoverErosionInwardsFailureMechanism = new GrassCoverErosionInwardsFailureMechanism();

            // Call & Assert
            grassCoverErosionInwardsFailureMechanism.Detach(observer);
            mockRepository.VerifyAll();
        }
    }
}