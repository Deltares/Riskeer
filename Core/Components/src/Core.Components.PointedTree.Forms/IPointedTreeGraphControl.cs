// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Components.PointedTree.Data;

namespace Core.Components.PointedTree.Forms
{
    /// <summary>
    /// Interface describing pointed tree interactions.
    /// </summary>
    public interface IPointedTreeGraphControl
    {
        /// <summary>
        /// Fired when the selection has been changed.
        /// </summary>
        event EventHandler<EventArgs> SelectionChanged;

        /// <summary>
        /// Gets or sets the root node of the control.
        /// </summary>
        GraphNode Data { get; set; }

        /// <summary>
        /// Gets the selected <see cref="GraphNode"/>; or <c>null</c> if nothing is selected.
        /// </summary>
        GraphNode Selection { get; }
    }
}