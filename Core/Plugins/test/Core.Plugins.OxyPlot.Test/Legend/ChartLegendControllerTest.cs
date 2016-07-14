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
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Forms.ViewHost;
using Core.Plugins.OxyPlot.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.OxyPlot.Test.Legend
{
    [TestFixture]
    public class ChartLegendControllerTest
    {
        [Test]
        public void Constructor_WithoutPlugin_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ChartLegendController(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithViewController_DoesNotThrow()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ChartLegendController(viewController);

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

            if (open)
            {
                var viewHost = mocks.Stub<IViewHost>();
                var toolViewList = new List<IView>();

                viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
                viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
                viewHost.Expect(vm => vm.AddToolView(Arg<ChartLegendView>.Matches(c => true), Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left))).WhenCalled(invocation =>
                {
                    toolViewList.Add(invocation.Arguments[0] as ChartLegendView);
                });
                viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
            }

            mocks.ReplayAll();

            var controller = new ChartLegendController(viewController);

            if (open)
            {
                controller.ToggleView();
            }

            // Call
            var result = controller.IsLegendViewOpen;

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
            var viewController = mocks.StrictMock<IViewController>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
            viewHost.Stub(tvc => tvc.SetImage(null, null)).IgnoreArguments();

            var toolViewList = new List<IView>();
            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
            viewHost.Expect(vm => vm.AddToolView(Arg<ChartLegendView>.Matches(c => true), Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left))).WhenCalled(invocation =>
            {
                toolViewList.Add(invocation.Arguments[0] as ChartLegendView);
            });

            if (open)
            {
                viewHost.Expect(p => p.Remove(Arg<ChartLegendView>.Matches(c => true)));
            }

            mocks.ReplayAll();

            var controller = new ChartLegendController(viewController);

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