using System.IO;

using Core.Common.Gui.Appenders;
using Core.Common.Gui.Settings;

using log4net.Util;

using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Common.Gui.Test.Appenders
{
    [TestFixture]
    public class RingtoetsUserDataFolderConverterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var converter = new RingtoetsUserDataFolderConverter();

            // Assert
            Assert.IsInstanceOf<FormattingInfo>(converter.FormattingInfo);
            Assert.IsNull(converter.Next);
            Assert.IsNull(converter.Option);
            Assert.IsNull(converter.Properties);
        }

        [Test]
        public void Convert_Always_WriteLocalUserDataDirectory()
        {
            // Setup
            var settingsDirectory = SettingsHelper.GetApplicationLocalUserSettingsDirectory();

            var mocks = new MockRepository();
            var textWriter = mocks.StrictMock<TextWriter>();
            textWriter.Expect(w => w.Write(settingsDirectory));
            mocks.ReplayAll();

            var converter = new RingtoetsUserDataFolderConverter();

            // Call
            converter.Format(textWriter, null);

            // Assert
            mocks.VerifyAll();
        }
    }
}