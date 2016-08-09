﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
        /// Gets or sets the name of the export information.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the export information.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the image of the export information.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or sets the file filter of the export information.
        /// </summary>
        /// <example>
        /// An example string would be:
        /// <code>"My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2"</code>
        /// </example>
        public string FileFilter { get; set; }
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
        public Type DataType
        {
            get
            {
                return typeof(TData);
            }
        }

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
        /// Gets or sets the name of the export information.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the category of the export information.
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        /// Gets or sets the image of the export information.
        /// </summary>
        public Image Image { get; set; }

        /// <summary>
        /// Gets or sets the file filter of the export information.
        /// </summary>
        /// <example>
        /// An example string would be:
        /// <code>"My file format1 (*.ext1)|*.ext1|My file format2 (*.ext2)|*.ext2"</code>
        /// </example>
        public string FileFilter { get; set; }

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
                CreateFileExporter = (data, filePath) => exportInfo.CreateFileExporter != null
                                                             ? exportInfo.CreateFileExporter((TData) data, filePath)
                                                             : null,
                Name = exportInfo.Name,
                Category = exportInfo.Category,
                Image = exportInfo.Image,
                FileFilter = exportInfo.FileFilter
            };
        }
    }
}