using System;
using NUnit.Framework;
using Wti.Calculation.Piping;

namespace Wti.Calculation.Test.Piping
{
    public class PipingCalculationTest
    {
        [Test]
        public void GivenACompleteInput_WhenCalculationPerformed_ThenResultContainsNoNaN()
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

            Assert.That(actual.UpliftZValue, Is.Not.NaN);
            Assert.That(actual.UpliftFactorOfSafety, Is.Not.NaN);
            Assert.That(actual.HeaveZValue, Is.Not.NaN);
            Assert.That(actual.HeaveFactorOfSafety, Is.Not.NaN);
            Assert.That(actual.SellmeijerZValue, Is.Not.NaN);
            Assert.That(actual.SellmeijerFactorOfSafety, Is.Not.NaN);
        }
    }
}