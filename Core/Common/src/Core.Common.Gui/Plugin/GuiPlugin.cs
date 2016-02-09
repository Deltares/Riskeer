// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.Collections.Generic;
using System.Linq;

using Core.Common.Controls.TreeView;
using Core.Common.Gui.Forms;

namespace Core.Common.Gui.Plugin
{
    /// <summary>
    /// Template class for gui plugin definitions.
    /// </summary>
    public abstract class GuiPlugin : IDisposable
    {
        /// <summary>
        /// Gets or sets the gui.
        /// </summary>
        public virtual IGui Gui { get; set; }

        /// <summary>
        /// Ribbon command handler (adding tabs, groups, buttons, etc.) which can be provided by the gui plugin.
        /// </summary>
        public virtual IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Activates the gui plugin.
        /// </summary>
        public virtual void Activate() {}

        /// <summary>
        /// Deactivates the gui plugin.
        /// </summary>
        public virtual void Deactivate() {}

        /// <summary>
        /// Property info objects which can be provided by the gui plugin.
        /// </summary>
        public virtual IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            return Enumerable.Empty<PropertyInfo>();
        }

        /// <summary>
        /// View information objects which can be provided by the gui plugin.
        /// </summary>
        public virtual IEnumerable<ViewInfo> GetViewInfoObjects()
        {
            yield break;
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="TreeNodeInfo"/>.
        /// </summary>
        /// <returns>The enumeration of <see cref="TreeNodeInfo"/> provided by the <see cref="GuiPlugin"/>.</returns>
        public virtual IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield break;
        }

        public virtual IEnumerable<object> GetChildDataWithViewDefinitions(object dataObject)
        {
            yield break;
        }

        public virtual void Dispose()
        {
            Gui = null;
        }
    }
}