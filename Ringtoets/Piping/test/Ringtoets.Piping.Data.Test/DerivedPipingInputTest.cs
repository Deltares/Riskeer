// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Base.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class DerivedPipingInputTest
    {
        [Test]
        public void Constructor_PipingInputNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DerivedPipingInput(null);

            // Assert
            string paramName = Assert.Throws<ArgumentNullException>(call).ParamName;
            Assert.AreEqual("input", paramName);
        }

        [Test]
        public void PiezometricHeadExit_ValidInput_SetsParametersForCalculatorAndReturnsNotNaN()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var derivedInput = new DerivedPipingInput(input);

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                RoundedDouble piezometricHead = derivedInput.PiezometricHeadExit;

                // Assert
                Assert.AreEqual(2, piezometricHead.NumberOfDecimalPlaces);
                Assert.IsFalse(double.IsNaN(piezometricHead));

                var factory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                PiezoHeadCalculatorStub piezometricHeadAtExitCalculator = factory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(input.AssessmentLevel, piezometricHeadAtExitCalculator.HRiver,
                                input.AssessmentLevel.GetAccuracy());
                Assert.AreEqual(PipingSemiProbabilisticDesignVariableFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                input.PhreaticLevelExit.GetAccuracy());
                Assert.AreEqual(PipingSemiProbabilisticDesignVariableFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                input.DampingFactorExit.GetAccuracy());
            }
        }

        [Test]
        public void PiezometricHeadExit_InputWithoutAssessmentLevel_ReturnsNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            RoundedDouble piezometricHead = derivedInput.PiezometricHeadExit;

            // Assert
            Assert.IsNaN(piezometricHead);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsDistributionWithExpectedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            AssertEffectiveThicknessCoverageLayer(effectiveThicknessCoverageLayer, 2.0);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputWithoutSoilProfile_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            AssertEffectiveThicknessCoverageLayer(effectiveThicknessCoverageLayer);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputWithoutSurfaceLine_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.SurfaceLine = null;

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            AssertEffectiveThicknessCoverageLayer(effectiveThicknessCoverageLayer);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void EffectiveThicknessCoverageLayer_SoilProfileSingleAquiferAboveSurfaceLine_ReturnsDistributionWithMeanNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            AssertEffectiveThicknessCoverageLayer(effectiveThicknessCoverageLayer);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_ProfileWithoutAquiferLayer_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D)
            );

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            AssertEffectiveThicknessCoverageLayer(effectiveThicknessCoverageLayer);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D)
            );

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            AssertEffectiveThicknessCoverageLayer(effectiveThicknessCoverageLayer);
        }

        [Test]
        public void ThicknessCoverageLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsDistributionWithExpectedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            AssertThicknessCoverageLayer(thicknessCoverageLayer, 2.0);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSoilProfile_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            AssertThicknessCoverageLayer(thicknessCoverageLayer);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSurfaceLine_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.SurfaceLine = null;

            // Call
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            AssertThicknessCoverageLayer(thicknessCoverageLayer);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessCoverageLayer_SoilProfileSingleAquiferAboveSurfaceLine_ReturnsDistributionWithMeanNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            AssertThicknessCoverageLayer(thicknessCoverageLayer);
        }

        [Test]
        public void ThicknessCoverageLayer_ProfileWithoutAquiferLayer_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D)
            );

            // Call
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            AssertThicknessCoverageLayer(thicknessCoverageLayer);
        }

        [Test]
        public void ThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D)
            );

            // Call
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            AssertThicknessCoverageLayer(thicknessCoverageLayer);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsDistributionWithExpectedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer, 1.0);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSoilProfile_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSurfaceLine_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.SurfaceLine = null;

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAboveSurfaceLine_ReturnsDistributionWithMeanNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_ReturnsDistributionWithMeanSetToTopmostConsecutiveAquiferLayerThickness()
        {
            // Setup
            double expectedThickness;
            PipingInput input = PipingInputFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer, expectedThickness);
        }

        [Test]
        public void ThicknessAquiferLayer_ExitPointSetToNaN_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.ExitPointL = RoundedDouble.NaN;

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer);
        }

        [Test]
        public void ThicknessAquiferLayer_ProfileWithoutAquiferLayer_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D)
            );

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferUnderSurfaceLine_ReturnsDistributionWithExpectedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquifer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer, 1.0);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_ReturnsDistributionWithMeanSetToConsecutiveAquiferLayerThickness()
        {
            // Setup
            double expectedThickness;
            PipingInput input = PipingInputFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer, expectedThickness);
        }

        [Test]
        public void ThicknessAquiferLayer_InputResultsInZeroAquiferThickness_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(0.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D)
            );

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer);
        }

        [Test]
        public void ThicknessAquiferLayer_SurfaceLineHalfWayProfileLayer_ReturnsDistributionWithMeanSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile(string.Empty, 0, new[]
                {
                    new PipingSoilLayer(2.5)
                    {
                        IsAquifer = true
                    },
                    new PipingSoilLayer(1.5)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D)
            );

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            AssertThicknessAquiferLayer(thicknessAquiferLayer, 2.0);
        }

        [Test]
        public void SeepageLength_ValidData_ReturnsDistributionWithExpectedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            VariationCoefficientLogNormalDistribution seepageLength = derivedInput.SeepageLength;

            // Assert
            AssertSeepageLength(seepageLength, 0.5);
        }

        [Test]
        public void SeepageLength_EntryPointNaN_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.EntryPointL = RoundedDouble.NaN;

            // Call
            VariationCoefficientLogNormalDistribution seepageLength = derivedInput.SeepageLength;

            // Assert
            AssertSeepageLength(seepageLength);
        }

        [Test]
        public void SeepageLength_ExitPointNaN_ReturnsDistributionWithMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.ExitPointL = RoundedDouble.NaN;

            // Call
            VariationCoefficientLogNormalDistribution seepageLength = derivedInput.SeepageLength;

            // Assert
            AssertSeepageLength(seepageLength);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoStochasticSoilProfile_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            AssertSaturatedVolumicWeightOfCoverageLayer(result);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoSurfaceLine_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.SurfaceLine = null;

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            AssertSaturatedVolumicWeightOfCoverageLayer(result);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoExitPointL_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.ExitPointL = RoundedDouble.NaN;

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            AssertSaturatedVolumicWeightOfCoverageLayer(result);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoAquitardLayers_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(1.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            AssertSaturatedVolumicWeightOfCoverageLayer(result);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoAquiferLayers_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(1.0)
                    {
                        IsAquifer = false
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            AssertSaturatedVolumicWeightOfCoverageLayer(result);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoCoverageLayersAboveTopAquiferLayer_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(2.0)
                    {
                        IsAquifer = false
                    },
                    new PipingSoilLayer(1.0)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            AssertSaturatedVolumicWeightOfCoverageLayer(result);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_SingleLayer_ReturnsDistributionWithParametersFromLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            var random = new Random(21);
            double belowPhreaticLevelMean = 0.1 + random.NextDouble();
            double deviation = random.NextDouble();
            double shift = random.NextDouble();
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", 0.0, new[]
                {
                    new PipingSoilLayer(2.5)
                    {
                        BelowPhreaticLevel = new LogNormalDistribution
                        {
                            Mean = (RoundedDouble) belowPhreaticLevelMean,
                            StandardDeviation = (RoundedDouble) deviation,
                            Shift = (RoundedDouble) shift
                        }
                    },
                    new PipingSoilLayer(0.5)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            AssertSaturatedVolumicWeightOfCoverageLayer(result, belowPhreaticLevelMean, deviation, shift);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_MultipleLayersEqualStandardDeviationAndShift_ReturnsDistributionWithWeightedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            var random = new Random(21);
            double belowPhreaticLevelMeanA = 0.1 + random.NextDouble();
            double belowPhreaticLevelMeanB = 0.1 + random.NextDouble();
            double deviation = random.NextDouble();
            double shift = random.NextDouble();
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(2.5)
                    {
                        BelowPhreaticLevel = new LogNormalDistribution
                        {
                            Mean = (RoundedDouble) belowPhreaticLevelMeanA,
                            StandardDeviation = (RoundedDouble) deviation,
                            Shift = (RoundedDouble) shift
                        }
                    },
                    new PipingSoilLayer(-0.5)
                    {
                        BelowPhreaticLevel = new LogNormalDistribution
                        {
                            Mean = (RoundedDouble) belowPhreaticLevelMeanB,
                            StandardDeviation = (RoundedDouble) deviation,
                            Shift = (RoundedDouble) shift
                        }
                    },
                    new PipingSoilLayer(-1.5)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            double expectedMean = (belowPhreaticLevelMeanA * 2.5 + belowPhreaticLevelMeanB * 1.0) / 3.5;
            AssertSaturatedVolumicWeightOfCoverageLayer(result, expectedMean, deviation, shift);
        }

        [Test]
        [TestCase(0.01, 0)]
        [TestCase(0, 0.01)]
        [TestCase(2, 1)]
        [TestCase(3, -1)]
        [TestCase(-0.01, 0)]
        [TestCase(0, -0.01)]
        public void SaturatedVolumicWeightOfCoverageLayer_MultipleLayersInequalStandardDeviationOrShift_ReturnsDistributionWithParametersNaN(double deviationDelta, double shiftDelta)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            var random = new Random(21);
            double belowPhreaticLevelMeanA = 0.1 + random.NextDouble();
            double belowPhreaticLevelMeanB = 0.1 + random.NextDouble();
            double deviation = random.NextDouble();
            double shift = random.NextDouble();
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(2.5)
                    {
                        BelowPhreaticLevel = new LogNormalDistribution
                        {
                            Mean = (RoundedDouble) belowPhreaticLevelMeanA,
                            StandardDeviation = (RoundedDouble) deviation,
                            Shift = (RoundedDouble) shift
                        }
                    },
                    new PipingSoilLayer(-0.5)
                    {
                        BelowPhreaticLevel = new LogNormalDistribution
                        {
                            Mean = (RoundedDouble) belowPhreaticLevelMeanB,
                            StandardDeviation = (RoundedDouble) (deviation + deviationDelta),
                            Shift = (RoundedDouble) (shift + shiftDelta)
                        }
                    },
                    new PipingSoilLayer(-1.5)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            AssertSaturatedVolumicWeightOfCoverageLayer(result);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_MultipleLayersInequalStandardDeviationOrShiftButEqualWhenRounded_ReturnsDistributionWithWeightedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            const double belowPhreaticLevelMeanA = 2.5;
            const double belowPhreaticLevelMeanB = 3.4;
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(2.5)
                    {
                        BelowPhreaticLevel = new LogNormalDistribution
                        {
                            Mean = (RoundedDouble) belowPhreaticLevelMeanA,
                            StandardDeviation = (RoundedDouble) 1.014,
                            Shift = (RoundedDouble) 1.014
                        }
                    },
                    new PipingSoilLayer(-0.5)
                    {
                        BelowPhreaticLevel = new LogNormalDistribution
                        {
                            Mean = (RoundedDouble) belowPhreaticLevelMeanB,
                            StandardDeviation = (RoundedDouble) 1.006,
                            Shift = (RoundedDouble) 1.006
                        }
                    },
                    new PipingSoilLayer(-1.5)
                    {
                        IsAquifer = true
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            LogNormalDistribution result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            double expectedMean = (belowPhreaticLevelMeanA * 2.5 + belowPhreaticLevelMeanB * 1.0) / 3.5;
            AssertSaturatedVolumicWeightOfCoverageLayer(result, expectedMean, (RoundedDouble) 1.01, (RoundedDouble) 1.01);
        }

        [Test]
        public void DarcyPermeability_NoStochasticSoilProfile_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DarcyPermeability;

            // Assert
            AssertDarcyPermeability(result);
        }

        [Test]
        public void DarcyPermeability_NoSurfaceLine_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.SurfaceLine = null;

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DarcyPermeability;

            // Assert
            AssertDarcyPermeability(result);
        }

        [Test]
        public void DarcyPermeability_NoExitPointL_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.ExitPointL = RoundedDouble.NaN;

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DarcyPermeability;

            // Assert
            AssertDarcyPermeability(result);
        }

        [Test]
        public void DarcyPermeability_NoAquiferLayers_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D));

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DarcyPermeability;

            // Assert
            AssertDarcyPermeability(result);
        }

        [Test]
        public void DarcyPermeability_MultipleAquiferLayersWithSameVariation_ReturnsDistributionWithWeightedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            var random = new Random(21);
            double mean = 0.1 + random.NextDouble();
            double mean2 = 0.1 + random.NextDouble();
            const double coefficientOfVariation = 0.5;
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", 0.0, new[]
                {
                    new PipingSoilLayer(0.5)
                    {
                        IsAquifer = true,
                        Permeability = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) mean,
                            CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                        }
                    },
                    new PipingSoilLayer(1.5)
                    {
                        IsAquifer = true,
                        Permeability = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) mean2,
                            CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
                        }
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DarcyPermeability;

            // Assert
            double weightedMean = (mean * 0.5 + mean2) / 1.5;
            AssertDarcyPermeability(result, weightedMean, coefficientOfVariation);
        }

        [Test]
        public void DarcyPermeability_SingleAquiferLayerWithRandomMeanAndDeviation_ReturnsDistributionWithWeightedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            var random = new Random(21);
            double permeabilityMean = 0.1 + random.NextDouble();
            double permeabilityCoefficientOfVariation = random.NextDouble();
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(1.0)
                    {
                        IsAquifer = true,
                        Permeability = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) permeabilityMean,
                            CoefficientOfVariation = (RoundedDouble) permeabilityCoefficientOfVariation
                        }
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DarcyPermeability;

            // Assert
            var expectedMean = new RoundedDouble(6, permeabilityMean);
            var expectedCoefficientOfVariation = new RoundedDouble(6, permeabilityCoefficientOfVariation);
            AssertDarcyPermeability(result, expectedMean, expectedCoefficientOfVariation);
        }

        [Test]
        public void DarcyPermeability_MultipleAquiferLayersWithDifferentMeanAndDeviation_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(1.0)
                    {
                        IsAquifer = true,
                        Permeability = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) 0.5,
                            CoefficientOfVariation = (RoundedDouble) 0.2
                        }
                    },
                    new PipingSoilLayer(0.0)
                    {
                        IsAquifer = true,
                        Permeability = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) 12.5,
                            CoefficientOfVariation = (RoundedDouble) 2.3
                        }
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DarcyPermeability;

            // Assert
            AssertDarcyPermeability(result);
        }

        [Test]
        public void DiameterD70_NoStochasticSoilProfile_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DiameterD70;

            // Assert
            AssertDiameterD70(result);
        }

        [Test]
        public void DiameterD70_NoSurfaceLine_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.SurfaceLine = null;

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DiameterD70;

            // Assert
            AssertDiameterD70(result);
        }

        [Test]
        public void DiameterD70_NoExitPointL_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.ExitPointL = RoundedDouble.NaN;

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DiameterD70;

            // Assert
            AssertDiameterD70(result);
        }

        [Test]
        public void DiameterD70_NoAquiferLayers_ReturnsDistributionWithParametersNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(1.0)
                }, SoilProfileType.SoilProfile1D));

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DiameterD70;

            // Assert
            AssertDiameterD70(result);
        }

        [Test]
        public void DiameterD70_SingleAquiferLayers_ReturnsDistributionWithParametersFromLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            var random = new Random(21);
            double diameterD70Mean = 0.1 + random.NextDouble();
            double diameterD70CoefficientOfVariation = random.NextDouble();
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(1.0)
                    {
                        IsAquifer = true,
                        DiameterD70 = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) diameterD70Mean,
                            CoefficientOfVariation = (RoundedDouble) diameterD70CoefficientOfVariation
                        }
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DiameterD70;

            // Assert
            AssertDiameterD70(result, diameterD70Mean, diameterD70CoefficientOfVariation);
        }

        [Test]
        public void DiameterD70_MultipleAquiferLayers_ReturnsWithParametersFromTopmostLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            const double diameterD70Mean = 0.5;
            const double diameterD70CoefficientOfVariation = 0.2;
            input.StochasticSoilProfile = new PipingStochasticSoilProfile(
                0.0, new PipingSoilProfile("", -2.0, new[]
                {
                    new PipingSoilLayer(1.0)
                    {
                        IsAquifer = true,
                        DiameterD70 = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) diameterD70Mean,
                            CoefficientOfVariation = (RoundedDouble) diameterD70CoefficientOfVariation
                        }
                    },
                    new PipingSoilLayer(0.0)
                    {
                        IsAquifer = true,
                        DiameterD70 = new VariationCoefficientLogNormalDistribution
                        {
                            Mean = (RoundedDouble) 12.5,
                            CoefficientOfVariation = (RoundedDouble) 2.3
                        }
                    }
                }, SoilProfileType.SoilProfile1D));

            // Call
            VariationCoefficientLogNormalDistribution result = derivedInput.DiameterD70;

            // Assert
            AssertDiameterD70(result, diameterD70Mean, diameterD70CoefficientOfVariation);
        }

        private static void AssertEffectiveThicknessCoverageLayer(LogNormalDistribution effectiveThicknessCoverageLayer, double mean = double.NaN)
        {
            var expected = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) mean,
                StandardDeviation = (RoundedDouble) 0.5
            };

            DistributionAssert.AreEqual(expected, effectiveThicknessCoverageLayer);
        }

        private static void AssertThicknessCoverageLayer(LogNormalDistribution thicknessCoverageLayer, double mean = double.NaN)
        {
            var expected = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) mean,
                StandardDeviation = (RoundedDouble) 0.5
            };

            DistributionAssert.AreEqual(expected, thicknessCoverageLayer);
        }

        private static void AssertThicknessAquiferLayer(LogNormalDistribution thicknessAquiferLayer, double mean = double.NaN)
        {
            var expected = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) mean,
                StandardDeviation = (RoundedDouble) 0.5
            };

            DistributionAssert.AreEqual(expected, thicknessAquiferLayer);
        }

        private static void AssertSeepageLength(VariationCoefficientLogNormalDistribution seepageLength, double mean = double.NaN)
        {
            var expected = new VariationCoefficientLogNormalDistribution(2)
            {
                Mean = (RoundedDouble) mean,
                CoefficientOfVariation = (RoundedDouble) 0.1
            };

            DistributionAssert.AreEqual(expected, seepageLength);
        }

        private static void AssertSaturatedVolumicWeightOfCoverageLayer(LogNormalDistribution saturatedVolumicWeightOfCoverageLayer,
                                                                        double mean = double.NaN,
                                                                        double standardDeviation = double.NaN,
                                                                        double shift = double.NaN)
        {
            var expected = new LogNormalDistribution(2)
            {
                Mean = (RoundedDouble) mean,
                StandardDeviation = (RoundedDouble) standardDeviation,
                Shift = (RoundedDouble) shift
            };

            DistributionAssert.AreEqual(expected, saturatedVolumicWeightOfCoverageLayer);
        }

        private static void AssertDarcyPermeability(VariationCoefficientLogNormalDistribution darcyPermeability,
                                                    double mean = double.NaN,
                                                    double standardDeviation = double.NaN)
        {
            var expected = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = (RoundedDouble) mean,
                CoefficientOfVariation = (RoundedDouble) standardDeviation
            };

            DistributionAssert.AreEqual(expected, darcyPermeability);
        }

        private static void AssertDiameterD70(VariationCoefficientLogNormalDistribution diameterD70,
                                              double mean = double.NaN,
                                              double coefficientOfVariation = double.NaN)
        {
            var expected = new VariationCoefficientLogNormalDistribution(6)
            {
                Mean = (RoundedDouble) mean,
                CoefficientOfVariation = (RoundedDouble) coefficientOfVariation
            };

            DistributionAssert.AreEqual(expected, diameterD70);
        }
    }
}