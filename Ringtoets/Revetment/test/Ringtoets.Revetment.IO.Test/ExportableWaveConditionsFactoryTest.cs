﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.TestUtil;

namespace Ringtoets.Revetment.IO.Test
{
    [TestFixture]
    public class ExportableWaveConditionsFactoryTest
    {
        private static readonly TestWaveConditionsOutput waveConditionsOutput = new TestWaveConditionsOutput();

        private readonly WaveConditionsOutput[] waveConditionsOutputCollection =
        {
            waveConditionsOutput
        };

        private readonly WaveConditionsInput emptyWaveConditionsInput = new WaveConditionsInput();
        private readonly WaveConditionsInput waveConditionsInputWithAsphalt = new WaveConditionsInput();

        [Test]
        public void CreateExportableWaveConditionsCollection_NameNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(null,
                                                                                                               emptyWaveConditionsInput,
                                                                                                               waveConditionsOutputCollection,
                                                                                                               waveConditionsOutputCollection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_WaveConditionsInputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               null,
                                                                                                               waveConditionsOutputCollection,
                                                                                                               waveConditionsOutputCollection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_ColumnsOutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               emptyWaveConditionsInput,
                                                                                                               null,
                                                                                                               waveConditionsOutputCollection);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("columnsOutput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_BlocksOutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               emptyWaveConditionsInput,
                                                                                                               waveConditionsOutputCollection,
                                                                                                               (IEnumerable<WaveConditionsOutput>) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("blocksOutput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_DataEmpty_ReturnsEmptyCollection()
        {
            // Call
            IEnumerable<ExportableWaveConditions> exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                         emptyWaveConditionsInput,
                                                                                         Enumerable.Empty<WaveConditionsOutput>(),
                                                                                         Enumerable.Empty<WaveConditionsOutput>());

            // Assert
            Assert.IsEmpty(exportableWaveConditionsCollection);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_ValidData_ReturnsValidCollection()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hblName", 1.0, 8.0),
                ForeshoreProfile = new TestForeshoreProfile(),
                UseForeshore = true
            };

            // Call
            ExportableWaveConditions[] exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("ewcName",
                                                                                         waveConditionsInput,
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
            Assert.AreEqual("id", exportableWaveConditions.ForeshoreName);
            Assert.AreEqual(false, exportableWaveConditions.UseBreakWater);
            Assert.AreEqual(true, exportableWaveConditions.UseForeshore);
            Assert.AreEqual(CoverType.StoneCoverColumns, exportableWaveConditions.CoverType);
            Assert.AreEqual(2, exportableWaveConditions.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WavePeriod.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(waveConditionsOutput.WaterLevel, exportableWaveConditions.WaterLevel);
            Assert.AreEqual(waveConditionsOutput.WaveHeight, exportableWaveConditions.WaveHeight);
            Assert.AreEqual(waveConditionsOutput.WavePeakPeriod, exportableWaveConditions.WavePeriod);
            Assert.AreEqual(waveConditionsOutput.WaveAngle, exportableWaveConditions.WaveAngle);
            Assert.AreEqual(waveConditionsOutput.WaveDirection, exportableWaveConditions.WaveDirection);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_NameNullWithCoverType_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(null,
                                                                                                               waveConditionsInputWithAsphalt,
                                                                                                               waveConditionsOutputCollection,
                                                                                                               CoverType.Asphalt);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_WaveConditionsInputNullWithCoverType_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               null,
                                                                                                               waveConditionsOutputCollection,
                                                                                                               CoverType.Asphalt);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_OutputNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               waveConditionsInputWithAsphalt,
                                                                                                               null,
                                                                                                               CoverType.Asphalt);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_CoverTypeNull_ThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName",
                                                                                                               waveConditionsInputWithAsphalt,
                                                                                                               Enumerable.Empty<WaveConditionsOutput>(),
                                                                                                               (CoverType) null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("coverType", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollection_ValidDataWithCoverType_ReturnsValidCollection()
        {
            // Setup
            var waveConditionsInput = new WaveConditionsInput
            {
                HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, "hblName", 1.0, 8.0),
                ForeshoreProfile = new TestForeshoreProfile(),
                UseForeshore = true
            };

            // Call
            ExportableWaveConditions[] exportableWaveConditionsCollection =
                ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("ewcName",
                                                                                         waveConditionsInput,
                                                                                         waveConditionsOutputCollection,
                                                                                         CoverType.Grass).ToArray();

            // Assert
            Assert.AreEqual(1, exportableWaveConditionsCollection.Length);
            ExportableWaveConditions exportableWaveConditions = exportableWaveConditionsCollection[0];
            Assert.AreEqual("ewcName", exportableWaveConditions.CalculationName);
            Assert.AreEqual("hblName", exportableWaveConditions.LocationName);
            Assert.AreEqual(1.0, exportableWaveConditions.LocationXCoordinate);
            Assert.AreEqual(8.0, exportableWaveConditions.LocationYCoordinate);
            Assert.AreEqual("id", exportableWaveConditions.ForeshoreName);
            Assert.AreEqual(false, exportableWaveConditions.UseBreakWater);
            Assert.AreEqual(true, exportableWaveConditions.UseForeshore);
            Assert.AreEqual(CoverType.Grass, exportableWaveConditions.CoverType);
            Assert.AreEqual(2, exportableWaveConditions.WaterLevel.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveHeight.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WavePeriod.NumberOfDecimalPlaces);
            Assert.AreEqual(2, exportableWaveConditions.WaveAngle.NumberOfDecimalPlaces);
            Assert.AreEqual(waveConditionsOutput.WaterLevel, exportableWaveConditions.WaterLevel);
            Assert.AreEqual(waveConditionsOutput.WaveHeight, exportableWaveConditions.WaveHeight);
            Assert.AreEqual(waveConditionsOutput.WavePeakPeriod, exportableWaveConditions.WavePeriod);
            Assert.AreEqual(waveConditionsOutput.WaveAngle, exportableWaveConditions.WaveAngle);
            Assert.AreEqual(waveConditionsOutput.WaveDirection, exportableWaveConditions.WaveDirection);
        }
    }
}