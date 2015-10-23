using System;
using NUnit.Framework;

namespace Wti.Data.Test
{
    public class PipingOutputTest
    {
        [Test]
        public void GivenSomeValues_WhenConstructedWithValues_ThenPropertiesAreSet()
        {
            var random = new Random(22);
            var zuValue = random.NextDouble();
            var foSuValue = random.NextDouble();
            var zhValue = random.NextDouble();
            var foShValue = random.NextDouble();
            var zsValue = random.NextDouble();
            var foSsValue = random.NextDouble();

            var actual = new PipingOutput(zuValue, foSuValue, zhValue, foShValue, zsValue, foSsValue);

            Assert.That(actual.UpliftZValue, Is.EqualTo(zuValue));
            Assert.That(actual.UpliftFactorOfSafety, Is.EqualTo(foSuValue));
            Assert.That(actual.HeaveZValue, Is.EqualTo(zhValue));
            Assert.That(actual.HeaveFactorOfSafety, Is.EqualTo(foShValue));
            Assert.That(actual.SellmeijerZValue, Is.EqualTo(zsValue));
            Assert.That(actual.SellmeijerFactorOfSafety, Is.EqualTo(foSsValue));
        }
    }
}