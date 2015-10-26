using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using DelftTools.Controls;
using DelftTools.Controls.Swf.TreeViewControls;
using DelftTools.Tests.TestObjects;
using DelftTools.Utils.Collections;
using DelftTools.Utils.Reflection;
using NUnit.Framework;
using Rhino.Mocks;
using Category = NUnit.Framework.CategoryAttribute;
using Child = DelftTools.Tests.Controls.Swf.UndoRedoTestClasses.Child;
using Parent = DelftTools.Tests.Controls.Swf.UndoRedoTestClasses.Parent;

namespace DelftTools.Tests.Controls.Swf.TreeViewControls
{
    [TestFixture]
    public class TreeViewControllerTest
    {
        [Test]
        [ExpectedException(typeof(ArgumentException), ExpectedMessage = "Tree view can't be null")]
        public void ConstructWithoutTreeView()
        {
            new TreeViewController(null);
        }

        [Test]
        public void ResolveNodePresenterForDataWalksUpClassHierarchy()
        {
            var mocks = new MockRepository();
            var treeview = mocks.StrictMock<ITreeView>();

            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null);

            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview);
            var baseClassNodePresenter = new BaseClassNodePresenter();
            var subClassNodePresenter = new SubClassNodePresenter();

            presenter.NodePresenters.AddRange(new ITreeNodePresenter[]
            {
                baseClassNodePresenter,
                subClassNodePresenter
            });

            Assert.AreEqual(subClassNodePresenter, presenter.ResolveNodePresenterForData(new SubClass()));

            mocks.VerifyAll();
        }

        [Test]
        public void ResolveNodePresenterForDataReturnsNullIfNotFound()
        {
            var mocks = new MockRepository();
            var treeview = mocks.StrictMock<ITreeView>();

            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null);

            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview);
            Assert.IsNull(presenter.ResolveNodePresenterForData(new SubClass()));

            mocks.VerifyAll();
        }

        [Test]
        public void ResolveNodePresenterForDataReturnsNullIfItemIsNull()
        {
            var mocks = new MockRepository();
            var treeview = mocks.StrictMock<ITreeView>();

            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();

            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview);
            Assert.IsNull(presenter.ResolveNodePresenterForData(null));

            mocks.VerifyAll();
        }

        [Test]
        public void ResolveNodePresenterCanMatchOnInterface()
        {
            var mocks = new MockRepository();
            var treeview = mocks.StrictMock<ITreeView>();

            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null);

            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview);

            var interfaceNodePresenter = new SomeInterfaceNodePresenter();
            var interfaceImplementor = new SubClass();
            presenter.NodePresenters.AddRange(new[]
            {
                interfaceNodePresenter
            });

            Assert.AreEqual(interfaceNodePresenter, presenter.ResolveNodePresenterForData(interfaceImplementor));

            mocks.VerifyAll();
        }

        [Test]
        public void TestOnNodeChecked()
        {
            var mocks = new MockRepository();

            var treeNode = mocks.Stub<ITreeNode>();
            var treeview = mocks.StrictMock<ITreeView>();
            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();

            nodePresenter.TreeView = treeview;
            treeNode.Presenter = nodePresenter;

            Expect.Call(() => nodePresenter.OnNodeChecked(treeNode));
            //Expect.Call(nodePresenter.NodeTagType).Return(typeof(object));
            //Expect.Call(nodePresenter.IsPresenterForNode(null)).IgnoreArguments().Return(true);
            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();

            treeNode.Tag = new object();
            Expect.Call(treeNode.IsUpdating).Return(true);

            mocks.ReplayAll();

            var controller = new TreeViewController(treeview);
            controller.NodePresenters.Add(nodePresenter);

            controller.OnNodeChecked(treeNode);

            mocks.BackToRecord(treeNode);
            treeNode.Tag = new object();
            treeNode.Presenter = nodePresenter;
            Expect.Call(treeNode.IsUpdating).Return(false);
            mocks.Replay(treeNode);

            controller.OnNodeChecked(treeNode);

            mocks.VerifyAll();
        }

        [Test]
        [ExpectedException(ExpectedMessage = "Unable to resolve node presenter for null data")]
        public void TestOnNodeCheckedWithNodeWithoutTagValue()
        {
            var mocks = new MockRepository();

            var treeNode = mocks.Stub<ITreeNode>();
            var treeview = mocks.StrictMock<TreeView>();

            treeNode.Tag = null;
            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview);

            mocks.VerifyAll();

            presenter.OnNodeChecked(treeNode);
        }

        [Test]
        public void TestSetData()
        {
            var mocks = new MockRepository();

            var treeview = mocks.StrictMock<ITreeView>();
            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();
            var nodes = new List<ITreeNode>();

            nodePresenter.TreeView = treeview;

            Expect.Call(treeview.Nodes).Return(nodes).Repeat.Any();
            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
            treeview.SelectedNode = null;
            LastCall.On(treeview).IgnoreArguments();

            Expect.Call(nodePresenter.NodeTagType).Return(typeof(object)).Repeat.Any();
            Expect.Call(nodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(Enumerable.Empty<object>());
            Expect.Call(() => nodePresenter.UpdateNode(null, null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview);
            presenter.NodePresenters.Add(nodePresenter);

            presenter.Data = new object();

            Assert.AreEqual(1, nodes.Count);
            mocks.VerifyAll();
        }

        [Test]
        public void TestSetDataWithNull()
        {
            var mocks = new MockRepository();

            var treeview = mocks.Stub<ITreeView>();
            var nodes = new List<ITreeNode>();

            Expect.Call(treeview.Nodes).Return(nodes).Repeat.Any();

            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview)
            {
                Data = null
            };

            Assert.IsNull(presenter.Data);

            Assert.AreEqual(0, nodes.Count);
            mocks.VerifyAll();
        }

        [Test]
        public void TestRegisterAndUnRegisterOnPropertyChangedOnSetData()
        {
            var mocks = new MockRepository();

            var treeview = mocks.StrictMock<ITreeView>();
            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();
            var nodes = new List<ITreeNode>();
            var parent = new Parent();

            nodePresenter.TreeView = treeview;

            Expect.Call(treeview.GetNodeByTag(parent)).Return(nodes.FirstOrDefault(n => n.Tag == parent)).Repeat.Any();
            Expect.Call(treeview.Nodes).Return(nodes).Repeat.Any();
            Expect.Call(treeview.Refresh).IgnoreArguments().Repeat.Any();
            treeview.SelectedNode = null;
            LastCall.On(treeview).IgnoreArguments();

            Expect.Call(nodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
            Expect.Call(nodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(Enumerable.Empty<object>());
            Expect.Call(() => nodePresenter.UpdateNode(null, null, null)).IgnoreArguments();

            // Property changed is bubbled to nodePresenter => expect 1 call during parent.Name = "Test"
            // not that in some cases Full refresh can be called
            Expect.Call(() => nodePresenter.OnPropertyChanged(parent, null, new PropertyChangedEventArgs(""))).IgnoreArguments().Repeat.Any();

            mocks.ReplayAll();

            var controller = new TreeViewController(treeview);
            controller.NodePresenters.Add(nodePresenter);

            controller.OnTreeViewHandleCreated();

            controller.Data = parent;

            // generate property changed with listeners enabled
            parent.Name = "Test";

            controller.Data = null;

            // generate property changed with listeners disabled
            parent.Name = "Test 2";

            mocks.VerifyAll();
        }

        [Test]
        public void TestRegisterAndUnRegisterOnCollectionChangedOnSetData()
        {
            var mocks = new MockRepository();

            var treeview = mocks.StrictMock<ITreeView>();
            var parentNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
            var childNodePresenter = mocks.StrictMock<ITreeNodePresenter>();

            var controller = new TreeViewController(treeview);
            var treeViewNodes = new List<ITreeNode>();

            var parent = new Parent();
            var child1 = new Child();
            var child2 = new Child();

            parent.Children.AddRange(new[]
            {
                child1,
                child2
            });
            treeview.Expect(tv => tv.RefreshChildNodes(null)).IgnoreArguments().Repeat.Once();
            treeview.Expect(tv => tv.Visible).IgnoreArguments().Return(false).Repeat.Any();
            treeview.Expect(tv => tv.BeginUpdate()).IgnoreArguments().Repeat.Any();
            treeview.Expect(tv => tv.Refresh()).IgnoreArguments().Repeat.Any();

            treeview.SelectedNode = null;
            LastCall.On(treeview).IgnoreArguments();

            Expect.Call(treeview.Nodes).Return(treeViewNodes).Repeat.Any();
            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();

            parentNodePresenter.TreeView = treeview;
            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
            Expect.Call(parentNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(new[]
            {
                child1,
                child2
            });
            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();

            childNodePresenter.TreeView = treeview;
            Expect.Call(childNodePresenter.NodeTagType).Return(typeof(Child)).Repeat.Any();

            // Collection changed is bubbled to nodePresenter => expect 1 call during parent.Children.Remove(child2);
            // not that in some cases Full refresh can be called
            Expect.Call(() => childNodePresenter.OnCollectionChanged(parent, new NotifyCollectionChangingEventArgs())).IgnoreArguments().Repeat.Any();

            mocks.ReplayAll();

            controller.OnTreeViewHandleCreated();

            controller.NodePresenters.AddRange(new[]
            {
                parentNodePresenter,
                childNodePresenter
            });
            controller.Data = parent;

            // generate collection changed with listeners enabled
            parent.Children.Remove(child2);

            controller.Data = null;

            // generate collection changed with listeners disabled
            parent.Children.Add(child2);
            
            mocks.VerifyAll();
        }

        [Test]
        public void TestRefreshChildNodesOfNodeWithoutChildren()
        {
            var mocks = new MockRepository();

            var treeView = mocks.Stub<ITreeView>();

            var treeViewController = new TreeViewController(treeView);

            var parentNodePresenter = mocks.Stub<ITreeNodePresenter>();
            treeViewController.NodePresenters.AddRange(new[]
            {
                parentNodePresenter
            });

            var nodes = new List<ITreeNode>();
            Expect.Call(treeView.Nodes).Return(nodes).Repeat.Any();
            Expect.Call(treeView.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
            treeView.Expect(tv => tv.RefreshChildNodes(null)).IgnoreArguments().Repeat.Once();

            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
            Expect.Call(parentNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Twice();
            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
            mocks.ReplayAll();

            treeViewController.Data = new Parent();

            // should not create create sub nodes
            treeViewController.RefreshChildNodes(nodes[0]);

            Assert.AreEqual(0, nodes[0].Nodes.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void TestRefreshChildNodesOfExpandedNode()
        {
            var mocks = new MockRepository();
            var parentNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
            var childNodePresenter = mocks.StrictMock<ITreeNodePresenter>();

            var treeview = new TreeView();
            var parent = new Parent();
            var child1 = new Child();
            var child2 = new Child();

            parent.Children.AddRange(new[]
            {
                child1,
                child2
            });

            parentNodePresenter.TreeView = treeview;
            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
            Expect.Call(parentNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(parent.Children).Repeat.Any();

            childNodePresenter.TreeView = treeview;
            Expect.Call(childNodePresenter.NodeTagType).Return(typeof(Child)).Repeat.Any();
            Expect.Call(() => childNodePresenter.UpdateNode(null, null, null)).IgnoreArguments().Repeat.Times(5);
            Expect.Call(childNodePresenter.GetChildNodeObjects(parent, null)).Return(parent.Children).Repeat.Any();
            Expect.Call(childNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Any();

            mocks.ReplayAll();

            var controller = TypeUtils.GetField<TreeView, TreeViewController>(treeview, "controller");
            controller.NodePresenters.AddRange(new[]
            {
                parentNodePresenter,
                childNodePresenter
            });

            controller.Data = parent;

            var parentNode = treeview.Nodes[0];

            // setting the controller.Data creates a root node and expands it if it has children
            Assert.AreEqual(2, parentNode.Nodes.Count);

            // simulate removing child2 from parent by changing the number of children returned by the parent node presenter
            parentNodePresenter.BackToRecord(BackToRecordOptions.All);
            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
            Expect.Call(parentNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(new[]
            {
                child1
            });
            parentNodePresenter.Replay();

            // updates the tree view to the new parent node state (parent has 1 child item)
            controller.RefreshChildNodes(parentNode);

            Assert.AreEqual(1, parentNode.Nodes.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void TestRefreshChildNodesOfUnExpandedNode()
        {
            var mocks = new MockRepository();

            var parentNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
            var childNodePresenter = mocks.StrictMock<ITreeNodePresenter>();

            var treeview = new TreeView();
            var parent = new Parent();
            var child1 = new Child();
            var child2 = new Child();

            parentNodePresenter.TreeView = treeview;
            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
            Expect.Call(parentNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(new[]
            {
                child1,
                child2
            }).Repeat.Any();

            childNodePresenter.TreeView = treeview;
            Expect.Call(childNodePresenter.NodeTagType).Return(typeof(Child)).Repeat.Any();
            Expect.Call(() => childNodePresenter.UpdateNode(null, null, null)).IgnoreArguments().Repeat.Times(6);
            Expect.Call(childNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Any();

            mocks.ReplayAll();

            var controller = TypeUtils.GetField<TreeView, TreeViewController>(treeview, "controller");
            controller.NodePresenters.AddRange(new[]
            {
                parentNodePresenter,
                childNodePresenter
            });

            controller.Data = parent;

            var parentNode = treeview.Nodes[0];

            parentNode.Collapse();
            controller.RefreshChildNodes(parentNode);

            Assert.AreEqual(2, parentNode.Nodes.Count);

            mocks.VerifyAll();
        }

        [Test]
        public void TestRefreshUnLoadedNode()
        {
            var mocks = new MockRepository();

            var treeNode = mocks.StrictMock<TreeNode>();
            var treeview = mocks.StrictMock<ITreeView>();
            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();

            var tag = new object();

            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
            Expect.Call(treeview.BeginUpdate).IgnoreArguments().Repeat.Any();
            Expect.Call(treeview.EndUpdate).IgnoreArguments().Repeat.Any();

            treeNode.HasChildren = true;
            Expect.Call(((ITreeNode) treeNode).Tag).Return(tag).Repeat.Any();
            Expect.Call(((ITreeNode) treeNode).IsLoaded).Return(false).Repeat.Any();
            Expect.Call(((ITreeNode) treeNode).Parent).Return(null);
            Expect.Call(((ITreeNode) treeNode).Presenter).Return(nodePresenter).Repeat.Any();

            nodePresenter.TreeView = treeview;
            Expect.Call(nodePresenter.NodeTagType).Return(typeof(object)).Repeat.Any();
            Expect.Call(nodePresenter.GetChildNodeObjects(tag, null)).IgnoreArguments().Return(new[]
            {
                new object()
            });
            Expect.Call(() => nodePresenter.UpdateNode(null, null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview);
            presenter.NodePresenters.Add(nodePresenter);

            presenter.UpdateNode(treeNode);

            mocks.VerifyAll();
        }

        [Test]
        public void TestRefreshLoadedNode()
        {
            var mocks = new MockRepository();

            var parentTreeNode = mocks.StrictMock<TreeNode>();
            var childTreeNode = mocks.StrictMock<TreeNode>();
            var treeview = mocks.StrictMock<ITreeView>();
            var parentNodePresenter = mocks.StrictMock<ITreeNodePresenter>();
            var childNodePresenter = mocks.StrictMock<ITreeNodePresenter>();

            var parent = new Parent();
            var child = new Child();
            var subNodes = new List<ITreeNode>
            {
                childTreeNode
            };

            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
            Expect.Call(treeview.BeginUpdate).IgnoreArguments().Repeat.Any();
            Expect.Call(treeview.EndUpdate).IgnoreArguments().Repeat.Any();

            parentTreeNode.HasChildren = true;
            Expect.Call(((ITreeNode) parentTreeNode).Tag).Return(parent).Repeat.Any();
            Expect.Call(((ITreeNode) parentTreeNode).IsLoaded).Return(true).Repeat.Any();
            Expect.Call(((ITreeNode) parentTreeNode).Parent).Return(null);
            Expect.Call(((ITreeNode) parentTreeNode).Nodes).Return(subNodes).Repeat.Any();
            Expect.Call(((ITreeNode) parentTreeNode).Presenter).Return(parentNodePresenter).Repeat.Any();

            childTreeNode.HasChildren = false;
            Expect.Call(((ITreeNode) childTreeNode).Tag).Return(child).Repeat.Any();
            Expect.Call(((ITreeNode) childTreeNode).IsLoaded).Return(false).Repeat.Any();
            Expect.Call(((ITreeNode) childTreeNode).Parent).Return(parentTreeNode).Repeat.Any();
            Expect.Call(((ITreeNode) childTreeNode).Nodes).Return(new List<ITreeNode>()).Repeat.Any();
            Expect.Call(((ITreeNode) childTreeNode).Presenter).Return(childNodePresenter).Repeat.Any();

            parentNodePresenter.TreeView = treeview;
            Expect.Call(parentNodePresenter.NodeTagType).Return(typeof(Parent)).Repeat.Any();
            Expect.Call(parentNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(new[]
            {
                child
            }).Repeat.Any();

            childNodePresenter.TreeView = treeview;
            Expect.Call(childNodePresenter.NodeTagType).Return(typeof(Child)).Repeat.Any();
            Expect.Call(childNodePresenter.GetChildNodeObjects(null, null)).IgnoreArguments().Return(Enumerable.Empty<object>()).Repeat.Any();

            // expect 1 update call for the parent & child
            Expect.Call(() => parentNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();
            Expect.Call(() => childNodePresenter.UpdateNode(null, null, null)).IgnoreArguments();

            mocks.ReplayAll();

            var controller = new TreeViewController(treeview);
            controller.NodePresenters.Add(parentNodePresenter);
            controller.NodePresenters.Add(childNodePresenter);

            controller.UpdateNode(parentTreeNode);

            mocks.VerifyAll();
        }

        [Test]
        public void TestCanRenameNode()
        {
            var mocks = new MockRepository();
            var treeview = mocks.StrictMock<ITreeView>();
            var treeNode = mocks.StrictMock<ITreeNode>();
            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();
            var tag = new object();

            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
            Expect.Call(treeNode.Tag).Return(tag).Repeat.Any();
            Expect.Call(treeNode.Presenter).Return(null).Repeat.Once();
            Expect.Call(treeNode.Presenter).Return(nodePresenter).Repeat.Once();

            nodePresenter.TreeView = treeview;
            //Expect.Call(nodePresenter.IsPresenterForNode(null)).IgnoreArguments().Return(true);
            Expect.Call(nodePresenter.NodeTagType).Return(typeof(object)).Repeat.Any();

            // node presenter decides if a node can be renamed
            Expect.Call(nodePresenter.CanRenameNode(null)).IgnoreArguments().Return(true);

            mocks.ReplayAll();

            var presenter = new TreeViewController(treeview);

            // no node
            Assert.IsFalse(presenter.CanRenameNode(null));

            // no node presenter
            Assert.IsFalse(presenter.CanRenameNode(treeNode));

            presenter.NodePresenters.Add(nodePresenter);

            // node presenter decides
            Assert.IsTrue(presenter.CanRenameNode(treeNode));

            mocks.VerifyAll();
        }

        [Test]
        public void TestCanDeleteNode()
        {
            var mocks = new MockRepository();
            var nodePresenter = mocks.StrictMock<ITreeNodePresenter>();
            var treeNode = mocks.StrictMock<ITreeNode>();
            var treeview = mocks.StrictMock<ITreeView>();
            var tag = new object();

            nodePresenter.TreeView = treeview;
            //Expect.Call(nodePresenter.IsPresenterForNode(null)).IgnoreArguments().Return(true);
            Expect.Call(nodePresenter.NodeTagType).Return(typeof(object)).Repeat.Any();

            // node presenter decides if a node can be removed
            Expect.Call(nodePresenter.CanRemove(null, null)).IgnoreArguments().Return(true);

            Expect.Call(treeview.GetNodeByTag(null)).IgnoreArguments().Return(null).Repeat.Any();
            Expect.Call(treeNode.Tag).Return(tag).Repeat.Any();
            Expect.Call(treeNode.Parent).Return(null);
            Expect.Call(treeNode.Presenter).Return(null).Repeat.Once();
            Expect.Call(treeNode.Presenter).Return(nodePresenter).Repeat.Once();

            mocks.ReplayAll();

            var controller = new TreeViewController(treeview);

            // no node
            Assert.IsFalse(controller.CanDeleteNode(null));

            // no node presenter
            Assert.IsFalse(controller.CanDeleteNode(treeNode));

            controller.NodePresenters.Add(nodePresenter);

            // node presenter decides
            Assert.IsTrue(controller.CanDeleteNode(treeNode));

            mocks.VerifyAll();
        }
    }
}