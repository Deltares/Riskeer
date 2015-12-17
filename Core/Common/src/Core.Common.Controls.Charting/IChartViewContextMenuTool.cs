using System.Windows.Forms;

namespace Core.Common.Controls.Charting
{
    public interface IChartViewContextMenuTool : IChartViewTool
    {
        void OnBeforeContextMenu(ContextMenuStrip menu);
    }
}