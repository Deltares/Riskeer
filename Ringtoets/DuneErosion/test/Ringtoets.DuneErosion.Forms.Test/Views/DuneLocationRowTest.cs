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

using System.Globalization;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.TypeConverters;
using Ringtoets.Common.Forms.Views;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms.Views;

namespace Ringtoets.DuneErosion.Forms.Test.Views
{
    [TestFixture]
    public class DuneLocationRowTest
    {
        [Test]
        [TestCase(34.1)]
        [TestCase(34.0)]
        public void Constructor_WithOutput_ExpectedValues(double offSet)
        {
            // Setup
            var duneLocation = new DuneLocation(1, "test location", new Point2D(3.3, 4.4), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = 2,
                Offset = offSet,
                D50 = 0.000183
            });
            var duneLocationCalculation = new DuneLocationCalculation
            {
                Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                {
                    WaterLevel = 3.0,
                    WaveHeight = 4.0,
                    WavePeriod = 5.0
                })
            };

            // Call
            var row = new DuneLocationRow(duneLocation, dl => duneLocationCalculation);

            // Assert
            Assert.IsInstanceOf<CalculatableRow<DuneLocation>>(row);
            Assert.AreSame(duneLocation, row.CalculatableObject);
            Assert.AreEqual(duneLocation.Id, row.Id);
            Assert.AreEqual(duneLocation.Name, row.Name);
            Assert.AreSame(duneLocation.Location, row.Location);
            Assert.AreEqual(duneLocation.CoastalAreaId, row.CoastalAreaId);
            Assert.AreEqual(duneLocation.Offset.ToString("0.#", CultureInfo.InvariantCulture), row.Offset);
            Assert.AreEqual(duneLocation.D50, row.D50);
            Assert.AreEqual(duneLocationCalculation.Output.WaterLevel, row.WaterLevel);
            Assert.AreEqual(duneLocationCalculation.Output.WaveHeight, row.WaveHeight);
            Assert.AreEqual(duneLocationCalculation.Output.WavePeriod, row.WavePeriod);

            TestHelper.AssertTypeConverter<DuneLocationRow, NoValueRoundedDoubleConverter>(
                nameof(DuneLocationRow.WaterLevel));
            TestHelper.AssertTypeConverter<DuneLocationRow, NoValueRoundedDoubleConverter>(
                nameof(DuneLocationRow.WaveHeight));
            TestHelper.AssertTypeConverter<DuneLocationRow, NoValueRoundedDoubleConverter>(
                nameof(DuneLocationRow.WavePeriod));
        }

        [Test]
        public void Constructor_WithoutOutput_ExpectedValues()
        {
            // Setup
            var duneLocation = new DuneLocation(1, "test location", new Point2D(3.3, 4.4), new DuneLocation.ConstructionProperties());
            var duneLocationCalculation = new DuneLocationCalculation();

            // Call
            var row = new DuneLocationRow(duneLocation, dl => duneLocationCalculation);

            // Assert
            Assert.IsNaN(row.WaterLevel);
            Assert.IsNaN(row.WaveHeight);
            Assert.IsNaN(row.WavePeriod);
        }
    }
}