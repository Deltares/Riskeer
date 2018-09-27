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
using System.ComponentModel;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class DesignTablesSettingsProviderTest
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
        public void GetDesignTablesSetting_FailureMechanismTypeIsInvalid_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            using (var designTablesSettingsProvider = new DesignTablesSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                TestDelegate test = () => designTablesSettingsProvider.GetDesignTablesSetting(0,
                                                                                              (HydraRingFailureMechanismType) 4000);

                // Assert
                Assert.Throws<InvalidEnumArgumentException>(test);
            }
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 700131, 0.29, 2.29)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 700137, 0.98, 2.98)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 700138, -2.0, 0)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 700138, 1.0, 5.0)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 700138, 2.0, 5.0)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 700138, -2.0, 0.0)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 700132, 2.0, 5.0)]
        public void GetDesignTablesSetting_KnownLocationIdAndFailureMechanismType_ReturnsExpectedDesignTablesSetting(
            HydraRingFailureMechanismType failureMechanismType, long locationId, double expectedValueMin, double expectedValueMax)
        {
            // Setup
            using (var designTablesSettingsProvider = new DesignTablesSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                DesignTablesSetting designTablesSetting = designTablesSettingsProvider.GetDesignTablesSetting(
                    locationId, failureMechanismType);

                // Assert
                Assert.AreEqual(expectedValueMin, designTablesSetting.ValueMin);
                Assert.AreEqual(expectedValueMax, designTablesSetting.ValueMax);
            }
        }

        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, -1, 2.0, 4.0)]
        [TestCase(HydraRingFailureMechanismType.QVariant, -1, 10.0, 50.0)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, -1, 1.0, 4.0)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, -1, 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, -1, 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, -1, 2.0, 4.0)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 700131, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 700131, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 700131, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 700131, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.DunesBoundaryConditions, 700131, 3.5, 6.0)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, -1, 0.001, 0.01)]
        public void GetDesignTablesSetting_UnknownFailureMechanismTypeOrLocationId_ReturnsDefaultDesignTablesSetting(
            HydraRingFailureMechanismType failureMechanismType, long locationId, double expectedValueMin, double expectedValueMax)
        {
            // Setup
            using (var designTablesSettingsProvider = new DesignTablesSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                DesignTablesSetting designTablesSetting = designTablesSettingsProvider.GetDesignTablesSetting(locationId, failureMechanismType);

                // Assert
                Assert.AreEqual(expectedValueMin, designTablesSetting.ValueMin);
                Assert.AreEqual(expectedValueMax, designTablesSetting.ValueMax);
            }
        }
    }
}