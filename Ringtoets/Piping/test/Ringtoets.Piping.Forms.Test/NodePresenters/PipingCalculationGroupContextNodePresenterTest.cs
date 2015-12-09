using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base;
using Core.Common.Base.Service;
using Core.Common.Controls;
using Core.Common.Controls.Swf.TreeViewControls;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.TestUtil.ContextMenu;
using Core.Common.TestUtil;
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
    public class PipingCalculationGroupContextNodePresenterTest
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

            var groupContextNode = mockRepository.StrictMock<ITreeNode>();
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

            var targetGroup = new PipingCalculationGroup();
            var targetGroupContext = new PipingCalculationGroupContext(targetGroup,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingSoilProfile>());
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
            mockRepository.ReplayAll();
        }

        [Test]
        [Combinatorial]
        public void CanDropOrCanInsert_DraggingPipingCalculationItemContextOntoGroupNotContainingItemButFromOtherFailureMechanism_ReturnNoneOrFalse(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(PipingCalculationItemType.Calculation, PipingCalculationItemType.Group)] PipingCalculationItemType draggedItemType)
        {
            // Setup
            IPipingCalculationItem draggedItem;
            object draggedItemContext;
            CreatePipingCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext);

            var sourceFailureMechanism = new PipingFailureMechanism();
            sourceFailureMechanism.CalculationsGroup.Children.Add(draggedItem);

            var targetGroup = new PipingCalculationGroup();
            var targetGroupContext = new PipingCalculationGroupContext(targetGroup,
                                                                       Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                       Enumerable.Empty<PipingSoilProfile>());
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
            mockRepository.ReplayAll();
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
            
            var originalOwnerGroup = new PipingCalculationGroup();
            originalOwnerGroup.Children.Add(draggedItem);
            var originalOwnerGroupContext = new PipingCalculationGroupContext(originalOwnerGroup,
                                                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                              Enumerable.Empty<PipingSoilProfile>());

            var newOwnerGroup = new PipingCalculationGroup();
            var newOwnerGroupContext = new PipingCalculationGroupContext(newOwnerGroup,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var originalOwnerObserver = mockRepository.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var updatewasCalled = false;
            var newOwnerObserver = mockRepository.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver()).WhenCalled(invocation => updatewasCalled = true);

            var newOwnerGroupContextNode = mockRepository.Stub<ITreeNode>();

            var preUpdateDraggedItemContextNode = mockRepository.Stub<ITreeNode>();
            preUpdateDraggedItemContextNode.Tag = draggedItemContext;
            preUpdateDraggedItemContextNode.Expect(n => n.IsExpanded).Return(false);
            preUpdateDraggedItemContextNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var postUpdateDraggedItemContextNode = mockRepository.Stub<ITreeNode>();
            postUpdateDraggedItemContextNode.Tag = draggedItemContext;
            postUpdateDraggedItemContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);
            postUpdateDraggedItemContextNode.Expect(n => n.IsExpanded).Return(true);
            postUpdateDraggedItemContextNode.Expect(n => n.Collapse());
            postUpdateDraggedItemContextNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            newOwnerGroupContextNode.Tag = newOwnerGroupContext;
            newOwnerGroupContextNode.Expect(n => n.IsExpanded).Return(false).Repeat.Times(3);
            newOwnerGroupContextNode.Expect(n => n.Expand()); // Must expand new owner, otherwise selecting dragged node could not be visible when parent is collapsed.
            newOwnerGroupContextNode.Expect(n => n.Nodes).WhenCalled(invocation =>
            {
                if (updatewasCalled)
                {
                    invocation.ReturnValue = new[]
                    {
                        postUpdateDraggedItemContextNode
                    };
                }
            }).Return(new ITreeNode[0]).Repeat.Times(2);

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

            var originalOwnerGroup = new PipingCalculationGroup();
            originalOwnerGroup.Children.Add(existingItemStub);
            originalOwnerGroup.Children.Add(draggedItem);
            originalOwnerGroup.Children.Add(existingItemStub);
            var originalOwnerGroupContext = new PipingCalculationGroupContext(originalOwnerGroup,
                                                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                              Enumerable.Empty<PipingSoilProfile>());

            bool updatewasCalled = false;
            var originalOwnerObserver = mockRepository.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver()).WhenCalled(invocation => updatewasCalled = true);

            var newOwnerGroupContextNode = mockRepository.Stub<ITreeNode>();

            var preUpdateDraggedItemContextNode = CreateTreeNodeLeafForData(draggedItemContext);

            var postUpdateDraggedItemContextNode = CreateTreeNodeLeafForData(draggedItemContext);
            postUpdateDraggedItemContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);

            var existingItemStub1 = CreateStubTreeNode();
            var existingItemStub2 = CreateStubTreeNode();

            newOwnerGroupContextNode.Expect(n => n.IsExpanded).Return(true).Repeat.Times(3);
            newOwnerGroupContextNode.Tag = originalOwnerGroupContext;
            newOwnerGroupContextNode.Expect(n => n.Nodes).WhenCalled(invocation =>
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
            }).Return(new[]
            {
                existingItemStub1,
                preUpdateDraggedItemContextNode,
                existingItemStub2
            }).Repeat.Times(2);

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

            var originalOwnerGroup = new PipingCalculationGroup();
            originalOwnerGroup.Children.Add(draggedItem);
            var originalOwnerGroupContext = new PipingCalculationGroupContext(originalOwnerGroup,
                                                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                              Enumerable.Empty<PipingSoilProfile>());

            var newOwnerGroup = new PipingCalculationGroup();
            var newOwnerGroupContext = new PipingCalculationGroupContext(newOwnerGroup,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var sameNamedItem = mockRepository.Stub<IPipingCalculationItem>();
            sameNamedItem.Stub(i => i.Name).Return(draggedItem.Name);

            var originalOwnerObserver = mockRepository.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var updateWasCalled = false;
            var newOwnerObserver = mockRepository.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver()).WhenCalled(invocation => updateWasCalled = true);

            var newOwnerGroupContextNode = mockRepository.Stub<ITreeNode>();
            var preUpdateCalculationContextNode = mockRepository.Stub<ITreeNode>();
            preUpdateCalculationContextNode.Tag = draggedItemContext;
            preUpdateCalculationContextNode.Expect(n => n.IsExpanded).Return(true);
            preUpdateCalculationContextNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var postUpdateCalculationContextNode = mockRepository.Stub<ITreeNode>();
            postUpdateCalculationContextNode.Tag = draggedItemContext;
            postUpdateCalculationContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);
            postUpdateCalculationContextNode.Expect(n => n.IsExpanded).Return(false);
            postUpdateCalculationContextNode.Expect(n => n.Expand());
            postUpdateCalculationContextNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            newOwnerGroupContextNode.Tag = originalOwnerGroupContext;
            newOwnerGroupContextNode.Expect(n => n.IsExpanded).Return(false).Repeat.Times(3);
            newOwnerGroupContextNode.Expect(n => n.Expand()); // Must expand new owner, otherwise selecting dragged node could not be visible when parent is collapsed.
            newOwnerGroupContextNode.Expect(n => n.Nodes).WhenCalled(invocation =>
            {
                if (updateWasCalled)
                {
                    invocation.ReturnValue = new[]
                    {
                        postUpdateCalculationContextNode
                    };
                }
            }).Return(new ITreeNode[0]).Repeat.Times(2);

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

            var originalOwnerGroup = new PipingCalculationGroup();
            originalOwnerGroup.Children.Add(draggedItem);
            var originalOwnerGroupContext = new PipingCalculationGroupContext(originalOwnerGroup,
                                                                              Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                              Enumerable.Empty<PipingSoilProfile>());

            var newOwnerGroup = new PipingCalculationGroup();
            var newOwnerGroupContext = new PipingCalculationGroupContext(newOwnerGroup,
                                                                 Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                                 Enumerable.Empty<PipingSoilProfile>());

            var originalOwnerObserver = mockRepository.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var updatewasCalled = false;
            var newOwnerObserver = mockRepository.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver()).WhenCalled(invocation => updatewasCalled = true);

            var nodeStub = CreateStubTreeNode();

            var expandedNodeData = new object();
            var preUpdateExpandedNode = mockRepository.Stub<ITreeNode>();
            preUpdateExpandedNode.Tag = expandedNodeData;
            preUpdateExpandedNode.Expect(n => n.IsExpanded).Return(true);
            preUpdateExpandedNode.Stub(n => n.Nodes).Return(new[]
            {
                nodeStub
            });

            var postUpdateExpandedNode = mockRepository.Stub<ITreeNode>();
            postUpdateExpandedNode.Tag = expandedNodeData;
            postUpdateExpandedNode.Expect(n => n.IsExpanded).Return(false);
            postUpdateExpandedNode.Expect(n => n.Expand());
            postUpdateExpandedNode.Stub(n => n.Nodes).Return(new[]
            {
                nodeStub
            });

            var collapsedNodeData = new object();
            var preUpdateCollapsedNode = mockRepository.Stub<ITreeNode>();
            preUpdateCollapsedNode.Tag = collapsedNodeData;
            preUpdateCollapsedNode.Expect(n => n.IsExpanded).Return(true);
            preUpdateCollapsedNode.Stub(n => n.Nodes).Return(new[]
            {
                nodeStub
            });

            var postUpdateCollapsedNode = mockRepository.Stub<ITreeNode>();
            postUpdateCollapsedNode.Tag = collapsedNodeData;
            postUpdateCollapsedNode.Expect(n => n.IsExpanded).Return(false);
            postUpdateCollapsedNode.Expect(n => n.Expand());
            postUpdateCollapsedNode.Stub(n => n.Nodes).Return(new[]
            {
                nodeStub
            });

            var newOwnerGroupContextNode = mockRepository.Stub<ITreeNode>();
            newOwnerGroupContextNode.Tag = newOwnerGroupContext;
            newOwnerGroupContextNode.Expect(n => n.IsExpanded).Return(false).Repeat.Times(3); // 2x for recording and restoring state, 1x for expanding
            newOwnerGroupContextNode.Expect(n => n.Expand()); // Must expand new owner, otherwise selecting dragged node could not be visible when parent is collapsed.
            newOwnerGroupContextNode.Stub(n => n.Nodes).WhenCalled(invocation =>
            {
                if (updatewasCalled)
                {
                    invocation.ReturnValue = new[]
                    {
                        postUpdateExpandedNode,
                        postUpdateCollapsedNode
                    };
                }
            }).Return(new[]
            {
                preUpdateExpandedNode,
                preUpdateCollapsedNode
            });

            var preUpdateDraggedItemContextNode = mockRepository.Stub<ITreeNode>();
            preUpdateDraggedItemContextNode.Tag = draggedItemContext;
            preUpdateDraggedItemContextNode.Expect(n => n.IsExpanded).Return(false);
            preUpdateDraggedItemContextNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

            var postUpdateDraggedItemContextNode = mockRepository.Stub<ITreeNode>();
            postUpdateDraggedItemContextNode.Tag = draggedItemContext;
            postUpdateDraggedItemContextNode.Expect(n => n.Parent).Return(newOwnerGroupContextNode);
            postUpdateDraggedItemContextNode.Expect(n => n.IsExpanded).Return(true);
            postUpdateDraggedItemContextNode.Expect(n => n.Collapse());
            postUpdateDraggedItemContextNode.Stub(n => n.Nodes).Return(new List<ITreeNode>());

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
            var node = mockRepository.Stub<ITreeNode>();
            var guiCommandHandler = mockRepository.Stub<IGuiCommandHandler>();

            mockRepository.ReplayAll();

            var menuBuilder = new ContextMenuBuilder(guiCommandHandler, node);

            var contextMenuBuilderProvider = new SimpleContextMenuBuilderProvider(menuBuilder);

            var group = new PipingCalculationGroup();
            group.Children.Add(new PipingCalculation
            {
                Output = new TestPipingOutput()
            });
            var nodeData = new PipingCalculationGroupContext(group,
                                                             Enumerable.Empty<RingtoetsPipingSurfaceLine>(),
                                                             Enumerable.Empty<PipingSoilProfile>());

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProvider);

            // Call
            ContextMenuStrip menu = nodePresenter.GetContextMenu(node, nodeData);

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
                                                          "Valideren",
                                                          "Valideer alle berekeningen binnen deze berekeningsmap.",
                                                          PipingFormsResources.ValidationIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuCalculateAllIndex,
                                                          RingtoetsFormsResources.Calculate_all,
                                                          "Valideer en voer alle berekeningen binnen deze berekeningsmap uit.",
                                                          RingtoetsFormsResources.CalculateAllIcon);
            TestHelper.AssertContextMenuStripContainsItem(menu, contextMenuClearOutputIndex,
                                                          "&Wis alle uitvoer",
                                                          "Wis de uitvoer van alle berekeningen binnen deze berekeningsmap.",
                                                          RingtoetsFormsResources.ClearIcon);

            TestHelper.AssertContextMenuStripContainsItem(menu, 7, 
                                                          CoreCommonGuiResources.Expand_all,
                                                          CoreCommonGuiResources.Expand_all_ToolTip,
                                                          CoreCommonGuiResources.ExpandAllIcon,
                                                          false);
            TestHelper.AssertContextMenuStripContainsItem(menu, 8,
                                                          CoreCommonGuiResources.Collapse_all,
                                                          CoreCommonGuiResources.Collapse_all_ToolTip,
                                                          CoreCommonGuiResources.CollapseAllIcon,
                                                          false);

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
                                                          CoreCommonGuiResources.Properties,
                                                          CoreCommonGuiResources.Properties_ToolTip,
                                                          CoreCommonGuiResources.PropertiesIcon,
                                                          false);
            CollectionAssert.AllItemsAreInstancesOfType(new[] { menu.Items[2], menu.Items[6], menu.Items[9], menu.Items[12] }, typeof(ToolStripSeparator));

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
        public void GetContextMenu_ClickOnClearOutputItem_ClearOutputAllChildCalculationsAndNotifyCalculationObservers()
        {
            // Setup
            var node = mockRepository.StrictMock<ITreeNode>();

            var calculation1Observer = mockRepository.StrictMock<IObserver>();
            calculation1Observer.Expect(o => o.UpdateObserver());

            var calculation2Observer = mockRepository.StrictMock<IObserver>();
            calculation2Observer.Expect(o => o.UpdateObserver());

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

            // Precondition
            Assert.IsTrue(group.HasOutput);

            var nodePresenter = new PipingCalculationGroupContextNodePresenter(contextMenuBuilderProvider);

            var contextMenu = nodePresenter.GetContextMenu(node, nodeData);

            // Call
            contextMenu.Items[contextMenuClearOutputIndex].PerformClick();

            // Assert
            Assert.IsFalse(group.HasOutput);
            Assert.IsFalse(calculation1.HasOutput);
            Assert.IsFalse(calculation2.HasOutput);
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