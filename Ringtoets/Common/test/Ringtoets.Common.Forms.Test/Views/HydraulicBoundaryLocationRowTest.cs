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
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryLocationRowTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();

            // Call
            TestDelegate call = () => new TestHydraulicBoundaryLocationRow(hydraulicBoundaryLocation, null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("hydraulicBoundaryLocationCalculation", paramName);
        }

        [Test]
        public void Constructor_WithHydraulicBoundaryLocation_PropertiesFromHydraulicBoundaryLocation()
        {
            // Setup
            const int id = 1;
            const string locationname = "LocationName";
            const double coordinateX = 1.0;
            const double coordinateY = 2.0;
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, locationname, coordinateX, coordinateY);
            var calculation = new HydraulicBoundaryLocationCalculation();

            // Call
            var row = new TestHydraulicBoundaryLocationRow(hydraulicBoundaryLocation,
                                                           calculation);

            // Assert
            Assert.IsInstanceOf<CalculatableRow<HydraulicBoundaryLocation>>(row);
            Assert.AreEqual(id, row.Id);
            Assert.AreEqual(locationname, row.Name);
            var expectedPoint2D = new Point2D(coordinateX, coordinateY);
            Assert.AreEqual(expectedPoint2D, row.Location);
            Assert.IsNaN(row.Result);

            Assert.AreSame(hydraulicBoundaryLocation, row.CalculatableObject);
            Assert.IsFalse(row.ShouldCalculate);
        }

        [Test]
        public void IncludeIllustrationPoints_NewValue_SetsProperties(
            [Values(true, false)] bool setIllustrationPoints)
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new HydraulicBoundaryLocationCalculation();
            var row = new TestHydraulicBoundaryLocationRow(hydraulicBoundaryLocation,
                                                           calculation);

            // Call
            row.IncludeIllustrationPoints = setIllustrationPoints;

            // Assert
            Assert.AreEqual(setIllustrationPoints, row.IncludeIllustrationPoints);
            Assert.AreEqual(setIllustrationPoints, calculation.InputParameters.ShouldIllustrationPointsBeCalculated);
        }

        [Test]
        public void Result_WithCalculationOutput_ReturnsResult()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var calculation = new HydraulicBoundaryLocationCalculation();
            var row = new TestHydraulicBoundaryLocationRow(hydraulicBoundaryLocation,
                                                           calculation);

            var random = new Random(432);
            var locationOutput = new TestHydraulicBoundaryLocationOutput(random.NextDouble());

            // Call
            calculation.Output = locationOutput;

            // Assert
            Assert.AreEqual(locationOutput.Result, row.Result);
        }

        private class TestHydraulicBoundaryLocationRow : HydraulicBoundaryLocationRow
        {
            public TestHydraulicBoundaryLocationRow(HydraulicBoundaryLocation hydraulicBoundaryLocation,
                                                    HydraulicBoundaryLocationCalculation hydraulicBoundaryLocationCalculation)
                : base(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation) {}
        }
    }
}