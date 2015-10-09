using Deltares.WTIPiping;
using NUnit.Framework;
using Wti.Calculation.Piping;

namespace Wti.Calculation.Test.Piping
{
    public class PipingSurfaceLineCreatorTest
    {
        [Test]
        public void GivenASurfaceLineCreator_WhenCreatingPipingSurfaceLine_ThenASurfaceLineWithASinglePointAtOrigin()
        {
            var pipingSurfaceLineCreator = new PipingSurfaceLineCreator();
            PipingSurfaceLine actual = pipingSurfaceLineCreator.Create();
            Assert.That(actual.Points.Count, Is.EqualTo(1));
            Assert.That(actual.Points[0].X, Is.EqualTo(0));
            Assert.That(actual.Points[0].Y, Is.EqualTo(0));
            Assert.That(actual.Points[0].Z, Is.EqualTo(0));
        }
    }
}