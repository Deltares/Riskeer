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

using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms.Layout;
using Core.Components.PointedTree.Data;
using Core.Components.PointedTree.Forms;
using NUnit.Framework;
using WPFExtensions.Controls;

namespace Core.Components.GraphSharp.Forms.Test
{
    [TestFixture]
    [Apartment(ApartmentState.STA)]
    public class PointedTreeGraphControlTest
    {
        [Test]
        public void Constructor_ExptedValues()
        {
            // Call
            using (var graphControl = new PointedTreeGraphControl())
            {
                // Assert
                Assert.IsInstanceOf<UserControl>(graphControl);
                Assert.IsInstanceOf<IPointedTreeGraphControl>(graphControl);

                Assert.IsNull(graphControl.Data);

                Assert.AreEqual(1, graphControl.Controls.Count);

                var elementHost = graphControl.Controls[0] as ElementHost;
                var zoomControl = (ZoomControl) elementHost.Child;

                Assert.AreEqual(1, zoomControl.Resources.MergedDictionaries.Count);
                ResourceDictionary templateDictionary = zoomControl.Resources.MergedDictionaries.First();
                Assert.AreEqual("/Core.Components.GraphSharp.Forms;component/Templates/PointedTreeGraphTemplate.xaml", templateDictionary.Source.AbsolutePath);

                var graphLayout = (PointedTreeGraphLayout) zoomControl.Content;
                Assert.IsInstanceOf<PointedTreeGraph>(graphLayout.Graph);
            }
        }

        [Test]
        public void GivenGraphControlWithoutData_WhenDataSetToGraphNode_ThenGraphControlUpdated()
        {
            // Given
            using (var graphControl = new PointedTreeGraphControl())
            {
                var doubleUsedNode = new GraphNode("<text>Double used</text>", new GraphNode[0], false);
                var node = new GraphNode("<text>Root</text>", new[]
                {
                    new GraphNode("<text>Child 1</text>", new[]
                    {
                        doubleUsedNode
                    }, false),
                    new GraphNode("<text>Child 2</text>", new[]
                    {
                        doubleUsedNode
                    }, false)
                }, false);

                // When
                graphControl.Data = node;

                // Then
                var elementHost = graphControl.Controls[0] as ElementHost;
                var zoomControl = (ZoomControl) elementHost.Child;
                var graphLayout = (PointedTreeGraphLayout) zoomControl.Content;
                PointedTreeGraph graph = graphLayout.Graph;

                Assert.AreEqual(5, graph.VertexCount);
                Assert.AreEqual(4, graph.EdgeCount);
            }
        }

        [Test]
        public void GivenGraphControlWithData_WhenDataSetToOtherGraphNode_ThenGraphControlUpdated()
        {
            // Given
            var node = new GraphNode("<text>Root</text>", new[]
            {
                new GraphNode("<text>Child 1</text>", new GraphNode[0], false)
            }, false);

            using (var graphControl = new PointedTreeGraphControl
            {
                Data = node
            })
            {
                var elementHost = graphControl.Controls[0] as ElementHost;
                var zoomControl = (ZoomControl) elementHost.Child;
                var graphLayout = (PointedTreeGraphLayout) zoomControl.Content;
                PointedTreeGraph graph = graphLayout.Graph;

                // Precondition
                Assert.AreEqual(2, graph.VertexCount);
                Assert.AreEqual(1, graph.EdgeCount);

                // When
                graphControl.Data = new GraphNode("<text>Double used</text>", new GraphNode[0], false);

                // Then
                Assert.AreEqual(1, graph.VertexCount);
                Assert.AreEqual(0, graph.EdgeCount);
            }
        }

        [Test]
        public void GivenGraphControlWithData_WhenDataSetToNull_ThenGraphControlUpdated()
        {
            // Given
            var node = new GraphNode("<text>Root</text>", new[]
            {
                new GraphNode("<text>Child 1</text>", new GraphNode[0], false)
            }, false);

            using (var graphControl = new PointedTreeGraphControl
            {
                Data = node
            })
            {
                var elementHost = graphControl.Controls[0] as ElementHost;
                var zoomControl = (ZoomControl) elementHost.Child;
                var graphLayout = (PointedTreeGraphLayout) zoomControl.Content;
                PointedTreeGraph graph = graphLayout.Graph;

                // Precondition
                Assert.AreEqual(2, graph.VertexCount);
                Assert.AreEqual(1, graph.EdgeCount);

                // When
                graphControl.Data = null;

                // Then
                Assert.AreEqual(0, graph.VertexCount);
                Assert.AreEqual(0, graph.EdgeCount);
            }
        }
    }
}