// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Properties;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;

namespace Core.Common.Gui.Test.ContextMenu
{
    [TestFixture]
    public class TreeViewContextMenuItemFactoryTest : NUnitFormTest
    {
        private MockRepository mocks;

        public override void Setup()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Constructor_WithoutDataObject_ThrowsArgumentNullException()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => new TreeViewContextMenuItemFactory(null, treeViewControl);

                // Assert
                string message = Assert.Throws<ArgumentNullException>(test).Message;
                StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_data, message);
                StringAssert.EndsWith("dataObject", message);
            }
        }

        [Test]
        public void Constructor_WithoutTreeViewControl_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new TreeViewContextMenuItemFactory(new object(), null);

            // Assert
            string message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(Resources.ContextMenuItemFactory_Can_not_create_context_menu_items_without_tree_view_control, message);
            StringAssert.EndsWith("treeViewControl", message);
        }

        [Test]
        public void Constructor_WithAllInput_DoesNotThrow()
        {
            // Setup
            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                TestDelegate test = () => new TreeViewContextMenuItemFactory(new object(), treeViewControl);

                // Assert
                Assert.DoesNotThrow(test);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDeleteItem_DependingOnCanRemoveNodeForData_ItemWithDeleteFunctionWillBeEnabled(bool canDelete)
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var nodeData = new object();

            treeViewControl.Expect(tvc => tvc.CanRemoveNodeForData(nodeData)).Return(canDelete);

            if (canDelete)
            {
                treeViewControl.Expect(tvc => tvc.TryRemoveNodeForData(nodeData));
            }

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(nodeData, treeViewControl);

            // Call
            ToolStripItem item = factory.CreateDeleteItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Delete, item.Text);
            Assert.AreEqual(Resources.Delete_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.DeleteIcon, item.Image);
            Assert.AreEqual(canDelete, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateDeleteChildrenItem_DependingOnCanRemoveChildNodesOfData_ItemWithDeleteChildrenFunctionWillBeEnabled(bool canDelete)
        {
            // Setup
            var nodeData = new object();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            treeViewControl.Expect(tvc => tvc.CanRemoveChildNodesOfData(nodeData)).Return(canDelete).Repeat.AtLeastOnce();

            if (canDelete)
            {
                treeViewControl.Expect(tvc => tvc.TryRemoveChildNodesOfData(nodeData));
            }

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(nodeData, treeViewControl);

            // Call
            ToolStripItem item = factory.CreateDeleteChildrenItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.DeleteChildren, item.Text);
            string expectedTooltip = canDelete
                                         ? "Verwijder alle onderliggende elementen van dit element."
                                         : "Er zijn geen onderliggende elementen om te verwijderen.";
            Assert.AreEqual(expectedTooltip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.DeleteChildrenIcon, item.Image);
            Assert.AreEqual(canDelete, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateRenameItem_DependingOnCanRenameNodeForData_ItemWithDeleteFunctionWillBeEnabled(bool canRename)
        {
            // Setup
            var dataObject = new object();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(dataObject)).Return(canRename);

            if (canRename)
            {
                treeViewControl.Expect(tvc => tvc.TryRenameNodeForData(dataObject));
            }

            mocks.ReplayAll();

            var factory = new TreeViewContextMenuItemFactory(dataObject, treeViewControl);

            // Call
            ToolStripItem item = factory.CreateRenameItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Rename, item.Text);
            Assert.AreEqual(Resources.Rename_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.RenameIcon, item.Image);
            Assert.AreEqual(canRename, item.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateExpandAllItem_DependingOnCanExpandOrCollapseForData_ItemWithExpandFunctionWillBeEnabled(bool hasChildren)
        {
            // Setup
            const string dataObject = "string";
            var treeNodeInfo = new TreeNodeInfo<string>();
            var childTreeNodeInfo = new TreeNodeInfo<object>();
            var treeViewControl = new TreeViewControl();

            if (hasChildren)
            {
                treeNodeInfo.ChildNodeObjects = o => new object[]
                {
                    10.0
                };
            }

            var factory = new TreeViewContextMenuItemFactory(dataObject, treeViewControl);

            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            treeViewControl.Data = dataObject;

            // Call
            ToolStripItem item = factory.CreateExpandAllItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Expand_all, item.Text);
            Assert.AreEqual(Resources.Expand_all_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.ExpandAllIcon, item.Image);
            Assert.AreEqual(hasChildren, item.Enabled);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CreateCollapseAllItem_DependingOnCanExpandOrCollapseForData_ItemWithCollapseFunctionWillBeEnabled(bool hasChildren)
        {
            // Setup
            const string dataObject = "string";
            var treeNodeInfo = new TreeNodeInfo<string>();
            var childTreeNodeInfo = new TreeNodeInfo<object>();
            var treeViewControl = new TreeViewControl();

            if (hasChildren)
            {
                treeNodeInfo.ChildNodeObjects = o => new object[]
                {
                    10.0
                };
            }

            var factory = new TreeViewContextMenuItemFactory(dataObject, treeViewControl);

            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            treeViewControl.Data = dataObject;

            // Call
            ToolStripItem item = factory.CreateCollapseAllItem();
            item.PerformClick();

            // Assert
            Assert.AreEqual(Resources.Collapse_all, item.Text);
            Assert.AreEqual(Resources.Collapse_all_ToolTip, item.ToolTipText);
            TestHelper.AssertImagesAreEqual(Resources.CollapseAllIcon, item.Image);
            Assert.AreEqual(hasChildren, item.Enabled);
        }
    }
}