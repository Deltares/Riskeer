// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using DotSpatial.Symbology;
using NUnit.Framework;

using Rhino.Mocks;

namespace Core.Components.DotSpatial.Test.MapFunctions
{
    [TestFixture]
    public class MapFunctionSelectionZoomTest
    {
        [Test]
        public void Constructor_Always_ExpectedValues()
        {
            // Setup
            var mockingRepository = new MockRepository();
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
            var mockingRepository = new MockRepository();
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
            var mockingRepository = new MockRepository();
            var mapMock = mockingRepository.Stub<IMap>();
            mapMock.MapFrame = mockingRepository.Stub<IMapFrame>();
            mockingRepository.ReplayAll();

            mapMock.Cursor = Cursors.Cross;
            var mapFunction = new MapFunctionSelectionZoom(mapMock);

            // Call
            mapFunction.DoMouseDown(new GeoMouseArgs(new MouseEventArgs(MouseButtons.Left, 1, 0, 0, 0), mapMock));

            // Assert
            Assert.IsTrue(mapMock.IsBusy);
            Assert.AreEqual(Cursors.SizeNWSE,mapMock.Cursor);
            mockingRepository.VerifyAll();
        }

        [Test]
        public void OnDraw_NotDragging_NoDrawing()
        {
            // Setup
            var mockingRepository = new MockRepository();
            var mapMock = mockingRepository.Stub<IMap>();
            var mapFrame = mockingRepository.Stub<IMapFrame>();
            var inGraphics = mockingRepository.Stub<Graphics>();
            inGraphics.Expect(e => e.DrawRectangle(null, 0,0,0,0)).IgnoreArguments().Repeat.Never();
            mockingRepository.ReplayAll();

            mapMock.Cursor = Cursors.Cross;
            var mapFunction = new MapFunctionSelectionZoom(mapMock);

            var clipRectangle = new Rectangle(0,0,0,0);

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

            var mockingRepository = new MockRepository();
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