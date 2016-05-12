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

using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using BaseResources = Core.Common.Base.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsContextMenuItemFactoryTest : NUnitFormTest
    {
        [Test]
        public void AddCreateCalculationGroupItem_Always_CreatesDecoratedCalculationGroupItem()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddCreateCalculationGroupItem(menuBuilder, calculationGroup);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menuBuilder.Build(), 0,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup,
                                                          RingtoetsFormsResources.CalculationGroup_Add_CalculationGroup_Tooltip,
                                                          RingtoetsFormsResources.AddFolderIcon);
        }

        [Test]
        public void AddCreateCalculationGroupItem_PerformClickOnCreatedItem_CalculationGroupAdded()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);
            RingtoetsContextMenuItemFactory.AddCreateCalculationGroupItem(menuBuilder, calculationGroup);
            var contextMenuItem = menuBuilder.Build().Items[0];

            // Call
            contextMenuItem.PerformClick();

            // Assert
            Assert.AreEqual(1, calculationGroup.Children.Count);
            Assert.IsTrue(calculationGroup.Children[0] is CalculationGroup);
        }

        [Test]
        public void AddCreateCalculationItem_Always_CreatesDecoratedCalculationItem()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);
            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddCreateCalculationItem(menuBuilder, calculationGroupContext, null);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menuBuilder.Build(), 0,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation,
                                                          RingtoetsFormsResources.CalculationGroup_Add_Calculation_Tooltip,
                                                          RingtoetsFormsResources.FailureMechanismIcon);
        }

        [Test]
        public void AddCreateCalculationItem_PerformClickOnCreatedItem_AddCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var counter = 0;
            var calculationGroup = new CalculationGroup();
            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);
            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            RingtoetsContextMenuItemFactory.AddCreateCalculationItem(menuBuilder, calculationGroupContext, context => counter++);
            var contextMenuItem = menuBuilder.Build().Items[0];

            // Call
            contextMenuItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_GroupWithCalculationOutput_CreatesDecoratedAndEnabledClearItem()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
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

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddClearAllCalculationOutputInGroupItem(menuBuilder, calculationGroup);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menuBuilder.Build(), 0,
                                                          RingtoetsFormsResources.Clear_all_output,
                                                          RingtoetsFormsResources.CalculationGroup_ClearOutput_ToolTip,
                                                          RingtoetsFormsResources.ClearIcon);
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_GroupWithoutCalculationOutput_CreatesDecoratedAndDisabledClearItem()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithoutOutput.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithoutOutput
                }
            };

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddClearAllCalculationOutputInGroupItem(menuBuilder, calculationGroup);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menuBuilder.Build(), 0,
                                                          RingtoetsFormsResources.Clear_all_output,
                                                          RingtoetsFormsResources.CalculationGroup_ClearOutput_No_calculation_with_output_to_clear,
                                                          RingtoetsFormsResources.ClearIcon,
                                                          false);
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            var messageBoxText = "";
            var messageBoxTitle = "";
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculationWithOutput1 = mocks.StrictMock<ICalculation>();
            var calculationWithOutput2 = mocks.StrictMock<ICalculation>();
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutput2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutput.Stub(c => c.HasOutput).Return(false);

            calculationWithOutput1.Expect(c => c.ClearOutput());
            calculationWithOutput1.Expect(c => c.NotifyObservers());
            calculationWithOutput2.Expect(c => c.ClearOutput());
            calculationWithOutput2.Expect(c => c.NotifyObservers());

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutput1,
                    new CalculationGroup
                    {
                        Children =
                        {
                            calculationWithOutput2,
                            calculationWithoutOutput
                        }
                    }
                }
            };

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            RingtoetsContextMenuItemFactory.AddClearAllCalculationOutputInGroupItem(menuBuilder, calculationGroup);
            var contextMenuItem = menuBuilder.Build().Items[0];

            // Call
            contextMenuItem.PerformClick();

            // Assert
            Assert.AreEqual(BaseResources.Confirm, messageBoxTitle);
            Assert.AreEqual(RingtoetsFormsResources.CalculationGroup_ClearOutput_Are_you_sure_clear_all_output, messageBoxText);

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearAllCalculationOutputInGroupItem_PerformClickOnCreatedItemAndCancelChange_CalculationOutputNotCleared()
        {
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculationWithOutput1 = mocks.StrictMock<ICalculation>();
            var calculationWithOutput2 = mocks.StrictMock<ICalculation>();
            var calculationWithoutOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput1.Stub(c => c.HasOutput).Return(true);
            calculationWithOutput2.Stub(c => c.HasOutput).Return(true);
            calculationWithoutOutput.Stub(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculationWithOutput1,
                    new CalculationGroup
                    {
                        Children =
                        {
                            calculationWithOutput2,
                            calculationWithoutOutput
                        }
                    }
                }
            };

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            RingtoetsContextMenuItemFactory.AddClearAllCalculationOutputInGroupItem(menuBuilder, calculationGroup);
            var contextMenuItem = menuBuilder.Build().Items[0];

            // Call
            contextMenuItem.PerformClick();

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GroupWithCalculations_CreatesDecoratedAndEnabledPerformItem()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculation = mocks.StrictMock<ICalculation>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddPerformAllCalculationsInGroupItem<TestCalculationGroupContext>(menuBuilder, calculationGroup, null, null);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menuBuilder.Build(), 0,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          RingtoetsFormsResources.CalculationGroup_CalculateAll_ToolTip,
                                                          RingtoetsFormsResources.CalculateAllIcon);
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_GroupWithoutCalculations_CreatesDecoratedAndDisabledPerformItem()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var calculationGroup = new CalculationGroup();
            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddPerformAllCalculationsInGroupItem<TestCalculationGroupContext>(menuBuilder, calculationGroup, null, null);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menuBuilder.Build(), 0,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          RingtoetsFormsResources.CalculationGroup_CalculateAll_No_calculations_to_run,
                                                          RingtoetsFormsResources.CalculateAllIcon,
                                                          false);
        }

        [Test]
        public void AddPerformAllCalculationsInGroupItem_PerformClickOnCreatedItem_PerformAllCalculationMethodPerformed()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculation = mocks.StrictMock<ICalculation>();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var counter = 0;
            var calculationGroup = new CalculationGroup
            {
                Children =
                {
                    calculation
                }
            };

            var calculationGroupContext = new TestCalculationGroupContext(calculationGroup, failureMechanism);

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationGroup, treeViewControl);

            RingtoetsContextMenuItemFactory.AddPerformAllCalculationsInGroupItem(menuBuilder, calculationGroup, calculationGroupContext, (group, context) => counter++);
            var contextMenuItem = menuBuilder.Build().Items[0];

            // Call
            contextMenuItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
        }

        [Test]
        public void AddPerformCalculationItem_Always_CreatesPerformItem()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();

            var menubuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculation, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddPerformCalculationItem<TestCalculation, TestCalculationContext>(menubuilder, calculation, null, null);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menubuilder.Build(), 0,
                                                          RingtoetsFormsResources.Calculate,
                                                          RingtoetsFormsResources.Calculate_ToolTip,
                                                          RingtoetsFormsResources.CalculateIcon);

            mocks.VerifyAll();
        }

        [Test]
        public void AddPerformCalculationItem_PerformClickOnCreatedItem_PerformCalculationMethod()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var failureMechanisMock = mocks.StrictMock<IFailureMechanism>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            mocks.ReplayAll();

            var calculation = new TestCalculation();
            var testCalculationContext = new TestCalculationContext(calculation, failureMechanisMock);

            var counter = 0;

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculation, treeViewControl);
            RingtoetsContextMenuItemFactory.AddPerformCalculationItem(menuBuilder, calculation, testCalculationContext, (calc, context) => counter++);
            var contextMenuItem = menuBuilder.Build().Items[0];

            // Call
            contextMenuItem.PerformClick();

            // Assert
            Assert.AreEqual(1, counter);
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_CalculationWithOutput_CreatesDecoratedAndEnabledClearItem()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Expect(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationWithOutput, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddClearCalculationOutputItem(menuBuilder, calculationWithOutput);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menuBuilder.Build(), 0,
                                                          RingtoetsFormsResources.Clear_output,
                                                          RingtoetsFormsResources.Clear_output_ToolTip,
                                                          RingtoetsFormsResources.ClearIcon);
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_CalculationWithoutOutput_CreatesDecoratedAndDisabledClearItem()
        {
            // Setup
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Expect(c => c.HasOutput).Return(false);

            mocks.ReplayAll();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationWithOutput, treeViewControl);

            // Call
            RingtoetsContextMenuItemFactory.AddClearCalculationOutputItem(menuBuilder, calculationWithOutput);

            // Assert
            TestHelper.AssertContextMenuStripContainsItem(menuBuilder.Build(), 0,
                                                          RingtoetsFormsResources.Clear_output,
                                                          RingtoetsFormsResources.ClearOutput_No_output_to_clear,
                                                          RingtoetsFormsResources.ClearIcon,
                                                          false);
            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_PerformClickOnCreatedItemAndConfirmChange_CalculationOutputClearedAndObserversNotified()
        {
            var messageBoxText = "";
            var messageBoxTitle = "";
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Stub(c => c.HasOutput).Return(true);

            calculationWithOutput.Expect(c => c.ClearOutput());
            calculationWithOutput.Expect(c => c.NotifyObservers());

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                messageBox.ClickOk();
            };

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationWithOutput, treeViewControl);

            RingtoetsContextMenuItemFactory.AddClearCalculationOutputItem(menuBuilder, calculationWithOutput);
            var contextMenuItem = menuBuilder.Build().Items[0];

            // Call
            contextMenuItem.PerformClick();

            // Assert
            Assert.AreEqual(BaseResources.Confirm, messageBoxTitle);
            Assert.AreEqual(RingtoetsFormsResources.Calculation_ContextMenuStrip_Are_you_sure_clear_output, messageBoxText);

            mocks.VerifyAll();
        }

        [Test]
        public void AddClearCalculationOutputItem_PerformClickOnCreatedItemAndCancelChange_CalculationOutputNotCleared()
        {
            var mocks = new MockRepository();
            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var calculationWithOutput = mocks.StrictMock<ICalculation>();

            calculationWithOutput.Stub(c => c.HasOutput).Return(true);

            mocks.ReplayAll();

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBox.ClickCancel();
            };

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, calculationWithOutput, treeViewControl);

            RingtoetsContextMenuItemFactory.AddClearCalculationOutputItem(menuBuilder, calculationWithOutput);
            var contextMenuItem = menuBuilder.Build().Items[0];

            // Call
            contextMenuItem.PerformClick();

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
