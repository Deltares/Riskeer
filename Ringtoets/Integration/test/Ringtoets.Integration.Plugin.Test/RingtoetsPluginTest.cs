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
using Core.Components.Gis;
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
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.Views;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.StandAlone.SectionResults;
using Ringtoets.Integration.Forms;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Forms.Views.SectionResultViews;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test
{
    [TestFixture]
    public class RingtoetsPluginTest : NUnitFormTest
    {
        [Test]
        [Apartment(ApartmentState.STA)] // For creation of XAML UI component
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
        [Apartment(ApartmentState.STA)] // For creation of XAML UI component
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
                    Action action = () => gui.SetProject(new RingtoetsProject(), null);

                    // Then
                    TestHelper.AssertLogMessagesCount(action, 0);
                }
            }

            mocks.VerifyAll();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        [Apartment(ApartmentState.STA)] // For creation of XAML UI component
        public void GivenPluginWithGuiSet_WhenProjectOnGuiChangesToProjectWithHydraulicBoundaryDatabaseWithExistingLocation_ThenNoWarning()
        {
            // Given
            var mocks = new MockRepository();
            var projectStore = mocks.Stub<IStoreProject>();
            mocks.ReplayAll();

            var testDataDir = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Common.IO, "HydraulicBoundaryDatabaseImporter");
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
                    Action action = () => gui.SetProject(project, null);

                    // Then
                    TestHelper.AssertLogMessagesCount(action, 0);
                }
            }

            mocks.VerifyAll();
            Dispatcher.CurrentDispatcher.InvokeShutdown();
        }

        [Test]
        [Apartment(ApartmentState.STA)] // For creation of XAML UI component
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
                    Action action = () => gui.SetProject(project, null);

                    // Then
                    var fileMissingMessage = string.Format("Fout bij het lezen van bestand '{0}': het bestand bestaat niet.", nonExistingFileExistingFile);
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
                Assert.AreEqual(14, propertyInfos.Length);

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
                    typeof(BackgroundMapDataContainer),
                    typeof(BackgroundMapDataContainerProperties));

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
                    typeof(StandAloneFailureMechanismContextProperties));

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
                    typeof(ProbabilityAssessmentOutput),
                    typeof(ProbabilityAssessmentOutputProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DesignWaterLevelLocationsContext),
                    typeof(DesignWaterLevelLocationsContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(DesignWaterLevelLocationContext),
                    typeof(DesignWaterLevelLocationContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveHeightLocationsContext),
                    typeof(WaveHeightLocationsContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(WaveHeightLocationContext),
                    typeof(WaveHeightLocationContextProperties));

                PluginTestHelper.AssertPropertyInfoDefined(
                    propertyInfos,
                    typeof(ForeshoreProfile),
                    typeof(ForeshoreProfileProperties));
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
                Assert.AreEqual(15, viewInfos.Length);

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismContributionContext),
                    typeof(FailureMechanismContribution),
                    typeof(FailureMechanismContributionView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(DesignWaterLevelLocationsContext),
                    typeof(IEnumerable<HydraulicBoundaryLocation>),
                    typeof(DesignWaterLevelLocationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(WaveHeightLocationsContext),
                    typeof(IEnumerable<HydraulicBoundaryLocation>),
                    typeof(WaveHeightLocationsView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(IAssessmentSection),
                    typeof(AssessmentSectionView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismContext<IFailureMechanism>),
                    typeof(FailureMechanismView<IFailureMechanism>));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>),
                    typeof(IEnumerable<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>),
                    typeof(StrengthStabilityLengthwiseConstructionResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>),
                    typeof(IEnumerable<WaterPressureAsphaltCoverFailureMechanismSectionResult>),
                    typeof(WaterPressureAsphaltCoverResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<MacrostabilityOutwardsFailureMechanismSectionResult>),
                    typeof(IEnumerable<MacrostabilityOutwardsFailureMechanismSectionResult>),
                    typeof(MacrostabilityOutwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<MacrostabilityInwardsFailureMechanismSectionResult>),
                    typeof(IEnumerable<MacrostabilityInwardsFailureMechanismSectionResult>),
                    typeof(MacrostabilityInwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>),
                    typeof(IEnumerable<GrassCoverSlipOffInwardsFailureMechanismSectionResult>),
                    typeof(GrassCoverSlipOffInwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>),
                    typeof(IEnumerable<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>),
                    typeof(GrassCoverSlipOffOutwardsResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>),
                    typeof(IEnumerable<MicrostabilityFailureMechanismSectionResult>),
                    typeof(MicrostabilityResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>),
                    typeof(IEnumerable<PipingStructureFailureMechanismSectionResult>),
                    typeof(PipingStructureResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>),
                    typeof(IEnumerable<TechnicalInnovationFailureMechanismSectionResult>),
                    typeof(TechnicalInnovationResultView));

                PluginTestHelper.AssertViewInfoDefined(
                    viewInfos,
                    typeof(Comment),
                    typeof(CommentView));
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
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(BackgroundMapDataContainer)));
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
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<GrassCoverSlipOffOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<MicrostabilityFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<PipingStructureFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<TechnicalInnovationFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<StrengthStabilityLengthwiseConstructionFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<WaterPressureAsphaltCoverFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<MacrostabilityOutwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(FailureMechanismSectionResultContext<MacrostabilityInwardsFailureMechanismSectionResult>)));
                Assert.IsTrue(treeNodeInfos.Any(tni => tni.TagType == typeof(Comment)));
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
            var assessmentSectionStub = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                // Call
                var childrenWithViewDefinitions = plugin.GetChildDataWithViewDefinitions(assessmentSectionStub);

                // Assert
                CollectionAssert.AreEqual(new object[]
                {
                    assessmentSectionStub.FailureMechanismContribution
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
        public void GetImportInfos_ReturnsSupportedImportInfos()
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
        [Apartment(ApartmentState.STA)] // Due to creating fluent Ribbon
        public void Activate_WithGui_ExpectedProperties()
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
                plugin.Activate();

                // Assert
                Assert.IsInstanceOf<RingtoetsRibbon>(plugin.RibbonCommandHandler);
            }

            mockRepository.VerifyAll();
        }
    }
}