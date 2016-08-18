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
    public class DesignTablesSettingsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.Calculation, "Settings");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            DesignTablesSettingsCsvReader reader = new DesignTablesSettingsCsvReader("path.csv");

            // Assert
            Assert.IsInstanceOf<HydraRingSettingsCsvReader<IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>>>>(reader);
        }

        [Test]
        public void Constructor_PathNotSet_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DesignTablesSettingsCsvReader(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "File contents must be set.");
        }

        [Test]
        public void ReadSettings_ValidFile_ReturnsSettings()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "DesignTablesSettingsTest.csv");

            using (StreamReader streamReader = new StreamReader(testFile))
            {
                string fileContents = streamReader.ReadToEnd();

                DesignTablesSettingsCsvReader reader = new DesignTablesSettingsCsvReader(fileContents);
                IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>> expectedDictionary = GetDictionary();

                // Call
                IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>> settings = reader.ReadSettings();

                // Assert
                Assert.AreEqual(2, settings.Count);

                foreach (KeyValuePair<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>> expectedMechanism in expectedDictionary)
                {
                    Assert.IsTrue(settings.ContainsKey(expectedMechanism.Key));
                    Assert.IsInstanceOf<IDictionary<string, DesignTablesSetting>>(settings[expectedMechanism.Key]);

                    foreach (KeyValuePair<string, DesignTablesSetting> expectedDesignTablesSetting in expectedMechanism.Value)
                    {
                        Assert.IsTrue(settings[expectedMechanism.Key].ContainsKey(expectedDesignTablesSetting.Key));
                        Assert.IsInstanceOf<DesignTablesSetting>(settings[expectedMechanism.Key][expectedDesignTablesSetting.Key]);

                        DesignTablesSetting setting = settings[expectedMechanism.Key][expectedDesignTablesSetting.Key];

                        Assert.AreEqual(expectedDesignTablesSetting.Value.ValueMin, setting.ValueMin);
                        Assert.AreEqual(expectedDesignTablesSetting.Value.ValueMax, setting.ValueMax);
                    }
                }
            }
        }

        private static IDictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>> GetDictionary()
        {
            return new Dictionary<HydraRingFailureMechanismType, IDictionary<string, DesignTablesSetting>>
            {
                {
                    HydraRingFailureMechanismType.AssessmentLevel, new Dictionary<string, DesignTablesSetting>
                    {
                        {
                            "205", new DesignTablesSetting(5, 15)
                        },
                        {
                            "11-1", new DesignTablesSetting(5, 15)
                        }
                    }
                },
                {
                    HydraRingFailureMechanismType.QVariant, new Dictionary<string, DesignTablesSetting>
                    {
                        {
                            "205", new DesignTablesSetting(5, 15)
                        },
                        {
                            "11-1", new DesignTablesSetting(5, 15)
                        }
                    }
                }
            };
        }
    }
}