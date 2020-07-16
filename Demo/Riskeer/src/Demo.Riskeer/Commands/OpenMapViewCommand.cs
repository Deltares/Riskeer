// Copyright (C) Stichting Deltares 2019. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System.Collections.Generic;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Commands;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Demo.Riskeer.Properties;

namespace Demo.Riskeer.Commands
{
    /// <summary>
    /// The command for opening a view for <see cref="MapData"/> with some arbitrary data.
    /// </summary>
    public class OpenMapViewCommand : ICommand
    {
        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="OpenMapViewCommand"/>.
        /// </summary>
        /// <param name="viewCommands">The <see cref="IViewCommands"/> to use internally.</param>
        public OpenMapViewCommand(IViewCommands viewCommands)
        {
            this.viewCommands = viewCommands;
        }

        public bool Checked => false;

        public void Execute()
        {
            var mapDataCollection = new MapDataCollection(Resources.OpenMapViewCommand_Execute_Demo_map_netherlands);
            var waddenMapDataCollection = new MapDataCollection(Resources.OpenMapViewCommand_Execute_Wadden);
            var nestedWaddenMapDataCollection = new MapDataCollection(Resources.OpenMapViewCommand_Execute_Wadden_two);
            var emptyMapDataCollection = new MapDataCollection(Resources.OpenMapViewCommand_Execute_Empty);

            waddenMapDataCollection.Add(new MapPolygonData(Resources.OpenMapViewCommand_Execute_Texel)
            {
                Features = GetFeatureWithPoints(new[]
                {
                    new Point2D(4.764723, 52.990274),
                    new Point2D(4.713888, 53.056108),
                    new Point2D(4.883333, 53.184168)
                })
            });

            waddenMapDataCollection.Add(new MapPolygonData(Resources.OpenMapViewCommand_Execute_Vlieland)
            {
                Features = GetFeatureWithPoints(new[]
                {
                    new Point2D(4.957224, 53.23778),
                    new Point2D(4.879999, 53.214441),
                    new Point2D(5.10639, 53.303331)
                })
            });

            waddenMapDataCollection.Add(new MapPolygonData(Resources.OpenMapViewCommand_Execute_Terschelling)
            {
                Features = GetFeatureWithPoints(new[]
                {
                    new Point2D(5.213057, 53.35),
                    new Point2D(5.16889, 53.373888),
                    new Point2D(5.581945, 53.447779)
                })
            });

            nestedWaddenMapDataCollection.Add(new MapPolygonData(Resources.OpenMapViewCommand_Execute_Ameland)
            {
                Features = GetFeatureWithPoints(new[]
                {
                    new Point2D(5.699167, 53.462778),
                    new Point2D(5.956114, 53.462778),
                    new Point2D(5.633055, 53.441668)
                })
            });

            nestedWaddenMapDataCollection.Add(new MapPolygonData(Resources.OpenMapViewCommand_Execute_Schiermonnikoog)
            {
                Features = GetFeatureWithPoints(new[]
                {
                    new Point2D(6.135, 53.453608),
                    new Point2D(6.14889, 53.497499),
                    new Point2D(6.341112, 53.502779)
                })
            });

            waddenMapDataCollection.Add(emptyMapDataCollection);
            waddenMapDataCollection.Add(nestedWaddenMapDataCollection);
            mapDataCollection.Add(waddenMapDataCollection);

            mapDataCollection.Add(new MapPointData(Resources.OpenMapViewCommand_Execute_Randstad)
            {
                Features = GetFeatureWithPoints(new[]
                {
                    new Point2D(4.4818, 51.9242),
                    new Point2D(4.7167, 52.0167),
                    new Point2D(5.1146, 52.0918),
                    new Point2D(4.3007, 52.0705),
                    new Point2D(4.8952, 52.3702),
                    new Point2D(4.3667, 52.0167)
                })
            });

            mapDataCollection.Add(new MapLineData(Resources.OpenMapViewCommand_Execute_Snelwegen_randstad)
            {
                Features = GetFeatureWithPoints(new[]
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
                })
            });

            mapDataCollection.Add(new MapLineData(Resources.OpenMapViewCommand_Execute_Kustlijn_Flevoland)
            {
                Features = GetFeatureWithPoints(new[]
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
                })
            });

            mapDataCollection.Add(new MapPolygonData(Resources.OpenMapViewCommand_Execute_Continentaal_Nederland)
            {
                Features = GetFeatureWithPoints(new[]
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
                })
            });

            viewCommands.OpenView(mapDataCollection);
        }

        private static IEnumerable<MapFeature> GetFeatureWithPoints(Point2D[] points)
        {
            return new[]
            {
                new MapFeature(new[]
                {
                    new MapGeometry(new[]
                    {
                        points
                    })
                })
            };
        }
    }
}