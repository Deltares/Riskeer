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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicLocationConfigurationDatabase;

namespace Ringtoets.HydraRing.IO.Test.HydraulicLocationConfigurationDatabase
{
    [TestFixture]
    public class ReadHydraulicLocationConfigurationDatabaseTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            IEnumerable<ReadHydraulicLocationMapping> locationIdMappings = Enumerable.Empty<ReadHydraulicLocationMapping>();
            IEnumerable<ReadHydraulicLocationConfigurationDatabaseSettings> databaseSettings = Enumerable.Empty<ReadHydraulicLocationConfigurationDatabaseSettings>();

            // Call
            var readDatabase = new ReadHydraulicLocationConfigurationDatabase(locationIdMappings, databaseSettings);

            // Assert
            Assert.AreSame(locationIdMappings, readDatabase.LocationIdMappings);
            Assert.AreSame(databaseSettings, readDatabase.ReadHydraulicLocationConfigurationDatabaseSettings);
        }
    }
}