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
    /// Information for creating an exporter for a particular data object.
    /// </summary>
    public class ExportInfo
    {
        /// <summary>
        /// Gets or sets the data type associated with this export info.
        /// </summary>
        public Type DataType { get; set; }

        /// <summary>
        /// Gets or sets the method used to create a <see cref="IFileExporter"/>. Function arguments:
        /// <list type="number">
        ///     <item>The data to export.</item>
        ///     <item>The output file path.</item>
        ///     <item>out - The created exporter.</item>
        /// </list>
        /// </summary>
        public Func<object, string, IFileExporter> CreateFileExporter { get; set; }

        /// <summary>
        /// Gets or sets the method used to determine whether or not the export routine should be enabled. Function arguments:
        /// <list type="number">
        ///     <item>The data to export.</item>
        ///     <item>out - <c>true</c> if the export should be enabled, <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<object, bool> IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the export information.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the export information.
        /// </summary>
        /// <remarks>Should never return null.</remarks>
        public string Category { get; set; } = Resources.ExportInfo_Default_category;

        /// <summary>
        /// Gets or sets the image of the export information.
        /// </summary>
        /// <remarks>Should never return null.</remarks>
        public Image Image { get; set; } = Resources.ExportIcon;

        /// <summary>
        /// Gets or sets the file filter generator of the export information used to make file filters.
        /// </summary>
        public FileFilterGenerator FileFilterGenerator { get; set; }

        /// <summary>
        /// Gets or sets the method used to get the path where the export should save the data. Function arguments:
        /// <list type="number">
        ///     <item>out - the path to export to.</item>
        /// </list>
        /// </summary>
        public Func<FileFilterGenerator, string> GetExportPath { get; set; }
    }

    /// <summary>
    /// Information for creating an exporter for a particular data object.
    /// </summary>
    /// <typeparam name="TData">The data type associated with this export info.</typeparam>
    public class ExportInfo<TData>
    {
        /// <summary>
        /// Gets the data type associated with this export info.
        /// </summary>
        public Type DataType => typeof(TData);

        /// <summary>
        /// Gets or sets the method used to create a <see cref="IFileExporter"/>. Function arguments:
        /// <list type="number">
        ///     <item>The data to export.</item>
        ///     <item>The output file path.</item>
        ///     <item>out - The created exporter.</item>
        /// </list>
        /// </summary>
        public Func<TData, string, IFileExporter> CreateFileExporter { get; set; }

        /// <summary>
        /// Gets or sets the method used to determine whether or not the export routine should be enabled. Function arguments:
        /// <list type="number">
        ///     <item>The data to export.</item>
        ///     <item>out - <c>true</c> if the export should be enabled, <c>false</c> otherwise.</item>
        /// </list>
        /// </summary>
        public Func<TData, bool> IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the name of the export information.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the export information.
        /// </summary>
        /// <remarks>Should never return null.</remarks>
        public string Category { get; set; } = Resources.ExportInfo_Default_category;

        /// <summary>
        /// Gets or sets the image of the export information.
        /// </summary>
        /// <remarks>Should never return null.</remarks>
        public Image Image { get; set; } = Resources.ExportIcon;

        /// <summary>
        /// Gets or sets the file filter generator of the export information used to make file filters.
        /// </summary>
        public FileFilterGenerator FileFilterGenerator { get; set; }

        /// <summary>
        /// Gets or sets the method used to get the path where the export should save the data. Function arguments:
        /// <list type="number">
        ///     <item>out - the path to export to.</item>
        /// </list>
        /// </summary>
        public Func<FileFilterGenerator, string> GetExportPath { get; set; }

        /// <summary>
        /// Performs an implicit conversion from <see cref="ExportInfo{TData}"/> to <see cref="ExportInfo"/>.
        /// </summary>
        /// <param name="exportInfo">The export information to convert.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator ExportInfo(ExportInfo<TData> exportInfo)
        {
            return new ExportInfo
            {
                DataType = exportInfo.DataType,
                CreateFileExporter = (data, filePath) => exportInfo.CreateFileExporter?.Invoke((TData) data, filePath),
                IsEnabled = data => exportInfo.IsEnabled == null || exportInfo.IsEnabled((TData) data),
                Name = exportInfo.Name,
                Category = exportInfo.Category,
                Image = exportInfo.Image,
                FileFilterGenerator = exportInfo.FileFilterGenerator,
                GetExportPath = (fileFilter) => exportInfo.GetExportPath?.Invoke(fileFilter)
            };
        }
    }
}