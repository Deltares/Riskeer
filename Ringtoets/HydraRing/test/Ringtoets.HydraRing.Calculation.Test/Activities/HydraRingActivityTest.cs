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

using System;
using Core.Common.Base.Service;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Activities;

namespace Ringtoets.HydraRing.Calculation.Test.Activities
{
    [TestFixture]
    public class HydraRingActivityTest
    {
        [Test]
        public void Run_ValidationFuncReturnsFalse_PerformsValidationAndStateFailed()
        {
            // Setup
            TestHydraRingActivity activity = new TestHydraRingActivity(false, null);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_ValidationFuncReturnsTrue_PerformsValidationAndCalculation()
        {
            // Setup
            TestHydraRingActivity activity = new TestHydraRingActivity(true, null);

            // Call
            activity.Run();

            // Assert
            Assert.IsNaN(activity.Value);
        }

        [Test]
        public void Run_CalculationFuncReturnsNull_PerformsValidationAndCalculationAndStateFailed()
        {
            // Setup
            TestHydraRingActivity activity = new TestHydraRingActivity(true, () => null);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Failed, activity.State);
        }

        [Test]
        public void Run_CalculationFuncReturnsValue_PerformsValidationAndCalculationAndStateNotExecuted()
        {
            // Setup
            TestHydraRingActivity activity = new TestHydraRingActivity(true, () => 2.0);

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Executed, activity.State);
        }

        [Test]
        public void Finish_StateExecuted_SetsOutput()
        {
            // Setup
            double newValue = 2.0;
            TestHydraRingActivity activity = new TestHydraRingActivity(true, () => newValue);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.AreEqual(newValue, activity.Value);
        }

        [Test]
        public void Finish_OutputNull_DoesNotSetOutput()
        {
            // Setup
            TestHydraRingActivity activity = new TestHydraRingActivity(true, () => null);

            activity.Run();

            // Call
            activity.Finish();

            // Assert
            Assert.IsNaN(activity.Value);
        }

        private class TestHydraRingActivity : HydraRingActivity<object>
        {
            private readonly bool valid;
            private readonly Func<object> calculationFunc;
            private double value = 3.0;

            public TestHydraRingActivity(bool valid, Func<object> calculationFunc)
            {
                this.valid = valid;
                this.calculationFunc = calculationFunc;
            }

            public double Value
            {
                get
                {
                    return value;
                }
            }

            protected override void OnRun()
            {
                PerformRun(() => valid, () => value = double.NaN, calculationFunc);
            }

            protected override void OnFinish()
            {
                PerformFinish(() => value = (double) Output);
            }
        }
    }
}