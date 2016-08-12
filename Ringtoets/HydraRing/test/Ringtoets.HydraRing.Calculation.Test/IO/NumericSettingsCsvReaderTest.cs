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
    public class NumericSettingsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Settings");

        [Test]
        public void Constructor_PathSet_DoesNotThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new NumericsSettingsCsvReader("path.csv");

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Constructor_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new NumericsSettingsCsvReader(null);

            // Assert
            const string expectedMessage = "A file must be set.";
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
                IDictionary<int, IDictionary<int, IDictionary<string, NumericsSettings>>> expectedDictionary = GetDictionary();

                // Call
                IDictionary<int, IDictionary<int, IDictionary<string, NumericsSettings>>> settings = reader.ReadSettings();

                // Assert
                Assert.AreEqual(2, settings.Count);

                foreach (KeyValuePair<int, IDictionary<int, IDictionary<string, NumericsSettings>>> expectedMechanism in expectedDictionary)
                {
                    Assert.IsTrue(settings.ContainsKey(expectedMechanism.Key));
                    Assert.IsInstanceOf<IDictionary<int, IDictionary<string, NumericsSettings>>>(settings[expectedMechanism.Key]);

                    foreach (KeyValuePair<int, IDictionary<string, NumericsSettings>> expectedSubMechanism in expectedMechanism.Value)
                    {
                        Assert.IsTrue(settings[expectedMechanism.Key].ContainsKey(expectedSubMechanism.Key));
                        Assert.IsInstanceOf<IDictionary<string, NumericsSettings>>(settings[expectedMechanism.Key][expectedSubMechanism.Key]);

                        foreach (KeyValuePair<string, NumericsSettings> expectedNumericsSettings in expectedSubMechanism.Value)
                        {
                            Assert.IsTrue(settings[expectedMechanism.Key][expectedSubMechanism.Key].ContainsKey(expectedNumericsSettings.Key));
                            Assert.IsInstanceOf<NumericsSettings>(settings[expectedMechanism.Key][expectedSubMechanism.Key][expectedNumericsSettings.Key]);

                            NumericsSettings setting = settings[expectedMechanism.Key][expectedSubMechanism.Key][expectedNumericsSettings.Key];

                            Assert.AreEqual(expectedNumericsSettings.Value.CalculationTechniqueId, setting.CalculationTechniqueId);
                            Assert.AreEqual(expectedNumericsSettings.Value.FormStartMethod, setting.FormStartMethod);
                            Assert.AreEqual(expectedNumericsSettings.Value.FormNumberOfIterations, setting.FormNumberOfIterations);
                            Assert.AreEqual(expectedNumericsSettings.Value.FormRelaxationFactor, setting.FormRelaxationFactor);
                            Assert.AreEqual(expectedNumericsSettings.Value.FormEpsBeta, setting.FormEpsBeta);
                            Assert.AreEqual(expectedNumericsSettings.Value.FormEpsHoh, setting.FormEpsHoh);
                            Assert.AreEqual(expectedNumericsSettings.Value.FormEpsZFunc, setting.FormEpsZFunc);
                            Assert.AreEqual(expectedNumericsSettings.Value.DsStartMethod, setting.DsStartMethod);
                            Assert.AreEqual(expectedNumericsSettings.Value.DsMaxNumberOfIterations, setting.DsMaxNumberOfIterations);
                            Assert.AreEqual(expectedNumericsSettings.Value.DsMinNumberOfIterations, setting.DsMinNumberOfIterations);
                            Assert.AreEqual(expectedNumericsSettings.Value.DsVarCoefficient, setting.DsVarCoefficient);
                            Assert.AreEqual(expectedNumericsSettings.Value.NiNumberSteps, setting.NiNumberSteps);
                            Assert.AreEqual(expectedNumericsSettings.Value.NiUMax, setting.NiUMax);
                            Assert.AreEqual(expectedNumericsSettings.Value.NiUMin, setting.NiUMin);
                        }
                    }
                }
            }
        }

        private static IDictionary<int, IDictionary<int, IDictionary<string, NumericsSettings>>> GetDictionary()
        {
            return new Dictionary<int, IDictionary<int, IDictionary<string, NumericsSettings>>>
            {
                {
                    1, new Dictionary<int, IDictionary<string, NumericsSettings>>
                    {
                        {
                            1, new Dictionary<string, NumericsSettings>
                            {
                                {
                                    "205", new NumericsSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 20000, 100000, 0.1, -6, 6, 25)
                                }
                            }
                        }
                    }
                },
                {
                    11, new Dictionary<int, IDictionary<string, NumericsSettings>>
                    {
                        {
                            11, new Dictionary<string, NumericsSettings>
                            {
                                {
                                    "205", new NumericsSettings(2, 6, 30, 0.50, 0.06, 0.08, 0.02, 3, 1000, 2000, 0.4, -10, 10, 14)
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}