// Copyright (C) Stichting Deltares 2021. All rights reserved.
//
// This file is part of Riskeer.
//
// Riskeer is free software: you can redistribute it and/or modify
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
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.Controls.TreeView;
using Core.Common.Controls.Views;
using Core.Common.Util;
using Core.Gui;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.ProgressDialog;
using Core.Gui.Forms.ViewHost;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.TypeConverters;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Service;
using Riskeer.DuneErosion.Data;
using Riskeer.DuneErosion.Forms.GuiServices;
using Riskeer.DuneErosion.Forms.PresentationObjects;
using Riskeer.DuneErosion.Forms.PropertyClasses;
using Riskeer.DuneErosion.Forms.Views;
using Riskeer.DuneErosion.IO;
using Riskeer.DuneErosion.Plugin.FileImporters;
using Riskeer.DuneErosion.Plugin.Handlers;
using Riskeer.DuneErosion.Plugin.Properties;
using Riskeer.DuneErosion.Service;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonPluginResources = Riskeer.Common.Plugin.Properties.Resources;

namespace Riskeer.DuneErosion.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public class DuneErosionPlugin : PluginBase
    {
        private static readonly NoProbabilityValueDoubleConverter noProbabilityValueDoubleConverter = new NoProbabilityValueDoubleConverter();

        private static readonly IDictionary<IView, IEnumerable<IObserver>> observersForViewTitles = new Dictionary<IView, IEnumerable<IObserver>>();

        private DuneLocationCalculationGuiService duneLocationCalculationGuiService;

        public override IGui Gui
        {
            get => base.Gui;
            set
            {
                if (base.Gui != null)
                {
                    base.Gui.ViewHost.ViewOpened -= OnViewOpened;
                    base.Gui.ViewHost.ViewClosed -= OnViewClosed;
                }

                base.Gui = value;

                if (value != null)
                {
                    base.Gui.ViewHost.ViewOpened += OnViewOpened;
                    base.Gui.ViewHost.ViewClosed += OnViewClosed;
                }
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<DuneErosionHydraulicLoadsContext, DuneErosionHydraulicLoadsProperties>
            {
                CreateInstance = context => new DuneErosionHydraulicLoadsProperties(context.WrappedData)
            };

            yield return new PropertyInfo<DuneErosionFailurePathContext, DuneErosionFailurePathProperties>
            {
                CreateInstance = context => new DuneErosionFailurePathProperties(context.WrappedData)
            };

            yield return new PropertyInfo<DuneLocationCalculationsForUserDefinedTargetProbabilityContext, DuneLocationCalculationsForUserDefinedTargetProbabilityProperties>
            {
                CreateInstance = context => new DuneLocationCalculationsForUserDefinedTargetProbabilityProperties(
                    context.WrappedData,
                    new DuneLocationCalculationsForTargetProbabilityChangeHandler(context.WrappedData))
            };

            yield return new PropertyInfo<DuneLocationCalculation, DuneLocationCalculationProperties>
            {
                CreateInstance = calculation => new DuneLocationCalculationProperties(calculation)
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<DuneErosionHydraulicLoadsContext>(
                HydraulicLoadsChildNodeObjects,
                HydraulicLoadsContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<DuneErosionFailurePathContext>(
                FailurePathChildNodeObjects,
                FailurePathContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext>
            {
                Text = context => RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                ForeColor = context => context.FailureMechanism.DuneLocations.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextMenuStrip,
                ChildNodeObjects = DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextChildNodeObjects,
                CanInsert = DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert,
                CanDrop = DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert,
                OnDrop = DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext_OnDrop
            };

            yield return new TreeNodeInfo<DuneLocationCalculationsForUserDefinedTargetProbabilityContext>
            {
                Text = context => TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(context.WrappedData,
                                                                                                                     context.FailureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities,
                                                                                                                     probability => probability.TargetProbability),
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                EnsureVisibleOnCreate = (context, o) => true,
                CanRemove = (context, o) => true,
                OnNodeRemoved = DuneLocationCalculationsForUserDefinedTargetProbabilityOnNodeRemoved,
                ContextMenuStrip = DuneLocationCalculationsForUserDefinedTargetProbabilityContextMenuStrip,
                CanDrag = (context, o) => true
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>,
                IObservableEnumerable<DuneErosionFailureMechanismSectionResult>,
                DuneErosionFailureMechanismResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new DuneErosionFailureMechanismResultView(
                    context.WrappedData,
                    (DuneErosionFailureMechanism) context.FailureMechanism)
            };

            yield return new ViewInfo<DuneErosionHydraulicLoadsContext, DuneErosionFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CreateInstance = context => new DuneErosionFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<DuneErosionFailurePathContext, DuneErosionFailurePathView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CreateInstance = context => new DuneErosionFailurePathView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<DuneLocationCalculationsForUserDefinedTargetProbabilityContext, IObservableEnumerable<DuneLocationCalculation>, DuneLocationCalculationsView>
            {
                GetViewName = (view, context) => GetDuneLocationCalculationsViewName(context.WrappedData, context.FailureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities),
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                GetViewData = context => context.WrappedData.DuneLocationCalculations,
                CloseForData = CloseDuneLocationCalculationsViewForData,
                CreateInstance = context => new DuneLocationCalculationsView(context.WrappedData.DuneLocationCalculations,
                                                                             context.FailureMechanism,
                                                                             context.AssessmentSection,
                                                                             () => context.WrappedData.TargetProbability,
                                                                             () => noProbabilityValueDoubleConverter.ConvertToString(context.WrappedData.TargetProbability)),
                AfterCreate = (view, context) =>
                {
                    view.CalculationGuiService = duneLocationCalculationGuiService;
                }
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<DuneLocationCalculationsForUserDefinedTargetProbabilityContext>
            {
                Name = context => RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Extension = Resources.DuneErosionPlugin_GetExportInfos_Boundary_conditions_file_filter_Extension,
                CreateFileExporter = (context, filePath) => CreateDuneLocationCalculationsExporter(
                    context.WrappedData
                           .DuneLocationCalculations
                           .Select(calc => new ExportableDuneLocationCalculation(
                                       calc, context.WrappedData.TargetProbability))
                           .ToArray(),
                    filePath),
                IsEnabled = context => context.WrappedData.DuneLocationCalculations.Any(calculation => calculation.Output != null),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), GetFileFilterGenerator())
            };

            yield return new ExportInfo<DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext>
            {
                Name = context => RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Extension = Resources.DuneErosionPlugin_GetExportInfos_Boundary_conditions_file_filter_Extension,
                CreateFileExporter = CreateDuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextFileExporter,
                IsEnabled = IsDuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextExportMenuItemEnabled,
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), GetFileFilterGenerator())
            };
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                DuneErosionFailureMechanismSectionsContext, DuneErosionFailureMechanism, DuneErosionFailureMechanismSectionResult>(
                new DuneErosionFailureMechanismSectionResultUpdateStrategy());
        }

        public override void Activate()
        {
            base.Activate();

            if (Gui == null)
            {
                throw new InvalidOperationException("Gui cannot be null");
            }

            duneLocationCalculationGuiService = new DuneLocationCalculationGuiService(Gui.MainWindow);
        }

        private void OnViewOpened(object sender, ViewChangeEventArgs e)
        {
            if (e.View is DuneLocationCalculationsView duneLocationCalculationsView)
            {
                DuneErosionFailureMechanism duneErosionFailureMechanism = duneLocationCalculationsView.FailureMechanism;
                IObservableEnumerable<DuneLocationCalculationsForTargetProbability> userDefinedTargetProbabilities =
                    duneErosionFailureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities;
                DuneLocationCalculationsForTargetProbability calculationsForUserDefinedTargetProbabilities =
                    userDefinedTargetProbabilities.SingleOrDefault(calculations => ReferenceEquals(calculations.DuneLocationCalculations, duneLocationCalculationsView.Data));

                if (calculationsForUserDefinedTargetProbabilities != null)
                {
                    Action updateViewTitleAction = () => Gui.ViewHost.SetTitle(e.View, GetDuneLocationCalculationsViewName(calculationsForUserDefinedTargetProbabilities,
                                                                                                                           userDefinedTargetProbabilities));

                    var observers = new IObserver[]
                    {
                        new Observer(updateViewTitleAction)
                        {
                            Observable = userDefinedTargetProbabilities
                        },
                        new RecursiveObserver<IObservableEnumerable<DuneLocationCalculationsForTargetProbability>, DuneLocationCalculationsForTargetProbability>(
                            updateViewTitleAction, item => item)
                        {
                            Observable = userDefinedTargetProbabilities
                        }
                    };

                    observersForViewTitles[e.View] = observers;
                }
            }
        }

        private static void OnViewClosed(object sender, ViewChangeEventArgs e)
        {
            if (observersForViewTitles.TryGetValue(e.View, out IEnumerable<IObserver> observersForViewTitle))
            {
                foreach (IObserver observer in observersForViewTitle)
                {
                    var disposableObserver = observer as IDisposable;
                    disposableObserver?.Dispose();
                }

                observersForViewTitles.Remove(e.View);
            }
        }

        private static string GetDuneLocationCalculationsViewName(DuneLocationCalculationsForTargetProbability calculationsForTargetProbability,
                                                                  IEnumerable<DuneLocationCalculationsForTargetProbability> calculationsForTargetProbabilities)
        {
            if (!calculationsForTargetProbabilities.Contains(calculationsForTargetProbability))
            {
                return null;
            }

            string targetProbability = TargetProbabilityCalculationsDisplayNameHelper.GetUniqueDisplayNameForCalculations(calculationsForTargetProbability,
                                                                                                                          calculationsForTargetProbabilities,
                                                                                                                          probability => probability.TargetProbability);
            return $"{RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName} - {targetProbability}";
        }

        #region ViewInfos

        private static bool CloseFailureMechanismResultViewForData(DuneErosionFailureMechanismResultView view, object dataToCloseFor)
        {
            DuneErosionFailureMechanism failureMechanism = null;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<DuneErosionFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is IFailureMechanismContext<DuneErosionFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        private static bool CloseDuneLocationCalculationsViewForData(DuneLocationCalculationsView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as DuneErosionFailureMechanism;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<DuneErosionFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(failureMechanism, view.FailureMechanism);
        }

        #endregion

        #region TreeNodeInfos

        #region DuneErosionHydraulicLoadsContext TreeNodeInfo

        private static object[] HydraulicLoadsChildNodeObjects(DuneErosionHydraulicLoadsContext context)
        {
            DuneErosionFailureMechanism wrappedData = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetHydraulicLoadsInputs(wrappedData), TreeFolderCategory.Input),
                new DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext(context.WrappedData.DuneLocationCalculationsForUserDefinedTargetProbabilities,
                                                                                          context.WrappedData, assessmentSection)
            };
        }

        private static IEnumerable<object> GetHydraulicLoadsInputs(DuneErosionFailureMechanism failureMechanism)
        {
            return new object[]
            {
                failureMechanism.InputComments
            };
        }

        private ContextMenuStrip HydraulicLoadsContextMenuStrip(DuneErosionHydraulicLoadsContext context,
                                                                object parentData,
                                                                TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddCustomItem(CreateCalculateAllItem(context.WrappedData, context.Parent))
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

        #region DuneErosionFailurePathContext TreeNodeInfo

        private static object[] FailurePathChildNodeObjects(DuneErosionFailurePathContext context)
        {
            DuneErosionFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetFailurePathInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetFailurePathOutputs(failureMechanism, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetFailurePathInputs(DuneErosionFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new DuneErosionFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetFailurePathOutputs(DuneErosionFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
                new FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailurePathContextMenuStrip(DuneErosionFailurePathContext context,
                                                             object parentData,
                                                             TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

        #region DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext TreeNodeInfo

        private ContextMenuStrip DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextMenuStrip(
            DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext nodeData,
            object parentData,
            TreeViewControl treeViewControl)
        {
            var addTargetProbabilityItem = new StrictContextMenuItem(
                RiskeerCommonPluginResources.ContextMenuStrip_Add_TargetProbability,
                RiskeerCommonPluginResources.ContextMenuStrip_Add_TargetProbability_ToolTip,
                RiskeerCommonFormsResources.GenericInputOutputIcon,
                (sender, args) =>
                {
                    DuneLocationCalculationsForTargetProbability hydraulicBoundaryLocationCalculationsForTargetProbability = CreateDuneLocationCalculationsForTargetProbability(nodeData.FailureMechanism);

                    nodeData.WrappedData.Add(hydraulicBoundaryLocationCalculationsForTargetProbability);
                    nodeData.WrappedData.NotifyObservers();
                });

            if (!nodeData.FailureMechanism.DuneLocations.Any())
            {
                addTargetProbabilityItem.Enabled = false;
            }

            return Gui.Get(nodeData, treeViewControl)
                      .AddCustomItem(addTargetProbabilityItem)
                      .AddSeparator()
                      .AddExportItem()
                      .AddSeparator()
                      .AddCustomItem(CreateCalculateAllItem(nodeData.FailureMechanism, nodeData.AssessmentSection))
                      .AddSeparator()
                      .AddDeleteChildrenItem()
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .Build();
        }

        private static object[] DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextChildNodeObjects(
            DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext context)
        {
            if (!context.FailureMechanism.DuneLocations.Any())
            {
                return Array.Empty<object>();
            }

            return context.WrappedData
                          .Select(dlc => new DuneLocationCalculationsForUserDefinedTargetProbabilityContext(dlc,
                                                                                                            context.FailureMechanism,
                                                                                                            context.AssessmentSection))
                          .Cast<object>()
                          .ToArray();
        }

        private static DuneLocationCalculationsForTargetProbability CreateDuneLocationCalculationsForTargetProbability(DuneErosionFailureMechanism failureMechanism)
        {
            var hydraulicBoundaryLocationCalculationsForTargetProbability = new DuneLocationCalculationsForTargetProbability(0.01);

            hydraulicBoundaryLocationCalculationsForTargetProbability.DuneLocationCalculations.AddRange(
                failureMechanism.DuneLocations.Select(dl => new DuneLocationCalculation(dl)));

            return hydraulicBoundaryLocationCalculationsForTargetProbability;
        }

        private static bool DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext_CanDropOrInsert(object draggedData, object targetData)
        {
            var duneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext = (DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext) targetData;

            return draggedData is DuneLocationCalculationsForUserDefinedTargetProbabilityContext duneLocationCalculationsForUserDefinedTargetProbabilityContext
                   && duneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext.WrappedData.Contains(duneLocationCalculationsForUserDefinedTargetProbabilityContext.WrappedData);
        }

        private static void DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext_OnDrop(object droppedData, object newParentData, object oldParentData, int position, TreeViewControl treeViewControl)
        {
            var duneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext = (DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext) newParentData;
            var duneLocationCalculationsForUserDefinedTargetProbabilityContext = (DuneLocationCalculationsForUserDefinedTargetProbabilityContext) droppedData;

            duneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext.WrappedData.Remove(
                duneLocationCalculationsForUserDefinedTargetProbabilityContext.WrappedData);
            duneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext.WrappedData.Insert(
                position, duneLocationCalculationsForUserDefinedTargetProbabilityContext.WrappedData);

            duneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext.WrappedData.NotifyObservers();
        }

        #endregion

        #region DuneLocationCalculationsForUserDefinedTargetProbabilityContext TreeNodeInfo

        private static void DuneLocationCalculationsForUserDefinedTargetProbabilityOnNodeRemoved(DuneLocationCalculationsForUserDefinedTargetProbabilityContext context, object o)
        {
            ObservableList<DuneLocationCalculationsForTargetProbability> parent = ((DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext) o).WrappedData;

            parent.Remove(context.WrappedData);
            parent.NotifyObservers();
        }

        private ContextMenuStrip DuneLocationCalculationsForUserDefinedTargetProbabilityContextMenuStrip(DuneLocationCalculationsForUserDefinedTargetProbabilityContext context, object parent, TreeViewControl treeViewControl)
        {
            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.HydraulicLoads_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (duneLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    duneLocationCalculationGuiService.Calculate(context.WrappedData.DuneLocationCalculations,
                                                                context.AssessmentSection,
                                                                context.WrappedData.TargetProbability,
                                                                noProbabilityValueDoubleConverter.ConvertToString(context.WrappedData.TargetProbability));
                });

            string validationText = HydraulicBoundaryDatabaseConnectionValidator.Validate(context.AssessmentSection.HydraulicBoundaryDatabase);

            if (!string.IsNullOrEmpty(validationText))
            {
                calculateAllItem.Enabled = false;
                calculateAllItem.ToolTipText = validationText;
            }

            return Gui.Get(context, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddExportItem()
                      .AddSeparator()
                      .AddCustomItem(calculateAllItem)
                      .AddSeparator()
                      .AddDeleteItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        private StrictContextMenuItem CreateCalculateAllItem(DuneErosionFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.HydraulicLoads_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                                     DuneLocationCalculationActivityFactory.CreateCalculationActivities(failureMechanism, assessmentSection));
                });

            string validationText = HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);

            if (string.IsNullOrEmpty(validationText) && !failureMechanism.DuneLocations.Any())
            {
                validationText = Resources.DuneErosionPlugin_CreateCalculateAllItem_No_calculatable_locations_present;
            }

            if (!string.IsNullOrEmpty(validationText))
            {
                calculateAllItem.Enabled = false;
                calculateAllItem.ToolTipText = validationText;
            }

            return calculateAllItem;
        }

        #endregion

        #region ExportInfos

        private static FileFilterGenerator GetFileFilterGenerator()
        {
            return new FileFilterGenerator(
                Resources.DuneErosionPlugin_GetExportInfos_Boundary_conditions_file_filter_Extension,
                Resources.DuneErosionPlugin_GetExportInfos_Boundary_conditions_file_filter_Description);
        }

        private static bool IsDuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextExportMenuItemEnabled(DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext context)
        {
            return context.FailureMechanism
                          .DuneLocationCalculationsForUserDefinedTargetProbabilities
                          .Any(calculationsForTargetProbability => calculationsForTargetProbability.DuneLocationCalculations
                                                                                                   .Any(c => c.Output != null));
        }

        private static IFileExporter CreateDuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContextFileExporter(DuneLocationCalculationsForUserDefinedTargetProbabilitiesGroupContext context, string filePath)
        {
            return CreateDuneLocationCalculationsExporter(GetExportableDuneLocationCalculations(context.FailureMechanism), filePath);
        }

        private static IEnumerable<ExportableDuneLocationCalculation> GetExportableDuneLocationCalculations(DuneErosionFailureMechanism failureMechanism)
        {
            foreach (DuneLocationCalculationsForTargetProbability calculationsForUserDefinedTargetProbability in failureMechanism.DuneLocationCalculationsForUserDefinedTargetProbabilities)
            {
                foreach (DuneLocationCalculation calculation in calculationsForUserDefinedTargetProbability.DuneLocationCalculations)
                {
                    yield return new ExportableDuneLocationCalculation(
                        calculation, calculationsForUserDefinedTargetProbability.TargetProbability);
                }
            }
        }

        private static DuneLocationCalculationsExporter CreateDuneLocationCalculationsExporter(IEnumerable<ExportableDuneLocationCalculation> exportableCalculations,
                                                                                               string filePath)
        {
            return new DuneLocationCalculationsExporter(exportableCalculations, filePath);
        }

        #endregion
    }
}