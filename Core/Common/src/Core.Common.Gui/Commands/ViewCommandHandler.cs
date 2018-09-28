// Copyright (C) Stichting Deltares 2018. All rights reserved.
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
        private readonly IPluginsHost pluginsHost;

        /// <summary>
        /// Initializes a new instance of the <see cref="ViewCommandHandler"/> class.
        /// </summary>
        /// <param name="viewController">The controller for views.</param>
        /// <param name="applicationSelection">The application selection mechanism.</param>
        /// <param name="pluginsHost">The plugins host.</param>
        public ViewCommandHandler(IViewController viewController, IApplicationSelection applicationSelection,
                                  IPluginsHost pluginsHost)
        {
            this.viewController = viewController;
            this.applicationSelection = applicationSelection;
            this.pluginsHost = pluginsHost;
        }

        public void OpenViewForSelection()
        {
            OpenView(applicationSelection.Selection);
        }

        public bool CanOpenViewFor(object viewData)
        {
            return viewController.DocumentViewController.GetViewInfosFor(viewData).Any();
        }

        public void OpenView(object viewData)
        {
            viewController.DocumentViewController.OpenViewForData(viewData);
        }

        public void RemoveAllViewsForItem(object viewData)
        {
            if (viewData == null || viewController.ViewHost == null)
            {
                return;
            }

            var objectsToRemoveViewsFor = new List<object>
            {
                viewData
            };

            objectsToRemoveViewsFor.AddRange(pluginsHost.GetAllDataWithViewDefinitionsRecursively(viewData).Cast<object>());

            foreach (object data in objectsToRemoveViewsFor)
            {
                viewController.DocumentViewController.CloseAllViewsFor(data);
            }
        }
    }
}