using System.Collections.Generic;
using System.Linq;

using Core.Common.Controls.Views;

namespace Core.Common.Gui
{
    /// <summary>
    /// This class provides concrete implementation of <see cref="IViewCommands"/>.
    /// </summary>
    public class ViewCommandHandler : IViewCommands
    {
        private readonly IGui gui;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCommandHandler"/> class.
        /// </summary>
        /// <param name="gui">The GUI.</param>
        public ViewCommandHandler(IGui gui)
        {
            this.gui = gui;
        }

        public object GetDataOfActiveView()
        {
            return gui.DocumentViews.ActiveView != null ? gui.DocumentViews.ActiveView.Data : null;
        }

        public bool CanOpenSelectViewDialog()
        {
            return gui.Selection != null && gui.DocumentViewsResolver.GetViewInfosFor(gui.Selection).Count() > 1;
        }

        public void OpenSelectViewDialog()
        {
            gui.DocumentViewsResolver.OpenViewForData(gui.Selection, true);
        }

        public bool CanOpenViewFor(object obj)
        {
            return gui.DocumentViewsResolver.GetViewInfosFor(obj).Any();
        }

        public void OpenView(object dataObject)
        {
            gui.DocumentViewsResolver.OpenViewForData(dataObject);
        }

        public void OpenViewForSelection()
        {
            gui.DocumentViewsResolver.OpenViewForData(gui.Selection);
        }

        /// <summary>
        /// Removes all document and tool views that are associated to the dataObject and/or its children.
        /// </summary>
        /// <param name="dataObject"></param>
        public void RemoveAllViewsForItem(object dataObject)
        {
            if (dataObject == null || gui == null || gui.DocumentViews == null || gui.DocumentViews.Count == 0)
            {
                return;
            }
            foreach (var data in gui.GetAllDataWithViewDefinitionsRecursively(dataObject))
            {
                gui.DocumentViewsResolver.CloseAllViewsFor(data);
                RemoveViewsAndData(gui.ToolWindowViews.Where(v => v.Data == data).ToArray());
            }
        }

        private void RemoveViewsAndData(IEnumerable<IView> toolViews)
        {
            // set all tool windows where dataObject was used to null
            foreach (var view in toolViews)
            {
                view.Data = null;
            }
        }
    }
}