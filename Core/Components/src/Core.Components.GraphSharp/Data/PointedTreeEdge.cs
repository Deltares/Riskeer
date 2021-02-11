﻿// Copyright (C) Stichting Deltares 2021. All rights reserved.
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

using GraphSharp.Controls;
using QuickGraph;

namespace Core.Components.GraphSharp.Data
{
    /// <summary>
    /// Class describing the edge to show in the <see cref="GraphLayout"/>.
    /// </summary>
    public class PointedTreeEdge : Edge<PointedTreeElementVertex>
    {
        /// <summary>
        /// Creates a new instance of <see cref="PointedTreeEdge"/>.
        /// </summary>
        /// <param name="source">The source of the edge.</param>
        /// <param name="target">The target of the edge.</param>
        public PointedTreeEdge(PointedTreeElementVertex source, PointedTreeElementVertex target) : base(source, target) {}
    }
}