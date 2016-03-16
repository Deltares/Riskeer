using NUnit.Framework;

namespace Ringtoets.Piping.Data.Test
{
    public class PipingSemiProbabilisticOutputTest
    {
        [Test]
        public void Constructor_DefaultPropertiesSet()
        {
            // Call
            var output = new PipingSemiProbabilisticOutput();

            // Assert
            Assert.IsNaN(output.PipingFactorOfSafety);
        } 
    }
}