using System;
using NUnit.Framework;

namespace Wti.Calculation.Test
{
    public class PipingCalculationTest
    {
        [Test]
        public void GivenACompleteInput_WhenCalculationPerformed_ThenAResultIsReturned()
        {
            Random random = new Random(22);
            PipingCalculationInput input = new PipingCalculationInput(
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble(),
                random.NextDouble());
            PipingCalculationResult actual = new PipingCalculation(input).Calculate();

            Assert.That(actual.UpliftZValue, Is.Not.EqualTo(0));
            Assert.That(actual.UpliftFactorOfSafety, Is.Not.EqualTo(0));
            Assert.That(actual.HeaveZValue, Is.Not.EqualTo(0));
            Assert.That(actual.HeaveFactorOfSafety, Is.Not.EqualTo(0));
            Assert.That(actual.SellmeijerZValue, Is.Not.EqualTo(0));
            Assert.That(actual.SellmeijerFactorOfSafety, Is.Not.EqualTo(0));
        }
    }
}