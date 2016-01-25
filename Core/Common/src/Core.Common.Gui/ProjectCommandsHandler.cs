using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

using Core.Common.Base.Plugin;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Properties;

using log4net;

namespace Core.Common.Gui
{
    /// <summary>
    /// This class provides concrete implementation for <see cref="IProjectCommands"/>;
    /// </summary>
    public class ProjectCommandsHandler : IProjectCommands
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ProjectCommandsHandler));
        private readonly IGui gui;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectCommandsHandler"/> class.
        /// </summary>
        /// <param name="gui">The GUI.</param>
        public ProjectCommandsHandler(IGui gui)
        {
            this.gui = gui;
        }

        public object AddNewChildItem(object parent, IEnumerable<Type> childItemValueTypes)
        {
            using (var selectDataDialog = CreateSelectionDialogWithItems(GetSupportedDataItemInfosByValueTypes(parent, childItemValueTypes).ToList()))
            {
                if (selectDataDialog.ShowDialog() == DialogResult.OK)
                {
                    return GetNewDataObject(selectDataDialog, parent);
                }
                return null;
            }
        }

        public void AddNewItem(object parent)
        {
            if (gui.Project == null)
            {
                log.Error(Resources.GuiCommandHandler_AddNewItem_There_needs_to_be_a_project_to_add_an_item);
            }

            using (var selectDataDialog = CreateSelectionDialogWithItems(gui.ApplicationCore.GetSupportedDataItemInfos(parent).ToList()))
            {
                if (selectDataDialog.ShowDialog() == DialogResult.OK)
                {
                    var newItem = GetNewDataObject(selectDataDialog, parent);
                    if (newItem != null)
                    {
                        AddItemToProject(newItem);

                        gui.Selection = newItem;
                        gui.DocumentViewsResolver.OpenViewForData(gui.Selection);
                    }
                }
            }
        }

        public void AddItemToProject(object newItem)
        {
            gui.Project.Items.Add(newItem);
            gui.Project.NotifyObservers();
        }

        private IEnumerable<DataItemInfo> GetSupportedDataItemInfosByValueTypes(object parent, IEnumerable<Type> valueTypes)
        {
            return gui.ApplicationCore.GetSupportedDataItemInfos(parent).Where(dii => valueTypes.Contains(dii.ValueType));
        }

        private SelectItemDialog CreateSelectionDialogWithItems(IList<DataItemInfo> dataItemInfos)
        {
            var selectDataDialog = new SelectItemDialog(gui.MainWindow);

            foreach (var dataItemInfo in dataItemInfos)
            {
                selectDataDialog.AddItemType(dataItemInfo.Name, dataItemInfo.Category, dataItemInfo.Image, dataItemInfo);
            }

            return selectDataDialog;
        }

        private static object GetNewDataObject(SelectItemDialog selectDataDialog, object parent)
        {
            var dataItemInfo = selectDataDialog.SelectedItemTag as DataItemInfo;
            if (dataItemInfo == null)
            {
                return null;
            }

            return dataItemInfo.CreateData != null ? dataItemInfo.CreateData(parent) : null;
        }
    }
}