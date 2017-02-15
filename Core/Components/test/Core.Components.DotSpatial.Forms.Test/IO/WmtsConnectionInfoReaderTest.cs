﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Xml;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.DotSpatial.Forms.IO;
using NUnit.Framework;

namespace Core.Components.DotSpatial.Forms.Test.IO
{
    [TestFixture]
    public class WmtsConnectionInfoReaderTest
    {
        private static readonly string testPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "WmtsConnectionInfo");

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void ReadWmtsConnectionInfos_NoFilePath_ThrowArgumentException(string filePath)
        {
            // Setup
            var reader = new WmtsConnectionInfoReader();

            // Call
            TestDelegate call = () => reader.ReadWmtsConnectionInfos(filePath);

            // Assert
            const string expectedMessage = "bestandspad mag niet leeg of ongedefinieerd zijn.";
            string message = Assert.Throws<ArgumentException>(call).Message;
            StringAssert.Contains(expectedMessage, message);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FilePathIsDirectory_ThrowArgumentException()
        {
            // Setup
            var reader = new WmtsConnectionInfoReader();

            // Call
            TestDelegate call = () => reader.ReadWmtsConnectionInfos("c:/");

            // Assert
            const string expectedMessage = "bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            string message = Assert.Throws<ArgumentException>(call).Message;
            StringAssert.Contains(expectedMessage, message);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();
            var filePath = "c:/_.config".Replace('_', invalidPathChars[0]);
            var reader = new WmtsConnectionInfoReader();

            // Call
            TestDelegate call = () => reader.ReadWmtsConnectionInfos(filePath);

            // Assert
            string invalidChars = string.Join(", ", invalidPathChars);
            var expectedMessage = $"Fout bij het lezen van bestand 'c:/\".config': bestandspad mag niet de volgende tekens bevatten: {invalidChars}";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileMissing_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testPath, Path.GetRandomFileName());
            var reader = new WmtsConnectionInfoReader();

            // Call
            TestDelegate call = () => reader.ReadWmtsConnectionInfos(filePath);

            // Assert
            var expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand bestaat niet.";
            string actualMessage = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, actualMessage);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileWithTwoWmtsConnectionInfos_ReturnsExpectedWmtsConnectionInfos()
        {
            // Setup
            string filePath = Path.Combine(testPath, "twoValidWmtsConnectionInfos.txt");
            var reader = new WmtsConnectionInfoReader();

            // Call
            WmtsConnectionInfo[] readConnectionInfos = reader.ReadWmtsConnectionInfos(filePath).ToArray();

            // Assert
            Assert.AreEqual(2, readConnectionInfos.Length);
            var firstExpected = new WmtsConnectionInfo(@"Actueel Hoogtebestand Nederland (AHN1)", @"https://geodata.nationaalgeoregister.nl/tiles/service/wmts/ahn1?request=GetCapabilities");
            var secondExpected = new WmtsConnectionInfo(@"Zeegraskartering", @"https://geodata.nationaalgeoregister.nl/zeegraskartering/wfs?request=GetCapabilities");

            AssertAreEqual(firstExpected, readConnectionInfos[0]);
            AssertAreEqual(secondExpected, readConnectionInfos[1]);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileWithTwoWmtsConnectionInfosReversedOrder_ReturnsExpectedWmtsConnectionInfos()
        {
            // Setup
            string filePath = Path.Combine(testPath, "twoValidWmtsConnectionInfosReversedOrder.txt");
            var reader = new WmtsConnectionInfoReader();

            // Call
            WmtsConnectionInfo[] readConnectionInfos = reader.ReadWmtsConnectionInfos(filePath).ToArray();

            // Assert
            Assert.AreEqual(2, readConnectionInfos.Length);
            var firstExpected = new WmtsConnectionInfo(@"Actueel Hoogtebestand Nederland (AHN1)", @"https://geodata.nationaalgeoregister.nl/tiles/service/wmts/ahn1?request=GetCapabilities");
            var secondExpected = new WmtsConnectionInfo(@"Zeegraskartering", @"https://geodata.nationaalgeoregister.nl/zeegraskartering/wfs?request=GetCapabilities");

            AssertAreEqual(firstExpected, readConnectionInfos[0]);
            AssertAreEqual(secondExpected, readConnectionInfos[1]);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileWithoutWmtsConnectionInfos_ReturnsEmptyList()
        {
            // Setup
            string filePath = Path.Combine(testPath, "WmtsConnectionInfosZeroWmtsConnections.txt");
            var reader = new WmtsConnectionInfoReader();

            // Call
            WmtsConnectionInfo[] readConnectionInfos = reader.ReadWmtsConnectionInfos(filePath).ToArray();

            // Assert
            Assert.AreEqual(0, readConnectionInfos.Length);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileWithoutWmtsConnectionsElement_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testPath, "WmtsConnectionInfosWithoutWmtsConnectionsElement.txt");
            var reader = new WmtsConnectionInfoReader();

            // Call
            TestDelegate call = () => reader.ReadWmtsConnectionInfos(filePath);

            // Assert
            var exception = Assert.Throws<CriticalFileReadException>(call);
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand "
                                     + "kon niet worden geopend. Mogelijk is het bestand corrupt "
                                     + "of in gebruik door een andere applicatie.";
            Assert.AreEqual(expectedMessage, exception.Message);
            Assert.IsInstanceOf<XmlException>(exception.InnerException);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileEmptyUrlElement_WarnsAndReadsRestOfFile()
        {
            // Setup
            string filePath = Path.Combine(testPath, "twoWmtsConnectionInfosOneEmptyUrl.txt");
            var reader = new WmtsConnectionInfoReader();

            WmtsConnectionInfo[] readConnectionInfos = null;

            // Call
            Action action = () => { readConnectionInfos = reader.ReadWmtsConnectionInfos(filePath).ToArray(); };

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het is niet mogelijk om WMTS connectie First name aan te maken met URL ''.";
            TestHelper.AssertLogMessageWithLevelIsGenerated(action, Tuple.Create(expectedMessage, LogLevelConstant.Warn));

            Assert.IsNotNull(readConnectionInfos);
            Assert.AreEqual(1, readConnectionInfos.Length);
            var expectedWmtsConnectionInfo = new WmtsConnectionInfo(@"second name", @"https://domain.com");

            AssertAreEqual(expectedWmtsConnectionInfo, readConnectionInfos[0]);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileMissingOneUrlElement_SkipdAndReadsRestOfFile()
        {
            // Setup
            string filePath = Path.Combine(testPath, "twoWmtsConnectionInfosOneWithoutUrlElement.txt");
            var reader = new WmtsConnectionInfoReader();

            // Call
            WmtsConnectionInfo[] readConnectionInfos = reader.ReadWmtsConnectionInfos(filePath).ToArray();

            // Assert
            Assert.AreEqual(1, readConnectionInfos.Length);
            var expectedWmtsConnectionInfo = new WmtsConnectionInfo(@"second name", @"https://domain.com");

            AssertAreEqual(expectedWmtsConnectionInfo, readConnectionInfos[0]);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileMissingOneNameElement_SkipdAndReadsRestOfFile()
        {
            // Setup
            string filePath = Path.Combine(testPath, "twoWmtsConnectionInfosOneWithoutNameElement.txt");
            var reader = new WmtsConnectionInfoReader();

            // Call
            WmtsConnectionInfo[] readConnectionInfos = reader.ReadWmtsConnectionInfos(filePath).ToArray();

            // Assert
            Assert.AreEqual(1, readConnectionInfos.Length);
            var expectedWmtsConnectionInfo = new WmtsConnectionInfo(@"second name", @"https://domain.com");

            AssertAreEqual(expectedWmtsConnectionInfo, readConnectionInfos[0]);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileLocked_ThrowsCriticalFileReadException()
        {
            // Setup
            string filePath = Path.Combine(testPath, Path.GetRandomFileName());
            var reader = new WmtsConnectionInfoReader();

            using (var fileDisposeHelper = new FileDisposeHelper(filePath))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => reader.ReadWmtsConnectionInfos(filePath);

                // Assert
                var exception = Assert.Throws<CriticalFileReadException>(call);
                string expectedMessage = $"Fout bij het lezen van bestand '{filePath}': het bestand "
                                         + "kon niet worden geopend. Mogelijk is het bestand corrupt "
                                         + "of in gebruik door een andere applicatie.";
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        private static void AssertAreEqual(WmtsConnectionInfo expected, WmtsConnectionInfo actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Url, actual.Url);
        }
    }
}