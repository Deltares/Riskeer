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
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile;
using Ringtoets.Piping.IO.SoilProfile.Schema;

namespace Ringtoets.Piping.IO.Test.SoilProfile
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

        [Test]
        public void Constructor_WithReaderValuesValid_SetProperties()
        {
            // Setup
            var reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            string profileName = "profile";
            var layerCount = 1;
            long soilProfileId = 1234;

            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).IgnoreArguments().Return(profileName);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).IgnoreArguments().Return(layerCount);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.SoilProfileId)).IgnoreArguments().Return(soilProfileId);

            mocks.ReplayAll();

            // Call
            var properties = new CriticalProfileProperties(reader);

            // Assert
            Assert.AreEqual(profileName, properties.ProfileName);
            Assert.AreEqual(layerCount, properties.LayerCount);
            Assert.AreEqual(soilProfileId, properties.ProfileId);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithReaderInvalidProfileName_SetProperties()
        {
            // Setup
            var reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            var layerCount = 1;
            string path = "A";
            var invalidCastException = new InvalidCastException();

            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).IgnoreArguments().Throw(invalidCastException);
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).IgnoreArguments().Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .Build(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithReaderInvalidLayerCount_SetProperties()
        {
            // Setup
            var reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            string profileName = "profile";
            string path = "A";
            var invalidCastException = new InvalidCastException();

            reader.Expect(r => r.Read<string>(SoilProfileTableColumns.ProfileName)).IgnoreArguments().Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<long>(SoilProfileTableColumns.LayerCount)).Throw(invalidCastException);
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            CriticalFileReadException exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            string expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisatie '{0}'", profileName))
                .Build(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }
    }
}