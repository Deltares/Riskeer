// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Core.Components.Gis.Theme;
using DotSpatial.Controls;
using DotSpatial.Data;
using DotSpatial.Projections;
using DotSpatial.Symbology;
using DotSpatial.Topology;

namespace Core.Components.DotSpatial.Converter
{
    /// <summary>
    /// Abstract base class for transforming <see cref="FeatureBasedMapData"/> data into <see cref="IMapFeatureLayer"/> data.
    /// </summary>
    /// <typeparam name="TFeatureBasedMapData">The type of feature based map data to convert.</typeparam>
    /// <typeparam name="TMapFeatureLayer">The type of map feature layer to set the converted data to.</typeparam>
    public abstract class FeatureBasedMapDataConverter<TFeatureBasedMapData, TMapFeatureLayer>
        where TFeatureBasedMapData : FeatureBasedMapData
        where TMapFeatureLayer : FeatureLayer, IMapFeatureLayer
    {
        /// <summary>
        /// Converts all feature related data from <paramref name="data"/> to <paramref name="layer"/>.
        /// </summary>
        /// <param name="data">The data to convert the feature related data from.</param>
        /// <param name="layer">The layer to convert the feature related data to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> or <paramref name="layer"/> is <c>null</c>.</exception>
        public void ConvertLayerFeatures(TFeatureBasedMapData data, TMapFeatureLayer layer)
        {
            ValidateParameters(data, layer);

            ClearLayerData(data, layer);
            SetDataTableColumns(data.MetaData, layer);

            ProjectionInfo originalLayerProjection = layer.Projection;
            if (originalLayerProjection == null || !originalLayerProjection.Equals(MapDataConstants.FeatureBasedMapDataCoordinateSystem))
            {
                layer.Projection = MapDataConstants.FeatureBasedMapDataCoordinateSystem;
            }

            Dictionary<string, int> attributeMapping = GetAttributeMapping(data);
            foreach (MapFeature mapFeature in data.Features)
            {
                IEnumerable<IFeature> features = CreateFeatures(mapFeature);

                foreach (IFeature feature in features)
                {
                    AddFeatureToLayer(layer, feature, mapFeature, attributeMapping);
                }
            }

            layer.DataSet.InitializeVertices();
            layer.DataSet.UpdateExtent();

            if (originalLayerProjection != null && !originalLayerProjection.Equals(MapDataConstants.FeatureBasedMapDataCoordinateSystem))
            {
                layer.Reproject(originalLayerProjection);
            }

            layer.AssignFastDrawnStates();
        }

        /// <summary>
        /// Converts all general properties (like <see cref="FeatureBasedMapData.Name"/> and <see cref="FeatureBasedMapData.IsVisible"/>) 
        /// from <paramref name="data"/> to <paramref name="layer"/>.
        /// </summary>
        /// <param name="data">The data to convert the general properties from.</param>
        /// <param name="layer">The layer to convert the general properties to.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> or <paramref name="layer"/> is <c>null</c>.</exception>
        public void ConvertLayerProperties(TFeatureBasedMapData data, TMapFeatureLayer layer)
        {
            ValidateParameters(data, layer);

            layer.IsVisible = data.IsVisible;
            layer.Name = data.Name;
            layer.ShowLabels = data.ShowLabels;
            ((IMapFeatureLayer) layer).LabelLayer = GetLabelLayer(GetAttributeMapping(data), layer.DataSet, data.SelectedMetaDataAttribute);

            if (data.MapTheme == null)
            {
                layer.Symbolizer = CreateSymbolizer(data);
            }
            else
            {
                layer.Symbology = CreateCategorySchemes(data);
            }
        }

        /// <summary>
        /// Creates an <see cref="IEnumerable{T}"/> of <see cref="IFeature"/> based on
        /// <paramref name="mapFeature"/> that have been defined in the coordinate system
        /// given by <see cref="MapDataConstants.FeatureBasedMapDataCoordinateSystem"/>.
        /// </summary>
        /// <param name="mapFeature">The <see cref="MapFeature"/> to create features for.</param>
        /// <returns>An <see cref="IEnumerable{T}"/> of <see cref="IFeature"/>.</returns>
        protected abstract IEnumerable<IFeature> CreateFeatures(MapFeature mapFeature);

        /// <summary>
        /// Creates a new <see cref="IFeatureSymbolizer"/>.
        /// </summary>
        /// <param name="mapData">The map data to create the symbolizer for.</param>
        /// <returns>The newly created <see cref="IFeatureSymbolizer"/>.</returns>
        /// <remarks><c>null</c> should never be returned as this will break DotSpatial.</remarks>
        protected abstract IFeatureSymbolizer CreateSymbolizer(TFeatureBasedMapData mapData);

        /// <summary>
        /// Creates a new <see cref="IFeatureScheme"/> to be applied on the data.
        /// </summary>
        /// <returns>The newly created <see cref="IFeatureScheme"/>.</returns>
        protected abstract IFeatureScheme CreateScheme();

        /// <summary>
        /// Creates a new <see cref="IFeatureCategory"/> based on <paramref name="mapData"/>.
        /// </summary>
        /// <param name="mapData">The map data to base the category on.</param>
        /// <returns>The newly created <see cref="IFeatureCategory"/>.</returns>
        /// <remarks><c>null</c> should never be returned as this will break DotSpatial.</remarks>
        protected abstract IFeatureCategory CreateDefaultCategory(TFeatureBasedMapData mapData);

        /// <summary>
        /// Creates a new <see cref="IFeatureCategory"/> with a different color than specified in the <paramref name="mapData"/>.
        /// </summary>
        /// <param name="mapData">The map data to base the category on.</param>
        /// <param name="color">The desired color of the category.</param>
        /// <returns>The newly created <see cref="IFeatureCategory"/>.</returns>
        /// <remarks><c>null</c> should never be returned as this will break DotSpatial.</remarks>
        protected abstract IFeatureCategory CreateCategory(TFeatureBasedMapData mapData, Color color);

        /// <summary>
        /// Converts an <see cref="IEnumerable{T}"/> of <see cref="Point2D"/> to an <see cref="IEnumerable{T}"/>
        /// of <see cref="Coordinate"/>.
        /// </summary>
        /// <param name="points">The <see cref="IEnumerable{T}"/> of <see cref="Point2D"/> to convert.</param>
        /// <returns>The converted <see cref="IEnumerable{T}"/> of <see cref="Coordinate"/>.</returns>
        protected static IEnumerable<Coordinate> ConvertPoint2DElementsToCoordinates(IEnumerable<Point2D> points)
        {
            return points.Select(point => new Coordinate(point.X, point.Y)).ToArray();
        }

        /// <summary>
        /// Creates the <see cref="IFeatureScheme"/> based on the <paramref name="mapData"/>.
        /// </summary>
        /// <param name="mapData">The map data to base the scheme on.</param>
        /// <returns>The newly created <see cref="IFeatureScheme"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="mapData"/>
        /// could not be successfully converted to a scheme.</exception>
        private IFeatureScheme CreateCategorySchemes(TFeatureBasedMapData mapData)
        {
            IFeatureScheme scheme = CreateScheme();
            ClearFeatureScheme(mapData, scheme);

            MapTheme mapTheme = mapData.MapTheme;
            Dictionary<string, int> attributeMapping = GetAttributeMapping(mapData);

            if (attributeMapping.ContainsKey(mapTheme.AttributeName))
            {
                int attributeIndex = attributeMapping[mapTheme.AttributeName];

                foreach (CategoryTheme categoryTheme in mapTheme.CategoryThemes)
                {
                    IFeatureCategory category = CreateCategory(mapData, categoryTheme.Color);
                    category.FilterExpression = CreateFilterExpression(attributeIndex, categoryTheme.Criterion);
                    scheme.AddCategory(category);
                }
            }

            return scheme;
        }

        private void ClearFeatureScheme(TFeatureBasedMapData mapData, IScheme scheme)
        {
            if (mapData.MapTheme != null)
            {
                scheme.ClearCategories();
                scheme.AddCategory(CreateDefaultCategory(mapData));
            }
        }

        private static void ValidateParameters(TFeatureBasedMapData data, TMapFeatureLayer layer)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data), @"Null data cannot be converted into feature layer data.");
            }

            if (layer == null)
            {
                throw new ArgumentNullException(nameof(layer), @"Null data cannot be used as conversion target.");
            }
        }

        private void ClearLayerData(TFeatureBasedMapData mapData, IFeatureLayer layer)
        {
            layer.DataSet.Features.Clear();
            layer.DataSet.DataTable.Reset();
            ClearFeatureScheme(mapData, layer.Symbology);
        }

        private static void SetDataTableColumns(IEnumerable<string> metaData, IFeatureLayer layer)
        {
            int count = metaData.Count();

            for (var i = 1; i <= count; i++)
            {
                layer.DataSet.DataTable.Columns.Add(i.ToString(), typeof(string));
            }
        }

        private static void AddFeatureToLayer(TMapFeatureLayer layer, IFeature feature, MapFeature mapFeature, Dictionary<string, int> attributeMapping)
        {
            layer.DataSet.Features.Add(feature);

            AddMetaDataToFeature(feature, mapFeature, attributeMapping);
        }

        private static void AddMetaDataToFeature(IFeature feature, MapFeature mapFeature, Dictionary<string, int> attributeMapping)
        {
            foreach (KeyValuePair<string, object> attribute in mapFeature.MetaData)
            {
                feature.DataRow[attributeMapping[attribute.Key].ToString()] = attribute.Value;
            }
        }

        /// <remarks>
        /// This method is used for obtaining a mapping between map data attribute names and DotSpatial
        /// attribute names. This mapping is needed because DotSpatial can't handle special characters.
        /// </remarks>
        private static Dictionary<string, int> GetAttributeMapping(TFeatureBasedMapData data)
        {
            return Enumerable.Range(0, data.MetaData.Count())
                             .ToDictionary(md => data.MetaData.ElementAt(md), mdi => mdi + 1);
        }

        private static MapLabelLayer GetLabelLayer(IDictionary<string, int> attributeMapping, IFeatureSet featureSet, string labelToShow)
        {
            var labelLayer = new MapLabelLayer();

            if (!string.IsNullOrEmpty(labelToShow)
                && attributeMapping.ContainsKey(labelToShow)
                && featureSet.DataTable.Columns.Contains(attributeMapping[labelToShow].ToString()))
            {
                labelLayer.Symbology.Categories[0].Symbolizer = new LabelSymbolizer
                {
                    Orientation = ContentAlignment.MiddleRight,
                    OffsetX = 5
                };
                labelLayer.Symbology.Categories[0].Expression = string.Format(CultureInfo.CurrentCulture, "[{0}]", attributeMapping[labelToShow]);
            }

            return labelLayer;
        }

        /// <summary>
        /// Creates a filter expression based for an attribute and the criteria to apply.
        /// </summary>
        /// <param name="attributeIndex">The index of the attribute in the metadata table.</param>
        /// <param name="criterion">The criterion to convert to an expression.</param>
        /// <returns>The filter expression based on the <paramref name="attributeIndex"/>
        /// and <paramref name="criterion"/>.</returns>
        /// <exception cref="NotSupportedException">Thrown when the <paramref name="criterion"/>
        /// cannot be used to create a filter expression.</exception>
        private static string CreateFilterExpression(int attributeIndex, ValueCriterion criterion)
        {
            ValueCriterionOperator valueOperator = criterion.ValueOperator;
            switch (valueOperator)
            {
                case ValueCriterionOperator.EqualValue:
                    return $"[{attributeIndex}] = '{criterion.Value}'";
                case ValueCriterionOperator.UnequalValue:
                    return $"NOT [{attributeIndex}] = '{criterion.Value}'";
                default:
                    throw new NotSupportedException();
            }
        }
    }
}