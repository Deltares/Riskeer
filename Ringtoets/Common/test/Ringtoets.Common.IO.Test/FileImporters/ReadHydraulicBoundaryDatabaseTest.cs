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

using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.IO.FileImporters;

namespace Ringtoets.Common.IO.Test.FileImporters
{
    [TestFixture]
    public class ReadHydraulicBoundaryDatabaseTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const string version = "version";
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = Enumerable.Empty<HydraulicBoundaryLocation>();

            // Call
            var readHydraulicBoundaryDatabase = new ReadHydraulicBoundaryDatabase(version, hydraulicBoundaryLocations);

            // Assert
            Assert.AreEqual(version, readHydraulicBoundaryDatabase.Version);
            Assert.AreSame(hydraulicBoundaryLocations, readHydraulicBoundaryDatabase.Locations);
        }
    }
}