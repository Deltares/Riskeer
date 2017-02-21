// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.IO;
using Core.Common.Gui.Settings;
using Core.Common.Gui.TestUtil.Settings;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Core.Common.Gui.TestUtil.Test.Settings
{
    [TestFixture]
    public class TestSettingsHelperTest
    {
        [Test]
        public void Constructor_ExpectedProperties()
        {
            // Call
            var settingsHelper = new TestSettingsHelper();

            // Assert
            Assert.IsInstanceOf<ISettingsHelper>(settingsHelper);
            Assert.IsEmpty(settingsHelper.ApplicationName);
            Assert.IsEmpty(settingsHelper.ApplicationVersion);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_NullPostfix_ReturnsRootFolder()
        {
            // Setup
            var settingsHelper = new TestSettingsHelper();

            // Call
            string directory = settingsHelper.GetApplicationLocalUserSettingsDirectory(null);

            // Assert
            string testDataPath = TestHelper.GetScratchPadPath();
            Assert.AreEqual(testDataPath, directory);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectoryWithExpectedDirectory_NullPostfix_ReturnsRootFolder()
        {
            // Setup
            const string userSettingsDirectory = "someFolder";
            var settingsHelper = new TestSettingsHelper
            {
                ExpectedApplicationLocalUserSettingsDirectory = userSettingsDirectory
            };

            // Call
            string directory = settingsHelper.GetApplicationLocalUserSettingsDirectory(null);

            // Assert
            Assert.AreEqual(userSettingsDirectory, directory);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_WithPostfix_ReturnsRootFolderWithPostfix()
        {
            // Setup
            var settingsHelper = new TestSettingsHelper();
            string postfix = Path.GetRandomFileName();

            // Call
            string directory = settingsHelper.GetApplicationLocalUserSettingsDirectory(postfix);

            // Assert
            string testDataPath = TestHelper.GetScratchPadPath(postfix);
            Assert.AreEqual(testDataPath, directory);
            Assert.IsTrue(Directory.Exists(testDataPath));
            Directory.Delete(testDataPath);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectoryWithExpectedDirectory_WithPostfix_ReturnsRootFolderWithPostfix()
        {
            // Setup
            string subFolder = Path.GetRandomFileName();
            string subSubFolder = Path.GetRandomFileName();
            const string userSettingsDirectory = "someFolder";
            var settingsHelper = new TestSettingsHelper
            {
                ExpectedApplicationLocalUserSettingsDirectory = userSettingsDirectory
            };

            // Call
            string directory = settingsHelper.GetApplicationLocalUserSettingsDirectory(subFolder, subSubFolder);

            // Assert
            string testDataPath = Path.Combine(userSettingsDirectory, subFolder, subSubFolder);
            Assert.AreEqual(testDataPath, directory);

            string testDataPathParent = Path.Combine(userSettingsDirectory, subFolder);
            Assert.IsTrue(Directory.Exists(testDataPathParent));
            Assert.IsTrue(Directory.Exists(testDataPath));
            Directory.Delete(testDataPathParent, true);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_ValidPathFileExistsDirectoryNotWritable_ThrowsIOException()
        {
            // Setup
            const string workingDirectory = "folderToCr*eate";
            const string userSettingsDirectory = "someFolder";
            var settingsHelper = new TestSettingsHelper
            {
                ExpectedApplicationLocalUserSettingsDirectory = userSettingsDirectory
            };

            // Call
            TestDelegate test = () => settingsHelper.GetApplicationLocalUserSettingsDirectory(workingDirectory);

            // Assert
            string dataPath = Path.Combine(userSettingsDirectory, workingDirectory);
            var expectedMessage = $"Unable to create '{dataPath}'.";
            var message = Assert.Throws<IOException>(test).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ApplicationName_WithExpectedSet_ReturnsExpected()
        {
            // Setup
            const string expectedApplicationName = "some name";
            var settingsHelper = new TestSettingsHelper
            {
                ExpectedApplicationName = expectedApplicationName
            };

            // Call
            string applicationName = settingsHelper.ApplicationName;

            // Assert
            Assert.AreEqual(expectedApplicationName, applicationName);
        }

        [Test]
        public void ApplicationVersion_WithExpectedSet_ReturnsExpected()
        {
            // Setup
            const string expectedApplicationVersion = "some version";
            var settingsHelper = new TestSettingsHelper
            {
                ExpectedApplicationVersion = expectedApplicationVersion
            };

            // Call
            string applicationVersion = settingsHelper.ApplicationVersion;

            // Assert
            Assert.AreEqual(expectedApplicationVersion, applicationVersion);
        }
    }
}