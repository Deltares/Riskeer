using System;
using System.Collections.ObjectModel;
using Core.Common.Gui;
using Core.Components.OxyPlot;

namespace Core.Plugins.OxyPlot
{
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
            ChartData line = new ChartData(new Collection<Tuple<double, double>>
            {
                new Tuple<double, double>(0.0, 1.1),    
                new Tuple<double, double>(1.0, 2.1),
                new Tuple<double, double>(1.6, 1.6)    
            });
            Gui.DocumentViewsResolver.OpenViewForData(line);
        }
    }
}