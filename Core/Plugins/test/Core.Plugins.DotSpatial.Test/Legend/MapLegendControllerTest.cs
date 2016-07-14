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
using System.Collections.Generic;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ViewHost;
using Core.Plugins.DotSpatial.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.DotSpatial.Test.Legend
{
    [TestFixture]
    public class MapLegendControllerTest
    {
        [Test]
        public void Constructor_WithoutViewController_ThrowsArgumentNullException()
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
            Assert.AreEqual("viewController", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithoutContextMenuBuilderProvider_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();
            var parentWindow = mocks.StrictMock<IWin32Window>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(viewController, null, parentWindow);

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
            var viewController = mocks.StrictMock<IViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(viewController, contextMenuBuilderProvider, null);

            // Assert
            ArgumentNullException exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("parentWindow", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_WithViewControllerAndContextMenuBuilderProvider_DoesNotThrowException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new MapLegendController(viewController, contextMenuBuilderProvider, parentWindow);

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
            var viewController = mocks.StrictMock<IViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            if (open)
            {
                var viewHost = mocks.Stub<IViewHost>();
                var toolViewList = new List<IView>();

                viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
                viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
                viewHost.Expect(vm => vm.AddToolView(Arg<MapLegendView>.Matches(c => true), Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left))).WhenCalled(invocation =>
                {
                    toolViewList.Add(invocation.Arguments[0] as MapLegendView);
                });
                viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
            }

            mocks.ReplayAll();

            var controller = new MapLegendController(viewController, contextMenuBuilderProvider, parentWindow);

            if (open)
            {
                controller.ToggleView();
            }

            // Call
            var result = controller.IsMapLegendViewOpen;

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
            var viewHost = mocks.StrictMock<IViewHost>();
            var viewController = mocks.StrictMock<IViewController>();
            var contextMenuBuilderProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            var parentWindow = mocks.StrictMock<IWin32Window>();

            var toolViewList = new List<IView>();
            viewController.Stub(vc => vc.ViewHost).Return(viewHost);
            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
            viewHost.Expect(vm => vm.AddToolView(Arg<MapLegendView>.Matches(c => true), Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left))).WhenCalled(invocation =>
            {
                toolViewList.Add(invocation.Arguments[0] as MapLegendView);
            });
            viewHost.Stub(tvc => tvc.SetImage(null, null)).IgnoreArguments();

            if (open)
            {
                viewHost.Expect(p => p.Remove(Arg<MapLegendView>.Matches(c => true)));
            }

            mocks.ReplayAll();

            var controller = new MapLegendController(viewController, contextMenuBuilderProvider, parentWindow);

            if (open)
            {
                controller.ToggleView();
            }

            // Call
            controller.ToggleView();

            // Assert
            mocks.VerifyAll();
        }
    }
}