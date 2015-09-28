using System.Drawing;
using DelftTools.TestUtils;
using GisSharpBlog.NetTopologySuite.Geometries;
using NUnit.Framework;
using SharpMap.Rendering;

namespace SharpMap.Tests.Rendering
{
    [TestFixture]
    public class VectorRenderingHelperTest
    {
        [Test]
        public void RenderNormal()
        {
            TestLinesAreDrawnCorrectly(1.0);
        }
        
        [Test]
        public void RenderVerySmallWithoutOverFlow()
        {
            TestLinesAreDrawnCorrectly(0.0000001);
        }

        [Test]
        public void RenderVerySmallWithoutGlitches()
        {
            TestLinesAreDrawnCorrectly(0.000001);
        }

        [Test]
        [Category(TestCategory.Performance)]
        public void RenderManyLinesShouldBeFast()
        {
            using (var bmp = new Bitmap(200, 200))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    var map = new Map(new Size(200, 200)) {Center = new Coordinate(1, 0)};

                    var line = new LineString(new[]
                                                  {
                                                      new Coordinate(0, 0),
                                                      new Coordinate(1, 0),
                                                      new Coordinate(1, 1)
                                                  });
                    map.Zoom = 0.000001; //difficult zoom level to make sure limiting kicks in

                    TestHelper.AssertIsFasterThan(
                        200,
                        () =>
                            {
                                for (int i = 0; i < 5000; i++)
                                {
                                    //orig (without limiting): 75 ms for 5000 lines on W510 i7
                                    VectorRenderingHelper.DrawLineString(g, line, Pens.Black, map);
                                }
                            });
                }
            }
        }
        
        private static void TestLinesAreDrawnCorrectly(double zoom)
        {
            using (var bmp = new Bitmap(200, 200))
            {
                using (var g = Graphics.FromImage(bmp))
                {
                    var map = new Map(new Size(200, 200)) { Center = new Coordinate(1, -0.2) };

                    var line = new LineString(new[]
                                                  {
                                                      new Coordinate(0, 0.2),
                                                      new Coordinate(1, -0.2),
                                                      new Coordinate(1, 1)
                                                  });
                    map.Zoom = zoom;

                    VectorRenderingHelper.DrawLineString(g, line, Pens.Black, map);

                    Assert.IsTrue(ColorEquals(Color.Black, bmp.GetPixel(0, 60)), "Visual glitch detected");
                    Assert.IsTrue(ColorEquals(Color.Black, bmp.GetPixel(100, 50)), "Visual glitch detected");
                }
            }
        }

        private static bool ColorEquals(Color a, Color b)
        {
            return
                a.A == b.A &&
                a.R == b.R &&
                a.G == b.G &&
                a.B == b.B;
        }
    }
}