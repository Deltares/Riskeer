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

using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class ContextMenuBuilderTest
    {
        private MockRepository mocks;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_NoApplicationFeatureCommands_ThrowsContextMenuBuilderException()
        {
            // Setup
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => new ContextMenuBuilder(null,
                                                                 importCommandHandlerMock,
                                                                 exportCommandHandlerMock,
                                                                 viewCommandsMock,
                                                                 new object(),
                                                                 treeViewControl);

                // Assert
                string message = Assert.Throws<ContextMenuBuilderException>(test).Message;
                Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoImportCommandHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                 null,
                                                                 exportCommandHandlerMock,
                                                                 viewCommandsMock,
                                                                 new object(),
                                                                 treeViewControl);

                // Assert
                string message = Assert.Throws<ContextMenuBuilderException>(test).Message;
                Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoExportCommandHandler_ThrowsContextMenuBuilderException()
        {
            // Setup
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                 importCommandHandlerMock,
                                                                 null,
                                                                 viewCommandsMock,
                                                                 new object(),
                                                                 treeViewControl);

                // Assert
                string message = Assert.Throws<ContextMenuBuilderException>(test).Message;
                Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoViewCommands_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommandsMockMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMockMock,
                                                                 importCommandHandlerMock,
                                                                 exportCommandHandlerMock,
                                                                 null,
                                                                 new object(),
                                                                 treeViewControl);

                // Assert
                string message = Assert.Throws<ContextMenuBuilderException>(test).Message;
                Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoDataObject_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                 importCommandHandlerMock,
                                                                 exportCommandHandlerMock,
                                                                 viewCommandsMock,
                                                                 null,
                                                                 treeViewControl);

                // Assert
                string message = Assert.Throws<ContextMenuBuilderException>(test).Message;
                Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Constructor_NoTreeViewControl_ThrowsContextMenuBuilderException()
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            // Call
            TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                             importCommandHandlerMock,
                                                             exportCommandHandlerMock,
                                                             viewCommandsMock,
                                                             new object(),
                                                             null);

            // Assert
            string message = Assert.Throws<ContextMenuBuilderException>(test).Message;
            Assert.AreEqual(Resources.ContextMenuBuilder_ContextMenuBuilder_Cannot_create_instances_of_factories, message);
        }

        [Test]
        public void Constructor_ParamsSet_DoesNotThrow()
        {
            // Setup
            var applicationFeatureCommandsMockMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => new ContextMenuBuilder(applicationFeatureCommandsMockMock,
                                                                 importCommandHandlerMock,
                                                                 exportCommandHandlerMock,
                                                                 viewCommandsMock,
                                                                 new object(),
                                                                 treeViewControl);

                // Assert
                Assert.DoesNotThrow(test);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Build_NothingAdded_EmptyContextMenu()
        {
            // Setup
            var applicationFeatureCommandsMockMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMockMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     new object(),
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                CollectionAssert.IsEmpty(result.Items);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddRenameItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var dataObject = new object();
            using (var treeViewControl = new TreeViewControl())
            {
                var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
                var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
                var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
                var viewCommandsMock = mocks.StrictMock<IViewCommands>();
                var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo<object>>();

                treeNodeInfoMock.CanRename = (data, parentData) => data == dataObject;

                treeViewControl.RegisterTreeNodeInfo(treeNodeInfoMock);

                treeViewControl.Data = dataObject;

                mocks.ReplayAll();

                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     dataObject,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddRenameItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Rename, Resources.Rename_ToolTip, Resources.RenameIcon);

                mocks.VerifyAll();
            }
        }

        [Test]
        public void AddDeleteItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var nodeData = "string";
            var parentData = new object();
            using (var treeViewControl = new TreeViewControl())
            {
                var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
                var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
                var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
                var viewCommandsMock = mocks.StrictMock<IViewCommands>();
                var treeNodeInfoMock = mocks.StrictMock<TreeNodeInfo<string>>();
                var parentTreeNodeInfoMock = mocks.StrictMock<TreeNodeInfo<object>>();

                treeNodeInfoMock.CanRemove = (nd, pnd) => nd == nodeData && pnd == parentData;
                parentTreeNodeInfoMock.ChildNodeObjects = nd => new object[]
                {
                    nodeData
                };

                treeViewControl.RegisterTreeNodeInfo(treeNodeInfoMock);
                treeViewControl.RegisterTreeNodeInfo(parentTreeNodeInfoMock);

                treeViewControl.Data = parentData;

                mocks.ReplayAll();

                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddDeleteItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Delete, Resources.Delete_ToolTip, Resources.DeleteIcon);

                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddRemoveAllChildrenItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var dataObject = new object();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            
            treeViewControlMock.Expect(tvc => tvc.CanRemoveChildNodesOfData(dataObject)).Return(hasChildren);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                 importCommandHandlerMock,
                                                 exportCommandHandlerMock,
                                                 viewCommandsMock,
                                                 dataObject,
                                                 treeViewControlMock);

            // Call
            ContextMenuStrip result = builder.AddDeleteChildrenItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                            Resources.DeleteChildren,
                                                            Resources.DeleteChildren_ToolTip,
                                                            Resources.DeleteChildrenIcon,
                                                            hasChildren);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExpandAllItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var dataObject = new object();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            treeViewControlMock.Expect(tvc => tvc.CanExpandOrCollapseForData(dataObject)).Return(hasChildren);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                 importCommandHandlerMock,
                                                 exportCommandHandlerMock,
                                                 viewCommandsMock,
                                                 dataObject,
                                                 treeViewControlMock);

            // Call
            ContextMenuStrip result = builder.AddExpandAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Expand_all, Resources.Expand_all_ToolTip, Resources.ExpandAllIcon, hasChildren);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddCollapseAllItem_WhenBuild_ItemAddedToContextMenu(bool hasChildren)
        {
            // Setup
            var dataObject = new object();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            treeViewControlMock.Expect(tvc => tvc.CanExpandOrCollapseForData(dataObject)).Return(hasChildren);

            mocks.ReplayAll();

            var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                 importCommandHandlerMock,
                                                 exportCommandHandlerMock,
                                                 viewCommandsMock,
                                                 dataObject,
                                                 treeViewControlMock);

            // Call
            ContextMenuStrip result = builder.AddCollapseAllItem().Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Collapse_all, Resources.Collapse_all_ToolTip, Resources.CollapseAllIcon, hasChildren);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddOpenItem_WhenBuild_ItemAddedToContextMenu(bool hasViewForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            viewCommandsMock.Expect(ch => ch.CanOpenViewFor(nodeData)).Return(hasViewForNodeData);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddOpenItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Open, Resources.Open_ToolTip, Resources.OpenIcon, hasViewForNodeData);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddExportItem_WhenBuild_ItemAddedToContextMenu(bool hasExportersForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            exportCommandHandlerMock.Expect(ch => ch.CanExportFrom(nodeData)).Return(hasExportersForNodeData);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddExportItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Export, Resources.Export_ToolTip, Resources.ExportIcon, hasExportersForNodeData);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddImportItem_WhenBuild_ItemAddedToContextMenu(bool hasImportersForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            importCommandHandlerMock.Expect(ch => ch.CanImportOn(nodeData)).Return(hasImportersForNodeData);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddImportItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Import, Resources.Import_ToolTip, Resources.ImportIcon, hasImportersForNodeData);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddPropertiesItem_WhenBuild_ItemAddedToContextMenu(bool hasPropertiesForNodeData)
        {
            // Setup
            var nodeData = new object();

            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            applicationFeatureCommandsMock.Expect(ch => ch.CanShowPropertiesFor(nodeData)).Return(hasPropertiesForNodeData);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     nodeData,
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddPropertiesItem().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0, Resources.Properties, Resources.Properties_ToolTip, Resources.PropertiesHS, hasPropertiesForNodeData);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock, exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     new object(),
                                                     treeViewControl);
                var item = new StrictContextMenuItem(null, null, null, null);

                // Call
                ContextMenuStrip result = builder.AddCustomItem(item).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                Assert.AreSame(item, result.Items[0]);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddSeparator_NoOtherItemsWhenBuild_EmptyContextMenu()
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     new object(),
                                                     treeViewControl);

                // Call
                ContextMenuStrip result = builder.AddSeparator().Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.IsEmpty(result.Items);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorAddedAtStart_SeparatorsNotAdded(int count)
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     new object(),
                                                     treeViewControl);

                var someItem = new StrictContextMenuItem(null, null, null, null);

                // Call
                for (int i = 0; i < count; i++)
                {
                    builder.AddSeparator();
                }
                ContextMenuStrip result = builder.AddCustomItem(someItem).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                Assert.IsInstanceOf<ToolStripMenuItem>(result.Items[0]);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorsAddedInBetweenItems_OneSeparatorAdded(int count)
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     new object(),
                                                     treeViewControl);

                var someItem = new StrictContextMenuItem(null, null, null, null);
                var someOtherItem = new StrictContextMenuItem(null, null, null, null);

                builder.AddCustomItem(someItem);

                // Call
                for (int i = 0; i < count; i++)
                {
                    builder.AddSeparator();
                }
                ContextMenuStrip result = builder.AddCustomItem(someOtherItem).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(3, result.Items.Count);

                Assert.IsInstanceOf<ToolStripSeparator>(result.Items[1]);
                Assert.IsInstanceOf<ToolStripMenuItem>(result.Items[2]);
            }
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(10)]
        public void AddSeparator_SeparatorsAddedAtEnd_SeparatorsNotAdded(int count)
        {
            // Setup
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var builder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                     importCommandHandlerMock,
                                                     exportCommandHandlerMock,
                                                     viewCommandsMock,
                                                     new object(),
                                                     treeViewControl);

                builder.AddCustomItem(new StrictContextMenuItem(null, null, null, null));

                // Call
                for (int i = 0; i < count; i++)
                {
                    builder.AddSeparator();
                }
                ContextMenuStrip result = builder.Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
            }
            mocks.VerifyAll();
        }
    }
}