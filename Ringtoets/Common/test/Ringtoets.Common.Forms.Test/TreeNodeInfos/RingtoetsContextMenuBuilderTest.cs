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

using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsContextMenuBuilderTest
    {
        [Test]
        public void AddCreateCalculationGroupItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var calculationGroup = new CalculationGroup();
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddCreateCalculationGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                              RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup_Tooltip,
                                                              RingtoetsFormsResources.AddFolderIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddCreateCalculationItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var calculationGroup = new CalculationGroup();
                var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanismMock);
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddCreateCalculationItem(calculationGroupContext, context => { }).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                              RingtoetsFormsResources.CalculationGroup_Add_Calculation_Tooltip,
                                                              RingtoetsFormsResources.FailureMechanismIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutputMock
                }
            };
            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup, treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Clear_all_output,
                                                              RingtoetsFormsResources.CalculationGroup_ClearOutput_ToolTip,
                                                              RingtoetsFormsResources.ClearIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInGroupItem(calculationGroup).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Clear_all_output,
                                                              RingtoetsFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
                                                              RingtoetsFormsResources.ClearIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInFailureMechanismItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();
            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.Calculations).Return(new[]
            {
                calculationWithOutputMock
            });
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Clear_all_output,
                                                              RingtoetsFormsResources.Clear_all_output_ToolTip,
                                                              RingtoetsFormsResources.ClearIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInFailureMechanismItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.Stub<IFailureMechanism>();
            failureMechanismMock.Stub(fm => fm.Calculations).Return(new List<ICalculation>());

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInFailureMechanismItem(failureMechanismMock).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Clear_all_output,
                                                              RingtoetsFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
                                                              RingtoetsFormsResources.ClearIcon,
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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            failureMechanismMock.Expect(fm => fm.IsRelevant).Return(isRelevant);
            var failureMechanismContextMock = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();
            failureMechanismContextMock.Expect(fmc => fmc.WrappedData).Return(failureMechanismMock);
            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContextMock, null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                var checkboxIcon = isRelevant ? RingtoetsFormsResources.Checkbox_ticked : RingtoetsFormsResources.Checkbox_empty;
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.FailureMechanismContextMenuStrip_Is_relevant,
                                                              RingtoetsFormsResources.FailureMechanismContextMenuStrip_Is_relevant_Tooltip,
                                                              checkboxIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var calculationWithOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithOutputMock.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationWithOutputMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddClearCalculationOutputItem(calculationWithOutputMock).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Clear_output,
                                                              RingtoetsFormsResources.Clear_output_ToolTip,
                                                              RingtoetsFormsResources.ClearIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithoutOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var calculationWithoutOutputMock = mocks.StrictMock<ICalculation>();

            calculationWithoutOutputMock.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationWithoutOutputMock,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                var result = ringtoetsContextMenuBuilder.AddClearCalculationOutputItem(calculationWithoutOutputMock).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Clear_output,
                                                              RingtoetsFormsResources.ClearOutput_No_output_to_clear,
                                                              RingtoetsFormsResources.ClearIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddRenameItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddRenameItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddRenameItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddDeleteItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddDeleteItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddDeleteItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddExpandAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddExpandAllItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddExpandAllItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCollapseAllItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddCollapseAllItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddCollapseAllItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddOpenItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddOpenItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddOpenItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddExportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddExportItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddExportItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddImportItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddImportItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddImportItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddPropertiesItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddPropertiesItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddPropertiesItem();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddSeparator_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddSeparator());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddSeparator();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void AddCustomItem_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuItemMock = mocks.StrictMock<StrictContextMenuItem>();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.AddCustomItem(contextMenuItemMock));

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.AddCustomItem(contextMenuItemMock);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void Build_ContextMenuBuilder_CorrectlyDecorated()
        {
            // Setup
            var mocks = new MockRepository();
            var contextMenuBuilderMock = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilderMock.Expect(cmb => cmb.Build());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilderMock);

            // Call
            ringtoetsContextMenuBuilder.Build();

            // Assert
            mocks.VerifyAll();
        }

        #region AddPerformCalculationItem

        [Test]
        public void AddPerformCalculationItem_AdditionalValidationNull_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanisMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculation,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformCalculationItem(calculation, calculationContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate,
                                                              RingtoetsFormsResources.Calculate_ToolTip,
                                                              RingtoetsFormsResources.CalculateIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformCalculationItem_AdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanisMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculation,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "No valid data";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformCalculationItem(calculation, calculationContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate,
                                                              errorMessage,
                                                              RingtoetsFormsResources.CalculateIcon,
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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculation,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateCalculationItem(calculationContext, null, c => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate,
                                                              RingtoetsFormsResources.Validate_ToolTip,
                                                              RingtoetsFormsResources.ValidateIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateCalculationItem_AdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageInTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationContext = new TestCalculationContext(calculation, failureMechanismMock);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculation,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "No valid data";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateCalculationItem(calculationContext, null, c => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate,
                                                              errorMessage,
                                                              RingtoetsFormsResources.ValidateIcon,
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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
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
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate_all,
                                                              RingtoetsFormsResources.CalculationGroup_CalculateAll_ToolTip,
                                                              RingtoetsFormsResources.CalculateAllIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate_all,
                                                              RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run,
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
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
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate_all,
                                                              errorMessage,
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate_all,
                                                              RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run,
                                                              RingtoetsFormsResources.CalculateAllIcon,
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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
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
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate_all,
                                                              RingtoetsFormsResources.CalculationGroup_Validate_all_ToolTip,
                                                              RingtoetsFormsResources.ValidateAllIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate_all,
                                                              RingtoetsFormsResources.ValidateAll_No_calculations_to_validate,
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
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
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate_all,
                                                              errorMessage,
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInGroupItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();

            mocks.ReplayAll();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                calculationGroup,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInGroupItem(calculationGroupContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);

                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate_all,
                                                              RingtoetsFormsResources.ValidateAll_No_calculations_to_validate,
                                                              RingtoetsFormsResources.ValidateAllIcon,
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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate_all,
                                                              RingtoetsFormsResources.Calculate_all_ToolTip,
                                                              RingtoetsFormsResources.CalculateAllIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate_all,
                                                              RingtoetsFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run,
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate_all,
                                                              errorMessage,
                                                              RingtoetsFormsResources.CalculateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, null, context => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Calculate_all,
                                                              RingtoetsFormsResources.FailureMechanism_CreateCalculateAllItem_No_calculations_to_run,
                                                              RingtoetsFormsResources.CalculateAllIcon,
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
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate_all,
                                                              RingtoetsFormsResources.FailureMechanism_Validate_all_ToolTip,
                                                              RingtoetsFormsResources.ValidateAllIcon);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationNull_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(failureMechanismContext, null, fm => null).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate_all,
                                                              RingtoetsFormsResources.ValidateAll_No_calculations_to_validate,
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationTrueAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(new[]
            {
                new TestCalculation()
            });
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(
                    failureMechanismContext,
                    null,
                    fm => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate_all,
                                                              errorMessage,
                                                              RingtoetsFormsResources.ValidateAllIcon,
                                                              false);
            }
            mocks.VerifyAll();
        }

        [Test]
        public void AddValidateAllCalculationsInFailureMechanismItem_GeneralValidationFalseAdditionalValidationContainsMessage_ItemAddedToContextMenuDisabledWithGeneralValidationMessageTooltip()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var importHandlerMock = mocks.StrictMock<IImportCommandHandler>();
            var exportHandlerMock = mocks.StrictMock<IExportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var assessmentSectionMock = mocks.StrictMock<IAssessmentSection>();

            var failureMechanism = new TestFailureMechanism(Enumerable.Empty<ICalculation>());
            var failureMechanismContext = new TestFailureMechanismContext(failureMechanism, assessmentSectionMock);

            mocks.ReplayAll();

            using (var treeViewControl = new TreeViewControl())
            {
                var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock,
                                                                importHandlerMock,
                                                                exportHandlerMock,
                                                                viewCommandsMock,
                                                                failureMechanismContext,
                                                                treeViewControl);
                var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

                var errorMessage = "Additional validation failed.";

                // Call
                ContextMenuStrip result = ringtoetsContextMenuBuilder.AddValidateAllCalculationsInFailureMechanismItem(
                    failureMechanismContext,
                    null,
                    fm => errorMessage).Build();

                // Assert
                Assert.IsInstanceOf<ContextMenuStrip>(result);
                Assert.AreEqual(1, result.Items.Count);
                TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                              RingtoetsFormsResources.Validate_all,
                                                              RingtoetsFormsResources.ValidateAll_No_calculations_to_validate,
                                                              RingtoetsFormsResources.ValidateAllIcon,
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
            public TestCalculationGroupContext(CalculationGroup wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; private set; }

            public IFailureMechanism FailureMechanism { get; private set; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, IFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; private set; }

            public IFailureMechanism FailureMechanism { get; private set; }
        }

        private class TestCalculation : Observable, ICalculation
        {
            public TestCalculation()
            {
                Name = "Nieuwe berekening";
            }

            public string Name { get; set; }

            public Commentable Comments { get; private set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() {}
        }

        #endregion
    }
}