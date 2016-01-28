using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

using Core.Common.Controls.Commands;
using Core.Common.Gui;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Commands
{
    /// <summary>
    /// This class describes the command for opening a <see cref="ChartDataView"/> with some arbitrary data.
    /// </summary>
    public class OpenChartViewCommand : ICommand
    {
        private readonly IDocumentViewController documentViewController;

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
            var line = new LineData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(1.0, 2.1),
                new Tuple<double, double>(1.6, 1.6)    
            });
            var area = new AreaData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(1.0, 2.1),
                new Tuple<double, double>(1.6, 1.6),
                new Tuple<double, double>(1.6, 0.5),
                new Tuple<double, double>(0.0, 0.5),
                new Tuple<double, double>(0.0, 1.1)
            });
            var clearArea = new AreaData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.5, 0.5),    
                new Tuple<double, double>(0.5, 1.0),
                new Tuple<double, double>(1.0, 1.0),
                new Tuple<double, double>(0.5, 0.5)
            });
            var points = new PointData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(0.5, 1.6),  
                new Tuple<double, double>(1.0, 2.1)
            });
            documentViewController.DocumentViewsResolver.OpenViewForData(new ChartDataCollection(new List<ChartData> { area, clearArea, line, points }));
        }
    }
}