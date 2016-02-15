using System;
using System.IO;
using System.Windows.Forms;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;

namespace Core.Common.Controls.TreeView.Test
{
    [TestFixture]
    public class TreeViewControlTest : NUnitFormTest
    {
        [Test]
        public void DefaultConstructor_InitializedTreeViewControl()
        {
            // Call
            var treeViewControl = new TreeViewControl();

            // Assert
            var treeView = treeViewControl.Controls[0] as System.Windows.Forms.TreeView;
            Assert.NotNull(treeView);
            Assert.IsTrue(treeView.AllowDrop);
            Assert.IsTrue(treeView.LabelEdit);
            Assert.IsFalse(treeView.HideSelection);
            Assert.AreEqual(TreeViewDrawMode.Normal, treeView.DrawMode);
            Assert.AreEqual("\\", treeView.PathSeparator);

            Assert.NotNull(treeView.ImageList);
            Assert.AreEqual(0, treeView.ImageList.Images.Count);

            Assert.NotNull(treeView.StateImageList);
            Assert.AreEqual(2, treeView.StateImageList.Images.Count);
        }

        [Test]
        public void RegisterTreeNodeInfo_Null_ThrowsNullReferenceException()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            
            // Call
            TestDelegate test = () => treeViewControl.RegisterTreeNodeInfo(null);

            // Assert
            Assert.Throws<NullReferenceException>(test);
        }

        [Test]
        public void RegisterTreeNodeInfo_NodeInfoWithoutTagType_ThrowsArgumentNullException()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo();
            
            // Call
            TestDelegate test = () => treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

            // Assert
            Assert.Throws<ArgumentNullException>(test);
        }

        [Test]
        public void RegisterTreeNodeInfo_NodeInfoWithTagType_DoesNotThrow()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object)
            };

            // Call
            TestDelegate test = () => treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

            // Assert
            Assert.DoesNotThrow(test);
        }
        
        [Test]
        public void RegisterTreeNodeInfo_NodeInfoForTagTypeAlreadySet_OverridesNodeInfo()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRename = (o, p) => false
            };
            var treeNodeInfoOverride = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRename = (o,p) => true
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

            var dataObject = new object();
            treeViewControl.Data = dataObject;

            // Call
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfoOverride);

            // Assert
            Assert.IsTrue(treeViewControl.CanRenameNodeForData(dataObject));
        }

        [Test]
        public void Data_SetToNull_DataIsNull()
        {
            // Setup
            var treeViewControl = new TreeViewControl();

            // Call
            treeViewControl.Data = null;

            // Assert
            Assert.IsNull(treeViewControl.Data);
        }

        [Test]
        public void Data_NoNodeInfoSet_ThrowsInvalidOperationException()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var testNodeData = new object();

            // Call
            TestDelegate test = () => treeViewControl.Data = testNodeData;

            // Assert
            Assert.Throws<InvalidOperationException>(test);
        }

        [Test]
        public void Data_NodeInfoSet_DataSet()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            treeViewControl.RegisterTreeNodeInfo(new TreeNodeInfo
            {
                TagType = typeof(object)
            });

            var testNodeData = new object();

            // Call
            treeViewControl.Data = testNodeData;

            // Assert
            Assert.AreSame(testNodeData, treeViewControl.Data);
            Assert.AreSame(testNodeData, treeViewControl.SelectedData);
        }

        [Test]
        public void CanRenameNodeForData_Null_ReturnsFalse()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRename = (o, p) => true
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.Data = new object();

            // Call
            var result = treeViewControl.CanRenameNodeForData(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CanRenameNodeForData_DataNotSet_ReturnsFalse()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRename = (o, p) => true
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

            // Call
            var result = treeViewControl.CanRenameNodeForData(new object());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRenameNodeForData_DataSet_ReturnsValueOfCanRename(bool expected)
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRename = (o, p) => expected
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var dataObject = new object();
            treeViewControl.Data = dataObject;

            // Call
            var result = treeViewControl.CanRenameNodeForData(dataObject);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        [RequiresSTA]
        public void TryRenameNodeForData_NotRenameable_ShowsDialog()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRename = (o, p) => false
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var dataObject = new object();
            treeViewControl.Data = dataObject;

            // Call
            Action test = () =>
            {
                treeViewControl.TryRenameNodeForData(dataObject);
            };

            // Assert
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                Assert.AreEqual(Properties.Resources.TreeViewControl_The_selected_item_cannot_be_renamed, helper.Text);
                helper.ClickOk();
            };
            test();
        }

        [Test]
        [RequiresSTA]
        public void TryRenameNodeForData_Renameable_BeginEdit()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRename = (o, p) => true
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var dataObject = new object();
            treeViewControl.Data = dataObject;

            WindowsFormsTestHelper.Show(treeViewControl);

            // Call
            treeViewControl.TryRenameNodeForData(dataObject);

            // Assert
            var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
            Assert.IsTrue(treeView.Nodes[0].IsEditing);

            WindowsFormsTestHelper.CloseAll();
        }

        [Test]
        public void CanRemoveNodeForData_Null_ReturnsFalse()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRemove = (o, p) => true
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.Data = new object();

            // Call
            var result = treeViewControl.CanRemoveNodeForData(null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CanRemoveNodeForData_DataNotSet_ReturnsFalse()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRemove = (o, p) => true
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

            // Call
            var result = treeViewControl.CanRemoveNodeForData(new object());

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRemoveNodeForData_DataSet_ReturnsValueOfCanRename(bool expected)
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRemove = (o, p) => expected
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var dataObject = new object();
            treeViewControl.Data = dataObject;

            // Call
            var result = treeViewControl.CanRemoveNodeForData(dataObject);

            // Assert
            Assert.AreEqual(expected, result);
        }

        [Test]
        [RequiresSTA]
        public void TryRemoveNodeForData_NotRemoveable_ShowsDialog()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRemove = (o, p) => false
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var dataObject = new object();
            treeViewControl.Data = dataObject;

            // Call
            Action test = () =>
            {
                treeViewControl.TryRemoveNodeForData(dataObject);
            };

            // Assert
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                Assert.AreEqual(Properties.Resources.TreeViewControl_The_selected_item_cannot_be_removed, helper.Text);
                helper.ClickOk();
            };
            test();
        }

        [Test]
        [RequiresSTA]
        public void TryRemoveNodeForData_Removeable_OnNodeRemovedCalled()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var hit = 0;
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                CanRemove = (o, p) => true,
                OnNodeRemoved = (o, p) => hit++
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var dataObject = new object();
            treeViewControl.Data = dataObject;

            WindowsFormsTestHelper.Show(treeViewControl);

            // Call
            Action test = () =>
            {
                treeViewControl.TryRemoveNodeForData(dataObject);
            };

            // Assert
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                Assert.AreEqual(Properties.Resources.TreeViewControl_Are_you_sure_you_want_to_remove_the_selected_item, helper.Text);
                helper.ClickOk();
            };
            test();
            WindowsFormsTestHelper.CloseAll();

            Assert.AreEqual(1, hit);
        }

        [Test]
        public void CanExpandOrCollapseForData_NoDataForNull_ReturnsFalse()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object)
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

            // Call
            treeViewControl.CanExpandOrCollapseForData(null);

            // Assert

        }

        [Test]
        public void CanExpandOrCollapseForData_NoChildren_ReturnsFalse()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object)
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            // Call
            var result = treeViewControl.CanExpandOrCollapseForData(data);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CanExpandOrCollapseForData_WithChildren_ReturnsTrue()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty }
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string)
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            // Call
            var result = treeViewControl.CanExpandOrCollapseForData(data);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void TryCollapseAllNodesForData_WithoutChildrenForNull_NoChange()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
            var treeNode = treeView.Nodes[0];
            treeNode.Expand();

            // Call
            treeViewControl.TryCollapseAllNodesForData(null);

            // Assert
            Assert.IsTrue(treeNode.IsExpanded);
        }

        [Test]
        public void TryCollapseAllNodesForData_WithoutChildren_Collapsed()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
            var treeNode = treeView.Nodes[0];
            treeNode.Expand();

            // Call
            treeViewControl.TryCollapseAllNodesForData(data);

            // Assert
            Assert.IsFalse(treeNode.IsExpanded);
        }

        [Test]
        public void TryCollapseAllNodesForData_WithChildren_CollapsesNodeAndChildren()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty }
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string)
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            var treeView = (System.Windows.Forms.TreeView)treeViewControl.Controls[0];
            var treeNode = treeView.Nodes[0];
            treeNode.Expand();
            var childNode = treeNode.Nodes[0];
            childNode.Expand();

            // Call
            treeViewControl.TryCollapseAllNodesForData(data);

            // Assert
            Assert.IsFalse(treeNode.IsExpanded);
            Assert.IsFalse(childNode.IsExpanded);
        }

        [Test]
        public void TryExpandAllNodesForData_ForNull_NoChange()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            var treeView = (System.Windows.Forms.TreeView)treeViewControl.Controls[0];
            var treeNode = treeView.Nodes[0];
            treeNode.Collapse();

            // Call
            treeViewControl.TryExpandAllNodesForData(null);

            // Assert
            Assert.IsFalse(treeNode.IsExpanded);
        }

        [Test]
        public void TryExpandAllNodesForData_WithoutChildren_Expanded()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            var treeView = (System.Windows.Forms.TreeView) treeViewControl.Controls[0];
            var treeNode = treeView.Nodes[0];
            treeNode.Collapse();

            // Call
            treeViewControl.TryExpandAllNodesForData(data);

            // Assert
            Assert.IsTrue(treeNode.IsExpanded);
        }

        [Test]
        public void TryExpandAllNodesForData_WithChildren_ExpandsNodeAndChildren()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty }
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string)
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            var treeView = (System.Windows.Forms.TreeView)treeViewControl.Controls[0];
            var treeNode = treeView.Nodes[0];
            treeNode.Collapse();
            var childNode = treeNode.Nodes[0];
            childNode.Collapse();

            // Call
            treeViewControl.TryExpandAllNodesForData(data);

            // Assert
            Assert.IsTrue(treeNode.IsExpanded);
            Assert.IsTrue(childNode.IsExpanded);
        }

        [Test]
        public void TrySelectNodeForData_NoData_SelectNull()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty }
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);

            // Call
            treeViewControl.TrySelectNodeForData(null);

            // Assert
            Assert.IsNull(treeViewControl.SelectedData);
        }

        [Test]
        public void TrySelectNodeForData_WithChildDataSelectRoot_RootSelected()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty }
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string)
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            // Call
            treeViewControl.TrySelectNodeForData(data);

            // Assert
            Assert.AreSame(data, treeViewControl.SelectedData);
        }

        [Test]
        public void TrySelectNodeForData_WithChildDataSelectChild_ChildSelected()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty }
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string)
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            // Call
            treeViewControl.TrySelectNodeForData(string.Empty);

            // Assert
            Assert.AreSame(string.Empty, treeViewControl.SelectedData);
        }

        [Test]
        public void TrySelectNodeForData_WithChildDataSelectNull_NoSelection()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty }
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string)
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            // Call
            treeViewControl.TrySelectNodeForData(null);

            // Assert
            Assert.IsNull(treeViewControl.SelectedData);
        }

        [Test]
        public void TryGetPathForData_WithChildDataForRoot_ReturnsPathToRoot()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var rootText = "root";
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty },
                Text = o => rootText
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string),
                Text = o => "child"
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            // Call
            var result = treeViewControl.TryGetPathForData(data);

            // Assert
            Assert.AreEqual(rootText, result);
        }

        [Test]
        public void TryGetPathForData_WithChildDataForChild_ReturnsPathToChild()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var rootText = "root";
            var childText = "child";
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty },
                Text = o => rootText
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string),
                Text = o => "child"
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            var treeView = (System.Windows.Forms.TreeView)treeViewControl.Controls[0];

            // Call
            var result = treeViewControl.TryGetPathForData(string.Empty);

            // Assert
            Assert.AreEqual(rootText + treeView.PathSeparator + childText, result);
        }

        [Test]
        public void TryGetPathForData_WithChildDataForNull_ReturnsNull()
        {
            // Setup
            var treeViewControl = new TreeViewControl();
            var treeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(object),
                ChildNodeObjects = o => new[] { string.Empty },
                Text = o => "root"
            };
            var childTreeNodeInfo = new TreeNodeInfo
            {
                TagType = typeof(string),
                Text = o => "child"
            };
            treeViewControl.RegisterTreeNodeInfo(treeNodeInfo);
            treeViewControl.RegisterTreeNodeInfo(childTreeNodeInfo);
            var data = new object();
            treeViewControl.Data = data;

            // Call
            var result = treeViewControl.TryGetPathForData(null);

            // Assert
            Assert.IsNull(result);
        }
//
//        [Test]
//        public void ResolveNodePresenterForDataWalksUpClassHierarchy()
//        {
//            var mocks = new MockRepository();
//            var treeview = mocks.StrictMock<TreeView>();
//
//            mocks.ReplayAll();
//
//            var controller = new TreeViewController(treeview);
//            var baseClassNodePresenter = new BaseClassNodePresenter();
//            var subClassNodePresenter = new SubClassNodePresenter();
//
//            controller.RegisterNodePresenter(baseClassNodePresenter);
//            controller.RegisterNodePresenter(subClassNodePresenter);
//
//            Assert.AreEqual(subClassNodePresenter, controller.ResolveNodePresenterForData(new SubClass()));
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void ResolveNodePresenterForDataReturnsNullIfNotFound()
//        {
//            var mocks = new MockRepository();
//            var treeview = mocks.StrictMock<TreeView>();
//
//            mocks.ReplayAll();
//
//            var presenter = new TreeViewController(treeview);
//            Assert.IsNull(presenter.ResolveNodePresenterForData(new SubClass()));
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void ResolveNodePresenterForDataReturnsNullIfItemIsNull()
//        {
//            var mocks = new MockRepository();
//            var treeview = mocks.StrictMock<TreeView>();
//
//            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
//
//            mocks.ReplayAll();
//
//            var presenter = new TreeViewController(treeview);
//            Assert.IsNull(presenter.ResolveNodePresenterForData(null));
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void ResolveNodePresenterCanMatchOnInterface()
//        {
//            var mocks = new MockRepository();
//            var treeview = mocks.StrictMock<TreeView>();
//
//            mocks.ReplayAll();
//
//            var presenter = new TreeViewController(treeview);
//
//            var interfaceNodePresenter = new SomeInterfaceNodePresenter();
//            var interfaceImplementor = new SubClass();
//            presenter.RegisterNodePresenter(interfaceNodePresenter);
//
//            Assert.AreEqual(interfaceNodePresenter, presenter.ResolveNodePresenterForData(interfaceImplementor));
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void TestOnNodeChecked()
//        {
//            var mocks = new MockRepository();
//
//            var treeNode = mocks.Stub<TreeNode>();
//            var treeview = mocks.StrictMock<TreeView>();
//            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//
//            nodePresenter.TreeView = treeview;
//            treeNode.Presenter = nodePresenter;
//
//            Expect.Call(() => nodePresenter.OnNodeChecked(treeNode));
//
//            treeNode.Tag = new object();
//            Expect.Call(treeNode.IsUpdating).Return(true);
//
//            mocks.ReplayAll();
//
//            var controller = new TreeViewController(treeview);
//            controller.RegisterNodePresenter(nodePresenter);
//
//            controller.OnNodeChecked(treeNode);
//
//            mocks.BackToRecord(treeNode);
//            treeNode.Tag = new object();
//            treeNode.Presenter = nodePresenter;
//            Expect.Call(treeNode.IsUpdating).Return(false);
//            mocks.Replay(treeNode);
//
//            controller.OnNodeChecked(treeNode);
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        [ExpectedException(ExpectedMessage = "Niet in staat om een presentatieobject te vinden voor niet geïnitialiseerd object.")]
//        public void TestOnNodeCheckedWithNodeWithoutTagValue()
//        {
//            var mocks = new MockRepository();
//
//            var treeNode = mocks.Stub<TreeNode>();
//            var treeview = mocks.StrictMock<TreeView>();
//
//            treeNode.Tag = null;
//            mocks.ReplayAll();
//
//            var presenter = new TreeViewController(treeview);
//
//            mocks.VerifyAll();
//
//            presenter.OnNodeChecked(treeNode);
//        }
//
//        [Test]
//        public void TestSetData()
//        {
//            var mocks = new MockRepository();
//
//            var treeview = mocks.StrictMock<TreeView>();
//            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//            var nodes = new List<TreeNode>();
//
//            nodePresenter.TreeView = treeview;
//
//            Expect.Call(treeview.Nodes).Return(nodes).Repeat.Any();
//            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
//            treeview.SelectedNode = null;
//            LastCall.On(treeview).IgnoreArguments();
//
//            Expect.Call(nodePresenter.NodeTagType).Return(typeof(object)).Repeat.Any();
//            Expect.Call(nodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Any();
//            Expect.Call(() => nodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
//
//            mocks.ReplayAll();
//
//            var presenter = new TreeViewController(treeview);
//            presenter.RegisterNodePresenter(nodePresenter);
//
//            presenter.Data = new object();
//
//            Assert.AreEqual(1, nodes.Count);
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void TestSetDataWithNull()
//        {
//            var mocks = new MockRepository();
//
//            var treeview = mocks.Stub<TreeView>();
//            var nodes = new List<TreeNode>();
//
//            Expect.Call(treeview.Nodes).Return(nodes).Repeat.Any();
//
//            mocks.ReplayAll();
//
//            var presenter = new TreeViewController(treeview)
//            {
//                Data = null
//            };
//
//            Assert.IsNull(presenter.Data);
//
//            Assert.AreEqual(0, nodes.Count);
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void TestRefreshChildNodesOfNodeWithoutChildren()
//        {
//            var mocks = new MockRepository();
//
//            var treeView = mocks.Stub<TreeView>();
//
//            var treeViewController = new TreeViewController(treeView);
//
//            var parentNodePresenter = mocks.Stub<ITreeNodePresenter>();
//            treeViewController.RegisterNodePresenter(parentNodePresenter);
//
//            var nodes = new List<TreeNode>();
//            Expect.Call(treeView.Nodes).Return(nodes).Repeat.Any();
//            Expect.Call(treeView.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
//
//            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
//            Expect.Call(parentNodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Twice();
//            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
//            mocks.ReplayAll();
//
//            treeViewController.Data = new Parent();
//
//            // should not create create sub nodes
//            treeViewController.RefreshChildNodes(nodes[0]);
//
//            Assert.AreEqual(0, nodes[0].Nodes.Count);
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void TestRefreshChildNodesOfExpandedNode()
//        {
//            var mocks = new MockRepository();
//            var parentNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//            var childNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//
//            var treeview = new TreeView();
//            var parent = new Parent();
//            var child1 = new Child();
//            var child2 = new Child();
//
//            parent.Children.AddRange(new[]
//            {
//                child1,
//                child2
//            });
//
//            parentNodePresenter.TreeView = treeview;
//            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
//            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
//            Expect.Call(parentNodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(parent.Children).Repeat.Any();
//
//            childNodePresenter.TreeView = treeview;
//            Expect.Call(childNodePresenter.NodeTagType).Return(typeof(Child)).Repeat.Any();
//            Expect.Call(() => childNodePresenter.UpdateNode(null, null, null)).IgnoreArguments().Repeat.Times(3);
//            Expect.Call(childNodePresenter.GetChildNodeObjects(parent)).Return(parent.Children).Repeat.Any();
//            Expect.Call(childNodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Any();
//
//            mocks.ReplayAll();
//
//            var controller = TypeUtils.GetField<TreeViewController>(treeview, "controller");
//            controller.RegisterNodePresenter(parentNodePresenter);
//            controller.RegisterNodePresenter(childNodePresenter);
//
//            controller.Data = parent;
//
//            var parentNode = treeview.Nodes[0];
//
//            // setting the controller.Data creates a root node and expands it if it has children
//            Assert.AreEqual(2, parentNode.Nodes.Count);
//
//            // simulate removing child2 from parent by changing the number of children returned by the parent node presenter
//            parentNodePresenter.BackToRecord(BackToRecordOptions.All);
//            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
//            Expect.Call(parentNodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(new[]
//            {
//                child1
//            });
//            parentNodePresenter.Replay();
//
//            // updates the tree view to the new parent node state (parent has 1 child item)
//            controller.RefreshChildNodes(parentNode);
//
//            Assert.AreEqual(1, parentNode.Nodes.Count);
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void TestRefreshChildNodesOfUnExpandedNode()
//        {
//            var mocks = new MockRepository();
//
//            var parentNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//            var childNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//
//            var treeview = new TreeView();
//            var parent = new Parent();
//            var child1 = new Child();
//            var child2 = new Child();
//
//            parentNodePresenter.TreeView = treeview;
//            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
//            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
//            Expect.Call(parentNodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(new[]
//            {
//                child1,
//                child2
//            }).Repeat.Any();
//
//            childNodePresenter.TreeView = treeview;
//            Expect.Call(childNodePresenter.NodeTagType).Return(typeof(Child)).Repeat.Any();
//            Expect.Call(() => childNodePresenter.UpdateNode(null, null, null)).IgnoreArguments().Repeat.Times(4);
//            Expect.Call(childNodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Any();
//
//            mocks.ReplayAll();
//
//            var controller = TypeUtils.GetField<TreeViewController>(treeview, "controller");
//            controller.RegisterNodePresenter(parentNodePresenter);
//            controller.RegisterNodePresenter(childNodePresenter);
//
//            controller.Data = parent;
//
//            var parentNode = treeview.Nodes[0];
//
//            parentNode.Collapse();
//            controller.RefreshChildNodes(parentNode);
//
//            Assert.AreEqual(2, parentNode.Nodes.Count);
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void TestRefreshLoadedNode()
//        {
//            var mocks = new MockRepository();
//
//            var parentTreeNode = mocks.StrictMock<TreeNode>();
//            var childTreeNode = mocks.StrictMock<TreeNode>();
//            var treeview = mocks.StrictMock<TreeView>();
//            var parentNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//            var childNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//
//            var parent = new Parent();
//            var child = new Child();
//            var subNodes = new List<TreeNode>
//            {
//                childTreeNode
//            };
//
//            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
//            Expect.Call(treeview.BeginUpdate).IgnoreArguments().Repeat.Any();
//            Expect.Call(treeview.EndUpdate).IgnoreArguments().Repeat.Any();
//            Expect.Call(treeview.InvokeRequired).Return(false).Repeat.Any();
//
//            parentTreeNode.HasChildren = true;
//            Expect.Call(parentTreeNode.Tag).Return(parent).Repeat.Any();
//            Expect.Call(parentTreeNode.Parent).Return(null);
//            Expect.Call(parentTreeNode.Nodes).Return(subNodes).Repeat.Any();
//            Expect.Call(parentTreeNode.Presenter).Return(parentNodePresenter).Repeat.Any();
//
//            childTreeNode.HasChildren = false;
//            Expect.Call(childTreeNode.Tag).Return(child).Repeat.Any();
//            Expect.Call(childTreeNode.Parent).Return(parentTreeNode).Repeat.Any();
//            Expect.Call(childTreeNode.Nodes).Return(new List<TreeNode>()).Repeat.Any();
//            Expect.Call(childTreeNode.Presenter).Return(childNodePresenter).Repeat.Any();
//
//            parentNodePresenter.TreeView = treeview;
//            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
//            Expect.Call(parentNodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(new[]
//            {
//                child
//            }).Repeat.Any();
//
//            childNodePresenter.TreeView = treeview;
//            Expect.Call(childNodePresenter.NodeTagType).Return(typeof(Child)).Repeat.Any();
//            Expect.Call(childNodePresenter.GetChildNodeObjects(null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Any();
//
//            // expect 1 update call for the parent & child
//            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
//            Expect.Call(() => childNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
//
//            mocks.ReplayAll();
//
//            var controller = new TreeViewController(treeview);
//            controller.RegisterNodePresenter(parentNodePresenter);
//            controller.RegisterNodePresenter(childNodePresenter);
//
//            controller.UpdateNode(parentTreeNode);
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void TestCanRenameNode()
//        {
//            var mocks = new MockRepository();
//            var treeview = mocks.StrictMock<TreeView>();
//            var treeNode = mocks.StrictMock<TreeNode>();
//            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//            var tag = new object();
//
//            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
//            Expect.Call(treeNode.Tag).Return(tag).Repeat.Any();
//            Expect.Call(treeNode.Presenter).Return(null);
//            Expect.Call(treeNode.Presenter).Return(nodePresenter);
//
//            nodePresenter.TreeView = treeview;
//            //Expect.Call(nodePresenter.IsPresenterForNode(null)).IgnoreArguments().Return(true);
//            Expect.Call(nodePresenter.NodeTagType).Return(typeof(object)).Repeat.Any();
//
//            // node presenter decides if a node can be renamed
//            Expect.Call(nodePresenter.CanRenameNode(null)).IgnoreArguments().Return(true);
//
//            mocks.ReplayAll();
//
//            var presenter = new TreeViewController(treeview);
//
//            // no node
//            Assert.IsFalse(presenter.CanRenameNode(null));
//
//            // no node presenter
//            Assert.IsFalse(presenter.CanRenameNode(treeNode));
//
//            presenter.RegisterNodePresenter(nodePresenter);
//
//            // node presenter decides
//            Assert.IsTrue(presenter.CanRenameNode(treeNode));
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void TestCanDeleteNode()
//        {
//            var mocks = new MockRepository();
//            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();
//            var treeNode = mocks.StrictMock<TreeNode>();
//            var treeview = mocks.StrictMock<TreeView>();
//            var tag = new object();
//
//            nodePresenter.TreeView = treeview;
//            //Expect.Call(nodePresenter.IsPresenterForNode(null)).IgnoreArguments().Return(true);
//            Expect.Call(nodePresenter.NodeTagType).Return(typeof(object)).Repeat.Any();
//
//            // node presenter decides if a node can be removed
//            Expect.Call(nodePresenter.CanRemove(null, null)).IgnoreArguments().Return(true);
//
//            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
//            Expect.Call(treeNode.Tag).Return(tag).Repeat.Any();
//            Expect.Call(treeNode.Parent).Return(null);
//            Expect.Call(treeNode.Presenter).Return(null);
//            Expect.Call(treeNode.Presenter).Return(nodePresenter);
//
//            mocks.ReplayAll();
//
//            var controller = new TreeViewController(treeview);
//
//            // no node
//            Assert.IsFalse(controller.CanDeleteNode(null));
//
//            // no node presenter
//            Assert.IsFalse(controller.CanDeleteNode(treeNode));
//
//            controller.RegisterNodePresenter(nodePresenter);
//
//            // node presenter decides
//            Assert.IsTrue(controller.CanDeleteNode(treeNode));
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void RegisterNodePresenter_ItemNotRegisteredYet_AddItemToAvailableNodePresenters()
//        {
//            // Setup
//            var mocks = new MockRepository();
//            var treeView = mocks.Stub<TreeView>();
//            var nodePresenter = mocks.Stub<ITreeNodePresenter>();
//            mocks.ReplayAll();
//
//            var controller = new TreeViewController(treeView);
//
//            // Call
//            controller.RegisterNodePresenter(nodePresenter);
//
//            // Assert
//            CollectionAssert.Contains(controller.NodePresenters, nodePresenter);
//            Assert.AreEqual(1, controller.NodePresenters.Count());
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        public void RegisterNodePresenter_ItemAlreadyRegistered_DoNothing()
//        {
//            // Setup
//            var mocks = new MockRepository();
//            var treeView = mocks.Stub<TreeView>();
//            var nodePresenter = mocks.Stub<ITreeNodePresenter>();
//            mocks.ReplayAll();
//
//            var controller = new TreeViewController(treeView);
//
//            // Call
//            controller.RegisterNodePresenter(nodePresenter);
//            controller.RegisterNodePresenter(nodePresenter);
//
//            // Assert
//            CollectionAssert.Contains(controller.NodePresenters, nodePresenter);
//            Assert.AreEqual(1, controller.NodePresenters.Count());
//            mocks.VerifyAll();
//        }
//
//        private class Parent
//        {
//            public Parent()
//            {
//                Children = new List<Child>();
//            }
//
//            public List<Child> Children { get; set; }
//
//            public string Name { get; set; }
//        }
//
//        private class Child
//        {
//            private string Name { get; set; }
//
//            public override string ToString()
//            {
//                return Name;
//            }
//        }
//
//        private interface ISomeInterface { }
//
//        private class BaseClass { }
//
//        private class SubClass : BaseClass, ISomeInterface { }
//
//        private class SomeInterfaceNodePresenter : TreeViewNodePresenterBase<ISomeInterface>
//        {
//            public override void UpdateNode(TreeNode parentNode, TreeNode node, ISomeInterface nodeData) { }
//        }
//
//        private class SubClassNodePresenter : TreeViewNodePresenterBase<SubClass>
//        {
//            public override void UpdateNode(TreeNode parentNode, TreeNode node, SubClass nodeData) { }
//        }
//
//        private class BaseClassNodePresenter : TreeViewNodePresenterBase<BaseClass>
//        {
//            public override void UpdateNode(TreeNode parentNode, TreeNode node, BaseClass nodeData) { }
//        }

        # region Refactor

//        [Test]
//        public void Clear_ChildrenSubscribed_ChildrenUnsubscribe()
//        {
//            // Setup
//            var mocks = new MockRepository();
//            var treeView = mocks.Stub<TreeView>();
//            var someTreeNodeCollectionContainer = new System.Windows.Forms.TreeNode();
//            var parentNode = new TreeNode(treeView);
//            var childNode = new TreeNode(treeView);
//            var childChildNode = new TreeNode(treeView);
//
//            var parentData = mocks.Stub<IObservable>();
//            var childData = mocks.Stub<IObservable>();
//            var childChildData = mocks.Stub<IObservable>();
//
//            parentData.Expect(p => p.Attach(parentNode));
//            childData.Expect(p => p.Attach(childNode));
//            childChildData.Expect(p => p.Attach(childChildNode));
//
//            parentData.Expect(p => p.Detach(parentNode));
//            childData.Expect(p => p.Detach(childNode));
//            childChildData.Expect(p => p.Detach(childChildNode));
//
//            mocks.ReplayAll();
//
//            parentNode.Tag = parentData;
//            childNode.Tag = childData;
//            childChildNode.Tag = childChildData;
//
//            var treeNodeList = new TreeNodeList(someTreeNodeCollectionContainer.Nodes);
//            treeNodeList.Add(parentNode);
//            treeNodeList.Add(childNode);
//            treeNodeList.Add(childChildNode);
//
//            // Call
//            treeNodeList.Clear();
//
//            // Assert
//            mocks.VerifyAll();
//        }

//        [Test]
//        public void TestTextLengthLimit()
//        {
//            var node = new TreeNode(null);
//
//            var largeText = "";
//            for (var i = 0; i <= 24; i++) 
//            {
//                largeText += "1234567890";
//            }
//
//            node.Text = largeText;
//            Assert.AreEqual(250, node.Text.Length);
//
//            node.Text = largeText + "123456789";
//            Assert.AreEqual(259, node.Text.Length);
//
//            node.Text = largeText + "1234567890";
//            Assert.AreEqual(259, node.Text.Length, "Text length limit should be 259");
//        }
//
//        [Test]
//        public void TestTextSetToNull()
//        {
//            var node = new TreeNode(null);
//
//            var originalText = "test";
//            node.Text = originalText;
//            Assert.AreEqual(originalText, node.Text);
//
//            node.Text = null;
//            Assert.AreEqual("", node.Text);
//        }

        # endregion
    }
}