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
        where TMapFeatureLayer : FeatureLayer, IMapFeatureLayer
    {
        public bool CanConvertMapData(FeatureBasedMapData data)
        {
            return data is TFeatureBasedMapData;
        }

        public IMapFeatureLayer Convert(FeatureBasedMapData data)
        {
            ValidateParameters(data);

            var layer = CreateLayer();

            ConvertLayerFeaturesInternal(data, layer);
            ConvertLayerPropertiesInternal(data, layer);

            return layer;
        }

        public void ConvertLayerFeatures(FeatureBasedMapData data, IMapFeatureLayer layer)
        {
            ValidateParameters(data);

            ConvertLayerFeaturesInternal(data, layer);
        }

        public void ConvertLayerProperties(FeatureBasedMapData data, IMapFeatureLayer layer)
        {
            ValidateParameters(data);

            ConvertLayerPropertiesInternal(data, layer);
        }

        /// <summary>
        /// Creates a new <see cref="IMapFeatureLayer"/>.
        /// </summary>
        /// <returns>The newly created <see cref="IMapFeatureLayer"/>.</returns>
        protected abstract IMapFeatureLayer CreateLayer();

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> of <see cref="IFeature"/> based on <paramref name="mapFeature"/>.
        /// </summary>
        /// <param name="mapFeature">The <see cref="MapFeature"/> to create features for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IFeature"/>.</returns>
        protected abstract IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature);

        /// <summary>
        /// Creates a new <see cref="IFeatureSymbolizer"/>.
        /// </summary>
        /// <param name="mapData">The map data to create the symbolizer for.</param>
        /// <returns>The newly created <see cref="IFeatureSymbolizer"/>.</returns>
        /// <remarks><c>Null</c> should never be returned as this will break DotSpatial.</remarks>
        protected abstract IFeatureSymbolizer CreateSymbolizer(TFeatureBasedMapData mapData);

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

        private void ConvertLayerFeaturesInternal(FeatureBasedMapData data, IFeatureLayer layer)
        {
            ClearLayerData(layer);
            SetDataTableColumns(data, layer);

            var columnNameLookup = GetColumnNameLookup(data);

            foreach (MapFeature mapFeature in data.Features)
            {
                IEnumerable<IFeature> features = CreateFeatures(mapFeature);

                foreach (var feature in features)
                {
                    layer.DataSet.Features.Add(feature);

                    foreach (var attribute in mapFeature.MetaData)
                    {
                        feature.DataRow[columnNameLookup[attribute.Key].ToString()] = attribute.Value;
                    }
                }
            }

            layer.DataSet.InitializeVertices();
            layer.AssignFastDrawnStates();
        }

        private void ConvertLayerPropertiesInternal(FeatureBasedMapData data, IFeatureLayer layer)
        {
            layer.IsVisible = data.IsVisible;
            ((TMapFeatureLayer) layer).Name = data.Name;
            layer.ShowLabels = data.ShowLabels;
            layer.LabelLayer = GetLabelLayer(GetColumnNameLookup(data), layer.DataSet, data.ShowLabels, data.SelectedMetaDataAttribute);
            layer.Symbolizer = CreateSymbolizer((TFeatureBasedMapData) data);
        }

        private static void ClearLayerData(IFeatureLayer layer)
        {
            layer.DataSet.Features.Clear();
            layer.DataSet.DataTable.Reset();
        }

        private static void SetDataTableColumns(FeatureBasedMapData data, IFeatureLayer layer)
        {
            for (var i = 1; i <= data.MetaData.Count(); i++)
            {
                layer.DataSet.DataTable.Columns.Add(i.ToString(), typeof(string));
            }
        }

        /// <remarks>
        /// This method is used for obtaining a mapping between map data attribute names and DotSpatial
        /// attribute names. This mapping is needed because DotSpatial can't handle special characters.
        /// </remarks>
        private static Dictionary<string, int> GetColumnNameLookup(FeatureBasedMapData data)
        {
            return Enumerable.Range(0, data.MetaData.Count())
                             .ToDictionary(md => data.MetaData.ElementAt(md), mdi => mdi + 1);
        }

        private static MapLabelLayer GetLabelLayer(IDictionary<string, int> metaDataLookup, IFeatureSet featureSet, bool showLabels, string labelToShow)
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
    }
}