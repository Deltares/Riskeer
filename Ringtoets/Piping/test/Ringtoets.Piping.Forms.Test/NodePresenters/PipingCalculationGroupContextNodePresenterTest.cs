using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Base.Service;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
using NUnit.Extensions.Forms;
using NUnit.Framework;

using Rhino.Mocks;

using Ringtoets.Common.Forms.NodePresenters;
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.NodePresenters;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Service;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.NodePresenters
{
    [TestFixture]
    public class PipingCalculationGroupContextNodePresenterTest : NUnitFormTest
    {
        private MockRepository mockRepository;

        private const int contextMenuAddCalculationGroupIndex = 0;
        private const int contextMenuAddCalculationIndex = 1;
        private const int contextMenuValidateAllIndex = 3;
        private const int contextMenuCalculateAllIndex = 4;
        private const int contextMenuClearOutputIndex = 5;
        
        [SetUp]
        public void SetUp()
        {
            mockRepository = new MockRepository();
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
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            // Call
            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Assert
            Assert.IsInstanceOf<RingtoetsNodePresenterBase<PipingCalculationGroupContext>>(nodePresenter);
            Assert.AreEqual(typeof(PipingCalculationGroupContext), nodePresenter.NodeTagType);
            Assert.IsNull(nodePresenter.TreeView);

            mockRepository.VerifyAll();
        }

        [Test]
        public void UpdateNode_WithData_InitializeNode()
        {
            // Setup
            var parentNode = mockRepository.StrictMock<ITreeNode>();
            var node = mockRepository.Stub<ITreeNode>();
            node.ForegroundColor = Color.AliceBlue;
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();
            
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            nodePresenter.UpdateNode(parentNode, node, nodeData);

            // Assert
            Assert.AreEqual(group.Name, node.Text);
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), node.ForegroundColor);
            TestHelper.AssertImagesAreEqual(PipingFormsResources.FolderIcon, node.Image);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanDrag_DefaultBehavior_ReturnMove()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());
            var otherTreeNode = mockRepository.Stub<ITreeNode>();

            var groupContextNode = mockRepository.Stub<ITreeNode>();
            groupContextNode.Tag = groupContext;
            groupContextNode.Expect(n => n.Parent).Return(otherTreeNode);

            var builderProvider = mockRepository.Stub<IContextMenuBuilderProvider>();

            var treeView = mockRepository.StrictMock<ITreeView>();
            treeView.Expect(v => v.GetNodeByTag(groupContext)).Return(groupContextNode);
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(builderProvider)
            {
                TreeView = treeView
            };

            // Call
            DragOperations supportedOperation = nodePresenter.CanDrag(groupContext);

            // Assert
            Assert.AreEqual(DragOperations.Move, supportedOperation);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanDrag_ParentIsPipingFailureMechanism_ReturnNone()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();

            var group = pipingFailureMechanism.CalculationsGroup;
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var builderProvider = mockRepository.Stub<IContextMenuBuilderProvider>();

            var failureMechanismNode = mockRepository.Stub<ITreeNode>();
            failureMechanismNode.Tag = pipingFailureMechanism;

            var groupContextNode = mockRepository.Stub<ITreeNode>();
            groupContextNode.Tag = groupContext;
            groupContextNode.Expect(n => n.Parent).Return(failureMechanismNode);

            var treeView = mockRepository.StrictMock<ITreeView>();
            treeView.Expect(v => v.GetNodeByTag(groupContext)).Return(groupContextNode);
            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(builderProvider)
            {
                TreeView = treeView
            };

            // Call
            DragOperations supportedOperation = nodePresenter.CanDrag(groupContext);

            // Assert
            Assert.AreEqual(DragOperations.None, supportedOperation);
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void CanDropOrCanInsert_DraggingPipingCalculationItemContextOntoGroupNotContainingItem_ReturnMoveOrTrue(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);

            PipingCalculationGroup targetGroup;
            PipingCalculationGroupContext targetGroupContext;
            CreatePipingCalculationGroupAndContext(out targetGroup, out targetGroupContext);
            
            var failureMechanism = new PipingFailureMechanism();
            failureMechanism.CalculationsGroup.Children.Add(draggedItem);
            failureMechanism.CalculationsGroup.Children.Add(targetGroup);

            #region Mock node tree for source and target in same failure mechanism

            var failureMechanismNode = mockRepository.Stub<ITreeNode>();
            failureMechanismNode.Tag = failureMechanism;

            var failureMechanismGroupNode = mockRepository.Stub<ITreeNode>();
            failureMechanismGroupNode.Tag = failureMechanism.CalculationsGroup;
            failureMechanismGroupNode.Expect(n => n.Parent).Return(failureMechanismNode).Repeat.AtLeastOnce();

            var calculationNode = mockRepository.Stub<ITreeNode>();
            calculationNode.Tag = draggedItemContext;
            calculationNode.Expect(n => n.Parent).Return(failureMechanismGroupNode).Repeat.AtLeastOnce();

            var targetGroupNode = mockRepository.Stub<ITreeNode>();
            targetGroupNode.Tag = targetGroupContext;
            targetGroupNode.Expect(n => n.Parent).Return(failureMechanismGroupNode).Repeat.AtLeastOnce();

            #endregion

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    DragOperations supportedOperations = nodePresenter.CanDrop(draggedItemContext, calculationNode, targetGroupNode, DragOperations.Move);

                    // Assert
                    Assert.AreEqual(DragOperations.Move, supportedOperations);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = nodePresenter.CanInsert(draggedItemContext, calculationNode, targetGroupNode);

                    // Assert
                    Assert.IsTrue(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void CanDropOrInsert_DraggingCalculationItemContextOntoGroupNotContainingItemOtherFailureMechanism_ReturnNoneOrFalse(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);

            var sourceFailureMechanism = new PipingFailureMechanism();
            sourceFailureMechanism.CalculationsGroup.Children.Add(draggedItem);

            PipingCalculationGroup targetGroup;
            PipingCalculationGroupContext targetGroupContext;
            CreatePipingCalculationGroupAndContext(out targetGroup, out targetGroupContext);

            var targetFailureMechanism = new PipingFailureMechanism();
            targetFailureMechanism.CalculationsGroup.Children.Add(targetGroup);

            #region Mock node tree for source IPipingCalculationItem

            var sourceFailureMechanismNode = mockRepository.Stub<ITreeNode>();
            sourceFailureMechanismNode.Tag = sourceFailureMechanism;

            var sourceFailureMechanismGroupNode = mockRepository.Stub<ITreeNode>();
            sourceFailureMechanismGroupNode.Tag = sourceFailureMechanism.CalculationsGroup;
            sourceFailureMechanismGroupNode.Expect(n => n.Parent).Return(sourceFailureMechanismNode).Repeat.AtLeastOnce();

            var calculationNode = mockRepository.Stub<ITreeNode>();
            calculationNode.Tag = draggedItemContext;
            calculationNode.Expect(n => n.Parent).Return(sourceFailureMechanismGroupNode).Repeat.AtLeastOnce();

            #endregion

            #region Mock node tree for target PipingCalculationGroup

            var targetFailureMechanismNode = mockRepository.Stub<ITreeNode>();
            targetFailureMechanismNode.Tag = targetFailureMechanism;

            var targetFailureMechanismGroupNode = mockRepository.Stub<ITreeNode>();
            targetFailureMechanismGroupNode.Tag = targetFailureMechanism.CalculationsGroup;
            targetFailureMechanismGroupNode.Expect(n => n.Parent).Return(targetFailureMechanismNode).Repeat.AtLeastOnce();

            var groupNode = mockRepository.Stub<ITreeNode>();
            groupNode.Tag = targetGroupContext;
            groupNode.Expect(n => n.Parent).Return(targetFailureMechanismGroupNode).Repeat.AtLeastOnce();

            #endregion

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    DragOperations supportedOperations = nodePresenter.CanDrop(draggedItemContext, calculationNode, groupNode, DragOperations.Move);

                    // Assert
                    Assert.AreEqual(DragOperations.None, supportedOperations);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = nodePresenter.CanInsert(draggedItemContext, calculationNode, groupNode);

                    // Assert
                    Assert.IsFalse(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDragDrop_DraggingPipingCalculationItemContextOntoGroupEnd_MoveCalculationItemInstanceToNewGroup(
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);

            PipingCalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext);
            originalOwnerGroup.Children.Add(draggedItem);

            PipingCalculationGroup newOwnerGroup;
            PipingCalculationGroupContext newOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext);

            var originalOwnerObserver = mockRepository.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var updatewasCalled = false;
            var newOwnerObserver = CreateObserverStubWithUpdateExpectancy(invocation => updatewasCalled = true);

            var preUpdateDraggedItemContextNode = CreateCollapsedNodeStub(draggedItemContext, new ITreeNode[0]);
            var postUpdateDraggedItemContextNode = CreateNodeStubToBeCollapsed(draggedItemContext, new ITreeNode[0]);
            var newOwnerGroupContextNode = CreateNodeStubToBeExpanded(newOwnerGroupContext, new ITreeNode[0], invocation =>
            {
                if (updatewasCalled)
                {
                    invocation.ReturnValue = new[] { postUpdateDraggedItemContextNode };
                }
            });
            postUpdateDraggedItemContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);

            var treeView = mockRepository.Stub<ITreeView>();
            treeView.Expect(v => v.GetNodeByTag(draggedItemContext)).WhenCalled(invocation =>
            {
                if (updatewasCalled)
                {
                    invocation.ReturnValue = postUpdateDraggedItemContextNode;
                }
            }).Return(preUpdateDraggedItemContextNode).Repeat.Twice();
            treeView.Expect(v => v.GetNodeByTag(newOwnerGroupContext)).Return(newOwnerGroupContextNode);

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            // Precondition:
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);
            
            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock)
            {
                TreeView = treeView
            };

            // Call
            nodePresenter.OnDragDrop(draggedItemContext, originalOwnerGroupContext, newOwnerGroupContext, DragOperations.Move, newOwnerGroup.Children.Count);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.Last(),
                "Dragging node at the end of the target PipingCalculationGroup should put the dragged data at the end of 'newOwnerGroup'.");

            Assert.AreSame(postUpdateDraggedItemContextNode, treeView.SelectedNode);

            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDragDrop_InsertingPipingCalculationItemContextAtDifferentLocationWithingSameGroup_ChangeItemIndexOfCalculationItem(
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType,
            [Values(0, 2)] int newIndex)
        {
            // Setup
            const string name = "Very cool name";
            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, name);

            var existingItemStub = mockRepository.Stub<IPipingCalculationItem>();
            existingItemStub.Stub(i => i.Name).Return("");

            PipingCalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext);
            originalOwnerGroup.Children.Add(existingItemStub);
            originalOwnerGroup.Children.Add(draggedItem);
            originalOwnerGroup.Children.Add(existingItemStub);

            bool updatewasCalled = false;
            var originalOwnerObserver = CreateObserverStubWithUpdateExpectancy(invocation => updatewasCalled = true);
            
            var existingItemStub1 = CreateStubTreeNode();
            var existingItemStub2 = CreateStubTreeNode();
            
            var preUpdateDraggedItemContextNode = CreateTreeNodeLeafForData(draggedItemContext);
            var postUpdateDraggedItemContextNode = CreateTreeNodeLeafForData(draggedItemContext);
            ITreeNode[] preUpdateNewOwnerChildNodes = { existingItemStub1, preUpdateDraggedItemContextNode, existingItemStub2 };
            var newOwnerGroupContextNode = CreateNodeStubToBeExpanded(originalOwnerGroupContext, preUpdateNewOwnerChildNodes, invocation =>
            {
                if (updatewasCalled)
                {
                    invocation.ReturnValue = new[]
                    {
                        existingItemStub1,
                        postUpdateDraggedItemContextNode,
                        existingItemStub2
                    };
                }
            });
            postUpdateDraggedItemContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);

            // Dragging within same group requires only recording state on that group.
            var treeView = mockRepository.Stub<ITreeView>();
            treeView.Expect(v => v.GetNodeByTag(originalOwnerGroupContext)).Return(newOwnerGroupContextNode);
            treeView.Expect(v => v.GetNodeByTag(draggedItemContext)).WhenCalled(invocation =>
            {
                if (updatewasCalled)
                {
                    invocation.ReturnValue = postUpdateDraggedItemContextNode;
                }
            }).Return(null);

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            originalOwnerGroup.Attach(originalOwnerObserver);

            // Precondition:
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock)
            {
                TreeView = treeView
            };

            // Call
            nodePresenter.OnDragDrop(draggedItemContext, originalOwnerGroupContext, originalOwnerGroupContext, DragOperations.Move, newIndex);

            // Assert
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            Assert.AreNotSame(draggedItem, originalOwnerGroup.Children[1],
                "Should have removed 'draggedItem' from its original location in the collection.");
            Assert.AreSame(draggedItem, originalOwnerGroup.Children[newIndex],
                "Dragging node to specific location within owning PipingCalculationGroup should put the dragged data at that index.");
            Assert.AreEqual(name, draggedItem.Name,
                "No renaming should occur when dragging within the same PipingCalculationGroup.");

            Assert.AreSame(postUpdateDraggedItemContextNode, treeView.SelectedNode);

            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDragDrop_DraggingPipingCalculationItemContextOntoGroupStartWithSameNamedItem_MoveCalculationItemInstanceToNewGroupAndRename(
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);

            PipingCalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext);
            originalOwnerGroup.Children.Add(draggedItem);

            PipingCalculationGroup newOwnerGroup;
            PipingCalculationGroupContext newOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext);

            var sameNamedItem = mockRepository.Stub<IPipingCalculationItem>();
            sameNamedItem.Stub(i => i.Name).Return(draggedItem.Name);

            var updateWasCalled = false;
            var originalOwnerObserver = CreateObserverStubWithUpdateExpectancy();
            var newOwnerObserver = CreateObserverStubWithUpdateExpectancy(invocation => updateWasCalled = true);

            var preUpdateCalculationContextNode = CreateExpandedNodeStub(draggedItemContext, new ITreeNode[0]);
            var postUpdateCalculationContextNode = CreateNodeStubToBeExpanded(draggedItemContext, new ITreeNode[0]);
            var newOwnerGroupContextNode = CreateNodeStubToBeExpanded(originalOwnerGroupContext, new ITreeNode[0], invocation =>
            {
                if (updateWasCalled)
                {
                    invocation.ReturnValue = new[]
                    {
                        postUpdateCalculationContextNode
                    };
                }
            });
            postUpdateCalculationContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);

            var treeView = mockRepository.Stub<ITreeView>();
            treeView.Expect(v => v.GetNodeByTag(draggedItemContext)).WhenCalled(invocation =>
            {
                if (updateWasCalled)
                {
                    invocation.ReturnValue = postUpdateCalculationContextNode;
                }
            }).Return(preUpdateCalculationContextNode).Repeat.Twice();
            treeView.Expect(v => v.GetNodeByTag(newOwnerGroupContext)).Return(newOwnerGroupContextNode);
            treeView.Expect(v => v.StartLabelEdit());

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            newOwnerGroup.Children.Add(sameNamedItem);

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            // Precondition:
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children.Select(c => c.Name), draggedItem.Name,
                "Name of the dragged item should already exist in new owner.");

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock)
            {
                TreeView = treeView
            };

            // Call
            nodePresenter.OnDragDrop(draggedItemContext, originalOwnerGroupContext, newOwnerGroupContext, DragOperations.Move, 0);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.First(),
                "Dragging to insert node at start of newOwnerGroup should place the node at the start of the list.");
            switch (draggedItemType)
            {
                case PipingCalculationItemType.Calculation:
                    Assert.AreEqual("Nieuwe berekening (1)", draggedItem.Name);
                    break;
                case PipingCalculationItemType.Group:
                    Assert.AreEqual("Nieuwe map (1)", draggedItem.Name);
                    break;
            }
            
            Assert.AreSame(postUpdateCalculationContextNode, treeView.SelectedNode);

            mockRepository.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDragDrop_DraggingPipingCalculationItemContextOntoGroupWithOtherItems_ExpandedCollapsedStateOfOtherItemsRestored(
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);

            PipingCalculationGroup originalOwnerGroup;
            PipingCalculationGroupContext originalOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext);
            originalOwnerGroup.Children.Add(draggedItem);

            PipingCalculationGroup newOwnerGroup;
            PipingCalculationGroupContext newOwnerGroupContext;
            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext);

            var updatewasCalled = false;
            var originalOwnerObserver = CreateObserverStubWithUpdateExpectancy();
            var newOwnerObserver = CreateObserverStubWithUpdateExpectancy(methodInvocation => updatewasCalled = true);

            var nodeStub = CreateStubTreeNode();
            ITreeNode[] childNodesStub = { nodeStub };

            var expandedNodeData = new object();
            var preUpdateExpandedNode = CreateExpandedNodeStub(expandedNodeData, childNodesStub);
            var postUpdateExpandedNode = CreateNodeStubToBeExpanded(expandedNodeData, childNodesStub);

            var collapsedNodeData = new object();
            var preUpdateCollapsedNode = CreateCollapsedNodeStub(collapsedNodeData, childNodesStub);
            var postUpdateCollapsedNode = CreateNodeStubToBeCollapsed(collapsedNodeData, childNodesStub);

            ITreeNode[] preUpdateChildNodes = { preUpdateExpandedNode, preUpdateCollapsedNode };
            ITreeNode[] postUpdateChildNodes = { postUpdateExpandedNode, postUpdateCollapsedNode };
            var newOwnerGroupContextNode = CreateNodeStubToBeExpanded(newOwnerGroupContext, preUpdateChildNodes, invocation =>
            {
                if (updatewasCalled)
                {
                    invocation.ReturnValue = postUpdateChildNodes;
                }
            });

            var preUpdateDraggedItemContextNode = CreateTreeNodeLeafForData(draggedItemContext);
            var postUpdateDraggedItemContextNode = CreateNodeStubToBeCollapsed(draggedItemContext, new ITreeNode[0]);
            postUpdateDraggedItemContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);

            var treeView = mockRepository.Stub<ITreeView>();
            treeView.Expect(v => v.GetNodeByTag(draggedItemContext)).WhenCalled(invocation =>
            {
                if (updatewasCalled)
                {
                    invocation.ReturnValue = postUpdateDraggedItemContextNode;
                }
            }).Return(preUpdateDraggedItemContextNode).Repeat.Twice();
            treeView.Expect(v => v.GetNodeByTag(newOwnerGroupContext)).Return(newOwnerGroupContextNode);

            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            // Precondition:
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock)
            {
                TreeView = treeView
            };

            // Call
            nodePresenter.OnDragDrop(draggedItemContext, originalOwnerGroupContext, newOwnerGroupContext, DragOperations.Move, newOwnerGroup.Children.Count);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.Last(),
                "Dragging node at the end of the target PipingCalculationGroup should put the dragged data at the end of 'newOwnerGroup'.");

            Assert.AreSame(postUpdateDraggedItemContextNode, treeView.SelectedNode);

            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRenameNode_ParentIsPipingFailureMechanismNode_ReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();
            
            var parentNode = mockRepository.Stub<ITreeNode>();
            parentNode.Tag = failureMechanism;

            var node = mockRepository.StrictMock<ITreeNode>();
            node.Expect(n => n.Parent).Return(parentNode).Repeat.Twice();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(node);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRenameNode_EverythingElse_ReturnTrue()
        {
            // Setup
            var parentNode = mockRepository.Stub<ITreeNode>();
            parentNode.Tag = new object();

            var node = mockRepository.StrictMock<ITreeNode>();
            node.Expect(n => n.Parent).Return(parentNode).Repeat.Twice();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNode(node);

            // Assert
            Assert.IsTrue(isRenamingAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRenamedNodeTo_Always_ReturnTrue()
        {
            // Setup
            var node = mockRepository.StrictMock<ITreeNode>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRenamingAllowed = nodePresenter.CanRenameNodeTo(node, "newName");

            // Assert
            Assert.IsTrue(isRenamingAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void OnNodeRenamed_WithData_RenameGroupAndNotifyObservers()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());
            nodeData.Attach(observer);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            const string newName = "new name";
            nodePresenter.OnNodeRenamed(nodeData, newName);

            // Assert
            Assert.AreEqual(newName, group.Name);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsFailureMechanism_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var parentNodeData = new PipingFailureMechanism();
            parentNodeData.CalculationsGroup.Children.Add(group);
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(parentNodeData, nodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupContainingGroup_ReturnTrue()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var parentGroup = new PipingCalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(parentNodeData, nodeData);

            // Assert
            Assert.IsTrue(isRemovalAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupNotContainingGroup_ReturnFalse()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var parentGroup = new PipingCalculationGroup();
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Precondition
            CollectionAssert.DoesNotContain(parentGroup.Children, group);

            // Call
            bool isRemovalAllowed = nodePresenter.CanRemove(parentNodeData, nodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
            mockRepository.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var parentGroup = new PipingCalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());
            parentNodeData.Attach(observer);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Precondition
            Assert.IsTrue(nodePresenter.CanRemove(parentNodeData, nodeData));

            // Call
            bool removealSuccesful = nodePresenter.RemoveNodeData(parentNodeData, nodeData);

            // Assert
            Assert.IsTrue(removealSuccesful);
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ValidDataWithCalculationOutput_ReturnContextWithItems()
        {
            // Setup
            var parentData = new object();

            var group = new PipingCalculationGroup();
            group.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var presenter = mockRepository.Stub<ITreeNodePresenter>();

            var nodeParent = mockRepository.Stub<ITreeNode>();
            nodeParent.Tag = parentData;

            var node = mockRepository.Stub<ITreeNode>();
            node.Tag = group;
            node.Presenter = presenter;
            node.Stub(n => n.Parent).Return(nodeParent);
            node.Stub(n => n.Nodes).Return(new ITreeNode[0]);

            presenter.Expect(p => p.CanRemove(parentData, group)).Return(true);

            var guiCommandHandler = mockRepository.Stub<IGuiCommandHandler>();

            mockRepository.ReplayAll();

            var menuBuilder = new ContextMenuBuilder(guiCommandHandler, node);

            var contextMenuBuilderProvider = new SimpleContextMenuBuilderProvider(menuBuilder);



            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProvider);

            // Call
            ContextMenuStrip menu = nodePresenter.GetContextMenu(node, nodeData);

            // Assert
            Assert.AreEqual(16, menu.Items.Count);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationGroupIndex,
                                                          PipingFormsResources.PipingCalculationGroup_Add_PipingCalculationGroup,
                                                          "Voeg een nieuwe berekeningsmap toe aan deze berekeningsmap.",
                                                          PipingFormsResources.AddFolderIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuAddCalculationIndex,
                                                          PipingFormsResources.PipingCalculationGroup_Add_PipingCalculation,
                                                          "Voeg een nieuwe berekening toe aan deze berekeningsmap.",
                                                          PipingFormsResources.PipingIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuValidateAllIndex,
                                                          RingtoetsFormsResources.Validate_all,
                                                          "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ValidateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                                          RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndex,
                                                          "&Wis alle uitvoer...",
                                                          "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 7,
                                                          CoreCommonGuiResources.Delete,
                                                          CoreCommonGuiResources.Delete_ToolTip,
                                                          CoreCommonGuiResources.DeleteIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 9,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 12,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 15,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesIcon,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[2], menu.Items[6], menu.Items[8], menu.Items[11], menu.Items[14] }, typeof(ToolStripSeparator));

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var calculationItem = mockRepository.Stub<IPipingCalculationItem>();
            calculationItem.Expect(ci => ci.Name).Return("Nieuwe map");

            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var newCalculationGroupContextNode = mockRepository.Stub<ITreeNode>();

            // Parent node of newly added item, should be expanded from collapsed state to show selected node:
            var node = mockRepository.Stub<ITreeNode>();
            node.Tag = nodeData;
            node.Expect(n => n.IsExpanded).Return(false);
            node.Expect(n => n.Expand());
            node.Expect(n => n.Nodes).WhenCalled(invocation =>
            {
                if (group.Children.Last() is PipingCalculationGroup)
                {
                    invocation.ReturnValue = new List<ITreeNode>
                    {
                        newCalculationGroupContextNode
                    };
                }
            }).Return(null);

            var treeView = mockRepository.Stub<ITreeView>();
            mockRepository.ReplayAll();

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            var builderProvider = new SimpleContextMenuBuilderProvider(new CustomItemsOnlyContextMenuBuilder());
            var nodePresenter = new PipingCalculationGroupContextNodePresenter(builderProvider)
            {
                TreeView = treeView
            };

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationGroupIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<PipingCalculationGroup>(newlyAddedItem);
            Assert.AreEqual("Nieuwe map (1)", newlyAddedItem.Name,
                "An item with the same name default name already exists, therefore '(1)' needs to be appended.");

            Assert.AreSame(newCalculationGroupContextNode, treeView.SelectedNode,
                "The node of the newly added item should be selected.");

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var calculationItem = mockRepository.Stub<IPipingCalculationItem>();
            calculationItem.Expect(ci => ci.Name).Return("Nieuwe berekening");

            var observer = mockRepository.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var newCalculationContextNode = mockRepository.Stub<ITreeNode>();

            // Parent node of newly added item, should be expanded from collapsed state to show selected node:
            var node = mockRepository.Stub<ITreeNode>();
            node.Tag = nodeData;
            node.Expect(n => n.IsExpanded).Return(false);
            node.Expect(n => n.Expand());
            node.Expect(n => n.Nodes).WhenCalled(invocation =>
            {
                if (group.Children.Last() is PipingCalculation)
                {
                    invocation.ReturnValue = new List<ITreeNode>
                    {
                        newCalculationContextNode
                    };
                }
            }).Return(null);

            var treeView = mockRepository.Stub<ITreeView>();
            mockRepository.ReplayAll();

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            var builderProvider = new SimpleContextMenuBuilderProvider(new CustomItemsOnlyContextMenuBuilder());
            var nodePresenter = new PipingCalculationGroupContextNodePresenter(builderProvider)
            {
                TreeView = treeView
            };

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Precondition
            Assert.AreEqual(1, group.Children.Count);

            // Call
            contextMenu.Items[contextMenuAddCalculationIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, group.Children.Count);
            var newlyAddedItem = group.Children.Last();
            Assert.IsInstanceOf<PipingCalculation>(newlyAddedItem);
            Assert.AreEqual("Nieuwe berekening (1)", newlyAddedItem.Name,
                "An item with the same name default name already exists, therefore '(1)' needs to be appended.");

            Assert.AreSame(newCalculationContextNode, treeView.SelectedNode,
                "The node of the newly added item should be selected.");

            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var node = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var contextMenuBuilderProvider = new SimpleContextMenuBuilderProvider(new CustomItemsOnlyContextMenuBuilder());

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(validCalculation);
            
            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(invalidCalculation);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProvider);

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

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
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var node = mockRepository.StrictMock<ITreeNode>();
            mockRepository.ReplayAll();

            var menuBuilderProvider = new SimpleContextMenuBuilderProvider(new CustomItemsOnlyContextMenuBuilder());

            var validCalculation = PipingCalculationFactory.CreateCalculationWithValidInput();
            validCalculation.Name = "A";
            var invalidCalculation = PipingCalculationFactory.CreateCalculationWithInvalidData();
            invalidCalculation.Name = "B";

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(validCalculation);

            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(invalidCalculation);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var activitesToRun = new List<Activity>();

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(menuBuilderProvider)
            {
                RunActivitiesAction = activity => activitesToRun.AddRange(activity)
            };

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Call
            contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

            // Assert
            Assert.AreEqual(2, activitesToRun.Count);
            foreach (var activity in activitesToRun)
            {
                Assert.IsInstanceOf<PipingCalculationActivity>(activity);
            }
            CollectionAssert.AreEquivalent(new[]{validCalculation.Name, invalidCalculation.Name}, activitesToRun.Select(a=>a.Name));
            mockRepository.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetContextMenu_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers(bool confirm)
        {
            // Setup
            var node = mockRepository.StrictMock<ITreeNode>();

            var calculation1Observer = mockRepository.StrictMock<IObserver>();
            var calculation2Observer = mockRepository.StrictMock<IObserver>();

            if (confirm)
            {
                calculation1Observer.Expect(o => o.UpdateObserver());
                calculation2Observer.Expect(o => o.UpdateObserver());
            }

            mockRepository.ReplayAll();

            var contextMenuBuilderProvider = new SimpleContextMenuBuilderProvider(new CustomItemsOnlyContextMenuBuilder());

            var calculation1 = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation1.Name = "A";
            calculation1.Output = new TestPipingOutput();
            calculation1.Attach(calculation1Observer);
            var calculation2 = PipingCalculationFactory.CreateCalculationWithValidInput();
            calculation2.Name = "B";
            calculation2.Output = new TestPipingOutput();
            calculation1.Attach(calculation2Observer);

            var childGroup = new PipingCalculationGroup();
            childGroup.Children.Add(calculation1);
            
            var emptyChildGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(childGroup);
            group.Children.Add(emptyChildGroup);
            group.Children.Add(calculation2);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            DialogBoxHandler = (name, wnd) =>
            {
                var messageBox = new MessageBoxTester(wnd);
                Assert.AreEqual("Weet u zeker dat u alle uitvoer wilt wissen?", messageBox.Text);
                Assert.AreEqual("Bevestigen", messageBox.Title);
                if (confirm)
                {
                    messageBox.ClickOk();
                }
                else
                {
                    messageBox.ClickCancel();
                }
            };

            // Precondition
            Assert.IsTrue(group.HasOutput);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProvider);

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Call
            contextMenu.Items[contextMenuClearOutputIndex].PerformClick();

            // Assert
            Assert.AreNotEqual(confirm, group.HasOutput);
            Assert.AreNotEqual(confirm, calculation1.HasOutput);
            Assert.AreNotEqual(confirm, calculation2.HasOutput);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var group = new PipingCalculationGroup();
            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var children = nodePresenter.GetChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
            mockRepository.VerifyAll();
        }

        [Test]
        public void GetChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var calculationItem = mockRepository.StrictMock<IPipingCalculationItem>();
            var contextMenuBuilderProviderMock = mockRepository.StrictMock<IContextMenuBuilderProvider>();

            mockRepository.ReplayAll();

            var childCalculation = new PipingCalculation();
            
            var childGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());


            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProviderMock);

            // Call
            var children = nodePresenter.GetChildNodeObjects(nodeData).OfType<object>().ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationContext = (PipingCalculationContext)children[1];
            Assert.AreSame(childCalculation, returnedCalculationContext.WrappedData);
            var returnedCalculationGroupContext = (PipingCalculationGroupContext)children[2];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            mockRepository.VerifyAll();
        }

        /// <summary>
        /// Creates a <see cref="ITreeNode"/> stub that is in the expanded state.
        /// </summary>
        /// <param name="nodeData">The data corresponding with the created node.</param>
        /// <param name="childNodes">The child nodes.</param>
        private ITreeNode CreateExpandedNodeStub(object nodeData, ITreeNode[] childNodes)
        {
            var preUpdateExpandedNode = mockRepository.Stub<ITreeNode>();
            preUpdateExpandedNode.Tag = nodeData;
            preUpdateExpandedNode.Expect(n => n.IsExpanded).Return(true);
            preUpdateExpandedNode.Stub(n => n.Nodes).Return(childNodes);
            return preUpdateExpandedNode;
        }

        /// <summary>
        /// Creates an <see cref="ITreeNode"/> stub that is expected to be expanded.
        /// </summary>
        /// <param name="nodeData">The data corresponding with the created node.</param>
        /// <param name="childNodes">The child nodes.</param>
        /// <param name="whenNodesCalledAction">Optional: action to be called when <see cref="ITreeNode.Nodes"/> is being retrieved.</param>
        private ITreeNode CreateNodeStubToBeExpanded(object nodeData, ITreeNode[] childNodes, Action<MethodInvocation> whenNodesCalledAction = null)
        {
            var postUpdateExpandedNode = mockRepository.Stub<ITreeNode>();
            postUpdateExpandedNode.Tag = nodeData;
            postUpdateExpandedNode.Stub(n => n.IsExpanded).Return(false);
            postUpdateExpandedNode.Expect(n => n.Expand());
            postUpdateExpandedNode.Stub(n => n.Nodes).WhenCalled(invocation =>
            {
                if (whenNodesCalledAction != null)
                {
                    whenNodesCalledAction(invocation);
                }
            }).Return(childNodes);
            return postUpdateExpandedNode;
        }

        /// <summary>
        /// Creates a <see cref="ITreeNode"/> stub that is in the collapsed state.
        /// </summary>
        /// <param name="nodeData">The data corresponding with the created node.</param>
        /// <param name="childNodes">The child nodes.</param>
        private ITreeNode CreateCollapsedNodeStub(object nodeData, ITreeNode[] childNodes)
        {
            var collapsedNode = mockRepository.Stub<ITreeNode>();
            collapsedNode.Tag = nodeData;
            collapsedNode.Stub(n => n.IsExpanded).Return(false);
            collapsedNode.Stub(n => n.Nodes).Return(childNodes);
            return collapsedNode;
        }

        /// <summary>
        /// Creates an <see cref="ITreeNode"/> stub that is expected to be collapsed.
        /// </summary>
        /// <param name="nodeData">The data corresponding with the created node.</param>
        /// <param name="childNodes">The child nodes.</param>
        private ITreeNode CreateNodeStubToBeCollapsed(object nodeData, ITreeNode[] childNodes)
        {
            var nodeToBeCollapsed = mockRepository.Stub<ITreeNode>();
            nodeToBeCollapsed.Tag = nodeData;
            nodeToBeCollapsed.Stub(n => n.IsExpanded).Return(true);
            nodeToBeCollapsed.Expect(n => n.Collapse());
            nodeToBeCollapsed.Stub(n => n.Nodes).Return(childNodes);
            return nodeToBeCollapsed;
        }

        /// <summary>
        /// Creates an <see cref="IObserver"/> stub that expects its <see cref="IObserver.UpdateObserver"/>
        /// method will be called once.
        /// </summary>
        /// <param name="whenCalledAction">Optional: Action to be performed when <see cref="IObserver.UpdateObserver"/>
        /// is being called.</param>
        private IObserver CreateObserverStubWithUpdateExpectancy(Action<MethodInvocation> whenCalledAction = null)
        {
            var newOwnerObserver = mockRepository.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver()).WhenCalled(invocation =>
            {
                if (whenCalledAction != null)
                {
                    whenCalledAction(invocation);
                }
            });
            return newOwnerObserver;
        }

        /// <summary>
        /// Creates an instance of <see cref="PipingCalculationGroup"/> and the corresponding
        /// <see cref="PipingCalculationGroupContext"/>.
        /// </summary>
        /// <param name="data">The created group without any children.</param>
        /// <param name="dataContext">The context object for <paramref name="data"/>, without any other data.</param>
        private static void CreatePipingCalculationGroupAndContext(out PipingCalculationGroup data, out PipingCalculationGroupContext dataContext)
        {
            data = new PipingCalculationGroup();
            dataContext = new PipingCalculationGroupContext(data,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingSoilProfile>());
        }

        /// <summary>
        /// Creates an instance of <see cref="IPipingCalculationItem"/> and the corresponding context.
        /// </summary>
        /// <param name="type">Defines the implementation of <see cref="IPipingCalculationItem"/> to be constructed.</param>
        /// <param name="data">Output: The concrete create class based on <paramref name="type"/>.</param>
        /// <param name="dataContext">Output: The <see cref="PipingContext{T}"/> corresponding with <paramref name="data"/>.</param>
        /// <param name="initialName">Optional: The name of <paramref name="data"/>.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        private static void CreatePipingCalculationItemAndContext(PipingCalculationItemType type, out IPipingCalculationItem data, out object dataContext, string initialName = null)
        {
            switch (type)
            {
                case PipingCalculationItemType.Calculation:
                    var calculation = new PipingCalculation();
                    if (initialName != null)
                    {
                        calculation.Name = initialName;
                    }
                    data = calculation;
                    dataContext = new PipingCalculationContext(calculation,
                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                               Enumerable.Empty<PipingSoilProfile>());
                    break;
                case PipingCalculationItemType.Group:
                    var group = new PipingCalculationGroup();
                    if (initialName != null)
                    {
                        group.Name = initialName;
                    }
                    data = group;
                    dataContext = new PipingCalculationGroupContext(group,
                                                                    Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                    Enumerable.Empty<PipingSoilProfile>());
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Use <see cref="mockRepository"/> to creates a stub tree node as a simple node-tree leaf.
        /// </summary>
        private ITreeNode CreateStubTreeNode()
        {
            return CreateTreeNodeLeafForData(new object());
        }

        /// <summary>
        /// Creates an <see cref="ITreeNode"/> stub that represents a tree-node leaf.
        /// </summary>
        /// <param name="nodeData">The data associated with the node.</param>
        private ITreeNode CreateTreeNodeLeafForData(object nodeData)
        {
            var preUpdateDraggedItemContextNode = mockRepository.Stub<ITreeNode>();
            preUpdateDraggedItemContextNode.Tag = nodeData;
            preUpdateDraggedItemContextNode.Stub(n => n.IsExpanded).Return(false);
            preUpdateDraggedItemContextNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());
            return preUpdateDraggedItemContextNode;
        }

        /// <summary>
        /// Type indicator for testing methods on <see cref="ITreeNodePresenter"/>.
        /// </summary>
        public enum DragDropTestMethod
        {
            /// <summary>
            /// Indicates <see cref="ITreeNodePresenter.CanDrop"/>.
            /// </summary>
            CanDrop,
            /// <summary>
            /// Indicates <see cref="ITreeNodePresenter.CanInsert"/>.
            /// </summary>
            CanInsert
        }

        /// <summary>
        /// Type indicator for implementations of <see cref="IPipingCalculationItem"/> to be created in a test.
        /// </summary>
        public enum PipingCalculationItemType
        {
            /// <summary>
            /// Indicates <see cref="PipingCalculation"/>.
            /// </summary>
            Calculation,
            /// <summary>
            /// Indicates <see cref="PipingCalculationGroup"/>.
            /// </summary>
            Group
        }
    }
}