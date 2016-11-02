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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraRingSettingsDatabaseReaderTest
    {
        private static readonly string completeDatabaseDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine("HydraRingSettingsDatabaseReader", "7_67.config.sqlite"));

        private static readonly string emptyDatabaseDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine("HydraRingSettingsDatabaseReader", "7_67-empty.config.sqlite"));

        [Test]
        [TestCase(-1)]
        [TestCase(12)]
        [TestCase(15)]
        public void ReadDesignTableSetting_InvalidFailureMechanismType_ThrowsInvalidEnumArgumentException(HydraRingFailureMechanismType calculationType)
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            TestDelegate test = () => reader.ReadDesignTableSetting(123, calculationType);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(test);
        }

        [Test]
        [TestCase(700131, 0, 0.29, 2.29)]
        [TestCase(700131, 3, 1.0, 5.0)]
        [TestCase(700134, 2, -2.0, 0.0)]
        [TestCase(700135, 4, 2.0, 5.0)]
        public void ReadDesignTableSetting_ValidLocationIdAndFailureMechanismType_DesignTableSettingWithExpectedValues(
            long locationId, HydraRingFailureMechanismType calculationType, double expectedMin, double expectedMax)
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            DesignTablesSetting setting = reader.ReadDesignTableSetting(locationId, calculationType);

            // Assert
            Assert.AreEqual(expectedMin, setting.ValueMin);
            Assert.AreEqual(expectedMax, setting.ValueMax);
        }

        [Test]
        [TestCase(700134, 7)]
        [TestCase(0, 5)]
        public void ReadDesignTableSetting_ValidLocationIdAndFailureMechanismTypeNotInDatabase_ReturnNull(long locationId, HydraRingFailureMechanismType calculationType)
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            DesignTablesSetting setting = reader.ReadDesignTableSetting(locationId, calculationType);

            // Assert
            Assert.IsNull(setting);
        }

        [Test]
        public void ReadDesignTableSetting_EmptyTable_ReturnNull()
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(emptyDatabaseDataPath);

            // Call
            DesignTablesSetting setting = reader.ReadDesignTableSetting(700131, 0);

            // Assert
            Assert.IsNull(setting);
        }

        [Test]
        [TestCase(-1)]
        [TestCase(12)]
        [TestCase(15)]
        public void ReadTimeIntegrationSetting_InvalidFailureMechanismType_ThrowsInvalidEnumArgumentException(HydraRingFailureMechanismType calculationType)
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            TestDelegate test = () => reader.ReadTimeIntegrationSetting(123, calculationType);

            // Assert
            Assert.Throws<InvalidEnumArgumentException>(test);
        }

        [Test]
        [TestCase(700131, 0, 1)]
        [TestCase(700131, 3, 1)]
        [TestCase(700134, 2, 1)]
        [TestCase(700135, 4, 1)]
        public void ReadTimeIntegrationSetting_ValidLocationIdAndFailureMechanismType_DesignTableSettingWithExpectedValues(
            long locationId, HydraRingFailureMechanismType calculationType, int expectedTimeIntegrationScheme)
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            HydraulicModelsSetting setting = reader.ReadTimeIntegrationSetting(locationId, calculationType);

            // Assert
            Assert.AreEqual(expectedTimeIntegrationScheme, setting.TimeIntegrationSchemeId);
        }

        [Test]
        [TestCase(15, 10)]
        [TestCase(0, 5)]
        public void ReadTimeIntegrationSetting_ValidLocationIdAndFailureMechanismTypeNotInDatabase_ReturnNull(long locationId, HydraRingFailureMechanismType calculationType)
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            HydraulicModelsSetting setting = reader.ReadTimeIntegrationSetting(locationId, calculationType);

            // Assert
            Assert.IsNull(setting);
        }

        [Test]
        public void ReadTimeIntegrationSetting_EmptyTable_ReturnNull()
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(emptyDatabaseDataPath);

            // Call
            HydraulicModelsSetting setting = reader.ReadTimeIntegrationSetting(700131, 0);

            // Assert
            Assert.IsNull(setting);
        }

        [Test]
        [TestCase(700132, 11, 3, 16, 1, 4, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 20000, 100000.0, 0.1, -6.0, 6)]
        [TestCase(700135, 3, 1, 5, 4, 1, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 10000, 10000.0, 0.1, -6.0, 6)]
        [TestCase(700135, 101, 1, 102, 1, 4, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 20000, 100000.0, 0.1, -6.0, 6)]
        public void ReadNumericsSetting_ValidLocationIdAndFailureMechanismType_DesignTableSettingWithExpectedValues(
            long locationId,
            int mechanismId,
            int expectedNumberOfSettings,
            int subMechanismIdForSample,
            int expectedCalculationTechniqueId,
            int expectedFormStartMethod,
            int expectedFormNumberOfIterations,
            double expectedFormRelaxationFactor,
            double expectedFormEpsBeta,
            double expectedFormEpsHoh,
            double expectedFormEpsZFunc,
            int expectedDsStartMethod,
            int expectedDsMinNumberOfIterations,
            int expectedDsMaxNumberOfIterations,
            double expectedDsVarCoefficient,
            double expectedNiUMin,
            double expectedNiUMax,
            int expectedNiNumberSteps)
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            Dictionary<int, NumericsSetting> settings = reader.ReadNumericsSetting(locationId, mechanismId);

            // Assert
            Assert.AreEqual(expectedNumberOfSettings, settings.Count);
            var setting = settings[subMechanismIdForSample];
            Assert.AreEqual(expectedCalculationTechniqueId, setting.CalculationTechniqueId);
            Assert.AreEqual(expectedFormStartMethod, setting.FormStartMethod);
            Assert.AreEqual(expectedFormNumberOfIterations, setting.FormNumberOfIterations);
            Assert.AreEqual(expectedFormRelaxationFactor, setting.FormRelaxationFactor);
            Assert.AreEqual(expectedFormEpsBeta, setting.FormEpsBeta);
            Assert.AreEqual(expectedFormEpsHoh, setting.FormEpsHoh);
            Assert.AreEqual(expectedFormEpsZFunc, setting.FormEpsZFunc);
            Assert.AreEqual(expectedDsStartMethod, setting.DsStartMethod);
            Assert.AreEqual(expectedDsMinNumberOfIterations, setting.DsMinNumberOfIterations);
            Assert.AreEqual(expectedDsMaxNumberOfIterations, setting.DsMaxNumberOfIterations);
            Assert.AreEqual(expectedDsVarCoefficient, setting.DsVarCoefficient);
            Assert.AreEqual(expectedNiUMin, setting.NiUMin);
            Assert.AreEqual(expectedNiUMax, setting.NiUMax);
            Assert.AreEqual(expectedNiNumberSteps, setting.NiNumberSteps);
        }

        [Test]
        [TestCase(700134, 7)]
        [TestCase(0, 5)]
        [TestCase(700134, 5)]
        public void ReadNumericsSetting_ValidLocationIdFailureMechanismTypeNotInDatabase_ReturnEmptyDictionary(
            long locationId, int mechanismId)
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            Dictionary<int, NumericsSetting> setting = reader.ReadNumericsSetting(locationId, mechanismId);

            // Assert
            Assert.IsEmpty(setting);
        }

        [Test]
        public void ReadNumericsSetting_EmptyTable_ReturnEmptyDictionary()
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(emptyDatabaseDataPath);

            // Call
            Dictionary<int, NumericsSetting> setting = reader.ReadNumericsSetting(700135, 101);

            // Assert
            Assert.IsEmpty(setting);
        }

        [Test]
        public void ReadExcludedLocations_TableWithRows_ReturnsAllLocationIdsInTable()
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(completeDatabaseDataPath);

            // Call
            IEnumerable<long> locations = reader.ReadExcludedLocations();

            // Assert
            CollectionAssert.AreEqual(new[]
            {
                700141,
                700142,
                700143,
                700146
            }, locations);
        }

        [Test]
        public void ReadExcludedLocations_EmptyTable_ReturnsEmptyList()
        {
            // Setup
            var reader = new HydraRingSettingsDatabaseReader(emptyDatabaseDataPath);

            // Call
            IEnumerable<long> locations = reader.ReadExcludedLocations();

            // Assert
            Assert.IsEmpty(locations);
        }
    }
}