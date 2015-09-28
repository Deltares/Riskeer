using System;
using System.Drawing;
using System.Linq;
using DelftTools.TestUtils;
using NUnit.Framework;
using SharpMap.Data.Providers.EGIS.ShapeFileLib;

namespace SharpMap.Tests.Data.Providers
{
    [TestFixture]
    public class QuadTreeTest
    {
        private static readonly Random Rand = new Random(1234); //deterministic random (for fair comparison)

        [Test]
        [Category(TestCategory.Performance)]
        public void ConstructQuadTree()
        {
            // generate random bounds
            RectangleF extends;
            var allBounds = CreateRandomRectangles(out extends);

            // construct the quad tree (and time it)
            // 40ms on my pc
            TestHelper.AssertIsFasterThan(300, () =>
                {
                    var quadTree = new QuadTree(extends, 5, false);
                    for (int i = 0; i < allBounds.Length; i++)
                        quadTree.Insert(i, allBounds[i]);
                });
        }

        [Test]
        [Category(TestCategory.Performance)]
        public void QueryQuadTree()
        {
            // generate random bounds
            RectangleF extends;
            var allBounds = CreateRandomRectangles(out extends);

            // construct the quad tree
            var quadTree = new QuadTree(extends, 5, false);
            for (int i = 0; i < allBounds.Length; i++)
                quadTree.Insert(i, allBounds[i]);

            var queryRectangles = Enumerable.Range(0, 10000)
                                            .Select(i => CreateRandomRectangle(2f))
                                            .ToArray();

            // query the quad tree (and time it)
            // 1050ms on my pc
            TestHelper.AssertIsFasterThan(3000, () =>
            {
                for (int i = 0; i < queryRectangles.Length; i++)
                {
                    var rect = queryRectangles[i];
                    var res = quadTree.GetIndices(ref rect, 0f).ToList();
                }
            });
        }
        
        private static RectangleF[] CreateRandomRectangles(out RectangleF extends)
        {
            var allBounds = new RectangleF[20000];
            float minX = float.MaxValue, minY = float.MaxValue;
            float maxX = float.MinValue, maxY = float.MinValue;

            for (int i = 0; i < allBounds.Length; i++)
            {
                var rect = CreateRandomRectangle(1f);
                allBounds[i] = rect;

                // get max extends
                if (rect.Left < minX) minX = rect.Left;
                if (rect.Right > maxX) maxX = rect.Right;
                if (rect.Top < minY) minY = rect.Top;
                if (rect.Bottom > maxY) maxY = rect.Bottom;
            }
            extends = new RectangleF(minX, minY, maxX - minX, maxY - minY);
            return allBounds;
        }

        private static RectangleF CreateRandomRectangle(float avgSize)
        {
            var x = (float) Rand.NextDouble()*10.0f; //between 0 and 10
            var y = (float) Rand.NextDouble()*10.0f; //between 0 and 10
            var width = (float)Rand.NextDouble() * avgSize;
            var height = (float)Rand.NextDouble() * avgSize;
            var rect = new RectangleF(x, y, width, height);
            return rect;
        }
    }
}