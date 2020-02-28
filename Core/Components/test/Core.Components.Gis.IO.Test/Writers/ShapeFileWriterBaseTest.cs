// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Security.AccessControl;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.IO.Writers;
using DotSpatial.Data;
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
        public void CopyToFeature_MapDataNull_ThrowsArgumentNullException()
        {
            // Setup
            using (var writer = new TestShapeFileWriterBase())
            {
                // Call
                TestDelegate test = () => writer.CopyToFeature(null);

                // Assert
                var exception = Assert.Throws<ArgumentNullException>(test);
                Assert.AreEqual("featureBasedMapData", exception.ParamName);
            }
        }

        [Test]
        public void CopyToFeature_MapFeaturesEmpty_ThrowsArgumentException()
        {
            // Setup
            using (var writer = new TestShapeFileWriterBase())
            {
                // Call
                TestDelegate test = () => writer.CopyToFeature(new MapLineData("test"));

                // Assert
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, "Mapdata mag maar één feature bevatten.");
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
                const string expectedMessage = "bestandspad mag niet leeg of ongedefinieerd zijn.";
                string message = Assert.Throws<ArgumentException>(call).Message;
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
                const string expectedMessage = "bestandspad mag niet verwijzen naar een lege bestandsnaam.";
                string message = Assert.Throws<ArgumentException>(call).Message;
                StringAssert.Contains(expectedMessage, message);
            }
        }

        [Test]
        public void SaveAs_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();
            string filePath = "c:/_.shp".Replace('_', invalidPathChars[0]);

            using (var writer = new TestShapeFileWriterBase())
            {
                // Call
                TestDelegate call = () => writer.SaveAs(filePath);

                // Assert
                const string expectedMessage = "Fout bij het lezen van bestand 'c:/\".shp': "
                                               + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            }
        }

        [Test]
        public void SaveAs_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            string directoryPath = TestHelper.GetScratchPadPath(nameof(SaveAs_InvalidDirectoryRights_ThrowCriticalFileWriteException));
            string filePath = Path.Combine(directoryPath, "test.shp");

            using (var disposeHelper = new DirectoryDisposeHelper(TestHelper.GetScratchPadPath(), nameof(SaveAs_InvalidDirectoryRights_ThrowCriticalFileWriteException)))
            using (var writer = new TestShapeFileWriterBase())
            {
                disposeHelper.LockDirectory(FileSystemRights.Write);

                // Call
                TestDelegate call = () => writer.SaveAs(filePath);

                // Assert
                string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'.";
                string message = Assert.Throws<CriticalFileWriteException>(call).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        private class TestShapeFileWriterBase : ShapeFileWriterBase
        {
            protected override IFeature AddFeature(MapFeature mapFeature)
            {
                throw new NotImplementedException();
            }
        }
    }
}