using System;

using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;

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
            Assert.AreEqual(0, inputParameters.PhreaticLevelExit.Mean.Value);
            Assert.AreEqual(3, inputParameters.PhreaticLevelExit.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.PhreaticLevelExit.StandardDeviation.Value);
            Assert.AreEqual(3, inputParameters.PhreaticLevelExit.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.DampingFactorExit);
            Assert.AreEqual(0.7, inputParameters.DampingFactorExit.Mean.Value);
            Assert.AreEqual(3, inputParameters.DampingFactorExit.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, inputParameters.DampingFactorExit.StandardDeviation,
                            GetErrorTolerance(inputParameters.DampingFactorExit.StandardDeviation));
            Assert.AreEqual(3, inputParameters.DampingFactorExit.StandardDeviation.NumberOfDecimalPlaces);

            double defaultLogNormalMean = Math.Exp(-0.5);
            double defaultLogNormalStandardDev = Math.Sqrt((Math.Exp(1) - 1) * Math.Exp(1));

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.Diameter70);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.Diameter70.Mean,
                            GetErrorTolerance(inputParameters.Diameter70.Mean));
            Assert.AreEqual(2, inputParameters.Diameter70.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.Diameter70.StandardDeviation,
                            GetErrorTolerance(inputParameters.Diameter70.StandardDeviation));
            Assert.AreEqual(2, inputParameters.Diameter70.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.DarcyPermeability);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.DarcyPermeability.Mean,
                            GetErrorTolerance(inputParameters.DarcyPermeability.Mean));
            Assert.AreEqual(3, inputParameters.DarcyPermeability.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.DarcyPermeability.StandardDeviation,
                            GetErrorTolerance(inputParameters.DarcyPermeability.StandardDeviation));
            Assert.AreEqual(3, inputParameters.DarcyPermeability.StandardDeviation.NumberOfDecimalPlaces);

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
            Assert.AreEqual(2, inputParameters.ThicknessCoverageLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, inputParameters.ThicknessCoverageLayer.StandardDeviation.Value);
            Assert.AreEqual(2, inputParameters.ThicknessCoverageLayer.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<ShiftedLognormalDistribution>(inputParameters.SaturatedVolumicWeightOfCoverageLayer);
            Assert.AreEqual(17.5, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean.Value);
            Assert.AreEqual(2, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0, inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.Value);
            Assert.AreEqual(2, inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.ThicknessAquiferLayer);
            Assert.IsNaN(inputParameters.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(2, inputParameters.ThicknessAquiferLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, inputParameters.ThicknessAquiferLayer.StandardDeviation.Value);
            Assert.AreEqual(2, inputParameters.ThicknessAquiferLayer.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LognormalDistribution>(inputParameters.SeepageLength);
            Assert.IsNaN(inputParameters.SeepageLength.Mean);
            Assert.AreEqual(2, inputParameters.SeepageLength.Mean.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.SeepageLength.StandardDeviation);
            Assert.AreEqual(2, inputParameters.SeepageLength.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsNaN(inputParameters.ExitPointL);
            Assert.AreEqual(2, inputParameters.ExitPointL.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.EntryPointL);
            Assert.AreEqual(2, inputParameters.EntryPointL.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.PiezometricHeadExit);
            Assert.AreEqual(2, inputParameters.PiezometricHeadExit.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<RoundedDouble>(inputParameters.AssessmentLevel);
            Assert.AreEqual(2, inputParameters.AssessmentLevel.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.AssessmentLevel);
        }

        private static double GetErrorTolerance(RoundedDouble roundedDouble)
        {
            return Math.Pow(10.0, -roundedDouble.NumberOfDecimalPlaces);
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
        public void ExitPointL_SetToValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            int originalNumberOfDecimalPlaces = pipingInput.ExitPointL.NumberOfDecimalPlaces;

            // Call
            pipingInput.ExitPointL = new RoundedDouble(5, 1.23456);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, pipingInput.ExitPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(1.23, pipingInput.ExitPointL.Value);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-21)]
        public void ExitPointL_ValueLessThanOrEqualToZero_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            TestDelegate test = () => pipingInput.ExitPointL = (RoundedDouble)value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, Resources.PipingInput_ExitPointL_Value_must_be_greater_than_zero);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-21)]
        public void EntryPointL_ValueLessThanZero_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            TestDelegate test = () => pipingInput.EntryPointL = (RoundedDouble)value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, Resources.PipingInput_EntryPointL_Value_must_be_greater_than_or_equal_to_zero);
        }

        [Test]
        public void EntryPointL_SetToNewValueWithTooManyDecimalPlaces_ValueIsRounded()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());

            int originalNumberOfDecimalPlaces = pipingInput.EntryPointL.NumberOfDecimalPlaces;

            // Call
            pipingInput.EntryPointL = new RoundedDouble(5, 9.87654);

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, pipingInput.EntryPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(9.88, pipingInput.EntryPointL.Value);
        }

        [Test]
        public void SurfaceLine_WithDikeToes_ThenExitPointLAndEntryPointLUpdated()
        {
            // Given
            var input = new PipingInput(new GeneralPipingInput());

            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0), 
                new Point3D(1, 0, 2), 
                new Point3D(2, 0, 3)
            });
            surfaceLine.SetDikeToeAtRiverAt(new Point3D(1, 0, 2));
            surfaceLine.SetDikeToeAtPolderAt(new Point3D(2, 0, 3));

            // Call
            input.SurfaceLine = surfaceLine;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, 1), input.EntryPointL);
            Assert.AreEqual(new RoundedDouble(2, 2), input.ExitPointL);
        }

        [Test]
        public void PhreaticLevelExit_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var inputs = new PipingInput(new GeneralPipingInput());
            NormalDistribution originalPhreaticLevelExit = inputs.PhreaticLevelExit;

            var newValue = new NormalDistribution(5)
            {
                Mean = (RoundedDouble)1.23456,
                StandardDeviation = (RoundedDouble)7.89123
            };

            // Call
            inputs.PhreaticLevelExit = newValue;

            // Assert
            Assert.AreSame(originalPhreaticLevelExit, inputs.PhreaticLevelExit,
                "Stochast instance hasn't changed to 'newValue'.");
            Assert.AreEqual(3, originalPhreaticLevelExit.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.235, originalPhreaticLevelExit.Mean.Value);
            Assert.AreEqual(3, originalPhreaticLevelExit.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(7.891, originalPhreaticLevelExit.StandardDeviation.Value);
        }

        [Test]
        public void DampingFactorExit_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var inputs = new PipingInput(new GeneralPipingInput());
            LognormalDistribution originalDampingFactorExit = inputs.DampingFactorExit;

            var newValue = new LognormalDistribution(5)
            {
                Mean = (RoundedDouble)4.56789,
                StandardDeviation = (RoundedDouble)1.23456
            };

            // Call
            inputs.DampingFactorExit = newValue;

            // Assert
            Assert.AreSame(originalDampingFactorExit, inputs.DampingFactorExit,
                "Stochast instance hasn't changed to 'newValue'.");
            Assert.AreEqual(3, originalDampingFactorExit.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(4.568, originalDampingFactorExit.Mean.Value);
            Assert.AreEqual(3, originalDampingFactorExit.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(1.235, originalDampingFactorExit.StandardDeviation.Value);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var inputs = new PipingInput(new GeneralPipingInput());
            ShiftedLognormalDistribution originalSaturatedVolumicWeightOfCoverageLayer = inputs.SaturatedVolumicWeightOfCoverageLayer;

            var newValue = new ShiftedLognormalDistribution(5)
            {
                Mean = (RoundedDouble)1.11111,
                StandardDeviation = (RoundedDouble)2.22222,
                Shift = (RoundedDouble)(-3.33333)
            };

            // Call
            inputs.SaturatedVolumicWeightOfCoverageLayer = newValue;

            // Assert
            Assert.AreSame(originalSaturatedVolumicWeightOfCoverageLayer, inputs.SaturatedVolumicWeightOfCoverageLayer,
                "Stochast instance hasn't changed to 'newValue'.");
            Assert.AreEqual(2, originalSaturatedVolumicWeightOfCoverageLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.11, originalSaturatedVolumicWeightOfCoverageLayer.Mean.Value);
            Assert.AreEqual(2, originalSaturatedVolumicWeightOfCoverageLayer.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(2.22, originalSaturatedVolumicWeightOfCoverageLayer.StandardDeviation.Value);
            Assert.AreEqual(2, originalSaturatedVolumicWeightOfCoverageLayer.Shift.NumberOfDecimalPlaces);
            Assert.AreEqual(-3.33, originalSaturatedVolumicWeightOfCoverageLayer.Shift.Value);
        }

        [Test]
        public void Diameter70_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var inputs = new PipingInput(new GeneralPipingInput());
            LognormalDistribution originalDiameter70 = inputs.Diameter70;

            var newValue = new LognormalDistribution(5)
            {
                Mean = (RoundedDouble)8.8888,
                StandardDeviation = (RoundedDouble)9.14363
            };

            // Call
            inputs.Diameter70 = newValue;

            // Assert
            Assert.AreSame(originalDiameter70, inputs.Diameter70,
                "Stochast instance hasn't changed to 'newValue'.");
            Assert.AreEqual(2, originalDiameter70.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(8.89, originalDiameter70.Mean.Value);
            Assert.AreEqual(2, originalDiameter70.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(9.14, originalDiameter70.StandardDeviation.Value);
        }

        [Test]
        public void DarcyPermeability_SetNewValue_UpdateMeanAndStandardDeviation()
        {
            // Setup
            var inputs = new PipingInput(new GeneralPipingInput());
            LognormalDistribution originalDarcyPermeability = inputs.DarcyPermeability;

            var newValue = new LognormalDistribution(5)
            {
                Mean = (RoundedDouble)1.93753,
                StandardDeviation = (RoundedDouble)859.49028
            };

            // Call
            inputs.DarcyPermeability = newValue;

            // Assert
            Assert.AreSame(originalDarcyPermeability, inputs.DarcyPermeability,
                "Stochast instance hasn't changed to 'newValue'.");
            Assert.AreEqual(3, originalDarcyPermeability.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1.938, originalDarcyPermeability.Mean.Value);
            Assert.AreEqual(3, originalDarcyPermeability.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(859.490, originalDarcyPermeability.StandardDeviation.Value);
        }

        [Test]
        public void AssessmentLevel_InputHasNewHydraulicBoundaryLocationSet_AssessmentLevelUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            double testLevel = new Random(21).NextDouble();

            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0)
            {
                DesignWaterLevel = testLevel
            };

            // Call
            var calculatedAssesmentLevel = input.AssessmentLevel;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, testLevel), calculatedAssesmentLevel);
        }

        [Test]
        public void PiezometricHeadExit_ValidInput_SetsParametersForCalculatorAndReturnsPiezometricHead()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                var piezometricHead = input.PiezometricHeadExit;

                // Assert
                Assert.AreEqual(2, piezometricHead.NumberOfDecimalPlaces);
                Assert.IsFalse(double.IsNaN(piezometricHead));

                var factory = (TestPipingSubCalculatorFactory)PipingSubCalculatorFactory.Instance;
                var piezometricHeadAtExitCalculator = factory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(piezometricHeadAtExitCalculator.HRiver, input.AssessmentLevel, input.AssessmentLevel.GetAccuracy());
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                input.PhreaticLevelExit.GetAccuracy());
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                input.DampingFactorExit.GetAccuracy());
            }
        }

        [Test]
        public void PiezometricHeadExit_InputWithAssessmentLevelMissing_PiezometricHeadSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);

            // Call
            var piezometricHead = input.PiezometricHeadExit;

            // Assert
            Assert.IsNaN(piezometricHead);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsThicknessAquiferLayer()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            var thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;

            // Call
            var thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;

            // Call
            var thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            LognormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            LognormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessCoverageLayer_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Call
            LognormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Call
            LognormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_AquiferMeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = PipingCalculationFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            LognormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-8);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetExitPointSetToNaN_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)double.NaN;

            // Call
            LognormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetExitPointSetBeyondSurfaceLine_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)3.0;

            // Call
            LognormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetExitPointSetBeyondSurfaceLine_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)3.0;

            // Call
            LognormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);

            input.SoilProfile = null;

            // Call
            LognormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            }, 0);

            // Call
            LognormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_ProfileWithoutAquiferLayer_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            }, 0);

            // Call
            LognormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            var thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = PipingCalculationFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            var thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-8);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetSoilProfileSetToNull_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            input.SoilProfile = null;

            // Call
            var thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputResultsInZeroAquiferThickness_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(0.0)
                {
                    IsAquifer = true
                }
            }, 0);

            // Call
            LognormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = true
                }
            }, 0);

            // Call
            LognormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.5)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = true
                }
            }, 0);

            // Call
            var thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(0.5, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void SeepageLength_ValidData_ReturnsSeepageLength()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            var seepageLength = input.SeepageLength;

            // Assert
            Assert.AreEqual(0.5, seepageLength.Mean.Value);
            Assert.AreEqual(0.05, seepageLength.StandardDeviation.Value);
        }

        [Test]
        public void SeepageLength_ExitPointSetBeyondEntryPoint_SeepageLengthNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)2;
            input.EntryPointL = (RoundedDouble)3;

            // Call
            var seepageLength = input.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.IsNaN(seepageLength.StandardDeviation);
        }

        [Test]
        public void SeepageLength_EntryPointNaN_SeepageLengthNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = (RoundedDouble)double.NaN;

            // Call
            var seepageLength = input.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.IsNaN(seepageLength.StandardDeviation);
        }

        [Test]
        public void SeepageLength_ExitPointNaN_SeepageLengthNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)double.NaN;

            // Call
            var seepageLength = input.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.IsNaN(seepageLength.StandardDeviation);
        }
    }
}