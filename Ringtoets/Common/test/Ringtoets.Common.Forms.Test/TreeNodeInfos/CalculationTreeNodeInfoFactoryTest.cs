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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;

using RingtoetsFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.Common.Forms.Test.TreeNodeInfos
{
    [TestFixture]
    public class CalculationTreeNodeInfoFactoryTest
    {
        [Test]
        public void TextOfCalculationGroupContextTreeNodeInfo_Always_ReturnsWrappedDataName()
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
        public void ImageOfCalculationGroupContextTreeNodeInfo_Always_ReturnsFolderIcon()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var image = treeNodeInfo.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreateOfCalculationGroupContextTreeNodeInfo_Always_ReturnsTrue()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var result = treeNodeInfo.EnsureVisibleOnCreate(null);

            // Assert
            Assert.IsTrue(result);
        }

        [Test]
        public void CanRenameNodeOfCalculationGroupContextTreeNodeInfo_NestedCalculationGroup_ReturnsTrue()
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
        public void CanRenameNodeOfCalculationGroupContextTreeNodeInfo_WithoutParentNodeDefaultBehavior_ReturnsFalse()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool isRenamingAllowed = treeNodeInfo.CanRename(null, null);

            // Assert
            Assert.IsFalse(isRenamingAllowed);
        }

        [Test]
        public void OnNodeRenamedOfCalculationGroupContextTreeNodeInfo_WithData_RenameGroupAndNotifyObservers()
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
        public void CanRemoveOfCalculationGroupContextTreeNodeInfo_NestedCalculationGroup_ReturnsTrue()
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
        public void CanRemoveOfCalculationGroupContextTreeNodeInfo_WithoutParentNodeDefaultBehavior_ReturnsFalse()
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
        public void CanDragOfCalculationGroupContextTreeNodeInfo_NestedCalculationGroup_ReturnsTrue()
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
        public void CanDragOfCalculationGroupContextTreeNodeInfo_WithoutParentNodeDefaultBehavior_ReturnsFalse()
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
        public void CanDropOrCanInsertOfCalculationGroupContextTreeNodeInfo_DraggingPipingCalculationItemContextOntoGroupNotContainingItem_ReturnsTrue(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(CalculationType.Calculation, CalculationType.Group)] CalculationType draggedItemType)
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            object draggedItemContext;
            ICalculationBase draggedItem;
            CreateCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, failureMechanism);

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
        public void CanDropOrInsertOfCalculationGroupContextTreeNodeInfo_DraggingCalculationItemContextOntoGroupNotContainingItemOtherFailureMechanism_ReturnsFalse(
            [Values(DragDropTestMethod.CanDrop, DragDropTestMethod.CanInsert)] DragDropTestMethod methodToTest,
            [Values(CalculationType.Calculation, CalculationType.Group)] CalculationType draggedItemType)
        {
            // Setup
            var mocks = new MockRepository();
            var sourceFailureMechanism = mocks.StrictMock<IFailureMechanism>();
            var targetFailureMechanism = mocks.StrictMock<IFailureMechanism>();

            mocks.ReplayAll();

            object draggedItemContext;
            ICalculationBase draggedItem;
            CreateCalculationAndContext(draggedItemType, out draggedItem, out draggedItemContext, targetFailureMechanism);

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
            public string Name { get; set; }

            public string Comments { get; set; }

            public bool HasOutput
            {
                get
                {
                    return false;
                }
            }

            public void ClearOutput()
            {

            }

            public void ClearHydraulicBoundaryLocation()
            {

            }

            public ICalculationInput GetObservableInput()
            {
                return null;
            }
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
        private static void CreateCalculationAndContext(CalculationType type, out ICalculationBase data, out object dataContext, IFailureMechanism failureMechanism, string initialName = null)
        {
            switch (type)
            {
                case CalculationType.Calculation:
                    var calculation = new TestCalculation();
                    if (initialName != null)
                    {
                        calculation.Name = initialName;
                    }
                    data = calculation;
                    dataContext = new TestCalculationContext(calculation, failureMechanism);
                    break;
                case CalculationType.Group:
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
        public enum CalculationType
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
    }
}