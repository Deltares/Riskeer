﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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

using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class RingtoetsTreeNodeInfoFactoryTest
    {
        #region CreateCalculationGroupContextTreeNodeInfo

        [Test]
        public void CreateCalculationGroupContextTreeNodeInfo_Always_ExpectedPropertiesSet()
        {
            // Setup
            Func<TestCalculationGroupContext, object[]> childNodeObjects = context => new object[0];
            Func<TestCalculationGroupContext, object, TreeViewControl, ContextMenuStrip> contextMenuStrip = (context, parent, treeViewControl) => new ContextMenuStrip();
            Action<TestCalculationGroupContext, object> onNodeRemoved = (context, parent) => { };

            // Call
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo(childNodeObjects, contextMenuStrip, onNodeRemoved);

            // Assert
            Assert.AreEqual(typeof(TestCalculationGroupContext), treeNodeInfo.TagType);
            Assert.AreSame(childNodeObjects, treeNodeInfo.ChildNodeObjects);
            Assert.AreSame(contextMenuStrip, treeNodeInfo.ContextMenuStrip);
            Assert.AreSame(onNodeRemoved, treeNodeInfo.OnNodeRemoved);
            Assert.IsNull(treeNodeInfo.ForeColor);
            Assert.IsNull(treeNodeInfo.CanCheck);
            Assert.IsNull(treeNodeInfo.IsChecked);
            Assert.IsNull(treeNodeInfo.OnNodeChecked);
        }

        [Test]
        public void Text_CalculationGroup_Always_ReturnsWrappedDataName()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            const string groupName = "testName";
            var group = new CalculationGroup
            {
                Name = groupName
            };
            var groupContext = new TestCalculationGroupContext(group, failureMechanismMock);
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            string text = treeNodeInfo.Text(groupContext);

            // Assert
            Assert.AreEqual(groupName, text);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_CalculationGroup_Always_ReturnsFolderIcon()
        {
            // Setup
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            Image image = treeNodeInfo.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreate_CalculationGroup_ForCalculationGroup_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationGroup = new TestCalculationGroupContext(new CalculationGroup(), mocks.Stub<IFailureMechanism>());
            mocks.ReplayAll();

            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool result = treeNodeInfo.EnsureVisibleOnCreate(null, calculationGroup);

            // Assert
            Assert.IsTrue(result);
            mocks.VerifyAll();
        }

        [Test]
        public void EnsureVisibleOnCreate_CalculationGroup_AnyOtherObject_ReturnsFalse()
        {
            // Setup
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool result = treeNodeInfo.EnsureVisibleOnCreate(null, null);

            // Assert
            Assert.IsFalse(result);
        }

        [Test]
        public void CanRenameNode_CalculationGroup_NestedCalculationGroup_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var calculationGroupMock = mocks.StrictMock<CalculationGroup>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var groupContext = new TestCalculationGroupContext(calculationGroupMock, failureMechanismMock);
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool isRenamingAllowed = treeNodeInfo.CanRename(null, groupContext);

            // Assert
            Assert.IsTrue(isRenamingAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRenameNode_CalculationGroup_WithoutParentNodeDefaultBehavior_ReturnsFalse()
        {
            // Setup
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool isRenamingAllowed = treeNodeInfo.CanRename(null, null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void OnNodeRenamed_CalculationGroup_WithData_RenameGroupAndNotifyObservers()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            const string newName = "new name";
            var group = new CalculationGroup();
            var nodeData = new TestCalculationGroupContext(group, failureMechanismMock);

            nodeData.Attach(observerMock);

            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            treeNodeInfo.OnNodeRenamed(nodeData, newName);

            // Assert
            Assert.AreEqual(newName, group.Name);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_CalculationGroup_NestedCalculationGroup_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var nodeData = new TestCalculationGroupContext(new CalculationGroup(), failureMechanismMock);
            var parentNodeData = new TestCalculationGroupContext(new CalculationGroup(), failureMechanismMock);
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool isRemovalAllowed = treeNodeInfo.CanRemove(nodeData, parentNodeData);

            // Assert
            Assert.IsTrue(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemove_CalculationGroup_WithoutParentNodeDefaultBehavior_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var nodeData = new TestCalculationGroupContext(new CalculationGroup(), failureMechanismMock);
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool isRemovalAllowed = treeNodeInfo.CanRemove(nodeData, null);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_CalculationGroup_NestedCalculationGroup_ReturnsTrue()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var groupContext = new TestCalculationGroupContext(new CalculationGroup(), failureMechanismMock);
            var parentGroupContext = new TestCalculationGroupContext(new CalculationGroup(), failureMechanismMock);
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool canDrag = treeNodeInfo.CanDrag(groupContext, parentGroupContext);

            // Assert
            Assert.IsTrue(canDrag);
            mocks.VerifyAll();
        }

        [Test]
        public void CanDrag_CalculationGroup_WithoutParentNodeDefaultBehavior_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var groupContext = new TestCalculationGroupContext(new CalculationGroup(), failureMechanismMock);
            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool canDrag = treeNodeInfo.CanDrag(groupContext, null);

            // Assert
            Assert.IsFalse(canDrag);
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void CanDropOrCanInsert_CalculationGroup_DragCalculationItemOntoGroupNotContainingItem_ReturnsTrue(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(CalculationItemType.Calculation, CalculationItemType.Group)] CalculationItemType draggedItemType)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            object draggedItemContext;
            ICalculationBase draggedItem;
            CreateCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanismMock);

            CalculationGroup targetGroup;
            TestCalculationGroupContext targetGroupContext;
            CreateCalculationGroupAndContext(out targetGroup, out targetGroupContext, failureMechanismMock);

            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    bool canDrop = treeNodeInfo.CanDrop(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsTrue(canDrop);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = treeNodeInfo.CanInsert(draggedItemContext, targetGroupContext);

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
        public void CanDropOrInsert_CalculationGroup_DragCalculationItemOntoGroupNotContainingItemOtherFailureMechanism_ReturnsFalse(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(CalculationItemType.Calculation, CalculationItemType.Group)] CalculationItemType draggedItemType)
        {
            // Setup
            var mocks = new MockRepository();
            var sourceFailureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            var targetFailureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            object draggedItemContext;
            ICalculationBase draggedItem;
            CreateCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, targetFailureMechanismMock);

            CalculationGroup targetGroup;
            TestCalculationGroupContext targetGroupContext;
            CreateCalculationGroupAndContext(out targetGroup, out targetGroupContext, sourceFailureMechanismMock);

            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    bool canDrop = treeNodeInfo.CanDrop(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsFalse(canDrop);
                    break;
                case DragDropTestMethod.CanInsert:
                    // Call
                    bool canInsert = treeNodeInfo.CanInsert(draggedItemContext, targetGroupContext);

                    // Assert
                    Assert.IsFalse(canInsert);
                    break;
                default:
                    Assert.Fail(methodToTest + " not supported.");
                    break;
            }
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_CalculationGroup_DragCalculationItemOntoGroupEnd_MoveCalculationItemToNewGroup(
            [Values(CalculationItemType.Calculation, CalculationItemType.Group)] CalculationItemType draggedItemType)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            var originalOwnerObserverMock = mocks.StrictMock<IObserver>();
            originalOwnerObserverMock.Expect(o => o.UpdateObserver());
            var newOwnerObserverMock = mocks.StrictMock<IObserver>();
            newOwnerObserverMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            ICalculationBase draggedItem;
            object draggedItemContext;
            CreateCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanismMock);

            CalculationGroup originalOwnerGroup;
            TestCalculationGroupContext originalOwnerGroupContext;
            CreateCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, failureMechanismMock);
            originalOwnerGroup.Children.Add(draggedItem);

            CalculationGroup newOwnerGroup;
            TestCalculationGroupContext newOwnerGroupContext;
            CreateCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, failureMechanismMock);

            originalOwnerGroup.Attach(originalOwnerObserverMock);
            newOwnerGroup.Attach(newOwnerObserverMock);

            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                treeNodeInfo.OnDrop(draggedItemContext, newOwnerGroupContext, originalOwnerGroupContext, 0, treeViewControl);

                // Assert
                CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
                CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
                Assert.AreSame(draggedItem, newOwnerGroup.Children.Last(),
                               "Dragging node at the end of the target TestCalculationGroup should put the dragged data at the end of 'newOwnerGroup'.");
            }
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_CalculationGroup_InsertCalculationItemAtDifferentLocationWithinSameGroup_ChangeItemIndexOfCalculationItem(
            [Values(CalculationItemType.Calculation, CalculationItemType.Group)] CalculationItemType draggedItemType,
            [Values(0, 2)] int newIndex)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            var existingItemMock = mocks.StrictMock<ICalculationBase>();
            existingItemMock.Stub(ci => ci.Name).Return("");
            var originalOwnerObserverMock = mocks.StrictMock<IObserver>();
            originalOwnerObserverMock.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            const string name = "Very cool name";

            object draggedItemContext;
            ICalculationBase draggedItem;
            CreateCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanismMock, name);

            CalculationGroup originalOwnerGroup;
            TestCalculationGroupContext originalOwnerGroupContext;
            CreateCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, failureMechanismMock);

            originalOwnerGroup.Children.Add(existingItemMock);
            originalOwnerGroup.Children.Add(draggedItem);
            originalOwnerGroup.Children.Add(existingItemMock);

            originalOwnerGroup.Attach(originalOwnerObserverMock);

            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);

            using (var treeViewControl = new TreeViewControl())
            {
                // Call
                treeNodeInfo.OnDrop(draggedItemContext, originalOwnerGroupContext, originalOwnerGroupContext, newIndex, treeViewControl);

                // Assert
                CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
                Assert.AreNotSame(draggedItem, originalOwnerGroup.Children[1],
                                  "Should have removed 'draggedItem' from its original location in the collection.");
                Assert.AreSame(draggedItem, originalOwnerGroup.Children[newIndex],
                               "Dragging node to specific location within owning TestCalculationGroup should put the dragged data at that index.");
                Assert.AreEqual(name, draggedItem.Name,
                                "No renaming should occur when dragging within the same TestCalculationGroup.");
            }
            mocks.VerifyAll();
        }

        [Test]
        [Combinatorial]
        public void OnDrop_CalculationGroup_DragCalculationItemOntoGroupWithSameNamedItem_MoveCalculationItemToNewGroupAndRename(
            [Values(CalculationItemType.Calculation, CalculationItemType.Group)] CalculationItemType draggedItemType)
        {
            // Setup
            var mocks = new MockRepository();
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            ICalculationBase draggedItem;
            object draggedItemContext;

            CreateCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanismMock);
            treeViewControlMock.Expect(tvc => tvc.TryRenameNodeForData(draggedItemContext));

            CalculationGroup originalOwnerGroup;
            TestCalculationGroupContext originalOwnerGroupContext;
            CreateCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, failureMechanismMock);
            originalOwnerGroup.Children.Add(draggedItem);

            CalculationGroup newOwnerGroup;
            TestCalculationGroupContext newOwnerGroupContext;
            CreateCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, failureMechanismMock);

            var sameNamedItemMock = mocks.StrictMock<ICalculationBase>();
            sameNamedItemMock.Stub(sni => sni.Name).Return(draggedItem.Name);

            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var newOwnerObserver = mocks.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver());
            mocks.ReplayAll();

            newOwnerGroup.Children.Add(sameNamedItemMock);

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            TreeNodeInfo<TestCalculationGroupContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children.Select(c => c.Name), draggedItem.Name,
                                      "Name of the dragged item should already exist in new owner.");

            // Call
            treeNodeInfo.OnDrop(draggedItemContext, newOwnerGroupContext, originalOwnerGroupContext, 0, treeViewControlMock);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.First(),
                           "Dragging to insert node at start of newOwnerGroup should place the node at the start of the list.");
            switch (draggedItemType)
            {
                case CalculationItemType.Calculation:
                    Assert.AreEqual("Nieuwe berekening", draggedItem.Name);
                    break;
                case CalculationItemType.Group:
                    Assert.AreEqual("Nieuwe map", draggedItem.Name);
                    break;
            }

            mocks.VerifyAll();
        }

        /// <summary>
        /// Creates an instance of <see cref="CalculationGroup"/> and the corresponding <see cref="TestCalculationGroupContext"/>.
        /// </summary>
        /// <param name="data">The created group without any children.</param>
        /// <param name="dataContext">The context object for <paramref name="data"/>, without any other data.</param>
        /// <param name="failureMechanism">The failure mechanism the item and context it belong to.</param>
        private static void CreateCalculationGroupAndContext(out CalculationGroup data, out TestCalculationGroupContext dataContext, IFailureMechanism failureMechanism)
        {
            data = new CalculationGroup();
            dataContext = new TestCalculationGroupContext(data, failureMechanism);
        }

        /// <summary>
        /// Creates an instance of <see cref="ICalculationBase"/> and the corresponding context.
        /// </summary>
        /// <param name="type">Defines the implementation of <see cref="ICalculationBase"/> to be constructed.</param>
        /// <param name="data">Output: The concrete create class based on <paramref name="type"/>.</param>
        /// <param name="dataContext">Output: The context corresponding with <paramref name="data"/>.</param>
        /// <param name="failureMechanism">The failure mechanism the item and context belong to.</param>
        /// <param name="initialName">Optional: The name of <paramref name="data"/>.</param>
        /// <exception cref="NotSupportedException"></exception>
        private static void CreateCalculationItemAndContext(CalculationItemType type, out ICalculationBase data, out object dataContext, IFailureMechanism failureMechanism, string initialName = null)
        {
            switch (type)
            {
                case CalculationItemType.Calculation:
                    var calculation = new TestCalculation();
                    if (initialName != null)
                    {
                        calculation.Name = initialName;
                    }
                    data = calculation;
                    dataContext = new TestCalculationContext(calculation, failureMechanism);
                    break;
                case CalculationItemType.Group:
                    var group = new CalculationGroup();
                    if (initialName != null)
                    {
                        group.Name = initialName;
                    }
                    data = group;
                    dataContext = new TestCalculationGroupContext(group, failureMechanism);
                    break;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion

        #region CreateCalculationContextTreeNodeInfo

        [Test]
        public void CreateCalculationContextTreeNodeInfo_Always_ExpectedPropertiesSet()
        {
            // Setup
            Bitmap icon = RingtoetsFormsResources.CalculationIcon;
            Func<TestCalculationContext, object[]> childNodeObjects = context => new object[0];
            Func<TestCalculationContext, object, TreeViewControl, ContextMenuStrip> contextMenuStrip = (context, parent, treeViewControl) => new ContextMenuStrip();
            Action<TestCalculationContext, object> onNodeRemoved = (context, parent) => { };

            // Call
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo(childNodeObjects, contextMenuStrip, onNodeRemoved);

            // Assert
            Assert.AreEqual(typeof(TestCalculationContext), treeNodeInfo.TagType);
            TestHelper.AssertImagesAreEqual(icon, treeNodeInfo.Image(null));
            Assert.AreSame(childNodeObjects, treeNodeInfo.ChildNodeObjects);
            Assert.AreSame(contextMenuStrip, treeNodeInfo.ContextMenuStrip);
            Assert.AreSame(onNodeRemoved, treeNodeInfo.OnNodeRemoved);
            Assert.IsNull(treeNodeInfo.ForeColor);
            Assert.IsNull(treeNodeInfo.CanCheck);
            Assert.IsNull(treeNodeInfo.IsChecked);
            Assert.IsNull(treeNodeInfo.OnNodeChecked);
        }

        [Test]
        public void TextOfCalculationContextTreeNodeInfo_Always_ReturnsWrappedDataName()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            const string calculationName = "calculationName";
            var calculation = new TestCalculation
            {
                Name = calculationName
            };

            var context = new TestCalculationContext(calculation, failureMechanismMock);
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null);

            // Call
            string text = treeNodeInfo.Text(context);

            // Assert
            Assert.AreEqual(calculationName, text);
            mocks.VerifyAll();
        }

        [Test]
        public void EnsureVisibleOnCreateOfCalculationContextTreeNodeInfo_Always_ReturnsTrue()
        {
            // Setup
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null);

            // Call
            bool result = treeNodeInfo.EnsureVisibleOnCreate(null, null);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanRenameCalculationContextTreeNodeInfo_Always_ReturnTrue()
        {
            // Setup
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null);

            // Call
            bool renameAllowed = treeNodeInfo.CanRename(null, null);

            // Assert
            Assert.IsTrue(renameAllowed);
        }

        [Test]
        public void OnNodeRenamedOfCalculationContextTreeNodeInfo_Always_SetNewNameToCalculationItemAndNotifyObserver()
        {
            // Setup
            var mocks = new MockRepository();
            var observerMock = mocks.StrictMock<IObserver>();
            observerMock.Expect(o => o.UpdateObserver());
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculation = new TestCalculation
            {
                Name = "<Original name>"
            };

            var context = new TestCalculationContext(calculation, failureMechanismMock);
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null);

            context.WrappedData.Attach(observerMock);

            // Call
            const string newName = "<Insert New Name Here>";
            treeNodeInfo.OnNodeRenamed(context, newName);

            // Assert
            Assert.AreEqual(newName, calculation.Name);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRemoveCalculationContextTreeNodeInfo_ParentIsCalculationGroupWithCalculation_ReturnTrue(bool groupNameEditable)
        {
            // Setup
            var calculationToBeRemoved = new TestCalculation();
            var group = new CalculationGroup("", groupNameEditable);
            group.Children.Add(calculationToBeRemoved);

            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new TestCalculationContext(calculationToBeRemoved, failureMechanismMock);
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null);

            var groupContext = new TestCalculationGroupContext(group, failureMechanismMock);

            // Call
            bool removalAllowed = treeNodeInfo.CanRemove(context, groupContext);

            // Assert
            Assert.IsTrue(removalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void CanRemoveCalculationContextTreeNodeInfo_ParentIsCalculationGroupWithoutCalculation_ReturnFalse(bool groupNameEditable)
        {
            // Setup
            var calculationToBeRemoved = new TestCalculation();
            var group = new CalculationGroup("", groupNameEditable);

            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var context = new TestCalculationContext(calculationToBeRemoved, failureMechanismMock);
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null);

            var groupContext = new TestCalculationGroupContext(group, failureMechanismMock);

            // Call
            bool removalAllowed = treeNodeInfo.CanRemove(context, groupContext);

            // Assert
            Assert.IsFalse(removalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemoveCalculationContextTreeNodeInfo_EverythingElse_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var calculationContext = new TestCalculationContext(new TestCalculation(), failureMechanismMock);
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null);

            // Call
            bool removalAllowed = treeNodeInfo.CanRemove(calculationContext, dataMock);

            // Assert
            Assert.IsFalse(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanDragCalculationContextTreeNodeInfo_Always_ReturnTrue()
        {
            // Setup
            TreeNodeInfo<TestCalculationContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null);

            // Call
            bool canDrag = treeNodeInfo.CanDrag(null, null);

            // Assert
            Assert.IsTrue(canDrag);
        }

        #endregion

        #region CreateFailureMechanismContextTreeNodeInfo

        [Test]
        public void CreateFailureMechanismContextTreeNodeInfo_Always_ExpectedPropertiesSet()
        {
            // Call
            TreeNodeInfo<FailureMechanismContext<IFailureMechanism>> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<FailureMechanismContext<IFailureMechanism>>(null, null, null, null);

            // Assert
            Assert.AreEqual(typeof(FailureMechanismContext<IFailureMechanism>), treeNodeInfo.TagType);
            Assert.IsNotNull(treeNodeInfo.Text);
            Assert.IsNotNull(treeNodeInfo.Image);
            Assert.IsNotNull(treeNodeInfo.ForeColor);
            Assert.IsNotNull(treeNodeInfo.ChildNodeObjects);
            Assert.IsNotNull(treeNodeInfo.ContextMenuStrip);
            Assert.IsNull(treeNodeInfo.EnsureVisibleOnCreate);
            Assert.IsNull(treeNodeInfo.CanRename);
            Assert.IsNull(treeNodeInfo.OnNodeRenamed);
            Assert.IsNull(treeNodeInfo.CanRemove);
            Assert.IsNull(treeNodeInfo.OnNodeRemoved);
            Assert.IsNull(treeNodeInfo.CanCheck);
            Assert.IsNull(treeNodeInfo.IsChecked);
            Assert.IsNull(treeNodeInfo.OnNodeChecked);
            Assert.IsNull(treeNodeInfo.CanDrag);
            Assert.IsNull(treeNodeInfo.CanDrop);
            Assert.IsNull(treeNodeInfo.CanInsert);
            Assert.IsNull(treeNodeInfo.OnDrop);
        }

        [Test]
        public void Text_FailureMechanism_Always_ReturnsWrappedDataName()
        {
            // Setup
            const string name = "A";

            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            failureMechanism.Stub(fm => fm.Name).Return(name);
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            TreeNodeInfo<TestFailureMechanismContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(null, null, null, null);

            // Call
            string text = treeNodeInfo.Text(context);

            // Assert
            Assert.AreEqual(name, text);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_FailureMechanism_Always_ReturnsFailureMechanismIcon()
        {
            // Setup
            TreeNodeInfo<FailureMechanismContext<IFailureMechanism>> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<FailureMechanismContext<IFailureMechanism>>(null, null, null, null);

            // Call
            Image image = treeNodeInfo.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.FailureMechanismIcon, image);
        }

        [Test]
        public void ForeColor_FailureMechanismIsRelevant_ReturnsControlText()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            failureMechanism.IsRelevant = true;

            var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            TreeNodeInfo<TestFailureMechanismContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(null, null, null, null);

            // Call
            Color color = treeNodeInfo.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.ControlText), color);
            mocks.VerifyAll();
        }

        [Test]
        public void ForeColor_FailureMechanismIsNotRelevant_ReturnsGrayText()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            failureMechanism.IsRelevant = false;

            var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            TreeNodeInfo<TestFailureMechanismContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(null, null, null, null);

            // Call
            Color color = treeNodeInfo.ForeColor(context);

            // Assert
            Assert.AreEqual(Color.FromKnownColor(KnownColor.GrayText), color);
            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsRelevant_ReturnResultFromConstructorMethod()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            failureMechanism.IsRelevant = true;

            var resultIsRelevant = new[]
            {
                new object(),
                1.1
            };

            var resultIsNotRelevant = new[]
            {
                2.2,
                new object()
            };

            var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            TreeNodeInfo<TestFailureMechanismContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(mechanismContext => resultIsRelevant, mechanismContext => resultIsNotRelevant, null, null);

            // Call
            object[] children = treeNodeInfo.ChildNodeObjects(context);

            // Assert
            CollectionAssert.AreEqual(resultIsRelevant, children);

            mocks.VerifyAll();
        }

        [Test]
        public void ChildNodeObjects_FailureMechanismIsNotRelevant_ReturnResultFromConstructorMethod()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.Stub<IFailureMechanism>();
            var assessmentSection = mocks.Stub<IAssessmentSection>();
            mocks.ReplayAll();

            failureMechanism.IsRelevant = false;

            var resultIsRelevant = new[]
            {
                new object(),
                1.1
            };

            var resultIsNotRelevant = new[]
            {
                2.2,
                new object()
            };

            var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            TreeNodeInfo<TestFailureMechanismContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(mechanismContext => resultIsRelevant, mechanismContext => resultIsNotRelevant, null, null);

            // Call
            object[] children = treeNodeInfo.ChildNodeObjects(context);

            // Assert
            CollectionAssert.AreEqual(resultIsNotRelevant, children);

            mocks.VerifyAll();
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsRelevant_ReturnResultFromConstructorMethod()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            using (var contextMenuStripRelevant = new ContextMenuStrip())
            using (var contextMenuStripNotRelevant = new ContextMenuStrip())
            {
                var mocks = new MockRepository();
                var failureMechanism = mocks.Stub<IFailureMechanism>();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                mocks.ReplayAll();

                failureMechanism.IsRelevant = true;

                var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);
                TreeNodeInfo<TestFailureMechanismContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(
                    null,
                    null,
                    (mechanismContext, parent, treeViewControl) =>
                    {
                        Assert.AreEqual(context, mechanismContext);
                        Assert.AreEqual(assessmentSection, parent);
                        Assert.AreEqual(treeView, treeViewControl);

                        return contextMenuStripRelevant;
                    },
                    (mechanismContext, parent, treeViewControl) =>
                    {
                        Assert.AreEqual(context, mechanismContext);
                        Assert.AreEqual(assessmentSection, parent);
                        Assert.AreEqual(treeView, treeViewControl);

                        return contextMenuStripNotRelevant;
                    });

                // Call
                using (ContextMenuStrip contextMenuStrip = treeNodeInfo.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreSame(contextMenuStripRelevant, contextMenuStrip);
                }
                mocks.VerifyAll();
            }
        }

        [Test]
        public void ContextMenuStrip_FailureMechanismIsNotRelevant_ReturnResultFromConstructorMethod()
        {
            // Setup
            using (var treeView = new TreeViewControl())
            using (var contextMenuStripRelevant = new ContextMenuStrip())
            using (var contextMenuStripNotRelevant = new ContextMenuStrip())
            {
                var mocks = new MockRepository();
                var failureMechanism = mocks.Stub<IFailureMechanism>();
                var assessmentSection = mocks.Stub<IAssessmentSection>();
                mocks.ReplayAll();

                failureMechanism.IsRelevant = false;

                var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);
                TreeNodeInfo<TestFailureMechanismContext> treeNodeInfo = RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(
                    null,
                    null,
                    (mechanismContext, parent, treeViewControl) =>
                    {
                        Assert.AreEqual(context, mechanismContext);
                        Assert.AreEqual(assessmentSection, parent);
                        Assert.AreEqual(treeView, treeViewControl);

                        return contextMenuStripRelevant;
                    },
                    (mechanismContext, parent, treeViewControl) =>
                    {
                        Assert.AreEqual(context, mechanismContext);
                        Assert.AreEqual(assessmentSection, parent);
                        Assert.AreEqual(treeView, treeViewControl);

                        return contextMenuStripNotRelevant;
                    });

                // Call
                using (ContextMenuStrip result = treeNodeInfo.ContextMenuStrip(context, assessmentSection, treeView))
                {
                    // Assert
                    Assert.AreSame(contextMenuStripNotRelevant, result);
                }
                mocks.VerifyAll();
            }
        }

        #endregion

        #region Nested types

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, IFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; }

            public IFailureMechanism FailureMechanism { get; }
        }

        private class TestCalculation : Observable, ICalculation
        {
            public TestCalculation()
            {
                Name = "Nieuwe berekening";
            }

            public string Name { get; set; }

            public Comment Comments { get; private set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() {}
        }

        private class TestFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public TestFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) : base(wrappedFailureMechanism, parent) {}
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
        /// Type indicator for implementations of <see cref="ICalculationBase"/> to be created in a test.
        /// </summary>
        public enum CalculationItemType
        {
            /// <summary>
            /// Indicates <see cref="ICalculation"/>.
            /// </summary>
            Calculation,

            /// <summary>
            /// Indicates <see cref="CalculationGroup"/>.
            /// </summary>
            Group
        }

        #endregion
    }
}