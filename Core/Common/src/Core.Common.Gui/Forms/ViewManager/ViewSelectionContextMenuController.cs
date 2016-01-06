using System;
using System.Windows.Forms;
using Core.Common.Controls.Views;

namespace Core.Common.Gui.Forms.ViewManager
{
    public partial class ViewSelectionContextMenuController : UserControl
    {
        private IView selectedView;
        private IViewList viewManager;

        public ViewSelectionContextMenuController()
        {
            InitializeComponent();
            ContextMenuStrip = contextMenuStrip;
        }

        public new ContextMenuStrip ContextMenuStrip { get; private set; }

        public bool ContextMenuStripValidate(IView view, IViewList viewManager)
        {
            this.viewManager = viewManager;

            selectedView = view ?? viewManager.ActiveView;

            UpdateMenuItemsValidity();

            return true;
        }

        private void UpdateMenuItemsValidity()
        {
            menuItemClose.Enabled = selectedView != null;
            menuItemCloseAll.Enabled = viewManager.Count > 0;
            menuItemCloseOther.Enabled = selectedView != null && viewManager.Count > 1;
        }

        private void MenuItemCloseClick(object sender, EventArgs e)
        {
            viewManager.Remove(selectedView);
        }

        private void MenuItemCloseOtherClick(object sender, EventArgs e)
        {
            viewManager.Clear(selectedView);
        }

        private void MenuItemCloseAllClick(object sender, EventArgs e)
        {
            viewManager.Clear();
        }
    }
}