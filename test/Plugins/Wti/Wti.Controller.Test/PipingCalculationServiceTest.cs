using NUnit.Framework;
using Wti.Calculation.Piping;
using Wti.Data;

namespace Wti.Service.Test
{
    public class PipingCalculationServiceTest
    {
        private readonly PipingData validPipingData = new PipingData
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

        [Test]
        public void Calculate_ValidPipingData_ShouldSetOutput()
        {
            // Call
            PipingCalculationService.Calculate(validPipingData);

            // Assert
            Assert.NotNull(validPipingData.Output);
        }

        [Test]
        public void Calculate_InValidPipingData_ThrowsException()
        {
            // Setup
            var invalidPipingData = new PipingData();

            // Call
            TestDelegate testDelegate = () => PipingCalculationService.Calculate(invalidPipingData);
            
            // Assert
            Assert.Throws<PipingCalculationException>(testDelegate);
        }
    }
}