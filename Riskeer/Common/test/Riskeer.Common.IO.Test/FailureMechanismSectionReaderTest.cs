// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.FailureMechanism;

namespace Riskeer.Common.IO.Test
{
    [TestFixture]
    public class FailureMechanismSectionReaderTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

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
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              "traject_1-1_vakken.shp");
            string invalidFilePath = validFilePath.Replace("_", invalidPathChars[1].ToString());

            // Call
            TestDelegate call = () => new FailureMechanismSectionReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new FailureMechanismSectionReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ShapefileDoesntExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                                "I_do_not_exist.shp");

            // Call
            TestDelegate call = () => new FailureMechanismSectionReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
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
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': kon geen lijnen vinden in dit bestand.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("traject_1-1_vakken.shp", 62)]
        [TestCase("traject_227_vakken.shp", 6)]
        public void GetFailureMechanismSectionCount_ValidFilePath_ReturnElementCount(string shapeFileName, int expectedElementCount)
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", shapeFileName));

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
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "traject_227_vakken_LacksVaknaamAttribute.shp"));

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetFailureMechanismSectionCount();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = $"Fout bij het lezen van bestand '{validFilePath}': het bestand heeft geen attribuut 'Vaknaam'. Dit attribuut is vereist.";
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadFailureMechanismSection_ValidFilePath1_ReturnElement()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

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
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "traject_227_vakken.shp"));

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
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "traject_1-1_vakken.shp"));

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                int count = reader.GetFailureMechanismSectionCount();
                for (var i = 0; i < count; i++)
                {
                    // Call
                    FailureMechanismSection section = reader.ReadFailureMechanismSection();

                    // Assert
                    string expectedSectionName = $"1-1_{i}";
                    Assert.AreEqual(expectedSectionName, section.Name, $"Section name is not as expected at index {i}");
                }
            }
        }

        [Test]
        [TestCase("traject_227_vakken.shp")]
        [TestCase("traject_1-1_vakken.shp")]
        public void ReadFailureMechanismSection_ReadingToEndOfFile_ReturnNull(string shapeFileName)
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", shapeFileName));

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                int count = reader.GetFailureMechanismSectionCount();
                for (var i = 0; i < count; i++)
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
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "traject_227_vakken_LacksVaknaamAttribute.shp"));

            using (var reader = new FailureMechanismSectionReader(validFilePath))
            {
                // Call
                TestDelegate call = () => reader.ReadFailureMechanismSection();

                // Assert
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = $"Fout bij het lezen van bestand '{validFilePath}': het bestand heeft geen attribuut 'Vaknaam'. Dit attribuut is vereist.";
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadFailureMechanismSection_FileHadMultiPolylines_ThrowCriticalFileReadException()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO,
                                                              Path.Combine("FailureMechanismSections", "Artificial_MultiPolyline_vakken.shp"));

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
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                string expectedMessage = $"Fout bij het lezen van bestand '{validFilePath}': het bestand bevat één of meerdere multi-polylijnen. " +
                                         "Multi-polylijnen worden niet ondersteund.";
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void ReadFailureMechanismSection_FileInUse_ThrowCriticalFileReadException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath(nameof(ReadFailureMechanismSection_FileInUse_ThrowCriticalFileReadException));

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new FailureMechanismSectionReader(path);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }
    }
}