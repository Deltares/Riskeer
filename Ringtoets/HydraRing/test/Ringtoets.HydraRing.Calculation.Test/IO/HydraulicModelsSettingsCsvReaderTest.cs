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
using Ringtoets.HydraRing.Calculation.Data.Settings;
using Ringtoets.HydraRing.Calculation.IO;

namespace Ringtoets.HydraRing.Calculation.Test.IO
{
    [TestFixture]
    public class HydraulicModelsSettingsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Settings");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            HydraulicModelsSettingsCsvReader reader = new HydraulicModelsSettingsCsvReader("path.csv");

            // Assert
            Assert.IsInstanceOf<HydraRingSettingsCsvReader<IDictionary<HydraRingFailureMechanismType, IDictionary<string, TimeIntegrationSetting>>>>(reader);
        }

        [Test]
        public void Constructor_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraulicModelsSettingsCsvReader(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "File contents must be set.");
        }

        [Test]
        public void ReadSettings_ValidFile_ReturnsSettings()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "HydraulicModelsSettingsTest.csv");

            using (StreamReader streamReader = new StreamReader(testFile))
            {
                string fileContents = streamReader.ReadToEnd();

                var reader = new HydraulicModelsSettingsCsvReader(fileContents);
                IDictionary<HydraRingFailureMechanismType, IDictionary<string, TimeIntegrationSetting>> expectedDictionary = GetDictionary();

                // Call
                IDictionary<HydraRingFailureMechanismType, IDictionary<string, TimeIntegrationSetting>> settings = reader.ReadSettings();

                // Assert
                Assert.AreEqual(10, settings.Count);

                foreach (KeyValuePair<HydraRingFailureMechanismType, IDictionary<string, TimeIntegrationSetting>> expectedMechanism in expectedDictionary)
                {
                    Assert.IsTrue(settings.ContainsKey(expectedMechanism.Key));
                    Assert.IsInstanceOf<IDictionary<string, TimeIntegrationSetting>>(settings[expectedMechanism.Key]);

                    foreach (KeyValuePair<string, TimeIntegrationSetting> expectedDesignTablesSetting in expectedMechanism.Value)
                    {
                        Assert.IsTrue(settings[expectedMechanism.Key].ContainsKey(expectedDesignTablesSetting.Key));
                        Assert.IsInstanceOf<TimeIntegrationSetting>(settings[expectedMechanism.Key][expectedDesignTablesSetting.Key]);

                        TimeIntegrationSetting setting = settings[expectedMechanism.Key][expectedDesignTablesSetting.Key];

                        Assert.AreEqual(expectedDesignTablesSetting.Value.TimeIntegrationSchemeId, setting.TimeIntegrationSchemeId);
                    }
                }
            }
        }

        private static IDictionary<HydraRingFailureMechanismType, IDictionary<string, TimeIntegrationSetting>> GetDictionary()
        {
            return new Dictionary<HydraRingFailureMechanismType, IDictionary<string, TimeIntegrationSetting>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(1)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(1)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveHeight, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(2)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WavePeakPeriod, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(3)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.WaveSpectralPeriod, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(1)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesHeight, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(1)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.DikesOvertopping, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(1)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresOvertopping, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(1)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresClosure, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(2)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.StructuresStructuralFailure, new Dictionary<string, TimeIntegrationSetting>
                    {
                        {
                            "205", new TimeIntegrationSetting(2)
                        }
                    }
                }
            };
        }
    }
}