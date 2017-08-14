// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Core.Components.PointedTree.Data;
using NUnit.Framework;

namespace Core.Components.PointedTree.Test.Data
{
    [TestFixture]
    public class GraphNodeTest
    {
        [Test]
        public void Constructor_WithStyling_ExpectedValues()
        {
            // Setup
            Color fillColor = Color.Aqua;
            Color lineColor = Color.Brown;
            const GraphNodeShape shape = GraphNodeShape.Diamond;
            const int lineWidth = 3;

            var style = new GraphNodeStyle(shape, fillColor, lineColor, lineWidth);

            GraphNode[] childNodes =
            {
                new GraphNode("node 1", new GraphNode[0], false, style),
                new GraphNode("node 2", new[]
                {
                    new GraphNode("node 3", new GraphNode[0], false, style)
                }, true, style)
            };

            const string content = "test";
            const bool isSelectable = false;

            // Call
            var node = new GraphNode(content, childNodes, isSelectable, style);

            // Assert
            Assert.AreEqual(content, node.Content);
            Assert.AreEqual(isSelectable, node.IsSelectable);
            Assert.AreSame(style, node.Style);

            GraphNode[] actualChildNodes = node.ChildNodes.ToArray();
            AssertChildNodes(childNodes, actualChildNodes);
        }

        [Test]
        public void Constructor_WithoutStyling_ExpectedValues()
        {
            // Setup
            GraphNode[] childNodes =
            {
                new GraphNode("node 1", new GraphNode[0], false),
                new GraphNode("node 2", new[]
                {
                    new GraphNode("node 3", new GraphNode[0], true)
                }, true)
            };

            const string content = "test";
            const bool isSelectable = false;

            // Call
            var node = new GraphNode(content, childNodes, isSelectable);

            // Assert
            Assert.AreEqual(content, node.Content);
            Assert.AreEqual(isSelectable, node.IsSelectable);
            Assert.AreEqual(GraphNodeShape.Rectangle, node.Style.Shape);
            Assert.AreEqual(Color.Gray, node.Style.FillColor);
            Assert.AreEqual(Color.Black, node.Style.LineColor);
            Assert.AreEqual(2, node.Style.LineWidth);

            GraphNode[] actualChildNodes = node.ChildNodes.ToArray();
            AssertChildNodes(childNodes, actualChildNodes);
        }

        [Test]
        public void Constructor_ContentNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GraphNode(null, new GraphNode[0], false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("content", exception.ParamName);
        }

        [Test]
        public void Constructor_ChildNodesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GraphNode("test", null, false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("childNodes", exception.ParamName);
        }

        [Test]
        public void Constructor_StyleNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GraphNode("test", new GraphNode[0], false, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("style", exception.ParamName);
        }

        private static void AssertChildNodes(IList<GraphNode> childNodes, IList<GraphNode> actualChildNodes)
        {
            Assert.AreEqual(childNodes.Count, actualChildNodes.Count);

            for (var i = 0; i < childNodes.Count; i++)
            {
                Assert.AreEqual(childNodes[i], actualChildNodes[i]);
                AssertChildNodes(childNodes[i].ChildNodes.ToArray(), actualChildNodes[i].ChildNodes.ToArray());
            }
        }
    }
}