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
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Revetment.Data;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class StabilityStoneCoverWaveConditionsCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int validateMenuItemIndex = 3;
        private const int calculateMenuItemIndex = 4;
        private const int clearOutputMenuItemIndex = 6;

        private MockRepository mocks;
        private StabilityStoneCoverPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new StabilityStoneCoverPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(StabilityStoneCoverWaveConditionsCalculationContext));
        }

        [TearDown]
        public override void TearDown()
        {
            plugin.Dispose();
            mocks.VerifyAll();
            base.TearDown();
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
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = name
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            };
            mocks.ReplayAll();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = null
            };
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 1));

            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                ForeshoreProfiles =
                {
                    foreshoreProfile
                }
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, inputContext.ForeshoreProfiles);
            CollectionAssert.AreEqual(new[]
            {
                location
            }, inputContext.HydraulicBoundaryLocations);

            Assert.IsInstanceOf<EmptyStabilityStoneCoverOutput>(children[2]);
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

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var foreshoreProfile = new TestForeshoreProfile(new BreakWater(BreakWaterType.Caisson, 1));

            var failureMechanism = new StabilityStoneCoverFailureMechanism
            {
                ForeshoreProfiles =
                {
                    foreshoreProfile
                }
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
            CollectionAssert.AreEqual(new[]
            {
                foreshoreProfile
            }, inputContext.ForeshoreProfiles);
            CollectionAssert.AreEqual(new[]
            {
                location
            }, inputContext.HydraulicBoundaryLocations);

            var output = (StabilityStoneCoverWaveConditionsOutput) children[2];
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

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            calculation.Attach(observer);

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            failureMechanism.WaveConditionsCalculationGroup.Children.Add(calculation);
            failureMechanism.WaveConditionsCalculationGroup.Attach(observer);
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var parentContext = new StabilityStoneCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
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
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation();
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSection);

            var menuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
                menuBuilderMock.Expect(mb => mb.AddRenameItem()).Return(menuBuilderMock);
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

                mocks.ReplayAll();

                plugin.Gui = gui;

                // Call
                info.ContextMenuStrip(context, null, treeViewControl);
            }
            // Assert
            // Assert expectancies are called in TearDown()
        }

        [Test]
        public void GivenFailureMechanismWithoutSections_ThenValidationItemDisabled()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
        public void GivenAssessmentSectionWithoutHydraulicBoundaryDatabase_ThenValidationItemDisabled()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
        public void GivenValidInput_ThenValidationItemEnabled()
        {
            // Given
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                       Path.Combine("HydraulicBoundaryDatabaseImporter", "complete.sqlite"));

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHydroDatabasePath
            };

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
        [TestCase(false)]
        [TestCase(true)]
        public void GivenCalculation_WhenValidating_ThenCalculationValidated(bool validCalculation)
        {
            // Given
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                       Path.Combine("HydraulicBoundaryDatabaseImporter", "complete.sqlite"));

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydroDatabasePath);

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };

            if (validCalculation)
            {
                calculation.InputParameters.HydraulicBoundaryLocation = TestHydraulicBoundaryLocation.CreateDesignWaterLevelCalculated(12.0);
                calculation.InputParameters.LowerBoundaryRevetment = (RoundedDouble) 1.0;
                calculation.InputParameters.UpperBoundaryRevetment = (RoundedDouble) 10.0;
                calculation.InputParameters.StepSize = WaveConditionsInputStepSize.One;
                calculation.InputParameters.LowerBoundaryWaterLevels = (RoundedDouble) 1.0;
                calculation.InputParameters.UpperBoundaryWaterLevels = (RoundedDouble) 10.0;
                calculation.InputParameters.Orientation = (RoundedDouble) 0;
            }

            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSectionStub);

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

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Precondition
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  validateMenuItemIndex,
                                                                  "&Valideren",
                                                                  "Valideer de invoer voor deze berekening.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon);

                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[validateMenuItemIndex];
                    Action call = () => validateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        int expectedMessageCount = validCalculation ? 2 : 3;
                        Assert.AreEqual(expectedMessageCount, messages.Length);
                        StringAssert.StartsWith("Validatie van 'A' gestart om: ", messages[0]);

                        if (!validCalculation)
                        {
                            StringAssert.StartsWith("Validatie mislukt: Er is geen hydraulische randvoorwaardenlocatie geselecteerd.", messages[1]);
                        }

                        StringAssert.StartsWith("Validatie van 'A' beëindigd om: ", messages.Last());
                    });
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithoutHydraulicBoundaryDatabase_ThenCalculationItemDisabled()
        {
            // Given
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
        public void GivenValidInput_ThenCalculationItemEnabled()
        {
            // Given
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                       Path.Combine("HydraulicBoundaryDatabaseImporter", "complete.sqlite"));

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHydroDatabasePath
            };

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A"
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO,
                                                                       Path.Combine("HydraulicBoundaryDatabaseImporter", "complete.sqlite"));

            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            IAssessmentSection assessmentSectionStub = AssessmentSectionHelper.CreateAssessmentSectionStub(
                failureMechanism, mocks, validHydroDatabasePath);

            StabilityStoneCoverWaveConditionsCalculation calculation = GetValidCalculation();
            calculation.Name = "A";
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                  failureMechanism,
                                                                                  assessmentSectionStub);

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

                mocks.ReplayAll();

                plugin.Gui = gui;

                using (new HydraRingCalculatorFactoryConfig())
                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Precondition
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  "Voer deze berekening uit.",
                                                                  RingtoetsCommonFormsResources.CalculateIcon);

                    // When
                    ToolStripItem calculateMenuItem = contextMenu.Items[calculateMenuItemIndex];
                    Action call = () => calculateMenuItem.PerformClick();

                    // Then
                    TestHelper.AssertLogMessages(call, logMessages =>
                    {
                        string[] messages = logMessages.ToArray();
                        Assert.AreEqual(27, messages.Length);
                        StringAssert.StartsWith("Berekening van 'A' gestart om: ", messages[2]);
                        StringAssert.StartsWith("Berekening van 'A' beëindigd om: ", messages[25]);
                        StringAssert.StartsWith("Uitvoeren van 'Golfcondities voor blokken en zuilen voor 'A' berekenen' is gelukt.", messages[26]);
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
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = null
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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
            var failureMechanism = new StabilityStoneCoverFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = "A",
                Output = new StabilityStoneCoverWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>(),
                                                                     Enumerable.Empty<WaveConditionsOutput>())
            };
            calculation.Attach(observer);
            var context = new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
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

                mocks.ReplayAll();

                plugin.Gui = gui;

                DialogBoxHandler = (name, wnd) =>
                {
                    var messageBox = new MessageBoxTester(wnd);
                    messageBox.ClickOk();
                };

                using (ContextMenuStrip contextMenu = info.ContextMenuStrip(context, null, treeViewControl))
                {
                    // Precondition
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  clearOutputMenuItemIndex,
                                                                  "&Wis uitvoer...",
                                                                  "Wis de uitvoer van deze berekening.",
                                                                  RingtoetsCommonFormsResources.ClearIcon);

                    // When
                    ToolStripItem validateMenuItem = contextMenu.Items[clearOutputMenuItemIndex];
                    validateMenuItem.PerformClick();

                    // Then
                    Assert.IsNull(calculation.Output);
                    // Check expectancies in TearDown()
                }
            }
        }

        private static StabilityStoneCoverWaveConditionsCalculation GetValidCalculation()
        {
            return new StabilityStoneCoverWaveConditionsCalculation
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