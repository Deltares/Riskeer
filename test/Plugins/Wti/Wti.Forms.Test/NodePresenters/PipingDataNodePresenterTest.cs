using System;
using System.ComponentModel;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Utils.Collections;
using DeltaShell.Tests.TestObjects;
using NUnit.Framework;
using Rhino.Mocks;
using Wti.Data;
using Wti.Forms.NodePresenters;
using WtiFormsResources = Wti.Forms.Properties.Resources;

namespace Wti.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingDataNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var nodePresenter = new PipingDataNodePresenter();

            // assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(PipingData), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // setup
            const string projectName = "Piping";

            var mocks = new MockRepository();
            var pipingNode = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            var project = new PipingData
            {
                AssessmentLevel = 2.0
            };

            // call
            nodePresenter.UpdateNode(null, pipingNode, project);

            // assert
            Assert.AreEqual(projectName, pipingNode.Text);
            Assert.AreEqual(16, pipingNode.Image.Height);
            Assert.AreEqual(16, pipingNode.Image.Width);
        }

        [Test]
        public void GetChildNodeObjects_WithData_ReturnAllChildNodes()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            var project = new PipingData
            {
                Output = new PipingOutput(0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
            };

            // call
            var children = nodePresenter.GetChildNodeObjects(project, nodeMock);

            // assert
            Assert.AreEqual(1, children.Count());
            CollectionAssert.AllItemsAreInstancesOfType(children, typeof(PipingOutput));
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void GetChildNodeObjects_WithDataButNoOutput_ReturnAllChildNodes()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            var project = new PipingData();

            // call
            var children = nodePresenter.GetChildNodeObjects(project, nodeMock);

            // assert
            Assert.AreEqual(0, children.Count());
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            var renameAllowed = nodePresenter.CanRenameNode(nodeMock);

            // assert
            Assert.IsFalse(renameAllowed);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnTrue()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            var renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // assert
            Assert.IsFalse(renameAllowed);
            mocks.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            nodePresenter.OnNodeChecked(nodeMock);

            // assert
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanDrag_Always_ReturnNone()
        {
            // setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            DragOperations dragAllowed = nodePresenter.CanDrag(dataMock);

            // assert
            Assert.AreEqual(DragOperations.None, dragAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_Always_ReturnNone()
        {
            // setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            DragOperations dropAllowed = nodePresenter.CanDrop(dataMock, sourceMock, targetMock, DragOperations.Move);

            // assert
            Assert.AreEqual(DragOperations.None, dropAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void CanInsert_Always_ReturnFalse()
        {
            // setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            bool insertionAllowed = nodePresenter.CanInsert(dataMock, sourceMock, targetMock);

            // assert
            Assert.IsFalse(insertionAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnDragDrop_Always_DoNothing()
        {
            // setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            nodePresenter.OnDragDrop(dataMock, sourceParentNodeMock, targetParentNodeDataMock, DragOperations.Move, 2);

            // assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnNodeSelected_Always_DoNothing()
        {
            // setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            nodePresenter.OnNodeSelected(dataMock);

            // assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_NoContextMenuFunctionSet_ThrowsNullReferenceException()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            TestDelegate contextMenu = () => nodePresenter.GetContextMenu(nodeMock, dataMock);

            // assert
            Assert.Throws<NullReferenceException>(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_ContextMenuFunctionSet_CallsContextMenuFunction()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();
            var testContext = new TestMenuItem();
            nodePresenter.ContextMenu = a => testContext;

            // call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // assert
            Assert.AreSame(testContext, contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            nodePresenter.OnPropertyChanged(dataMock, nodeMock, eventArgsMock);

            // assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnCollectionChange_Always_DoNothing()
        {
            // setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // assert
            Assert.IsTrue(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_ProjectWithPipingData_ReturnFalseAlways()
        {
            // setup
            var PipingData = new PipingData();

            var project = new Project();
            project.Items.Add(PipingData);

            var nodePresenter = new PipingDataNodePresenter();

            // call
            bool removalSuccesful = nodePresenter.RemoveNodeData(project, PipingData);

            // assert
            Assert.IsFalse(removalSuccesful);
            CollectionAssert.Contains(project.Items, PipingData);
        }
    }
}