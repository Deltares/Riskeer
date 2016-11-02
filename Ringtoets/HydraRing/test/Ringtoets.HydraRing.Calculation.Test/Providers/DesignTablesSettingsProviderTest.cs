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
    public class DesignTablesSettingsProviderTest
    {
        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, "205", 2.0, 4.0)]
        [TestCase(HydraRingFailureMechanismType.QVariant, "205", 10, 50.0)]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, "11-1", 3.28, 5.28)]
        [TestCase(HydraRingFailureMechanismType.QVariant, "11-1", 1.0, 5.0)]
        public void GetDesignTablesSetting_KnownRingIdAndFailureMechanismType_ReturnsExpectedDesignTablesSetting(HydraRingFailureMechanismType failureMechanismType, string ringId, double expectedValueMin, double expectedValueMax)
        {
            // Setup
            DesignTablesSettingsProvider designTablesSettingsProvider = new DesignTablesSettingsProvider();

            // Call
            DesignTablesSetting designTablesSetting = designTablesSettingsProvider.GetDesignTablesSetting(failureMechanismType, ringId);

            // Assert
            Assert.AreEqual(expectedValueMin, designTablesSetting.ValueMin);
            Assert.AreEqual(expectedValueMax, designTablesSetting.ValueMax);
        }

        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, "4", 2.0, 4.0)]
        [TestCase(HydraRingFailureMechanismType.QVariant, "4", 10.0, 50.0)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, "205", 1.0, 4.0)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, "205", 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, "205", 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, "205", double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, "205", 2.0, 4.0)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, "205", double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, "205", double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, "205", double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, "205", double.NaN, double.NaN)]
        public void GetDesignTablesSetting_UnknownFailureMechanismTypeOrRingId_ReturnsDefaultDesignTablesSetting(HydraRingFailureMechanismType failureMechanismType, string ringId, double expectedValueMin, double expectedValueMax)
        {
            // Setup
            DesignTablesSettingsProvider designTablesSettingsProvider = new DesignTablesSettingsProvider();

            // Call
            DesignTablesSetting designTablesSetting = designTablesSettingsProvider.GetDesignTablesSetting(failureMechanismType, ringId);

            // Assert
            Assert.AreEqual(expectedValueMin, designTablesSetting.ValueMin);
            Assert.AreEqual(expectedValueMax, designTablesSetting.ValueMax);
        }
    }
}