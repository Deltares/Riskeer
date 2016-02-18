// Copyright (C) Stichting Deltares 2016. All rights reserved.
//
// This file is part of Ringtoets.
//
// Ringtoets is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with this program. If not, see <http://www.gnu.org/licenses/>.
//
// All names, logos, and references to "Deltares" are registered trademarks of
// Stichting Deltares and remain full property of Stichting Deltares at all times.
// All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Plugin;
using Core.Common.IO.Exceptions;
using Core.Common.Utils;
using log4net;
using Ringtoets.Common.Data;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Placeholder;
using Ringtoets.HydraRing.Forms.PresentationObjects;
using Ringtoets.Integration.Data;
using Ringtoets.Integration.Data.Contribution;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using RingtoetsDataResources = Ringtoets.Integration.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using UtilsResources = Core.Common.Utils.Properties.Resources;

namespace Ringtoets.Integration.Plugin
{
    /// <summary>
    /// The GUI plugin for the Ringtoets application.
    /// </summary>
    public class RingtoetsGuiPlugin : GuiPlugin
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GuiPlugin));

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new RingtoetsRibbon();
            }
        }

        /// <summary>
        /// Returns all <see cref="PropertyInfo"/> instances provided for data of <see cref="RingtoetsGuiPlugin"/>.
        /// </summary>
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<AssessmentSectionBase, AssessmentSectionBaseProperties>();
            yield return new PropertyInfo<HydraulicBoundaryDatabaseContext, HydraulicBoundaryDatabaseProperties>();
        }

        /// <summary>
        /// Returns all <see cref="ViewInfo"/> instances provided for data of <see cref="RingtoetsGuiPlugin"/>.
        /// </summary>
        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<FailureMechanismContribution, FailureMechanismContributionView>
            {
                GetViewName = (v, o) => RingtoetsDataResources.FailureMechanismContribution_DisplayName,
                Image = RingtoetsFormsResources.GenericInputOutputIcon,
                CloseForData = (v, o) =>
                {
                    var assessmentSection = o as AssessmentSectionBase;
                    return assessmentSection != null && assessmentSection.FailureMechanismContribution == v.Data;
                }
            };
        }

        /// <summary>
        /// Gets the child data instances that have <see cref="ViewInfo"/> definitions of some parent data object.
        /// </summary>
        /// <param name="dataObject">The parent data object.</param>
        /// <returns>Sequence of child data.</returns>
        public override IEnumerable<object> GetChildDataWithViewDefinitions(object dataObject)
        {
            var assessmentSection = dataObject as AssessmentSectionBase;
            if (assessmentSection != null)
            {
                yield return assessmentSection.FailureMechanismContribution;
            }
        }

        /// <summary>
        /// Returns all <see cref="TreeNodeInfo"/> instances provided for data of <see cref="RingtoetsGuiPlugin"/>.
        /// </summary>
        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return new TreeNodeInfo<AssessmentSectionBase>
            {
                Text = assessmentSectionBase => assessmentSectionBase.Name,
                Image = assessmentSectionBase => RingtoetsFormsResources.AssessmentSectionFolderIcon,
                EnsureVisibleOnCreate = assessmentSectionBase => true,
                ChildNodeObjects = AssessmentSectionBaseChildNodeObjects,
                ContextMenuStrip = AssessmentSectionBaseContextMenuStrip,
                CanRename = (assessmentSectionBase, parentData) => true,
                OnNodeRenamed = AssessmentSectionBaseOnNodeRenamed,
                CanRemove = (assessmentSectionBase, parentNodeData) => true,
                OnNodeRemoved = AssessmentSectionBaseOnNodeRemoved
            };

            yield return new TreeNodeInfo<ReferenceLineContext>
            {
                Text = context => "Referentielijn",
                Image = context => RingtoetsFormsResources.ReferenceLineIcon,
                ForeColor = context => context.WrappedData == null ? 
                    Color.FromKnownColor(KnownColor.GrayText) : 
                    Color.FromKnownColor(KnownColor.ControlText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => 
                    Gui.Get(nodeData, treeViewControl).AddImportItem().Build()
            };

            yield return new TreeNodeInfo<FailureMechanismPlaceholder>
            {
                Text = failureMechanismPlaceholder => failureMechanismPlaceholder.Name,
                Image = failureMechanismPlaceholder => RingtoetsFormsResources.FailureMechanismIcon,
                ForeColor = failureMechanismPlaceholder => Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = FailureMechanismPlaceholderChildNodeObjects,
                ContextMenuStrip = FailureMechanismPlaceholderContextMenuStrip
            };

            yield return new TreeNodeInfo<PlaceholderWithReadonlyName>
            {
                Text = placeholderWithReadonlyName => placeholderWithReadonlyName.Name,
                Image = placeholderWithReadonlyName => GetIconForPlaceholder(placeholderWithReadonlyName),
                ForeColor = placeholderWithReadonlyName => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = PlaceholderWithReadonlyNameContextMenuStrip
            };

            yield return new TreeNodeInfo<CategoryTreeFolder>
            {
                Text = categoryTreeFolder => categoryTreeFolder.Name,
                Image = categoryTreeFolder => GetFolderIcon(categoryTreeFolder.Category),
                ChildNodeObjects = categoryTreeFolder => categoryTreeFolder.Contents.Cast<object>().ToArray(),
                ContextMenuStrip = CategoryTreeFolderContextMenu
            };

            yield return new TreeNodeInfo<FailureMechanismContribution>
            {
                Text = failureMechanismContribution => RingtoetsDataResources.FailureMechanismContribution_DisplayName,
                Image = failureMechanismContribution => RingtoetsFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (failureMechanismContribution, parentData, treeViewControl) => Gui.Get(failureMechanismContribution, treeViewControl)
                                                                                                     .AddOpenItem()
                                                                                                     .AddSeparator()
                                                                                                     .AddExportItem()
                                                                                                     .Build()
            };

            yield return new TreeNodeInfo<HydraulicBoundaryDatabaseContext>
            {
                Text = hydraulicBoundaryDatabase => RingtoetsDataResources.HydraulicBoundaryDatabase_DisplayName,
                Image = hydraulicBoundaryDatabase => RingtoetsFormsResources.GenericInputOutputIcon,
                CanRename = (context, o) => false,
                ContextMenuStrip = HydraulicBoundaryDatabaseContextMenuStrip
            };
        }

        # region AssessmentSectionBase

        private object[] AssessmentSectionBaseChildNodeObjects(AssessmentSectionBase nodeData)
        {
            var childNodes = new List<object>
            {
                new ReferenceLineContext(nodeData.ReferenceLine, nodeData),
                nodeData.FailureMechanismContribution,
                new HydraulicBoundaryDatabaseContext (nodeData.HydraulicBoundaryDatabase, nodeData)
            };

            childNodes.AddRange(nodeData.GetFailureMechanisms());

            return childNodes.ToArray();
        }

        private void AssessmentSectionBaseOnNodeRenamed(AssessmentSectionBase nodeData, string newName)
        {
            nodeData.Name = newName;
            nodeData.NotifyObservers();
        }

        private void AssessmentSectionBaseOnNodeRemoved(AssessmentSectionBase nodeData, object parentNodeData)
        {
            var parentProject = (Project) parentNodeData;

            parentProject.Items.Remove(nodeData);
            parentProject.NotifyObservers();
        }

        private ContextMenuStrip AssessmentSectionBaseContextMenuStrip(AssessmentSectionBase nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddRenameItem()
                      .AddDeleteItem()
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        # endregion

        # region FailureMechanismPlaceholder

        private object[] FailureMechanismPlaceholderChildNodeObjects(FailureMechanismPlaceholder nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(nodeData),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(nodeData),
                                       TreeFolderCategory.Output)
            };
        }

        private IList GetInputs(FailureMechanismPlaceholder nodeData)
        {
            return new ArrayList
            {
                nodeData.SectionDivisions,
                nodeData.Locations,
                nodeData.BoundaryConditions
            };
        }

        private IList GetOutputs(FailureMechanismPlaceholder nodeData)
        {
            return new ArrayList
            {
                nodeData.AssessmentResult
            };
        }

        private ContextMenuStrip FailureMechanismPlaceholderContextMenuStrip(FailureMechanismPlaceholder nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var calculateItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                RingtoetsCommonFormsResources.Calculate_all_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                null)
            {
                Enabled = false
            };
            var clearOutputItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Clear_all_output,
                RingtoetsCommonFormsResources.Clear_all_output_ToolTip,
                RingtoetsCommonFormsResources.ClearIcon, null
                )
            {
                Enabled = false
            };

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(calculateItem)
                      .AddCustomItem(clearOutputItem)
                      .AddSeparator()
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        # endregion

        # region PlaceholderWithReadonlyName

        private static Bitmap GetIconForPlaceholder(PlaceholderWithReadonlyName nodeData)
        {
            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                return RingtoetsFormsResources.GenericInputOutputIcon;
            }
            return RingtoetsFormsResources.PlaceholderIcon;
        }

        private ContextMenuStrip PlaceholderWithReadonlyNameContextMenuStrip(PlaceholderWithReadonlyName nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IContextMenuBuilder menuBuilder = Gui.Get(nodeData, treeViewControl);

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                menuBuilder.AddOpenItem();
            }

            if (nodeData is OutputPlaceholder)
            {
                var clearItem = new StrictContextMenuItem(
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase,
                    RingtoetsCommonFormsResources.FailureMechanism_InputsOutputs_Erase_ToolTip,
                    RingtoetsCommonFormsResources.ClearIcon,
                    null)
                {
                    Enabled = false
                };

                menuBuilder.AddCustomItem(clearItem);
            }

            if (nodeData is InputPlaceholder || nodeData is OutputPlaceholder)
            {
                menuBuilder.AddSeparator();
            }
            return menuBuilder.AddImportItem()
                              .AddExportItem()
                              .AddSeparator()
                              .AddPropertiesItem()
                              .Build();
        }

        # endregion

        # region CategoryTreeFolder

        private Image GetFolderIcon(TreeFolderCategory category)
        {
            switch (category)
            {
                case TreeFolderCategory.General:
                    return RingtoetsCommonFormsResources.GeneralFolderIcon;
                case TreeFolderCategory.Input:
                    return RingtoetsCommonFormsResources.InputFolderIcon;
                case TreeFolderCategory.Output:
                    return RingtoetsCommonFormsResources.OutputFolderIcon;
                default:
                    throw new NotImplementedException();
            }
        }

        private ContextMenuStrip CategoryTreeFolderContextMenu(CategoryTreeFolder nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddExpandAllItem()
                      .AddCollapseAllItem()
                      .Build();
        }

        # endregion

        #region HydraulicBoundaryDatabase

        private ContextMenuStrip HydraulicBoundaryDatabaseContextMenuStrip(HydraulicBoundaryDatabaseContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var connectionItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Connect,
                RingtoetsCommonFormsResources.HydraulicBoundaryDatabase_Connect_ToolTip,
                RingtoetsCommonFormsResources.DatabaseIcon, (sender, args) => SelectDatabaseFile(nodeData));

            var toetsPeilItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Toetspeil_Calculate,
                RingtoetsCommonFormsResources.Toetspeil_Calculate_ToolTip,
                GetFolderIcon(TreeFolderCategory.General), null);

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(connectionItem)
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddCustomItem(toetsPeilItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        private void SelectDatabaseFile(HydraulicBoundaryDatabaseContext nodeData)
        {
            var windowTitle = RingtoetsCommonFormsResources.SelectDatabaseFile_Title;
            using (var dialog = new OpenFileDialog
            {
                Filter = string.Format("{0} (*.sqlite)|*.sqlite", RingtoetsCommonFormsResources.SelectDatabaseFile_FilterName),
                Multiselect = false,
                Title = windowTitle,
                RestoreDirectory = true,
                CheckFileExists = true,
            })
            {
                if (dialog.ShowDialog(Gui.MainWindow) == DialogResult.OK)
                {
                    ValidateSelectedFile(nodeData, dialog.FileName);
                }
            }
        }

        private static void ValidateSelectedFile(HydraulicBoundaryDatabaseContext nodeData, string selectedFile)
        {
            try
            {
                if (!string.IsNullOrEmpty(nodeData.BoundaryDatabase.FilePath))
                {
                    // Compare
                    bool isEqual = FileUtils.CompareFiles(nodeData.BoundaryDatabase.FilePath, selectedFile);

                    if (!isEqual)
                    {
                        // show dialog
                        ShowCleanDialog(nodeData, selectedFile);
                        return;
                    }
                }

                SetBoundaryDatabaseFilePath(nodeData, selectedFile);
            }
            catch (Exception e)
            {
                throw new CriticalFileReadException(string.Format(UtilsResources.Error_General_IO_ErrorMessage_0_, selectedFile), e);
            }
        }

        private static void ShowCleanDialog(HydraulicBoundaryDatabaseContext nodeData, string filePath)
        {
            var confirmation = MessageBox.Show(
                RingtoetsCommonFormsResources.Delete_ToetsPeil_Calculations_Text,
                RingtoetsCommonFormsResources.Delete_ToetsPeil_Calculations_Title,
                MessageBoxButtons.YesNo);

            if (confirmation == DialogResult.Yes)
            {
                ClearCalculations(nodeData.BaseNode);

                SetBoundaryDatabaseFilePath(nodeData, filePath);
            }
        }

        private static void ClearCalculations(AssessmentSectionBase nodeData)
        {
            var failureMechanisms = nodeData.GetFailureMechanisms();

            foreach (ICalculationItem calc in failureMechanisms.SelectMany(fm => fm.CalculationItems))
            {
                calc.ClearOutput();
                calc.NotifyObservers();
            }
        }

        private static void SetBoundaryDatabaseFilePath(HydraulicBoundaryDatabaseContext nodeData, string selectedFile)
        {
            nodeData.BoundaryDatabase.FilePath = selectedFile;
            nodeData.NotifyObservers();
            log.InfoFormat(RingtoetsCommonFormsResources.RingtoetsGuiPlugin_SetBoundaryDatabaseFilePath_Database_on_path__0__linked, selectedFile);
        }

        #endregion
    }
}