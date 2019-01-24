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

using System.Drawing;
using Core.Common.Base.Geometry;
using Core.Common.Controls.Commands;
using Core.Common.Gui.Commands;
using Core.Components.Chart.Data;
using Core.Components.Chart.Styles;

namespace Demo.Riskeer.Commands
{
    /// <summary>
    /// This class describes the command for opening a view for <see cref="ChartData"/> with some arbitrary data.
    /// </summary>
    public class OpenChartViewCommand : ICommand
    {
        private readonly IViewCommands viewCommands;

        /// <summary>
        /// Creates a new instance of <see cref="OpenChartViewCommand"/>.
        /// </summary>
        /// <param name="viewCommands">The <see cref="IViewCommands"/> to use internally.</param>
        public OpenChartViewCommand(IViewCommands viewCommands)
        {
            this.viewCommands = viewCommands;
        }

        public bool Checked
        {
            get
            {
                return false;
            }
        }

        public void Execute()
        {
            var line1 = new ChartLineData(Resources.OpenChartViewCommand_Execute_Line_one,
                                          new ChartLineStyle
                                          {
                                              Color = Color.DarkRed,
                                              Width = 3,
                                              DashStyle = ChartLineDashStyle.Solid,
                                              IsEditable = true
                                          })
            {
                Points = new[]
                {
                    new Point2D(0.0, 0.500),
                    new Point2D(0.1, 0.723),
                    new Point2D(0.2, 0.892),
                    new Point2D(0.3, 1.013),
                    new Point2D(0.4, 1.092),
                    new Point2D(0.5, 1.135),
                    new Point2D(0.6, 1.148),
                    new Point2D(0.7, 1.137),
                    new Point2D(0.8, 1.108),
                    new Point2D(0.9, 1.067),
                    new Point2D(1.0, 1.020),
                    new Point2D(1.1, 0.973),
                    new Point2D(1.2, 0.932),
                    new Point2D(1.3, 0.903),
                    new Point2D(1.4, 0.892),
                    new Point2D(1.5, 0.905),
                    new Point2D(1.6, 0.948),
                    new Point2D(1.7, 1.027),
                    new Point2D(1.8, 1.148),
                    new Point2D(1.9, 1.317),
                    new Point2D(2.0, 1.540),
                    new Point2D(2.1, 1.823)
                }
            };

            var line2 = new ChartLineData(Resources.OpenChartViewCommand_Execute_Line_two,
                                          new ChartLineStyle
                                          {
                                              Color = Color.DarkSlateBlue,
                                              Width = 2,
                                              DashStyle = ChartLineDashStyle.DashDot,
                                              IsEditable = true
                                          })
            {
                Points = new[]
                {
                    new Point2D(0.0, 0.800),
                    new Point2D(0.1, 1.009),
                    new Point2D(0.2, 1.162),
                    new Point2D(0.3, 1.267),
                    new Point2D(0.4, 1.328),
                    new Point2D(0.5, 1.351),
                    new Point2D(0.6, 1.340),
                    new Point2D(0.7, 1.302),
                    new Point2D(0.8, 1.242),
                    new Point2D(0.9, 1.165),
                    new Point2D(1.0, 1.076),
                    new Point2D(1.1, 0.982),
                    new Point2D(1.2, 0.886),
                    new Point2D(1.3, 0.796),
                    new Point2D(1.4, 0.716),
                    new Point2D(1.5, 0.652),
                    new Point2D(1.6, 0.608),
                    new Point2D(1.7, 0.591),
                    new Point2D(1.8, 0.606),
                    new Point2D(1.9, 0.658),
                    new Point2D(2.0, 0.752),
                    new Point2D(2.1, 0.895)
                }
            };

            var area1 = new ChartAreaData(Resources.OpenChartViewCommand_Execute_Area_one,
                                          new ChartAreaStyle
                                          {
                                              FillColor = Color.DarkSeaGreen,
                                              StrokeColor = Color.DarkGreen,
                                              StrokeThickness = 5,
                                              IsEditable = true
                                          })
            {
                Points = new[]
                {
                    new Point2D(0.0, 0.500),
                    new Point2D(0.1, 0.723),
                    new Point2D(0.2, 0.892),
                    new Point2D(0.3, 1.013),
                    new Point2D(0.4, 1.092),
                    new Point2D(0.5, 1.135),
                    new Point2D(0.6, 1.148),
                    new Point2D(0.7, 1.137),
                    new Point2D(0.8, 1.108),
                    new Point2D(0.9, 1.067),
                    new Point2D(1.0, 1.020),
                    new Point2D(1.1, 0.973),
                    new Point2D(1.2, 0.932),
                    new Point2D(1.3, 0.903),
                    new Point2D(1.4, 0.892),
                    new Point2D(1.5, 0.905),
                    new Point2D(1.5, 0.905 - 0.5),
                    new Point2D(1.4, 0.892 - 0.5),
                    new Point2D(1.3, 0.903 - 0.5),
                    new Point2D(1.2, 0.932 - 0.5),
                    new Point2D(1.1, 0.973 - 0.5),
                    new Point2D(1.0, 1.020 - 0.5),
                    new Point2D(0.9, 1.067 - 0.5),
                    new Point2D(0.8, 1.108 - 0.51),
                    new Point2D(0.7, 1.137 - 0.52),
                    new Point2D(0.6, 1.148 - 0.53),
                    new Point2D(0.5, 1.135 - 0.52),
                    new Point2D(0.4, 1.092 - 0.51),
                    new Point2D(0.3, 1.013 - 0.5),
                    new Point2D(0.2, 0.892 - 0.5),
                    new Point2D(0.1, 0.723 - 0.5),
                    new Point2D(0.0, 0.000),
                    new Point2D(0.0, 0.500)
                }
            };

            var area2 = new ChartAreaData(Resources.OpenChartViewCommand_Execute_Area_two,
                                          new ChartAreaStyle
                                          {
                                              FillColor = Color.FromArgb(120, Color.Wheat),
                                              StrokeColor = Color.DarkOrange,
                                              StrokeThickness = 2,
                                              IsEditable = true
                                          })
            {
                Points = new[]
                {
                    new Point2D(0.1, 0.723 - 0.5),
                    new Point2D(0.2, 0.892 - 0.5),
                    new Point2D(0.3, 1.013 - 0.49),
                    new Point2D(0.4, 1.092 - 0.48),
                    new Point2D(0.5, 1.135 - 0.47),
                    new Point2D(0.6, 1.148 - 0.46),
                    new Point2D(0.7, 1.137 - 0.47),
                    new Point2D(0.8, 1.108 - 0.48),
                    new Point2D(0.9, 1.067 - 0.49),
                    new Point2D(1.0, 1.020 - 0.50),
                    new Point2D(1.1, 0.973 - 0.5),
                    new Point2D(1.2, 0.932 - 0.5),
                    new Point2D(1.3, 0.903 - 0.5),
                    new Point2D(1.4, 0.892 - 0.5),
                    new Point2D(1.5, 0.905 - 0.5),
                    new Point2D(1.5, 0.905),
                    new Point2D(1.6, 0.948),
                    new Point2D(1.7, 1.027),
                    new Point2D(1.8, 1.148),
                    new Point2D(1.8, 0.606),
                    new Point2D(1.9, 0.658),
                    new Point2D(2.0, 0.752),
                    new Point2D(2.0, 0.350),
                    new Point2D(1.5, 0.905 - 0.7),
                    new Point2D(1.4, 0.892 - 0.7),
                    new Point2D(1.3, 0.903 - 0.7),
                    new Point2D(1.2, 0.932 - 0.7),
                    new Point2D(1.1, 0.973 - 0.7),
                    new Point2D(1.0, 1.020 - 0.7),
                    new Point2D(0.9, 1.067 - 0.7),
                    new Point2D(0.8, 1.108 - 0.7),
                    new Point2D(0.7, 1.137 - 0.7),
                    new Point2D(0.6, 1.148 - 0.7),
                    new Point2D(0.5, 1.135 - 0.7),
                    new Point2D(0.4, 1.092 - 0.7),
                    new Point2D(0.3, 1.013 - 0.7),
                    new Point2D(0.2, 0.892 - 0.7),
                    new Point2D(0.1, 0.723 - 0.7),
                    new Point2D(0.1, 0.723 - 0.5)
                }
            };

            var points1 = new ChartPointData(Resources.OpenChartViewCommand_Execute_Points_one,
                                             new ChartPointStyle
                                             {
                                                 Color = Color.Crimson,
                                                 StrokeColor = Color.AntiqueWhite,
                                                 Size = 6,
                                                 StrokeThickness = 3,
                                                 Symbol = ChartPointSymbol.Circle,
                                                 IsEditable = true
                                             })
            {
                Points = new[]
                {
                    new Point2D(0.2, 0.892 + 0.04),
                    new Point2D(0.3, 1.013 + 0.02),
                    new Point2D(0.4, 1.092),
                    new Point2D(0.5, 1.135 - 0.02),
                    new Point2D(0.6, 1.148 + 0.01),
                    new Point2D(1.4, 0.892 - 0.02),
                    new Point2D(1.5, 0.905 + 0.01),
                    new Point2D(1.8, 1.148 + 0.02)
                }
            };

            var points2 = new ChartPointData(Resources.OpenChartViewCommand_Execute_Points_two,
                                             new ChartPointStyle
                                             {
                                                 Color = Color.FromArgb(190, Color.Gold),
                                                 StrokeColor = Color.DeepSkyBlue,
                                                 Size = 7,
                                                 StrokeThickness = 2,
                                                 Symbol = ChartPointSymbol.Diamond,
                                                 IsEditable = true
                                             })
            {
                Points = new[]
                {
                    new Point2D(0.0, 0.800 + 0.01),
                    new Point2D(0.1, 1.009 + 0.02),
                    new Point2D(0.2, 1.162 + 0.03),
                    new Point2D(0.2, 1.162 + 0.05),
                    new Point2D(0.2, 1.162 - 0.03),
                    new Point2D(0.2, 1.162 - 0.01),
                    new Point2D(0.3, 1.267),
                    new Point2D(0.4, 1.328 - 0.01),
                    new Point2D(0.53, 1.351),
                    new Point2D(0.69, 1.340),
                    new Point2D(0.73, 1.302),
                    new Point2D(1.4, 0.716 - 0.02),
                    new Point2D(1.4, 0.716 + 0.02),
                    new Point2D(1.7, 0.591),
                    new Point2D(1.8, 0.606)
                }
            };

            var chartDataCollection = new ChartDataCollection(Resources.OpenChartViewCommand_Execute_Graph_data);

            chartDataCollection.Add(area1);
            chartDataCollection.Add(area2);
            chartDataCollection.Add(line1);
            chartDataCollection.Add(line2);
            chartDataCollection.Add(points1);
            chartDataCollection.Add(points2);

            viewCommands.OpenView(chartDataCollection);
        }
    }
}