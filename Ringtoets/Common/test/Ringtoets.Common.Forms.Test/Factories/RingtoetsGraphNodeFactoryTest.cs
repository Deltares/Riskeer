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
using System.Drawing;
using System.Linq;
using Core.Components.PointedTree.Data;
using NUnit.Framework;
using Ringtoets.Common.Forms.Factories;

namespace Ringtoets.Common.Forms.Test.Factories
{
    [TestFixture]
    public class RingtoetsGraphNodeFactoryTest
    {
        [Test]
        public void CreateEndGraphNode_ValidInput_ReturnsGraphNodeWithExpectedStyling()
        {
            // Setup
            const string title = "title";
            const string content = "content";

            // Call
            GraphNode node = RingtoetsGraphNodeFactory.CreateEndGraphNode(title, content);

            // Assert
            string expectedContent = $"<text><bold>{title}</bold>{Environment.NewLine}{content}</text>";
            Assert.AreEqual(expectedContent, node.Content);
            Assert.IsTrue(node.IsSelectable);

            var expectedStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.SkyBlue, Color.Black, 1);
            AssertEqualStyle(expectedStyle, node.Style);

            CollectionAssert.IsEmpty(node.ChildNodes);
        }

        [Test]
        public void CreateCompositeGraphNode_ValidInput_ReturnsGraphNodeWithExpectedStyling()
        {
            // Setup
            const string title = "compositeTitle";
            const string content = "compositeContent";

            GraphNode childNode = new TestGraphNode();

            // Call
            GraphNode node = RingtoetsGraphNodeFactory.CreateCompositeGraphNode(title, content, new[]
            {
                childNode
            });

            // Assert
            string expectedContent = $"<text><bold>{title}</bold>{Environment.NewLine}{content}</text>";
            Assert.AreEqual(expectedContent, node.Content);
            Assert.IsTrue(node.IsSelectable);

            var expectedStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.LightSkyBlue, Color.Black, 1);
            AssertEqualStyle(expectedStyle, node.Style);

            Assert.AreEqual(1, node.ChildNodes.Count());
            Assert.AreSame(childNode, node.ChildNodes.First());
        }

        [Test]
        public void CreateCompositeGraphNode_ChildNodesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsGraphNodeFactory.CreateCompositeGraphNode("title",
                                                                                         "content",
                                                                                         null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("childNodes", exception.ParamName);
        }

        [Test]
        public void CreateConnectingGraphNode_ValidInput_ReturnsGraphNodeWithExpectedStyling()
        {
            // Setup
            const string title = "title";
            GraphNode childNode = new TestGraphNode();

            // Call
            GraphNode node = RingtoetsGraphNodeFactory.CreateConnectingGraphNode(title, new[]
            {
                childNode
            });

            // Assert
            string expectedContent = $"<text>{title}</text>";
            Assert.AreEqual(expectedContent, node.Content);
            Assert.IsFalse(node.IsSelectable);

            var expectedStyle = new GraphNodeStyle(GraphNodeShape.Rectangle, Color.BlanchedAlmond, Color.Black, 1);
            AssertEqualStyle(expectedStyle, node.Style);

            Assert.AreEqual(1, node.ChildNodes.Count());
            Assert.AreSame(childNode, node.ChildNodes.First());
        }

        [Test]
        public void CreateConnectingGraphNode_ChildNodesNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate test = () => RingtoetsGraphNodeFactory.CreateConnectingGraphNode("title",
                                                                                          null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(test);
            Assert.AreEqual("childNodes", exception.ParamName);
        }

        private static void AssertEqualStyle(GraphNodeStyle expected, GraphNodeStyle actual)
        {
            Assert.AreEqual(expected.FillColor, actual.FillColor);
            Assert.AreEqual(expected.LineColor, actual.LineColor);
            Assert.AreEqual(expected.LineWidth, actual.LineWidth);
            Assert.AreEqual(expected.Shape, actual.Shape);
        }

        private class TestGraphNode : GraphNode
        {
            public TestGraphNode() : base("<text>content</text>", new GraphNode[0], false, new TestGraphNodeStyle()) {}
        }

        private class TestGraphNodeStyle : GraphNodeStyle
        {
            public TestGraphNodeStyle() : base(GraphNodeShape.None, Color.Empty, Color.Empty, 1) {}
        }
    }
}