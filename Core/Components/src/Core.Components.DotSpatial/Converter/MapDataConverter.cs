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
using System.Globalization;
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
    /// Abstract base class for transforming <see cref="MapData"/> into <see cref="IMapFeatureLayer"/>.
    /// </summary>
    public abstract class MapDataConverter<T> : IMapDataConverter where T : MapData
    {
        /// <summary>
        /// <remarks>Needed because DotSpatial can't handle special characters.
        /// Therefore we create an id as column name for the data table in the featureSet.
        /// We need this lookup to match the selected attribute from the MapData with the created id.</remarks>
        /// </summary>
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

        public IMapFeatureLayer Convert(MapData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data", @"Null data cannot be converted into a feature layer.");
            }

            if (!CanConvertMapData(data))
            {
                var message = string.Format("The data of type {0} cannot be converted by this converter.", data.GetType());
                throw new ArgumentException(message);
            }

            return Convert((T) data);
        }

        /// <summary>
        /// Creates a <see cref="IMapFeatureLayer"/> based on the <paramref name="data"/> that was given.
        /// </summary>
        /// <param name="data">The data to transform into a <see cref="IMapFeatureLayer"/>.</param>
        /// <returns>A new <see cref="IMapFeatureLayer"/>.</returns>
        protected abstract IMapFeatureLayer Convert(T data);

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> of <see cref="Point2D"/> to an
        /// <see cref="IEnumerable{T}"/> of <see cref="Coordinate"/>.
        /// </summary>
        /// <param name="points">The <see cref="IEnumerable{T}"/> of <see cref="Point2D"/> to convert.</param>
        /// <returns>The converted <see cref="IEnumerable{T}"/> of <see cref="Coordinate"/>.</returns>
        protected static IEnumerable<Coordinate> ConvertPoint2DElementsToCoordinates(IEnumerable<Point2D> points)
        {
            return points.Select(point => new Coordinate(point.X, point.Y));
        }

        /// <summary>
        /// Adds <see cref="MapFeature.MetaData"/> as attributes to the given <see cref="Feature"/> and to the <see cref="FeatureSet.DataTable"/>.
        /// </summary>
        /// <param name="ringtoetsMapFeature">The <see cref="MapFeature"/> to get the mete data from.</param>
        /// <param name="featureSet">The <see cref="FeatureSet"/> to add the attributes to.</param>
        /// <param name="feature">The <see cref="Feature"/> to add the attributes to.</param>
        protected void AddMetaDataAsAttributes(MapFeature ringtoetsMapFeature, FeatureSet featureSet, Feature feature)
        {
            var columnKey = 1;
            foreach (var attribute in ringtoetsMapFeature.MetaData)
            {
                string attributeName = attribute.Key;

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

        /// <summary>
        /// Gets a new <see cref="MapLabelLayer"/>.
        /// </summary>
        /// <param name="featureSet">The <see cref="FeatureSet"/> to add the <see cref="MapLabelLayer"/> to.</param>
        /// <param name="showLabels">Indicator whether to show the labels or not.</param>
        /// <param name="labelToShow">The key of the attribute to show the labels for.</param>
        /// <returns>A new <see cref="MapLabelLayer"/>.</returns>
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
                labelLayer.Symbology.Categories[0].Expression = string.Format(CultureInfo.CurrentCulture, "[{0}]", columnLookup[labelToShow]);
            }

            return labelLayer;
        }
    }
}