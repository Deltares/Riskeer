// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Data;
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
            Assert.IsInstanceOf<ICalculationInput>(inputParameters);

            Assert.IsInstanceOf<NormalDistribution>(inputParameters.PhreaticLevelExit);
            Assert.AreEqual(0, inputParameters.PhreaticLevelExit.Mean.Value);
            Assert.AreEqual(3, inputParameters.PhreaticLevelExit.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(1, inputParameters.PhreaticLevelExit.StandardDeviation.Value);
            Assert.AreEqual(3, inputParameters.PhreaticLevelExit.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.DampingFactorExit);
            Assert.AreEqual(0.7, inputParameters.DampingFactorExit.Mean.Value);
            Assert.AreEqual(3, inputParameters.DampingFactorExit.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0.0, inputParameters.DampingFactorExit.StandardDeviation,
                            GetErrorTolerance(inputParameters.DampingFactorExit.StandardDeviation));
            Assert.AreEqual(3, inputParameters.DampingFactorExit.StandardDeviation.NumberOfDecimalPlaces);

            double defaultLogNormalMean = Math.Exp(-0.5);
            double defaultLogNormalStandardDev = Math.Sqrt((Math.Exp(1) - 1)*Math.Exp(1));

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.Diameter70);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.Diameter70.Mean,
                            GetErrorTolerance(inputParameters.Diameter70.Mean));
            Assert.AreEqual(2, inputParameters.Diameter70.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.Diameter70.StandardDeviation,
                            GetErrorTolerance(inputParameters.Diameter70.StandardDeviation));
            Assert.AreEqual(2, inputParameters.Diameter70.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.DarcyPermeability);
            Assert.AreEqual(defaultLogNormalMean, inputParameters.DarcyPermeability.Mean,
                            GetErrorTolerance(inputParameters.DarcyPermeability.Mean));
            Assert.AreEqual(3, inputParameters.DarcyPermeability.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(defaultLogNormalStandardDev, inputParameters.DarcyPermeability.StandardDeviation,
                            GetErrorTolerance(inputParameters.DarcyPermeability.StandardDeviation));
            Assert.AreEqual(3, inputParameters.DarcyPermeability.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsNull(inputParameters.SurfaceLine);
            Assert.IsNull(inputParameters.StochasticSoilModel);
            Assert.IsNull(inputParameters.StochasticSoilProfile);
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

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.ThicknessCoverageLayer);
            Assert.IsNaN(inputParameters.ThicknessCoverageLayer.Mean);
            Assert.AreEqual(2, inputParameters.ThicknessCoverageLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, inputParameters.ThicknessCoverageLayer.StandardDeviation.Value);
            Assert.AreEqual(2, inputParameters.ThicknessCoverageLayer.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<ShiftedLogNormalDistribution>(inputParameters.SaturatedVolumicWeightOfCoverageLayer);
            Assert.AreEqual(17.5, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean.Value);
            Assert.AreEqual(2, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0, inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.Value);
            Assert.AreEqual(2, inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.ThicknessAquiferLayer);
            Assert.IsNaN(inputParameters.ThicknessAquiferLayer.Mean);
            Assert.AreEqual(2, inputParameters.ThicknessAquiferLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(0.5, inputParameters.ThicknessAquiferLayer.StandardDeviation.Value);
            Assert.AreEqual(2, inputParameters.ThicknessAquiferLayer.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.SeepageLength);
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
        public void ExitPointL_ExitPointSmallerThanEntryPoint_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                EntryPointL = (RoundedDouble) 3.5
            };
            const double value = 1.23456;

            // Call
            TestDelegate call = () => pipingInput.ExitPointL = (RoundedDouble)value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(
                call, 
                Resources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL);
        }

        [Test]
        public void ExitPointL_Always_SameNumberOfDecimalsAsSurfaceLineLocalGeometry()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = CreateSurfaceLine()
            };

            // Call
            RoundedPoint2DCollection localGeometry = pipingInput.SurfaceLine.ProjectGeometryToLZ();

            // Assert
            Assert.AreEqual(pipingInput.ExitPointL.NumberOfDecimalPlaces, localGeometry.NumberOfDecimalPlaces);
        }

        [Test]
        public void ExitPointL_ExitPointNotOnSurfaceLine_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            const double value = 5.4;

            // Call
            TestDelegate call = () => input.ExitPointL = (RoundedDouble) value;

            // Assert
            var expectedMessage = string.Format("Kan geen hoogte bepalen. De lokale coördinaat moet in het bereik [{0}, {1}] liggen.",
                                               0, 1);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(1.0)]
        public void ExitPointL_SetToNew_ValueIsRounded(double entryPointValue)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = CreateSurfaceLine(),
                EntryPointL = (RoundedDouble) entryPointValue
            };

            const double value = 1.23456;
            int originalNumberOfDecimalPlaces = pipingInput.ExitPointL.NumberOfDecimalPlaces;

            // Call
            pipingInput.ExitPointL = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, pipingInput.ExitPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlaces, value), pipingInput.ExitPointL);
        }

        [Test]
        public void EntryPointL_EntryPointGreaterThanExitPoint_ThrowsArgumentOutOfRangeException()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble)3.5
            };
            const double value = 5.0;

            // Call
            TestDelegate call = () => pipingInput.EntryPointL = (RoundedDouble)value;

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(
                call,
                Resources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL);
        }

        [Test]
        public void EntryPointL_EntryPointNotOnSurfaceLine_ThrowsArgumentOutOfRangeException()
        {
             // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            const double value = -3.0;

            // Call
            TestDelegate call = () => input.EntryPointL = (RoundedDouble)value;

            // Assert
            var expectedMessage = string.Format("Kan geen hoogte bepalen. De lokale coördinaat moet in het bereik [{0}, {1}] liggen.",
                                               0, 1);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
         }

        [Test]
        [TestCase(-1e-3, Description = "Valid EntryPointL due to rounding to 0.0")]
        [TestCase(1.23456789)]
        public void EntryPointL_SetToNew_ValueIsRounded(double value)
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput());
            int originalNumberOfDecimalPlaces = pipingInput.EntryPointL.NumberOfDecimalPlaces;

            // Call
            pipingInput.EntryPointL = (RoundedDouble) value;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, pipingInput.EntryPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlaces, value), pipingInput.EntryPointL);
        }

        [Test]
        public void EntryPointL_Always_SameNumberOfDecimalsAsSurfaceLineLocalGeometry()
        {
            // Setup
            var pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = CreateSurfaceLine()
            };

            // Call
            RoundedPoint2DCollection localGeometry = pipingInput.SurfaceLine.ProjectGeometryToLZ();

            // Assert
            Assert.AreEqual(pipingInput.EntryPointL.NumberOfDecimalPlaces, localGeometry.NumberOfDecimalPlaces);
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
                Mean = (RoundedDouble) 1.23456,
                StandardDeviation = (RoundedDouble) 7.89123
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
            LogNormalDistribution originalDampingFactorExit = inputs.DampingFactorExit;

            var newValue = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) 4.56789,
                StandardDeviation = (RoundedDouble) 1.23456
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
            ShiftedLogNormalDistribution originalSaturatedVolumicWeightOfCoverageLayer = inputs.SaturatedVolumicWeightOfCoverageLayer;

            var newValue = new ShiftedLogNormalDistribution(5)
            {
                Mean = (RoundedDouble) 1.11111,
                StandardDeviation = (RoundedDouble) 2.22222,
                Shift = (RoundedDouble) (-3.33333)
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
            LogNormalDistribution originalDiameter70 = inputs.Diameter70;

            var newValue = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) 8.8888,
                StandardDeviation = (RoundedDouble) 9.14363
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
            LogNormalDistribution originalDarcyPermeability = inputs.DarcyPermeability;

            var newValue = new LogNormalDistribution(5)
            {
                Mean = (RoundedDouble) 1.93753,
                StandardDeviation = (RoundedDouble) 859.49028
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

                var factory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
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
            input.StochasticSoilProfile = null;

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
            input.StochasticSoilProfile = null;

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
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

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
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

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
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

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
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_AquiferMeanSetToConsecutiveAquiferLayerThickness()
        {
            // Setup
            double expectedThickness;
            var input = PipingCalculationFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-6);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetExitPointSetToNaN_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) double.NaN;

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_ProfileWithoutAquiferLayer_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

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
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToConsecutiveAquiferLayerThickness()
        {
            // Setup
            double expectedThickness;
            var input = PipingCalculationFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            var thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-6);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetSoilProfileSetToNull_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            input.StochasticSoilProfile = null;

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
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(0.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.5)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(1.5)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D, 0)
            };

            // Call
            var thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(2.0, thicknessAquiferLayer.Mean.Value, 1e-6);
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
        public void SeepageLength_EntryPointNaN_SeepageLengthNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = (RoundedDouble) double.NaN;

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
            input.ExitPointL = (RoundedDouble) double.NaN;

            // Call
            var seepageLength = input.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.IsNaN(seepageLength.StandardDeviation);
        }

        private static double GetErrorTolerance(RoundedDouble roundedDouble)
        {
            return Math.Pow(10.0, -roundedDouble.NumberOfDecimalPlaces);
        }

        private static RingtoetsPipingSurfaceLine CreateSurfaceLine()
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(2, 0, 2)
            });

            return surfaceLine;
        }
    }
}