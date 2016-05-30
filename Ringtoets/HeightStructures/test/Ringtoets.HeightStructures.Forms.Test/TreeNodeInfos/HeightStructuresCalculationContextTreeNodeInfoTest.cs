// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Geometry;
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Plugin;
using Ringtoets.HeightStructures.Plugin.Properties;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using HeightStructuresFormsResources = Ringtoets.HeightStructures.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.HeightStructures.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class HeightStructuresCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        private MockRepository mocks;
        private HeightStructuresGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new HeightStructuresGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(HeightStructuresCalculationContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(HeightStructuresCalculationContext), info.TagType);
            Assert.IsNotNull(info.Text);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.EnsureVisibleOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Image_Always_ReturnsCalculationIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(HeightStructuresFormsResources.CalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithoutOutput_ReturnCollectionWithEmptyOutputObject()
        {
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new HeightStructuresCalculation(failureMechanism.GeneralInput, failureMechanism.ProbabilityAssessmentInput);
            var calculationContext = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(calculationContext.WrappedData, commentContext.CommentContainer);

            var heightStructuresInputContext = children[1] as HeightStructuresInputContext;
            Assert.IsNotNull(heightStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, heightStructuresInputContext.WrappedData);

            var emptyOutput = children[2] as EmptyProbabilisticOutput;
            Assert.IsNotNull(emptyOutput);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_CalculationWithOutput_ReturnCollectionWithOutputObject()
        {
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var failureMechanism = new HeightStructuresFailureMechanism();
            var calculation = new HeightStructuresCalculation(failureMechanism.GeneralInput, failureMechanism.ProbabilityAssessmentInput)
            {
                Output = new TestHeightStructuresOutput()
            };

            var calculationContext = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(calculationContext.WrappedData, commentContext.CommentContainer);

            var heightStructuresInputContext = children[1] as HeightStructuresInputContext;
            Assert.IsNotNull(heightStructuresInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, heightStructuresInputContext.WrappedData);

            var output = children[2] as ProbabilisticOutput;
            Assert.IsNotNull(output);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new HeightStructuresCalculation(failureMechanism.GeneralInput, failureMechanism.ProbabilityAssessmentInput);
            var nodeData = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
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
            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilderMock);

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new HeightStructuresCalculation(failureMechanism.GeneralInput, failureMechanism.ProbabilityAssessmentInput);
            var nodeData = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            var menu = info.ContextMenuStrip(nodeData, assessmentSectionMock, treeViewControlMock);

            // Assert
            Assert.AreEqual(6, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateIndex,
                                                          RingtoetsCommonFormsResources.Calculate,
                                                          Resources.HeightStructuresGuiPlugin_AllDataAvailable_No_failure_mechanism_sections_imported,
                                                          RingtoetsCommonFormsResources.CalculateIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIndex,
                                                          RingtoetsCommonFormsResources.Clear_output,
                                                          RingtoetsCommonFormsResources.ClearOutput_No_output_to_clear,
                                                          RingtoetsCommonFormsResources.ClearIcon,
                                                          false);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_NoFailureMechanismSections_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                              new ProbabilityAssessmentInput());

            var nodeData = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var guiMock = mocks.StrictMock<IGui>();
            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                          RingtoetsCommonFormsResources.Calculate,
                                                          Resources.HeightStructuresGuiPlugin_AllDataAvailable_No_failure_mechanism_sections_imported,
                                                          RingtoetsCommonFormsResources.CalculateIcon,
                                                          false);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetNoHydraulicBoundaryDatabase_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Expect(asm => asm.HydraulicBoundaryDatabase).Return(null);

            var calculation = new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                              new ProbabilityAssessmentInput());

            var nodeData = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                          RingtoetsCommonFormsResources.Calculate,
                                                          Resources.HeightStructuresGuiPlugin_AllDataAvailable_No_hydraulic_boundary_database_imported,
                                                          RingtoetsCommonFormsResources.CalculateIcon,
                                                          false);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsSetHydraulicBoundaryDatabaseNotValid_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());

            var calculation = new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                              new ProbabilityAssessmentInput());

            var nodeData = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateIndex];

            Assert.AreEqual(RingtoetsCommonFormsResources.Calculate, contextMenuItem.Text);
            StringAssert.Contains(String.Format(RingtoetsCommonFormsResources.GuiPlugin_VerifyHydraulicBoundaryDatabasePath_Hydraulic_boundary_database_connection_failed_0_, ""), contextMenuItem.ToolTipText);
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateIcon, contextMenuItem.Image);
            Assert.IsFalse(contextMenuItem.Enabled);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismSectionsAndHydraulicBoundaryDatabaseSet_ContextMenuItemPerformCalculationEnabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var calculation = new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                              new ProbabilityAssessmentInput());

            var nodeData = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());

            mocks.ReplayAll();

            plugin.Gui = guiMock;

            // Call
            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControlMock);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                          RingtoetsCommonFormsResources.Calculate,
                                                          RingtoetsCommonFormsResources.Calculate_ToolTip,
                                                          RingtoetsCommonFormsResources.CalculateIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void GivenCalculationWithNonExistingFilePath_WhenCalculatingFromContextMenu_ThenOutputClearedLogMessagesAddedObserversNotNotified()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();
            var mainWindow = mocks.DynamicMock<IMainWindow>();
            var observerMock = mocks.StrictMock<IObserver>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculateContextMenuItemIndex = 0;

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });

            var failureMechanism = new HeightStructuresFailureMechanism();
            failureMechanism.AddSection(section);

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0.0, 1.1);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random"
            };
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                HydraulicBoundaryDatabase = hydraulicBoundaryDatabase
            };
            var calculation = new HeightStructuresCalculation(new GeneralHeightStructuresInput(),
                                                              new ProbabilityAssessmentInput())
            {
                Output = new ProbabilisticOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN),
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            };

            var calculationContext = new HeightStructuresCalculationContext(calculation, failureMechanism, assessmentSection);

            gui.Expect(g => g.Get(calculationContext, treeViewControlMock)).Return(new CustomItemsOnlyContextMenuBuilder());
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            calculation.Attach(observerMock);

            var contextMenuAdapter = info.ContextMenuStrip(calculationContext, null, treeViewControlMock);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            // When
            Action action = () => { contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick(); };

            // Then
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.GetEnumerator();
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Er is een fout opgetreden tijdens de berekening.", msgs.Current);
            });

            Assert.IsNull(calculation.Output);

            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromGroup()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new HeightStructuresFailureMechanism();
            var elementToBeRemoved = new HeightStructuresCalculation(failureMechanism.GeneralInput, failureMechanism.ProbabilityAssessmentInput);
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculationContext = new HeightStructuresCalculationContext(elementToBeRemoved,
                                                                            failureMechanism,
                                                                            assessmentSectionMock);
            var groupContext = new HeightStructuresCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionMock);

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new HeightStructuresCalculation(failureMechanism.GeneralInput, failureMechanism.ProbabilityAssessmentInput));
            group.Attach(observerMock);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);

            mocks.VerifyAll();
        }

        private const int contextMenuCalculateIndex = 0;
        private const int contextMenuClearIndex = 1;

        private class TestHeightStructuresOutput : ProbabilisticOutput
        {
            public TestHeightStructuresOutput() : base(0, 0, 0, 0, 0) {}
        }
    }
}