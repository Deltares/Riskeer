using Core.Common.Base.IO;
using NUnit.Framework;
using Riskeer.MacroStabilityInwards.IO.Exporters;

namespace Riskeer.MacroStabilityInwards.IO.Test
{
    [TestFixture]
    public class MacroStabilityInwardsCalculationGroupExporterTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Call
            var exporter = new MacroStabilityInwardsCalculationGroupExporter();

            // Assert
            Assert.IsInstanceOf<IFileExporter>(exporter);
        }
    }
}