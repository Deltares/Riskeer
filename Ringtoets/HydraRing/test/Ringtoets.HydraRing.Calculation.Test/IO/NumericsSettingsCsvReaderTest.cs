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

using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.IO;

namespace Ringtoets.HydraRing.Calculation.Test.IO
{
    [TestFixture]
    public class NumericsSettingsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Settings");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            NumericsSettingsCsvReader reader = new NumericsSettingsCsvReader("path.csv");

            // Assert
            Assert.IsInstanceOf<HydraRingSettingsCsvReader<IDictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>>>>(reader);
        }

        [Test]
        public void Constructor_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new NumericsSettingsCsvReader(null);

            // Assert
            const string expectedMessage = "File contents must be set.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void ReadSettings_ValidFile_ReturnsSettings()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "NumericsSettingsTest.csv");

            using (StreamReader streamReader = new StreamReader(testFile))
            {
                string fileContents = streamReader.ReadToEnd();

                NumericsSettingsCsvReader reader = new NumericsSettingsCsvReader(fileContents);
                IDictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>> expectedDictionary = GetDictionary();

                // Call
                IDictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>> settings = reader.ReadSettings();

                // Assert
                Assert.AreEqual(2, settings.Count);

                foreach (KeyValuePair<int, IDictionary<int, IDictionary<string, NumericsSetting>>> expectedMechanism in expectedDictionary)
                {
                    Assert.IsTrue(settings.ContainsKey(expectedMechanism.Key));
                    Assert.IsInstanceOf<IDictionary<int, IDictionary<string, NumericsSetting>>>(settings[expectedMechanism.Key]);

                    foreach (KeyValuePair<int, IDictionary<string, NumericsSetting>> expectedSubMechanism in expectedMechanism.Value)
                    {
                        Assert.IsTrue(settings[expectedMechanism.Key].ContainsKey(expectedSubMechanism.Key));
                        Assert.IsInstanceOf<IDictionary<string, NumericsSetting>>(settings[expectedMechanism.Key][expectedSubMechanism.Key]);

                        foreach (KeyValuePair<string, NumericsSetting> expectedNumericsSetting in expectedSubMechanism.Value)
                        {
                            Assert.IsTrue(settings[expectedMechanism.Key][expectedSubMechanism.Key].ContainsKey(expectedNumericsSetting.Key));
                            Assert.IsInstanceOf<NumericsSetting>(settings[expectedMechanism.Key][expectedSubMechanism.Key][expectedNumericsSetting.Key]);

                            NumericsSetting setting = settings[expectedMechanism.Key][expectedSubMechanism.Key][expectedNumericsSetting.Key];

                            Assert.AreEqual(expectedNumericsSetting.Value.CalculationTechniqueId, setting.CalculationTechniqueId);
                            Assert.AreEqual(expectedNumericsSetting.Value.FormStartMethod, setting.FormStartMethod);
                            Assert.AreEqual(expectedNumericsSetting.Value.FormNumberOfIterations, setting.FormNumberOfIterations);
                            Assert.AreEqual(expectedNumericsSetting.Value.FormRelaxationFactor, setting.FormRelaxationFactor);
                            Assert.AreEqual(expectedNumericsSetting.Value.FormEpsBeta, setting.FormEpsBeta);
                            Assert.AreEqual(expectedNumericsSetting.Value.FormEpsHoh, setting.FormEpsHoh);
                            Assert.AreEqual(expectedNumericsSetting.Value.FormEpsZFunc, setting.FormEpsZFunc);
                            Assert.AreEqual(expectedNumericsSetting.Value.DsStartMethod, setting.DsStartMethod);
                            Assert.AreEqual(expectedNumericsSetting.Value.DsMaxNumberOfIterations, setting.DsMaxNumberOfIterations);
                            Assert.AreEqual(expectedNumericsSetting.Value.DsMinNumberOfIterations, setting.DsMinNumberOfIterations);
                            Assert.AreEqual(expectedNumericsSetting.Value.DsVarCoefficient, setting.DsVarCoefficient);
                            Assert.AreEqual(expectedNumericsSetting.Value.NiNumberSteps, setting.NiNumberSteps);
                            Assert.AreEqual(expectedNumericsSetting.Value.NiUMax, setting.NiUMax);
                            Assert.AreEqual(expectedNumericsSetting.Value.NiUMin, setting.NiUMin);
                        }
                    }
                }
            }
        }

        private static IDictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>> GetDictionary()
        {
            return new Dictionary<int, IDictionary<int, IDictionary<string, NumericsSetting>>>
            {
                {
                    1, new Dictionary<int, IDictionary<string, NumericsSetting>>
                    {
                        {
                            1, new Dictionary<string, NumericsSetting>
                            {
                                {
                                    "205", new NumericsSetting(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 20000, 100000, 0.1, -6, 6, 25)
                                }
                            }
                        }
                    }
                },
                {
                    11, new Dictionary<int, IDictionary<string, NumericsSetting>>
                    {
                        {
                            11, new Dictionary<string, NumericsSetting>
                            {
                                {
                                    "205", new NumericsSetting(2, 6, 30, 0.50, 0.06, 0.08, 0.02, 3, 1000, 2000, 0.4, -10, 10, 14)
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}