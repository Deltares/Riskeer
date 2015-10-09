using System.Collections.Generic;
using DelftTools.Shell.Gui;
using DeltaShell.Plugins.SharpMapGis.Gui.Forms.GridProperties;
using GeoAPI.Geometries;
using SharpMap;
using SharpMap.Api.Layers;
using SharpMap.Data;
using SharpMap.Layers;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;
using SharpMap.UI.Tools.Decorations;

namespace DeltaShell.Plugins.SharpMapGis.Gui
{
    public static class SharpMapGisPropertyInfoProvider
    {
        public static IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<Map, MapProperties>();
            yield return new PropertyInfo<LegendTool, LegendToolProperties>();
            yield return new PropertyInfo<LayoutComponentTool, LayoutComponentToolProperties>();
            yield return new PropertyInfo<ILayer, LayerProperties>();
            yield return new PropertyInfo<VectorLayer, VectorLayerPointProperties>
            {
                AdditionalDataCheck = o => o.Style.GeometryType == typeof(IPoint)
            };
            yield return new PropertyInfo<VectorLayer, VectorLayerPolygonProperties>
            {
                AdditionalDataCheck = o => o.Style.GeometryType == typeof(IPolygon)
                                           || o.Style.GeometryType == typeof(IMultiPolygon)
            };
            yield return new PropertyInfo<VectorLayer, VectorLayerLineProperties>
            {
                AdditionalDataCheck = o => o.Style.GeometryType == typeof(ILineString)
                                           || o.Style.GeometryType == typeof(IMultiLineString)
            };
            yield return new PropertyInfo<VectorLayer, VectorLayerProperties>();
            yield return new PropertyInfo<VectorStyle, PointStyleProperties>
            {
                AdditionalDataCheck = o => o.GeometryType == typeof(IPoint)
            };
            yield return new PropertyInfo<VectorStyle, PolygonStyleProperties>
            {
                AdditionalDataCheck = o => o.GeometryType == typeof(IPolygon)
                                           || o.GeometryType == typeof(IMultiPolygon)
            };
            yield return new PropertyInfo<VectorStyle, LineStyleProperties>
            {
                AdditionalDataCheck = o => o.GeometryType == typeof(ILineString)
                                           || o.GeometryType == typeof(IMultiLineString)
            };
            yield return new PropertyInfo<VectorStyle, VectorStyleProperties>();
            yield return new PropertyInfo<ThemeItem, PointStyleProperties>
            {
                AdditionalDataCheck = o =>
                {
                    var themeItemVectorStyle = o.Style as VectorStyle;

                    return themeItemVectorStyle != null
                           && themeItemVectorStyle.GeometryType == typeof(IPoint);
                },
                GetObjectPropertiesData = o => o.Style
            };
            yield return new PropertyInfo<ThemeItem, PolygonStyleProperties>
            {
                AdditionalDataCheck = o =>
                {
                    var themeItemVectorStyle = o.Style as VectorStyle;

                    return themeItemVectorStyle != null
                           && (themeItemVectorStyle.GeometryType == typeof(IPolygon)
                               || themeItemVectorStyle.GeometryType == typeof(IMultiPolygon));
                },
                GetObjectPropertiesData = o => o.Style
            };
            yield return new PropertyInfo<ThemeItem, LineStyleProperties>
            {
                AdditionalDataCheck = o =>
                {
                    var themeItemVectorStyle = o.Style as VectorStyle;

                    return themeItemVectorStyle != null
                           && (themeItemVectorStyle.GeometryType == typeof(ILineString)
                               || themeItemVectorStyle.GeometryType == typeof(IMultiLineString));
                },
                GetObjectPropertiesData = o => o.Style
            };
            yield return new PropertyInfo<ThemeItem, ThemeItemProperties>();
            yield return new PropertyInfo<GroupLayer, GroupLayerProperties>();
            yield return new PropertyInfo<FeatureDataRow, FeatureDataRowProperties>();
        }
    }
}