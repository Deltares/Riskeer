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
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.TreeView.Test
{
    [TestFixture]
    public class TreeNodeInfoTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var treeNodeInfo = new TreeNodeInfo();

            // Assert
            Assert.IsNull(treeNodeInfo.TagType);
            Assert.IsNull(treeNodeInfo.Text);
            Assert.IsNull(treeNodeInfo.ForeColor);
            Assert.IsNull(treeNodeInfo.Image);
            Assert.IsNull(treeNodeInfo.ContextMenuStrip);
            Assert.IsNull(treeNodeInfo.EnsureVisibleOnCreate);
            Assert.IsNull(treeNodeInfo.ExpandOnCreate);
            Assert.IsNull(treeNodeInfo.ChildNodeObjects);
            Assert.IsNull(treeNodeInfo.CanRename);
            Assert.IsNull(treeNodeInfo.OnNodeRenamed);
            Assert.IsNull(treeNodeInfo.CanRemove);
            Assert.IsNull(treeNodeInfo.OnNodeRemoved);
            Assert.IsNull(treeNodeInfo.CanCheck);
            Assert.IsNull(treeNodeInfo.IsChecked);
            Assert.IsNull(treeNodeInfo.OnNodeChecked);
            Assert.IsNull(treeNodeInfo.CanDrag);
            Assert.IsNull(treeNodeInfo.CanDrop);
            Assert.IsNull(treeNodeInfo.CanInsert);
            Assert.IsNull(treeNodeInfo.OnDrop);
        }

        [Test]
        public void SimpleProperties_SetNewValues_GetNewlySetValues()
        {
            // Setup
            var treeNodeInfo = new TreeNodeInfo();
            Type tagType = typeof(int);
            Func<object, string> text = o => "";
            Func<object, Color> foreColor = o => Color.Azure;
            Func<object, Image> image = o => new Bitmap(16, 16);
            Func<object, object, TreeViewControl, ContextMenuStrip> contextMenuStrip = (o1, o2, tvc) => new ContextMenuStrip();
            Func<object, object, bool> ensureVisibleOnCreate = (o, p) => true;
            Func<object, bool> expandOnCreate = o => true;
            Func<object, object[]> childNodeObjects = o => new object[0];
            Func<object, object, bool> canRename = (o1, o2) => true;
            Action<object, string> onNodeRenamed = (o, newName) => {};
            Func<object, object, bool> canRemove = (o1, o2) => true;
            Action<object, object> onNodeRemoved = (o1, o2) => {};
            Func<object, bool> canCheck = o => true;
            Func<object, bool> isChecked = o => true;
            Action<object, object> onNodeChecked = (o1, o2) => {};
            Func<object, object, bool> canDrag = (o1, o2) => true;
            Func<object, object, bool> canDrop = (o1, o2) => true;
            Func<object, object, bool> canInsert = (o1, o2) => true;
            Action<object, object, object, int, TreeViewControl> onDrop = (o1, o2, o3, index, tvc) => {};

            // Call
            treeNodeInfo.TagType = tagType;
            treeNodeInfo.Text = text;
            treeNodeInfo.ForeColor = foreColor;
            treeNodeInfo.Image = image;
            treeNodeInfo.ContextMenuStrip = contextMenuStrip;
            treeNodeInfo.EnsureVisibleOnCreate = ensureVisibleOnCreate;
            treeNodeInfo.ExpandOnCreate = expandOnCreate;
            treeNodeInfo.ChildNodeObjects = childNodeObjects;
            treeNodeInfo.CanRename = canRename;
            treeNodeInfo.OnNodeRenamed = onNodeRenamed;
            treeNodeInfo.CanRemove = canRemove;
            treeNodeInfo.OnNodeRemoved = onNodeRemoved;
            treeNodeInfo.CanCheck = canCheck;
            treeNodeInfo.IsChecked = isChecked;
            treeNodeInfo.OnNodeChecked = onNodeChecked;
            treeNodeInfo.CanDrag = canDrag;
            treeNodeInfo.CanDrop = canDrop;
            treeNodeInfo.CanInsert = canInsert;
            treeNodeInfo.OnDrop = onDrop;

            // Assert
            Assert.AreEqual(tagType, treeNodeInfo.TagType);
            Assert.AreEqual(text, treeNodeInfo.Text);
            Assert.AreEqual(foreColor, treeNodeInfo.ForeColor);
            Assert.AreEqual(image, treeNodeInfo.Image);
            Assert.AreEqual(contextMenuStrip, treeNodeInfo.ContextMenuStrip);
            Assert.AreEqual(ensureVisibleOnCreate, treeNodeInfo.EnsureVisibleOnCreate);
            Assert.AreEqual(expandOnCreate, treeNodeInfo.ExpandOnCreate);
            Assert.AreEqual(childNodeObjects, treeNodeInfo.ChildNodeObjects);
            Assert.AreEqual(canRename, treeNodeInfo.CanRename);
            Assert.AreEqual(onNodeRenamed, treeNodeInfo.OnNodeRenamed);
            Assert.AreEqual(canRemove, treeNodeInfo.CanRemove);
            Assert.AreEqual(onNodeRemoved, treeNodeInfo.OnNodeRemoved);
            Assert.AreEqual(canCheck, treeNodeInfo.CanCheck);
            Assert.AreEqual(isChecked, treeNodeInfo.IsChecked);
            Assert.AreEqual(onNodeChecked, treeNodeInfo.OnNodeChecked);
            Assert.AreEqual(canDrag, treeNodeInfo.CanDrag);
            Assert.AreEqual(canDrop, treeNodeInfo.CanDrop);
            Assert.AreEqual(canInsert, treeNodeInfo.CanInsert);
            Assert.AreEqual(onDrop, treeNodeInfo.OnDrop);
        }

        [Test]
        public void DefaultGenericConstructor_ExpectedValues()
        {
            // Call
            var treeNodeInfo = new TreeNodeInfo<object>();

            // Assert
            Assert.AreEqual(typeof(object), treeNodeInfo.TagType);
            Assert.IsNull(treeNodeInfo.Text);
            Assert.IsNull(treeNodeInfo.ForeColor);
            Assert.IsNull(treeNodeInfo.Image);
            Assert.IsNull(treeNodeInfo.ContextMenuStrip);
            Assert.IsNull(treeNodeInfo.EnsureVisibleOnCreate);
            Assert.IsNull(treeNodeInfo.ExpandOnCreate);
            Assert.IsNull(treeNodeInfo.ChildNodeObjects);
            Assert.IsNull(treeNodeInfo.CanRename);
            Assert.IsNull(treeNodeInfo.OnNodeRenamed);
            Assert.IsNull(treeNodeInfo.CanRemove);
            Assert.IsNull(treeNodeInfo.OnNodeRemoved);
            Assert.IsNull(treeNodeInfo.CanCheck);
            Assert.IsNull(treeNodeInfo.IsChecked);
            Assert.IsNull(treeNodeInfo.OnNodeChecked);
            Assert.IsNull(treeNodeInfo.CanDrag);
            Assert.IsNull(treeNodeInfo.CanDrop);
            Assert.IsNull(treeNodeInfo.CanInsert);
            Assert.IsNull(treeNodeInfo.OnDrop);
        }

        [Test]
        public void SimpleProperties_GenericTreeNodeInfoSetNewValues_GetNewlySetValues()
        {
            // Setup
            var treeNodeInfo = new TreeNodeInfo<int>();
            Func<int, string> text = o => "";
            Func<int, Color> foreColor = o => Color.Azure;
            Func<int, Image> image = o => new Bitmap(16, 16);
            Func<int, object, TreeViewControl, ContextMenuStrip> contextMenuStrip = (o1, o2, tvc) => new ContextMenuStrip();
            Func<int, object, bool> ensureVisibleOnCreate = (o, p) => true;
            Func<int, bool> expandOnCreate = o => true;
            Func<int, object[]> childNodeObjects = o => new object[0];
            Func<int, object, bool> canRename = (o1, o2) => true;
            Action<int, string> onNodeRenamed = (o, newName) => {};
            Func<int, object, bool> canRemove = (o1, o2) => true;
            Action<int, object> onNodeRemoved = (o1, o2) => {};
            Func<int, bool> canCheck = o => true;
            Func<int, bool> isChecked = o => true;
            Action<int, object> onNodeChecked = (o1, o2) => {};
            Func<int, object, bool> canDrag = (o1, o2) => true;
            Func<object, object, bool> canDrop = (o1, o2) => true;
            Func<object, object, bool> canInsert = (o1, o2) => true;
            Action<object, object, object, int, TreeViewControl> onDrop = (o1, o2, o3, index, tvc) => {};

            // Call
            treeNodeInfo.Text = text;
            treeNodeInfo.ForeColor = foreColor;
            treeNodeInfo.Image = image;
            treeNodeInfo.ContextMenuStrip = contextMenuStrip;
            treeNodeInfo.EnsureVisibleOnCreate = ensureVisibleOnCreate;
            treeNodeInfo.ExpandOnCreate = expandOnCreate;
            treeNodeInfo.ChildNodeObjects = childNodeObjects;
            treeNodeInfo.CanRename = canRename;
            treeNodeInfo.OnNodeRenamed = onNodeRenamed;
            treeNodeInfo.CanRemove = canRemove;
            treeNodeInfo.OnNodeRemoved = onNodeRemoved;
            treeNodeInfo.CanCheck = canCheck;
            treeNodeInfo.IsChecked = isChecked;
            treeNodeInfo.OnNodeChecked = onNodeChecked;
            treeNodeInfo.CanDrag = canDrag;
            treeNodeInfo.CanDrop = canDrop;
            treeNodeInfo.CanInsert = canInsert;
            treeNodeInfo.OnDrop = onDrop;

            // Assert
            Assert.AreEqual(typeof(int), treeNodeInfo.TagType);
            Assert.AreEqual(text, treeNodeInfo.Text);
            Assert.AreEqual(foreColor, treeNodeInfo.ForeColor);
            Assert.AreEqual(image, treeNodeInfo.Image);
            Assert.AreEqual(contextMenuStrip, treeNodeInfo.ContextMenuStrip);
            Assert.AreEqual(ensureVisibleOnCreate, treeNodeInfo.EnsureVisibleOnCreate);
            Assert.AreEqual(expandOnCreate, treeNodeInfo.ExpandOnCreate);
            Assert.AreEqual(childNodeObjects, treeNodeInfo.ChildNodeObjects);
            Assert.AreEqual(canRename, treeNodeInfo.CanRename);
            Assert.AreEqual(onNodeRenamed, treeNodeInfo.OnNodeRenamed);
            Assert.AreEqual(canRemove, treeNodeInfo.CanRemove);
            Assert.AreEqual(onNodeRemoved, treeNodeInfo.OnNodeRemoved);
            Assert.AreEqual(canCheck, treeNodeInfo.CanCheck);
            Assert.AreEqual(isChecked, treeNodeInfo.IsChecked);
            Assert.AreEqual(onNodeChecked, treeNodeInfo.OnNodeChecked);
            Assert.AreEqual(canDrag, treeNodeInfo.CanDrag);
            Assert.AreEqual(canDrop, treeNodeInfo.CanDrop);
            Assert.AreEqual(canInsert, treeNodeInfo.CanInsert);
            Assert.AreEqual(onDrop, treeNodeInfo.OnDrop);
        }

        [Test]
        public void ImplicitOperator_WithNoMethodsSet_InfoFullyConverted()
        {
            var genericTreeNodeInfo = new TreeNodeInfo<int>();

            // Precondition
            Assert.IsInstanceOf<TreeNodeInfo<int>>(genericTreeNodeInfo);

            // Call
            TreeNodeInfo treeNodeInfo = genericTreeNodeInfo;

            // Assert
            Assert.AreEqual(typeof(int), treeNodeInfo.TagType);
            Assert.IsNull(treeNodeInfo.Text);
            Assert.IsNull(treeNodeInfo.ForeColor);
            Assert.IsNull(treeNodeInfo.Image);
            Assert.IsNull(treeNodeInfo.ContextMenuStrip);
            Assert.IsNull(treeNodeInfo.EnsureVisibleOnCreate);
            Assert.IsNull(treeNodeInfo.ExpandOnCreate);
            Assert.IsNull(treeNodeInfo.ChildNodeObjects);
            Assert.IsNull(treeNodeInfo.CanRename);
            Assert.IsNull(treeNodeInfo.OnNodeRenamed);
            Assert.IsNull(treeNodeInfo.CanRemove);
            Assert.IsNull(treeNodeInfo.OnNodeRemoved);
            Assert.IsNull(treeNodeInfo.CanCheck);
            Assert.IsNull(treeNodeInfo.IsChecked);
            Assert.IsNull(treeNodeInfo.OnNodeChecked);
            Assert.IsNull(treeNodeInfo.CanDrag);
            Assert.IsNull(treeNodeInfo.CanDrop);
            Assert.IsNull(treeNodeInfo.CanInsert);
            Assert.IsNull(treeNodeInfo.OnDrop);
        }

        [Test]
        public void ImplicitOperator_WithAllMethodsSet_InfoFullyConverted()
        {
            // Setup
            var onDropCounter = 0;
            var onNodeRenamedCounter = 0;
            var onNodeRemovedCounter = 0;
            var onNodeCheckedCounter = 0;

            var genericTreeNodeInfo = new TreeNodeInfo<int>
            {
                Text = o => "text",
                ForeColor = o => Color.Azure,
                Image = o => new Bitmap(16, 16),
                ContextMenuStrip = (o1, o2, tvc) => new ContextMenuStrip
                {
                    Items =
                    {
                        new ToolStripButton()
                    }
                },
                EnsureVisibleOnCreate = (o, p) => true,
                ExpandOnCreate = o => true,
                ChildNodeObjects = o => new[]
                {
                    new object()
                },
                CanRename = (o1, o2) => true,
                OnNodeRenamed = (o, newName) => { onNodeRenamedCounter++; },
                CanRemove = (o1, o2) => true,
                OnNodeRemoved = (o1, o2) => { onNodeRemovedCounter++; },
                CanCheck = o => true,
                IsChecked = o => true,
                OnNodeChecked = (o1, o2) => { onNodeCheckedCounter++; },
                CanDrag = (o1, o2) => true,
                CanDrop = (o1, o2) => true,
                CanInsert = (o1, o2) => true,
                OnDrop = (o1, o2, o3, index, tvc) => { onDropCounter++; }
            };

            // Precondition
            Assert.IsInstanceOf<TreeNodeInfo<int>>(genericTreeNodeInfo);

            // Call
            TreeNodeInfo treeNodeInfo = genericTreeNodeInfo;

            // Assert
            using (var treeViewControl = new TreeViewControl())
            using (ContextMenuStrip contextMenuStrip = treeNodeInfo.ContextMenuStrip(0, 1, treeViewControl))
            {
                Assert.AreEqual(1, contextMenuStrip.Items.Count);
                treeNodeInfo.OnDrop(0, 1, 2, 3, treeViewControl);
                Assert.AreEqual(1, onDropCounter);
            }

            Assert.AreEqual(typeof(int), treeNodeInfo.TagType);
            Assert.AreEqual("text", treeNodeInfo.Text(0));
            Assert.AreEqual(Color.Azure, treeNodeInfo.ForeColor(0));
            Assert.AreEqual(16, treeNodeInfo.Image(0).Height);
            Assert.IsTrue(treeNodeInfo.EnsureVisibleOnCreate(0, 1));
            Assert.IsTrue(treeNodeInfo.ExpandOnCreate(0));
            Assert.AreEqual(1, treeNodeInfo.ChildNodeObjects(0).Length);
            Assert.IsTrue(treeNodeInfo.CanRename(0, 1));
            Assert.IsTrue(treeNodeInfo.CanRemove(0, 1));
            Assert.IsTrue(treeNodeInfo.CanCheck(0));
            Assert.IsTrue(treeNodeInfo.IsChecked(0));
            Assert.IsTrue(treeNodeInfo.CanDrag(0, 1));
            Assert.IsTrue(treeNodeInfo.CanDrop(0, 1));
            Assert.IsTrue(treeNodeInfo.CanInsert(0, 1));

            treeNodeInfo.OnNodeRenamed(0, "newName");
            Assert.AreEqual(1, onNodeRenamedCounter);

            treeNodeInfo.OnNodeRemoved(0, 1);
            Assert.AreEqual(1, onNodeRemovedCounter);

            treeNodeInfo.OnNodeChecked(0, 1);
            Assert.AreEqual(1, onNodeCheckedCounter);
        }
    }
}