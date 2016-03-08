using System;
using System.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using Core.Components.Gis.Data;
using Core.Components.Gis.IO.Readers;
using DotSpatial.Data;
using NUnit.Framework;

namespace Core.Components.Gis.IO.Test.Readers
{
    [TestFixture]
    public class ShapeFileReaderBaseTest
    {
        [Test]
        public void ParameteredConstructor_ExpectedValues()
        {
            // Setup
            string testFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                             "traject_10-1.shp");

            // Call
            using (var reader = new TestShapeFileReaderBase(testFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
                Assert.AreEqual(testFilePath, reader.GetFilePath);
                Assert.IsNull(reader.GetShapeFile);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void ParameteredConstructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new TestShapeFileReaderBase(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ParameteredConstructor_FileDoesNotExist_ThrowArgumentException()
        {
            // Call
            string pathToNotExistingShapeFile = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                           "I_do_not_exist.shp");
            TestDelegate call = () => new TestShapeFileReaderBase(pathToNotExistingShapeFile);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                                                pathToNotExistingShapeFile);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void ParameteredConstructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                              "traject_10-1.shp");
            string invalidFilePath = validFilePath.Replace("_", invalidFileNameChars[0].ToString());

            // Call
            TestDelegate call = () => new TestShapeFileReaderBase(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, String.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void ParameteredConstructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new TestShapeFileReaderBase(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        private class TestShapeFileReaderBase : ShapeFileReaderBase
        {
            public TestShapeFileReaderBase(string filePath) : base(filePath) {}

            public override FeatureBasedMapData ReadLine(string name = null)
            {
                return null;
            }

            public override FeatureBasedMapData ReadShapeFile(string name = null)
            {
                return null;
            }

            public override IFeature GetFeature(int index)
            {
                return null;
            }

            public string GetFilePath
            {
                get
                {
                    return FilePath;
                }
            }

            public Shapefile GetShapeFile
            {
                get
                {
                    return ShapeFile;
                }
            }
        }
    }
}