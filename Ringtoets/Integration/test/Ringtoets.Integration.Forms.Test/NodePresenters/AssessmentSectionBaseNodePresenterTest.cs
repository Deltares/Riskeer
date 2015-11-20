using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Utils.Collections;

using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.NodePresenters;

namespace Ringtoets.Integration.Forms.Test.NodePresenters
{
    [TestFixture]
    public class AssessmentSectionBaseNodePresenterTest
    {
        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(AssessmentSectionBase), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string projectName = "<Insert Project Name Here>";

            var mocks = new MockRepository();
            var projectNode = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            var assessmentSection = new DikeAssessmentSection
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
        public void GetChildNodeObjects_DataIsDikeAssessmentSection_ReturnDikeInputsAndFailureMechanisms()
        {
            // Setup
            var assessmentSection = new AssessmentSectionBaseTester();

            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            assessmentSection.FailureMechanismsInjectionPoint = new[]
            {
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
            };
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();




            // Call
            var children = nodePresenter.GetChildNodeObjects(assessmentSection, nodeMock).Cast<object>().AsList();

            // Assert
            Assert.AreEqual(7, children.Count);
            Assert.AreSame(assessmentSection.ReferenceLine, children[0]);
            Assert.AreSame(assessmentSection.FailureMechanismContribution, children[1]);
            Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase, children[2]);
            CollectionAssert.AreEqual(assessmentSection.GetFailureMechanisms(), children.Skip(3));
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            var assessmentSection = new DikeAssessmentSection();
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

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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
            var dataMock = mocks.StrictMock<DikeAssessmentSection>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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
            var dataMock = mocks.StrictMock<DikeAssessmentSection>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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
            var dataMock = mocks.StrictMock<DikeAssessmentSection>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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
            var dataMock = mocks.StrictMock<DikeAssessmentSection>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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
            var dataMock = mocks.StrictMock<DikeAssessmentSection>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var assessmentSection = new DikeAssessmentSection();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, assessmentSection);

            // Assert
            Assert.IsNull(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<DikeAssessmentSection>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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
            var dataMock = mocks.StrictMock<DikeAssessmentSection>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

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
            var dataMock = mocks.StrictMock<DikeAssessmentSection>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            // Call
            bool removalAllowed = nodePresenter.CanRemove(nodeMock, dataMock);

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

            var assessmentSection = new DikeAssessmentSection();

            var project = new Project();
            project.Items.Add(assessmentSection);
            project.Attach(observerMock);

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            // Call
            bool removalSuccesful = nodePresenter.RemoveNodeData(project, assessmentSection);

            // Assert
            Assert.IsTrue(removalSuccesful);
            CollectionAssert.DoesNotContain(project.Items, assessmentSection);
            mocks.VerifyAll();
        }

        private class AssessmentSectionBaseTester : AssessmentSectionBase
        {
            public AssessmentSectionBaseTester()
            {
                FailureMechanismsInjectionPoint = new IFailureMechanism[0];
            }

            public IEnumerable<IFailureMechanism> FailureMechanismsInjectionPoint { get; set; }

            public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                return FailureMechanismsInjectionPoint;

            }
        }
    }
}