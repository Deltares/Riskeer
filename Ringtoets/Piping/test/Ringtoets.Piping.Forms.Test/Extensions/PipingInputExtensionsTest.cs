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
            var inputParameters = new PipingInput();
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
        public void SetSurfaceLine_WithoutDikeToeDikeSideAndDikeToeRiverSide_ExitPointAtEndAndSeePageLengthIsLengthInX()
        {
            // Setup
            var inputParameters = new PipingInput();
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
            var inputParameters = new PipingInput();
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
            var inputParameters = new PipingInput();
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
            var input = new PipingInput
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
        [TestCase(2, 2, 4)]
        [TestCase(4, 2, 6)]
        [TestCase(4, 0.5, 4.5)]
        [TestCase(1e-6, 4, 4 + 1e-6)]
        [TestCase(3, -1e-6, 3 - 1e-6)]
        [TestCase(0.5, 1e-6, 0.5 + 1e-6)]
        public void SetExitPointL_ExitPointAndSeepageLengthSet_UpdatesSeepageLength(double seepageLength, double exitPoint, double newSeepageLength)
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
            var input = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                SeepageLength =
                {
                    Mean = seepageLength
                }
            };

            input.SetExitPointL(exitPoint);

            // Call & Assert
            Assert.AreEqual(exitPoint, input.ExitPointL);
            Assert.AreEqual(newSeepageLength, input.SeepageLength.Mean);
        }

        [Test]
        public void SetEntryPointL_SetResultInInvalidSeePage_ThrowsArgumentException()
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

            var l = 2.0;
            var input = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
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
            var random = new Random(22);

            var surfaceLine = ValidSurfaceLine(0.0, 4.0);
            var soilProfile = new PipingSoilProfile(String.Empty, random.NextDouble(), new[]
            {
                new PipingSoilLayer(random.NextDouble())
                {
                    IsAquifer = true
                }
            });
            var l = -2.0;
            var input = new PipingInput
            {
                SurfaceLine = surfaceLine,
                SoilProfile = soilProfile,
                SeepageLength =
                {
                    Mean = -l
                }
            };


            // Call
            TestDelegate test = () => input.SetExitPointL(l);

            // Assert
            var message = string.Format(Resources.PipingInputContextProperties_ExitPointL_Value_0_results_in_invalid_seepage_length, l);
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, message);
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