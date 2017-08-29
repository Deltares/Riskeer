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
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.Views;
using Core.Components.PointedTree.Data;
using Ringtoets.Common.Data.IllustrationPoints;

namespace Ringtoets.Common.Forms.Views
{
    /// <summary>
    /// Control to show fault tree illustration points.
    /// </summary>
    public partial class IllustrationPointsFaultTreeControl : UserControl, ISelectionProvider
    {
        private readonly List<DrawnIllustrationPointNode> drawnNodes = new List<DrawnIllustrationPointNode>();
        private IllustrationPointNode data;

        public event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Creates a new instance of <see cref="IllustrationPointsFaultTreeControl"/>.
        /// </summary>
        public IllustrationPointsFaultTreeControl()
        {
            InitializeComponent();
            InitializeEventHandlers();
        }

        /// <summary>
        /// Gets or sets the data of the control.
        /// </summary>
        public IllustrationPointNode Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;

                if (value == null)
                {
                    drawnNodes.Clear();
                    pointedTreeGraphControl.Data = null;
                    return;
                }

                RegisterNode(data);

                pointedTreeGraphControl.Data = drawnNodes
                    .Where(d => d.IllustrationPointNode == data)
                    .Select(d => d.GraphNode)
                    .FirstOrDefault();
            }
        }

        public object Selection
        {
            get
            {
                GraphNode selectedGraphNode = pointedTreeGraphControl.Selection;

                return drawnNodes.Where(l => l.GraphNode == selectedGraphNode).Select(l => l.IllustrationPointNode).FirstOrDefault();
            }
        }

        private void RegisterNode(IllustrationPointNode node)
        {
            var childNodes = new List<GraphNode>();

            foreach (IllustrationPointNode childNode in node.Children)
            {
                RegisterNode(childNode);

                foreach (DrawnIllustrationPointNode drawnIllustrationPointNode in drawnNodes)
                {
                    if (drawnIllustrationPointNode.IllustrationPointNode == childNode)
                    {
                        childNodes.Add(drawnIllustrationPointNode.GraphNode);
                        break;
                    }
                }
            }

            drawnNodes.Add(new DrawnIllustrationPointNode
            {
                IllustrationPointNode = node,
                GraphNode = CreateGraphNodes(node.Data, childNodes)
            });
        }

        private static GraphNode CreateGraphNodes(IllustrationPointBase illustrationPoint, IEnumerable<GraphNode> childNodes)
        {
            var subMechanismIllustrationPoint = illustrationPoint as SubMechanismIllustrationPoint;
            if (subMechanismIllustrationPoint != null)
            {
                return GraphNodeConverter.ConvertSubMechanismIllustrationPoint(subMechanismIllustrationPoint);
            }

            var faultTreeIllustrationPoint = illustrationPoint as FaultTreeIllustrationPoint;
            if (faultTreeIllustrationPoint != null)
            {
                return GraphNodeConverter.ConvertFaultTreeIllustrationPoint(faultTreeIllustrationPoint,
                                                                            childNodes);
            }

            return null;
        }

        private void InitializeEventHandlers()
        {
            pointedTreeGraphControl.SelectionChanged += FaultTreeIllustrationPointsControlOnSelectionChanged;
        }

        private void FaultTreeIllustrationPointsControlOnSelectionChanged(object sender, EventArgs e)
        {
            OnSelectionChanged(e);
        }

        private void OnSelectionChanged(EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }

        private class DrawnIllustrationPointNode
        {
            public IllustrationPointNode IllustrationPointNode { get; set; }

            public GraphNode GraphNode { get; set; }
        }
    }
}