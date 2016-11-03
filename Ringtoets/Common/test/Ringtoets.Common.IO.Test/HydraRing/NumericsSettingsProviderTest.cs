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

using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class NumericsSettingsProviderTest
    {
        private static readonly string completeDatabaseDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine("HydraRingSettingsDatabaseReader", "7_67.config.sqlite"));

        [Test]
        public void GetNumericsSetting_KnownLocationId_ReturnsExpectedNumericsSetting()
        {
            // Setup
            using (NumericsSettingsProvider numericsSettingsProvider = new NumericsSettingsProvider(completeDatabaseDataPath))
            {
                NumericsSetting expectedValues = GetExpectedNumericsSetting();

                // Call
                Dictionary<long, NumericsSetting> numericsSettings = numericsSettingsProvider.GetNumericsSettings(700132, HydraRingFailureMechanismType.AssessmentLevel);

                // Assert
                NumericsSetting numericsSetting = numericsSettings[1];
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
                Assert.AreEqual(expectedValues.NiUMin, numericsSetting.NiUMin);
                Assert.AreEqual(expectedValues.NiUMax, numericsSetting.NiUMax);
            }
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 1, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 5, 4, 4, 3000, 10000)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 11, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 14, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 16, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 102, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 103, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 102, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesHeight, 103, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 311, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 313, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesPiping, 314, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 421, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 422, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresOvertopping, 423, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 424, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresClosure, 425, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 422, 1, 1, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 424, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 425, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 430, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 431, 1, 1, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 432, 1, 1, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 433, 1, 1, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 434, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.StructuresStructuralFailure, 435, 11, 4, 10000, 40000)]
        public void GetNumericsSetting_UnknownLocationId_ReturnsExpectedDefaultNumericsSetting(
            HydraRingFailureMechanismType failureMechanismType, int subMechanismId, int expectedCalculationTechniqueId,
            int expectedFormStartMethod, int expectedDsMinNumberOfIterations, int expectedDsMaxNumberOfIterations)
        {
            // Setup
            using (NumericsSettingsProvider numericsSettingsProvider = new NumericsSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                Dictionary<long, NumericsSetting> numericsSettings = numericsSettingsProvider.GetNumericsSettings(-1, failureMechanismType);

                // Assert
                NumericsSetting numericsSetting = numericsSettings[subMechanismId];
                Assert.AreEqual(expectedCalculationTechniqueId, numericsSetting.CalculationTechniqueId);
                Assert.AreEqual(expectedFormStartMethod, numericsSetting.FormStartMethod);
                Assert.AreEqual(150, numericsSetting.FormNumberOfIterations);
                Assert.AreEqual(0.15, numericsSetting.FormRelaxationFactor);
                Assert.AreEqual(0.005, numericsSetting.FormEpsBeta);
                Assert.AreEqual(0.005, numericsSetting.FormEpsHoh);
                Assert.AreEqual(0.005, numericsSetting.FormEpsZFunc);
                Assert.AreEqual(2, numericsSetting.DsStartMethod);
                Assert.AreEqual(expectedDsMinNumberOfIterations, numericsSetting.DsMinNumberOfIterations);
                Assert.AreEqual(expectedDsMaxNumberOfIterations, numericsSetting.DsMaxNumberOfIterations);
                Assert.AreEqual(0.1, numericsSetting.DsVarCoefficient);
                Assert.AreEqual(-6.0, numericsSetting.NiUMin);
                Assert.AreEqual(6.0, numericsSetting.NiUMax);
                Assert.AreEqual(25, numericsSetting.NiNumberSteps);
            }
        }

        private static NumericsSetting GetExpectedNumericsSetting()
        {
            return new NumericsSetting(1, 4, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 20000, 100000, 0.1, -6, 6);
        }
    }
}