using System;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.FailureMechanism;

namespace Ringtoets.Common.IO.Test
{
    [TestFixture]
    public class FailureMechanismSectionReaderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "traject_1-1_vakken.shp");

            // Call
            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_NoFilePath_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new FailureMechanismSectionReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "traject_1-1_vakken.shp");
            string invalidFilePath = validFilePath.Replace("_", invalidFileNameChars[1].ToString());

            // Call
            TestDelegate call = () => new FailureMechanismSectionReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, String.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new FailureMechanismSectionReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ShapefileDoesntExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                "I_do_not_exist.shp");

            // Call
            TestDelegate call = () => new FailureMechanismSectionReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                                                invalidFilePath);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("Multiple_Point_with_ID.shp")]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Single_Point_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        public void Constructor_ShapefileDoesNotHaveSinglePolyline_ThrowCriticalFileReadException(string shapeFileName)
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                shapeFileName);

            // Call
            TestDelegate call = () => new FailureMechanismSectionReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand mag uitsluitend polylijnen bevatten.",
                                                invalidFilePath);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("traject_1-1_vakken.shp", 62)]
        [TestCase("traject_227_vakken.shp", 6)]
        public void GetFailureMechanismSectionCount_ValidFilePath_ReturnElementCount(string shapeFileName, int expectedElementCount)
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              shapeFileName);

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Call
                int count = reader.GetFailureMechanismSectionCount();

                // Assert
                Assert.AreEqual(expectedElementCount, count);
            }
        }

        [Test]
        public void GetFailureMechanismSectionCount_FileLacksNameAttribute_ThrowCriticalFileReadException()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "traject_227_vakken_LacksVaknaamAttribute.shp");

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetFailureMechanismSectionCount();

                // Assert
                var message = Assert.Throws<CriticalFileReadException>(call).Message;
                Assert.AreEqual("Het bestand heeft geen attribuut 'Vaknaam' welke vereist is om een vakindeling te importeren.", message);
            }
        }

        [Test]
        public void ReadFailureMechanismSection_ValidFilePath1_ReturnElement()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "traject_1-1_vakken.shp");

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Call
                FailureMechanismSection section = reader.ReadFailureMechanismSection();

                // Assert
                Assert.AreEqual("1-1_0", section.Name);
                Point2D[] geometryPoints = section.Points.ToArray();
                Assert.AreEqual(10, geometryPoints.Length);
                Assert.AreEqual(209655.500, geometryPoints[4].X, 1e-6);
                Assert.AreEqual(610800.687, geometryPoints[4].Y, 1e-6);
            }
        }

        [Test]
        public void ReadFailureMechanismSection_ValidFilePath2_ReturnElement()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "traject_227_vakken.shp");

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Call
                FailureMechanismSection section = reader.ReadFailureMechanismSection();

                // Assert
                Assert.AreEqual("227_0", section.Name);
                Point2D[] geometryPoints = section.Points.ToArray();
                Assert.AreEqual(2, geometryPoints.Length);
                Assert.AreEqual(187518.859, geometryPoints[0].X, 1e-6);
                Assert.AreEqual(503867.688, geometryPoints[0].Y, 1e-6);
                Assert.AreEqual(187448.585284, geometryPoints[1].X, 1e-6);
                Assert.AreEqual(503832.715294, geometryPoints[1].Y, 1e-6);
            }
        }

        [Test]
        public void ReadFailureMechanismSection_ValidFilePathAndForAllElements_CorrectSectionNameRead()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "traject_1-1_vakken.shp");

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                var count = reader.GetFailureMechanismSectionCount();
                for (int i = 0; i < count; i++)
                {
                    // Call
                    FailureMechanismSection section = reader.ReadFailureMechanismSection();

                    // Assert
                    var expectedSectionName = string.Format("1-1_{0}", i);
                    Assert.AreEqual(expectedSectionName, section.Name,
                                    string.Format("Section name is not as expected at index {0}", i));
                }
            }
        }

        [Test]
        [TestCase("traject_227_vakken.shp")]
        [TestCase("traject_1-1_vakken.shp")]
        public void ReadFailureMechanismSection_ReadingToEndOfFile_ReturnNull(string shapeFileName)
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              shapeFileName);

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                int count = reader.GetFailureMechanismSectionCount();
                for (int i = 0; i < count; i++)
                {
                    reader.ReadFailureMechanismSection();
                }

                // Call
                FailureMechanismSection resultBeyondEndOfFile = reader.ReadFailureMechanismSection();

                // Assert
                Assert.IsNull(resultBeyondEndOfFile);
            }
        }

        [Test]
        public void ReadFailureMechanismSection_FileLacksNameAttribute_ThrowCriticalFileReadException()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "traject_227_vakken_LacksVaknaamAttribute.shp");

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Call
                TestDelegate call = () => reader.ReadFailureMechanismSection();

                // Assert
                var message = Assert.Throws<CriticalFileReadException>(call).Message;
                Assert.AreEqual("Het bestand heeft geen attribuut 'Vaknaam' welke vereist is om een vakindeling te importeren.", message);
            }
        }

        [Test]
        public void ReadFailureMechanismSection_FileHadMultiPolylines_ThrowCriticalFileReadException()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              "Artificial_MultiPolyline_vakken.shp");

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Call
                TestDelegate call = () =>
                {
                    reader.ReadFailureMechanismSection();
                    reader.ReadFailureMechanismSection();
                    reader.ReadFailureMechanismSection();
                    reader.ReadFailureMechanismSection();
                };

                // Assert
                var message = Assert.Throws<CriticalFileReadException>(call).Message;
                Assert.AreEqual("Het bestand heeft een of meerdere multi-polylijnen, welke niet ondersteund worden.", message);
            }
        }
    }
}