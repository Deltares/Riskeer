using System;
using Core.Common.Base.IO;
using Core.Common.TestUtil;
using NUnit.Framework;
using Riskeer.Common.Data.Calculation;
using Riskeer.MacroStabilityInwards.IO.Exporters;

namespace Riskeer.MacroStabilityInwards.IO.Test.Exporters
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationGroupExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), "ValidFolderPath");

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }

        [Test]
        public void Constructor_CalculationGroupNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(null, string.Empty);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("calculationGroup", exception.ParamName);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("   ")]
        [TestCase("C:\\Not:Valid")]
        public void Constructor_InvalidFolderPath_ThrowsArgumentException(string folderPath)
        {
            // Call
            void Call() => new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), folderPath);

            // Assert
            Assert.Throws<ArgumentException>(Call);
        }

        [Test]
        public void Export_Always_ReturnsFalse()
        {
            // Setup
            var exporter = new MacroStabilityInwardsCalculationGroupExporter(new CalculationGroup(), "ValidFolderPath");

            // Call
            bool exportResult = exporter.Export();

            // Assert
            Assert.IsFalse(exportResult);
        }
    }
}