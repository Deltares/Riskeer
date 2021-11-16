// Copyright (C) Stichting Deltares 2021. All rights reserved.
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
using System.Windows.Media;
using System.Windows.Threading;
using Core.Common.Base;
using Core.Common.Base.Storage;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.TestUtil;
using Core.Common.Util.Extensions;
using Core.Gui;
using Core.Gui.Forms.Main;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Plugin;
using Core.Gui.Settings;
using Core.Gui.TestUtil;
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
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.Views;
using Riskeer.Integration.Data;
using Riskeer.Integration.Data.FailurePath;
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
        public void Constructor_ExpectedValues()
        {
            // Call
            using (var plugin = new RiskeerPlugin())
            {
                // Assert
                Assert.IsInstanceOf<PluginBase>(plugin);
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var project = new RiskeerProject(new AssessmentSection(AssessmentSectionComposition.Dike));

                // When
                void Action() => gui.SetProject(project, null);

                // Then
                TestHelper.AssertLogMessagesCount(Action, 0);
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    HydraulicBoundaryDatabase =
                    {
                        FilePath = testFilePath
                    }
                };
                HydraulicBoundaryDatabaseTestHelper.SetHydraulicBoundaryLocationConfigurationSettings(assessmentSection.HydraulicBoundaryDatabase);
                var project = new RiskeerProject(assessmentSection);

                // When
                void Action() => gui.SetProject(project, null);

                // Then
                TestHelper.AssertLogMessagesCount(Action, 0);
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

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                const string nonExistingFile = "not_existing_file";
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    HydraulicBoundaryDatabase =
                    {
                        FilePath = nonExistingFile
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                // When
                void Action() => gui.SetProject(project, null);

                // Then
                var fileMissingMessage = $"Fout bij het lezen van bestand '{nonExistingFile}': het bestand bestaat niet.";
                string message = string.Format(
                    RiskeerCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                    fileMissingMessage);
                TestHelper.AssertLogMessageWithLevelIsGenerated(Action, Tuple.Create(message, LogLevelConstant.Warn));
            }

            mocks.VerifyAll();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        public void GetStateInfos_ReturnsSupportedStateInfos()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                StateInfo[] stateInfos = plugin.GetStateInfos().ToArray();

                // Assert
                Assert.AreEqual(4, stateInfos.Length);
                Assert.IsTrue(stateInfos.Any(si => si.Name == "Traject"));
                Assert.IsTrue(stateInfos.Any(si => si.Name == "Hydraulische\r\nbelastingen"));
                Assert.IsTrue(stateInfos.Any(si => si.Name == "Sterkte-\r\nberekeningen"));
                Assert.IsTrue(stateInfos.Any(si => si.Name == "Faalpaden /\r\nassemblage"));
            }
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
                Assert.AreEqual(27, propertyInfos.Length);

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StateRootContext),
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
                    typeof(NormContext),
                    typeof(NormProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IFailurePathContext<IFailureMechanism>),
                    typeof(StandAloneFailurePathProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(SpecificFailurePathContext),
                    typeof(SpecificFailurePathProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(MacroStabilityOutwardsFailurePathContext),
                    typeof(MacroStabilityOutwardsFailurePathProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(PipingStructureFailurePathContext),
                    typeof(PipingStructureFailurePathProperties));

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
                    typeof(WaterLevelCalculationsForNormTargetProbabilityContext),
                    typeof(WaterLevelCalculationsForNormTargetProbabilityProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaterLevelCalculationsForUserDefinedTargetProbabilityContext),
                    typeof(WaterLevelCalculationsForUserDefinedTargetProbabilityProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveHeightCalculationsForUserDefinedTargetProbabilityContext),
                    typeof(WaveHeightCalculationsForUserDefinedTargetProbabilityProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DesignWaterLevelCalculationContext),
                    typeof(DesignWaterLevelCalculationProperties));

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
                    typeof(AssemblyResultCategoriesContext),
                    typeof(AssemblyResultCategoriesProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(StructuresOutputContext),
                    typeof(StructuresOutputProperties));
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
                Assert.AreEqual(9, exportInfos.Length);
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(ReferenceLineContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(HydraulicBoundaryDatabaseContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(AssemblyResultsContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(WaterLevelCalculationsForUserDefinedTargetProbabilityContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(WaveHeightCalculationsForUserDefinedTargetProbabilityContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(WaterLevelCalculationsForNormTargetProbabilityContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(WaterLevelCalculationsForNormTargetProbabilitiesGroupContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext)));
                Assert.IsTrue(exportInfos.Any(ei => ei.DataType == typeof(WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext)));
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfos()
        {
            // Setup
            const string symbol = "<symbol>";
            var fontFamily = new FontFamily();

            var mockRepository = new MockRepository();
            var gui = mockRepository.Stub<IGui>();
            gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
            gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            gui.Stub(g => g.ViewHost).Return(mockRepository.Stub<IViewHost>());
            gui.Stub(g => g.ActiveStateInfo).Return(new StateInfo(string.Empty, symbol, fontFamily, p => p));
            mockRepository.ReplayAll();

            using (var plugin = new RiskeerPlugin
            {
                Gui = gui
            })
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(34, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(NormContext),
                    typeof(FailureMechanismContribution),
                    typeof(AssessmentSectionAssemblyCategoriesView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaterLevelCalculationsForNormTargetProbabilityContext),
                    typeof(IObservableEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(DesignWaterLevelCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaterLevelCalculationsForUserDefinedTargetProbabilityContext),
                    typeof(IObservableEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(DesignWaterLevelCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaveHeightCalculationsForUserDefinedTargetProbabilityContext),
                    typeof(IObservableEnumerable<HydraulicBoundaryLocationCalculation>),
                    typeof(WaveHeightCalculationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(AssessmentSectionStateRootContext),
                    typeof(AssessmentSectionReferenceLineView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(HydraulicLoadsStateRootContext),
                    typeof(AssessmentSectionExtendedView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(CalculationsStateRootContext),
                    typeof(AssessmentSectionExtendedView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailurePathsStateRootContext),
                    typeof(AssessmentSectionExtendedView));

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
                    typeof(MacroStabilityOutwardsFailurePathContext),
                    typeof(MacroStabilityOutwardsFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<MacroStabilityOutwardsFailureMechanism, MacroStabilityOutwardsFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(MicrostabilityFailurePathContext),
                    typeof(MicrostabilityFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<MicrostabilityFailureMechanism, MicrostabilityFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverSlipOffOutwardsFailurePathContext),
                    typeof(GrassCoverSlipOffOutwardsFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<GrassCoverSlipOffOutwardsFailureMechanism, GrassCoverSlipOffOutwardsFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(GrassCoverSlipOffInwardsFailurePathContext),
                    typeof(GrassCoverSlipOffInwardsFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<GrassCoverSlipOffInwardsFailureMechanism, GrassCoverSlipOffInwardsFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(PipingStructureFailurePathContext),
                    typeof(PipingStructureFailureMechanism),
                    typeof(FailureMechanismWithDetailedAssessmentView<PipingStructureFailureMechanism, PipingStructureFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(StrengthStabilityLengthwiseConstructionFailurePathContext),
                    typeof(StrengthStabilityLengthwiseConstructionFailureMechanism),
                    typeof(FailureMechanismWithoutDetailedAssessmentView<StrengthStabilityLengthwiseConstructionFailureMechanism, StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(TechnicalInnovationFailurePathContext),
                    typeof(TechnicalInnovationFailureMechanism),
                    typeof(FailureMechanismWithoutDetailedAssessmentView<TechnicalInnovationFailureMechanism, TechnicalInnovationFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaterPressureAsphaltCoverFailurePathContext),
                    typeof(WaterPressureAsphaltCoverFailureMechanism),
                    typeof(FailureMechanismWithoutDetailedAssessmentView<WaterPressureAsphaltCoverFailureMechanism, WaterPressureAsphaltCoverFailureMechanismSectionResult>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(SpecificFailurePathContext),
                    typeof(SpecificFailurePathView));

                viewInfos.ForEachElementDo(vi =>
                {
                    Assert.AreEqual(symbol, vi.GetSymbol());
                    Assert.AreSame(fontFamily, vi.GetFontFamily());
                });
            }

            mockRepository.VerifyAll();
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
                Assert.AreEqual(47, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssessmentSectionStateRootContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicLoadsStateRootContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CalculationsStateRootContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailurePathsStateRootContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(BackgroundData)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(NormContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ReferenceLineContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverSlipOffInwardsFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GrassCoverSlipOffOutwardsFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityOutwardsFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MicrostabilityFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(PipingStructureFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StrengthStabilityLengthwiseConstructionFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(TechnicalInnovationFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaterPressureAsphaltCoverFailurePathContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CategoryTreeFolder)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaterLevelCalculationsForNormTargetProbabilitiesGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaterLevelCalculationsForNormTargetProbabilityContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaterLevelCalculationsForUserDefinedTargetProbabilitiesGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaterLevelCalculationsForUserDefinedTargetProbabilityContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveHeightCalculationsForUserDefinedTargetProbabilitiesGroupContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveHeightCalculationsForUserDefinedTargetProbabilityContext)));
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
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(StructuresOutputContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultTotalContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultPerSectionContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(MacroStabilityOutwardsAssemblyCategoriesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismAssemblyCategoriesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultCategoriesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssemblyResultPerSectionMapContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(GenericFailurePathsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(SpecificFailurePathsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(SpecificFailurePathContext)));
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
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            var project = new RiskeerProject(assessmentSection);

            using (var plugin = new RiskeerPlugin())
            {
                // Call
                IEnumerable<object> childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(project);

                // Assert
                CollectionAssert.AreEquivalent(new[]
                {
                    project.AssessmentSection
                }, childrenWithViewDefinitions);
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
                Assert.AreEqual(10, updateInfos.Length);
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(ForeshoreProfilesContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(GrassCoverSlipOffInwardsFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(GrassCoverSlipOffOutwardsFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(MacroStabilityOutwardsFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(MicrostabilityFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(PipingStructureFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(StrengthStabilityLengthwiseConstructionFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(TechnicalInnovationFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(WaterPressureAsphaltCoverFailureMechanismSectionsContext)));
                Assert.IsTrue(updateInfos.Any(i => i.DataType == typeof(SpecificFailurePathSectionsContext)));
            }
        }

        [Test]
        public void Activate_WithoutGui_ThrowsInvalidOperationException()
        {
            // Setup
            using (var plugin = new RiskeerPlugin())
            {
                // Call
                void Call() => plugin.Activate();

                // Assert
                Assert.Throws<InvalidOperationException>(Call);
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedSpecificFailurePathView_WhenChangingCorrespondingSpecificFailurePathAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var failurePath = new SpecificFailurePath();
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    SpecificFailurePaths =
                    {
                        failurePath
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new SpecificFailurePathContext(failurePath, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<SpecificFailurePathView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, failurePath.Name));

                // When
                const string newName = "Awesome faalpad";
                failurePath.Name = newName;
                failurePath.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, newName));
                mocks.VerifyAll();
            }
        }

        private static void SetPlugins(IPluginsHost gui)
        {
            gui.Plugins.AddRange(new PluginBase[]
            {
                new RiskeerPlugin()
            });
        }

        #region WaterLevelCalculations for norms

        [Test]
        [TestCase(0.01, 0.01, "1/100", 0.1, 0.01, "1/10")]
        [TestCase(0.1, 0.01, "1/10", 0.01, 0.01, "1/100")]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedDesignWaterLevelCalculationsViewForLowerLimitNorm_WhenChangingNormAndObserversNotified_ThenViewTitleUpdated(
            double lowerLimitNorm, double signallingNorm, string originalProbabilityText,
            double newLowerLimitNorm, double newSignallingNorm, string expectedProbabilityText)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    FailureMechanismContribution =
                    {
                        LowerLimitNorm = lowerLimitNorm,
                        SignalingNorm = signallingNorm
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaterLevelCalculationsForNormTargetProbabilityContext(
                                                               assessmentSection.WaterLevelCalculationsForLowerLimitNorm, assessmentSection, () => 0.1));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DesignWaterLevelCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, $"Waterstanden bij norm - {originalProbabilityText}"));

                // When
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                failureMechanismContribution.LowerLimitNorm = newLowerLimitNorm;
                failureMechanismContribution.SignalingNorm = newSignallingNorm;
                failureMechanismContribution.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, $"Waterstanden bij norm - {expectedProbabilityText}"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [TestCase(0.01, 0.01, "1/100 (1)", 0.1, 0.01, "1/100")]
        [TestCase(0.1, 0.01, "1/100", 0.01, 0.01, "1/100 (1)")]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedDesignWaterLevelCalculationsViewForSignallingNorm_WhenChangingNormAndObserversNotified_ThenViewTitleUpdated(
            double lowerLimitNorm, double signallingNorm, string originalProbabilityText,
            double newLowerLimitNorm, double newSignallingNorm, string expectedProbabilityText)
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    FailureMechanismContribution =
                    {
                        LowerLimitNorm = lowerLimitNorm,
                        SignalingNorm = signallingNorm
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaterLevelCalculationsForNormTargetProbabilityContext(
                                                               assessmentSection.WaterLevelCalculationsForSignalingNorm, assessmentSection, () => 0.1));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DesignWaterLevelCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, $"Waterstanden bij norm - {originalProbabilityText}"));

                // When
                FailureMechanismContribution failureMechanismContribution = assessmentSection.FailureMechanismContribution;
                failureMechanismContribution.LowerLimitNorm = newLowerLimitNorm;
                failureMechanismContribution.SignalingNorm = newSignallingNorm;
                failureMechanismContribution.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, $"Waterstanden bij norm - {expectedProbabilityText}"));
                mocks.VerifyAll();
            }
        }

        #endregion

        #region WaterLevelCalculations for target probabilities

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedDesignWaterLevelCalculationsView_WhenChangingCorrespondingUserDefinedTargetProbabilityAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaterLevelCalculationsForUserDefinedTargetProbabilities =
                    {
                        calculationsForTargetProbability
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                                                               calculationsForTargetProbability, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DesignWaterLevelCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Waterstanden bij doelkans - 1/10"));

                // When
                calculationsForTargetProbability.TargetProbability = 0.01;
                calculationsForTargetProbability.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Waterstanden bij doelkans - 1/100"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedWaterLevelCalculationsView_WhenFailureMechanismContributionUpdatedAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                const double targetProbability = 0.1;
                var calculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaterLevelCalculationsForUserDefinedTargetProbabilities =
                    {
                        calculations
                    }
                };

                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                                                               calculations, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DesignWaterLevelCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Waterstanden bij doelkans - 1/10"));

                // When
                assessmentSection.FailureMechanismContribution.LowerLimitNorm = 0.1;
                assessmentSection.FailureMechanismContribution.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Waterstanden bij doelkans - 1/10 (1)"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedWaterLevelCalculationsView_WhenItemInUserDefinedTargetProbabilityCollectionUpdatedAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                const double targetProbability = 0.1;
                var updatedCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var affectedCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaterLevelCalculationsForUserDefinedTargetProbabilities =
                    {
                        updatedCalculations,
                        affectedCalculations
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                                                               affectedCalculations, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DesignWaterLevelCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Waterstanden bij doelkans - 1/10 (1)"));

                // When
                updatedCalculations.TargetProbability = 0.01;
                updatedCalculations.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Waterstanden bij doelkans - 1/10"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedWaterLevelCalculationsView_WhenUserDefinedTargetProbabilityRemovedFromCollectionAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                const double targetProbability = 0.1;
                var removedCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var affectedCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaterLevelCalculationsForUserDefinedTargetProbabilities =
                    {
                        removedCalculations,
                        affectedCalculations
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                                                               affectedCalculations, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DesignWaterLevelCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Waterstanden bij doelkans - 1/10 (1)"));

                // When
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Remove(removedCalculations);
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Waterstanden bij doelkans - 1/10"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedWaterLevelCalculationsView_WhenRemovingDataForOpenedViewAndObserversNotified_ThenNoExceptionThrown()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var calculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaterLevelCalculationsForUserDefinedTargetProbabilities =
                    {
                        calculations
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaterLevelCalculationsForUserDefinedTargetProbabilityContext(
                                                               calculations, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<DesignWaterLevelCalculationsView>(view);

                // When
                assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.Remove(calculations);
                void Call() => assessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                Assert.DoesNotThrow(Call);
                mocks.VerifyAll();
            }
        }

        #endregion

        #region WaveHeightCalculations for target probabilities

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedWaveHeightCalculationsView_WhenChangingCorrespondingUserDefinedTargetProbabilityAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var calculationsForTargetProbability = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaveHeightCalculationsForUserDefinedTargetProbabilities =
                    {
                        calculationsForTargetProbability
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaveHeightCalculationsForUserDefinedTargetProbabilityContext(
                                                               calculationsForTargetProbability, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<WaveHeightCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Golfhoogten bij doelkans - 1/10"));

                // When
                calculationsForTargetProbability.TargetProbability = 0.01;
                calculationsForTargetProbability.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Golfhoogten bij doelkans - 1/100"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedWaveHeightCalculationsView_WhenItemInUserDefinedTargetProbabilityCollectionUpdatedAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                const double targetProbability = 0.1;
                var updatedCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var affectedCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaveHeightCalculationsForUserDefinedTargetProbabilities =
                    {
                        updatedCalculations,
                        affectedCalculations
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaveHeightCalculationsForUserDefinedTargetProbabilityContext(
                                                               affectedCalculations, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<WaveHeightCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Golfhoogten bij doelkans - 1/10 (1)"));

                // When
                updatedCalculations.TargetProbability = 0.01;
                updatedCalculations.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Golfhoogten bij doelkans - 1/10"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedWaveHeightCalculationsView_WhenUserDefinedTargetProbabilityRemovedFromCollectionAndObserversNotified_ThenViewTitleUpdated()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                const double targetProbability = 0.1;
                var removedCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var affectedCalculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(targetProbability);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaveHeightCalculationsForUserDefinedTargetProbabilities =
                    {
                        removedCalculations,
                        affectedCalculations
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaveHeightCalculationsForUserDefinedTargetProbabilityContext(
                                                               affectedCalculations, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<WaveHeightCalculationsView>(view);
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Golfhoogten bij doelkans - 1/10 (1)"));

                // When
                assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Remove(removedCalculations);
                assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                Assert.IsTrue(AvalonDockViewHostTestHelper.IsTitleSet((AvalonDockViewHost) gui.ViewHost, view, "Golfhoogten bij doelkans - 1/10"));
                mocks.VerifyAll();
            }
        }

        [Test]
        [Apartment(ApartmentState.STA)]
        public void GivenPluginWithGuiSetAndOpenedWaveHeightCalculationsView_WhenRemovingDataForOpenedViewAndObserversNotified_ThenNoExceptionThrown()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            var projectMigrator = mocks.Stub<IMigrateProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, projectMigrator, new RiskeerProjectFactory(() => null), new GuiCoreSettings()))
            {
                SetPlugins(gui);
                gui.Run();

                var calculations = new HydraulicBoundaryLocationCalculationsForTargetProbability(0.1);
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
                {
                    WaveHeightCalculationsForUserDefinedTargetProbabilities =
                    {
                        calculations
                    }
                };
                var project = new RiskeerProject(assessmentSection);

                gui.SetProject(project, null);

                gui.DocumentViewController.CloseAllViews();
                gui.DocumentViewController.OpenViewForData(new WaveHeightCalculationsForUserDefinedTargetProbabilityContext(
                                                               calculations, assessmentSection));

                IView view = gui.ViewHost.DocumentViews.First();

                // Precondition
                Assert.IsInstanceOf<WaveHeightCalculationsView>(view);

                // When
                assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.Remove(calculations);
                void Call() => assessmentSection.WaveHeightCalculationsForUserDefinedTargetProbabilities.NotifyObservers();

                // Then
                Assert.DoesNotThrow(Call);
                mocks.VerifyAll();
            }
        }

        #endregion
    }
}