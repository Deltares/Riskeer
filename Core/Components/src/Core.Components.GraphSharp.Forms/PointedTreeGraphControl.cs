// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.ComponentModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using Core.Common.Util.Extensions;
using Core.Components.GraphSharp.Converters;
using Core.Components.GraphSharp.Data;
using Core.Components.GraphSharp.Forms.Layout;
using Core.Components.PointedTree.Data;
using Core.Components.PointedTree.Forms;
using WPFExtensions.Controls;

namespace Core.Components.GraphSharp.Forms
{
    /// <summary>
    /// This class describes a graph control with configured projection and function mode.
    /// </summary>
    public partial class PointedTreeGraphControl : UserControl, IPointedTreeGraphControl
    {
        private readonly List<DrawnGraphNode> drawnGraphNodeList = new List<DrawnGraphNode>();
        private ZoomControl zoomControl;
        private GraphNode data;
        private PointedTreeGraph graph;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="PointedTreeGraphControl"/>.
        /// </summary>
        public PointedTreeGraphControl()
        {
            InitializeComponent();
            InitializeZoomControl();
        }

        public GraphNode Data
        {
            get
            {
                return data;
            }
            set
            {
                ClearData();
                CreateNewGraph();

                data = value;

                if (data != null)
                {
                    DrawNode(data);
                }
            }
        }

        public GraphNode Selection { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ClearData();
                components?.Dispose();
            }

            base.Dispose(disposing);
        }

        private void InitializeZoomControl()
        {
            zoomControl = new ZoomControl
            {
                Mode = ZoomControlModes.Original,
                ModifierMode = ZoomViewModifierMode.None,
                ZoomDeltaMultiplier = 300,
                AnimationLength = new TimeSpan(0)
            };

            zoomControl.PreviewMouseWheel += ZoomControl_MouseWheel;

            var myResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Core.Components.GraphSharp.Forms;component/Templates/PointedTreeGraphTemplate.xaml", UriKind.Absolute)
            };
            zoomControl.Resources.MergedDictionaries.Add(myResourceDictionary);
            wpfElementHost.Child = zoomControl;

            CreateNewGraph();
        }

        private void ClearData()
        {
            foreach (DrawnGraphNode drawnGraphNode in drawnGraphNodeList)
            {
                drawnGraphNode.Vertex.PropertyChanged -= VertexOnPropertyChanged;
            }

            drawnGraphNodeList.Clear();
        }

        private void CreateNewGraph()
        {
            graph = new PointedTreeGraph();
            zoomControl.Content = new PointedTreeGraphLayout
            {
                Graph = graph
            };
        }

        private void DrawNode(GraphNode node, PointedTreeElementVertex parentVertex = null)
        {
            PointedTreeElementVertex vertex = GraphNodeConverter.Convert(node);

            vertex.PropertyChanged += VertexOnPropertyChanged;

            var drawnGraphNode = new DrawnGraphNode
            {
                GraphNode = node,
                Vertex = vertex
            };

            drawnGraphNodeList.Add(drawnGraphNode);

            graph.AddVertex(vertex);

            node.ChildNodes.ForEachElementDo(cn => DrawNode(cn, vertex));

            if (parentVertex != null)
            {
                graph.AddEdge(new PointedTreeEdge(parentVertex, vertex));
            }
        }

        private void VertexOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(PointedTreeElementVertex.IsSelected))
            {
                var changedVertex = sender as PointedTreeElementVertex;
                if (changedVertex != null && changedVertex.IsSelected)
                {
                    foreach (DrawnGraphNode drawnGraphNode in drawnGraphNodeList)
                    {
                        if (drawnGraphNode.Vertex.IsSelected && drawnGraphNode.Vertex != changedVertex)
                        {
                            drawnGraphNode.Vertex.IsSelected = false;
                        }

                        if (drawnGraphNode.Vertex == changedVertex && changedVertex.IsSelected)
                        {
                            Selection = drawnGraphNode.GraphNode;
                            OnSelectionChanged(e);
                        }
                    }
                }
            }
        }

        private void OnSelectionChanged(PropertyChangedEventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        /// <summary>
        /// Lookup class for administration related to drawn vertices.
        /// </summary>
        private class DrawnGraphNode
        {
            /// <summary>
            /// The graph node which the drawn <see cref="PointedTreeElementVertex "/> is based upon.
            /// </summary>
            public GraphNode GraphNode { get; set; }

            /// <summary>
            /// The drawn vertex.
            /// </summary>
            public PointedTreeElementVertex Vertex { get; set; }
        }

        #region Zooming

        private void ZoomControl_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            double deltaZoom = GetZoomDelta(e.Delta);
            double width = zoomControl.ActualWidth * deltaZoom;
            double height = zoomControl.ActualHeight * deltaZoom;

            Point cursorPosition = e.GetPosition(zoomControl);
            double currentRelativeLeft = cursorPosition.X / zoomControl.ActualWidth;
            double currentRelativeTop = cursorPosition.Y / zoomControl.ActualHeight;

            var topLeftCorner = new Point(
                cursorPosition.X - width * currentRelativeLeft,
                cursorPosition.Y - height * currentRelativeTop);

            var newSize = new Size(width, height);
            var zoomTo = new Rect(topLeftCorner, newSize);

            zoomControl.ZoomTo(zoomTo);
            e.Handled = true;
        }

        private double GetZoomDelta(int delta)
        {
            return Math.Max(1.0 / zoomControl.MaxZoomDelta,
                            Math.Min(zoomControl.MaxZoomDelta, delta / -zoomControl.ZoomDeltaMultiplier + 1.0));
        }

        #endregion
    }
}