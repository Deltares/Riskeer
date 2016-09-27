﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Common.Utils.Builders;
using NUnit.Framework;
using Ringtoets.Common.IO.Structures;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Common.IO.Test.Structures
{
    [TestFixture]
    public class StructuresCharacteristicsCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HeightStructures.IO, string.Format("Characteristics{0}", Path.DirectorySeparatorChar));

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void Constructor_InvalidStringArgument_ThrowsArgumentException(string path)
        {
            // Call
            TestDelegate call = () => new StructuresCharacteristicsCsvReader(path);

            // Assert
            string expectedMessage = new FileReaderErrorMessageBuilder(path).Build(UtilsResources.Error_Path_must_be_specified);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_InvalidPathCharactersInPath_ThrowsArgumentException()
        {
            // Setup
            string path = Path.Combine(testDataPath, "ValidCharacteristics.csv");

            char[] invalidCharacters = Path.GetInvalidPathChars();

            string corruptPath = path.Replace('V', invalidCharacters[0]);

            // Call
            TestDelegate call = () => new StructuresCharacteristicsCsvReader(corruptPath);

            // Assert
            string innerExpectedMessage = string.Format((string)UtilsResources.Error_Path_cannot_contain_Characters_0_,
                                                        string.Join(", ", Path.GetInvalidFileNameChars()));
            string expectedMessage = new FileReaderErrorMessageBuilder(corruptPath).Build(innerExpectedMessage);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_PathToFolder_ThrowsArgumentException()
        {
            // Call
            TestDelegate call = () => new StructuresCharacteristicsCsvReader(testDataPath);

            // Assert
            ArgumentException exception = Assert.Throws<ArgumentException>(call);
            string expectedMessage = new FileReaderErrorMessageBuilder(testDataPath).Build(UtilsResources.Error_Path_must_not_point_to_empty_file_name);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GetLineCount_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            const string invalidFilePath = "I_do_not_exist.csv";

            var reader = new StructuresCharacteristicsCsvReader(invalidFilePath);

            // Call
            TestDelegate call = () => reader.GetLineCount();

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual("Fout bij het lezen van bestand 'I_do_not_exist.csv': Het bestand bestaat niet.", message);
        }

        [Test]
        public void GetLineCount_FolderDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine("I_do_not_exist", "I do not exist either.csv");

            var reader = new StructuresCharacteristicsCsvReader(invalidFilePath);

            // Call
            TestDelegate call = () => reader.GetLineCount();

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestandspad verwijst naar een map die niet bestaat.",
                                                   invalidFilePath);
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ReadLine_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            const string invalidFilePath = "I_do_not_exist.csv";

            var reader = new StructuresCharacteristicsCsvReader(invalidFilePath);

            // Call
            TestDelegate call = () => reader.ReadLine();

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual("Fout bij het lezen van bestand 'I_do_not_exist.csv': Het bestand bestaat niet.", message);
        }

        [Test]
        public void ReadLine_FolderDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = Path.Combine("I_do_not_exist", "I do not exist either.csv");

            var reader = new StructuresCharacteristicsCsvReader(invalidFilePath);

            // Call
            TestDelegate call = () => reader.ReadLine();

            // Assert
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            string expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestandspad verwijst naar een map die niet bestaat.",
                                                   invalidFilePath);
            Assert.AreEqual(expectedMessage, message);
        }
    }
}