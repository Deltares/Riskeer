using System.Drawing;
using System.Windows.Forms;
using DelftTools.Utils.Collections;

namespace DelftTools.Controls.Swf.Charting.Tools
{
    public interface ISelectPointTool : IChartViewTool
    {
        event SelectionChangedEventHandler SelectionChanged;
        event NotifyCollectionChangedEventHandler CollectionChanged;
        NearestPointStyles Style { get; set; }
        Cursor Cursor { get; set; }
        Color SelectedPointerColor { get; set; }
        bool HandleDelete { get; set; }

        /// <summary>
        /// Defines the Size of the NearestTool shape.
        /// </summary>
        int Size { get; set; }

        void Invalidate();
        void ClearSelection();

        /// <summary>
        /// Adds a point to the selection
        /// </summary>
        /// <param name="chartSeries">The chart series a data source.</param>
        /// <param name="resultIndex">The data source index of the added point.</param>
        void AddPointAtIndexToSelection(IChartSeries chartSeries, int resultIndex);
    }
}