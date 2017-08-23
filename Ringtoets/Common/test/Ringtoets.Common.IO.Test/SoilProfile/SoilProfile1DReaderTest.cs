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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.IO.Exceptions;
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
        public void Constructor_InvalidPath_ThrowsCriticalFileReadException(string fileName)
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
        public void ReadSoilProfile_IncorrectCriticalProperty_ThrowsCriticalFileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileInvalidId.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(test);

                string expectedMessage = new FileReaderErrorMessageBuilder(dbFile)
                    .WithSubject("ondergrondschematisatie 'Profile'")
                    .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
                Assert.AreEqual(expectedMessage, exception.Message);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void GivenReadSoilProfileThrowsException_WhenReadingNextProfile_ReturnsNextProfile()
        {
            // Given
            string dbFile = Path.Combine(testDataPath, "1dprofileWithInvalidLayerProperty.soil");

            SoilProfileReadException exception = null;
            var readSoilProfiles = new List<SoilProfile1D>();
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // When
                try
                {
                    reader.ReadSoilProfile();
                }
                catch (SoilProfileReadException e)
                {
                    exception = e;
                }

                // Then
                readSoilProfiles.Add(reader.ReadSoilProfile());
            }

            Assert.IsInstanceOf<SoilProfileReadException>(exception);
            Assert.AreEqual("Profile", exception.ProfileName);
            Assert.AreEqual(1, readSoilProfiles.Count);
            Assert.AreEqual("Profile2", readSoilProfiles[0].Name);

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

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_LayerTopInvalidValue_ThrowsSoilProfileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithInvalidTopLayer.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<SoilProfileReadException>(test);
                const string expectedMessage = "Het lezen van de ondergrondschematisatie 'INCORRECT' is mislukt. " +
                                               "Geen geldige waarde in kolom 'Top'.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.AreEqual("INCORRECT", exception.ProfileName);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_BottomInvalidValue_ThrowsSoilProfileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithInvalidBottom.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<SoilProfileReadException>(test);
                const string expectedMessage = "Het lezen van de ondergrondschematisatie 'Profile' is mislukt. " +
                                               "Geen geldige waarde in kolom 'Bottom'.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.AreEqual("Profile", exception.ProfileName);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_BottomAboveLayers_ThrowsSoilProfileReadException()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofileWithIncorrectBottom.soil");

            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                TestDelegate test = () => reader.ReadSoilProfile();

                // Assert
                var exception = Assert.Throws<SoilProfileReadException>(test);
                Assert.AreEqual("Het uitlezen van de ondergrondschematisatie is mislukt.", exception.Message);
                Assert.AreEqual("Profile", exception.ProfileName);
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }

        [Test]
        public void ReadSoilProfile_DatabaseWith1DProfile3Layers_ReturnsProfile()
        {
            // Setup
            string dbFile = Path.Combine(testDataPath, "1dprofile.soil");
            using (var reader = new SoilProfile1DReader(dbFile))
            {
                reader.Initialize();

                // Call
                SoilProfile1D profile = reader.ReadSoilProfile();

                // Assert
                CollectionAssert.AreEqual(new[]
                {
                    false,
                    false,
                    true
                }, profile.Layers.Select(l => l.IsAquifer));
                CollectionAssert.AreEqual(new[]
                {
                    Color.FromArgb(128, 255, 128),
                    Color.FromArgb(255, 0, 0),
                    Color.FromArgb(70, 130, 180)
                }, profile.Layers.Select(l => l.Color));
                CollectionAssert.AreEqual(new[]
                {
                    3.88,
                    0.71,
                    0.21
                }, profile.Layers.Select(l => l.BelowPhreaticLevelMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.08,
                    0.02,
                    0.001
                }, profile.Layers.Select(l => l.BelowPhreaticLevelDeviation));
                CollectionAssert.AreEqual(new[]
                {
                    0.4,
                    0.32,
                    0.3
                }, profile.Layers.Select(l => l.BelowPhreaticLevelShift));
                CollectionAssert.AreEqual(new[]
                {
                    11.3,
                    0.01,
                    0.51
                }, profile.Layers.Select(l => l.DiameterD70Mean));
                CollectionAssert.AreEqual(new[]
                {
                    0.017699,
                    0.1,
                    0.029412
                }, profile.Layers.Select(l => l.DiameterD70CoefficientOfVariation));
                CollectionAssert.AreEqual(new[]
                {
                    5.21,
                    9.99,
                    1.01
                }, profile.Layers.Select(l => l.PermeabilityMean));
                CollectionAssert.AreEqual(new[]
                {
                    0.057582,
                    0.01001,
                    0.024752
                }, profile.Layers.Select(l => l.PermeabilityCoefficientOfVariation));
            }

            Assert.IsTrue(TestHelper.CanOpenFileForWrite(dbFile));
        }
    }
}