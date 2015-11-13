using System.ComponentModel;
using System.Linq;
using Core.Common.Controls;
using Core.Common.Utils.Collections;
using System.Windows.Forms;
using Core.Common.Base;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class AssessmentSectionNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new AssessmentSectionNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(AssessmentSection), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string projectName = "<Insert Project Name Here>";

            var mocks = new MockRepository();
            var projectNode = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionNodePresenter();

            var assessmentSection = new AssessmentSection
            {
                Name = projectName
            };

            // Call
            nodePresenter.UpdateNode(null, projectNode, assessmentSection);

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

            var nodePresenter = new AssessmentSectionNodePresenter();

            var assessmentSection = new AssessmentSection();

            // Call
            var children = nodePresenter.GetChildNodeObjects(assessmentSection, nodeMock);

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

            var nodePresenter = new AssessmentSectionNodePresenter();

            var assessmentSection = new AssessmentSection();
            assessmentSection.InitializePipingFailureMechanism();

            // Call
            var children = nodePresenter.GetChildNodeObjects(assessmentSection, nodeMock).Cast<object>().AsList();

            // Assert
            Assert.AreEqual(1, children.Count);
            Assert.AreSame(assessmentSection.PipingFailureMechanism, children[0]);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionNodePresenter();

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

            var nodePresenter = new AssessmentSectionNodePresenter();

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
            var assessmentSectionObserver = mocks.StrictMock<IObserver>();
            assessmentSectionObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionNodePresenter();

            var assessmentSection = new AssessmentSection();
            assessmentSection.Attach(assessmentSectionObserver);

            // Call
            const string newName = "New Name";
            nodePresenter.OnNodeRenamed(assessmentSection, newName);

            // Assert
            Assert.AreEqual(newName, assessmentSection.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionNodePresenter();

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

            var nodePresenter = new AssessmentSectionNodePresenter();

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

            var nodePresenter = new AssessmentSectionNodePresenter();

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

            var nodePresenter = new AssessmentSectionNodePresenter();

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

            var nodePresenter = new AssessmentSectionNodePresenter();

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

            var nodePresenter = new AssessmentSectionNodePresenter();

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
            var assessmentSection = new AssessmentSection();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, assessmentSection);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(1, contextMenu.Items.Count);
            ToolStripItem addPipingItem = contextMenu.Items[0];
            Assert.AreEqual(RingtoetsFormsResources.AssessmentSectionNodePresenter_ContextMenu_Add_PipingFailureMechanism, addPipingItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.AssessmentSectionNodePresenter_ContextMenu_Add_PipingFailureMechanism_Tooltip, addPipingItem.ToolTipText);
            Assert.IsTrue(addPipingItem.Enabled);
            Assert.AreSame(assessmentSection, addPipingItem.Tag);
            Assert.AreEqual(16, addPipingItem.Image.Height);
            Assert.AreEqual(16, addPipingItem.Image.Width);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_WithPipingFailureMechanismAlreadySet_ReturnsContextMenuWithOneDisabledItem()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var assessmentSection = new AssessmentSection();
            assessmentSection.InitializePipingFailureMechanism();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, assessmentSection);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(1, contextMenu.Items.Count);
            ToolStripItem addPipingItem = contextMenu.Items[0];
            Assert.AreEqual(RingtoetsFormsResources.AssessmentSectionNodePresenter_ContextMenu_Add_PipingFailureMechanism, addPipingItem.Text);
            Assert.AreEqual(RingtoetsFormsResources.AssessmentSectionNodePresenter_ContextMenu_PipingFailureMechanism_Already_Added_Tooltip, addPipingItem.ToolTipText);
            Assert.IsFalse(addPipingItem.Enabled);
            Assert.IsNull(addPipingItem.Tag);
            Assert.AreEqual(16, addPipingItem.Image.Height);
            Assert.AreEqual(16, addPipingItem.Image.Width);
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

            var nodePresenter = new AssessmentSectionNodePresenter();

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

            var nodePresenter = new AssessmentSectionNodePresenter();

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

            var nodePresenter = new AssessmentSectionNodePresenter();

            // Call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // Assert
            Assert.IsTrue(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_ProjectWithAssessmentSection_ReturnTrueAndRemoveAssessmentSection()
        {
            // Setup
            var mocks = new MockRepository();

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection();

            var project = new Project();
            project.Items.Add(assessmentSection);
            project.Attach(observerMock);

            var nodePresenter = new AssessmentSectionNodePresenter();

            // Call
            bool removalSuccesful = nodePresenter.RemoveNodeData(project, assessmentSection);

            // Assert
            Assert.IsTrue(removalSuccesful);
            CollectionAssert.DoesNotContain(project.Items, assessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void AssessmentSectionWithoutPipingFailureMechanism_AddPipingFailureMechanismThroughContextMenu_AssessmentSectionHasPipingFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var assessmentSection = new AssessmentSection();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionNodePresenter();
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, assessmentSection);

            // Preconditions
            Assert.IsNotNull(contextMenu);
            Assert.IsNull(assessmentSection.PipingFailureMechanism);
            Assert.AreEqual(1, contextMenu.Items.Count);

            // Call
            contextMenu.Items[0].PerformClick();

            // Assert
            Assert.IsNotNull(assessmentSection.PipingFailureMechanism);
        }
    }
}