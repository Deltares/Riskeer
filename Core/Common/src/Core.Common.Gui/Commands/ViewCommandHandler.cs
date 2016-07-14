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
using Core.Common.Gui.Selection;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// This class provides concrete implementations for <see cref="IViewCommands"/>.
    /// </summary>
    public class ViewCommandHandler : IViewCommands
    {
        private readonly IViewController viewController;
        private readonly IApplicationSelection applicationSelection;
        private readonly IGuiPluginsHost guiPluginsHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCommandHandler"/> class.
        /// </summary>
        /// <param name="viewController">The controller for views.</param>
        /// <param name="applicationSelection">The application selection mechanism.</param>
        /// <param name="guiPluginsHost">The gui-plugins host.</param>
        public ViewCommandHandler(IViewController viewController, IApplicationSelection applicationSelection,
                                  IGuiPluginsHost guiPluginsHost)
        {
            this.viewController = viewController;
            this.applicationSelection = applicationSelection;
            this.guiPluginsHost = guiPluginsHost;
        }

        public void OpenViewForSelection()
        {
            OpenView(applicationSelection.Selection);
        }

        public bool CanOpenViewFor(object dataObject)
        {
            return viewController.DocumentViewController.GetViewInfosFor(dataObject).Any();
        }

        public void OpenView(object dataObject)
        {
            viewController.DocumentViewController.OpenViewForData(dataObject);
        }

        public void RemoveAllViewsForItem(object dataObject)
        {
            if (dataObject == null || viewController.ViewHost == null)
            {
                return;
            }

            var objectsToRemoveViewsFor = new List<object>
            {
                dataObject
            };

            objectsToRemoveViewsFor.AddRange(guiPluginsHost.GetAllDataWithViewDefinitionsRecursively(dataObject).Cast<object>());

            foreach (var data in objectsToRemoveViewsFor)
            {
                viewController.DocumentViewController.CloseAllViewsFor(data);
            }
        }
    }
}