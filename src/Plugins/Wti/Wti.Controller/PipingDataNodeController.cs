using DelftTools.Controls;
using DelftTools.Controls.Swf;
using Wti.Data;
using Wti.Forms.NodePresenters;

namespace Wti.Controller
{
    public class PipingDataNodeController : IContextMenuProvider
    {
        private PipingDataNodePresenter nodePresenter = new PipingDataNodePresenter();

        public PipingDataNodePresenter NodePresenter
        {
            get
            {
                return nodePresenter;
            }
        }

        public PipingDataNodeController()
        {
            nodePresenter.ContextMenu = GetContextMenu;
        }

        public IMenuItem GetContextMenu(object pipingData)
        {
            var contextMenu = new PipingContextMenuStrip((PipingData) pipingData);
            var contextMenuAdapter = new MenuItemContextMenuStripAdapter(contextMenu);
            return contextMenuAdapter;
        }
    }
}