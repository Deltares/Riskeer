using System;
using System.ComponentModel;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections;
using NUnit.Framework;
using Rhino.Mocks;

using Ringtoets.Piping.Data;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;

using WtiFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingCalculationInputsNodePresenterTest
    {
        private MockRepository mockRepository;

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
            var dataMock = mockRepository.StrictMock<object>();
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
            var dataMock = mockRepository.StrictMock<object>();
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
            var dataMock = mockRepository.StrictMock<object>();
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
            var dataMock = mockRepository.StrictMock<object>();
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
            var dataMock = mockRepository.StrictMock<object>();
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_ContextMenuWithOneItemForCalculate()
        {
            // Setup
            var nodeMock = mockRepository.StrictMock<ITreeNode>();
            var dataMock = mockRepository.StrictMock<PipingCalculationInputs>();

            var nodePresenter = new PipingCalculationInputsNodePresenter();

            mockRepository.ReplayAll();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(2, contextMenu.Items.Count);
            Assert.AreEqual(WtiFormsResources.PipingDataContextMenuCalculate, contextMenu.Items[1].Text);
            Assert.AreEqual(WtiFormsResources.PipingDataContextMenuValidate, contextMenu.Items[0].Text);
            Assert.IsInstanceOf<PipingContextMenuStrip>(contextMenu);
            mockRepository.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var dataMock = mockRepository.StrictMock<object>();
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
            var dataMock = mockRepository.StrictMock<object>();
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
            var nodeMock = mockRepository.StrictMock<object>();
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
            var expectedMessage = string.Format("Cannot delete node of type {0}.", nodePresenter.GetType().Name);
            Assert.AreEqual(expectedMessage, exception.Message);
        }

        [Test]
        public void GivenInvalidPipingData_WhenCalculatingFromContextMenu_ThenPipingDataNotifiesObserversAndLogMessageAdded()
        {
            // Given
            var expectedValidationMessageCount = 7;
            var expectedStatusMessageCount = 2;
            var expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;

            var calculateContextMenuItemIndex = 1;
            var pipingData = new PipingData();
            var observer = mockRepository.StrictMock<IObserver>();
            var nodePresenter = new PipingCalculationInputsNodePresenter();
            observer.Expect(o => o.UpdateObserver());
            pipingData.Attach(observer);

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(null, new PipingCalculationInputs { PipingData = pipingData });
            
            // When
            Action action = () => contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();

            // Then
            TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);
            Assert.IsNull(pipingData.Output);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GivenInvalidPipingData_WhenValidatingFromContextMenu_ThenLogMessageAddedAndNoNotifyObserver()
        {
            // Given
            var expectedValidationMessageCount = 7;
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
            var expectedCalculationStatusMessageCount = 2;
            var expectedValidationStatusMessageCount = 2;
            var expectedLogMessageCount = expectedCalculationStatusMessageCount + expectedValidationStatusMessageCount;

            var random = new Random(22);

            var calculateContextMenuItemIndex = 1;
            var pipingData = new PipingData();
            var validPipingInput = new TestPipingInput();
            pipingData.AssessmentLevel = validPipingInput.AssessmentLevel;
            pipingData.BeddingAngle = validPipingInput.BeddingAngle;
            pipingData.CriticalHeaveGradient = validPipingInput.CriticalHeaveGradient;
            pipingData.DampingFactorExit = validPipingInput.DampingFactorExit;
            pipingData.DarcyPermeability = validPipingInput.DarcyPermeability;
            pipingData.Diameter70 = validPipingInput.Diameter70;
            pipingData.ExitPointXCoordinate = validPipingInput.ExitPointXCoordinate;
            pipingData.Gravity = validPipingInput.Gravity;
            pipingData.MeanDiameter70 = validPipingInput.MeanDiameter70;
            pipingData.PhreaticLevelExit = validPipingInput.PhreaticLevelExit;
            pipingData.PiezometricHeadExit = validPipingInput.PiezometricHeadExit;
            pipingData.PiezometricHeadPolder = validPipingInput.PiezometricHeadPolder;
            pipingData.SandParticlesVolumicWeight = validPipingInput.SandParticlesVolumicWeight;
            pipingData.SeepageLength = validPipingInput.SeepageLength;
            pipingData.SellmeijerModelFactor = validPipingInput.SellmeijerModelFactor;
            pipingData.SellmeijerReductionFactor = validPipingInput.SellmeijerReductionFactor;
            pipingData.ThicknessAquiferLayer = validPipingInput.ThicknessAquiferLayer;
            pipingData.ThicknessCoverageLayer = validPipingInput.ThicknessCoverageLayer;
            pipingData.UpliftModelFactor = validPipingInput.UpliftModelFactor;
            pipingData.WaterVolumetricWeight = validPipingInput.WaterVolumetricWeight;
            pipingData.WaterKinematicViscosity = validPipingInput.WaterKinematicViscosity;
            pipingData.WhitesDragCoefficient = validPipingInput.WhitesDragCoefficient;
            pipingData.SurfaceLine = new RingtoetsPipingSurfaceLine();
            pipingData.SoilProfile = new PipingSoilProfile(String.Empty,random.NextDouble(), new []
            {
                new PipingSoilLayer(1.0) 
            });

            var observer = mockRepository.StrictMock<IObserver>();
            var nodePresenter = new PipingCalculationInputsNodePresenter();
            observer.Expect(o => o.UpdateObserver());
            pipingData.Attach(observer);

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(null, new PipingCalculationInputs { PipingData = pipingData });

            // When
            Action action = () => contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();

            // Then
            TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);
            Assert.IsNotNull(pipingData.Output);
            mockRepository.VerifyAll();
        }
    }
}