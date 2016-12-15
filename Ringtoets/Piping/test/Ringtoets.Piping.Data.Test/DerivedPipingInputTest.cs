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
using Core.Common.Base.Data;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.Probabilistics;
using Ringtoets.Common.Data.TestUtil;
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
        public void Constructor_WithoutPipingInput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DerivedPipingInput(null);

            // Assert
            const string expectedMessage = "Cannot create DerivedPipingInput without PipingInput.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, expectedMessage);
        }

        [Test]
        public void Constructor_WithPipingInput_DoesNotThrow()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.1);

            // Call
            TestDelegate call = () => new DerivedPipingInput(input);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void PiezometricHeadExit_ValidInput_SetsParametersForCalculatorAndReturnsPiezometricHead()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var derivedInput = new DerivedPipingInput(input);

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                var piezometricHead = derivedInput.PiezometricHeadExit;

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
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var piezometricHead = derivedInput.PiezometricHeadExit;

            // Assert
            Assert.IsNaN(piezometricHead);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsThicknessCoverageLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.AreEqual(2.0, effectiveThicknessCoverageLayer.Mean.Value);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = null;
            Action call = () => effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void EffectiveThicknessCoverageLayer_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution effectiveThicknessCoverageLayer = null;
            Action call = () => effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            LogNormalDistribution effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void EffectiveThicknessCoverageLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var effectiveThicknessCoverageLayer = derivedInput.EffectiveThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(effectiveThicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsThicknessCoverageLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.AreEqual(2.0, thicknessCoverageLayer.Mean.Value);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessCoverageLayer = null;
            Action call = () => thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessCoverageLayer_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            LogNormalDistribution thicknessCoverageLayer = null;
            Action call = () => thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            LogNormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsMeanExpectedThicknessAquiferLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSoilProfile_ReturnMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.StochasticSoilProfile = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSurfaceLine_ReturnMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = null;
            Action call = () => thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAboveSurfaceLine_ReturnMeanNaN(double deltaAboveSurfaceLine)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToTopmostConsecutiveAquiferLayerThickness()
        {
            // Setup
            double expectedThickness;
            PipingInput input = PipingInputFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-6);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetExitPointSetToNaN_ReturnMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = RoundedDouble.NaN;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LogNormalDistribution thicknessAquiferLayer = null;
            Action call = () => thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_ProfileWithoutAquiferLayer_ReturnMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferUnderSurfaceLine_ReturnMeanExpectedThicknessAquiferLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetSoilProfileSetToNull_ReturnMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.StochasticSoilProfile = null;

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputResultsInZeroAquiferThickness_ReturnMeanNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            LogNormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SurfaceLineHalfWayProfileLayer_ConsecutiveThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(2.0, thicknessAquiferLayer.Mean.Value, 1e-6);
        }

        [Test]
        public void SeepageLength_ValidData_ReturnsSeepageLength()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var seepageLength = derivedInput.SeepageLength;

            // Assert
            Assert.AreEqual(0.5, seepageLength.Mean.Value);
            Assert.AreEqual(0.05, seepageLength.StandardDeviation.Value);
        }

        [Test]
        public void SeepageLength_EntryPointNaN_SeepageLengthNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = RoundedDouble.NaN;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var seepageLength = derivedInput.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.IsNaN(seepageLength.StandardDeviation);
        }

        [Test]
        public void SeepageLength_ExitPointNaN_SeepageLengthNaN()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = RoundedDouble.NaN;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var seepageLength = derivedInput.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.IsNaN(seepageLength.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoSoilProfile_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = null;

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoStochasticSoilProfile_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile = null;

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoSurfaceLine_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.SurfaceLine = null;

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoExitPointL_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.ExitPointL = RoundedDouble.NaN;

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoAquitardLayers_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoAquiferLayers_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = false
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_NoCoverageLayersAboveTopAquiferLayer_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_SingleLayer_ReturnsWithParametersFromLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var random = new Random(21);
            double belowPhreaticLevelMean = 0.1 + random.NextDouble();
            double deviation = random.NextDouble();
            double shift = random.NextDouble();
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", 0.0, new[]
            {
                new PipingSoilLayer(2.5)
                {
                    BelowPhreaticLevelDeviation = deviation,
                    BelowPhreaticLevelShift = shift,
                    BelowPhreaticLevelMean = belowPhreaticLevelMean
                },
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true
                },
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.AreEqual(belowPhreaticLevelMean, result.Mean, result.Mean.GetAccuracy());
            Assert.AreEqual(shift, result.Shift, result.Shift.GetAccuracy());
            Assert.AreEqual(deviation, result.StandardDeviation, result.StandardDeviation.GetAccuracy());
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_MultipleLayersEqualStandardDeviationAndShift_ReturnsWithWeightedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var random = new Random(21);
            var belowPhreaticLevelMeanA = 0.1 + random.NextDouble();
            var belowPhreaticLevelMeanB = 0.1 + random.NextDouble();
            double deviation = random.NextDouble();
            double shift = random.NextDouble();
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(2.5)
                {
                    BelowPhreaticLevelDeviation = deviation,
                    BelowPhreaticLevelShift = shift,
                    BelowPhreaticLevelMean = belowPhreaticLevelMeanA
                },
                new PipingSoilLayer(-0.5)
                {
                    BelowPhreaticLevelDeviation = deviation,
                    BelowPhreaticLevelShift = shift,
                    BelowPhreaticLevelMean = belowPhreaticLevelMeanB
                },
                new PipingSoilLayer(-1.5)
                {
                    IsAquifer = true
                },
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.AreEqual((belowPhreaticLevelMeanA*2.5 + belowPhreaticLevelMeanB*1.0)/3.5, result.Mean, result.Mean.GetAccuracy());
            Assert.AreEqual(shift, result.Shift, result.Shift.GetAccuracy());
            Assert.AreEqual(deviation, result.StandardDeviation, result.StandardDeviation.GetAccuracy());
        }

        [Test]
        [TestCase(0.01, 0)]
        [TestCase(0, 0.01)]
        [TestCase(2, 1)]
        [TestCase(3, -1)]
        [TestCase(-0.01, 0)]
        [TestCase(0, -0.01)]
        [TestCase(-2, 1)]
        [TestCase(-3, -1)]
        public void SaturatedVolumicWeightOfCoverageLayer_MultipleLayersInequalStandardDeviationOrShift_ReturnsNaNValues(double deviationDelta, double shiftDelta)
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var random = new Random(21);
            var belowPhreaticLevelMeanA = 0.1 + random.NextDouble();
            var belowPhreaticLevelMeanB = 0.1 + random.NextDouble();
            double deviation = random.NextDouble();
            double shift = random.NextDouble();
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(2.5)
                {
                    BelowPhreaticLevelDeviation = deviation,
                    BelowPhreaticLevelShift = shift,
                    BelowPhreaticLevelMean = belowPhreaticLevelMeanA
                },
                new PipingSoilLayer(-0.5)
                {
                    BelowPhreaticLevelDeviation = deviation + deviationDelta,
                    BelowPhreaticLevelShift = shift + shiftDelta,
                    BelowPhreaticLevelMean = belowPhreaticLevelMeanB
                },
                new PipingSoilLayer(-1.5)
                {
                    IsAquifer = true
                },
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_MultipleLayersInequalStandardDeviationOrShiftButEqualWhenRounded_ReturnsWithWeightedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var belowPhreaticLevelMeanA = 2.5;
            var belowPhreaticLevelMeanB = 3.4;
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(2.5)
                {
                    BelowPhreaticLevelDeviation = 1.014,
                    BelowPhreaticLevelShift = 1.014,
                    BelowPhreaticLevelMean = belowPhreaticLevelMeanA
                },
                new PipingSoilLayer(-0.5)
                {
                    BelowPhreaticLevelDeviation = 1.006,
                    BelowPhreaticLevelShift = 1.006,
                    BelowPhreaticLevelMean = belowPhreaticLevelMeanB
                },
                new PipingSoilLayer(-1.5)
                {
                    IsAquifer = true
                },
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.AreEqual((belowPhreaticLevelMeanA*2.5 + belowPhreaticLevelMeanB*1.0)/3.5, result.Mean, result.Mean.GetAccuracy());
            Assert.AreEqual((RoundedDouble) 1.01, result.Shift);
            Assert.AreEqual((RoundedDouble) 1.01, result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_OneLayerWithIncorrectShiftMeanCombination_ReturnsNaNValues()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(2.5)
                {
                    BelowPhreaticLevelDeviation = 2.5,
                    BelowPhreaticLevelShift = 1.01,
                    BelowPhreaticLevelMean = 1.00
                },
                new PipingSoilLayer(-1.5)
                {
                    IsAquifer = true
                },
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void SaturatedVolumicWeightOfCoverageLayer_MultipleLayersOneLayerWithIncorrectShiftMeanCombination_ReturnsNaNValues()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(2.5)
                {
                    BelowPhreaticLevelDeviation = 3.5,
                    BelowPhreaticLevelShift = 0.5,
                    BelowPhreaticLevelMean = 1.00
                },
                new PipingSoilLayer(-0.5)
                {
                    BelowPhreaticLevelDeviation = 2.5,
                    BelowPhreaticLevelShift = 1.01,
                    BelowPhreaticLevelMean = 1.00
                },
                new PipingSoilLayer(-1.5)
                {
                    IsAquifer = true
                },
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.SaturatedVolumicWeightOfCoverageLayer;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.Shift);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DarcyPermeability_NoSoilProfile_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = null;

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DarcyPermeability_NoStochasticSoilProfile_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile = null;

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DarcyPermeability_NoSurfaceLine_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.SurfaceLine = null;

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DarcyPermeability_NoExitPointL_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.ExitPointL = RoundedDouble.NaN;

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DarcyPermeability_NoAquiferLayers_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DarcyPermeability_SingleLayerWithIncorrectMean_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", 0.0, new[]
            {
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true,
                    PermeabilityDeviation = 0.3,
                    PermeabilityMean = 0
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DarcyPermeability_MultiplelayersWithOneIncorrectLayerMean_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", 0.0, new[]
            {
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true,
                    PermeabilityDeviation = 0.3,
                    PermeabilityMean = 0
                },
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = true,
                    PermeabilityDeviation = 0.3,
                    PermeabilityMean = 2.4
                },
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DarcyPermeability_MultipleAquiferLayersWithSameVariation_ReturnsWithWeightedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var random = new Random(21);
            double mean = 0.1 + random.NextDouble();
            double deviation = mean/2;
            double mean2 = 0.1 + random.NextDouble();
            double deviation2 = mean2/2;
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", 0.0, new[]
            {
                new PipingSoilLayer(0.5)
                {
                    IsAquifer = true,
                    PermeabilityDeviation = deviation,
                    PermeabilityMean = mean
                },
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = true,
                    PermeabilityDeviation = deviation2,
                    PermeabilityMean = mean2
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            var weightedMean = (mean*0.5 + mean2)/1.5;
            Assert.AreEqual(weightedMean, result.Mean, result.Mean.GetAccuracy());
            Assert.AreEqual(weightedMean/2, result.StandardDeviation, result.StandardDeviation.GetAccuracy());
        }

        [Test]
        public void DarcyPermeability_SingleAquiferLayerWithRandomMeanAndDeviation_ReturnsWithWeightedMean()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var random = new Random(21);
            var permeabilityMean = 0.1 + random.NextDouble();
            var permeabilityDeviation = random.NextDouble();
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true,
                    PermeabilityMean = permeabilityMean,
                    PermeabilityDeviation = permeabilityDeviation
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            var expectedMean = new RoundedDouble(6, permeabilityMean);
            var expectedDeviation = new RoundedDouble(6, permeabilityDeviation);
            Assert.AreEqual(expectedMean, result.Mean);
            Assert.AreEqual(expectedDeviation, result.StandardDeviation, result.StandardDeviation.GetAccuracy());
        }

        [Test]
        public void DarcyPermeability_MultipleAquiferLayersWithRandomMeanAndDeviation_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var permeabilityMean = 0.5;
            var permeabilityDeviation = 0.2;
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true,
                    PermeabilityMean = permeabilityMean,
                    PermeabilityDeviation = permeabilityDeviation
                },
                new PipingSoilLayer(0.0)
                {
                    IsAquifer = true,
                    PermeabilityMean = 12.5,
                    PermeabilityDeviation = 2.3
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DarcyPermeability;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DiameterD70_NoSoilProfile_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = null;

            // Call
            var result = derivedInput.DiameterD70;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DiameterD70_NoStochasticSoilProfile_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile = null;

            // Call
            var result = derivedInput.DiameterD70;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DiameterD70_NoSurfaceLine_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.SurfaceLine = null;

            // Call
            var result = derivedInput.DiameterD70;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DiameterD70_NoExitPointL_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.ExitPointL = RoundedDouble.NaN;

            // Call
            var result = derivedInput.DiameterD70;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DiameterD70_NoAquiferLayers_ReturnsNaNForParameters()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(1.0)
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DiameterD70;

            // Assert
            Assert.IsNaN(result.Mean);
            Assert.IsNaN(result.StandardDeviation);
        }

        [Test]
        public void DiameterD70_SingleAquiferLayers_ReturnsWithParametersFromLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var random = new Random(21);
            var diameterD70Mean = 0.1 + random.NextDouble();
            var diameterD70Deviation = random.NextDouble();
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true,
                    DiameterD70Mean = diameterD70Mean,
                    DiameterD70Deviation = diameterD70Deviation
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DiameterD70;

            // Assert
            Assert.AreEqual(diameterD70Mean, result.Mean, result.Mean.GetAccuracy());
            Assert.AreEqual(diameterD70Deviation, result.StandardDeviation, result.StandardDeviation.GetAccuracy());
        }

        [Test]
        public void DiameterD70_MultipleAquiferLayers_ReturnsWithParametersFromTopmostLayer()
        {
            // Setup
            PipingInput input = PipingInputFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            var diameterD70Mean = 0.5;
            var diameterD70Deviation = 0.2;
            input.StochasticSoilProfile.SoilProfile = new PipingSoilProfile("", -2.0, new[]
            {
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true,
                    DiameterD70Mean = diameterD70Mean,
                    DiameterD70Deviation = diameterD70Deviation
                },
                new PipingSoilLayer(0.0)
                {
                    IsAquifer = true,
                    DiameterD70Mean = 12.5,
                    DiameterD70Deviation = 2.3
                }
            }, SoilProfileType.SoilProfile1D, 0);

            // Call
            var result = derivedInput.DiameterD70;

            // Assert
            Assert.AreEqual(diameterD70Mean, result.Mean, result.Mean.GetAccuracy());
            Assert.AreEqual(diameterD70Deviation, result.StandardDeviation, result.StandardDeviation.GetAccuracy());
        }
    }
}