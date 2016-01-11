using Core.Common.Controls.Views;
using Core.Components.OxyPlot.Forms;

namespace Core.Plugins.OxyPlot.Forms
{
    /// <summary>
    /// Interface describing <see cref="IView"/> that contain a <see cref="BaseChart"/> as one of its components.
    /// </summary>
    public interface IChartView : IView
    {
        /// <summary>
        /// Gets the <see cref="BaseChart"/> set for this <see cref="IChartView"/>.
        /// </summary>
        BaseChart Chart { get; }
    }
}