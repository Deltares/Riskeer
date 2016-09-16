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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;

namespace Ringtoets.Revetment.IO.Test
{
    [TestFixture]
    public class ExportableWaveConditionsFactoryTest
    {
        private readonly WaveConditionsOutput[] waveConditionsOutputCollection =
        {
            new WaveConditionsOutput(0.0, 1.1, 2.2, 3.3)
        };

        [Test]
        public void CreateExportableWaveConditionsCollectionFourParameters_NameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(null, new WaveConditionsInput(WaveConditionsRevetment.StabilityStone), waveConditionsOutputCollection, waveConditionsOutputCollection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionFourParameters_WaveConditionsInputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", null, waveConditionsOutputCollection, waveConditionsOutputCollection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionFourParameters_ColumnsOutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", new WaveConditionsInput(WaveConditionsRevetment.StabilityStone), null, waveConditionsOutputCollection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("columnsOutput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionFourParameters_BlocksOutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", new WaveConditionsInput(WaveConditionsRevetment.StabilityStone), waveConditionsOutputCollection, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("blocksOutput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionFourParameters_DataEmpty_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", new WaveConditionsInput(WaveConditionsRevetment.StabilityStone),
                                                                                         Enumerable.Empty<WaveConditionsOutput>(),
                                                                                         Enumerable.Empty<WaveConditionsOutput>());

            // Assert
            Assert.IsEmpty(exportableWaveConditionsCollection);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionFourParameters_ValidData_ReturnsValidCollection()
        {
            // Call
            ExportableWaveConditions[] exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("ewcName",
                                                                                         new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
                                                                                         {
                                                                                             HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hblName", 1.0, 8.0),
                                                                                             ForeshoreProfile = new ForeshoreProfile(new Point2D(8.7, 7.8), Enumerable.Empty<Point2D>(), null, new ForeshoreProfile.ConstructionProperties()),
                                                                                             UseForeshore = true
                                                                                         },
                                                                                         waveConditionsOutputCollection,
                                                                                         waveConditionsOutputCollection).ToArray();

            // Assert
            Assert.AreEqual(2, exportableWaveConditionsCollection.Length);
            Assert.AreEqual(1, exportableWaveConditionsCollection.Count(e => e.CoverType == CoverType.StoneCoverColumns));
            Assert.AreEqual(1, exportableWaveConditionsCollection.Count(e => e.CoverType == CoverType.StoneCoverBlocks));

            ExportableWaveConditions exportableWaveConditions = exportableWaveConditionsCollection[0];
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

        [Test]
        public void CreateExportableWaveConditionsCollectionThreeParameters_NameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(null, new WaveConditionsInput(WaveConditionsRevetment.StabilityStone), waveConditionsOutputCollection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionThreeParameters_WaveConditionsInputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", null, waveConditionsOutputCollection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionThreeParameters_OutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", new WaveConditionsInput(WaveConditionsRevetment.StabilityStone), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionThreeParameters_DataEmpty_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", new WaveConditionsInput(WaveConditionsRevetment.StabilityStone), Enumerable.Empty<WaveConditionsOutput>());

            // Assert
            Assert.IsEmpty(exportableWaveConditionsCollection);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionThreeParameters_ValidData_ReturnsValidCollection()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput(WaveConditionsRevetment.StabilityStone)
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hblName", 1.0, 8.0),
                ForeshoreProfile = new ForeshoreProfile(new Point2D(8.7, 7.8), Enumerable.Empty<Point2D>(), null, new ForeshoreProfile.ConstructionProperties()),
                UseForeshore = true
            };

            // Call
            ExportableWaveConditions[] exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("ewcName",
                                                                                         waveConditionsInput,
                                                                                         waveConditionsOutputCollection).ToArray();

            // Assert
            Assert.AreEqual(1, exportableWaveConditionsCollection.Length);
            ExportableWaveConditions exportableWaveConditions = exportableWaveConditionsCollection[0];
            Assert.AreEqual("ewcName", exportableWaveConditions.CalculationName);
            Assert.AreEqual("hblName", exportableWaveConditions.LocationName);
            Assert.AreEqual(1.0, exportableWaveConditions.LocationXCoordinate);
            Assert.AreEqual(8.0, exportableWaveConditions.LocationYCoordinate);
            Assert.AreEqual(null, exportableWaveConditions.ForeshoreName);
            Assert.AreEqual(false, exportableWaveConditions.UseBreakWater);
            Assert.AreEqual(true, exportableWaveConditions.UseForeshore);
            Assert.AreEqual(CoverType.Asphalt, exportableWaveConditions.CoverType);
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