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
using System.Collections.ObjectModel;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Utils.Reflection;

namespace Core.Common.Base.Plugin
{
    /// <summary>
    /// Class that manages <see cref="ApplicationPlugin"/> plugins and exposes their contents (file importers, file exporters and data items).
    /// </summary>
    public class ApplicationCore : IDisposable
    {
        private readonly ICollection<ApplicationPlugin> plugins;

        /// <summary>
        /// Constructs a new <see cref="ApplicationCore"/>.
        /// </summary>
        public ApplicationCore()
        {
            plugins = new Collection<ApplicationPlugin>();
        }

        /// <summary>
        /// This method adds an <see cref="ApplicationPlugin"/> to the <see cref="ApplicationCore"/>.
        /// </summary>
        /// <param name="applicationPlugin">The <see cref="ApplicationPlugin"/> to add.</param>
        public void AddPlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Add(applicationPlugin);
        }

        /// <summary>
        /// This method removes an <see cref="ApplicationPlugin"/> from the <see cref="ApplicationCore"/>.
        /// </summary>
        /// <param name="applicationPlugin">The <see cref="ApplicationPlugin"/> to remove.</param>
        public void RemovePlugin(ApplicationPlugin applicationPlugin)
        {
            plugins.Remove(applicationPlugin);
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="IFileExporter"/> that support the <paramref name="source"/>.
        /// </summary>
        /// <param name="source">The source to get the enumeration of supported <see cref="IFileExporter"/> for.</param>
        /// <returns>The enumeration of supported <see cref="IFileExporter"/>.</returns>
        public IEnumerable<IFileExporter> GetSupportedFileExporters(object source)
        {
            if (source == null)
            {
                return Enumerable.Empty<IFileExporter>();
            }

            var sourceType = source.GetType();

            return plugins.SelectMany(plugin => plugin.GetFileExporters())
                          .Where(fileExporter => (fileExporter.SupportedItemType == sourceType || sourceType.Implements(fileExporter.SupportedItemType)));
        }

        /// <summary>
        /// This method returns an enumeration of <see cref="DataItemInfo"/> that are supported for <paramref name="owner"/>.
        /// </summary>
        /// <param name="owner">The owner to get the enumeration of supported <see cref="DataItemInfo"/> for.</param>
        /// <returns>The enumeration of supported <see cref="DataItemInfo"/>.</returns>
        public IEnumerable<DataItemInfo> GetSupportedDataItemInfos(object owner)
        {
            if (owner == null)
            {
                return Enumerable.Empty<DataItemInfo>();
            }

            return plugins.SelectMany(p => p.GetDataItemInfos())
                          .Where(dataItemInfo => dataItemInfo.AdditionalOwnerCheck == null || dataItemInfo.AdditionalOwnerCheck(owner));
        }

        public virtual void Dispose()
        {
            foreach (var plugin in plugins.ToArray())
            {
                RemovePlugin(plugin);
            }
        }
    }
}