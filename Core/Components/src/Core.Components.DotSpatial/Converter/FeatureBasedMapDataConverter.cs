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
    /// Abstract base class for transforming <see cref="FeatureBasedMapData"/> into <see cref="IMapFeatureLayer"/>.
    /// </summary>
    /// <typeparam name="TFeatureBasedMapData">The type of feature based map data to convert.</typeparam>
    /// <typeparam name="TMapFeatureLayer">The type of map feature layer to set the converted data to.</typeparam>
    public abstract class FeatureBasedMapDataConverter<TFeatureBasedMapData, TMapFeatureLayer> : IFeatureBasedMapDataConverter
        where TFeatureBasedMapData : FeatureBasedMapData
        where TMapFeatureLayer : IMapFeatureLayer
    {
        public bool CanConvertMapData(FeatureBasedMapData data)
        {
            return data is TFeatureBasedMapData;
        }

        public IMapFeatureLayer Convert(FeatureBasedMapData data)
        {
            ValidateParameters(data);

            return Convert((TFeatureBasedMapData) data);
        }

        public void ConvertLayerFeatures(FeatureBasedMapData data, IFeatureLayer layer)
        {
            ValidateParameters(data);

            ClearLayerData(layer);
            SetDataTableColumns(data, layer);

            ConvertLayerFeatures((TFeatureBasedMapData) data, (TMapFeatureLayer) layer);

            layer.DataSet.InitializeVertices();
            layer.AssignFastDrawnStates();
        }

        public void ConvertLayerProperties(FeatureBasedMapData data, IFeatureLayer layer)
        {
            ValidateParameters(data);

            ConvertLayerProperties((TFeatureBasedMapData) data, (TMapFeatureLayer) layer);
        }

        protected abstract IFeatureLayer CreateLayer();

        protected abstract IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature);

        /// <summary>
        /// Creates a <see cref="IMapFeatureLayer"/> based on the <paramref name="data"/> that was given.
        /// </summary>
        /// <param name="data">The data to transform into a <see cref="IMapFeatureLayer"/>.</param>
        /// <returns>A new <see cref="IMapFeatureLayer"/>.</returns>
        protected abstract IMapFeatureLayer Convert(TFeatureBasedMapData data);

        /// <summary>
        /// Converts all feature related data from <paramref name="data"/> to <paramref name="layer"/>.
        /// Any features already part of <paramref name="layer"/> are cleared.
        /// </summary>
        /// <param name="data">The data to convert the feature related data from.</param>
        /// <param name="layer">The layer to convert the feature related data to.</param>
        protected abstract void ConvertLayerFeatures(TFeatureBasedMapData data, TMapFeatureLayer layer);

        /// <summary>
        /// Converts all general properties (like <see cref="FeatureBasedMapData.Name"/> and <see cref="FeatureBasedMapData.IsVisible"/>) 
        /// from <paramref name="data"/> to <paramref name="layer"/>.
        /// </summary>
        /// <param name="data">The data to convert the general properties from.</param>
        /// <param name="layer">The layer to convert the general properties to.</param>
        protected abstract void ConvertLayerProperties(TFeatureBasedMapData data, TMapFeatureLayer layer);

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

        protected static void ClearLayerData(IFeatureLayer layer)
        {
            layer.DataSet.Features.Clear();
            layer.DataSet.DataTable.Reset();
        }

        protected static void SetDataTableColumns(FeatureBasedMapData data, IFeatureLayer layer)
        {
            for (var i = 1; i <= data.MetaData.Count(); i++)
            {
                layer.DataSet.DataTable.Columns.Add(i.ToString(), typeof(string));
            }
        }

        protected static Dictionary<string, int> GetColumnNameLookup(FeatureBasedMapData data)
        {
            return Enumerable.Range(0, data.MetaData.Count())
                             .ToDictionary(md => data.MetaData.ElementAt(md), mdi => mdi + 1);
        }

        /// <summary>
        /// Gets a new <see cref="MapLabelLayer"/>.
        /// </summary>
        /// <param name="metaDataLookup">The lookup to use for determining the label layer expression.</param>
        /// <param name="featureSet">The <see cref="IFeatureSet"/> to add the <see cref="MapLabelLayer"/> to.</param>
        /// <param name="showLabels">Indicator whether to show the labels or not.</param>
        /// <param name="labelToShow">The key of the attribute to show the labels for.</param>
        /// <returns>A new <see cref="MapLabelLayer"/>.</returns>
        protected MapLabelLayer GetLabelLayer(IDictionary<string, int> metaDataLookup, IFeatureSet featureSet, bool showLabels, string labelToShow)
        {
            var labelLayer = new MapLabelLayer();

            if (featureSet.DataTable.Columns.Count > 0 && showLabels)
            {
                labelLayer.Symbology.Categories[0].Symbolizer = new LabelSymbolizer
                {
                    Orientation = ContentAlignment.MiddleRight,
                    OffsetX = 5
                };
                labelLayer.Symbology.Categories[0].Expression = string.Format(CultureInfo.CurrentCulture, "[{0}]", metaDataLookup[labelToShow]);
            }

            return labelLayer;
        }

        private void ValidateParameters(FeatureBasedMapData data)
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
        }
    }
}