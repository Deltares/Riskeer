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
using Core.Components.PointedTree.Data;
using NUnit.Framework;
using Ringtoets.Common.Data.IllustrationPoints;
using Ringtoets.Common.Data.TestUtil.IllustrationPoints;
using Ringtoets.Common.Forms.Views;

namespace Ringtoets.Common.Forms.Test
{
    [TestFixture]
    public class GraphNodeItemTest
    {
        [Test]
        public void Constructor_GraphNodeItemNull_ThrowsArgumentNullException()
        {
            // Setup
            var illustrationPointNode = new IllustrationPointNode(new TestIllustrationPoint());

            // Call
            TestDelegate test = () => new GraphNodeItem(null, illustrationPointNode);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("graphNode", exception.ParamName);
        }

        [Test]
        public void Constructor_IllustrationPointNodeItemNull_ThrowsArgumentNullException()
        {
            // Setup
            var graphNode = new GraphNode("<text />", new GraphNode[0], false);

            // Call
            TestDelegate test = () => new GraphNodeItem(graphNode, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("illustrationPointNode", exception.ParamName);
        }

        [Test]
        public void Constructor_ValidArguments_ExpectedProperties()
        {
            // Setup
            var graphNode = new GraphNode("<text />", new GraphNode[0], false);
            var illustrationPointNode = new IllustrationPointNode(new TestIllustrationPoint());

            // Call
            var graphNodeItem = new GraphNodeItem(graphNode, illustrationPointNode);

            // Assert
            Assert.AreSame(graphNode, graphNodeItem.GraphNode);
            Assert.AreSame(illustrationPointNode, graphNodeItem.IllustrationPointNode);
        }
    }
}