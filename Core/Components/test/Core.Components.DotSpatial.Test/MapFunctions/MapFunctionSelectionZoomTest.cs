﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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
// along with this program. If not, see <http://www.gnu.org/licenses/>.
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
using DotSpatial.Topology;
using NUnit.Framework;
using Rhino.Mocks;
using Point = System.Drawing.Point;

namespace Core.Components.DotSpatial.Test.MapFunctions
{
    [TestFixture]
    public class MapFunctionSelectionZoomTest
    {
        private MockRepository mockingRepository;

        [SetUp]
        public void SetUp()
        {
            mockingRepository = new MockRepository();
        }

        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            var mapMock = mockingRepository.StrictMock<IMap>();
            mockingRepository.ReplayAll();

            // Call
            MapFunctionSelectionZoom mapFunction = new MapFunctionSelectionZoom(mapMock);

            // Assert
            Assert.IsInstanceOf<MapFunction>(mapFunction);
            mockingRepository.VerifyAll();
        }

        [Test]
        public void Constructor_ActivateEvent_ExpectedValues()
        {
            // Setup
            var mapMock = mockingRepository.StrictMock<IMap>();
            mapMock.Expect(m => m.Cursor).SetPropertyWithArgument(Cursors.Default);
            mockingRepository.ReplayAll();

            // Call
            MapFunctionSelectionZoom mapFunction = new MapFunctionSelectionZoom(mapMock);
            mapFunction.Activate();

            // Assert
            Assert.IsInstanceOf<MapFunction>(mapFunction);
            mockingRepository.VerifyAll();
        }

        [Test]
        public void OnMouseDown_Always_SetsCursorToHand()
        {
            // Setup
            var mapMock = mockingRepository.Stub<IMap>();
            mapMock.MapFrame = mockingRepository.Stub<IMapFrame>();
            mockingRepository.ReplayAll();

            mapMock.Cursor = Cursors.Cross;
            var mapFunction = new MapFunctionSelectionZoom(mapMock);

            // Call
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), mapMock));

            // Assert
            Assert.IsTrue(mapMock.IsBusy);
            Assert.AreEqual(Cursors.SizeNWSE, mapMock.Cursor);
            mockingRepository.VerifyAll();
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
            
            var mapMock = mockingRepository.Stub<IMap>();
            mapMock.Expect(e => e.PixelToProj(Arg.Is(new Point(startPointX, startPointY)))).Return(null);
            mapMock.Expect(e => e.PixelToProj(Arg.Is(new Point(endPointX, endPointY)))).Return(null);
            mapMock.Expect(e => e.Invalidate(Arg<Rectangle>.Matches(m => m.Equals(expectedRectangle))));
            mockingRepository.ReplayAll();

            var mapFunction = new MapFunctionSelectionZoom(mapMock);
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startPointX, startPointY, 0), mapMock));

            // Call
            mapFunction.DoMouseMove(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 0, endPointX, endPointY, 0), mapMock));

            // Assert
            Assert.IsTrue(mapMock.IsBusy);
            Assert.AreEqual(Cursors.SizeNWSE, mapMock.Cursor);
            mockingRepository.VerifyAll();
        }

        [Test]
        public void OnMouseDown_NotZoomedSameLocation_DoesNotZoom()
        {
            // Setup
            var mapMock = mockingRepository.Stub<IMap>();
            mockingRepository.ReplayAll();

            int startPointX = 0;
            int startPointY = 0;
            var mapFunction = new MapFunctionSelectionZoom(mapMock);
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startPointX, startPointY, 0), mapMock));

            // Call
            mapFunction.DoMouseUp(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 0, startPointX, startPointY, 0), mapMock));

            // Assert
            Assert.IsNull(mapMock.ViewExtents);
            Assert.IsFalse(mapMock.IsBusy);
            Assert.AreEqual(Cursors.Default, mapMock.Cursor);
            mockingRepository.VerifyAll();
        }

        [Test]
        [TestCase(0, 0, 100, 50)]
        [TestCase(-50, -50, 10, 50)]
        [TestCase(50, 50, -100, -50)]
        public void OnMouseUp_DraggingToOtherLocation_ZoomsToCoordinates(int startPointX, int startPointY, int endPointX, int endPointY)
        {
            // Setup
            double geoStartPointX = 0.0 + startPointX;
            double geoStartPointY = 0.0 + startPointY;
            double geoEndPointX = 0.0 + endPointX;
            double geoEndPointY = 0.0 + endPointY;

            var mapMock = mockingRepository.Stub<IMap>();
            mapMock.Expect(e => e.PixelToProj(Arg.Is(new Point(startPointX, startPointY)))).Return(new Coordinate(geoStartPointX, geoStartPointY));
            mapMock.Expect(e => e.PixelToProj(Arg.Is(new Point(endPointX, endPointY)))).Return(new Coordinate(geoEndPointX, geoEndPointY));
            mapMock.Expect(e => e.Invalidate());
            mockingRepository.ReplayAll();

            Extent expectedExtend = new Envelope(geoStartPointX, geoEndPointX, geoStartPointY, geoEndPointY).ToExtent();

            var mapFunction = new MapFunctionSelectionZoom(mapMock);
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startPointX, startPointY, 0), mapMock));

            // Call
            mapFunction.DoMouseUp(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, endPointX, endPointY, 0), mapMock));

            // Assert
            Assert.AreEqual(expectedExtend, mapMock.ViewExtents);
            Assert.IsFalse(mapMock.IsBusy);
            Assert.AreEqual(Cursors.Default, mapMock.Cursor);
            mockingRepository.VerifyAll();
        }

        [Test]
        [TestCase(0, 0)]
        [TestCase(10, 10)]
        public void OnMouseUp_NotDragging_ResetExtents(int startPointX, int startPointY)
        {
            // Setup
            var mapMock = mockingRepository.Stub<IMap>();
            var mapFrame = mockingRepository.Stub<IMapFrame>();
            mapMock.MapFrame = mapFrame;

            double geoStartPointX = 0.0 + startPointX;
            double geoStartPointY = 0.0 + startPointY;

            mapMock.Expect(e => e.PixelToProj(Arg.Is(new Point(startPointX, startPointY)))).Return(new Coordinate(geoStartPointX, geoStartPointY));
            mapMock.Expect(e => e.Invalidate());
            mapFrame.Expect(e => e.ResetExtents());
            mockingRepository.ReplayAll();

            var mapFunction = new MapFunctionSelectionZoom(mapMock);

            // Call
            mapFunction.DoMouseUp(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startPointX, startPointY, 0), mapMock));

            // Assert
            Assert.IsFalse(mapMock.IsBusy);
            Assert.AreEqual(Cursors.Default, mapMock.Cursor);
            mockingRepository.VerifyAll();
        }

        [Test]
        public void OnDraw_NotDragging_NoDrawing()
        {
            // Setup
            var mapMock = mockingRepository.Stub<IMap>();
            var mapFrame = mockingRepository.Stub<IMapFrame>();
            var inGraphics = mockingRepository.Stub<Graphics>();
            inGraphics.Expect(e => e.DrawRectangle(null, 0, 0, 0, 0)).IgnoreArguments().Repeat.Never();
            mockingRepository.ReplayAll();

            mapMock.Cursor = Cursors.Cross;
            var mapFunction = new MapFunctionSelectionZoom(mapMock);

            var clipRectangle = new Rectangle(0, 0, 0, 0);

            // Call
            mapFunction.Draw(new MapDrawArgs(inGraphics, clipRectangle, mapFrame));

            // Assert
            mockingRepository.VerifyAll();
        }

        [Test]
        public void OnDraw_Dragging_DrawRectangle()
        {
            // Setup
            var random = new Random(21);
            int startX = random.Next(1, 100);
            int startY = random.Next(1, 100);
            var rectangleFromPoints = Opp.RectangleFromPoints(new Point(startX, startY), new Point(startX, startY));
            rectangleFromPoints.Width -= 1;
            rectangleFromPoints.Height -= 1;

            var mapMock = mockingRepository.Stub<IMap>();
            var mapFrame = mockingRepository.Stub<IMapFrame>();
            var inGraphics = mockingRepository.Stub<Graphics>();

            inGraphics.Expect(e => e.DrawRectangle(Pens.White, rectangleFromPoints));
            inGraphics.Expect(e => e.DrawRectangle(Arg<Pen>.Matches(p => p.Color.Equals(Color.Black) && p.DashStyle.Equals(DashStyle.Dash)), Arg.Is(rectangleFromPoints)));

            mockingRepository.ReplayAll();

            mapMock.Cursor = Cursors.Cross;
            var mapFunction = new MapFunctionSelectionZoom(mapMock);

            var clipRectangle = new Rectangle(0, 0, 0, 0);

            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, startX, startY, 0), mapMock));

            // Call
            mapFunction.Draw(new MapDrawArgs(inGraphics, clipRectangle, mapFrame));

            // Assert
            mockingRepository.VerifyAll();
        }
    }
}