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

using Core.Common.Base;
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.HydraRing.Data;

namespace Ringtoets.HeightStructures.Data.Test
{
    [TestFixture]
    public class HeightStructuresCalculationTest
    {
        [Test]
        public void Constructor_DefaultPropertyValuesAreSet()
        {
            // Call
            var calculation = new HeightStructuresCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(calculation);
            Assert.IsInstanceOf<Observable>(calculation);
            Assert.AreEqual("Nieuwe berekening", calculation.Name);
            Assert.IsNotNull(calculation.InputParameters);
            Assert.IsNull(calculation.Comments);
            Assert.IsFalse(calculation.HasOutput);
            AssertDemoInput(calculation.InputParameters);
        }

        [Test]
        public void ClearOutput_Always_SetsOutputToNull()
        {
            // Setup
            var calculation = new HeightStructuresCalculation
            {
                Output = new TestHeightStructuresOutput()
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
            var calculation = new HeightStructuresCalculation
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
            var calculation = new HeightStructuresCalculation
            {
                Output = new TestHeightStructuresOutput()
            };

            // Call & Assert
            Assert.IsTrue(calculation.HasOutput);
        }

        [Test]
        public void GetObservableInput_Always_ReturnsInputParameters()
        {
            // Setup
            var calculation = new HeightStructuresCalculation();

            // Call
            ICalculationInput input = calculation.GetObservableInput();

            // Assert
            Assert.AreSame(calculation.InputParameters, input);
        }

        [Test]
        public void GetObservableOutput_Always_ReturnsOutput()
        {
            // Setup
            var calculation = new HeightStructuresCalculation
            {
                Output = new TestHeightStructuresOutput()
            };

            // Call
            ICalculationOutput output = calculation.GetObservableOutput();

            // Assert
            Assert.AreSame(calculation.Output, output);
        }

        [Test]
        public void ClearHydraulicBoundaryLocation_Always_SetHydraulicBoundaryLocationToNull()
        {
            // Setup
            var calculation = new HeightStructuresCalculation();
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "test", 1.0, 2.0);
            calculation.InputParameters.HydraulicBoundaryLocation = hydraulicBoundaryLocation;

            // Precondition
            Assert.AreSame(hydraulicBoundaryLocation, calculation.InputParameters.HydraulicBoundaryLocation);

            // Call
            calculation.ClearHydraulicBoundaryLocation();

            // Assert
            Assert.IsNull(calculation.InputParameters.HydraulicBoundaryLocation);
        }

        private void AssertDemoInput(HeightStructuresInput inputParameters)
        {
            Assert.AreEqual(2, inputParameters.LevelOfCrestOfStructure.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(5.74, inputParameters.LevelOfCrestOfStructure.Mean.Value);
            Assert.AreEqual(2, inputParameters.OrientationOfTheNormalOfTheStructure.NumberOfDecimalPlaces);
            Assert.AreEqual(115, inputParameters.OrientationOfTheNormalOfTheStructure.Value);
            Assert.AreEqual(2, inputParameters.AllowableIncreaseOfLevelForStorage.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, inputParameters.AllowableIncreaseOfLevelForStorage.Mean.Value);
            Assert.AreEqual(2, inputParameters.StorageStructureArea.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.0, inputParameters.StorageStructureArea.Mean.Value);
            Assert.AreEqual(2, inputParameters.FlowWidthAtBottomProtection.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(18, inputParameters.FlowWidthAtBottomProtection.Mean.Value);
            Assert.AreEqual(2, inputParameters.CriticalOvertoppingDischarge.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.CriticalOvertoppingDischarge.Mean.Value);
            Assert.AreEqual(2, inputParameters.WidthOfFlowApertures.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(18, inputParameters.WidthOfFlowApertures.Mean.Value);
            Assert.AreEqual(2, inputParameters.DeviationOfTheWaveDirection.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, inputParameters.DeviationOfTheWaveDirection.Value);
            Assert.AreEqual(RoundedDouble.MaximumNumberOfDecimalPlaces, inputParameters.FailureProbabilityOfStructureGivenErosion.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.FailureProbabilityOfStructureGivenErosion.Value);
        }

        private class TestHeightStructuresOutput : ProbabilityAssessmentOutput
        {
            public TestHeightStructuresOutput() : base(0, 0, 0, 0, 0) {}
        }
    }
}