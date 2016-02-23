using System;

using Core.Common.Base;
using Core.Common.Base.Geometry;
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
            Assert.AreEqual(defaultLogNormalMean, inputParameters.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.ThicknessCoverageLayer.StandardDeviation);
            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.SeepageLength);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.SeepageLength.Mean);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.SeepageLength.StandardDeviation);
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
        public void SurfaceLine_WithDikeToeDikeSideAndDikeToeRiverSide_SetsExitPointLAndSeePageLength()
        {
            // Setup
            var inputParameters = new PipingInput();
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            var firstPointX = 1.0;
            var secondPointX = 4.0;
            var point1 = new Point3D { X = firstPointX, Y = 0.0, Z = 2.0 };
            var point2 = new Point3D { X = secondPointX, Y = 0.0, Z = 1.8 };
            surfaceLine.SetGeometry(new []
            {
                point1,
                point2,
            });
            surfaceLine.SetDikeToeAtRiverAt(point1);
            surfaceLine.SetDikeToeAtPolderAt(point2);
            
            // Call
            inputParameters.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(secondPointX - firstPointX, inputParameters.SeepageLength.Mean);
            Assert.AreEqual(secondPointX - firstPointX, inputParameters.ExitPointL);
        }

        [Test]
        public void SurfaceLine_WithoutDikeToeDikeSideAndDikeToeRiverSide_NoChangeForExitPointLAndSeePageLength()
        {
            // Setup
            var inputParameters = new PipingInput();
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            
            // Call
            inputParameters.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(Math.Exp(-0.5), inputParameters.SeepageLength.Mean);
            Assert.AreEqual(0, inputParameters.ExitPointL);
        }
    }
}