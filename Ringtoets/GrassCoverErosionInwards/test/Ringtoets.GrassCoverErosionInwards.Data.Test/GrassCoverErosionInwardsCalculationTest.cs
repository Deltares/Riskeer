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

using System.Linq;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationTest
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
            // Setup & Call
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<ICommentable>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsInstanceOf<GrassCoverErosionInwardsInput>(calculation.InputParameters);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments);
            Assert.IsNull(calculation.Output);
            AssertDemoInput(calculation.InputParameters);
        }

        [Test]
        public void Notify_SingleListenerAttached_ListenerIsNotified()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();

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

            var calculation = new GrassCoverErosionInwardsCalculation();

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

            var calculation = new GrassCoverErosionInwardsCalculation();

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

            var calculation = new GrassCoverErosionInwardsCalculation();

            calculation.Attach(observerA);
            calculation.Attach(observerB);
            calculation.Detach(observerA);

            // Call & Assert
            calculation.NotifyObservers();
        }

        [Test]
        public void Detach_DetachNonAttachedObserver_DoesNotThrowException()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();

            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call & Assert
            calculation.Detach(observer);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Properties_Name_ReturnsExpectedValues(string name)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            calculation.Name = name;

            // Assert
            Assert.AreEqual(name, calculation.Name);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Property_Comments_ReturnsExpectedValues(string comments)
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Call
            calculation.Comments = comments;

            // Assert
            Assert.AreEqual(comments, calculation.Comments);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation()
            {
                Output = new TestGrassCoverErosionInwardsOutput()
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
            var calculation = new GrassCoverErosionInwardsCalculation()
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
            var calculation = new GrassCoverErosionInwardsCalculation()
            {
                Output = new TestGrassCoverErosionInwardsOutput()
            };

            // Call & Assert
            Assert.IsTrue(calculation.HasOutput);
        }

        [Test]
        public void ClearHydraulicBoundaryLocation_Always_SetHydraulicBoundaryLocationToNull()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1.0, 2.0);
            calculation.InputParameters.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Precondition
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);

            // Call
            calculation.ClearHydraulicBoundaryLocation();

            // Assert
            Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
        }

        [Test]
        public void Input_Always_ReturnsInputParamaters()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var inputParameters = calculation.InputParameters;

            // Call
            var input = calculation.Input;

            // Assert
            Assert.AreEqual(inputParameters, input);
        }

        private void AssertDemoInput(GrassCoverErosionInwardsInput inputParameters)
        {
            // BreakWater
            var breakWater = inputParameters.BreakWater.FirstOrDefault();
            Assert.IsNotNull(breakWater);
            Assert.AreEqual(10, breakWater.Height);
            Assert.AreEqual(BreakWaterType.Dam, breakWater.Type);
            Assert.IsTrue(inputParameters.BreakWaterPresent);

            // Orientation
            Assert.AreEqual(2, inputParameters.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(5.5, inputParameters.Orientation.Value);

            // CriticalFlowRate
            Assert.IsNotNull(inputParameters.CriticalFlowRate);

            // Dike and Foreshore
            Assert.IsTrue(inputParameters.ForeshoreGeometry.Any());
            Assert.IsTrue(inputParameters.DikeGeometry.Any());
            Assert.AreEqual(1, inputParameters.ForeshoreDikeGeometryPoints);
            Assert.IsTrue(inputParameters.ForeshorePresent);

            // Hydraulic boundaries location
            Assert.AreEqual("Demo", inputParameters.HydraulicBoundaryLocation.Name);
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);

            // Dike height
            Assert.AreEqual(10, inputParameters.DikeHeight);
        }
    }

    public class TestGrassCoverErosionInwardsOutput : GrassCoverErosionInwardsOutput
    {
        public TestGrassCoverErosionInwardsOutput() : base(0, 0, 0, 0, 0) {}
    }
}