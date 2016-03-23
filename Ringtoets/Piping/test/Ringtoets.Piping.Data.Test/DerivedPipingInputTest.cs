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
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data.Probabilistics;
using Ringtoets.Piping.Data.Properties;
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
            var input = CreateInputWithAquiferAndCoverageLayer(1.0, 1.1);

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

                Assert.AreEqual(derivedInput.AssessmentLevel.Value, piezometricHeadAtExitCalculator.HRiver);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                GetAccuracy(input.PhreaticLevelExit.Mean));
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                GetAccuracy(input.DampingFactorExit.Mean));
            }
        }

        [Test]
        public void PiezometricHeadExit_InputWithAssessmentLevelMissing_PiezometricHeadSetToNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);
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
            var input = CreateInputWithAquiferAndCoverageLayer();
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
            var input = CreateInputWithAquiferAndCoverageLayer();
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
            var input = CreateInputWithAquiferAndCoverageLayer();
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
            var input = CreateInputWithAquiferAndCoverageLayer();
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
            var input = CreateInputWithAquiferAndCoverageLayer();
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
            var input = CreateInputWithAquiferAndCoverageLayer();
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
        public void ThicknessCoverageLayer_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaNAndLog(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);
        
            // Call
            LognormalDistribution thicknessCoverageLayer = null;
            Action call = () => thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;
        
            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 1);
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaNAndLog(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LognormalDistribution thicknessAquiferLayer = null;
            Action call = () => thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 1);
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileMultipleAquiferUnderSurfaceLine_AquiferMeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
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
            var input = CreateInputWithAquiferAndCoverageLayer();
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
        public void ThicknessAquiferLayer_MeanSetExitPointSetBeyondSurfaceLine_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) 3.0;
            var derivedInput = new DerivedPipingInput(input);
        
            // Call
            LognormalDistribution thicknessAquiferLayer = null;
            Action call = () => thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;
        
            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 1);
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetExitPointSetBeyondSurfaceLine_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)3.0;
            var derivedInput = new DerivedPipingInput(input);

            // Call
            LognormalDistribution thicknessCoverageLayer = null;
            Action call = () => thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 1);
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
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
        public void ThicknessCoverageLayer_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            }, 0);

            // Call
            LognormalDistribution thicknessCoverageLayer = null;
            Action call = () => thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 1);
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_ProfileWithoutAquiferLayer_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            }, 0);

            // Call
            LognormalDistribution thicknessAquiferLayer = null;
            Action call = () => thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 1);
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
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
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
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
            var input = CreateInputWithAquiferAndCoverageLayer();
            var derivedInput = new DerivedPipingInput(input);

            input.SoilProfile = null;

            // Call
            var thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_InputResultsInZeroAquiferThickness_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
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
            LognormalDistribution thicknessAquiferLayer = null;
            Action call = () => thicknessAquiferLayer = derivedInput.ThicknessAquiferLayer;

            // Assert
            var message = Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer;
            TestHelper.AssertLogMessageIsGenerated(call, message, 1);
            Assert.IsNaN(thicknessAquiferLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
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
            LognormalDistribution thicknessCoverageLayer = null;
            Action call = () => thicknessCoverageLayer = derivedInput.ThicknessCoverageLayer;

            // Assert
            var message = Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer;
            TestHelper.AssertLogMessageIsGenerated(call, message, 1);
            Assert.IsNaN(thicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessAquiferLayer_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
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

        private static PipingInput CreateInputWithAquiferAndCoverageLayer(double thicknessAquiferLayer = 1.0, double thicknessCoverageLayer = 2.0)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, thicknessCoverageLayer),
                new Point3D(1.0, 0, thicknessCoverageLayer)
            });
            var soilProfile = new PipingSoilProfile(String.Empty, -thicknessAquiferLayer, new[]
            {
                new PipingSoilLayer(thicknessCoverageLayer)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(0.0)
                {
                    IsAquifer = true
                }
            }, 0);

            return new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                ExitPointL = (RoundedDouble)0.5
            };
        }

        private static PipingInput CreateInputWithSingleAquiferLayerAboveSurfaceLine(double deltaAboveSurfaceLine)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var surfaceLineTopLevel = 2.0;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, surfaceLineTopLevel),
                new Point3D(1.0, 0, surfaceLineTopLevel),
            });
            var soilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(surfaceLineTopLevel + deltaAboveSurfaceLine + 2)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(surfaceLineTopLevel + deltaAboveSurfaceLine + 1)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(surfaceLineTopLevel + deltaAboveSurfaceLine)
                {
                    IsAquifer = false
                }
            }, 0);
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                ExitPointL = (RoundedDouble)0.5
            };
            return input;
        }

        private static PipingInput CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out double expectedThickness)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 3.3),
                new Point3D(1.0, 0, 3.3),
            });
            var soilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(4.3)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(3.3)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(1.1)
                {
                    IsAquifer = true
                }
            }, 0);
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                ExitPointL = (RoundedDouble)0.5
            };
            expectedThickness = 2.2;
            return input;
        }

        private static double GetAccuracy(RoundedDouble roundedDouble)
        {
            return Math.Pow(10.0, -roundedDouble.NumberOfDecimalPlaces);
        }
    }
}
