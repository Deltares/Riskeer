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
            var tagType = typeof(int);
            Func<object, string> text = o => "";
            Func<object, Color> foreColor = o => Color.Azure;
            Func<object, Image> image = o => new Bitmap(16, 16);
            Func<object, object, TreeViewControl, ContextMenuStrip> contextMenuStrip = (o1, o2, tvc) => new ContextMenuStrip();
            Func<object, bool> ensureVisibleOnCreate = o => true;
            Func<object, object[]> childNodeObjects = o => new object[0];
            Func<object, object, bool> canRename = (o1, o2) => true;
            Action<object, string> onNodeRenamed = (o, newName) => { };
            Func<object, object, bool> canRemove = (o1, o2) => true;
            Action<object, object> onNodeRemoved = (o1, o2) => { };
            Func<object, bool> canCheck = o => true;
            Func<object, bool> isChecked = o => true;
            Action<object, object> onNodeChecked = (o1, o2) => { };
            Func<object, object, bool> canDrag = (o1, o2) => true;
            Func<object, object, bool> canDrop = (o1, o2) => true;
            Func<object, object, bool> canInsert = (o1, o2) => true;
            Action<object, object, object, int, TreeViewControl> onDrop = (o1, o2, o3, index, tvc) => { };

            // Call
            treeNodeInfo.TagType = tagType;
            treeNodeInfo.Text = text;
            treeNodeInfo.ForeColor = foreColor;
            treeNodeInfo.Image = image;
            treeNodeInfo.ContextMenuStrip = contextMenuStrip;
            treeNodeInfo.EnsureVisibleOnCreate = ensureVisibleOnCreate;
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
            var treeNodeInfo = new TreeNodeInfo();
            var tagType = typeof(int);
            Func<object, string> text = o => "";
            Func<object, Color> foreColor = o => Color.Azure;
            Func<object, Image> image = o => new Bitmap(16, 16);
            Func<object, object, TreeViewControl, ContextMenuStrip> contextMenuStrip = (o1, o2, tvc) => new ContextMenuStrip();
            Func<object, bool> ensureVisibleOnCreate = o => true;
            Func<object, object[]> childNodeObjects = o => new object[0];
            Func<object, object, bool> canRename = (o1, o2) => true;
            Action<object, string> onNodeRenamed = (o, newName) => { };
            Func<object, object, bool> canRemove = (o1, o2) => true;
            Action<object, object> onNodeRemoved = (o1, o2) => { };
            Func<object, bool> canCheck = o => true;
            Func<object, bool> isChecked = o => true;
            Action<object, object> onNodeChecked = (o1, o2) => { };
            Func<object, object, bool> canDrag = (o1, o2) => true;
            Func<object, object, bool> canDrop = (o1, o2) => true;
            Func<object, object, bool> canInsert = (o1, o2) => true;
            Action<object, object, object, int, TreeViewControl> onDrop = (o1, o2, o3, index, tvc) => { };

            // Call
            treeNodeInfo.TagType = tagType;
            treeNodeInfo.Text = text;
            treeNodeInfo.ForeColor = foreColor;
            treeNodeInfo.Image = image;
            treeNodeInfo.ContextMenuStrip = contextMenuStrip;
            treeNodeInfo.EnsureVisibleOnCreate = ensureVisibleOnCreate;
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
    }
}
