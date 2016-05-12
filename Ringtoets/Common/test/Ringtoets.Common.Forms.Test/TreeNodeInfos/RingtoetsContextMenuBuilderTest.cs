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

using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
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
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculationGroup, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

            // Call
            var result = ringtoetsContextMenuBuilder.AddCreateCalculationGroupItem(calculationGroup).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup_Tooltip,
                                                          RingtoetsFormsResources.AddFolderIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void AddCreateCalculationItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanism = mocks.Stub<IFailureMechanism>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);
            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculationGroup, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

            // Call
            var result = ringtoetsContextMenuBuilder.AddCreateCalculationItem(calculationGroupContext, context => { }).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation_Tooltip,
                                                          RingtoetsFormsResources.FailureMechanismIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithCalculationOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
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
            
            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculationGroup, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

            // Call
            var result = ringtoetsContextMenuBuilder.AddClearAllCalculationOutputInGroupItem(calculationGroup).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                          RingtoetsFormsResources.Clear_all_output,
                                                          RingtoetsFormsResources.CalculationGroup_ClearOutput_ToolTip,
                                                          RingtoetsFormsResources.ClearIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_WhenBuildWithoutCalculationOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();

            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculationGroup, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

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

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_WhenBuildWithCalculation_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };
            var testCalculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanisMock);

            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculationGroup, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

            // Call
            var result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, testCalculationGroupContext, null).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          RingtoetsFormsResources.CalculationGroup_CalculateAll_ToolTip,
                                                          RingtoetsFormsResources.CalculateAllIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_WhenBuildWithoutCalculation_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var testCalculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanisMock);

            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculationGroup, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

            // Call
            var result = ringtoetsContextMenuBuilder.AddPerformAllCalculationsInGroupItem(calculationGroup, testCalculationGroupContext, null).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run,
                                                          RingtoetsFormsResources.CalculateAllIcon,
                                                          false);

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_WhenBuild_ItemAddedToContextMenu()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var testCalculationContext = new TestCalculationContext(calculation, failureMechanisMock);

            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculation, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

            // Call
            var result = ringtoetsContextMenuBuilder.AddPerformCalculationItem(calculation, testCalculationContext, null).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                          RingtoetsFormsResources.Calculate,
                                                          RingtoetsFormsResources.Calculate_ToolTip,
                                                          RingtoetsFormsResources.CalculateIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithOutput_ItemAddedToContextMenuEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculationWithOutput, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

            // Call
            var result = ringtoetsContextMenuBuilder.AddClearCalculationOutputItem(calculationWithOutput).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                          RingtoetsFormsResources.Clear_output,
                                                          RingtoetsFormsResources.Clear_output_ToolTip,
                                                          RingtoetsFormsResources.ClearIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_WhenBuildWithCalculationWithoutOutput_ItemAddedToContextMenuDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandsMock = mocks.StrictMock<IApplicationFeatureCommands>();
            var exportImportHandlerMock = mocks.StrictMock<IExportImportCommandHandler>();
            var viewCommandsMock = mocks.StrictMock<IViewCommands>();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithoutOutput.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            var contextMenuBuilder = new ContextMenuBuilder(applicationFeatureCommandsMock, exportImportHandlerMock, viewCommandsMock, calculationWithoutOutput, treeViewControlMock);
            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            mocks.ReplayAll();

            // Call
            var result = ringtoetsContextMenuBuilder.AddClearCalculationOutputItem(calculationWithoutOutput).Build();

            // Assert
            Assert.IsInstanceOf<ContextMenuStrip>(result);
            Assert.AreEqual(1, result.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(result, 0,
                                                          RingtoetsFormsResources.Clear_output,
                                                          RingtoetsFormsResources.ClearOutput_No_output_to_clear,
                                                          RingtoetsFormsResources.ClearIcon,
                                                          false);

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

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddDeleteItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddExpandAllItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddCollapseAllItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddOpenItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddExportItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddImportItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddPropertiesItem());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddSeparator());

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

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
            var contextMenuItem = mocks.StrictMock<StrictContextMenuItem>();
            var contextMenuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            contextMenuBuilder.Expect(cmb => cmb.AddCustomItem(contextMenuItem));

            mocks.ReplayAll();

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.AddCustomItem(contextMenuItem);

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

            var ringtoetsContextMenuBuilder = new RingtoetsContextMenuBuilder(contextMenuBuilder);

            // Call
            ringtoetsContextMenuBuilder.Build();

            // Assert
            mocks.VerifyAll();
        }

        # region Nested types

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

            public string Comments { get; set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() { }

            public void ClearHydraulicBoundaryLocation() { }

            public ICalculationInput GetObservableInput()
            {
                return null;
            }

            public ICalculationOutput GetObservableOutput()
            {
                return null;
            }
        }

        # endregion
    }
}
