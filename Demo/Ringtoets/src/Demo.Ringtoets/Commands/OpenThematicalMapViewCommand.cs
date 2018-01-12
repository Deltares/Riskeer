// Copyright (C) Stichting Deltares 2017. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
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

using System;
using System.Collections.Generic;
using System.Drawing;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Commands;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Theme;
using Core.Components.Gis.Theme.Criteria;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// The command for opening a view for <see cref="MapData"/> with 
    /// categorial theming on random data.
    /// </summary>
    public class OpenThematicalMapViewCommand : ICommand
    {
        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="OpenThematicalMapViewCommand"/>.
        /// </summary>
        /// <param name="viewCommands">The <see cref="IViewCommands"/> to be used internally.</param>
        public OpenThematicalMapViewCommand(IViewCommands viewCommands)
        {
            this.viewCommands = viewCommands;
        }

        public bool Checked { get; } = false;

        public void Execute()
        {
            var mapDataCollection = new MapDataCollection("Demo kaart categorial theming");

            var random = new Random(21);
            var mapPointData = new MapPointData("Punt Data met Gelijke criterium")
            {
                Features = CreateMapPointFeaturesWithMetaData(40),
                Style =
                {
                    Size = 10
                },
                MapTheme = new MapTheme("Waarde", new[]
                {
                    new CategoryTheme(Color.DarkOrange, new ValueCriteria(ValueCriteriaOperator.EqualValue, random.Next(0, 40))),
                    new CategoryTheme(Color.OrangeRed, new ValueCriteria(ValueCriteriaOperator.EqualValue, random.Next(0, 40))),
                    new CategoryTheme(Color.SkyBlue, new ValueCriteria(ValueCriteriaOperator.EqualValue, random.Next(0, 40))),
                    new CategoryTheme(Color.GreenYellow, new ValueCriteria(ValueCriteriaOperator.EqualValue, random.Next(0, 40)))
                })
            };
            mapDataCollection.Add(mapPointData);

            var mapPointDataUnequalCriteria = new MapPointData("Punt Data met Ongelijk criterium")
            {
                Features = CreateMapPointFeaturesWithMetaData(15),
                Style =
                {
                    Size = 10
                },
                MapTheme = new MapTheme("Waarde", new[]
                {
                    new CategoryTheme(Color.GreenYellow, new ValueCriteria(ValueCriteriaOperator.UnequalValue, 5))
                })
            };
            mapDataCollection.Add(mapPointDataUnequalCriteria);

            viewCommands.OpenView(mapDataCollection);
        }

        private static IEnumerable<MapFeature> CreateMapPointFeaturesWithMetaData(int nrOfPoints)
        {
            var features = new MapFeature[nrOfPoints];
            for (var i = 0; i < nrOfPoints; i++)
            {
                MapFeature feature = GetFeatureWithPoints(new[]
                {
                    new Point2D(i, i)
                });
                feature.MetaData["Waarde"] = i;
                features[i] = feature;
            }

            return features;
        }

        private static MapFeature GetFeatureWithPoints(Point2D[] points)
        {
            return new MapFeature(new[]
            {
                new MapGeometry(new[]
                {
                    points
                })
            });
        }
    }
}