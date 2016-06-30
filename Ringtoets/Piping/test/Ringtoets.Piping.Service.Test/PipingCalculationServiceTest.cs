// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
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

            var calculation = new PipingCalculation(new GeneralPipingInput());
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(7, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[2]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[3]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen waarde voor het intredepunt opgegeven.", msgs[4]);
                StringAssert.StartsWith("Validatie mislukt: Er is geen waarde voor het uittredepunt opgegeven.", msgs[5]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutHydraulicBoundaryLocation_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";
            
            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.HydraulicBoundaryLocation = null;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithHydraulicBoundaryLocationNotCalculated_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = double.NaN;
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
                StringAssert.StartsWith("Validatie mislukt: Kan het toetspeil niet afleiden op basis van de invoer.", msgs[1]);
                StringAssert.StartsWith("Validatie mislukt: Kan de stijghoogte bij het uittredepunt niet afleiden op basis van de invoer.", msgs[2]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutEntryPointL_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";
            
            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.EntryPointL = (RoundedDouble) double.NaN;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen waarde voor het intredepunt opgegeven.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutExitPointL_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";

            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.ExitPointL = (RoundedDouble)double.NaN;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen waarde voor het uittredepunt opgegeven.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutSurfaceLine_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";
            
            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.SurfaceLine = null;
            calculation.InputParameters.ExitPointL = (RoundedDouble) 0.9;
            calculation.InputParameters.EntryPointL = (RoundedDouble) 0.1;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen profielschematisatie geselecteerd.", msgs[1]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
            Assert.IsFalse(isValid);
        }

        [Test]
        public void Validate_CalculationWithoutStochasticSoilProfile_LogsErrorAndReturnsFalse()
        {
            // Setup
            const string name = "<very nice name>";
            
            PipingCalculation calculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation.InputParameters.StochasticSoilProfile = null;
            calculation.Name = name;

            // Call
            bool isValid = false;
            Action call = () => isValid = PipingCalculationService.Validate(calculation);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(3, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith("Validatie mislukt: Er is geen ondergrondschematisatie geselecteerd.", msgs[1]);
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
            var testFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
            var heaveCalculator = testFactory.LastCreatedHeaveCalculator;
            var upliftCalculator = testFactory.LastCreatedUpliftCalculator;
            var sellmeijerCalculator = testFactory.LastCreatedSellmeijerCalculator;

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue(), heaveCalculator.DTotal,
                            input.ThicknessCoverageLayer.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), heaveCalculator.HExit,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(input.CriticalHeaveGradient, heaveCalculator.Ich);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), heaveCalculator.PhiPolder,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(input.PiezometricHeadExit.Value, heaveCalculator.PhiExit);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), heaveCalculator.RExit,
                            input.DampingFactorExit.GetAccuracy());

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), upliftCalculator.HExit,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(input.AssessmentLevel.Value, upliftCalculator.HRiver);
            Assert.AreEqual(input.UpliftModelFactor, upliftCalculator.ModelFactorUplift);
            Assert.AreEqual(input.PiezometricHeadExit.Value, upliftCalculator.PhiExit);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), upliftCalculator.PhiPolder,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), upliftCalculator.RExit,
                            input.DampingFactorExit.GetAccuracy());
            Assert.AreEqual(input.WaterVolumetricWeight, upliftCalculator.VolumetricWeightOfWater);
            RoundedDouble effectiveStress = PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue()*
                                            (PipingSemiProbabilisticDesignValueFactory.GetSaturatedVolumicWeightOfCoverageLayer(input).GetDesignValue() - input.WaterVolumetricWeight);
            Assert.AreEqual(effectiveStress, upliftCalculator.EffectiveStress,
                            effectiveStress.GetAccuracy());

            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetSeepageLength(input).GetDesignValue(), sellmeijerCalculator.SeepageLength,
                            input.SeepageLength.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), sellmeijerCalculator.HExit,
                            input.PhreaticLevelExit.GetAccuracy());
            Assert.AreEqual(input.AssessmentLevel.Value, sellmeijerCalculator.HRiver);
            Assert.AreEqual(input.WaterKinematicViscosity, sellmeijerCalculator.KinematicViscosityWater);
            Assert.AreEqual(input.SellmeijerModelFactor, sellmeijerCalculator.ModelFactorPiping);
            Assert.AreEqual(input.SellmeijerReductionFactor, sellmeijerCalculator.Rc);
            Assert.AreEqual(input.WaterVolumetricWeight, sellmeijerCalculator.VolumetricWeightOfWater);
            Assert.AreEqual(input.WhitesDragCoefficient, sellmeijerCalculator.WhitesDragCoefficient);
            Assert.AreEqual(input.BeddingAngle, sellmeijerCalculator.BeddingAngle);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessCoverageLayer(input).GetDesignValue(), sellmeijerCalculator.DTotal,
                            input.ThicknessCoverageLayer.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDiameter70(input).GetDesignValue(), sellmeijerCalculator.D70,
                            input.Diameter70.GetAccuracy());
            Assert.AreEqual(input.MeanDiameter70, sellmeijerCalculator.D70Mean);
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetThicknessAquiferLayer(input).GetDesignValue(), sellmeijerCalculator.DAquifer,
                            input.ThicknessAquiferLayer.GetAccuracy());
            Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDarcyPermeability(input).GetDesignValue(), sellmeijerCalculator.DarcyPermeability,
                            input.DarcyPermeability.GetAccuracy());
            Assert.AreEqual(input.SandParticlesVolumicWeight, sellmeijerCalculator.GammaSubParticles);
            Assert.AreEqual(input.Gravity, sellmeijerCalculator.Gravity);
        }
    }
}