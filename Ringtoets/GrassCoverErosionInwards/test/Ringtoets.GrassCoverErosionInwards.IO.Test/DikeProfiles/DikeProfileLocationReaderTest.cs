using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.IO.Exceptions;
using Core.Common.TestUtil;
using NUnit.Framework;

using Ringtoets.GrassCoverErosionInwards.IO.DikeProfiles;

namespace Ringtoets.GrassCoverErosionInwards.IO.Test.DikeProfiles
{
    [TestFixture]
    public class DikeProfileLocationReaderTest
    {
        [Test]
        public void Constructor_ValidFilePath_ExpectedValues()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                  Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));

            // Call
            using (var reader = new DikeProfileLocationReader(validFilePath))
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
            TestDelegate call = () => new DikeProfileLocationReader(invalidFilePath);

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

            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                              Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));
            string invalidFilePath = validFilePath.Replace("1", invalidFileNameChars[1].ToString());

            // Call
            TestDelegate call = () => new DikeProfileLocationReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet de volgende tekens bevatten: {1}",
                                                invalidFilePath, String.Join(", ", invalidFileNameChars));
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_FilePathIsActuallyDirectoryPath_ThrowArgumentException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                                Path.DirectorySeparatorChar.ToString());

            // Call
            TestDelegate call = () => new DikeProfileLocationReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Bestandspad mag niet naar een map verwijzen.",
                                                invalidFilePath);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_ShapefileDoesntExist_ThrowCriticalFileReadException()
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                                                "I_do_not_exist.shp");

            // Call
            TestDelegate call = () => new DikeProfileLocationReader(invalidFilePath);

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
            TestDelegate call = () => new DikeProfileLocationReader(invalidFilePath);

            // Assert
            var expectedMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand mag uitsluitend punten bevatten.",
                                                invalidFilePath);
            var message = Assert.Throws<CriticalFileReadException>(call).Message;
            Assert.AreEqual(expectedMessage, message);
        }

        [Test]
        [TestCase("Voorlanden 12-2.shp", 5)]
        [TestCase("Voorlanden_12-2_Alternative.shp", 9)]
        public void GetDikeProfileLocationCount_FileWithFiveElements_ReturnFive(
            string fileName, int expectedNumberOfElements)
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                      Path.Combine("DikeProfiles", fileName));

            using (var reader = new DikeProfileLocationReader(validFilePath))
            {
                // Call
                int count = reader.GetDikeProfileLocations().Count;

                // Assert
                Assert.AreEqual(expectedNumberOfElements, count);
            }
        }

        [Test]
        [TestCase("Voorlanden_12-2_WithoutId.shp", "ID")]
        [TestCase("Voorlanden_12-2_WithoutName.shp", "Naam")]
        [TestCase("Voorlanden_12-2_WithoutX0.shp", "X0")]
        public void GetDikeProfileLocationCount_FileWithoutIdColumn_ThrowCriticalFileReadException(
            string fileName, string missingColumnName)
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                          Path.Combine("DikeProfiles", fileName));

            using (var reader = new DikeProfileLocationReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetDikeProfileLocations();

                // Assert
                var expectedMessage = string.Format("Het bestand heeft geen attribuut '{0}' welke vereist is om de locaties van de dijkprofielen in te lezen.",
                                                    missingColumnName);
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        [TestCase("Voorlanden_12-2_EmptyId.shp", "ID")]
        [TestCase("Voorlanden_12-2_EmptyName.shp", "Naam")]
        [TestCase("Voorlanden_12-2_EmptyX0.shp", "X0")]
        public void GetDikeProfileLocations_FileWithEmptyEntryInColumn_ThrowCriticalFileReadException(
            string fileName, string offendingColumnName)
        {
            // Setup
            string invalidFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                          Path.Combine("DikeProfiles", fileName));

            using (var reader = new DikeProfileLocationReader(invalidFilePath))
            {
                // Call
                TestDelegate call = () => reader.GetDikeProfileLocations();

                // Assert
                var expectedMessage = string.Format("Het bestand heeft een attribuut '{0}' zonder geldige waarde, welke vereist is om de locaties van de dijkprofielen in te lezen.",
                                                    offendingColumnName);
                string message = Assert.Throws<CriticalFileReadException>(call).Message;
                Assert.AreEqual(expectedMessage, message);
            }
        }

        [Test]
        public void GetDikeProfileLocations_FileWithFivePoints_GetFiveLocations()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                      Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));

            using (var reader = new DikeProfileLocationReader(validFilePath))
            {
                // Call
                IEnumerable<DikeProfileLocation> locations = reader.GetDikeProfileLocations();

                // Assert
                Assert.AreEqual(5, locations.Count());
            }
        }

        [Test]
        public void GetDikeProfileLocations_FileWithFivePoints_GetFiveLocationsWithCorrectAtrributes()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                      Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));

            using (var reader = new DikeProfileLocationReader(validFilePath))
            {
                // Call
                IList<DikeProfileLocation> locations = reader.GetDikeProfileLocations();

                // Assert
                Assert.AreEqual("profiel001", locations[0].Id);
                Assert.AreEqual("profiel002", locations[1].Id);
                Assert.AreEqual("profiel003", locations[2].Id);
                Assert.AreEqual("profiel004", locations[3].Id);
                Assert.AreEqual("profiel005", locations[4].Id);

                Assert.AreEqual("profiel001", locations[0].Name);
                Assert.AreEqual("profiel002", locations[1].Name);
                Assert.AreEqual("profiel003", locations[2].Name);
                Assert.AreEqual("profiel004", locations[3].Name);
                Assert.AreEqual("profiel005", locations[4].Name);

                Assert.AreEqual(-10.61273321, locations[0].X0);
                Assert.AreEqual(-9.4408575, locations[1].X0);
                Assert.AreEqual(8.25860742, locations[2].X0);
                Assert.AreEqual(-17.93475471, locations[3].X0);
                Assert.AreEqual(15.56165507, locations[4].X0);
            }
        }

        [Test]
        public void GetDikeProfileLocations_FileWithFivePoints_GetFiveLocationsWithPoint2D()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                      Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));

            using (var reader = new DikeProfileLocationReader(validFilePath))
            {
                // Call
                IList<DikeProfileLocation> locations = reader.GetDikeProfileLocations();

                // Assert
                Assert.IsInstanceOf(typeof(Point2D), locations[0].Point);
                Assert.IsInstanceOf(typeof(Point2D), locations[1].Point);
                Assert.IsInstanceOf(typeof(Point2D), locations[2].Point);
                Assert.IsInstanceOf(typeof(Point2D), locations[3].Point);
                Assert.IsInstanceOf(typeof(Point2D), locations[4].Point);
            }
        }

        [Test]
        public void GetDikeProfileLocations_FileWithFivePoints_GetFivePoint2DsWithCorrectCoordinates()
        {
            // Setup
            string validFilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.GrassCoverErosionInwards.IO,
                                      Path.Combine("DikeProfiles", "Voorlanden 12-2.shp"));

            using (var reader = new DikeProfileLocationReader(validFilePath))
            {
                // Call
                IList<DikeProfileLocation> locations = reader.GetDikeProfileLocations();

                // Assert
                Assert.AreEqual(131223.21400000341, locations[0].Point.X);
                Assert.AreEqual(133854.31200000079, locations[1].Point.X);
                Assert.AreEqual(135561.0960000027, locations[2].Point.X);
                Assert.AreEqual(136432.12250000238, locations[3].Point.X);
                Assert.AreEqual(136039.49100000039, locations[4].Point.X);

                Assert.AreEqual(548393.43800000288, locations[0].Point.Y);
                Assert.AreEqual(545323.13749999879, locations[1].Point.Y);
                Assert.AreEqual(541920.34149999847, locations[2].Point.Y);
                Assert.AreEqual(538235.26300000318, locations[3].Point.Y);
                Assert.AreEqual(533920.28050000477, locations[4].Point.Y);
            }
        }
    }
}