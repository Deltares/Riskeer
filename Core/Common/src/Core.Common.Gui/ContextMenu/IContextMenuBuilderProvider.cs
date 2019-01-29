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

using System.Windows.Forms;
using Core.Common.Controls.TreeView;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// Interface which describes classes that are able to provide a <see cref="ContextMenuBuilder"/>.
    /// </summary>
    public interface IContextMenuBuilderProvider
    {
        /// <summary>
        /// Returns a new <see cref="ContextMenuBuilder"/> for creating a <see cref="ContextMenuStrip"/>
        /// for the given <paramref name="value"/>.
        /// </summary>
        /// <param name="value">The data object to have the <see cref="ContextMenuBuilder"/>
        /// create a <see cref="ContextMenuStrip"/> for.</param>
        /// <param name="treeViewControl">The <see cref="TreeViewControl"/> to use while executing the
        /// <see cref="ContextMenuStrip"/> actions.</param>
        /// <returns>The <see cref="ContextMenuBuilder"/> which can be used to create a <see cref="ContextMenuStrip"/>
        /// for <paramref name="value"/>.</returns>
        /// <exception cref="ContextMenuBuilderException">Thrown when the <see cref="IContextMenuBuilder"/> instance could
        /// not be created.</exception>
        IContextMenuBuilder Get(object value, TreeViewControl treeViewControl);
    }
}