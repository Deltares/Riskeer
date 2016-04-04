﻿using System.Linq;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;

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
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(IAssessmentSection));
        }

        [TearDown]
        public void TearDown()
        {
            plugin.Dispose();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(IAssessmentSection), info.TagType);
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
            var testName = "ttt";

            var assessmentSection = mocks.Stub<IAssessmentSection>();
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
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            var image = info.Image(assessmentSection);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.AssessmentSectionFolderIcon, image);

            mocks.VerifyAll();
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            // Call
            var result = info.EnsureVisibleOnCreate(assessmentSection);

            // Assert
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildsOnData()
        {
            // Setup
            var failureMechanisms = new IFailureMechanism[]
            {
                new PipingFailureMechanism(),
                new FailureMechanismPlaceholder("A")
            };
            var contribution = new FailureMechanismContribution(failureMechanisms, 10.0, 2);
            var comments = new AssessmentSectionComment
            {
                Text = "some comment"
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(section => section.FailureMechanismContribution).Return(contribution);
            assessmentSection.Stub(section => section.GetFailureMechanisms()).Return(failureMechanisms);
            assessmentSection.Stub(section => section.Comments).Return(comments);
            mocks.ReplayAll();


            // Call
            var objects = info.ChildNodeObjects(assessmentSection).ToArray();

            // Assert
            Assert.AreEqual(6, objects.Length);
            var referenceLineContext = (ReferenceLineContext)objects[0];
            Assert.AreSame(assessmentSection.ReferenceLine, referenceLineContext.WrappedData);
            Assert.AreSame(assessmentSection, referenceLineContext.Parent);

            Assert.AreSame(contribution, objects[1]);

            var context = (HydraulicBoundaryDatabaseContext)objects[2];
            Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase, context.Parent.HydraulicBoundaryDatabase);
            Assert.AreSame(assessmentSection, context.Parent);

            var pipingFailureMechanismContext = (PipingFailureMechanismContext)objects[4];
            Assert.AreSame(failureMechanisms[0], pipingFailureMechanismContext.WrappedData);
            Assert.AreSame(assessmentSection, pipingFailureMechanismContext.Parent);

            var placeholderFailureMechanismContext = (FailureMechanismPlaceholderContext)objects[5];
            Assert.AreSame(failureMechanisms[1], placeholderFailureMechanismContext.WrappedData);
            Assert.AreSame(assessmentSection, placeholderFailureMechanismContext.Parent);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var gui = mocks.StrictMultiMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            gui.Expect(g => g.Get(null, treeViewControlMock)).Return(menuBuilderMock);

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
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
            info.ContextMenuStrip(null, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void CanRename_Always_ReturnsTrue()
        {
            // Call
            var canRename = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(canRename);
        }
        
        [Test]
        public void OnNodeRenamed_WithData_SetProjectNameWithNotification()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Expect(section => section.NotifyObservers());

            mocks.ReplayAll();
        
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
        
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
}