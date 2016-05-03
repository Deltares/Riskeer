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

using Core.Common.Base;
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
        public void CanRenameNodeOfCalculationGroupContextTreeNodeInfo_ParentIsCalculationGroupContext_ReturnTrue()
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
        public void CanRenameNodeOfCalculationGroupContextTreeNodeInfo_EverythingElse_ReturnFalse()
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
        public void CanRemoveOfCalculationGroupContextTreeNodeInfo_CalculationGroupWithoutParent_ReturnFalse()
        {
            // Setup
            var mocks = new MockRepository();
            var failureMechanismMock = mocks.StrictMock<IFailureMechanism>();
            mocks.ReplayAll();

            var group = new CalculationGroup();
            var nodeData = new TestCalculationGroupContext(group, failureMechanismMock);

            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            bool isRemovalAllowed = treeNodeInfo.CanRemove(nodeData, null);

            // Assert
            Assert.IsFalse(isRemovalAllowed);
            mocks.VerifyAll();
        }

        [Test]
        public void CanRemoveOfCalculationGroupContextTreeNodeInfo_NestedCalculationGroup_ReturnTrue()
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
    }
}