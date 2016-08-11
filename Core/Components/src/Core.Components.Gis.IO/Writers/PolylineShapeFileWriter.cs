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
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using DotSpatial.Data;
using DotSpatial.Topology;

namespace Core.Components.Gis.IO.Writers
{
    /// <summary>
    /// Class to be used to write polylines to a shapefile.
    /// </summary>
    public class PolylineShapeFileWriter : ShapeFileWriterBase<MapLineData>
    {
        private bool hasPropertyTable;

        /// <summary>
        /// Initializes a new instance of the <see cref="PolylineShapeFileWriter"/> class.
        /// </summary>
        public PolylineShapeFileWriter()
        {
            ShapeFile = new LineShapefile
            {
                FeatureType = FeatureType.Line
            };
        }

        /// <summary>
        /// Create a new feature from a <see cref="MapLineData"/> object and store it in the shapefile.
        /// </summary>
        /// <param name="mapData">The <see cref="MapLineData"/> which is to be stored as a feature.</param>
        /// <exception cref="ArgumentException">Thrown when a <paramref name="mapData"/> contains different metadata keys
        /// than the <paramref name="mapData"/> of the first call to this method.</exception>
        public override void AddFeature(MapLineData mapData)
        {
            MapFeature mapFeature = mapData.Features.First();

            if (!hasPropertyTable)
            {
                CreateAttributeTable(mapFeature);
                hasPropertyTable = true;
            }

            LineString lineString = CreateLineStringForMapFeature(mapFeature);

            IFeature lineFeature = ShapeFile.AddFeature(lineString);

            CopyMetaDataFromMapFeatureToAttributeTable(mapFeature, lineFeature);
        }

        private void CreateAttributeTable(MapFeature mapFeature)
        {
            IDictionary<string, object> metaData = mapFeature.MetaData;
            List<string> sortedKeys = metaData.Keys.ToList();
            sortedKeys.Sort();

            var columns = ShapeFile.DataTable.Columns;
            foreach (string key in sortedKeys)
            {
                var value = metaData[key];
                columns.Add(new DataColumn
                {
                    DataType = value.GetType(),
                    ColumnName = key
                });
            }
        }

        private static LineString CreateLineStringForMapFeature(MapFeature mapFeature)
        {
            MapGeometry geometry = mapFeature.MapGeometries.First();

            IEnumerable<Point2D> mapGeometryPointCollection = geometry.PointCollections.First();

            IList<Coordinate> coordinates = mapGeometryPointCollection.Select(p => new Coordinate(p.X, p.Y)).ToList();

            return new LineString(coordinates);
        }

        /// <summary>
        /// Copy the content of a feature's metadata to the attribute table of the shapefile.
        /// </summary>
        /// <param name="mapFeature">The <see cref="MapFeature"/> whose metadata is to be copied.</param>
        /// <param name="feature">The shapefile feature to whose attribute table row the metadata is copied.</param>
        /// <exception cref="ArgumentException">Thrown when a <paramref name="mapFeature"/> contains different metadata keys
        /// than the <paramref name="mapFeature"/> of the first call to <see cref="AddFeature"/>.</exception>
        private static void CopyMetaDataFromMapFeatureToAttributeTable(MapFeature mapFeature, IFeature feature)
        {
            IDictionary<string, object> metaData = mapFeature.MetaData;
            List<string> sortedKeys = metaData.Keys.ToList();
            sortedKeys.Sort();

            foreach (string key in sortedKeys)
            {
                var value = metaData[key];
                feature.DataRow.BeginEdit();
                feature.DataRow[key] = value;
                feature.DataRow.EndEdit();
            }
        }
    }
}