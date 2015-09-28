using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using DelftTools.Controls;
using DelftTools.Controls.Swf.TreeViewControls;
using DelftTools.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;
using TreeNode = DelftTools.Controls.Swf.TreeViewControls.TreeNode;
using TreeView = DelftTools.Controls.Swf.TreeViewControls.TreeView;

namespace DelftTools.Tests.Controls.Swf.TreeViewControls
{
    [TestFixture]
    public class TreeNodeGraphicExtensionsTest
    {
        [Test, Category(TestCategory.WindowsForms)]
        public void TestDrawNode()
        {
            var bitmap = new Bitmap(200, 200);
            var pictureBox = new PictureBox();
            var icon = new Bitmap(16, 16);
            var nodeFont = new Font(new FontFamily("Courier New"), 12, FontStyle.Regular, GraphicsUnit.Pixel);
            const int xOffset = 20;
            const int yOffset = 0;
            var parentBounds = new Rectangle(xOffset, yOffset, 150, 16);
            var child1Bounds = new Rectangle(xOffset + 16, yOffset + 16, 150, 16);
            var subChildNodeBounds = new Rectangle(xOffset + 32, yOffset + 32, 150, 16);
            var child2Bounds = new Rectangle(xOffset + 16, yOffset + 48, 150, 16);
            
            Graphics.FromImage(icon).FillPie(new SolidBrush(Color.Blue), 0, 0, icon.Width, icon.Height, 90, 270);

            var mocks = new MockRepository();
            var treeView = mocks.StrictMock<TreeView>();
            var parentNode = mocks.Stub<TreeNode>();
            var childNode1 = mocks.Stub<TreeNode>();
            var childNode2 = mocks.Stub<TreeNode>();
            var subChildNode = mocks.Stub<TreeNode>();

            Expect.Call(treeView.FullRowSelect).Return(false).Repeat.Any();
            Expect.Call(treeView.ForeColor).Return(Color.Black).Repeat.Any();
            Expect.Call(treeView.Nodes).Return(new[] { parentNode }).Repeat.Any();
            Expect.Call(treeView.Font).Return(nodeFont).Repeat.Any();
            Expect.Call(treeView.Focused).Return(true).Repeat.Any();
            Expect.Call(((ITreeView)treeView).Nodes).Return((new[] {parentNode})).Repeat.Any();

            parentNode.Text = "Parent";
            parentNode.BackColor = Color.White;
            Expect.Call(parentNode.Parent).Return(null).Repeat.Any();
            Expect.Call(parentNode.TreeView).Return(treeView).Repeat.Any();
            Expect.Call(parentNode.Bounds).Return(parentBounds).Repeat.Any();
            Expect.Call(((ITreeNode)parentNode).Bounds).Return(parentBounds).Repeat.Any();
            Expect.Call(((ITreeNode)parentNode).Parent).Return(null).Repeat.Any();
            Expect.Call(((ITreeNode)parentNode).TreeView).Return(treeView).Repeat.Any();
            Expect.Call(((ITreeNode)parentNode).Nodes).Return(new[] { childNode1, childNode2 }).Repeat.Any();

            childNode1.HasChildren = true;
            childNode1.Text = "Child 1";
            childNode1.ForeColor = Color.Gray;
            childNode1.BackColor = Color.White;
            
            ((ITreeNode)childNode1).ShowCheckBox = true;
            ((ITreeNode)childNode1).Checked = true;
            ((ITreeNode)childNode1).Image = icon;
            Expect.Call(childNode1.IsSelected).Return(true);
            Expect.Call(childNode1.IsExpanded).Return(true);
            Expect.Call(childNode1.Parent).Return(parentNode).Repeat.Any();
            Expect.Call(childNode1.TreeView).Return(treeView).Repeat.Any();
            Expect.Call(childNode1.Bounds).Return(child1Bounds).Repeat.Any();
            Expect.Call(((ITreeNode)childNode1).Bounds).Return(child1Bounds).Repeat.Any();
            Expect.Call(((ITreeNode)childNode1).Parent).Return(parentNode).Repeat.Any();
            Expect.Call(((ITreeNode)childNode1).TreeView).Return(treeView).Repeat.Any();
            Expect.Call(((ITreeNode) childNode1).Nodes).Return(new[] {subChildNode}).Repeat.Any();

            childNode2.Text = "Child 2";
            childNode2.BackColor = Color.White;
            childNode2.Bold = true;
            Expect.Call(childNode2.Parent).Return(parentNode).Repeat.Any();
            Expect.Call(childNode2.TreeView).Return(treeView).Repeat.Any();
            Expect.Call(childNode2.Bounds).Return(child2Bounds).Repeat.Any();
            Expect.Call(((ITreeNode)childNode2).Bounds).Return(child2Bounds).Repeat.Any();
            Expect.Call(((ITreeNode)childNode2).Parent).Return(parentNode).Repeat.Any();
            Expect.Call(((ITreeNode)childNode2).TreeView).Return(treeView).Repeat.Any();
            Expect.Call(((ITreeNode)childNode2).Nodes).Return(new List<ITreeNode>()).Repeat.Any();

            subChildNode.Text = "Sub child 2";
            subChildNode.BackColor = Color.Orange;
            Expect.Call(subChildNode.Level).Return(2).Repeat.Any();
            Expect.Call(subChildNode.Parent).Return(childNode1).Repeat.Any();
            Expect.Call(subChildNode.TreeView).Return(treeView).Repeat.Any();
            Expect.Call(subChildNode.Bounds).Return(subChildNodeBounds).Repeat.Any();
            Expect.Call(((ITreeNode)subChildNode).Bounds).Return(subChildNodeBounds).Repeat.Any();
            Expect.Call(((ITreeNode)subChildNode).Parent).Return(childNode1).Repeat.Any();
            Expect.Call(((ITreeNode)subChildNode).TreeView).Return(treeView).Repeat.Any();
            Expect.Call(((ITreeNode)subChildNode).Nodes).Return(new List<ITreeNode>()).Repeat.Any();

            mocks.ReplayAll();

            var graphics = Graphics.FromImage(bitmap);

            // make background white
            graphics.FillRectangle(new SolidBrush(Color.White), 0,0,bitmap.Width, bitmap.Height);

            parentNode.DrawNode(graphics, false);
            childNode1.DrawNode(graphics, true);
            subChildNode.DrawNode(graphics, false);
            childNode2.DrawNode(graphics, false);
            
            pictureBox.Image = bitmap;
            
            WindowsFormsTestHelper.ShowModal(pictureBox);

            mocks.VerifyAll();
        }

        [Test]
        public void TestIsOnCheckbox()
        {
            var mocks = new MockRepository();
            var treeNode = mocks.Stub<TreeNode>();

            treeNode.ShowCheckBox = false;

            mocks.ReplayAll();

            var pointOnCheckbox = new Point(25, 5);
            var bounds = new Rectangle(40, 0, 200, 16);

            Assert.IsFalse(TreeNodeGraphicExtensions.IsOnCheckBox(null, new Point()));
            Assert.IsFalse(treeNode.IsOnCheckBox(pointOnCheckbox));

            // Change node expectations to have a checkbox
            mocks.BackToRecord(treeNode, BackToRecordOptions.PropertyBehavior);
            treeNode.ShowCheckBox = true;
            Expect.Call(treeNode.Bounds).Return(bounds);
            Expect.Call(((ITreeNode)treeNode).Bounds).Return(bounds);
            treeNode.Replay();

            Assert.IsTrue(treeNode.IsOnCheckBox(pointOnCheckbox));
        }

        [Test, Category(TestCategory.Integration)]
        public void TestIsOnExpandButton()
        {
            var mocks = new MockRepository();
            var treeNode = mocks.Stub<TreeNode>();
            
            treeNode.HasChildren = true;
            Expect.Call(treeNode.TreeView).Return(new TreeView());

            mocks.ReplayAll();

            var pointOnExpandButton = new Point(5, 5);
            var bounds = new Rectangle(40, 0, 200, 16);

            Assert.IsFalse(TreeNodeGraphicExtensions.IsOnExpandButton(null, new Point()));
            Assert.IsFalse(treeNode.IsOnExpandButton(pointOnExpandButton));

            // Change node expectations to have a checkbox
            mocks.BackToRecord(treeNode, BackToRecordOptions.PropertyBehavior);

            treeNode.HasChildren = true;
            Expect.Call(treeNode.Bounds).Return(bounds);
            Expect.Call(((ITreeNode)treeNode).Bounds).Return(bounds);
            treeNode.Replay();

            Assert.IsTrue(treeNode.IsOnExpandButton(pointOnExpandButton));
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void DrawPlaceHolderTop()
        {
            var mocks = new MockRepository();
            var treeNode = mocks.Stub<ITreeNode>();

            var bounds = new Rectangle(30, 36, 150, 16);
            treeNode.ShowCheckBox = false;
            treeNode.Image = null;
            Expect.Call(treeNode.Bounds).Return(bounds);

            mocks.ReplayAll();

            var bitmap = new Bitmap(200, 200);
            var pictureBox = new PictureBox();

            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);
            graphics.FillRectangle(new SolidBrush(Color.YellowGreen), bounds);

            treeNode.DrawPlaceHolder(PlaceholderLocation.Top, graphics);
            
            pictureBox.Image = bitmap;
            WindowsFormsTestHelper.ShowModal(pictureBox);

            mocks.VerifyAll();
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void DrawPlaceHolderBottom()
        {
            var mocks = new MockRepository();
            var treeNode = mocks.Stub<ITreeNode>();

            var bounds = new Rectangle(30, 36, 150, 16);
            treeNode.ShowCheckBox = false;
            treeNode.Image = null;
            Expect.Call(treeNode.Bounds).Return(bounds);

            mocks.ReplayAll();

            var bitmap = new Bitmap(200, 200);
            var pictureBox = new PictureBox();

            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);
            graphics.FillRectangle(new SolidBrush(Color.YellowGreen), bounds);

            treeNode.DrawPlaceHolder(PlaceholderLocation.Bottom, graphics);

            pictureBox.Image = bitmap;
            WindowsFormsTestHelper.ShowModal(pictureBox);

            mocks.VerifyAll();
        }

        [Test, Category(TestCategory.WindowsForms)]
        public void DrawPlaceHolderMiddle()
        {
            var mocks = new MockRepository();
            var treeNode = mocks.Stub<ITreeNode>();

            var bounds = new Rectangle(30, 36, 150, 16);
            treeNode.ShowCheckBox = false;
            treeNode.Image = null;
            Expect.Call(treeNode.Bounds).Return(bounds);

            mocks.ReplayAll();

            var bitmap = new Bitmap(200, 200);
            var pictureBox = new PictureBox();

            var graphics = Graphics.FromImage(bitmap);

            graphics.Clear(Color.White);
            graphics.FillRectangle(new SolidBrush(Color.YellowGreen), bounds);

            treeNode.DrawPlaceHolder(PlaceholderLocation.Middle, graphics);

            pictureBox.Image = bitmap;
            WindowsFormsTestHelper.ShowModal(pictureBox);

            mocks.VerifyAll();
        }
    }
}