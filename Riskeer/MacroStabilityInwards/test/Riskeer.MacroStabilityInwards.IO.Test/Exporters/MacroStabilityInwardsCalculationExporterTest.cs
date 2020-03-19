using Core.Common.Base.IO;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Exporters;

namespace Riskeer.MacroStabilityInwards.IO.Test.Exporters
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new MacroStabilityInwardsCalculationExporter();

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }
    }
}