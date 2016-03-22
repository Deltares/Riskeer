using System;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.InputParameterCalculation;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service.TestUtil;

namespace Ringtoets.Piping.Data.Test
{
    [TestFixture]
    public class PipingInputSynchronizerTest
    {
        [Test]
        public void Constructor_WithoutPipingInput_ArgumentNullException()
        {
            // Call
            TestDelegate call = () => new PipingInputSynchronizer(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Cannot create PipingInputSynchronizer without PipingInput.");
        }

        [Test]
        public void Constructor_WithPipingInput_InputDataSynchronized()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(1.0, 1.1);

            // Call
            TestDelegate call = () => new PipingInputSynchronizer(input);

            // Assert
            Assert.DoesNotThrow(call);
        }

        [Test]
        public void Synchronize_InputHasNewHydraulicBoundaryLocationSet_AssessmentLevelUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var synchronizer = new PipingInputSynchronizer(input);

            double testLevel = new Random(21).NextDouble();

            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0)
            {
                DesignWaterLevel = testLevel
            };

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.AreEqual(new RoundedDouble(2, testLevel), input.AssessmentLevel);
        }

        [Test]
        public void Synchronize_InputWithoutHydraulicBoundaryLocationSet_AssessmentLevelSetToNaN()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var synchronizer = new PipingInputSynchronizer(input);

            input.HydraulicBoundaryLocation = null;

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.IsNaN(input.AssessmentLevel);
        }

        [Test]
        public void Synchronize_ValidInput_SetsParametersForCalculatorAndSetPiezometricHead()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            var synchronizer = new PipingInputSynchronizer(input);

            using (new PipingSubCalculatorFactoryConfig())
            {
                // Call
                synchronizer.Synchronize();

                // Assert
                var result = input.PiezometricHeadExit;
                Assert.AreEqual(2, result.NumberOfDecimalPlaces);
                Assert.IsFalse(double.IsNaN(result));

                var factory = (TestPipingSubCalculatorFactory)PipingSubCalculatorFactory.Instance;
                var piezometricHeadAtExitCalculator = factory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(input.AssessmentLevel.Value, piezometricHeadAtExitCalculator.HRiver);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                GetAccuracy(input.PhreaticLevelExit.Mean));
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                GetAccuracy(input.DampingFactorExit.Mean));
            }
        }

        [Test]
        public void Synchronize_InputWithAssessmentLevelMissing_PiezometricHeadSetToNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);
            input.AssessmentLevel = (RoundedDouble) double.NaN;
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.IsNaN(input.PiezometricHeadExit);
        }

        [Test]
        public void Synchronize_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_MeansSet()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean.Value);
            Assert.AreEqual(2.0, input.ThicknessCoverageLayer.Mean.Value);
        }

        [Test]
        public void Synchronize_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void Synchronize_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            Action call = () => synchronizer.Synchronize();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void Synchronize_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaNAndLog(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            Action call = () => synchronizer.Synchronize();

            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
                Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 2);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void Synchronize_SoilProfileMultipleAquiferUnderSurfaceLine_AquiferMeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.AreEqual(expectedThickness, input.ThicknessAquiferLayer.Mean, 1e-8);
        }

        [Test]
        public void Synchronize_MeanSetExitPointSetToNaN_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) double.NaN;
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            Action call = () => synchronizer.Synchronize();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void Synchronize_MeanSetExitPointSetBeyondSurfaceLine_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble) 3.0;
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            Action call = () => synchronizer.Synchronize();

            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
                Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 2);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void Synchronize_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var synchronizer = new PipingInputSynchronizer(input);
            input.ThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);

            input.SoilProfile = null;

            // Call
            Action call = () => synchronizer.Synchronize();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void Synchronize_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var synchronizer = new PipingInputSynchronizer(input);
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            }, 0);

            // Call
            Action call = () => synchronizer.Synchronize();

            // Assert
            var messages = new[]
            {
                Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
                Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 2);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void Synchronize_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void Synchronize_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            var synchronizer = new PipingInputSynchronizer(input);

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.AreEqual(expectedThickness, input.ThicknessAquiferLayer.Mean, 1e-8);
        }

        [Test]
        public void Synchronize_MeanSetSoilProfileSetToNull_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var synchronizer = new PipingInputSynchronizer(input);

            input.SoilProfile = null;

            // Call
            synchronizer.Synchronize();

            // Assert
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void Synchronize_InputResultsInZeroAquiferThickness_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var synchronizer = new PipingInputSynchronizer(input);
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
            Action call = () => synchronizer.Synchronize();

            // Assert
            var message = Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer;
            TestHelper.AssertLogMessageIsGenerated(call, message, 1);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void Synchronize_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var synchronizer = new PipingInputSynchronizer(input);
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
            Action call = () => synchronizer.Synchronize();

            // Assert
            var message = Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer;
            TestHelper.AssertLogMessageIsGenerated(call, message, 1);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void Synchronize_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            var synchronizer = new PipingInputSynchronizer(input);
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
            synchronizer.Synchronize();

            // Assert
            Assert.AreEqual(0.5, input.ThicknessAquiferLayer.Mean.Value);
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
                ExitPointL = (RoundedDouble) 0.5
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
                ExitPointL = (RoundedDouble) 0.5
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
                ExitPointL = (RoundedDouble) 0.5
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