﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Settings;
using Core.Common.Gui.TestUtil;
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
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PresentationObjects.StandAlone;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.PropertyClasses.StandAlone;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Forms.Views.SectionResultViews;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.PresentationObjects;
using Ringtoets.Revetment.Forms.Views;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsPluginTest : NUnitFormTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new RingtoetsPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
                Assert.IsNull(plugin.RibbonCommandHandler);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSet_WhenProjectOnGuiChangesToProjectWithHydraulicBoundaryDatabaseNotLinked_ThenNoWarning()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;
                    gui.Run();

                    var project = new RingtoetsProject
                    {
                        AssessmentSections =
                        {
                            new AssessmentSection(AssessmentSectionComposition.Dike)
                        }
                    };

                    // When
                    Action action = () => gui.SetProject(project, null);

                    // Then
                    TestHelper.AssertLogMessagesCount(action, 0);
                }
            }

            mocks.VerifyAll();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSet_WhenProjectOnGuiChangesToProjectWithHydraulicBoundaryDatabaseLinkedToExistingLocation_ThenNoWarning()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            string testDataDir = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");
            string testFilePath = Path.Combine(testDataDir, "complete.sqlite");

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;
                    gui.Run();

                    var project = new RingtoetsProject
                    {
                        AssessmentSections =
                        {
                            new AssessmentSection(AssessmentSectionComposition.Dike)
                            {
                                HydraulicBoundaryDatabase =
                                {
                                    FilePath = testFilePath
                                }
                            }
                        }
                    };

                    // When
                    Action action = () => gui.SetProject(project, null);

                    // Then
                    TestHelper.AssertLogMessagesCount(action, 0);
                }
            }

            mocks.VerifyAll();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSet_WhenProjectOnGuiChangesToProjectWithHydraulicBoundaryDatabaseLinkedToNonExistingLocation_ThenWarning()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;
                    gui.Run();

                    const string nonExistingFile = "not_existing_file";

                    var project = new RingtoetsProject
                    {
                        AssessmentSections =
                        {
                            new AssessmentSection(AssessmentSectionComposition.Dike)
                            {
                                HydraulicBoundaryDatabase =
                                {
                                    FilePath = nonExistingFile
                                }
                            }
                        }
                    };

                    // When
                    Action action = () => gui.SetProject(project, null);

                    // Then
                    string fileMissingMessage = $"Fout bij het lezen van bestand '{nonExistingFile}': het bestand bestaat niet.";
                    string message = string.Format(
                        RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                        fileMissingMessage);
                    TestHelper.AssertLogMessageWithLevelIsGenerated(action, Tuple.Create(message, LogLevelConstant.Warn));
                }
            }

            mocks.VerifyAll();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        public void GetPropertyInfos_ReturnsSupportedPropertyInfos()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(22, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IProject),
                    typeof(RingtoetsProjectProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IAssessmentSection),
                    typeof(AssessmentSectionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(BackgroundData),
                    typeof(BackgroundDataProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(HydraulicBoundaryDatabaseContext),
                    typeof(HydraulicBoundaryDatabaseProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(FailureMechanismContributionContext),
                    typeof(FailureMechanismContributionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(FailureMechanismContext<IFailureMechanism>),
                    typeof(StandAloneFailureMechanismProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityOutwardsFailureMechanismContext),
                    typeof(MacroStabilityOutwardsFailureMechanismProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(PipingStructureFailureMechanismContext),
                    typeof(PipingStructureFailureMechanismProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ICalculationContext<CalculationGroup, IFailureMechanism>),
                    typeof(CalculationGroupContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ICalculationContext<ICalculation, IFailureMechanism>),
                    typeof(CalculationContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DesignWaterLevelCalculationsContext),
                    typeof(DesignWaterLevelCalculationsProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DesignWaterLevelCalculationContext),
                    typeof(DesignWaterLevelCalculationProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveHeightCalculationsContext),
                    typeof(WaveHeightCalculationsProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveHeightCalculationContext),
                    typeof(WaveHeightCalculationProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ForeshoreProfile),
                    typeof(ForeshoreProfileProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ForeshoreProfilesContext),
                    typeof(ForeshoreProfileCollectionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(SelectedTopLevelSubMechanismIllustrationPoint),
                    typeof(TopLevelSubMechanismIllustrationPointProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(SelectedTopLevelFaultTreeIllustrationPoint),
                    typeof(TopLevelFaultTreeIllustrationPointProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IllustrationPointContext<FaultTreeIllustrationPoint>),
                    typeof(FaultTreeIllustrationPointProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IllustrationPointContext<SubMechanismIllustrationPoint>),
                    typeof(SubMechanismIllustrationPointProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(FailureMechanismSectionsContext),
                    typeof(FailureMechanismSectionsProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ReferenceLineContext),
                    typeof(ReferenceLineProperties));
            }
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(2, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(ReferenceLineContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(HydraulicBoundaryDatabaseContext)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(17, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismContributionContext),
                    typeof(FailureMechanismContribution),
                    typeof(FailureMechanismContributionView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DesignWaterLevelCalculationsContext),
                    typeof(IEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(DesignWaterLevelCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaveHeightCalculationsContext),
                    typeof(IEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(WaveHeightCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(IAssessmentSection),
                    typeof(AssessmentSectionView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(IFailureMechanismContext<IFailureMechanism>),
                    typeof(FailureMechanismView<IFailureMechanism>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>),
                    typeof(IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>),
                    typeof(StrengthStabilityLengthwiseConstructionResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>),
                    typeof(IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult>),
                    typeof(WaterPressureAsphaltCoverResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<MacroStabilityOutwardsFailureMechanismSectionResult>),
                    typeof(IEnumerable<MacroStabilityOutwardsFailureMechanismSectionResult>),
                    typeof(MacroStabilityOutwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>),
                    typeof(IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult>),
                    typeof(GrassCoverSlipOffInwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>),
                    typeof(IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>),
                    typeof(GrassCoverSlipOffOutwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>),
                    typeof(IEnumerable<MicrostabilityFailureMechanismSectionResult>),
                    typeof(MicrostabilityResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>),
                    typeof(IEnumerable<PipingStructureFailureMechanismSectionResult>),
                    typeof(PipingStructureResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>),
                    typeof(IEnumerable<TechnicalInnovationFailureMechanismSectionResult>),
                    typeof(TechnicalInnovationResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(Comment),
                    typeof(CommentView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaveConditionsInputContext),
                    typeof(ICalculation<WaveConditionsInput>),
                    typeof(WaveConditionsInputView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StructuresOutputContext),
                    typeof(IStructuresCalculation),
                    typeof(GeneralResultFaultTreeIllustrationPointView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionsContext),
                    typeof(IEnumerable<FailureMechanismSection>),
                    typeof(FailureMechanismSectionsView));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(29, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssessmentSection)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(BackgroundData)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ReferenceLineContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismContext<IFailureMechanism>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityOutwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingStructureFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CategoryTreeFolder)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismContributionContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DesignWaterLevelCalculationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DesignWaterLevelLocationsGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveHeightCalculationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveHeightLocationsGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ForeshoreProfilesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DikeProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ForeshoreProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<MacroStabilityOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(Comment)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsProject)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveConditionsInputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StructuresOutputContext)));
            }
        }

        [Test]
        public void GetChildDataWithViewDefinitions_AssessmentSection_ReturnFailureMechanismContribution()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                IEnumerable<object> childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(assessmentSection);

                // Assert
                CollectionAssert.AreEqual(new object[]
                {
                    assessmentSection.FailureMechanismContribution
                }, childrenWithViewDefinitions);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GetChildDataWithViewDefinitions_RingtoetsProjectWithChildren_ReturnChildren()
        {
            // Setup
            var project = new RingtoetsProject();
            project.AssessmentSections.Add(new AssessmentSection(AssessmentSectionComposition.Dike));
            project.AssessmentSections.Add(new AssessmentSection(AssessmentSectionComposition.Dike));
            project.AssessmentSections.Add(new AssessmentSection(AssessmentSectionComposition.Dike));

            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                IEnumerable<object> childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(project);

                // Assert
                List<AssessmentSection> expectedResult = project.AssessmentSections;
                CollectionAssert.AreEquivalent(expectedResult, childrenWithViewDefinitions);
            }
        }

        [Test]
        public void GetChildDataWithViewDefinitions_UnsupportedData_ReturnEmpty()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                IEnumerable<object> childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(1);

                // Assert
                CollectionAssert.IsEmpty(childrenWithViewDefinitions);
            }
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedImportInfos()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(3, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ReferenceLineContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(FailureMechanismSectionsContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ForeshoreProfilesContext)));
            }
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedUpdateInfos()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                UpdateInfo[] updateInfos = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(1, updateInfos.Length);
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(ForeshoreProfilesContext)));
            }
        }

        [Test]
        public void Activate_WithoutGui_ThrowsInvalidOperationException()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                Assert.Throws<InvalidOperationException>(test);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void Activate_WithGui_ExpectedProperties()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindow = mockRepository.StrictMock<IMainWindow>();
            var documentViewController = mockRepository.StrictMock<IDocumentViewController>();
            var gui = mockRepository.StrictMock<IGui>();
            gui.Expect(g => g.MainWindow).Return(mainWindow).Repeat.AtLeastOnce();
            gui.Expect(g => g.DocumentViewController).Return(documentViewController);
            gui.Expect(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            mockRepository.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                plugin.Gui = gui;

                // Call
                plugin.Activate();

                // Assert
                Assert.IsInstanceOf<RingtoetsRibbon>(plugin.RibbonCommandHandler);
            }

            mockRepository.VerifyAll();
        }
    }
}