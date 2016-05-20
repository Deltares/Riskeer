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
        public void Constructor_NullGeneralInput_ThrowsArgumentNullException()
        {
            // Setup
            var normProbabilityInput = new GeneralNormProbabilityInput();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculation(null, normProbabilityInput);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("generalInputParameters", exception.ParamName);
        }

        [Test]
        public void Constructor_NullNormProbabilityGrassCoverErosionInwardsInput_ThrowsArgumentNullException()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();

            // Call
            TestDelegate test = () => new GrassCoverErosionInwardsCalculation(generalInput, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("normProbabilityInput", exception.ParamName);
        }

        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();

            // Call
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput);

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsFalse(calculation.HasOutput);
            Assert.IsNull(calculation.Comments);
            Assert.IsNull(calculation.Output);
            Assert.AreSame(normProbabilityInput, calculation.NormProbabilityInput);
            AssertDemoInput(calculation.InputParameters);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        public void Properties_Name_ReturnsExpectedValues(string name)
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput);

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
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput);

            // Call
            calculation.Comments = comments;

            // Assert
            Assert.AreEqual(comments, calculation.Comments);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput)
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
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput)
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
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput)
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
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput);
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
        public void GetObservableInput_Always_ReturnsInputParameters()
        {
            // Setup
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput);
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
            var output = new GrassCoverErosionInwardsOutput(2.0, 3.0, 1.4, 50.3, 16.3);
            var generalInput = new GeneralGrassCoverErosionInwardsInput();
            var normProbabilityInput = new GeneralNormProbabilityInput();
            var calculation = new GrassCoverErosionInwardsCalculation(generalInput, normProbabilityInput)
            {
                Output = output
            };

            // Call
            ICalculationOutput calculationOutput = calculation.GetObservableOutput();

            // Assert
            Assert.AreSame(output, calculationOutput);
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
            Assert.IsTrue(inputParameters.UseForeshore);

            // Dike height
            var expectedDikeHeight = new RoundedDouble(inputParameters.DikeHeight.NumberOfDecimalPlaces, 10);
            Assert.AreEqual(expectedDikeHeight, inputParameters.DikeHeight);
        }

        private class TestGrassCoverErosionInwardsOutput : GrassCoverErosionInwardsOutput
        {
            public TestGrassCoverErosionInwardsOutput() : base(0, 0, 0, 0, 0) {}
        }
    }
}