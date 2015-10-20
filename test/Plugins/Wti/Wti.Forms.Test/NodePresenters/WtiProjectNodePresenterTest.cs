using System.ComponentModel;
using System.Linq;
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
            // Call
            var nodePresenter = new WtiProjectNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(WtiProject), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string projectName = "<Insert Project Name Here>";

            var mocks = new MockRepository();
            var projectNode = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            var project = new WtiProject
            {
                Name = projectName
            };

            // Call
            nodePresenter.UpdateNode(null, projectNode, project);

            // Assert
            Assert.AreEqual(projectName, projectNode.Text);
            Assert.AreEqual(16, projectNode.Image.Height);
            Assert.AreEqual(16, projectNode.Image.Width);
        }

        [Test]
        public void GetChildNodeObjects_WithoutPipingFailureMechanism_ReturnsEmptyList()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            var project = new WtiProject();

            // Call
            var children = nodePresenter.GetChildNodeObjects(project, nodeMock);

            // Assert
            CollectionAssert.IsEmpty(children);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void GetChildNodeObjects_WithPipingFailureMechanism_ReturnPipingFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            var project = new WtiProject();
            project.InitializePipingFailureMechanism();

            // Call
            var children = nodePresenter.GetChildNodeObjects(project, nodeMock).Cast<object>().AsList();

            // Assert
            Assert.AreEqual(1, children.Count);
            Assert.AreSame(project.PipingFailureMechanism, children[0]);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            var renameAllowed = nodePresenter.CanRenameNode(nodeMock);

            // Assert
            Assert.IsTrue(renameAllowed);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            var renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsTrue(renameAllowed);
            mocks.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeRenamed_WithData_SetProjectNameWithNotification()
        {
            // Setup
            var mocks = new MockRepository();
            var projectObserver = mocks.StrictMock<IObserver>();
            projectObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            var project = new WtiProject();
            project.Attach(projectObserver);

            // Call
            const string newName = "New Name";
            nodePresenter.OnNodeRenamed(project, newName);

            // Assert
            Assert.AreEqual(newName, project.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            nodePresenter.OnNodeChecked(nodeMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanDrag_Always_ReturnNone()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            DragOperations dragAllowed = nodePresenter.CanDrag(dataMock);

            // Assert
            Assert.AreEqual(DragOperations.None, dragAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_Always_ReturnNone()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            DragOperations dropAllowed = nodePresenter.CanDrop(dataMock, sourceMock, targetMock, DragOperations.Move);

            // Assert
            Assert.AreEqual(DragOperations.None, dropAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void CanInsert_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            bool insertionAllowed = nodePresenter.CanInsert(dataMock, sourceMock, targetMock);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnDragDrop_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            nodePresenter.OnDragDrop(dataMock, sourceParentNodeMock, targetParentNodeDataMock, DragOperations.Move, 2);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnNodeSelected_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_WithNoPipingFailureMechanism_ReturnsContextMenuWithOneItemWithDataAsTag()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var wtiProject = new WtiProject();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, wtiProject);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(1, contextMenu.Items.Count);
            Assert.AreEqual(WtiFormsResources.AddPipingFailureMechanismContextMenuItem, contextMenu.Items[0].Text);
            Assert.AreEqual(WtiFormsResources.WtiProjectTooltipAddPipingFailureMechanism, contextMenu.Items[0].ToolTipText);
            Assert.IsTrue(contextMenu.Items[0].Enabled);
            Assert.AreSame(wtiProject, contextMenu.Items[0].Tag);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_WithPipingFailureMechanismAlreadySet_ReturnsContextMenuWithOneDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var wtiProject = new WtiProject();
            wtiProject.InitializePipingFailureMechanism();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, wtiProject);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(1, contextMenu.Items.Count);
            Assert.AreEqual(WtiFormsResources.AddPipingFailureMechanismContextMenuItem, contextMenu.Items[0].Text);
            Assert.AreEqual(WtiFormsResources.WtiProjectTooltipPipingFailureMechanismAlreadyAdded, contextMenu.Items[0].ToolTipText);
            Assert.IsFalse(contextMenu.Items[0].Enabled);
            Assert.IsNull(contextMenu.Items[0].Tag);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            nodePresenter.OnPropertyChanged(dataMock, nodeMock, eventArgsMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnCollectionChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // Assert
            Assert.IsTrue(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_ProjectWithWtiProject_ReturnTrueAndRemoveWtiProject()
        {
            // Setup
            var mocks = new MockRepository();

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var wtiProject = new WtiProject();

            var project = new Project();
            project.Items.Add(wtiProject);
            project.Attach(observerMock);

            var nodePresenter = new WtiProjectNodePresenter();

            // Call
            bool removalSuccesful = nodePresenter.RemoveNodeData(project, wtiProject);

            // Assert
            Assert.IsTrue(removalSuccesful);
            CollectionAssert.DoesNotContain(project.Items, wtiProject);
            mocks.VerifyAll();
        }

        [Test]
        public void GivenWtiProjectWithoutPipingFailureMechanism_WhenAddPipingFailureMechanismThroughContextMenu_ThenWtiProjectHasPipingFailureMechanismAssigned()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var wtiProject = new WtiProject();
            mocks.ReplayAll();

            var nodePresenter = new WtiProjectNodePresenter();
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, wtiProject);

            // Preconditions
            Assert.IsNotNull(contextMenu);
            Assert.IsNull(wtiProject.PipingFailureMechanism);
            Assert.AreEqual(1, contextMenu.Items.Count);

            // Call
            contextMenu.Items[0].PerformClick();

            // Assert
            Assert.IsNotNull(wtiProject.PipingFailureMechanism);
        }
    }
}