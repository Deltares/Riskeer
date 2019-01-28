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
using System.Collections.Generic;
using System.IO;
using Core.Common.Base.Geometry;
using Core.Common.Base.IO;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.Structures;

namespace Ringtoets.Common.IO.Test.Structures
{
    [TestFixture]
    public class StructureLocationReaderTest
    {
        [Test]
        public void Constructor_ValidFilePath_ExpectedValues()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            // Call
            using (var reader = new StructureLocationReader(validFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        [TestCase("")]
        [TestCase("      ")]
        [TestCase(null)]
        public void Constructor_FilePathIsNullOrWhiteSpace_ThrowArgumentException(string invalidFilePath)
        {
            // Call
            TestDelegate call = () => new StructureLocationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet leeg of ongedefinieerd zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathHasInvalidPathCharacter_ThrowArgumentException()
        {
            // Setup
            char[] invalidPathChars = Path.GetInvalidPathChars();

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));
            string invalidFilePath = validFilePath.Replace("e", invalidPathChars[1].ToString());

            // Call
            TestDelegate call = () => new StructureLocationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': "
                                     + "er zitten ongeldige tekens in het bestandspad. Alle tekens in het bestandspad moeten geldig zijn.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new StructureLocationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': bestandspad mag niet verwijzen naar een lege bestandsnaam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FileDoesNotExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "I_do_not_exist.shp");

            // Call
            TestDelegate call = () => new StructureLocationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand bestaat niet.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("Multiple_Polygon_with_ID.shp")]
        [TestCase("Multiple_PolyLine_with_ID.shp")]
        [TestCase("Single_Multi-Polygon_with_ID.shp")]
        [TestCase("Single_Multi-PolyLine_with_ID.shp")]
        [TestCase("Single_Polygon_with_ID.shp")]
        [TestCase("Single_PolyLine_with_ID.shp")]
        public void Constructor_ShapefileDoesNotHavePointFeatures_ThrowCriticalFileReadException(string shapeFileName)
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO, shapeFileName);

            // Call
            TestDelegate call = () => new StructureLocationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': kon geen punten vinden in dit bestand.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Constructor_ShapefileWithoutAttributeKWKIDENT_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("Structures", "StructuresWithoutKWKIDENT", "Kunstwerken.shp"));

            // Call
            TestDelegate call = () => new StructureLocationReader(invalidFilePath);

            // Assert
            string expectedMessage = $"Fout bij het lezen van bestand '{invalidFilePath}': het bestand heeft geen attribuut '{"KWKIDENT"}'. Dit attribuut is vereist.";
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Constructor_FileInUse_ThrowCriticalFileReadException()
        {
            // Setup
            string path = TestHelper.GetScratchPadPath($"{nameof(StructureLocationReaderTest)}.{nameof(Constructor_FileInUse_ThrowCriticalFileReadException)}");

            using (var fileDisposeHelper = new FileDisposeHelper(path))
            {
                fileDisposeHelper.LockFiles();

                // Call
                TestDelegate call = () => new StructureLocationReader(path);

                // Assert
                string expectedMessage = $"Fout bij het lezen van bestand '{path}': het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.";
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void GetStructureCount_FileWithSixPoints_GetSix()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));

            using (var reader = new StructureLocationReader(validFilePath))
            {
                // Call
                int count = reader.GetStructureCount;

                // Assert
                Assert.AreEqual(6, count);
            }
        }

        [Test]
        public void GetNextStructure_ShapefileWithoutAttributeKWKNAAM_NamesEqualAttributeKWKIDENT()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("Structures", "StructuresWithoutKWKNAAM", "Kunstwerken.shp"));
            var structures = new List<StructureLocation>();

            using (var reader = new StructureLocationReader(validFilePath))
            {
                // Call
                int count = reader.GetStructureCount;
                for (var i = 0; i < count; i++)
                {
                    structures.Add(reader.GetNextStructureLocation());
                }

                // Assert
                Assert.AreEqual(structures[0].Id, structures[0].Name);
                Assert.AreEqual(structures[1].Id, structures[1].Name);
                Assert.AreEqual(structures[2].Id, structures[2].Name);
                Assert.AreEqual(structures[3].Id, structures[3].Name);
                Assert.AreEqual(structures[4].Id, structures[4].Name);
                Assert.AreEqual(structures[5].Id, structures[5].Name);
            }
        }

        [Test]
        public void GetNextStructure_ShapefileAttributeKWKNAAMSometimesNullOrWhitespace_GetSixStructuresWithCorrectAttributes()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("Structures", "StructuresSomeWithEmptyKWKNAAM", "Kunstwerken.shp"));
            var structures = new List<StructureLocation>();

            using (var reader = new StructureLocationReader(validFilePath))
            {
                // Call
                int count = reader.GetStructureCount;
                for (var i = 0; i < count; i++)
                {
                    structures.Add(reader.GetNextStructureLocation());
                }

                // Assert
                Assert.AreEqual(6, structures.Count);

                Assert.AreEqual("Coupure Den Oever (90k1)", structures[0].Name);
                Assert.AreEqual("KUNST2", structures[1].Name);
                Assert.AreEqual("Gemaal Lely (93k4)", structures[2].Name);
                Assert.AreEqual("Gemaal de Stontele (94k1)", structures[3].Name);
                Assert.AreEqual("KUNST5", structures[4].Name);
                Assert.AreEqual("KUNST6", structures[5].Name);
            }
        }

        [Test]
        public void GetNextStructure_ShapefileAttributeKWKIDENTValuesAreNull_ThrowLineParseException()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("Structures", "StructuresWithNullKWKIDENT", "Kunstwerken.shp"));

            using (var reader = new StructureLocationReader(validFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetNextStructureLocation();

                // Assert
                var exception = Assert.Throws<LineParseException>(call);
                Assert.AreEqual("Het kunstwerk heeft geen geldige waarde voor attribuut 'KWKIDENT'.", exception.Message);
            }
        }

        [Test]
        public void GetNextStructure_FileWithSixStructures_GetSixStructuresWithCorrectAttributes()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("Structures", "CorrectFiles", "Kunstwerken.shp"));
            var structures = new List<StructureLocation>();

            using (var reader = new StructureLocationReader(validFilePath))
            {
                // Call
                int count = reader.GetStructureCount;
                for (var i = 0; i < count; i++)
                {
                    structures.Add(reader.GetNextStructureLocation());
                }

                // Assert
                Assert.AreEqual(6, structures.Count);

                Assert.AreEqual("KUNST1", structures[0].Id);
                Assert.AreEqual("KUNST2", structures[1].Id);
                Assert.AreEqual("KUNST3", structures[2].Id);
                Assert.AreEqual("KUNST4", structures[3].Id);
                Assert.AreEqual("KUNST5", structures[4].Id);
                Assert.AreEqual("KUNST6", structures[5].Id);

                Assert.AreEqual("Coupure Den Oever (90k1)", structures[0].Name);
                Assert.AreEqual("Gemaal Leemans (93k3)", structures[1].Name);
                Assert.AreEqual("Gemaal Lely (93k4)", structures[2].Name);
                Assert.AreEqual("Gemaal de Stontele (94k1)", structures[3].Name);
                Assert.AreEqual("Stontelerkeersluis (93k1)", structures[4].Name);
                Assert.AreEqual("Stontelerschutsluis (93k2)", structures[5].Name);

                Assert.IsInstanceOf(typeof(Point2D), structures[0].Point);
                Assert.IsInstanceOf(typeof(Point2D), structures[1].Point);
                Assert.IsInstanceOf(typeof(Point2D), structures[2].Point);
                Assert.IsInstanceOf(typeof(Point2D), structures[3].Point);
                Assert.IsInstanceOf(typeof(Point2D), structures[4].Point);
                Assert.IsInstanceOf(typeof(Point2D), structures[5].Point);

                Assert.AreEqual(131144.094, structures[0].Point.X);
                Assert.AreEqual(131538.705, structures[1].Point.X);
                Assert.AreEqual(135878.442, structures[2].Point.X);
                Assert.AreEqual(131225.017, structures[3].Point.X);
                Assert.AreEqual(131270.38, structures[4].Point.X);
                Assert.AreEqual(131507.119, structures[5].Point.X);

                Assert.AreEqual(549979.893, structures[0].Point.Y);
                Assert.AreEqual(548316.752, structures[1].Point.Y);
                Assert.AreEqual(532149.859, structures[2].Point.Y);
                Assert.AreEqual(548395.948, structures[3].Point.Y);
                Assert.AreEqual(548367.462, structures[4].Point.Y);
                Assert.AreEqual(548322.951, structures[5].Point.Y);
            }
        }
    }
}