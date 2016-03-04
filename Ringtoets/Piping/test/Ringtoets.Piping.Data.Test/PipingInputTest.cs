using System;

using Core.Common.Base;
using Core.Common.TestUtil;
using NUnit.Framework;

using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.Properties;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingInputTest
    {
        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var generalInputParameters = new GeneralPipingInput();

            // Call
            var inputParameters = new PipingInput(generalInputParameters);

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
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.Diameter70);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.Diameter70.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.Diameter70.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.DarcyPermeability);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.DarcyPermeability.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.DarcyPermeability.StandardDeviation);

            Assert.AreEqual(0, inputParameters.PiezometricHeadExit);
            Assert.AreEqual(0, inputParameters.PiezometricHeadPolder);
            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.SoilProfile);
            Assert.IsNull(inputParameters.HydraulicBoundaryLocation);

            Assert.AreEqual(generalInputParameters.UpliftModelFactor, inputParameters.UpliftModelFactor);
            Assert.AreEqual(generalInputParameters.SellmeijerModelFactor, inputParameters.SellmeijerModelFactor);
            Assert.AreEqual(generalInputParameters.CriticalHeaveGradient, inputParameters.CriticalHeaveGradient);
            Assert.AreEqual(generalInputParameters.SellmeijerReductionFactor, inputParameters.SellmeijerReductionFactor);
            Assert.AreEqual(generalInputParameters.Gravity, inputParameters.Gravity);
            Assert.AreEqual(generalInputParameters.WaterKinematicViscosity, inputParameters.WaterKinematicViscosity);
            Assert.AreEqual(generalInputParameters.WaterVolumetricWeight, inputParameters.WaterVolumetricWeight);
            Assert.AreEqual(generalInputParameters.SandParticlesVolumicWeight, inputParameters.SandParticlesVolumicWeight);
            Assert.AreEqual(generalInputParameters.WhitesDragCoefficient, inputParameters.WhitesDragCoefficient);
            Assert.AreEqual(generalInputParameters.BeddingAngle, inputParameters.BeddingAngle);
            Assert.AreEqual(generalInputParameters.MeanDiameter70, inputParameters.MeanDiameter70);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.ThicknessCoverageLayer);
            Assert.IsNaN(inputParameters.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(0.5, inputParameters.ThicknessCoverageLayer.StandardDeviation);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.ThicknessAquiferLayer);
            Assert.IsNaN(inputParameters.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(0.5, inputParameters.ThicknessAquiferLayer.StandardDeviation);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.SeepageLength);
            Assert.IsNaN(inputParameters.SeepageLength.Mean);
            Assert.IsNaN(inputParameters.SeepageLength.StandardDeviation);

            Assert.IsNaN(inputParameters.ExitPointL);
            Assert.IsNaN(inputParameters.AssessmentLevel);
        }

        [Test]
        public void Constructor_GeneralPipingInputIsNull_ArgumentNullException()
        {
            // Setup

            // Call
            TestDelegate call = () => new PipingInput(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        public void AssessmentLevel_ValueIsNaN_ThrowsArgumentException()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            TestDelegate test = () => pipingInput.AssessmentLevel = double.NaN;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, Resources.PipingInput_AssessmentLevel_Cannot_set_to_NaN);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-21)]
        public void ExitPointL_ValueLessOrEqualToZero_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            TestDelegate test = () => pipingInput.ExitPointL = value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, Resources.PipingInput_ExitPointL_Value_must_be_greater_than_zero);
        }
    }
}