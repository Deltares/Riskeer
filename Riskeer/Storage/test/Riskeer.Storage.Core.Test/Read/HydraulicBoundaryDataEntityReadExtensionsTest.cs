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
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class HydraulicBoundaryDataEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((HydraulicBoundaryDatabaseEntity) null).Read(new HydraulicBoundaryData());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new HydraulicBoundaryDatabaseEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Read_WithValidEntity_UpdatesHydraulicBoundaryDatabase()
        {
            // Setup
            var random = new Random(21);
            bool usePreprocessorClosure = random.NextBoolean();
            var entity = new HydraulicBoundaryDatabaseEntity
            {
                FilePath = "hrdFilePath",
                Version = "1.0",
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

            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            entity.Read(hydraulicBoundaryData);

            // Assert
            Assert.AreEqual(entity.FilePath, hydraulicBoundaryData.FilePath);
            Assert.AreEqual(entity.Version, hydraulicBoundaryData.Version);

            HydraulicLocationConfigurationDatabase database = hydraulicBoundaryData.HydraulicLocationConfigurationSettings;
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsFilePath, database.FilePath);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsScenarioName, database.ScenarioName);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsYear, database.Year);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsScope, database.Scope);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsSeaLevel, database.SeaLevel);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsRiverDischarge, database.RiverDischarge);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsLakeLevel, database.LakeLevel);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsWindDirection, database.WindDirection);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsWindSpeed, database.WindSpeed);
            Assert.AreEqual(entity.HydraulicLocationConfigurationSettingsComment, database.Comment);
            Assert.AreEqual(usePreprocessorClosure, database.UsePreprocessorClosure);
        }
    }
}