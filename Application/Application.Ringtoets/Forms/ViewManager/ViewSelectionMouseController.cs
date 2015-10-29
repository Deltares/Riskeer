using System;
using System.Windows.Forms;
using Application.Ringtoets.Properties;
using Core.Common.Controls;
using Core.Common.Gui;

namespace Application.Ringtoets.Forms.ViewManager
{
    public class ViewSelectionMouseController
    {
        private readonly ViewSelectionContextMenuController contextMenuController; // expose when necessary

        public ViewSelectionMouseController(IDockingManager dockingManager, IViewList viewList)
        {
            DockingManager = dockingManager;
            ViewManager = viewList;

            dockingManager.ViewSelectionMouseDown += OnViewSelectionMouseDown;

            contextMenuController = new ViewSelectionContextMenuController();

            viewList.ActiveViewChanged += ViewManagerActiveViewChanged;
        }

        public IDockingManager DockingManager { get; private set; }

        public IViewList ViewManager { get; set; }

        private void ViewManagerActiveViewChanged(object sender, ActiveViewChangeEventArgs e)
        {
            contextMenuController.ContextMenuStripValidate(null, ViewManager);
        }

        private void OnViewSelectionMouseDown(object sender, MouseEventArgs e, IView selectedView)
        {
            if (!(sender is Control))
            {
                throw new ArgumentException(Resources.ViewSelectionMouseController_OnViewSelectionMouseDown_Sender_must_be_non_null_and_of_type_Control);
            }

            if (!ViewManager.Contains(selectedView))
            {
                return; //View is not in our ViewList, don't handle this
            }

            if (e.Button == MouseButtons.Right)
            {
                if (contextMenuController.ContextMenuStripValidate(selectedView, ViewManager))
                {
                    contextMenuController.ContextMenuStrip.Show((sender as Control).PointToScreen(e.Location));
                }
            }
        }
    }
}