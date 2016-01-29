// Copyright (C) Stichting Deltares 2016. All rights preserved.
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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights preserved.

using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;

namespace Core.Common.Gui
{
    /// <summary>
    /// Interface which describes classes that are able to provide a <see cref="ContextMenuBuilder"/>.
    /// </summary>
    public interface IContextMenuBuilderProvider
    {
        /// <summary>
        /// Returns a new <see cref="ContextMenuBuilder"/> for creating a <see cref="ContextMenuStrip"/>
        /// for the given <paramref name="treeNode"/>.
        /// </summary>
        /// <param name="treeNode">The <see cref="TreeNode"/> to have the <see cref="ContextMenuBuilder"/>
        /// create a <see cref="ContextMenuStrip"/> for.</param>
        /// <param name="treeNodeInfo">The <see cref="TreeNodeInfo"/> to use while creating the
        /// <see cref="ContextMenuStrip"/>.</param>
        /// <returns>The <see cref="ContextMenuBuilder"/> which can be used to create a <see cref="ContextMenuStrip"/>
        /// for <paramref name="treeNode"/>.</returns>
        /// <exception cref="ContextMenuBuilderException">Thrown when the <see cref="IContextMenuBuilder"/> instance could
        /// not be created.</exception>
        IContextMenuBuilder Get(TreeNode treeNode, TreeNodeInfo treeNodeInfo);
    }
}