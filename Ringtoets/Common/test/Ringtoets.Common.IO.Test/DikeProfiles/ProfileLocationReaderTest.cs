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
using System.Collections.Generic;
using System.IO;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.IO.DikeProfiles;

namespace Ringtoets.Common.IO.Test.DikeProfiles
{
    [TestFixture]
    public class ProfileLocationReaderTest
    {
        [Test]
        public void Constructor_ValidFilePath_ExpectedValues()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));

            // Call
            using (var reader = new ProfileLocationReader(validFilePath))
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
            TestDelegate call = () => new ProfileLocationReader(invalidFilePath);

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
                                                              Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));
            string invalidFilePath = validFilePath.Replace("1", invalidFileNameChars[1].ToString());

            // Call
            TestDelegate call = () => new ProfileLocationReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, string.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new ProfileLocationReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet verwijzen naar een lege bestandsnaam.",
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
            TestDelegate call = () => new ProfileLocationReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                                                invalidFilePath);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
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
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Core.Components.Gis.IO,
                                                                shapeFileName);

            // Call
            TestDelegate call = () => new ProfileLocationReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Kon geen punten vinden in dit bestand.",
                                                invalidFilePath);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("Voorlanden_12-2_WithoutId.shp", "ID")]
        [TestCase("Voorlanden_12-2_WithoutName.shp", "Naam")]
        [TestCase("Voorlanden_12-2_WithoutX0.shp", "X0")]
        public void Constructor_FileMissingAttributeColumn_ThrowCriticalFileReadException(
            string fileName, string missingColumnName)
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("DikeProfiles", fileName));

            // Call
            TestDelegate call = () => new ProfileLocationReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Het bestand heeft geen attribuut '{0}'. Dit attribuut is vereist.",
                                                missingColumnName);
            string message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        public void Constructor_FileInUse_ThrowCriticalFileReadException()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));

            using (new FileStream(validFilePath, FileMode.Open))
            {
                // Call
                TestDelegate call = () => new ProfileLocationReader(validFilePath);

                // Assert
                var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand kon niet worden geopend. Mogelijk is het bestand corrupt of in gebruik door een andere applicatie.",
                                                    validFilePath);
                var exception = Assert.Throws<CriticalFileReadException>(call);
                Assert.AreEqual(expectedMessage, exception.Message);
                Assert.IsInstanceOf<IOException>(exception.InnerException);
            }
        }

        [Test]
        public void GetLocationCount_FileWithFivePoints_GetFive()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));

            using (var reader = new ProfileLocationReader(validFilePath))
            {
                // Call
                int count = reader.GetLocationCount;

                // Assert
                Assert.AreEqual(5, count);
            }
        }

        [Test]
        [TestCase("Voorlanden 12-2.shp", 5)]
        [TestCase("Voorlanden_12-2_Alternative.shp", 9)]
        public void GetNextProfileLocation_FileWithNLocations_GetNDikeProfileLocations(
            string fileName, int expectedNumberOfDikeProfileLocations)
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", fileName));
            IList<ProfileLocation> dikeProfileLocations = new List<ProfileLocation>();

            using (var reader = new ProfileLocationReader(validFilePath))
            {
                // Call
                int count = reader.GetLocationCount;
                for (int i = 0; i < count; i++)
                {
                    dikeProfileLocations.Add(reader.GetNextProfileLocation());
                }

                // Assert
                Assert.AreEqual(expectedNumberOfDikeProfileLocations, dikeProfileLocations.Count);
            }
        }

        [Test]
        public void GetNextProfileLocation_FileWithNullId_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyId.shp"));

            using (var reader = new ProfileLocationReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetNextProfileLocation();

                // Assert
                var expectedMessage = "De locatie parameter 'ID' heeft geen waarde.";
                string message = Assert.Throws<LineParseException>(call).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void GetNextProfileLocation_FileWithNullX0_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyX0.shp"));

            using (var reader = new ProfileLocationReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetNextProfileLocation();

                // Assert
                var expectedMessage = "Het profiel heeft geen geldige waarde voor attribuut 'X0'.";
                string message = Assert.Throws<LineParseException>(call).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase("Voorlanden_12-2_IdWithSymbol.shp")]
        [TestCase("Voorlanden_12-2_IdWithWhitespace.shp")]
        public void GetNextProfileLocation_FileWithIllegalCharactersInId_ThrowCriticalFileReadException(string fileName)
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("DikeProfiles", fileName));

            using (var reader = new ProfileLocationReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetNextProfileLocation();

                // Assert
                var expectedMessage = "De locatie parameter 'ID' mag uitsluitend uit letters en cijfers bestaan.";
                string message = Assert.Throws<LineParseException>(call).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void GetNextProfileLocation_FileWithNullAsNameAttribute_GetLocations()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                Path.Combine("DikeProfiles", "Voorlanden_12-2_EmptyName.shp"));
            IList<ProfileLocation> dikeProfileLocations = new List<ProfileLocation>();

            using (var reader = new ProfileLocationReader(invalidFilePath))
            {
                int count = reader.GetLocationCount;
                for (int i = 0; i < count; i++)
                {
                    // Call
                    dikeProfileLocations.Add(reader.GetNextProfileLocation());
                }

                // Assert
                Assert.AreEqual(5, dikeProfileLocations.Count);
            }
        }

        [Test]
        public void GetNextProfileLocation_FileWithFivePoints_GetFiveLocationsWithCorrectAtrributes()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));
            IList<ProfileLocation> dikeProfileLocations = new List<ProfileLocation>();

            using (var reader = new ProfileLocationReader(validFilePath))
            {
                int count = reader.GetLocationCount;
                for (int i = 0; i < count; i++)
                {
                    // Call
                    dikeProfileLocations.Add(reader.GetNextProfileLocation());
                }

                // Assert
                Assert.AreEqual("profiel001", dikeProfileLocations[0].Id);
                Assert.AreEqual("profiel002", dikeProfileLocations[1].Id);
                Assert.AreEqual("profiel003", dikeProfileLocations[2].Id);
                Assert.AreEqual("profiel004", dikeProfileLocations[3].Id);
                Assert.AreEqual("profiel005", dikeProfileLocations[4].Id);

                Assert.AreEqual("profiel001", dikeProfileLocations[0].Name);
                Assert.AreEqual("profiel002", dikeProfileLocations[1].Name);
                Assert.AreEqual("profiel003", dikeProfileLocations[2].Name);
                Assert.AreEqual("profiel004", dikeProfileLocations[3].Name);
                Assert.AreEqual("profiel005", dikeProfileLocations[4].Name);

                Assert.AreEqual(-10.61273321, dikeProfileLocations[0].Offset);
                Assert.AreEqual(-9.4408575, dikeProfileLocations[1].Offset);
                Assert.AreEqual(8.25860742, dikeProfileLocations[2].Offset);
                Assert.AreEqual(-17.93475471, dikeProfileLocations[3].Offset);
                Assert.AreEqual(15.56165507, dikeProfileLocations[4].Offset);
            }
        }

        [Test]
        public void GetNextProfileLocation_FileWithFivePoints_GetFiveLocationsWithPoint2D()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));
            IList<ProfileLocation> dikeProfileLocations = new List<ProfileLocation>();

            using (var reader = new ProfileLocationReader(validFilePath))
            {
                int count = reader.GetLocationCount;
                for (int i = 0; i < count; i++)
                {
                    // Call
                    dikeProfileLocations.Add(reader.GetNextProfileLocation());
                }

                // Assert
                Assert.IsInstanceOf(typeof(Point2D), dikeProfileLocations[0].Point);
                Assert.IsInstanceOf(typeof(Point2D), dikeProfileLocations[1].Point);
                Assert.IsInstanceOf(typeof(Point2D), dikeProfileLocations[2].Point);
                Assert.IsInstanceOf(typeof(Point2D), dikeProfileLocations[3].Point);
                Assert.IsInstanceOf(typeof(Point2D), dikeProfileLocations[4].Point);
            }
        }

        [Test]
        public void GetNextProfileLocation_FileWithFivePoints_GetFivePoint2DsWithCorrectCoordinates()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                              Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));
            IList<ProfileLocation> dikeProfileLocations = new List<ProfileLocation>();

            using (var reader = new ProfileLocationReader(validFilePath))
            {
                int count = reader.GetLocationCount;
                for (int i = 0; i < count; i++)
                {
                    // Call
                    dikeProfileLocations.Add(reader.GetNextProfileLocation());
                }

                // Assert
                Assert.AreEqual(131223.21400000341, dikeProfileLocations[0].Point.X);
                Assert.AreEqual(133854.31200000079, dikeProfileLocations[1].Point.X);
                Assert.AreEqual(135561.0960000027, dikeProfileLocations[2].Point.X);
                Assert.AreEqual(136432.12250000238, dikeProfileLocations[3].Point.X);
                Assert.AreEqual(136039.49100000039, dikeProfileLocations[4].Point.X);

                Assert.AreEqual(548393.43800000288, dikeProfileLocations[0].Point.Y);
                Assert.AreEqual(545323.13749999879, dikeProfileLocations[1].Point.Y);
                Assert.AreEqual(541920.34149999847, dikeProfileLocations[2].Point.Y);
                Assert.AreEqual(538235.26300000318, dikeProfileLocations[3].Point.Y);
                Assert.AreEqual(533920.28050000477, dikeProfileLocations[4].Point.Y);
            }
        }
    }
}