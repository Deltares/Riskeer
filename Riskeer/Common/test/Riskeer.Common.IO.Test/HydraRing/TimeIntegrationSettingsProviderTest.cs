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

using System;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.IO.HydraRing;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Settings;

namespace Riskeer.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class TimeIntegrationSettingsProviderTest
    {
        private static readonly string completeDatabaseDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine("HydraRingSettingsDatabaseReader", "7_67.config.sqlite"));

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("!")]
        [TestCase("nonExisting")]
        public void Constructor_InvalidPath_ThrowCriticalFileReadException(string databasePath)
        {
            // Call
            TestDelegate test = () => new DesignTablesSettingsProvider(databasePath);

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_ValidPath_ReturnsNewInstance()
        {
            // Call
            using (var provider = new DesignTablesSettingsProvider(completeDatabaseDataPath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(provider);
            }
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 700134, 1)]
        public void GetTimeIntegrationSetting_KnownLocationIdAndFailureMechanismType_ReturnsExpectedTimeIntegrationSetting(
            HydraRingFailureMechanismType failureMechanismType, long locationId, int expectedTimeIntegrationSchemeId)
        {
            // Setup
            using (var timeIntegrationSettingsProvider = new TimeIntegrationSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                TimeIntegrationSetting timeIntegrationSetting = timeIntegrationSettingsProvider.GetTimeIntegrationSetting(locationId, failureMechanismType);

                // Assert
                Assert.AreEqual(expectedTimeIntegrationSchemeId, timeIntegrationSetting.TimeIntegrationSchemeId);
            }
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 1)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 1)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 1)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 1)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 1)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 1)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 1)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 1)]
        public void GetTimeIntegrationSetting_UnknownLocationId_ReturnsDefaultTimeIntegrationSetting(
            HydraRingFailureMechanismType failureMechanismType, int expectedTimeIntegrationSchemeId)
        {
            // Setup
            using (var timeIntegrationSettingsProvider = new TimeIntegrationSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                TimeIntegrationSetting timeIntegrationSetting = timeIntegrationSettingsProvider.GetTimeIntegrationSetting(-1, failureMechanismType);

                // Assert
                Assert.AreEqual(expectedTimeIntegrationSchemeId, timeIntegrationSetting.TimeIntegrationSchemeId);
            }
        }
    }
}