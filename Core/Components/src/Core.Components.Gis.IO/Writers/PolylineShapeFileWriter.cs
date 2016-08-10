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

        public override void AddFeature(MapLineData mapData)
        {
            MapFeature mapFeature = mapData.Features.First();

            if (!hasPropertyTable)
            {
                CreatePropertyTable(mapFeature);
                hasPropertyTable = true;
            }

            var lineString = CreateLineStringForMapFeature(mapFeature);

            var lineFeature = ShapeFile.AddFeature(lineString);

            CopyMetaDataFromMapFeatureToPropertyTable(mapFeature, lineFeature);
        }

        private void CreatePropertyTable(MapFeature mapFeature)
        {
            var metaData = mapFeature.MetaData;
            var sortedKeys = metaData.Keys.ToList();
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

        private static void CopyMetaDataFromMapFeatureToPropertyTable(MapFeature mapFeature, IFeature feature)
        {
            var metaData = mapFeature.MetaData;
            var sortedKeys = metaData.Keys.ToList();
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