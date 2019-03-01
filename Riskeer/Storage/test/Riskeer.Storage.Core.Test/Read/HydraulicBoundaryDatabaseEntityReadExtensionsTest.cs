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
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => ((HydraulicBoundaryDatabaseEntity) null).Read(new HydraulicBoundaryDatabase());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new HydraulicBoundaryDatabaseEntity();

            // Call
            TestDelegate call = () => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Read_WithValidEntity_UpdatesHydraulicBoundaryDatabase()
        {
            // Setup
            var random = new Random(21);
            bool usePreprocessorClosure = random.NextBoolean();
            var entity = new HydraulicBoundaryDatabaseEntity
            {
                FilePath = "hydraulicBoundaryDatabaseFilePath",
                Version = "hydraulicBoundaryDatabaseVersion",
                HydraulicLocationConfigurationSettingsFilePath = "hlcdFilePath",
                HydraulicLocationConfigurationSettingsUsePreprocessorClosure = Convert.ToByte(usePreprocessorClosure),
                HydraulicLocationConfigurationSettingsScenarioName = "ScenarioName",
                HydraulicLocationConfigurationSettingsYear = random.Next(),
                HydraulicLocationConfigurationSettingsScope = "Scope",
                HydraulicLocationConfigurationSettingsSeaLevel = "SeaLevel",
                HydraulicLocationConfigurationSettingsRiverDischarge = "RiverDischarge",
                HydraulicLocationConfigurationSettingsLakeLevel = "LakeLevel",
                HydraulicLocationConfigurationSettingsWindDirection = "WindDirection",
                HydraulicLocationConfigurationSettingsWindSpeed = "WindSpeed",
                HydraulicLocationConfigurationSettingsComment = "Comment"
            };

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            entity.Read(hydraulicBoundaryDatabase);

            // Assert
            Assert.AreEqual(entity.FilePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(entity.Version, hydraulicBoundaryDatabase.Version);

            HydraulicLocationConfigurationSettings settings = hydraulicBoundaryDatabase.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsFilePath, settings.FilePath);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsScenarioName, settings.ScenarioName);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsYear, settings.Year);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsScope, settings.Scope);
            Assert.AreEqual(usePreprocessorClosure, settings.UsePreprocessorClosure);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsSeaLevel, settings.SeaLevel);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsRiverDischarge, settings.RiverDischarge);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsLakeLevel, settings.LakeLevel);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsWindDirection, settings.WindDirection);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsWindSpeed, settings.WindSpeed);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsComment, settings.Comment);
        }
    }
}