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
    public class DesignTableSettingsProviderTest
    {
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, "4", 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.QVariant, "4", 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, "205", 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, "205", 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, "205", 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, "205", double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, "205", 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, "205", double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, "205", double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, "205", double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, "205", double.NaN, double.NaN)]
        public void GetDesignTableSettings_UnknownFailureMechanismTypeOrRingId_ReturnsDefaultDesignTableSettings(HydraRingFailureMechanismType failureMechanismType, string ringId, double expectedValueMin, double expectedValueMax)
        {
            // Setup
            DesignTableSettingsProvider designTablesSettingsProvider = new DesignTableSettingsProvider();

            // Call
            DesignTableSettings designTableSettings = designTablesSettingsProvider.GetDesignTableSettings(failureMechanismType, ringId);

            // Assert
            Assert.AreEqual(expectedValueMin, designTableSettings.ValueMin);
            Assert.AreEqual(expectedValueMax, designTableSettings.ValueMax);
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, "205")]
        [TestCase(HydraRingFailureMechanismType.QVariant, "205")]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, "11-1")]
        [TestCase(HydraRingFailureMechanismType.QVariant, "11-1")]
        public void GetDesignTableSettings_KnownRingIdAndFailureMechanismType_ReturnsExpectedDesignTableSettings(HydraRingFailureMechanismType failureMechanismType, string ringId)
        {
            // Setup
            DesignTableSettingsProvider designTableSettingsProvider = new DesignTableSettingsProvider();
            DesignTableSettings expectedSettings = new DesignTableSettings(5, 15);

            // Call
            DesignTableSettings settings = designTableSettingsProvider.GetDesignTableSettings(failureMechanismType, ringId);

            // Assert
            Assert.AreEqual(expectedSettings.ValueMin, settings.ValueMin);
            Assert.AreEqual(expectedSettings.ValueMax, settings.ValueMax);
        }
    }
}