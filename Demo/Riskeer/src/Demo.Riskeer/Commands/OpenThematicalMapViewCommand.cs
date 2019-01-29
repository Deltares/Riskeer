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
using System.Drawing;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Commands;
using Core.Common.Util;
using Core.Common.Util.Attributes;
using Core.Components.Gis.Data;
using Core.Components.Gis.Features;
using Core.Components.Gis.Geometries;
using Core.Components.Gis.Style;
using Core.Components.Gis.Theme;
using Demo.Riskeer.Properties;

namespace Demo.Riskeer.Commands
{
    /// <summary>
    /// The command for opening a view for <see cref="MapData"/> with 
    /// categorial theming on random data.
    /// </summary>
    public class OpenThematicalMapViewCommand : ICommand
    {
        private enum ThematicalFeatureTypes
        {
            [ResourcesDisplayName(typeof(Resources), nameof(Resources.ThematicalFeatureTypes_A))]
            A,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.ThematicalFeatureTypes_B))]
            B,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.ThematicalFeatureTypes_C))]
            C,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.ThematicalFeatureTypes_D))]
            D,

            [ResourcesDisplayName(typeof(Resources), nameof(Resources.ThematicalFeatureTypes_E))]
            E
        }

        private const string selectedMetaDataAttributeName = "Waarde";
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
            var mapDataCollection = new MapDataCollection(Resources.OpenThematicalMapViewCommand_Execute_Demo_map_with_theming);

            var mapPointDataEqualCriteria = new MapPointData(Resources.OpenThematicalMapViewCommand_Execute_MapPointData_with_EqualValueCriteria,
                                                             CreatePointStyle(Color.Black),
                                                             CreatePointCategoryThemeWithEqualCriteria())
            {
                Features = CreateMapPointFeaturesWithMetaData(40, 0),
                Style =
                {
                    Size = 10
                },
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            mapDataCollection.Add(mapPointDataEqualCriteria);

            var mapPointDataUnequalCriteria = new MapPointData(Resources.OpenThematicalMapViewCommand_Execute_MapPointData_with_UnequalValueCriteria,
                                                               CreatePointStyle(Color.Black),
                                                               CreatePointCategoryThemeWithUnequalCriteria())
            {
                Features = CreateMapPointFeaturesWithMetaData(15, 5),
                Style =
                {
                    Size = 10
                },
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            mapDataCollection.Add(mapPointDataUnequalCriteria);

            var mapLineDataEqualCriteria = new MapLineData(Resources.OpenThematicalMapViewCommand_Execute_MapLineData_with_EqualValueCriteria,
                                                           CreateLineStyle(Color.Black),
                                                           CreateLineCategoryThemeWithEqualCriteria())
            {
                Features = CreateMapLineFeaturesWithMetaData(40, 10),
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            mapDataCollection.Add(mapLineDataEqualCriteria);

            var mapLineDataUnequalCriteria = new MapLineData(Resources.OpenThematicalMapViewCommand_Execute_MapLineData_with_UnequalValueCriteria,
                                                             CreateLineStyle(Color.Black),
                                                             CreateLineCategoryThemeWithUnequalCriteria())
            {
                Features = CreateMapLineFeaturesWithMetaData(10, 15),
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            mapDataCollection.Add(mapLineDataUnequalCriteria);

            var mapPolygonDataEqualCriteria = new MapPolygonData(Resources.OpenThematicalMapViewCommand_Execute_MapPolygonData_with_EqualValueCriteria,
                                                                 CreatePolygonStyle(Color.Black),
                                                                 CreatePolygonCategoryThemeWithEqualCriteria())
            {
                Features = CreatePolygonFeaturesWithMetaData(40, 20),
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            mapDataCollection.Add(mapPolygonDataEqualCriteria);

            var mapPolygonDataUnequalCriteria = new MapPolygonData(Resources.OpenThematicalMapViewCommand_Execute_MapPolygonData_with_UnequalValueCriteria,
                                                                   CreatePolygonStyle(Color.Black),
                                                                   CreatePolygonCategoryThemeWithUnequalCriteria())
            {
                Features = CreatePolygonFeaturesWithMetaData(10, 25),
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            mapDataCollection.Add(mapPolygonDataUnequalCriteria);

            viewCommands.OpenView(mapDataCollection);
        }

        private static ValueCriterion CreateEqualValueCriterion(ThematicalFeatureTypes featureType)
        {
            return new ValueCriterion(ValueCriterionOperator.EqualValue, GetDisplayName(featureType));
        }

        private static ValueCriterion CreateUnEqualValueCriterion(ThematicalFeatureTypes featureType)
        {
            return new ValueCriterion(ValueCriterionOperator.UnequalValue, GetDisplayName(featureType));
        }

        private static string GetDisplayName(ThematicalFeatureTypes thematicalFeatureTypes)
        {
            return new EnumDisplayWrapper<ThematicalFeatureTypes>(thematicalFeatureTypes).DisplayName;
        }

        private static IEnumerable<MapFeature> CreateMapPointFeaturesWithMetaData(int nrOfPoints, int bottom)
        {
            const double offset = 12;
            double xCoordinate = 0;

            var features = new MapFeature[nrOfPoints];

            for (var i = 0; i < nrOfPoints; i++)
            {
                MapFeature feature = GetFeatureWithPoints(new[]
                {
                    new Point2D(xCoordinate, bottom)
                });
                feature.MetaData[selectedMetaDataAttributeName] = GetThematicalFeatureType(i);
                features[i] = feature;

                xCoordinate += offset;
            }

            return features;
        }

        private static IEnumerable<MapFeature> CreateMapLineFeaturesWithMetaData(int nrOfLines, int bottom)
        {
            double xCoordinate = 0;

            var features = new MapFeature[nrOfLines];

            for (var i = 0; i < nrOfLines; i++)
            {
                MapFeature feature = GetFeatureWithPoints(new[]
                {
                    new Point2D(xCoordinate, bottom),
                    new Point2D(xCoordinate++, bottom + 3)
                });
                feature.MetaData[selectedMetaDataAttributeName] = GetThematicalFeatureType(i);
                features[i] = feature;
            }

            return features;
        }

        private static string GetThematicalFeatureType(int i)
        {
            return GetDisplayName((ThematicalFeatureTypes) (i % 5));
        }

        private static IEnumerable<MapFeature> CreatePolygonFeaturesWithMetaData(int nrOfPolygons, int bottom)
        {
            const double offset = 3;
            double leftCoordinate = 0;
            double rightCoordinate = 1;

            var features = new MapFeature[nrOfPolygons];

            for (var i = 0; i < nrOfPolygons; i++)
            {
                MapFeature feature = GetFeatureWithPoints(new[]
                {
                    new Point2D(leftCoordinate, bottom),
                    new Point2D(leftCoordinate, bottom + 3),
                    new Point2D(rightCoordinate, bottom + 3),
                    new Point2D(rightCoordinate, bottom),
                    new Point2D(leftCoordinate, bottom)
                });
                feature.MetaData[selectedMetaDataAttributeName] = GetThematicalFeatureType(i);
                features[i] = feature;

                leftCoordinate += offset;
                rightCoordinate += offset;
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

        #region MapLineData CategoryThemes

        private static MapTheme<LineCategoryTheme> CreateLineCategoryThemeWithEqualCriteria()
        {
            return new MapTheme<LineCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new LineCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.A), CreateLineStyle(Color.DarkOrange)),
                new LineCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.B), CreateLineStyle(Color.OrangeRed)),
                new LineCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.C), CreateLineStyle(Color.SkyBlue)),
                new LineCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.D), CreateLineStyle(Color.GreenYellow))
            });
        }

        private static MapTheme<LineCategoryTheme> CreateLineCategoryThemeWithUnequalCriteria()
        {
            return new MapTheme<LineCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new LineCategoryTheme(CreateUnEqualValueCriterion(ThematicalFeatureTypes.E), CreateLineStyle(Color.HotPink))
            });
        }

        private static LineStyle CreateLineStyle(Color color)
        {
            return new LineStyle
            {
                Width = 6,
                Color = color,
                DashStyle = LineDashStyle.Solid
            };
        }

        #endregion

        #region MapPointData CategoryThemes

        private static MapTheme<PointCategoryTheme> CreatePointCategoryThemeWithEqualCriteria()
        {
            return new MapTheme<PointCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new PointCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.A), CreatePointStyle(Color.DarkOrange)),
                new PointCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.B), CreatePointStyle(Color.OrangeRed)),
                new PointCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.C), CreatePointStyle(Color.SkyBlue)),
                new PointCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.D), CreatePointStyle(Color.GreenYellow))
            });
        }

        private static MapTheme<PointCategoryTheme> CreatePointCategoryThemeWithUnequalCriteria()
        {
            return new MapTheme<PointCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new PointCategoryTheme(CreateUnEqualValueCriterion(ThematicalFeatureTypes.E), CreatePointStyle(Color.HotPink))
            });
        }

        private static PointStyle CreatePointStyle(Color color)
        {
            return new PointStyle
            {
                Color = color,
                Size = 6,
                Symbol = PointSymbol.Hexagon,
                StrokeColor = color,
                StrokeThickness = 2
            };
        }

        #endregion

        #region MapPolygonData CategoryThemes

        private static MapTheme<PolygonCategoryTheme> CreatePolygonCategoryThemeWithEqualCriteria()
        {
            return new MapTheme<PolygonCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new PolygonCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.A), CreatePolygonStyle(Color.DarkOrange)),
                new PolygonCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.B), CreatePolygonStyle(Color.OrangeRed)),
                new PolygonCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.C), CreatePolygonStyle(Color.SkyBlue)),
                new PolygonCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.D), CreatePolygonStyle(Color.GreenYellow))
            });
        }

        private static MapTheme<PolygonCategoryTheme> CreatePolygonCategoryThemeWithUnequalCriteria()
        {
            return new MapTheme<PolygonCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new PolygonCategoryTheme(CreateUnEqualValueCriterion(ThematicalFeatureTypes.E), CreatePolygonStyle(Color.HotPink))
            });
        }

        private static PolygonStyle CreatePolygonStyle(Color color)
        {
            return new PolygonStyle
            {
                FillColor = color,
                StrokeColor = color,
                StrokeThickness = 2
            };
        }

        #endregion
    }
}