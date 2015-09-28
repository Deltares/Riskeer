using System.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using SharpMap.Api.Delegates;

namespace SharpMap.Api.Editors
{
    public interface IFeatureRelationInteractor
    {
        // TODO: this logic sounds a bit confusing / complicated - review design

        /// <summary>
        /// Activates the TopologyRule based on feature. If ITopologyRule accepts feature as a source for a rule it 
        /// instantiates a clone of itself and returns the clone. The clone will be used to store the related features.
        /// The topology rule will also keep record of the activated topology rules of related features.
        /// </summary>
        /// <param name="feature"></param>
        /// source feature for this topology rule
        /// <param name="cloneFeature"></param>
        /// clone feature for this topology rule
        /// <param name="addRelatedFeature"></param>
        /// This delegate allows the caller to be notified of all temporary clones that the topology rule creates
        /// of related features. The caller is then responsible to activate related topology rule(s).
        /// <param name="level"></param>
        /// The calling level. Topology rules can update features that are also the source of another topology rule. 
        /// The toplevel rule has level 0. For example when dragging a node in a 1d model. The Node2Branch rule will have 
        /// level 0 and a subsequent Branch2CrossSection has level 1.
        /// Some topology rules will only be actived at level 0. For example the SegmentBoundary2Branch. 
        /// <param name="fallOffPolicy"></param>
        /// <returns></returns>
        IFeatureRelationInteractor Activate(IFeature feature, IFeature cloneFeature, AddRelatedFeature addRelatedFeature, int level, IFallOffPolicy fallOffPolicy);

        /// <summary>
        /// Updates all features related to feature with regard to the toplogy rule. Only an internal clone feature
        /// and clone geometry is is updated
        /// </summary>
        /// <param name="feature"></param>
        /// source feature for this topology rule
        /// <param name="newGeometry"></param>
        /// A new geometry that matches the source feature
        /// <param name="trackerIndices"></param>
        /// Indices that are source of operation
        void UpdateRelatedFeatures(IFeature feature, IGeometry newGeometry, IList<int> trackerIndices);

        /// <summary>
        /// Updates all features related to feature with regard to the toplogy rule. Not the internal clone feature
        /// but the actual feature and geometry is updated
        /// </summary>
        /// <param name="feature"></param>
        /// source feature for this topology rule
        /// <param name="newGeometry"></param>
        /// A new geometry that matches the source feature
        /// <param name="trackerIndices"></param>
        /// Indices that are source of operation
        void StoreRelatedFeatures(IFeature feature, IGeometry newGeometry, IList<int> trackerIndices);
    }
}