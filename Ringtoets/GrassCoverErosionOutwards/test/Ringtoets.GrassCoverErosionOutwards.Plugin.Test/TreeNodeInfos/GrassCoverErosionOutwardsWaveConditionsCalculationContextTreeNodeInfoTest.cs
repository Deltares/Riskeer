// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuUpdateForeshoreProfileIndex = 3;
        private const int validateMenuItemIndex = 5;
        private const int calculateMenuItemIndex = 6;
        private const int clearOutputMenuItemIndex = 8;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");

        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        public override void Setup()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext));
        }

        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
        }

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
            Assert.IsNull(info.IsChecked);
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
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = name
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            Image icon = info.Image(context);

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

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            calculation.Attach(observer);

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
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

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            var parentContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            var parentContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                                   failureMechanism,
                                                                                                   assessmentSection);

            // Call
            bool canRemoveCalculation = info.CanRemove(context, parentContext);

            // Assert
            Assert.IsFalse(canRemoveCalculation);
        }

        [Test]
        public void OnNodeRemoved_CalculationInParent_ReturnTrue()
        {
            // Setup
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Attach(observer);
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            var parentContext = new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
        public void ChildNodeObjects_CalculationWithoutOutput_ReturnChildrenWithEmptyOutput()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            };
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = null
            };
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 1));

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    location
                }
            };
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = (Comment) children[0];
            Assert.AreSame(calculation.Comments, comment);

            var inputContext = (GrassCoverErosionOutwardsWaveConditionsInputContext) children[1];
            Assert.AreSame(calculation.InputParameters, inputContext.WrappedData);
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, inputContext.ForeshoreProfiles);
            CollectionAssert.AreEqual(new[]
            {
                location
            }, inputContext.HydraulicBoundaryLocations);

            Assert.IsInstanceOf<EmptyGrassCoverErosionOutwardsOutput>(children[2]);
        }

        [Test]
        public void ChildNodeObjects_CalculationWithOutput_ReturnChildrenWithOutput()
        {
            // Setup
            var location = new TestHydraulicBoundaryLocation();

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            };
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 1));

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                HydraulicBoundaryLocations =
                {
                    location
                }
            };
            failureMechanism.ForeshoreProfiles.AddRange(new[]
            {
                foreshoreProfile
            }, "path");

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(3, children.Length);

            var comment = (Comment) children[0];
            Assert.AreSame(calculation.Comments, comment);

            var inputContext = (GrassCoverErosionOutwardsWaveConditionsInputContext) children[1];
            Assert.AreSame(calculation.InputParameters, inputContext.WrappedData);
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, inputContext.ForeshoreProfiles);
            CollectionAssert.AreEqual(new[]
            {
                location
            }, inputContext.HydraulicBoundaryLocations);

            var output = (GrassCoverErosionOutwardsWaveConditionsOutput) children[2];
            Assert.AreSame(calculation.Output, output);
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddDeleteItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddPropertiesItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.Build()).Return(null);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            string validFilePath = Path.Combine(testDataPath, "complete.sqlite");

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validFilePath,
                Version = "random"
            };

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
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
                    Assert.AreEqual(15, menu.Items.Count);

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
        public void GivenAssessmentSectionWithoutHydraulicBoundaryDatabase_ThenValidationItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithInvalidHydraulicBoundaryDatabase_ThenValidationItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand '': bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenFailureMechanismContributionZero_ThenValidationItemDisabled()
        {
            // Given
            string validHydroDatabasePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 0
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHydroDatabasePath
            };

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenValidInput_ThenValidationItemEnabled()
        {
            // Given
            string validHydroDatabasePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHydroDatabasePath
            };

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            calculation.InputParameters.ForeshoreProfile = new TestForeshoreProfile();

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile();
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation();
            calculation.InputParameters.ForeshoreProfile = foreshoreProfileInput;
            TestForeshoreProfile.ChangeBreakWaterProperties(foreshoreProfileInput);

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
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

            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                }
            };
            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                         failureMechanism,
                                                                                         assessmentSectionStub);

            calculation.Attach(calculationObserver);
            calculation.InputParameters.Attach(calculationInputObserver);

            using (var treeViewControl = new TreeViewControl())
            {
                var guiStub = mocks.Stub<IGui>();
                guiStub.Stub(g => g.Get(nodeData, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                guiStub.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = guiStub;

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
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();

            var foreshoreProfileInput = new TestForeshoreProfile(true);
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    ForeshoreProfile = foreshoreProfileInput
                },
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };

            var nodeData = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                         failureMechanism,
                                                                                         assessmentSectionStub);

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
            string validHydroDatabasePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydroDatabasePath);

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };

            if (validCalculation)
            {
                calculation.InputParameters.HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(12);
                calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) 1.0;
                calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) 10.0;
                calculation.InputParameters.StepSize = WaveConditionsInputStepSize.One;
                calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 1.0;
                calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 10.0;
                calculation.InputParameters.Orientation = (RoundedDouble) 0;
            }

            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
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
                        Assert.AreEqual("Validatie van 'A' is gestart.", messages[0]);

                        if (!validCalculation)
                        {
                            Assert.AreEqual("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", messages[1]);
                        }

                        Assert.AreEqual("Validatie van 'A' is beëindigd.", messages.Last());
                    });
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithoutHydraulicBoundaryDatabase_ThenCalculationItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Er is geen hydraulische randvoorwaardendatabase geïmporteerd.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithInvalidHydraulicBoundaryDatabase_ThenCalculationItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand '': bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenFailureMechanismContributionZero_ThenCalculationItemDisabled()
        {
            // Given
            string validHydroDatabasePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 0
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHydroDatabasePath
            };

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(cmp => cmp.MainWindow).Return(mocks.Stub<IMainWindow>());
                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Then
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "De bijdrage van dit toetsspoor is nul.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenValidInput_ThenCalculationItemEnabled()
        {
            // Given
            string validHydroDatabasePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 5
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHydroDatabasePath
            };

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
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
            string validHydroDatabasePath = Path.Combine(testDataPath, "complete.sqlite");

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            IAssessmentSection assessmentSection = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydroDatabasePath);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = GetValidCalculation();
            calculation.Name = "A";
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
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
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);
                var observerMock = mocks.StrictMock<IObserver>();
                observerMock.Expect(o => o.UpdateObserver());
                calculation.Attach(observerMock);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
                gui.Stub(g => g.MainWindow).Return(mainWindow);

                var calculatorFactory = mocks.Stub<IHydraRingCalculatorFactory>();
                calculatorFactory.Stub(cf => cf.CreateWaveConditionsCosineCalculator(testDataPath)).Return(new TestWaveConditionsCosineCalculator());
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
                    });

                    Assert.AreEqual(3, calculation.Output.Items.Count());
                }
            }
        }

        [Test]
        public void GivenCalculationWithoutOutput_ThenClearOutputItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A",
                Output = null
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A",
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
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
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A",
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            calculation.Attach(observer);
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            using (var treeViewControl = new TreeViewControl())
            {
                var appFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
                var importHandler = mocks.Stub<IImportCommandHandler>();
                var exportHandler = mocks.Stub<IExportCommandHandler>();
                var updateHandler = mocks.Stub<IUpdateCommandHandler>();
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
                                                             updateHandler,
                                                             viewCommands,
                                                             context,
                                                             treeViewControl);

                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.Get(context, treeViewControl)).Return(menuBuilderMock);
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

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation()
        {
            return new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(9.3),
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