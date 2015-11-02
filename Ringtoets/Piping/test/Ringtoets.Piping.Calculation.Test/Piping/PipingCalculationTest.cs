using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

using Ringtoets.Piping.Calculation.Piping;

using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Test.Piping
{
    public class PipingCalculationTest
    {
        [Test]
        public void Calculate_CompleteValidInput_ReturnsResultWithNoNaN()
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
        public void Validate_CompleteValidInput_ReturnsNoValidationMessages()
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput().AsRealInput();
            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(0, validationMessages.Count);
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
        public void Validate_NoSurfaceLineSet_ValidationMessageForHavingNoSurfaceLineSelected()
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                SurfaceLine = null,
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Een dwarsdoorsnede moet geselecteerd zijn om een Uplift berekening uit te kunnen voeren.", validationMessages[0]);
        }

        [Test]
        public void Validate_NoSoilProfileSet_ValidationMessageForHavingNoSoilProfileSelected()
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                SoilProfile = null,
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Een ondergrondprofiel moet geselecteerd zijn om een Uplift berekening uit te kunnen voeren.", validationMessages[0]);
        }

        [Test]
        public void Validate_SoilProfileWithoutAquiferSet_ValidationWithDefaultNullReferenceMessage()
        {
            // Setup
            PipingCalculationInput input = new TestPipingInput
            {
                SoilProfile = new PipingSoilProfile(String.Empty, -1.0,new []
                {
                    new PipingSoilLayer(0) 
                })
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            var nullReferenceException = new NullReferenceException();
            Assert.IsTrue(validationMessages.Any(vm => vm.Contains(nullReferenceException.Message)));
        }

        [Test]
        [TestCase(-1e-8)]
        [TestCase(0)]
        public void Validate_SoilProfileBottomAtTopLevel_ValidationMessageForHavingTooHighBottom(double bottom)
        {
            // Setup
            var top = 0;
            PipingCalculationInput input = new TestPipingInput
            {
                SoilProfile = new PipingSoilProfile(String.Empty, bottom, new[]
                {
                    new PipingSoilLayer(top) 
                })
            }.AsRealInput();

            var calculation = new PipingCalculation(input);

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            var message = string.Format("The bottomlevel ({0}) of the profile is not deep enough. It must be below at least 0.001 m below the toplevel of the deepest layer ({1}).", bottom, top);
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(vm => vm.Contains(message)));
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