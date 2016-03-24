using System;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Deltares.WTIPiping;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.Service.Test
{
    public class PipingCalculationServiceTest
    {
        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation pipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            pipingCalculation.Name = name;

            // Call
            Action call = () => PipingCalculationService.Validate(pipingCalculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void Validate_InValidPipingCalculationWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestPipingOutput();
            PipingCalculation invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidPipingCalculation.Output = output;

            // Call
            var isValid = PipingCalculationService.Validate(invalidPipingCalculation);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidPipingCalculation.Output);
        }

        [Test]
        public void Validate_InValidCalculationInput_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            var calculation = new PipingCalculation(new GeneralPipingInput(), new SemiProbabilisticPipingInput());
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(4, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Kan de dikte van de deklaag niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingCalculation_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Name = name;

            // Call
            Action call = () =>
            {
                Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));
                PipingCalculationService.Calculate(validPipingCalculation);
            };

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs[1]);

                StringAssert.StartsWith(String.Format("Berekening van '{0}' gestart om: ", name), msgs[2]);
                StringAssert.StartsWith(String.Format("Berekening van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingCalculationNoOutput_ShouldSetOutput()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();

            // Precondition
            Assert.IsNull(validPipingCalculation.Output);

            // Call
            Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));
            PipingCalculationService.Calculate(validPipingCalculation);

            // Assert
            Assert.IsNotNull(validPipingCalculation.Output);
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingCalculationWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestPipingOutput();

            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validPipingCalculation.Output = output;

            // Call
            Assert.IsTrue(PipingCalculationService.Validate(validPipingCalculation));
            PipingCalculationService.Calculate(validPipingCalculation);

            // Assert
            Assert.AreNotSame(output, validPipingCalculation.Output);
        }

        [Test]
        public void Validate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingCalculationService.Validate(validPipingCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        [Test]
        public void Calculate_CompleteInput_InputSetOnSubCalculators()
        {
            // Setup
            PipingCalculation validPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            PipingInput input = validPipingCalculation.InputParameters;

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                PipingCalculationService.Calculate(validPipingCalculation);

                // Assert
                AssertSubCalculatorInputs(input);
            }
        }

        private void AssertSubCalculatorInputs(PipingInput input)
        {
            var testFactory = (TestPipingSubCalculatorFactory)PipingSubCalculatorFactory.Instance;
            var heaveCalculator = testFactory.LastCreatedHeaveCalculator;
            var upliftCalculator = testFactory.LastCreatedUpliftCalculator;
            var sellmeijerCalculator = testFactory.LastCreatedSellmeijerCalculator;

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue(), heaveCalculator.DTotal,
                            GetAccuracy(input.ThicknessCoverageLayer.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), heaveCalculator.HExit,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(input.CriticalHeaveGradient, heaveCalculator.Ich);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), heaveCalculator.PhiPolder,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(input.PiezometricHeadExit.Value, heaveCalculator.PhiExit);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), heaveCalculator.RExit,
                            GetAccuracy(input.DampingFactorExit.Mean));

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), upliftCalculator.HExit,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(input.AssessmentLevel.Value, upliftCalculator.HRiver);
            Assert.AreEqual(input.UpliftModelFactor, upliftCalculator.ModelFactorUplift);
            Assert.AreEqual(input.PiezometricHeadExit.Value, upliftCalculator.PhiExit);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), upliftCalculator.PhiPolder,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), upliftCalculator.RExit,
                            GetAccuracy(input.DampingFactorExit.Mean));
            Assert.AreEqual(input.WaterVolumetricWeight, upliftCalculator.VolumetricWeightOfWater);
            RoundedDouble effectiveStress = PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue()*
                                            (PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue() - input.WaterVolumetricWeight);
            Assert.AreEqual(effectiveStress, upliftCalculator.EffectiveStress,
                            GetAccuracy(effectiveStress));

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(input).GetDesignValue(), sellmeijerCalculator.SeepageLength,
                            GetAccuracy(input.SeepageLength.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), sellmeijerCalculator.HExit,
                            GetAccuracy(input.PhreaticLevelExit.Mean));
            Assert.AreEqual(input.AssessmentLevel.Value, sellmeijerCalculator.HRiver);
            Assert.AreEqual(input.WaterKinematicViscosity, sellmeijerCalculator.KinematicViscosityWater);
            Assert.AreEqual(input.SellmeijerModelFactor, sellmeijerCalculator.ModelFactorPiping);
            Assert.AreEqual(input.SellmeijerReductionFactor, sellmeijerCalculator.Rc);
            Assert.AreEqual(input.WaterVolumetricWeight, sellmeijerCalculator.VolumetricWeightOfWater);
            Assert.AreEqual(input.WhitesDragCoefficient, sellmeijerCalculator.WhitesDragCoefficient);
            Assert.AreEqual(input.BeddingAngle, sellmeijerCalculator.BeddingAngle);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue(), sellmeijerCalculator.DTotal,
                            GetAccuracy(input.ThicknessCoverageLayer.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDiameter70(input).GetDesignValue(), sellmeijerCalculator.D70,
                            GetAccuracy(input.Diameter70.Mean));
            Assert.AreEqual(input.MeanDiameter70, sellmeijerCalculator.D70Mean);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(input).GetDesignValue(), sellmeijerCalculator.DAquifer,
                            GetAccuracy(input.ThicknessAquiferLayer.Mean));
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(input).GetDesignValue(), sellmeijerCalculator.DarcyPermeability,
                            GetAccuracy(input.DarcyPermeability.Mean));
            Assert.AreEqual(input.SandParticlesVolumicWeight, sellmeijerCalculator.GammaSubParticles);
            Assert.AreEqual(input.Gravity, sellmeijerCalculator.Gravity);
        }

        private static double GetAccuracy(RoundedDouble roundedDouble)
        {
            return Math.Pow(10.0, -roundedDouble.NumberOfDecimalPlaces);
        }
    }
}