using DelftTools.Utils.Editing;
using GeoAPI.Extensions.Feature;
using SharpMap.Api.Layers;
using SharpMap.Styles;

namespace SharpMap.Editors.Interactors
{
    public class Feature2DLineInteractor : LineStringInteractor
    {
        public Feature2DLineInteractor(ILayer layer, IFeature feature, VectorStyle vectorStyle,
                                       IEditableObject editableObject)
            : base(layer, feature, vectorStyle, editableObject)
        {
        }

        protected override bool AllowDeletionCore()
        {
            return true;
        }

        protected override bool AllowMoveCore()
        {
            return true;
        }
    }
}