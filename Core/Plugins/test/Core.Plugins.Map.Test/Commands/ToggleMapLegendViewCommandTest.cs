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

using System.Collections.Generic;
using Core.Common.Controls.Commands;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ViewHost;
using Core.Plugins.Map.Commands;
using Core.Plugins.Map.Legend;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.Map.Test.Commands
{
    [TestFixture]
    public class ToggleMapLegendViewCommandTest
    {
        [Test]
        public void Constructor_Always_CreatesICommand()
        {
            // Call
            var command = new ToggleMapLegendViewCommand(null);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Checked_LegendViewOpenOrClosed_ReturnsExpectedState(bool open)
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();

            var contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();

            if (open)
            {
                var toolViewList = new List<IView>();
                var viewHost = mocks.Stub<IViewHost>();

                viewController.Stub(vc => vc.ViewHost).Return(viewHost);
                viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
                viewHost.Expect(vm => vm.AddToolView(Arg<MapLegendView>.Is.NotNull,
                                                     Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)))
                        .WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as MapLegendView));

                viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
            }

            mocks.ReplayAll();

            var controller = new MapLegendController(viewController, contextMenuBuilderProvider);
            var command = new ToggleMapLegendViewCommand(controller);

            if (open)
            {
                controller.ToggleView();
            }

            // Call
            bool result = command.Checked;

            // Assert
            Assert.AreEqual(open, result);
            mocks.VerifyAll();
        }

        [Test]
        public void Execute_Always_TogglesLegend()
        {
            // Setup
            var mocks = new MockRepository();
            var viewController = mocks.StrictMock<IViewController>();
            var contextMenuBuilderProvider = mocks.Stub<IContextMenuBuilderProvider>();
            var viewHost = mocks.Stub<IViewHost>();
            var toolViewList = new List<IView>();

            viewController.Stub(vc => vc.ViewHost).Return(viewHost);
            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);

            // Open
            viewHost.Expect(vm => vm.AddToolView(Arg<MapLegendView>.Is.NotNull,
                                                 Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)))
                    .WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as MapLegendView));
            viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();

            // Close
            viewHost.Expect(vm => vm.Remove(Arg<MapLegendView>.Is.NotNull));

            mocks.ReplayAll();

            var controller = new MapLegendController(viewController, contextMenuBuilderProvider);
            var command = new ToggleMapLegendViewCommand(controller);

            // Call
            command.Execute();
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }
    }
}