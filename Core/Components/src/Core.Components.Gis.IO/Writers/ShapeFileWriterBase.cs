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
using System.Data;
using System.Linq;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.IO.Properties;
using DotSpatial.Data;
using CoreCommonUtilsResources = Core.Common.Utils.Properties.Resources;

namespace Core.Components.Gis.IO.Writers
{
    /// <summary>
    /// The base class to write data to a shapefile.
    /// </summary>
    public abstract class ShapeFileWriterBase : IDisposable
    {
        protected Shapefile ShapeFile;
        private bool hasPropertyTable;

        /// <summary>
        /// Creates a new feature from <paramref name="mapData"/> and adds it to the in-memory shapefile.
        /// </summary>
        /// <param name="mapData">The <see cref="FeatureBasedMapData"/> to add to the in-memory shapefile as a feature.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="mapData"/> is <c>null</c>.</exception>
        /// <exception cref="ArgumentException">Thrown when:
        /// <list type="bullet">
        /// <item>A <paramref name="mapData"/> contains different metadata keys
        /// than the <paramref name="mapData"/> of the first call to <see cref="CopyToFeature"/>.</item>
        /// <item><paramref name="mapData"/> does not contain exactly one <see cref="Feature"/>.</item>
        /// </list>
        /// </exception>
        public void CopyToFeature(FeatureBasedMapData mapData)
        {
            if (mapData == null)
            {
                throw new ArgumentNullException("mapData");
            }

            if (mapData.Features.Length != 1)
            {
                throw new ArgumentException(Resources.ShapeFileWriterBase_CopyToFeature_Mapdata_can_only_contain_one_feature);
            }

            MapFeature mapFeature = mapData.Features.First();
            EnsureAttributeTableExists(mapFeature);
            IFeature feature = AddFeature(mapFeature);
            CopyMetaDataFromMapFeatureToAttributeTable(mapFeature, feature);
        }

        /// <summary>
        /// Saves the in-memory shapefile to a file, overwriting when necessary.
        /// </summary>
        /// <param name="filePath">The path to the shapefile.</param>
        /// <exception cref="ArgumentException">Thrown when <paramref name="filePath"/> is invalid.</exception>
        /// <exception cref="CriticalFileWriteException">Thrown when the shapefile cannot be written.</exception>
        public void SaveAs(string filePath)
        {
            FileUtils.ValidateFilePath(filePath);

            try
            {
                ShapeFile.SaveAs(filePath, true);
            }
            catch (Exception e)
            {
                throw new CriticalFileWriteException(string.Format(CoreCommonUtilsResources.Error_General_output_error_0, filePath), e);
            }
        }

        public void Dispose()
        {
            if (ShapeFile != null)
            {
                ShapeFile.Close();
            }
        }

        /// <summary>
        /// Create a new feature from a <see cref="MapFeature"/>.
        /// </summary>
        /// <param name="mapFeature">The <see cref="MapFeature"/> from which to create a feature.
        /// This cannot be <c>null</c>.</param>
        /// <returns>The created feature.</returns>
        protected abstract IFeature AddFeature(MapFeature mapFeature);

        private void EnsureAttributeTableExists(MapFeature mapFeature)
        {
            if (mapFeature == null)
            {
                throw new ArgumentNullException("mapFeature");
            }

            if (hasPropertyTable)
            {
                return;
            }

            CreateAttributeTable(mapFeature);
            hasPropertyTable = true;
        }

        private static void CopyMetaDataFromMapFeatureToAttributeTable(MapFeature mapFeature, IFeature feature)
        {
            IDictionary<string, object> metaData = mapFeature.MetaData;
            List<string> sortedKeys = metaData.Keys.ToList();
            sortedKeys.Sort();

            foreach (string key in sortedKeys)
            {
                object value = metaData[key];
                feature.DataRow.BeginEdit();
                feature.DataRow[key] = value;
                feature.DataRow.EndEdit();
            }
        }

        private void CreateAttributeTable(MapFeature mapFeature)
        {
            IDictionary<string, object> metaData = mapFeature.MetaData;
            List<string> sortedKeys = metaData.Keys.ToList();
            sortedKeys.Sort();

            DataColumnCollection columns = ShapeFile.DataTable.Columns;
            foreach (string key in sortedKeys)
            {
                object value = metaData[key];
                columns.Add(new DataColumn
                {
                    DataType = value.GetType(),
                    ColumnName = key
                });
            }
        }
    }
}