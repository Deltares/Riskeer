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
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Writers;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Writers
{
    [TestFixture]
    public class ShapeFileWriterBaseTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var writer = new TestShapeFileWriterBase())
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(writer);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void SaveAs_NoFilePath_ThrowArgumentException(string filePath)
        {
            // Setup
            using (var writer = new TestShapeFileWriterBase())
            {
                // Call
                TestDelegate call = () => writer.SaveAs(filePath);
                
                // Assert
                var expectedMessage = "Bestandspad mag niet leeg of ongedefinieerd zijn.";
                var message = Assert.Throws<ArgumentException>(call).Message;
                StringAssert.Contains(expectedMessage, message);
            }
        }

        [Test]
        public void SaveAs_FilePathIsDirectory_ThrowArgumentException()
        {
            // Setup
            using (var writer = new TestShapeFileWriterBase())
            {
                // Call
                TestDelegate call = () => writer.SaveAs("c:/");

                // Assert
                var expectedMessage = "Bestandspad mag niet verwijzen naar een lege bestandsnaam.";
                var message = Assert.Throws<ArgumentException>(call).Message;
                StringAssert.Contains(expectedMessage, message);
            }
        }

        [Test]
        public void SaveAs_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            string invalidCharacter = invalidFileNameChars[0].ToString();
            var filePath = "c:/_.shp".Replace("_", invalidCharacter);

            using (var writer = new TestShapeFileWriterBase())
            {
                // Call
                TestDelegate call = () => writer.SaveAs(filePath);

                // Assert
                const string expectedMessage = "Fout bij het lezen van bestand 'c:/\".shp': Bestandspad mag niet de volgende tekens bevatten: \", <, >, |, \0, , , , , , , \a, \b, \t, \n, \v, \f, \r, , , , , , , , , , , , , , , , , , , :, *, ?, \\, /";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            }
        }

        private class TestShapeFileWriterBase : ShapeFileWriterBase<MapLineData>
        {
            public override void AddFeature(MapLineData mapData)
            {
                // do nothing
            }
        }
    }
}