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
using Ringtoets.HydraRing.Calculation.Settings;
using Ringtoets.HydraRing.Calculation.Types;

namespace Ringtoets.HydraRing.Calculation.Test.Settings
{
    [TestFixture]
    public class FailureMechanismSettingsProviderTest
    {
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 0.0, 50.0)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 0.0, 50.0)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 0.0, 50.0)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 0.0, 50.0)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 0.0, 50.0)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, double.NaN, double.NaN)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, double.NaN, double.NaN)]
        public void GetFailureMechanismSettings_StructuresStructuralFailureDefaultsOnly_ReturnsExpectedFailureMechanismSettings(HydraRingFailureMechanismType failureMechanismType, double expectedValueMin, double expectedValueMax)
        {
            // Setup
            var failureMechanismSettingsProvider = new FailureMechanismSettingsProvider();

            // Call
            var failureMechanismSettings = failureMechanismSettingsProvider.GetFailureMechanismSettings(failureMechanismType);

            // Assert
            Assert.AreEqual(expectedValueMin, failureMechanismSettings.ValueMin);
            Assert.AreEqual(expectedValueMax, failureMechanismSettings.ValueMax);
        }
    }
}
