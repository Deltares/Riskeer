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

using System.Globalization;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
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
            var location = new DuneLocation(1, "test location", new Point2D(3.3, 4.4), new DuneLocation.ConstructionProperties
            {
                CoastalAreaId = 2,
                Offset = offSet,
                D50 = 0.000183
            })
            {
                Output = new DuneLocationOutput(CalculationConvergence.CalculatedConverged, new DuneLocationOutput.ConstructionProperties
                {
                    WaterLevel = 3.0,
                    WaveHeight = 4.0,
                    WavePeriod = 5.0
                })
            };

            // Call
            var row = new DuneLocationRow(location);

            // Assert
            Assert.IsInstanceOf<CalculatableRow<DuneLocation>>(row);
            Assert.AreSame(location, row.CalculatableObject);
            Assert.AreEqual(location.Id, row.Id);
            Assert.AreEqual(location.Name, row.Name);
            Assert.AreSame(location.Location, row.Location);
            Assert.AreEqual(location.CoastalAreaId, row.CoastalAreaId);
            Assert.AreEqual(location.Offset.ToString("0.#", CultureInfo.InvariantCulture), row.Offset);
            Assert.AreEqual(location.D50, row.D50);
            Assert.AreEqual(location.Output.WaterLevel, row.WaterLevel);
            Assert.AreEqual(location.Output.WaveHeight, row.WaveHeight);
            Assert.AreEqual(location.Output.WavePeriod, row.WavePeriod);

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
            var location = new DuneLocation(1, "test location", new Point2D(3.3, 4.4), new DuneLocation.ConstructionProperties());

            // Call
            var row = new DuneLocationRow(location);

            // Assert
            Assert.IsNaN(row.WaterLevel);
            Assert.IsNaN(row.WaveHeight);
            Assert.IsNaN(row.WavePeriod);
        }
    }
}