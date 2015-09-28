using System.Collections.Generic;
using GeoAPI.Extensions.Feature;
using SharpMap.Api.Editors;

namespace SharpMap.Api.Delegates
{
    /// <summary>
    /// Delegate used to notify the owner of the topology rules to be notified when a clone of a related feature 
    /// is created. For example this allows the owner to draw these related clones and to activate the related
    /// topology rules
    /// </summary>
    /// <param name="childTopologyRules"></param>
    /// The list of active toplogy rules for the related feature. The owner can actvate new rulkes and add them
    /// to this list.
    /// <param name="sourceFeature"></param>
    /// the related source feature
    /// <param name="cloneFeature"></param>
    /// the cloned source feature
    /// <param name="level"></param>
    /// the level of the rule in the calling hierarchy starting at 0
    public delegate void AddRelatedFeature(IList<IFeatureRelationInteractor> childTopologyRules, IFeature sourceFeature, IFeature cloneFeature, int level);
}