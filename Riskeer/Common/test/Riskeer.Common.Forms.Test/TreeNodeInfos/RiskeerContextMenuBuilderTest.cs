// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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
// along with this program. If not, see <https://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using Core.Gui.Commands;
using Core.Gui.ContextMenu;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using NSubstitute;
using NUnit.Framework;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TestUtil;
using Riskeer.Common.Forms.TreeNodeInfos;
using RiskeerFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RiskeerContextMenuBuilderTest
    {
        [Test]
        public void AddCreateCalculationGroupItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            using (var treeViewControl = new TreeViewControl())
            {
                var calculationGroup = new CalculationGroup();
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddCreateCalculationGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Map toevoegen",
                                                              "Voeg een nieuwe map toe aan deze map met berekeningen.",
                                                              RiskeerFormsResources.AddFolderIcon);
            }
        }

        [Test]
        [TestCaseSource(typeof(CalculationTypeTestHelper), nameof(CalculationTypeTestHelper.CalculationTypeWithImageCases))]
        public void AddCreateCalculationItem_WhenBuild_ItemAddedToContextMenu(CalculationType calculationType, Bitmap expectedImage)
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            using (var treeViewControl = new TreeViewControl())
            {
                var parent = new CalculationGroup();
                var calculationGroup = new CalculationGroup();
                var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddCreateCalculationItem(calculationGroupContext, context => {}, calculationType).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Berekening &toevoegen",
                                                              "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                              expectedImage);
            }
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculationWithOutput = Substitute.For<ICalculation>();

            calculationWithOutput.HasOutput.Returns(true);
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutput
                }
            };
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup, treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddClearAllCalculationOutputInGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen deze map met berekeningen.",
                                                              RiskeerFormsResources.ClearIcon);
            }
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculationGroup = new CalculationGroup();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddClearAllCalculationOutputInGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis alle uitvoer...",
                                                              "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                              RiskeerFormsResources.ClearIcon,
                                                              false);
            }
        }

        [Test]
        public void AddClearAllCalculationOutputInFailureMechanismItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculationWithOutput = Substitute.For<ICalculation>();
            calculationWithOutput.HasOutput.Returns(true);
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(new[]
            {
                calculationWithOutput
            });
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanism,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddClearAllCalculationOutputInFailureMechanismItem(failureMechanism).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis alle uitvoer...",
                                                              "Wis de uitvoer van alle berekeningen binnen dit faalmechanisme.",
                                                              RiskeerFormsResources.ClearIcon);
            }
        }

        [Test]
        public void AddClearAllCalculationOutputInFailureMechanismItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            failureMechanism.Calculations.Returns(new List<ICalculation>());
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanism,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddClearAllCalculationOutputInFailureMechanismItem(failureMechanism).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis alle uitvoer...",
                                                              "Er zijn geen berekeningen met uitvoer om te wissen.",
                                                              RiskeerFormsResources.ClearIcon,
                                                              false);
            }
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddToggleInAssemblyOfFailureMechanismItem_WhenBuild_ItemAddedToContextMenuEnabled(bool inAssembly)
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = Substitute.For<IFailureMechanism>();
            failureMechanism.InAssembly.Returns(inAssembly);
            var failureMechanismContext = Substitute.For<IFailureMechanismContext<IFailureMechanism>>();
            failureMechanismContext.WrappedData.Returns(failureMechanism);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanism,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddToggleInAssemblyOfFailureMechanismItem(failureMechanismContext, null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                Bitmap checkboxIcon = inAssembly ? RiskeerFormsResources.Checkbox_ticked : RiskeerFormsResources.Checkbox_empty;
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "In &assemblage",
                                                              "Geeft aan of dit faalmechanisme wordt meegenomen in de assemblage.",
                                                              checkboxIcon);
            }
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculationWithOutput = Substitute.For<ICalculation>();

            calculationWithOutput.HasOutput.Returns(true);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationWithOutput,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddClearCalculationOutputItem(calculationWithOutput).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis uitvoer...",
                                                              "Wis de uitvoer van deze berekening.",
                                                              RiskeerFormsResources.ClearIcon);
            }
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithoutOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculationWithoutOutput = Substitute.For<ICalculation>();

            calculationWithoutOutput.HasOutput.Returns(false);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationWithoutOutput,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddClearCalculationOutputItem(calculationWithoutOutput).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Wis uitvoer...",
                                                              "Deze berekening heeft geen uitvoer om te wissen.",
                                                              RiskeerFormsResources.ClearIcon,
                                                              false);
            }
        }

        [Test]
        public void AddRenameItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call    
            riskeerContextMenuBuilder.AddRenameItem();

            // Assert
            contextMenuBuilder.Received().AddRenameItem();
        }

        [Test]
        public void AddDeleteItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddDeleteItem();

            // Assert
            contextMenuBuilder.Received().AddDeleteItem();
        }

        [Test]
        public void AddExpandAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddExpandAllItem();

            // Assert
            contextMenuBuilder.Received().AddExpandAllItem();
        }

        [Test]
        public void AddCollapseAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var menuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(menuBuilder);

            // Call
            riskeerContextMenuBuilder.AddCollapseAllItem();

            // Assert
            menuBuilder.Received().AddCollapseAllItem();
        }

        [Test]
        public void AddOpenItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddOpenItem();

            // Assert
            contextMenuBuilder.Received().AddOpenItem();
        }

        [Test]
        public void AddExportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddExportItem();

            // Assert
            contextMenuBuilder.Received().AddExportItem();
        }

        [Test]
        public void AddImportItemWithoutParameters_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddImportItem();

            // Assert
            contextMenuBuilder.Received().AddImportItem();
        }

        [Test]
        public void AddImportItemWithImportInfosParameter_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            IEnumerable<ImportInfo> importInfos = Enumerable.Empty<ImportInfo>();
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddImportItem(importInfos);

            // Assert
            contextMenuBuilder.Received().AddImportItem(importInfos);
        }

        [Test]
        public void AddImportItemWithTextualParameters_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            const string text = "import";
            const string toolTip = "import tooltip";
            Bitmap image = RiskeerFormsResources.DatabaseIcon;
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddImportItem(text, toolTip, image);

            // Assert
            contextMenuBuilder.Received().AddImportItem(text, toolTip, image);
        }

        [Test]
        public void AddImportItemWithAllParameters_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            const string text = "import";
            const string toolTip = "import tooltip";
            Bitmap image = RiskeerFormsResources.DatabaseIcon;
            IEnumerable<ImportInfo> importInfos = Enumerable.Empty<ImportInfo>();
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddImportItem(text, toolTip, image, importInfos);

            // Assert
            contextMenuBuilder.Received().AddImportItem(text, toolTip, image, importInfos);
        }

        [Test]
        public void AddPropertiesItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddPropertiesItem();

            // Assert
            contextMenuBuilder.Received().AddPropertiesItem();
        }

        [Test]
        public void AddSeparator_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddSeparator();

            // Assert
            contextMenuBuilder.Received().AddSeparator();
        }

        [Test]
        public void AddCustomItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuItem = new StrictContextMenuItem("Custom Text", "Custom Tooltip", null, null);
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddCustomItem(contextMenuItem);

            // Assert
            contextMenuBuilder.Received().AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item.Name == contextMenuItem.Name));
        }

        [Test]
        public void Build_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.Build();

            // Assert
            contextMenuBuilder.Received().Build();
        }

        #region AddUpdateForeshoreProfileOfCalculationItem

        [Test]
        [Combinatorial]
        public void AddUpdateForeshoreProfileOfCalculationItem_ForeshoreProfileStates_ItemAddedToContextMenuAsExpected(
            [Values(true, false)] bool hasForeshoreProfile,
            [Values(true, false)] bool isSynchronized)
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = Substitute.For<ICalculationInputWithForeshoreProfile>();

            if (hasForeshoreProfile)
            {
                input.ForeshoreProfile.Returns(new TestForeshoreProfile());
                input.IsForeshoreProfileInputSynchronized.Returns(isSynchronized);
            }
            else
            {
                input.ForeshoreProfile.Returns((TestForeshoreProfile) null);
            }

            calculation.InputParameters.Returns(input);
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculation,
                                                                treeViewControl);

                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddUpdateForeshoreProfileOfCalculationItem(
                    calculation,
                    inquiryHelper,
                    c => {}).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                string tooltip;
                if (hasForeshoreProfile)
                {
                    tooltip = isSynchronized
                                  ? "Er zijn geen wijzigingen om bij te werken."
                                  : "Berekening bijwerken met het voorlandprofiel.";
                }
                else
                {
                    tooltip = "Er moet een voorlandprofiel geselecteerd zijn.";
                }

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Bijwerken voorlandprofiel...",
                                                              tooltip,
                                                              RiskeerFormsResources.UpdateItemIcon,
                                                              hasForeshoreProfile && !isSynchronized);
            }
        }

        #endregion

        #region AddUpdateForeshoreProfileOfCalculationsItem

        [Test]
        [Combinatorial]
        public void AddUpdateForeshoreProfileOfCalculationsItem_ForeshoreProfileStates_ItemAddedToContextMenuAsExpected(
            [Values(true, false)] bool hasForeshoreProfile,
            [Values(true, false)] bool isSynchronized)
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();

            var calculation = Substitute.For<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = Substitute.For<ICalculationInputWithForeshoreProfile>();
            if (hasForeshoreProfile)
            {
                input.ForeshoreProfile.Returns(new TestForeshoreProfile());
                input.IsForeshoreProfileInputSynchronized.Returns(isSynchronized);
            }
            else
            {
                input.ForeshoreProfile.Returns((TestForeshoreProfile) null);
            }

            calculation.InputParameters.Returns(input);
            var inquiryHelper = Substitute.For<IInquiryHelper>();
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculation,
                                                                treeViewControl);

                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddUpdateForeshoreProfileOfCalculationsItem(
                    new[]
                    {
                        calculation
                    },
                    inquiryHelper,
                    c => {}).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                string tooltip = hasForeshoreProfile && !isSynchronized
                                     ? "Alle berekeningen met een voorlandprofiel bijwerken."
                                     : "Er zijn geen berekeningen om bij te werken.";

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Bijwerken voorlandprofielen...",
                                                              tooltip,
                                                              RiskeerFormsResources.UpdateItemIcon,
                                                              hasForeshoreProfile && !isSynchronized);
            }
        }

        #endregion

        #region AddDuplicateCalculationItem

        [Test]
        public void AddDuplicateCalculationItem_WhenBuildWithCalculationItem_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculationItem = Substitute.For<ICalculationBase>();
            var calculationItemContext = Substitute.For<ICalculationContext<ICalculationBase, ICalculatableFailureMechanism>>();
            calculationItemContext.Parent.Returns(new CalculationGroup());
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationItem,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddDuplicateCalculationItem(calculationItem, calculationItemContext).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "D&upliceren",
                                                              "Dupliceer dit element.",
                                                              RiskeerFormsResources.CopyHS);
            }
        }

        #endregion

        #region AddClearIllustrationPointsOfCalculationsItem

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddClearIllustrationPointsOfCalculationsItem_EnabledSituation_ItemAddedToContextMenuAsExpected(bool isEnabled)
        {
            // Setup
            string expectedToolTipMessage = isEnabled
                                                ? "Wis alle berekende illustratiepunten."
                                                : "Er zijn geen berekeningen met illustratiepunten om te wissen.";
            var changeHandler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            contextMenuBuilder.When(x => x.AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item != null)))
                              .Do(callinfo =>
                              {
                                  var contextMenuItem = callinfo.Arg<StrictContextMenuItem>();
                                  Assert.AreEqual("Wis alle &illustratiepunten...", contextMenuItem.Text);
                                  Assert.AreEqual(expectedToolTipMessage, contextMenuItem.ToolTipText);
                                  Assert.AreEqual(isEnabled, contextMenuItem.Enabled);
                              });
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddClearIllustrationPointsOfCalculationsItem(() => isEnabled, changeHandler);

            // Assert
            contextMenuBuilder.Received().AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item != null));
        }

        #endregion

        #region AddClearIllustrationPointsOfCalculationsInGroupItem

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddClearIllustrationPointsOfCalculationsInGroupItem_EnabledSituation_ItemAddedToContextMenuAsExpected(bool isEnabled)
        {
            // Setup
            string expectedToolTipMessage = isEnabled
                                                ? "Wis alle berekende illustratiepunten binnen deze map met berekeningen."
                                                : "Er zijn geen berekeningen met illustratiepunten om te wissen.";
            var changeHandler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            contextMenuBuilder.When(x => x.AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item != null)))
                              .Do(callinfo =>
                              {
                                  var contextMenuItem = callinfo.Arg<StrictContextMenuItem>();
                                  Assert.AreEqual("Wis alle &illustratiepunten...", contextMenuItem.Text);
                                  Assert.AreEqual(expectedToolTipMessage, contextMenuItem.ToolTipText);
                                  Assert.AreEqual(isEnabled, contextMenuItem.Enabled);
                              });
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddClearIllustrationPointsOfCalculationsInGroupItem(() => isEnabled, changeHandler);

            // Assert
            contextMenuBuilder.Received().AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item != null));
        }

        #endregion

        #region AddClearIllustrationPointsOfCalculationsInFailureMechanismItem

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddClearIllustrationPointsOfCalculationsInFailureMechanismItem_EnabledSituation_ItemAddedToContextMenuAsExpected(bool isEnabled)
        {
            // Setup
            string expectedToolTipMessage = isEnabled
                                                ? "Wis alle berekende illustratiepunten binnen dit faalmechanisme."
                                                : "Er zijn geen berekeningen met illustratiepunten om te wissen.";
            var changeHandler = Substitute.For<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            contextMenuBuilder.When(x => x.AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item != null)))
                              .Do(callinfo =>
                              {
                                  var contextMenuItem = callinfo.Arg<StrictContextMenuItem>();
                                  Assert.AreEqual("Wis alle &illustratiepunten...", contextMenuItem.Text);
                                  Assert.AreEqual(expectedToolTipMessage, contextMenuItem.ToolTipText);
                                  Assert.AreEqual(isEnabled, contextMenuItem.Enabled);
                              });
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddClearIllustrationPointsOfCalculationsInFailureMechanismItem(() => isEnabled, changeHandler);

            // Assert
            contextMenuBuilder.Received().AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item != null));
        }

        #endregion

        #region AddClearIllustrationPointsOfCalculationsItem

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddClearIllustrationPointsOfCalculationItem_EnabledSituation_ItemAddedToContextMenuAsExpected(bool isEnabled)
        {
            // Setup
            string expectedToolTipMessage = isEnabled
                                                ? "Wis de berekende illustratiepunten van deze berekening."
                                                : "Deze berekening heeft geen illustratiepunten om te wissen.";
            var changeHandler = Substitute.For<IClearIllustrationPointsOfCalculationChangeHandler>();
            var contextMenuBuilder = Substitute.For<IContextMenuBuilder>();
            contextMenuBuilder.When(x => x.AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item != null)))
                              .Do(callinfo =>
                              {
                                  var contextMenuItem = callinfo.Arg<StrictContextMenuItem>();
                                  Assert.AreEqual("Wis illustratiepunten...", contextMenuItem.Text);
                                  Assert.AreEqual(expectedToolTipMessage, contextMenuItem.ToolTipText);
                                  Assert.AreEqual(isEnabled, contextMenuItem.Enabled);
                              });
            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddClearIllustrationPointsOfCalculationItem(() => isEnabled, changeHandler);

            // Assert
            contextMenuBuilder.Received().AddCustomItem(Arg.Is<StrictContextMenuItem>(item => item != null));
        }

        #endregion

        #region AddPerformCalculationItem

        [Test]
        public void AddPerformCalculationItem_AdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculation,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformCalculationItem<TestCalculation, TestCalculationContext>(
                    calculationContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Be&rekenen",
                                                              "Voer deze berekening uit.",
                                                              RiskeerFormsResources.CalculateIcon);
            }
        }

        [Test]
        public void AddPerformCalculationItem_AdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageInTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculation,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "No valid data";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformCalculationItem<TestCalculation, TestCalculationContext>(
                    calculationContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Be&rekenen",
                                                              errorMessage,
                                                              RiskeerFormsResources.CalculateIcon,
                                                              false);
            }
        }

        #endregion

        #region AddValidateCalculationItem

        [Test]
        public void AddValidateCalculationItem_AdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculation,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateCalculationItem(calculationContext, null, c => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Valideren",
                                                              "Valideer de invoer voor deze berekening.",
                                                              RiskeerFormsResources.ValidateIcon);
            }
        }

        [Test]
        public void AddValidateCalculationItem_AdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageInTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = Substitute.For<ICalculatableFailureMechanism>();
            var parent = new CalculationGroup();
            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculation,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "No valid data";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateCalculationItem(calculationContext, null, c => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "&Valideren",
                                                              errorMessage,
                                                              RiskeerFormsResources.ValidateIcon,
                                                              false);
            }
        }

        #endregion

        #region AddPerformAllCalculationsInGroupItem

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculation = new TestCalculation();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                              RiskeerFormsResources.CalculateAllIcon);
            }
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculation = new TestCalculation();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              errorMessage,
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }
        }

        #endregion

        #region AddValidateAllCalculationsInGroupItem

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculation = new TestCalculation();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Valideer alle berekeningen binnen deze map met berekeningen.",
                                                              RiskeerFormsResources.ValidateAllIcon);
            }
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RiskeerFormsResources.ValidateAllIcon,
                                                              false);
            }
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var calculation = new TestCalculation();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                calculation
            });
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              errorMessage,
                                                              RiskeerFormsResources.ValidateAllIcon,
                                                              false);
            }
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, parent, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                calculationGroup,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RiskeerFormsResources.ValidateAllIcon,
                                                              false);
            }
        }

        #endregion

        #region AddPerformAllCalculationsInFailureMechanismItem

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen dit faalmechanisme uit.",
                                                              RiskeerFormsResources.CalculateAllIcon);
            }
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              errorMessage,
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }
        }

        #endregion

        #region AddValidateAllCalculationsInFailureMechanismItem

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Valideer alle berekeningen binnen dit faalmechanisme.",
                                                              RiskeerFormsResources.ValidateAllIcon);
            }
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RiskeerFormsResources.ValidateAllIcon,
                                                              false);
            }
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new TestCalculatableFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(
                    failureMechanismContext,
                    null,
                    fm => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              errorMessage,
                                                              RiskeerFormsResources.ValidateAllIcon,
                                                              false);
            }
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var applicationFeatureCommands = Substitute.For<IApplicationFeatureCommands>();
            var importCommandHandler = Substitute.For<IImportCommandHandler>();
            var exportCommandHandler = Substitute.For<IExportCommandHandler>();
            var updateCommandHandler = Substitute.For<IUpdateCommandHandler>();
            var viewCommands = Substitute.For<IViewCommands>();
            var assessmentSection = Substitute.For<IAssessmentSection>();

            var failureMechanism = new TestCalculatableFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommands,
                                                                importCommandHandler,
                                                                exportCommandHandler,
                                                                updateCommandHandler,
                                                                viewCommands,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

                const string errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = riskeerContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(
                    failureMechanismContext,
                    null,
                    fm => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles &valideren",
                                                              "Er zijn geen berekeningen om te valideren.",
                                                              RiskeerFormsResources.ValidateAllIcon,
                                                              false);
            }
        }

        #endregion

        #region Nested types

        private class TestFailureMechanismContext : FailureMechanismContext<ICalculatableFailureMechanism>
        {
            public TestFailureMechanismContext(ICalculatableFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) :
                base(wrappedFailureMechanism, parent) {}
        }

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, ICalculatableFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, CalculationGroup parent, ICalculatableFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; }

            public CalculationGroup Parent { get; }

            public ICalculatableFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, ICalculatableFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, CalculationGroup parent, ICalculatableFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; }

            public CalculationGroup Parent { get; }

            public ICalculatableFailureMechanism FailureMechanism { get; }
        }

        public interface ICalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile {}

        #endregion
    }
}