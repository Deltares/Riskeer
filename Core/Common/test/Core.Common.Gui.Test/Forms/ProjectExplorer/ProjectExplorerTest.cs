﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Threading;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.ProjectExplorer;
using Core.Common.TestUtil;
using Core.Common.Util.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.Forms.ProjectExplorer
{
    [TestFixture]
    public class ProjectExplorerTest : NUnitFormTest
    {
        [Test]
        public void Constructor_ViewCommandsNull_ThrowsArgumentNullException()
        {
            // Call
            void Call() => new Gui.Forms.ProjectExplorer.ProjectExplorer(null, Enumerable.Empty<TreeNodeInfo>());

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("viewCommands", exception.ParamName);
        }

        [Test]
        public void Constructor_TreeNodeInfosNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            mocks.ReplayAll();

            // Call
            void Call() => new Gui.Forms.ProjectExplorer.ProjectExplorer(viewCommands, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(Call);
            Assert.AreEqual("treeNodeInfos", exception.ParamName);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_ExpectedValues()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            // Call
            using (var explorer = new Gui.Forms.ProjectExplorer.ProjectExplorer(viewCommands, Enumerable.Empty<TreeNodeInfo>()))
            {
                // Assert
                Assert.IsInstanceOf<IProjectExplorer>(explorer);
                Assert.IsInstanceOf<UserControl>(explorer);
                Assert.IsNull(explorer.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void Data_Always_SetTreeViewControlData()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var project = mocks.Stub<IProject>();
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(IProject)
                }
            };

            using (var explorer = new Gui.Forms.ProjectExplorer.ProjectExplorer(viewCommands, treeNodeInfos))
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(explorer, "treeViewControl");

                // Call
                explorer.Data = project;

                // Assert
                Assert.AreSame(project, treeViewControl.Data);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ProjectDataDeleted_Always_CallsRemoveAllViewsForItemWithTag()
        {
            // Setup
            var mocks = new MockRepository();
            var project = mocks.Stub<IProject>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(project));
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(IProject),
                    CanRemove = (item, parent) => true
                }
            };

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBox.ClickOk();
            };

            using (var explorer = new Gui.Forms.ProjectExplorer.ProjectExplorer(viewCommands, treeNodeInfos)
            {
                Data = project
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(explorer, "treeViewControl");

                // Call
                treeViewControl.TryRemoveNodeForData(project);
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TreeViewSelectedNodeChanged_Always_SelectionChangedFired()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var project = mocks.Stub<IProject>();
            mocks.ReplayAll();

            const string stringA = "testA";
            const string stringB = "testB";

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(IProject),
                    ChildNodeObjects = o => new object[]
                    {
                        stringA,
                        stringB
                    }
                },
                new TreeNodeInfo
                {
                    TagType = typeof(string)
                }
            };

            using (var explorer = new Gui.Forms.ProjectExplorer.ProjectExplorer(viewCommands, treeNodeInfos)
            {
                Data = project
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(explorer, "treeViewControl");

                WindowsFormsTestHelper.Show(treeViewControl);

                var selectionChangedCount = 0;
                explorer.SelectionChanged += (sender, args) => selectionChangedCount++;

                // Call
                treeViewControl.TrySelectNodeForData(stringA);

                // Assert
                Assert.AreEqual(1, selectionChangedCount);
            }

            WindowsFormsTestHelper.CloseAll();
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void TreeViewEnterPress_Always_OpenViewForSelection()
        {
            // Setup
            const string treeIdentifier = "SomeName";
            const string formIdentifier = "SomeForm";

            var mocks = new MockRepository();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            viewCommands.Expect(a => a.OpenViewForSelection());
            var project = mocks.Stub<IProject>();
            mocks.ReplayAll();

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(IProject)
                }
            };

            using (var explorer = new Gui.Forms.ProjectExplorer.ProjectExplorer(viewCommands, treeNodeInfos)
            {
                Data = project
            })
            {
                var form = new Form
                {
                    Name = formIdentifier
                };
                form.Controls.Add(explorer);
                form.Show();

                var treeViewControl = TypeUtils.GetField<TreeViewControl>(explorer, "treeViewControl");

                TypeUtils.GetField<TreeView>(treeViewControl, "treeView").Name = treeIdentifier;

                // Precondition
                Assert.AreSame(treeViewControl.SelectedData, project);

                var tester = new TreeViewTester(treeIdentifier);

                // Call
                tester.DoubleClick();
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Selection_Always_ReturnsSelectedNodeData()
        {
            // Setup
            var mocks = new MockRepository();
            var viewCommands = mocks.Stub<IViewCommands>();
            var project = mocks.Stub<IProject>();
            mocks.ReplayAll();

            const string stringA = "testA";
            const string stringB = "testB";

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(IProject),
                    ChildNodeObjects = o => new object[]
                    {
                        stringA,
                        stringB
                    }
                },
                new TreeNodeInfo
                {
                    TagType = typeof(string)
                }
            };

            using (var explorer = new Gui.Forms.ProjectExplorer.ProjectExplorer(viewCommands, treeNodeInfos)
            {
                Data = project
            })
            {
                var treeViewControl = TypeUtils.GetField<TreeViewControl>(explorer, "treeViewControl");

                WindowsFormsTestHelper.Show(treeViewControl);
                treeViewControl.TrySelectNodeForData(stringA);

                // Call
                object selection = explorer.Selection;

                // Assert
                Assert.AreSame(stringA, selection);
            }

            WindowsFormsTestHelper.CloseAll();
            mocks.VerifyAll();
        }
    }
}