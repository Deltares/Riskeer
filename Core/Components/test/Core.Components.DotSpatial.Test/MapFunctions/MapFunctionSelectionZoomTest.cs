// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using Core.Components.DotSpatial.MapFunctions;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using GeoAPI.Geometries;
using NUnit.Framework;
using NSubstitute;

namespace Core.Components.DotSpatial.Test.MapFunctions
{
    [TestFixture]
    public class MapFunctionSelectionZoomTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            var map = Substitute.For<IMap>();
            // Call
            var mapFunction = new MapFunctionSelectionZoom(map);

            // Assert
            Assert.IsInstanceOf<MapFunctionZoom>(mapFunction);
            const YieldStyles expectedYieldStyle = YieldStyles.LeftButton | YieldStyles.RightButton | YieldStyles.Scroll;
            Assert.AreEqual(expectedYieldStyle, mapFunction.YieldStyle);
        }

        [Test]
        public void OnMouseDown_Always_SetsMapBusy()
        {
            // Setup
            var map = Substitute.For<IMap>();
            map.MapFrame = Substitute.For<IMapFrame>();
            var mapFunction = new MapFunctionSelectionZoom(map);

            // Call
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), map));

            // Assert
            Assert.IsTrue(map.IsBusy);
        }

        [Test]
        [TestCase(0, 0, 100, 50)]
        [TestCase(-50, -50, 10, 50)]
        [TestCase(50, 50, -100, -50)]
        public void OnMouseMove_Dragging_DrawNewRectangle(int startPointX, int startPointY, int endPointX, int endPointY)
        {
            // Setup
            int x = Math.Min(Math.Min(startPointX, 0), endPointX);
            int y = Math.Min(Math.Min(startPointY, 0), endPointY);
            int mx = Math.Max(Math.Max(startPointX, 0), endPointX);
            int my = Math.Max(Math.Max(startPointY, 0), endPointY);
            var expectedRectangle = new Rectangle(x, y, mx - x, my - y);

            var map = Substitute.For<IMap>();
            map.PixelToProj(Arg.Any<Point>()).Returns((Coordinate) null);

            var mapFunction = new MapFunctionSelectionZoom(map);
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startPointX, startPointY, 0), map));

            // Call
            mapFunction.DoMouseMove(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 0, endPointX, endPointY, 0), map));

            // Assert
            Assert.IsTrue(map.IsBusy);
            map.Received().Invalidate(
                Arg.Is<Rectangle>(m => m.Equals(expectedRectangle)));
        }

        [Test]
        public void OnMouseDown_NotZoomedSameLocation_DoesNotZoom()
        {
            // Setup
            var map = Substitute.For<IMap>();
            const int startPointX = 0;
            const int startPointY = 0;
            var mapFunction = new MapFunctionSelectionZoom(map);
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startPointX, startPointY, 0), map));

            // Call
            mapFunction.DoMouseUp(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 0, startPointX, startPointY, 0), map));

            // Assert
            Assert.IsNull(map.ViewExtents);
            Assert.IsFalse(map.IsBusy);
        }

        [Test]
        [TestCase(0, 0, 100, 50)]
        [TestCase(-50, -50, 10, 50)]
        [TestCase(50, 50, -100, -50)]
        public void OnMouseUp_DraggingToOtherLocation_ZoomsToCoordinates(int startPointX, int startPointY, int endPointX, int endPointY)
        {
            // Setup
            double geoStartPointX = startPointX;
            double geoStartPointY = startPointY;
            double geoEndPointX = endPointX;
            double geoEndPointY = endPointY;

            var map = Substitute.For<IMap>();
            map.PixelToProj(new Point(startPointX, startPointY)).Returns(new Coordinate(geoStartPointX, geoStartPointY));
            map.PixelToProj(new Point(endPointX, endPointY)).Returns(new Coordinate(geoEndPointX, geoEndPointY));
            map.Invalidate();
            Extent expectedExtend = new Envelope(geoStartPointX, geoEndPointX, geoStartPointY, geoEndPointY).ToExtent();

            var mapFunction = new MapFunctionSelectionZoom(map);
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startPointX, startPointY, 0), map));

            // Call
            mapFunction.DoMouseUp(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, endPointX, endPointY, 0), map));

            // Assert
            Assert.AreEqual(expectedExtend, map.ViewExtents);
            Assert.IsFalse(map.IsBusy);
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(10, 10)]
        public void OnMouseUp_NotDragging_ResetExtents(int startPointX, int startPointY)
        {
            // Setup
            var map = Substitute.For<IMap>();
            var mapFrame = Substitute.For<IMapFrame>();
            map.MapFrame = mapFrame;

            double geoStartPointX = startPointX;
            double geoStartPointY = startPointY;

            map.PixelToProj(Arg.Is<Point>(p => p.X == startPointX && p.Y == startPointY))
               .Returns(new Coordinate(geoStartPointX, geoStartPointY));

            var mapFunction = new MapFunctionSelectionZoom(map);

            // Call
            mapFunction.DoMouseUp(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startPointX, startPointY, 0), map));

            // Assert
            Assert.IsFalse(map.IsBusy);
            map.Received().Invalidate();
            mapFrame.Received().ResetExtents();
        }

        [Test]
        public void OnDraw_NotDragging_NoDrawing()
        {
            // Setup
            var random = new Random(21);
            int startX = random.Next(1, 100);
            int startY = random.Next(1, 100);
            int endX = random.Next(1, 100);
            int endY = random.Next(1, 100);
            Rectangle rectangle = Opp.RectangleFromPoints(new Point(startX, startY), new Point(endX, endY));
            rectangle.Width -= 1;
            rectangle.Height -= 1;

            var map = Substitute.For<IMap>();
            var mapFrame = Substitute.For<IMapFrame>();
            var mapFunction = new MapFunctionSelectionZoom(map);
            var clipRectangle = new Rectangle(0, 0, 0, 0);

            var bitmap = new Bitmap(200, 200);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Magenta);

            mapFunction.DoMouseMove(
                new GeoMouseArgs(
                    new MouseEventArgs(MouseButtons.None, 0, endX, endY, 0),
                    map));
            // Call
            mapFunction.Draw(new MapDrawArgs(graphics, clipRectangle, mapFrame));

            // Assert unchanged colors on edges
            Assert.AreEqual(Color.Magenta.ToArgb(),
                            bitmap.GetPixel(rectangle.Left, rectangle.Top).ToArgb());
            Assert.AreEqual(Color.Magenta.ToArgb(),
                            bitmap.GetPixel(rectangle.Right, rectangle.Top).ToArgb());
            Assert.AreEqual(Color.Magenta.ToArgb(),
                            bitmap.GetPixel(rectangle.Left, rectangle.Bottom).ToArgb());
            Assert.AreEqual(Color.Magenta.ToArgb(),
                            bitmap.GetPixel(rectangle.Right, rectangle.Bottom).ToArgb());
        }

        [Test]
        public void OnDraw_Dragging_DrawRectangle()
        {
            // Setup
            var random = new Random(21);
            int startX = random.Next(1, 100);
            int startY = random.Next(1, 100);
            int endX = random.Next(1, 100);
            int endY = random.Next(1, 100);
            Rectangle rectangle = Opp.RectangleFromPoints(new Point(startX, startY), new Point(endX, endY));
            rectangle.Width -= 1;
            rectangle.Height -= 1;

            var map = Substitute.For<IMap>();
            var mapFrame = Substitute.For<IMapFrame>();
            var mapFunction = new MapFunctionSelectionZoom(map);
            var clipRectangle = new Rectangle(0, 0, 0, 0);

            var bitmap = new Bitmap(200, 200);
            var graphics = Graphics.FromImage(bitmap);
            graphics.Clear(Color.Magenta);

            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startX, startY, 0), map));
            mapFunction.DoMouseMove(
                new GeoMouseArgs(
                    new MouseEventArgs(MouseButtons.None, 0, endX, endY, 0),
                    map));
            // Call
            mapFunction.Draw(new MapDrawArgs(graphics, clipRectangle, mapFrame));

            // Assert changed color on edges
            Assert.AreNotEqual(Color.Magenta.ToArgb(),
                               bitmap.GetPixel(rectangle.Left, rectangle.Top).ToArgb());
            Assert.AreNotEqual(Color.Magenta.ToArgb(),
                               bitmap.GetPixel(rectangle.Right, rectangle.Top).ToArgb());
            Assert.AreNotEqual(Color.Magenta.ToArgb(),
                               bitmap.GetPixel(rectangle.Left, rectangle.Bottom).ToArgb());
            Assert.AreNotEqual(Color.Magenta.ToArgb(),
                               bitmap.GetPixel(rectangle.Right, rectangle.Bottom).ToArgb());
        }

        [Test]
        public void OnMouseMove_DraggingWithMiddleMouseButtonDown_DoesNotPan()
        {
            // Setup
            var map = Substitute.For<IMap>();
            map.MapFrame = Substitute.For<IMapFrame>();
            var mapFunction = new MapFunctionSelectionZoom(map);
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Middle, 1, 10, 10, 0), map));

            // Call
            Rectangle view = map.MapFrame.View;
            mapFunction.DoMouseMove(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Middle, 1, 20, 20, 0), map));

            // Assert
            Assert.AreEqual(view, map.MapFrame.View);
        }
    }
}