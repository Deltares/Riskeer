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
    public interface ITreeViewControl
    {
        /// <summary>
        /// This method returns whether or not the tree node corresponding to the <paramref name="dataObject"/>
        /// can be renamed.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns>Whether or not the tree node can be renamed or <c>false</c> when no corresponding tree node is found.</returns>
        bool CanRenameNodeForData(object dataObject);

        /// <summary>
        /// This method tries to start a rename action for the tree node corresponding to the
        /// <paramref name="dataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <remarks>
        /// When a tree node is found that cannot be renamed, a popup is shown for notifying the end user.
        /// The renaming logic will be skipped when no corresponding tree node is found.
        /// </remarks>
        void TryRenameNodeForData(object dataObject);

        /// <summary>
        /// This method returns whether or not the tree node corresponding to the <paramref name="dataObject"/>
        /// can be removed.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns>Whether or not the tree node can be removed or <c>false</c> when no corresponding tree node is found.</returns>
        bool CanRemoveNodeForData(object dataObject);

        /// <summary>
        /// This method tries to remove the tree node corresponding to the <paramref name="dataObject"/>.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <remarks>
        /// When a tree node is found that can be removed, a popup is shown for confirmation by the end user.
        /// When a tree node is found that cannot be removed, a popup is shown for notifying the end user.
        /// The removing logic will be skipped when no corresponding tree node is found.
        /// </remarks>
        void TryRemoveNodeForData(object dataObject);

        /// <summary>
        /// This method returns whether or not the tree node corresponding to the <paramref name="dataObject"/>
        /// has children which can be removed.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns><c>true</c> if the tree node has a child node which can be removed or <c>false</c> otherwise.</returns>
        bool CanRemoveChildNodesOfData(object dataObject);

        /// <summary>
        /// This method tries to remove all child nodes of the tree node for  <paramref name="dataObject"/>
        /// </summary>
        void TryRemoveChildNodesOfData(object dataObject);

        /// <summary>
        /// This method returns whether or not the tree node corresponding to the <paramref name="dataObject"/>
        /// can be collapsed/expanded.
        /// </summary>
        /// <param name="dataObject">The data object to obtain the corresponding tree node for.</param>
        /// <returns>Whether or not the tree node can be collapsed/expanded or <c>false</c> when no corresponding tree node is found.</returns>
        bool CanExpandOrCollapseForData(object dataObject);

     
        /// <summary>
        /// This method tries to expand all nodes of the tree node corresponding to the <paramref name="dataObject"/>
        /// (child nodes are taken into account recursively).
        /// </summary>
        /// <remarks>
        /// The expanding logic will be skipped when no corresponding tree node is found.
        /// </remarks>
        void TryExpandAllNodesForData(object dataObject);

        /// <summary>
        /// This method tries to collapse all nodes of the tree node corresponding to the <paramref name="dataObject"/>
        /// (child nodes are taken into account recursively).
        /// </summary>
        /// <remarks>
        /// The collapsing logic will be skipped when no corresponding tree node is found.
        /// </remarks>
        void TryCollapseAllNodesForData(object dataObject);
    }
}