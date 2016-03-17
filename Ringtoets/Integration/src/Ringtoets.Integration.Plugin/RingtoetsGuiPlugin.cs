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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.IO.Exceptions;
using log4net;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Placeholder;
using Ringtoets.HydraRing.Calculation.Activities;
using Ringtoets.HydraRing.Calculation.Data;
using Ringtoets.HydraRing.Calculation.Data.Input.Hydraulics;
using Ringtoets.HydraRing.Calculation.Data.Output;
using Ringtoets.HydraRing.Data;
using Ringtoets.Integration.Data.Placeholders;
using Ringtoets.Integration.Forms.PresentationObjects;
using Ringtoets.Integration.Forms.PropertyClasses;
using Ringtoets.Integration.Forms.Views;
using Ringtoets.Integration.Plugin.FileImporters;
using Ringtoets.Integration.Plugin.Properties;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms.PresentationObjects;
using RingtoetsDataResources = Ringtoets.Integration.Data.Properties.Resources;
using RingtoetsFormsResources = Ringtoets.Integration.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using UtilsResources = Core.Common.Utils.Properties.Resources;
using BaseResources = Core.Common.Base.Properties.Resources;

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
        /// Returns all <see cref="Core.Common.Gui.Plugin.PropertyInfo"/> instances provided for data of <see cref="RingtoetsGuiPlugin"/>.
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

            yield return new ViewInfo<AssessmentSectionBase, AssessmentSectionView>
            {
                GetViewName = (v, o) => RingtoetsFormsResources.AssessmentSectionMap_DisplayName,
                Image = RingtoetsFormsResources.Map
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
                Text = context => RingtoetsCommonDataResources.ReferenceLine_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ReferenceLineIcon,
                ForeColor = context => context.WrappedData == null ?
                                           Color.FromKnownColor(KnownColor.GrayText) :
                                           Color.FromKnownColor(KnownColor.ControlText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) =>
                                   Gui.Get(nodeData, treeViewControl).AddImportItem().Build()
            };

            yield return new TreeNodeInfo<FailureMechanismPlaceholderContext>
            {
                Text = failureMechanismPlaceholder => failureMechanismPlaceholder.WrappedData.Name,
                Image = failureMechanismPlaceholder => RingtoetsFormsResources.FailureMechanismIcon,
                ForeColor = failureMechanismPlaceholder => Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = FailureMechanismPlaceholderChildNodeObjects,
                ContextMenuStrip = FailureMechanismPlaceholderContextMenuStrip
            };

            yield return new TreeNodeInfo<FailureMechanismSectionsContext>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_Sections_DisplayName,
                Image = context => RingtoetsCommonFormsResources.Sections,
                ForeColor = context => context.WrappedData.Any() ?
                                           Color.FromKnownColor(KnownColor.ControlText) :
                                           Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = FailureMechanismSectionsContextMenuStrip
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
                Text = hydraulicBoundaryDatabase => RingtoetsFormsResources.HydraulicBoundaryDatabase_DisplayName,
                Image = hydraulicBoundaryDatabase => RingtoetsFormsResources.GenericInputOutputIcon,
                CanRename = (context, o) => false,
                ForeColor = context => context.Parent.HydraulicBoundaryDatabase == null ?
                                           Color.FromKnownColor(KnownColor.GrayText) :
                                           Color.FromKnownColor(KnownColor.ControlText),
                ContextMenuStrip = HydraulicBoundaryDatabaseContextMenuStrip
            };
        }

        #region FailureMechanismSectionsContext

        private ContextMenuStrip FailureMechanismSectionsContextMenuStrip(FailureMechanismSectionsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddImportItem()
                      .Build();
        }

        #endregion

        # region AssessmentSectionBase

        private object[] AssessmentSectionBaseChildNodeObjects(AssessmentSectionBase nodeData)
        {
            var childNodes = new List<object>
            {
                new ReferenceLineContext(nodeData),
                nodeData.FailureMechanismContribution,
                new HydraulicBoundaryDatabaseContext(nodeData)
            };

            var failureMechanismContexts = WrapFailureMechanismsInContexts(nodeData);
            childNodes.AddRange(failureMechanismContexts);

            return childNodes.ToArray();
        }

        private static IEnumerable<object> WrapFailureMechanismsInContexts(AssessmentSectionBase nodeData)
        {
            foreach (IFailureMechanism failureMechanism in nodeData.GetFailureMechanisms())
            {
                var placeHolder = failureMechanism as FailureMechanismPlaceholder;
                var piping = failureMechanism as PipingFailureMechanism;
                if (placeHolder != null)
                {
                    yield return new FailureMechanismPlaceholderContext(placeHolder, nodeData);
                }
                else if (piping != null)
                {
                    yield return new PipingFailureMechanismContext(piping, nodeData);
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
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
                      .AddOpenItem()
                      .AddSeparator()
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

        # region FailureMechanismPlaceHolderContext

        private object[] FailureMechanismPlaceholderChildNodeObjects(FailureMechanismPlaceholderContext nodeData)
        {
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(nodeData.WrappedData, nodeData.Parent),
                                       TreeFolderCategory.Input),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(nodeData.WrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private IList GetInputs(FailureMechanismPlaceholder nodeData, AssessmentSectionBase assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(nodeData, assessmentSection),
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

        private ContextMenuStrip FailureMechanismPlaceholderContextMenuStrip(FailureMechanismPlaceholderContext nodeData, object parentData, TreeViewControl treeViewControl)
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
                RingtoetsFormsResources.HydraulicBoundaryDatabase_Connect,
                RingtoetsFormsResources.HydraulicBoundaryDatabase_Connect_ToolTip,
                RingtoetsCommonFormsResources.DatabaseIcon, (sender, args) => { SelectDatabaseFile(nodeData); });

            var designWaterLevelItem = new StrictContextMenuItem(
                RingtoetsFormsResources.DesignWaterLevel_Calculate,
                RingtoetsFormsResources.DesignWaterLevel_Calculate_ToolTip,
                RingtoetsFormsResources.FailureMechanismIcon,
                (sender, args) =>
                {
                    var hlcdDirectory = Path.GetDirectoryName(nodeData.Parent.HydraulicBoundaryDatabase.FilePath);
                    var activities = nodeData.Parent.HydraulicBoundaryDatabase.Locations.Select(hbl => CreateHydraRingActivity(nodeData.Parent, hbl, hlcdDirectory)).ToList();

                    ActivityProgressDialogRunner.Run(Gui.MainWindow, activities);

                    nodeData.Parent.NotifyObservers();
                    nodeData.NotifyObservers();
                }
                );

            if (nodeData.Parent.HydraulicBoundaryDatabase == null)
            {
                designWaterLevelItem.Enabled = false;
                designWaterLevelItem.ToolTipText = RingtoetsFormsResources.DesignWaterLevel_No_HRD_To_Calculate;
            }

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(connectionItem)
                      .AddImportItem()
                      .AddExportItem()
                      .AddSeparator()
                      .AddCustomItem(designWaterLevelItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        private void SelectDatabaseFile(HydraulicBoundaryDatabaseContext nodeData)
        {
            var windowTitle = RingtoetsFormsResources.SelectHydraulicBoundaryDatabaseFile_Title;
            using (var dialog = new OpenFileDialog
            {
                Filter = string.Format("{0} (*.sqlite)|*.sqlite", RingtoetsFormsResources.SelectHydraulicBoundaryDatabaseFile_FilterName),
                Multiselect = false,
                Title = windowTitle,
                RestoreDirectory = true,
                CheckFileExists = false,
            })
            {
                if (dialog.ShowDialog(Gui.MainWindow) == DialogResult.OK)
                {
                    ValidateAndImportSelectedFile(nodeData, dialog.FileName);
                }
            }
        }

        private static void ValidateAndImportSelectedFile(HydraulicBoundaryDatabaseContext nodeData, string selectedFile)
        {
            var hydraulicBoundaryLocationsImporter = new HydraulicBoundaryDatabaseImporter();

            string newVersion;
            try
            {
                hydraulicBoundaryLocationsImporter.ValidateAndConnectTo(selectedFile);

                if (nodeData.Parent.HydraulicBoundaryDatabase == null)
                {
                    ImportSelectedFile(nodeData, hydraulicBoundaryLocationsImporter);
                    return;
                }

                newVersion = hydraulicBoundaryLocationsImporter.GetHydraulicBoundaryDatabaseVersion();
            }
            catch (CriticalFileReadException exception)
            {
                log.Error(exception.Message, exception);
                return;
            }

            var currentVersion = nodeData.Parent.HydraulicBoundaryDatabase.Version;
            var currentFilePath = nodeData.Parent.HydraulicBoundaryDatabase.FilePath;

            // Compare
            if (currentVersion != newVersion)
            {
                // Show dialog
                ShowCleanDialog(nodeData, hydraulicBoundaryLocationsImporter);
                return;
            }

            if (currentFilePath != selectedFile)
            {
                // Only set the new file path. Don't import the complete database.
                SetBoundaryDatabaseData(nodeData, selectedFile);
            }
        }

        private static TargetProbabilityCalculationActivity CreateHydraRingActivity(AssessmentSectionBase assessmentSection, HydraulicBoundaryLocation hydraulicBoundaryLocation, string hlcdDirectory)
        {
            return HydraRingActivityFactory.Create(
                string.Format(Resources.RingtoetsGuiPlugin_Calculate_assessment_level_for_location_0_, hydraulicBoundaryLocation.Id),
                hlcdDirectory,
                assessmentSection.Name, // TODO: Provide name of reference line instead
                HydraRingTimeIntegrationSchemeType.FBC,
                HydraRingUncertaintiesType.All,
                new AssessmentLevelCalculationInput((int)hydraulicBoundaryLocation.Id, assessmentSection.FailureMechanismContribution.Norm),
                output =>
                {
                    ParseHydraRingOutput(hydraulicBoundaryLocation, output);
                });
        }

        private static void ParseHydraRingOutput(HydraulicBoundaryLocation hydraulicBoundaryLocation, TargetProbabilityCalculationOutput output)
        {
            if (output != null)
            {
                hydraulicBoundaryLocation.DesignWaterLevel = output.Result;
            }
            else
            {
                throw new InvalidOperationException(Resources.RingtoetsGuiPlugin_Error_during_assessment_level_calculation);
            }
        }

        private static void ShowCleanDialog(HydraulicBoundaryDatabaseContext nodeData,
                                            HydraulicBoundaryDatabaseImporter hydraulicBoundaryLocationsImporter)
        {
            var confirmation = MessageBox.Show(
                RingtoetsFormsResources.Delete_Calculations_Text,
                BaseResources.Confirm,
                MessageBoxButtons.OKCancel);

            if (confirmation == DialogResult.OK)
            {
                ClearCalculations(nodeData.Parent);

                ImportSelectedFile(nodeData, hydraulicBoundaryLocationsImporter);
            }
        }

        private static void ClearCalculations(AssessmentSectionBase nodeData)
        {
            var failureMechanisms = nodeData.GetFailureMechanisms();

            foreach (ICalculationItem calc in failureMechanisms.SelectMany(fm => fm.CalculationItems))
            {
                calc.ClearOutput();
                calc.ClearHydraulicBoundaryLocation();
                calc.NotifyObservers();
            }

            log.Info(RingtoetsFormsResources.Calculations_Deleted);
        }

        private static void ImportSelectedFile(HydraulicBoundaryDatabaseContext nodeData,
                                               HydraulicBoundaryDatabaseImporter hydraulicBoundaryLocationsImporter)
        {
            if (hydraulicBoundaryLocationsImporter.Import(nodeData))
            {
                SetBoundaryDatabaseData(nodeData);
            }
        }

        private static void SetBoundaryDatabaseData(HydraulicBoundaryDatabaseContext nodeData, string selectedFile = null)
        {
            if (!String.IsNullOrEmpty(selectedFile))
            {
                nodeData.Parent.HydraulicBoundaryDatabase.FilePath = selectedFile;
            }

            nodeData.Parent.NotifyObservers();
            nodeData.NotifyObservers();
            log.InfoFormat(RingtoetsFormsResources.RingtoetsGuiPlugin_SetBoundaryDatabaseFilePath_Database_on_path_0_linked, nodeData.Parent.HydraulicBoundaryDatabase.FilePath);
        }

        #endregion
    }
}