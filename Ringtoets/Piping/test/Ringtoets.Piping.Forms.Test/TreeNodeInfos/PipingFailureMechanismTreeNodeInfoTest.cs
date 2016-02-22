using System;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.Properties;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Plugin;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    public class PipingFailureMechanismTreeNodeInfoTest : NUnitFormTest
    {
        private const int contextMenuAddFolderIndex = 0;
        private const int contextMenuAddCalculationIndex = 1;
        private const int contextMenuValidateAllIndex = 3;
        private const int contextMenuCalculateAllIndex = 4;
        private const int contextMenuClearIndex = 5;

        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingFailureMechanism));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PipingFailureMechanism), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.EnsureVisibleOnCreate);
            Assert.IsNull(info.CanRename);
            Assert.IsNull(info.OnNodeRenamed);
            Assert.IsNull(info.CanRemove);
            Assert.IsNull(info.OnNodeRemoved);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
            Assert.IsNull(info.CanDrag);
            Assert.IsNull(info.CanDrop);
            Assert.IsNull(info.CanInsert);
            Assert.IsNull(info.OnDrop);
        }

        [Test]
        public void Text_Always_ReturnsName()
        {
            // Setup
            var mechanism = new PipingFailureMechanism();

            // Call
            var text = info.Text(mechanism);

            // Assert
            Assert.AreEqual(Resources.PipingFailureMechanism_DisplayName, text);
        }

        [Test]
        public void Image_Always_ReturnsPlaceHolderIcon()
        {
            // Call
            var image = info.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, image);
        }

        [Test]
        public void ChildNodeObjects_Always_ReturnChildDataNodes()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculation());
            pipingFailureMechanism.CalculationsGroup.Children.Add(new PipingCalculation());

            // Call
            var children = info.ChildNodeObjects(pipingFailureMechanism).ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder) children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);
            CollectionAssert.AreEqual(new object[]
            {
                pipingFailureMechanism.SectionDivisions,
                pipingFailureMechanism.SurfaceLines,
                pipingFailureMechanism.SoilProfiles,
                pipingFailureMechanism.BoundaryConditions
            }, inputsFolder.Contents);

            var calculationsFolder = (PipingCalculationGroupContext) children[1];
            Assert.AreEqual("Berekeningen", calculationsFolder.WrappedData.Name);
            CollectionAssert.AreEqual(pipingFailureMechanism.CalculationsGroup.Children, calculationsFolder.WrappedData.Children);
            Assert.AreSame(pipingFailureMechanism.SurfaceLines, calculationsFolder.AvailablePipingSurfaceLines);
            Assert.AreSame(pipingFailureMechanism.SoilProfiles, calculationsFolder.AvailablePipingSoilProfiles);
            Assert.AreSame(pipingFailureMechanism, calculationsFolder.PipingFailureMechanism);

            var outputsFolder = (CategoryTreeFolder) children[2];
            Assert.AreEqual("Uitvoer", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
            CollectionAssert.AreEqual(new object[]
            {
                pipingFailureMechanism.AssessmentResult
            }, outputsFolder.Contents);
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GivenMultiplePipingCalculationsWithOutput_WhenClearingOutputFromContextMenu_ThenPipingOutputCleared(bool confirm)
        {
            // Given
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var pipingCalculation1 = new PipingCalculation
            {
                Output = new TestPipingOutput()
            };
            var pipingCalculation2 = new PipingCalculation
            {
                Output = new TestPipingOutput()
            };

            var observer = mocks.StrictMock<IObserver>();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();

            if (confirm)
            {
                observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
            }

            gui.Expect(cmp => cmp.Get(dataMock, treeViewControl)).Return(menuBuilder);
            dataMock.Stub(dm => dm.CalculationItems).Return(new ICalculationItem[]
            {
                pipingCalculation1,
                pipingCalculation2
            });

            mocks.ReplayAll();

            plugin.Gui = gui;

            dataMock.CalculationsGroup.Children.Clear();
            dataMock.CalculationsGroup.Children.Add(pipingCalculation1);
            dataMock.CalculationsGroup.Children.Add(pipingCalculation2);
            dataMock.CalculationsGroup.Children.ElementAt(0).Attach(observer);
            dataMock.CalculationsGroup.Children.ElementAt(1).Attach(observer);

            ContextMenuStrip contextMenuAdapter = info.ContextMenuStrip(dataMock, null, treeViewControl);

            string messageBoxTitle = null, messageBoxText = null;
            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);

                messageBoxText = messageBox.Text;
                messageBoxTitle = messageBox.Title;

                if (confirm)
                {
                    messageBox.ClickOk();
                }
                else
                {
                    messageBox.ClickCancel();
                }
            };

            // When
            contextMenuAdapter.Items[contextMenuClearIndex].PerformClick();

            // Then
            Assert.AreNotEqual(confirm, dataMock.CalculationsGroup.HasOutput);

            Assert.AreEqual("Bevestigen", messageBoxTitle);
            Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBoxText);
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_HasCalculationWithOutput_ReturnsContextMenuWithCommonItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.GetPipingCalculations().First().Output = new TestPipingOutput();

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, failureMechanism, treeViewControl);
            gui.Expect(cmp => cmp.Get(failureMechanism, treeViewControl)).Return(menuBuilder);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseForData(failureMechanism)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(failureMechanism, null, treeViewControl);

            // Assert
            Assert.AreEqual(12, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddFolderIndex, PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip, PipingFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex, PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation_Tooltip, PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 3, RingtoetsFormsResources.Validate_all, RingtoetsFormsResources.Validate_all_ToolTip, RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 4, RingtoetsFormsResources.Calculate_all, RingtoetsFormsResources.Calculate_all_ToolTip, RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearIndex, RingtoetsFormsResources.Clear_all_output, RingtoetsFormsResources.Clear_all_output_ToolTip, RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 7, CoreCommonGuiResources.Import, CoreCommonGuiResources.Import_ToolTip, CoreCommonGuiResources.ImportIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 8, CoreCommonGuiResources.Export, CoreCommonGuiResources.Export_ToolTip, CoreCommonGuiResources.ExportIcon, false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 10, CoreCommonGuiResources.Expand_all, CoreCommonGuiResources.Expand_all_ToolTip, CoreCommonGuiResources.ExpandAllIcon, false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 11, CoreCommonGuiResources.Collapse_all, CoreCommonGuiResources.Collapse_all_ToolTip, CoreCommonGuiResources.CollapseAllIcon, false);

            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[6],
                menu.Items[9]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_PipingFailureMechanismNoOutput_ClearAllOutputDisabled()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var dataMock = mocks.StrictMock<PipingFailureMechanism>();

            dataMock.Stub(dm => dm.CalculationItems).Return(new ICalculationItem[0]);

            var gui = mocks.StrictMock<IGui>();
            gui.Expect(cmp => cmp.Get(dataMock, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(dataMock, null, treeViewControl);

            // Assert
            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsFalse(clearOutputItem.Enabled);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", clearOutputItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_PipingFailureMechanismWithOutput_ClearAllOutputEnabled()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var pipingCalculation = new PipingCalculation
            {
                Output = new TestPipingOutput()
            };

            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var gui = mocks.StrictMock<IGui>();

            gui.Expect(cmp => cmp.Get(dataMock, treeViewControl)).Return(menuBuilder);
            dataMock.Stub(dm => dm.CalculationItems).Return(new ICalculationItem[] { pipingCalculation });

            mocks.ReplayAll();

            plugin.Gui = gui;

            dataMock.CalculationsGroup.Children.Add(pipingCalculation);

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(dataMock, null, treeViewControl);

            // Assert
            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsTrue(clearOutputItem.Enabled);
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output_ToolTip, clearOutputItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_PipingFailureMechanismWithNoCalculations_ValidateAndCalculateAllDisabled()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var dataMock = new PipingFailureMechanism();
            var gui = mocks.StrictMock<IGui>();

            gui.Expect(cmp => cmp.Get(dataMock, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            dataMock.CalculationsGroup.Children.Clear();

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(dataMock, null, treeViewControl);

            // Assert
            ToolStripItem validateItem = contextMenu.Items[contextMenuValidateAllIndex];
            ToolStripItem calculateItem = contextMenu.Items[contextMenuCalculateAllIndex];
            Assert.IsFalse(validateItem.Enabled);
            Assert.IsFalse(calculateItem.Enabled);
            Assert.AreEqual(PipingFormsResources.PipingFailureMechanism_CreateCalculateAllItem_No_calculations_to_run, calculateItem.ToolTipText);
            Assert.AreEqual(PipingFormsResources.PipingFailureMechanism_CreateValidateAllItem_No_calculations_to_validate, validateItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void ContextMenuStrip_Always_CallsContextMenuBuilderMethods()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = mocks.StrictMock<IContextMenuBuilder>();

            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCustomItem(null)).IgnoreArguments().Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddImportItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddExportItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddSeparator()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddExpandAllItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.AddCollapseAllItem()).Return(menuBuilder);
            menuBuilder.Expect(mb => mb.Build()).Return(null);

            gui.Expect(cmp => cmp.Get(pipingFailureMechanism, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            info.ContextMenuStrip(pipingFailureMechanism, null, treeViewControl);

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddCalculationItem_NewPipingCalculationInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var gui = mocks.StrictMock<IGui>();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Clear();
            failureMechanism.CalculationsGroup.Children.Add(new PipingCalculation());

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            gui.Expect(g => g.Get(failureMechanism, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            failureMechanism.CalculationsGroup.Attach(observerMock);

            // Precondition
            Assert.AreEqual(1, failureMechanism.CalculationsGroup.Children.Count);

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanism, null, treeViewControl);

            ToolStripItem addCalculationItem = contextMenu.Items[contextMenuAddCalculationIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);
            IPipingCalculationItem addedItem = failureMechanism.CalculationsGroup.Children.ElementAt(1);
            Assert.AreEqual("Nieuwe berekening (1)", addedItem.Name,
                "Because there is already an item with the same default name, '(1)' should be appended.");
            Assert.IsInstanceOf<PipingCalculation>(addedItem);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnAddFolderItem_NewPipingCalculationGroupInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var gui = mocks.StrictMock<IGui>();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Clear();
            failureMechanism.CalculationsGroup.Children.Add(new PipingCalculationGroup());

            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());

            gui.Expect(g => g.Get(failureMechanism, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            failureMechanism.CalculationsGroup.Attach(observerMock);

            // Precondition
            Assert.AreEqual(1, failureMechanism.CalculationsGroup.Children.Count);

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(failureMechanism, null, treeViewControl);
            ToolStripItem addCalculationItem = contextMenu.Items[contextMenuAddFolderIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.CalculationsGroup.Children.Count);
            IPipingCalculationItem addedItem = failureMechanism.CalculationsGroup.Children.ElementAt(1);
            Assert.AreEqual("Nieuwe map (1)", addedItem.Name,
                "Because there is already an item with the same default name, '(1)' should be appended.");
            Assert.IsInstanceOf<PipingCalculationGroup>(addedItem);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Clear();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            failureMechanism.CalculationsGroup.Children.Add(validCalculation);
            failureMechanism.CalculationsGroup.Children.Add(invalidCalculation);

            gui.Expect(g => g.Get(failureMechanism, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(failureMechanism, null, treeViewControl);

            // Call
            Action call = () => contextMenu.Items[contextMenuValidateAllIndex].PerformClick();

            // Assert
            TestHelper.AssertLogMessages(call, messages =>
            {
                var msgs = messages.ToArray();
                Assert.AreEqual(6, msgs.Length);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", validCalculation.Name), msgs[0]);
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", validCalculation.Name), msgs[1]);

                StringAssert.StartsWith(String.Format("Validatie van '{0}' gestart om: ", invalidCalculation.Name), msgs[2]);
                // Some validation error from validation service
                StringAssert.StartsWith(String.Format("Validatie van '{0}' beëindigd om: ", invalidCalculation.Name), msgs[5]);
            });
            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var mainWindow = mocks.Stub<IMainWindow>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Clear();

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            failureMechanism.CalculationsGroup.Children.Add(validCalculation);
            failureMechanism.CalculationsGroup.Children.Add(invalidCalculation);

            gui.Expect(g => g.Get(failureMechanism, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(failureMechanism, null, treeViewControl);

            DialogBoxHandler = (name, wnd) =>
            {
                // Don't care about dialogs in this test.
            };

            // Call
            contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

            // Assert
            mocks.VerifyAll();
        }
    }
}