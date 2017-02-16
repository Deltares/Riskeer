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
using System.Security.AccessControl;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.IO.Properties;
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
                var expectedMessage = Resources.ShapeFileWriterBase_CopyToFeature_Mapdata_can_only_contain_one_feature;
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
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
                var expectedMessage = "bestandspad mag niet leeg of ongedefinieerd zijn.";
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
                var expectedMessage = "bestandspad mag niet verwijzen naar een lege bestandsnaam.";
                var message = Assert.Throws<ArgumentException>(call).Message;
                StringAssert.Contains(expectedMessage, message);
            }
        }

        [Test]
        public void SaveAs_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();
            var filePath = "c:/_.shp".Replace('_', invalidPathChars[0]);

            using (var writer = new TestShapeFileWriterBase())
            {
                // Call
                TestDelegate call = () => writer.SaveAs(filePath);

                // Assert
                var expectedMessage = "Fout bij het lezen van bestand 'c:/\".shp': "
                                      + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
                TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            }
        }

        [Test]
        public void SaveAs_InvalidDirectoryRights_ThrowCriticalFileWriteException()
        {
            string directoryPath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                              "SaveAs_InvalidDirectoryRights_ThrowCriticalFileWriteException");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                using (var writer = new TestShapeFileWriterBase())
                {
                    // Call
                    TestDelegate call = () => writer.SaveAs(filePath);

                    // Assert
                    var expectedMessage = string.Format("Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{0}'.", filePath);
                    var message = Assert.Throws<CriticalFileWriteException>(call).Message;
                    Assert.AreEqual(expectedMessage, message);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
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