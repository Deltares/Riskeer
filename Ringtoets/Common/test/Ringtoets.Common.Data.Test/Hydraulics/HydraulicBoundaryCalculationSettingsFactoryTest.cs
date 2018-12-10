using System;
using System.Collections.Generic;
using System.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Hydraulics;

namespace Ringtoets.Common.Data.Test.Hydraulics
{
    [TestFixture]
    public class HydraulicBoundaryCalculationSettingsFactoryTest
    {
        [Test]
        public void CreateSettings_HydraulicBoundaryDatabaseNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => HydraulicBoundaryCalculationSettingsFactory.CreateSettings(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hydraulicBoundaryDatabase", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithInvalidFilePath_ReturnsExpectedSettings(string filePath)
        {
            // Setup
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = filePath
            };

            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(database);

            // Assert
            Assert.AreEqual(database.FilePath, settings.HydraulicBoundaryDatabaseFilePath);
            Assert.IsNull(settings.HlcdFilePath);
        }

        [Test]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithFilePath_ReturnsExpectedSettings()
        {
            // Setup
            string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Service, "HydraRingCalculation");
            string validFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var database = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            };

            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(database);

            // Assert
            Assert.AreEqual(validFilePath, settings.HydraulicBoundaryDatabaseFilePath);
            Assert.AreEqual(Path.Combine(testDataPath, "HLCD.sqlite"), settings.HlcdFilePath);
        }

        [Test]
        [TestCaseSource(nameof(GetPreprocessorConfigurations))]
        public void CreateSettings_WithHydraulicBoundaryDatabaseWithVariousPreprocessorConfigurations_ReturnsExpectedSettings(
            HydraulicBoundaryDatabase database,
            string expectedPreprocessorDirectory)
        {
            // Setup

            // Call
            HydraulicBoundaryCalculationSettings settings = HydraulicBoundaryCalculationSettingsFactory.CreateSettings(database);

            // Assert
            Assert.AreEqual(expectedPreprocessorDirectory, settings.PreprocessorDirectory);
        }

        private static IEnumerable<TestCaseData> GetPreprocessorConfigurations()
        {
            yield return new TestCaseData(new HydraulicBoundaryDatabase(),
                                          string.Empty)
                .SetName("UsePreprocessorFalse");

            yield return new TestCaseData(new HydraulicBoundaryDatabase
                {
                    CanUsePreprocessor = true,
                    PreprocessorDirectory = "Directory"
                }, string.Empty)
                .SetName("UsePreprocessorFalseWithPreprocessorDirectory");

            const string preprocessorDirectory = "Directory";
            yield return new TestCaseData(new HydraulicBoundaryDatabase
                {
                    CanUsePreprocessor = true,
                    UsePreprocessor = true,
                    PreprocessorDirectory = preprocessorDirectory
                }, preprocessorDirectory)
                .SetName("UsePreprocessorTrueWithPreprocessorDirectory");
        }
    }
}