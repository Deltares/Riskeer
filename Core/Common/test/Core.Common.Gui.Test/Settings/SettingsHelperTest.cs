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

using System;
using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Settings
{
    [TestFixture]
    public class SettingsHelperTest
    {
        private string originalLocalSettingsDirectoryPath = "";

        [SetUp]
        public void SetUp()
        {
            originalLocalSettingsDirectoryPath = GetLocalSettingsDirectoryPath();
        }

        [TearDown]
        public void TearDown()
        {
            SetLocalSettingsDirectoryPath(originalLocalSettingsDirectoryPath);
        }

        [Test]
        public void ApplicationName_ReturnsProductNameOfExecutingAssembly()
        {
            // Call
            var settings = SettingsHelper.Instance.ApplicationName;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Product, settings);
        }

        [Test]
        public void ApplicationVersion_ReturnsVersionOfExecutingAssembly()
        {
            // Call
            var settings = SettingsHelper.Instance.ApplicationVersion;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Version, settings);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase(" ")]
        [TestCase("some directory name")]
        public void GetApplicationLocalUserSettingsDirectory_VariousPostfixes_ReturnsApplicationLocalUserSettingsDirectory(string postfix)
        {
            // Setup
            var localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var appSettingsDirectoryPath = string.IsNullOrWhiteSpace(postfix) ? localSettingsDirectoryPath : Path.Combine(localSettingsDirectoryPath, postfix);

            // Call
            var pathFromSettings = SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(postfix);

            // Assert
            Assert.AreEqual(appSettingsDirectoryPath, pathFromSettings);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_ValidPathFileExistsDirectoryNotWritable_ThrowsIOException()
        {
            // Setup
            string workingDirectory = Path.Combine(".", "SettingsHelper", "NotWritable");
            Directory.CreateDirectory(workingDirectory);
            const string notWritableFolder = "folderToCreate";

            SetLocalSettingsDirectoryPath(workingDirectory);

            using (new DirectoryPermissionsRevoker(workingDirectory, FileSystemRights.Write))
            {
                // Call
                TestDelegate test = () => SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(notWritableFolder);

                try
                {
                    // Assert
                    var notWritableDirectory = Path.Combine(workingDirectory, notWritableFolder);
                    var expectedMessage = string.Format("De map '{0}' kan niet aangemaakt worden.", notWritableDirectory);
                    var message = Assert.Throws<IOException>(test).Message;
                    Assert.AreEqual(expectedMessage, message);
                }
                finally
                {
                    Directory.Delete(workingDirectory, true);
                }
            }
        }

        private static void SetLocalSettingsDirectoryPath(string nonExistingFolder)
        {
            string localSettingsDirectoryPath = "localSettingsDirectoryPath";
            Type settingsHelperType = typeof(SettingsHelper);
            FieldInfo fieldInfo = settingsHelperType.GetField(localSettingsDirectoryPath, BindingFlags.NonPublic | BindingFlags.Static);
            if (fieldInfo == null)
            {
                Assert.Fail("Unable to find private field '{0}'", localSettingsDirectoryPath);
            }

            fieldInfo.SetValue(null, nonExistingFolder);
        }

        private static string GetLocalSettingsDirectoryPath()
        {
            string localSettingsDirectoryPath = "localSettingsDirectoryPath";
            Type settingsHelperType = typeof(SettingsHelper);
            FieldInfo fieldInfo = settingsHelperType.GetField(localSettingsDirectoryPath, BindingFlags.NonPublic | BindingFlags.Static);
            if (fieldInfo == null)
            {
                Assert.Fail("Unable to find private field '{0}'", localSettingsDirectoryPath);
            }

            return (string) fieldInfo.GetValue(null);
        }
    }
}