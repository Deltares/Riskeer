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

using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class TestHydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NoParameters_ExpectedValues()
        {
            // Call
            var testLocation = new TestHydraulicBoundaryLocation();

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(testLocation);
            Assert.AreEqual(0, testLocation.Id);
            Assert.IsEmpty(testLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), testLocation.Location);

            Assert.IsNull(testLocation.DesignWaterLevelCalculation1.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation2.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation3.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation4.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation1.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation2.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation3.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation4.Output);
        }

        [Test]
        public void Constructor_WithName_ExpectedValues()
        {
            // Setup
            const string name = "some name";

            // Call
            var testLocation = new TestHydraulicBoundaryLocation(name);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(testLocation);
            Assert.AreEqual(0, testLocation.Id);
            Assert.AreEqual(name, testLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), testLocation.Location);

            Assert.IsNull(testLocation.DesignWaterLevelCalculation1.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation2.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation3.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation4.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation1.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation2.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation3.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation4.Output);
        }

        [Test]
        public void CreateDesignWaterLevelCalculated_DesignWaterLevel_ExpectedValues()
        {
            // Setup
            const double designWaterLevelValue = 4.5;

            // Call
            HydraulicBoundaryLocation testLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(designWaterLevelValue);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(testLocation);
            Assert.AreEqual(0, testLocation.Id);
            Assert.IsEmpty(testLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), testLocation.Location);

            HydraulicBoundaryLocationOutput expectedDesignWaterLevelOutput = CreateHydraulicBoundaryLocationOutput(designWaterLevelValue);
            HydraulicBoundaryLocationCalculation designWaterLevelCalculation = testLocation.DesignWaterLevelCalculation1;
            AssertAreEqual(expectedDesignWaterLevelOutput, designWaterLevelCalculation.Output);
            Assert.IsNull(designWaterLevelCalculation.Output.GeneralResult);

            Assert.IsNull(testLocation.DesignWaterLevelCalculation2.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation3.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation4.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation1.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation2.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation3.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation4.Output);
        }

        [Test]
        public void CreateWaveHeightCalculated_WaveHeight_ExpectedValues()
        {
            // Setup
            const double waveHeightValue = 5.5;

            // Call
            HydraulicBoundaryLocation testLocation = TestHydraulicBoundaryLocation.CreateWaveHeightCalculated(waveHeightValue);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(testLocation);
            Assert.AreEqual(0, testLocation.Id);
            Assert.IsEmpty(testLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), testLocation.Location);

            Assert.IsNull(testLocation.DesignWaterLevelCalculation1.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation2.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation3.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation4.Output);

            HydraulicBoundaryLocationOutput expectedWaveHeightOutput = CreateHydraulicBoundaryLocationOutput(waveHeightValue);
            HydraulicBoundaryLocationCalculation waveHeightCalculation = testLocation.WaveHeightCalculation1;
            AssertAreEqual(expectedWaveHeightOutput, waveHeightCalculation.Output);
            Assert.IsNull(waveHeightCalculation.Output.GeneralResult);

            Assert.IsNull(testLocation.WaveHeightCalculation2.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation3.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation4.Output);
        }

        [Test]
        public void CreateFullyCalculated_DesignWaterLevelAndWaveHeight_ExpectedValues()
        {
            // Call
            HydraulicBoundaryLocation testLocation = TestHydraulicBoundaryLocation.CreateFullyCalculated();

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocation>(testLocation);
            Assert.AreEqual(0, testLocation.Id);
            Assert.IsEmpty(testLocation.Name);
            Assert.AreEqual(new Point2D(0, 0), testLocation.Location);

            HydraulicBoundaryLocationOutput expectedDesignWaterLevelOutput = CreateHydraulicBoundaryLocationOutput(4.5);
            HydraulicBoundaryLocationCalculation designWaterLevelCalculation = testLocation.DesignWaterLevelCalculation1;
            AssertAreEqual(expectedDesignWaterLevelOutput, designWaterLevelCalculation.Output);
            Assert.IsNull(designWaterLevelCalculation.Output.GeneralResult);

            Assert.IsNull(testLocation.DesignWaterLevelCalculation2.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation3.Output);
            Assert.IsNull(testLocation.DesignWaterLevelCalculation4.Output);

            HydraulicBoundaryLocationOutput expectedWaveHeightOutput = CreateHydraulicBoundaryLocationOutput(5.5);
            HydraulicBoundaryLocationCalculation waveHeightCalculation = testLocation.WaveHeightCalculation1;
            AssertAreEqual(expectedWaveHeightOutput, waveHeightCalculation.Output);
            Assert.IsNull(waveHeightCalculation.Output.GeneralResult);

            Assert.IsNull(testLocation.WaveHeightCalculation2.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation3.Output);
            Assert.IsNull(testLocation.WaveHeightCalculation4.Output);
        }

        private static HydraulicBoundaryLocationOutput CreateHydraulicBoundaryLocationOutput(double result)
        {
            return new HydraulicBoundaryLocationOutput(result, 0, 0, 0, 0, CalculationConvergence.CalculatedConverged, null);
        }

        private static void AssertAreEqual(HydraulicBoundaryLocationOutput expected, HydraulicBoundaryLocationOutput actual)
        {
            if (expected == null)
            {
                Assert.IsNull(actual);
                return;
            }

            Assert.AreEqual(expected.Result, actual.Result, expected.Result.GetAccuracy());
            Assert.AreEqual(expected.TargetReliability, actual.TargetReliability, expected.TargetReliability.GetAccuracy());
            Assert.AreEqual(expected.TargetProbability, actual.TargetProbability);
            Assert.AreEqual(expected.CalculatedReliability, actual.CalculatedReliability, expected.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(expected.CalculatedProbability, actual.CalculatedProbability);
            Assert.AreEqual(expected.CalculationConvergence, actual.CalculationConvergence);
        }
    }
}