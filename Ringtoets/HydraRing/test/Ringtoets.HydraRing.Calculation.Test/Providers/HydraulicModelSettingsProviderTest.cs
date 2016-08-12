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
    public class HydraulicModelSettingsProviderTest
    {
        [Test]
        public void GetHydraulicModelSettings_KnownRingId_ReturnsExpectedHydarulicModelSettings()
        {
            // Setup
            HydraulicModelSettingsProvider hydraulicModelSettingsProvider = new HydraulicModelSettingsProvider();

            // Call
            HydraulicModelSettings settings = hydraulicModelSettingsProvider.GetHydraulicModelSettings(HydraRingFailureMechanismType.AssessmentLevel, 1, "205");

            // Assert
            Assert.AreEqual(HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta, settings.RingTimeIntegrationSchemeType);            
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 1, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 3, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 4, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 5, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 11, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 14, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 16, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 102, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 103, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 102, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 103, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 311, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 313, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 314, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 421, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 422, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 423, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 422, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 424, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 425, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 426, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 427, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 422, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 424, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 425, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 430, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 431, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 432, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 433, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 434, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 435, HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta)]
        public void GetHydraulicModelSettings_UnknownRingId_ReturnsExpectedDefaultHydraulicModelSettings(HydraRingFailureMechanismType failureMechanismType, int subMechanismId, HydraRingTimeIntegrationSchemeType expectedHydraRingFailureMechanismType)
        {
            // Setup
            HydraulicModelSettingsProvider numericsSettingsProvider = new HydraulicModelSettingsProvider();

            // Call
            HydraulicModelSettings settings = numericsSettingsProvider.GetHydraulicModelSettings(failureMechanismType, subMechanismId, "unknown ringId");

            // Assert
            Assert.AreEqual(expectedHydraRingFailureMechanismType, settings.RingTimeIntegrationSchemeType);
        }
    }
}