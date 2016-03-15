﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private RingtoetsGuiPlugin plugin;
        private TreeNodeInfo info;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new RingtoetsGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext));
        }

        [TearDown]
        public override void TearDown()
        {
            plugin.Dispose();
            base.TearDown();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(HydraulicBoundaryDatabaseContext), info.TagType);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            var name = "Hydraulische randvoorwaarden";
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            mocks.ReplayAll();

            var context = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            // Call
            var text = info.Text(context);

            // Assert
            Assert.AreEqual(name, text);

            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsGenericIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GenericInputOutputIcon, image);
        }

        [Test]
        public void CanRenameNode_Always_ReturnsFalse()
        {
            // Call
            var renameAllowed = info.CanRename(null, null);

            // Assert
            Assert.IsFalse(renameAllowed);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            menuBuilderMock.Expect(mb => mb.AddOpenItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddImportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabaseSet_ContextMenuItemCalculateToetspeilenDisabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            const string expectedItemText = "&Toetspeilen berekenen";
            const string expectedItemTooltip = "Er is geen hydraulische randvoorwaarden database beschikbaar om de toetspeilen te berekenen.";
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, expectedItemText, expectedItemTooltip, RingtoetsFormsResources.FailureMechanismIcon, false);
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseSet_ContextMenuItemCalculateToetspeilenEnabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.StrictMock<AssessmentSectionBase>();

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);
            nodeData.Parent.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            nodeData.Parent.HydraulicBoundaryDatabase.FilePath = testDataPath;
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

            const string expectedItemText = "&Toetspeilen berekenen";
            const string expectedItemTooltip = "Bereken de toetspeilen";
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, expectedItemText, expectedItemTooltip, RingtoetsFormsResources.FailureMechanismIcon);
        }

        [Test]
        public void GivenHydraulicBoundaryDatabaseWithNonExistingFilePath_WhenCalculatingAssessmentLevelFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffectedAndObserversNotified()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();

            var assessmentSectionBaseObserver = mocks.StrictMock<IObserver>();
            var hydraulicBoundaryDatabaseContextObserver = mocks.StrictMock<IObserver>();

            var mainWindow = mocks.DynamicMock<IMainWindow>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var contextMenuRunAssessmentLevelCalculationsIndex = 3;
            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(100001, "", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(100002, "", 3.3, 4.4)
            {
                DesignWaterLevel = 4.2
            };
            var assessmentSectionBase = new AssessmentSectionBaseImplementation
            {
                Name = "Dummy",
                HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                {
                    Locations =
                    {
                        hydraulicBoundaryLocation1,
                        hydraulicBoundaryLocation2
                    },
                    FilePath = "D:/nonExistingDirectory/nonExistingFile",
                    Version = "random"
                }
            };
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSectionBase);

            assessmentSectionBaseObserver.Expect(o => o.UpdateObserver());
            hydraulicBoundaryDatabaseContextObserver.Expect(o => o.UpdateObserver());
            gui.Expect(cmp => cmp.Get(hydraulicBoundaryDatabaseContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            assessmentSectionBase.Attach(assessmentSectionBaseObserver);
            hydraulicBoundaryDatabaseContext.Attach(hydraulicBoundaryDatabaseContextObserver);

            plugin.Gui = gui;

            var contextMenuAdapter = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControlMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Don't care about dialogs in this test.
            };

            // When
            Action action = () => { contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick(); };

            // Then
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.GetEnumerator();
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Er is een fout opgetreden tijdens de berekening: inspecteer het logbestand.", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Uitvoeren van 'Toetspeil berekenen voor locatie '100001'' is mislukt.", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Er is een fout opgetreden tijdens de berekening: inspecteer het logbestand.", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Uitvoeren van 'Toetspeil berekenen voor locatie '100002'' is mislukt.", msgs.Current);
            });

            Assert.IsNaN(hydraulicBoundaryLocation1.DesignWaterLevel); // No result set
            Assert.AreEqual(4.2, hydraulicBoundaryLocation2.DesignWaterLevel); // Previous result not cleared

            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasNoReferenceLine_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

            // Call
            Color color = info.ForeColor(hydraulicBoundaryDatabaseContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasReferenceLineData_ReturnControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<AssessmentSectionBase>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

            // Call
            Color color = info.ForeColor(hydraulicBoundaryDatabaseContext);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            mocks.VerifyAll();
        }

        private class AssessmentSectionBaseImplementation : AssessmentSectionBase
        {
            public AssessmentSectionBaseImplementation()
            {
                FailureMechanismContribution = new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 10, 30000);
            }

            public override IEnumerable<IFailureMechanism> GetFailureMechanisms()
            {
                yield break;
            }
        }
    }
}