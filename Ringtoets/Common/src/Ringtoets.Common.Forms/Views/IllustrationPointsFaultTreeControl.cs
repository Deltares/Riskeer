// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Ringtoets.Common.Forms.Factories;

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
        /// <exception cref="NotSupportedException">Thrown when <paramref name="value.Data"/> or any of its children
        /// is not of type <see cref="FaultTreeIllustrationPoint"/> or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        public TopLevelFaultTreeIllustrationPoint Data
        {
            get
            {
                return data;
            }
            set
            {
                drawnNodes.Clear();

                data = value;

                pointedTreeGraphControl.Data = data != null
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

        /// <summary>
        /// Creates a new <see cref="GraphNode"/> based on the <paramref name="node"/> and registers 
        /// the <paramref name="node"/> and <see cref="GraphNode"/> combination.
        /// </summary>
        /// <param name="node">The node to base the <see cref="GraphNode"/> on.</param>
        /// <returns>The newly created <see cref="GraphNode"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when <paramref name="node.Data"/> or any of its children
        /// is not of type <see cref="FaultTreeIllustrationPoint"/> or <see cref="SubMechanismIllustrationPoint"/>.</exception>
        private GraphNode RegisterNode(IllustrationPointNode node)
        {
            GraphNode[] childNodes = node.Children.Select(RegisterNode).ToArray();

            GraphNode graphNode = RingtoetsGraphNodeFactory.CreateGraphNode(node.Data, childNodes);
            drawnNodes.Add(new DrawnIllustrationPointNode
            {
                IllustrationPointNode = node,
                GraphNode = graphNode
            });

            return graphNode;
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