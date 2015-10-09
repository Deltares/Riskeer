using System.Windows.Forms;

namespace DelftTools.Controls.Swf.Charting
{
    public interface IChartViewContextMenuTool : IChartViewTool
    {
        void OnBeforeContextMenu(ContextMenuStrip menu);
    }
}