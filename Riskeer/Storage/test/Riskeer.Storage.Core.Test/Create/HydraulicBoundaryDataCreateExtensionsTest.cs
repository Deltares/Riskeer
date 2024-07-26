// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
            void Call() => ((HydraulicBoundaryData) null).Create(new PersistenceRegistry());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            void Call() => hydraulicBoundaryData.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_ValidHydraulicBoundaryData_ReturnsHydraulicBoundaryDataEntity()
        {
            // Setup
            var hydraulicBoundaryData = new HydraulicBoundaryData
            {
                HydraulicLocationConfigurationDatabase =
                {
                    FilePath = "hlcdFilePath",
                    ScenarioName = "ScenarioName",
                    Year = new Random(21).Next(),
                    Scope = "Scope",
                    SeaLevel = "SeaLevel",
                    RiverDischarge = "RiverDischarge",
                    LakeLevel = "LakeLevel",
                    WindDirection = "WindDirection",
                    WindSpeed = "WindSpeed",
                    Comment = "Comment"
                },
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase()
                }
            };

            // Call
            HydraulicBoundaryDataEntity entity = hydraulicBoundaryData.Create(new PersistenceRegistry());

            // Assert
            HydraulicLocationConfigurationDatabase hydraulicLocationConfigurationDatabase = hydraulicBoundaryData.HydraulicLocationConfigurationDatabase;
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.FilePath, entity.HydraulicLocationConfigurationDatabaseFilePath);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.ScenarioName, entity.HydraulicLocationConfigurationDatabaseScenarioName);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.Year, entity.HydraulicLocationConfigurationDatabaseYear);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.Scope, entity.HydraulicLocationConfigurationDatabaseScope);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.SeaLevel, entity.HydraulicLocationConfigurationDatabaseSeaLevel);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.RiverDischarge, entity.HydraulicLocationConfigurationDatabaseRiverDischarge);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.LakeLevel, entity.HydraulicLocationConfigurationDatabaseLakeLevel);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.WindDirection, entity.HydraulicLocationConfigurationDatabaseWindDirection);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.WindSpeed, entity.HydraulicLocationConfigurationDatabaseWindSpeed);
            TestHelper.AssertAreEqualButNotSame(hydraulicLocationConfigurationDatabase.Comment, entity.HydraulicLocationConfigurationDatabaseComment);

            int expectedNrOfHydraulicBoundaryLocations = hydraulicBoundaryData.HydraulicBoundaryDatabases.Count;
            Assert.AreEqual(expectedNrOfHydraulicBoundaryLocations, entity.HydraulicBoundaryDatabaseEntities.Count);
        }
    }
}