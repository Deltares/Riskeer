using System;
using System.Collections.Generic;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using SharpMap.Api.Layers;

namespace SharpMap.Api.Editors
{
    public interface IFeatureEditor
    {
        /// <summary>
        /// Snap rules defined for the layer, used during feature edits.
        /// </summary>
        IList<ISnapRule> SnapRules { get; set; }

        /// <summary>
        /// Get or set method used to creates a new feature which can be later added to the current layer.
        /// </summary>
        /// <returns></returns>
        Func<ILayer, IFeature> CreateNewFeature { get; set; }

        /// <summary>
        /// Creates a new instance of the feature interactor which can be used to manipulate <paramref name="feature"/>.
        /// 
        /// TODO: move interaction logic to SharpMap.UI, next to Select, Move and other tools. 
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="feature"></param>
        /// <returns></returns>
        IFeatureInteractor CreateInteractor(ILayer layer, IFeature feature);

        /// <summary>
        /// Adds new feature to the feature provider of the layer using geometry.
        /// </summary>
        /// <param name="layer"></param>
        /// <param name="geometry"></param>
        /// <returns></returns>
        IFeature AddNewFeatureByGeometry(ILayer layer, IGeometry geometry);
    }
}