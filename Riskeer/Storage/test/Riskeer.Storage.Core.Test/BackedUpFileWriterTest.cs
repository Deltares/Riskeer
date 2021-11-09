﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Riskeer.Storage.Core.Test
{
    [TestFixture]
    public class BackedUpFileWriterTest
    {
        private const string testContent = "Some test content";

        private readonly string testWorkDir = TestHelper.GetScratchPadPath(nameof(BackedUpFileWriterTest));

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("\"")]
        [TestCase("/")]
        public void Constructor_InvalidTargetFilePath_ThrowsArgumentException(string targetFilePath)
        {
            // Call
            void Call() => new BackedUpFileWriter(targetFilePath);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Perform_ValidTargetFileContext_ExpectedTemporaryFileCreatedAndTargetFileContextRestoredAfterwards(bool performWithExistingTargetFile)
        {
            // Setup
            string writableDirectory = Path.Combine(testWorkDir, nameof(Perform_ValidTargetFileContext_ExpectedTemporaryFileCreatedAndTargetFileContextRestoredAfterwards));
            string targetFilePath = Path.Combine(writableDirectory, "iDoExist.txt");
            string temporaryFilePath = targetFilePath + "~";

            using (new DirectoryDisposeHelper(testWorkDir, nameof(Perform_ValidTargetFileContext_ExpectedTemporaryFileCreatedAndTargetFileContextRestoredAfterwards)))
            {
                if (performWithExistingTargetFile)
                {
                    File.WriteAllText(targetFilePath, testContent);
                }

                var writer = new BackedUpFileWriter(targetFilePath);

                // Call
                writer.Perform(() =>
                {
                    // Assert
                    Assert.IsFalse(File.Exists(targetFilePath));
                    Assert.IsTrue(File.Exists(temporaryFilePath));
                    Assert.AreEqual(performWithExistingTargetFile ? testContent : string.Empty, ReadAllText(temporaryFilePath));
                });

                Assert.False(File.Exists(temporaryFilePath));
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Perform_WriteActionThrowsException_TargetFileContextRestoredAndExpectedExceptionThrown(bool performWithExistingTargetFile)
        {
            // Setup
            string writableDirectory = Path.Combine(testWorkDir, nameof(Perform_WriteActionThrowsException_TargetFileContextRestoredAndExpectedExceptionThrown));
            string targetFilePath = Path.Combine(writableDirectory, "iDoExist.txt");
            string temporaryFilePath = targetFilePath + "~";

            using (new DirectoryDisposeHelper(testWorkDir, nameof(Perform_WriteActionThrowsException_TargetFileContextRestoredAndExpectedExceptionThrown)))
            {
                if (performWithExistingTargetFile)
                {
                    File.WriteAllText(targetFilePath, testContent);
                }

                var exception = new Exception();
                var writer = new BackedUpFileWriter(targetFilePath);

                // Call
                Exception actualException = Assert.Throws(exception.GetType(), () => writer.Perform(() => throw exception));

                Assert.AreSame(exception, actualException);

                Assert.IsFalse(File.Exists(temporaryFilePath));

                if (performWithExistingTargetFile)
                {
                    Assert.IsTrue(File.Exists(targetFilePath));
                    Assert.AreEqual(testContent, File.ReadAllText(targetFilePath));
                }
                else
                {
                    Assert.IsFalse(File.Exists(targetFilePath));
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Perform_TemporaryFileInUse_ExpectedExceptionThrown(bool performWithExistingTargetFile)
        {
            // Setup
            string writableDirectory = Path.Combine(testWorkDir, nameof(Perform_WriteActionThrowsException_TargetFileContextRestoredAndExpectedExceptionThrown));
            string targetFilePath = Path.Combine(writableDirectory, "iDoExist.txt");
            string temporaryFilePath = targetFilePath + "~";

            using (new DirectoryDisposeHelper(testWorkDir, nameof(Perform_WriteActionThrowsException_TargetFileContextRestoredAndExpectedExceptionThrown)))
            using (new FileDisposeHelper(temporaryFilePath))
            {
                if (performWithExistingTargetFile)
                {
                    File.WriteAllText(targetFilePath, testContent);
                }

                var writer = new BackedUpFileWriter(targetFilePath);

                // Call
                var exception = Assert.Throws<IOException>(() => writer.Perform(() => {}));
                Assert.AreEqual("Het doelbestand is reeds in gebruik.", exception.Message);
            }
        }

        private static string ReadAllText(string filePath)
        {
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var textReader = new StreamReader(fileStream))
            {
                return textReader.ReadToEnd();
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            Directory.CreateDirectory(testWorkDir);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            Directory.Delete(testWorkDir, true);
        }
    }
}