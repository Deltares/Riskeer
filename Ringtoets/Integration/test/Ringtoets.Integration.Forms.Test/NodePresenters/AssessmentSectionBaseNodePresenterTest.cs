using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;
using Core.Common.Utils.Collections;

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
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), projectNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.AssessmentSectionFolderIcon, projectNode.Image);
        }

        [Test]
        public void GetChildNodeObjects_DataIsDikeAssessmentSection_ReturnDikeInputsAndFailureMechanisms()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();
            var nodeMock = mocks.StrictMock<ITreeNode>();

            var failureMechanismCollection = new[]
            {
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
                mocks.StrictMock<IFailureMechanism>(),
            };

            assessmentSectionMock.Expect(a => a.GetFailureMechanisms()).Return(failureMechanismCollection);
            mocks.ReplayAll();

            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            // Call
            var children = nodePresenter.GetChildNodeObjects(assessmentSectionMock).Cast<object>().AsList();

            // Assert
            Assert.AreEqual(7, children.Count);
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

        [Test]
        public void GetContextMenu_NoMenuBuilderProvider_ReturnsNull()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var nodePresenter = new AssessmentSectionBaseNodePresenter();

            mocks.ReplayAll();

            // Call
            var result = nodePresenter.GetContextMenu(nodeMock, new DikeAssessmentSection());

            // Assert
            Assert.IsNull(result);

            mocks.ReplayAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_MenuBuilderProvider_ReturnsFourItems(bool commonItemsEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var nodePresenter = new AssessmentSectionBaseNodePresenter
            {
                ContextMenuBuilderProvider = TestContextMenuBuilderProvider.Create(mocks, nodeMock, true)
            };

            mocks.ReplayAll();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, new DikeAssessmentSection());

            // Assert
            Assert.AreEqual(9, contextMenu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, CoreCommonGuiResources.Delete, CoreCommonGuiResources.Delete_ToolTip, CoreCommonGuiResources.DeleteIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 2, CoreCommonGuiResources.Expand_all, CoreCommonGuiResources.Expand_all_ToolTip, CoreCommonGuiResources.ExpandAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, CoreCommonGuiResources.Collapse_all, CoreCommonGuiResources.Collapse_all_ToolTip, CoreCommonGuiResources.CollapseAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 5, CoreCommonGuiResources.Import, CoreCommonGuiResources.Import_ToolTip, CoreCommonGuiResources.ImportIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 6, CoreCommonGuiResources.Export, CoreCommonGuiResources.Export_ToolTip, CoreCommonGuiResources.ExportIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 8, CoreCommonGuiResources.Properties, CoreCommonGuiResources.Properties_ToolTip, CoreCommonGuiResources.PropertiesIcon);

            Assert.IsInstanceOf<ToolStripSeparator>(contextMenu.Items[1]);
            Assert.IsInstanceOf<ToolStripSeparator>(contextMenu.Items[4]);
            Assert.IsInstanceOf<ToolStripSeparator>(contextMenu.Items[7]);
        }
    }
}