using System.Drawing;
using Core.Gis.GeoApi.Extensions.Feature;
using Core.Gis.GeoApi.Geometries;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.CoordinateSystems.Transformations;

namespace Core.GIS.SharpMap.UI.Tools
{
    public class LocalCoordinateSystemTrackerFeature : TrackerFeature
    {
        private readonly TrackerFeature tracker;

        public LocalCoordinateSystemTrackerFeature(TrackerFeature tracker) : base(null, null, 0, null)
        {
            this.tracker = tracker;
        }

        public override IGeometry Geometry
        {
            get
            {
                if (tracker.FeatureInteractor != null &&
                    tracker.FeatureInteractor.Layer != null &&
                    tracker.FeatureInteractor.Layer.CoordinateTransformation != null)
                {
                    return GeometryTransform.TransformGeometry(tracker.Geometry, tracker.FeatureInteractor.Layer.CoordinateTransformation.MathTransform);
                }

                return tracker.Geometry;
            }
            set
            {
                if (tracker.FeatureInteractor != null &&
                    tracker.FeatureInteractor.Layer != null &&
                    tracker.FeatureInteractor.Layer.CoordinateTransformation != null)
                {
                    tracker.Geometry = GeometryTransform.TransformGeometry(value, tracker.FeatureInteractor.Layer.CoordinateTransformation.MathTransform.Inverse());
                }
                else
                {
                    tracker.Geometry = value;
                }
            }
        }

        public override Bitmap Bitmap
        {
            get
            {
                return tracker.Bitmap;
            }
            set
            {
                tracker.Bitmap = value;
            }
        }

        public override bool Selected
        {
            get
            {
                return tracker.Selected;
            }
            set
            {
                tracker.Selected = value;
            }
        }

        public override IFeatureInteractor FeatureInteractor
        {
            get
            {
                return tracker.FeatureInteractor;
            }
            set
            {
                tracker.FeatureInteractor = value;
            }
        }

        public override int Index
        {
            get
            {
                return tracker.Index;
            }
            set
            {
                tracker.Index = value;
            }
        }

        public override long Id
        {
            get
            {
                return tracker.Id;
            }
            set
            {
                tracker.Id = value;
            }
        }

        public override IFeatureAttributeCollection Attributes
        {
            get
            {
                return tracker.Attributes;
            }
            set
            {
                tracker.Attributes = value;
            }
        }

        public override object Clone()
        {
            return new LocalCoordinateSystemTrackerFeature(tracker)
            {
                Geometry = (IGeometry) Geometry.Clone()
            };
        }
    }
}