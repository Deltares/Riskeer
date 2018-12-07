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
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ViewHost;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.ClosingStructures.Data.TestUtil;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Data.TestUtil;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionInwards.Data;
using Ringtoets.GrassCoverErosionInwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Data.TestUtil;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HydraRing.Calculation.Calculator.Factory;
using Ringtoets.HydraRing.Calculation.Data.Input;
using Ringtoets.HydraRing.Calculation.Data.Input.Structures;
using Ringtoets.HydraRing.Calculation.TestUtil.Calculator;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PresentationObjects.StandAlone;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Data.TestUtil;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.KernelWrapper.Kernels;
using Ringtoets.MacroStabilityInwards.KernelWrapper.TestUtil.Kernels;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.KernelWrapper.SubCalculator;
using Ringtoets.Piping.KernelWrapper.TestUtil.SubCalculator;
using Ringtoets.StabilityPointStructures.Data.TestUtil;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.WaveImpactAsphaltCover.Data;
using Ringtoets.WaveImpactAsphaltCover.Forms.PresentationObjects;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;
using RingtoetsIntegrationFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Integration.Plugin.Test.TreeNodeInfos
{
    [TestFixture]
    public class AssessmentSectionTreeNodeInfoTest
    {
        private const int contextMenuImportAssessmentSectionIndex = 2;
        private const int contextMenuCalculateAllIndex = 6;
        private MockRepository mocks;
        private readonly string testDataPath = TestHelper.GetTestDataPath(TestDataPath.Ringtoets.Integration.Forms, "HydraulicBoundaryDatabase");

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Setup
            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Assert
                Assert.IsNotNull(info.Text);
                Assert.IsNull(info.ForeColor);
                Assert.IsNotNull(info.Image);
                Assert.IsNotNull(info.ContextMenuStrip);
                Assert.IsNotNull(info.EnsureVisibleOnCreate);
                Assert.IsNotNull(info.ExpandOnCreate);
                Assert.IsNotNull(info.ChildNodeObjects);
                Assert.IsNotNull(info.CanRename);
                Assert.IsNotNull(info.OnNodeRenamed);
                Assert.IsNotNull(info.CanRemove);
                Assert.IsNotNull(info.OnNodeRemoved);
                Assert.IsNull(info.CanCheck);
                Assert.IsNull(info.CheckedState);
                Assert.IsNull(info.OnNodeChecked);
                Assert.IsNull(info.CanDrag);
                Assert.IsNull(info.CanDrop);
                Assert.IsNull(info.CanInsert);
                Assert.IsNull(info.OnDrop);
            }
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            const string testName = "ttt";

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike)
            {
                Name = testName
            };

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                string text = info.Text(assessmentSection);

                // Assert
                Assert.AreEqual(testName, text);
            }
        }

        [Test]
        public void Image_Always_ReturnsSetImage()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                Image image = info.Image(assessmentSection);

                // Assert
                TestHelper.AssertImagesAreEqual(RingtoetsIntegrationFormsResources.AssessmentSectionFolderIcon, image);
            }
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool result = info.EnsureVisibleOnCreate(null, null);

                // Assert
                Assert.IsTrue(result);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ExpandOnCreate_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool result = info.ExpandOnCreate(null);

                // Assert
                Assert.IsTrue(result);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnsChildrenOfData()
        {
            // Setup
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                // Call
                object[] objects = info.ChildNodeObjects(assessmentSection).ToArray();

                // Assert
                Assert.AreEqual(25, objects.Length);

                var referenceLineContext = (ReferenceLineContext) objects[0];
                Assert.AreSame(assessmentSection.ReferenceLine, referenceLineContext.WrappedData);
                Assert.AreSame(assessmentSection, referenceLineContext.AssessmentSection);

                var normContext = (NormContext) objects[1];
                Assert.AreSame(assessmentSection.FailureMechanismContribution, normContext.WrappedData);
                Assert.AreSame(assessmentSection, normContext.AssessmentSection);

                var contributionContext = (FailureMechanismContributionContext) objects[2];
                Assert.AreSame(assessmentSection.FailureMechanismContribution, contributionContext.WrappedData);
                Assert.AreSame(assessmentSection, contributionContext.Parent);

                var context = (HydraulicBoundaryDatabaseContext) objects[3];
                Assert.AreSame(assessmentSection.HydraulicBoundaryDatabase, context.WrappedData);
                Assert.AreSame(assessmentSection, context.AssessmentSection);

                var backgroundData = (BackgroundData) objects[4];
                Assert.AreSame(assessmentSection.BackgroundData, backgroundData);

                var comment = (Comment) objects[5];
                Assert.AreSame(assessmentSection.Comments, comment);

                var pipingFailureMechanismContext = (PipingFailureMechanismContext) objects[6];
                Assert.AreSame(assessmentSection.Piping, pipingFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingFailureMechanismContext.Parent);

                var grassCoverErosionInwardsFailureMechanismContext = (GrassCoverErosionInwardsFailureMechanismContext) objects[7];
                Assert.AreSame(assessmentSection.GrassCoverErosionInwards, grassCoverErosionInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionInwardsFailureMechanismContext.Parent);

                var macroStabilityInwardsFailureMechanismContext = (MacroStabilityInwardsFailureMechanismContext) objects[8];
                Assert.AreSame(assessmentSection.MacroStabilityInwards, macroStabilityInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, macroStabilityInwardsFailureMechanismContext.Parent);

                var macroStabilityOutwardsFailureMechanismContext = (MacroStabilityOutwardsFailureMechanismContext) objects[9];
                Assert.AreSame(assessmentSection.MacroStabilityOutwards, macroStabilityOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, macroStabilityOutwardsFailureMechanismContext.Parent);

                var microstabilityFailureMechanismContext = (MicrostabilityFailureMechanismContext) objects[10];
                Assert.AreSame(assessmentSection.Microstability, microstabilityFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, microstabilityFailureMechanismContext.Parent);

                var stabilityStoneCoverFailureMechanismContext = (StabilityStoneCoverFailureMechanismContext) objects[11];
                Assert.AreSame(assessmentSection.StabilityStoneCover, stabilityStoneCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityStoneCoverFailureMechanismContext.Parent);

                var waveImpactAsphaltCoverFailureMechanismContext = (WaveImpactAsphaltCoverFailureMechanismContext) objects[12];
                Assert.AreSame(assessmentSection.WaveImpactAsphaltCover, waveImpactAsphaltCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, waveImpactAsphaltCoverFailureMechanismContext.Parent);

                var waterPressureAsphaltCoverFailureMechanismContext = (WaterPressureAsphaltCoverFailureMechanismContext) objects[13];
                Assert.AreSame(assessmentSection.WaterPressureAsphaltCover, waterPressureAsphaltCoverFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, waterPressureAsphaltCoverFailureMechanismContext.Parent);

                var grassCoverErosionOutwardsFailureMechanismContext = (GrassCoverErosionOutwardsFailureMechanismContext) objects[14];
                Assert.AreSame(assessmentSection.GrassCoverErosionOutwards, grassCoverErosionOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverErosionOutwardsFailureMechanismContext.Parent);

                var grassCoverSlipOffOutwardsFailureMechanismContext = (GrassCoverSlipOffOutwardsFailureMechanismContext) objects[15];
                Assert.AreSame(assessmentSection.GrassCoverSlipOffOutwards, grassCoverSlipOffOutwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffOutwardsFailureMechanismContext.Parent);

                var grassCoverSlipOffInwardsFailureMechanismContext = (GrassCoverSlipOffInwardsFailureMechanismContext) objects[16];
                Assert.AreSame(assessmentSection.GrassCoverSlipOffInwards, grassCoverSlipOffInwardsFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, grassCoverSlipOffInwardsFailureMechanismContext.Parent);

                var heightStructuresFailureMechanismContext = (HeightStructuresFailureMechanismContext) objects[17];
                Assert.AreSame(assessmentSection.HeightStructures, heightStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, heightStructuresFailureMechanismContext.Parent);

                var closingStructuresFailureMechanismContext = (ClosingStructuresFailureMechanismContext) objects[18];
                Assert.AreSame(assessmentSection.ClosingStructures, closingStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, closingStructuresFailureMechanismContext.Parent);

                var pipingStructureFailureMechanismContext = (PipingStructureFailureMechanismContext) objects[19];
                Assert.AreSame(assessmentSection.PipingStructure, pipingStructureFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, pipingStructureFailureMechanismContext.Parent);

                var stabilityPointStructuresFailureMechanismContext = (StabilityPointStructuresFailureMechanismContext) objects[20];
                Assert.AreSame(assessmentSection.StabilityPointStructures, stabilityPointStructuresFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, stabilityPointStructuresFailureMechanismContext.Parent);

                var strengthStabilityLengthwiseConstructionFailureMechanismContext = (StrengthStabilityLengthwiseConstructionFailureMechanismContext) objects[21];
                Assert.AreSame(assessmentSection.StrengthStabilityLengthwiseConstruction, strengthStabilityLengthwiseConstructionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, strengthStabilityLengthwiseConstructionFailureMechanismContext.Parent);

                var duneErosionFailureMechanismContext = (DuneErosionFailureMechanismContext) objects[22];
                Assert.AreSame(assessmentSection.DuneErosion, duneErosionFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, duneErosionFailureMechanismContext.Parent);

                var technicalInnovationFailureMechanismContext = (TechnicalInnovationFailureMechanismContext) objects[23];
                Assert.AreSame(assessmentSection.TechnicalInnovation, technicalInnovationFailureMechanismContext.WrappedData);
                Assert.AreSame(assessmentSection, technicalInnovationFailureMechanismContext.Parent);

                var assemblyResultsContext = (AssemblyResultsContext) objects[24];
                Assert.AreSame(assessmentSection, assemblyResultsContext.WrappedData);
            }
        }

        [Test]
        public void ContextMenuStrip_Always_CallsBuilder()
        {
            // Setup
            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            using (mocks.Ordered())
            {
                menuBuilder.Expect(mb => mb.AddOpenItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddRenameItem()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
                menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
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
                gui.Stub(g => g.Get(null, treeViewControl)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    // Call
                    info.ContextMenuStrip(null, null, treeViewControl);
                }
            }

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_Always_AddCustomItems()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            {
                var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
                var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

                var gui = mocks.Stub<IGui>();
                gui.Stub(cmp => cmp.Get(assessmentSection, treeView)).Return(menuBuilder);
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    plugin.Gui = gui;

                    // Call
                    using (ContextMenuStrip menu = GetInfo(plugin).ContextMenuStrip(assessmentSection, assessmentSection, treeView))
                    {
                        // Assert
                        Assert.AreEqual(14, menu.Items.Count);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuImportAssessmentSectionIndex,
                                                                      "&Importeren...",
                                                                      "Importeer de gegevens vanuit een bestand.",
                                                                      CoreCommonGuiResources.ImportIcon);

                        TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                                      "Alles be&rekenen",
                                                                      "Voer alle berekeningen binnen dit traject uit.",
                                                                      RingtoetsCommonFormsResources.CalculateAllIcon);
                    }
                }
            }
        }

        [Test]
        public void CanRename_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                bool canRename = info.CanRename(null, null);

                // Assert
                Assert.IsTrue(canRename);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRenamed_WithData_SetProjectNameWithNotification()
        {
            // Setup
            var observer = mocks.Stub<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);
            assessmentSection.Attach(observer);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                // Call
                const string newName = "New Name";
                info.OnNodeRenamed(assessmentSection, newName);

                // Assert
                Assert.AreEqual(newName, assessmentSection.Name);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_Always_ReturnsTrue()
        {
            // Setup
            mocks.ReplayAll();

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);
                // Call
                bool canRemove = info.CanRemove(null, null);

                // Assert
                Assert.IsTrue(canRemove);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void RemoveNodeData_ProjectWithAssessmentSection_ReturnTrueAndRemoveAssessmentSection()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.Dike);

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            var project = new RingtoetsProject();
            project.AssessmentSections.Add(assessmentSection);
            project.Attach(observer);

            using (var plugin = new RingtoetsPlugin())
            {
                TreeNodeInfo info = GetInfo(plugin);

                // Call
                info.OnNodeRemoved(assessmentSection, project);

                // Assert
                CollectionAssert.DoesNotContain(project.AssessmentSections, assessmentSection);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void GivenValidCalculations_WhenCalculatingAllFromContextMenu_ThenAllCalculationsScheduled()
        {
            // Given
            string hydraulicBoundaryDatabaseFilePath = Path.Combine(testDataPath, "HRD dutch coast south.sqlite");
            var assessmentSection = new AssessmentSection(AssessmentSectionComposition.DikeAndDune)
            {
                HydraulicBoundaryDatabase =
                {
                    FilePath = hydraulicBoundaryDatabaseFilePath
                }
            };

            var hydraulicBoundaryLocation = new TestHydraulicBoundaryLocation();
            IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations = new[]
            {
                hydraulicBoundaryLocation
            };
            assessmentSection.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);
            assessmentSection.GrassCoverErosionOutwards.SetHydraulicBoundaryLocationCalculations(hydraulicBoundaryLocations);

            AddGrassCoverErosionInwardsCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddPipingCalculationScenario(assessmentSection, hydraulicBoundaryLocation);
            AddMacroStabilityInwardsCalculationScenario(assessmentSection, hydraulicBoundaryLocation);
            AddStabilityStoneCoverCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddWaveImpactAsphaltCoverCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddGrassCoverErosionOutwardsCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddHeightStructuresCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddClosingStructuresCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddStabilityPointStructuresCalculation(assessmentSection, hydraulicBoundaryLocation);
            AddDuneLocationCalculation(assessmentSection);

            var calculatorFactory = mocks.StrictMock<IHydraRingCalculatorFactory>();

            using (mocks.Ordered())
            {
                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestDesignWaterLevelCalculator
                                 {
                                     DesignWaterLevel = 2.0
                                 }).Repeat.Times(4);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, ""))
                                 .Return(new TestWaveHeightCalculator()).Repeat.Times(4);

                calculatorFactory.Expect(cf => cf.CreateOvertoppingCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestOvertoppingCalculator());

                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(9);

                calculatorFactory.Expect(cf => cf.CreateDesignWaterLevelCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestDesignWaterLevelCalculator()).Repeat.Times(3);
                calculatorFactory.Expect(cf => cf.CreateWaveHeightCalculator(testDataPath, ""))
                                 .Return(new TestWaveHeightCalculator()).Repeat.Times(3);

                calculatorFactory.Expect(cf => cf.CreateWaveConditionsCosineCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(hydraulicBoundaryDatabaseFilePath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestWaveConditionsCosineCalculator()).Repeat.Times(3);

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresOvertoppingCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresOvertoppingCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresClosureCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresClosureCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateStructuresCalculator<StructuresStabilityPointCalculationInput>(
                                             Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestStructuresCalculator<StructuresStabilityPointCalculationInput>());

                calculatorFactory.Expect(cf => cf.CreateDunesBoundaryConditionsCalculator(Arg<HydraRingCalculationSettings>.Is.NotNull))
                                 .WhenCalled(invocation =>
                                 {
                                     var settings = (HydraRingCalculationSettings) invocation.Arguments[0];
                                     Assert.AreEqual(testDataPath, settings.HlcdFilePath);
                                     Assert.IsEmpty(settings.PreprocessorDirectory);
                                 })
                                 .Return(new TestDunesBoundaryConditionsCalculator()).Repeat.Times(5);
            }

            using (var treeViewControl = new TreeViewControl())
            {
                var gui = mocks.Stub<IGui>();
                gui.Stub(g => g.MainWindow).Return(mocks.Stub<IMainWindow>());
                gui.Stub(g => g.ProjectOpened += null).IgnoreArguments();
                gui.Stub(g => g.ProjectOpened -= null).IgnoreArguments();
                gui.Stub(cmp => cmp.Get(assessmentSection, treeViewControl)).Return(new CustomItemsOnlyContextMenuBuilder());
                gui.Stub(g => g.DocumentViewController).Return(mocks.Stub<IDocumentViewController>());

                mocks.ReplayAll();

                using (var plugin = new RingtoetsPlugin())
                {
                    TreeNodeInfo info = GetInfo(plugin);
                    plugin.Gui = gui;

                    using (ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(assessmentSection, null, treeViewControl))
                    using (new HydraRingCalculatorFactoryConfig(calculatorFactory))
                    using (new PipingSubCalculatorFactoryConfig())
                    using (new MacroStabilityInwardsKernelFactoryConfig())
                    {
                        var pipingTestFactory = (TestPipingSubCalculatorFactory) PipingSubCalculatorFactory.Instance;
                        var macroStabilityTestFactory = (TestMacroStabilityInwardsKernelFactory) MacroStabilityInwardsKernelWrapperFactory.Instance;

                        // Precondition
                        Assert.IsFalse(pipingTestFactory.LastCreatedUpliftCalculator.Calculated);
                        Assert.IsFalse(macroStabilityTestFactory.LastCreatedUpliftVanKernel.Calculated);

                        // When
                        contextMenuAdapter.Items[contextMenuCalculateAllIndex].PerformClick();

                        // Then
                        Assert.IsTrue(pipingTestFactory.LastCreatedUpliftCalculator.Calculated);
                        Assert.IsTrue(macroStabilityTestFactory.LastCreatedUpliftVanKernel.Calculated);
                    }
                }

                mocks.VerifyAll();
            }
        }

        private TreeNodeInfo GetInfo(RingtoetsPlugin plugin)
        {
            return plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(AssessmentSection));
        }

        private static void AddGrassCoverErosionInwardsCalculation(AssessmentSection assessmentSection,
                                                                   HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.GrassCoverErosionInwards.CalculationsGroup.Children.Add(new GrassCoverErosionInwardsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    DikeProfile = DikeProfileTestFactory.CreateDikeProfile()
                }
            });
        }

        private static void AddPipingCalculationScenario(AssessmentSection assessmentSection,
                                                         HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            PipingCalculationScenario pipingCalculationScenario = PipingCalculationScenarioTestFactory.CreatePipingCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
            pipingCalculationScenario.InputParameters.UseAssessmentLevelManualInput = true;
            pipingCalculationScenario.InputParameters.AssessmentLevel = new Random(39).NextRoundedDouble();
            assessmentSection.Piping.CalculationsGroup.Children.Add(pipingCalculationScenario);
        }

        private static void AddMacroStabilityInwardsCalculationScenario(AssessmentSection assessmentSection,
                                                                        HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario =
                MacroStabilityInwardsCalculationScenarioTestFactory.CreateMacroStabilityInwardsCalculationScenarioWithValidInput(hydraulicBoundaryLocation);
            macroStabilityInwardsCalculationScenario.InputParameters.UseAssessmentLevelManualInput = true;
            macroStabilityInwardsCalculationScenario.InputParameters.AssessmentLevel = new Random(39).NextRoundedDouble();
            assessmentSection.MacroStabilityInwards.CalculationsGroup.Children.Add(macroStabilityInwardsCalculationScenario);
        }

        private static void AddStabilityStoneCoverCalculation(AssessmentSection assessmentSection,
                                                              HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.StabilityStoneCover.WaveConditionsCalculationGroup.Children.Add(new StabilityStoneCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = AssessmentSectionCategoryType.FactorizedLowerLimitNorm,
                    ForeshoreProfile = new TestForeshoreProfile(true),
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3,
                    Orientation = (RoundedDouble) 10
                }
            });
        }

        private static void AddWaveImpactAsphaltCoverCalculation(AssessmentSection assessmentSection,
                                                                 HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.WaveImpactAsphaltCover.WaveConditionsCalculationGroup.Children.Add(new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = AssessmentSectionCategoryType.FactorizedLowerLimitNorm,
                    UseForeshore = true,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3,
                    Orientation = (RoundedDouble) 10
                }
            });
        }

        private static void AddGrassCoverErosionOutwardsCalculation(AssessmentSection assessmentSection,
                                                                    HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.GrassCoverErosionOutwards.WaveConditionsCalculationGroup.Children.Add(new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation,
                    CategoryType = FailureMechanismCategoryType.FactorizedLowerLimitNorm,
                    UseBreakWater = true,
                    LowerBoundaryRevetment = (RoundedDouble) 1,
                    UpperBoundaryRevetment = (RoundedDouble) 3,
                    LowerBoundaryWaterLevels = (RoundedDouble) 1,
                    UpperBoundaryWaterLevels = (RoundedDouble) 3,
                    Orientation = (RoundedDouble) 10
                }
            });
        }

        private static void AddHeightStructuresCalculation(AssessmentSection assessmentSection,
                                                           HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.HeightStructures.CalculationsGroup.Children.Add(new TestHeightStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
        }

        private static void AddClosingStructuresCalculation(AssessmentSection assessmentSection,
                                                            HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.ClosingStructures.CalculationsGroup.Children.Add(new TestClosingStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
        }

        private static void AddStabilityPointStructuresCalculation(AssessmentSection assessmentSection,
                                                                   HydraulicBoundaryLocation hydraulicBoundaryLocation)
        {
            assessmentSection.StabilityPointStructures.CalculationsGroup.Children.Add(new TestStabilityPointStructuresCalculation
            {
                InputParameters =
                {
                    HydraulicBoundaryLocation = hydraulicBoundaryLocation
                }
            });
        }

        private static void AddDuneLocationCalculation(AssessmentSection assessmentSection)
        {
            var duneLocation = new TestDuneLocation();
            assessmentSection.DuneErosion.SetDuneLocations(new[]
            {
                duneLocation
            });
        }
    }
}