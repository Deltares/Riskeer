using System.Windows.Forms;

namespace Core.GIS.SharpMap.UI.Tools
{
    public class MapToolContextMenuItem
    {
        /// <summary>
        /// Priority indicator (used for sorting the menu items and creating categories using separators).
        /// 
        /// 1 = Most used item (should be on top)
        /// 2 = Frequently used item (should be near the top)
        /// 3 = Normal item (somewhere in the middle)
        /// 4 = Item that is used sometimes (should be near the bottom)
        /// >4 = Rarely used item (should be on the bottom)
        /// </summary>
        public int Priority { get; set; }

        public ToolStripMenuItem MenuItem { get; set; }
    }
}