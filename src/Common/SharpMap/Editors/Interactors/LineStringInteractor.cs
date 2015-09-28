﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using DelftTools.Utils.Collections.Extensions;
using DelftTools.Utils.Editing;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using SharpMap.Api.Editors;
using SharpMap.Api.Layers;
using SharpMap.Converters.Geometries;
using SharpMap.Editors.FallOff;
using SharpMap.Styles;

namespace SharpMap.Editors.Interactors
{
    public class LineStringInteractor : FeatureInteractor
    {
        protected TrackerFeature AllTracker { get; set; }

        static private Bitmap trackerSmallStart;
        static private Bitmap trackerSmallEnd;
        static private Bitmap trackerSmall;
        static private Bitmap selectedTrackerSmall;

        public LineStringInteractor(ILayer layer, IFeature feature, VectorStyle vectorStyle, IEditableObject editableObject)
            : base(layer, feature, vectorStyle, editableObject)
        {
        }

        protected override void CreateTrackers()
        {
            if (SourceFeature == null || SourceFeature.Geometry == null)
            {
                return;
            }

            if (trackerSmallStart == null)
            {
                trackerSmallStart = TrackerSymbolHelper.GenerateSimple(new Pen(Color.Blue), new SolidBrush(Color.DarkBlue), 6, 6);
                trackerSmallEnd = TrackerSymbolHelper.GenerateSimple(new Pen(Color.Tomato), new SolidBrush(Color.Maroon), 6, 6);
                trackerSmall = TrackerSymbolHelper.GenerateSimple(new Pen(Color.Green), new SolidBrush(Color.Lime), 6, 6);
                selectedTrackerSmall = TrackerSymbolHelper.GenerateSimple(new Pen(Color.DarkMagenta), new SolidBrush(Color.Magenta), 6, 6);
            }

            Trackers.Clear();
            Trackers.AddRange(CreateTrackersForGeometry(SourceFeature.Geometry));

            AllTracker = new TrackerFeature(this, null, -1, null);
        }

        protected IEnumerable<TrackerFeature> CreateTrackersForGeometry(IGeometry geometry)
        {
            var coordinates = geometry.Coordinates;
            if (coordinates.Length == 0)
            {
                yield break;
            }
            
            yield return new TrackerFeature(this, GeometryFactory.CreatePoint(coordinates[0].X, coordinates[0].Y), 0, trackerSmallStart);

            for (var i = 1; i < coordinates.Length - 1; i++)
            {
                yield return new TrackerFeature(this, GeometryFactory.CreatePoint(coordinates[i].X, coordinates[i].Y), i, trackerSmall);
            }

            if (coordinates.Length > 1)
            {
                yield return new TrackerFeature(this, GeometryFactory.CreatePoint(coordinates.Last().X, coordinates.Last().Y), coordinates.Length - 1, trackerSmallEnd);
            }
        }

        public override TrackerFeature GetTrackerAtCoordinate(ICoordinate worldPos)
        {
            var trackerFeature = base.GetTrackerAtCoordinate(worldPos);

            if (trackerFeature == null)
            {
                var org = Layer.Map.ImageToWorld(new PointF(0, 0));
                var range = Layer.Map.ImageToWorld(new PointF(6, 6)); // todo make attribute
                var sourceGeometry = SourceFeature.Geometry;

                if (sourceGeometry.Distance(GeometryFactory.CreatePoint(worldPos)) < Math.Abs(range.X - org.X))
                {
                    return AllTracker;
                }
            }
            return trackerFeature;
        }

        public override bool MoveTracker(TrackerFeature trackerFeature, double deltaX, double deltaY, SnapResult snapResult = null)
        {
            if (trackerFeature != AllTracker)
            {
                return base.MoveTracker(trackerFeature, deltaX, deltaY, snapResult);
            }

            if (FallOffPolicy == null)
            {
                FallOffPolicy = new NoFallOffPolicy();
            }

            var handles = TrackerIndices.ToList();

            FallOffPolicy.Move(TargetFeature.Geometry, Trackers.Select(t => t.Geometry).ToList(), handles, -1, deltaX, deltaY);

            foreach (var topologyRule in FeatureRelationEditors)
            {
                topologyRule.UpdateRelatedFeatures(SourceFeature, TargetFeature.Geometry, handles);
            }

            return true;
        }

        public override Cursor GetCursor(TrackerFeature trackerFeature)
        {
            return trackerFeature == AllTracker ? Cursors.SizeAll : MoveCursor;
        }

        public override void SetTrackerSelection(TrackerFeature trackerFeature, bool select)
        {
            trackerFeature.Selected = select;
            trackerFeature.Bitmap = select ? selectedTrackerSmall : trackerSmall;
        }
    }
}