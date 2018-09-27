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
using System.Drawing;
using System.Windows.Forms;

namespace Core.Common.Gui.ContextMenu
{
    /// <summary>
    /// Class used by the <see cref="ContextMenuBuilder"/> to enforce instantiation of the following properties:
    /// <list type="bullet">
    /// <item><see cref="ToolStripMenuItem.Text"/></item>
    /// <item><see cref="ToolStripItem.ToolTipText"/></item>
    /// <item><see cref="ToolStripMenuItem.Image"/></item>
    /// <item><see cref="ToolStripMenuItem.Click"/></item>
    /// </list>
    /// </summary>
    public sealed class StrictContextMenuItem : ToolStripMenuItem
    {
        /// <summary>
        /// Creates a new instance of <see cref="StrictContextMenuItem"/>.
        /// </summary>
        /// <param name="text">The text of the <see cref="StrictContextMenuItem"/>.</param>
        /// <param name="toolTip">The tooltip of the <see cref="StrictContextMenuItem"/>.</param>
        /// <param name="image">The icon used for the <see cref="StrictContextMenuItem"/>.</param>
        /// <param name="clickHandler">The handler for a mouse click on the created
        /// <see cref="StrictContextMenuItem"/>.</param>
        public StrictContextMenuItem(string text, string toolTip, Image image, EventHandler clickHandler)
        {
            Text = text;
            ToolTipText = toolTip;
            Image = image;
            Click += clickHandler;
        }
    }
}