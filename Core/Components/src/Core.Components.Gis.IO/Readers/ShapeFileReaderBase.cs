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
using System.Data;
using System.IO;
using System.Linq;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Common.Utils.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Data;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;
using GisIOResources = Core.Components.Gis.IO.Properties.Resources;

namespace Core.Components.Gis.IO.Readers
{
    /// <summary>
    /// The base class to read data from a shapefile.
    /// </summary>
    public abstract class ShapeFileReaderBase : IDisposable
    {
        protected readonly string FilePath;
        protected Shapefile ShapeFile;

        /// <summary>
        /// Creates a new instance of <see cref="ShapeFileReaderBase"/>.
        /// </summary>
        /// <param name="filePath">The path to the shape file.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="filePath"/> 
        /// points to a file that doesn't exist. </exception>
        protected ShapeFileReaderBase(string filePath)
        {
            FileUtils.ValidateFilePath(filePath);
            if (!File.Exists(filePath))
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilsResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            FilePath = filePath;
        }

        /// <summary>
        /// Gets the number of lines in the shapefile.
        /// </summary>
        public int GetNumberOfLines()
        {
            return ShapeFile != null ? ShapeFile.Features.Count : 0;
        }

        /// <summary>
        /// Determines whether the specified attribute has been defined in the shapefile attributes.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns><c>True</c> is the attribute is defined, <c>false</c> otherwise.</returns>
        /// <remarks>This check is case-sensitive.</remarks>
        public bool HasAttribute(string attributeName)
        {
            return ShapeFile != null && ShapeFile.Attributes.Columns.Any(c => c.ColumnName == attributeName);
        }

        /// <summary>
        /// Reads a line from the shape file.
        /// </summary>
        /// <param name="name">The name of the <see cref="FeatureBasedMapData"/>. When <c>null</c> a default value will be set.</param>
        /// <returns>The <see cref="FeatureBasedMapData"/> representing the shape, or 
        /// <c>null</c> when at the end of the shapefile.</returns>
        public abstract FeatureBasedMapData ReadLine(string name = null);

        /// <summary>
        /// Reads all lines from the shape file.
        /// </summary>
        /// <param name="name">The name of the <see cref="FeatureBasedMapData"/>. When <c>null</c>, a default value will be set.</param>
        /// <returns>The <see cref="FeatureBasedMapData"/> representing the shape.</returns>
        public abstract FeatureBasedMapData ReadShapeFile(string name = null);

        /// <summary>
        /// Gets the feature at the given index.
        /// </summary>
        /// <param name="index">The index of which feature to retrieve.</param>
        /// <returns>The feature.</returns>
        public abstract IFeature GetFeature(int index);

        public void Dispose()
        {
            if (ShapeFile != null)
            {
                ShapeFile.Close();
            }
        }

        /// <summary>
        /// Adds shapefile feature attributes to a <see cref="MapFeature"/> as metadata.
        /// </summary>
        /// <param name="targetFeature">The <see cref="MapFeature"/> whose metadata will be updated.</param>
        /// <param name="sourceFeatureIndex">The index of the feature in the shapefile on which the <see cref="MapFeature"/> is based.</param>
        protected void CopyMetaDataIntoFeature(MapFeature targetFeature, int sourceFeatureIndex)
        {
            DataTable table = ShapeFile.GetAttributes(sourceFeatureIndex, 1);
            DataRow dataRow = table.Rows[0];

            for (int i = 0; i < table.Columns.Count; i++)
            {
                targetFeature.MetaData[table.Columns[i].ColumnName] = dataRow[i];
            }
        }
    }
}