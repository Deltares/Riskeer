﻿using System;
using Core.Common.IO.Exceptions;
using Core.Common.IO.Readers;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.IO.Properties;
using Ringtoets.Piping.IO.SoilProfile;

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

            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).IgnoreArguments().Return(profileName);
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).IgnoreArguments().Return(layerCount);

            mocks.ReplayAll();

            // Call
            var properties = new CriticalProfileProperties(reader);

            // Assert
            Assert.AreEqual(profileName, properties.ProfileName);
            Assert.AreEqual(layerCount, properties.LayerCount);

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

            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).IgnoreArguments().Throw(invalidCastException);
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).IgnoreArguments().Return(layerCount).Repeat.Any();
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            var expectedMessage = new FileReaderErrorMessageBuilder(path)
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

            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).IgnoreArguments().Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Throw(invalidCastException);
            reader.Expect(r => r.Path).Return(path);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            var expectedMessage = new FileReaderErrorMessageBuilder(path)
                .WithSubject(string.Format("ondergrondschematisering '{0}'", profileName))
                .Build(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column);
            Assert.AreEqual(expectedMessage, exception.Message);

            mocks.VerifyAll();
        }
    }
}