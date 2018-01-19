// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Security.AccessControl;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.IO.Hydraulics;

namespace Ringtoets.Common.IO.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryLocationsExporterTest
    {
        [Test]
        public void Constructor_ValidParameters_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IHydraulicBoundaryLocationMetaDataAttributeNameProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            var hydraulicBoundaryLocationsExporter = new HydraulicBoundaryLocationsExporter(Enumerable.Empty<HydraulicBoundaryLocation>(), filePath, provider);

            // Assert
            Assert.IsInstanceOf<IFileExporter>(hydraulicBoundaryLocationsExporter);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_HydraulicBoundaryLocationsNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IHydraulicBoundaryLocationMetaDataAttributeNameProvider>();
            mocks.ReplayAll();

            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(null, filePath, provider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryLocations", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_FilePathNull_ThrowArgumentException()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IHydraulicBoundaryLocationMetaDataAttributeNameProvider>();
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2);

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, null, provider);

            // Assert
            Assert.Throws<ArgumentException>(call);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_DesignWaterLevelNameNull_ThrowArgumentNullException()
        {
            // Setup
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2);
            string filePath = TestHelper.GetScratchPadPath(Path.Combine("export", "test.shp"));

            // Call
            TestDelegate call = () => new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("metaDataAttributeNameProvider", exception.ParamName);
        }

        [Test]
        public void Export_ValidData_ReturnsTrueAndWritesCorrectData()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IHydraulicBoundaryLocationMetaDataAttributeNameProvider>();
            provider.Stub(p => p.DesignWaterLevelCalculation1AttributeName).Return("h(A+_A)");
            provider.Stub(p => p.DesignWaterLevelCalculation2AttributeName).Return("h(A_B)");
            provider.Stub(p => p.DesignWaterLevelCalculation3AttributeName).Return("h(B_C)");
            provider.Stub(p => p.DesignWaterLevelCalculation4AttributeName).Return("h(C_D)");
            provider.Stub(p => p.WaveHeightCalculation1AttributeName).Return("Hs(A+_A)");
            provider.Stub(p => p.WaveHeightCalculation2AttributeName).Return("Hs(A_B)");
            provider.Stub(p => p.WaveHeightCalculation3AttributeName).Return("Hs(B_C)");
            provider.Stub(p => p.WaveHeightCalculation4AttributeName).Return("Hs(C_D)");
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2);
            SetHydraulicBoundaryLocationOutput(hydraulicBoundaryLocation);

            string directoryPath = TestHelper.GetScratchPadPath("Export_ValidData_ReturnTrue");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");
            const string baseName = "test";

            var exporter = new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath, provider);

            // Precondition
            AssertEssentialShapefileExists(directoryPath, baseName, false);

            bool isExported;
            try
            {
                // Call
                isExported = exporter.Export();

                // Assert
                AssertEssentialShapefileExists(directoryPath, baseName, true);
                AssertEssentialShapefileMd5Hashes(directoryPath, baseName);
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }

            Assert.IsTrue(isExported);
            mocks.VerifyAll();
        }

        [Test]
        public void Export_InvalidDirectoryRights_LogErrorAndReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var provider = mocks.Stub<IHydraulicBoundaryLocationMetaDataAttributeNameProvider>();
            provider.Stub(p => p.DesignWaterLevelCalculation1AttributeName).Return("h(A+_A)");
            provider.Stub(p => p.DesignWaterLevelCalculation2AttributeName).Return("h(A_B)");
            provider.Stub(p => p.DesignWaterLevelCalculation3AttributeName).Return("h(B_C)");
            provider.Stub(p => p.DesignWaterLevelCalculation4AttributeName).Return("h(C_D)");
            provider.Stub(p => p.WaveHeightCalculation1AttributeName).Return("Hs(A+_A)");
            provider.Stub(p => p.WaveHeightCalculation2AttributeName).Return("Hs(A_B)");
            provider.Stub(p => p.WaveHeightCalculation3AttributeName).Return("Hs(B_C)");
            provider.Stub(p => p.WaveHeightCalculation4AttributeName).Return("Hs(C_D)");
            mocks.ReplayAll();

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(123, "aName", 1.1, 2.2);

            string directoryPath = TestHelper.GetScratchPadPath("Export_InvalidDirectoryRights_LogErrorAndReturnFalse");
            Directory.CreateDirectory(directoryPath);
            string filePath = Path.Combine(directoryPath, "test.shp");

            var exporter = new HydraulicBoundaryLocationsExporter(new[]
            {
                hydraulicBoundaryLocation
            }, filePath, provider);

            try
            {
                using (new DirectoryPermissionsRevoker(directoryPath, FileSystemRights.Write))
                {
                    // Call
                    var isExported = true;
                    Action call = () => isExported = exporter.Export();

                    // Assert
                    string expectedMessage = $"Er is een onverwachte fout opgetreden tijdens het schrijven van het bestand '{filePath}'. " +
                                             "Er zijn geen hydraulische randvoorwaarden locaties geëxporteerd.";
                    TestHelper.AssertLogMessageIsGenerated(call, expectedMessage);
                    Assert.IsFalse(isExported);
                }
            }
            finally
            {
                Directory.Delete(directoryPath, true);
            }

            mocks.VerifyAll();
        }

        private static void SetHydraulicBoundaryLocationOutput(HydraulicBoundaryLocation location)
        {
            location.DesignWaterLevelCalculation1.Output = new TestHydraulicBoundaryLocationOutput(111.111);
            location.DesignWaterLevelCalculation2.Output = new TestHydraulicBoundaryLocationOutput(333.333);
            location.DesignWaterLevelCalculation3.Output = new TestHydraulicBoundaryLocationOutput(555.555);
            location.DesignWaterLevelCalculation4.Output = new TestHydraulicBoundaryLocationOutput(777.777);

            location.WaveHeightCalculation1.Output = new TestHydraulicBoundaryLocationOutput(222.222);
            location.WaveHeightCalculation2.Output = new TestHydraulicBoundaryLocationOutput(444.444);
            location.WaveHeightCalculation3.Output = new TestHydraulicBoundaryLocationOutput(666.666);
            location.WaveHeightCalculation4.Output = new TestHydraulicBoundaryLocationOutput(888.888);
        }

        private static void AssertEssentialShapefileExists(string directoryPath, string baseName, bool shouldExist)
        {
            string pathName = Path.Combine(directoryPath, baseName);
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shp"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".shx"));
            Assert.AreEqual(shouldExist, File.Exists(pathName + ".dbf"));
        }

        private static void AssertEssentialShapefileMd5Hashes(string directoryPath, string baseName)
        {
            string refPathName = Path.Combine(TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO), "PointShapefileMd5");
            string pathName = Path.Combine(directoryPath, baseName);

            AssertBinaryFileContent(refPathName, pathName, ".shp", 100, 28);
            AssertBinaryFileContent(refPathName, pathName, ".shx", 100, 8);
            AssertBinaryFileContent(refPathName, pathName, ".dbf", 32, 741);
        }

        private static void AssertBinaryFileContent(string refPathName, string pathName, string extension, int headerLength, int bodyLength)
        {
            byte[] refContent = File.ReadAllBytes(refPathName + extension);
            byte[] content = File.ReadAllBytes(pathName + extension);
            Assert.AreEqual(headerLength + bodyLength, content.Length);
            Assert.AreEqual(refContent.Skip(headerLength).Take(bodyLength),
                            content.Skip(headerLength).Take(bodyLength));
        }
    }
}