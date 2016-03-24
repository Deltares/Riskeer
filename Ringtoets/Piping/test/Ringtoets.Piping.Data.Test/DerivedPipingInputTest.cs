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
using Core.Common.Base.Data;
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
    public class DerivedPipingInputTest
    {
        [Test]
        public void Constructor_WithoutPipingInput_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new DerivedPipingInput(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Cannot create DerivedPipingInput without PipingInput.");
        }

        [Test]
        public void Constructor_WithPipingInput_DoesNotThrow()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.1);

            // Call
            TestDelegate call = () => new DerivedPipingInput(input);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void AssessmentLevel_InputHasNewHydraulicBoundaryLocationSet_AssessmentLevelUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var derivedInput = new DerivedPipingInput(input);
                
            double testLevel = new Random(21).NextDouble();

            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0)
            {
                DesignWaterLevel = testLevel
            };

            // Call
            var calculatedAssesmentLevel = derivedInput.AssessmentLevel;

            // Assert
            Assert.AreEqual(new RoundedDouble(2, testLevel), calculatedAssesmentLevel);
        }

        [Test]
        public void AssessmentLevel_InputWithoutHydraulicBoundaryLocationSet_AssessmentLevelSetToNaN()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var derivedInput = new DerivedPipingInput(input);

            input.HydraulicBoundaryLocation = null;

            // Call
            var calculatedAssesmentLevel = derivedInput.AssessmentLevel;

            // Assert
            Assert.IsNaN(calculatedAssesmentLevel);
        }

        [Test]
        public void AssessmentLevel_HydraulicBoundaryCalculationUncalculated_AssessmentLevelNaN()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var derivedInput = new DerivedPipingInput(input);

            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0);

            // Call
            var calculatedAssesmentLevel = derivedInput.AssessmentLevel;

            // Assert
            Assert.IsNaN(calculatedAssesmentLevel);
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
        
                var factory = (TestPipingSubCalculatorFactory)PipingSubCalculatorFactory.Instance;
                var piezometricHeadAtExitCalculator = factory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(piezometricHeadAtExitCalculator.HRiver, derivedInput.AssessmentLevel.Value, PipingCalculationFactory.GetAccuracy(derivedInput.AssessmentLevel));
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                PipingCalculationFactory.GetAccuracy(input.PhreaticLevelExit.Mean));
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                PipingCalculationFactory.GetAccuracy(input.DampingFactorExit.Mean));
            }
        }

        [Test]
        public void PiezometricHeadExit_InputWithAssessmentLevelMissing_PiezometricHeadSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);
            var derivedInput = new DerivedPipingInput(input);
        
            // Call
            var piezometricHead = derivedInput.PiezometricHeadExit;
        
            // Assert
            Assert.IsNaN(piezometricHead);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsThicknessAquiferLayer()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
        
            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;
        
            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void ThicknessCoverageLayer_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_ReturnsThicknessCoverageLayer()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.AreEqual(2.0, thicknessCoverageLayer.Mean.Value);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;
            var derivedInput = new DerivedPipingInput(input);
        
            // Call
            var thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;
        
            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LognormalDistribution thicknessAquiferLayer = null;
            Action call = () => thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LognormalDistribution thicknessCoverageLayer = null;
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
            var input = PipingCalculationFactory.CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);
        
            // Call
            LognormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;
        
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
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LognormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_AquiferMeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = PipingCalculationFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LognormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-8);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetExitPointSetToNaN_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) double.NaN;
            var derivedInput = new DerivedPipingInput(input);
        
            // Call
            LognormalDistribution thicknessAquiferLayer = null;
            Action call = () => thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;
        
            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetExitPointSetBeyondSurfaceLine_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) 3.0;
            var derivedInput = new DerivedPipingInput(input);
        
            // Call
            LognormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;
        
            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetExitPointSetBeyondSurfaceLine_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)3.0;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LognormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);
            var derivedInput = new DerivedPipingInput(input);

            input.SoilProfile = null;

            // Call
            LognormalDistribution thicknessCoverageLayer = null;
            Action call = () => thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            }, 0);

            // Call
            LognormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_ProfileWithoutAquiferLayer_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            }, 0);

            // Call
            LognormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(1.0, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = PipingCalculationFactory.CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(expectedThickness, thicknessAquiferLayer.Mean, 1e-8);
        }

        [Test]
        public void ThicknessAquiferLayer_MeanSetSoilProfileSetToNull_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.SoilProfile = null;

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputResultsInZeroAquiferThickness_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            LognormalDistribution thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            LognormalDistribution thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
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
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.AreEqual(0.5, thicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void SeepageLength_ValidData_ReturnsSeepageLength()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var seepageLength = derivedInput.SeepageLength;

            // Assert
            Assert.AreEqual(0.5, seepageLength.Mean.Value);
            Assert.AreEqual(0.05, seepageLength.StandardDeviation.Value);
        }

        [Test]
        public void SeepageLength_ExitPointSetBeyondEntryPoint_SeepageLengthNaN()
        {
            // Setup
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) 2;
            input.EntryPointL = (RoundedDouble) 3;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var seepageLength = derivedInput.SeepageLength;

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
            var input = PipingCalculationFactory.CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)double.NaN;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            var seepageLength = derivedInput.SeepageLength;

            // Assert
            Assert.IsNaN(seepageLength.Mean);
            Assert.IsNaN(seepageLength.StandardDeviation);
        }
    }
}