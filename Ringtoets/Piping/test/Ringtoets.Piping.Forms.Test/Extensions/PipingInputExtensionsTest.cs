using Core.Common.Base.Geometry;
using NUnit.Framework;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.Extensions;

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
    }
}