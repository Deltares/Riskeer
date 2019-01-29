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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;
using Riskeer.HydraRing.IO.TestUtil;
using Riskeer.Integration.Plugin.Helpers;

namespace Riskeer.Integration.Plugin.Test.Helpers
{
    [TestFixture]
    public class HydraulicLocationConfigurationSettingsUpdateHelperTest
    {
        [Test]
        public void SetHydraulicLocationConfigurationSettings_HydraulicLocationConfigurationSettingsNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicLocationConfigurationSettingsUpdateHelper.SetHydraulicLocationConfigurationSettings(
                null, ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicLocationConfigurationSettings", exception.ParamName);
        }

        [Test]
        public void SetHydraulicLocationConfigurationSettings_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicLocationConfigurationSettingsUpdateHelper.SetHydraulicLocationConfigurationSettings(
                new HydraulicLocationConfigurationSettings(), ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        public void SetHydraulicLocationConfigurationSettings_ReadHydraulicLocationConfigurationDatabaseSettingsNull_SetDefaultValuesAndLogsWarning()
        {
            // Setup
            const string filePath = "some/file/path";
            var settings = new HydraulicLocationConfigurationSettings();
            
            // Call
            Action call = () => HydraulicLocationConfigurationSettingsUpdateHelper.SetHydraulicLocationConfigurationSettings(settings, null, filePath);

            // Assert
            const string expectedMessage = "De tabel 'ScenarioInformation' in het HLCD bestand is niet aanwezig, er worden standaardwaarden " +
                                           "conform WBI2017 voor de HLCD bestand informatie gebruikt.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);

            Assert.AreEqual(filePath, settings.FilePath);
            Assert.AreEqual("WBI2017", settings.ScenarioName);
            Assert.AreEqual(2023, settings.Year);
            Assert.AreEqual("WBI2017", settings.Scope);
            Assert.AreEqual("Conform WBI2017", settings.SeaLevel);
            Assert.AreEqual("Conform WBI2017", settings.RiverDischarge);
            Assert.AreEqual("Conform WBI2017", settings.LakeLevel);
            Assert.AreEqual("Conform WBI2017", settings.WindDirection);
            Assert.AreEqual("Conform WBI2017", settings.WindSpeed);
            Assert.AreEqual("Gegenereerd door Ringtoets (conform WBI2017)", settings.Comment);
        }

        [Test]
        public void SetHydraulicLocationConfigurationSettings_ReadHydraulicLocationConfigurationDatabaseSettingsNotNull_SetExpectedValuesAndDoesNotLog()
        {
            // Setup
            const string filePath = "some/file/path";
            var settings = new HydraulicLocationConfigurationSettings();
            ReadHydraulicLocationConfigurationDatabaseSettings readSettings = ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create();

            // Call
            Action call = () => HydraulicLocationConfigurationSettingsUpdateHelper.SetHydraulicLocationConfigurationSettings(settings, readSettings, filePath);

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);

            Assert.AreEqual(filePath, settings.FilePath);
            Assert.AreEqual(readSettings.ScenarioName, settings.ScenarioName);
            Assert.AreEqual(readSettings.Year, settings.Year);
            Assert.AreEqual(readSettings.Scope, settings.Scope);
            Assert.AreEqual(readSettings.SeaLevel, settings.SeaLevel);
            Assert.AreEqual(readSettings.RiverDischarge, settings.RiverDischarge);
            Assert.AreEqual(readSettings.LakeLevel, settings.LakeLevel);
            Assert.AreEqual(readSettings.WindDirection, settings.WindDirection);
            Assert.AreEqual(readSettings.WindSpeed, settings.WindSpeed);
            Assert.AreEqual(readSettings.Comment, settings.Comment);
        }
    }
}