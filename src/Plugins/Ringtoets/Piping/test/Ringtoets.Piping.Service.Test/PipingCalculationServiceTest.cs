using System;
using System.Linq;

using DelftTools.TestUtils;

using NUnit.Framework;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Calculation.Test.Piping.Stub;

namespace Ringtoets.Piping.Service.Test
{
    public class PipingCalculationServiceTest
    {
        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            const string name = "<very nice name>";

            var pipingData = CreateCalculationWithValidInput();
            pipingData.Name = name;

            // Call
            Action call = () => PipingCalculationService.Validate(pipingData);

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", name), msgs.First());
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", name), msgs.Last());
            });
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingData_LogStartAndEndOfValidatingInputsAndCalculation()
        {
            // Setup
            const string name = "<very nice name>";

            var pipingData = CreateCalculationWithValidInput();
            pipingData.Name = name;

            // Call
            Action call = () => PipingCalculationService.PerfromValidatedCalculation(pipingData);

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
        public void PerformValidatedCalculation_ValidPipingDataNoOutput_ShouldSetOutput()
        {
            // Setup
            PipingData validPipingData = CreateCalculationWithValidInput();

            // Precondition
            Assert.IsNull(validPipingData.Output);

            // Call
            PipingCalculationService.PerfromValidatedCalculation(validPipingData);

            // Assert
            Assert.IsNotNull(validPipingData.Output);
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingDataWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestPipingOutput();

            PipingData validPipingData = CreateCalculationWithValidInput();
            validPipingData.Output = output;

            // Call
            PipingCalculationService.PerfromValidatedCalculation(validPipingData);

            // Assert
            Assert.AreNotSame(output, validPipingData.Output);
        }

        [Test]
        public void PerformValidatedCalculation_InValidPipingDataWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestPipingOutput();
            var invalidPipingData = CreateCalculationWithInvalidData();
            invalidPipingData.Output = output;

            // Call
            PipingCalculationService.PerfromValidatedCalculation(invalidPipingData);
            
            // Assert
            Assert.AreSame(output, invalidPipingData.Output);
        }

        [Test]
        public void PerformValidatedCalculation_Diameter70AndAquiferPermeabilityZero_CalculationErrorOutputNull()
        {
            // Setup
            PipingData validPipingData = CreateCalculationWithValidInput();
            validPipingData.Diameter70 = 0;
            validPipingData.DarcyPermeability = 0;

            // Call
            PipingCalculationService.PerfromValidatedCalculation(validPipingData);

            // Assert
            Assert.IsNull(validPipingData.Output);
        }

        [Test]
        public void PerformValidatedCalculation_FailureMechanismHasMultipleValidCalculations_SetOutpufOnEach()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Calculations.Clear();
            failureMechanism.Calculations.Add(CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(CreateCalculationWithValidInput());

            // Call
            PipingCalculationService.PerfromValidatedCalculation(failureMechanism);

            // Assert
            foreach (var calculation in failureMechanism.Calculations)
            {
                Assert.IsNotNull(calculation.Output);
            }
        }

        [Test]
        public void PerformValidatedCalculation_FailureMechanismHasCalculationsWithOutputs_ReplaceOutputOnEach()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Calculations.Clear();
            failureMechanism.Calculations.Add(CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(CreateCalculationWithValidInput());

            foreach (var calculation in failureMechanism.Calculations)
            {
                calculation.Output = new TestPipingOutput();
            }
            var originalOutputs = failureMechanism.Calculations.Select(c => c.Output).ToArray();

            // Call
            PipingCalculationService.PerfromValidatedCalculation(failureMechanism);

            // Assert
            var index = 0;
            foreach (var calculation in failureMechanism.Calculations)
            {
                Assert.IsNotNull(calculation.Output);
                Assert.AreNotSame(originalOutputs[index++], calculation.Output);
            }
        }

        [Test]
        public void PerformValidatedCalculation_FailureMechanismHasOneInvalidCalculation_ReplaceOutputOnEach()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Calculations.Clear();
            failureMechanism.Calculations.Add(CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(CreateCalculationWithInvalidData()); // Unable to calculate
            failureMechanism.Calculations.Add(CreateCalculationWithValidInput());

            // Call
            PipingCalculationService.PerfromValidatedCalculation(failureMechanism);

            // Assert
            var index = 0;
            foreach (var calculation in failureMechanism.Calculations)
            {
                if (index++ == 1)
                {
                    Assert.IsNull(calculation.Output,
                        "Calculation input is not valid, therefore calculation cannot produce output.");
                }
                else
                {
                    Assert.IsNotNull(calculation.Output);
                }
            }
        }

        private PipingData CreateCalculationWithInvalidData()
        {
            return new PipingData();
        }

        private PipingData CreateCalculationWithValidInput()
        {
            return new PipingData
            {
                AssessmentLevel = 1.0,
                BeddingAngle = 1.0,
                CriticalHeaveGradient = 1.0,
                DampingFactorExit = 1.0,
                DarcyPermeability = 1.0,
                Diameter70 = 1.0,
                ExitPointXCoordinate = 1.0,
                Gravity = 1.0,
                MeanDiameter70 = 1.0,
                PiezometricHeadExit = 1.0,
                PiezometricHeadPolder = 1.0,
                PhreaticLevelExit = 2.0,
                SandParticlesVolumicWeight = 1.0,
                SeepageLength = 1.0,
                SellmeijerModelFactor = 1.0,
                SellmeijerReductionFactor = 1.0,
                ThicknessAquiferLayer = 1.0,
                ThicknessCoverageLayer = 1.0,
                UpliftModelFactor = 1.0,
                WaterKinematicViscosity = 1.0,
                WaterVolumetricWeight = 1.0,
                WhitesDragCoefficient = 1.0,
                SurfaceLine = new RingtoetsPipingSurfaceLine()
            };
        }
    }
}