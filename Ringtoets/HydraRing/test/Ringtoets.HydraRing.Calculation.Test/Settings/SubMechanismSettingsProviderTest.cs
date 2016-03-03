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
using Ringtoets.HydraRing.Calculation.Settings;

namespace Ringtoets.HydraRing.Calculation.Test.Settings
{
    [TestFixture]
    public class SubMechanismSettingsProviderTest
    {
        [Test]
        public void GetSubMechanismSettings_KnownRingId_ReturnsExpectedSubMechanismSettings()
        {
            // Setup
            var subMechanismSettingsProvider = new SubMechanismSettingsProvider();
            var expectedValues = GetExpectedSubMechanismSettings();

            // Call
            var settings = subMechanismSettingsProvider.GetSubMechanismSettings(HydraRingFailureMechanismType.AssessmentLevel, 1, "205");

            // Assert
            Assert.AreEqual(expectedValues.CalculationTechniqueId, settings.CalculationTechniqueId);
            Assert.AreEqual(expectedValues.FormStartMethod, settings.FormStartMethod);
            Assert.AreEqual(expectedValues.FormNumberOfIterations, settings.FormNumberOfIterations);
            Assert.AreEqual(expectedValues.FormRelaxationFactor, settings.FormRelaxationFactor);
            Assert.AreEqual(expectedValues.FormEpsBeta, settings.FormEpsBeta);
            Assert.AreEqual(expectedValues.FormEpsHoh, settings.FormEpsHoh);
            Assert.AreEqual(expectedValues.FormEpsZFunc, settings.FormEpsZFunc);
            Assert.AreEqual(expectedValues.DsStartMethod, settings.DsStartMethod);
            Assert.AreEqual(expectedValues.DsMinNumberOfIterations, settings.DsMinNumberOfIterations);
            Assert.AreEqual(expectedValues.DsMaxNumberOfIterations, settings.DsMaxNumberOfIterations);
            Assert.AreEqual(expectedValues.DsVarCoefficient, settings.DsVarCoefficient);
            Assert.AreEqual(expectedValues.NiNumberSteps, settings.NiNumberSteps);
            Assert.AreEqual(expectedValues.NiUMax, settings.NiUMax);
            Assert.AreEqual(expectedValues.NiUMin, settings.NiUMin);
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 1, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 3, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 4, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 5, 4, 4)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 11, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 14, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 16, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 102, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 103, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 311, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 313, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 314, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 421, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 422, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 423, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 422, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 424, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 425, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 426, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 427, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 422, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 424, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 425, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 430, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 431, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 432, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 433, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 434, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 435, 1, 4)]
        public void GetSubMechanismSettings_UnknownRingId_ReturnsExpectedDefaultSubMechanismSettings(HydraRingFailureMechanismType failureMechanismType, int subMechanismId, int expectedCalculationTechniqueId, int expectedFormStartMethod)
        {
            // Setup
            var subMechanismSettingsProvider = new SubMechanismSettingsProvider();

            // Call
            var subMechanismSettings = subMechanismSettingsProvider.GetSubMechanismSettings(failureMechanismType, subMechanismId, "unknown ringId");

            // Assert
            Assert.AreEqual(expectedCalculationTechniqueId, subMechanismSettings.CalculationTechniqueId);
            Assert.AreEqual(expectedFormStartMethod, subMechanismSettings.FormStartMethod);
            Assert.AreEqual(50, subMechanismSettings.FormNumberOfIterations);
            Assert.AreEqual(0.15, subMechanismSettings.FormRelaxationFactor);
            Assert.AreEqual(0.01, subMechanismSettings.FormEpsBeta);
            Assert.AreEqual(0.01, subMechanismSettings.FormEpsHoh);
            Assert.AreEqual(0.01, subMechanismSettings.FormEpsZFunc);
            Assert.AreEqual(2, subMechanismSettings.DsStartMethod);
            Assert.AreEqual(10000, subMechanismSettings.DsMinNumberOfIterations);
            Assert.AreEqual(20000, subMechanismSettings.DsMaxNumberOfIterations);
            Assert.AreEqual(0.1, subMechanismSettings.DsVarCoefficient);
            Assert.AreEqual(-6.0, subMechanismSettings.NiUMin);
            Assert.AreEqual(6.0, subMechanismSettings.NiUMax);
            Assert.AreEqual(25, subMechanismSettings.NiNumberSteps);
        }

        private static SubMechanismSettings GetExpectedSubMechanismSettings()
        {
            return new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 20000, 100000, 0.1, -6, 6, 25);
        }
    }
}