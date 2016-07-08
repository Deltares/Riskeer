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
using System.Windows.Forms;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.DotSpatial.Test.Legend
{
    [TestFixture]
    public class MapLegendControllerTest
    {
        [Test]
        public void Constructor_WithoutToolViewController_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(null, contextMenuBuilderProvider, parentWindow);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("toolViewController", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutContextMenuBuilderProvider_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var parentWindow = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(toolViewController, null, parentWindow);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("contextMenuBuilderProvider", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutParentWindow_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(toolViewController, contextMenuBuilderProvider, null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentWindow", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithToolViewControllerAndContextMenuBuilderProvider_DoesNotThrowException()
        {
            // Setup
            var mocks = new MockRepository();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(toolViewController, contextMenuBuilderProvider, parentWindow);

            // Assert
            Assert.DoesNotThrow(test);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsLegendViewOpen_LegendViewOpenAndClosedState_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IToolViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();
            plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new MapLegendController(plugin, contextMenuBuilderProvider, parentWindow);

            // Call
            var result = controller.IsLegendViewOpen();

            // Assert
            Assert.AreEqual(open, result);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void ToggleLegendView_LegendViewOpenAndClosedState_TogglesStateOfLegendView(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var plugin = mocks.StrictMock<IToolViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            if (open)
            {
                plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(false);
                plugin.Expect(p => p.OpenToolView(Arg<MapLegendView>.Matches(c => true)));
                plugin.Expect(p => p.CloseToolView(Arg<MapLegendView>.Matches(c => true)));
            }
            else
            {
                plugin.Expect(p => p.OpenToolView(Arg<MapLegendView>.Matches(c => true)));
            }
            plugin.Expect(p => p.IsToolWindowOpen<MapLegendView>()).Return(open);

            mocks.ReplayAll();

            var controller = new MapLegendController(plugin, contextMenuBuilderProvider, parentWindow);

            if (open)
            {
                controller.ToggleLegend();
            }

            // Call
            controller.ToggleLegend();

            // Assert
            mocks.VerifyAll();
        }
    }
}