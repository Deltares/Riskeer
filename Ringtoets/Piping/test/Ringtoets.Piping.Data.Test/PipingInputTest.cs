using System;

using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingInputTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var inputParameters = new PipingInput();

            // Assert
            Assert.IsInstanceOf<Observable>(inputParameters);

            Assert.IsInstanceOf<NormalDistribution>(inputParameters.PhreaticLevelExit);
            Assert.AreEqual(0, inputParameters.PhreaticLevelExit.Mean);
            Assert.AreEqual(1, inputParameters.PhreaticLevelExit.StandardDeviation);

            double defaultLogNormalMean = Math.Exp(-0.5);
            double defaultLogNormalStandardDev = Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1));
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.DampingFactorExit);
            Assert.AreEqual(1, inputParameters.DampingFactorExit.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.DampingFactorExit.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.ThicknessCoverageLayer);
            Assert.IsNaN(inputParameters.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.ThicknessCoverageLayer.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.SeepageLength);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.SeepageLength.Mean);
            Assert.AreEqual(defaultLogNormalMean * 0.1, inputParameters.SeepageLength.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.Diameter70);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.Diameter70.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.Diameter70.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.DarcyPermeability);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.DarcyPermeability.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.DarcyPermeability.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.ThicknessAquiferLayer);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.ThicknessAquiferLayer.StandardDeviation);

            Assert.AreEqual(0, inputParameters.PiezometricHeadExit);
            Assert.AreEqual(0, inputParameters.PiezometricHeadPolder);
            Assert.AreEqual(0, inputParameters.AssessmentLevel);
            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.SoilProfile);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);

            Assert.AreEqual(1.0, inputParameters.UpliftModelFactor);
            Assert.AreEqual(1, inputParameters.SellmeijerModelFactor);
            Assert.AreEqual(0.3, inputParameters.CriticalHeaveGradient);
            Assert.AreEqual(0.3, inputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(9.81, inputParameters.Gravity);
            Assert.AreEqual(1.33e-6, inputParameters.WaterKinematicViscosity);
            Assert.AreEqual(10.0, inputParameters.WaterVolumetricWeight);
            Assert.AreEqual(16.5, inputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(0.25, inputParameters.WhitesDragCoefficient);
            Assert.AreEqual(37, inputParameters.BeddingAngle);
            Assert.AreEqual(2.08e-4, inputParameters.MeanDiameter70);
        }

        [Test]
        public void AssessmentLevel_ValueIsNaN_ThrowsArgumentException()
        {
            // Setup
            var pipingInput = new PipingInput();

            // Call
            TestDelegate test = () => pipingInput.AssessmentLevel = double.NaN;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, Properties.Resources.PipingInput_AssessmentLevel_Cannot_set_to_NaN);
        }
    }
}