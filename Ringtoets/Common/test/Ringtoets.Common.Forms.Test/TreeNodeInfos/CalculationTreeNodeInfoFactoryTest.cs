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
        public void Text_CreatedCalculationGroupContextTreeNodeInfo_AlwaysReturnsWrappedDataName()
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
        public void Image_CreatedCalculationGroupContextTreeNodeInfo_AlwaysReturnsFolderIcon()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var image = treeNodeInfo.Image(null);

            // Assert
            TestHelper.AssertImagesAreEqual(RingtoetsFormsResources.GeneralFolderIcon, image);
        }

        [Test]
        public void EnsureVisibleOnCreate_CreatedCalculationGroupContextTreeNodeInfo_AlwaysReturnsTrue()
        {
            // Setup
            var treeNodeInfo = CalculationTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<TestCalculationGroupContext>(null, null, null);

            // Call
            var result = treeNodeInfo.EnsureVisibleOnCreate(null);

            // Assert
            Assert.IsTrue(result);
        }

        private class TestCalculationGroupContext : ICalculationContext<CalculationGroup, IFailureMechanism>
        {
            public TestCalculationGroupContext(CalculationGroup wrappedData, IFailureMechanism failureMechanism)
            {
                WrappedData = wrappedData;
                FailureMechanism = failureMechanism;
            }

            public CalculationGroup WrappedData { get; private set; }

            public IFailureMechanism FailureMechanism { get; private set; }

            public void Attach(IObserver observer) {}

            public void Detach(IObserver observer) {}

            public void NotifyObservers() {}
        }
    }
}