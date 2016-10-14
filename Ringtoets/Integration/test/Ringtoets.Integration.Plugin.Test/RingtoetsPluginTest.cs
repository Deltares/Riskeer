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
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
using Ringtoets.Common.Data.DikeProfiles;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Forms.Views.SectionResultViews;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsPluginTest : NUnitFormTest
    {
        [Test]
        [STAThread] // For creation of XAML UI component
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
        [STAThread] // For creation of XAML UI component
        public void GivenPluginWithGuiSet_WhenProjectOnGuiChangesToProjectWithoutHydraulicBoundaryDatabase_ThenNoWarning()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;
                    gui.Run();

                    // When
                    Action action = () => gui.Project = new RingtoetsProject();

                    // Then
                    TestHelper.AssertLogMessagesCount(action, 0);
                }
            }

            mocks.VerifyAll();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        [STAThread] // For creation of XAML UI component
        public void GivenPluginWithGuiSet_WhenProjectOnGuiChangesToProjectWithHydraulicBoundaryDatabaseWithExistingLocation_ThenNoWarning()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var testDataDir = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.HydraRing.IO, "HydraulicBoundaryLocationReader");
            var testFilePath = Path.Combine(testDataDir, "complete.sqlite");

            using (var gui = new GuiCore(new MainWindow(), projectStore, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;
                    gui.Run();

                    var project = new RingtoetsProject();
                    var section = new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                        {
                            FilePath = testFilePath
                        }
                    };
                    project.AssessmentSections.Add(section);

                    // When
                    Action action = () => { gui.Project = project; };

                    // Then
                    TestHelper.AssertLogMessagesCount(action, 0);
                }
            }

            mocks.VerifyAll();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        [STAThread] // For creation of XAML UI component
        public void GivenPluginWithGuiSet_WhenProjectOnGuiChangesToProjectWithHydraulicBoundaryDatabaseWithNonExistingLocation_ThenWarning()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            using (var gui = new GuiCore(new MainWindow(), projectStore, new RingtoetsProjectFactory(), new GuiCoreSettings()))
            {
                using (var plugin = new RingtoetsPlugin())
                {
                    var project = new RingtoetsProject();
                    const string nonExistingFileExistingFile = "not_existing_file";

                    var section = new AssessmentSection(AssessmentSectionComposition.Dike)
                    {
                        HydraulicBoundaryDatabase = new HydraulicBoundaryDatabase
                        {
                            FilePath = nonExistingFileExistingFile
                        }
                    };
                    project.AssessmentSections.Add(section);

                    plugin.Gui = gui;
                    gui.Run();

                    // When
                    Action action = () => { gui.Project = project; };

                    // Then
                    var fileMissingMessage = string.Format("Fout bij het lezen van bestand '{0}': Het bestand bestaat niet.", nonExistingFileExistingFile);
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
        public void GetPropertyInfos_ReturnsSupportedPropertyClassesWithExpectedValues()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                PropertyInfo[] propertyInfos = plugin.GetPropertyInfos().ToArray();

                // Assert
                Assert.AreEqual(12, propertyInfos.Length);

                PropertyInfo ringtoetsProjectProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IProject),
                    typeof(RingtoetsProjectProperties));
                Assert.IsNull(ringtoetsProjectProperties.AdditionalDataCheck);
                Assert.IsNull(ringtoetsProjectProperties.GetObjectPropertiesData);
                Assert.IsNull(ringtoetsProjectProperties.AfterCreate);

                PropertyInfo assessmentSectionProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(IAssessmentSection),
                    typeof(AssessmentSectionProperties));
                Assert.IsNull(assessmentSectionProperties.AdditionalDataCheck);
                Assert.IsNull(assessmentSectionProperties.GetObjectPropertiesData);
                Assert.IsNull(assessmentSectionProperties.AfterCreate);

                PropertyInfo hydraulicBoundaryDatabaseProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(HydraulicBoundaryDatabaseContext),
                    typeof(HydraulicBoundaryDatabaseProperties));
                Assert.IsNull(hydraulicBoundaryDatabaseProperties.AdditionalDataCheck);
                Assert.IsNull(hydraulicBoundaryDatabaseProperties.GetObjectPropertiesData);
                Assert.IsNull(hydraulicBoundaryDatabaseProperties.AfterCreate);

                PropertyInfo standAloneFailureMechanismProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(FailureMechanismContext<IFailureMechanism>),
                    typeof(StandAloneFailureMechanismContextProperties));
                Assert.IsNull(standAloneFailureMechanismProperties.AdditionalDataCheck);
                Assert.IsNull(standAloneFailureMechanismProperties.GetObjectPropertiesData);
                Assert.IsNull(standAloneFailureMechanismProperties.AfterCreate);

                PropertyInfo calculationGroupProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ICalculationContext<CalculationGroup, IFailureMechanism>),
                    typeof(CalculationGroupContextProperties));
                Assert.IsNull(calculationGroupProperties.AdditionalDataCheck);
                Assert.IsNull(calculationGroupProperties.GetObjectPropertiesData);
                Assert.IsNull(calculationGroupProperties.AfterCreate);

                PropertyInfo calculationContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ICalculationContext<ICalculation, IFailureMechanism>),
                    typeof(CalculationContextProperties));
                Assert.IsNull(calculationContextProperties.AdditionalDataCheck);
                Assert.IsNull(calculationContextProperties.GetObjectPropertiesData);
                Assert.IsNull(calculationContextProperties.AfterCreate);

                PropertyInfo outputContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ProbabilityAssessmentOutput),
                    typeof(ProbabilityAssessmentOutputProperties));
                Assert.IsNull(outputContextProperties.AdditionalDataCheck);
                Assert.IsNull(outputContextProperties.GetObjectPropertiesData);
                Assert.IsNull(outputContextProperties.AfterCreate);

                PropertyInfo designWaterLevelLocationsContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DesignWaterLevelLocationsContext),
                    typeof(DesignWaterLevelLocationsContextProperties));
                Assert.IsNull(designWaterLevelLocationsContextProperties.AdditionalDataCheck);
                Assert.IsNotNull(designWaterLevelLocationsContextProperties.GetObjectPropertiesData);
                Assert.IsNull(designWaterLevelLocationsContextProperties.AfterCreate);

                PropertyInfo designWaterLevelLocationContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DesignWaterLevelLocationContext),
                    typeof(DesignWaterLevelLocationContextProperties));
                Assert.IsNull(designWaterLevelLocationContextProperties.AdditionalDataCheck);
                Assert.IsNull(designWaterLevelLocationContextProperties.GetObjectPropertiesData);
                Assert.IsNull(designWaterLevelLocationContextProperties.AfterCreate);

                PropertyInfo waveHeightLocationsContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveHeightLocationsContext),
                    typeof(WaveHeightLocationsContextProperties));
                Assert.IsNull(waveHeightLocationsContextProperties.AdditionalDataCheck);
                Assert.IsNotNull(waveHeightLocationsContextProperties.GetObjectPropertiesData);
                Assert.IsNull(waveHeightLocationsContextProperties.AfterCreate);

                PropertyInfo waveHeightLocationContextProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveHeightLocationContext),
                    typeof(WaveHeightLocationContextProperties));
                Assert.IsNull(waveHeightLocationContextProperties.AdditionalDataCheck);
                Assert.IsNull(waveHeightLocationContextProperties.GetObjectPropertiesData);
                Assert.IsNull(waveHeightLocationContextProperties.AfterCreate);

                PropertyInfo foreshoreProfileProperties = PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ForeshoreProfile),
                    typeof(ForeshoreProfileProperties));
                Assert.IsNull(foreshoreProfileProperties.AdditionalDataCheck);
                Assert.IsNull(foreshoreProfileProperties.GetObjectPropertiesData);
                Assert.IsNull(foreshoreProfileProperties.AfterCreate);
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
                var referenceLineExportInfo = exportInfos.Single(ei => ei.DataType == typeof(ReferenceLineContext));
                Assert.IsNull(referenceLineExportInfo.Name);
                Assert.IsNull(referenceLineExportInfo.Image);
                Assert.IsNull(referenceLineExportInfo.Category);
                var hydraulicBoundaryExportInfo = exportInfos.Single(ei => ei.DataType == typeof(HydraulicBoundaryDatabaseContext));
                Assert.IsNull(hydraulicBoundaryExportInfo.Name);
                Assert.IsNull(hydraulicBoundaryExportInfo.Image);
                Assert.IsNull(hydraulicBoundaryExportInfo.Category);
            }
        }

        [Test]
        public void GetViewInfos_ReturnsSupportedViewInfoClasses()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                ViewInfo[] viewInfos = plugin.GetViewInfos().ToArray();

                // Assert
                Assert.AreEqual(15, viewInfos.Length);

                var contributionViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismContributionContext));
                Assert.AreEqual(typeof(FailureMechanismContributionView), contributionViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismContributionIcon, contributionViewInfo.Image);

                var designWaterLevelLocationsViewInfo = viewInfos.Single(vi => vi.DataType == typeof(DesignWaterLevelLocationsContext));
                Assert.AreEqual(typeof(DesignWaterLevelLocationsView), designWaterLevelLocationsViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, designWaterLevelLocationsViewInfo.Image);

                var waveHeightLocationsViewInfo = viewInfos.Single(vi => vi.DataType == typeof(WaveHeightLocationsContext));
                Assert.AreEqual(typeof(WaveHeightLocationsView), waveHeightLocationsViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.GenericInputOutputIcon, waveHeightLocationsViewInfo.Image);

                var mapViewInfo = viewInfos.Single(vi => vi.DataType == typeof(IAssessmentSection));
                Assert.AreEqual(typeof(AssessmentSectionView), mapViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.Map, mapViewInfo.Image);

                var strengthStabilityLengthwiseConstructionResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>), strengthStabilityLengthwiseConstructionResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(StrengthStabilityLengthwiseConstructionResultView), strengthStabilityLengthwiseConstructionResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, strengthStabilityLengthwiseConstructionResultViewInfo.Image);

                var waterPressureAsphaltCoverResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult>), waterPressureAsphaltCoverResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(WaterPressureAsphaltCoverResultView), waterPressureAsphaltCoverResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, waterPressureAsphaltCoverResultViewInfo.Image);

                var macrostabilityOutwardsResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<MacrostabilityOutwardsFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult>), macrostabilityOutwardsResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(MacrostabilityOutwardsResultView), macrostabilityOutwardsResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, macrostabilityOutwardsResultViewInfo.Image);

                var macrostabilityInwardsResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<MacrostabilityInwardsFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult>), macrostabilityInwardsResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(MacrostabilityInwardsResultView), macrostabilityInwardsResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, macrostabilityInwardsResultViewInfo.Image);

                var duneErosionResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<DuneErosionFailureMechanismSectionResult>), duneErosionResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(DuneErosionResultView), duneErosionResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, duneErosionResultViewInfo.Image);

                var grassCoverSlipOffInwardsResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult>), grassCoverSlipOffInwardsResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(GrassCoverSlipOffInwardsResultView), grassCoverSlipOffInwardsResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, grassCoverSlipOffInwardsResultViewInfo.Image);

                var grassCoverSlipOffOutwardsResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>), grassCoverSlipOffOutwardsResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(GrassCoverSlipOffOutwardsResultView), grassCoverSlipOffOutwardsResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, grassCoverSlipOffOutwardsResultViewInfo.Image);

                var microstabilityResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<MicrostabilityFailureMechanismSectionResult>), microstabilityResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(MicrostabilityResultView), microstabilityResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, microstabilityResultViewInfo.Image);

                var pipingStructureResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<PipingStructureFailureMechanismSectionResult>), pipingStructureResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(PipingStructureResultView), pipingStructureResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, pipingStructureResultViewInfo.Image);

                var technicalInnovationResultViewInfo = viewInfos.Single(vi => vi.DataType == typeof(FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>));
                Assert.AreEqual(typeof(IEnumerable<TechnicalInnovationFailureMechanismSectionResult>), technicalInnovationResultViewInfo.ViewDataType);
                Assert.AreEqual(typeof(TechnicalInnovationResultView), technicalInnovationResultViewInfo.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon, technicalInnovationResultViewInfo.Image);

                var commentView = viewInfos.Single(vi => vi.DataType == typeof(CommentContext<ICommentable>));
                Assert.AreEqual(typeof(CommentView), commentView.ViewType);
                TestHelper.AssertImagesAreEqual(RingtoetsCommonFormsResources.EditDocumentIcon, commentView.Image);
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
                Assert.AreEqual(26, treeNodeInfos.Length);
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(AssessmentSection)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ReferenceLineContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismContext<IFailureMechanism>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CategoryTreeFolder)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismContributionContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(HydraulicBoundaryDatabaseContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DesignWaterLevelLocationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(WaveHeightLocationsContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ForeshoreProfilesContext)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(DikeProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ForeshoreProfile)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<MacrostabilityOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<MacrostabilityInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(CommentContext<ICommentable>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(ProbabilityAssessmentOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(EmptyProbabilityAssessmentOutput)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(RingtoetsProject)));
            }
        }

        [Test]
        public void GetChildDataWithViewDefinitions_AssessmentSection_ReturnFailureMechanismContribution()
        {
            // Setup
            var mocks = new MockRepository();
            var assessmentSectionMock = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                var childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(assessmentSectionMock);

                // Assert
                CollectionAssert.AreEqual(new object[]
                {
                    assessmentSectionMock.FailureMechanismContribution
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
                var childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(project);

                // Assert
                var expectedResult = project.AssessmentSections;
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
                var childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(1);

                // Assert
                CollectionAssert.IsEmpty(childrenWithViewDefinitions);
            }
        }

        [Test]
        public void GetImportInfos_ReturnsExpectedImportInfos()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                ImportInfo[] importInfos = plugin.GetImportInfos().ToArray();

                // Assert
                Assert.AreEqual(4, importInfos.Length);
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ReferenceLineContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(FailureMechanismSectionsContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(ForeshoreProfilesContext)));
                Assert.IsTrue(importInfos.Any(i => i.DataType == typeof(DikeProfilesContext)));
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
        [STAThread] // Due to creating fluent Ribbon
        public void Activate_WithGui_DoesNotThrowException()
        {
            // Setup
            var mockRepository = new MockRepository();
            var mainWindowMock = mockRepository.StrictMock<IMainWindow>();
            var documentViewControllerMock = mockRepository.StrictMock<IDocumentViewController>();
            var guiMock = mockRepository.StrictMock<IGui>();
            guiMock.Expect(g => g.MainWindow).Return(mainWindowMock).Repeat.AtLeastOnce();
            guiMock.Expect(g => g.DocumentViewController).Return(documentViewControllerMock);
            guiMock.Expect(g => g.ProjectOpened += null).IgnoreArguments();
            guiMock.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
            mockRepository.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                plugin.Gui = guiMock;

                // Call
                TestDelegate test = () => plugin.Activate();

                // Assert
                Assert.DoesNotThrow(test);
                Assert.IsInstanceOf<RingtoetsRibbon>(plugin.RibbonCommandHandler);
            }

            mockRepository.VerifyAll();
        }
    }
}