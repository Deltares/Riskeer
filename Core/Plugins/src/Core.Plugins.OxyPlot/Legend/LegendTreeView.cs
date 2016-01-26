using Core.Common.Base;
using Core.Components.Charting.Data;
using Core.Components.OxyPlot.Forms;
using TreeView = Core.Common.Controls.TreeView.TreeView;

namespace Core.Plugins.OxyPlot.Legend
{
    /// <summary>
    /// This class describes the view for showing and configuring the data of a <see cref="BaseChart"/>.
    /// </summary>
    public class LegendTreeView : TreeView
    {
        /// <summary>
        /// Creates a new instance of <see cref="LegendTreeView"/>.
        /// </summary>
        public LegendTreeView()
        {
            RegisterNodePresenter(new LineDataNodePresenter());
            RegisterNodePresenter(new PointDataNodePresenter());
            RegisterNodePresenter(new AreaDataNodePresenter());
            RegisterNodePresenter(new ChartNodePresenter());
        }

        /// <summary>
        /// Gets or sets the <see cref="BaseChart"/> that is used as the source of the <see cref="LegendTreeView"/>.
        /// </summary>
        public ChartData ChartData
        {
            get
            {
                return (ChartData)Data;
            }
            set
            {
                Data = value;
                
                if (value == null)
                {
                    Nodes.Clear();
                }
            }
        }

        protected override void Dispose(bool disposing)
        {
            Data = null;
            base.Dispose(disposing);
        }
    }
}