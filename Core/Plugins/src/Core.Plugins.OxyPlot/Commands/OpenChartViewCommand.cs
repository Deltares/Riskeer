using System;
using System.Collections.ObjectModel;
using Core.Common.Gui;
using Core.Components.OxyPlot.Data;
using Core.Plugins.OxyPlot.Forms;
using OxyPlot;

namespace Core.Plugins.OxyPlot.Commands
{
    /// <summary>
    /// This class describes the command for opening a <see cref="ChartDataView"/> with some random data.
    /// </summary>
    public class OpenChartViewCommand : IGuiCommand {

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

        public IGui Gui { get; set; }

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
            clearArea.Fill = OxyColor.FromArgb(255,255,255,255);
            var points = new PointData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(0.5, 1.6),  
                new Tuple<double, double>(1.0, 2.1)
            });
            var data = new CollectionData();
            data.Add(area);
            data.Add(clearArea);
            data.Add(line);
            data.Add(points);
            Gui.DocumentViewsResolver.OpenViewForData(data);
        }
    }
}