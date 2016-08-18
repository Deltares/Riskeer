﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Piping.KernelWrapper.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingCalculationTest
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
            // Setup
            var generalInputParameters = new GeneralPipingInput();

            // Call
            var calculation = new PipingCalculation(generalInputParameters);

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);

            Assert.IsInstanceOf<PipingInput>(calculation.InputParameters);

            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments);
            Assert.IsNull(calculation.Output);
            Assert.IsNull(calculation.SemiProbabilisticOutput);
        }

        [Test]
        public void Constructor_GeneralPipingInputIsNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingCalculation(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            mockRepository.ReplayAll();

            var calculation = new PipingCalculation(new GeneralPipingInput());

            calculation.Attach(observer);

            // Call & Assert
            calculation.NotifyObservers();
        }

        [Test]
        public void Notify_SingleListenerAttachedAndDeattached_ListenerIsNotNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            mockRepository.ReplayAll();

            var calculation = new PipingCalculation(new GeneralPipingInput());

            calculation.Attach(observer);
            calculation.Detach(observer);

            // Call & Assert
            calculation.NotifyObservers();
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

            var calculation = new PipingCalculation(new GeneralPipingInput());

            calculation.Attach(observerA);
            calculation.Attach(observerB);

            // Call & Assert
            calculation.NotifyObservers();
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

            var calculation = new PipingCalculation(new GeneralPipingInput());

            calculation.Attach(observerA);
            calculation.Attach(observerB);
            calculation.Detach(observerA);

            // Call & Assert
            calculation.NotifyObservers();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_ThrowsNoException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            var calculation = new PipingCalculation(new GeneralPipingInput());

            // Call & Assert
            calculation.Detach(observer);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };

            // Call
            calculation.ClearOutput();

            // Assert
            Assert.IsNull(calculation.Output);
        }

        [Test]
        public void HasOutput_OutputNull_ReturnsFalse()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = null
            };

            // Call & Assert
            Assert.IsFalse(calculation.HasOutput);
        }

        [Test]
        public void HasOutput_OutputSet_ReturnsTrue()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = new TestPipingOutput()
            };

            // Call & Assert
            Assert.IsTrue(calculation.HasOutput);
        }

        [Test]
        public void GetObservableInput_Always_ReturnsInputParameters()
        {
            // Setup
            var calculation = new PipingCalculation(new GeneralPipingInput());
            var inputParameters = calculation.InputParameters;

            // Call
            ICalculationInput input = calculation.GetObservableInput();

            // Assert
            Assert.AreSame(inputParameters, input);
        }

        [Test]
        public void GetObservableOutput_Always_ReturnsOutput()
        {
            // Setup
            var output = new PipingOutput(2.0, 3.0, 1.4, 50.3, 16.3, 58.2);
            var calculation = new PipingCalculation(new GeneralPipingInput())
            {
                Output = output
            };

            // Call
            ICalculationOutput calculationOutput = calculation.GetObservableOutput();

            // Assert
            Assert.AreSame(output, calculationOutput);
        }
    }
}