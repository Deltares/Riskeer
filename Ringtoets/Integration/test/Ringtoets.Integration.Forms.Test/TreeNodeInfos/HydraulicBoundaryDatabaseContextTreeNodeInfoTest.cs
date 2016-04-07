﻿using System;
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Plugin;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HydraulicBoundaryDatabaseContextTreeNodeInfoTest
    {
        private MockRepository mocks;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

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
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            var name = "Hydraulische randvoorwaarden";
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            var context = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var text = info.Text(context);

                // Assert
                Assert.AreEqual(name, text);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Image_Always_ReturnsGenericIcon()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
            }
        }

        [Test]
        public void CanRenameNode_Always_ReturnsFalse()
        {
            // Setup
            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var renameAllowed = info.CanRename(null, null);

                // Assert
                Assert.IsFalse(renameAllowed);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();

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
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControlMock);
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabaseSet_ContextMenuItemCalculateToetspeilenDisabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                plugin.Gui = guiMock;

                // Call
                var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

                const string expectedItemText = "&Toetspeilen berekenen";
                const string expectedItemTooltip = "Er is geen hydraulische randvoorwaarden database beschikbaar om de toetspeilen te berekenen.";
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, expectedItemText, expectedItemTooltip, RingtoetsFormsResources.FailureMechanismIcon, false);
            }
            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseSet_ContextMenuItemCalculateToetspeilenEnabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();

            var nodeData = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);
            nodeData.Parent.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);

                plugin.Gui = guiMock;

                // Call
                nodeData.Parent.HydraulicBoundaryDatabase.FilePath = testDataPath;
                var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

                const string expectedItemText = "&Toetspeilen berekenen";
                const string expectedItemTooltip = "Bereken de toetspeilen";
                TestHelper.AssertContextMenuStripContainsItem(contextMenu, 3, expectedItemText, expectedItemTooltip, RingtoetsFormsResources.FailureMechanismIcon);
            }
            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments

        }

        [Test]
        public void GivenHydraulicBoundaryDatabaseWithNonExistingFilePath_WhenCalculatingAssessmentLevelFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffected()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();

            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var contextMenuRunAssessmentLevelCalculationsIndex = 3;

            var hydraulicBoundaryLocation1 = new HydraulicBoundaryLocation(100001, "", 1.1, 2.2);
            var hydraulicBoundaryLocation2 = new HydraulicBoundaryLocation(100002, "", 3.3, 4.4)
            {
                DesignWaterLevel = 4.2
            };

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    hydraulicBoundaryLocation1,
                    hydraulicBoundaryLocation2
                },
                FilePath = "D:/nonExistingDirectory/nonExistingFile",
                Version = "random"
            };

            var assessmentSectionMock = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };
            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSectionMock);

            gui.Expect(cmp => cmp.Get(hydraulicBoundaryDatabaseContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);
                plugin.Gui = gui;

                var contextMenuAdapter = info.ContextMenuStrip(hydraulicBoundaryDatabaseContext, null, treeViewControlMock);

                // When
                Action action = () => { contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick(); };

                // Then
                string message = string.Format("Kon geen berekeningen starten. Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                    hydraulicBoundaryDatabase.FilePath);
                TestHelper.AssertLogMessageWithLevelIsGenerated(action, new Tuple<string, LogLevelConstant>(message, LogLevelConstant.Error));

                Assert.IsNaN(hydraulicBoundaryLocation1.DesignWaterLevel); // No result set
                Assert.AreEqual(4.2, hydraulicBoundaryLocation2.DesignWaterLevel); // Previous result not cleared
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasNoReferenceLine_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                Color color = info.ForeColor(hydraulicBoundaryDatabaseContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasReferenceLineData_ReturnControlText()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            mocks.ReplayAll();

            var hydraulicBoundaryDatabaseContext = new HydraulicBoundaryDatabaseContext(assessmentSection);

            using (var plugin = new RingtoetsGuiPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                Color color = info.ForeColor(hydraulicBoundaryDatabaseContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mocks.VerifyAll();
        }

        private TreeNodeInfo GetInfo(RingtoetsGuiPlugin guiPlugin)
        {
            return guiPlugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext));
        }
    }
}