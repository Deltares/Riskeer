﻿using NUnit.Framework;
using Ringtoets.Piping.Calculation.SemiProbabilistic;

namespace Ringtoets.Piping.Calculation.Test.SemiProbabilistic
{
    [TestFixture]
    public class PipingSemiProbabilisticResultTransformerTest
    {
        [Test]
        [TestCase(30000, 1.2, 7.36633E-06)]
        [TestCase(30000, 1.0, 4.13743E-05)]
        [TestCase(20000, 1.2, 9.53353E-06)]
        [TestCase(20000, 1.0, 5.24017E-05)]
        public void FailureProbabilityUplift_DifferentInputs_ReturnsExpectedValue(int norm, double factorOfSafety, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingCalculatorResult(double.NaN, factorOfSafety, double.NaN, double.NaN, double.NaN, double.NaN);
            var transformer = new PipingSemiProbabilisticResultTransformer(calculatorResult, norm, double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            double result = transformer.FailureProbabilityUplift();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-6);
        }

        [Test]
        [TestCase(30000, 0.6, 0.000233011)]
        [TestCase(30000, 0.4, 0.003967252)]
        [TestCase(20000, 0.6, 0.000292194)]
        [TestCase(20000, 0.4, 0.004742775)]
        public void FailureProbabilityHeave_DifferentInputs_ReturnsExpectedValue(int norm, double factorOfSafety, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingCalculatorResult(double.NaN, double.NaN, double.NaN, factorOfSafety, double.NaN, double.NaN);
            var transformer = new PipingSemiProbabilisticResultTransformer(calculatorResult, norm, double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            double result = transformer.FailureProbabilityHeave();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-6);
        }

        [Test]
        [TestCase(30000, 0.9, 1.09882E-05)]
        [TestCase(30000, 0.6, 0.000822098)]
        [TestCase(20000, 0.9, 1.808E-05)]
        [TestCase(20000, 0.6, 0.001203129)]
        public void FailureProbabilitySellmeijer_DifferentInputs_ReturnsExpectedValue(int norm, double factorOfSafety, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingCalculatorResult(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, factorOfSafety);
            var transformer = new PipingSemiProbabilisticResultTransformer(calculatorResult, norm, double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            double result = transformer.FailureProbabilitySellmeijer();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-8);
        }

        [Test]
        [TestCase(30000, 1.2, 0.6, 0.9, 4.332647923)]
        [TestCase(30000, 1.2, 1.4, 0.9, 5.264767065)]
        [TestCase(30000, 1.2, 0.6, 1.1, 4.786155161)]
        [TestCase(20000, 1.2, 0.6, 0.9, 4.275544655)]
        [TestCase(20000, 1.2, 1.4, 0.9, 5.203962658)]
        [TestCase(20000, 1.2, 0.6, 1.1, 4.673091832)]
        public void BetaCrossPiping_DifferentInputs_ReturnsExpectedValue(int norm, double fosUplift, double fosHeave, double fosSellmeijer, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingCalculatorResult(double.NaN, fosUplift, double.NaN, fosHeave, double.NaN, fosSellmeijer);
            var transformer = new PipingSemiProbabilisticResultTransformer(calculatorResult, norm, double.NaN, double.NaN, double.NaN, double.NaN);

            // Call
            double result = transformer.BetaCrossPiping();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-8);
        }

        [Test]
        [TestCase(30000, 1, 350, 6000, 0.24, 4.916313847)]
        [TestCase(20000, 1, 350, 6000, 0.12, 4.972362935)]
        [TestCase(20000, 14, 350, 6000, 0.24, 5.327479413)]
        [TestCase(20000, 1, 112, 6000, 0.24, 5.050875101)]
        [TestCase(20000, 1, 350, 8000, 0.24, 4.890463519)]
        public void BetaCrossAllowed_DifferentInputs_ReturnsExpectedValue(int norm, double a, double b, double assessmentSectionLength, double contribution, double expectedResult)
        {
            // Setup
            var calculatorResult = new PipingCalculatorResult(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN, double.NaN);
            var transformer = new PipingSemiProbabilisticResultTransformer(calculatorResult, norm, a, b, assessmentSectionLength, contribution);

            // Call
            double result = transformer.BetaCrossAllowed();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-8);
        }

        [Test]
        public void FactorOfSafety_SampleInput_ReturnsExpectedValue()
        {
            // Setup
            int norm = 30000;
            double a = 1;
            double b = 350;
            double assessmentSectionLength = 6000;
            double contribution = 0.24;
            double fosUplift = 1.2;
            double fosHeave = 0.6;
            double fosSellmeijer = 0.9;
            double expectedResult = 1.134713444;

            var calculatorResult = new PipingCalculatorResult(double.NaN, fosUplift, double.NaN, fosHeave, double.NaN, fosSellmeijer);
            var transformer = new PipingSemiProbabilisticResultTransformer(calculatorResult, norm, a, b, assessmentSectionLength, contribution);

            // Call
            double result = transformer.FactorOfSafety();

            // Assert
            Assert.AreEqual(expectedResult, result, 1e-8);
        }

        [Test]
        [Combinatorial]
        public void FactorOfSafety_DifferentInputs_ReturnsExpectedValue(
            [Values(20000, 30000)] int norm,
            [Values(1, 14)] double a,
            [Values(112, 350)] double b,
            [Values(6000, 8000)] double assessmentSectionLength,
            [Values(0.12, 0.24)] double contribution,
            [Values(1.2, 1.0)] double fosUplift,
            [Values(1.4, 0.6)] double fosHeave,
            [Values(0.9, 1.1)] double fosSellmeijer)
        {
            // Setup
            var calculatorResult = new PipingCalculatorResult(double.NaN, fosUplift, double.NaN, fosHeave, double.NaN, fosSellmeijer);
            var transformer = new PipingSemiProbabilisticResultTransformer(calculatorResult, norm, a, b, assessmentSectionLength, contribution);

            var betaAllowed = transformer.BetaCrossAllowed();
            var betaPiping = transformer.BetaCrossPiping();

            // Call
            double result = transformer.FactorOfSafety();

            // Assert
            Assert.AreEqual(betaAllowed/betaPiping, result, 1e-8);
        }
    }
}