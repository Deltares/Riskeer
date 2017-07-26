// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System.Collections.ObjectModel;
using System.IO;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.IO.SoilProfile;

namespace Ringtoets.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class SoilProfile1DReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(SoilProfile1DReader));

        [Test]
        public void Constructor_NonExistingPath_ThrowsCriticalFileReadException()
        {
            // Setup
            string testFile = Path.Combine(testDataPath, "does not exist");

            // Call
            TestDelegate test = () =>
            {
                using (new SoilProfile1DReader(testFile)) {}
            };

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            string expectedMessage = new FileReaderErrorMessageBuilder(testFile).Build("Het bestand bestaat niet.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_FileNullOrEmpty_ThrowsCriticalFileReadException(string fileName)
        {
            // Call
            TestDelegate test = () =>
            {
                using (new SoilProfile1DReader(fileName)) {}
            };

            // Assert
            Assert.Throws<CriticalFileReadException>(test);
        }

        [Test]
        public void Constructor_PathToExistingFile_ExpectedValues()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "emptySchema.soil");

            // Call
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                // Assert
                Assert.AreEqual(dbFile, reader.Path);
                Assert.IsInstanceOf<SqLiteDatabaseReaderBase>(reader);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void Initialize_IncorrectFormatFileOrInvalidSchema_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "text.txt");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                // Call
                TestDelegate test = () => reader.Initialize();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile).Build(
                    "Kon geen ondergrondschematisaties verkrijgen uit de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith1DAnd1DSoilProfileWithoutSoilLayers_ReturnOneProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithEmpty1d.soil");

            var result = new Collection<SoilProfile1D>();
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                while (reader.HasNext)
                {
                    result.Add(reader.ReadSoilProfile());
                }
            }

            // Assert
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("Profile", result[0].Name);
        }
    }
}