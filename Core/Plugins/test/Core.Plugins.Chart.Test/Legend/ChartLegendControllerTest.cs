// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ViewHost;
using Core.Plugins.Chart.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Chart.Test.Legend
{
    [TestFixture]
    public class ChartLegendControllerTest
    {
        [Test]
        public void Constructor_WithoutViewController_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();

            mocks.ReplayAll();
            // Call
            TestDelegate test = () => new ChartLegendController(null, menuBuilderProvider);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("viewController", exception.ParamName);
        }

        [Test]
        public void Constructor_WithoutContextMenuBuilderProvider_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ChartLegendController(viewController, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("contextMenuBuilderProvider", exception.ParamName);
        }

        [Test]
        public void Constructor_WithViewController_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ChartLegendController(viewController, menuBuilderProvider);

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
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();

            if (open)
            {
                var viewHost = mocks.Stub<IViewHost>();
                var toolViewList = new List<IView>();

                viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
                viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
                viewHost.Expect(vm => vm.AddToolView(Arg<ChartLegendView>.Is.NotNull, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left))).WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as ChartLegendView));
                viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
            }

            mocks.ReplayAll();

            var controller = new ChartLegendController(viewController, menuBuilderProvider);

            if (open)
            {
                controller.ToggleView();
            }

            // Call
            bool result = controller.IsLegendViewOpen;

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
            var menuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            var viewController = mocks.StrictMock<IViewController>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
            viewHost.Stub(tvc => tvc.SetImage(null, null)).IgnoreArguments();

            var toolViewList = new List<IView>();
            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
            viewHost.Expect(vm => vm.AddToolView(Arg<ChartLegendView>.Is.NotNull, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left))).WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as ChartLegendView));

            if (open)
            {
                viewHost.Expect(p => p.Remove(Arg<ChartLegendView>.Is.NotNull));
            }

            mocks.ReplayAll();

            var controller = new ChartLegendController(viewController, menuBuilderProvider);

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