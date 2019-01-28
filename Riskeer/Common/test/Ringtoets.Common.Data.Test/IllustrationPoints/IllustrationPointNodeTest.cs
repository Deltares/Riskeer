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
using Core.Common.Data.TestUtil;
using Core.Common.TestUtil;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;

namespace Ringtoets.Common.Data.Test.IllustrationPoints
{
    [TestFixture]
    public class IllustrationPointNodeTest
    {
        [Test]
        public void Constructor_DataNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new IllustrationPointNode(null);

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
            var illustrationPointNode = new IllustrationPointNode(data);

            // Assert
            Assert.IsInstanceOf<ICloneable>(illustrationPointNode);
            Assert.AreSame(data, illustrationPointNode.Data);
            CollectionAssert.IsEmpty(illustrationPointNode.Children);
        }

        [Test]
        public void SetChildren_ChildrenNull_ThrowsArgumentNullException()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestIllustrationPoint());

            // Call
            TestDelegate call = () => illustrationPointNode.SetChildren(null);

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
            var illustrationPointNode = new IllustrationPointNode(new TestIllustrationPoint());
            var childrenToBeAttached = new IllustrationPointNode[nrOfChildren];

            // Call
            TestDelegate call = () => illustrationPointNode.SetChildren(childrenToBeAttached);

            // Assert
            const string expectedMessage = "Een illustratiepunt node in de foutenboom moet 0 of 2 onderliggende nodes hebben.";
            var exception = TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, expectedMessage);
            Assert.AreEqual("children", exception.ParamName);
            CollectionAssert.IsEmpty(illustrationPointNode.Children);
        }

        [Test]
        [TestCase(0)]
        [TestCase(2)]
        public void SetChildren_ValidNrOfChildren_ReturnsExpectedProperties(int nrOfChildren)
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestIllustrationPoint());
            var childrenToBeAdded = new IllustrationPointNode[nrOfChildren];

            // Call
            illustrationPointNode.SetChildren(childrenToBeAdded);

            // Assert
            IEnumerable<IllustrationPointNode> addedChildren = illustrationPointNode.Children;
            Assert.AreSame(childrenToBeAdded, addedChildren);
            Assert.AreEqual(nrOfChildren, addedChildren.Count());
        }

        [Test]
        public void SetChildren_ChildNamesNotUnique_ThrowArgumentException()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestFaultTreeIllustrationPoint("Top"));
            var childrenToBeAdded = new[]
            {
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint("A")),
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint("A"))
            };

            // Call
            TestDelegate test = () => illustrationPointNode.SetChildren(childrenToBeAdded);

            // Assert
            const string expectedMessage = "Een of meerdere illustratiepunten bevatten illustratiepunten met dezelfde naam.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void SetChildren_ParentDoesNotContainSameStochastsAsChild_ThrowArgumentException()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new FaultTreeIllustrationPoint("Top",
                                                                                                 0.0,
                                                                                                 new[]
                                                                                                 {
                                                                                                     new Stochast("Stochast A", 0, 0)
                                                                                                 },
                                                                                                 CombinationType.And));
            var childrenToBeAdded = new[]
            {
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint(new[]
                {
                    new Stochast("Stochast B", 0, 0)
                })),
                new IllustrationPointNode(new TestFaultTreeIllustrationPoint("B"))
            };

            // Call
            TestDelegate test = () => illustrationPointNode.SetChildren(childrenToBeAdded);

            // Assert
            const string expectedMessage = "De stochasten van een illustratiepunt bevatten niet dezelfde stochasten als in de onderliggende illustratiepunten.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(test, expectedMessage);
        }

        [Test]
        public void Clone_Always_ReturnNewInstanceWithCopiedValues()
        {
            // Setup
            var original = new IllustrationPointNode(new TestIllustrationPoint());

            original.SetChildren(new[]
            {
                new IllustrationPointNode(new TestIllustrationPoint()),
                new IllustrationPointNode(new TestIllustrationPoint())
            });

            // Call
            object clone = original.Clone();

            // Assert
            CoreCloneAssert.AreObjectClones(original, clone, CommonCloneAssert.AreClones);
        }
    }
}