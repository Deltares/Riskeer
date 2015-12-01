using System;
using System.ComponentModel;
using System.Linq;

using Core.Common.Controls;
using Core.Common.Utils.Collections;
using Core.Common.TestUtils;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Gui;
using Core.Common.Gui.TestUtils;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingCalculationContextNodePresenterTest
    {
        private MockRepository mockRepository;
        private IContextMenuBuilderProvider contextMenuBuilderProviderMock;

        private const int contextMenuValidateIndex = 0;
        private const int contextMenuCalculateIndex = 1;
        private const int contextMenuClearIndex = 2;
        private const int contextMenuDeleteIndex = 4;
        private const int contextMenuExpandIndex = 6;
        private const int contextMenuCollapseIndex = 7;
        private const int contextMenuImportIndex = 9;
        private const int contextMenuExportIndex = 10;
        private const int contextMenuPropertiesIndex = 12;

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
            TestDelegate test = () => new PipingCalculationContextNodePresenter(null);

            // Assert
            var message = Assert.Throws<ArgumentNullException>(test).Message;
            StringAssert.StartsWith(CoreCommonGuiResources.NodePresenter_ContextMenuBuilderProvider_required, message);
            StringAssert.EndsWith("contextMenuBuilderProvider", message);
        }

        [Test]
        public void Constructor_WithParamsSet_NewInstance()
        {
            // Call
            var result = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<PipingCalculationContextNodePresenter>(result);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string nodeName = "<Cool name>";

            var pipingNode = mockRepository.Stub<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            var calculation = new PipingCalculation
            {
                Name = nodeName
            };
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>());

            // Call
            nodePresenter.UpdateNode(null, pipingNode, pipingCalculationContext);

            // Assert
            Assert.AreEqual(nodeName, pipingNode.Text);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.PipingIcon, pipingNode.Image);
        }

        [Test]
        public void GetChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {
            // Setup
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            var calculation = new PipingCalculation
            {
                Output = new PipingOutput(0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
            };
            var pipingCalculationContext = new PipingCalculationContext(calculation,
                                                                        new[]
                                                                        {
                                                                            new RingtoetsPipingSurfaceLine()
                                                                        },
                                                                        new[]
                                                                        {
                                                                            new TestPipingSoilProfile()
                                                                        });

            // Call
            var children = nodePresenter.GetChildNodeObjects(pipingCalculationContext).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(4, children.Length);
            Assert.AreSame(pipingCalculationContext.WrappedData.Comments, children[0]);
            var pipingInputContext = (PipingInputContext)children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedPipingInput);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSoilProfiles, pipingInputContext.AvailablePipingSoilProfiles);
            Assert.AreSame(pipingCalculationContext.WrappedData.Output, children[2]);
            Assert.AreSame(pipingCalculationContext.WrappedData.CalculationReport, children[3]);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void GetChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            var pipingCalculationContext = new PipingCalculationContext(new PipingCalculation(),
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>());
            

            // Precondition
            Assert.IsFalse(pipingCalculationContext.WrappedData.HasOutput);

            // Call
            var children = nodePresenter.GetChildNodeObjects(pipingCalculationContext).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(4, children.Length);
            Assert.AreSame(pipingCalculationContext.WrappedData.Comments, children[0]);
            var pipingInputContext = (PipingInputContext)children[1];
            Assert.AreSame(pipingCalculationContext.WrappedData.InputParameters, pipingInputContext.WrappedPipingInput);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSurfaceLines, pipingInputContext.AvailablePipingSurfaceLines);
            CollectionAssert.AreEqual(pipingCalculationContext.AvailablePipingSoilProfiles, pipingInputContext.AvailablePipingSoilProfiles);

            Assert.IsInstanceOf<EmptyPipingOutput>(children[2]);
            Assert.IsInstanceOf<EmptyPipingCalculationReport>(children[3]);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var renameAllowed = nodePresenter.CanRenameNode(nodeMock);

            // Assert
            Assert.IsTrue(renameAllowed);
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNodeTo_Always_ReturnTrue()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsTrue(renameAllowed);
            mockRepository.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeRenamed_Always_SetNewNameToPipingCalculation()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var calculation = new PipingCalculation
            {
                Name = "<Original name>"
            };
            var pipingCalculationsInputs = new PipingCalculationContext(calculation,
                                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                        Enumerable.Empty<PipingSoilProfile>());
            pipingCalculationsInputs.Attach(observerMock);

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            const string newName = "<Insert New Name Here>";
            nodePresenter.OnNodeRenamed(pipingCalculationsInputs, newName);

            // Assert
            Assert.AreEqual(newName, calculation.Name);
            mockRepository.VerifyAll();
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnNodeChecked(nodeMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanDrag_Always_ReturnNone()
        {
            // Setup
            var nodeData = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            DragOperations dragAllowed = nodePresenter.CanDrag(nodeData);

            // Assert
            Assert.AreEqual(DragOperations.None, dragAllowed);
        }

        [Test]
        public void CanDrop_Always_ReturnNone()
        {
            // Setup
            var sourceMock = mockRepository.StrictMock<ITreeNode>();
            var targetMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationContext>(new PipingCalculation(),
                                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                               Enumerable.Empty<PipingSoilProfile>());
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            DragOperations dropAllowed = nodePresenter.CanDrop(dataMock, sourceMock, targetMock, DragOperations.Move);

            // Assert
            Assert.AreEqual(DragOperations.None, dropAllowed);
            mockRepository.VerifyAll(); // Expect no calls on mockRepository.
        }

        [Test]
        public void CanInsert_Always_ReturnFalse()
        {
            // Setup
            var sourceMock = mockRepository.StrictMock<ITreeNode>();
            var targetMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationContext>(new PipingCalculation(),
                                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                               Enumerable.Empty<PipingSoilProfile>());
            mockRepository.ReplayAll();



            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool insertionAllowed = nodePresenter.CanInsert(dataMock, sourceMock, targetMock);

            // Assert
            Assert.IsFalse(insertionAllowed);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnDragDrop_Always_DoNothing()
        {
            // Setup
            var sourceParentNodeMock = mockRepository.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationContext>(new PipingCalculation(),
                                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                               Enumerable.Empty<PipingSoilProfile>());
            mockRepository.ReplayAll();



            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnDragDrop(dataMock, sourceParentNodeMock, targetParentNodeDataMock, DragOperations.Move, 2);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnNodeSelected_Always_DoNothing()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<PipingCalculationContext>(new PipingCalculation(),
                                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                               Enumerable.Empty<PipingSoilProfile>());
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void GetContextMenu_WithPipingCalculation_ContextMenuWithElevenItems(bool commonItemsEnabled)
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            var nodeData = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(TestContextMenuBuilderProvider.Create(mockRepository, nodeMock, commonItemsEnabled));

            mockRepository.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, nodeData);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(13, contextMenu.Items.Count);

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuValidateIndex, PipingFormsResources.Validate, null, PipingFormsResources.ValidationIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCalculateIndex, PipingFormsResources.Calculate, null, PipingFormsResources.Play);

            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuDeleteIndex, CoreCommonGuiResources.Delete, CoreCommonGuiResources.Delete_ToolTip, CoreCommonGuiResources.DeleteIcon);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuExpandIndex, CoreCommonGuiResources.Expand_all, CoreCommonGuiResources.Expand_all_ToolTip, CoreCommonGuiResources.ExpandAllIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuCollapseIndex, CoreCommonGuiResources.Collapse_all, CoreCommonGuiResources.Collapse_all_ToolTip, CoreCommonGuiResources.CollapseAllIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuImportIndex, CoreCommonGuiResources.Import, CoreCommonGuiResources.Import_ToolTip, CoreCommonGuiResources.ImportIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuExportIndex, CoreCommonGuiResources.Export, CoreCommonGuiResources.Export_ToolTip, CoreCommonGuiResources.ExportIcon, commonItemsEnabled);
            TestHelper.AssertContextMenuStripContainsItem(contextMenu, contextMenuPropertiesIndex, CoreCommonGuiResources.Properties, CoreCommonGuiResources.Properties_ToolTip, CoreCommonGuiResources.PropertiesIcon, commonItemsEnabled);

            Assert.IsInstanceOf<ToolStripSeparator>(contextMenu.Items[3]);
            Assert.IsInstanceOf<ToolStripSeparator>(contextMenu.Items[5]);
            Assert.IsInstanceOf<ToolStripSeparator>(contextMenu.Items[8]);
            Assert.IsInstanceOf<ToolStripSeparator>(contextMenu.Items[11]);

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_PipingCalculationWithoutOutput_ContextMenuItemClearOutputDisabled()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            var nodeData = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(TestContextMenuBuilderProvider.Create(mockRepository, nodeMock, true));

            mockRepository.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, nodeData);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(13, contextMenu.Items.Count);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsFalse(clearOutputItem.Enabled);
            Assert.AreEqual(PipingFormsResources.ClearOutput_No_output_to_clear, clearOutputItem.ToolTipText);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_PipingCalculationWithOutput_ContextMenuItemClearOutputEnabled()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();

            var calculation = new PipingCalculation
            {
                Output = new TestPipingOutput()
            };
            var nodeData = new PipingCalculationContext(calculation,
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(TestContextMenuBuilderProvider.Create(mockRepository, nodeMock, true));

            mockRepository.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, nodeData);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(13, contextMenu.Items.Count);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsTrue(clearOutputItem.Enabled);
            Assert.IsNull(clearOutputItem.ToolTipText);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var eventArgsMock = mockRepository.StrictMock<PropertyChangedEventArgs>("");
            mockRepository.ReplayAll();

            var dataMock = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnPropertyChanged(dataMock, nodeMock, eventArgsMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnCollectionChange_Always_DoNothing()
        {
            // Setup
            var eventArgsMock = mockRepository.StrictMock<NotifyCollectionChangingEventArgs>();
            mockRepository.ReplayAll();

            var dataMock = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationsTreeFolder_ReturnTrue()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingCalculationsFolder = new PipingCalculationsTreeFolder("Berekeningen", pipingFailureMechanism);

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool removalAllowed = nodePresenter.CanRemove(pipingCalculationsFolder, pipingCalculationsFolder.Contents.OfType<object>().First());

            // Assert
            Assert.IsTrue(removalAllowed);
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationsTreeFolderForCalculationNotInFolder_ReturnFalse()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var pipingCalculationsFolder = new PipingCalculationsTreeFolder("Berekeningen", pipingFailureMechanism);

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            var calculationContext = new PipingCalculationContext(new PipingCalculation(),
                                                                  Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                  Enumerable.Empty<PipingSoilProfile>());

            // Call
            bool removalAllowed = nodePresenter.CanRemove(pipingCalculationsFolder, calculationContext);

            // Assert
            Assert.IsFalse(removalAllowed);
        }

        [Test]
        public void CanRemove_EverythingElse_ReturnFalse()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<object>();
            mockRepository.ReplayAll();

            var nodeMock = new PipingCalculationContext(new PipingCalculation(),
                                                        Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                        Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_ParentIsPipingCalculationsTreeFolder_RemoveCalculationFromFailureMechanism()
        {
            // Setup
            var mocks = new MockRepository();
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            var elementToBeRemoved = new PipingCalculation();

            var pipingFailureMechanism = new PipingFailureMechanism();
            pipingFailureMechanism.Calculations.Add(elementToBeRemoved);
            pipingFailureMechanism.Calculations.Add(new PipingCalculation());
            pipingFailureMechanism.Attach(observer);

            var pipingCalculationsFolder = new PipingCalculationsTreeFolder("Berekeningen", pipingFailureMechanism);

            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Precondition
            var treeElementToRemove = pipingCalculationsFolder.Contents.OfType<object>().ElementAt(1);
            Assert.IsTrue(nodePresenter.CanRemove(pipingCalculationsFolder, treeElementToRemove));
            Assert.AreEqual(3, pipingFailureMechanism.Calculations.Count);

            // Call
            bool removalSuccesful = nodePresenter.RemoveNodeData(pipingCalculationsFolder, treeElementToRemove);

            // Assert
            Assert.IsTrue(removalSuccesful);
            Assert.AreEqual(2, pipingFailureMechanism.Calculations.Count);
            CollectionAssert.DoesNotContain(pipingFailureMechanism.Calculations, elementToBeRemoved);

            mocks.VerifyAll();
        }

        [Test]
        public void RemoveNodeData_Always_ThrowsInvalidOperationException()
        {
            // Setup
            var nodePresenter = new PipingCalculationContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            TestDelegate removeAction = () => nodePresenter.RemoveNodeData(null, null);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(removeAction);
            var expectedMessage = string.Format("Kan knoop uit boom van type {0} niet verwijderen.", nodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GivenInvalidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObserversAndLogMessageAdded()
        {
            // Given
            var expectedValidationMessageCount = 2; // No surfaceline or soil profile selected for calculation

            var observer = mockRepository.StrictMock<IObserver>();

            var calculateContextMenuItemIndex = 1;
            var calculation = new PipingCalculation();
            calculation.Attach(observer);

            ITreeNode treeNodeMock = mockRepository.StrictMock<ITreeNode>();

            var nodePresenter = new PipingCalculationContextNodePresenter(TestContextMenuBuilderProvider.Create(mockRepository, treeNodeMock, true))
            {
                RunActivityAction = activity =>
                {
                    activity.Run();
                    activity.Finish();
                }
            };

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(treeNodeMock, new PipingCalculationContext(calculation,
                                                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingSoilProfile>()));


            // When
            Action action = () =>
            {
                contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();
            };

            // Then
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.GetEnumerator();
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Nieuwe berekening' gestart om: ", msgs.Current);
                for (int i = 0; i < expectedValidationMessageCount; i++)
                {
                    Assert.IsTrue(msgs.MoveNext());
                    StringAssert.StartsWith("Validatie mislukt: ", msgs.Current);
                }
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Nieuwe berekening' beëindigd om: ", msgs.Current);
            });
            Assert.IsNull(calculation.Output);
            mockRepository.VerifyAll();// Expect no calls on observer as no calculation has been performed
        }

        [Test]
        public void GivenInvalidPipingCalculation_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            var expectedValidationMessageCount = 2; // No surfaceline or soil profile selected for calculation
            var expectedStatusMessageCount = 2;
            var expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;

            var validateContextMenuItemIndex = 0;
            var calculation = new PipingCalculation();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            calculation.Attach(observer);

            ITreeNode treeNodeMock = mockRepository.StrictMock<ITreeNode>();

            var nodePresenter = new PipingCalculationContextNodePresenter(TestContextMenuBuilderProvider.Create(mockRepository, treeNodeMock, true));

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(treeNodeMock, new PipingCalculationContext(calculation,
                                                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingSoilProfile>()));

            // When
            Action action = () => contextMenuAdapter.Items[validateContextMenuItemIndex].PerformClick();

            // Then
            TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenValidPipingCalculation_WhenCalculatingFromContextMenu_ThenPipingCalculationNotifiesObservers()
        {
            // Given
            var calculateContextMenuItemIndex = 1;
            var calculation = new PipingCalculation();
            var validPipingInput = new TestPipingInput();
            calculation.InputParameters.AssessmentLevel = validPipingInput.AssessmentLevel;
            calculation.InputParameters.BeddingAngle = validPipingInput.BeddingAngle;
            calculation.InputParameters.DampingFactorExit.Mean = validPipingInput.DampingFactorExit;
            calculation.InputParameters.DarcyPermeability.Mean = validPipingInput.DarcyPermeability;
            calculation.InputParameters.Diameter70.Mean = validPipingInput.Diameter70;
            calculation.InputParameters.ExitPointXCoordinate = validPipingInput.ExitPointXCoordinate;
            calculation.InputParameters.Gravity = validPipingInput.Gravity;
            calculation.InputParameters.MeanDiameter70 = validPipingInput.MeanDiameter70;
            calculation.InputParameters.PhreaticLevelExit.Mean = validPipingInput.PhreaticLevelExit;
            calculation.InputParameters.PiezometricHeadExit = validPipingInput.PiezometricHeadExit;
            calculation.InputParameters.PiezometricHeadPolder = validPipingInput.PiezometricHeadPolder;
            calculation.InputParameters.SandParticlesVolumicWeight = validPipingInput.SandParticlesVolumicWeight;
            calculation.InputParameters.SeepageLength.Mean = validPipingInput.SeepageLength;
            calculation.InputParameters.SellmeijerModelFactor = validPipingInput.SellmeijerModelFactor;
            calculation.InputParameters.SellmeijerReductionFactor = validPipingInput.SellmeijerReductionFactor;
            calculation.InputParameters.ThicknessAquiferLayer.Mean = validPipingInput.ThicknessAquiferLayer;
            calculation.InputParameters.ThicknessCoverageLayer.Mean = validPipingInput.ThicknessCoverageLayer;
            calculation.InputParameters.UpliftModelFactor = validPipingInput.UpliftModelFactor;
            calculation.InputParameters.WaterVolumetricWeight = validPipingInput.WaterVolumetricWeight;
            calculation.InputParameters.WaterKinematicViscosity = validPipingInput.WaterKinematicViscosity;
            calculation.InputParameters.WhitesDragCoefficient = validPipingInput.WhitesDragCoefficient;
            calculation.InputParameters.SurfaceLine = validPipingInput.SurfaceLine;
            calculation.InputParameters.SoilProfile = validPipingInput.SoilProfile;

            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            calculation.Attach(observer);

            ITreeNode treeNodeMock = mockRepository.StrictMock<ITreeNode>();

            var nodePresenter = new PipingCalculationContextNodePresenter(TestContextMenuBuilderProvider.Create(mockRepository, treeNodeMock, true));

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(treeNodeMock, new PipingCalculationContext(calculation,
                                                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingSoilProfile>()));
            nodePresenter.RunActivityAction = activity =>
            {
                activity.Run();
                activity.Finish();
            };

            // When
            Action action = () =>
            {
                contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();
            };

            // Then
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.GetEnumerator();
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Nieuwe berekening' gestart om: ", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Nieuwe berekening' beëindigd om: ", msgs.Current);

                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Berekening van 'Nieuwe berekening' gestart om: ", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Berekening van 'Nieuwe berekening' beëindigd om: ", msgs.Current);
            });
            Assert.IsNotNull(calculation.Output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPipingCalculationWithOutput_WhenClearingOutputFromContextMenu_ThenPipingCalculationOutputClearedAndNotified()
        {
            // Given
            int clearOutputItemPosition = 2;
            var calculation = new PipingCalculation();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            calculation.Output = new TestPipingOutput();
            calculation.Attach(observer);

            ITreeNode treeNodeMock = mockRepository.StrictMock<ITreeNode>();

            var nodePresenter = new PipingCalculationContextNodePresenter(TestContextMenuBuilderProvider.Create(mockRepository, treeNodeMock, true));

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(treeNodeMock, new PipingCalculationContext(calculation,
                                                                                                     Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                                                     Enumerable.Empty<PipingSoilProfile>()));

            // When
            contextMenuAdapter.Items[clearOutputItemPosition].PerformClick();

            // Then
            Assert.IsFalse(calculation.HasOutput);

            mockRepository.VerifyAll();
        }
    }
}