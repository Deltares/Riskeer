using DelftTools.Utils.Editing;
using GeoAPI.Extensions.Feature;
using SharpMap.Api.Editors;
using SharpMap.Api.Layers;
using SharpMap.Editors.FallOff;
using SharpMap.Styles;

namespace SharpMap.Editors.Interactors
{
    public class LinearRingInteractor : LineStringInteractor
    {
        public LinearRingInteractor(ILayer layer, IFeature feature, VectorStyle vectorStyle, IEditableObject editableObject)
            : base(layer, feature, vectorStyle, editableObject)
        {
            ringFallOffPolicy = new RingFallOffPolicy();
        }

        public override IFallOffPolicy FallOffPolicy
        {
            get { return ringFallOffPolicy; }
        }

        private readonly RingFallOffPolicy ringFallOffPolicy;
    }
}