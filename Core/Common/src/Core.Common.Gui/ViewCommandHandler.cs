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
        private readonly IDocumentViewController documentViewController;
        private readonly IToolViewController toolViewController;
        private readonly IApplicationSelection applicationSelection;
        private readonly IGuiPluginsHost guiPluginsHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCommandHandler"/> class.
        /// </summary>
        /// <param name="documentViewController"></param>
        /// <param name="toolViewController"></param>
        /// <param name="applicationSelection"></param>
        /// <param name="pluginsHost"></param>
        public ViewCommandHandler(IDocumentViewController documentViewController, IToolViewController toolViewController,
                                  IApplicationSelection applicationSelection, IGuiPluginsHost pluginsHost)
        {
            this.documentViewController = documentViewController;
            this.toolViewController = toolViewController;
            this.applicationSelection = applicationSelection;
            guiPluginsHost = pluginsHost;
        }

        public object GetDataOfActiveView()
        {
            return documentViewController.DocumentViews.ActiveView != null ? documentViewController.DocumentViews.ActiveView.Data : null;
        }

        public bool CanOpenSelectViewDialog()
        {
            return applicationSelection.Selection != null && documentViewController.DocumentViewsResolver.GetViewInfosFor(applicationSelection.Selection).Count() > 1;
        }

        public void OpenSelectViewDialog()
        {
            documentViewController.DocumentViewsResolver.OpenViewForData(applicationSelection.Selection, true);
        }

        public bool CanOpenViewFor(object obj)
        {
            return documentViewController.DocumentViewsResolver.GetViewInfosFor(obj).Any();
        }

        public void OpenView(object dataObject)
        {
            documentViewController.DocumentViewsResolver.OpenViewForData(dataObject);
        }

        public void OpenViewForSelection()
        {
            documentViewController.DocumentViewsResolver.OpenViewForData(applicationSelection.Selection);
        }

        /// <summary>
        /// Removes all document and tool views that are associated to the dataObject and/or its children.
        /// </summary>
        /// <param name="dataObject"></param>
        public void RemoveAllViewsForItem(object dataObject)
        {
            if (dataObject == null || documentViewController == null || documentViewController.DocumentViews == null || documentViewController.DocumentViews.Count == 0)
            {
                return;
            }
            foreach (var data in guiPluginsHost.GetAllDataWithViewDefinitionsRecursively(dataObject))
            {
                documentViewController.DocumentViewsResolver.CloseAllViewsFor(data);
                RemoveViewsAndData(toolViewController.ToolWindowViews.Where(v => v.Data == data).ToArray());
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