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
using System.Windows;
using System.Windows.Forms;
using System.Windows.Media;
using Core.Common.Utils.Extensions;
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
        private ZoomControl zoomControl;
        private PointedTreeGraphLayout graphLayout;
        private GraphNode data;
        private PointedTreeGraph graph;

        /// <summary>
        /// Creates a new instance of <see cref="PointedTreeGraphControl"/>.
        /// </summary>
        public PointedTreeGraphControl()
        {
            InitializeComponent();
            InitializeGraph();
        }

        public GraphNode Data
        {
            get
            {
                return data;
            }
            set
            {
                graph.Clear();

                data = value;

                if (data != null)
                {
                    DrawNode(data);
                }
            }
        }

        private void InitializeGraph()
        {
            zoomControl = new ZoomControl
            {
                Mode = ZoomControlModes.Original,
                ModifierMode = ZoomViewModifierMode.None
            };

            graphLayout = new PointedTreeGraphLayout();

            graph = new PointedTreeGraph();
            graphLayout.Graph = graph;

            zoomControl.Content = graphLayout;

            var myResourceDictionary = new ResourceDictionary
            {
                Source = new Uri("pack://application:,,,/Core.Components.GraphSharp.Forms;component/Templates/PointedTreeGraphTemplate.xaml", UriKind.Absolute)
            };
            zoomControl.Resources.MergedDictionaries.Add(myResourceDictionary);
            wpfElementHost.Child = zoomControl;
        }

        private void DrawNode(GraphNode node, PointedTreeElementVertex parentVertex = null)
        {
            PointedTreeElementVertex vertex = GraphNodeConverter.Convert(node);

            graph.AddVertex(vertex);

            node.ChildNodes.ForEachElementDo(cn => DrawNode(cn, vertex));

            if (parentVertex != null)
            {
                graph.AddEdge(new PointedTreeEdge(parentVertex, vertex));
            }
        }
    }
}