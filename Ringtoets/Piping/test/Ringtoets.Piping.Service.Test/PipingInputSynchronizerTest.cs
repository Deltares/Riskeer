using System;
using Core.Common.Base.Data;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Calculation;
using Ringtoets.Piping.Calculation.TestUtil.SubCalculator;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Service.TestUtil;

namespace Ringtoets.Piping.Service.Test
{
    [TestFixture]
    public class PipingInputSynchronizerTest
    {
        [Test]
        public void Create_WithoutPipingInput_ArgumentNullException()
        {
            // Call
            TestDelegate call = () => PipingInputSynchronizer.Synchronize(null);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentNullException>(call, "Cannot create PipingInputSynchronizer without PipingInput.");
        }

        [Test]
        public void Create_WithPipingInput_InputDataSynchronized()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(1.0, 1.1);
            double randomLevel = new Random(22).NextDouble();
            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0, 0)
            {
                DesignWaterLevel = randomLevel
            };

            // Call
            PipingInputSynchronizer.Synchronize(input);

            // Assert
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean.Value);
            Assert.AreEqual(1.1, input.ThicknessCoverageLayer.Mean.Value);
            Assert.AreEqual(0.5, input.SeepageLength.Mean.Value);
            Assert.AreEqual(new RoundedDouble(2, randomLevel), input.AssessmentLevel);
            Assert.IsFalse(double.IsNaN(input.PiezometricHeadExit));
        }

        [Test]
        public void NotifyObservers_InputHasNewHydraulicBoundaryLocationSet_AssessmentLevelUpdated()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            PipingInputSynchronizer.Synchronize(input);

            double testLevel = new Random(21).NextDouble();

            input.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(0, string.Empty, 0.0, 0.0)
            {
                DesignWaterLevel = testLevel
            };

            // Call
            input.NotifyObservers();

            // Assert
            Assert.AreEqual(new RoundedDouble(2, testLevel), input.AssessmentLevel);
        }

        [Test]
        public void NotifyObservers_InputWithoutHydraulicBoundaryLocationSet_AssessmentLevelSetToNaN()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            PipingInputSynchronizer.Synchronize(input);

            input.HydraulicBoundaryLocation = null;

            // Call
            input.NotifyObservers();

            // Assert
            Assert.IsNaN(input.AssessmentLevel);
        }

        [Test]
        public void NotifyObservers_ValidInput_SetsParametersForCalculatorAndSetPiezometricHead()
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());
            PipingInputSynchronizer.Synchronize(input);

            using (new PipingCalculationServiceConfig())
            {
                // Call
                input.NotifyObservers();

                // Assert
                var result = input.PiezometricHeadExit;
                Assert.AreEqual(2, result.NumberOfDecimalPlaces);
                Assert.IsFalse(double.IsNaN(result));

                var factory = (TestPipingSubCalculatorFactory)PipingCalculationService.SubCalculatorFactory;
                var piezometricHeadAtExitCalculator = factory.LastCreatedPiezometricHeadAtExitCalculator;

                Assert.AreEqual(input.AssessmentLevel.Value, piezometricHeadAtExitCalculator.HRiver);
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetPhreaticLevelExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.PhiPolder,
                                GetAccuracy(input.PhreaticLevelExit.Mean));
                Assert.AreEqual(PipingSemiProbabilisticDesignValueFactory.GetDampingFactorExit(input).GetDesignValue(), piezometricHeadAtExitCalculator.RExit,
                                GetAccuracy(input.DampingFactorExit.Mean));
            }
        }

        [Test]
        public void NotifyObservers_InputWithAssessmentLevelMissing_PiezometricHeadSetToNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(1.0, 1.0);
            input.AssessmentLevel = (RoundedDouble) double.NaN;
            PipingInputSynchronizer.Synchronize(input);

            // Call
            input.NotifyObservers();

            // Assert
            Assert.IsNaN(input.PiezometricHeadExit);
        }

        [Test]
        public void GivenSynchronizerBoundToInputOutOfScope_WhenGarbageCollected_InputNotAlive()
        {
            // Given
            WeakReference input;

            {
                input = new WeakReference(new PipingInput(new GeneralPipingInput()));
                PipingInputSynchronizer.Synchronize(input.Target as PipingInput);
            }

            // When
            GC.Collect();
            GC.WaitForFullGCComplete();

            // Then
            Assert.IsFalse(input.IsAlive);
        }

        [Test]
        public void NotifyObservers_SoilProfileSingleAquiferAndCoverageUnderSurfaceLine_MeansSet()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            PipingInputSynchronizer.Synchronize(input);

            // Call
            input.NotifyObservers();

            // Assert
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean.Value);
            Assert.AreEqual(2.0, input.ThicknessCoverageLayer.Mean.Value);
        }

        [Test]
        public void NotifyObservers_InputWithoutSoilProfile_MeansSetToNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;
            PipingInputSynchronizer.Synchronize(input);

            // Call
            input.NotifyObservers();

            // Assert
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void NotifyObservers_InputWithoutSurfaceLine_MeansSetToNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;
            PipingInputSynchronizer.Synchronize(input);

            // Call
            Action call = () => input.NotifyObservers();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void NotifyObservers_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessCoverageLayerNaNAndLog(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);
            PipingInputSynchronizer.Synchronize(input);

            // Call
            Action call = () => input.NotifyObservers();

            // Assert
            var messages = new[]
            {
                Properties.Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
                Properties.Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 2);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void NotifyObservers_SoilProfileMultipleAquiferUnderSurfaceLine_AquiferMeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            PipingInputSynchronizer.Synchronize(input);

            // Call
            input.NotifyObservers();

            // Assert
            Assert.AreEqual(expectedThickness, input.ThicknessAquiferLayer.Mean, 1e-8);
        }

        [Test]
        public void NotifyObservers_MeanSetExitPointSetToNaN_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)double.NaN;
            PipingInputSynchronizer.Synchronize(input);

            // Call
            Action call = () => input.NotifyObservers();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void NotifyObservers_MeanSetExitPointSetBeyondSurfaceLine_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = (RoundedDouble)3.0;
            PipingInputSynchronizer.Synchronize(input);

            // Call
            Action call = () => input.NotifyObservers();

            // Assert
            var messages = new[]
            {
                Properties.Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
                Properties.Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 2);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void NotifyObservers_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            PipingInputSynchronizer.Synchronize(input);
            input.ThicknessCoverageLayer.Mean = new RoundedDouble(2, new Random(21).NextDouble() + 1);

            input.SoilProfile = null;

            // Call
            Action call = () => input.NotifyObservers();

            // Assert
            TestHelper.AssertLogMessagesCount(call, 0);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void NotifyObservers_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            PipingInputSynchronizer.Synchronize(input);
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.NotifyObservers();

            // Assert
            var messages = new[]
            {
                Properties.Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer,
                Properties.Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer
            };
            TestHelper.AssertLogMessagesAreGenerated(call, messages, 2);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void NotifyObservers_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            PipingInputSynchronizer.Synchronize(input);

            // Call
            input.NotifyObservers();

            // Assert
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean.Value);
        }

        [Test]
        public void NotifyObservers_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);
            PipingInputSynchronizer.Synchronize(input);

            // Call
            input.NotifyObservers();

            // Assert
            Assert.AreEqual(expectedThickness, input.ThicknessAquiferLayer.Mean, 1e-8);
        }

        [Test]
        public void NotifyObservers_MeanSetSoilProfileSetToNull_ThicknessAquiferLayerNaN()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            PipingInputSynchronizer.Synchronize(input);

            input.SoilProfile = null;

            // Call
            input.NotifyObservers();

            // Assert
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void NotifyObservers_InputResultsInZeroAquiferThickness_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            PipingInputSynchronizer.Synchronize(input);
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
            });

            // Call
            Action call = () => input.NotifyObservers();

            // Assert
            var message = Properties.Resources.PipingInputSynchronizer_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer;
            TestHelper.AssertLogMessageIsGenerated(call, message, 1);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void NotifyObservers_InputResultsInZeroCoverageThickness_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            PipingInputSynchronizer.Synchronize(input);
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
            });

            // Call
            Action call = () => input.NotifyObservers();

            // Assert
            var message = Properties.Resources.PipingInputSynchronizer_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer;            
            TestHelper.AssertLogMessageIsGenerated(call, message, 1);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void NotifyObservers_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            PipingInputSynchronizer.Synchronize(input);
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
            });

            // Call
            input.NotifyObservers();

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
            });

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
            });
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
            });
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