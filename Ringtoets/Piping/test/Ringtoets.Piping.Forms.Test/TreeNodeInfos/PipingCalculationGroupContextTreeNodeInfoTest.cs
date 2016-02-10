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
using Ringtoets.Piping.Calculation.TestUtil;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.TestUtil;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Plugin;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using CoreCommonGuiResources = Core.Common.Gui.Properties.Resources;

namespace Ringtoets.Piping.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class PipingCalculationGroupContextTreeNodeInfoTest : NUnitFormTest
    {
        private MockRepository mocks;
        private PipingGuiPlugin plugin;
        private TreeNodeInfo info;

        [SetUp]
        public void SetUp()
        {
            mocks = new MockRepository();
            plugin = new PipingGuiPlugin();
            info = plugin.GetTreeNodeInfos().First(tni => tni.TagType == typeof(PipingCalculationGroupContext));
        }

        [Test]
        public void Initialized_Always_ExpectedPropertiesSet()
        {
            // Assert
            Assert.AreEqual(typeof(PipingCalculationGroupContext), info.TagType);
            Assert.IsNull(info.ForeColor);
            Assert.IsNull(info.CanCheck);
            Assert.IsNull(info.IsChecked);
            Assert.IsNull(info.OnNodeChecked);
        }

        [Test]
        public void EnsureVisibleOnCreate_Always_ReturnsTrue()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                                 pipingFailureMechanismMock);

            mocks.ReplayAll();

            // Call
            var result = info.EnsureVisibleOnCreate(groupContext);

            // Assert
            Assert.IsTrue(result);

            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_WithParentNodeDefaultBehavior_ReturnTrue()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                                 pipingFailureMechanismMock);

            // Call
            var canDrag = info.CanDrag(groupContext, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanDrag_ParentIsPipingFailureMechanism_ReturnFalse()
        {
            // Setup
            var pipingFailureMechanism = new PipingFailureMechanism();
            var group = pipingFailureMechanism.CalculationsGroup;
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                                 pipingFailureMechanismMock);

            // Call
            var canDrag = info.CanDrag(groupContext, pipingFailureMechanism);

            // Assert
            Assert.IsFalse(canDrag);
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

            var failureMechanism = new PipingFailureMechanism();

            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanism);

            PipingCalculationGroup targetGroup;
            PipingCalculationGroupContext targetGroupContext;
            CreatePipingCalculationGroupAndContext(out targetGroup, out targetGroupContext, failureMechanism);

            failureMechanism.CalculationsGroup.Children.Add(draggedItem);
            failureMechanism.CalculationsGroup.Children.Add(targetGroup);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    var canDrop = info.CanDrop(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsTrue(canDrop);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = info.CanInsert(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsTrue(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mocks.VerifyAll();
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

            var targetFailureMechanism = new PipingFailureMechanism();

            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, targetFailureMechanism);

            var sourceFailureMechanism = new PipingFailureMechanism();
            sourceFailureMechanism.CalculationsGroup.Children.Add(draggedItem);

            PipingCalculationGroup targetGroup;
            PipingCalculationGroupContext targetGroupContext;
            CreatePipingCalculationGroupAndContext(out targetGroup, out targetGroupContext, sourceFailureMechanism);

            targetFailureMechanism.CalculationsGroup.Children.Add(targetGroup);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    var canDrop = info.CanDrop(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsFalse(canDrop);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = info.CanInsert(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsFalse(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mocks.VerifyAll();
        }

//        [Test]
//        [Combinatorial]
//        public void OnDragDrop_DraggingPipingCalculationItemContextOntoGroupEnd_MoveCalculationItemInstanceToNewGroup(
//            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
//        {
//            // Setup
//            IPipingCalculationItem draggedItem;
//            object draggedItemContext;
//            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);
//
//            PipingCalculationGroup originalOwnerGroup;
//            PipingCalculationGroupContext originalOwnerGroupContext;
//            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext);
//            originalOwnerGroup.Children.Add(draggedItem);
//
//            PipingCalculationGroup newOwnerGroup;
//            PipingCalculationGroupContext newOwnerGroupContext;
//            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext);
//
//            var originalOwnerObserver = mocks.StrictMock<IObserver>();
//            originalOwnerObserver.Expect(o => o.UpdateObserver());
//
//            var updatewasCalled = false;
//            var newOwnerObserver = CreateObserverStubWithUpdateExpectancy(invocation => updatewasCalled = true);
//
//            var newOwnerGroupContextNode = new TreeNode
//            {
//                Tag = newOwnerGroupContext
//            };
//
//            var originalOwnerGroupContextNode = new TreeNode
//            {
//                Tag = originalOwnerGroupContext
//            };
//
//            var draggedItemContextNode = new TreeNode
//            {
//                Tag = draggedItemContext
//            };
//            draggedItemContextNode.Collapse();
//
//            newOwnerGroupContextNode.Nodes.Add(draggedItemContextNode);
//
//            mocks.ReplayAll();
//
//            var treeView = new Core.Common.Controls.TreeView.TreeView();
//            var root = new TreeNode();
//            treeView.Nodes.Add(root);
//            root.Nodes.Add(originalOwnerGroupContextNode);
//            root.Nodes.Add(newOwnerGroupContextNode);
//
//            originalOwnerGroup.Attach(originalOwnerObserver);
//            newOwnerGroup.Attach(newOwnerObserver);
//            
//            // Precondition:
//            Assert.IsFalse(draggedItemContextNode.IsExpanded);
//            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
//            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);
//            
//            // Call
//            info.OnDrop(draggedItemContextNode, newOwnerGroupContextNode, DragOperations.Move, newOwnerGroup.Children.Count);
//
//            // Assert
//            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
//            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
//            Assert.AreSame(draggedItem, newOwnerGroup.Children.Last(),
//                "Dragging node at the end of the target PipingCalculationGroup should put the dragged data at the end of 'newOwnerGroup'.");
//
//            Assert.AreSame(draggedItemContextNode, draggedItemContextNode.TreeView.SelectedNode);
//            Assert.IsTrue(newOwnerGroupContextNode.IsExpanded);
//            Assert.IsFalse(draggedItemContextNode.IsExpanded);
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        [Combinatorial]
//        public void OnDragDrop_InsertingPipingCalculationItemContextAtDifferentLocationWithingSameGroup_ChangeItemIndexOfCalculationItem(
//            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType,
//            [Values(0, 2)] int newIndex)
//        {
//            // Setup
//            const string name = "Very cool name";
//            IPipingCalculationItem draggedItem;
//            object draggedItemContext;
//            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, name);
//
//            var existingItemStub = mocks.Stub<IPipingCalculationItem>();
//            existingItemStub.Stub(i => i.Name).Return("");
//
//            PipingCalculationGroup originalOwnerGroup;
//            PipingCalculationGroupContext originalOwnerGroupContext;
//            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext);
//            originalOwnerGroup.Children.Add(existingItemStub);
//            originalOwnerGroup.Children.Add(draggedItem);
//            originalOwnerGroup.Children.Add(existingItemStub);
//
//            bool updatewasCalled = false;
//            var originalOwnerObserver = CreateObserverStubWithUpdateExpectancy(invocation => updatewasCalled = true);
//            
//            var existingItemStub1 = CreateStubTreeNode();
//            var existingItemStub2 = CreateStubTreeNode();
//            
//            var preUpdateDraggedItemContextNode = CreateTreeNodeLeafForData(draggedItemContext);
//            var postUpdateDraggedItemContextNode = CreateTreeNodeLeafForData(draggedItemContext);
//            TreeNode[] preUpdateNewOwnerChildNodes = { existingItemStub1, preUpdateDraggedItemContextNode, existingItemStub2 };
//            var newOwnerGroupContextNode = CreateNodeStubToBeExpanded(originalOwnerGroupContext, preUpdateNewOwnerChildNodes, invocation =>
//            {
//                if (updatewasCalled)
//                {
//                    invocation.ReturnValue = new[]
//                    {
//                        existingItemStub1,
//                        postUpdateDraggedItemContextNode,
//                        existingItemStub2
//                    };
//                }
//            });
//            postUpdateDraggedItemContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);
//
//            mocks.ReplayAll();
//
//            originalOwnerGroup.Attach(originalOwnerObserver);
//
//            // Precondition:
//            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
//
//            // Call
//            info.OnDrop(preUpdateDraggedItemContextNode, newOwnerGroupContextNode, DragOperations.Move, newIndex);
//
//            // Assert
//            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
//            Assert.AreNotSame(draggedItem, originalOwnerGroup.Children[1],
//                "Should have removed 'draggedItem' from its original location in the collection.");
//            Assert.AreSame(draggedItem, originalOwnerGroup.Children[newIndex],
//                "Dragging node to specific location within owning PipingCalculationGroup should put the dragged data at that index.");
//            Assert.AreEqual(name, draggedItem.Name,
//                "No renaming should occur when dragging within the same PipingCalculationGroup.");
//
//            Assert.AreSame(postUpdateDraggedItemContextNode, postUpdateDraggedItemContextNode.TreeView.SelectedNode);
//
//            Assert.IsTrue(newOwnerGroupContextNode.IsExpanded);
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        [Combinatorial]
//        public void OnDragDrop_DraggingPipingCalculationItemContextOntoGroupStartWithSameNamedItem_MoveCalculationItemInstanceToNewGroupAndRename(
//            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
//        {
//            // Setup
//            IPipingCalculationItem draggedItem;
//            object draggedItemContext;
//            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);
//
//            PipingCalculationGroup originalOwnerGroup;
//            PipingCalculationGroupContext originalOwnerGroupContext;
//            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext);
//            originalOwnerGroup.Children.Add(draggedItem);
//
//            PipingCalculationGroup newOwnerGroup;
//            PipingCalculationGroupContext newOwnerGroupContext;
//            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext);
//
//            var sameNamedItem = mocks.Stub<IPipingCalculationItem>();
//            sameNamedItem.Stub(i => i.Name).Return(draggedItem.Name);
//
//            var updateWasCalled = false;
//            var originalOwnerObserver = CreateObserverStubWithUpdateExpectancy();
//            var newOwnerObserver = CreateObserverStubWithUpdateExpectancy(invocation => updateWasCalled = true);
//
//            var preUpdateCalculationContextNode = CreateExpandedNodeStub(draggedItemContext, new TreeNode[0]);
//            var postUpdateCalculationContextNode = CreateNodeStubToBeExpanded(draggedItemContext, new TreeNode[0]);
//            var newOwnerGroupContextNode = CreateNodeStubToBeExpanded(originalOwnerGroupContext, new TreeNode[0], invocation =>
//            {
//                if (updateWasCalled)
//                {
//                    invocation.ReturnValue = new[]
//                    {
//                        postUpdateCalculationContextNode
//                    };
//                }
//            });
//            postUpdateCalculationContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);
//
//            mocks.ReplayAll();
//
//            newOwnerGroup.Children.Add(sameNamedItem);
//
//            originalOwnerGroup.Attach(originalOwnerObserver);
//            newOwnerGroup.Attach(newOwnerObserver);
//
//            // Precondition:
//            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
//            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);
//            CollectionAssert.Contains(newOwnerGroup.Children.Select(c => c.Name), draggedItem.Name,
//                "Name of the dragged item should already exist in new owner.");
//
//            // Call
//            info.OnDrop(preUpdateCalculationContextNode, newOwnerGroupContextNode, DragOperations.Move, 0);
//
//            // Assert
//            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
//            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
//            Assert.AreSame(draggedItem, newOwnerGroup.Children.First(),
//                "Dragging to insert node at start of newOwnerGroup should place the node at the start of the list.");
//            switch (draggedItemType)
//            {
//                case PipingCalculationItemType.Calculation:
//                    Assert.AreEqual("Nieuwe berekening (1)", draggedItem.Name);
//                    break;
//                case PipingCalculationItemType.Group:
//                    Assert.AreEqual("Nieuwe map (1)", draggedItem.Name);
//                    break;
//            }
//
//            Assert.AreSame(postUpdateCalculationContextNode, postUpdateCalculationContextNode.TreeView.SelectedNode);
//
//            Assert.IsTrue(postUpdateCalculationContextNode.IsExpanded);
//            Assert.IsTrue(newOwnerGroupContextNode.IsExpanded);
//
//            mocks.VerifyAll();
//        }
//
//        [Test]
//        [Combinatorial]
//        public void OnDragDrop_DraggingPipingCalculationItemContextOntoGroupWithOtherItems_ExpandedCollapsedStateOfOtherItemsRestored(
//            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
//        {
//            // Setup
//            IPipingCalculationItem draggedItem;
//            object draggedItemContext;
//            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);
//
//            PipingCalculationGroup originalOwnerGroup;
//            PipingCalculationGroupContext originalOwnerGroupContext;
//            CreatePipingCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext);
//            originalOwnerGroup.Children.Add(draggedItem);
//
//            PipingCalculationGroup newOwnerGroup;
//            PipingCalculationGroupContext newOwnerGroupContext;
//            CreatePipingCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext);
//
//            var updatewasCalled = false;
//            var originalOwnerObserver = CreateObserverStubWithUpdateExpectancy();
//            var newOwnerObserver = CreateObserverStubWithUpdateExpectancy(methodInvocation => updatewasCalled = true);
//
//            var nodeStub = CreateStubTreeNode();
//            TreeNode[] childNodesStub = { nodeStub };
//
//            var expandedNodeData = new object();
//            var preUpdateExpandedNode = CreateExpandedNodeStub(expandedNodeData, childNodesStub);
//            var postUpdateExpandedNode = CreateNodeStubToBeExpanded(expandedNodeData, childNodesStub);
//
//            var collapsedNodeData = new object();
//            var preUpdateCollapsedNode = CreateCollapsedNodeStub(collapsedNodeData, childNodesStub);
//            var postUpdateCollapsedNode = CreateNodeStubToBeCollapsed(collapsedNodeData, childNodesStub);
//
//            TreeNode[] preUpdateChildNodes = { preUpdateExpandedNode, preUpdateCollapsedNode };
//            TreeNode[] postUpdateChildNodes = { postUpdateExpandedNode, postUpdateCollapsedNode };
//            var newOwnerGroupContextNode = CreateNodeStubToBeExpanded(newOwnerGroupContext, preUpdateChildNodes, invocation =>
//            {
//                if (updatewasCalled)
//                {
//                    invocation.ReturnValue = postUpdateChildNodes;
//                }
//            });
//
//            var preUpdateDraggedItemContextNode = CreateTreeNodeLeafForData(draggedItemContext);
//            var postUpdateDraggedItemContextNode = CreateNodeStubToBeCollapsed(draggedItemContext, new TreeNode[0]);
//            postUpdateDraggedItemContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);
//
//            mocks.ReplayAll();
//
//            originalOwnerGroup.Attach(originalOwnerObserver);
//            newOwnerGroup.Attach(newOwnerObserver);
//
//            // Precondition:
//            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
//            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);
//
//            // Call
//            info.OnDrop(preUpdateDraggedItemContextNode, postUpdateDraggedItemContextNode, DragOperations.Move, newOwnerGroup.Children.Count);
//
//            // Assert
//            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
//            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
//            Assert.AreSame(draggedItem, newOwnerGroup.Children.Last(),
//                "Dragging node at the end of the target PipingCalculationGroup should put the dragged data at the end of 'newOwnerGroup'.");
//
//            Assert.AreSame(postUpdateDraggedItemContextNode, postUpdateDraggedItemContextNode.TreeView.SelectedNode);
//            Assert.IsFalse(postUpdateCollapsedNode.IsExpanded);
//            Assert.IsFalse(postUpdateDraggedItemContextNode.IsExpanded);
//            Assert.IsTrue(postUpdateExpandedNode.IsExpanded);
//            Assert.IsTrue(newOwnerGroupContextNode.IsExpanded);
//
//            mocks.VerifyAll();
//        }

        [Test]
        public void CanRenameNode_ParentIsPipingFailureMechanismNode_ReturnFalse()
        {
            // Setup
            var failureMechanism = new PipingFailureMechanism();

            mocks.ReplayAll();

            // Call
            bool isRenamingAllowed = info.CanRename(null, failureMechanism);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_EverythingElse_ReturnTrue()
        {
            // Call
            bool isRenamingAllowed = info.CanRename(null, null);

            // Assert
            Assert.IsTrue(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void OnNodeRenamed_WithData_RenameGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);
            nodeData.Attach(observer);

            // Call
            const string newName = "new name";
            info.OnNodeRenamed(nodeData, newName);

            // Assert
            Assert.AreEqual(newName, group.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsFailureMechanism_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var parentNodeData = new PipingFailureMechanism();
            parentNodeData.CalculationsGroup.Children.Add(group);

            mocks.ReplayAll();

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupContainingGroup_ReturnTrue()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var parentGroup = new PipingCalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<PipingSoilProfile>(),
                                                                   pipingFailureMechanismMock);

            mocks.ReplayAll();

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsTrue(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupNotContainingGroup_ReturnFalse()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var parentGroup = new PipingCalculationGroup();
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<PipingSoilProfile>(),
                                                                   pipingFailureMechanismMock);

            // Precondition
            CollectionAssert.DoesNotContain(parentGroup.Children, group);

            // Call
            bool isRemovalAllowed = info.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
        }

        [Test]
        public void CanRemove_ParentIsPipingCalculationGroupContainingGroup_RemoveGroupAndNotifyObservers()
        {
            // Setup
            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var parentGroup = new PipingCalculationGroup();
            parentGroup.Children.Add(group);
            var parentNodeData = new PipingCalculationGroupContext(parentGroup,
                                                                   Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                   Enumerable.Empty<PipingSoilProfile>(),
                                                                   pipingFailureMechanismMock);
            parentNodeData.Attach(observer);

            // Precondition
            Assert.IsTrue(info.CanRemove(nodeData, parentNodeData));

            // Call
            info.OnNodeRemoved(nodeData, parentNodeData);

            // Assert
            CollectionAssert.DoesNotContain(parentGroup.Children, group);
            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ChildOfGroupValidDataWithCalculationOutput_ReturnContextMenuWithAllItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();

            var parentGroup = new PipingCalculationGroup();
            var group = new PipingCalculationGroup();

            parentGroup.Children.Add(group);
            group.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            var parentData = new PipingCalculationGroupContext(parentGroup,
                                                               Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                               Enumerable.Empty<PipingSoilProfile>(),
                                                               pipingFailureMechanismMock);

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanRemoveNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseAllNodesForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            Assert.AreEqual(17, menu.Items.Count);
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
                                                          CoreCommonGuiResources.Rename,
                                                          CoreCommonGuiResources.Rename_ToolTip,
                                                          CoreCommonGuiResources.RenameIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                          CoreCommonGuiResources.Delete,
                                                          CoreCommonGuiResources.Delete_ToolTip,
                                                          CoreCommonGuiResources.DeleteIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 14,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 16,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesIcon,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[6],
                menu.Items[9],
                menu.Items[12],
                menu.Items[15]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_NotValidDataWithCalculationOutput_ReturnContextWithItems()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new PipingCalculationGroup();

            group.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            var parentData = new object();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);

            treeViewControl.Expect(tvc => tvc.CanRenameNodeForData(nodeData)).Return(true);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseAllNodesForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

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
                                                          CoreCommonGuiResources.Rename,
                                                          CoreCommonGuiResources.Rename_ToolTip,
                                                          CoreCommonGuiResources.RenameIcon);
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
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[6],
                menu.Items[8],
                menu.Items[11],
                menu.Items[14]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ParentWithFailureMechanism_ReturnContextMenuWithoutRenameRemove()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var group = new PipingCalculationGroup();

            group.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            var parentData = new PipingFailureMechanism();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var applicationFeatureCommandHandler = mocks.Stub<IApplicationFeatureCommands>();
            var exportImportHandler = mocks.Stub<IExportImportCommandHandler>();
            var viewCommandsHandler = mocks.StrictMock<IViewCommands>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            var menuBuilder = new ContextMenuBuilder(applicationFeatureCommandHandler, exportImportHandler, viewCommandsHandler, nodeData, treeViewControl);
            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
            treeViewControl.Expect(tvc => tvc.CanExpandOrCollapseAllNodesForData(nodeData)).Repeat.Twice().Return(false);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip menu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

            // Assert
            Assert.AreEqual(14, menu.Items.Count);
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
                                                          CoreCommonGuiResources.Import,
                                                          CoreCommonGuiResources.Import_ToolTip,
                                                          CoreCommonGuiResources.ImportIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                          CoreCommonGuiResources.Export,
                                                          CoreCommonGuiResources.Export_ToolTip,
                                                          CoreCommonGuiResources.ExportIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 10,
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 11,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

            TestHelper.AssertContextMenuStripContainsItem(menu, 13,
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesIcon,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[]
            {
                menu.Items[2],
                menu.Items[6],
                menu.Items[9],
                menu.Items[12]
            }, typeof(ToolStripSeparator));

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_GroupWithNoCalculations_ValidateAndCalculateAllDisabled()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var group = new PipingCalculationGroup();
            var parentData = new PipingFailureMechanism();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            // Call
            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, parentData, treeViewControl);

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
        public void GetContextMenu_ClickOnAddGroupItem_AddGroupToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var calculationItem = mocks.Stub<IPipingCalculationItem>();
            calculationItem.Expect(ci => ci.Name).Return("Nieuwe map");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            ContextMenuStrip contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

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

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnAddCalculationItem_AddCalculationToCalculationGroupAndNotifyObservers()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            var calculationItem = mocks.Stub<IPipingCalculationItem>();
            calculationItem.Expect(ci => ci.Name).Return("Nieuwe berekening");

            var observer = mocks.StrictMock<IObserver>();
            observer.Expect(o => o.UpdateObserver());

            var treeViewControl = mocks.StrictMock<TreeViewControl>();

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            group.Children.Add(calculationItem);

            nodeData.Attach(observer);

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

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

            mocks.VerifyAll();
        }

        [Test]
        public void GetContextMenu_ClickOnValidateAllItem_ValidateAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

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

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

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
        public void GetContextMenu_ClickOnCalculateAllItem_ScheduleAllChildCalculations()
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var mainWindow = mocks.Stub<IMainWindow>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

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

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            gui.Expect(g => g.Get(nodeData, treeViewControl)).Return(menuBuilder);
            gui.Expect(g => g.MainWindow).Return(mainWindow);

            mocks.ReplayAll();

            plugin.Gui = gui;

            DialogBoxHandler = (name, wnd) => { };

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Call
            contextMenu.Items[contextMenuCalculateAllIndex].PerformClick();

            // Assert
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(false)]
        [TestCase(true)]
        public void GetContextMenu_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers(bool confirm)
        {
            // Setup
            var gui = mocks.StrictMock<IGui>();
            var treeViewControl = mocks.StrictMock<TreeViewControl>();
            var menuBuilder = new CustomItemsOnlyContextMenuBuilder();

            var calculation1Observer = mocks.StrictMock<IObserver>();
            var calculation2Observer = mocks.StrictMock<IObserver>();

            if (confirm)
            {
                calculation1Observer.Expect(o => o.UpdateObserver());
                calculation2Observer.Expect(o => o.UpdateObserver());
            }

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

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            gui.Expect(cmp => cmp.Get(nodeData, treeViewControl)).Return(menuBuilder);

            mocks.ReplayAll();

            plugin.Gui = gui;

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

            var contextMenu = info.ContextMenuStrip(nodeData, null, treeViewControl);

            // Call
            contextMenu.Items[contextMenuClearOutputIndex].PerformClick();

            // Assert
            Assert.AreNotEqual(confirm, group.HasOutput);
            Assert.AreNotEqual(confirm, calculation1.HasOutput);
            Assert.AreNotEqual(confirm, calculation2.HasOutput);
            mocks.VerifyAll();
        }

        [Test]
        public void GetChildNodeObjects_EmptyGroup_ReturnEmpty()
        {
            // Setup
            var group = new PipingCalculationGroup();
            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();
            mocks.ReplayAll();

            var groupContext = new PipingCalculationGroupContext(group,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>(),
                                                                 pipingFailureMechanismMock);

            // Call
            var children = info.ChildNodeObjects(groupContext);

            // Assert
            CollectionAssert.IsEmpty(children);
        }

        [Test]
        public void GetChildNodeObjects_GroupWithMixedContents_ReturnChildren()
        {
            // Setup
            var calculationItem = mocks.StrictMock<IPipingCalculationItem>();

            var childCalculation = new PipingCalculation();

            var childGroup = new PipingCalculationGroup();

            var group = new PipingCalculationGroup();
            group.Children.Add(calculationItem);
            group.Children.Add(childCalculation);
            group.Children.Add(childGroup);

            var pipingFailureMechanismMock = mocks.StrictMock<PipingFailureMechanism>();

            mocks.ReplayAll();

            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>(),
                                                             pipingFailureMechanismMock);

            // Call
            var children = info.ChildNodeObjects(nodeData).ToArray();

            // Assert
            Assert.AreEqual(group.Children.Count, children.Length);
            Assert.AreSame(calculationItem, children[0]);
            var returnedCalculationContext = (PipingCalculationContext) children[1];
            Assert.AreSame(childCalculation, returnedCalculationContext.WrappedData);
            Assert.AreSame(pipingFailureMechanismMock, returnedCalculationContext.PipingFailureMechanism);
            var returnedCalculationGroupContext = (PipingCalculationGroupContext)children[2];
            Assert.AreSame(childGroup, returnedCalculationGroupContext.WrappedData);
            Assert.AreSame(pipingFailureMechanismMock, returnedCalculationGroupContext.PipingFailureMechanism);
            mocks.VerifyAll();
        }

        private const int contextMenuAddCalculationGroupIndex = 0;
        private const int contextMenuAddCalculationIndex = 1;
        private const int contextMenuValidateAllIndex = 3;
        private const int contextMenuCalculateAllIndex = 4;
        private const int contextMenuClearOutputIndex = 5;

        /// <summary>
        /// Creates a <see cref="TreeNode"/> stub that is in the expanded state.
        /// </summary>
        /// <param name="nodeData">The data corresponding with the created node.</param>
        /// <param name="childNodes">The child nodes.</param>
        private TreeNode CreateExpandedNodeStub(object nodeData, TreeNode[] childNodes)
        {
            var preUpdateExpandedNode = mocks.Stub<TreeNode>();
            preUpdateExpandedNode.Tag = nodeData;
            preUpdateExpandedNode.Expect(n => n.IsExpanded).Return(true);
            preUpdateExpandedNode.Nodes.AddRange(childNodes);
            return preUpdateExpandedNode;
        }

        /// <summary>
        /// Creates an <see cref="TreeNode"/> stub that is expected to be expanded.
        /// </summary>
        /// <param name="nodeData">The data corresponding with the created node.</param>
        /// <param name="childNodes">The child nodes.</param>
        /// <param name="whenNodesCalledAction">Optional: action to be called when <see cref="TreeNode.Nodes"/> is being retrieved.</param>
        private TreeNode CreateNodeStubToBeExpanded(object nodeData, TreeNode[] childNodes, Action<MethodInvocation> whenNodesCalledAction = null)
        {
            var postUpdateExpandedNode = new TreeNode();
            postUpdateExpandedNode.Tag = nodeData;
            postUpdateExpandedNode.Collapse();
            postUpdateExpandedNode.Nodes.AddRange(childNodes);
            return postUpdateExpandedNode;
        }

        /// <summary>
        /// Creates a <see cref="TreeNode"/> stub that is in the collapsed state.
        /// </summary>
        /// <param name="nodeData">The data corresponding with the created node.</param>
        /// <param name="childNodes">The child nodes.</param>
        private TreeNode CreateCollapsedNodeStub(object nodeData, TreeNode[] childNodes)
        {
            var collapsedNode = new TreeNode();
            collapsedNode.Tag = nodeData;
            collapsedNode.Collapse();
            collapsedNode.Nodes.AddRange(childNodes);
            return collapsedNode;
        }

        /// <summary>
        /// Creates an <see cref="TreeNode"/> stub that is expected to be collapsed.
        /// </summary>
        /// <param name="nodeData">The data corresponding with the created node.</param>
        /// <param name="childNodes">The child nodes.</param>
        private TreeNode CreateNodeStubToBeCollapsed(object nodeData, TreeNode[] childNodes)
        {
            var nodeToBeCollapsed = new TreeNode();
            nodeToBeCollapsed.Tag = nodeData;
            nodeToBeCollapsed.Nodes.AddRange(childNodes);
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
            var newOwnerObserver = mocks.StrictMock<IObserver>();
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
        /// <param name="failureMechanism"></param>
        private void CreatePipingCalculationGroupAndContext(out PipingCalculationGroup data, out PipingCalculationGroupContext dataContext, PipingFailureMechanism failureMechanism)
        {
            data = new PipingCalculationGroup();

            dataContext = new PipingCalculationGroupContext(data,
                                                            Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                            Enumerable.Empty<PipingSoilProfile>(),
                                                            failureMechanism);
        }

        /// <summary>
        /// Creates an instance of <see cref="IPipingCalculationItem"/> and the corresponding context.
        /// </summary>
        /// <param name="type">Defines the implementation of <see cref="IPipingCalculationItem"/> to be constructed.</param>
        /// <param name="data">Output: The concrete create class based on <paramref name="type"/>.</param>
        /// <param name="dataContext">Output: The <see cref="PipingContext{T}"/> corresponding with <paramref name="data"/>.</param>
        /// <param name="pipingFailureMechanism">The piping failure mechanism the item and context belong to.</param>
        /// <param name="initialName">Optional: The name of <paramref name="data"/>.</param>
        /// <exception cref="System.NotSupportedException"></exception>
        private void CreatePipingCalculationItemAndContext(PipingCalculationItemType type, out IPipingCalculationItem data, out object dataContext, PipingFailureMechanism pipingFailureMechanism, string initialName = null)
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
                                                               Enumerable.Empty<PipingSoilProfile>(),
                                                               pipingFailureMechanism);
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
                                                                    Enumerable.Empty<PipingSoilProfile>(),
                                                                    pipingFailureMechanism);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        /// <summary>
        /// Use <see cref="MockRepository"/> to creates a stub tree node as a simple node-tree leaf.
        /// </summary>
        private TreeNode CreateStubTreeNode()
        {
            return CreateTreeNodeLeafForData(new object());
        }

        /// <summary>
        /// Creates an <see cref="TreeNode"/> stub that represents a tree-node leaf.
        /// </summary>
        /// <param name="nodeData">The data associated with the node.</param>
        private TreeNode CreateTreeNodeLeafForData(object nodeData)
        {
            var preUpdateDraggedItemContextNode = mocks.Stub<TreeNode>();
            preUpdateDraggedItemContextNode.Tag = nodeData;
            preUpdateDraggedItemContextNode.Stub(n => n.IsExpanded).Return(false);
            return preUpdateDraggedItemContextNode;
        }

        /// <summary>
        /// Type indicator for testing methods on <see cref="TreeNodeInfo"/>.
        /// </summary>
        public enum DragDropTestMethod
        {
            /// <summary>
            /// Indicates <see cref="TreeNodeInfo.CanDrop"/>.
            /// </summary>
            CanDrop,

            /// <summary>
            /// Indicates <see cref="TreeNodeInfo.CanInsert"/>.
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