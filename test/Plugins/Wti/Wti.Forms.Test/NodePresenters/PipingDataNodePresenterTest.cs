using System;
using System.ComponentModel;
using DelftTools.Controls;
using DelftTools.Shell.Core;
using DelftTools.TestUtils;
using DelftTools.Utils.Collections;
using NUnit.Framework;
using Rhino.Mocks;
using Wti.Calculation.Test.Piping.Stub;
using Wti.Data;
using Wti.Forms.NodePresenters;
using WtiFormsResources = Wti.Forms.Properties.Resources;

namespace Wti.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingDataNodePresenterTest
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
            var nodePresenter = new PipingDataNodePresenter();

            // Assert
            Assert.IsInstanceOf<ITreeNodePresenter>(nodePresenter);
            Assert.IsNull(nodePresenter.TreeView);
            Assert.AreEqual(typeof(PipingData), nodePresenter.NodeTagType);
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            const string nodeName = "Piping";

            var mocks = new MockRepository();
            var pipingNode = mocks.Stub<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            var pipingData = new PipingData
            {
                AssessmentLevel = 2.0
            };

            // Call
            nodePresenter.UpdateNode(null, pipingNode, pipingData);

            // Assert
            Assert.AreEqual(nodeName, pipingNode.Text);
            Assert.AreEqual(16, pipingNode.Image.Height);
            Assert.AreEqual(16, pipingNode.Image.Width);
        }

        [Test]
        public void GetChildNodeObjects_WithOutputData_ReturnOutputChildNode()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            var pipingData = new PipingData
            {
                Output = new PipingOutput(0.0, 0.0, 0.0, 0.0, 0.0, 0.0)
            };

            // Call
            var children = nodePresenter.GetChildNodeObjects(pipingData, nodeMock);

            // Assert
            Assert.AreEqual(1, children.Count());
            CollectionAssert.AllItemsAreInstancesOfType(children, typeof(PipingOutput));
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void GetChildNodeObjects_WithoutOutput_ReturnNoChildNodes()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            var pipingData = new PipingData();

            // Call
            var children = nodePresenter.GetChildNodeObjects(pipingData, nodeMock);

            // Assert
            Assert.AreEqual(0, children.Count());
            mocks.VerifyAll(); // Expect no calls on tree node
        }

        [Test]
        public void CanRenameNode_Always_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // Call
            var renameAllowed = nodePresenter.CanRenameNode(nodeMock);

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

            var nodePresenter = new PipingDataNodePresenter();

            // Call
            var renameAllowed = nodePresenter.CanRenameNodeTo(nodeMock, "<Insert New Name Here>");

            // Assert
            Assert.IsFalse(renameAllowed);
            mocks.ReplayAll(); // Expect no calls on tree node
        }


        [Test]
        public void OnNodeRenamed_Always_ThrowInvalidOperationException()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // Call
            TestDelegate call = () => { nodePresenter.OnNodeRenamed(nodeMock, "<Insert New Name Here>"); };

            // Assert
            var exception = Assert.Throws<InvalidOperationException>(call);
            var expectedMessage = string.Format("Cannot rename tree node of type {0}.", nodePresenter.GetType().Name);
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

            var nodePresenter = new PipingDataNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var sourceMock = mocks.StrictMock<ITreeNode>();
            var targetMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var sourceParentNodeMock = mocks.StrictMock<ITreeNode>();
            var targetParentNodeDataMock = mocks.StrictMock<ITreeNode>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // Call
            nodePresenter.OnNodeSelected(dataMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void GetContextMenu_Always_ContextMenuWithOneItemForCalculate()
        {
            // Setup
            var mocks = new MockRepository();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var dataMock = mocks.StrictMock<PipingData>();

            var nodePresenter = new PipingDataNodePresenter();

            mocks.ReplayAll();

            // Call
            var contextMenu = nodePresenter.GetContextMenu(nodeMock, dataMock);

            // Assert
            Assert.IsNotNull(contextMenu);
            Assert.AreEqual(2, contextMenu.Items.Count);
            Assert.AreEqual(WtiFormsResources.PipingDataContextMenuCalculate, contextMenu.Items[1].Text);
            Assert.AreEqual(WtiFormsResources.PipingDataContextMenuValidate, contextMenu.Items[0].Text);
            Assert.IsInstanceOf<PipingContextMenuStrip>(contextMenu);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void OnPropertyChange_Always_DoNothing()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<ITreeNode>();
            var eventArgsMock = mocks.StrictMock<PropertyChangedEventArgs>("");
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

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
            var dataMock = mocks.StrictMock<object>();
            var eventArgsMock = mocks.StrictMock<NotifyCollectionChangingEventArgs>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // Call
            nodePresenter.OnCollectionChanged(dataMock, eventArgsMock);

            // Assert
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanRemove_Always_ReturnTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var nodeMock = mocks.StrictMock<object>();
            mocks.ReplayAll();

            var nodePresenter = new PipingDataNodePresenter();

            // Call
            bool removalAllowed = nodePresenter.CanRemove(dataMock, nodeMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void RemoveNodeData_Always_ThrowsInvalidOperationException()
        {
            // Setup
            var nodePresenter = new PipingDataNodePresenter();

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
            var expectedValidationMessageCount = 6;
            var expectedStatusMessageCount = 2;
            var expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;

            var calculateContextMenuItemIndex = 1;
            var pipingData = new PipingData();
            var observer = mockRepository.StrictMock<IObserver>();
            var nodePresenter = new PipingDataNodePresenter();
            observer.Expect(o => o.UpdateObserver());
            pipingData.Attach(observer);

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(null, pipingData);
            
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
            var expectedValidationMessageCount = 6;
            var expectedStatusMessageCount = 2;
            var expectedLogMessageCount = expectedValidationMessageCount + expectedStatusMessageCount;

            var validateContextMenuItemIndex = 0;
            var pipingData = new PipingData();
            var observer = mockRepository.StrictMock<IObserver>();
            var nodePresenter = new PipingDataNodePresenter();
            observer.Expect(o => o.UpdateObserver()).Repeat.Never();
            pipingData.Attach(observer);

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(null, pipingData);

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

            var observer = mockRepository.StrictMock<IObserver>();
            var nodePresenter = new PipingDataNodePresenter();
            observer.Expect(o => o.UpdateObserver());
            pipingData.Attach(observer);

            mockRepository.ReplayAll();

            var contextMenuAdapter = nodePresenter.GetContextMenu(null, pipingData);

            // When
            Action action = () => contextMenuAdapter.Items[calculateContextMenuItemIndex].PerformClick();

            // Then
            TestHelper.AssertLogMessagesCount(action, expectedLogMessageCount);
            Assert.IsNotNull(pipingData.Output);
            mockRepository.VerifyAll();
        }
    }
}