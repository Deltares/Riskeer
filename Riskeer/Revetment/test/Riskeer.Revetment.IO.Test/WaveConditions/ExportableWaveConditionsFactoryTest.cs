﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Data.TestUtil;
using Riskeer.Revetment.IO.WaveConditions;

namespace Riskeer.Revetment.IO.Test.WaveConditions
{
    [TestFixture]
    public class ExportableWaveConditionsFactoryTest
    {
        private static readonly TestWaveConditionsOutput waveConditionsOutput = new TestWaveConditionsOutput();

        private readonly WaveConditionsOutput[] waveConditionsOutputCollection =
        {
            waveConditionsOutput
        };

        [Test]
        public void CreateExportableWaveConditionsCollectionWithWaveConditionsInput_NameNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(null, new WaveConditionsInput(),
                                                                                                    waveConditionsOutputCollection, CoverType.Asphalt, i => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("name", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithWaveConditionsInput_WaveConditionsInputNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", null,
                                                                                                    waveConditionsOutputCollection, CoverType.Asphalt, i => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("waveConditionsInput", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithWaveConditionsInput_OutputNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", new WaveConditionsInput(),
                                                                                                    null, CoverType.Asphalt, i => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("output", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithWaveConditionsInput_CoverTypeNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", new WaveConditionsInput(),
                                                                                                    waveConditionsOutputCollection, null, i => "1/100");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("coverType", exception.ParamName);
        }

        [Test]
        public void CreateExportableWaveConditionsCollectionWithWaveConditionsInput_GetTargetProbabilityFuncNull_ThrowArgumentNullException()
        {
            // Call
            void Call() => ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection("aName", new WaveConditionsInput(),
                                                                                                    waveConditionsOutputCollection, CoverType.Asphalt, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("getTargetProbabilityFunc", exception.ParamName);
        }

        [Test]
        [TestCaseSource(nameof(GetCoverTypes))]
        public void CreateExportableWaveConditionsCollectionWithWaveConditionsInput_ValidDataWithCoverType_ReturnsValidCollection_ValidDataWithCoverType_ReturnsValidCollection(CoverType coverType)
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
                                                                                         coverType,
                                                                                         i => "1/100").ToArray();

            // Assert
            Assert.AreEqual(1, exportableWaveConditionsCollection.Length);
            ExportableWaveConditions exportableWaveConditions = exportableWaveConditionsCollection[0];
            Assert.AreEqual("ewcName", exportableWaveConditions.CalculationName);
            Assert.AreEqual("hblName", exportableWaveConditions.LocationName);
            Assert.AreEqual(1.0, exportableWaveConditions.LocationXCoordinate);
            Assert.AreEqual(8.0, exportableWaveConditions.LocationYCoordinate);
            Assert.AreEqual("id", exportableWaveConditions.ForeshoreId);
            Assert.AreEqual(false, exportableWaveConditions.UseBreakWater);
            Assert.AreEqual(true, exportableWaveConditions.UseForeshore);
            Assert.AreEqual(coverType, exportableWaveConditions.CoverType);
            Assert.AreEqual("1/100", exportableWaveConditions.TargetProbability);
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

        private static IEnumerable<TestCaseData> GetCoverTypes()
        {
            yield return new TestCaseData(CoverType.Asphalt);
            yield return new TestCaseData(CoverType.GrassWaveRunUp);
            yield return new TestCaseData(CoverType.GrassWaveImpact);
            yield return new TestCaseData(CoverType.StoneCoverBlocks);
            yield return new TestCaseData(CoverType.StoneCoverColumns);
        }
    }
}