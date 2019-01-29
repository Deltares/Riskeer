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
using Core.Components.GraphSharp.Data;
using GraphSharp.Algorithms.Layout;
using GraphSharp.Algorithms.Layout.Simple.Tree;
using GraphSharp.Algorithms.OverlapRemoval;
using GraphSharp.Controls;

namespace Core.Components.GraphSharp.Forms.Layout
{
    /// <summary>
    /// Custom <see cref="GraphLayout{TVertex,TEdge,TGraph}"/> to use in the <see cref="PointedTreeGraphControl"/>.
    /// </summary>
    public class PointedTreeGraphLayout : GraphLayout<PointedTreeElementVertex, PointedTreeEdge, PointedTreeGraph>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PointedTreeGraphLayout"/>.
        /// </summary>
        public PointedTreeGraphLayout()
        {
            HighlightAlgorithmType = "Simple";
            LayoutAlgorithmType = "Tree";
            OverlapRemovalAlgorithmType = "FSA";
            OverlapRemovalConstraint = AlgorithmConstraints.Must;
            OverlapRemovalParameters = new OverlapRemovalParameters
            {
                HorizontalGap = 10,
                VerticalGap = 10
            };
            LayoutParameters = new SimpleTreeLayoutParameters
            {
                VertexGap = 10,
                Direction = LayoutDirection.TopToBottom,
                SpanningTreeGeneration = SpanningTreeGeneration.DFS
            };
            AnimationLength = new TimeSpan(0);
            IsAnimationEnabled = false;
        }
    }
}