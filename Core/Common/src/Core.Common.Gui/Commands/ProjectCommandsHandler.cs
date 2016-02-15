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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.Data;
using Core.Common.Base.Plugin;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Properties;
using Core.Common.Gui.Selection;

using log4net;

namespace Core.Common.Gui.Commands
{
    /// <summary>
    /// This class provides concrete implementation for <see cref="IProjectCommands"/>.
    /// </summary>
    public class ProjectCommandsHandler : IProjectCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectCommandsHandler));

        private readonly IProjectOwner projectOwner;
        private readonly IWin32Window dialogOwner;
        private readonly ApplicationCore applicationCore;
        private readonly IApplicationSelection applicationSelection;
        private readonly IDocumentViewController documentViewController;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCommandsHandler"/> class.
        /// </summary>
        /// <param name="projectOwner">Class owning the application's <see cref="Project"/> instance.</param>
        /// <param name="dialogParent">The window on which dialogs should be shown on top.</param>
        /// <param name="applicationCore">The application-plugins host.</param>
        /// <param name="applicationSelection">The application selection mechanism.</param>
        /// <param name="documentViewController">The controller for Document Views.</param>
        public ProjectCommandsHandler(IProjectOwner projectOwner, IWin32Window dialogParent,
                                      ApplicationCore applicationCore, IApplicationSelection applicationSelection,
                                      IDocumentViewController documentViewController)
        {
            this.projectOwner = projectOwner;
            dialogOwner = dialogParent;
            this.applicationCore = applicationCore;
            this.applicationSelection = applicationSelection;
            this.documentViewController = documentViewController;
        }

        public object AddNewChildItem(object parent, IEnumerable<Type> childItemValueTypes)
        {
            using (var selectDataDialog = CreateSelectionDialogWithItems(GetSupportedDataItemInfosByValueTypes(parent, childItemValueTypes).ToArray()))
            {
                if (selectDataDialog.ShowDialog() == DialogResult.OK)
                {
                    return GetNewDataObject(parent, selectDataDialog.SelectedItemTag as DataItemInfo);
                }
                return null;
            }
        }

        public void AddNewItem(object parent)
        {
            if (projectOwner.Project == null)
            {
                log.Error(Resources.ProjectCommandHandler_AddNewItem_There_needs_to_be_a_project_to_add_an_item);
            }

            using (var selectDataDialog = CreateSelectionDialogWithItems(applicationCore.GetSupportedDataItemInfos(parent).ToArray()))
            {
                if (selectDataDialog.ShowDialog() == DialogResult.OK)
                {
                    var newItem = GetNewDataObject(parent, selectDataDialog.SelectedItemTag as DataItemInfo);
                    if (newItem != null)
                    {
                        AddItemToProject(newItem);

                        applicationSelection.Selection = newItem;
                        documentViewController.DocumentViewsResolver.OpenViewForData(applicationSelection.Selection);
                    }
                }
            }
        }

        public void AddItemToProject(object newItem)
        {
            projectOwner.Project.Items.Add(newItem);
            projectOwner.Project.NotifyObservers();
        }

        private IEnumerable<DataItemInfo> GetSupportedDataItemInfosByValueTypes(object parent, IEnumerable<Type> valueTypes)
        {
            return applicationCore.GetSupportedDataItemInfos(parent).Where(dii => valueTypes.Contains(dii.ValueType));
        }

        private SelectItemDialog CreateSelectionDialogWithItems(IEnumerable<DataItemInfo> dataItemInfos)
        {
            var selectDataDialog = new SelectItemDialog(dialogOwner);

            foreach (var dataItemInfo in dataItemInfos)
            {
                selectDataDialog.AddItemType(dataItemInfo.Name, dataItemInfo.Category, dataItemInfo.Image, dataItemInfo);
            }

            return selectDataDialog;
        }

        private static object GetNewDataObject(object parent, DataItemInfo dataItemInfo)
        {
            if (dataItemInfo == null)
            {
                return null;
            }

            return dataItemInfo.CreateData != null ? dataItemInfo.CreateData(parent) : null;
        }
    }
}