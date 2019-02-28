// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Riskeer.Common.Data.Hydraulics;

namespace Riskeer.Common.Data.TestUtil.Test
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseTestHelperTest
    {
        [Test]
        public void SetHydraulicBoundaryLocationConfigurationSettings_DatabaseWithFilePathAndWithoutUsePreprocessorClosure_SetsExpectedValues()
        {
            // Setup
            const string path = "C:\\TestPath";
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(path, "hrd.sqlite")
            };

            // Call
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            // Assert
            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(Path.Combine(path, "hlcd.sqlite"), settings.FilePath);
            Assert.AreEqual("ScenarioName", settings.ScenarioName);
            Assert.AreEqual(1337, settings.Year);
            Assert.AreEqual("Scope", settings.Scope);
            Assert.IsFalse(settings.UsePreprocessorClosure);
            Assert.AreEqual("SeaLevel", settings.SeaLevel);
            Assert.AreEqual("RiverDischarge", settings.RiverDischarge);
            Assert.AreEqual("LakeLevel", settings.LakeLevel);
            Assert.AreEqual("WindDirection", settings.WindDirection);
            Assert.AreEqual("WindSpeed", settings.WindSpeed);
            Assert.AreEqual("Comment", settings.Comment);
        }
        [Test]
        public void SetHydraulicBoundaryLocationConfigurationSettings_DatabaseWithFilePathAndWithUsePreprocessorClosure_SetsExpectedValues()
        {
            // Setup
            const string path = "C:\\TestPath";
            const bool usePreprocessorClosure = true;
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = Path.Combine(path, "hrd.sqlite")
            };

            // Call
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase, usePreprocessorClosure);

            // Assert
            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(Path.Combine(path, "hlcd.sqlite"), settings.FilePath);
            Assert.AreEqual("ScenarioName", settings.ScenarioName);
            Assert.AreEqual(1337, settings.Year);
            Assert.AreEqual("Scope", settings.Scope);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
            Assert.AreEqual("SeaLevel", settings.SeaLevel);
            Assert.AreEqual("RiverDischarge", settings.RiverDischarge);
            Assert.AreEqual("LakeLevel", settings.LakeLevel);
            Assert.AreEqual("WindDirection", settings.WindDirection);
            Assert.AreEqual("WindSpeed", settings.WindSpeed);
            Assert.AreEqual("Comment", settings.Comment);
        }

        [Test]
        public void SetHydraulicBoundaryLocationConfigurationSettings_DatabaseWithoutFilePath_ThrowsArgumentException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            TestDelegate call = () => HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(hydraulicBoundaryDatabase);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "FilePath must be set.");
        }
    }
}