using System;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.NodePresenters;

using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.NodePresenters
{
    [TestFixture]
    public class AssessmentSectionBaseNodePresenterTest
    {
        private MockRepository mockRepository;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new AssessmentSectionBaseNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            
            mockRepository.ReplayAll();

            // Call
            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(AssessmentSectionBase), nodePresenter.NodeTagType);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string projectName = "<Insert Project Name Here>";

            var mocks = new MockRepository();
            var projectNode = mocks.Stub<TreeNode>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

            assessmentSection.Name = projectName;

            // Call
            nodePresenter.UpdateNode(null, projectNode, assessmentSection);

            // Assert
            Assert.AreEqual(projectName, projectNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), projectNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.AssessmentSectionFolderIcon, projectNode.Image);
        }

        [Test]
        public void GetChildNodeObjects_DataIsAssessmentSectionBase_ReturnDikeInputsAndFailureMechanisms()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
            
            var failureMechanismCollection = new[]
            {
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
            };

            assessmentSectionMock.Expect(a => a.GetFailureMechanisms()).Return(failureMechanismCollection);

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var children = nodePresenter.GetChildNodeObjects(assessmentSectionMock).Cast<object>().ToArray();

            // Assert
            Assert.AreEqual(7, children.Length);
            Assert.AreSame(assessmentSectionMock.ReferenceLine, children[0]);
            Assert.AreSame(assessmentSectionMock.FailureMechanismContribution, children[1]);
            Assert.AreSame(assessmentSectionMock.HydraulicBoundaryDatabase, children[2]);

            CollectionAssert.AreEqual(failureMechanismCollection, children.Skip(3));

            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<TreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

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
            var nodeMock = mocks.StrictMock<TreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            assessmentSectionObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

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
            var nodeMock = mocks.StrictMock<TreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<AssessmentSectionBase>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<AssessmentSectionBase>();
            var sourceMock = mocks.StrictMock<TreeNode>();
            var targetMock = mocks.StrictMock<TreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMock = mocks.StrictMock<AssessmentSectionBase>();
            var sourceMock = mocks.StrictMock<TreeNode>();
            var targetMock = mocks.StrictMock<TreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

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
            var dataMockOwner = mocks.StrictMock<object>();
            var targetMock = mocks.StrictMock<AssessmentSectionBase>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnDragDrop(dataMock, dataMockOwner, targetMock, DragOperations.Move, 2);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<AssessmentSectionBase>();
            var nodeMock = mocks.StrictMock<TreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

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
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var project = new Project();
            project.Items.Add(assessmentSection);
            project.Attach(observerMock);

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool removalSuccesful = nodePresenter.RemoveNodeData(project, assessmentSection);

            // Assert
            Assert.IsTrue(removalSuccesful);
            CollectionAssert.DoesNotContain(project.Items, assessmentSection);
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderProviderMock = mocks.StrictMock<IContextMenuBuilderProvider>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            var nodeMock = mocks.StrictMock<TreeNode>();

            menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            contextMenuBuilderProviderMock.Expect(cmp => cmp.Get(nodeMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.GetContextMenu(nodeMock, assessmentSection);

            // Assert
            mocks.VerifyAll();
        }
    }
}