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
            Assert.IsNull(testLocation.WaveHeightCalculation1.Output);
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
            Assert.IsNull(testLocation.WaveHeightCalculation1.Output);
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

            AssertCalculationHasOutput(4.5, testLocation.DesignWaterLevelCalculation1);
            AssertCalculationHasOutput(5.5, testLocation.WaveHeightCalculation1);
        }

        private static void AssertCalculationHasOutput(double value, HydraulicBoundaryLocationCalculation calculation)
        {
            HydraulicBoundaryLocationOutput actual = calculation.Output;

            Assert.AreEqual(value, actual.Result, actual.Result.GetAccuracy());
            Assert.AreEqual(0, actual.TargetReliability, actual.TargetReliability.GetAccuracy());
            Assert.AreEqual(0, actual.TargetProbability);
            Assert.AreEqual(0, actual.CalculatedReliability, actual.CalculatedReliability.GetAccuracy());
            Assert.AreEqual(0, actual.CalculatedProbability);
            Assert.AreEqual(CalculationConvergence.CalculatedConverged, actual.CalculationConvergence);
            Assert.IsNull(actual.GeneralResult);
        }
    }
}