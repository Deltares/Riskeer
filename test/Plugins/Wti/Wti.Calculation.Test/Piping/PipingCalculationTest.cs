using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wti.Calculation.Piping;
using Wti.Calculation.Test.Piping.Stub;

namespace Wti.Calculation.Test.Piping
{
    public class PipingCalculationTest
    {
        [Test]
        public void GivenACompleteInput_WhenCalculationPerformed_ThenResultContainsNoNaN()
        {
            PipingCalculationInput input = new TestPipingInput().AsRealInput();

            PipingCalculationResult actual = new PipingCalculation(input).Calculate();

            Assert.IsNotNull(actual);
            Assert.IsFalse(double.IsNaN(actual.UpliftZValue));
            Assert.IsFalse(double.IsNaN(actual.UpliftFactorOfSafety));
            Assert.IsFalse(double.IsNaN(actual.HeaveZValue));
            Assert.IsFalse(double.IsNaN(actual.HeaveFactorOfSafety));
            Assert.IsFalse(double.IsNaN(actual.SellmeijerZValue));
            Assert.IsFalse(double.IsNaN(actual.SellmeijerFactorOfSafety));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_ZeroOrNegativeSeepageLength_ValidationMessageForPipingLength(double seepageLength)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                SeepageLength = seepageLength
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("Piping length")));
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_ZeroOrNegativeAquiferThickness_ValidationMessageForDAquifer(double aquiferThickness)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                ThicknessAquiferLayer = aquiferThickness
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("DAquifer")));
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_NegativeBeddingAngle_ValidationMessageForBeddingAngle(double beddingAngle)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                BeddingAngle = beddingAngle
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("Bedding angle")));
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-100)]
        public void Validate_PiezometricHeadExitSameAsPhreaticLevelExit_ValidationMessageForPhiExitAndHExit(double level)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                PiezometricHeadExit = level,
                PhreaticLevelExit = level
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(2, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("PhiExit -  HExit")));
            Assert.IsTrue(validationMessages.Any(message => message.Contains("phiExit - hExit")));
        }

        [Test]
        public void Validate_PhreaticLevelExitZero_TwoValidationMessageForRExit()
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                DampingFactorExit = 0
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(2, validationMessages.Count);
            Assert.AreEqual(2, validationMessages.Count(message => message.Contains("Rexit")));
        }

        [Test]
        public void Validate_ThicknessCoverageLayerZero_ValidationMessageForDTotal()
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                ThicknessCoverageLayer = 0
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("Dtotal")));
        }

        [Test]
        public void Validate_ThicknessAquiferLayerZero_ValidationMessageForDAquifer()
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                ThicknessAquiferLayer = 0
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("DAquifer")));
        }

        [Test]
        public void Validate_VolumetricWeightWaterZero_ValidationMessageForVolumetricWeightWater()
        {                                                                                                                                                                                
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                WaterVolumetricWeight = 0
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("Volumetric weight water")));
        }

        [Test]
        [TestCase(2, 1, 2, 0.5)]
        [TestCase(1, 1, 0, 2)]
        [TestCase(8, 4, 2, 2)]
        public void Validate_DifferenceAssessmentLevelAndPhreaticLevelExitEqualToSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidationMessageForHRiverHExitRcDTotal(
            double assessmentLevel, double phreaticLevelExit, double sellmeijerReductionFactor, double thicknessCoverageLayer)
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                AssessmentLevel = assessmentLevel,
                PhreaticLevelExit = phreaticLevelExit,
                SellmeijerReductionFactor = sellmeijerReductionFactor,
                ThicknessCoverageLayer = thicknessCoverageLayer
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("HRiver - HExit - (Rc*DTotal)")));
        }

        [Test]
        public void Validate_AssessmentLevelPhreaticLevelExitSellmeijerReductionFactorThicknessCoverageLayerZero_ValidationMessageForHRiverHExitRcDTotalAndDTotal()
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                AssessmentLevel = 0,
                PhreaticLevelExit = 0,
                SellmeijerReductionFactor = 0,
                ThicknessCoverageLayer = 0
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(2, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("HRiver - HExit - (Rc*DTotal)")));
            Assert.IsTrue(validationMessages.Any(message => message.Contains("Dtotal")));
        }

    }
}