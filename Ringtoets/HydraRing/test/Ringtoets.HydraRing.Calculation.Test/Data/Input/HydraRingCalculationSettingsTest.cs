using System;
using NUnit.Framework;
using Ringtoets.HydraRing.Calculation.Data.Input;

namespace Ringtoets.HydraRing.Calculation.Test.Data.Input
{
    [TestFixture]
    public class HydraRingCalculationSettingsTest
    {
        [Test]
        public void Constructor_HlcdFilePathNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraRingCalculationSettings(null,
                                                                       string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("hlcdFilePath", exception.ParamName);
        }

        [Test]
        public void Constructor_PreprocessorDirectoryNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new HydraRingCalculationSettings(string.Empty,
                                                                       null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("preprocessorDirectory", exception.ParamName);
        }

        [Test]
        public void Constructor_WithArguments_ExpectedValues()
        {
            // Setup
            const string hlcdFilePath = "hlcdFilePath";
            const string preProcessorDirectory = "PreprocessorDirectory";

            // Call
            var settings = new HydraRingCalculationSettings(hlcdFilePath,
                                                            preProcessorDirectory);

            // Assert
            Assert.AreEqual(hlcdFilePath, settings.HlcdFilePath);
            Assert.AreEqual(preProcessorDirectory, settings.PreprocessorDirectory);
        }
    }
}