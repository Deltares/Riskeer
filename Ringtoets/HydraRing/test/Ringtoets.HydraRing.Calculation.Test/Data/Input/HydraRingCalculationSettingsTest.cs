using System;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input
{
    [TestFixture]
    public class HydraRingCalculationSettingsTest
    {
        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_WithInvalidHydraulicBoundaryDatabaseFilePath_ThrowsArgumentException(
            string invalidHydraulicBoundaryDatabaseFilePath)
        {
            // Setup
            string hlcdFilePath = TestHelper.GetScratchPadPath();
            string preProcessorDirectory = TestHelper.GetScratchPadPath();

            // Call
            TestDelegate call = () => new HydraRingCalculationSettings(invalidHydraulicBoundaryDatabaseFilePath,
                                                                       hlcdFilePath,
                                                                       preProcessorDirectory);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        [TestCaseSource(typeof(InvalidPathHelper), nameof(InvalidPathHelper.InvalidPaths))]
        public void Constructor_WithInvalidHydraulicBoundaryLocationsConfigurationsFilePath_ThrowsArgumentException(
            string invalidHlcdFilePath)
        {
            // Setup
            string hydraulicBoundaryLocationsDatabaseFilePath = TestHelper.GetScratchPadPath();
            string preProcessorDirectory = TestHelper.GetScratchPadPath();

            // Call
            TestDelegate call = () => new HydraRingCalculationSettings(hydraulicBoundaryLocationsDatabaseFilePath,
                                                                       invalidHlcdFilePath,
                                                                       preProcessorDirectory);

            // Assert
            Assert.Throws<ArgumentException>(call);
        }

        [Test]
        public void Constructor_WithPreprocessorDirectoryNull_ThrowsArgumentNullException()
        {
            // Setup
            string hydraulicBoundaryLocationsDatabaseFilePath = TestHelper.GetScratchPadPath();
            string hlcdFilePath = TestHelper.GetScratchPadPath();

            // Call
            TestDelegate call = () => new HydraRingCalculationSettings(hydraulicBoundaryLocationsDatabaseFilePath,
                                                                       hlcdFilePath,
                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preprocessorDirectory", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            string hydraulicBoundaryDatabaseFilePath = TestHelper.GetScratchPadPath();
            string hlcdFilePath = TestHelper.GetScratchPadPath();
            string preProcessorDirectory = TestHelper.GetScratchPadPath();

            // Call
            var settings = new HydraRingCalculationSettings(hydraulicBoundaryDatabaseFilePath,
                                                            hlcdFilePath,
                                                            preProcessorDirectory);

            // Assert
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HydraulicBoundaryDatabaseFilePath);
            Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HydraulicBoundaryLocationsConfigurationFilePath);
            Assert.AreEqual(preProcessorDirectory, settings.PreprocessorDirectory);
        }
    }
}