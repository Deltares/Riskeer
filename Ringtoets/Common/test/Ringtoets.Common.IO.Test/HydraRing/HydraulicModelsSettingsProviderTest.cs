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

using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class HydraulicModelsSettingsProviderTest
    {
        private static readonly string completeDatabaseDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine("HydraRingSettingsDatabaseReader", "7_67.config.sqlite"));

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 700131, 1)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 700134, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 700134, 1)]
        public void GetHydraulicModelsSetting_KnownLocationIdAndFailureMechanismType_ReturnsExpectedHydraulicModelsSetting(
            HydraRingFailureMechanismType failureMechanismType, long locationId, int expectedTimeIntegrationSchemeId)
        {
            // Setup
            using (HydraulicModelsSettingsProvider hydraulicModelsSettingsProvider = new HydraulicModelsSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                HydraulicModelsSetting hydraulicModelsSetting = hydraulicModelsSettingsProvider.GetHydraulicModelsSetting(locationId, failureMechanismType);

                // Assert
                Assert.AreEqual(expectedTimeIntegrationSchemeId, hydraulicModelsSetting.TimeIntegrationSchemeId);
            }
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 1)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 1)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 1)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 1)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 1)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 1)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 1)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 1)]
        public void GetHydraulicModelsSetting_UnknownLocationId_ReturnsDefaultHydraulicModelsSetting(
            HydraRingFailureMechanismType failureMechanismType, int expectedTimeIntegrationSchemeId)
        {
            // Setup
            using (HydraulicModelsSettingsProvider hydraulicModelsSettingsProvider = new HydraulicModelsSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                HydraulicModelsSetting hydraulicModelsSetting = hydraulicModelsSettingsProvider.GetHydraulicModelsSetting(-1, failureMechanismType);

                // Assert
                Assert.AreEqual(expectedTimeIntegrationSchemeId, hydraulicModelsSetting.TimeIntegrationSchemeId);
            }
        }
    }
}