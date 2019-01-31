// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Threading;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.Plugin;
using Core.Common.Gui.Settings;
using Core.Common.Gui.TestUtil;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Structures;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.StandAlone;
using Riskeer.Integration.Data.StandAlone.SectionResults;
using Riskeer.Integration.Forms.PresentationObjects;
using Riskeer.Integration.Forms.PresentationObjects.StandAlone;
using Riskeer.Integration.Forms.PropertyClasses;
using Riskeer.Integration.Forms.PropertyClasses.StandAlone;
using Riskeer.Integration.Forms.Views;
using Riskeer.Integration.Forms.Views.SectionResultViews;
using RiskeerCommonServiceResources = Riskeer.Common.Service.Properties.Resources;

namespace Riskeer.Integration.Plugin.Test
{
    [TestFixture]
    public class RiskeerPluginTest : NUnitFormTest
    {
        [Test]
        [Apartment(ApartmentState.STA)]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            using (var plugin = new RiskeerPlugin())
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RiskeerPlugin())
                {
                    plugin.Gui = gui;
                    gui.Run();

                    var project = new RiskeerProject
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

            string testDataDir = TestHelper.GetTestDataPath(TestDataPath.Riskeer.Common.IO, nameof(HydraulicBoundaryDatabase));
            string testFilePath = Path.Combine(testDataDir, "complete.sqlite");

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RiskeerPlugin())
                {
                    plugin.Gui = gui;
                    gui.Run();

                    var project = new RiskeerProject
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RiskeerPlugin())
                {
                    plugin.Gui = gui;
                    gui.Run();

                    const string nonExistingFile = "not_existing_file";

                    var project = new RiskeerProject
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
                        RiskeerCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
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
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(28, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IProject),
                    typeof(RiskeerProjectProperties));

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
                    typeof(AssessmentSectionCompositionProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(NormContext),
                    typeof(NormProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IFailureMechanismContext<IFailureMechanism>),
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

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(FailureMechanismAssemblyCategoriesContext),
                    typeof(FailureMechanismAssemblyCategoriesProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityOutwardsAssemblyCategoriesContext),
                    typeof(FailureMechanismSectionAssemblyCategoriesProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DesignWaterLevelCalculationsGroupContext),
                    typeof(DesignWaterLevelCalculationsGroupProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveHeightCalculationsGroupContext),
                    typeof(WaveHeightCalculationsGroupProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(AssemblyResultCategoriesContext),
                    typeof(AssemblyResultCategoriesProperties));
            }
        }

        [Test]
        public void GetExportInfos_ReturnsSupportedExportInfos()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                ExportInfo[] exportInfos = plugin.GetExportInfos().ToArray();

                // Assert
                Assert.AreEqual(3, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(ReferenceLineContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(HydraulicBoundaryDatabaseContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(AssemblyResultsContext)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(30, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismContributionContext),
                    typeof(FailureMechanismContribution),
                    typeof(FailureMechanismContributionView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(NormContext),
                    typeof(FailureMechanismContribution),
                    typeof(AssessmentSectionAssemblyCategoriesView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DesignWaterLevelCalculationsContext),
                    typeof(IObservableEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(DesignWaterLevelCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaveHeightCalculationsContext),
                    typeof(IObservableEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(WaveHeightCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(IAssessmentSection),
                    typeof(AssessmentSectionView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>),
                    typeof(StrengthStabilityLengthwiseConstructionResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult>),
                    typeof(WaterPressureAsphaltCoverResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(ProbabilityFailureMechanismSectionResultContext<MacroStabilityOutwardsFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<MacroStabilityOutwardsFailureMechanismSectionResult>),
                    typeof(MacroStabilityOutwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult>),
                    typeof(GrassCoverSlipOffInwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>),
                    typeof(GrassCoverSlipOffOutwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<MicrostabilityFailureMechanismSectionResult>),
                    typeof(MicrostabilityResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<PipingStructureFailureMechanismSectionResult>),
                    typeof(PipingStructureResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>),
                    typeof(IObservableEnumerable<TechnicalInnovationFailureMechanismSectionResult>),
                    typeof(TechnicalInnovationResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(Comment),
                    typeof(CommentView));

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

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(AssemblyResultTotalContext),
                    typeof(AssessmentSection),
                    typeof(AssemblyResultTotalView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(AssemblyResultPerSectionContext),
                    typeof(AssessmentSection),
                    typeof(AssemblyResultPerSectionView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(AssemblyResultPerSectionMapContext),
                    typeof(AssessmentSection),
                    typeof(AssemblyResultPerSectionMapView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismAssemblyCategoriesContextBase),
                    typeof(IFailureMechanism),
                    typeof(FailureMechanismAssemblyCategoriesView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MacroStabilityOutwardsAssemblyCategoriesContext),
                    typeof(MacroStabilityOutwardsFailureMechanism),
                    typeof(MacroStabilityOutwardsAssemblyCategoriesView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(AssemblyResultCategoriesContext),
                    typeof(AssessmentSection),
                    typeof(AssemblyResultCategoriesView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MacroStabilityOutwardsFailureMechanismContext),
                    typeof(MacroStabilityOutwardsFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<MacroStabilityOutwardsFailureMechanism, MacroStabilityOutwardsFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MicrostabilityFailureMechanismContext),
                    typeof(MicrostabilityFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<MicrostabilityFailureMechanism, MicrostabilityFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverSlipOffOutwardsFailureMechanismContext),
                    typeof(GrassCoverSlipOffOutwardsFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<GrassCoverSlipOffOutwardsFailureMechanism, GrassCoverSlipOffOutwardsFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverSlipOffInwardsFailureMechanismContext),
                    typeof(GrassCoverSlipOffInwardsFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<GrassCoverSlipOffInwardsFailureMechanism, GrassCoverSlipOffInwardsFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(PipingStructureFailureMechanismContext),
                    typeof(PipingStructureFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StrengthStabilityLengthwiseConstructionFailureMechanismContext),
                    typeof(StrengthStabilityLengthwiseConstructionFailureMechanism),
                    typeof(FailureMechanismWithoutDetailedAssessmentView<StrengthStabilityLengthwiseConstructionFailureMechanism, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(TechnicalInnovationFailureMechanismContext),
                    typeof(TechnicalInnovationFailureMechanism),
                    typeof(FailureMechanismWithoutDetailedAssessmentView<TechnicalInnovationFailureMechanism, TechnicalInnovationFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaterPressureAsphaltCoverFailureMechanismContext),
                    typeof(WaterPressureAsphaltCoverFailureMechanism),
                    typeof(FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult>));
            }
        }

        [Test]
        public void GetTreeNodeInfos_ReturnsSupportedTreeNodeInfos()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                TreeNodeInfo[] treeNodeInfos = plugin.GetTreeNodeInfos().ToArray();

                // Assert
                Assert.AreEqual(41, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssessmentSection)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(BackgroundData)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(NormContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ReferenceLineContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverSlipOffInwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverSlipOffOutwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityOutwardsFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MicrostabilityFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingStructureFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StrengthStabilityLengthwiseConstructionFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(TechnicalInnovationFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaterPressureAsphaltCoverFailureMechanismContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CategoryTreeFolder)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismContributionContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DesignWaterLevelCalculationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DesignWaterLevelCalculationsGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveHeightCalculationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveHeightCalculationsGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ForeshoreProfilesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DikeProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ForeshoreProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityFailureMechanismSectionResultContext<MacroStabilityOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(Comment)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RiskeerProject)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StructuresOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultTotalContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultPerSectionContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityOutwardsAssemblyCategoriesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismAssemblyCategoriesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultCategoriesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultPerSectionMapContext)));
            }
        }

        [Test]
        public void GetChildDataWithViewDefinitions_AssessmentSection_ReturnFailureMechanismContribution()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var plugin = new RiskeerPlugin())
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
        public void GetChildDataWithViewDefinitions_ProjectWithChildren_ReturnChildren()
        {
            // Setup
            var project = new RiskeerProject();
            project.AssessmentSections.Add(new AssessmentSection(AssessmentSectionComposition.Dike));
            project.AssessmentSections.Add(new AssessmentSection(AssessmentSectionComposition.Dike));
            project.AssessmentSections.Add(new AssessmentSection(AssessmentSectionComposition.Dike));

            using (var plugin = new RiskeerPlugin())
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
            using (var plugin = new RiskeerPlugin())
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
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(4, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ReferenceLineContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(FailureMechanismSectionsContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ForeshoreProfilesContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(HydraulicBoundaryDatabaseContext)));
            }
        }

        [Test]
        public void GetImportInfos_ReturnsSupportedUpdateInfos()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                UpdateInfo[] updateInfos = plugin.GetUpdateInfos().ToArray();

                // Assert
                Assert.AreEqual(9, updateInfos.Length);
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(ForeshoreProfilesContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(GrassCoverSlipOffInwardsFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(GrassCoverSlipOffOutwardsFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(MacroStabilityOutwardsFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(MicrostabilityFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(PipingStructureFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(StrengthStabilityLengthwiseConstructionFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(TechnicalInnovationFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(WaterPressureAsphaltCoverFailureMechanismSectionsContext)));
            }
        }

        [Test]
        public void Activate_WithoutGui_ThrowsInvalidOperationException()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
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
            var viewCommands = mockRepository.StrictMock<IViewCommands>();
            var gui = mockRepository.StrictMock<IGui>();
            gui.Expect(g => g.MainWindow).Return(mainWindow).Repeat.AtLeastOnce();
            gui.Expect(g => g.DocumentViewController).Return(documentViewController);
            gui.Expect(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Expect(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Expect(g => g.ViewCommands).Return(viewCommands);
            mockRepository.ReplayAll();

            using (var plugin = new RiskeerPlugin())
            {
                plugin.Gui = gui;

                // Call
                plugin.Activate();

                // Assert
                Assert.IsInstanceOf<RiskeerRibbon>(plugin.RibbonCommandHandler);
            }

            mockRepository.VerifyAll();
        }
    }
}