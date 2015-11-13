using System;
using System.ComponentModel;
using Core.Common.Controls;
using Core.Common.Utils.Collections;
using Core.Common.TestUtils;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Workflow;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using RingtoetsFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingCalculationInputsNodePresenterTest
    {
        private MockRepository mockRepository;

        private const int contextMenuValidateIndex = 0;
        private const int contextMenuCalculateIndex = 1;
        private const int contextMenuClearIndex = 2;

        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
        }

        [Test]
        public void DefaultConstructor_ExpectedValues()
        {
            // Call
            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(PipingCalculationInputs), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string nodeName = "<Cool name>";

            var pipingNode = mockRepository.Stub<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            var pipingCalculationInputs = new PipingCalculationInputs
            {
                PipingData = new PipingData
                {
                    Name = nodeName,
                    AssessmentLevel = 2.0
                }
            };

            // Call
            nodePresenter.UpdateNode(null, pipingNode, pipingCalculationInputs);

            // Assert
            Assert.AreEqual(nodeName, pipingNode.Text);
            Assert.AreEqual(16, pipingNode.Image.Height);
            Assert.AreEqual(16, pipingNode.Image.Width);
        }

        [Test]
        public void GetChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            var pipingCalculationInputs = new PipingCalculationInputs
            {
                PipingData = new PipingData
                {
                    Output = new PipingOutput(0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
                }
            };

            // Call
            var children = nodePresenter.GetChildNodeObjects(pipingCalculationInputs, nodeMock);

            // Assert
            Assert.AreEqual(1, children.Count());
            CollectionAssert.AllItemsAreInstancesOfType(children, typeof(PipingOutput));
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void GetChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            var pipingCalculationInputs = new PipingCalculationInputs
            {
                PipingData = new PipingData()
            };

            // Call
            var children = nodePresenter.GetChildNodeObjects(pipingCalculationInputs, nodeMock);

            // Assert
            Assert.AreEqual(0, children.Count());
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnTrue()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

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

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            var renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsTrue(renameAllowed);
            mockRepository.ReplayAll(); // Expect no calls on tree node
        }

        [Test]
        public void OnNodeRenamed_Always_SetNewNameToPipingData()
        {
            // Setup
            var observerMock = mockRepository.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            mockRepository.ReplayAll();

            var pipingData = new PipingData
            {
                Name = "<Original name>"
            };
            var pipingCalculationsInputs = new PipingCalculationInputs
            {
                PipingData = pipingData
            };
            pipingCalculationsInputs.Attach(observerMock);

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            const string newName = "<Insert New Name Here>";
            nodePresenter.OnNodeRenamed(pipingCalculationsInputs, newName);

            // Assert
            Assert.AreEqual(newName, pipingData.Name);
            mockRepository.VerifyAll();
        }

        [Test]
        public void OnNodeChecked_Always_DoNothing()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            nodePresenter.OnNodeChecked(nodeMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanDrag_Always_ReturnNone()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            DragOperations dragAllowed = nodePresenter.CanDrag(dataMock);

            // Assert
            Assert.AreEqual(DragOperations.None, dragAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanDrop_Always_ReturnNone()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            var sourceMock = mockRepository.StrictMock<ITreeNode>();
            var targetMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

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
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            var sourceMock = mockRepository.StrictMock<ITreeNode>();
            var targetMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

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
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            var sourceParentNodeMock = mockRepository.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            nodePresenter.OnDragDrop(dataMock, sourceParentNodeMock, targetParentNodeDataMock, DragOperations.Move, 2);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnNodeSelected_Always_DoNothing()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_WithPipingData_ContextMenuWithThreeItems()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            dataMock.PipingData = new PipingData();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            mockRepository.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(3, contextMenu.Items.Count);
            Assert.AreEqual(RingtoetsFormsResources.Validate, contextMenu.Items[contextMenuValidateIndex].Text);

            ToolStripItem calculatePipingItem = contextMenu.Items[contextMenuCalculateIndex];
            Assert.AreEqual(RingtoetsFormsResources.Calculate, calculatePipingItem.Text);
            Assert.AreEqual(16, calculatePipingItem.Image.Height);
            Assert.AreEqual(16, calculatePipingItem.Image.Width);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.AreEqual(RingtoetsFormsResources.Clear_output, clearOutputItem.Text);
            Assert.AreEqual(16, clearOutputItem.Image.Height);
            Assert.AreEqual(16, clearOutputItem.Image.Width);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_PipingDataWithoutOutput_ContextMenuItemClearOutputDisabled()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            dataMock.PipingData = new PipingData();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            mockRepository.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(3, contextMenu.Items.Count);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsFalse(clearOutputItem.Enabled);
            Assert.AreEqual(RingtoetsFormsResources.ClearOutput_No_output_to_clear, clearOutputItem.ToolTipText);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_PipingDataWithOutput_ContextMenuItemClearOutputEnabled()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            dataMock.PipingData = new PipingData
            {
                Output = new TestPipingOutput()
            };

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            mockRepository.ReplayAll();

            // Call
            ContextMenuStrip contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(3, contextMenu.Items.Count);

            ToolStripItem clearOutputItem = contextMenu.Items[contextMenuClearIndex];
            Assert.IsTrue(clearOutputItem.Enabled);
            Assert.IsNull(clearOutputItem.ToolTipText);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var eventArgsMock = mockRepository.StrictMock<PropertyChangedEventArgs>("");
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            nodePresenter.OnPropertyChanged(dataMock, nodeMock, eventArgsMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnCollectionChange_Always_DoNothing()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();
            var eventArgsMock = mockRepository.StrictMock<NotifyCollectionChangingEventArgs>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<object>();
            var nodeMock = mockRepository.StrictMock<PipingCalculationInputs>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_Always_ThrowsInvalidOperationException()
        {
            // Setup
            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            TestDelegate removeAction = () => nodePresenter.RemoveNodeData(null, null);

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(removeAction);
            var expectedMessage = string.Format("Kan node uit boom van type {0} niet verwijderen.", nodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GivenInvalidPipingData_WhenCalculatingFromContextMenu_ThenPipingDataNotifiesObserversAndLogMessageAdded()
        {
            // Given
            var expectedValidationMessageCount = 2; // No surfaceline or soil profile selected for calculation

            var observer = mockRepository.StrictMock<IObserver>();
            mockRepository.ReplayAll();

            var calculateContextMenuItemIndex = 1;
            var pipingData = new PipingData();
            pipingData.Attach(observer);
            
            var activityRunner = new ActivityRunner();

            var nodePresenter = new PipingCalculationInputsNodePresenter
            {
                RunActivityAction = activity => activityRunner.Enqueue(activity)
            };
            var contextMenuAdapter = nodePresenter.GetContextMenu(null, new PipingCalculationInputs { PipingData = pipingData });
            
            // When
            Action action = () =>
            {
                contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();
                while (activityRunner.IsRunning)
                {
                    // Do something useful while waiting for calculation to finish...
                    Application.DoEvents();
                }
            };

            // Then
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.GetEnumerator();
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Piping' gestart om: ", msgs.Current);
                for (int i = 0; i < expectedValidationMessageCount; i++)
                {
                    Assert.IsTrue(msgs.MoveNext());
                    StringAssert.StartsWith("Validatie mislukt: ", msgs.Current);
                }
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Piping' beëindigd om: ", msgs.Current);
            });
            Assert.IsNull(pipingData.Output);
            mockRepository.VerifyAll();// Expect no calls on observer as no calculation has been performed
        }

        [Test]
        public void GivenInvalidPipingData_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            var expectedValidationMessageCount = 2; // No surfaceline or soil profile selected for calculation
            var expectedStatusMessageCount = 2;
            var expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;

            var validateContextMenuItemIndex = 0;
            var pipingData = new PipingData();
            var observer = mockRepository.StrictMock<IObserver>();
            var nodePresenter = new PipingCalculationInputsNodePresenter();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            pipingData.Attach(observer);

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(null, new PipingCalculationInputs { PipingData = pipingData });

            // When
            Action action = () => contextMenuAdapter.Items[validateContextMenuItemIndex].PerformClick();

            // Then
            TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenValidPipingData_WhenCalculatingFromContextMenu_ThenPipingDataNotifiesObservers()
        {
            // Given
            var calculateContextMenuItemIndex = 1;
            var pipingData = new PipingData();
            var validPipingInput = new TestPipingInput();
            pipingData.AssessmentLevel = validPipingInput.AssessmentLevel;
            pipingData.BeddingAngle = validPipingInput.BeddingAngle;
            pipingData.DampingFactorExit.Mean = validPipingInput.DampingFactorExit;
            pipingData.DarcyPermeability.Mean = validPipingInput.DarcyPermeability;
            pipingData.Diameter70.Mean = validPipingInput.Diameter70;
            pipingData.ExitPointXCoordinate = validPipingInput.ExitPointXCoordinate;
            pipingData.Gravity = validPipingInput.Gravity;
            pipingData.MeanDiameter70 = validPipingInput.MeanDiameter70;
            pipingData.PhreaticLevelExit.Mean = validPipingInput.PhreaticLevelExit;
            pipingData.PiezometricHeadExit = validPipingInput.PiezometricHeadExit;
            pipingData.PiezometricHeadPolder = validPipingInput.PiezometricHeadPolder;
            pipingData.SandParticlesVolumicWeight = validPipingInput.SandParticlesVolumicWeight;
            pipingData.SeepageLength.Mean = validPipingInput.SeepageLength;
            pipingData.SellmeijerModelFactor = validPipingInput.SellmeijerModelFactor;
            pipingData.SellmeijerReductionFactor = validPipingInput.SellmeijerReductionFactor;
            pipingData.ThicknessAquiferLayer.Mean = validPipingInput.ThicknessAquiferLayer;
            pipingData.ThicknessCoverageLayer.Mean = validPipingInput.ThicknessCoverageLayer;
            pipingData.UpliftModelFactor = validPipingInput.UpliftModelFactor;
            pipingData.WaterVolumetricWeight = validPipingInput.WaterVolumetricWeight;
            pipingData.WaterKinematicViscosity = validPipingInput.WaterKinematicViscosity;
            pipingData.WhitesDragCoefficient = validPipingInput.WhitesDragCoefficient;
            pipingData.SurfaceLine = validPipingInput.SurfaceLine;
            pipingData.SoilProfile = validPipingInput.SoilProfile;

            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            pipingData.Attach(observer);

            mockRepository.ReplayAll();

            var activityRunner = new ActivityRunner();

            var nodePresenter = new PipingCalculationInputsNodePresenter();
            var contextMenuAdapter = nodePresenter.GetContextMenu(null, new PipingCalculationInputs { PipingData = pipingData });
            nodePresenter.RunActivityAction = activity => activityRunner.Enqueue(activity);

            // When
            Action action = () =>
            {
                contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();
                while (activityRunner.IsRunning)
                {
                    // Do something useful while waiting for calculation to finish...
                    Application.DoEvents();
                }
            };

            // Then
            TestHelper.AssertLogMessages(action, messages =>
            {
                var msgs = messages.GetEnumerator();
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Piping' gestart om: ", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Validatie van 'Piping' beëindigd om: ", msgs.Current);

                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Berekening van 'Piping' gestart om: ", msgs.Current);
                Assert.IsTrue(msgs.MoveNext());
                StringAssert.StartsWith("Berekening van 'Piping' beëindigd om: ", msgs.Current);
            });
            Assert.IsNotNull(pipingData.Output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenPipingDataWithOutput_WhenClearingOutputFromContextMenu_ThenPipingDataOutputClearedAndNotified()
        {
            // Given
            int clearOutputItemPosition = 2;
            var pipingData = new PipingData();
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            pipingData.Output = new TestPipingOutput();
            pipingData.Attach(observer);

            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();
            var contextMenuAdapter = nodePresenter.GetContextMenu(null, new PipingCalculationInputs { PipingData = pipingData });

            // When
            contextMenuAdapter.Items[clearOutputItemPosition].PerformClick();

            // Then
            Assert.IsFalse(pipingData.HasOutput);

            mockRepository.VerifyAll();
        }
    }
}