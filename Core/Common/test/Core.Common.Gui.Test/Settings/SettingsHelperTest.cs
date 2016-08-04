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
using System.Security.Principal;
using Core.Common.Gui.Settings;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using log4net;
using NUnit.Framework;

namespace Core.Common.Gui.Test.Settings
{
    [TestFixture]
    public class SettingsHelperTest
    {
        [TearDown]
        public void TearDown()
        {
            SetLocalSettingsDirectoryPath(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData));
        }

        [Test]
        public void ApplicationName_ReturnsProductNameOfExecutingAssembly()
        {
            // Call
            var settings = SettingsHelper.ApplicationName;

            // Assert
            Assert.AreEqual(AssemblyUtils.GetExecutingAssemblyInfo().Product, settings);
        }

        [Test]
        public void ApplicationVersion_ReturnsVersionOfExecutingAssembly()
        {
            // Call
            var settings = SettingsHelper.ApplicationVersion;

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
            var pathFromSettings = SettingsHelper.GetApplicationLocalUserSettingsDirectory(postfix);

            // Assert
            Assert.AreEqual(appSettingsDirectoryPath, pathFromSettings);
        }

        [Test]
        public void GetApplicationLocalUserSettingsDirectory_ValidPathFileExistsDirectoryNotWritable_ThrowsIOException()
        {
            // Setup
            var workingDirectory = TestHelper.GetTestDataPath(TestDataPath.Core.Common.Gui, "SettingsHelper");
            Directory.CreateDirectory(workingDirectory);
            var notWritableFolder = "notWritable";

            DenyDirectoryRight(workingDirectory, FileSystemRights.Write);

            SetLocalSettingsDirectoryPath(workingDirectory);

            // Call
            TestDelegate test = () => SettingsHelper.GetApplicationLocalUserSettingsDirectory(notWritableFolder);

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
                RevertDenyDirectoryRight(workingDirectory, FileSystemRights.FullControl);
                Directory.Delete(workingDirectory, true);
            }

        }

        /// <summary>
        /// Adds a <paramref name="rights"/> of type <see cref="AccessControlType.Deny"/> to the access
        /// rule set for the file at <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to change the right for.</param>
        /// <param name="rights">The right to deny.</param>
        private static void DenyDirectoryRight(string filePath, FileSystemRights rights)
        {
            var sid = GetSecurityIdentifier();

            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.AddAccessRule(new FileSystemAccessRule(sid, rights, AccessControlType.Deny));

            directoryInfo.SetAccessControl(directorySecurity);
        }

        /// <summary>
        /// Removes a <paramref name="rights"/> of type <see cref="AccessControlType.Deny"/> from the 
        /// access rule set for the file at <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">The path of the file to change the right for.</param>
        /// <param name="rights">The right to revert.</param>
        private static void RevertDenyDirectoryRight(string filePath, FileSystemRights rights)
        {
            var sid = GetSecurityIdentifier();

            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.RemoveAccessRule(new FileSystemAccessRule(sid, rights, AccessControlType.Deny));

            directoryInfo.SetAccessControl(directorySecurity);
        }

        private static SecurityIdentifier GetSecurityIdentifier()
        {
            SecurityIdentifier id = WindowsIdentity.GetCurrent().User.AccountDomainSid;
            return new SecurityIdentifier(WellKnownSidType.WorldSid, id);
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
            else
            {
                fieldInfo.SetValue(null, nonExistingFolder);
            }
        }
    }
}