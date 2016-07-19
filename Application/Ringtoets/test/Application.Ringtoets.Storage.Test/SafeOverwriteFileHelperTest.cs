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
using System.IO;
using System.Security.AccessControl;
using System.Security.Principal;
using NUnit.Framework;

namespace Application.Ringtoets.Storage.Test
{
    [TestFixture]
    public class SafeOverwriteFileHelperTest
    {
        private string testWorkDir = Path.Combine(".", "SafeOverwriteFileHelperTest");

        [TestFixtureSetUp]
        public void SetUpFixture()
        {
            Directory.CreateDirectory(testWorkDir);
        }

        [TestFixtureTearDown]
        public void TearDownFixture()
        {
            Directory.Delete(testWorkDir);
        }

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("\"")]
        [TestCase("/")]
        public void Constructor_InvalidPath_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate test = () => new SafeOverwriteFileHelper(path);

            // Assert
            Assert.Throws<ArgumentException>(test);
        }

        [Test]
        public void Constructor_ValidPathFileDoesNotExist_NewFileCreatedCanBeDeleted()
        {
            // Setup
            var filePath = Path.Combine(testWorkDir, "iDoNotExist.txt");

            // Precondition
            Assert.IsFalse(File.Exists(filePath));

            // Call
            new SafeOverwriteFileHelper(filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
            File.Delete(filePath);
        }

        [Test]
        public void Constructor_ValidPathFileExists_NewFileAndTemporaryFileCreatedCanBeDeleted()
        {
            // Setup
            var filePath = Path.Combine(testWorkDir, "iDoExist.txt");
            var temporaryFilePath = filePath + "~";

            using (File.Create(filePath)) {}

            // Precondition
            Assert.IsTrue(File.Exists(filePath));

            // Call
            new SafeOverwriteFileHelper(filePath);

            // Assert
            Assert.IsTrue(File.Exists(filePath));
            Assert.IsTrue(File.Exists(temporaryFilePath));
            File.Delete(filePath);
            File.Delete(temporaryFilePath);
        }

        [Test]
        public void Constructor_ValidPathDirectoryNotWritable_ThrowsIOException()
        {
            // Setup
            var notWritableDirectory = Path.Combine(testWorkDir, "NotWritable");
            var filePath = Path.Combine(notWritableDirectory, "iDoNotExist.txt");

            Directory.CreateDirectory(notWritableDirectory);
            DenyDirectoryRight(notWritableDirectory, FileSystemRights.Write);

            // Call
            TestDelegate test = () => new SafeOverwriteFileHelper(filePath);
            
            try
            {
                // Assert
                var expectedMessage = string.Format("Kan geen nieuw doelbestand maken ({0}). Probeer ergens anders op te slaan.", filePath);
                var message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
            finally
            {
                Directory.Delete(notWritableDirectory, true);
            }
        }

        [Test]
        public void Constructor_ValidPathFileExistsDirectoryNotWritable_ThrowsIOException()
        {
            // Setup
            var notWritableDirectory = Path.Combine(testWorkDir, "NotWritable");
            var filePath = Path.Combine(notWritableDirectory, "iDoExist.txt");

            Directory.CreateDirectory(notWritableDirectory);
            using (File.Create(filePath)) {}
            DenyDirectoryRight(notWritableDirectory, FileSystemRights.Write);

            try
            {
                // Precondition
                Assert.IsTrue(File.Exists(filePath));

                // Call
                TestDelegate test = () => new SafeOverwriteFileHelper(filePath);

                // Assert
                var expectedMessage = string.Format("Kan geen tijdelijk bestand maken van het originele bestand ({0}). Probeer ergens anders op te slaan.", filePath);
                var message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
            finally
            {
                Directory.Delete(notWritableDirectory, true);
            }
        }

        [Test]
        public void Constructor_ValidFile_TemporaryFileCreatedFromOriginal()
        {
            // Setup
            var writableDirectory = Path.Combine(testWorkDir, "Writable");
            var filePath = Path.Combine(writableDirectory, "iDoExist.txt");
            var temporaryFilePath = filePath + "~";
            var testContent = "Some test text to write into file.";

            Directory.CreateDirectory(writableDirectory);
            File.WriteAllText(filePath, testContent);

            try
            {
                // Precondition
                Assert.IsTrue(File.Exists(filePath));

                // Call
                new SafeOverwriteFileHelper(filePath);

                // Assert
                Assert.IsTrue(File.Exists(temporaryFilePath));
                Assert.AreEqual(testContent, File.ReadAllText(temporaryFilePath));
            }
            finally
            {
                Directory.Delete(writableDirectory, true);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Finish_AccessRightsRevoked_DoesNotThrow(bool revert)
        {
            // Setup
            var noAccessDirectory = Path.Combine(testWorkDir, "NoAccess");
            var filePath = Path.Combine(noAccessDirectory, "iDoNotExist.txt");

            Directory.CreateDirectory(noAccessDirectory);
            var helper = new SafeOverwriteFileHelper(filePath);

            var right = FileSystemRights.FullControl;
            DenyDirectoryRight(noAccessDirectory, right);

            // Call
            TestDelegate test = () => helper.Finish(revert);

            try
            {
                // Assert
                Assert.DoesNotThrow(test);
            }
            finally
            {
                RevertDenyDirectoryRight(noAccessDirectory, right);
                Directory.Delete(noAccessDirectory, true);
            }
        }

        [Test]
        public void Finish_FileExistsRevertTrueCannotDeleteFile_ThrowsIOException()
        {
            // Setup
            var noAccessDirectory = Path.Combine(testWorkDir, "Access");
            var filePath = Path.Combine(noAccessDirectory, "iDoExist.txt");

            Directory.CreateDirectory(noAccessDirectory);
            using (File.Create(filePath)) {}

            var helper = new SafeOverwriteFileHelper(filePath);

            var fileStream = File.Create(filePath);

            // Call
            TestDelegate test = () => helper.Finish(true);

            try
            {
                // Assert
                var expectedMessage = string.Format("Kan het originele bestand ({0}) niet herstellen. Het tijdelijke bestand dient handmatig hersteld worden.", filePath);
                var message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
            finally
            {
                fileStream.Dispose();
                Directory.Delete(noAccessDirectory, true);
            }
        }

        [Test]
        public void Finish_FileExistsRevertTrueCannotMoveFile_ThrowsIOException()
        {
            // Setup
            var noAccessDirectory = Path.Combine(testWorkDir, "NoAccess");
            var filePath = Path.Combine(noAccessDirectory, "iDoExist.txt");

            Directory.CreateDirectory(noAccessDirectory);
            using (File.Create(filePath)) {}

            var helper = new SafeOverwriteFileHelper(filePath);

            var right = FileSystemRights.FullControl;
            DenyDirectoryRight(noAccessDirectory, right);

            // Call
            TestDelegate test = () => helper.Finish(true);

            try
            {
                // Assert
                var expectedMessage = string.Format("Kan het originele bestand ({0}) niet herstellen. Het tijdelijke bestand dient handmatig hersteld worden.", filePath);
                var message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
            finally
            {
                RevertDenyDirectoryRight(noAccessDirectory, right);
                Directory.Delete(noAccessDirectory, true);
            }
        }

        [Test]
        public void Finish_FileExistsRevertFalseCannotDeleteFile_ThrowsIOException()
        {
            // Setup
            var accessDirectory = Path.Combine(testWorkDir, "Access");
            var filePath = Path.Combine(accessDirectory, "iDoExist.txt");
            var temporaryFilePath = filePath + "~";

            Directory.CreateDirectory(accessDirectory);
            using (File.Create(filePath)) {}

            var helper = new SafeOverwriteFileHelper(filePath);

            var fileStream = File.Create(temporaryFilePath);

            // Call
            TestDelegate test = () => helper.Finish(false);

            try
            {
                // Assert
                var expectedMessage = string.Format("Kan het tijdelijke bestand ({0}) niet opruimen. Het tijdelijke bestand dient handmatig verwijderd te worden.", temporaryFilePath);
                var message = Assert.Throws<IOException>(test).Message;
                Assert.AreEqual(expectedMessage, message);
            }
            finally
            {
                fileStream.Dispose();
                Directory.Delete(accessDirectory, true);
            }
        }

        // Removes an ACL entry on the specified directory for the specified account.
        public static void DenyDirectoryRight(string fileName, FileSystemRights rights)
        {
            var sid = GetSecurityIdentifier();

            DirectoryInfo directoryInfo = new DirectoryInfo(fileName);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.AddAccessRule(new FileSystemAccessRule(sid, rights, AccessControlType.Deny));

            directoryInfo.SetAccessControl(directorySecurity);
        }

        // Removes an ACL entry on the specified directory for the specified account.
        public static void RevertDenyDirectoryRight(string fileName, FileSystemRights rights)
        {
            var sid = GetSecurityIdentifier();

            DirectoryInfo directoryInfo = new DirectoryInfo(fileName);
            DirectorySecurity directorySecurity = directoryInfo.GetAccessControl();

            directorySecurity.RemoveAccessRule(new FileSystemAccessRule(sid, rights, AccessControlType.Deny));

            directoryInfo.SetAccessControl(directorySecurity);
        }

        private static SecurityIdentifier GetSecurityIdentifier()
        {
            SecurityIdentifier id = WindowsIdentity.GetCurrent().User.AccountDomainSid;
            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, id);
            return sid;
        }
    }
}