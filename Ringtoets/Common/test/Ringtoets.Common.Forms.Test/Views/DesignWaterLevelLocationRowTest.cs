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

using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test.Views
{
    [TestFixture]
    public class DesignWaterLevelLocationRowTest
    {
        [Test]
        public void Constructor_WithDesignWaterLevelLocationContext_PropertiesFromHydraulicBoundaryLocation()
        {
            // Setup
            const double designWaterLevel = 3.0;
            HydraulicBoundaryLocation hydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(
                designWaterLevel);

            // Call
            var row = new DesignWaterLevelLocationRow(hydraulicBoundaryLocation);

            // Assert
            Assert.IsInstanceOf<HydraulicBoundaryLocationRow>(row);
            Assert.AreEqual(hydraulicBoundaryLocation.Id, row.Id);
            Assert.AreEqual(hydraulicBoundaryLocation.Name, row.Name);
            Assert.AreEqual(designWaterLevel, row.DesignWaterLevel, hydraulicBoundaryLocation.DesignWaterLevel.GetAccuracy());
            Assert.AreEqual(hydraulicBoundaryLocation.Location, row.Location);
            Assert.AreSame(hydraulicBoundaryLocation, row.CalculatableObject);
            Assert.IsFalse(row.ShouldCalculate);
            TestHelper.AssertTypeConverter<DesignWaterLevelLocationRow, NoValueRoundedDoubleConverter>(
                nameof(DesignWaterLevelLocationRow.DesignWaterLevel));
        }

        [Test]
        public void Constructor_Property_SetPropertyAsExpected()
        {
            // Setup
            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            var row = new DesignWaterLevelLocationRow(hydraulicBoundaryLocation);

            // Call
            row.ShouldCalculate = true;

            // Assert
            Assert.IsTrue(row.ShouldCalculate);
        }
    }
}