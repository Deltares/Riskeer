using System;
using System.Linq;

using DelftTools.TestUtils;

using NUnit.Framework;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Calculation.Test.Piping.Stub;
using Ringtoets.Piping.Service.Test.TestHelpers;

namespace Ringtoets.Piping.Service.Test
{
    public class PipingCalculationServiceTest
    {
        [Test]
        public void Validate_Always_LogStartAndEndOfValidatingInputs()
        {
            // Setup
            const string name = "<very nice name>";

            var pipingData = PipingDataFactory.CreateCalculationWithValidInput();
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

            var pipingData = PipingDataFactory.CreateCalculationWithValidInput();
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
            PipingData validPipingData = PipingDataFactory.CreateCalculationWithValidInput();

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

            PipingData validPipingData = PipingDataFactory.CreateCalculationWithValidInput();
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
            var invalidPipingData = PipingDataFactory.CreateCalculationWithInvalidData();
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
            PipingData validPipingData = PipingDataFactory.CreateCalculationWithValidInput();
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
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithValidInput());

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
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithValidInput());

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
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithValidInput());
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithInvalidData()); // Unable to calculate
            failureMechanism.Calculations.Add(PipingDataFactory.CreateCalculationWithValidInput());

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
    }
}