// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.TestUtil;
using Core.Common.Util.Settings;
using Core.Common.Util.TestUtil.Settings;
using NUnit.Framework;

namespace Core.Common.Util.TestUtil.Test.Settings
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
        public void GetApplicationLocalUserSettingsDirectory_NoPostfix_ReturnsRootFolder()
        {
            // Setup
            var settingsHelper = new TestSettingsHelper();

            // Call
            string directory = settingsHelper.GetApplicationLocalUserSettingsDirectory();

            // Assert
            string testDataPath = TestHelper.GetScratchPadPath();
            Assert.AreEqual(testDataPath, directory);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_WithPostfix_ReturnsRootFolderWithPostfix()
        {
            // Setup
            var settingsHelper = new TestSettingsHelper();
            const string postfix = nameof(GetApplicationLocalUserSettingsDirectory_WithPostfix_ReturnsRootFolderWithPostfix);

            // Call
            string directory = settingsHelper.GetApplicationLocalUserSettingsDirectory(postfix);

            // Assert
            string testDataPath = Path.Combine(TestHelper.GetScratchPadPath(), postfix);
            Assert.AreEqual(testDataPath, directory);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectoryWithExpectedDirectory_NullPostfix_ReturnsRootFolder()
        {
            // Setup
            const string userSettingsDirectory = nameof(GetApplicationLocalUserSettingsDirectoryWithExpectedDirectory_NullPostfix_ReturnsRootFolder);
            var settingsHelper = new TestSettingsHelper
            {
                ApplicationLocalUserSettingsDirectory = userSettingsDirectory
            };

            // Call
            string directory = settingsHelper.GetApplicationLocalUserSettingsDirectory(null);

            // Assert
            Assert.AreEqual(userSettingsDirectory, directory);
        }

        [Test]
        public void GetCommonDocumentsDirectory_NoPostfix_ReturnsRootFolder()
        {
            // Setup
            var settingsHelper = new TestSettingsHelper();

            // Call
            string directory = settingsHelper.GetCommonDocumentsDirectory();

            // Assert
            string testDataPath = TestHelper.GetScratchPadPath();
            Assert.AreEqual(testDataPath, directory);
        }

        [Test]
        public void GetCommonDocumentsDirectory_WithPostfix_ReturnsRootFolderWithPostfix()
        {
            // Setup
            var settingsHelper = new TestSettingsHelper();
            const string postfix = nameof(GetCommonDocumentsDirectory_WithPostfix_ReturnsRootFolderWithPostfix);

            // Call
            string directory = settingsHelper.GetCommonDocumentsDirectory(postfix);

            // Assert
            string testDataPath = Path.Combine(TestHelper.GetScratchPadPath(), postfix);
            Assert.AreEqual(testDataPath, directory);
        }

        [Test]
        public void GetCommonDocumentsDirectoryWithExpectedDirectory_NullPostfix_ReturnsRootFolder()
        {
            // Setup
            const string userSettingsDirectory = nameof(GetCommonDocumentsDirectoryWithExpectedDirectory_NullPostfix_ReturnsRootFolder);
            var settingsHelper = new TestSettingsHelper
            {
                CommonDocumentsDirectory = userSettingsDirectory
            };

            // Call
            string directory = settingsHelper.GetCommonDocumentsDirectory(null);

            // Assert
            Assert.AreEqual(userSettingsDirectory, directory);
        }

        [Test]
        public void ApplicationName_WithExpectedSet_ReturnsExpected()
        {
            // Setup
            const string expectedApplicationName = "some name";
            var settingsHelper = new TestSettingsHelper();
            settingsHelper.SetApplicationName(expectedApplicationName);

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
            var settingsHelper = new TestSettingsHelper();
            settingsHelper.SetApplicationVersion(expectedApplicationVersion);

            // Call
            string applicationVersion = settingsHelper.ApplicationVersion;

            // Assert
            Assert.AreEqual(expectedApplicationVersion, applicationVersion);
        }

        [Test]
        public void GetLocalUserTemporaryDirectory_WithoutExpectedSet_ReturnsExpectedTempPath()
        {
            // Setup
            var settingsHelper = new TestSettingsHelper();

            // Call
            string pathFromSettings = settingsHelper.GetLocalUserTemporaryDirectory();

            // Assert
            string tempPath = TestHelper.GetScratchPadPath();
            Assert.AreEqual(tempPath, pathFromSettings);
        }

        [Test]
        public void GetLocalUserTemporaryDirectory_WithExpectedSet_ReturnsExpectedPath()
        {
            // Setup
            const string folder = "folder";
            var settingsHelper = new TestSettingsHelper
            {
                TempPath = folder
            };

            // Call
            string pathFromSettings = settingsHelper.GetLocalUserTemporaryDirectory();

            // Assert
            Assert.AreEqual(folder, pathFromSettings);
        }
    }
}