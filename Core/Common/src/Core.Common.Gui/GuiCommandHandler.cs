using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Base.IO;
using Core.Common.Base.Plugin;
using Core.Common.Base.Storage;
using Core.Common.Controls.Views;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Properties;
using Core.Common.Utils;
using log4net;
using log4net.Appender;
using UtilsResources = Core.Common.Utils.Properties.Resources;

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

        public void CreateNewProject()
        {
            CloseProject();

            Log.Info(Resources.Project_new_opening);
            gui.Project = new Project();
            Log.Info(Resources.Project_new_successfully_opened);

            RefreshGui();
        }

        /// <summary>
        /// Opens a new <see cref="OpenFileDialog"/> where a file can be selected to open.
        /// </summary>
        /// <returns><c>true</c> if an existing <see cref="Project"/> has been loaded, <c>false</c> otherwise.</returns>
        public bool OpenExistingProject()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Resources.Ringtoets_project_file_filter,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog(gui.MainWindow) != DialogResult.Cancel)
            {
                return OpenExistingProject(openFileDialog.FileName);
            }
            Log.Warn(Resources.Project_existing_project_opening_cancelled);
            return false;
        }

        /// <summary>
        /// Loads a <see cref="Project"/>, based upon <paramref name="filePath"/>.
        /// </summary>
        /// <param name="filePath">Location of the storage file.</param>
        /// <returns><c>true</c> if an existing <see cref="Project"/> has been loaded, <c>false</c> otherwise.</returns>
        public bool OpenExistingProject(string filePath)
        {
            Log.Info(Resources.Project_existing_opening_project);

            var storage = gui.Storage;
            Project loadedProject;
            try
            {
                loadedProject = storage.LoadProject(filePath);
            }
            catch (ArgumentException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }
            catch (CouldNotConnectException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }
            catch (StorageValidationException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_saving_project_failed);
                return false;
            }

            if (loadedProject == null)
            {
                Log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }

            // Project loaded successfully, close current project
            CloseProject();

            gui.ProjectFilePath = filePath;
            gui.Project = loadedProject;
            gui.Project.Name = Path.GetFileNameWithoutExtension(filePath);

            RefreshGui();
            Log.Info(Resources.Project_existing_successfully_opened);
            return true;
        }

        /// <summary>
        /// Close current project (if any) and related views.
        /// </summary>
        public void CloseProject()
        {
            if (gui.Project == null)
            {
                return;
            }

            // remove views before closing project. 
            RemoveAllViewsForItem(gui.Project);

            gui.Project = null;

            RefreshGui();
        }

        /// <summary>
        /// Saves the current <see cref="Project"/> to the selected storage file.
        /// </summary>
        /// <returns>Returns if the save was succesful.</returns>
        public bool SaveProjectAs()
        {
            var project = gui.Project;
            if (project == null)
            {
                return false;
            }

            Log.Info(Resources.Project_saving_project);
            // show file open dialog and select project file
            var saveFileDialog = new SaveFileDialog
            {
                Filter = string.Format(Resources.Ringtoets_project_file_filter),
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = string.Format("{0}", project.Name)
            };

            if (saveFileDialog.ShowDialog() != DialogResult.OK)
            {
                Log.Warn(Resources.Project_saving_project_cancelled);
                return false;
            }

            var filePath = saveFileDialog.FileName;
            var storage = gui.Storage;
            try
            {
                storage.SaveProjectAs(filePath, gui.Project);
            }
            catch (ArgumentException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_existing_project_opening_failed);
                return false;
            }
            catch (CouldNotConnectException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (StorageValidationException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (UpdateStorageException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_saving_project_failed);
                return false;
            }

            // Save was successful, store location
            gui.ProjectFilePath = filePath;
            project.Name = Path.GetFileNameWithoutExtension(filePath);
            Log.Info(Resources.Project_saving_project_saved);
            return true;
        }

        /// <summary>
        /// Saves the current <see cref="Project"/> to the defined storage file.
        /// </summary>
        /// <returns>Returns if the save was succesful.</returns>
        public bool SaveProject()
        {
            var project = gui.Project;
            if (project == null)
            {
                return false;
            }
            var filePath = gui.ProjectFilePath;

            // If filepath is not set, go to SaveAs
            if (string.IsNullOrWhiteSpace(filePath))
            {
                return SaveProjectAs();
            }

            Log.Info(Resources.Project_saving_project);
            var storage = gui.Storage;
            try
            {
                storage.SaveProject(filePath, gui.Project);
            }
            catch (ArgumentException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (CouldNotConnectException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (StorageValidationException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_saving_project_failed);
                return false;
            }
            catch (UpdateStorageException e)
            {
                Log.Warn(e.Message);
                Log.Warn(Resources.Project_saving_project_failed);
                return false;
            }

            Log.Info(Resources.Project_saving_project_saved);
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
                Log.Error(Resources.GuiCommandHandler_AddNewItem_There_needs_to_be_a_project_to_add_an_item);
            }

            using (var selectDataDialog = CreateSelectionDialogWithItems(gui.ApplicationCore.GetSupportedDataItemInfos(parent).ToList()))
            {
                if (selectDataDialog.ShowDialog() == DialogResult.OK)
                {
                    var newItem = GetNewItem(selectDataDialog, parent);

                    if (newItem != null)
                    {
                        gui.Selection = newItem;
                        OpenViewForSelection();
                    }
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
            foreach (var data in gui.GetAllDataWithViewDefinitionsRecursively(dataObject))
            {
                gui.DocumentViewsResolver.CloseAllViewsFor(data);
                RemoveViewsAndData(gui.ToolWindowViews.Where(v => v.Data == data).ToArray());
            }
        }

        public void Dispose()
        {
            gui.ProjectOpened -= ApplicationProjectOpened;
            gui.ProjectClosing -= ApplicationProjectClosing;
        }

        public void AddItemToProject(object newItem)
        {
            gui.Project.Items.Add(newItem);
            gui.Project.NotifyObservers();
        }

        public void UpdateObserver()
        {
            RefreshGui();
        }

        private GuiImportHandler CreateGuiImportHandler()
        {
            return new GuiImportHandler(gui);
        }

        private GuiExportHandler CreateGuiExportHandler()
        {
            return new GuiExportHandler(gui.MainWindow, o => gui.ApplicationCore.GetSupportedFileExporters(o), o => gui.DocumentViewsResolver.CreateViewForData(o));
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
        }

        private void ApplicationProjectOpened(Project project)
        {
            gui.Selection = project;

            project.Attach(this);
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
            var selectDataDialog = new SelectItemDialog(gui.MainWindow);

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
    }
}