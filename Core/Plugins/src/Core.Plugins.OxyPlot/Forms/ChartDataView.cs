using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Core.Plugins.OxyPlot.Forms
{
    public class ChartDataView : UserControl, IView
    {
        public object Data { get; set; }
    }
}