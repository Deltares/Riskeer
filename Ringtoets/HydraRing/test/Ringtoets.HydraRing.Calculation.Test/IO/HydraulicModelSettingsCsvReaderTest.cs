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
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.IO;

namespace Ringtoets.HydraRing.Calculation.Test.IO
{
    [TestFixture]
    public class HydraulicModelSettingsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Settings");

        [Test]
        public void Constructor_PathSet_DoesNotThrowArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicModelSettingsCsvReader("path.csv");

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Constructor_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicModelSettingsCsvReader(null);

            // Assert
            const string expectedMessage = "A file must be set.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void ReadSettings_ValidFile_ReturnsSettings()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "HydraulicModelSettingsTest.csv");

            using (StreamReader streamReader = new StreamReader(testFile))
            {
                string fileContents = streamReader.ReadToEnd();

                HydraulicModelSettingsCsvReader reader = new HydraulicModelSettingsCsvReader(fileContents);
                IDictionary<int, IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>> expectedDictionary = GetDictionary();

                // Call
                IDictionary<int, IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>> settings = reader.ReadSettings();

                // Assert
                Assert.AreEqual(2, settings.Count);

                foreach (KeyValuePair<int, IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>> expectedMechanism in expectedDictionary)
                {
                    Assert.IsTrue(settings.ContainsKey(expectedMechanism.Key));
                    Assert.IsInstanceOf<IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>>(settings[expectedMechanism.Key]);

                    foreach (KeyValuePair<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>> expectedSubMechanism in expectedMechanism.Value)
                    {
                        Assert.IsTrue(settings[expectedMechanism.Key].ContainsKey(expectedSubMechanism.Key));
                        Assert.IsInstanceOf<IDictionary<string, HydraRingTimeIntegrationSchemeType>>(settings[expectedMechanism.Key][expectedSubMechanism.Key]);

                        foreach (KeyValuePair<string, HydraRingTimeIntegrationSchemeType> expectedHydraRingTimeIntegrationSchemeType in expectedSubMechanism.Value)
                        {
                            Assert.IsTrue(settings[expectedMechanism.Key][expectedSubMechanism.Key].ContainsKey(expectedHydraRingTimeIntegrationSchemeType.Key));
                            Assert.IsInstanceOf<HydraRingTimeIntegrationSchemeType>(settings[expectedMechanism.Key][expectedSubMechanism.Key][expectedHydraRingTimeIntegrationSchemeType.Key]);

                            HydraRingTimeIntegrationSchemeType setting = settings[expectedMechanism.Key][expectedSubMechanism.Key][expectedHydraRingTimeIntegrationSchemeType.Key];

                            Assert.AreEqual(expectedHydraRingTimeIntegrationSchemeType.Value, setting);
                        }
                    }
                }
            }
        }

        private IDictionary<int, IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>> GetDictionary()
        {
            return new Dictionary<int, IDictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>>
            {
                {
                    1, new Dictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>
                    {
                        {
                            1, new Dictionary<string, HydraRingTimeIntegrationSchemeType>
                            {
                                {
                                    "205", HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta
                                },
                                {
                                    "11-1", HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta
                                }
                            }
                        }
                    }
                },
                {
                    11, new Dictionary<int, IDictionary<string, HydraRingTimeIntegrationSchemeType>>
                    {
                        {
                            11, new Dictionary<string, HydraRingTimeIntegrationSchemeType>
                            {
                                {
                                    "205", HydraRingTimeIntegrationSchemeType.FerryBorgesCastanheta
                                },
                                {
                                    "11-1", HydraRingTimeIntegrationSchemeType.NumericalTimeIntegration
                                }
                            }
                        }
                    }
                }
            };
        }
    }
}