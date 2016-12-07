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

using System;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.Revetment.IO.Test
{
    [TestFixture]
    public class ExportableWaveConditionsTest
    {
        private readonly WaveConditionsInput waveConditionsInput = new WaveConditionsInput()
        {
            HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hblName", 1.0, 8.0),
            ForeshoreProfile = new ForeshoreProfile(new Point2D(8.7, 7.8), new Point2D[0], null, new ForeshoreProfile.ConstructionProperties()),
            UseForeshore = true
        };

        private readonly WaveConditionsOutput waveConditionsOutput = new TestWaveConditionsOutput(0.0, 1.1, 2.2, 3.3);

        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableWaveConditions(null, waveConditionsInput, waveConditionsOutput, CoverType.StoneCoverColumns);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void Constructor_WaveConditionsInputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableWaveConditions("aName", null, waveConditionsOutput, CoverType.StoneCoverColumns);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void Constructor_WaveConditionsOutputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ExportableWaveConditions("aName", waveConditionsInput, null, CoverType.StoneCoverColumns);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsOutput", exception.ParamName);
        }

        [Test]
        public void Constructor_WaveConditionsInputHydraulicBoundaryLocationNull_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new ExportableWaveConditions("aName", new WaveConditionsInput(), waveConditionsOutput, CoverType.StoneCoverColumns);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            StringAssert.StartsWith("HydraulicBoundaryLocation is null.", exception.Message);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidDataWithoutForeshore_ExpectedValues()
        {
            // Call
            ExportableWaveConditions exportableWaveConditions =
                new ExportableWaveConditions("ewcName",
                                             new WaveConditionsInput()
                                             {
                                                 HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hblName", 1.0, 8.0)
                                             },
                                             waveConditionsOutput,
                                             CoverType.StoneCoverColumns);

            // Assert
            Assert.AreEqual("ewcName", exportableWaveConditions.CalculationName);
            Assert.AreEqual("hblName", exportableWaveConditions.LocationName);
            Assert.AreEqual(1.0, exportableWaveConditions.LocationXCoordinate);
            Assert.AreEqual(8.0, exportableWaveConditions.LocationYCoordinate);
            Assert.AreEqual(null, exportableWaveConditions.ForeshoreName);
            Assert.AreEqual(false, exportableWaveConditions.UseBreakWater);
            Assert.AreEqual(false, exportableWaveConditions.UseForeshore);
            Assert.AreEqual(CoverType.StoneCoverColumns, exportableWaveConditions.CoverType);
            Assert.AreEqual(2, exportableWaveConditions.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WavePeriod.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, exportableWaveConditions.WaterLevel.Value);
            Assert.AreEqual(1.1, exportableWaveConditions.WaveHeight.Value);
            Assert.AreEqual(2.2, exportableWaveConditions.WavePeriod.Value);
            Assert.AreEqual(3.3, exportableWaveConditions.WaveAngle.Value);
        }

        [Test]
        public void Constructor_ValidDataWithForeshore_ExpectedValues()
        {
            // Call
            ExportableWaveConditions exportableWaveConditions =
                new ExportableWaveConditions("ewcName",
                                             waveConditionsInput,
                                             waveConditionsOutput,
                                             CoverType.StoneCoverColumns);

            // Assert
            Assert.AreEqual("ewcName", exportableWaveConditions.CalculationName);
            Assert.AreEqual("hblName", exportableWaveConditions.LocationName);
            Assert.AreEqual(1.0, exportableWaveConditions.LocationXCoordinate);
            Assert.AreEqual(8.0, exportableWaveConditions.LocationYCoordinate);
            Assert.AreEqual(null, exportableWaveConditions.ForeshoreName);
            Assert.AreEqual(false, exportableWaveConditions.UseBreakWater);
            Assert.AreEqual(true, exportableWaveConditions.UseForeshore);
            Assert.AreEqual(CoverType.StoneCoverColumns, exportableWaveConditions.CoverType);
            Assert.AreEqual(2, exportableWaveConditions.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WavePeriod.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, exportableWaveConditions.WaterLevel.Value);
            Assert.AreEqual(1.1, exportableWaveConditions.WaveHeight.Value);
            Assert.AreEqual(2.2, exportableWaveConditions.WavePeriod.Value);
            Assert.AreEqual(3.3, exportableWaveConditions.WaveAngle.Value);
        }
    }
}