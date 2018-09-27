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
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ViewHost;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectExplorerViewControllerTest
    {
        [Test]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        public void Constructor_ArgumentsNull_ThrowsArgumentNullException(int paramNullIndex)
        {
            // Setup
            var mocks = new MockRepository();
            IViewCommands viewCommands = paramNullIndex == 1 ? null : mocks.StrictMock<IViewCommands>();
            IViewController viewController = paramNullIndex == 2 ? null : mocks.StrictMock<IViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = paramNullIndex == 3 ? null : Enumerable.Empty<TreeNodeInfo>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void ToggleView_ViewOpen_ViewClosed()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var viewHost = mocks.Stub<IViewHost>();
            var toolViewList = new List<IView>();

            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
            viewHost.Expect(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.NotNull,
                                                 Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)))
                    .WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as ProjectExplorer));
            viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
            viewHost.Expect(vm => vm.Remove(Arg<ProjectExplorer>.Is.TypeOf));

            var viewController = mocks.StrictMock<IViewController>();
            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos))
            {
                controller.ToggleView();
                // Call
                controller.ToggleView();
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ToggleView_ViewInactive_ViewOpened()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var viewHost = mocks.Stub<IViewHost>();

            viewHost.Stub(vm => vm.ToolViews).Return(new List<IView>());
            viewHost.Stub(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.TypeOf, Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)));
            viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();

            var viewController = mocks.StrictMock<IViewController>();
            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos))
            {
                // Call
                controller.ToggleView();
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsProjectExplorerOpen_Always_CallsIsToolWindowOpenWithProjectExplorer(bool isOpen)
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var toolViewList = new List<IView>();
            var viewHost = mocks.StrictMock<IViewHost>();
            var viewController = mocks.StrictMock<IViewController>();

            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);

            if (isOpen)
            {
                viewHost.Expect(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.NotNull,
                                                     Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)))
                        .WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as ProjectExplorer));
                viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
                viewHost.Expect(vm => vm.Remove(Arg<ProjectExplorer>.Is.TypeOf));
            }

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos))
            {
                if (isOpen)
                {
                    controller.ToggleView();
                }

                // Call
                bool result = controller.IsProjectExplorerOpen;
                // Assert
                Assert.AreEqual(isOpen, result);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ViewIsOpen_UpdateData()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var viewHost = mocks.Stub<IViewHost>();
            var toolViewList = new List<IView>();

            viewHost.Stub(vm => vm.ToolViews).Return(toolViewList);
            viewHost.Expect(vm => vm.AddToolView(Arg<ProjectExplorer>.Is.NotNull,
                                                 Arg<ToolViewLocation>.Matches(vl => vl == ToolViewLocation.Left)))
                    .WhenCalled(invocation => toolViewList.Add(invocation.Arguments[0] as ProjectExplorer));
            viewHost.Expect(vm => vm.SetImage(null, null)).IgnoreArguments();
            viewHost.Expect(vm => vm.Remove(Arg<ProjectExplorer>.Is.TypeOf));

            var viewController = mocks.Stub<IViewController>();
            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);

            var project = mocks.Stub<IProject>();
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(IProject)
                }
            };

            using (var controller = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos))
            {
                controller.ToggleView();

                // Call
                controller.Update(project);

                // Assert
                Assert.AreEqual(1, toolViewList.Count);
                Assert.AreSame(project, toolViewList[0].Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Update_ViewIsClosed_NoDataUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var viewController = mocks.Stub<IViewController>();
            var viewHost = mocks.StrictMock<IViewHost>();

            viewController.Stub(tvc => tvc.ViewHost).Return(viewHost);
            viewHost.Stub(vm => vm.ToolViews).Return(new List<IView>());

            var project = mocks.StrictMock<IProject>();
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(viewCommands, viewController, treeNodeInfos))
            {
                // Call
                controller.Update(project);
            }

            // Assert
            mocks.VerifyAll();
        }
    }
}