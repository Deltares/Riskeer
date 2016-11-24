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
using System.Drawing;
using System.Linq;
using Core.Common.Base.Geometry;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// The abstract base class for transforming <see cref="MapData"/> in specific <see cref="IMapFeatureLayer"/> instances.
    /// </summary>
    public abstract class MapDataConverter<T> : IMapDataConverter where T : MapData
    {
        // Needed because DotSpatial can't handle special characters.
        // Therefor we create an id as column name for the data table in the featureSet.
        // We need this lookup to match the selected attribute from the MapData with the created id.
        private readonly Dictionary<string, string> columnLookup;

        /// <summary>
        /// Creates a new instance of <see cref="MapDataConverter{T}"/>
        /// </summary>
        protected MapDataConverter()
        {
            columnLookup = new Dictionary<string, string>();
        }

        public bool CanConvertMapData(MapData data)
        {
            return data is T;
        }

        public IList<IMapFeatureLayer> Convert(MapData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", @"Null data cannot be converted into feature sets.");
            }

            if (!CanConvertMapData(data))
            {
                var message = string.Format("The data of type {0} cannot be converted by this converter.", data.GetType());
                throw new ArgumentException(message);
            }
            return Convert((T) data);
        }

        /// <summary>
        /// Creates one or more <see cref="IMapFeatureLayer"/> based on the <paramref name="data"/> that was given.
        /// </summary>
        /// <param name="data">The data to transform into one or more <see cref="IMapFeatureLayer"/>.</param>
        /// <returns>A new <see cref="List{T}"/> of <see cref="IMapFeatureLayer"/>.</returns>
        protected abstract IList<IMapFeatureLayer> Convert(T data);

        protected static IEnumerable<Coordinate> ConvertPoint2DElementsToCoordinates(IEnumerable<Point2D> points)
        {
            return points.Select(point => new Coordinate(point.X, point.Y));
        }

        protected void AddMetaDataAsAttributes(MapFeature ringtoetsMapFeature, FeatureSet featureSet, Feature feature)
        {
            var columnKey = 1;
            foreach (var attribute in ringtoetsMapFeature.MetaData)
            {
                var attributeName = attribute.Key;

                var columnName = columnKey.ToString();
                if (!columnLookup.ContainsKey(attributeName))
                {
                    columnLookup.Add(attributeName, columnName);

                    if(!featureSet.DataTable.Columns.Contains(columnName))
                    {
                        featureSet.DataTable.Columns.Add(columnName, typeof(string));
                    }
                }

                feature.DataRow[columnName] = attribute.Value;
                columnKey++;
            }
        }

        protected MapLabelLayer GetLabelLayer(FeatureSet featureSet, bool showLabels, string labelToShow)
        {
            var labelLayer = new MapLabelLayer();

            if (featureSet.DataTable.Columns.Count > 0 && showLabels)
            {
                labelLayer.Symbology.Categories[0].Symbolizer = new LabelSymbolizer
                {
                    Orientation = ContentAlignment.MiddleRight,
                    OffsetX = 5
                };
                labelLayer.Symbology.Categories[0].Expression = string.Format("[{0}]", columnLookup[labelToShow]);
            }

            return labelLayer;
        }
    }
}