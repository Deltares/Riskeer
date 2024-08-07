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
using Riskeer.Storage.Core.DbContext;
using Riskeer.Storage.Core.Read;
using Riskeer.Storage.Core.TestUtil.Hydraulics;

namespace Riskeer.Storage.Core.Test.Read
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseEntityReadExtensionsTest
    {
        [Test]
        public void Read_EntityNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => ((HydraulicBoundaryDatabaseEntity) null).Read(new ReadConversionCollector());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("entity", exception.ParamName);
        }

        [Test]
        public void Read_CollectorNull_ThrowsArgumentNullException()
        {
            // Setup
            var entity = new HydraulicBoundaryDatabaseEntity();

            // Call
            void Call() => entity.Read(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("collector", exception.ParamName);
        }

        [Test]
        public void Read_WithValidEntity_UpdatesHydraulicBoundaryData()
        {
            // Setup
            var random = new Random(21);
            bool usePreprocessorClosure = random.NextBoolean();
            var entity = new HydraulicBoundaryDatabaseEntity
            {
                FilePath = "hrdFilePath",
                Version = "1.0",
                UsePreprocessorClosure = Convert.ToByte(usePreprocessorClosure),
                HydraulicLocationEntities =
                {
                    HydraulicLocationEntityTestFactory.CreateHydraulicLocationEntity()
                }
            };

            // Call
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = entity.Read(new ReadConversionCollector());

            // Assert
            Assert.AreEqual(entity.FilePath, hydraulicBoundaryDatabase.FilePath);
            Assert.AreEqual(entity.Version, hydraulicBoundaryDatabase.Version);
            Assert.AreEqual(usePreprocessorClosure, hydraulicBoundaryDatabase.UsePreprocessorClosure);

            int expectedNrOfHydraulicBoundaryLocations = entity.HydraulicLocationEntities.Count;
            Assert.AreEqual(expectedNrOfHydraulicBoundaryLocations, hydraulicBoundaryDatabase.Locations.Count);
        }
    }
}