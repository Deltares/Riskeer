using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Core.Common.Utils;
using Core.Common.Utils.Collections;
using Core.Common.Utils.Reflection;
using Core.Gis.GeoApi.CoordinateSystems;
using Core.Gis.GeoApi.Extensions.Feature;
using Core.Gis.GeoApi.Geometries;
using Core.GIS.NetTopologySuite.Geometries;
using Core.GIS.SharpMap.Api;

namespace Core.GIS.SharpMap.Data.Providers
{
    //TODO: get this class generic FeatureCollection<F>:where F: is IFeature. This will speed up and prettify the code :)
    //Note: See the WFDExplorer plugin for an implementation of FeatureCollection<T>!! It is not fully covered with tests 
    //yet because not all requirements are clear

    public class FeatureCollection : IFeatureProvider
    {
        public virtual event EventHandler CoordinateSystemChanged;

        public virtual event EventHandler FeaturesChanged;
        protected IList features;

        protected IEnvelope envelope; // TODO: make it private, currently used to improve performance and still remain robust (clear envelope on change)
        private Type featureType;
        private IEnumerable<DateTime> times;
        private ICoordinateSystem coordinateSystem;

        private DateTime? timeSelectionEnd;

        public FeatureCollection()
        {
            features = new List<IFeature>();
        }

        public FeatureCollection(IList features, Type featureType)
        {
            if (!featureType.IsClass)
            {
                // We only accept a class because we want to use Activator to create object
                throw new ArgumentException("Can only instantiate FeatureCollection with class");
            }
            if (!typeof(IFeature).IsAssignableFrom((featureType)))
            {
                throw new ArgumentException("Feature type should be IFeature");
            }
            Features = features;
            FeatureType = featureType;
        }

        public virtual string SrsWkt { get; set; }

        public virtual ICoordinateSystem CoordinateSystem
        {
            get
            {
                return coordinateSystem;
            }
            set
            {
                coordinateSystem = value;

                OnCoordinateSystemChanged();
            }
        }

        public virtual IList Features
        {
            get
            {
                return features;
            }
            set
            {
                if (value == null)
                {
                    throw new InvalidOperationException("Features cannot be null in a feature collection.");
                }

                Unsubscribe();

                features = value;
                GuessFeatureType();

                var featuresCollectionChanged = features as INotifyCollectionChanged;
                if (featuresCollectionChanged != null)
                {
                    featuresCollectionChanged.CollectionChanged += FeaturesCollectionChanged;
                }

                var featuresPropertyChanged = features as INotifyPropertyChange;
                if (featuresPropertyChanged != null)
                {
                    featuresPropertyChanged.PropertyChanged += FeaturesPropertyChanged;
                }
            }
        }

        public virtual bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public virtual Type FeatureType
        {
            get
            {
                return featureType;
            }
            set
            {
                featureType = value;
                if (!value.Implements(typeof(IFeature)))
                {
                    throw new ArgumentException(string.Format("Type '{0}' is not a IFeature.", value));
                }
            }
        }

        public virtual Func<IFeatureProvider, IGeometry, IFeature> AddNewFeatureFromGeometryDelegate { get; set; }

        public virtual bool Add(IFeature feature)
        {
            if (featureType == null)
            {
                GuessFeatureType();
                if (featureType == null || featureType != feature.GetType())
                {
                    throw new NotSupportedException("FeatureType must be set in order to add a new feature");
                }
            }

            Features.Add(feature);

            return true;
        }

        public virtual int GetFeatureCount()
        {
            return Features.Count;
        }

        public virtual IFeature GetFeature(int index)
        {
            return (IFeature) Features[index];
        }

        public virtual bool Contains(IFeature feature)
        {
            if (Features == null || Features.Count == 0)
            {
                return false;
            }
            // Since Features can be strongly collection typed we must prevent searching objects of an invalid type
            if (FeatureType != null)
            {
                // test if feature we are looking for is derived from FeatureType
                if (!FeatureType.IsInstanceOfType(feature))
                {
                    return false;
                }
            }
            else
            {
                // if FeatureType is not set use type of first object in collection.
                if (Features[0].GetType() != feature.GetType())
                {
                    return false;
                }
            }
            return Features.Contains(feature);
        }

        public virtual int IndexOf(IFeature feature)
        {
            if (Features.Count == 0 || Features[0].GetType() != feature.GetType())
            {
                return -1;
            }
            return Features.IndexOf(feature);
        }

        public virtual IEnvelope GetBounds(int recordIndex)
        {
            return GetFeature(recordIndex).Geometry.EnvelopeInternal;
        }

        public virtual IEnvelope GetExtents()
        {
            // TODO: cache envelope, but make sure it is updated after changes occur
            if (Features == null || Features.Count == 0)
            {
                return null;
            }

            if (this.envelope != null)
            {
                return this.envelope;
            }

            IEnvelope envelope = new Envelope();

            foreach (IFeature feature in Features)
            {
                if (feature.Geometry == null)
                {
                    continue;
                }

                // HACK: probably we should not use EnvelopeInternal here but Envelope

                if (envelope.IsNull)
                {
                    envelope = (IEnvelope) feature.Geometry.EnvelopeInternal.Clone();
                }

                envelope.ExpandToInclude(feature.Geometry.EnvelopeInternal);
            }

            this.envelope = envelope;

            return envelope;
        }

        public virtual IGeometry GetGeometryByID(int oid)
        {
            return ((IFeature) Features[oid]).Geometry;
        }

        public virtual void Dispose()
        {
            AddNewFeatureFromGeometryDelegate = null;
            Unsubscribe();
        }

        public virtual IFeature Add(IGeometry geometry)
        {
            if (featureType == null)
            {
                GuessFeatureType();
                if (featureType == null)
                {
                    throw new NotSupportedException("FeatureType must be set in order to add a new feature geometry");
                }
            }

            IFeature newFeature;
            if (AddNewFeatureFromGeometryDelegate != null)
            {
                newFeature = AddNewFeatureFromGeometryDelegate(this, geometry);
            }
            else
            {
                newFeature = (IFeature) Activator.CreateInstance(featureType);
                newFeature.Geometry = geometry;
                Features.Add(newFeature);
            }

            return newFeature;
        }

        protected void OnCoordinateSystemChanged()
        {
            if (CoordinateSystemChanged != null)
            {
                envelope = null;
                CoordinateSystemChanged(this, EventArgs.Empty);
            }
        }

        protected void FireFeaturesChanged()
        {
            envelope = null;

            if (FeaturesChanged != null)
            {
                FeaturesChanged(this, EventArgs.Empty);
            }
        }

        private void Unsubscribe()
        {
            var featuresCollectionChanged = features as INotifyCollectionChanged;
            if (featuresCollectionChanged != null)
            {
                featuresCollectionChanged.CollectionChanged -= FeaturesCollectionChanged;
            }
            var featuresPropertyChanged = features as INotifyPropertyChange;
            if (featuresPropertyChanged != null)
            {
                featuresPropertyChanged.PropertyChanged -= FeaturesPropertyChanged;
            }
        }

        private void FeaturesPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (sender is IFeature && e.PropertyName == "Geometry")
            {
                FireFeaturesChanged();
            }
            envelope = null;
        }

        private void FeaturesCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            FireFeaturesChanged();
        }

        private void GuessFeatureType()
        {
            if (featureType != null)
            {
                return;
            }

            // try to obtain feature type from given collection of features
            Type featuresCollectionType = Features.GetType();
            if (featuresCollectionType.IsGenericType && !featuresCollectionType.IsInterface)
            {
                featureType = featuresCollectionType.GetGenericArguments()[0];
            }

            // guess feature type from the first feature
            if (featureType == null && Features.Count > 0)
            {
                featureType = Features[0].GetType();
            }
        }
    }
}