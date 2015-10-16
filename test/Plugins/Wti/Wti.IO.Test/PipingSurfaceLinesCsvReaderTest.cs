﻿using System;
using System.IO;
using System.Linq;

using DelftTools.TestUtils;

using NUnit.Framework;

namespace Wti.IO.Test
{
    [TestFixture]
    public class PipingSurfaceLinesCsvReaderTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Plugins.Wti.WtiIOPath, "PipingSurfaceLinesCsvReader");

        [Test]
        [TestCase("")]
        [TestCase(null)]
        [TestCase("     ")]
        public void ParameterdConstructor_InvalidStringArgument_ArgumentException(string path)
        {
            // Call
            TestDelegate call = () => new PipingSurfaceLinesCsvReader(path);

            // Assert
            var exception = Assert.Throws<ArgumentException>(call);
            Assert.AreEqual("Bestandspad mag niet leeg of ongedefinieerd zijn.", exception.Message);
        }

        [Test]
        public void ParameterdConstructor_AnyPath_ExpectedValues()
        {
            // Setup
            const string fakeFilePath = @"I\Dont\Really\Exist";

            // Call
            using (var reader = new PipingSurfaceLinesCsvReader(fakeFilePath))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(reader);
            }
        }

        [Test]
        public void GetLineCount_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnNumberOfSurfaceLines()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                int linesCount = reader.GetSurfaceLinesCount();

                // Assert
                Assert.AreEqual(2, linesCount);
            }
        }

        [Test]
        public void GetLineCount_OpenedValidFileWithHeaderAndNoSurfaceLines_ReturnZero()
        {
            // Setup
            string path = Path.Combine(testDataPath, "ValidFileWithoutSurfaceLines.csv");

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                int linesCount = reader.GetSurfaceLinesCount();

                // Assert
                Assert.AreEqual(0, linesCount);
            }
        }

        [Test]
        [SetCulture("nl-NL")]
        public void ReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLinesWithCultureNL_ReturnCreatedSurfaceLine()
        {
            DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnCreatedSurfaceLine();
        }

        [Test]
        [SetCulture("en-US")]
        public void ReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLinesWithCultureEN_ReturnCreatedSurfaceLine()
        {
            DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnCreatedSurfaceLine();
        }

        private void DoReadLine_OpenedValidFileWithHeaderAndTwoSurfaceLines_ReturnCreatedSurfaceLine()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                // Call
                var surfaceLine1 = reader.ReadLine();
                var surfaceLine2 = reader.ReadLine();

                // Assert

                #region 1st surfaceline

                Assert.AreEqual("Rotterdam1", surfaceLine1.Name);
                Assert.AreEqual(8, surfaceLine1.Points.Count());
                Assert.AreEqual(94263.0026213, surfaceLine1.StartingWorldPoint.X);
                Assert.AreEqual(427776.654093, surfaceLine1.StartingWorldPoint.Y);
                Assert.AreEqual(-1.02, surfaceLine1.StartingWorldPoint.Z);
                Assert.AreEqual(94331.1767309, surfaceLine1.EndingWorldPoint.X);
                Assert.AreEqual(427960.112661, surfaceLine1.EndingWorldPoint.Y);
                Assert.AreEqual(1.44, surfaceLine1.EndingWorldPoint.Z);
                Assert.AreEqual(surfaceLine1.StartingWorldPoint, surfaceLine1.Points.First());
                Assert.AreEqual(surfaceLine1.EndingWorldPoint, surfaceLine1.Points.Last());

                #endregion

                #region 2nd surfaceline

                Assert.AreEqual("ArtificalLocal", surfaceLine2.Name);
                Assert.AreEqual(3, surfaceLine2.Points.Count());
                Assert.AreEqual(2.3, surfaceLine2.StartingWorldPoint.X);
                Assert.AreEqual(0, surfaceLine2.StartingWorldPoint.Y);
                Assert.AreEqual(1, surfaceLine2.StartingWorldPoint.Z);
                Assert.AreEqual(4.4, surfaceLine2.EndingWorldPoint.X);
                Assert.AreEqual(0, surfaceLine2.EndingWorldPoint.Y);
                Assert.AreEqual(1.1, surfaceLine2.EndingWorldPoint.Z);
                Assert.AreEqual(surfaceLine2.StartingWorldPoint, surfaceLine2.Points.First());
                Assert.AreEqual(surfaceLine2.EndingWorldPoint, surfaceLine2.Points.Last());

                #endregion
            }
        }

        [Test]
        public void ReadLine_OpenedValidFileWithoutHeaderAndTwoSurfaceLinesWhileAtTheEndOfFile_ReturnNull()
        {
            // Setup
            string path = Path.Combine(testDataPath, "TwoValidSurfaceLines.csv");

            using (var reader = new PipingSurfaceLinesCsvReader(path))
            {
                int surfaceLinesCount = reader.GetSurfaceLinesCount();
                for (int i = 0; i < surfaceLinesCount; i++)
                {
                    var pipingSurfaceLine = reader.ReadLine();
                    Assert.IsNotInstanceOf<IDisposable>(pipingSurfaceLine,
                        "Fail Fast: Disposal logic required to be implemented in test.");
                    Assert.IsNotNull(pipingSurfaceLine);
                }

                // Call
                var result = reader.ReadLine();

                // Assert
                Assert.IsNull(result);
            }
        }
    }
}