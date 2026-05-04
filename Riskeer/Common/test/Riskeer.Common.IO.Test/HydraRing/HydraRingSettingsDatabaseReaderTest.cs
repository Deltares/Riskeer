// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
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
using Riskeer.Common.IO.HydraRing;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Settings;

namespace Riskeer.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraRingSettingsDatabaseReaderTest
    {
        private const string completeDatabase = "7_67.config.sqlite";
        private const string emptyDatabase = "7_67-empty.config.sqlite";
        private const string invalidDatabase = "7_67-invalid-value-types.config.sqlite";
        private const string completeDatabaseWithOptionalColumns = "7_67-with-optional-columns.config.sqlite";
        private const string invalidDatabaseWithOptionalColumns = "7_67-invalid-value-types-in-optional-columns.config.sqlite";

        [Test]
        public void Constructor_DatabaseWithValidSchema_ReturnsNewReader()
        {
            // Call
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
            {
                // Assert
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }
        }

        [Test]
        [TestCase(-1)]
        [TestCase(13)]
        [TestCase(15)]
        public void ReadDesignTableSetting_InvalidFailureMechanismType_ThrowsInvalidEnumArgumentException(HydraRingFailureMechanismType calculationType)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(emptyDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(invalidDatabase)))
            {
                // Call
                TestDelegate test = () => reader.ReadDesignTableSetting(locationId, type);

                // Assert
                Assert.Throws<CriticalFileReadException>(test);
            }
        }

        [Test]
        [TestCase(-1)]
        [TestCase(13)]
        [TestCase(15)]
        public void ReadTimeIntegrationSetting_InvalidFailureMechanismType_ThrowsInvalidEnumArgumentException(HydraRingFailureMechanismType calculationType)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
            {
                // Call
                TestDelegate test = () => reader.ReadTimeIntegrationSetting(123, calculationType);

                // Assert
                Assert.Throws<InvalidEnumArgumentException>(test);
            }
        }

        [Test]
        [TestCase(invalidDatabase, 700131)]
        [TestCase(invalidDatabaseWithOptionalColumns, 700131)]
        [TestCase(invalidDatabaseWithOptionalColumns, 700132)]
        public void ReadTimeIntegrationSetting_InvalidValueInReadLocation_ThrowsCriticalFileReadException(string databaseName, long locationId)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(databaseName)))
            {
                // Call
                TestDelegate test = () => reader.ReadTimeIntegrationSetting(locationId, HydraRingFailureMechanismType.AssessmentLevel);

                // Assert
                Assert.Throws<CriticalFileReadException>(test);
            }
        }

        [Test]
        [TestCase(completeDatabase, 700131, 0, 1, 3, 1)]
        [TestCase(completeDatabase, 700131, 3, 1, 3, 1)]
        [TestCase(completeDatabase, 700134, 2, 1, 3, 1)]
        [TestCase(completeDatabase, 700135, 4, 1, 3, 1)]
        [TestCase(completeDatabaseWithOptionalColumns, 700131, 0, 1, 5, 0.7)]
        [TestCase(completeDatabaseWithOptionalColumns, 700131, 3, 1, 5, 0.7)]
        [TestCase(completeDatabaseWithOptionalColumns, 700134, 2, 1, 5, 0.7)]
        [TestCase(completeDatabaseWithOptionalColumns, 700135, 4, 1, 5, 0.7)]
        public void ReadTimeIntegrationSetting_ValidLocationIdAndFailureMechanismType_TimeIntegrationSettingWithExpectedValues(
            string databaseName, long locationId, HydraRingFailureMechanismType calculationType, int expectedTimeIntegrationScheme,
            int expectedMaxIterations, double expectedRelaxationFactor)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(databaseName)))
            {
                // Call
                TimeIntegrationSetting setting = reader.ReadTimeIntegrationSetting(locationId, calculationType);

                // Assert
                Assert.AreEqual(expectedTimeIntegrationScheme, setting.TimeIntegrationSchemeId);
                Assert.AreEqual(expectedMaxIterations, setting.MaxIterations);
                Assert.AreEqual(expectedRelaxationFactor, setting.RelaxationFactor);
            }
        }

        [Test]
        [TestCase(15, 10)]
        [TestCase(0, 5)]
        public void ReadTimeIntegrationSetting_ValidLocationIdAndFailureMechanismTypeNotInDatabase_ReturnNull(long locationId, HydraRingFailureMechanismType calculationType)
        {
            // Setup
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(emptyDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(invalidDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(emptyDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(completeDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(invalidDatabase)))
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
            using (var reader = new HydraRingSettingsDatabaseReader(GetDatabasePath(emptyDatabase)))
            {
                // Call
                IEnumerable<long> locations = reader.ReadExcludedLocations();

                // Assert
                CollectionAssert.IsEmpty(locations);
            }
        }

        private static string GetDatabasePath(string databaseName)
        {
            return TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, Path.Combine("HydraRingSettingsDatabaseReader", databaseName));
        }
    }
}