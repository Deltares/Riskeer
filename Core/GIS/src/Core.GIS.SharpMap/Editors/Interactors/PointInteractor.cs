using System.Drawing;
using System.Windows.Forms;
using Core.Common.Utils.Editing;
using Core.GIS.GeoAPI.Extensions.Feature;
using Core.GIS.GeoAPI.Geometries;
using Core.GIS.SharpMap.Api.Editors;
using Core.GIS.SharpMap.Api.Layers;
using Core.GIS.SharpMap.Converters.Geometries;
using Core.GIS.SharpMap.Styles;

namespace Core.GIS.SharpMap.Editors.Interactors
{
    public class PointInteractor : FeatureInteractor
    {
        public PointInteractor(ILayer layer, IFeature feature, VectorStyle vectorStyle, IEditableObject editableObject)
            : base(layer, feature, vectorStyle, editableObject) {}

        public override Cursor GetCursor(TrackerFeature trackerFeature)
        {
            return Trackers[0] == trackerFeature ? MoveCursor : null;
        }

        public override void SetTrackerSelection(TrackerFeature trackerFeature, bool select)
        {
            if (trackerFeature.Selected == select)
            {
                return;
            }

            trackerFeature.Selected = select;

            if (VectorStyle == null)
            {
                return;
            }

            var penColor = select ? Color.Blue : Color.Lime;
            var brushColor = select ? Color.DarkBlue : Color.Green;

            trackerFeature.Bitmap = TrackerSymbolHelper.GenerateComposite(new Pen(penColor),
                                                                          new SolidBrush(brushColor),
                                                                          VectorStyle.Symbol.Width,
                                                                          VectorStyle.Symbol.Height, 6, 6);
        }

        protected override void CreateTrackers()
        {
            var point = GeometryFactory.CreatePoint((ICoordinate) SourceFeature.Geometry.Coordinates[0].Clone());

            var bitmap = (VectorStyle != null)
                             ? TrackerSymbolHelper.GenerateComposite(new Pen(Color.Blue), new SolidBrush(Color.DarkBlue), VectorStyle.Symbol.Width, VectorStyle.Symbol.Height, 6, 6)
                             : null;

            Trackers.Add(new TrackerFeature(this, point, 0, bitmap));
            Trackers[0].Selected = true;
        }
    }
}