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
    public class NumericsSettingsProviderTest
    {
        [Test]
        public void GetNumericsSetting_KnownRingId_ReturnsExpectedNumericsSetting()
        {
            // Setup
            NumericsSettingsProvider numericsSettingsProvider = new NumericsSettingsProvider();
            NumericsSetting expectedValues = GetExpectedNumericsSetting();

            // Call
            NumericsSetting numericsSetting = numericsSettingsProvider.GetNumericsSetting(HydraRingFailureMechanismType.AssessmentLevel, 1, "205");

            // Assert
            Assert.AreEqual(expectedValues.CalculationTechniqueId, numericsSetting.CalculationTechniqueId);
            Assert.AreEqual(expectedValues.FormStartMethod, numericsSetting.FormStartMethod);
            Assert.AreEqual(expectedValues.FormNumberOfIterations, numericsSetting.FormNumberOfIterations);
            Assert.AreEqual(expectedValues.FormRelaxationFactor, numericsSetting.FormRelaxationFactor);
            Assert.AreEqual(expectedValues.FormEpsBeta, numericsSetting.FormEpsBeta);
            Assert.AreEqual(expectedValues.FormEpsHoh, numericsSetting.FormEpsHoh);
            Assert.AreEqual(expectedValues.FormEpsZFunc, numericsSetting.FormEpsZFunc);
            Assert.AreEqual(expectedValues.DsStartMethod, numericsSetting.DsStartMethod);
            Assert.AreEqual(expectedValues.DsMinNumberOfIterations, numericsSetting.DsMinNumberOfIterations);
            Assert.AreEqual(expectedValues.DsMaxNumberOfIterations, numericsSetting.DsMaxNumberOfIterations);
            Assert.AreEqual(expectedValues.DsVarCoefficient, numericsSetting.DsVarCoefficient);
            Assert.AreEqual(expectedValues.NiNumberSteps, numericsSetting.NiNumberSteps);
            Assert.AreEqual(expectedValues.NiUMax, numericsSetting.NiUMax);
            Assert.AreEqual(expectedValues.NiUMin, numericsSetting.NiUMin);
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 1, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 5, 4, 4)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 11, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 14, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 16, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 102, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 103, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 102, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 103, 1, 4)]
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
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 424, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 425, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 430, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 431, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 432, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 433, 1, 1)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 434, 1, 4)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 435, 1, 4)]
        public void GetNumericsSetting_UnknownRingId_ReturnsExpectedDefaultNumericsSetting(HydraRingFailureMechanismType failureMechanismType, int subMechanismId, int expectedCalculationTechniqueId, int expectedFormStartMethod)
        {
            // Setup
            NumericsSettingsProvider numericsSettingsProvider = new NumericsSettingsProvider();

            // Call
            NumericsSetting numericsSetting = numericsSettingsProvider.GetNumericsSetting(failureMechanismType, subMechanismId, "unknown ringId");

            // Assert
            Assert.AreEqual(expectedCalculationTechniqueId, numericsSetting.CalculationTechniqueId);
            Assert.AreEqual(expectedFormStartMethod, numericsSetting.FormStartMethod);
            Assert.AreEqual(50, numericsSetting.FormNumberOfIterations);
            Assert.AreEqual(0.15, numericsSetting.FormRelaxationFactor);
            Assert.AreEqual(0.01, numericsSetting.FormEpsBeta);
            Assert.AreEqual(0.01, numericsSetting.FormEpsHoh);
            Assert.AreEqual(0.01, numericsSetting.FormEpsZFunc);
            Assert.AreEqual(2, numericsSetting.DsStartMethod);
            Assert.AreEqual(10000, numericsSetting.DsMinNumberOfIterations);
            Assert.AreEqual(20000, numericsSetting.DsMaxNumberOfIterations);
            Assert.AreEqual(0.1, numericsSetting.DsVarCoefficient);
            Assert.AreEqual(-6.0, numericsSetting.NiUMin);
            Assert.AreEqual(6.0, numericsSetting.NiUMax);
            Assert.AreEqual(25, numericsSetting.NiNumberSteps);
        }

        private static NumericsSetting GetExpectedNumericsSetting()
        {
            return new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 20000, 100000, 0.1, -6, 6, 25);
        }
    }
}