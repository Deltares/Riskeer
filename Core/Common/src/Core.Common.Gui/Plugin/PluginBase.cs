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
using Core.Common.Base.IO;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Forms;

namespace Core.Common.Gui.Plugin
{
    /// <summary>
    /// Template class for a plugin definitions.
    /// </summary>
    public abstract class PluginBase : IDisposable
    {
        /// <summary>
        /// Gets or sets the gui.
        /// </summary>
        public virtual IGui Gui { get; set; }

        /// <summary>
        /// Ribbon command handler (adding tabs, groups, buttons, etc.) which can be provided by the plugin.
        /// </summary>
        public virtual IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return null;
            }
        }

        /// <summary>
        /// Activates the plugin.
        /// </summary>
        public virtual void Activate() {}

        /// <summary>
        /// Deactivates the plugin.
        /// </summary>
        public virtual void Deactivate() {}

        /// <summary>
        /// This method returns an enumeration of <see cref="IFileImporter"/>.
        /// </summary>
        /// <returns>The enumeration of <see cref="IFileImporter"/> provided by the <see cref="PluginBase"/>.</returns>
        public virtual IEnumerable<IFileImporter> GetFileImporters()
        {
            yield break;
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="IFileExporter"/>.
        /// </summary>
        /// <returns>The enumeration of <see cref="IFileExporter"/> provided by the <see cref="PluginBase"/>.</returns>
        public virtual IEnumerable<IFileExporter> GetFileExporters()
        {
            yield break;
        }

        /// <summary>
        /// Returns all <see cref="PropertyInfo"/> instances provided for data of this plugin.
        /// </summary>
        public virtual IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            return Enumerable.Empty<PropertyInfo>();
        }

        /// <summary>
        /// Returns all <see cref="ViewInfo"/> instances provided for data of this plugin.
        /// </summary>
        public virtual IEnumerable<ViewInfo> GetViewInfos()
        {
            yield break;
        }

        /// <summary>
        /// Returns all <see cref="TreeNodeInfo"/> instances provided for data of this plugin.
        /// </summary>
        public virtual IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield break;
        }

        /// <summary>
        /// Gets the child data instances that have <see cref="ViewInfo"/> definitions of
        /// some parent data object.
        /// </summary>
        /// <param name="dataObject">The parent data object.</param>
        /// <returns>Sequence of child data.</returns>
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