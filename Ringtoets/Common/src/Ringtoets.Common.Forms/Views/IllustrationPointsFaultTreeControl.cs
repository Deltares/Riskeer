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
        private TopLevelFaultTreeIllustrationPoint data;

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
        public TopLevelFaultTreeIllustrationPoint Data
        {
            get
            {
                return data;
            }
            set
            {
                data = value;

                drawnNodes.Clear();

                pointedTreeGraphControl.Data = value != null
                                                   ? RegisterNode(data.FaultTreeNodeRoot)
                                                   : null;
            }
        }

        public object Selection
        {
            get
            {
                return drawnNodes.FirstOrDefault(d => d.GraphNode == pointedTreeGraphControl.Selection)?.IllustrationPointNode;
            }
        }

        private GraphNode RegisterNode(IllustrationPointNode node)
        {
            List<GraphNode> childNodes = node.Children.Select(RegisterNode).ToList();

            GraphNode graphNode = CreateGraphNode(node.Data, childNodes);
            drawnNodes.Add(new DrawnIllustrationPointNode
            {
                IllustrationPointNode = node,
                GraphNode = graphNode
            });

            return graphNode;
        }

        private static GraphNode CreateGraphNode(IllustrationPointBase illustrationPoint, IEnumerable<GraphNode> childNodes)
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