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
using System.Linq;
using Core.Common.Controls.Commands;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Selection;
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

            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var toolViewController = mocks.StrictMock<IToolViewController>();

            mocks.ReplayAll();

            var treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            var explorerViewController = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos);

            // Call
            var command = new ToggleProjectExplorerCommand(explorerViewController);

            // Assert
            Assert.IsInstanceOf<ICommand>(command);
            Assert.IsTrue(command.Enabled);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Checked_Always_CallsViewControllerIsViewActive(bool isViewOpen)
        {
            // Setup
            var mocks = new MockRepository();

            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();
            var toolViewController = mocks.StrictMock<IToolViewController>();
            toolViewController.Expect(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(isViewOpen);

            mocks.ReplayAll();

            var treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            var explorerViewController = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos);

            var command = new ToggleProjectExplorerCommand(explorerViewController);

            // Call
            var result = command.Checked;

            // Assert
            Assert.AreEqual(isViewOpen, result);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void Execute_Always_CallsViewControllerToggleView(bool isViewOpen)
        {
            // Setup
            var mocks = new MockRepository();

            var documentViewController = mocks.StrictMock<IDocumentViewController>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var applicationSelection = mocks.StrictMock<IApplicationSelection>();

            var toolViewController = mocks.StrictMock<IToolViewController>();
            bool explorerViewActuallyOpened = false;
            if (isViewOpen)
            {
                toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>())
                    .Return(false)
                    .WhenCalled(invocation => invocation.ReturnValue = explorerViewActuallyOpened);
                toolViewController.Stub(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf));
                toolViewController.Expect(tvc => tvc.CloseToolView(Arg<ProjectExplorer>.Is.TypeOf));
            }
            else
            {
                toolViewController.Expect(tvc => tvc.OpenToolView(Arg<ProjectExplorer>.Is.TypeOf));
                toolViewController.Stub(tvc => tvc.IsToolWindowOpen<ProjectExplorer>()).Return(false);
            }

            mocks.ReplayAll();

            var treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            var explorerViewController = new ProjectExplorerViewController(documentViewController, viewCommands, applicationSelection, toolViewController, treeNodeInfos);
            if (isViewOpen)
            {
                explorerViewController.OpenView();
                explorerViewActuallyOpened = true;
            }

            var command = new ToggleProjectExplorerCommand(explorerViewController);

            // Call
            command.Execute();

            // Assert
            mocks.VerifyAll();
        }
    }
}