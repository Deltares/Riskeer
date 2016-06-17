using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.Charting.Data;
using Core.Components.Charting.Styles;
using Demo.Ringtoets.Properties;

namespace Demo.Ringtoets.Commands
{
    /// <summary>
    /// This class describes the command for opening a view for <see cref="ChartData"/> with some arbitrary data.
    /// </summary>
    public class OpenChartViewCommand : ICommand
    {
        private readonly IDocumentViewController documentViewController;

        /// <summary>
        /// Creates a new instance of <see cref="OpenChartViewCommand"/>.
        /// </summary>
        /// <param name="documentViewController">The <see cref="IDocumentViewController"/> to use internally.</param>
        public OpenChartViewCommand(IDocumentViewController documentViewController)
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
            var line1 = new ChartLineData(new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.0, 0.500),
                Tuple.Create(0.1, 0.723),
                Tuple.Create(0.2, 0.892),
                Tuple.Create(0.3, 1.013),
                Tuple.Create(0.4, 1.092),
                Tuple.Create(0.5, 1.135),
                Tuple.Create(0.6, 1.148),
                Tuple.Create(0.7, 1.137),
                Tuple.Create(0.8, 1.108),
                Tuple.Create(0.9, 1.067),
                Tuple.Create(1.0, 1.020),
                Tuple.Create(1.1, 0.973),
                Tuple.Create(1.2, 0.932),
                Tuple.Create(1.3, 0.903),
                Tuple.Create(1.4, 0.892),
                Tuple.Create(1.5, 0.905),
                Tuple.Create(1.6, 0.948),
                Tuple.Create(1.7, 1.027),
                Tuple.Create(1.8, 1.148),
                Tuple.Create(1.9, 1.317),
                Tuple.Create(2.0, 1.540),
                Tuple.Create(2.1, 1.823)
            }, Resources.OpenChartViewCommand_Execute_Line_one)
            {
                Style = new ChartLineStyle(Color.DarkRed, 3, DashStyle.Solid)
            };


            var line2 = new ChartLineData(new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.0, 0.800),
                Tuple.Create(0.1, 1.009),
                Tuple.Create(0.2, 1.162),
                Tuple.Create(0.3, 1.267),
                Tuple.Create(0.4, 1.328),
                Tuple.Create(0.5, 1.351),
                Tuple.Create(0.6, 1.340),
                Tuple.Create(0.7, 1.302),
                Tuple.Create(0.8, 1.242),
                Tuple.Create(0.9, 1.165),
                Tuple.Create(1.0, 1.076),
                Tuple.Create(1.1, 0.982),
                Tuple.Create(1.2, 0.886),
                Tuple.Create(1.3, 0.796),
                Tuple.Create(1.4, 0.716),
                Tuple.Create(1.5, 0.652),
                Tuple.Create(1.6, 0.608),
                Tuple.Create(1.7, 0.591),
                Tuple.Create(1.8, 0.606),
                Tuple.Create(1.9, 0.658),
                Tuple.Create(2.0, 0.752),
                Tuple.Create(2.1, 0.895)
            }, Resources.OpenChartViewCommand_Execute_Line_two)
            {
                Style = new ChartLineStyle(Color.DarkSlateBlue, 2, DashStyle.DashDot)
            };

            var area1 = new ChartAreaData(new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.0, 0.500),
                Tuple.Create(0.1, 0.723),
                Tuple.Create(0.2, 0.892),
                Tuple.Create(0.3, 1.013),
                Tuple.Create(0.4, 1.092),
                Tuple.Create(0.5, 1.135),
                Tuple.Create(0.6, 1.148),
                Tuple.Create(0.7, 1.137),
                Tuple.Create(0.8, 1.108),
                Tuple.Create(0.9, 1.067),
                Tuple.Create(1.0, 1.020),
                Tuple.Create(1.1, 0.973),
                Tuple.Create(1.2, 0.932),
                Tuple.Create(1.3, 0.903),
                Tuple.Create(1.4, 0.892),
                Tuple.Create(1.5, 0.905),
                Tuple.Create(1.5, 0.905 - 0.5),
                Tuple.Create(1.4, 0.892 - 0.5),
                Tuple.Create(1.3, 0.903 - 0.5),
                Tuple.Create(1.2, 0.932 - 0.5),
                Tuple.Create(1.1, 0.973 - 0.5),
                Tuple.Create(1.0, 1.020 - 0.5),
                Tuple.Create(0.9, 1.067 - 0.5),
                Tuple.Create(0.8, 1.108 - 0.51),
                Tuple.Create(0.7, 1.137 - 0.52),
                Tuple.Create(0.6, 1.148 - 0.53),
                Tuple.Create(0.5, 1.135 - 0.52),
                Tuple.Create(0.4, 1.092 - 0.51),
                Tuple.Create(0.3, 1.013 - 0.5),
                Tuple.Create(0.2, 0.892 - 0.5),
                Tuple.Create(0.1, 0.723 - 0.5),
                Tuple.Create(0.0, 0.000),
                Tuple.Create(0.0, 0.500)
            }, Resources.OpenChartViewCommand_Execute_Area_one)
            {
                Style = new ChartAreaStyle(Color.DarkSeaGreen, Color.DarkGreen, 5)
            };


            var area2 = new ChartAreaData(new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.1, 0.723 - 0.5),
                Tuple.Create(0.2, 0.892 - 0.5),
                Tuple.Create(0.3, 1.013 - 0.49),
                Tuple.Create(0.4, 1.092 - 0.48),
                Tuple.Create(0.5, 1.135 - 0.47),
                Tuple.Create(0.6, 1.148 - 0.46),
                Tuple.Create(0.7, 1.137 - 0.47),
                Tuple.Create(0.8, 1.108 - 0.48),
                Tuple.Create(0.9, 1.067 - 0.49),
                Tuple.Create(1.0, 1.020 - 0.50),
                Tuple.Create(1.1, 0.973 - 0.5),
                Tuple.Create(1.2, 0.932 - 0.5),
                Tuple.Create(1.3, 0.903 - 0.5),
                Tuple.Create(1.4, 0.892 - 0.5),
                Tuple.Create(1.5, 0.905 - 0.5),
                Tuple.Create(1.5, 0.905),
                Tuple.Create(1.6, 0.948),
                Tuple.Create(1.7, 1.027),
                Tuple.Create(1.8, 1.148),
                Tuple.Create(1.8, 0.606),
                Tuple.Create(1.9, 0.658),
                Tuple.Create(2.0, 0.752),
                Tuple.Create(2.0, 0.350),
                Tuple.Create(1.5, 0.905 - 0.7),
                Tuple.Create(1.4, 0.892 - 0.7),
                Tuple.Create(1.3, 0.903 - 0.7),
                Tuple.Create(1.2, 0.932 - 0.7),
                Tuple.Create(1.1, 0.973 - 0.7),
                Tuple.Create(1.0, 1.020 - 0.7),
                Tuple.Create(0.9, 1.067 - 0.7),
                Tuple.Create(0.8, 1.108 - 0.7),
                Tuple.Create(0.7, 1.137 - 0.7),
                Tuple.Create(0.6, 1.148 - 0.7),
                Tuple.Create(0.5, 1.135 - 0.7),
                Tuple.Create(0.4, 1.092 - 0.7),
                Tuple.Create(0.3, 1.013 - 0.7),
                Tuple.Create(0.2, 0.892 - 0.7),
                Tuple.Create(0.1, 0.723 - 0.7),
                Tuple.Create(0.1, 0.723 - 0.5)
            }, Resources.OpenChartViewCommand_Execute_Area_two)
            {
                Style = new ChartAreaStyle(Color.FromArgb(120, Color.Wheat), Color.DarkOrange, 2)
            };

            var points1 = new ChartPointData(new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.2, 0.892 + 0.04),
                Tuple.Create(0.3, 1.013 + 0.02),
                Tuple.Create(0.4, 1.092),
                Tuple.Create(0.5, 1.135 - 0.02),
                Tuple.Create(0.6, 1.148 + 0.01),
                Tuple.Create(1.4, 0.892 - 0.02),
                Tuple.Create(1.5, 0.905 + 0.01),
                Tuple.Create(1.8, 1.148 + 0.02)
            }, Resources.OpenChartViewCommand_Execute_Points_one)
            {
                Style = new ChartPointStyle(Color.Crimson, 6, Color.AntiqueWhite, 3, ChartPointSymbol.Circle)
            };

            var points2 = new ChartPointData(new Collection<Tuple<double, double>>
            {
                Tuple.Create(0.0, 0.800 + 0.01),
                Tuple.Create(0.1, 1.009 + 0.02),
                Tuple.Create(0.2, 1.162 + 0.03),
                Tuple.Create(0.2, 1.162 + 0.05),
                Tuple.Create(0.2, 1.162 - 0.03),
                Tuple.Create(0.2, 1.162 - 0.01),
                Tuple.Create(0.3, 1.267),
                Tuple.Create(0.4, 1.328 - 0.01),
                Tuple.Create(0.53, 1.351),
                Tuple.Create(0.69, 1.340),
                Tuple.Create(0.73, 1.302),
                Tuple.Create(1.4, 0.716 - 0.02),
                Tuple.Create(1.4, 0.716 + 0.02),
                Tuple.Create(1.7, 0.591),
                Tuple.Create(1.8, 0.606)
            }, Resources.OpenChartViewCommand_Execute_Points_two)
            {
                Style = new ChartPointStyle(Color.FromArgb(190, Color.Gold), 7, Color.DeepSkyBlue, 2, ChartPointSymbol.Diamond)
            };

            documentViewController.DocumentViewsResolver.OpenViewForData(new ChartDataCollection(new List<ChartData>
            {
                area1, area2, line1, line2, points1, points2
            }, Resources.OpenChartViewCommand_Execute_Graph_data));
        }
    }
}