using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Properties;
using Core.Common.Utils;
using Core.Common.Utils.Collections;
using log4net;
using log4net.Appender;
using MainWindow = Core.Common.Gui.Forms.MainWindow.MainWindow;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Core.Common.Gui
{
    public class GuiCommandHandler : IGuiCommandHandler, IObserver
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GuiCommandHandler));

        private readonly GuiImportHandler guiImportHandler;
        private readonly GuiExportHandler guiExportHandler;
        private readonly IGui gui;

        public GuiCommandHandler(IGui gui)
        {
            this.gui = gui;

            this.gui.ProjectOpened += ApplicationProjectOpened;
            this.gui.ProjectClosing += ApplicationProjectClosing;

            guiImportHandler = CreateGuiImportHandler();
            guiExportHandler = CreateGuiExportHandler();
        }

        public void TryCreateNewProject()
        {
            if (!TryCloseProject())
            {
                Log.Warn(Resources.Opening_new_project_cancelled);
                return;
            }

            Log.Info(Resources.Opening_new_project);
            gui.Project = new Project();
            Log.Info(Resources.New_project_successfully_opened);

            RefreshGui();
        }

        public bool TryOpenExistingProject()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Resources.Ringtoets_project_file_filter,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                Log.Warn(Resources.Opening_existing_project_cancelled);
                return false;
            }

            Log.Info(Resources.Opening_existing_project);

            return TryOpenExistingProject(openFileDialog.FileName);
        }

        public bool TryOpenExistingProject(string filePath)
        {
            if (!TryCloseProject())
            {
                Log.Warn(Resources.Opening_existing_project_cancelled);
                return false;
            }

            var result = false;

            // TODO: Implement logic for opening the project from the provided file path
            ProgressBarDialog.PerformTask(Resources.Loading_project_from_selected_file, () => gui.Project = new Project());

            RefreshGui();

            return result;
        }

        public bool TryCloseProject()
        {
            if (gui.Project != null)
            {
                Log.Info(Resources.GuiCommandHandler_TryCloseProject_Closing_current_project);

                // DO NOT REMOVE CODE BELOW. If the views are not cleaned up here we access disposable stuff like in issue 4161.
                // SO VIEWS SHOULD ALWAYS BE CLOSED!
                // remove views before closing project. 
                if (!ViewList.DoNotDisposeViewsOnRemove)
                {
                    RemoveAllViewsForItem(gui.Project);
                }

                gui.Project = null;

                RefreshGui();

                Log.Info(Resources.GuiCommandHandler_CloseProject_Project_closed);
            }

            return true;
        }

        public object GetDataOfActiveView()
        {
            return gui.DocumentViews.ActiveView != null ? gui.DocumentViews.ActiveView.Data : null;
        }

        /// <summary>
        /// Makes the properties window visible and updates the <see cref="IGui.Selection"/> to the
        /// given <paramref name="obj"/>.
        /// </summary>
        /// <param name="obj">The object for which to show its properties.</param>
        public void ShowPropertiesFor(object obj)
        {
            ((MainWindow) gui.MainWindow).InitPropertiesWindowAndActivate();
            gui.Selection = obj;
        }

        public bool CanImportOn(object obj)
        {
            return gui.ApplicationCore.GetSupportedFileImporters(obj).Any();
        }

        public bool CanExportFrom(object obj)
        {
            return gui.ApplicationCore.GetSupportedFileExporters(obj).Any();
        }

        public bool CanShowPropertiesFor(object obj)
        {
            return gui.PropertyResolver.GetObjectProperties(obj) != null;
        }

        public void ImportOn(object target, IFileImporter importer = null)
        {
            try
            {
                if (importer == null)
                {
                    guiImportHandler.ImportDataTo(target);
                }
                else
                {
                    guiImportHandler.ImportUsingImporter(importer, target);
                }
            }
            catch (Exception)
            {
                Log.ErrorFormat(Resources.GuiCommandHandler_ImportOn_Unable_to_import_on_0_, target);
            }
        }

        public bool CanOpenSelectViewDialog()
        {
            return gui.Selection != null && gui.DocumentViewsResolver.GetViewInfosFor(gui.Selection).Count() > 1;
        }

        public void OpenSelectViewDialog()
        {
            gui.DocumentViewsResolver.OpenViewForData(gui.Selection, null, true);
        }

        public bool CanOpenDefaultViewFor(object obj)
        {
            return gui.DocumentViewsResolver.GetDefaultViewType(obj) != null;
        }

        public void OpenView(object dataObject, Type viewType = null)
        {
            gui.DocumentViewsResolver.OpenViewForData(dataObject, viewType);
        }

        public void OpenViewForSelection(Type viewType = null)
        {
            gui.DocumentViewsResolver.OpenViewForData(gui.Selection, viewType);
        }

        public void OpenLogFileExternal()
        {
            bool logFileOpened = false;

            try
            {
                var fileAppender =
                    LogManager.GetAllRepositories().SelectMany(r => r.GetAppenders()).OfType
                        <FileAppender>().FirstOrDefault();
                if (fileAppender != null)
                {
                    var logFile = fileAppender.File;
                    Process.Start(logFile);
                    logFileOpened = true;
                }
            }
            catch (Exception) {}

            if (!logFileOpened)
            {
                MessageBox.Show(Resources.GuiCommandHandler_OpenLogFileExternal_Unable_to_open_log_file_Opening_log_file_directory_instead, Resources.GuiCommandHandler_OpenLogFileExternal_Unable_to_open_log_file);
                Process.Start(SettingsHelper.GetApplicationLocalUserSettingsDirectory());
            }
        }

        public object AddNewChildItem(object parent, IEnumerable<Type> childItemValueTypes)
        {
            var selectDataDialog = CreateSelectionDialogWithItems(GetSupportedDataItemInfosByValueTypes(parent, childItemValueTypes).ToList());

            if (selectDataDialog.ShowDialog(gui.MainWindow as Form) == DialogResult.OK)
            {
                return GetNewDataObject(selectDataDialog, parent);
            }
            return null;
        }

        public void AddNewItem(object parent)
        {
            if (gui.Project == null)
            {
                Log.Error(Resources.GuiCommandHandler_AddNewItem_There_needs_to_be_a_project_to_add_an_item);
            }

            var selectDataDialog = CreateSelectionDialogWithItems(gui.ApplicationCore.GetSupportedDataItemInfos(parent).ToList());

            if (selectDataDialog.ShowDialog(gui.MainWindow as Form) == DialogResult.OK)
            {
                var newItem = GetNewItem(selectDataDialog, parent);

                if (newItem != null)
                {
                    gui.Selection = newItem;
                    OpenViewForSelection();
                }
            }
        }

        public void ExportFrom(object data, IFileExporter exporter = null)
        {
            if (exporter == null)
            {
                guiExportHandler.ExportFrom(data);
            }
            else
            {
                guiExportHandler.GetExporterDialog(exporter, data);
            }
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

            gui.DocumentViewsResolver.CloseAllViewsFor(dataObject);
            RemoveViewsAndData(gui.ToolWindowViews.Where(v => v.Data == dataObject));
        }

        public void Dispose()
        {
            gui.ProjectOpened -= ApplicationProjectOpened;
            gui.ProjectClosing -= ApplicationProjectClosing;
        }

        private GuiImportHandler CreateGuiImportHandler()
        {
            return new GuiImportHandler(gui);
        }

        private GuiExportHandler CreateGuiExportHandler()
        {
            return new GuiExportHandler(o => gui.ApplicationCore.GetSupportedFileExporters(o), o => gui.DocumentViewsResolver.CreateViewForData(o));
        }

        private void ApplicationProjectClosing(Project project)
        {
            // clean all views
            if (gui.DocumentViews != null)
            {
                RemoveAllViewsForItem(project);
            }

            if (gui.ToolWindowViews != null)
            {
                foreach (IView view in gui.ToolWindowViews)
                {
                    view.Data = null;
                }
            }

            project.Detach(this);
            project.Items.CollectionChanged -= ProjectCollectionChanged;
        }

        private void ApplicationProjectOpened(Project project)
        {
            gui.Selection = project;

            project.Attach(this);
            project.Items.CollectionChanged += ProjectCollectionChanged;
        }

        private void ProjectCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            if (e.Action == NotifyCollectionChangeAction.Remove)
            {
                RemoveAllViewsForItem(e.Item);
            }
        }

        private void AddProjectToMruList()
        {
            var mruList = (StringCollection) Settings.Default["mruList"];
            if (mruList.Contains(gui.ProjectFilePath))
            {
                mruList.Remove(gui.ProjectFilePath);
            }

            mruList.Insert(0, gui.ProjectFilePath);
        }

        private static void UnselectActiveControlToForceBinding()
        {
            var elementWithFocus = Keyboard.FocusedElement;

            if (elementWithFocus is WindowsFormsHost)
            {
                var host = (WindowsFormsHost) elementWithFocus;
                ControlHelper.UnfocusActiveControl(host.Child as IContainerControl, true);
            }
            // add more if more cases are found (like a pure wpf case, for example)
        }

        private IEnumerable<DataItemInfo> GetSupportedDataItemInfosByValueTypes(object parent, IEnumerable<Type> valueTypes)
        {
            return gui.ApplicationCore.GetSupportedDataItemInfos(parent).Where(dii => valueTypes.Contains(dii.ValueType));
        }

        private SelectItemDialog CreateSelectionDialogWithItems(IList<DataItemInfo> dataItemInfos)
        {
            var selectDataDialog = new SelectItemDialog();

            foreach (var dataItemInfo in dataItemInfos)
            {
                selectDataDialog.AddItemType(dataItemInfo.Name, dataItemInfo.Category, dataItemInfo.Image, dataItemInfo);
            }

            return selectDataDialog;
        }

        private object GetNewItem(SelectItemDialog selectDataDialog, object parent)
        {
            var newDataObject = GetNewDataObject(selectDataDialog, parent);

            if (newDataObject == null)
            {
                return null;
            }

            AddItemToProject(newDataObject);

            return newDataObject;
        }

        private static object GetNewDataObject(SelectItemDialog selectDataDialog, object parent = null)
        {
            var dataItemInfo = selectDataDialog.SelectedItemTag as DataItemInfo;
            if (dataItemInfo == null)
            {
                return null;
            }

            return dataItemInfo.CreateData != null ? dataItemInfo.CreateData(parent) : null;
        }

        public void AddItemToProject(object newItem)
        {
            gui.Project.Items.Add(newItem);
            gui.Project.NotifyObservers();
        }

        private void RemoveViewsAndData(IEnumerable<IView> toolViews)
        {
            // set all tool windows where dataObject was used to null
            foreach (var view in toolViews)
            {
                view.Data = null;
            }
        }

        private void RefreshGui()
        {
            // Set the gui selection to the current project
            gui.Selection = gui.Project;

            // Update the window title
            gui.UpdateTitle();
        }

        public void UpdateObserver()
        {
            RefreshGui();
        }
    }
}