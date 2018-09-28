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
using Core.Common.Base.IO;
using Core.Common.Gui.Properties;
using Core.Common.Util;

namespace Core.Common.Gui.Plugin
{
    /// <summary>
    /// Information for creating an importer for a particular data object.
    /// </summary>
    public class ImportInfo
    {
        /// <summary>
        /// Gets or sets the data type associated with this import info.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the method used to create a <see cref="IFileImporter"/>. Function arguments:
        /// <list type="number">
        ///     <item>The data to import.</item>
        ///     <item>The file to import data from.</item>
        ///     <item>out - The created importer.</item>
        /// </list>
        /// </summary>
        public Func<object, string, IFileImporter> CreateFileImporter { get; set; }

        /// <summary>
        /// Gets or sets the method used to verify whether changes that are induced by the importer
        /// are allowed.
        /// </summary>
        public Func<object, bool> VerifyUpdates { get; set; }

        /// <summary>
        /// Gets or sets the method used to determine whether or not the import routine should be enabled. Function arguments:
        /// <list type="number">
        ///     <item>The data to import.</item>
        ///     <item>out - <c>true</c> if the import should be enabled, <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<object, bool> IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the import information.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the import information.
        /// </summary>
        /// <remarks>This should never return null.</remarks>
        public string Category { get; set; } = Resources.ImportInfo_Default_category;

        /// <summary>
        /// Gets or sets the image of the import information.
        /// </summary>
        /// <remarks>This should never return null.</remarks>
        public Image Image { get; set; } = Resources.brick;

        /// <summary>
        /// Gets or sets the file filter generator of the import information used to make file filters.
        /// </summary>
        public FileFilterGenerator FileFilterGenerator { get; set; }
    }

    /// <summary>
    /// Information for creating an importer for a particular data object.
    /// </summary>
    /// <typeparam name="TData">The data type associated with this import info.</typeparam>
    public class ImportInfo<TData>
    {
        /// <summary>
        /// Gets the data type associated with this import info.
        /// </summary>
        public Type DataType
        {
            get
            {
                return typeof(TData);
            }
        }

        /// <summary>
        /// Gets or sets the method used to create a <see cref="IFileImporter"/>. Function arguments:
        /// <list type="number">
        ///     <item>The data to import.</item>
        ///     <item>The path to the file to import from.</item>
        ///     <item>out - The created importer.</item>
        /// </list>
        /// </summary>
        public Func<TData, string, IFileImporter> CreateFileImporter { get; set; }

        /// <summary>
        /// Gets or sets the method used to determine whether or not the import routine should be enabled. Function arguments:
        /// <list type="number">
        ///     <item>The data to import.</item>
        ///     <item>out - <c>true</c> if the import should be enabled, <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<TData, bool> IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the method used to verify whether changes that are induced by the importer
        /// are allowed.
        /// </summary>
        public Func<TData, bool> VerifyUpdates { get; set; }

        /// <summary>
        /// Gets or sets the name of the import information.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the import information.
        /// </summary>
        /// <remarks>This should never return null.</remarks>
        public string Category { get; set; } = Resources.ImportInfo_Default_category;

        /// <summary>
        /// Gets or sets the image of the import information.
        /// </summary>
        /// <remarks>This should never return null.</remarks>
        public Image Image { get; set; } = Resources.brick;

        /// <summary>
        /// Gets or sets the file filter generator of the import information used to make file filters.
        /// </summary>
        public FileFilterGenerator FileFilterGenerator { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ImportInfo{TData}"/> to <see cref="ImportInfo"/>.
        /// </summary>
        /// <param name="importInfo">The import information to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ImportInfo(ImportInfo<TData> importInfo)
        {
            return new ImportInfo
            {
                DataType = importInfo.DataType,
                CreateFileImporter = (data, path) => importInfo.CreateFileImporter?.Invoke((TData) data, path),
                IsEnabled = data => importInfo.IsEnabled == null || importInfo.IsEnabled((TData) data),
                VerifyUpdates = data => importInfo.VerifyUpdates == null || importInfo.VerifyUpdates((TData) data),
                Name = importInfo.Name,
                Category = importInfo.Category,
                Image = importInfo.Image,
                FileFilterGenerator = importInfo.FileFilterGenerator
            };
        }
    }
}