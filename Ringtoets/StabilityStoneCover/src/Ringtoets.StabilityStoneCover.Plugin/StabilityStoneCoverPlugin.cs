﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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

using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.HydraRing.Data;
using Ringtoets.HydraRing.IO;
using Ringtoets.Revetment.IO;
using Ringtoets.Revetment.Service;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Forms.Views;
using Ringtoets.StabilityStoneCover.Plugin.Properties;
using Ringtoets.StabilityStoneCover.Service;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using StabilityStoneCoverDataResources = Ringtoets.StabilityStoneCover.Data.Properties.Resources;
using StabilityStoneCoverFormsResources = Ringtoets.StabilityStoneCover.Forms.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Plugin
{
    /// <summary>
    /// The plugin for the <see cref="StabilityStoneCoverFailureMechanism"/>.
    /// </summary>
    public class StabilityStoneCoverPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<StabilityStoneCoverFailureMechanismContext, StabilityStoneCoverFailureMechanismContextProperties>();
            yield return new PropertyInfo<StabilityStoneCoverWaveConditionsCalculationInputContext, StabilityStoneCoverWaveConditionsCalculationInputContextProperties>();
            yield return new PropertyInfo<StabilityStoneCoverWaveConditionsOutput, StabilityStoneCoverWaveConditionsOutputProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>,
                IEnumerable<StabilityStoneCoverFailureMechanismSectionResult>,
                StabilityStoneCoverResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<StabilityStoneCoverFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<StabilityStoneCoverWaveConditionsCalculationGroupContext>(
                WaveConditionsCalculationGroupContextChildNodeObjects,
                WaveConditionsCalculationGroupContextContextMenuStrip,
                WaveConditionsCalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<StabilityStoneCoverWaveConditionsCalculationContext>(
                StabilityStoneCoverFormsResources.CalculationIcon,
                WaveConditionsCalculationContextChildNodeObjects,
                WaveConditionsCalculationContextContextMenuStrip,
                WaveConditionsCalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<StabilityStoneCoverWaveConditionsCalculationInputContext>
            {
                Text = context => RingtoetsCommonFormsResources.Calculation_Input,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyStabilityStoneCoverOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityStoneCoverWaveConditionsOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<StabilityStoneCoverWaveConditionsCalculationGroupContext>
            {
                CreateFileExporter = (context, filePath) => CreateWaveConditionsExporter(context.WrappedData.Children.OfType<ICalculation>().Cast<StabilityStoneCoverWaveConditionsCalculation>(), filePath),
                IsEnabled = context => context.WrappedData.Children.OfType<ICalculation>().Cast<StabilityStoneCoverWaveConditionsCalculation>().Any(c => c.HasOutput),
                FileFilter = Resources.DataTypeDisplayName_csv_file_filter
            };

            yield return new ExportInfo<StabilityStoneCoverWaveConditionsCalculationContext>
            {
                CreateFileExporter = (context, filePath) => CreateWaveConditionsExporter(new[] { context.WrappedData }, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                FileFilter = Resources.DataTypeDisplayName_csv_file_filter
            };
        }

        #region ExportInfos

        private static WaveConditionsExporter CreateWaveConditionsExporter(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations, string filePath)
        {
            var exportableWaveConditions = new List<ExportableWaveConditions>();

            IEnumerable<StabilityStoneCoverWaveConditionsCalculation> exportableCalculations = calculations.Where(c => c.HasOutput);

            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in exportableCalculations)
            {
                exportableWaveConditions.AddRange(
                    ExportableWaveConditionsFactory.CreateExportableWaveConditionsCollection(
                        calculation.Name, calculation.InputParameters, calculation.Output.ColumnsOutput, calculation.Output.BlocksOutput));
            }
            return new WaveConditionsExporter(exportableWaveConditions, filePath);
        }

        #endregion

        #region ViewInfos

        #region FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>

        private static bool CloseFailureMechanismResultViewForData(StabilityStoneCoverResultView view, object dataToCloseFor)
        {
            var viewData = view.Data;
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as StabilityStoneCoverFailureMechanism;
            var failureMechanismContext = dataToCloseFor as IFailureMechanismContext<StabilityStoneCoverFailureMechanism>;

            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<StabilityStoneCoverFailureMechanism>()
                    .Any(fm => ReferenceEquals(viewData, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #endregion

        #region TreeNodeInfos

        #region StabilityStoneCoverFailureMechanismContext

        private object[] FailureMechanismEnabledChildNodeObjects(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            StabilityStoneCoverFailureMechanism wrappedData = failureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, failureMechanismContext.Parent), TreeFolderCategory.Input),
                new StabilityStoneCoverWaveConditionsCalculationGroupContext(wrappedData.WaveConditionsCalculationGroup, wrappedData, failureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(failureMechanismContext.WrappedData)
            };
        }

        private static IList GetInputs(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private IList GetOutputs(StabilityStoneCoverFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(StabilityStoneCoverFailureMechanismContext failureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(StabilityStoneCoverFailureMechanismContext failureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        #endregion

        #region StabilityStoneCoverWaveConditionsCalculationGroupContext

        private object[] WaveConditionsCalculationGroupContextChildNodeObjects(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as StabilityStoneCoverWaveConditionsCalculation;
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                                 nodeData.FailureMechanism,
                                                                                                 nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                                      nodeData.FailureMechanism,
                                                                                                      nodeData.AssessmentSection));
                }
                else
                {
                    childNodeObjects.Add(item);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip WaveConditionsCalculationGroupContextContextMenuStrip(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var group = nodeData.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var isNestedGroup = parentData is StabilityStoneCoverWaveConditionsCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateWaveConditionsCalculationsItem(nodeData);

            if (!isNestedGroup)
            {
                builder.AddCustomItem(generateCalculationsItem);
            }

            builder.AddExportItem()
                   .AddSeparator()
                   .AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(nodeData, AddWaveConditionsCalculation);

            if (!isNestedGroup)
            {
                builder.AddSeparator()
                       .AddRemoveAllChildrenItem(group, Gui.ViewCommands);
            }

            builder.AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(nodeData,
                                                          c => ValidateAll(
                                                              c.WrappedData.GetCalculations().OfType<StabilityStoneCoverWaveConditionsCalculation>(),
                                                              c.FailureMechanism.GeneralInput,
                                                              c.AssessmentSection.FailureMechanismContribution.Norm,
                                                              c.AssessmentSection.HydraulicBoundaryDatabase),
                                                          ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(group, nodeData, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup)
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem()
                       .AddDeleteItem()
                       .AddSeparator();
            }

            return builder.AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private string ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup(StabilityStoneCoverWaveConditionsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private string ValidateAllDataAvailableAndGetErrorMessageForCalculation(StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, StabilityStoneCoverFailureMechanism failureMechanism)
        {
            if (!failureMechanism.Sections.Any())
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported;
            }

            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported;
            }

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }

            return null;
        }

        private StrictContextMenuItem CreateGenerateWaveConditionsCalculationsItem(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = nodeData.AssessmentSection.HydraulicBoundaryDatabase;
            bool locationsAvailable = hydraulicBoundaryDatabase != null && hydraulicBoundaryDatabase.Locations.Any();

            string stabilityStoneCoverWaveConditionsCalculationGroupContextToolTip = locationsAvailable
                                                                                         ? Resources.StabilityStoneCoverPlugin_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip
                                                                                         : Resources.StabilityStoneCover_WaveConditions_No_HRD_To_Calculate;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationsGroup_Generate_calculations,
                                             stabilityStoneCoverWaveConditionsCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => { ShowStabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(nodeData); })
            {
                Enabled = locationsAvailable
            };
        }

        private void ShowStabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            using (var dialog = new StabilityStoneCoverHydraulicBoundaryLocationSelectionDialog(Gui.MainWindow,
                                                                                                nodeData.AssessmentSection.HydraulicBoundaryDatabase.Locations))
            {
                dialog.ShowDialog();
                GenerateStabilityStoneCoverCalculations(nodeData.WrappedData, dialog.SelectedItems);

                if (dialog.SelectedItems.Any())
                {
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateStabilityStoneCoverCalculations(CalculationGroup target, IEnumerable<IHydraulicBoundaryLocation> hydraulicBoundaryLocations)
        {
            StabilityStoneCoverCalculationConfigurationHelper.AddCalculationsFromLocations(
                hydraulicBoundaryLocations.OfType<HydraulicBoundaryLocation>(),
                target.Children);
        }

        private void AddWaveConditionsCalculation(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children,
                                                  StabilityStoneCoverDataResources.StabilityStoneCoverWaveConditionsCalculation_DefaultName,
                                                  c => c.Name)
            };
            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private void ValidateAll(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations, GeneralStabilityStoneCoverWaveConditionsInput generalInput, int norm, HydraulicBoundaryDatabase database)
        {
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculations)
            {
                WaveConditionsCalculationService.Instance.Validate(calculation.Name);
            }
        }

        private void CalculateAll(CalculationGroup group, StabilityStoneCoverWaveConditionsCalculationGroupContext context)
        {
            var calculations = group.GetCalculations().OfType<StabilityStoneCoverWaveConditionsCalculation>().ToArray();

            CalculateAll(calculations, context.FailureMechanism, context.AssessmentSection);
        }

        private void CalculateAll(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations,
                                  StabilityStoneCoverFailureMechanism failureMechanism,
                                  IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                calculations
                    .Select(calculation => new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                                    Path.GetDirectoryName(assessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                                    failureMechanism,
                                                                                                    assessmentSection))
                    .ToList());

            foreach (var calculation in calculations)
            {
                calculation.NotifyObservers();
            }
        }

        private void WaveConditionsCalculationGroupContextOnNodeRemoved(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (StabilityStoneCoverWaveConditionsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        #endregion

        #region StabilityStoneCoverWaveConditionsCalculationContext

        private object[] WaveConditionsCalculationContextChildNodeObjects(StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(context.WrappedData),
                new StabilityStoneCoverWaveConditionsCalculationInputContext(context.WrappedData.InputParameters,
                                                                             context.FailureMechanism,
                                                                             context.AssessmentSection)
            };

            if (context.WrappedData.HasOutput)
            {
                childNodes.Add(context.WrappedData.Output);
            }
            else
            {
                childNodes.Add(new EmptyStabilityStoneCoverOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip WaveConditionsCalculationContextContextMenuStrip(StabilityStoneCoverWaveConditionsCalculationContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            StabilityStoneCoverWaveConditionsCalculation calculation = nodeData.WrappedData;

            return builder.AddValidateCalculationItem(nodeData,
                                                      c => ValidateAll(
                                                          new[]
                                                          {
                                                              c.WrappedData
                                                          },
                                                          c.FailureMechanism.GeneralInput,
                                                          c.AssessmentSection.FailureMechanismContribution.Norm,
                                                          c.AssessmentSection.HydraulicBoundaryDatabase),
                                                      ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                          .AddPerformCalculationItem(calculation, nodeData, PerformCalculation)
                          .AddClearCalculationOutputItem(calculation)
                          .AddExportItem()
                          .AddSeparator()
                          .AddRenameItem()
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void PerformCalculation(StabilityStoneCoverWaveConditionsCalculation calculation,
                                        StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                                      Path.GetDirectoryName(context.AssessmentSection.HydraulicBoundaryDatabase.FilePath),
                                                                                                      context.FailureMechanism,
                                                                                                      context.AssessmentSection));
            calculation.NotifyObservers();
        }

        private void WaveConditionsCalculationContextOnNodeRemoved(StabilityStoneCoverWaveConditionsCalculationContext nodeData, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as StabilityStoneCoverWaveConditionsCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                bool successfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);
                if (successfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
            }
        }

        #endregion

        #endregion
    }
}