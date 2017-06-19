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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NullName_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new HydraulicBoundaryLocation(0L, null, 0.0, 0.0);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_ValidParameters_PropertiesAsExpected()
        {
            // Setup
            const long id = 1234L;
            const string name = "<some name>";
            const double x = 567.0;
            const double y = 890.0;

            // Call
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, name, x, y);

            // Assert
            Assert.IsInstanceOf<Observable>(hydraulicBoundaryLocation);

            Assert.AreEqual(id, hydraulicBoundaryLocation.Id);
            Assert.AreEqual(name, hydraulicBoundaryLocation.Name);
            Point2D location = hydraulicBoundaryLocation.Location;
            Assert.IsInstanceOf<Point2D>(location);
            Assert.AreEqual(x, location.X);
            Assert.AreEqual(y, location.Y);

            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculation>(hydraulicBoundaryLocation.DesignWaterLevelCalculation);
            Assert.IsInstanceOf<HydraulicBoundaryLocationCalculation>(hydraulicBoundaryLocation.WaveHeightCalculation);

            Assert.IsNaN(hydraulicBoundaryLocation.DesignWaterLevel);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.DesignWaterLevelCalculationConvergence);
            Assert.IsFalse(hydraulicBoundaryLocation.DesignWaterLevelCalculation.HasOutput);
            Assert.IsNull(hydraulicBoundaryLocation.DesignWaterLevelCalculation.Output);

            Assert.IsNaN(hydraulicBoundaryLocation.WaveHeight);
            Assert.AreEqual(CalculationConvergence.NotCalculated, hydraulicBoundaryLocation.WaveHeightCalculationConvergence);
            Assert.IsFalse(hydraulicBoundaryLocation.WaveHeightCalculation.HasOutput);
            Assert.IsNull(hydraulicBoundaryLocation.WaveHeightCalculation.Output);
        }

        [Test]
        [TestCase(CalculationConvergence.CalculatedConverged)]
        [TestCase(CalculationConvergence.CalculatedNotConverged)]
        public void DesignWaterLevelCalculationConvergence_ValidOutput_SetsCalculationOutputAndCalculationConvergence(CalculationConvergence converged)
        {
            // Setup
            var location = new HydraulicBoundaryLocation(0, "", 0, 0);
            var output = new TestHydraulicBoundaryLocationOutput(0, converged);

            // Call
            location.DesignWaterLevelCalculation.Output = output;

            // Assert
            Assert.AreSame(output, location.DesignWaterLevelCalculation.Output);
            Assert.AreEqual(converged, location.DesignWaterLevelCalculationConvergence);
        }

        [Test]
        [TestCase(CalculationConvergence.CalculatedConverged)]
        [TestCase(CalculationConvergence.CalculatedNotConverged)]
        public void WaveHeightCalculationConvergence_ValidOutput_SetsCalculationOutputAndCalculationConvergence(CalculationConvergence converged)
        {
            // Setup
            var location = new HydraulicBoundaryLocation(0, "", 0, 0);
            var output = new TestHydraulicBoundaryLocationOutput(0, converged);

            // Call
            location.WaveHeightCalculation.Output = output;

            // Assert
            Assert.AreSame(output, location.WaveHeightCalculation.Output);
            Assert.AreEqual(converged, location.WaveHeightCalculationConvergence);
        }

        [Test]
        public void ToString_Always_ReturnsName()
        {
            // Setup
            const string testName = "testName";
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, testName, 0, 0);

            // Call
            string result = hydraulicBoundaryLocation.ToString();

            // Assert
            Assert.AreEqual(testName, result);
        }
    }
}