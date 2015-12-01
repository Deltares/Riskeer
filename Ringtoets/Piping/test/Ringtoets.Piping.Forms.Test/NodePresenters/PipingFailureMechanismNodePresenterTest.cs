using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtils;
using Core.Common.TestUtils;
using Core.Common.Utils.Collections;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;

using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    public class PipingFailureMechanismNodePresenterTest
    {
        private MockRepository mockRepository;
        private IContextMenuBuilderProvider contextMenuBuilderProviderMock;

        private const int contextMenuAddCalculationIndex = 0;
        private const int contextMenuAddFolderIndex = 1;
        private const int contextMenuClearIndex = 4;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
            contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();
        }

        [Test]
        public void Constructor_NoMenuBuilderProvider_ArgumentNullException()
        {
            // Call
            TestDelegate test = () => new PipingFailureMechanismNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(PipingFailureMechanism), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var mocks = new MockRepository();
            var pipingNode = mocks.Stub<ITreeNode>();
            pipingNode.ForegroundColor = Color.AliceBlue;
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            var failureMechanism = new PipingFailureMechanism();

            // Call
            nodePresenter.UpdateNode(null, pipingNode, failureMechanism);

            // Assert
            Assert.AreEqual("Dijken - Piping", pipingNode.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), pipingNode.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, pipingNode.Image);
        }

        [Test]
        public void GetChildNodeObjects_Always_ReturnChildDataNodes()
        {
            // Setup
            var mocks = new MockRepository();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.Calculations.Add(new PipingCalculation());
            pipingFailureMechanism.Calculations.Add(new PipingCalculation());

            // Call
            var children = nodePresenter.GetChildNodeObjects(pipingFailureMechanism).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(3, children.Length);
            var inputsFolder = (CategoryTreeFolder)children[0];
            Assert.AreEqual("Invoer", inputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Input, inputsFolder.Category);
            CollectionAssert.AreEqual(new object[]
            {
                pipingFailureMechanism.SectionDivisions,
                pipingFailureMechanism.SurfaceLines,
                pipingFailureMechanism.SoilProfiles,
                pipingFailureMechanism.BoundaryConditions
            }, inputsFolder.Contents);

            var calculationsFolder = (CategoryTreeFolder)children[1];
            Assert.AreEqual("Berekeningen", calculationsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.General, calculationsFolder.Category);
            var pipingCalculationChildObjects = calculationsFolder.Contents.Cast<PipingCalculationContext>()
                                                                        .Take(pipingFailureMechanism.Calculations.Count)
                                                                        .ToArray();
            CollectionAssert.AreEqual(pipingFailureMechanism.Calculations, pipingCalculationChildObjects.Select(pci => pci.WrappedData).ToArray());
            foreach (var pipingCalculationContext in pipingCalculationChildObjects)
            {
                Assert.AreSame(pipingFailureMechanism.SurfaceLines, pipingCalculationContext.AvailablePipingSurfaceLines);
                Assert.AreSame(pipingFailureMechanism.SoilProfiles, pipingCalculationContext.AvailablePipingSoilProfiles);
            }

            var outputsFolder = (CategoryTreeFolder)children[2];
            Assert.AreEqual("Uitvoer", outputsFolder.Name);
            Assert.AreEqual(TreeFolderCategory.Output, outputsFolder.Category);
            CollectionAssert.AreEqual(new object[]
            {
                pipingFailureMechanism.AssessmentResult
            }, outputsFolder.Contents);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool renameAllowed = nodePresenter.CanRenameNode(nodeMock);

            // Assert
            Assert.IsFalse(renameAllowed);
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsFalse(renameAllowed);
            mocks.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeRenamed_Always_ThrowInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate call = () => { nodePresenter.OnNodeRenamed(nodeMock, "<Insert New Name Here>"); };

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            var expectedMessage = string.Format("Kan knoop uit boom van type {0} niet hernoemen.", nodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);
            mocks.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnNodeChecked(nodeMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanDrag_Always_ReturnNone()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            DragOperations dragAllowed = nodePresenter.CanDrag(dataMock);

            // Assert
            Assert.AreEqual(DragOperations.None, dragAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrop_Always_ReturnNone()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            DragOperations dropAllowed = nodePresenter.CanDrop(dataMock, sourceMock, targetMock, DragOperations.Move);

            // Assert
            Assert.AreEqual(DragOperations.None, dropAllowed);
            mocks.VerifyAll(); // Expect no calls on mocks.
        }

        [Test]
        public void CanInsert_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool insertionAllowed = nodePresenter.CanInsert(dataMock, sourceMock, targetMock);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnDragDrop_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnDragDrop(dataMock, sourceParentNodeMock, targetParentNodeDataMock, DragOperations.Move, 2);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnNodeSelected_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnPropertyChanged(dataMock, nodeMock, eventArgsMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnCollectionChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var parentNodeDataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool removalAllowed = nodePresenter.CanRemove(parentNodeDataMock, nodeMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_PipingFailureMechanism_PipingFailureMechanismRemovedFromAssessmentSection()
        {
            // Setup
            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate call = () => nodePresenter.RemoveNodeData(new object(), new PipingFailureMechanism());

            // Assert
            Assert.Throws<InvalidOperationException>(call);
        }

        [Test]
        public void GivenMultiplePipingCalculationsWithOutput_WhenClearingOutputFromContextMenu_ThenPipingOutputCleared()
        {
            // Given
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Twice();
            var nodeMock = mocks.Stub<ITreeNode>();

            var contextMenuProvider = mocks.StrictMock<IContextMenuBuilderProvider>();
            contextMenuProvider.Expect(cmp => cmp.Get(null)).IgnoreArguments().Return(new ContextMenuBuilder(null, nodeMock));

            var dataMock = mocks.StrictMock<PipingFailureMechanism>();
            dataMock.Calculations.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });
            dataMock.Calculations.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });
            dataMock.Calculations.ElementAt(0).Attach(observer);
            dataMock.Calculations.ElementAt(1).Attach(observer);

            var nodePresenter = new PipingFailureMechanismNodePresenter(contextMenuProvider);

            mocks.ReplayAll();

            ContextMenuStrip contextMenuAdapter = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // When
            contextMenuAdapter.Items[contextMenuClearIndex].PerformClick();

            // Then
            CollectionAssert.IsEmpty(dataMock.Calculations.Where(c => c.HasOutput));

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_Always_ReturnsContextMenuWithCommonItems(bool commonItemsEnabled)
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.Stub<ITreeNode>();

            var nodePresenter = new PipingFailureMechanismNodePresenter(TestContextMenuBuilderProvider.Create(mocks, nodeMock, commonItemsEnabled));
          
            var failureMechanism = mocks.StrictMock<PipingFailureMechanism>();
            nodeMock.Tag = failureMechanism;

            mocks.ReplayAll();

            // Call
            ContextMenuStrip menu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);

            // Assert
            Assert.AreEqual(11, menu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(menu, 0, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculation_Tooltip, PipingFormsResources.PipingIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 1, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup, PipingFormsResources.PipingFailureMechanism_Add_PipingCalculationGroup_Tooltip, PipingFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 3, RingtoetsFormsResources.Calculate_all, PipingFormsResources.PipingFailureMechanism_Calculate_Tooltip, RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 6, CoreCommonGuiResources.Expand_all, CoreCommonGuiResources.Expand_all_ToolTip, CoreCommonGuiResources.ExpandAllIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(menu, 7, CoreCommonGuiResources.Collapse_all, CoreCommonGuiResources.Collapse_all_ToolTip, CoreCommonGuiResources.CollapseAllIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(menu, 9, CoreCommonGuiResources.Import, CoreCommonGuiResources.Import_ToolTip, CoreCommonGuiResources.ImportIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(menu, 10, CoreCommonGuiResources.Export, CoreCommonGuiResources.Export_ToolTip, CoreCommonGuiResources.ExportIcon, commonItemsEnabled);

            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[2], menu.Items[5], menu.Items[8] }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_PipingFailureMechanismNoOutput_ClearAllOutputDisabled()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();

            var nodePresenter = new PipingFailureMechanismNodePresenter(TestContextMenuBuilderProvider.Create(mocks, nodeMock));
          
            mocks.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(11, contextMenu.Items.Count);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsFalse(clearOutputItem.Enabled);
            Assert.AreEqual("Er zijn geen berekeningen met uitvoer om te wissen.", clearOutputItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }
        
        [Test]
        public void GetContextMenu_PipingFailureMechanismWithOutput_ClearAllOutputEnabled()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<PipingFailureMechanism>();

            dataMock.Calculations.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });

            var nodePresenter = new PipingFailureMechanismNodePresenter(TestContextMenuBuilderProvider.Create(mocks, nodeMock));

            mocks.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.AreEqual(11, contextMenu.Items.Count);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsTrue(clearOutputItem.Enabled);
            Assert.AreEqual(RingtoetsFormsResources.Clear_all_output_ToolTip, clearOutputItem.ToolTipText);

            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_ClickOnAddCalculationItem_NewPipingCalculationInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var observerMock = mocks.StrictMock<IObserver>();

            observerMock.Expect(o => o.UpdateObserver());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observerMock);

            var nodePresenter = new PipingFailureMechanismNodePresenter(TestContextMenuBuilderProvider.Create(mocks, nodeMock));

            mocks.ReplayAll();
            // Precondition
            Assert.AreEqual(1, failureMechanism.Calculations.Count);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);
            ToolStripItem addCalculationItem = contextMenu.Items[contextMenuAddCalculationIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.Calculations.Count);
            IPipingCalculationItem addedItem = failureMechanism.Calculations.ElementAt(1);
            Assert.AreEqual("Nieuwe berekening (1)", addedItem.Name);
            Assert.IsInstanceOf<PipingCalculation>(addedItem);
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddFolderItem_NewPipingCalculationGroupInstanceAddedToFailureMechanismAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var observerMock = mocks.StrictMock<IObserver>();
            
            observerMock.Expect(o => o.UpdateObserver());

            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.Attach(observerMock);
            failureMechanism.Calculations.Clear();
            failureMechanism.Calculations.Add(new PipingCalculationGroup());

            var nodePresenter = new PipingFailureMechanismNodePresenter(TestContextMenuBuilderProvider.Create(mocks, nodeMock));

            mocks.ReplayAll();

            // Precondition
            Assert.AreEqual(1, failureMechanism.Calculations.Count);

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, failureMechanism);
            ToolStripItem addCalculationItem = contextMenu.Items[contextMenuAddFolderIndex];
            addCalculationItem.PerformClick();

            // Assert
            Assert.AreEqual(2, failureMechanism.Calculations.Count);
            IPipingCalculationItem addedItem = failureMechanism.Calculations.ElementAt(1);
            Assert.AreEqual("Nieuwe map (1)", addedItem.Name);
            Assert.IsInstanceOf<PipingCalculationGroup>(addedItem);
            mocks.VerifyAll();
        }
    }
}