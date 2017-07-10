// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class FaultTreeIllustrationPointNodeTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new FaultTreeIllustrationPointNode(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("data", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ReturnExpectedValues()
        {
            // Setup
            var data = new TestIllustrationPoint();

            // Call
            var faultTreeNode = new FaultTreeIllustrationPointNode(data);

            // Assert
            Assert.AreSame(data, faultTreeNode.Data);
            Assert.IsEmpty(faultTreeNode.Children);
        }

        [Test]
        public void SetChildren_ChildrenNull_ThrowsArgumentNullException()
        {
            // Setup
            var faultTreeNode = new FaultTreeIllustrationPointNode(new TestIllustrationPoint());

            // Call
            TestDelegate call = () => faultTreeNode.SetChildren(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("children", exception.ParamName);
        }

        [Test]
        [TestCase(1)]
        [TestCase(3)]
        public void SetChildren_InvalidNrOfChildren_ThrowsInvalidArgumentException(int nrOfChildren)
        {
            // Setup
            var faultTreeNode = new FaultTreeIllustrationPointNode(new TestIllustrationPoint());

            var childrenToBeAttached = new List<FaultTreeIllustrationPointNode>();
            for (var i = 0; i < nrOfChildren; i++)
            {
                childrenToBeAttached.Add(new FaultTreeIllustrationPointNode(new TestIllustrationPoint()));
            }

            // Call
            TestDelegate call = () => faultTreeNode.SetChildren(childrenToBeAttached.ToArray());

            // Assert
            const string expectedMessage = "Een illustratiepunt node in de foutenboom moet 0 of 2 kind nodes hebben.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            Assert.AreEqual("children", exception.ParamName);
        }

        [Test]
        public void SetChildren_NoChildren_ReturnsExpectedProperties()
        {
            // Setup
            var faultTreeNode = new FaultTreeIllustrationPointNode(new TestIllustrationPoint());

            // Call
            faultTreeNode.SetChildren(new FaultTreeIllustrationPointNode[0]);

            // Assert
            CollectionAssert.IsEmpty(faultTreeNode.Children);
        }

        [Test]
        public void SetChildren_TwoChildren_ReturnsExpectedProperties()
        {
            // Setup
            var faultTreeNode = new FaultTreeIllustrationPointNode(new TestIllustrationPoint());

            var childrenToBeAttached = new[]
            {
                new FaultTreeIllustrationPointNode(new TestIllustrationPoint()),
                new FaultTreeIllustrationPointNode(new TestIllustrationPoint())
            };

            // Call
            faultTreeNode.SetChildren(childrenToBeAttached);

            // Assert
            CollectionAssert.AreEqual(childrenToBeAttached, faultTreeNode.Children);
        }
    }
}