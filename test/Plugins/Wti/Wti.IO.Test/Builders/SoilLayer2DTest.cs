using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Wti.Data;
using Wti.IO.Builders;

namespace Wti.IO.Test.Builders
{
    public class SoilLayer2DTest
    {
        [Test]
        public void DefaultConstructor_Always_NotInstantiatedOuterAndInnerLoops()
        {
            // Call
            var result = new SoilLayer2D();

            // Assert
            Assert.IsNull(result.OuterLoop);
            Assert.IsNull(result.InnerLoop);
        }

        [Test]
        public void AsPipingSoilLayers_DefaultConstructed_ReturnsEmptyCollectionWithMaxValueBottom()
        {
            // Setup
            var layer = new SoilLayer2D();
            double bottom;

            // Call
            var result = layer.AsPipingSoilLayers(0.0, out bottom);

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.AreEqual(double.MaxValue, bottom);
        }

        [Test]
        public void AsPipingSoilLayers_WithOuterLoopNotIntersectingX_ReturnsEmptyCollectionWithMaxValueBottom()
        {
            // Setup
            var layer = new SoilLayer2D
            {
                OuterLoop = new HashSet<Point3D>
                {
                    new Point3D
                    {
                        X = 0.1, Z = new Random(22).NextDouble()
                    }
                }
            };
            double bottom;

            // Call
            var result = layer.AsPipingSoilLayers(0.0, out bottom);

            // Assert
            CollectionAssert.IsEmpty(result);
            Assert.AreEqual(double.MaxValue, bottom);
        }
    }
}