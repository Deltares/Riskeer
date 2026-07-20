// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class CriticalProfilePropertiesTest
    {
        [SetUp]
        public void SetUp() {}

        [TearDown]
        public void TearDown() {}

        [Test]
        public void Constructor_WithReaderValuesValid_SetProperties()
        {
            // Setup
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            const string profileName = "profile";
            const int layerCount = 1;
            const long soilProfileId = 1234;

            reader.Read<string>(SoilProfileTableDefinitions.ProfileName).Returns(profileName);
            reader.Read<long>(SoilProfileTableDefinitions.LayerCount).Returns(layerCount);
            reader.Read<long>(SoilProfileTableDefinitions.SoilProfileId).Returns(soilProfileId);
            // Call
            var properties = new CriticalProfileProperties(reader);

            // Assert
            Assert.AreEqual(profileName, properties.ProfileName);
            Assert.AreEqual(layerCount, properties.LayerCount);
            Assert.AreEqual(soilProfileId, properties.ProfileId);
        }

        [Test]
        public void Constructor_WithReaderInvalidProfileId_ThrowsCriticalFileReadException()
        {
            // Setup
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            const string profileName = "profile";
            const int layerCount = 1;
            const string path = "A";
            var invalidCastException = new InvalidCastException();

            reader.Read<string>(SoilProfileTableDefinitions.ProfileName).Returns(profileName);
            reader.Read<long>(SoilProfileTableDefinitions.LayerCount).Returns(layerCount);
            reader.Read<long>(SoilProfileTableDefinitions.SoilProfileId).Returns(_ => throw invalidCastException);
            reader.Path.Returns(path);
            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                     .WithSubject($"ondergrondschematisatie '{profileName}'")
                                     .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithReaderInvalidProfileName_ThrowsCriticalFileReadException()
        {
            // Setup
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            const string path = "A";
            var invalidCastException = new InvalidCastException();

            reader.Read<string>(SoilProfileTableDefinitions.ProfileName).Returns(_ => throw invalidCastException);
            reader.Path.Returns(path);
            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void Constructor_WithReaderInvalidLayerCount_ThrowsCriticalFileReadException()
        {
            // Setup
            var reader = Substitute.For<IRowBasedDatabaseReader>();
            const string profileName = "profile";
            const string path = "A";
            var invalidCastException = new InvalidCastException();

            reader.Read<string>(SoilProfileTableDefinitions.ProfileName).Returns(profileName);
            reader.Read<long>(SoilProfileTableDefinitions.LayerCount).Returns(_ => throw invalidCastException);
            reader.Path.Returns(path);
            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                                     .WithSubject($"ondergrondschematisatie '{profileName}'")
                                     .Build("Kritieke fout opgetreden bij het uitlezen van waardes uit kolommen in de database.");
            Assert.AreEqual(expectedMessage, exception.Message);
        }
    }
}