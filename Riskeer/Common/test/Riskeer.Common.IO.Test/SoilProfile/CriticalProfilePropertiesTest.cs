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
using Core.Common.Base.IO;
using Core.Common.IO.Readers;
using Core.Common.Util.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SoilProfile.Schema;

namespace Riskeer.Common.IO.Test.SoilProfile
{
    [TestFixture]
    public class CriticalProfilePropertiesTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [TearDown]
        public void TearDown()
        {
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithReaderValuesValid_SetProperties()
        {
            // Setup
            var reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            const string profileName = "profile";
            const int layerCount = 1;
            const long soilProfileId = 1234;

            reader.Expect(r => r.Read<string>(SoilProfileTableDefinitions.ProfileName)).IgnoreArguments().Return(profileName);
            reader.Expect(r => r.Read<long>(SoilProfileTableDefinitions.LayerCount)).IgnoreArguments().Return(layerCount);
            reader.Expect(r => r.Read<long>(SoilProfileTableDefinitions.SoilProfileId)).IgnoreArguments().Return(soilProfileId);

            mocks.ReplayAll();

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
            var reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            const string profileName = "profile";
            const int layerCount = 1;
            const string path = "A";
            var invalidCastException = new InvalidCastException();

            reader.Expect(r => r.Read<string>(SoilProfileTableDefinitions.ProfileName)).IgnoreArguments().Return(profileName);
            reader.Expect(r => r.Read<long>(SoilProfileTableDefinitions.LayerCount)).IgnoreArguments().Return(layerCount);
            reader.Expect(r => r.Read<long>(SoilProfileTableDefinitions.SoilProfileId)).IgnoreArguments().Throw(invalidCastException);
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

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
            var reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            const int layerCount = 1;
            const string path = "A";
            var invalidCastException = new InvalidCastException();

            reader.Expect(r => r.Read<string>(SoilProfileTableDefinitions.ProfileName)).IgnoreArguments().Throw(invalidCastException);
            reader.Expect(r => r.Read<long>(SoilProfileTableDefinitions.LayerCount)).IgnoreArguments().Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

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
            var reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            const string profileName = "profile";
            const string path = "A";
            var invalidCastException = new InvalidCastException();

            reader.Expect(r => r.Read<string>(SoilProfileTableDefinitions.ProfileName)).IgnoreArguments().Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<long>(SoilProfileTableDefinitions.LayerCount)).Throw(invalidCastException);
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

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