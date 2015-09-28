using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.TestUtils;
using GeoAPI.Geometries;
using GeoAPI.Operations.Buffer;
using GisSharpBlog.NetTopologySuite.Geometries;
using GisSharpBlog.NetTopologySuite.Operation.Buffer;
using NUnit.Framework;
using SharpMap;
using SharpMap.Data.Providers;
using SharpMap.Layers;
using SharpMapTestUtils;
using Point = System.Drawing.Point;

namespace NetTopologySuite.Tests
{
    [TestFixture]
    public class BufferTest 
    {
        [Test,Category(TestCategory.WindowsForms)]
        public void BufferAroundLine()
        {
            var form = new Form {BackColor = Color.White, Size = new Size(500, 200)};

            form.Paint += delegate
                              {

                                  Graphics g = form.CreateGraphics();

                                  List<ICoordinate> vertices = new List<ICoordinate>();

                                  vertices.Add(new Coordinate(0, 4));
                                  vertices.Add(new Coordinate(40, 15));
                                  vertices.Add(new Coordinate(50, 50));
                                  vertices.Add(new Coordinate(100, 62));
                                  vertices.Add(new Coordinate(240, 45));
                                  vertices.Add(new Coordinate(350, 5));

                                  IGeometry geometry = new LineString(vertices.ToArray());

                                  g.DrawLines(new Pen(Color.Blue, 1), GetPoints(geometry));

                                  BufferOp bufferOp = new BufferOp(geometry);
                                  bufferOp.EndCapStyle = BufferStyle.CapButt;
                                  bufferOp.QuadrantSegments = 0;

                                  IGeometry bufGeo = bufferOp.GetResultGeometry(5);

                                  bufGeo = bufGeo.Union(geometry);
                                  g.FillPolygon(new SolidBrush(Color.Pink), GetPoints(bufGeo));
                              };

            WindowsFormsTestHelper.ShowModal(form);
        }


        [Test]
        [Category(TestCategory.WindowsForms)]
        public void IntersectTriangles()
        {
            ICoordinate[] v1 = new ICoordinate[7];
            v1[0] = new Coordinate(0,0);
            v1[1] = new Coordinate(5,10);
            v1[2] = new Coordinate(10,0);

            v1[3] = new Coordinate(8, 0);
            v1[4] = new Coordinate(5, 3);
            v1[5] = new Coordinate(4, 0);

            v1[6] = new Coordinate(0,0);


            IGeometry pol1 = new Polygon(new LinearRing(v1));

            ICoordinate[] v2 = new ICoordinate[5];
            v2[0] = new Coordinate(0, 0);
            v2[1] = new Coordinate(10, 0);
            v2[2] = new Coordinate(10, 1);
            v2[3] = new Coordinate(0, 1);
            v2[4] = new Coordinate(0, 0);

            IGeometry pol2 = new Polygon(new LinearRing(v2));

/*
            IGeometry g = pol1.Difference(pol2);
            Assert.AreEqual(6, g.Coordinates.Length);

            IGeometry g1 = pol1.Union(pol2);
            Assert.AreEqual(7, g1.Coordinates.Length);

*/
            var map = new Map();

            IGeometry gIntersection = pol1.Intersection(pol2);
            map.Layers.Add(new VectorLayer("g1", new DataTableFeatureProvider(new [] { gIntersection })));


/*
            map.Layers.Add(new VectorLayer("g", new DataTableFeatureProvider(new IGeometry[] { pol1 }) ));
            map.Layers.Add(new VectorLayer("g1", new DataTableFeatureProvider(new IGeometry[] { pol2 })));
*/

            MapTestHelper.ShowModal(map);
        }

        [Test]
        public void IntersectPolygonWithLine()
        {
            ICoordinate[] coordinates = new ICoordinate[5];
            coordinates[0] = new Coordinate(0, 0);
            coordinates[1] = new Coordinate(10, 0);
            coordinates[2] = new Coordinate(10, 10);
            coordinates[3] = new Coordinate(0, 10);
            coordinates[4] = new Coordinate(0, 0);

            IGeometry pol2 = new Polygon(new LinearRing(coordinates));
            var line = new LineString(new[] { new Coordinate(5,5), new Coordinate(6,20),new Coordinate(7,5) });

            var result = pol2.Intersection(line);
        }
        private Point[] GetPoints(IGeometry geometry)
        {
            List<Point> points = new List<Point>();
            foreach (ICoordinate vertex in geometry.Coordinates)
            {
                points.Add(new Point((int)vertex.X, (int)vertex.Y));
            }

            return points.ToArray();
            
        }
    }
}
