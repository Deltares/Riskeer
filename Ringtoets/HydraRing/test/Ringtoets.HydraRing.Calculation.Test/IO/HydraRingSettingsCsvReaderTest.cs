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
    public class HydraRingSettingsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Settings");

        [Test]
        public void Constructor_PathSet_DoesNotThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraRingSettingsCsvReader("path.csv");

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Constructor_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraRingSettingsCsvReader(null);

            // Assert
            const string expectedMessage = "A file must be set.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void ReadSettings_ValidFile_ReturnsSettings()
        {
            // Setup
            var testFile = Path.Combine(testDataPath, "HydraRingSettingsTest.csv");

            using (var streamReader = new StreamReader(testFile))
            {
                var fileContents = streamReader.ReadToEnd();

                var reader = new HydraRingSettingsCsvReader(fileContents);
                var expectedDictionary = GetDictionary();

                // Call
                var settings = reader.ReadSettings();

                // Assert
                Assert.IsInstanceOf<IDictionary<int, IDictionary<int, IDictionary<string, SubMechanismSettings>>>>(settings);
                Assert.AreEqual(2, settings.Count);

                foreach (KeyValuePair<int, IDictionary<int, IDictionary<string, SubMechanismSettings>>> expectedMechanism in expectedDictionary)
                {
                    Assert.IsTrue(settings.ContainsKey(expectedMechanism.Key));
                    Assert.IsInstanceOf<IDictionary<int, IDictionary<string, SubMechanismSettings>>>(settings[expectedMechanism.Key]);

                    foreach (KeyValuePair<int, IDictionary<string, SubMechanismSettings>> expectedSubMechanism in expectedMechanism.Value)
                    {
                        Assert.IsTrue(settings[expectedMechanism.Key].ContainsKey(expectedSubMechanism.Key));
                        Assert.IsInstanceOf<IDictionary<string, SubMechanismSettings>>(settings[expectedMechanism.Key][expectedSubMechanism.Key]);

                        foreach (KeyValuePair<string, SubMechanismSettings> expectedSubMechanismSettings in expectedSubMechanism.Value)
                        {
                            Assert.IsTrue(settings[expectedMechanism.Key][expectedSubMechanism.Key].ContainsKey(expectedSubMechanismSettings.Key));
                            Assert.IsInstanceOf<SubMechanismSettings>(settings[expectedMechanism.Key][expectedSubMechanism.Key][expectedSubMechanismSettings.Key]);

                            SubMechanismSettings setting = settings[expectedMechanism.Key][expectedSubMechanism.Key][expectedSubMechanismSettings.Key];

                            Assert.AreEqual(expectedSubMechanismSettings.Value.CalculationTechniqueId, setting.CalculationTechniqueId);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.FormStartMethod, setting.FormStartMethod);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.FormNumberOfIterations, setting.FormNumberOfIterations);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.FormRelaxationFactor, setting.FormRelaxationFactor);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.FormEpsBeta, setting.FormEpsBeta);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.FormEpsHoh, setting.FormEpsHoh);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.FormEpsZFunc, setting.FormEpsZFunc);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.DsStartMethod, setting.DsStartMethod);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.DsMaxNumberOfIterations, setting.DsMaxNumberOfIterations);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.DsMinNumberOfIterations, setting.DsMinNumberOfIterations);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.DsVarCoefficient, setting.DsVarCoefficient);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.NiNumberSteps, setting.NiNumberSteps);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.NiUMax, setting.NiUMax);
                            Assert.AreEqual(expectedSubMechanismSettings.Value.NiUMin, setting.NiUMin);
                        }
                    }
                }
            }
        }

        private static IDictionary<int, IDictionary<int, IDictionary<string, SubMechanismSettings>>> GetDictionary()
        {
            return new Dictionary<int, IDictionary<int, IDictionary<string, SubMechanismSettings>>>
            {
                {
                    1, new Dictionary<int, IDictionary<string, SubMechanismSettings>>
                    {
                        {
                            1, new Dictionary<string, SubMechanismSettings>
                            {
                                {
                                    "205", new SubMechanismSettings(1, 4, 50, 0.15, 0.01, 0.01, 0.01, 2, 20000, 100000, 0.1, -6, 6, 25)
                                }
                            }
                        }
                    }
                },
                {
                    11, new Dictionary<int, IDictionary<string, SubMechanismSettings>>
                    {
                        {
                            11, new Dictionary<string, SubMechanismSettings>
                            {
                                {
                                    "205", new SubMechanismSettings(2, 6, 30, 0.50, 0.06, 0.08, 0.02, 3, 1000, 2000, 0.4, -10, 10, 14)
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}