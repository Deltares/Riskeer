// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Service.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Riskeer.HydraRing.Calculation.Calculator.Factory;
using Riskeer.HydraRing.Calculation.Data.Input;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.WaveImpactAsphaltCover.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class WaveImpactAsphaltCoverWaveConditionsCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuDuplicateIndex = 2;
        private const int contextMenuUpdateForeshoreProfileIndex = 5;
        private const int validateMenuItemIndex = 7;
        private const int calculateMenuItemIndex = 8;
        private const int clearOutputMenuItemIndex = 10;

        private static readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, nameof(HydraulicBoundaryDatabase));
        private static readonly string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

        private MockRepository mocks;
        private WaveImpactAsphaltCoverPlugin plugin;
        private TreeNodeInfo info;

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            mocks.ReplayAll();

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            const string name = "cool name";
            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = name
            };
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
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
            // Setup
            mocks.ReplayAll();

            // Call
            Image icon = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.CalculationIcon, icon);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool shouldBeVisible = info.EnsureVisibleOnCreate(null, null);

            // Assert
            Assert.IsTrue(shouldBeVisible);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithoutOutput_ReturnChildrenWithEmptyOutput()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            });

            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = null
            };

            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 1));
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(3, children.Length);

            var comments = (Comment) children[0];
            Assert.AreSame(calculation.Comments, comments);

            var inputContext = (WaveImpactAsphaltCoverWaveConditionsInputContext) children[1];
            Assert.AreSame(calculation.InputParameters, inputContext.WrappedData);
            Assert.AreSame(calculation, inputContext.Calculation);
            Assert.AreSame(assessmentSection, inputContext.AssessmentSection);
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, inputContext.ForeshoreProfiles);
            CollectionAssert.AreEqual(new[]
            {
                location
            }, inputContext.HydraulicBoundaryLocations);

            Assert.IsInstanceOf<EmptyWaveImpactAsphaltCoverOutput>(children[2]);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithOutput_ReturnChildrenWithOutput()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            });

            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 1));
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(3, children.Length);

            var comments = (Comment) children[0];
            Assert.AreSame(calculation.Comments, comments);

            var inputContext = (WaveImpactAsphaltCoverWaveConditionsInputContext) children[1];
            Assert.AreSame(calculation.InputParameters, inputContext.WrappedData);
            Assert.AreSame(calculation, inputContext.Calculation);
            Assert.AreSame(assessmentSection, inputContext.AssessmentSection);
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, inputContext.ForeshoreProfiles);
            CollectionAssert.AreEqual(new[]
            {
                location
            }, inputContext.HydraulicBoundaryLocations);

            var output = (WaveImpactAsphaltCoverWaveConditionsOutput) children[2];
            Assert.AreSame(calculation.Output, output);
        }

        [Test]
        public void CanRename_Always_ReturnTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool canRename = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(canRename);
        }

        [Test]
        public void OnNodeRenamed_ChangeNameOfCalculationAndNotifyObservers()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            calculation.Attach(observer);

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);
            const string name = "the new name!";

            // Call
            info.OnNodeRenamed(context, name);

            // Assert
            Assert.AreEqual(name, calculation.Name);
        }

        [Test]
        public void CanRemove_CalculationInParent_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            var parentContext = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            var parentContext = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Attach(observer);
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            var parentContext = new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                null,
                                                                                                failureMechanism,
                                                                                                assessmentSection);

            // Call
            info.OnNodeRemoved(context, parentContext);

            // Assert
            CollectionAssert.DoesNotContain(failureMechanism.WaveConditionsCalculationGroup.Children, calculation);
        }

        [Test]
        public void CanDrag_Always_ReturnTrue()
        {
            // Setup
            mocks.ReplayAll();

            // Call
            bool canDrag = info.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            var orderedMocks = new MockRepository();
            var menuBuilder = orderedMocks.StrictMock<IContextMenuBuilder>();
            using (orderedMocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddDeleteItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();
                orderedMocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }

            // Assert
            orderedMocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random"
            });

            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                using (ContextMenuStrip menu = info.ContextMenuStrip(context, assessmentSection, treeViewControl))
                {
                    // Assert
                    Assert.AreEqual(17, menu.Items.Count);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuDuplicateIndex,
                                                                  "D&upliceren",
                                                                  "Dupliceer dit element.",
                                                                  RingtoetsCommonFormsResources.CopyHS);

                    TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuUpdateForeshoreProfileIndex,
                                                                  "&Bijwerken voorlandprofiel...",
                                                                  "Er moet een voorlandprofiel geselecteerd zijn.",
                                                                  RingtoetsCommonFormsResources.UpdateItemIcon,
                                                                  false);

                    TestHelper.AssertContextMenuStripContainsItem(menu, validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon);

                    TestHelper.AssertContextMenuStripContainsItem(menu, clearOutputMenuItemIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithHydraulicBoundaryDatabaseNotLinked_ThenValidationItemDisabled()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithHydraulicBoundaryDatabaseLinkedToInvalidFile_ThenValidationItemDisabled()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand 'invalidFilePath': het bestand bestaat niet.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenValidInput_ThenValidationItemEnabled()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            });

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithoutForeshoreProfile_ContextMenuItemUpdateForeshoreProfileDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation();

            var nodeData = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                      parent,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
                        RingtoetsCommonFormsResources.UpdateItemIcon,
                        false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithForeshoreProfileAndInputInSync_ContextMenuItemUpdateForeshoreProfileDisabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = new TestForeshoreProfile()
                }
            };

            var nodeData = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                      parent,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
                        RingtoetsCommonFormsResources.UpdateItemIcon,
                        false);
                }
            }
        }

        [Test]
        public void ContextMenuStrip_CalculationWithForeshoreProfileAndInputOutSync_ContextMenuItemUpdateForeshoreProfileEnabledAndToolTipSet()
        {
            // Setup
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile();
            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

            var nodeData = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                      parent,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
                        RingtoetsCommonFormsResources.UpdateItemIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutputAndWithInputOutOfSync_WhenUpdateForeshoreProfileClicked_ThenNoInquiryAndCalculationUpdatedAndInputObserverNotified()
        {
            // Given
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            calculationInputObserver.Expect(o => o.UpdateObserver());

            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            var nodeData = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                      parent,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculationWithOutputAndWithInputOutOfSync_WhenPerformClick_ThenInquiryAndExpectedOutputAndNotifications(bool continuation)
        {
            // Given
            var calculationObserver = mocks.StrictMock<IObserver>();
            var calculationInputObserver = mocks.StrictMock<IObserver>();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                },
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            var nodeData = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                      parent,
                                                                                      failureMechanism,
                                                                                      assessmentSection);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            if (continuation)
            {
                calculationObserver.Expect(o => o.UpdateObserver());
                calculationInputObserver.Expect(o => o.UpdateObserver());
            }

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
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculation_WhenValidating_ThenCalculationValidated(bool validCalculation)
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            var random = new Random(21);
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = validCalculation
                                                                              ? GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First())
                                                                              : new WaveImpactAsphaltCoverWaveConditionsCalculation
                                                                              {
                                                                                  Name = "A",
                                                                                  InputParameters =
                                                                                  {
                                                                                      CategoryType = random.NextEnumValue<AssessmentSectionCategoryType>()
                                                                                  }
                                                                              };

            var parent = new CalculationGroup();
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[validateMenuItemIndex];
                    Action call = () => validateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, logMessages =>
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
        public void GivenHydraulicBoundaryDatabaseWithCanUsePreprocessorFalse_WhenValidatingCalculation_ThenNoValidationErrorsLogged()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            var parent = new CalculationGroup();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[validateMenuItemIndex];
                    Action call = () => validateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        Assert.AreEqual(2, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[1]);
                    });
                }
            }
        }

        [Test]
        public void GivenHydraulicBoundaryDatabaseWithUsePreprocessorFalse_WhenValidatingCalculation_ThenNoValidationErrorsLogged()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = false;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            var parent = new CalculationGroup();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[validateMenuItemIndex];
                    Action call = () => validateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        Assert.AreEqual(2, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[1]);
                    });
                }
            }
        }

        [Test]
        public void GivenHydraulicBoundaryDatabaseWithUsePreprocessorTrue_WhenValidatingCalculation_ThenNoValidationErrorsLogged()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = TestHelper.GetScratchPadPath();

            var parent = new CalculationGroup();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[validateMenuItemIndex];
                    Action call = () => validateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        Assert.AreEqual(2, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[1]);
                    });
                }
            }
        }

        [Test]
        public void GivenHydraulicBoundaryDatabaseWithUsePreprocessorTrue_WhenValidatingCalculation_ThenValidationErrorsLogged()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism
            {
                Contribution = 5
            };

            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            assessmentSection.HydraulicBoundaryDatabase.CanUsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.UsePreprocessor = true;
            assessmentSection.HydraulicBoundaryDatabase.PreprocessorDirectory = "InvalidPreprocessorDirectory";

            var parent = new CalculationGroup();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[validateMenuItemIndex];
                    Action call = () => validateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        Assert.AreEqual(3, messages.Length);
                        CalculationServiceTestHelper.AssertValidationStartMessage(messages[0]);
                        Assert.AreEqual("De bestandsmap waar de preprocessor bestanden opslaat is ongeldig. De bestandsmap bestaat niet.", messages[1]);
                        CalculationServiceTestHelper.AssertValidationEndMessage(messages[2]);
                    });
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithHydraulicBoundaryDatabaseNotLinked_ThenCalculationItemDisabled()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Er is geen hydraulische belastingendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithHydraulicBoundaryDatabaseLinkedToInvalidFile_ThenCalculationItemDisabled()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(null, mocks, "invalidFilePath");

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Herstellen van de verbinding met de hydraulische belastingendatabase is mislukt. Fout bij het lezen van bestand 'invalidFilePath': het bestand bestaat niet.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenValidInput_ThenCalculationItemEnabled()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.Stub(a => a.HydraulicBoundaryDatabase).Return(new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath
            });

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon);
                }
            }
        }

        [Test]
        public void GivenValidCalculation_WhenCalculating_ThenCalculationReturnsResult()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = CreateAssessmentSectionWithHydraulicBoundaryOutput();

            var parent = new CalculationGroup();
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = GetValidCalculation(assessmentSection.HydraulicBoundaryDatabase.Locations.First());
            calculation.Name = "A";
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            DialogBoxHandler = (name, wnd) =>
            {
                // Expect an activity dialog which is automatically closed
            };

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var mainWindow = mocks.Stub<IMainWindow>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);
                var observer = mocks.StrictMock<IObserver>();
                observer.Expect(o => o.UpdateObserver());
                calculation.Attach(observer);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                RoundedDouble assessmentLevel = assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                     calculation.InputParameters.CategoryType);
                IEnumerable<RoundedDouble> waterLevels = calculation.InputParameters.GetWaterLevels(assessmentLevel);
                int nrOfCalculators = waterLevels.Count();
                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     HydraRingCalculationSettingsTestHelper.AssertHydraRingCalculationSettings(
                                         HydraulicBoundaryCalculationSettingsFactory.CreateSettings(assessmentSection.HydraulicBoundaryDatabase),
                                         (HydraRingCalculationSettings) invocation.Arguments[0]);
                                 })
                                 .Return(new TestWaveConditionsCosineCalculator())
                                 .Repeat
                                 .Times(nrOfCalculators);
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // When
                    ToolStripItem calculateMenuItem = contextMenu.Items[calculateMenuItemIndex];
                    Action call = () => calculateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        Assert.AreEqual(15, messages.Length);
                        Assert.AreEqual("Golfcondities berekenen voor 'A' is gestart.", messages[0]);
                        CalculationServiceTestHelper.AssertCalculationStartMessage(messages[3]);
                        CalculationServiceTestHelper.AssertCalculationEndMessage(messages[13]);
                        Assert.AreEqual("Golfcondities berekenen voor 'A' is gelukt.", messages[14]);
                    });
                    Assert.AreEqual(3, calculation.Output.Items.Count());
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutput_ThenClearOutputItemDisabled()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = null
            };
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  clearOutputMenuItemIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Deze berekening heeft geen uitvoer om te wissen.",
                                                                  RingtoetsCommonFormsResources.ClearIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutput_ThenClearOutputItemEnabled()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  clearOutputMenuItemIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Wis de uitvoer van deze berekening.",
                                                                  RingtoetsCommonFormsResources.ClearIcon);
                }
            }
        }

        [Test]
        public void GivenCalculationWithOutput_WhenClearingOutput_ThenClearOutput()
        {
            // Given
            var failureMechanism = new WaveImpactAsphaltCoverFailureMechanism();
            IAssessmentSection assessmentSection = AssessmentSectionTestHelper.CreateAssessmentSectionStub(mocks);

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var parent = new CalculationGroup();
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = new WaveImpactAsphaltCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            calculation.Attach(observer);
            var context = new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                     parent,
                                                                                     failureMechanism,
                                                                                     assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilder = new ContextMenuBuilder(appFeatureCommandHandler,
                                                         importHandler,
                                                         exportHandler,
                                                         updateHandler,
                                                         viewCommands,
                                                         context,
                                                         treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilder);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

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
                    // Check expectancies in TearDown()
                }
            }
        }

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new WaveImpactAsphaltCoverPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(WaveImpactAsphaltCoverWaveConditionsCalculationContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
            base.TearDown();
        }

        private IAssessmentSection CreateAssessmentSectionWithHydraulicBoundaryOutput()
        {
            var hydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, string.Empty, 0, 0);

            var assessmentSection = new AssessmentSectionStub
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = validFilePath,
                    Locations =
                    {
                        hydraulicBoundaryLocation
                    }
                }
            };
            HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
            
            assessmentSection.SetHydraulicBoundaryLocationCalculations(new[]
            {
                hydraulicBoundaryLocation
            });

            assessmentSection.WaterLevelCalculationsForLowerLimitNorm.First().Output = new TestHydraulicBoundaryLocationCalculationOutput(9.3);

            return assessmentSection;
        }

        private static WaveImpactAsphaltCoverWaveConditionsCalculation GetValidCalculation(HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            return new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = AssessmentSectionCategoryType.LowerLimitNorm,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
        }
    }
}