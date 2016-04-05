using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using NUnit.Framework;
using Ringtoets.Common.Data;
using Ringtoets.HydraRing.Data;
using Ringtoets.Piping.Forms.Properties;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.Primitives;

namespace Ringtoets.Piping.Forms.Test.Views
{
    [TestFixture]
    public class PipingMapDataFactoryTest
    {
        [Test]
        public void CreateEmptyLineData_Always_ReturnEmptyMapLineDataWithNameSet()
        {
            // Setup
            const string name = "<test>";

            // Call
            MapLineData mapData = PipingMapDataFactory.CreateEmptyLineData(name);

            // Assert
            Assert.AreEqual(name, mapData.Name);
            Assert.IsEmpty(mapData.Features);
        }

        [Test]
        public void CreateEmptyPointData_Always_ReturnEmptyMapLineDataWithNameSet()
        {
            // Setup
            const string name = "<test>";

            // Call
            MapPointData mapData = PipingMapDataFactory.CreateEmptyPointData(name);

            // Assert
            Assert.AreEqual(name, mapData.Name);
            Assert.IsEmpty(mapData.Features);
        }

        [Test]
        public void Create_GivenSurfaceLines_ReturnsMapFeaturesWithDefaultStyling()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point3D(1.2, 2.3, 4.0),
                new Point3D(2.7, 2.0, 6.0)
            };
            var pointsTwo = new[]
            {
                new Point3D(3.2, 23.3, 34.2),
                new Point3D(7.7, 12.6, 1.2)
            };
            var lines = new[]
            {
                new RingtoetsPipingSurfaceLine(),
                new RingtoetsPipingSurfaceLine()
            };
            lines[0].SetGeometry(pointsOne);
            lines[1].SetGeometry(pointsTwo);

            // Call
            MapData data = PipingMapDataFactory.Create(lines);

            // Assert
            Assert.IsInstanceOf<MapLineData>(data);
            var mapLineData = (MapLineData) data;
            Assert.AreEqual(1, mapLineData.Features.Count());
            Assert.AreEqual(2, mapLineData.Features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, mapLineData.Features.ElementAt(0).MapGeometries.ElementAt(0));
            AssertEqualPointCollections(pointsTwo, mapLineData.Features.ElementAt(0).MapGeometries.ElementAt(1));
            Assert.AreEqual(Resources.PipingSurfaceLinesCollection_DisplayName, data.Name);

            AssertEqualStyle(mapLineData.Style, Color.DarkSeaGreen, 2, DashStyle.Solid);
        }

        [Test]
        public void Create_NoSurfaceLines_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingMapDataFactory.Create((IEnumerable<RingtoetsPipingSurfaceLine>) null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("surfaceLines", parameter);
        }

        [Test]
        public void Create_GivenSections_ReturnsMapFeaturesWithDefaultStyling()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var pointsTwo = new[]
            {
                new Point2D(3.2, 23.3),
                new Point2D(7.7, 12.6)
            };
            var lines = new[]
            {
                new FailureMechanismSection(string.Empty, pointsOne),
                new FailureMechanismSection(string.Empty, pointsTwo)
            };

            // Call
            MapData data = PipingMapDataFactory.Create(lines);

            // Assert
            Assert.IsInstanceOf<MapLineData>(data);
            var mapLineData = (MapLineData) data;
            Assert.AreEqual(1, mapLineData.Features.Count());
            Assert.AreEqual(2, mapLineData.Features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, mapLineData.Features.ElementAt(0).MapGeometries.ElementAt(0));
            AssertEqualPointCollections(pointsTwo, mapLineData.Features.ElementAt(0).MapGeometries.ElementAt(1));
            Assert.AreEqual(Common.Forms.Properties.Resources.FailureMechanism_Sections_DisplayName, data.Name);

            AssertEqualStyle(mapLineData.Style, Color.Khaki, 3, DashStyle.Dot);
        }

        [Test]
        public void Create_NoSections_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingMapDataFactory.Create((IEnumerable<FailureMechanismSection>) null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sections", parameter);
        }

        [Test]
        public void CreateStartPoints_GivenSections_ReturnsMapFeaturesWithDefaultStyling()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var pointsTwo = new[]
            {
                new Point2D(3.2, 23.3),
                new Point2D(7.7, 12.6)
            };
            var lines = new[]
            {
                new FailureMechanismSection(string.Empty, pointsOne),
                new FailureMechanismSection(string.Empty, pointsTwo)
            };

            // Call
            MapData data = PipingMapDataFactory.CreateStartPoints(lines);

            // Assert
            Assert.IsInstanceOf<MapPointData>(data);
            var mapPointData = (MapPointData) data;
            Assert.AreEqual(1, mapPointData.Features.Count());
            Assert.AreEqual(1, mapPointData.Features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                pointsOne[0],
                pointsTwo[0]
            }, mapPointData.Features.ElementAt(0).MapGeometries.ElementAt(0));

            var name = SectionPointDisplayName(Common.Forms.Properties.Resources.FailureMechanismSections_StartPoints_DisplayName);
            Assert.AreEqual(name, data.Name);

            AssertEqualStyle(mapPointData.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateStartPoints_NoSections_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingMapDataFactory.CreateStartPoints(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sections", parameter);
        }

        [Test]
        public void CreateEndPoints_GivenSections_ReturnsMapFeaturesWithDefaultStyling()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var pointsTwo = new[]
            {
                new Point2D(3.2, 23.3),
                new Point2D(7.7, 12.6)
            };
            var lines = new[]
            {
                new FailureMechanismSection(string.Empty, pointsOne),
                new FailureMechanismSection(string.Empty, pointsTwo)
            };

            // Call
            MapData data = PipingMapDataFactory.CreateEndPoints(lines);

            // Assert
            Assert.IsInstanceOf<MapPointData>(data);
            var mapPointData = (MapPointData) data;
            Assert.AreEqual(1, mapPointData.Features.Count());
            Assert.AreEqual(1, mapPointData.Features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(new[]
            {
                pointsOne[1],
                pointsTwo[1]
            }, mapPointData.Features.ElementAt(0).MapGeometries.ElementAt(0));

            var name = SectionPointDisplayName(Common.Forms.Properties.Resources.FailureMechanismSections_EndPoints_DisplayName);
            Assert.AreEqual(name, data.Name);

            AssertEqualStyle(mapPointData.Style, Color.DarkKhaki, 15, PointSymbol.Triangle);
        }

        [Test]
        public void CreateEndPoints_NoSections_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingMapDataFactory.CreateEndPoints(null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("sections", parameter);
        }

        [Test]
        public void Create_GivenReferenceLine_ReturnsMapFeaturesWithDefaultStyling()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var line = new ReferenceLine();
            line.SetGeometry(pointsOne);

            // Call
            MapData data = PipingMapDataFactory.Create(line);

            // Assert
            Assert.IsInstanceOf<MapLineData>(data);
            var mapLineData = (MapLineData) data;
            Assert.AreEqual(1, mapLineData.Features.Count());
            Assert.AreEqual(1, mapLineData.Features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, mapLineData.Features.ElementAt(0).MapGeometries.ElementAt(0));

            Assert.AreEqual(Common.Data.Properties.Resources.ReferenceLine_DisplayName, data.Name);

            AssertEqualStyle(mapLineData.Style, Color.Red, 3, DashStyle.Solid);
        }

        [Test]
        public void Create_NoReferenceLine_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingMapDataFactory.Create((ReferenceLine) null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("referenceLine", parameter);
        }

        [Test]
        public void Create_GivenHydraulicDatabase_ReturnsMapFeaturesWithDefaultStyling()
        {
            // Setup
            var pointsOne = new[]
            {
                new Point2D(1.2, 2.3),
                new Point2D(2.7, 2.0)
            };
            var database = new HydraulicBoundaryDatabase();
            database.Locations.AddRange(pointsOne.Select(p => new HydraulicBoundaryLocation(0, "", p.X, p.Y)));

            // Call
            MapData data = PipingMapDataFactory.Create(database);

            // Assert
            Assert.IsInstanceOf<MapPointData>(data);
            var mapPointData = (MapPointData)data;
            Assert.AreEqual(1, mapPointData.Features.Count());
            Assert.AreEqual(1, mapPointData.Features.ElementAt(0).MapGeometries.Count());
            AssertEqualPointCollections(pointsOne, mapPointData.Features.ElementAt(0).MapGeometries.ElementAt(0));

            Assert.AreEqual(Common.Data.Properties.Resources.HydraulicBoundaryConditions_DisplayName, data.Name);

            AssertEqualStyle(mapPointData.Style, Color.DarkBlue, 6, PointSymbol.Circle);
        }

        [Test]
        public void Create_NoHydraulicDatabase_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => PipingMapDataFactory.Create((HydraulicBoundaryDatabase) null);

            // Assert
            var parameter = Assert.Throws<ArgumentNullException>(test).ParamName;
            Assert.AreEqual("hydraulicBoundaryDatabase", parameter);
        }

        private void AssertEqualStyle(PointStyle pointStyle, Color color, int width, PointSymbol symbol)
        {
            Assert.AreEqual(color, pointStyle.Color);
            Assert.AreEqual(width, pointStyle.Size);
            Assert.AreEqual(symbol, pointStyle.Symbol);
        }

        private static void AssertEqualStyle(LineStyle lineStyle, Color color, int width, DashStyle style)
        {
            Assert.AreEqual(color, lineStyle.Color);
            Assert.AreEqual(width, lineStyle.Width);
            Assert.AreEqual(style, lineStyle.Style);
        }

        private static string SectionPointDisplayName(string name)
        {
            return string.Format("{0} ({1})",
                                 Common.Forms.Properties.Resources.FailureMechanism_Sections_DisplayName,
                                 name);
        }

        private void AssertEqualPointCollections(IEnumerable<Point3D> points, MapGeometry geometry)
        {
            AssertEqualPointCollections(points.Select(p => new Point2D(p.X, p.Y)), geometry);
        }

        private void AssertEqualPointCollections(IEnumerable<Point2D> points, MapGeometry geometry)
        {
            CollectionAssert.AreEqual(points.Select(p => new Point2D(p.X, p.Y)), geometry.Points);
        }
    }
}