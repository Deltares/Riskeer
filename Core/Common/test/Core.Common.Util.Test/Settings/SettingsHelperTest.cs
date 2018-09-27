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

using System;
using System.IO;
using Core.Common.Util.Reflection;
using Core.Common.Util.Settings;
using NUnit.Framework;

namespace Core.Common.Util.Test.Settings
{
    [TestFixture]
    public class SettingsHelperTest
    {
        [Test]
        public void Instance_CalledTwice_ReturnsSameInstance()
        {
            // Setup
            ISettingsHelper expected = SettingsHelper.Instance;

            // Call
            ISettingsHelper actual = SettingsHelper.Instance;

            // Assert
            Assert.AreSame(expected, actual);
        }

        [Test]
        public void ApplicationName_ReturnsProductNameOfExecutingAssembly()
        {
            // Call
            string settings = SettingsHelper.Instance.ApplicationName;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Product, settings);
        }

        [Test]
        public void ApplicationVersion_ReturnsVersionOfExecutingAssembly()
        {
            // Call
            string settings = SettingsHelper.Instance.ApplicationVersion;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Version, settings);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_WithoutSubFolder_ReturnsApplicationLocalUserSettingsDirectory()
        {
            // Call
            string pathFromSettings = SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory();

            // Assert
            string localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            Assert.AreEqual(localSettingsDirectoryPath, pathFromSettings);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_WithPostfix_ReturnsRootFolderWithPostfix()
        {
            // Setup
            const string subFolder = nameof(GetApplicationLocalUserSettingsDirectory_WithPostfix_ReturnsRootFolderWithPostfix);
            const string subSubFolder = "subSubFolder";

            // Call
            string directory = SettingsHelper.Instance.GetApplicationLocalUserSettingsDirectory(subFolder, subSubFolder);

            // Assert
            string userSettingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);

            string testDataPath = Path.Combine(userSettingsDirectory, subFolder, subSubFolder);
            Assert.AreEqual(testDataPath, directory);
        }

        [Test]
        public void GetCommonDocumentsDirectory_WithoutSubFolder_ReturnsCommonDocuments()
        {
            // Call
            string pathFromSettings = SettingsHelper.Instance.GetCommonDocumentsDirectory();

            // Assert
            string localSettingsDirectoryPath = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);
            Assert.AreEqual(localSettingsDirectoryPath, pathFromSettings);
        }

        [Test]
        public void GetCommonDocumentsDirectory_WithPostfix_ReturnsRootFolderWithPostfix()
        {
            // Setup
            const string subFolder = nameof(GetCommonDocumentsDirectory_WithPostfix_ReturnsRootFolderWithPostfix);
            const string subSubFolder = "subSubFolder";

            // Call
            string directory = SettingsHelper.Instance.GetCommonDocumentsDirectory(subFolder, subSubFolder);

            // Assert
            string userSettingsDirectory = Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments);

            string testDataPath = Path.Combine(userSettingsDirectory, subFolder, subSubFolder);
            Assert.AreEqual(testDataPath, directory);
        }

        [Test]
        public void GetLocalUserTemporaryDirectory_ReturnsTempPath()
        {
            // Call
            string pathFromSettings = SettingsHelper.Instance.GetLocalUserTemporaryDirectory();

            // Assert
            string tempPath = Path.GetTempPath();
            Assert.AreEqual(tempPath, pathFromSettings);
        }
    }
}