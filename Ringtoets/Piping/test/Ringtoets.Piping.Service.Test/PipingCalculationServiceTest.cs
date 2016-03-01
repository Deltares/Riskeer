using System;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using Ringtoets.Piping.Calculation.TestUtil;

using NUnit.Framework;
using Ringtoets.Piping.Calculation;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;

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
        public void Calculate_InValidPipingCalculationWithOutput_LogsError()
        {
            // Setup
            PipingCalculation invalidPipingCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            invalidPipingCalculation.InputParameters.BeddingAngle = -1;

            // Call
            Action call = () => PipingCalculationService.Calculate(invalidPipingCalculation);

            // Assert

            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", invalidPipingCalculation.Name), msgs[0]);
                StringAssert.StartsWith("Piping berekening niet gelukt: ", msgs[1]);
                StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", invalidPipingCalculation.Name), msgs[2]);
            });
        }

        [Test]
        public void CalculateThicknessCoverageLayer_ValidInput_ReturnsThickness()
        {
            // Setup
            PipingInput input = new PipingInput();
            input.ExitPointL = 10;
            input.SurfaceLine = new RingtoetsPipingSurfaceLine();
            input.SurfaceLine.SetGeometry(new []
            {
                new Point3D(0, 0, 10),
                new Point3D(20, 0, 10)
            });
            input.SoilProfile = new PipingSoilProfile(string.Empty, 0, new []
            {
                new PipingSoilLayer(5)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(20)
                {
                    IsAquifer = false
                }
            });

            // Call
            var thickness = PipingCalculationService.CalculateThicknessCoverageLayer(input);

            // Assert
            Assert.AreEqual(5, thickness);
        }

        [Test]
        public void CalculateThicknessCoverageLayer_SurfaceLineOutsideProfile_ThrowsPipingCalculatorException()
        {
            // Setup
            PipingInput input = new PipingInput();
            input.ExitPointL = 10;
            input.SurfaceLine = new RingtoetsPipingSurfaceLine();
            input.SurfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 10),
                new Point3D(10, 0, 20+1e-3),
                new Point3D(20, 0, 10)
            });
            input.SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
            {
                new PipingSoilLayer(5)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(20)
                {
                    IsAquifer = false
                }
            });

            // Call
            TestDelegate test = () => PipingCalculationService.CalculateThicknessCoverageLayer(input);

            // Assert
            Assert.Throws<PipingCalculatorException>(test);
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
    }
}