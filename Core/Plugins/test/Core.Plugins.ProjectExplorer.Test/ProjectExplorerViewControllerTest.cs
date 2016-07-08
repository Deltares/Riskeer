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
using System.Collections.Generic;
using System.Linq;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Selection;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectExplorerViewControllerTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4)]
        public void Constructor_ArgumentsNull_ThrowsArgumentNullException(int paramNullIndex)
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = paramNullIndex == 0 ? null : mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = paramNullIndex == 1 ? null : mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = paramNullIndex == 2 ? null : mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = paramNullIndex == 3 ? null : mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = paramNullIndex == 4 ? null : Enumerable.Empty<TreeNodeInfo>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoNullArguments_CreatesNewInstance()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();
            mocks.ReplayAll();

            // Call
            using (var result = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Assert
                Assert.IsInstanceOf<IDisposable>(result);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void OpenView_ViewActiveNoOnOpenView_NoProjectExplorerOpened()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.Stub<IDocumentViewController>();
            IViewCommands viewCommands = mocks.Stub<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.Stub<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(true);
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.OpenView();
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void OpenView_ViewInactiveNoOnOpenView_OpenToolViewWithProjectExplorer()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false).Repeat.Twice();
            toolViewController.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Matches(v => true)));

            mocks.ReplayAll();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.OpenView();
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void OpenView_ViewInactiveOnOpenViewSet_OpenToolViewWithProjectExplorerCallsOnOpenView()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false).Repeat.Twice();
            toolViewController.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Matches(v => true)));

            mocks.ReplayAll();

            int activated = 0;
            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                controller.OnOpenView += (sender, args) => activated++;

                // Call
                controller.OpenView();
            }
            // Assert
            Assert.AreEqual(1, activated);
            mocks.VerifyAll();
        }

        [Test]
        public void ToggleView_ViewActive_ViewClosed()
        {
            // Setup
            bool projectExplorerHasBeenOpened = false;

            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.Stub<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>())
                .Return(false)
                .WhenCalled(invocation => invocation.ReturnValue = projectExplorerHasBeenOpened);
            toolViewController.Stub(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf));
            toolViewController.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Is.TypeOf));

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                controller.OpenView();
                projectExplorerHasBeenOpened = true;

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
            bool viewOpened = false;

            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.Stub<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>())
                              .Return(false)
                              .WhenCalled(invocation => invocation.ReturnValue = viewOpened);
            toolViewController.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf));
            toolViewController.Stub(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Is.TypeOf));

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.ToggleView();

                viewOpened = true;
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void IsViewActive_Always_CallsIsToolWindowOpenWithProjectExplorer(bool isOpen)
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(isOpen);

            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                var result = controller.IsViewActive();

                Assert.AreEqual(isOpen, result);
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ViewIsOpen_UpdateData()
        {
            // Setup
            ProjectExplorer projectExplorer = null;
            bool viewOpened = false;

            var mocks = new MockRepository();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.Stub<IApplicationSelection>();

            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            documentViewController.Expect(dvc => dvc.UpdateToolTips());
            
            IToolViewController toolViewController = mocks.Stub<IToolViewController>();
            toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>())
                              .Return(false)
                              .WhenCalled(invocation => invocation.ReturnValue = viewOpened);
            toolViewController.Stub(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf))
                              .WhenCalled(invocation => projectExplorer = (ProjectExplorer)invocation.Arguments[0]);
            toolViewController.Stub(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Is.TypeOf));
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(Project)
                }
            };

            var project = new Project();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                controller.OpenView();
                viewOpened = true;

                // Call
                controller.Update(project);

                // Assert
                Assert.AreSame(project, projectExplorer.Data);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Update_ViewIsClosed_NoDataUpdated()
        {
            // Setup
            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.StrictMock<IDocumentViewController>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IToolViewController toolViewController = mocks.Stub<IToolViewController>();
            toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            var project = new Project();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                // Call
                controller.Update(project);
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Dispose_ViewActive_ViewClosed()
        {
            // Setup
            var viewOpened = false;

            var mocks = new MockRepository();
            IDocumentViewController documentViewController = mocks.Stub<IDocumentViewController>();
            IViewCommands viewCommands = mocks.Stub<IViewCommands>();
            IApplicationSelection applicationSelection = mocks.Stub<IApplicationSelection>();
            applicationSelection.Stub(s => s.SelectionChanged += null).IgnoreArguments();
            applicationSelection.Stub(s => s.SelectionChanged -= null).IgnoreArguments();
            IToolViewController toolViewController = mocks.StrictMock<IToolViewController>();
            toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>())
                              .Return(false)
                              .WhenCalled(invocation => invocation.ReturnValue = viewOpened);
            toolViewController.Stub(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf));
            toolViewController.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Is.TypeOf));
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            using (var controller = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos))
            {
                controller.OpenView();
                viewOpened = true;

                // Call
                controller.Dispose();
            }
            // Assert
            mocks.VerifyAll();
        }
    }
}