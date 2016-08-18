﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
            GeneralPipingInput generalInputParameters = new GeneralPipingInput();

            // Call
            PipingInput inputParameters = new PipingInput(generalInputParameters);

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

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.Diameter70);
            Assert.IsNaN(inputParameters.Diameter70.Mean);
            Assert.IsNaN(inputParameters.Diameter70.StandardDeviation);
            Assert.AreEqual(6, inputParameters.Diameter70.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(6, inputParameters.Diameter70.StandardDeviation.NumberOfDecimalPlaces);

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.DarcyPermeability);
            Assert.IsNaN(inputParameters.DarcyPermeability.Mean);
            Assert.IsNaN(inputParameters.DarcyPermeability.StandardDeviation);
            Assert.AreEqual(6, inputParameters.DarcyPermeability.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(6, inputParameters.DarcyPermeability.StandardDeviation.NumberOfDecimalPlaces);

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

            Assert.IsInstanceOf<LogNormalDistribution>(inputParameters.SaturatedVolumicWeightOfCoverageLayer);
            Assert.AreEqual(2, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean.NumberOfDecimalPlaces);
            Assert.AreEqual(2, inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.NumberOfDecimalPlaces);
            Assert.AreEqual(2, inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift.NumberOfDecimalPlaces);
            Assert.IsNaN(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Mean.Value);
            Assert.IsNaN(inputParameters.SaturatedVolumicWeightOfCoverageLayer.StandardDeviation.Value);
            Assert.IsNaN(inputParameters.SaturatedVolumicWeightOfCoverageLayer.Shift.Value);

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
            // Call
            TestDelegate call = () => new PipingInput(null);

            // Assert
            Assert.Throws<ArgumentNullException>(call);
        }

        [Test]
        [TestCase(1.23456)]
        [TestCase(3.5)]
        public void ExitPointL_ExitPointEqualSmallerThanEntryPoint_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            PipingInput pipingInput = new PipingInput(new GeneralPipingInput())
            {
                EntryPointL = (RoundedDouble) 3.5
            };

            // Call
            TestDelegate call = () => pipingInput.ExitPointL = (RoundedDouble) value;

            // Assert
            var expectedMessage = Resources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        public void ExitPointL_Always_SameNumberOfDecimalsAsSurfaceLineLocalGeometry()
        {
            // Setup
            PipingInput pipingInput = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = CreateSurfaceLine()
            };

            // Call
            RoundedPoint2DCollection localGeometry = pipingInput.SurfaceLine.ProjectGeometryToLZ();

            // Assert
            Assert.AreEqual(localGeometry.NumberOfDecimalPlaces, pipingInput.ExitPointL.NumberOfDecimalPlaces);
        }

        [Test]
        [TestCase(5.4)]
        [TestCase(1.006)]
        [TestCase(-0.005)]
        [TestCase(-5.4)]
        public void ExitPointL_ExitPointNotOnSurfaceLine_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = (RoundedDouble) double.NaN;

            // Call
            TestDelegate call = () => input.ExitPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 1]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(-1e-3, Description = "Valid ExitPointL due to rounding to 0.0")]
        [TestCase(0.1004)]
        [TestCase(0.50)]
        public void ExitPointL_SetToNew_ValueIsRounded(double exitPointValue)
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = (RoundedDouble) double.NaN;

            int originalNumberOfDecimalPlaces = input.ExitPointL.NumberOfDecimalPlaces;

            // Call
            input.ExitPointL = (RoundedDouble) exitPointValue;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.ExitPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlaces, exitPointValue), input.ExitPointL);
        }

        [Test]
        [TestCase(5.0)]
        [TestCase(3.5)]
        public void EntryPointL_EntryPointEqualOrGreaterThanExitPoint_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            PipingInput pipingInput = new PipingInput(new GeneralPipingInput())
            {
                ExitPointL = (RoundedDouble) 3.5
            };

            // Call
            TestDelegate call = () => pipingInput.EntryPointL = (RoundedDouble) value;

            // Assert
            var expectedMessage = Resources.PipingInput_EntryPointL_greater_or_equal_to_ExitPointL;
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(5.4)]
        [TestCase(1.006)]
        [TestCase(-0.005)]
        [TestCase(-5.4)]
        public void EntryPointL_EntryPointNotOnSurfaceLine_ThrowsArgumentOutOfRangeException(double value)
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) double.NaN;
            // Call
            TestDelegate call = () => input.EntryPointL = (RoundedDouble) value;

            // Assert
            const string expectedMessage = "Het gespecificeerde punt moet op het profiel liggen (bereik [0, 1]).";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(call, expectedMessage);
        }

        [Test]
        [TestCase(double.NaN)]
        [TestCase(-1e-3, Description = "Valid EntryPointL due to rounding to 0.0")]
        [TestCase(0.005)]
        [TestCase(0.1004)]
        [TestCase(0.50)]
        public void EntryPointL_SetToNew_ValueIsRounded(double entryPointValue)
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) double.NaN;

            int originalNumberOfDecimalPlaces = input.EntryPointL.NumberOfDecimalPlaces;

            // Call
            input.EntryPointL = (RoundedDouble) entryPointValue;

            // Assert
            Assert.AreEqual(originalNumberOfDecimalPlaces, input.EntryPointL.NumberOfDecimalPlaces);
            Assert.AreEqual(new RoundedDouble(originalNumberOfDecimalPlaces, entryPointValue), input.EntryPointL);
        }

        [Test]
        public void EntryPointL_Always_SameNumberOfDecimalsAsSurfaceLineLocalGeometry()
        {
            // Setup
            RingtoetsPipingSurfaceLine surfaceLine = CreateSurfaceLine();
            PipingInput pipingInput = new PipingInput(new GeneralPipingInput());

            // Call
            RoundedPoint2DCollection localGeometry = surfaceLine.ProjectGeometryToLZ();

            // Assert
            Assert.AreEqual(localGeometry.NumberOfDecimalPlaces, pipingInput.EntryPointL.NumberOfDecimalPlaces);
        }

        [Test]
        public void SurfaceLine_WithDikeToes_ThenExitPointLAndEntryPointLUpdated()
        {
            // Given
            PipingInput input = new PipingInput(new GeneralPipingInput());

            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
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
            PipingInput inputs = new PipingInput(new GeneralPipingInput());
            NormalDistribution originalPhreaticLevelExit = inputs.PhreaticLevelExit;

            NormalDistribution newValue = new NormalDistribution(5)
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
            PipingInput inputs = new PipingInput(new GeneralPipingInput());
            LogNormalDistribution originalDampingFactorExit = inputs.DampingFactorExit;

            LogNormalDistribution newValue = new LogNormalDistribution(5)
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
        public void AssessmentLevel_InputHasNewHydraulicBoundaryLocationSet_AssessmentLevelUpdated()
        {
            // Setup
            PipingInput input = new PipingInput(new GeneralPipingInput());

            RoundedDouble testLevel = (RoundedDouble)new Random(21).NextDouble();

            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0)
            {
                DesignWaterLevel = testLevel
            };

            // Call
            RoundedDouble calculatedAssessmentLevel = input.AssessmentLevel;

            // Assert
            Assert.AreEqual(testLevel, calculatedAssessmentLevel, input.AssessmentLevel.GetAccuracy());
        }

        [Test]
        public void PiezometricHeadExit_ValidInput_SetsParametersForCalculatorAndReturnsPiezometricHead()
        {
            // Setup
            PipingInput input = new PipingInput(new GeneralPipingInput());

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                RoundedDouble piezometricHead = input.PiezometricHeadExit;

                // Assert
                Assert.AreEqual(2, piezometricHead.NumberOfDecimalPlaces);
                Assert.IsFalse(double.IsNaN(piezometricHead));

                TestPipingSubCalculatorFactory factory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                PiezoHeadCalculatorStub piezometricHeadAtExitCalculator = factory.LastCreatedPiezometricHeadAtExitCalculator;

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
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);

            // Call
            RoundedDouble piezometricHead = input.PiezometricHeadExit;

            // Assert
            Assert.IsNaN(piezometricHead);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsThicknessAquiferLayer()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessCoverageLayer = input.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
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
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
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
            PipingInput input = PipingCalculationFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

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
            PipingInput input = PipingCalculationFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

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
            PipingInput input = PipingCalculationFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-6);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetExitPointSetToNaN_ThicknessAquiferLayerNaN()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
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
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
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
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
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
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
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
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToConsecutiveAquiferLayerThickness()
        {
            // Setup
            double expectedThickness;
            PipingInput input = PipingCalculationFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-6);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetSoilProfileSetToNull_ThicknessAquiferLayerNaN()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputResultsInZeroAquiferThickness_ThicknessAquiferLayerNaN()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
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
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
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
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = new StochasticSoilProfile(0.0, SoilProfileType.SoilProfile1D, 0)
            {
                SoilProfile = new PipingSoilProfile(string.Empty, 0, new[]
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
            LogNormalDistribution thicknessAquiferLayer = input.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(2.0, thicknessAquiferLayer.Mean.Value, 1e-6);
        }

        [Test]
        public void SeepageLength_ValidData_ReturnsSeepageLength()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();

            // Call
            LogNormalDistribution seepageLength = input.SeepageLength;

            // Assert
            Assert.AreEqual(0.5, seepageLength.Mean.Value);
            Assert.AreEqual(0.05, seepageLength.StandardDeviation.Value);
        }

        [Test]
        public void SeepageLength_EntryPointNaN_SeepageLengthNaN()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = (RoundedDouble) double.NaN;

            // Call
            LogNormalDistribution seepageLength = input.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.IsNaN(seepageLength.StandardDeviation);
        }

        [Test]
        public void SeepageLength_ExitPointNaN_SeepageLengthNaN()
        {
            // Setup
            PipingInput input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) double.NaN;

            // Call
            LogNormalDistribution seepageLength = input.SeepageLength;

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
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 0),
                new Point3D(2, 0, 2)
            });

            return surfaceLine;
        }
    }
}