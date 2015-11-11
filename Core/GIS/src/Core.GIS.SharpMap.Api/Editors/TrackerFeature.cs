using System.Drawing;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;

namespace Core.GIS.SharpMap.Api.Editors
{
    /// <summary>
    /// The ITrackerFeature represent a simple feature that is typically used as a visual helper
    /// to the user to manipulate a feature in a mapcontrol.
    /// TrackerFeatures are usually managed by FeatureMutators.
    /// A TrackerFeature does have to be visible; a FeatureMutator can use invisible trackerFeatures
    /// to support extra manipulation features.
    /// eg. LineStringMutator and AllTracker
    /// todo: add support for Trackers that are not represented by a bitmap.
    /// </summary>
    public class TrackerFeature : IFeature
    {
        private IFeatureInteractor featureInteractor;
        private IGeometry geometry;
        private Bitmap bitmap;
        private int index;

        public TrackerFeature(IFeatureInteractor featureMutator, IGeometry geometry, int index, Bitmap bitmap)
        {
            featureInteractor = featureMutator;
            this.geometry = geometry;
            this.bitmap = bitmap;
            this.index = index;
        }

        /// <summary>
        ///  A bitmap that is used to draw a tracker on the map. This member
        ///  is null for invisible Trackers.
        ///  </summary>
        public virtual Bitmap Bitmap
        {
            get
            {
                return bitmap;
            }
            set
            {
                bitmap = value;
            }
        }

        /// <summary>
        ///  Indicates whether a tracker is focused. Focused Trackers are normally represented
        ///  by a different bitmap. 
        ///  </summary>
        public virtual bool Selected { get; set; }

        /// <summary>
        ///  The FeatureInteractor that is responsible for mutating the feature the tracker belongs to.
        ///  </summary>
        public virtual IFeatureInteractor FeatureInteractor
        {
            get
            {
                return featureInteractor;
            }
            set
            {
                featureInteractor = value;
            }
        }

        /// <summary>
        ///  A index that normally matches a coordinate in the geometry of the referenced Feature. For
        ///  special Trackers this value is typically -1.
        ///  </summary>
        public virtual int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }

        #region IFeature Members

        public string Name { get; set; }

        public virtual IGeometry Geometry
        {
            get
            {
                return geometry;
            }
            set
            {
                geometry = value;
            }
        }

        public virtual long Id { get; set; }

        public virtual IFeatureAttributeCollection Attributes { get; set; }

        public virtual object Clone()
        {
            return new TrackerFeature(FeatureInteractor, Geometry, Index, Bitmap);
        }

        #endregion
    }
}