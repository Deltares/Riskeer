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
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using Core.Common.Base.IO;
using Core.Common.Util;
using Core.Common.Util.Builders;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Data;
using CoreCommonUtilResources = Core.Common.Util.Properties.Resources;

namespace Core.Components.Gis.IO.Readers
{
    /// <summary>
    /// The base class to read data from a shapefile.
    /// </summary>
    public abstract class ShapeFileReaderBase : IDisposable
    {
        protected Shapefile ShapeFile;

        /// <summary>
        /// Creates a new instance of <see cref="ShapeFileReaderBase"/>.
        /// </summary>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileReadException">Thrown when <paramref name="filePath"/> 
        /// points to a file that doesn't exist. </exception>
        protected ShapeFileReaderBase(string filePath)
        {
            IOUtils.ValidateFilePath(filePath);
            if (!File.Exists(filePath))
            {
                string message = new FileReaderErrorMessageBuilder(filePath)
                    .Build(CoreCommonUtilResources.Error_File_does_not_exist);
                throw new CriticalFileReadException(message);
            }

            FilePath = filePath;
        }

        /// <summary>
        /// Gets the number of features in the shapefile.
        /// </summary>
        public int GetNumberOfFeatures()
        {
            return ShapeFile?.Features.Count ?? 0;
        }

        /// <summary>
        /// Determines whether the specified attribute has been defined in the shapefile attributes.
        /// </summary>
        /// <param name="attributeName">Name of the attribute.</param>
        /// <returns><c>true</c> is the attribute is defined, <c>false</c> otherwise.</returns>
        /// <remarks>This check is case-sensitive.</remarks>
        public bool HasAttribute(string attributeName)
        {
            return ShapeFile != null && ShapeFile.Attributes.Columns.Any(c => c.ColumnName == attributeName);
        }

        /// <summary>
        /// Reads a feature from the shapefile.
        /// </summary>
        /// <param name="name">The name of the <see cref="FeatureBasedMapData"/>. When <c>null</c> a default value will be set.</param>
        /// <returns>The <see cref="FeatureBasedMapData"/> representing the shape, or 
        /// <c>null</c> when at the end of the shapefile.</returns>
        public abstract FeatureBasedMapData ReadFeature(string name = null);

        /// <summary>
        /// Reads all features from the shapefile.
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
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Gets the path to the shapefile.
        /// </summary>
        protected string FilePath { get; }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                ShapeFile?.Close();
            }
        }

        /// <summary>
        /// Adds shapefile feature attributes to a <see cref="MapFeature"/> as metadata.
        /// </summary>
        /// <param name="targetFeature">The <see cref="MapFeature"/> whose metadata will be updated.</param>
        /// <param name="sourceFeatureIndex">The index of the feature in the shapefile on which the <see cref="MapFeature"/> is based.</param>
        /// <remarks> Attributes with <see cref="DBNull"/> value are translated to a <c>null</c> value in the
        /// <see cref="MapFeature.MetaData"/>.</remarks>
        protected void CopyMetaDataIntoFeature(MapFeature targetFeature, int sourceFeatureIndex)
        {
            DataRow dataRow = ShapeFile.GetAttributes(sourceFeatureIndex, 1).Rows[0];
            List<Field> columns = ShapeFile.DataTable.Columns.OfType<Field>().ToList();

            for (var i = 0; i < columns.Count; i++)
            {
                object dataRowValue = dataRow[i];

                object newValue = null;
                Field column = columns[i];

                if (!(dataRowValue is DBNull))
                {
                    if (column.TypeCharacter != 'C' && dataRowValue is string)
                    {
                        string nullValue = string.Join(string.Empty, Enumerable.Repeat('*', column.Length));
                        if (!Equals(dataRowValue, nullValue))
                        {
                            newValue = dataRowValue;
                        }
                    }
                    else
                    {
                        newValue = dataRowValue;
                    }
                }

                targetFeature.MetaData[column.ColumnName] = newValue;
            }
        }
    }
}