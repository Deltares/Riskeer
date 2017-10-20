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

using System;
using Core.Common.Base;
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;

namespace Ringtoets.MacroStabilityInwards.Data.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var calculation = new MacroStabilityInwardsCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<CloneableObservable>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);

            Assert.IsInstanceOf<MacroStabilityInwardsInput>(calculation.InputParameters);

            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments.Body);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.SemiProbabilisticOutput);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculation();

            calculation.Attach(observer);

            // Call & Assert
            calculation.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mockRepository.ReplayAll();

            var calculation = new MacroStabilityInwardsCalculation();

            calculation.Attach(observer);
            calculation.Detach(observer);

            // Call & Assert
            calculation.NotifyObservers();
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

            var calculation = new MacroStabilityInwardsCalculation();

            calculation.Attach(observerA);
            calculation.Attach(observerB);

            // Call & Assert
            calculation.NotifyObservers();
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

            var calculation = new MacroStabilityInwardsCalculation();

            calculation.Attach(observerA);
            calculation.Attach(observerB);
            calculation.Detach(observerA);

            // Call & Assert
            calculation.NotifyObservers();
            mockRepository.VerifyAll();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_ThrowsNoException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            var calculation = new MacroStabilityInwardsCalculation();

            // Call & Assert
            calculation.Detach(observer);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputAndSemiProbabilisticOutputToNull()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput(),
                SemiProbabilisticOutput = MacroStabilityInwardsSemiProbabilisticOutputTestFactory.CreateOutput()
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.SemiProbabilisticOutput);
        }

        [Test]
        public void HasOutput_OutputAndSemiProbabilisticOutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation
            {
                Output = null,
                SemiProbabilisticOutput = null
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsFalse(calculationHasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation
            {
                Output = MacroStabilityInwardsOutputTestFactory.CreateOutput()
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(calculationHasOutput);
        }

        [Test]
        public void HasOutput_SemiProbabilisticOutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new MacroStabilityInwardsCalculation
            {
                SemiProbabilisticOutput = MacroStabilityInwardsSemiProbabilisticOutputTestFactory.CreateOutput()
            };

            // Call
            bool calculationHasOutput = calculation.HasOutput;

            // Assert
            Assert.IsTrue(calculationHasOutput);
        }

        [Test]
        public void Clone_NotAllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            MacroStabilityInwardsCalculation original = CreateRandomCalculationWithoutOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        [Test]
        public void Clone_AllPropertiesSet_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            MacroStabilityInwardsCalculation original = CreateRandomCalculationWithoutOutput();
            original.SemiProbabilisticOutput = MacroStabilityInwardsSemiProbabilisticOutputTestFactory.CreateOutput();
            original.Output = MacroStabilityInwardsOutputTestFactory.CreateOutput();

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, MacroStabilityInwardsCloneAssert.AreClones);
        }

        private static MacroStabilityInwardsCalculation CreateRandomCalculationWithoutOutput()
        {
            var random = new Random(21);

            return new MacroStabilityInwardsCalculation
            {
                Name = "A Name",
                Comments =
                {
                    Body = "A comment"
                },
                InputParameters =
                {
                    SlipPlaneMinimumDepth = random.NextRoundedDouble(),
                    SlipPlaneMinimumLength = random.NextRoundedDouble(),
                    MaximumSliceWidth = random.NextRoundedDouble(),
                    MoveGrid = random.NextBoolean()
                }
            };
        }
    }
}