﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
    public class HydraulicLocationConfigurationDatabaseUpdateHelperTest
    {
        [Test]
        public void UpdateHydraulicLocationConfigurationDatabase_HydraulicLocationConfigurationDatabaseNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicLocationConfigurationDatabaseUpdateHelper.UpdateHydraulicLocationConfigurationDatabase(
                null, ReadHydraulicLocationConfigurationSettingsTestFactory.Create(), "");

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicLocationConfigurationDatabase", exception.ParamName);
        }

        [Test]
        public void UpdateHydraulicLocationConfigurationDatabase_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicLocationConfigurationDatabaseUpdateHelper.UpdateHydraulicLocationConfigurationDatabase(
                new HydraulicLocationConfigurationDatabase(), ReadHydraulicLocationConfigurationSettingsTestFactory.Create(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        public void UpdateHydraulicLocationConfigurationDatabase_ReadHydraulicLocationConfigurationDatabaseNull_SetsExpectedValuesAndLogsWarning()
        {
            // Setup
            const string filePath = "some/file/path";
            var hydraulicLocationConfigurationDatabase = new HydraulicLocationConfigurationDatabase();

            // Call
            Action call = () => HydraulicLocationConfigurationDatabaseUpdateHelper.UpdateHydraulicLocationConfigurationDatabase(
                hydraulicLocationConfigurationDatabase, null, filePath);

            // Assert
            const string expectedMessage = "De tabel 'ScenarioInformation' in het HLCD bestand is niet aanwezig.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(call, Tuple.Create(expectedMessage, LogLevelConstant.Warn), 1);

            Assert.AreEqual(filePath, hydraulicLocationConfigurationDatabase.FilePath);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.ScenarioName);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.Year);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.Scope);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.SeaLevel);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.RiverDischarge);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.LakeLevel);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.WindDirection);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.WindSpeed);
            Assert.IsNull(hydraulicLocationConfigurationDatabase.Comment);
        }

        [Test]
        public void UpdateHydraulicLocationConfigurationDatabase_ReadHydraulicLocationConfigurationDatabaseNotNull_SetsExpectedValuesAndDoesNotLog()
        {
            // Setup
            const string filePath = "some/file/path";
            var hydraulicLocationConfigurationDatabase = new HydraulicLocationConfigurationDatabase();
            ReadHydraulicLocationConfigurationSettings readSettings = ReadHydraulicLocationConfigurationSettingsTestFactory.Create();

            // Call
            Action call = () => HydraulicLocationConfigurationDatabaseUpdateHelper.UpdateHydraulicLocationConfigurationDatabase(
                hydraulicLocationConfigurationDatabase, readSettings, filePath);

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
        }
    }
}