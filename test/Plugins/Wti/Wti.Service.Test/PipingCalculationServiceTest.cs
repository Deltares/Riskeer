using NUnit.Framework;
using Rhino.Mocks;
using Wti.Calculation.Test.Piping.Stub;
using Wti.Data;

namespace Wti.Service.Test
{
    public class PipingCalculationServiceTest
    {
        private PipingData validPipingData;

        [SetUp]
        public void SetUp()
        {
            validPipingData = new PipingData
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
                WhitesDragCoefficient = 1.0
            };
        }

        [Test]
        public void PerfromValidatedCalculation_ValidPipingDataNoOutput_ShouldSetOutput()
        {
            // Precondition
            Assert.IsNull(validPipingData.Output);

            // Call
            var result = PipingCalculationService.PerfromValidatedCalculation(validPipingData);

            // Assert
            Assert.AreEqual(PipingCalculationResult.Successful, result);
            Assert.IsNotNull(validPipingData.Output);
        }

        [Test]
        public void PerfromValidatedCalculation_ValidPipingDataWithOutput_ShouldChangeOutput()
        {
            // Setup
            var output = new TestPipingOutput();
            validPipingData.Output = output;

            // Call
            var result = PipingCalculationService.PerfromValidatedCalculation(validPipingData);

            // Assert
            Assert.AreEqual(PipingCalculationResult.Successful, result);
            Assert.AreNotSame(output, validPipingData.Output);
        }

        [Test]
        public void PerfromValidatedCalculation_InValidPipingDataWithOutput_ReturnsFalseNoOutputChange()
        {
            // Setup
            var output = new TestPipingOutput();
            var invalidPipingData = new PipingData
            {
                Output = output
            };

            // Call
            var result = PipingCalculationService.PerfromValidatedCalculation(invalidPipingData);
            
            // Assert
            Assert.AreEqual(PipingCalculationResult.ValidationErrors, result);
            Assert.AreSame(output, invalidPipingData.Output);
        }

        [Test]
        public void PerformValidatedCalculation_Diameter70AndAquiferPermeabilityZero_CalculationErrorOutputNull()
        {
            // Setup
            validPipingData.Diameter70 = 0;
            validPipingData.DarcyPermeability = 0;

            // Call
            var result = PipingCalculationService.PerfromValidatedCalculation(validPipingData);

            // Assert
            Assert.AreEqual(PipingCalculationResult.CalculationErrors, result);
            Assert.IsNull(validPipingData.Output);
        }
    }
}