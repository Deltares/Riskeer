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
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Plugin;
using Ringtoets.HydraRing.Data;
using GrassCoverErosionInwardsFormsResources = Ringtoets.GrassCoverErosionInwards.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionInwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionInwardsCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuValidateIndex = 0;
        private const int contextMenuCalculateIndex = 1;
        private const int contextMenuClearIndex = 2;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");

        private MockRepository mocks;
        private GrassCoverErosionInwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionInwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionInwardsCalculationContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

            // Assert
            Assert.AreEqual(typeof(GrassCoverErosionInwardsCalculationContext), info.TagType);
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
            // Setup
            mocks.ReplayAll();

            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, image);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithoutOutput_ReturnCollectionWithEmptyOutputObject()
        {
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(calculationContext.WrappedData, commentContext.WrappedData);

            var grassCoverErosionInwardsInputContext = children[1] as GrassCoverErosionInwardsInputContext;
            Assert.IsNotNull(grassCoverErosionInwardsInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, grassCoverErosionInwardsInputContext.WrappedData);

            var emptyOutput = children[2] as EmptyProbabilityAssessmentOutput;
            Assert.IsNotNull(emptyOutput);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithOutput_ReturnCollectionWithOutputObject()
        {
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, true, new ProbabilityAssessmentOutput(0, 0, 0, 0, 0), 0.0)
            };
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            // Call
            var children = info.ChildNodeObjects(calculationContext).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);

            var commentContext = children[0] as CommentContext<ICommentable>;
            Assert.IsNotNull(commentContext);
            Assert.AreSame(calculationContext.WrappedData, commentContext.WrappedData);

            var grassCoverErosionInwardsInputContext = children[1] as GrassCoverErosionInwardsInputContext;
            Assert.IsNotNull(grassCoverErosionInwardsInputContext);
            Assert.AreSame(calculationContext.WrappedData.InputParameters, grassCoverErosionInwardsInputContext.WrappedData);

            var output = children[2] as GrassCoverErosionInwardsOutput;
            Assert.IsNotNull(output);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var menuBuilderMock = mocks.Stub<IContextMenuBuilder>();

            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.Build()).Return(null);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilderMock);

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                info.ContextMenuStrip(nodeData, null, treeViewControl);
            }
            // Assert
            // Assert expectancies called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);
                assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
                {
                    FilePath = validFilePath,
                    Version = "random"
                });

                mocks.ReplayAll();

                plugin.Gui = guiMock;
                failureMechanism.AddSection(new FailureMechanismSection("test", new[]
                {
                    new Point2D(0, 0)
                }));

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, assessmentSectionMock, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(10, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateIndex,
                                                                  RingtoetsCommonFormsResources.Validate,
                                                                  RingtoetsCommonFormsResources.Validate_ToolTip,
                                                                  RingtoetsCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateIndex,
                                                                  RingtoetsCommonFormsResources.Calculate,
                                                                  RingtoetsCommonFormsResources.Calculate_ToolTip,
                                                                  RingtoetsCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIndex,
                                                                  RingtoetsCommonFormsResources.Clear_output,
                                                                  RingtoetsCommonFormsResources.ClearOutput_No_output_to_clear,
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoSections_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();

            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var guiMock = mocks.StrictMock<IGui>();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  RingtoetsCommonFormsResources.Calculate,
                                                                  RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported,
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_SectionsSetNoDatabase_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  RingtoetsCommonFormsResources.Calculate,
                                                                  RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported,
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_SectionsSetDatabaseNotValid_ContextMenuItemPerformCalculationDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());

            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuCalculateIndex];

                    Assert.AreEqual(RingtoetsCommonFormsResources.Calculate, contextMenuItem.Text);
                    StringAssert.Contains(string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_, ""), contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculateIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_SectionsAndDatabaseSet_ContextMenuItemPerformCalculationEnabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex,
                                                                  RingtoetsCommonFormsResources.Calculate,
                                                                  RingtoetsCommonFormsResources.Calculate_ToolTip,
                                                                  RingtoetsCommonFormsResources.CalculateIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_NoSections_ContextMenuItemValidateCalculationDisabledAndTooltipSet()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();

            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);
            var guiMock = mocks.StrictMock<IGui>();

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  RingtoetsCommonFormsResources.Validate,
                                                                  RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported,
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_SectionsSetNoDatabase_ContextMenuItemValidateCalculationDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  RingtoetsCommonFormsResources.Validate,
                                                                  RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported,
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_SectionsSetDatabaseNotValid_ContextMenuItemValidateCalculationDisabledAndTooltipSet()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase());

            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    ToolStripItem contextMenuItem = contextMenu.Items[contextMenuValidateIndex];

                    Assert.AreEqual(RingtoetsCommonFormsResources.Validate, contextMenuItem.Text);
                    StringAssert.Contains(string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_, ""), contextMenuItem.ToolTipText);
                    TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.ValidateIcon, contextMenuItem.Image);
                    Assert.IsFalse(contextMenuItem.Enabled);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_SectionsAndDatabaseSet_ContextMenuItemValidateCalculationEnabled()
        {
            // Setup
            var guiMock = mocks.StrictMock<IGui>();

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "1.0"
            };

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0, 0)
            }));

            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            assessmentSectionMock.Stub(asm => asm.HydraulicBoundaryDatabase).Return(hydraulicBoundaryDatabase);

            var calculation = new GrassCoverErosionInwardsCalculation();
            var nodeData = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                guiMock.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = guiMock;

                // Call
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex,
                                                                  RingtoetsCommonFormsResources.Validate,
                                                                  RingtoetsCommonFormsResources.Validate_ToolTip,
                                                                  RingtoetsCommonFormsResources.ValidateIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithNonExistingLocationId_WhenCalculatingFromContextMenu_ThenOutputClearedLogMessagesAddedAndUpdateObserver()
        {
            // Given
            var gui = mocks.DynamicMock<IGui>();
            var mainWindow = mocks.DynamicMock<IMainWindow>();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(section);

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0.0, 1.1);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random"
            };
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            assessmentSectionMock.Stub(a => a.Id).Return(string.Empty);
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            assessmentSectionMock.Stub(a => a.FailureMechanismContribution).Return(new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 1, 1));

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                Output = new GrassCoverErosionInwardsOutput(0, true, new ProbabilityAssessmentOutput(double.NaN, double.NaN, double.NaN, double.NaN, double.NaN), double.NaN),
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties())
                }
            };

            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Expect(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Expect(g => g.MainWindow).Return(mainWindow);

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observerMock);

                DialogBoxHandler = (name, wnd) =>
                {
                    // Expect an activity dialog which is automatically closed
                };

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => { contextMenuStrip.Items[contextMenuCalculateIndex].PerformClick(); };

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        var msgs = messages.ToArray();
                        Assert.AreEqual(7, msgs.Length);
                        StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                        StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                        StringAssert.StartsWith(string.Format("Berekening van '{0}' gestart om: ", calculation.Name), msgs[2]);
                        StringAssert.StartsWith(string.Format("De berekening voor grasbekleding erosie kruin en binnentalud '{0}' is niet gelukt.", calculation.Name), msgs[3]);
                        StringAssert.StartsWith("Overloop berekeningsverslag. Klik op details voor meer informatie.", msgs[4]);
                        StringAssert.StartsWith(string.Format("Berekening van '{0}' beëindigd om: ", calculation.Name), msgs[5]);
                        StringAssert.StartsWith(string.Format("Uitvoeren van '{0}' is mislukt.", calculation.Name), msgs[6]);
                    });

                    Assert.IsNull(calculation.Output);
                }
            }
        }

        [Test]
        public void GivenCalculation_WhenValidatingFromContextMenu_ThenLogMessagesAdded()
        {
            // Given
            var gui = mocks.StrictMock<IGui>();
            var observerMock = mocks.StrictMock<IObserver>();

            var section = new FailureMechanismSection("A", new[]
            {
                new Point2D(1, 2),
                new Point2D(3, 4)
            });

            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(section);

            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, string.Empty, 0.0, 1.1);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random"
            };
            hydraulicBoundaryDatabase.Locations.Add(hydraulicBoundaryLocation);

            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            assessmentSectionMock.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

            var calculation = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = new DikeProfile(new Point2D(0, 0), new RoughnessPoint[0], new Point2D[0],
                                                  null, new DikeProfile.ConstructionProperties())
                }
            };

            var calculationContext = new GrassCoverErosionInwardsCalculationContext(calculation, failureMechanism, assessmentSectionMock);

            using (var treeViewControl = new TreeViewControl())
            {
                gui.Expect(g => g.Get(calculationContext, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());

                mocks.ReplayAll();

                plugin.Gui = gui;

                calculation.Attach(observerMock);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(calculationContext, null, treeViewControl))
                {
                    // When
                    Action action = () => contextMenuStrip.Items[contextMenuValidateIndex].PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(action, messages =>
                    {
                        var msgs = messages.ToArray();
                        StringAssert.StartsWith(string.Format("Validatie van '{0}' gestart om: ", calculation.Name), msgs[0]);
                        StringAssert.StartsWith(string.Format("Validatie van '{0}' beëindigd om: ", calculation.Name), msgs[1]);
                    });
                }
            }
        }

        [Test]
        public void OnNodeRemoved_ParentIsCalculationGroupContext_RemoveCalculationFromGroup()
        {
            // Setup
            var group = new CalculationGroup();
            var elementToBeRemoved = new GrassCoverErosionInwardsCalculation();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var observerMock = mocks.StrictMock<IObserver>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(elementToBeRemoved,
                                                                                    failureMechanism,
                                                                                    assessmentSectionMock);
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);

            observerMock.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new GrassCoverErosionInwardsCalculation());
            group.Attach(observerMock);

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
        }

        [Test]
        public void OnNodeRemoved_NodeRemoved_RemoveCalculationFromFailureMechanismSectionResults()
        {
            // Setup
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();
            mocks.ReplayAll();

            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            failureMechanism.AddSection(new FailureMechanismSection("test", new[]
            {
                new Point2D(0.0, 0.0),
                new Point2D(1.0, 1.0)
            }));
            var dikeProfile = new DikeProfile(new Point2D(0.5, 0.5), new RoughnessPoint[0], new Point2D[0],
                                              null, new DikeProfile.ConstructionProperties());
            failureMechanism.DikeProfiles.Add(dikeProfile);

            var elementToBeRemoved = new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    DikeProfile = dikeProfile
                }
            };

            var sectionResult = failureMechanism.SectionResults.First();
            sectionResult.Calculation = elementToBeRemoved;

            var calculationContext = new GrassCoverErosionInwardsCalculationContext(elementToBeRemoved,
                                                                                    failureMechanism,
                                                                                    assessmentSectionMock);
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                                   failureMechanism,
                                                                                   assessmentSectionMock);

            group.Children.Add(elementToBeRemoved);
            group.Children.Add(new GrassCoverErosionInwardsCalculation());

            // Precondition
            Assert.IsTrue(info.CanRemove(calculationContext, groupContext));
            Assert.AreEqual(2, group.Children.Count);
            Assert.AreEqual(elementToBeRemoved, sectionResult.Calculation);

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.AreEqual(1, group.Children.Count);
            CollectionAssert.DoesNotContain(group.Children, elementToBeRemoved);
            Assert.IsNull(sectionResult.Calculation);
        }

        [Test]
        public void OnNodeRemoved_CalculationInGroupAssignedToSection_CalculationDetachedFromSection()
        {
            // Setup
            var group = new CalculationGroup();
            var failureMechanism = new GrassCoverErosionInwardsFailureMechanism();
            var elementToBeRemoved = new GrassCoverErosionInwardsCalculation();
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var calculationContext = new GrassCoverErosionInwardsCalculationContext(elementToBeRemoved,
                                                                            failureMechanism,
                                                                            assessmentSectionStub);
            var groupContext = new GrassCoverErosionInwardsCalculationGroupContext(group,
                                                                           failureMechanism,
                                                                           assessmentSectionStub);

            mocks.ReplayAll();

            group.Children.Add(elementToBeRemoved);

            failureMechanism.AddSection(new FailureMechanismSection("section", new[]
            {
                new Point2D(0, 0)
            }));

            failureMechanism.SectionResults.First().Calculation = elementToBeRemoved;

            // Call
            info.OnNodeRemoved(calculationContext, groupContext);

            // Assert
            Assert.IsNull(failureMechanism.SectionResults.First().Calculation);
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();

            base.TearDown();
        }
    }
}