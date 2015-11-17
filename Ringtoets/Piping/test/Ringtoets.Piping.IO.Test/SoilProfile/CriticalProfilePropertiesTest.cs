using System;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.IO.Exceptions;
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
            var invalidCastException = new InvalidCastException();

            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).IgnoreArguments().Throw(invalidCastException);
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).IgnoreArguments().Return(layerCount).Repeat.Any();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            Assert.AreSame(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column, exception.Message);

            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithReaderInvalidLayerCount_SetProperties()
        {
            // Setup
            var reader = mocks.StrictMock<IRowBasedDatabaseReader>();
            string profileName = "profile";
            var invalidCastException = new InvalidCastException();

            reader.Expect(r => r.Read<string>(SoilProfileDatabaseColumns.ProfileName)).IgnoreArguments().Return(profileName).Repeat.Any();
            reader.Expect(r => r.Read<long>(SoilProfileDatabaseColumns.LayerCount)).Throw(invalidCastException);

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new CriticalProfileProperties(reader);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(test);
            Assert.AreSame(invalidCastException, exception.InnerException);
            Assert.AreSame(Resources.PipingSoilProfileReader_Critical_Unexpected_value_on_column, exception.Message);

            mocks.VerifyAll();
        }
    }
}