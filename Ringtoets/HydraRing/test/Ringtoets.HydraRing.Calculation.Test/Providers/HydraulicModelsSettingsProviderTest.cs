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

using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.Providers;

namespace Ringtoets.HydraRing.Calculation.Test.Providers
{
    [TestFixture]
    public class HydraulicModelsSettingsProviderTest
    {
        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, "205", 1)]
        [TestCase(HydraRingFailureMechanismType.QVariant, "205", 1)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, "205", 2)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, "205", 3)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, "205", 1)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, "205", 1)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, "205", 1)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, "205", 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, "205", 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, "205", 2)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, "205", 2)]
        public void GetHydraulicModelsSetting_KnownRingIdAndFailureMechanismType_ReturnsExpectedHydraulicModelsSetting(HydraRingFailureMechanismType failureMechanismType, string ringId, int expectedTimeIntegrationSchemeId)
        {
            // Setup
            HydraulicModelsSettingsProvider hydraulicModelsSettingsProvider = new HydraulicModelsSettingsProvider();

            // Call
            HydraulicModelsSetting hydraulicModelsSetting = hydraulicModelsSettingsProvider.GetHydraulicModelsSetting(failureMechanismType, ringId);

            // Assert
            Assert.AreEqual(expectedTimeIntegrationSchemeId, hydraulicModelsSetting.TimeIntegrationSchemeId);
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.QVariant, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, "4", 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, "4", 1)]
        public void GetHydraulicModelsSetting_UnknownFailureMechanismTypeOrRingId_ReturnsDefaultHydraulicModelsSetting(HydraRingFailureMechanismType failureMechanismType, string ringId, int expectedTimeIntegrationSchemeId)
        {
            // Setup
            HydraulicModelsSettingsProvider hydraulicModelsSettingsProvider = new HydraulicModelsSettingsProvider();

            // Call
            HydraulicModelsSetting hydraulicModelsSetting = hydraulicModelsSettingsProvider.GetHydraulicModelsSetting(failureMechanismType, ringId);

            // Assert
            Assert.AreEqual(expectedTimeIntegrationSchemeId, hydraulicModelsSetting.TimeIntegrationSchemeId);
        }
    }
}