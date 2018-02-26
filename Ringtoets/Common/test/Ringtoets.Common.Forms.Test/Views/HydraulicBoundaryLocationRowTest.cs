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
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class HydraulicBoundaryLocationRowTest
    {
        [Test]
        public void Constructor_HydraulicBoundaryLocationCalculationNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationRow(new TestHydraulicBoundaryLocation(),
                                                                       null);

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
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            // Call
            var row = new HydraulicBoundaryLocationRow(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation);

            // Assert
            Assert.IsInstanceOf<CalculatableRow<HydraulicBoundaryLocation>>(row);
            Assert.AreEqual(id, row.Id);
            Assert.AreEqual(locationname, row.Name);
            var expectedPoint2D = new Point2D(coordinateX, coordinateY);
            Assert.AreEqual(expectedPoint2D, row.Location);

            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationRow, NoValueRoundedDoubleConverter>(
                nameof(HydraulicBoundaryLocationRow.Result));
            Assert.IsNaN(row.Result);

            Assert.AreSame(hydraulicBoundaryLocation, row.CalculatableObject);
            Assert.IsFalse(row.ShouldCalculate);
        }

        [Test]
        public void IncludeIllustrationPoints_NewValue_SetsProperties(
            [Values(true, false)] bool setIllustrationPoints)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            var row = new HydraulicBoundaryLocationRow(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation);

            row.CalculatableObject.Attach(observer);

            // Call
            row.IncludeIllustrationPoints = setIllustrationPoints;

            // Assert
            Assert.AreEqual(setIllustrationPoints, row.IncludeIllustrationPoints);
            Assert.AreEqual(setIllustrationPoints, hydraulicBoundaryLocationCalculation.InputParameters.ShouldIllustrationPointsBeCalculated);

            mockRepository.VerifyAll();
        }

        [Test]
        public void Result_WithCalculationOutput_ReturnsResult()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            var row = new HydraulicBoundaryLocationRow(hydraulicBoundaryLocation, hydraulicBoundaryLocationCalculation);

            var random = new Random(432);
            var locationOutput = new TestHydraulicBoundaryLocationOutput(random.NextDouble());

            // Call
            hydraulicBoundaryLocationCalculation.Output = locationOutput;

            // Assert
            Assert.AreEqual(locationOutput.Result, row.Result);
        }
    }
}