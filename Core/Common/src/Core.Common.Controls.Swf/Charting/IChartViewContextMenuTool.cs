using System.Windows.Forms;

namespace Core.Common.Controls.Swf.Charting
{
    public interface IChartViewContextMenuTool : IChartViewTool
    {
        void OnBeforeContextMenu(ContextMenuStrip menu);
    }
}