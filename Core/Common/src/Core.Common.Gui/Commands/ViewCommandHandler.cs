// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of 
// Stichting Deltares and remain full property of Stichting Deltares at all times. 
// All rights reserved.

using System.Collections.Generic;
using System.Linq;

using Core.Common.Controls.Views;
using Core.Common.Gui.Selection;

namespace Core.Common.Gui.Commands
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