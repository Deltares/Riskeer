// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
            void Call() => ((HydraulicBoundaryDataEntity) null).Read(new HydraulicBoundaryData(), new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_HydraulicBoundaryDataNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new HydraulicBoundaryDataEntity();

            // Call
            void Call() => entity.Read(null, new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryData", exception.ParamName);
        }

        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new HydraulicBoundaryDataEntity();

            // Call
            void Call() => entity.Read(new HydraulicBoundaryData(), null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void Read_WithValidEntity_UpdatesHydraulicBoundaryData()
        {
            // Setup
            var random = new Random(21);
            var entity = new HydraulicBoundaryDataEntity
            {
                HydraulicLocationConfigurationDatabaseFilePath = "hlcdFilePath",
                HydraulicLocationConfigurationDatabaseScenarioName = "ScenarioName",
                HydraulicLocationConfigurationDatabaseYear = random.Next(),
                HydraulicLocationConfigurationDatabaseScope = "Scope",
                HydraulicLocationConfigurationDatabaseSeaLevel = "SeaLevel",
                HydraulicLocationConfigurationDatabaseRiverDischarge = "RiverDischarge",
                HydraulicLocationConfigurationDatabaseLakeLevel = "LakeLevel",
                HydraulicLocationConfigurationDatabaseWindDirection = "WindDirection",
                HydraulicLocationConfigurationDatabaseWindSpeed = "WindSpeed",
                HydraulicLocationConfigurationDatabaseComment = "Comment",
                HydraulicBoundaryDatabaseEntities =
                {
                    new HydraulicBoundaryDatabaseEntity()
                }
            };

            var hydraulicBoundaryData = new HydraulicBoundaryData();

            // Call
            entity.Read(hydraulicBoundaryData, new ReadConversionCollector());

            // Assert
            HydraulicLocationConfigurationDatabase database = hydraulicBoundaryData.HydraulicLocationConfigurationDatabase;
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseFilePath, database.FilePath);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseScenarioName, database.ScenarioName);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseYear, database.Year);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseScope, database.Scope);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseSeaLevel, database.SeaLevel);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseRiverDischarge, database.RiverDischarge);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseLakeLevel, database.LakeLevel);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseWindDirection, database.WindDirection);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseWindSpeed, database.WindSpeed);
            Assert.AreEqual(entity.HydraulicLocationConfigurationDatabaseComment, database.Comment);

            int expectedNrOfHydraulicBoundaryDatabases = entity.HydraulicBoundaryDatabaseEntities.Count;
            Assert.AreEqual(expectedNrOfHydraulicBoundaryDatabases, hydraulicBoundaryData.HydraulicBoundaryDatabases.Count);
        }
    }
}