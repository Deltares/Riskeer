using System;
using System.Linq;
using Core.Common.TestUtils;
using Ringtoets.Piping.Calculation.TestUtil;

using NUnit.Framework;

using Ringtoets.Piping.Data;

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
        public void Validate_InValidPipingDataWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestPipingOutput();
            var invalidPipingData = PipingDataFactory.CreateCalculationWithInvalidData();
            invalidPipingData.Output = output;

            // Call
            var isValid = PipingCalculationService.Validate(invalidPipingData);

            // Assert
            Assert.IsFalse(isValid);
            Assert.AreSame(output, invalidPipingData.Output);
        }

        [Test]
        public void Calculate_InValidPipingDataWithOutput_LogsError()
        {
            // Setup
            var invalidPipingData = PipingDataFactory.CreateCalculationWithValidInput();
            invalidPipingData.InputParameters.BeddingAngle = -1;

            // Call
            Action call = () => PipingCalculationService.Calculate(invalidPipingData);

            // Assert

            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", invalidPipingData.Name), msgs[0]);
                StringAssert.StartsWith("Piping berekening niet gelukt: ", msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", invalidPipingData.Name), msgs[2]);
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
            Action call = () =>
            {
                Assert.IsTrue(PipingCalculationService.Validate(pipingData));
                PipingCalculationService.Calculate(pipingData);
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
        public void PerformValidatedCalculation_ValidPipingDataNoOutput_ShouldSetOutput()
        {
            // Setup
            PipingCalculationData validPipingData = PipingDataFactory.CreateCalculationWithValidInput();

            // Precondition
            Assert.IsNull(validPipingData.Output);

            // Call
            Assert.IsTrue(PipingCalculationService.Validate(validPipingData));
            PipingCalculationService.Calculate(validPipingData);

            // Assert
            Assert.IsNotNull(validPipingData.Output);
        }

        [Test]
        public void PerformValidatedCalculation_ValidPipingDataWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestPipingOutput();

            PipingCalculationData validPipingData = PipingDataFactory.CreateCalculationWithValidInput();
            validPipingData.Output = output;

            // Call
            Assert.IsTrue(PipingCalculationService.Validate(validPipingData));
            PipingCalculationService.Calculate(validPipingData);

            // Assert
            Assert.AreNotSame(output, validPipingData.Output);
        }
    }
}