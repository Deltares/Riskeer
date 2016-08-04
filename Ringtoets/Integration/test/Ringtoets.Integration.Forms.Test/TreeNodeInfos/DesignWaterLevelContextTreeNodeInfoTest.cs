using System;
using System.Drawing;
using System.Linq;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
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
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class DesignWaterLevelContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;

        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "DesignWaterLevel");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Assert
                Assert.AreEqual(typeof(DesignWaterLevelContext), info.TagType);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.IsChecked);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void Text_Always_ReturnsSetName()
        {
            // Setup
            var name = "Toetspeil";
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            mocks.ReplayAll();

            var context = new DesignWaterLevelContext(assessmentSectionMock);

            using (var plugin = new RingtoetsPlugin())
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
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var image = info.Image(null);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, image);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_Always_ReturnsFalse()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                var renameAllowed = info.CanRename(null, null);

                // Assert
                Assert.IsFalse(renameAllowed);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();

            var nodeData = new DesignWaterLevelContext(assessmentSectionMock);

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    info.ContextMenuStrip(nodeData, null, treeViewControl);
                }
            }
            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoHydraulicBoundaryDatabaseSet_ContextMenuItemBerekenenDisabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();

            var nodeData = new DesignWaterLevelContext(assessmentSectionMock);

            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

                    const string expectedItemText = "&Berekenen";
                    const string expectedItemTooltip = "Er is geen hydraulische randvoorwaardendatabase beschikbaar om de toetspeilen te berekenen.";
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, expectedItemText, expectedItemTooltip, RingtoetsCommonFormsResources.FailureMechanismIcon, false);
                }
            }
            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_HydraulicBoundaryDatabaseSet_ContextMenuItemBerekenenEnabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();

            var nodeData = new DesignWaterLevelContext(assessmentSectionMock);
            nodeData.WrappedData.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            guiMock.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);

                    plugin.Gui = guiMock;

                    // Call
                    nodeData.WrappedData.HydraulicBoundaryDatabase.FilePath = testDataPath;
                    var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

                    const string expectedItemText = "&Berekenen";
                    const string expectedItemTooltip = "Bereken de toetspeilen";
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, 0, expectedItemText, expectedItemTooltip, RingtoetsCommonFormsResources.FailureMechanismIcon);
                }
            }
            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GivenHydraulicBoundaryDatabaseWithNonExistingFilePath_WhenCalculatingAssessmentLevelFromContextMenu_ThenLogMessagesAddedPreviousOutputNotAffected()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();

            var contextMenuRunAssessmentLevelCalculationsIndex = 0;

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
            var designWaterLevelContext = new DesignWaterLevelContext(assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Expect(cmp => cmp.Get(designWaterLevelContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    var info = GetInfo(plugin);
                    plugin.Gui = gui;

                    var contextMenuAdapter = info.ContextMenuStrip(designWaterLevelContext, null, treeViewControl);

                    // When
                    Action action = () => { contextMenuAdapter.Items[contextMenuRunAssessmentLevelCalculationsIndex].PerformClick(); };

                    // Then
                    string message = string.Format("Berekeningen konden niet worden gestart. Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.",
                                                   hydraulicBoundaryDatabase.FilePath);
                    TestHelper.AssertLogMessageWithLevelIsGenerated(action, new Tuple<string, LogLevelConstant>(message, LogLevelConstant.Error));

                    Assert.IsNaN(hydraulicBoundaryLocation1.DesignWaterLevel); // No result set
                    Assert.AreEqual(4.2, hydraulicBoundaryLocation2.DesignWaterLevel); // Previous result not cleared
                }
            }
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_ContextHasNoReferenceLine_ReturnDisabledColor()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var designWaterLevelContext = new DesignWaterLevelContext(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);
                // Call
                Color color = info.ForeColor(designWaterLevelContext);

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

            var designWaterLevelContext = new DesignWaterLevelContext(assessmentSection);

            using (var plugin = new RingtoetsPlugin())
            {
                var info = GetInfo(plugin);

                // Call
                Color color = info.ForeColor(designWaterLevelContext);

                // Assert
                Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            }
            mocks.VerifyAll();
        }
        
        private TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(DesignWaterLevelContext));
        }
    }
}