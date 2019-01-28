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
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.HydraRing;
using Riskeer.HydraRing.Calculation.Data.Settings;

namespace Ringtoets.Common.IO.Test.HydraRing
{
    [TestFixture]
    public class PreprocessorSettingsProviderTest
    {
        private static readonly string completeDatabaseDataPath = TestHelper.GetTestDataPath(
            TestDataPath.Ringtoets.Common.IO,
            Path.Combine("HydraRingSettingsDatabaseReader", "7_67.config.sqlite"));

        [Test]
        public void Constructor_ValidPath_ExpectedValues()
        {
            // Call
            using (var provider = new PreprocessorSettingsProvider(completeDatabaseDataPath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(provider);
            }
        }

        [Test]
        public void GetPreprocessorSetting_UsePreprocessorFalse_ReturnsExpectedPreprocessorSetting()
        {
            // Setup
            using (var provider = new PreprocessorSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                PreprocessorSetting setting = provider.GetPreprocessorSetting(700131, false);

                // Assert
                Assert.IsFalse(setting.RunPreprocessor);
            }
        }

        [Test]
        public void GetPreprocessorSetting_UsePreprocessorTrueLocationIdExcluded_ReturnsExpectedPreprocessorSetting()
        {
            // Setup
            using (var provider = new PreprocessorSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                PreprocessorSetting setting = provider.GetPreprocessorSetting(700136, true);

                // Assert
                Assert.IsFalse(setting.RunPreprocessor);
            }
        }

        [Test]
        public void GetPreprocessorSetting_UsePreprocessorTrueAndKnownLocationId_ReturnsExpectedPreprocessorSetting()
        {
            // Setup
            using (var provider = new PreprocessorSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                PreprocessorSetting setting = provider.GetPreprocessorSetting(700131, true);

                // Assert
                Assert.IsTrue(setting.RunPreprocessor);
                Assert.AreEqual(2, setting.ValueMin);
                Assert.AreEqual(8, setting.ValueMax);
                NumericsSetting numericsSetting = setting.NumericsSetting;
                Assert.AreEqual(2, numericsSetting.CalculationTechniqueId);
                Assert.AreEqual(3, numericsSetting.FormStartMethod);
                Assert.AreEqual(20, numericsSetting.FormNumberOfIterations);
                Assert.AreEqual(0.2, numericsSetting.FormRelaxationFactor);
                Assert.AreEqual(0.1, numericsSetting.FormEpsBeta);
                Assert.AreEqual(0.1, numericsSetting.FormEpsHoh);
                Assert.AreEqual(0.1, numericsSetting.FormEpsZFunc);
                Assert.AreEqual(3, numericsSetting.DsStartMethod);
                Assert.AreEqual(4, numericsSetting.DsMinNumberOfIterations);
                Assert.AreEqual(15000, numericsSetting.DsMaxNumberOfIterations);
                Assert.AreEqual(90000, numericsSetting.DsVarCoefficient);
                Assert.AreEqual(0.2, numericsSetting.NiUMin);
                Assert.AreEqual(-4, numericsSetting.NiUMax);
                Assert.AreEqual(5, numericsSetting.NiNumberSteps);
            }
        }

        [Test]
        public void GetPreprocessorSetting_UsePreprocessorTrueAndUnknownLocationId_ReturnsExpectedDefaultPreprocessorSetting()
        {
            // Setup
            using (var provider = new PreprocessorSettingsProvider(completeDatabaseDataPath))
            {
                // Call
                PreprocessorSetting setting = provider.GetPreprocessorSetting(700139, true);

                // Assert
                Assert.IsTrue(setting.RunPreprocessor);
                Assert.AreEqual(1, setting.ValueMin);
                Assert.AreEqual(6, setting.ValueMax);
                NumericsSetting numericsSetting = setting.NumericsSetting;
                Assert.AreEqual(11, numericsSetting.CalculationTechniqueId);
                Assert.AreEqual(4, numericsSetting.FormStartMethod);
                Assert.AreEqual(150, numericsSetting.FormNumberOfIterations);
                Assert.AreEqual(0.15, numericsSetting.FormRelaxationFactor);
                Assert.AreEqual(0.005, numericsSetting.FormEpsBeta);
                Assert.AreEqual(0.005, numericsSetting.FormEpsHoh);
                Assert.AreEqual(0.005, numericsSetting.FormEpsZFunc);
                Assert.AreEqual(2, numericsSetting.DsStartMethod);
                Assert.AreEqual(10000, numericsSetting.DsMinNumberOfIterations);
                Assert.AreEqual(40000, numericsSetting.DsMaxNumberOfIterations);
                Assert.AreEqual(0.1, numericsSetting.DsVarCoefficient);
                Assert.AreEqual(-6.0, numericsSetting.NiUMin);
                Assert.AreEqual(6.0, numericsSetting.NiUMax);
                Assert.AreEqual(25, numericsSetting.NiNumberSteps);
            }
        }
    }
}