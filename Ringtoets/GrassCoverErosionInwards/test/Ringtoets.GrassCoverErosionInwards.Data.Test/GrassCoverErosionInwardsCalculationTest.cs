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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.GrassCoverErosionInwards.Data.Test
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Setup & Call
            var calculation = new GrassCoverErosionInwardsCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);

            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsInstanceOf<GrassCoverErosionInwardsInput>(calculation.InputParameters);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments);
            Assert.IsNull(calculation.Output);
            Assert.IsInstanceOf<Observable>(calculation);
            AssertDemoInput(calculation.InputParameters);
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
        public void GetObservableInput_Always_ReturnsInputParamaters()
        {
            // Setup
            var calculation = new GrassCoverErosionInwardsCalculation();
            var inputParameters = calculation.InputParameters;

            // Call
            ICalculationInput input = calculation.GetObservableInput();

            // Assert
            Assert.AreEqual(inputParameters, input);
        }

        private void AssertDemoInput(GrassCoverErosionInwardsInput inputParameters)
        {
            // BreakWater
            Assert.IsNotNull(inputParameters.BreakWater);
            Assert.AreEqual(10, inputParameters.BreakWater.Height);
            Assert.AreEqual(BreakWaterType.Dam, inputParameters.BreakWater.Type);
            Assert.IsTrue(inputParameters.UseBreakWater);

            // Orientation
            Assert.AreEqual(2, inputParameters.Orientation.NumberOfDecimalPlaces);
            Assert.AreEqual(5.5, inputParameters.Orientation.Value);

            // CriticalFlowRate
            Assert.IsNotNull(inputParameters.CriticalFlowRate);

            // Dike and Foreshore
            Assert.IsTrue(inputParameters.ForeshoreGeometry.Any());
            Assert.IsTrue(inputParameters.DikeGeometry.Any());
            Assert.AreEqual(1, inputParameters.ForeshoreDikeGeometryPoints);
            Assert.IsTrue(inputParameters.UseForeshore);

            // Hydraulic boundaries location
            Assert.AreEqual("Demo", inputParameters.HydraulicBoundaryLocation.Name);
            Assert.AreEqual(1300001, inputParameters.HydraulicBoundaryLocation.Id);

            // Dike height
            var expectedDikeHeight = new RoundedDouble(inputParameters.DikeHeight.NumberOfDecimalPlaces, 10);
            Assert.AreEqual(expectedDikeHeight, inputParameters.DikeHeight);
        }
    }

    public class TestGrassCoverErosionInwardsOutput : GrassCoverErosionInwardsOutput
    {
        public TestGrassCoverErosionInwardsOutput() : base(0, 0, 0, 0, 0) {}
    }
}