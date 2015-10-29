using System.Collections.Generic;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api.Layers;

namespace Core.GIS.SharpMap.Api.Editors
{
    public interface IFeatureEditor
    {
        /// <summary>
        /// Snap rules defined for the layer, used during feature edits.
        /// </summary>
        IList<ISnapRule> SnapRules { get; }

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