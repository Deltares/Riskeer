using System;
using System.Collections.Generic;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Editors.Interactors;
using Core.GIS.SharpMap.Layers;

namespace Core.GIS.SharpMap.Editors
{
    public class FeatureEditor : IFeatureEditor
    {
        private IList<ISnapRule> snapRules;

        private bool addingNewFeature;

        public virtual IList<ISnapRule> SnapRules
        {
            get
            {
                if (snapRules == null)
                {
                    snapRules = new List<ISnapRule>();
                }

                return snapRules;
            }

            set
            {
                snapRules = value;
            }
        }

        public virtual Func<ILayer, IFeature> CreateNewFeature { get; set; }

        public virtual IFeature AddNewFeatureByGeometry(ILayer layer, IGeometry geometry)
        {
            if (CreateNewFeature != null)
            {
                var feature = CreateNewFeature(layer);
                feature.Geometry = geometry;
                AddFeatureToDataSource(layer, feature);
                return feature;
            }

            if (addingNewFeature)
            {
                throw new InvalidOperationException("loop detected, something is wrong with your feature provider (check AddNewFeatureFromGeometryDelegate)");
            }

            addingNewFeature = true;
            IFeature newFeature;

            try
            {
                newFeature = (IFeature) Activator.CreateInstance(layer.DataSource.FeatureType);
                newFeature.Geometry = geometry;
                AddFeatureToDataSource(layer, newFeature);
            }
            finally
            {
                addingNewFeature = false;
            }

            return newFeature;
        }

        public virtual IFeatureInteractor CreateInteractor(ILayer layer, IFeature feature)
        {
            if (null == feature)
            {
                return null;
            }

            var vectorLayer = layer as VectorLayer;
            var vectorStyle = (vectorLayer != null ? vectorLayer.Style : null);

            if (feature.Geometry is ILineString)
            {
                return new LineStringInteractor(layer, feature, vectorStyle, null);
            }

            if (feature.Geometry is IPoint)
            {
                return new PointInteractor(layer, feature, vectorStyle, null);
            }

            // todo implement custom mutator for Polygon and MultiPolygon
            // LineStringMutator will work as long as moving is not supported.
            if (feature.Geometry is IPolygon)
            {
                return new LineStringInteractor(layer, feature, vectorStyle, null);
            }

            if (feature.Geometry is IMultiPolygon)
            {
                return new LineStringInteractor(layer, feature, vectorStyle, null);
            }

            return null;
            //throw new ArgumentException("Unsupported type " + feature.Geometry);
        }

        protected virtual void AddFeatureToDataSource(ILayer layer, IFeature feature)
        {
            layer.DataSource.Features.Add(feature);
        }
    }
}