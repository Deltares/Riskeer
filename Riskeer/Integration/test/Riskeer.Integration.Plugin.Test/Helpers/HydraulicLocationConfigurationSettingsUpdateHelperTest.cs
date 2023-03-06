// Copyright (C) Stichting Deltares 2022. All rights reserved.
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
                null, ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create(), false, "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicLocationConfigurationSettings", exception.ParamName);
        }

        [Test]
        public void SetHydraulicLocationConfigurationSettings_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicLocationConfigurationSettingsUpdateHelper.SetHydraulicLocationConfigurationSettings(
                new HydraulicLocationConfigurationDatabase(), ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create(), false, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        public void SetHydraulicLocationConfigurationSettings_ReadHydraulicLocationConfigurationDatabaseSettingsNull_SetDefaultValuesAndLogsWarning()
        {
            // Setup
            const string filePath = "some/file/path";
            var hydraulicLocationConfigurationDatabase = new HydraulicLocationConfigurationDatabase();
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            // Call
            Action call = () => HydraulicLocationConfigurationSettingsUpdateHelper.SetHydraulicLocationConfigurationSettings(
                hydraulicLocationConfigurationDatabase, null, usePreprocessorClosure, filePath);

            // Assert
            const string expectedMessage = "De tabel 'ScenarioInformation' in het HLCD bestand is niet aanwezig. Er worden standaardwaarden " +
                                           "conform WBI2017 gebruikt voor de HLCD bestandsinformatie.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);

            Assert.AreEqual(filePath, hydraulicLocationConfigurationDatabase.FilePath);
            Assert.AreEqual("WBI2017", hydraulicLocationConfigurationDatabase.ScenarioName);
            Assert.AreEqual(2023, hydraulicLocationConfigurationDatabase.Year);
            Assert.AreEqual("WBI2017", hydraulicLocationConfigurationDatabase.Scope);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.SeaLevel);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.RiverDischarge);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.LakeLevel);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.WindDirection);
            Assert.AreEqual("Conform WBI2017", hydraulicLocationConfigurationDatabase.WindSpeed);
            Assert.AreEqual("Gegenereerd door Riskeer (conform WBI2017)", hydraulicLocationConfigurationDatabase.Comment);
            Assert.AreEqual(usePreprocessorClosure, hydraulicLocationConfigurationDatabase.UsePreprocessorClosure);
        }

        [Test]
        public void SetHydraulicLocationConfigurationSettings_ReadHydraulicLocationConfigurationDatabaseSettingsNotNull_SetExpectedValuesAndDoesNotLog()
        {
            // Setup
            const string filePath = "some/file/path";
            var hydraulicLocationConfigurationDatabase = new HydraulicLocationConfigurationDatabase();
            ReadHydraulicLocationConfigurationDatabaseSettings readSettings = ReadHydraulicLocationConfigurationDatabaseSettingsTestFactory.Create();
            bool usePreprocessorClosure = new Random(21).NextBoolean();

            // Call
            Action call = () => HydraulicLocationConfigurationSettingsUpdateHelper.SetHydraulicLocationConfigurationSettings(
                hydraulicLocationConfigurationDatabase, readSettings, usePreprocessorClosure, filePath);

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);

            Assert.AreEqual(filePath, hydraulicLocationConfigurationDatabase.FilePath);
            Assert.AreEqual(readSettings.ScenarioName, hydraulicLocationConfigurationDatabase.ScenarioName);
            Assert.AreEqual(readSettings.Year, hydraulicLocationConfigurationDatabase.Year);
            Assert.AreEqual(readSettings.Scope, hydraulicLocationConfigurationDatabase.Scope);
            Assert.AreEqual(readSettings.SeaLevel, hydraulicLocationConfigurationDatabase.SeaLevel);
            Assert.AreEqual(readSettings.RiverDischarge, hydraulicLocationConfigurationDatabase.RiverDischarge);
            Assert.AreEqual(readSettings.LakeLevel, hydraulicLocationConfigurationDatabase.LakeLevel);
            Assert.AreEqual(readSettings.WindDirection, hydraulicLocationConfigurationDatabase.WindDirection);
            Assert.AreEqual(readSettings.WindSpeed, hydraulicLocationConfigurationDatabase.WindSpeed);
            Assert.AreEqual(readSettings.Comment, hydraulicLocationConfigurationDatabase.Comment);
            Assert.AreEqual(usePreprocessorClosure, hydraulicLocationConfigurationDatabase.UsePreprocessorClosure);
        }
    }
}