// Copyright (C) Stichting Deltares 2018. All rights reserved.
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

using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;
using Riskeer.HydraRing.Calculation.Data;
using Riskeer.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class NumericsSettingsProviderTest
    {
        private static readonly string completeDatabaseDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine("HydraRingSettingsDatabaseReader", "7_67.config.sqlite"));

        [Test]
        [TestCase("")]
        [TestCase("  ")]
        [TestCase("!")]
        [TestCase("nonExisting")]
        public void Constructor_InvalidPath_ThrowCriticalFileReadException(string databasePath)
        {
            // Call
            TestDelegate test = () => new NumericsSettingsProvider(databasePath);

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_ValidPath_ReturnsNewInstance()
        {
            // Call
            using (var provider = new NumericsSettingsProvider(completeDatabaseDataPath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(provider);
            }
        }

        [Test]
        public void GetNumericsSettings_KnownLocationId_ReturnsExpectedNumericsSetting()
        {
            // Setup
            using (var numericsSettingsProvider = new NumericsSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                Dictionary<int, NumericsSetting> numericsSettings = numericsSettingsProvider.GetNumericsSettings(700132, HydraRingFailureMechanismType.AssessmentLevel);

                // Assert
                Assert.IsTrue(numericsSettings.ContainsKey(1));

                NumericsSetting expected = GetExpectedNumericsSetting();
                AssertNumericsSetting(expected, numericsSettings[1]);
            }
        }

        [Test]
        [TestCase(HydraRingFailureMechanismType.AssessmentLevel, 1, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.QVariant, 5, 4, 4, 3000, 10000)]
        [TestCase(HydraRingFailureMechanismType.WaveHeight, 11, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.WavePeakPeriod, 14, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.WaveSpectralPeriod, 16, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 102, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikeHeight, 103, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 102, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.DikesOvertopping, 103, 11, 4, 10000, 40000)]
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
        [TestCase(HydraRingFailureMechanismType.DunesBoundaryConditions, 6, 1, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 102, 11, 4, 10000, 40000)]
        [TestCase(HydraRingFailureMechanismType.OvertoppingRate, 103, 11, 4, 10000, 40000)]
        public void GetNumericsSettings_UnknownLocationId_ReturnsExpectedDefaultNumericsSetting(
            HydraRingFailureMechanismType failureMechanismType, int subMechanismId, int expectedCalculationTechniqueId,
            int expectedFormStartMethod, int expectedDsMinNumberOfIterations, int expectedDsMaxNumberOfIterations)
        {
            // Setup
            using (var numericsSettingsProvider = new NumericsSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                Dictionary<int, NumericsSetting> numericsSettings = numericsSettingsProvider.GetNumericsSettings(-1, failureMechanismType);

                // Assert
                Assert.IsTrue(numericsSettings.ContainsKey(subMechanismId));

                var expected = new NumericsSetting(expectedCalculationTechniqueId,
                                                   expectedFormStartMethod,
                                                   150, 0.15, 0.005, 0.005, 0.005, 2,
                                                   expectedDsMinNumberOfIterations,
                                                   expectedDsMaxNumberOfIterations,
                                                   0.1, -6.0, 6.0, 25);
                AssertNumericsSetting(expected, numericsSettings[subMechanismId]);
            }
        }

        [Test]
        public void GetNumericsSettingForPreprocessor_PreprocessorSubMechanismIdKnown_ReturnExpectedNumericsSetting()
        {
            // Setup
            using (var numericsSettingsProvider = new NumericsSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                NumericsSetting numericsSetting = numericsSettingsProvider.GetNumericsSettingForPreprocessor(700131);

                // Assert
                var expected = new NumericsSetting(2, 3, 20, 0.2, 0.1, 0.1, 0.1, 3, 4, 15000, 90000, 0.2, -4, 5);
                AssertNumericsSetting(expected, numericsSetting);
            }
        }

        [Test]
        public void GetNumericsSettingForPreprocessor_PreprocessorSubMechanismIdUnknownDefaultSubMechanismIdKnown_ReturnExpectedNumericsSetting()
        {
            // Setup
            using (var numericsSettingsProvider = new NumericsSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                NumericsSetting numericsSetting = numericsSettingsProvider.GetNumericsSettingForPreprocessor(700138);

                // Assert
                NumericsSetting expected = GetExpectedNumericsSetting();
                AssertNumericsSetting(expected, numericsSetting);
            }
        }

        [Test]
        public void GetNumericsSettingForPreprocessor_LocationIdUnknown_ReturnsExpectedDefaultNumericsSetting()
        {
            // Setup
            using (var numericsSettingsProvider = new NumericsSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                NumericsSetting numericsSetting = numericsSettingsProvider.GetNumericsSettingForPreprocessor(700139);

                // Assert
                var expected = new NumericsSetting(11, 4, 150, 0.15, 0.005, 0.005, 0.005, 2, 10000, 40000, 0.1, -6.0, 6.0, 25);
                AssertNumericsSetting(expected, numericsSetting);
            }
        }

        private static NumericsSetting GetExpectedNumericsSetting()
        {
            return new NumericsSetting(1, 4, 50, 0.15, 0.05, 0.01, 0.01, 0, 2, 20000, 100000, 0.1, -6, 6);
        }

        private static void AssertNumericsSetting(NumericsSetting expectedNumericsSetting, NumericsSetting actualNumericsSetting)
        {
            Assert.AreEqual(expectedNumericsSetting.CalculationTechniqueId, actualNumericsSetting.CalculationTechniqueId);
            Assert.AreEqual(expectedNumericsSetting.FormStartMethod, actualNumericsSetting.FormStartMethod);
            Assert.AreEqual(expectedNumericsSetting.FormNumberOfIterations, actualNumericsSetting.FormNumberOfIterations);
            Assert.AreEqual(expectedNumericsSetting.FormRelaxationFactor, actualNumericsSetting.FormRelaxationFactor);
            Assert.AreEqual(expectedNumericsSetting.FormEpsBeta, actualNumericsSetting.FormEpsBeta);
            Assert.AreEqual(expectedNumericsSetting.FormEpsHoh, actualNumericsSetting.FormEpsHoh);
            Assert.AreEqual(expectedNumericsSetting.FormEpsZFunc, actualNumericsSetting.FormEpsZFunc);
            Assert.AreEqual(expectedNumericsSetting.DsStartMethod, actualNumericsSetting.DsStartMethod);
            Assert.AreEqual(expectedNumericsSetting.DsMinNumberOfIterations, actualNumericsSetting.DsMinNumberOfIterations);
            Assert.AreEqual(expectedNumericsSetting.DsMaxNumberOfIterations, actualNumericsSetting.DsMaxNumberOfIterations);
            Assert.AreEqual(expectedNumericsSetting.DsVarCoefficient, actualNumericsSetting.DsVarCoefficient);
            Assert.AreEqual(expectedNumericsSetting.NiNumberSteps, actualNumericsSetting.NiNumberSteps);
            Assert.AreEqual(expectedNumericsSetting.NiUMin, actualNumericsSetting.NiUMin);
            Assert.AreEqual(expectedNumericsSetting.NiUMax, actualNumericsSetting.NiUMax);
        }
    }
}