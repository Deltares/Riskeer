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
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
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
        /// <param name="documentViewController">The controller for Document Views.</param>
        /// <param name="toolViewController">The controller for Tool Views.</param>
        /// <param name="applicationSelection">The application selection mechanism.</param>
        /// <param name="guiPluginsHost">The gui-plugins host.</param>
        public ViewCommandHandler(IDocumentViewController documentViewController, IToolViewController toolViewController,
                                  IApplicationSelection applicationSelection, IGuiPluginsHost guiPluginsHost)
        {
            this.documentViewController = documentViewController;
            this.toolViewController = toolViewController;
            this.applicationSelection = applicationSelection;
            this.guiPluginsHost = guiPluginsHost;
        }

        public object GetDataOfActiveView()
        {
            return documentViewController.DocumentViews.ActiveView != null ? documentViewController.DocumentViews.ActiveView.Data : null;
        }

        public bool CanOpenSelectViewDialog()
        {
            return applicationSelection.Selection != null && 
                documentViewController.DocumentViewsResolver.GetViewInfosFor(applicationSelection.Selection).Count() > 1;
        }

        public void OpenSelectViewDialog()
        {
            documentViewController.DocumentViewsResolver.OpenViewForData(applicationSelection.Selection, true);
        }

        public void OpenViewForSelection()
        {
            OpenView(applicationSelection.Selection);
        }

        public bool CanOpenViewFor(object dataObject)
        {
            return documentViewController.DocumentViewsResolver.GetViewInfosFor(dataObject).Any();
        }

        public void OpenView(object dataObject)
        {
            documentViewController.DocumentViewsResolver.OpenViewForData(dataObject);
        }

        public void RemoveAllViewsForItem(object dataObject)
        {
            if (dataObject == null || documentViewController.DocumentViews == null)
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