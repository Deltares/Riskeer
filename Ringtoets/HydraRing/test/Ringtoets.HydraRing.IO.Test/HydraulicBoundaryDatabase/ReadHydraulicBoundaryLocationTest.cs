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
using NUnit.Framework;
using Riskeer.HydraRing.IO.HydraulicBoundaryDatabase;

namespace Riskeer.HydraRing.IO.Test.HydraulicBoundaryDatabase
{
    [TestFixture]
    public class ReadHydraulicBoundaryLocationTest
    {
        [Test]
        public void Constructor_NameNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new ReadHydraulicBoundaryLocation(1, null, 2, 3);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("name", paramName);
        }

        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            const long id = 1;
            const string name = "name";
            var random = new Random(11);
            double xCoordinate = random.NextDouble();
            double yCoordinate = random.NextDouble();

            // Call
            var readHydraulicBoundaryLocation = new ReadHydraulicBoundaryLocation(id, name, xCoordinate, yCoordinate);

            // Assert
            Assert.AreEqual(id, readHydraulicBoundaryLocation.Id);
            Assert.AreEqual(name, readHydraulicBoundaryLocation.Name);
            Assert.AreEqual(xCoordinate, readHydraulicBoundaryLocation.CoordinateX);
            Assert.AreEqual(yCoordinate, readHydraulicBoundaryLocation.CoordinateY);
        }
    }
}