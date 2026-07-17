// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.Main;
using Core.Gui.TestUtil;
using Core.Gui.TestUtil.ContextMenu;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using NSubstitute;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Service.TestUtil;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using Riskeer.HydraRing.Calculation.TestUtil.Calculator;
using Riskeer.Revetment.Data;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Data.TestUtil;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuDuplicateIndex = 2;
        private const int contextMenuUpdateForeshoreProfileIndex = 5;
        private const int validateMenuItemIndex = 7;
        private const int calculateMenuItemIndex = 8;
        private const int clearOutputMenuItemIndex = 10;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryData));
        private static readonly string validHlcdFilePath = Path.Combine(testDataPath, "hlcd.sqlite");
        private static readonly string validHrdFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
        private static readonly string validHrdFileVersion = "Dutch coast South19-11-2015 12:0013";

        private StabilityStoneCoverPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.IsNotNull(info.Text);
            Assert.IsNull(info.ForeColor);
            Assert.IsNotNull(info.Image);
            Assert.IsNotNull(info.ContextMenuStrip);
            Assert.IsNotNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.ExpandOnCreate);
            Assert.IsNotNull(info.ChildNodeObjects);
            Assert.IsNotNull(info.CanRename);
            Assert.IsNotNull(info.OnNodeRenamed);
            Assert.IsNotNull(info.CanRemove);
            Assert.IsNotNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.CheckedState);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNotNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnCalculationName()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            const string name = "cool name";
            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = name
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            // Call
            string text = info.Text(context);

            // Assert
            Assert.AreEqual(name, text);
        }

        [Test]
        public void Image_Always_ReturnCalculationIcon()
        {
            // Call
            Image icon = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RiskeerCommonFormsResources.HydraulicCalculationIcon, icon);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnTrue()
        {
            // Call
            bool shouldBeVisible = info.EnsureVisibleOnCreate(null, null);

            // Assert
            Assert.IsTrue(shouldBeVisible);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithoutOutput_ReturnChildrenWithEmptyOutput()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryData.Returns(new HydraulicBoundaryData
            {
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        Locations =
                        {
                            new TestHydraulicBoundaryLocation()
                        }
                    }
                }
            });

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = null
            };
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 1));

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(3, children.Length);

            var comments = (Comment) children[0];
            Assert.AreSame(calculation.Comments, comments);

            var inputContext = (StabilityStoneCoverWaveConditionsInputContext) children[1];
            Assert.AreSame(calculation.InputParameters, inputContext.WrappedData);
            Assert.AreSame(calculation, inputContext.Calculation);
            Assert.AreSame(assessmentSection, inputContext.AssessmentSection);
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, inputContext.ForeshoreProfiles);
            CollectionAssert.AreEqual(assessmentSection.HydraulicBoundaryData.GetLocations(), inputContext.HydraulicBoundaryLocations);

            Assert.IsInstanceOf<EmptyStabilityStoneCoverOutput>(children[2]);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithOutput_ReturnChildrenWithOutput()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryData.Returns(new HydraulicBoundaryData
            {
                HydraulicBoundaryDatabases =
                {
                    new HydraulicBoundaryDatabase
                    {
                        Locations =
                        {
                            new TestHydraulicBoundaryLocation()
                        }
                    }
                }
            });

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 1));

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = (Comment) children[0];
            Assert.AreSame(calculation.Comments, comment);

            var inputContext = (StabilityStoneCoverWaveConditionsInputContext) children[1];
            Assert.AreSame(calculation.InputParameters, inputContext.WrappedData);
            Assert.AreSame(calculation, inputContext.Calculation);
            Assert.AreSame(assessmentSection, inputContext.AssessmentSection);
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, inputContext.ForeshoreProfiles);
            CollectionAssert.AreEqual(assessmentSection.HydraulicBoundaryData.GetLocations(), inputContext.HydraulicBoundaryLocations);

            var outputContext = (StabilityStoneCoverWaveConditionsOutputContext) children[2];
            Assert.AreSame(calculation.Output, outputContext.WrappedData);
            Assert.AreSame(calculation.InputParameters, outputContext.Input);
        }

        [Test]
        public void CanRename_Always_ReturnTrue()
        {
            // Call
            bool canRename = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(canRename);
        }

        [Test]
        public void OnNodeRenamed_ChangeNameOfCalculationAndNotifyObservers()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var observer = Substitute.For<IObserver>();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            calculation.Attach(observer);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);
            const string name = "the new name!";

            // Call
            info.OnNodeRenamed(context, name);

            // Assert
            Assert.AreEqual(name, calculation.Name);
            observer.Received().UpdateObserver();
        }

        [Test]
        public void CanRemove_CalculationInParent_ReturnTrue()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new StabilityStoneCoverCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            bool canRemoveCalculation = info.CanRemove(context, parentContext);

            // Assert
            Assert.IsTrue(canRemoveCalculation);
        }

        [Test]
        public void CanRemove_CalculationNotInParent_ReturnFalse()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new StabilityStoneCoverCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            bool canRemoveCalculation = info.CanRemove(context, parentContext);

            // Assert
            Assert.IsFalse(canRemoveCalculation);
        }

        [Test]
        public void OnNodeRemoved_CalculationInParent_CalculationRemovedFromParent()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var observer = Substitute.For<IObserver>();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(calculation);
            failureMechanism.CalculationsGroup.Attach(observer);
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new StabilityStoneCoverCalculationGroupContext(failureMechanism.CalculationsGroup,
                                                                               null,
                                                                               failureMechanism,
                                                                               assessmentSection);

            // Call
            info.OnNodeRemoved(context, parentContext);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.CalculationsGroup.Children, calculation);
            observer.Received().UpdateObserver();
        }

        [Test]
        public void CanDrag_Always_ReturnTrue()
        {
            // Call
            bool canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            menuBuilder.AddExportItem().Returns(menuBuilder);
            menuBuilder.AddSeparator().Returns(menuBuilder);
            menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>()).Returns(menuBuilder);
            menuBuilder.AddRenameItem().Returns(menuBuilder);
            menuBuilder.AddDeleteItem().Returns(menuBuilder);
            menuBuilder.AddCollapseAllItem().Returns(menuBuilder);
            menuBuilder.AddExpandAllItem().Returns(menuBuilder);
            menuBuilder.AddPropertiesItem().Returns(menuBuilder);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            Received.InOrder(() =>
            {
                menuBuilder.AddExportItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddRenameItem();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddSeparator();
                menuBuilder.AddCustomItem(Arg.Any<StrictContextMenuItem>());
                menuBuilder.AddDeleteItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddCollapseAllItem();
                menuBuilder.AddExpandAllItem();
                menuBuilder.AddSeparator();
                menuBuilder.AddPropertiesItem();
                menuBuilder.Build();
            });
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            var assessmentSection = Substitute.For<IAssessmentSection>();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(17, menu.Items.Count);

                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuDuplicateIndex,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RiskeerCommonFormsResources.CopyHS);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndex,
                                                                  "&Bijwerken voorlandprofiel...",
                                                                  "Er moet een voorlandprofiel geselecteerd zijn.",
                                                                  RiskeerCommonFormsResources.UpdateItemIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RiskeerCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RiskeerCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, clearOutputMenuItemIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenValidInput_ThenValidationItemEnabled()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var assessmentSection = Substitute.For<IAssessmentSection>();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RiskeerCommonFormsResources.ValidateIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutForeshoreProfile_ContextMenuItemUpdateForeshoreProfileDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation();

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                   parent,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(
                        menu,
                        contextMenuUpdateForeshoreProfileIndex,
                        "&Bijwerken voorlandprofiel...",
                        "Er moet een voorlandprofiel geselecteerd zijn.",
                        RiskeerCommonFormsResources.UpdateItemIcon,
                        false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithForeshoreProfileAndInputInSync_ContextMenuItemUpdateForeshoreProfileDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                   parent,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(
                        menu,
                        contextMenuUpdateForeshoreProfileIndex,
                        "&Bijwerken voorlandprofiel...",
                        "Er zijn geen wijzigingen om bij te werken.",
                        RiskeerCommonFormsResources.UpdateItemIcon,
                        false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithForeshoreProfileAndInputOutSync_ContextMenuItemUpdateForeshoreProfileEnabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile();
            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                   parent,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // Assert
                    TestHelper.AssertContextMenuStripContainsItem(
                        menu,
                        contextMenuUpdateForeshoreProfileIndex,
                        "&Bijwerken voorlandprofiel...",
                        "Berekening bijwerken met het voorlandprofiel.",
                        RiskeerCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateForeshoreProfileClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var calculationObserver = Substitute.For<IObserver>();
            var calculationInputObserver = Substitute.For<IObserver>();

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            var nodeData = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                   parent,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

                // Precondition
                Assert.IsFalse(calculation.InputParameters.IsForeshoreProfileInputSynchronized);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateForeshoreProfileIndex].PerformClick();

                    // Then
                    Assert.IsTrue(calculation.InputParameters.IsForeshoreProfileInputSynchronized);
                }
            }
            calculationInputObserver.Received().UpdateObserver();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationWithOutputAndWithInputOutOfSync_WhenPerformClick_ThenInquiryAndExpectedOutputAndNotifications(bool continuation)
        {
            // Given
            var calculationObserver = Substitute.For<IObserver>();
            var calculationInputObserver = Substitute.For<IObserver>();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                },
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };

            var nodeData = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                   parent,
                                                                                   failureMechanism,
                                                                                   assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            var messageBoxText = "";
            DialogBoxHandler = (name, wnd) =>
            {
                var helper = new MessageBoxTester(wnd);
                messageBoxText = helper.Text;

                if (continuation)
                {
                    helper.ClickOk();
                }
                else
                {
                    helper.ClickCancel();
                }
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = Substitute.For<IGui>();
                gui.Get(nodeData, treeViewControl).Returns(new CustomItemsOnlyContextMenuBuilder());
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

                // Precondition
                Assert.IsFalse(calculation.InputParameters.IsForeshoreProfileInputSynchronized);

                using (ContextMenuStrip contextMenuStrip = info.ContextMenuStrip(nodeData, null, treeViewControl))
                {
                    // When
                    contextMenuStrip.Items[contextMenuUpdateForeshoreProfileIndex].PerformClick();

                    // Then
                    Assert.AreEqual(continuation, calculation.InputParameters.IsForeshoreProfileInputSynchronized);
                    Assert.AreEqual(!continuation, calculation.HasOutput);
                }
            }

            string expectedMessageBoxText = "Als u kiest voor bijwerken, dan wordt het resultaat van deze berekening " +
                                            $"verwijderd.{Environment.NewLine}{Environment.NewLine}Weet u zeker dat u wilt doorgaan?";

            Assert.AreEqual(expectedMessageBoxText, messageBoxText);

            if (continuation)
            {
                calculationObserver.Received().UpdateObserver();
                calculationInputObserver.Received().UpdateObserver();
            }
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GivenCalculation_WhenValidating_ThenCalculationValidated(bool validCalculation)
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            StabilityStoneCoverWaveConditionsCalculation calculation = validCalculation
                                                                           ? GetValidCalculation(assessmentSection.HydraulicBoundaryData.GetLocations().First())
                                                                           : new StabilityStoneCoverWaveConditionsCalculation
                                                                           {
                                                                               Name = "A"
                                                                           };

            var parent = new CalculationGroup();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[validateMenuItemIndex];
                    void Call() => validateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(Call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        int expectedMessageCount = validCalculation ? 2 : 3;
                        Assert.AreEqual(expectedMessageCount, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);

                        if (!validCalculation)
                        {
                            Assert.AreEqual("Er is geen hydraulische belastingenlocatie geselecteerd.", messages[1]);
                        }

                        CalculationServiceTestHelper.AssertValidationEndMessage(messages.Last());
                    });
                }
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenValidInput_ThenCalculationItemEnabled(bool usePreprocessorClosure)
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            var assessmentSection = Substitute.For<IAssessmentSection>();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RiskeerCommonFormsResources.CalculateIcon);
                }
            }
        }

        [Test]
        public void GivenValidCalculation_WhenCalculating_ThenCalculationReturnsResult()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            HydraulicBoundaryLocation hydraulicBoundaryLocation = assessmentSection.HydraulicBoundaryData.GetLocations().First();

            var parent = new CalculationGroup();
            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation(hydraulicBoundaryLocation);
            calculation.Name = "A";
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                IMainWindow mainWindow = MainWindowTestHelper.CreateMainWindowStub();

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(mainWindow);

                var calculatorFactory = Substitute.For<IHydraRingCalculatorFactory>();
                calculatorFactory.CreateWaveConditionsCosineCalculator(Arg.Any<HydraRingCalculationSettings>())
                                 .Returns(callInfo =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(
                                             assessmentSection.HydraulicBoundaryData,
                                             hydraulicBoundaryLocation),
                                         (HydraRingCalculationSettings) callInfo[0]);
                                     return new TestWaveConditionsCosineCalculator();
                                 });

                plugin.Gui = gui;

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem calculateMenuItem = contextMenu.Items[calculateMenuItemIndex];
                    void Call() => calculateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(Call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        Assert.AreEqual(28, messages.Length);
                        Assert.AreEqual("Golfcondities berekenen voor 'A' is gestart.", messages[0]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messages[26]);
                        Assert.AreEqual("Golfcondities berekenen voor 'A' is gelukt.", messages[27]);
                    });
                    Assert.AreEqual(3, calculation.Output.BlocksOutput.Count());
                    Assert.AreEqual(3, calculation.Output.ColumnsOutput.Count());
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutput_ThenClearOutputItemDisabled()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = null
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  clearOutputMenuItemIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RiskeerCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutput_ThenClearOutputItemEnabled()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  clearOutputMenuItemIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Wis de uitvoer van deze berekening.",
                                                                  RiskeerCommonFormsResources.ClearIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutput_WhenClearingOutput_ThenClearOutput()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub();

            var observer = Substitute.For<IObserver>();

            var parent = new CalculationGroup();
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = StabilityStoneCoverWaveConditionsOutputTestFactory.Create()
            };
            calculation.Attach(observer);
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  parent,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = Substitute.For<IApplicationFeatureCommands>();
                var importHandler = Substitute.For<IImportCommandHandler>();
                var exportHandler = Substitute.For<IExportCommandHandler>();
                var updateHandler = Substitute.For<IUpdateCommandHandler>();
                var viewCommands = Substitute.For<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = Substitute.For<IGui>();
                gui.Get(context, treeViewControl).Returns(menuBuilder);
                gui.MainWindow.Returns(Substitute.For<IMainWindow>());

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickOk();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[clearOutputMenuItemIndex];
                    validateMenuItem.PerformClick();

                    // Then
                    Assert.IsNull(calculation.Output);
                    observer.Received().UpdateObserver();
                }
            }
        }

        public override void Setup()
        {
            plugin = new StabilityStoneCoverPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            base.TearDown();
        }

        private static IAssessmentSection CreateAssessmentSectionWithHydraulicBoundaryOutput(bool usePreprocessorClosure = false)
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryData =
                {
                    HydraulicLocationConfigurationDatabase =
                    {
                        FilePath = validHlcdFilePath
                    },
                    HydraulicBoundaryDatabases =
                    {
                        new HydraulicBoundaryDatabase
                        {
                            FilePath = validHrdFilePath,
                            Version = validHrdFileVersion,
                            UsePreprocessorClosure = usePreprocessorClosure,
                            Locations =
                            {
                                hydraulicBoundaryLocation
                            }
                        }
                    }
                }
            };

            assessmentSection.AddHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForMaximumAllowableFloodingProbability.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);

            return assessmentSection;
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    WaterLevelType = WaveConditionsInputWaterLevelType.MaximumAllowableFloodingProbability,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
        }
    }
}