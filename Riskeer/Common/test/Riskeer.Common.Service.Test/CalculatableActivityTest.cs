// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Data.Calculation;

namespace Riskeer.Common.Service.Test
{
    [TestFixture]
    public class CalculatableActivityTest
    {
        [Test]
        public void Constructor_ValidParameter_ExpectedValues()
        {
            // Call
            var activity = new TestCalculatableActivity(new TestCalculatable(true));

            // Assert
            Assert.IsInstanceOf<Activity>(activity);
        }

        [Test]
        public void Constructor_CalculatableNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TestCalculatableActivity(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("calculatable", exception.ParamName);
        }

        [Test]
        public void Run_ShouldNotCalculate_StateSkippedAndCalculatedFalse()
        {
            // Setup
            var activity = new TestCalculatableActivity(new TestCalculatable(false));

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Skipped, activity.State);
            Assert.IsFalse(activity.Calculated);
        }

        [Test]
        public void Run_ShouldCalculateAndIsNotValid_StateFailedAndCalculatedFalse()
        {
            // Setup
            var activity = new TestCalculatableActivity(new TestCalculatable(true));

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Failed, activity.State);
            Assert.IsFalse(activity.Calculated);
        }

        [Test]
        public void Run_ShouldCalculateAndIsValid_StateExecutedAndCalculatedTrue()
        {
            // Setup
            var activity = new TestCalculatableActivity(new TestCalculatable(true))
            {
                IsValid = true
            };

            // Call
            activity.Run();

            // Assert
            Assert.AreEqual(ActivityState.Executed, activity.State);
            Assert.IsTrue(activity.Calculated);
        }

        [Test]
        public void UpdateProgressText_Always_SetsProgressTextWithFormat()
        {
            // Setup
            var activity = new TestCalculatableActivity(new TestCalculatable(true));
            const string currentStepName = "Some step name.";
            int totalStep = new Random(21).Next();
            int currentStep = new Random(21).Next();

            // Call
            activity.PublicUpdateProgressText(currentStepName, currentStep, totalStep);

            // Assert
            Assert.AreEqual($"Stap {currentStep} van {totalStep} | {currentStepName}", activity.ProgressText);
        }

        private class TestCalculatable : ICalculatable
        {
            public TestCalculatable(bool shouldCalculate)
            {
                ShouldCalculate = shouldCalculate;
            }

            public bool ShouldCalculate { get; }
        }

        private class TestCalculatableActivity : CalculatableActivity
        {
            public TestCalculatableActivity(ICalculatable calculatable) : base(calculatable) {}

            public bool Calculated { get; private set; }

            public bool IsValid { private get; set; }

            public void PublicUpdateProgressText(string currentStepName, int currentStep, int totalStep)
            {
                UpdateProgressText(currentStepName, currentStep, totalStep);
            }

            protected override void OnCancel() {}

            protected override void OnFinish() {}

            protected override void PerformCalculation()
            {
                Calculated = true;
            }

            protected override bool Validate()
            {
                return IsValid;
            }
        }
    }
}