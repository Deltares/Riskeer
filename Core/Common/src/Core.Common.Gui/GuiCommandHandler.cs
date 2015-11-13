using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using Core.Common.Base;
using Core.Common.Controls;
using Core.Common.Controls.Swf;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ViewManager;
using Core.Common.Gui.Properties;
using Core.Common.Utils;
using Core.Common.Utils.Aop;
using Core.Common.Utils.Collections;
using log4net;
using log4net.Appender;
using MainWindow = Core.Common.Gui.Forms.MainWindow.MainWindow;
using MessageBox = Core.Common.Controls.Swf.MessageBox;

namespace Core.Common.Gui
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

        public void TryCreateNewProject()
        {
            if (!TryCloseProject())
            {
                Log.Warn(Resources.Opening_new_project_cancelled);
                return;
            }

            Log.Info(Resources.Opening_new_project);
            gui.Application.CreateNewProject();
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

            ProgressBarDialog.PerformTask(Resources.Loading_project_from_selected_file, () => result = gui.Application.OpenProject(filePath));

            RefreshGui();

            return result;
        }

        public bool TryCloseProject()
        {
            if (gui.Application.Project != null)
            {
                Log.Info("Closing current project.");

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

        public object GetDataOfActiveView()
        {
            return gui.DocumentViews.ActiveView != null ? gui.DocumentViews.ActiveView.Data : null;
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
            var selectDataDialog = CreateSelectionDialogWithItems(GetSupportedDataItemInfosByValueTypes(parent, childItemValueTypes).ToList());

            if (selectDataDialog.ShowDialog(gui.MainWindow as Form) == DialogResult.OK)
            {
                return GetNewDataObject(selectDataDialog, parent);
            }
            return null;
        }

        public void AddNewItem(object parent)
        {
            if (gui.Application.Project == null)
            {
                Log.Error(Resources.GuiCommandHandler_AddNewItem_There_needs_to_be_a_project_to_add_an_item);
            }

            var selectDataDialog = CreateSelectionDialogWithItems(GetSupportedDataItemInfos(parent).ToList());

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
            var mruList = (StringCollection) Core.Common.Gui.Properties.Settings.Default["mruList"];
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

        private SelectItemDialog CreateSelectionDialogWithItems(IList<DataItemInfo> dataItemInfos)
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

            var newDataObject = dataItemInfo.CreateData != null ? dataItemInfo.CreateData(parent) : null;

            if (selectDataDialog.IsExample)
            {
                dataItemInfo.AddExampleData(newDataObject);
            }

            return newDataObject;
        }

        public void AddItemToProject(object newItem)
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
                                      : "Ringtoets";

            if (project == null)
            {
                gui.MainWindow.Title = string.Format(Resources.GuiCommandHandler_UpdateGui__no_project_opened_____0_, mainWindowTitle);
                return;
            }
            gui.MainWindow.Title = project.Name + " - " + mainWindowTitle;

            ((INotifyPropertyChanged) project).PropertyChanged -= CurrentProjectChanged;
            ((INotifyPropertyChanged) project).PropertyChanged += CurrentProjectChanged;
        }
    }
}