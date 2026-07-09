// Copyright (C) Stichting Deltares and State of the Netherlands 2026. All rights reserved.
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

namespace Core.Common.Controls.TreeView
{
    /// <summary>
    /// Interface for <see cref="TreeViewControl"/> operations required by context-menu factories.
    /// </summary>
    public interface ITreeViewControl
    {
        /// <summary>Returns whether the tree node for <paramref name="dataObject"/> can be renamed.</summary>
        bool CanRenameNodeForData(object dataObject);

        /// <summary>Tries to start a rename action for the tree node for <paramref name="dataObject"/>.</summary>
        void TryRenameNodeForData(object dataObject);

        /// <summary>Returns whether the tree node for <paramref name="dataObject"/> can be removed.</summary>
        bool CanRemoveNodeForData(object dataObject);

        /// <summary>Tries to remove the tree node for <paramref name="dataObject"/>.</summary>
        void TryRemoveNodeForData(object dataObject);

        /// <summary>Returns whether the tree node for <paramref name="dataObject"/> has removable children.</summary>
        bool CanRemoveChildNodesOfData(object dataObject);

        /// <summary>Tries to remove all child nodes of the tree node for <paramref name="dataObject"/>.</summary>
        void TryRemoveChildNodesOfData(object dataObject);

        /// <summary>Returns whether the tree node for <paramref name="dataObject"/> can be expanded or collapsed.</summary>
        bool CanExpandOrCollapseForData(object dataObject);

        /// <summary>Tries to expand all nodes for the tree node for <paramref name="dataObject"/>.</summary>
        void TryExpandAllNodesForData(object dataObject);

        /// <summary>Tries to collapse all nodes for the tree node for <paramref name="dataObject"/>.</summary>
        void TryCollapseAllNodesForData(object dataObject);
    }
}

