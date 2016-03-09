using System;
using Core.Common.Base.Geometry;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Extensions;
using Ringtoets.Piping.Forms.Properties;

namespace Ringtoets.Piping.Forms.Test.Extensions
{
    [TestFixture]
    public class PipingInputExtensionsTest
    {
        
        [Test]
        public void SetSurfaceLine_WithDikeToeDikeSideAndDikeToeRiverSide_SetsExitPointLAndSeePageLength()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            var firstPointX = 1.0;
            var secondPointX = 4.0;
            var point1 = new Point3D(firstPointX, 0.0, 2.0);
            var point2 = new Point3D(secondPointX, 0.0, 1.8);
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2,
            });
            surfaceLine.SetDikeToeAtRiverAt(point1);
            surfaceLine.SetDikeToeAtPolderAt(point2);

            // Call
            inputParameters.SetSurfaceLine(surfaceLine);

            // Assert
            Assert.AreEqual(secondPointX - firstPointX, inputParameters.SeepageLength.Mean);
            Assert.AreEqual(secondPointX - firstPointX, inputParameters.ExitPointL);
        }

        [Test]
        public void SetSurfaceLine_Null_SetsExitPointLAndSeePageLengthMeanToNaN()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            var firstPointX = 1.0;
            var secondPointX = 4.0;
            var point1 = new Point3D(firstPointX, 0.0, 2.0);
            var point2 = new Point3D(secondPointX, 0.0, 1.8);
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2,
            });
            surfaceLine.SetDikeToeAtRiverAt(point1);
            surfaceLine.SetDikeToeAtPolderAt(point2);

            inputParameters.SetSurfaceLine(surfaceLine);

            // Call
            inputParameters.SetSurfaceLine(null);

            // Assert
            Assert.IsNaN(inputParameters.SeepageLength.Mean);
            Assert.IsNaN(inputParameters.ExitPointL);
        }

        [Test]
        public void SetSurfaceLine_WithoutDikeToeDikeSideAndDikeToeRiverSide_ExitPointAtEndAndSeePageLengthIsLengthInX()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            var firstPointX = 1.0;
            var secondPointX = 4.0;
            var point1 = new Point3D(firstPointX, 0.0, 2.0);
            var point2 = new Point3D(secondPointX, 0.0, 1.8);
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2
            });

            // Call
            inputParameters.SetSurfaceLine(surfaceLine);

            // Assert
            Assert.AreEqual(secondPointX - firstPointX, inputParameters.SeepageLength.Mean);
            Assert.AreEqual(secondPointX - firstPointX, inputParameters.ExitPointL);
        }

        [Test]
        public void SetSurfaceLine_WithoutDikeToeDikeSide_ExitPointSetSeePageLengthStartToExitPointInX()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            var firstPointX = 1.0;
            var secondPointX = 3.0;
            var thirdPointX = 5.0;
            var point1 = new Point3D(firstPointX, 0.0, 2.0);
            var point2 = new Point3D(secondPointX, 0.0, 1.8);
            var point3 = new Point3D(thirdPointX, 0.0, 1.8);
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2,
                point3
            });
            surfaceLine.SetDikeToeAtPolderAt(point2);

            // Call
            inputParameters.SetSurfaceLine(surfaceLine);

            // Assert
            Assert.AreEqual(2.0, inputParameters.SeepageLength.Mean);
            Assert.AreEqual(0.2, inputParameters.SeepageLength.StandardDeviation);
            Assert.AreEqual(secondPointX - firstPointX, inputParameters.ExitPointL);
        }

        [Test]
        public void SetSurfaceLine_WithoutDikeToeRiverSide_ExitPointAtEndSeePageLengthLessThanLengthInX()
        {
            // Setup
            var inputParameters = new PipingInput(new GeneralPipingInput());
            RingtoetsPipingSurfaceLine surfaceLine = new RingtoetsPipingSurfaceLine();
            var firstPointX = 1.0;
            var secondPointX = 3.0;
            var thirdPointX = 5.0;
            var point1 = new Point3D(firstPointX, 0.0, 2.0);
            var point2 = new Point3D(secondPointX, 0.0, 1.8);
            var point3 = new Point3D(thirdPointX, 0.0, 1.8);
            surfaceLine.SetGeometry(new[]
            {
                point1,
                point2,
                point3
            });
            surfaceLine.SetDikeToeAtRiverAt(point2);

            // Call
            inputParameters.SetSurfaceLine(surfaceLine);

            // Assert
            Assert.AreEqual(2.0, inputParameters.SeepageLength.Mean);
            Assert.AreEqual(0.2, inputParameters.SeepageLength.StandardDeviation);
            Assert.AreEqual(thirdPointX - firstPointX, inputParameters.ExitPointL);
        }

        [Test]
        [TestCase(-1e-6)]
        [TestCase(-6)]
        public void SetEntryPointL_ValueLessThanZero_ThrowsArgumentOutOfRangeException(double entryPoint)
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            // Call
            TestDelegate test = () => input.SetEntryPointL(entryPoint);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, "De waarde voor het L-coördinaat van het intredepunt mag niet kleiner zijn dan 0.");
        }

        [Test]
        [TestCase(2, 0, 2)]
        [TestCase(3, 0, 3)]
        [TestCase(1 + 1e-6, 1, 1e-6)]
        [TestCase(2, 2 - 1e-6, 1e-6)]
        public void SetEntryPointL_ExitPointAndSeepageLengthSet_UpdatesSeepageLength(double exitPoint, double entryPoint, double seepageLength)
        {
            // Setup
            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var input = new PipingInput(new GeneralPipingInput());
            input.SetSurfaceLine(surfaceLine);
            input.SetExitPointL(exitPoint);

            // Call
            input.SetEntryPointL(entryPoint);

            // Assert
            Assert.AreEqual(exitPoint, input.ExitPointL);
            Assert.AreEqual(seepageLength, input.SeepageLength.Mean, 1e-6);
        }

        [Test]
        public void SetEntryPointL_SetResultInInvalidSeePage_SeepageSetToNaN()
        {
            // Setup
            var surfaceLine = ValidSurfaceLine(0.0, 4.0);

            var l = 2.0;
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                ExitPointL = l
            };

            // Call
            input.SetEntryPointL(l);

            // Assert
            Assert.IsNaN(input.SeepageLength.Mean);
            Assert.IsNaN(input.SeepageLength.StandardDeviation);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1e-6)]
        [TestCase(-6)]
        public void SetExitPointL_ValueZeroOrLess_ThrowsArgumentOutOfRangeException(double exitPoint)
        {
            // Setup
            var input = new PipingInput(new GeneralPipingInput());

            // Call
            TestDelegate test = () => input.SetExitPointL(exitPoint);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentOutOfRangeException>(test, "De waarde voor het L-coördinaat van het uittredepunt moet groter zijn dan 0.");
        }

        [Test]
        [TestCase(1, 2, 1)]
        [TestCase(0, 2, 2)]
        [TestCase(0, 0.5, 0.5)]
        [TestCase(3 - 1e-6, 3, 1e-6)]
        [TestCase(0.5, 0.5 + 1e-6, 1e-6)]
        public void SetExitPointL_ExitPointAndSeepageLengthSet_UpdatesSeepageLength(double entryPoint, double exitPoint, double seepageLength)
        {
            // Setup
            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var input = new PipingInput(new GeneralPipingInput());
            input.SetSurfaceLine(surfaceLine);
            input.SetEntryPointL(entryPoint);

            // Call
            input.SetExitPointL(exitPoint);

            // Assert
            Assert.AreEqual(exitPoint, input.ExitPointL);
            Assert.AreEqual(seepageLength, input.SeepageLength.Mean, 1e-6);
        }

        [Test]
        public void SetExitPointL_SetResultInInvalidSeePage_SeePageSetToNaN()
        {
            // Setup
            var exitPointOld = 4.0;
            var surfaceLine = ValidSurfaceLine(0.0, exitPointOld);
            var input = new PipingInput(new GeneralPipingInput());
            input.SetSurfaceLine(surfaceLine);
            var entryPointL = 1.0;
            input.SetEntryPointL(entryPointL);

            // Call
            input.SetExitPointL(entryPointL);

            // Assert
            Assert.IsNaN(input.SeepageLength.Mean);
            Assert.IsNaN(input.SeepageLength.StandardDeviation);
        }

        [Test]
        public void SetSurfaceLine_InputWithoutEntryPointL_EntryPointLAtStartOfSurfaceLine()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.EntryPointL = double.NaN;

            // Call
            input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            Assert.AreEqual(0.0, input.EntryPointL);
        }

        #region thickness of the coverage layer

        [Test]
        public void SetExitPointL_CompleteInput_ThicknessUpdated()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();

            // Call
            input.SetExitPointL(input.ExitPointL);

            // Assert
            var thickness = input.ThicknessCoverageLayer.Mean;
            Assert.AreEqual(1.0, thickness);
        }

        [Test]
        public void SetExitPointL_InputWithoutSurfaceLine_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            Action call = () => input.SetExitPointL(0.5);
            
            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetExitPointL_InputWithoutSoilProfile_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;

            // Call
            Action call = () => input.SetExitPointL(0.5);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetExitPointL_MeanSetExitPointSetToNaN_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(true);

            // Call
            Action call = () => input.SetExitPointL(double.NaN);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetExitPointL_AquiferLayerThicknessZero_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.SetExitPointL(input.ExitPointL); 

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetExitPointL_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });


            // Call
            Action call = () => input.SetExitPointL(input.ExitPointL);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_CompleteInput_ThicknessUpdated()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();

            // Call
            input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            var thickness = input.ThicknessCoverageLayer.Mean;
            Assert.AreEqual(1.0, thickness);
        }

        [Test]
        public void SetSurfaceLine_InputWithoutExitPointL_ExitPointLAtEndOfSurfaceLineThicknessUpdated()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = double.NaN;

            // Call
            input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            Assert.AreEqual(1.0, input.ExitPointL);
            Assert.AreEqual(1.0, input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_InputWithoutSoilProfile_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;

            // Call
            Action call = () => input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_MeanSetSurfacelineSetToNull_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(true);

            // Call
            Action call = () => input.SetSurfaceLine(null);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_AquiferLayerThicknessZero_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_CompleteInput_ThicknessCoverageLayerUpdated()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();

            // Call
            input.SetSoilProfile(input.SoilProfile);

            // Assert
            var thickness = input.ThicknessCoverageLayer.Mean;
            Assert.AreEqual(1.0, thickness);
        }

        [Test]
        public void SetSoilProfile_InputWithoutSurfaceLine_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            Action call = () => input.SetSoilProfile(input.SoilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_InputWithoutExitPointL_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = double.NaN;

            // Call
            Action call = () => input.SetSoilProfile(input.SoilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_MeanSetSoilProfileSetToNull_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(true);

            // Call
            Action call = () => input.SetSoilProfile(null);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_AquiferLayerThicknessZero_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.SetSoilProfile(input.SoilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_ProfileWithoutAquiferLayer_ThicknessCoverageLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.SetSoilProfile(input.SoilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        #endregion

        #region thickness of the aquifer layer

        [Test]
        public void SetExitPointL_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();

            // Call
            input.SetExitPointL(input.ExitPointL);

            // Assert
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetExitPointL_WithoutSoilProfile_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;

            // Call
            Action call = () => input.SetExitPointL(input.ExitPointL);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetExitPointL_WithoutSurfaceLine_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            Action call = () => input.SetExitPointL(input.ExitPointL);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void SetExitPointL_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessAquiferLayerNaNAndLog(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Call
            Action call = () => input.SetExitPointL(input.ExitPointL);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetExitPointL_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            input.SetExitPointL(input.ExitPointL);

            // Assert
            Assert.AreEqual(expectedThickness, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetExitPointL_MeanSetExitPointSetToNaN_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(true);

            // Call
            Action call = () => input.SetExitPointL(double.NaN);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.");
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetExitPointL_InputResultsInZeroThickness_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(0.0)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.SetExitPointL(input.ExitPointL);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetExitPointL_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.5)
                {
                    IsAquifer = true
                }
            });

            // Call
            input.SetExitPointL(input.ExitPointL);

            // Assert
            Assert.AreEqual(0.5, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();

            // Call
            input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_WithExitPointLSetToNaN_ExitPointLAtEndOfSurfaceLineAndThicknessUpdated()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = double.NaN;

            // Call
            input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            Assert.AreEqual(1.0, input.ExitPointL);
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_WithoutSoilProfile_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = null;

            // Call
            Action call = () => input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void SetSurfaceLine_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessAquiferLayerNaNAndLog(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Call
            Action call = () => input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            Assert.AreEqual(expectedThickness, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_MeanSetSurfaceLineSetToNull_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(true);

            // Call
            Action call = () => input.SetSurfaceLine(null);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.");
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_InputResultsInZeroThickness_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(0.0)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.5)
                {
                    IsAquifer = true
                }
            });

            // Call
            input.SetSurfaceLine(input.SurfaceLine);

            // Assert
            Assert.AreEqual(0.5, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_SoilProfileSingleAquiferUnderSurfaceLine_ThicknessAquiferLayerMeanSet()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();

            // Call
            input.SetSoilProfile(input.SoilProfile);

            // Assert
            Assert.AreEqual(1.0, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_WithExitPointLSetToNaN_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.ExitPointL = double.NaN;

            // Call
            Action call = () => input.SetSoilProfile(input.SoilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_WithoutSurfaceLine_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SurfaceLine = null;

            // Call
            Action call = () => input.SetSoilProfile(input.SoilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        [TestCase(1e-6)]
        [TestCase(1)]
        public void SetSoilProfile_SoilProfileSingleAquiferAboveSurfaceLine_ThicknessAquiferLayerNaNAndLog(double deltaAboveSurfaceLine)
        {
            // Setup
            var input = CreateInputWithSingleAquiferLayerAboveSurfaceLine(deltaAboveSurfaceLine);

            // Call
            Action call = () => input.SetSoilProfile(input.SoilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_SoilProfileMultipleAquiferUnderSurfaceLine_MeanSetToTopAquiferThickness()
        {
            // Setup
            double expectedThickness;
            var input = CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out expectedThickness);

            // Call
            input.SetSoilProfile(input.SoilProfile);

            // Assert
            Assert.AreEqual(expectedThickness, input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_MeanSetSoilProfileSetToNull_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer(true);

            // Call
            Action call = () => input.SetSoilProfile(null);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, "Kan de dikte van het watervoerend pakket niet afleiden op basis van de invoer.");
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_InputResultsInZeroThickness_ThicknessAquiferLayerNaNAndLog()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(0.0)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });

            // Call
            Action call = () => input.SetSoilProfile(input.SoilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessAquiferLayer_Cannot_determine_thickness_aquifer_layer);
            Assert.IsNaN(input.ThicknessAquiferLayer.Mean);
        }

        [Test]
        public void SetSoilProfile_SurfaceLineHalfWayProfileLayer_ThicknessSetToLayerHeightUnderSurfaceLine()
        {
            // Setup
            var input = CreateInputWithAquiferAndCoverageLayer();
            input.SoilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(1.5)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.5)
                {
                    IsAquifer = true
                }
            });

            // Call
            input.SetSoilProfile(input.SoilProfile);

            // Assert
            Assert.AreEqual(0.5, input.ThicknessAquiferLayer.Mean);
        }

        #endregion

        #region test input creation helpers

        private static RingtoetsPipingSurfaceLine ValidSurfaceLine(double xMin, double xMax)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(xMin, 0.0, 0.0),
                new Point3D(xMax, 0.0, 1.0)
            });
            return surfaceLine;
        }

        private static PipingInput CreateInputWithAquiferAndCoverageLayer(bool meanSet = false)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, 2.0),
                new Point3D(1.0, 0, 2.0),
            });
            var soilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(1.0)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });
            var input = new PipingInput(new GeneralPipingInput());
            var exitPointL = 0.5;
            if (meanSet)
            {
                input.SetSurfaceLine(surfaceLine);
                input.SetSoilProfile(soilProfile);
                input.SetExitPointL(exitPointL);
            }
            else
            {
                input.SurfaceLine = surfaceLine;
                input.SoilProfile = soilProfile;
                input.ExitPointL = exitPointL;
            }
            return input;
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
                new PipingSoilLayer(surfaceLineTopLevel + deltaAboveSurfaceLine)
                {
                    IsAquifer = false
                },
                new PipingSoilLayer(surfaceLineTopLevel + deltaAboveSurfaceLine + 1)
                {
                    IsAquifer = true
                }
            });
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                ExitPointL = 0.5
            };
            return input;
        }

        private static PipingInput CreateInputWithMultipleAquiferLayersUnderSurfaceLine(out double expectedThickness)
        {
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            var firstAquiferThickness = 1.1;
            var secondAquiferThickness = 2.2;
            var totalAquiferThickness = firstAquiferThickness + secondAquiferThickness;
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0, 0, totalAquiferThickness),
                new Point3D(1.0, 0, totalAquiferThickness),
            });
            var soilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(firstAquiferThickness)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(firstAquiferThickness + secondAquiferThickness)
                {
                    IsAquifer = true
                },
                new PipingSoilLayer(totalAquiferThickness + 1)
                {
                    IsAquifer = false
                }
            });
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                ExitPointL = 0.5
            };
            expectedThickness = secondAquiferThickness;
            return input;
        }
        #endregion

    }
}