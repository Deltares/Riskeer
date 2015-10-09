using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using DelftTools.Controls;
using DelftTools.Controls.Swf;
using DelftTools.Shell.Core;
using DelftTools.Shell.Gui;
using DelftTools.Utils;
using DelftTools.Utils.Aop;
using DelftTools.Utils.Collections;
using DeltaShell.Gui.Forms;
using DeltaShell.Gui.Forms.MainWindow;
using DeltaShell.Gui.Forms.ViewManager;
using DeltaShell.Gui.Properties;
using log4net;
using log4net.Appender;
using MessageBox = DelftTools.Controls.Swf.MessageBox;

namespace DeltaShell.Gui
{
    public class GuiCommandHandler : IGuiCommandHandler
    {
        private static readonly ILog Log = LogManager.GetLogger(typeof(GuiCommandHandler));

        private readonly GuiImportHandler guiImportHandler;
        private readonly GuiExportHandler guiExportHandler;
        private readonly IGui gui;

        public GuiCommandHandler(IGui gui)
        {
            this.gui = gui;

            this.gui.Application.ProjectOpened += ApplicationProjectOpened;
            this.gui.Application.ProjectClosing += ApplicationProjectClosing;

            guiImportHandler = CreateGuiImportHandler();
            guiExportHandler = CreateGuiExportHandler();
        }

        public void TryCreateNewWTIProject()
        {
            if (!TryCloseWTIProject())
            {
                Log.Warn(Resources.Opening_new_wti_project_cancelled);
                return;
            }

            Log.Info(Resources.Opening_new_wti_project);
            gui.Application.CreateNewProject();
            Log.Info(Resources.New_wti_project_successfully_opened);

            RefreshGui();
        }

        public bool TryOpenExistingWTIProject()
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = Resources.Wti_project_file_filter,
                FilterIndex = 1,
                RestoreDirectory = true
            };

            if (openFileDialog.ShowDialog() == DialogResult.Cancel)
            {
                Log.Warn(Resources.Opening_existing_wti_project_cancelled);
                return false;
            }

            Log.Info(Resources.Opening_existing_wti_project);

            return TryOpenExistingWTIProject(openFileDialog.FileName);
        }

        public bool TryOpenExistingWTIProject(string filePath)
        {
            if (!TryCloseWTIProject())
            {
                Log.Warn(Resources.Opening_existing_wti_project_cancelled);
                return false;
            }

            var result = false;

            ProgressBarDialog.PerformTask(Resources.Loading_wti_project_from_selected_file, () => result = gui.Application.OpenProject(filePath));

            RefreshGui();

            return result;
        }

        public bool TryCloseWTIProject()
        {
            if (gui.Application.Project != null)
            {
                Log.Info("Closing current WTI project.");

                // Ask to save any changes first
                if (gui.Application.Project.IsChanged)
                {
                    var result =
                        MessageBox.Show(string.Format(Resources.GuiCommandHandler_CloseProject_Save_changes_to_the_project___0_, gui.Application.Project.Name),
                                        Resources.GuiCommandHandler_CloseProject_Closing_project____, MessageBoxButtons.YesNoCancel);

                    if (result == DialogResult.Yes)
                    {
                        // Code below disabled as save/load plugin not implemented yet. Keep existing framework though.
                        var alwaysOkResult = MessageBox.Show("Not implemented yet.");
                        if (alwaysOkResult != DialogResult.OK)
                        {
                            if (!SaveProject()) {}
                        }
                    }

                    if (result == DialogResult.Cancel)
                    {
                        return false;
                    }
                }

                // Disconnect project
                ((INotifyPropertyChanged) gui.Application.Project).PropertyChanged -= CurrentProjectChanged;

                // DO NOT REMOVE CODE BELOW. If the views are not cleaned up here we access disposable stuff like in issue 4161.
                // SO VIEWS SHOULD ALWAYS BE CLOSED!
                // remove views before closing project. 
                if (!ViewList.DoNotDisposeViewsOnRemove)
                {
                    RemoveAllViewsForItem(gui.Application.Project);
                }

                gui.Application.CloseProject();

                RefreshGui();

                Log.Info(Resources.GuiCommandHandler_CloseProject_Project_closed);
            }

            return true;
        }

        public IProjectItem GetProjectItemForActiveView()
        {
            var activeView = gui.DocumentViews.ActiveView;
            if (activeView == null || activeView.Data == null)
            {
                return null;
            }

            var projectItemActiveView = activeView.Data as IProjectItem;
            if (projectItemActiveView != null)
            {
                return projectItemActiveView;
            }

            return null;
        }

        public bool SaveProject()
        {
            var project = gui.Application.Project;
            if (project == null)
            {
                return false;
            }

            if (project.IsTemporary)
            {
                return SaveProjectAs();
            }

            string path = null;

            UnselectActiveControlToForceBinding();

            SaveProjectWithProgressDialog(path);

            AddProjectToMruList();

            RefreshGui();

            return true;
        }

        /// <summary>
        /// Shows a dialog for saving the project to file.
        /// </summary>
        /// <returns></returns>
        public bool SaveProjectAs()
        {
            var project = gui.Application.Project;
            if (project == null)
            {
                return false;
            }

            UnselectActiveControlToForceBinding();

            // show file open dialog and select project file
            var saveFileDialog = new SaveFileDialog
            {
                Filter = string.Format(Resources.Wti_project_file_filter),
                FilterIndex = 1,
                RestoreDirectory = true,
                FileName = String.Format("{0}.wti", project.Name)
            };

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                try
                {
                    string path = saveFileDialog.FileName; // Constructs file path, might throw PathTooLongException

                    project.Name = Path.GetFileNameWithoutExtension(path);
                    SaveProjectWithProgressDialog(path);

                    Log.InfoFormat(Resources.GuiCommandHandler_SaveProjectAs_Project__0__saved, path);
                }
                catch (PathTooLongException)
                {
                    MessageBox.Show(Resources.GuiCommandHandler_SaveProjectAs_The_specified_file_path_is_too_long__please_choose_a_shorter_path_, Resources.GuiCommandHandler_SaveProjectAs_Warning, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }
            }
            else
            {
                return false;
            }

            AddProjectToMruList();

            RefreshGui();

            return true;
        }

        public void ShowProperties()
        {
            ((MainWindow) gui.MainWindow).InitPropertiesWindowAndActivate();
        }

        public bool CanImportToGuiSelection()
        {
            return guiImportHandler.GetImporters(gui.Selection).Any();
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
                Log.ErrorFormat(Resources.GuiCommandHandler_ImportOn_Unable_to_import_on__0__, gui.Selection);
            }
        }

        public void ImportToGuiSelection()
        {
            try
            {
                guiImportHandler.ImportDataTo(gui.Selection);
            }
            catch (Exception)
            {
                Log.ErrorFormat(Resources.GuiCommandHandler_ImportOn_Unable_to_import_on__0__, gui.Selection);
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

        public bool CanOpenDefaultViewForSelection()
        {
            return gui.DocumentViewsResolver.GetDefaultViewType(gui.Selection) != null;
        }

        public void OpenDefaultViewForSelection()
        {
            gui.DocumentViewsResolver.OpenViewForData(gui.Selection);
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
                MessageBox.Show(Resources.GuiCommandHandler_OpenLogFileExternal_Unable_to_open_log_file__opening_log_file_directory_instead_, Resources.GuiCommandHandler_OpenLogFileExternal_Unable_to_open_log_file);
                Process.Start(SettingsHelper.GetApplicationLocalUserSettingsDirectory());
            }
        }

        public object AddNewChildItem(object parent, IEnumerable<Type> childItemValueTypes)
        {
            var selectDataDialog = CreateSelectionDialogWithProjectItems(GetSupportedDataItemInfosByValueTypes(parent, childItemValueTypes).ToList());

            if (selectDataDialog.ShowDialog(gui.MainWindow as Form) == DialogResult.OK)
            {
                return GetNewDataObject(selectDataDialog, parent);
            }
            return null;
        }

        public object AddNewProjectItem(object parent)
        {
            if (gui.Application.Project == null)
            {
                Log.Error(Resources.GuiCommandHandler_AddNewProjectItem_There_needs_to_be_a_project_to_add_an_item);
                return null;
            }

            var selectDataDialog = CreateSelectionDialogWithProjectItems(GetSupportedDataItemInfos(parent).ToList());

            if (selectDataDialog.ShowDialog(gui.MainWindow as Form) == DialogResult.OK)
            {
                var newProjectItem = GetNewProjectItem(selectDataDialog, parent);

                if (newProjectItem != null)
                {
                    gui.Selection = newProjectItem;
                    OpenViewForSelection();
                }

                return newProjectItem;
            }

            return null;
        }

        /// <summary>
        /// Adds item to project rootfolder.
        /// </summary>
        /// <param name="item"></param>
        public void AddItemToProject(object item)
        {
            gui.Application.Project.Items.Add(item);
        }

        public void ExportSelectedItem()
        {
            var selectedObject = gui.Selection;
            if (selectedObject == null)
            {
                return;
            }

            guiExportHandler.ExportFrom(selectedObject);
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
            gui.Application.ProjectOpened -= ApplicationProjectOpened;
            gui.Application.ProjectClosing -= ApplicationProjectClosing;
        }

        private void SaveProjectWithProgressDialog(string path = null)
        {
            var actualPath = path ?? gui.Application.ProjectFilePath;

            ProgressBarDialog.PerformTask(string.Format(Resources.GuiCommandHandler_SaveProjectWithProgressDialog_Saving__0_, actualPath),
                                          () =>
                                          {
                                              if (path == null)
                                              {
                                                  gui.Application.SaveProject();
                                              }
                                              else
                                              {
                                                  gui.Application.SaveProjectAs(path);
                                              }
                                          });
        }

        private GuiImportHandler CreateGuiImportHandler()
        {
            return new GuiImportHandler(gui);
        }

        private GuiExportHandler CreateGuiExportHandler()
        {
            return new GuiExportHandler(delegate { return gui.Application.FileExporters; }, o => gui.DocumentViewsResolver.CreateViewForData(o));
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
            ((INotifyCollectionChange) project).CollectionChanged -= ProjectCollectionChanged;
        }

        private void ApplicationProjectOpened(Project project)
        {
            gui.Selection = project;

            // listen to all changes in project
            ((INotifyCollectionChange) project).CollectionChanged += ProjectCollectionChanged;
        }

        private void ProjectCollectionChanged(object sender, NotifyCollectionChangingEventArgs e)
        {
            // Don't remove views during run-time of models:
            if (gui.Application.ActivityRunner.IsRunning)
            {
                return;
            }
            if (e.Action == NotifyCollectionChangeAction.Remove)
            {
                RemoveAllViewsForItem(e.Item);
            }
            if (e.Action == NotifyCollectionChangeAction.Replace)
            {
                //throw new NotImplementedException();
            }
        }

        private void AddProjectToMruList()
        {
            var mruList = (StringCollection) Settings.Default["mruList"];
            if (mruList.Contains(gui.Application.ProjectFilePath))
            {
                mruList.Remove(gui.Application.ProjectFilePath);
            }

            mruList.Insert(0, gui.Application.ProjectFilePath);
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

        private IEnumerable<DataItemInfo> GetSupportedDataItemInfos(object parent)
        {
            return gui.Application.Plugins
                      .SelectMany(p => p.GetDataItemInfos())
                      .Where(dataItemInfo => dataItemInfo.AdditionalOwnerCheck == null || dataItemInfo.AdditionalOwnerCheck(parent));
        }

        private IEnumerable<DataItemInfo> GetSupportedDataItemInfosByValueTypes(object parent, IEnumerable<Type> valueTypes)
        {
            return GetSupportedDataItemInfos(parent).Where(dii => valueTypes.Contains(dii.ValueType));
        }

        private SelectItemDialog CreateSelectionDialogWithProjectItems(IList<DataItemInfo> dataItemInfos)
        {
            var selectDataDialog = new SelectItemDialog();

            foreach (var dataItemInfo in dataItemInfos)
            {
                selectDataDialog.AddItemType(dataItemInfo.Name, dataItemInfo.Category, dataItemInfo.Image, dataItemInfo);
            }

            if (dataItemInfos.Any())
            {
                selectDataDialog.ItemSupportsExample = name => dataItemInfos.First(i => i.Name == name).AddExampleData != null;
            }

            return selectDataDialog;
        }

        private object GetNewProjectItem(SelectItemDialog selectDataDialog, object parent)
        {
            var newDataObject = GetNewDataObject(selectDataDialog, parent);

            if (newDataObject == null)
            {
                return null;
            }

            AddProjectItemToProject(newDataObject);

            return newDataObject;
        }

        private static object GetNewDataObject(SelectItemDialog selectDataDialog, object parent = null)
        {
            var dataItemInfo = selectDataDialog.SelectedItemTag as DataItemInfo;
            if (dataItemInfo == null)
            {
                return null;
            }

            var newDataObject = dataItemInfo.CreateData != null ? dataItemInfo.CreateData(parent) : null;

            if (selectDataDialog.IsExample)
            {
                dataItemInfo.AddExampleData(newDataObject);
            }

            return newDataObject;
        }

        private void AddProjectItemToProject(object newItem)
        {
            gui.Application.Project.Items.Add(newItem);
            gui.Application.Project.NotifyObservers();
        }

        private void CurrentProjectChanged(object sender, PropertyChangedEventArgs e)
        {
            var project = (sender as Project);
            if (project == null)
            {
                return;
            }

            RefreshGui();
        }

        [InvokeRequired]
        private void RemoveViewsAndData(IEnumerable<IView> toolViews)
        {
            // set all tool windows where dataObject was used to null
            foreach (var view in toolViews)
            {
                view.Data = null;
            }
        }

        [InvokeRequired]
        private void RefreshGui()
        {
            var project = gui.Application.Project;

            // Set the gui selection to the current project
            gui.Selection = gui.Application.Project;

            var mainWindowTitle = gui.Application.Settings != null
                                      ? gui.Application.Settings["mainWindowTitle"]
                                      : "DeltaShell";

            if (project == null)
            {
                gui.MainWindow.Title = string.Format(Resources.GuiCommandHandler_UpdateGui__no_project_opened_____0_, mainWindowTitle);
                return;
            }
            gui.MainWindow.Title = project.Name + (project.IsChanged ? "*" : "") + " - " + mainWindowTitle;

            ((INotifyPropertyChanged) project).PropertyChanged -= CurrentProjectChanged;
            ((INotifyPropertyChanged) project).PropertyChanged += CurrentProjectChanged;
        }
    }
}