using NUnit.Framework;
using SharpMap.Api;

namespace SharpMap.Tests.Api
{
    [TestFixture]
    public class GeometryApiTest
    {
        [Test]
        public void TriangulateTest()
        {
            //     ----(0,1)----
            //    |            |
            //    |      .?    |
            //    |            |
            //  (1,0)---------(1,2)

            var pointX = new[] {0.0, 1.0, 1.0};
            var pointY = new[] {1.0, 0.0, 2.0};
            var values = new[] {3.0, 5.0, 9.0};

            var gridX = new[] {0.5};
            var gridY = new[] {1.0};

            // result should be (0.25*3.0 + 0.125*5.0 + 0.125*9.0)/0.5 = 5
            using (var api = new RemoteGeometryApi())
            {
                var result = api.Triangulate(pointX, pointY, values, gridX, gridY);
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual(5, result[0], 1e-07);
            }
        }

        [Test]
        public void AveragingTest()
        {
            //    (0,1)------------(1,1)
            //      |                |
            //      |   X        X   |
            //      |                |
            //      |   X        X   |
            //      |                |
            //    (0,0)------------(1,0)

            var samplesX = new[] {0.25, 0.75, 0.75, 0.25};
            var samplesY = new[] {0.25, 0.25, 0.75, 0.75};
            var samplesZ = new[] {3.0, 5.0, 9.0, 11.0};
            var nodesX = new[] {0.5};
            var nodesY = new[] {0.5};
            var cellCornersX = new[,] {{0.0, 1.0, 1.0, 0.0}};
            var cellCornersY = new[,] {{0.0, 0.0, 1.0, 1.0}};
            var numCellCorners = new[] {4};

            using (var api = new RemoteGeometryApi())
            {
                var result = api.Averaging(samplesX, samplesY, samplesZ, nodesX, nodesY,
                    cellCornersX, cellCornersY,
                    numCellCorners);
                Assert.AreEqual(1, result.Length);
                Assert.AreEqual(7.0, result[0]);
            }
        }
    }
}
