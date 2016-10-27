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
using Core.Common.Base.Geometry;
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
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Plugin;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.HydraRing.Data;
using Ringtoets.Revetment.Data;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class GrassCoverErosionOutwardsWaveConditionsCalculationContextTreeNodeInfoTest : NUnitFormTest
    {
        private const int validateMenuItemIndex = 2;
        private const int calculateMenuItemIndex = 3;
        private const int clearOutputMenuItemIndex = 4;

        private MockRepository mocks;
        private GrassCoverErosionOutwardsPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new GrassCoverErosionOutwardsPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext));
        }

        [TearDown]
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
            Assert.AreEqual(typeof(GrassCoverErosionOutwardsWaveConditionsCalculationContext), info.TagType);

            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnCalculationName()
        {
            // Setup
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
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
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();
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
            var location = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = null
            };
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        Enumerable.Empty<Point2D>(),
                                                        new BreakWater(BreakWaterType.Caisson, 1),
                                                        new ForeshoreProfile.ConstructionProperties());

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                ForeshoreProfiles =
                {
                    foreshoreProfile
                },
                HydraulicBoundaryLocations =
                {
                    location
                }
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(3, children.Length);

            var commentsContext = (CommentContext<ICommentable>) children[0];
            Assert.AreSame(calculation, commentsContext.WrappedData);

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
            var location = new HydraulicBoundaryLocation(0, string.Empty, 0, 0);
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                Locations =
                {
                    location
                }
            };

            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;
            mocks.ReplayAll();

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Output = new GrassCoverErosionOutwardsWaveConditionsOutput(Enumerable.Empty<WaveConditionsOutput>())
            };
            var foreshoreProfile = new ForeshoreProfile(new Point2D(0, 0),
                                                        Enumerable.Empty<Point2D>(),
                                                        new BreakWater(BreakWaterType.Caisson, 1),
                                                        new ForeshoreProfile.ConstructionProperties());

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                ForeshoreProfiles =
                {
                    foreshoreProfile
                },
                HydraulicBoundaryLocations =
                {
                    location
                }
            };
            var context = new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                        failureMechanism,
                                                                                        assessmentSection);

            // Call
            object[] children = info.ChildNodeObjects(context);

            // Assert
            Assert.AreEqual(3, children.Length);

            var commentsContext = (CommentContext<ICommentable>) children[0];
            Assert.AreSame(calculation, commentsContext.WrappedData);

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
            menuBuilderMock.Expect(mb => mb.AddExportItem()).Return(menuBuilderMock);
            menuBuilderMock.Expect(mb => mb.AddSeparator()).Return(menuBuilderMock);
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
        public void GivenAssessmentSectionWithoutHydroDatabase_ThenValidationItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
        public void GivenAssessmentSectionWithoutValidPathForCalculation_ThenValidationItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
                                                                  "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand '': Bestandspad mag niet leeg of ongedefinieerd zijn.",
                                                                  RingtoetsCommonFormsResources.ValidateIcon,
                                                                  false);
                }
            }
        }

        [Test]
        public void GivenAssessmentSectionWithValidPathForCalculation_ThenValidationItemEnabled()
        {
            // Given
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO,
                                                                       Path.Combine("HydraulicBoundaryLocationReader", "complete.sqlite"));

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
        [TestCase(true)]
        [TestCase(false)]
        public void GivenCalculation_WhenValidating_ThenCalculationValidated(bool validCalculation)
        {
            // Given
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO,
                                                                       Path.Combine("HydraulicBoundaryLocationReader", "complete.sqlite"));

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHydroDatabasePath
            };
            assessmentSection.Stub(a => a.Id).Return("someId");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100, 20));

            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = "A"
            };

            if (validCalculation)
            {
                calculation.InputParameters.HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1, "", 1, 1)
                {
                    DesignWaterLevel = (RoundedDouble) 12.0
                };
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
                        var messages = logMessages.ToArray();
                        var expectedMessageCount = validCalculation ? 2 : 3;
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
        public void GivenAssessmentSectionWithoutHydroDatabase_ThenCalculationItemDisabled()
        {
            // Given
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
        [TestCase(true, TestName = "AssessmentSection_WithOrWithoutPath_CalculateEnabledOrDisabled(true)")]
        [TestCase(false, TestName = "AssessmentSection_WithOrWithoutPath_CalculateEnabledOrDisabled(false)")]
        public void AssessmentSection_WithOrWithoutValidPath_CalculateItemEnabledOrDisabled(bool validPath)
        {
            // Setup
            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            var hydraulicBoundaryDatabase = new HydraulicBoundaryDatabase();
            if (validPath)
            {
                hydraulicBoundaryDatabase.FilePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO,
                                                                                Path.Combine("HydraulicBoundaryLocationReader", "complete.sqlite"));
            }
            assessmentSection.HydraulicBoundaryDatabase = hydraulicBoundaryDatabase;

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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
                    var expectedTooltip = validPath
                                              ? "Voer deze berekening uit."
                                              : "Herstellen van de verbinding met de hydraulische randvoorwaardendatabase is mislukt. Fout bij het lezen van bestand '': Bestandspad mag niet leeg of ongedefinieerd zijn.";
                    TestHelper.AssertContextMenuStripContainsItem(contextMenu,
                                                                  calculateMenuItemIndex,
                                                                  "Be&rekenen",
                                                                  expectedTooltip,
                                                                  RingtoetsCommonFormsResources.CalculateIcon,
                                                                  validPath);
                }
            }
        }

        [Test]
        public void GivenValidCalculation_WhenCalculating_ThenCalculationReturnsResult()
        {
            // Given
            string validHydroDatabasePath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO,
                                                                       Path.Combine("HydraulicBoundaryLocationReader", "complete.sqlite"));

            var failureMechanism = new GrassCoverErosionOutwardsFailureMechanism
            {
                Contribution = 10
            };
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            assessmentSection.HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
            {
                FilePath = validHydroDatabasePath
            };
            assessmentSection.Stub(a => a.Id).Return("someId");
            assessmentSection.Stub(a => a.FailureMechanismContribution).Return(
                new FailureMechanismContribution(Enumerable.Empty<IFailureMechanism>(), 100, 20));

            var calculation = GetValidCalculation();
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var mainWindow = mocks.Stub<IMainWindow>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
                        var messages = logMessages.ToArray();
                        Assert.AreEqual(14, messages.Length);
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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
                var viewCommands = mocks.Stub<IViewCommands>();
                var menuBuilderMock = new ContextMenuBuilder(appFeatureCommandHandler,
                                                             importHandler,
                                                             exportHandler,
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

        private static GrassCoverErosionOutwardsWaveConditionsCalculation GetValidCalculation()
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = new HydraulicBoundaryLocation(1300001, "", 0.0, 0.0),
                    ForeshoreProfile = CreateForeshoreProfile(),
                    UseForeshore = true,
                    UseBreakWater = true,
                    StepSize = WaveConditionsInputStepSize.Half,
                    LowerBoundaryRevetment = (RoundedDouble) 4,
                    UpperBoundaryRevetment = (RoundedDouble) 10,
                    UpperBoundaryWaterLevels = (RoundedDouble) 8,
                    LowerBoundaryWaterLevels = (RoundedDouble) 7.1
                }
            };
            calculation.InputParameters.HydraulicBoundaryLocation.DesignWaterLevel = (RoundedDouble) 9.3;
            return calculation;
        }

        private static ForeshoreProfile CreateForeshoreProfile()
        {
            return new ForeshoreProfile(new Point2D(0, 0),
                                        new[]
                                        {
                                            new Point2D(3.3, 4.4),
                                            new Point2D(5.5, 6.6)
                                        },
                                        new BreakWater(BreakWaterType.Dam, 10.0),
                                        new ForeshoreProfile.ConstructionProperties());
        }
    }
}