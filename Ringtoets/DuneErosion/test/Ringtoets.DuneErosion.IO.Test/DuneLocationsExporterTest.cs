// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Data.TestUtil;

namespace Ringtoets.DuneErosion.IO.Test
{
    [TestFixture]
    public class DuneLocationsExporterTest
    {
        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.DuneErosion.IO, "DuneLocationsExporter");

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var filePath = Path.Combine(testDataPath, "tets.bnd");

            // Call
            var exporter = new DuneLocationsExporter(Enumerable.Empty<DuneLocation>(), filePath);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_LocationsNull_ThrowArgumentNullException()
        {
            // Setup
            var filePath = Path.Combine(testDataPath, "tets.bnd");

            // Call
            TestDelegate test = () => new DuneLocationsExporter(null, filePath);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("duneLocations", exception.ParamName);
        }

        [Test]
        public void Constructor_LocationWithoutOutput_ThrowArgumentException()
        {
            // Setup
            var filePath = Path.Combine(testDataPath, "tets.bnd");

            var location1 = new TestDuneLocation
            {
                Output = new TestDuneLocationOutput()
            };
            var location2 = new TestDuneLocation();

            // Call
            TestDelegate test = () => new DuneLocationsExporter(new[]
                                                                {
                                                                    location1,
                                                                    location2
                                                                }, filePath);

            // Assert
            var expectedMessage = "Locations should contain output";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathInvalid_ThrowArgumentException()
        {
            // Call
            TestDelegate test = () => new DuneLocationsExporter(Enumerable.Empty<DuneLocation>(), null);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }
    }
}