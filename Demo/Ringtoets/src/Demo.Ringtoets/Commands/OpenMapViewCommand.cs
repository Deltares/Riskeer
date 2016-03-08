using System.Collections.Generic;
using System.Collections.ObjectModel;

using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// The command for opening a view for <see cref="MapData"/> with some arbitrary data.
    /// </summary>
    public class OpenMapViewCommand : ICommand
    {
        private readonly IDocumentViewController documentViewController;

        /// <summary>
        /// Creates a new instance of <see cref="OpenMapViewCommand"/>.
        /// </summary>
        /// <param name="documentViewController">The <see cref="IDocumentViewController"/> to use internally.</param>
        public OpenMapViewCommand(IDocumentViewController documentViewController)
        {
            this.documentViewController = documentViewController;
        }

        public bool Enabled
        {
            get
            {
                return true;
            }
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public void Execute(params object[] arguments)
        {
            var polygons1 = new MapPolygonData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(4.764723, 52.990274),
                new Point2D(4.713888, 53.056108),
                new Point2D(4.883333, 53.184168)
            }), "Texel");

            var polygons2 = new MapPolygonData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(4.957224, 53.23778),
                new Point2D(4.879999, 53.214441),
                new Point2D(5.10639, 53.303331)
            }), "Vlieland");

            var polygons3 = new MapPolygonData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(5.213057, 53.35),
                new Point2D(5.16889, 53.373888),
                new Point2D(5.581945, 53.447779)
            }), "Terschelling");

            var polygons4 = new MapPolygonData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(5.699167, 53.462778),
                new Point2D(5.956114, 53.462778),
                new Point2D(5.633055, 53.441668)
            }), "Ameland");

            var polygons5 = new MapPolygonData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(6.135, 53.453608),
                new Point2D(6.14889, 53.497499),
                new Point2D(6.341112, 53.502779)
            }), "Schiermonnikoog");

            var pointsRandstad = new MapPointData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(4.4818, 51.9242),
                new Point2D(4.7167, 52.0167),
                new Point2D(5.1146, 52.0918),
                new Point2D(4.3007, 52.0705),
                new Point2D(4.8952, 52.3702),
                new Point2D(4.3667, 52.0167)
            }), "Randstad");

            var linesRandstad = new MapLineData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(4.4818, 51.9242),
                new Point2D(4.7167, 52.0167),
                new Point2D(5.1146, 52.0918),
                new Point2D(4.3007, 52.0705),
                new Point2D(4.7167, 52.0167),
                new Point2D(4.8952, 52.3702),
                new Point2D(4.3667, 52.0167),
                new Point2D(5.1146, 52.0918),
                new Point2D(4.8952, 52.3702)
            }), "Snelwegen randstad");

            var lines = new MapLineData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(5.763887, 52.415277),
                new Point2D(5.573057, 52.368052),
                new Point2D(5.534166, 52.283335),
                new Point2D(5.428614, 52.264162),
                new Point2D(5.135557, 52.380274),
                new Point2D(5.643614, 52.601107),
                new Point2D(5.855558, 52.544168),
                new Point2D(5.855558, 52.492495),
                new Point2D(5.763887, 52.415277)
            }), "Kustlijn Flevoland");

            var polygonNetherlands = new MapPolygonData(GetFeatureWithPoints(new Collection<Point2D>
            {
                new Point2D(6.871668, 53.416109),
                new Point2D(7.208364, 53.242807),
                new Point2D(7.051668, 52.64361),
                new Point2D(6.68889, 52.549166),
                new Point2D(7.065557, 52.385828),
                new Point2D(6.82889, 51.965555),
                new Point2D(5.9625, 51.807779),
                new Point2D(6.222223, 51.46583),
                new Point2D(5.864721, 51.046106),
                new Point2D(6.011801, 50.757273),
                new Point2D(5.640833, 50.839724),
                new Point2D(5.849173, 51.156382),
                new Point2D(5.041391, 51.486666),
                new Point2D(4.252371, 51.375147),
                new Point2D(3.440832, 51.53583),
                new Point2D(4.286112, 51.44861),
                new Point2D(3.687502, 51.709719),
                new Point2D(4.167753, 51.685572),
                new Point2D(3.865557, 51.814997),
                new Point2D(4.584433, 52.461504),
                new Point2D(5.424444, 52.248606),
                new Point2D(5.533609, 52.267221),
                new Point2D(5.624723, 52.354166),
                new Point2D(5.774168, 52.405275),
                new Point2D(5.878057, 52.509439),
                new Point2D(5.855001, 52.606913),
                new Point2D(5.599443, 52.658609),
                new Point2D(5.599169, 52.757776),
                new Point2D(5.718351, 52.838022),
                new Point2D(5.368612, 52.877779),
                new Point2D(5.420557, 52.964441),
                new Point2D(5.364168, 53.070276),
                new Point2D(5.100279, 52.948053),
                new Point2D(5.304167, 52.706942),
                new Point2D(5.033335, 52.634165),
                new Point2D(5.028334, 52.375834),
                new Point2D(4.58, 52.471666),
                new Point2D(4.734167, 52.955553),
                new Point2D(6.871668, 53.416109)
            }), "Continentaal Nederland");

            documentViewController.DocumentViewsResolver.OpenViewForData(new MapDataCollection(new List<MapData>
            {
                polygons1, polygons2, polygons3, polygons4, polygons5, lines, polygonNetherlands, linesRandstad, pointsRandstad
            }, "Demo kaart Nederland"));
        }

        private IEnumerable<MapFeature> GetFeatureWithPoints(Collection<Point2D> points)
        {
            return new Collection<MapFeature> 
            {
                new MapFeature(new Collection<MapGeometry>
                {
                    new MapGeometry(points)
                })
            };
        }
    }
}