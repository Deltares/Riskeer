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
using System.Linq;
using System.Security.AccessControl;
using System.Security.Principal;
using NUnit.Framework;

namespace Core.Common.TestUtil.Test
{
    [TestFixture]
    public class DirectoryRightsHelperTest
    {
        private readonly string testWorkDir = Path.Combine(".", "FileRightsHelperTest");

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void Constructor_NullPath_ThrowsArgumentException(string invalidPath)
        {
            // Call
            TestDelegate test = () => new DirectoryRightsHelper(invalidPath, FileSystemRights.Modify);

            // Assert
            string paramName = Assert.Throws<ArgumentException>(test).ParamName;
            Assert.AreEqual("filePath", paramName);
        }

        [Test]
        public void Constructor_PathDoesNotExist_ThrowsDirectoryNotFoundException()
        {
            // Setup
            const string invalidPath = @".\DirectoryDoesNotExist\fileDoesNotExist";

            // Call
            TestDelegate test = () => new DirectoryRightsHelper(invalidPath, FileSystemRights.Modify);

            // Assert
            Assert.Throws<DirectoryNotFoundException>(test);
        }

        [Test]
        public void Constructor_UnsupportedRight_ThrowsNotSupportedException()
        {
            // Setup
            const FileSystemRights rights = FileSystemRights.Synchronize;
            var accessDirectory = Path.Combine(testWorkDir, "Constructor_UnsupportedRight_ThrowsNotSupportedException", rights.ToString());
            Directory.CreateDirectory(accessDirectory);

            try
            {
                // Call
                TestDelegate test = () => { new DirectoryRightsHelper(accessDirectory, rights); };

                // Assert
                Assert.Throws<NotSupportedException>(test);
            }
            finally
            {
                Directory.Delete(accessDirectory, true);
            }
        }

        [Test]
        [TestCase(FileSystemRights.AppendData)]
        [TestCase(FileSystemRights.ChangePermissions)]
        [TestCase(FileSystemRights.Delete)]
        [TestCase(FileSystemRights.DeleteSubdirectoriesAndFiles)]
        [TestCase(FileSystemRights.ExecuteFile)]
        [TestCase(FileSystemRights.FullControl)]
        [TestCase(FileSystemRights.Modify)]
        [TestCase(FileSystemRights.Read)]
        [TestCase(FileSystemRights.ReadAndExecute)]
        [TestCase(FileSystemRights.ReadAttributes)]
        [TestCase(FileSystemRights.ReadData)]
        [TestCase(FileSystemRights.ReadExtendedAttributes)]
        [TestCase(FileSystemRights.ReadPermissions)]
        [TestCase(FileSystemRights.TakeOwnership)]
        [TestCase(FileSystemRights.Write)]
        [TestCase(FileSystemRights.WriteData)]
        [TestCase(FileSystemRights.WriteAttributes)]
        [TestCase(FileSystemRights.WriteExtendedAttributes)]
        public void Constructor_ValidPathDenyRight_SetsDenyRight(FileSystemRights rights)
        {
            // Setup
            var accessDirectory = Path.Combine(testWorkDir, "Constructor_ValidPathDenyRight_SetsDenyRight", rights.ToString());
            Directory.CreateDirectory(accessDirectory);

            try
            {
                // Call
                using (new DirectoryRightsHelper(accessDirectory, rights))
                {
                    // Assert
                    AssertPathHasAccessRuleSet(accessDirectory, rights);
                }

                AssertPathHasAccessRuleNotSet(accessDirectory, rights);
            }
            finally
            {
                Directory.Delete(accessDirectory, true);
            }
        }

        [Test]
        [TestCase(FileSystemRights.AppendData)]
        [TestCase(FileSystemRights.ChangePermissions)]
        [TestCase(FileSystemRights.Delete)]
        [TestCase(FileSystemRights.DeleteSubdirectoriesAndFiles)]
        [TestCase(FileSystemRights.ExecuteFile)]
        [TestCase(FileSystemRights.FullControl)]
        [TestCase(FileSystemRights.Modify)]
        [TestCase(FileSystemRights.Read)]
        [TestCase(FileSystemRights.ReadAndExecute)]
        [TestCase(FileSystemRights.ReadAttributes)]
        [TestCase(FileSystemRights.ReadData)]
        [TestCase(FileSystemRights.ReadExtendedAttributes)]
        [TestCase(FileSystemRights.ReadPermissions)]
        [TestCase(FileSystemRights.TakeOwnership)]
        [TestCase(FileSystemRights.Write)]
        [TestCase(FileSystemRights.WriteData)]
        [TestCase(FileSystemRights.WriteAttributes)]
        [TestCase(FileSystemRights.WriteExtendedAttributes)]
        public void Dispose_RightAlreadySet_DoesNotRemoveRight(FileSystemRights rights)
        {
            // Setup
            var accessDirectory = Path.Combine(testWorkDir, "Dispose_RightAlreadySet_DoesNotRemoveRight", rights.ToString());
            Directory.CreateDirectory(accessDirectory);

            AddDirectoryAccessRule(accessDirectory, rights);

            // Precondition
            AssertPathHasAccessRuleSet(accessDirectory, rights);

            try
            {
                // Call
                using (new DirectoryRightsHelper(accessDirectory, rights))
                {
                    // Assert
                    AssertPathHasAccessRuleSet(accessDirectory, rights);
                }

                AssertPathHasAccessRuleSet(accessDirectory, rights);
            }
            finally
            {
                RemoveDirectoryAccessRule(accessDirectory, rights);
                Directory.Delete(accessDirectory, true);
            }
        }

        [Test]
        public void Dispose_DirectoryAlreadyRemoved_DoesNotThrowException()
        {
            // Setup
            var accessDirectory = Path.Combine(testWorkDir, "Deleted");
            Directory.CreateDirectory(accessDirectory);

            TestDelegate test = () =>
            {
                // Call
                using (new DirectoryRightsHelper(accessDirectory, FileSystemRights.Write))
                {
                    Directory.Delete(accessDirectory, true);
                }
            };

            // Assert
            Assert.DoesNotThrow(test);
        }

        #region Assert access rules

        private static void AssertPathHasAccessRuleNotSet(string filePath, FileSystemRights rights)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            var rules = directorySecurity.GetAccessRules(true, false, typeof(SecurityIdentifier));
            var fileSystemAccessRule = rules.OfType<FileSystemAccessRule>().FirstOrDefault(fs => fs.FileSystemRights == rights && fs.AccessControlType == AccessControlType.Deny);
            Assert.IsNull(fileSystemAccessRule, string.Format("Rights '{0} {1}' are set for '{2}'", AccessControlType.Deny, rights, filePath));
        }

        private void AssertPathHasAccessRuleSet(string filePath, FileSystemRights rights)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();
            var rules = directorySecurity.GetAccessRules(true, false, typeof(SecurityIdentifier));
            var fileSystemAccessRule = rules.OfType<FileSystemAccessRule>().First(fs => fs.FileSystemRights == rights && fs.AccessControlType == AccessControlType.Deny);
            Assert.IsNotNull(fileSystemAccessRule, string.Format("Rights '{0} {1}' not set for '{2}'", AccessControlType.Deny, rights, filePath));
        }

        #endregion

        #region Apply access rules

        private static void AddDirectoryAccessRule(string filePath, FileSystemRights rights)
        {
            var sid = GetSecurityIdentifier();

            DirectoryInfo directoryInfo = new DirectoryInfo(filePath);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.AddAccessRule(new FileSystemAccessRule(sid, rights, AccessControlType.Deny));

            directoryInfo.SetAccessControl(directorySecurity);
        }

        private static void RemoveDirectoryAccessRule(string filePath, FileSystemRights rights)
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

        #endregion
    }
}