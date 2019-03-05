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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.DikeProfiles;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.TestUtil;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.PresentationObjects;
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
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddCreateCalculationItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

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
                ContextMenuStrip result = riskeerContextMenuBuilder.AddCreateCalculationItem(calculationGroupContext, context => {}).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Berekening &toevoegen",
                                                              "Voeg een nieuwe berekening toe aan deze map met berekeningen.",
                                                              RiskeerFormsResources.FailureMechanismIcon);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInFailureMechanismItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();
            calculationWithOutput.Expect(c => c.HasOutput).Return(true);
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithOutput
            });
            mocks.ReplayAll();

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
                                                              "Wis de uitvoer van alle berekeningen binnen dit toetsspoor.",
                                                              RiskeerFormsResources.ClearIcon);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInFailureMechanismItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Calculations).Return(new List<ICalculation>());

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddToggleRelevancyOfFailureMechanismItem_WhenBuild_ItemAddedToContextMenuEnabled(bool isRelevant)
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();
            failureMechanism.Expect(fm => fm.IsRelevant).Return(isRelevant);
            var failureMechanismContext = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();
            failureMechanismContext.Expect(fmc => fmc.WrappedData).Return(failureMechanism);
            mocks.ReplayAll();

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
                ContextMenuStrip result = riskeerContextMenuBuilder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                Bitmap checkboxIcon = isRelevant ? RiskeerFormsResources.Checkbox_ticked : RiskeerFormsResources.Checkbox_empty;
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "I&s relevant",
                                                              "Geeft aan of dit toetsspoor relevant is of niet.",
                                                              checkboxIcon);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithoutOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithoutOutput.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddRenameItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddRenameItem());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddRenameItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddDeleteItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddDeleteItem());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddDeleteItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddExpandAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddExpandAllItem());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddExpandAllItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCollapseAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            menuBuilder.Expect(cmb => cmb.AddCollapseAllItem());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(menuBuilder);

            // Call
            riskeerContextMenuBuilder.AddCollapseAllItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddOpenItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddOpenItem());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddOpenItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddExportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddExportItem());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddExportItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddImportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddImportItem());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddImportItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddPropertiesItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddPropertiesItem());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddPropertiesItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddSeparator_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddSeparator());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddSeparator();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuItem = mocks.StrictMock<StrictContextMenuItem>();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddCustomItem(contextMenuItem));

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddCustomItem(contextMenuItem);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomImportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            const string text = "import";
            const string toolTip = "import tooltip";
            Bitmap image = RiskeerFormsResources.DatabaseIcon;

            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddCustomImportItem(text, toolTip, image));

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddCustomImportItem(text, toolTip, image);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Build_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.Build());

            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.Build();

            // Assert
            mocks.VerifyAll();
        }

        #region AddUpdateForeshoreProfileOfCalculationItem

        [Test]
        [Combinatorial]
        public void AddUpdateForeshoreProfileOfCalculationItem_ForeshoreProfileStates_ItemAddedToContextMenuAsExpected(
            [Values(true, false)] bool hasForeshoreProfile,
            [Values(true, false)] bool isSynchronized)
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();

            if (hasForeshoreProfile)
            {
                input.Expect(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
                input.Expect(ci => ci.IsForeshoreProfileInputSynchronized).Return(isSynchronized);
            }
            else
            {
                input.Expect(ci => ci.ForeshoreProfile).Return(null);
            }

            calculation.Expect(c => c.InputParameters).Return(input).Repeat.Any();
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

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

            mocks.VerifyAll();
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
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            var calculation = mocks.StrictMock<ICalculation<ICalculationInputWithForeshoreProfile>>();
            var input = mocks.StrictMock<ICalculationInputWithForeshoreProfile>();
            if (hasForeshoreProfile)
            {
                input.Expect(ci => ci.ForeshoreProfile).Return(new TestForeshoreProfile());
                input.Expect(ci => ci.IsForeshoreProfileInputSynchronized).Return(isSynchronized);
            }
            else
            {
                input.Expect(ci => ci.ForeshoreProfile).Return(null);
            }

            calculation.Stub(c => c.InputParameters).Return(input);
            var inquiryHelper = mocks.StrictMock<IInquiryHelper>();
            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        #endregion

        #region AddDuplicateCalculationItem

        [Test]
        public void AddDuplicateCalculationItem_WhenBuildWithCalculationItem_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var calculationItem = mocks.Stub<ICalculationBase>();
            var calculationItemContext = mocks.Stub<ICalculationContext<ICalculationBase, IFailureMechanism>>();
            calculationItemContext.Stub(ci => ci.Parent).Return(new CalculationGroup());

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        #endregion

        #region AddClearIllustrationPointsOfCalculationsItem

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void AddClearIllustrationPointsOfCalculationsItem_EnabledSituation_ItemAddedToContextMenuAsExpected(bool isEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var changeHandler = mocks.StrictMock<IClearIllustrationPointsOfCalculationCollectionChangeHandler>();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();
            contextMenuBuilder.Expect(cmb => cmb.AddCustomItem(Arg<StrictContextMenuItem>.Is.NotNull))
                              .WhenCalled(arg =>
                              {
                                  string expectedToolTipMessage = isEnabled
                                                                      ? "Wis alle berekende illustratiepunten."
                                                                      : "Er zijn geen berekeningen met illustratiepunten om te wissen.";

                                  var contextMenuItem = (StrictContextMenuItem) arg.Arguments[0];
                                  Assert.AreEqual("Wis alle illustratiepunten...", contextMenuItem.Text);
                                  Assert.AreEqual(expectedToolTipMessage, contextMenuItem.ToolTipText);
                                  Assert.AreEqual(isEnabled, contextMenuItem.Enabled);
                              });
            mocks.ReplayAll();

            var riskeerContextMenuBuilder = new RiskeerContextMenuBuilder(contextMenuBuilder);

            // Call
            riskeerContextMenuBuilder.AddClearIllustrationPointsOfCalculationsItem(() => isEnabled, changeHandler);

            // Assert
            mocks.VerifyAll();
        }

        #endregion

        #region AddPerformCalculationItem

        [Test]
        public void AddPerformCalculationItem_AdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

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
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformCalculationItem(calculation, calculationContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Be&rekenen",
                                                              "Voer deze berekening uit.",
                                                              RiskeerFormsResources.CalculateIcon);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformCalculationItem_AdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

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
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformCalculationItem(calculation, calculationContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Be&rekenen",
                                                              errorMessage,
                                                              RiskeerFormsResources.CalculateIcon,
                                                              false);
            }

            mocks.VerifyAll();
        }

        #endregion

        #region AddValidateCalculationItem

        [Test]
        public void AddValidateCalculationItem_AdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateCalculationItem_AdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        #endregion

        #region AddPerformAllCalculationsInGroupItem

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
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
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Voer alle berekeningen binnen deze map met berekeningen uit.",
                                                              RiskeerFormsResources.CalculateAllIcon);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
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
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
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
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              errorMessage,
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
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
                ContextMenuStrip result = riskeerContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              "Alles be&rekenen",
                                                              "Er zijn geen berekeningen om uit te voeren.",
                                                              RiskeerFormsResources.CalculateAllIcon,
                                                              false);
            }

            mocks.VerifyAll();
        }

        #endregion

        #region AddValidateAllCalculationsInGroupItem

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
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

            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
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

            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var parent = new CalculationGroup();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var failureMechanism = new TestFailureMechanism(new[]
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

            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
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

            mocks.VerifyAll();
        }

        #endregion

        #region AddPerformAllCalculationsInFailureMechanismItem

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

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
                                                              "Voer alle berekeningen binnen dit toetsspoor uit.",
                                                              RiskeerFormsResources.CalculateAllIcon);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        #endregion

        #region AddValidateAllCalculationsInFailureMechanismItem

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

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
                                                              "Valideer alle berekeningen binnen dit toetsspoor.",
                                                              RiskeerFormsResources.ValidateAllIcon);
            }

            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommands = mocks.StrictMock<IApplicationFeatureCommands>();
            var importCommandHandler = mocks.StrictMock<IImportCommandHandler>();
            var exportCommandHandler = mocks.StrictMock<IExportCommandHandler>();
            var updateCommandHandler = mocks.StrictMock<IUpdateCommandHandler>();
            var viewCommands = mocks.StrictMock<IViewCommands>();
            var assessmentSection = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSection);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        #endregion

        #region Nested types

        private class TestFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public TestFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) :
                base(wrappedFailureMechanism, parent) {}
        }

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, CalculationGroup parent, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; }

            public CalculationGroup Parent { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, IFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, CalculationGroup parent, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                Parent = parent;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; }

            public CalculationGroup Parent { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        public interface ICalculationInputWithForeshoreProfile : ICalculationInput, IHasForeshoreProfile {}

        #endregion
    }
}