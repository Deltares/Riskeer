using System;
using System.Windows.Forms;
using Core.Common.Controls;
using Core.Common.Controls.Views;
using Core.Common.Gui.Properties;

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

            var lockable = selectedView as IReusableView;
            menuItemLockUnlock.Visible = lockable != null;

            if (lockable != null)
            {
                if (lockable.Locked)
                {
                    menuItemLockUnlock.Text = Resources.ViewSelectionContextMenuController_UpdateMenuItemsValidity_Unlock;
                    menuItemLockUnlock.Image = null;
                }
                else
                {
                    menuItemLockUnlock.Text = Resources.ViewSelectionContextMenuController_UpdateMenuItemsValidity_Lock;
                    menuItemLockUnlock.Image = Resources.lock_edit;
                }
            }
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

        private void LockToolStripMenuItemClick(object sender, EventArgs e)
        {
            ((IReusableView) selectedView).Locked = !((IReusableView) selectedView).Locked;
        }
    }
}