// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;
using Core.Common.TestUtil;
using NUnit.Framework;
using Rhino.Mocks;
using Riskeer.HydraRing.Calculation.Data.Output.IllustrationPoints;

namespace Riskeer.HydraRing.Calculation.Test.Data.Output.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointTreeNodeTest
    {
        [Test]
        public void Constructor_WithoutData_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => new IllustrationPointTreeNode(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_WithData_DataIsAssigned()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<IIllustrationPoint>();
            mocks.ReplayAll();

            // Call
            var node = new IllustrationPointTreeNode(data);

            // Assert
            Assert.AreSame(data, node.Data);
            CollectionAssert.IsEmpty(node.Children);
            mocks.VerifyAll();
        }

        [Test]
        public void SetChildren_ChildrenNull_ThrowsArgumentNullException()
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<IIllustrationPoint>();
            mocks.ReplayAll();

            var treeNode = new IllustrationPointTreeNode(data);

            // Call
            TestDelegate call = () => treeNode.SetChildren(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("children", exception.ParamName);

            mocks.VerifyAll();
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        public void SetChildren_InvalidNrOfChildren_ThrowsInvalidArgumentException(int nrOfChildren)
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<IIllustrationPoint>();
            mocks.ReplayAll();

            var treeNode = new IllustrationPointTreeNode(data);
            var childrenToBeAttached = new IllustrationPointTreeNode[nrOfChildren];

            // Call
            TestDelegate call = () => treeNode.SetChildren(childrenToBeAttached);

            // Assert
            const string expectedMessage = "Een illustratiepunt node in de foutenboom moet 0 of 2 onderliggende nodes hebben.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            Assert.AreEqual("children", exception.ParamName);
            CollectionAssert.IsEmpty(treeNode.Children);
            mocks.VerifyAll();
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void SetChildren_ValidNrOfChildren_ReturnsExpectedProperties(int nrOfChildren)
        {
            // Setup
            var mocks = new MockRepository();
            var data = mocks.Stub<IIllustrationPoint>();
            mocks.ReplayAll();

            var treeNode = new IllustrationPointTreeNode(data);
            var childrenToBeAttached = new IllustrationPointTreeNode[nrOfChildren];

            // Call
            treeNode.SetChildren(childrenToBeAttached);

            // Assert
            IEnumerable<IllustrationPointTreeNode> addedChildren = treeNode.Children;
            Assert.AreSame(childrenToBeAttached, addedChildren);
            Assert.AreEqual(nrOfChildren, addedChildren.Count());
            mocks.VerifyAll();
        }
    }
}