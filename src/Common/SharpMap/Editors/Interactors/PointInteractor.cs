﻿using System.Drawing;
using System.Windows.Forms;
using DelftTools.Utils.Editing;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using SharpMap.Api.Editors;
using SharpMap.Api.Layers;
using SharpMap.Converters.Geometries;
using SharpMap.Styles;

namespace SharpMap.Editors.Interactors
{
    public class PointInteractor : FeatureInteractor
    {
        public PointInteractor(ILayer layer, IFeature feature, VectorStyle vectorStyle, IEditableObject editableObject)
            : base(layer, feature, vectorStyle, editableObject)
        {
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

            if (VectorStyle == null) return;

            var penColor = select ? Color.Blue : Color.Lime;
            var brushColor = select ? Color.DarkBlue : Color.Green;

            trackerFeature.Bitmap = TrackerSymbolHelper.GenerateComposite(new Pen(penColor),
                new SolidBrush(brushColor),
                VectorStyle.Symbol.Width,
                VectorStyle.Symbol.Height, 6, 6);
        }
        
    }
}