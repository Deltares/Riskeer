// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
using Demo.Ringtoets.Properties;

namespace Demo.Ringtoets.Commands
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

            var mapPointDataEqualCriteria = new MapPointData(Resources.OpenThematicalMapViewCommand_Execute_MapPointData_with_EqualValueCriteria)
            {
                Features = CreateMapPointFeaturesWithMetaData(40, 0),
                Style =
                {
                    Size = 10
                },
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            SetEqualCriteria(mapPointDataEqualCriteria);
            mapDataCollection.Add(mapPointDataEqualCriteria);

            var mapPointDataUnequalCriteria = new MapPointData(Resources.OpenThematicalMapViewCommand_Execute_MapPointData_with_UnequalValueCriteria)
            {
                Features = CreateMapPointFeaturesWithMetaData(15, 5),
                Style =
                {
                    Size = 10
                },
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            SetUnequalCriteria(mapPointDataUnequalCriteria);
            mapDataCollection.Add(mapPointDataUnequalCriteria);

            var mapLineDataEqualCriteria = new MapLineData(Resources.OpenThematicalMapViewCommand_Execute_MapLineData_with_EqualValueCriteria)
            {
                Features = CreateMapLineFeaturesWithMetaData(40, 10),
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            SetEqualCriteria(mapLineDataEqualCriteria);
            mapDataCollection.Add(mapLineDataEqualCriteria);

            var mapLineDataUnequalCriteria = new MapLineData(Resources.OpenThematicalMapViewCommand_Execute_MapLineData_with_UnequalValueCriteria)
            {
                Features = CreateMapLineFeaturesWithMetaData(10, 15),
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            SetUnequalCriteria(mapLineDataUnequalCriteria);
            mapDataCollection.Add(mapLineDataUnequalCriteria);

            var mapPolygonDataEqualCriteria = new MapPolygonData(Resources.OpenThematicalMapViewCommand_Execute_MapPolygonData_with_EqualValueCriteria)
            {
                Features = CreatePolygonFeaturesWithMetaData(40, 20),
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            SetEqualCriteria(mapPolygonDataEqualCriteria);
            mapDataCollection.Add(mapPolygonDataEqualCriteria);

            var mapPolygonDataUnequalCriteria = new MapPolygonData(Resources.OpenThematicalMapViewCommand_Execute_MapPolygonData_with_UnequalValueCriteria)
            {
                Features = CreatePolygonFeaturesWithMetaData(10, 25),
                SelectedMetaDataAttribute = selectedMetaDataAttributeName
            };
            SetUnequalCriteria(mapPolygonDataUnequalCriteria);
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

        private static void SetEqualCriteria(MapLineData mapData)
        {
            LineStyle defaultStyle = mapData.Style;
            var theme = new MapTheme<LineCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new LineCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.A), CreateLineStyle(Color.DarkOrange, defaultStyle)),
                new LineCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.B), CreateLineStyle(Color.OrangeRed, defaultStyle)),
                new LineCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.C), CreateLineStyle(Color.SkyBlue, defaultStyle)),
                new LineCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.D), CreateLineStyle(Color.GreenYellow, defaultStyle))
            });

            mapData.Theme = theme;
        }

        private static void SetUnequalCriteria(MapLineData mapData)
        {
            LineStyle defaultLineStyle = mapData.Style;
            var theme = new MapTheme<LineCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new LineCategoryTheme(CreateUnEqualValueCriterion(ThematicalFeatureTypes.E), CreateLineStyle(Color.HotPink, defaultLineStyle))
            });

            mapData.Theme = theme;
        }

        private static LineStyle CreateLineStyle(Color color, LineStyle defaultStyle)
        {
            return new LineStyle
            {
                Width = defaultStyle.Width,
                Color = color,
                DashStyle = defaultStyle.DashStyle
            };
        }

        #endregion

        #region MapPointData CategoryThemes

        private static void SetEqualCriteria(MapPointData mapData)
        {
            PointStyle defaultStyle = mapData.Style;
            var theme = new MapTheme<PointCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new PointCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.A), CreatePointStyle(Color.DarkOrange, defaultStyle)),
                new PointCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.B), CreatePointStyle(Color.OrangeRed, defaultStyle)),
                new PointCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.C), CreatePointStyle(Color.SkyBlue, defaultStyle)),
                new PointCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.D), CreatePointStyle(Color.GreenYellow, defaultStyle))
            });

            mapData.Theme = theme;
        }

        private static void SetUnequalCriteria(MapPointData mapData)
        {
            PointStyle defaultStyle = mapData.Style;
            var theme = new MapTheme<PointCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new PointCategoryTheme(CreateUnEqualValueCriterion(ThematicalFeatureTypes.E), CreatePointStyle(Color.HotPink, defaultStyle))
            });

            mapData.Theme = theme;
        }

        private static PointStyle CreatePointStyle(Color color, PointStyle defaultStyle)
        {
            return new PointStyle
            {
                Color = color,
                Size = defaultStyle.Size,
                Symbol = defaultStyle.Symbol,
                StrokeColor = color,
                StrokeThickness = defaultStyle.StrokeThickness
            };
        }

        #endregion

        #region MapPointData CategoryThemes

        private static void SetEqualCriteria(MapPolygonData mapData)
        {
            PolygonStyle defaultStyle = mapData.Style;
            var theme = new MapTheme<PolygonCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new PolygonCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.A), CreatePointStyle(Color.DarkOrange, defaultStyle)),
                new PolygonCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.B), CreatePointStyle(Color.OrangeRed, defaultStyle)),
                new PolygonCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.C), CreatePointStyle(Color.SkyBlue, defaultStyle)),
                new PolygonCategoryTheme(CreateEqualValueCriterion(ThematicalFeatureTypes.D), CreatePointStyle(Color.GreenYellow, defaultStyle))
            });

            mapData.Theme = theme;
        }

        private static void SetUnequalCriteria(MapPolygonData mapData)
        {
            PolygonStyle defaultStyle = mapData.Style;
            var theme = new MapTheme<PolygonCategoryTheme>(selectedMetaDataAttributeName, new[]
            {
                new PolygonCategoryTheme(CreateUnEqualValueCriterion(ThematicalFeatureTypes.E), CreatePointStyle(Color.HotPink, defaultStyle))
            });

            mapData.Theme = theme;
        }

        private static PolygonStyle CreatePointStyle(Color color, PolygonStyle defaultStyle)
        {
            return new PolygonStyle
            {
                FillColor = color,
                StrokeColor = color,
                StrokeThickness = defaultStyle.StrokeThickness
            };
        }

        #endregion
    }
}