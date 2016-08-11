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
using Ringtoets.HydraRing.Calculation.Providers;

namespace Ringtoets.HydraRing.Calculation.Test.Providers
{
    [TestFixture]
    public class DesignTableSettingsProviderTest
    {
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 5.0, 15.0)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, double.NaN, double.NaN)]
        public void GetDesignTableSettings_DefaultsOnly_ReturnsExpectedDesignTableSettings(HydraRingFailureMechanismType failureMechanismType, double expectedValueMin, double expectedValueMax)
        {
            // Setup
            var designTablesSettingsProvider = new DesignTableSettingsProvider();

            // Call
            var designTableSettings = designTablesSettingsProvider.GetDesignTableSettings(failureMechanismType);

            // Assert
            Assert.AreEqual(expectedValueMin, designTableSettings.ValueMin);
            Assert.AreEqual(expectedValueMax, designTableSettings.ValueMax);
        }
    }
}