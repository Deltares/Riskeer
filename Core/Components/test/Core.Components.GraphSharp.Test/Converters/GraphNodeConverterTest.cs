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
using WpfBrush = System.Windows.Media.Brush;
using WpfColor = System.Windows.Media.Color;

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
            const int shapeAsInteger = 99;
            var graphNode = new GraphNode("<text>test</text>", new GraphNode[0], false,
                                          new GraphNodeStyle((GraphNodeShape) shapeAsInteger, Color.AliceBlue,
                                                             Color.AntiqueWhite, 2));

            // Call
            TestDelegate test = () => GraphNodeConverter.Convert(graphNode);

            // Assert
            string message = $"The value of argument 'shape' ({shapeAsInteger}) is invalid for Enum type '{typeof(GraphNodeShape).Name}'.";
            TestHelper.AssertThrowsArgumentExceptionAndTestMessage<InvalidEnumArgumentException>(test, message);
        }

        [Test]
        [TestCase(GraphNodeShape.None, PointedTreeVertexType.None)]
        [TestCase(GraphNodeShape.Rectangle, PointedTreeVertexType.Rectangle)]
        public void Convert_WithGraphNode_ReturnPointedTreeElementVertex(GraphNodeShape graphNodeShape,
                                                                         PointedTreeVertexType expectedVertexType)
        {
            // Setup
            var random = new Random(21);

            const string content = "<text>Node</text>";
            bool isSelectable = random.NextBoolean();
            int lineWidth = random.Next();
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

        private static void AssertColors(Color expectedColor, WpfBrush brushColor)
        {
            Assert.IsInstanceOf<SolidColorBrush>(brushColor);
            WpfColor actualColor = ((SolidColorBrush) brushColor).Color;
            Assert.AreEqual(expectedColor.A, actualColor.A);
            Assert.AreEqual(expectedColor.R, actualColor.R);
            Assert.AreEqual(expectedColor.G, actualColor.G);
            Assert.AreEqual(expectedColor.B, actualColor.B);
        }
    }
}