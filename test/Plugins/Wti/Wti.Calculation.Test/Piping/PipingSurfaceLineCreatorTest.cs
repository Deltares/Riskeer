using Deltares.WTIPiping;
using NUnit.Framework;
using Wti.Calculation.Piping;

namespace Wti.Calculation.Test.Piping
{
    public class PipingSurfaceLineCreatorTest
    {
        [Test]
        public void Create_Always_ReturnsSurfaceLineWithASinglePointAtOrigin()
        {
            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create();

            // Assert
            Assert.AreEqual(1, actual.Points.Count);
            Assert.AreEqual(0, actual.Points[0].X);
            Assert.AreEqual(0, actual.Points[0].Y);
            Assert.AreEqual(0, actual.Points[0].Z);
        }
    }
}