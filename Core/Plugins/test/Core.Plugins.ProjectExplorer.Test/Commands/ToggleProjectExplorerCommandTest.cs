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
using System.Linq;
using Core.Common.Controls.Commands;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ViewHost;
using Core.Plugins.ProjectExplorer.Commands;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test.Commands
{
    [TestFixture]
    public class ToggleProjectExplorerCommandTest
    {
        [Test]
        public void Constructor_WithoutController_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new ToggleProjectExplorerCommand(null);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void Constructor_WithController_CreatesNewICommand()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var viewHost = mocks.StrictMock<IViewHost>();
            var viewController = mocks.StrictMock<IViewController>();

            viewHost.Stub(vm => vm.ToolViews).Return(new List<IView>());
            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var explorerViewController = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos))
            {
                // Call
                var command = new ToggleProjectExplorerCommand(explorerViewController);

                // Assert
                Assert.IsInstanceOf<ICommand>(command);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Checked_Always_CallsViewControllerIsViewActive(bool isViewOpen)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var toolViewList = new List<IView>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);

            if (isViewOpen)
            {
                viewHost.Expect(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.NotNull,
                                                     Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)))
                        .WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as ProjectExplorer));
                viewHost.Expect(tvc => tvc.SetImage(null, null)).IgnoreArguments();
                viewHost.Expect(vm => vm.Remove(Arg<ProjectExplorer>.Is.TypeOf));
            }

            var viewController = mocks.StrictMock<IViewController>();
            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var explorerViewController = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos))
            {
                var command = new ToggleProjectExplorerCommand(explorerViewController);

                if (isViewOpen)
                {
                    explorerViewController.ToggleView();
                }

                // Call
                bool result = command.Checked;

                // Assert
                Assert.AreEqual(isViewOpen, result);
            }

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_Always_CallsViewControllerToggleView(bool isViewOpen)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var toolViewList = new List<IView>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
            viewHost.Expect(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.NotNull,
                                                 Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)))
                    .WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as ProjectExplorer));
            viewHost.Stub(vm => vm.SetImage(null, null)).IgnoreArguments();

            viewHost.Expect(tvc => tvc.Remove(Arg<ProjectExplorer>.Is.TypeOf));

            var viewController = mocks.StrictMock<IViewController>();
            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var explorerViewController = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos))
            {
                if (isViewOpen)
                {
                    explorerViewController.ToggleView();
                }

                var command = new ToggleProjectExplorerCommand(explorerViewController);

                // Call
                command.Execute();
            }

            // Assert
            mocks.VerifyAll();
        }
    }
}