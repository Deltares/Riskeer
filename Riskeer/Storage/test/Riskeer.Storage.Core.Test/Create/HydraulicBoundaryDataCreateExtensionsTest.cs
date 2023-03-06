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
using Riskeer.Storage.Core.Create;
using Riskeer.Storage.Core.DbContext;

namespace Riskeer.Storage.Core.Test.Create
{
    [TestFixture]
    public class HydraulicBoundaryDataCreateExtensionsTest
    {
        [Test]
        public void Create_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((HydraulicBoundaryData) null).Create();

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Create_ValidHydraulicBoundaryDatabase_ReturnsHydraulicBoundaryDatabaseEntity()
        {
            // Setup
            var random = new Random(21);
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                FilePath = "hrdFilePath",
                Version = "Version",
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = "hlcdFilePath",
                    ScenarioName = "ScenarioName",
                    Year = random.Next(),
                    Scope = "Scope",
                    SeaLevel = "SeaLevel",
                    RiverDischarge = "RiverDischarge",
                    LakeLevel = "LakeLevel",
                    WindDirection = "WindDirection",
                    WindSpeed = "WindSpeed",
                    Comment = "Comment",
                    UsePreprocessorClosure = random.NextBoolean()
                }
            };

            // Call
            HydraulicBoundaryDatabaseEntity entity = hydraulicBoundaryData.Create();

            // Assert
            TestHelper.AssertAreEqualButNotSame(hydraulicBoundaryData.FilePath, entity.FilePath);
            TestHelper.AssertAreEqualButNotSame(hydraulicBoundaryData.Version, entity.Version);

            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = hydraulicBoundaryData.HydraulicLocationConfigurationDatabase;
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.FilePath, entity.HydraulicLocationConfigurationSettingsFilePath);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.ScenarioName, entity.HydraulicLocationConfigurationSettingsScenarioName);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.Year, entity.HydraulicLocationConfigurationSettingsYear);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.Scope, entity.HydraulicLocationConfigurationSettingsScope);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.SeaLevel, entity.HydraulicLocationConfigurationSettingsSeaLevel);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.RiverDischarge, entity.HydraulicLocationConfigurationSettingsRiverDischarge);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.LakeLevel, entity.HydraulicLocationConfigurationSettingsLakeLevel);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.WindDirection, entity.HydraulicLocationConfigurationSettingsWindDirection);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.WindSpeed, entity.HydraulicLocationConfigurationSettingsWindSpeed);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.Comment, entity.HydraulicLocationConfigurationSettingsComment);
            Assert.AreEqual(Convert.ToByte(hydraulicLocationConfigurationDatabase.UsePreprocessorClosure), entity.HydraulicLocationConfigurationSettingsUsePreprocessorClosure);
        }
    }
}