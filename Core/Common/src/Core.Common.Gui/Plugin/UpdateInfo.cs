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

using System;
using System.Drawing;
using Core.Common.Base.IO;
using Core.Common.Gui.Properties;
using Core.Common.Util;

namespace Core.Common.Gui.Plugin
{
    /// <summary>
    /// Information for creating an importer for updating a particular data object.
    /// </summary>
    public class UpdateInfo
    {
        /// <summary>
        /// Gets or sets the data type associated with this update info.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the method used to create a <see cref="IFileImporter"/> used to update data.
        /// Function arguments:
        /// <list type="number">
        ///     <item>The data to import.</item>
        ///     <item>The file to import data from.</item>
        ///     <item>out - The created importer.</item>
        /// </list>
        /// </summary>
        public Func<object, string, IFileImporter> CreateFileImporter { get; set; }

        /// <summary>
        /// Gets or sets the method used to determine whether or not the update routine should be enabled. Function arguments:
        /// <list type="number">
        ///     <item>The data to update.</item>
        ///     <item>out - <c>true</c> if the update should be enabled, <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<object, bool> IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the method used to verify whether changes that are induced by the importer
        /// are allowed.
        /// </summary>
        public Func<object, bool> VerifyUpdates { get; set; }

        /// <summary>
        /// Gets or sets the current path from which the current data was imported.
        /// </summary>
        public Func<object, string> CurrentPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the update information.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the update information.
        /// </summary>
        /// <remarks>Should never return null.</remarks>
        public string Category { get; set; } = Resources.UpdateInfo_Default_category;

        /// <summary>
        /// Gets or sets the image of the update information.
        /// </summary>
        /// <remarks>Should never return null.</remarks>
        public Image Image { get; set; } = Resources.brick;

        /// <summary>
        /// Gets or sets the file filter generator of the update information used to make file filters.
        /// </summary>
        public FileFilterGenerator FileFilterGenerator { get; set; }
    }

    /// <summary>
    /// Information for creating an importer for updating a particular data object.
    /// </summary>
    /// <typeparam name="TData">The data type associated with this update info.</typeparam>
    public class UpdateInfo<TData>
    {
        /// <summary>
        /// Gets the data type associated with this update info.
        /// </summary>
        public Type DataType
        {
            get
            {
                return typeof(TData);
            }
        }

        /// <summary>
        /// Gets or sets the method used to create a <see cref="IFileImporter"/> used to update data.
        /// Function arguments:
        /// <list type="number">
        ///     <item>The data to import.</item>
        ///     <item>The path to the file to import from.</item>
        ///     <item>out - The created importer.</item>
        /// </list>
        /// </summary>
        public Func<TData, string, IFileImporter> CreateFileImporter { get; set; }

        /// <summary>
        /// Gets or sets the method used to determine whether or not the update routine should be enabled. Function arguments:
        /// <list type="number">
        ///     <item>The data to update.</item>
        ///     <item>out - <c>true</c> if the update should be enabled, <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<TData, bool> IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the method used to verify whether changes that are induced by the importer
        /// are allowed.
        /// </summary>
        public Func<TData, bool> VerifyUpdates { get; set; }

        /// <summary>
        /// Gets or sets the current path from which the current data was imported.
        /// </summary>
        public Func<TData, string> CurrentPath { get; set; }

        /// <summary>
        /// Gets or sets the name of the update information.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the update information.
        /// </summary>
        /// <remarks>Should never return null.</remarks>
        public string Category { get; set; } = Resources.UpdateInfo_Default_category;

        /// <summary>
        /// Gets or sets the image of the update information.
        /// </summary>
        /// <remarks>Should never return null.</remarks>
        public Image Image { get; set; } = Resources.brick;

        /// <summary>
        /// Gets or sets the file filter generator of the update information used to make file filters.
        /// </summary>
        public FileFilterGenerator FileFilterGenerator { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="UpdateInfo{TData}"/> to <see cref="UpdateInfo"/>.
        /// </summary>
        /// <param name="updateInfo">The update information to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator UpdateInfo(UpdateInfo<TData> updateInfo)
        {
            return new UpdateInfo
            {
                DataType = updateInfo.DataType,
                CreateFileImporter = (data, path) => updateInfo.CreateFileImporter?.Invoke((TData) data, path),
                IsEnabled = data => updateInfo.IsEnabled == null || updateInfo.IsEnabled((TData) data),
                VerifyUpdates = data => updateInfo.VerifyUpdates == null || updateInfo.VerifyUpdates((TData) data),
                CurrentPath = data => updateInfo.CurrentPath?.Invoke((TData) data),
                Name = updateInfo.Name,
                Category = updateInfo.Category,
                Image = updateInfo.Image,
                FileFilterGenerator = updateInfo.FileFilterGenerator
            };
        }
    }
}