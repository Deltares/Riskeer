using Core.Common.Base;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;

namespace Ringtoets.Common.Forms.TestUtil.Test
{
    [TestFixture]
    public class TestCalculationTest
    {

        [Test]
        public void DefaultConstructor_PropertiesSet()
        {
            // Call
            var result = new TestCalculation();

            // Assert
            Assert.IsInstanceOf<ICalculation>(result);
            Assert.IsInstanceOf<Observable>(result);
            Assert.IsNull(result.Output);
            Assert.IsFalse(result.HasOutput);
        }
    }
}