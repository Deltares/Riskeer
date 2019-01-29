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

using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Settings;

namespace Riskeer.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraRingSettingsDatabaseReaderTest
    {
        private const string testDataSubDirectory = "HydraRingSettingsDatabaseReader";

        private static readonly string completeDatabasePath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine(testDataSubDirectory, "7_67.config.sqlite"));

        private static readonly string emptyDatabasePath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine(testDataSubDirectory, "7_67-empty.config.sqlite"));

        private static readonly string invalidDatabasePath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine(testDataSubDirectory, "7_67-invalid-value-types.config.sqlite"));

        private static readonly string invalidSchemaDatabasePath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine(testDataSubDirectory, "invalid-settings-schema.config.sqlite"));

        [Test]
        public void Constructor_DatabaseWithValidSchema_ReturnsNewReader()
        {
            // Call
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }
        }

        [Test]
        [TestCase(-1)]
        [TestCase(12)]
        [TestCase(15)]
        public void ReadDesignTableSetting_InvalidFailureMechanismType_ThrowsInvalidEnumArgumentException(HydraRingFailureMechanismType calculationType)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                TestDelegate test = () => reader.ReadDesignTableSetting(123, calculationType);

                // Assert
                Assert.Throws<InvalidEnumArgumentException>(test);
            }
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
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                DesignTablesSetting setting = reader.ReadDesignTableSetting(locationId, calculationType);

                // Assert
                Assert.AreEqual(expectedMin, setting.ValueMin);
                Assert.AreEqual(expectedMax, setting.ValueMax);
            }
        }

        [Test]
        [TestCase(700134, 7)]
        [TestCase(0, 5)]
        public void ReadDesignTableSetting_ValidLocationIdAndFailureMechanismTypeNotInDatabase_ReturnNull(long locationId, HydraRingFailureMechanismType calculationType)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                DesignTablesSetting setting = reader.ReadDesignTableSetting(locationId, calculationType);

                // Assert
                Assert.IsNull(setting);
            }
        }

        [Test]
        public void ReadDesignTableSetting_EmptyTable_ReturnNull()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(emptyDatabasePath))
            {
                // Call
                DesignTablesSetting setting = reader.ReadDesignTableSetting(700131, 0);

                // Assert
                Assert.IsNull(setting);
            }
        }

        [Test]
        [TestCase(700131, 5)]
        [TestCase(700132, 0)]
        public void ReadDesignTableSetting_InvalidValueInReadLocation_ThrowsCriticalFileReadException(long locationId, HydraRingFailureMechanismType type)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(invalidDatabasePath))
            {
                // Call
                TestDelegate test = () => reader.ReadDesignTableSetting(locationId, type);

                // Assert
                Assert.Throws<CriticalFileReadException>(test);
            }
        }

        [Test]
        [TestCase(-1)]
        [TestCase(12)]
        [TestCase(15)]
        public void ReadTimeIntegrationSetting_InvalidFailureMechanismType_ThrowsInvalidEnumArgumentException(HydraRingFailureMechanismType calculationType)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                TestDelegate test = () => reader.ReadTimeIntegrationSetting(123, calculationType);

                // Assert
                Assert.Throws<InvalidEnumArgumentException>(test);
            }
        }

        [Test]
        public void ReadTimeIntegrationSetting_InvalidValueInReadLocation_ThrowsCriticalFileReadException()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(invalidDatabasePath))
            {
                // Call
                TestDelegate test = () => reader.ReadTimeIntegrationSetting(700131, HydraRingFailureMechanismType.AssessmentLevel);

                // Assert
                Assert.Throws<CriticalFileReadException>(test);
            }
        }

        [Test]
        [TestCase(700131, 0, 1)]
        [TestCase(700131, 3, 1)]
        [TestCase(700134, 2, 1)]
        [TestCase(700135, 4, 1)]
        public void ReadTimeIntegrationSetting_ValidLocationIdAndFailureMechanismType_TimeIntegrationSettingWithExpectedValues(
            long locationId, HydraRingFailureMechanismType calculationType, int expectedTimeIntegrationScheme)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                TimeIntegrationSetting setting = reader.ReadTimeIntegrationSetting(locationId, calculationType);

                // Assert
                Assert.AreEqual(expectedTimeIntegrationScheme, setting.TimeIntegrationSchemeId);
            }
        }

        [Test]
        [TestCase(15, 10)]
        [TestCase(0, 5)]
        public void ReadTimeIntegrationSetting_ValidLocationIdAndFailureMechanismTypeNotInDatabase_ReturnNull(long locationId, HydraRingFailureMechanismType calculationType)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                TimeIntegrationSetting setting = reader.ReadTimeIntegrationSetting(locationId, calculationType);

                // Assert
                Assert.IsNull(setting);
            }
        }

        [Test]
        public void ReadTimeIntegrationSetting_EmptyTable_ReturnNull()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(emptyDatabasePath))
            {
                // Call
                TimeIntegrationSetting setting = reader.ReadTimeIntegrationSetting(700131, 0);

                // Assert
                Assert.IsNull(setting);
            }
        }

        [Test]
        [TestCase(700132, 11, 16, 1, 4, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 20000, 100000.0, 0.1, -6.0, 6)]
        [TestCase(700135, 3, 5, 4, 1, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 10000, 10000.0, 0.1, -6.0, 6)]
        [TestCase(700135, 101, 102, 1, 4, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 20000, 100000.0, 0.1, -6.0, 6)]
        public void ReadNumericsSetting_ValidLocationIdAndFailureMechanismType_NumericsSettingWithExpectedValues(
            long locationId,
            int mechanismId,
            int subMechanismId,
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
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                NumericsSetting setting = reader.ReadNumericsSetting(locationId, mechanismId, subMechanismId);

                // Assert
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
        }

        [Test]
        [TestCase(700134, 7, 14)]
        [TestCase(0, 5, 11)]
        [TestCase(700134, 5, 25)]
        public void ReadNumericsSetting_ValidLocationIdFailureMechanismTypeAndSubMechanismIdNotInDatabase_ReturnNull(
            long locationId, int mechanismId, int subMechanismId)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                NumericsSetting setting = reader.ReadNumericsSetting(locationId, mechanismId, subMechanismId);

                // Assert
                Assert.IsNull(setting);
            }
        }

        [Test]
        [TestCase(700132, 11, 14)]
        [TestCase(700133, 11, 14)]
        [TestCase(700134, 11, 14)]
        [TestCase(700135, 11, 14)]
        [TestCase(700136, 11, 14)]
        [TestCase(700137, 11, 14)]
        [TestCase(700138, 11, 14)]
        [TestCase(700139, 11, 14)]
        [TestCase(700140, 1, 1)]
        [TestCase(700141, 1, 1)]
        [TestCase(700142, 1, 1)]
        [TestCase(700143, 1, 1)]
        [TestCase(700144, 1, 1)]
        [TestCase(700145, 1, 1)]
        public void ReadNumericsSetting_InvalidValueInReadLocation_ThrowsCriticalFileReadException(
            long locationId, int mechanismId, int subMechanismId)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(invalidDatabasePath))
            {
                // Call
                TestDelegate test = () => reader.ReadNumericsSetting(locationId, mechanismId, subMechanismId);

                // Assert
                Assert.Throws<CriticalFileReadException>(test);
            }
        }

        [Test]
        public void ReadNumericsSetting_EmptyTable_ReturnNull()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(emptyDatabasePath))
            {
                // Call
                NumericsSetting setting = reader.ReadNumericsSetting(700135, 101, 102);

                // Assert
                Assert.IsNull(setting);
            }
        }

        [Test]
        public void ReadExcludedLocations_TableWithRows_ReturnsAllLocationIdsInTable()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
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
        }

        [Test]
        public void ReadExcludedLocations_InvalidValueInReadLocation_ThrowsCriticalFileReadException()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(invalidDatabasePath))
            {
                // Call
                TestDelegate test = () => reader.ReadExcludedLocations().ToArray();

                // Assert
                Assert.Throws<CriticalFileReadException>(test);
            }
        }

        [Test]
        public void ReadExcludedLocations_EmptyTable_ReturnsEmptyEnumerable()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(emptyDatabasePath))
            {
                // Call
                IEnumerable<long> locations = reader.ReadExcludedLocations();

                // Assert
                CollectionAssert.IsEmpty(locations);
            }
        }

        [Test]
        [TestCase(700131, 2, 8)]
        [TestCase(700132, 1, 6)]
        [TestCase(700133, 1, 6)]
        [TestCase(700134, 1, 6)]
        public void ReadPreprocessorSetting_ValidLocationId_ReadPreprocessorSettingWithExpectedValues(long locationId,
                                                                                                      double valueMin,
                                                                                                      double valueMax)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                ReadPreprocessorSetting preprocessorSetting = reader.ReadPreprocessorSetting(locationId);

                // Assert
                Assert.AreEqual(valueMin, preprocessorSetting.ValueMin);
                Assert.AreEqual(valueMax, preprocessorSetting.ValueMax);
            }
        }

        [Test]
        public void ReadPreprocessorSetting_InvalidValueInReadLocation_ThrowsCriticalFileReadException()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(invalidDatabasePath))
            {
                // Call
                TestDelegate test = () => reader.ReadPreprocessorSetting(700131);

                // Assert
                Assert.Throws<CriticalFileReadException>(test);
            }
        }

        [Test]
        public void ReadPreprocessorSetting_ValidLocationIdNotInDatabase_ReturnNull()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(emptyDatabasePath))
            {
                // Call
                ReadPreprocessorSetting preprocessorSetting = reader.ReadPreprocessorSetting(700131);

                // Assert
                Assert.IsNull(preprocessorSetting);
            }
        }

        [Test]
        public void ReadExcludedPreprocessorLocations_TableWithRows_ReturnsAllLocationIdsInTable()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(completeDatabasePath))
            {
                // Call
                IEnumerable<long> locations = reader.ReadExcludedPreprocessorLocations();

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    700136
                }, locations);
            }
        }

        [Test]
        public void ReadExcludedPreprocessorLocations_InvalidValueInReadLocation_ThrowsCriticalFileReadException()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(invalidDatabasePath))
            {
                // Call
                TestDelegate test = () => reader.ReadExcludedPreprocessorLocations().ToArray();

                // Assert
                Assert.Throws<CriticalFileReadException>(test);
            }
        }

        [Test]
        public void ReadExcludedPreprocessorLocations_EmptyTable_ReturnsEmptyEnumerable()
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(emptyDatabasePath))
            {
                // Call
                IEnumerable<long> locations = reader.ReadExcludedPreprocessorLocations();

                // Assert
                CollectionAssert.IsEmpty(locations);
            }
        }
    }
}