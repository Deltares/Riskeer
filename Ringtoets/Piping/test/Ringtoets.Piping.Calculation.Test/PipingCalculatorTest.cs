using System;
using System.Collections.Generic;
using System.Linq;

using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Calculation.SubCalculator;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

namespace Ringtoets.Piping.Calculation.Test
{
    [TestFixture]
    public class PipingCalculatorTest
    {
        [Test]
        public void Constructor_WithoutInput_ArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingCalculator(null, null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "PipingCalculatorInput required for creating a PipingCalculator.");
        }

        [Test]
        public void Constructor_FactoryNull_ArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingCalculator(new TestPipingInput().AsRealInput(), null);
            
            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "IPipingSubCalculatorFactory required for creating a PipingCalculator.");
        }

        [Test]
        public void Calculate_CompleteValidInput_ReturnsResultWithNoNaN()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();

            // Call
            PipingCalculatorResult actual = new PipingCalculator(input, new PipingSubCalculatorFactory()).Calculate();

            // Assert
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
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();
            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

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
            PipingCalculatorInput input = new TestPipingInput
            {
                SeepageLength = seepageLength
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

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
            PipingCalculatorInput input = new TestPipingInput
            {
                ThicknessAquiferLayer = aquiferThickness
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

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
            PipingCalculatorInput input = new TestPipingInput
            {
                BeddingAngle = beddingAngle
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

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
            PipingCalculatorInput input = new TestPipingInput
            {
                PiezometricHeadExit = level,
                PhreaticLevelExit = level
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(2, validationMessages.Count);
            Assert.AreEqual(1, validationMessages.Count(message => message.Contains("PhiExit -  HExit")));
            Assert.AreEqual(1, validationMessages.Count(message => message.Contains("phiExit - hExit")));
        }

        [Test]
        public void Validate_DampingFactorExitZero_TwoValidationMessageForRExitTwoValidationMessagesForPhiExitAndHExitDifference()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                DampingFactorExit = 0
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

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
            PipingCalculatorInput input = new TestPipingInput
            {
                ThicknessCoverageLayer = 0
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

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
            PipingCalculatorInput input = new TestPipingInput
            {
                ThicknessAquiferLayer = 0
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

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
            PipingCalculatorInput input = new TestPipingInput
            {
                WaterVolumetricWeight = 0
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(message => message.Contains("Volumetric weight water")));
        }

        [Test] // Validate_DifferenceAssessmentLevelAndPhreaticLevelExitEqualToSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidationMessageForHRiverHExitRcDTotal
        [TestCase(2, 1, 2, 0.5, TestName = "AssessmntLvlPhreaticLevelExitEqualsSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidateMsgHRiverHExitRcDTotal(2,1,2,0.5)")]
        [TestCase(2, 1.5, 0.5, 1, TestName = "AssessmntLvlPhreaticLevelExitEqualsSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidateMsgHRiverHExitRcDTotal(2,1.5,0.5,1)")]
        [TestCase(8, 4, 2, 2, TestName = "AssessmntLvlPhreaticLevelExitEqualsSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidateMsgHRiverHExitRcDTotal(8,4,2,2)")]
        public void Validate_DifferenceAssessmentLevelAndPhreaticLevelExitEqualToSellmeijerReductionFactorTimesThicknessCoverageLayer_ValidationMessageForHRiverHExitRcDTotal(
            double assessmentLevel, double phreaticLevelExit, double sellmeijerReductionFactor, double thicknessCoverageLayer)
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                AssessmentLevel = (RoundedDouble)assessmentLevel,
                PhreaticLevelExit = phreaticLevelExit,
                SellmeijerReductionFactor = sellmeijerReductionFactor,
                ThicknessCoverageLayer = thicknessCoverageLayer
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

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
            PipingCalculatorInput input = new TestPipingInput
            {
                SurfaceLine = null,
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Een profielmeting moet geselecteerd zijn om een Uplift berekening uit te kunnen voeren.", validationMessages[0]);
        }

        [Test]
        public void Validate_NoSoilProfileSet_ValidationMessageForHavingNoSoilProfileSelected()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                SoilProfile = null,
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            Assert.AreEqual(1, validationMessages.Count);
            Assert.AreEqual("Een ondergrondschematisering moet geselecteerd zijn om een Uplift berekening uit te kunnen voeren.", validationMessages[0]);
        }
        
        [Test]
        [TestCase(-1e-8)]
        [TestCase(0)]
        public void Validate_SoilProfileBottomAtTopLevel_ValidationMessageForHavingTooHighBottom(double bottom)
        {
            // Setup
            var top = 0;
            PipingCalculatorInput input = new TestPipingInput
            {
                SoilProfile = new PipingSoilProfile(String.Empty, bottom, new[]
                {
                    new PipingSoilLayer(top) 
                })
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            List<string> validationMessages = calculation.Validate();

            // Assert
            var message = string.Format("The bottomlevel ({0}) of the profile is not deep enough. It must be below at least {1} m below the toplevel of the deepest layer ({2}).", bottom, 0.001, top);
            Assert.AreEqual(1, validationMessages.Count);
            Assert.IsTrue(validationMessages.Any(vm => vm.Contains(message)));
        }

        [Test]
        public void CalculateThicknessCoverageLayer_SoilProfileWithoutAquiferSet_ThrowsPipingCalculatorException()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                SoilProfile = new PipingSoilProfile(String.Empty, -1.0, new[]
                {
                    new PipingSoilLayer(0) 
                })
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            TestDelegate call = () => calculation.CalculateThicknessCoverageLayer();

            // Assert
            var exception = Assert.Throws<PipingCalculatorException>(call);
            Assert.IsInstanceOf<NullReferenceException>(exception.InnerException);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithValidInput_ReturnsSomeThickness()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            var result = calculation.CalculateThicknessCoverageLayer();

            // Assert
            Assert.IsFalse(double.IsNaN(result));
        }

        [Test]
        public void CalculateThicknessCoverageLayer_WithExitPointLBeyondSurfaceLineInput_ReturnsNaN()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput
            {
                ExitPointXCoordinate = (RoundedDouble)2.1
            }.AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            var result = calculation.CalculateThicknessCoverageLayer();

            // Assert
            Assert.IsNaN(result);
        }

        [Test]
        public void CalculatePiezometricHeadAtExit_WithValidInput_ReturnsSomeValue()
        {
            // Setup
            PipingCalculatorInput input = new TestPipingInput().AsRealInput();

            var calculation = new PipingCalculator(input, new PipingSubCalculatorFactory());

            // Call
            var result = calculation.CalculatePiezometricHeadAtExit();

            // Assert
            Assert.IsFalse(double.IsNaN(result));
        }
    }
}