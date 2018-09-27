// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using System.Drawing;
using Core.Common.TestUtil;
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
            const GraphNodeShape shape = GraphNodeShape.None;
            const int lineWidth = 3;

            var style = new GraphNodeStyle(shape, fillColor, lineColor, lineWidth);

            GraphNode[] childNodes =
            {
                new GraphNode("<text>node 1</text>", new GraphNode[0], false, style),
                new GraphNode("<text>node 2</text>", new[]
                {
                    new GraphNode("<text>node 3</text>", new GraphNode[0], false, style)
                }, true, style)
            };

            const string content = "<text>test</text>";
            const bool isSelectable = false;

            // Call
            var node = new GraphNode(content, childNodes, isSelectable, style);

            // Assert
            Assert.AreEqual(content, node.Content);
            Assert.AreEqual(isSelectable, node.IsSelectable);
            Assert.AreSame(style, node.Style);
            Assert.AreSame(childNodes, node.ChildNodes);
        }

        [Test]
        public void Constructor_WithoutStyling_ExpectedValues()
        {
            // Setup
            GraphNode[] childNodes =
            {
                new GraphNode("<text>node 1</text>", new GraphNode[0], false),
                new GraphNode("<text>node 2</text>", new[]
                {
                    new GraphNode("<text>node 3</text>", new GraphNode[0], true)
                }, true)
            };

            const string content = "<text>test</text>";
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
            Assert.AreSame(childNodes, node.ChildNodes);
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
            TestDelegate call = () => new GraphNode("<text>test</text>", null, false);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("childNodes", exception.ParamName);
        }

        [Test]
        public void Constructor_StyleNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => new GraphNode("<text>test</text>", new GraphNode[0], false, null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("style", exception.ParamName);
        }

        [Test]
        [TestCase("plain text")]
        [TestCase("<text>test")]
        [TestCase("<text>test <italic>italic</italic></text>")]
        public void Constructor_InvalidContent_ThrowsArgumentException(string content)
        {
            // Call
            TestDelegate call = () => new GraphNode(content, new GraphNode[0], false);

            // Assert
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<ArgumentException>(call, "Content is of invalid format.");
        }
    }
}