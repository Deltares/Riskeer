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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Selection;
using Core.Common.TestUtil;
using Core.Common.Utils.Reflection;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Plugins.ProjectExplorer.Test
{
    [TestFixture]
    public class ProjectExplorerTest : NUnitFormTest
    {
        [Test]
        [TestCase(0)]
        [TestCase(1)]
        [TestCase(2)]
        public void Constructor_ArgumentsNull_ThrowsArgumentNullException(int paramNullIndex)
        {
            // Setup
            var mocks = new MockRepository();
            IApplicationSelection applicationSelection = paramNullIndex == 0 ? null : mocks.StrictMock<IApplicationSelection>();
            IViewCommands viewCommands = paramNullIndex == 1 ? null : mocks.StrictMock<IViewCommands>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = paramNullIndex == 2 ? null : Enumerable.Empty<TreeNodeInfo>();

            mocks.ReplayAll();

            // Call
            TestDelegate test = () => new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoNullArguments_CreatesNewInstanceApplicationSelectionEventBound()
        {
            // Setup
            var mocks = new MockRepository();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = Enumerable.Empty<TreeNodeInfo>();

            mocks.ReplayAll();

            // Call
            using (var explorer = new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos))
            {
                // Assert
                Assert.IsInstanceOf<IProjectExplorer>(explorer);
                Assert.IsInstanceOf<UserControl>(explorer);
                Assert.IsNull(explorer.Data);
                Assert.IsInstanceOf<TreeViewControl>(explorer.TreeViewControl);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Data_Always_SetTreeViewControlData()
        {
            // Setup
            var mocks = new MockRepository();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(Project)
                }
            };

            mocks.ReplayAll();

            var project = new Project();

            using (var explorer = new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos))
            {
                // Call
                explorer.Data = project;

                // Assert
                Assert.AreSame(project, explorer.TreeViewControl.Data);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ProjectDataDeleted_Always_CallsRemoveAllViewsForItemWithTag()
        {
            // Setup
            var mocks = new MockRepository();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();
            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(Project),
                    CanRemove = (item, parent) => true
                }
            };

            var project = new Project();

            viewCommands.Expect(vc => vc.RemoveAllViewsForItem(project));

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBox.ClickOk();
            };

            using (var explorer = new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos)
            {
                Data = project
            })
            {
                // Call
                explorer.TreeViewControl.TryRemoveNodeForData(project);
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void TreeViewSelectedNodeChanged_Always_SetsApplicationSelection()
        {
            // Setup
            var mocks = new MockRepository();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();

            var project = new Project();
            var stringA = "testA";
            var stringB = "testB";

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(Project),
                    ChildNodeObjects = o => new[]
                    {
                        stringA,
                        stringB
                    }
                },
                new TreeNodeInfo
                {
                    TagType = typeof(String)
                }
            };

            using (mocks.Ordered())
            {
                applicationSelection.Expect(a => a.Selection = project);
                applicationSelection.Expect(a => a.Selection = stringA);
            }

            mocks.ReplayAll();

            using (var explorer = new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos)
            {
                Data = project
            })
            {
                WindowsFormsTestHelper.Show(explorer.TreeViewControl);

                // Precondition
                Assert.AreNotSame(explorer.TreeViewControl.SelectedData, stringA);

                // Call
                explorer.TreeViewControl.TrySelectNodeForData(stringA);

                // Assert
                Assert.AreSame(explorer.TreeViewControl.SelectedData, stringA);
            }
            WindowsFormsTestHelper.CloseAll();
            mocks.VerifyAll();
        }

        [Test]
        [RequiresSTA]
        public void TreeViewEnterPress_Always_OpenViewForSelection()
        {
            // Setup
            var treeIdentifier = "SomeName";
            var formIdentifier = "SomeForm";
            var mocks = new MockRepository();
            IApplicationSelection applicationSelection = mocks.StrictMock<IApplicationSelection>();
            IViewCommands viewCommands = mocks.StrictMock<IViewCommands>();

            var project = new Project();

            IEnumerable<TreeNodeInfo> treeNodeInfos = new[]
            {
                new TreeNodeInfo
                {
                    TagType = typeof(Project)
                }
            };

            using (mocks.Ordered())
            {
                applicationSelection.Expect(a => a.Selection = project);
                viewCommands.Expect(a => a.OpenViewForSelection());
            }

            mocks.ReplayAll();

            using (var explorer = new ProjectExplorer(applicationSelection, viewCommands, treeNodeInfos)
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

                TypeUtils.GetField<TreeView>(explorer.TreeViewControl, "treeView").Name = treeIdentifier;

                // Precondition
                Assert.AreSame(explorer.TreeViewControl.SelectedData, project);

                var tester = new TreeViewTester(treeIdentifier);

                // Call
                tester.DoubleClick();
            }
            // Assert
            mocks.VerifyAll();
        }
    }
}