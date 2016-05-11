// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using BaseResources = Core.Common.Base.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class CalculationTreeNodeInfoFactoryTest
    {
        # region CreateCalculationGroupContextTreeNodeInfo

        [Test]
        public void CreateCalculationGroupContextTreeNodeInfo_Always_ExpectedPropertiesSet()
        {
            // Setup
            Func<TestCalculationGroupContext, object[]> childNodeObjects = context => new object[0];
            Func<TestCalculationGroupContext, object, TreeViewControl, ContextMenuStrip> contextMenuStrip = (context, parent, treeViewControl) => new ContextMenuStrip();
            Action<TestCalculationGroupContext, object> onNodeRemoved = (context, parent) => { };

            // Call
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo(childNodeObjects, contextMenuStrip, onNodeRemoved);

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

            var groupName = "testName";
            var group = new CalculationGroup
            {
                Name = groupName
            };
            var groupContext = new TestCalculationGroupContext(group, failureMechanismMock);
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var text = treeNodeInfo.Text(groupContext);

            // Assert
            Assert.AreEqual(groupName, text);
            mocks.VerifyAll();
        }

        [Test]
        public void Image_CalculationGroup_Always_ReturnsFolderIcon()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var image = treeNodeInfo.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreate_CalculationGroup_ForFailureMechanismCalculationGroup_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanismContext<IFailureMechanism>>();
            mocks.ReplayAll();

            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);
            // Call
            var result = treeNodeInfo.EnsureVisibleOnCreate(null, failureMechanismMock);

            // Assert
            Assert.IsFalse(result);
            mocks.VerifyAll();
        }

        [Test]
        public void EnsureVisibleOnCreate_CalculationGroup_AnyOtherObject_ReturnsTrue()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var result = treeNodeInfo.EnsureVisibleOnCreate(null, null);

            // Assert
            Assert.IsTrue(result);
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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

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
            var observer = mocks.StrictMock<IObserver>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            observer.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            const string newName = "new name";
            var group = new CalculationGroup();
            var nodeData = new TestCalculationGroupContext(group, failureMechanismMock);

            nodeData.Attach(observer);

            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var canDrag = treeNodeInfo.CanDrag(groupContext, parentGroupContext);

            // Assert
            Assert.IsTrue(canDrag);
        }

        [Test]
        public void CanDrag_CalculationGroup_WithoutParentNodeDefaultBehavior_ReturnsFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            var groupContext = new TestCalculationGroupContext(new CalculationGroup(), failureMechanismMock);
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var canDrag = treeNodeInfo.CanDrag(groupContext, null);

            // Assert
            Assert.IsFalse(canDrag);
        }

        [Test]
        [Combinatorial]
        public void CanDropOrCanInsert_CalculationGroup_DragCalculationItemOntoGroupNotContainingItem_ReturnsTrue(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(CalculationItemType.Calculation, CalculationItemType.Group)] CalculationItemType draggedItemType)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            object draggedItemContext;
            ICalculationBase draggedItem;
            CreateCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanism);

            CalculationGroup targetGroup;
            TestCalculationGroupContext targetGroupContext;
            CreateCalculationGroupAndContext(out targetGroup, out targetGroupContext, failureMechanism);

            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    var canDrop = treeNodeInfo.CanDrop(draggedItemContext, targetGroupContext);

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
            var sourceFailureMechanism = mocks.StrictMock<IFailureMechanism>();
            var targetFailureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            object draggedItemContext;
            ICalculationBase draggedItem;
            CreateCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, targetFailureMechanism);

            CalculationGroup targetGroup;
            TestCalculationGroupContext targetGroupContext;
            CreateCalculationGroupAndContext(out targetGroup, out targetGroupContext, sourceFailureMechanism);

            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            switch (methodToTest)
            {
                case DragDropTestMethod.CanDrop:
                    // Call
                    var canDrop = treeNodeInfo.CanDrop(draggedItemContext, targetGroupContext);

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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            var newOwnerObserver = mocks.StrictMock<IObserver>();

            originalOwnerObserver.Expect(o => o.UpdateObserver());
            newOwnerObserver.Expect(o => o.UpdateObserver());

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

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.DoesNotContain(newOwnerGroup.Children, draggedItem);

            // Call
            treeNodeInfo.OnDrop(draggedItemContext, newOwnerGroupContext, originalOwnerGroupContext, 0, treeViewControlMock);

            // Assert
            CollectionAssert.DoesNotContain(originalOwnerGroup.Children, draggedItem);
            CollectionAssert.Contains(newOwnerGroup.Children, draggedItem);
            Assert.AreSame(draggedItem, newOwnerGroup.Children.Last(),
                           "Dragging node at the end of the target TestCalculationGroup should put the dragged data at the end of 'newOwnerGroup'.");

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
            var treeViewControlMock = mocks.StrictMock<TreeViewControl>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            var existingItemStub = mocks.Stub<ICalculationBase>();
            var originalOwnerObserver = mocks.StrictMock<IObserver>();

            existingItemStub.Stub(ci => ci.Name).Return("");
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            mocks.ReplayAll();

            const string name = "Very cool name";

            object draggedItemContext;
            ICalculationBase draggedItem;
            CreateCalculationItemAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanismMock, name);

            CalculationGroup originalOwnerGroup;
            TestCalculationGroupContext originalOwnerGroupContext;
            CreateCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, failureMechanismMock);

            originalOwnerGroup.Children.Add(existingItemStub);
            originalOwnerGroup.Children.Add(draggedItem);
            originalOwnerGroup.Children.Add(existingItemStub);

            originalOwnerGroup.Attach(originalOwnerObserver);

            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Precondition
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);

            // Call
            treeNodeInfo.OnDrop(draggedItemContext, originalOwnerGroupContext, originalOwnerGroupContext, newIndex, treeViewControlMock);

            // Assert
            CollectionAssert.Contains(originalOwnerGroup.Children, draggedItem);
            Assert.AreNotSame(draggedItem, originalOwnerGroup.Children[1],
                              "Should have removed 'draggedItem' from its original location in the collection.");
            Assert.AreSame(draggedItem, originalOwnerGroup.Children[newIndex],
                           "Dragging node to specific location within owning TestCalculationGroup should put the dragged data at that index.");
            Assert.AreEqual(name, draggedItem.Name,
                            "No renaming should occur when dragging within the same TestCalculationGroup.");

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

            CalculationGroup originalOwnerGroup;
            TestCalculationGroupContext originalOwnerGroupContext;
            CreateCalculationGroupAndContext(out originalOwnerGroup, out originalOwnerGroupContext, failureMechanismMock);
            originalOwnerGroup.Children.Add(draggedItem);

            CalculationGroup newOwnerGroup;
            TestCalculationGroupContext newOwnerGroupContext;
            CreateCalculationGroupAndContext(out newOwnerGroup, out newOwnerGroupContext, failureMechanismMock);

            var sameNamedItem = mocks.Stub<ICalculationBase>();
            sameNamedItem.Stub(sni => sni.Name).Return(draggedItem.Name);

            var originalOwnerObserver = mocks.StrictMock<IObserver>();
            originalOwnerObserver.Expect(o => o.UpdateObserver());

            var newOwnerObserver = mocks.StrictMock<IObserver>();
            newOwnerObserver.Expect(o => o.UpdateObserver());

            treeViewControlMock.Expect(tvc => tvc.TryRenameNodeForData(draggedItemContext));

            mocks.ReplayAll();

            newOwnerGroup.Children.Add(sameNamedItem);

            originalOwnerGroup.Attach(originalOwnerObserver);
            newOwnerGroup.Attach(newOwnerObserver);

            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

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
        private void CreateCalculationGroupAndContext(out CalculationGroup data, out TestCalculationGroupContext dataContext, IFailureMechanism failureMechanism)
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
        /// <exception cref="System.NotSupportedException"></exception>
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
            var icon = RingtoetsFormsResources.CalculateIcon;
            Func<TestCalculationContext, object[]> childNodeObjects = context => new object[0];
            Func<TestCalculationContext, object, TreeViewControl, ContextMenuStrip> contextMenuStrip = (context, parent, treeViewControl) => new ContextMenuStrip();
            Action<TestCalculationContext, object> onNodeRemoved = (context, parent) => { };

            // Call
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo(icon, childNodeObjects, contextMenuStrip, onNodeRemoved);

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

            var calculationName = "calculationName";
            var calculation = new TestCalculation
            {
                Name = calculationName
            };

            var context = new TestCalculationContext(calculation, failureMechanismMock);
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null, null);

            // Call
            var text = treeNodeInfo.Text(context);

            // Assert
            Assert.AreEqual(calculationName, text);
            mocks.VerifyAll();
        }

        [Test]
        public void EnsureVisibleOnCreateOfCalculationContextTreeNodeInfo_Always_ReturnsTrue()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null, null);

            // Call
            var result = treeNodeInfo.EnsureVisibleOnCreate(null, null);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanRenameCalculationContextTreeNodeInfo_Always_ReturnTrue()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null, null);

            // Call
            var renameAllowed = treeNodeInfo.CanRename(null, null);
        
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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null, null);

            var groupContext = new TestCalculationGroupContext(group, failureMechanismMock);
        
            // Call
            bool removalAllowed = treeNodeInfo.CanRemove(context, groupContext);
        
            // Assert
            Assert.IsTrue(removalAllowed);
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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null, null);

            var groupContext = new TestCalculationGroupContext(group, failureMechanismMock);
        
            // Call
            bool removalAllowed = treeNodeInfo.CanRemove(context, groupContext);
        
            // Assert
            Assert.IsFalse(removalAllowed);
        }
        
        [Test]
        public void CanRemoveCalculationContextTreeNodeInfo_EverythingElse_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var dataMock = mocks.StrictMock<object>();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var nodeMock = new TestCalculationContext(new TestCalculation(), failureMechanismMock);
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null, null);
        
            // Call
            bool removalAllowed = treeNodeInfo.CanRemove(nodeMock, dataMock);
        
            // Assert
            Assert.IsFalse(removalAllowed);
            mocks.VerifyAll(); // Expect no calls on arguments
        }

        [Test]
        public void CanDragCalculationContextTreeNodeInfo_Always_ReturnTrue()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<TestCalculationContext>(null, null, null, null);

            // Call
            var canDrag = treeNodeInfo.CanDrag(null, null);
        
            // Assert
            Assert.IsTrue(canDrag);
        }

        #endregion

        # region CreateFailureMechanismContextTreeNodeInfo

        [Test]
        public void CreateFailureMechanismContextTreeNodeInfo_Always_ExpectedPropertiesSet()
        {
            // Call
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<FailureMechanismContext<IFailureMechanism>>(null, null, null, null);

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
            var assessmentSection = mocks.Stub<IAssessmentSection>();

            failureMechanism.Stub(fm => fm.Name).Return(name);

            mocks.ReplayAll();

            var context = new TestFailureMechanismContext(failureMechanism, assessmentSection);
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(null, null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<FailureMechanismContext<IFailureMechanism>>(null, null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(null, null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(null, null, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(mechanismContext => resultIsRelevant, mechanismContext => resultIsNotRelevant, null, null);

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
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(mechanismContext => resultIsRelevant, mechanismContext => resultIsNotRelevant, null, null);

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
                var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(null, null, 
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
                ContextMenuStrip result = treeNodeInfo.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                Assert.AreSame(contextMenuStripRelevant, result);
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
                var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<TestFailureMechanismContext>(null, null,
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
                ContextMenuStrip result = treeNodeInfo.ContextMenuStrip(context, assessmentSection, treeView);

                // Assert
                Assert.AreSame(contextMenuStripNotRelevant, result);
                mocks.VerifyAll();
            }
        }

        # endregion

        # region Nested types

        private class TestCalculationGroupContext : Observable, ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; private set; }

            public IFailureMechanism FailureMechanism { get; private set; }
        }

        private class TestCalculationContext : Observable, ICalculationContext<TestCalculation, IFailureMechanism>
        {
            public TestCalculationContext(TestCalculation wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public TestCalculation WrappedData { get; private set; }

            public IFailureMechanism FailureMechanism { get; private set; }
        }

        private class TestCalculation : Observable, ICalculation
        {
            public TestCalculation()
            {
                Name = "Nieuwe berekening";
            }

            public string Name { get; set; }

            public string Comments { get; set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput() {}

            public void ClearHydraulicBoundaryLocation() {}

            public ICalculationInput GetObservableInput()
            {
                return null;
            }
        }

        private class TestFailureMechanismContext : FailureMechanismContext<IFailureMechanism>
        {
            public TestFailureMechanismContext(IFailureMechanism wrappedFailureMechanism, IAssessmentSection parent) : base(wrappedFailureMechanism, parent) { }
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

        # endregion
    }
}