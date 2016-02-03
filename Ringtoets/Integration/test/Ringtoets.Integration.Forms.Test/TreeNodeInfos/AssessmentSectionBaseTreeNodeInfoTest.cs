using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
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
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Plugin;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class AssessmentSectionBaseTreeNodeInfoTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(AssessmentSectionBase));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(AssessmentSectionBase), info.TagType);

            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();
            var testName = "ttt";
            assessmentSection.Name = testName;

            mocks.ReplayAll();

            // Call
            var text = info.Text(assessmentSection);

            // Assert
            Assert.AreEqual(testName, text);
            
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<AssessmentSectionBase>();

            mocks.ReplayAll();

            // Call
            var image = info.Image(assessmentSection);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.AssessmentSectionFolderIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            failureMechanism.Expect(f => f.Name).Return("some name");
            failureMechanism.Expect(f => f.Contribution).Return(100);

            mocks.ReplayAll();

            var failureMechanismList = new List<IFailureMechanism> { failureMechanism };
            var contribution = new FailureMechanismContribution(failureMechanismList, 10.0, 2);
            var assessmentSection = new TestAssessmentSectionBase(contribution, failureMechanismList);

            // Call
            var objects = info.ChildNodeObjects(assessmentSection);

            // Assert
            var collection = new List<object>();
            collection.Add(assessmentSection.ReferenceLine);
            collection.Add(contribution);
            collection.Add(assessmentSection.HydraulicBoundaryDatabase);
            collection.Add(failureMechanism);
            CollectionAssert.AreEqual(collection, objects);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var treeNode = new TreeNode();

            var gui = mocks.StrictMultiMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            gui.Expect(g => g.Get(treeNode, treeViewControlMock)).Return(menuBuilderMock);
            
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
            
            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(null, treeNode, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CanRename_Always_ReturnsTrue()
        {
            // Call
            var canRename = info.CanRename(null);

            // Assert
            Assert.IsTrue(canRename);
        }
        
        [Test]
        public void OnNodeRenamed_WithData_SetProjectNameWithNotification()
        {
            // Setup
            var assessmentSectionObserver = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
        
            assessmentSectionObserver.Expect(o => o.UpdateObserver());
        
            mocks.ReplayAll();
        
            assessmentSection.Attach(assessmentSectionObserver);
        
            // Call
            const string newName = "New Name";
            info.OnNodeRenamed(assessmentSection, newName);
        
            // Assert
            Assert.AreEqual(newName, assessmentSection.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnsTrue()
        {
            // Call
            var canRemove = info.CanRemove(null, null);

            // Assert
            Assert.IsTrue(canRemove);
        }
        
        [Test]
        public void RemoveNodeData_ProjectWithAssessmentSection_ReturnTrueAndRemoveAssessmentSection()
        {
            // Setup
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
        
            observerMock.Expect(o => o.UpdateObserver());
        
            mocks.ReplayAll();
        
            var project = new Project();
            project.Items.Add(assessmentSection);
            project.Attach(observerMock);
        
        
            // Call
            info.OnNodeRemoved(assessmentSection, project);
        
            // Assert
            CollectionAssert.DoesNotContain(project.Items, assessmentSection);
            mocks.VerifyAll();
        }
    }

    public class TestAssessmentSectionBase : AssessmentSectionBase {

        private readonly IEnumerable<IFailureMechanism> failureMechanisms;

        public TestAssessmentSectionBase(FailureMechanismContribution contribution, IEnumerable<IFailureMechanism> failureMechanisms)
        {
            FailureMechanismContribution = contribution;
            this.failureMechanisms = failureMechanisms;
        }
        
        public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
        {
            return failureMechanisms;
        }
    }
}