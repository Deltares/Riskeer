using System.ComponentModel;

using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.Utils.Collections;

using NUnit.Framework;

using Rhino.Mocks;

using Wti.Data;
using Wti.Forms.NodePresenters;

using WtiFormsResources = Wti.Forms.Properties.Resources;

namespace Wti.Forms.Test.NodePresenters
{
    [TestFixture]
    public class WtiProjectNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // call
            var nodePresenter = new WtiProjectNodePresenter();

            // assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(WtiProject), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // setup
            const string projectName = "<Insert Project Name Here>";

            var mocks = new MockRepository();
            var projectNode = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            var project = new WtiProject { Name = projectName };

            // call
            nodePresenter.UpdateNode(null, projectNode, project);

            // assert
            Assert.AreEqual(projectName, projectNode.Text);
            Assert.AreEqual(16, projectNode.Image.Height);
            Assert.AreEqual(16, projectNode.Image.Width);
        }

        [Test]
        public void GetChildNodeObjects_WithData_ReturnAllChildNodes()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            var project = new WtiProject();

            // call
            var children = nodePresenter.GetChildNodeObjects(project, nodeMock);

            // assert
            CollectionAssert.IsEmpty(children);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // call
            var renameAllowed = nodePresenter.CanRenameNode(nodeMock);

            // assert
            Assert.IsTrue(renameAllowed);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnTrue()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // call
            var renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // assert
            Assert.IsTrue(renameAllowed);
            mocks.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeRenamed_WithData_SetProjectName()
        {
            // setup
            var nodePresenter = new WtiProjectNodePresenter();

            var project = new WtiProject();

            // call
            const string newName = "New Name";
            nodePresenter.OnNodeRenamed(project, newName);

            // assert
            Assert.AreEqual(newName, project.Name);
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

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

            var nodePresenter = new WtiProjectNodePresenter();

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

            var nodePresenter = new WtiProjectNodePresenter();

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

            var nodePresenter = new WtiProjectNodePresenter();

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

            var nodePresenter = new WtiProjectNodePresenter();

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

            var nodePresenter = new WtiProjectNodePresenter();

            // call
            nodePresenter.OnNodeSelected(dataMock);

            // assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_ReturnNull()
        {
            // setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // assert
            Assert.IsNull(contextMenu);
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

            var nodePresenter = new WtiProjectNodePresenter();

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

            var nodePresenter = new WtiProjectNodePresenter();

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

            var nodePresenter = new WtiProjectNodePresenter();

            // call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // assert
            Assert.IsTrue(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_ProjectWithWtiProject_ReturnTrueAndRemoveWtiProject()
        {
            // setup
            var wtiProject = new WtiProject();

            var project = new Project();
            project.Items.Add(wtiProject);

            var nodePresenter = new WtiProjectNodePresenter();

            // call
            bool removalSuccesful = nodePresenter.RemoveNodeData(project, wtiProject);

            // assert
            Assert.IsTrue(removalSuccesful);
            CollectionAssert.DoesNotContain(project.Items, wtiProject);
        }
    }
}