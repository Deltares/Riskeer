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
using Core.Common.Base;
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
    public class HydraulicBoundaryLocationCalculationRowTest
    {
        [Test]
        public void Constructor_WithHydraulicBoundaryLocationCalculation_ExpectedProperties()
        {
            // Setup
            const int id = 1;
            const string locationName = "LocationName";
            const double coordinateX = 1.0;
            const double coordinateY = 2.0;

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(id, locationName, coordinateX, coordinateY);
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(hydraulicBoundaryLocation);

            // Call
            var row = new HydraulicBoundaryLocationCalculationRow(hydraulicBoundaryLocationCalculation);

            // Assert
            Assert.IsInstanceOf<CalculatableRow<HydraulicBoundaryLocationCalculation>>(row);

            Assert.AreEqual(id, row.Id);
            Assert.AreEqual(locationName, row.Name);
            Assert.AreSame(hydraulicBoundaryLocation.Location, row.Location);

            TestHelper.AssertTypeConverter<HydraulicBoundaryLocationCalculationRow, NoValueRoundedDoubleConverter>(nameof(HydraulicBoundaryLocationCalculationRow.Result));
            Assert.IsNaN(row.Result);

            Assert.AreSame(hydraulicBoundaryLocationCalculation, row.CalculatableObject);
            Assert.IsFalse(row.ShouldCalculate);
        }

        [Test]
        public void IncludeIllustrationPoints_NewValue_SetsPropertiesAndNotifiesObservers(
            [Values(true, false)] bool setIllustrationPoints)
        {
            // Setup
            var mockRepository = new MockRepository();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            var row = new HydraulicBoundaryLocationCalculationRow(hydraulicBoundaryLocationCalculation);

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
            var hydraulicBoundaryLocationCalculation = new HydraulicBoundaryLocationCalculation(new TestHydraulicBoundaryLocation());

            var row = new HydraulicBoundaryLocationCalculationRow(hydraulicBoundaryLocationCalculation);

            var random = new Random(432);
            var output = new TestHydraulicBoundaryLocationCalculationOutput(random.NextDouble());

            // Call
            hydraulicBoundaryLocationCalculation.Output = output;

            // Assert
            Assert.AreEqual(output.Result, row.Result);
        }
    }
}