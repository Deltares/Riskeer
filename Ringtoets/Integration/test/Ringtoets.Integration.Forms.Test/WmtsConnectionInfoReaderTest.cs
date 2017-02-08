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
using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;

namespace Ringtoets.Integration.Forms.Test
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
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            string invalidCharacter = invalidFileNameChars[0].ToString();
            var filePath = "c:/_.config".Replace("_", invalidCharacter);
            var reader = new WmtsConnectionInfoReader();

            // Call
            TestDelegate call = () => reader.ReadWmtsConnectionInfos(filePath);

            // Assert
            const string expectedMessage = "Fout bij het lezen van bestand 'c:/\".config': bestandspad " +
                                           "mag niet de volgende tekens bevatten: \", <, >, " +
                                           "|, \0, , , , , , , \a, \b, \t, \n, \v, \f, \r, " +
                                           ", , , , , , , , , , , , , , , , , , :, *, ?, \\, /";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ReadWmtsConnectionInfos_FileWithTwoWmtsConnectionInfos_ReturnsExpectedWmtsConnectionInfos()
        {
            // Setup
            string filePath = Path.Combine(testPath, "twoValidWmtsConnectionInfos.txt");
            var reader = new WmtsConnectionInfoReader();

            // Call
            List<WmtsConnectionInfo> readConnectionInfos = reader.ReadWmtsConnectionInfos(filePath);

            // Assert
            Assert.AreEqual(2, readConnectionInfos.Count);
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
            List<WmtsConnectionInfo> readConnectionInfos = reader.ReadWmtsConnectionInfos(filePath);

            // Assert
            Assert.AreEqual(2, readConnectionInfos.Count);
            var firstExpected = new WmtsConnectionInfo(@"Actueel Hoogtebestand Nederland (AHN1)", @"https://geodata.nationaalgeoregister.nl/tiles/service/wmts/ahn1?request=GetCapabilities");
            var secondExpected = new WmtsConnectionInfo(@"Zeegraskartering", @"https://geodata.nationaalgeoregister.nl/zeegraskartering/wfs?request=GetCapabilities");

            AssertAreEqual(firstExpected, readConnectionInfos[0]);
            AssertAreEqual(secondExpected, readConnectionInfos[1]);
        }

        private static void AssertAreEqual(WmtsConnectionInfo expected, WmtsConnectionInfo actual)
        {
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.Url, actual.Url);
        }
    }
}