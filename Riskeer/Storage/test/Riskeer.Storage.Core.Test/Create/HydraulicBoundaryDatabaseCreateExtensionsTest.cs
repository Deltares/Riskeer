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
    public class HydraulicBoundaryDatabaseCreateExtensionsTest
    {
        [Test]
        public void Create_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((HydraulicBoundaryDatabase) null).Create(new PersistenceRegistry(), 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        public void Create_RegistryNull_ThrowsArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            // Call
            void Call() => hydraulicBoundaryDatabase.Create(null, 0);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("registry", exception.ParamName);
        }

        [Test]
        public void Create_ValidHydraulicBoundaryDatabase_ReturnsHydraulicBoundaryDatabaseEntity()
        {
            // Setup
            var random = new Random(21);
            int order = random.Next();

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = "hrdFilePath",
                Version = "version",
                UsePreprocessorClosure = random.NextBoolean(),
                Locations =
                {
                    new HydraulicBoundaryLocation(-1, "name", 1, 2)
                }
            };

            // Call
            HydraulicBoundaryDatabaseEntity entity = hydraulicBoundaryDatabase.Create(new PersistenceRegistry(), order);

            // Assert
            TestHelper.AssertAreEqualButNotSame(hydraulicBoundaryDatabase.FilePath, entity.FilePath);
            TestHelper.AssertAreEqualButNotSame(hydraulicBoundaryDatabase.Version, entity.Version);
            TestHelper.AssertAreEqualButNotSame(Convert.ToByte(hydraulicBoundaryDatabase.UsePreprocessorClosure), entity.UsePreprocessorClosure);
            TestHelper.AssertAreEqualButNotSame(order, entity.Order);

            int expectedNrOfHydraulicBoundaryLocations = hydraulicBoundaryDatabase.Locations.Count;
            Assert.AreEqual(expectedNrOfHydraulicBoundaryLocations, entity.HydraulicLocationEntities.Count);
        }
    }
}