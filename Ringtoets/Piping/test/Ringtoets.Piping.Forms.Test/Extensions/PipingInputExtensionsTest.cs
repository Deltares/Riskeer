﻿using System;
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
        [TestCase(2, 0, 2)]
        [TestCase(2, -2, 4)]
        [TestCase(0.5, -3.5, 4)]
        [TestCase(1e-6, -(4 - 1e-6), 4)]
        [TestCase(3 + 1e-6, 3, 1e-6)]
        [TestCase(0.5, 0.5 - 1e-6, 1e-6)]
        public void SetEntryPointL_ExitPointAndSeepageLengthSet_UpdatesSeepageLength(double exitPoint, double entryPoint, double seepageLength)
        {
            // Setup
            var random = new Random(22);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                ExitPointL = exitPoint
            };

            input.SetEntryPointL(entryPoint);

            // Call & Assert
            Assert.AreEqual(exitPoint, input.ExitPointL);
            Assert.AreEqual(seepageLength, input.SeepageLength.Mean, 1e-6);
        }

        [Test]
        [TestCase(3, 2, 1)]
        [TestCase(4, 2, 2)]
        [TestCase(4, 0.5, 0.5)]
        [TestCase(1 + 1e-6, 3, 1e-6)]
        [TestCase(3.5, 0.5 + 1e-6, 1e-6)]
        public void SetExitPointL_ExitPointAndSeepageLengthSet_UpdatesSeepageLength(double seepageLength, double exitPoint, double newSeepageLength)
        {
            // Setup
            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var input = new PipingInput(new GeneralPipingInput());
            input.SetSurfaceLine(surfaceLine);

            input.SeepageLength.Mean = seepageLength; // L-coordinate of entry point at 4.0 - seepageLength

            // Call
            input.SetExitPointL(exitPoint);

            // Assert
            Assert.AreEqual(exitPoint, input.ExitPointL);
            Assert.AreEqual(newSeepageLength, input.SeepageLength.Mean, 1e-6);
        }

        [Test]
        public void SetEntryPointL_SetResultInInvalidSeePage_ThrowsArgumentException()
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
            TestDelegate test = () => input.SetEntryPointL(l);

            // Assert
            var message = string.Format(Resources.PipingInputContextProperties_EntryPointL_Value_0_results_in_invalid_seepage_length, l);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        public void SetExitPointL_SetResultInInvalidSeePage_ThrowsArgumentException()
        {
            // Setup
            var exitPointOld = 4.0;
            var seepageLength = 3.0;
            var surfaceLine = ValidSurfaceLine(0.0, exitPointOld);
            var input = new PipingInput(new GeneralPipingInput());
            input.SetSurfaceLine(surfaceLine);
            input.SeepageLength.Mean = seepageLength;
            var entryPointL = exitPointOld - seepageLength;

            // Call
            TestDelegate test = () =>
            {
                input.SetExitPointL(entryPointL);
            };

            // Assert
            var message = string.Format(Resources.PipingInputContextProperties_ExitPointL_Value_0_results_in_invalid_seepage_length, entryPointL);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void ThicknessCoverageLayer_WithMissingInput_ThicknessSetToNaNAndLog(int inputIndexMissing)
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0,0,2.0), 
                new Point3D(1.0,0,2.0), 
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

            // Call
            Action call = null;
            if (inputIndexMissing != 0)
            {
                call = () => input.SetSurfaceLine(surfaceLine);
            }
            if (inputIndexMissing != 1)
            {
                call = () => input.SetSoilProfile(soilProfile);
            }
            
            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputCausesPipingCalculatorException_ThicknessSetToNaNAndLog()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0,0,2.0), 
                new Point3D(1.0,0,2.0), 
            });
            var soilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = false
                }
            });
            var input = new PipingInput(new GeneralPipingInput())
            {
                SoilProfile = soilProfile
            };

            // Call
            Action call = () => input.SetSurfaceLine(surfaceLine);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        [TestCase(0)]
        [TestCase(1)]
        public void ThicknessCoverageLayer_HadThicknessCoverageLayerChangeToMissingInput_ThicknessSetToNaNAndLog(int inputIndexMissing)
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0,0,2.0), 
                new Point3D(1.0,0,2.0), 
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
            input.SetSurfaceLine(surfaceLine);
            input.SetSoilProfile(soilProfile);

            // Call
            Action call = null;
            if (inputIndexMissing == 0)
            {
                call = () => input.SetSurfaceLine(null);
            }
            if (inputIndexMissing == 1)
            {
                call = () => input.SetSoilProfile(null);
            }

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void ThicknessCoverageLayer_InputResultsInZeroThickness_ThicknessSetToNaNAndLog()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0,0,2.0), 
                new Point3D(1.0,0,2.0), 
            });
            var soilProfile = new PipingSoilProfile(String.Empty, 0, new[]
            {
                new PipingSoilLayer(2.0)
                {
                    IsAquifer = true
                }
            });

            var input = new PipingInput(new GeneralPipingInput());

            input.SetSurfaceLine(surfaceLine);
            input.SetExitPointL(0.5);

            // Call
            input.SetSoilProfile(soilProfile);

            // Call
            Action call = () => input.SetSoilProfile(soilProfile);

            // Assert
            TestHelper.AssertLogMessageIsGenerated(call, Resources.PipingInputExtensions_UpdateThicknessCoverageLayer_Cannot_determine_thickness_coverage_layer);
            Assert.IsNaN(input.ThicknessCoverageLayer.Mean);
        }

        [Test]
        public void SetSurfaceLine_WithSoilProfileAndExitPointL_ThicknessUpdated()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0,0,2.0), 
                new Point3D(1.0,0,2.0), 
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
            var input = new PipingInput(new GeneralPipingInput())
            {
                SoilProfile = soilProfile,
                ExitPointL = 0.5
            };

            var previousResult = input.ThicknessCoverageLayer.Mean;

            input.SetSurfaceLine(surfaceLine);

            // Call
            var result = input.ThicknessCoverageLayer.Mean;

            // Assert
            Assert.AreSame(surfaceLine, input.SurfaceLine);
            Assert.AreNotEqual(previousResult, result);
            Assert.AreEqual(1.0, result);
        }

        [Test]
        public void SetSoilProfile_WithSurfaceLineAndExitPointL_ThicknessUpdated()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new []
            {
                new Point3D(0,0,2.0), 
                new Point3D(1.0,0,2.0), 
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
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                ExitPointL = 0.5
            };

            var previousResult = input.ThicknessCoverageLayer.Mean;

            input.SetSoilProfile(soilProfile);

            // Call
            var result = input.ThicknessCoverageLayer.Mean;

            // Assert
            Assert.AreSame(soilProfile, input.SoilProfile);
            Assert.AreNotEqual(previousResult, result);
            Assert.AreEqual(1.0, result);
        }

        [Test]
        public void SetExitPointL_WithSoilProfileAndExitPointL_ThicknessUpdated()
        {
            // Setup
            var surfaceLine = new RingtoetsPipingSurfaceLine();
            surfaceLine.SetGeometry(new[]
            {
                new Point3D(0,0,2.0), 
                new Point3D(1.0,0,2.0), 
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
            var input = new PipingInput(new GeneralPipingInput())
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile
            };

            var previousResult = input.ThicknessCoverageLayer.Mean;

            var exitPointL = 0.5;
            input.SetExitPointL(exitPointL);

            // Call
            var result = input.ThicknessCoverageLayer.Mean;

            // Assert
            Assert.AreEqual(exitPointL, input.ExitPointL);
            Assert.AreNotEqual(previousResult, result);
            Assert.AreEqual(1.0, result);
        }

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
    }
}