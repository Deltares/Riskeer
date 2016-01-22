using System.Drawing;
using Core.Components.Charting.Data;
using Core.Plugins.OxyPlot.Properties;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class describes the presentation of <see cref="AreaData"/> as a tree node.
    /// </summary>
    public class AreaDataNodePresenter : ChartDataNodePresenter<AreaData>
    {
        protected override string Text
        {
            get
            {
                return Resources.ChartDataNodePresenter_Area_data_label;
            }
        }

        protected override Bitmap Icon
        {
            get
            {
                return Resources.AreaIcon;
            }
        }
    }
}