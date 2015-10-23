using System.Linq;

using Deltares.WTIPiping;
using NUnit.Framework;
using Wti.Calculation.Piping;
using Wti.Data;

namespace Wti.Calculation.Test.Piping
{
    public class PipingSurfaceLineCreatorTest
    {
        [Test]
        public void Create_Always_ReturnsSurfaceLineWithASinglePointAtOrigin()
        {
            // Setup
            const string name = "Local coordinate surfaceline";
            var surfaceLine = new RingtoetsPipingSurfaceLine
            {
                Name = name
            };
            surfaceLine.SetGeometry(new[]
            {
                new Point3D { X = 0.0, Y = 0.0, Z = 1.1 }, 
                new Point3D { X = 2.2, Y = 0.0, Z = 3.3 }, 
                new Point3D { X = 4.4, Y = 0.0, Z = 5.5 }
            });

            // Call
            PipingSurfaceLine actual = PipingSurfaceLineCreator.Create(surfaceLine);

            // Assert
            Assert.AreEqual(name, actual.Name);
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.X).ToArray(), actual.Points.Select(p => p.X).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Y).ToArray(), actual.Points.Select(p => p.Y).ToArray());
            CollectionAssert.AreEqual(surfaceLine.Points.Select(p => p.Z).ToArray(), actual.Points.Select(p => p.Z).ToArray());
            CollectionAssert.AreEqual(Enumerable.Repeat(PipingCharacteristicPointType.None, surfaceLine.Points.Count()), actual.Points.Select(p => p.Type));
        }
    }
}