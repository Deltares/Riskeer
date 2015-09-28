using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using DelftTools.Utils.Collections;
using GeoAPI.Extensions.Feature;
using GeoAPI.Geometries;
using SharpMap.Api.Editors;
using SharpMap.Api.Layers;
using SharpMap.Converters.Geometries;
using SharpMap.Data.Providers;
using SharpMap.Editors;
using SharpMap.Layers;
using SharpMap.Rendering;
using SharpMap.Rendering.Thematics;
using SharpMap.Styles;

namespace SharpMap.UI.Tools
{
    /// <summary>
    /// ISnapper basic function for snapping to objects. Polygon or Line basesd object should make there
    /// own implementation
    /// </summary>
    public class SnapTool : MapTool
    {
        private readonly VectorLayer snapLayer = new VectorLayer(String.Empty);
        private readonly Collection<IGeometry> geometries = new Collection<IGeometry>();
        private readonly Bitmap activeTracker;

        public SnapTool()
        {
            activeTracker = TrackerSymbolHelper.GenerateSimple(new Pen(Color.DarkGreen), new SolidBrush(Color.Orange), 8, 8);
            Failed = true;
            snapLayer.Name = "snapping";
            
            var provider = new DataTableFeatureProvider(geometries);
            snapLayer.DataSource = provider;
            snapLayer.Style.Line = new Pen(Color.DarkViolet, 2);
            snapLayer.Style.Fill = new SolidBrush(Color.FromArgb(127, Color.DarkSlateBlue));
            snapLayer.Style.Symbol = activeTracker;
            provider.Attributes.Columns.Add("id", typeof(string));

            snapLayer.Theme = new CustomTheme(GetTrackerStyle);
        }

        public bool Failed { get; private set; }

        public SnapResult SnapResult { get; set; }

        public VectorLayer SnapLayer { get { return snapLayer; } }

        public override void Render(Graphics graphics, Map mapBox)
        {
            snapLayer.OnRender(graphics, mapBox);
        }

        public void AddSnap(IGeometry geometry, ICoordinate from, ICoordinate to)
        {
            var vertices = new List<ICoordinate> {from, to};
            var snapLineString = GeometryFactory.CreateLineString(vertices.ToArray());
            snapLayer.DataSource.Add(snapLineString);
        }

        public void Reset()
        {
            SnapResult = null;
            Failed = true;
            snapLayer.DataSource.Features.Clear();
        }

        public override void Cancel()
        {
            Reset();
        }

        /// <summary>
        /// Executes snapRule and shows the result in the snaptools' layer
        /// </summary>
        public SnapResult ExecuteSnapRule(ISnapRule snapRule, IFeature sourceFeature, IGeometry snapSource, IList<IFeature> snapTargets, ICoordinate worldPos, int trackingIndex)
        {
            var marge = (float)MapHelper.ImageToWorld(Map, snapRule.PixelGravity);
            var envelope = MapHelper.GetEnvelope(worldPos, marge);

            var snapCandidates = GetSnapCandidates(envelope);

            var candidates = snapCandidates as Tuple<IFeature, ILayer>[] ?? snapCandidates.ToArray();

            SnapResult = snapRule.Execute(sourceFeature, candidates, snapSource, snapTargets, worldPos, envelope, trackingIndex);
            ShowSnapResult(SnapResult);
            return SnapResult;
        }

        /// <summary>
        /// Update snapping 
        /// </summary>
        /// <param name="sourceLayer"></param>
        /// The layer of feature. 
        /// <param name="feature"></param>
        /// Feature that is snapped. Feature is not always available. 
        /// <param name="geometry"></param>
        /// actual geometry of the feature that is snapped. 
        /// <param name="worldPosition"></param>
        /// <param name="trackerIndex"></param>
        public SnapResult ExecuteLayerSnapRules(ILayer sourceLayer, IFeature feature, IGeometry geometry, ICoordinate worldPosition, int trackerIndex)
        {
            var snapRules = sourceLayer.FeatureEditor.SnapRules;

            SnapResult = null;
            for (int i = 0; i < snapRules.Count; i++)
            {
                ISnapRule rule = snapRules[i];
                ExecuteSnapRule(rule, feature, geometry, null, worldPosition, trackerIndex);
                if (null != SnapResult)
                    break;
                // If snapping failed for the last rule and snapping is obligatory 
                // any position is valid
                // todo add rule with SnapRole.Free?
                if ((!rule.Obligatory) && (i == snapRules.Count - 1))
                {
                    SnapResult = new SnapResult(worldPosition, null, sourceLayer, null, -1, -1) { Rule = rule };
                }
            }
            if (0 == snapRules.Count)
            {
                SnapResult = new SnapResult(worldPosition, null, sourceLayer, null, -1, -1);
            }
            return SnapResult;
        }

        private IEnumerable<Tuple<IFeature, ILayer>> GetSnapCandidates(IEnvelope envelope)
        {
            foreach (var layer in Map.GetAllVisibleLayers(true).Where(l => l.DataSource != null))
            {
                foreach(var feature in layer.GetFeatures(envelope))
                {
                    yield return new Tuple<IFeature, ILayer>(feature, layer);
                }
            }
        }

        private void ShowSnapResult(SnapResult snapResult)
        {
            var dataTableFeatureProvider = (DataTableFeatureProvider)snapLayer.DataSource;

            dataTableFeatureProvider.Clear();

            if (snapResult == null)
                return;

            var visibleSnaps = snapResult.VisibleSnaps;

            if (visibleSnaps.Count == 0)
            {
                var vertices = CreateVertices(snapResult);

                if (vertices.Count > 1)
                {
                    dataTableFeatureProvider.Add(GeometryFactory.CreateLineString(vertices.ToArray()));
                }

                dataTableFeatureProvider.Add(GeometryFactory.CreatePoint(snapResult.Location.X, snapResult.Location.Y));
            }
            else
            {
                visibleSnaps.ForEach(s => dataTableFeatureProvider.Add(s));
            }
        }

        private static List<ICoordinate> CreateVertices(SnapResult snapResult)
        {
            var vertices = new List<ICoordinate>();

            if (snapResult.SnapIndexPrevious != -1)
            {
                vertices.Add((ICoordinate) snapResult.NearestTarget.Coordinates[snapResult.SnapIndexPrevious].Clone());
            }

            vertices.Add((ICoordinate) snapResult.Location.Clone());

            if (snapResult.SnapIndexNext != -1)
            {
                vertices.Add((ICoordinate) snapResult.NearestTarget.Coordinates[snapResult.SnapIndexNext].Clone());
            }
            return vertices;
        }

        private VectorStyle GetTrackerStyle(IFeature feature)
        {
            var style = (VectorStyle)snapLayer.Style.Clone();
            style.Symbol = activeTracker;
            return style;
        }
    }
}
