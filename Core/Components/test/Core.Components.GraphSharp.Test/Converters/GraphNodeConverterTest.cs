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
using System.ComponentModel;
using System.Windows.Media;
using Core.Common.TestUtil;
using Core.Components.GraphSharp.Converters;
using Core.Components.GraphSharp.Data;
using Core.Components.PointedTree.Data;
using NUnit.Framework;
using Color = System.Drawing.Color;
using wpfColor = System.Windows.Media.Color;

namespace Core.Components.GraphSharp.Test.Converters
{
    [TestFixture]
    public class GraphNodeConverterTest
    {
        [Test]
        public void Convert_GraphNodeNull_ThrowsArgumentNullException()
        {
            // Call
            TestDelegate call = () => GraphNodeConverter.Convert(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(call);
            Assert.AreEqual("graphNode", exception.ParamName);
        }

        [Test]
        public void Convert_InvalidShapeType_ThrowsInvalidEnumArgumentException()
        {
            // Setup
            var graphNode = new GraphNode("test", new GraphNode[0], false,
                                          new GraphNodeStyle((GraphNodeShape) 99, Color.AliceBlue,
                                                             Color.AntiqueWhite, 2));

            // Call
            TestDelegate test = () => GraphNodeConverter.Convert(graphNode);

            // Assert
            const string message = "The value of argument 'shape' (99) is invalid for Enum type 'GraphNodeShape'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(GraphNodeShape.Diamond, PointedTreeVertexType.Diamond)]
        [TestCase(GraphNodeShape.Rectangle, PointedTreeVertexType.Rectangle)]
        public void Convert_WithGraphNode_ReturnPointedTreeElementVertex(GraphNodeShape graphNodeShape,
                                                                         PointedTreeVertexType expectedVertexType)
        {
            // Setup
            const string content = "Node";
            const bool isSelectable = false;
            const int lineWidth = 3;
            Color fillColor = Color.Aquamarine;
            Color lineColor = Color.Coral;

            var graphNode = new GraphNode(content, new GraphNode[0], isSelectable,
                                          new GraphNodeStyle(graphNodeShape, fillColor,
                                                             lineColor, lineWidth));

            // Call
            PointedTreeElementVertex vertex = GraphNodeConverter.Convert(graphNode);

            // Assert
            Assert.AreEqual(content, vertex.Content);
            Assert.AreEqual(isSelectable, vertex.IsSelectable);
            Assert.AreEqual(expectedVertexType, vertex.Type);
            Assert.AreEqual(lineWidth, vertex.LineWidth);
            AssertColors(fillColor, vertex.FillColor);
            AssertColors(lineColor, vertex.LineColor);
        }

        private static void AssertColors(Color color, Brush brushColor)
        {
            Assert.AreEqual(new SolidColorBrush(wpfColor.FromArgb(color.A, color.R, color.G, color.B)).ToString(), brushColor.ToString());
        }
    }
}