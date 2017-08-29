// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Utils;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Service;
using Ringtoets.Revetment.IO.Configurations;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Forms.Views;
using Ringtoets.StabilityStoneCover.IO.Exporters;
using Ringtoets.StabilityStoneCover.Service;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="StabilityStoneCoverFailureMechanism"/>.
    /// </summary>
    public class StabilityStoneCoverPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<StabilityStoneCoverFailureMechanismContext, StabilityStoneCoverFailureMechanismProperties>
            {
                CreateInstance = context => new StabilityStoneCoverFailureMechanismProperties
                {
                    Data = context.WrappedData
                }
            };
            yield return new PropertyInfo<StabilityStoneCoverWaveConditionsOutput, StabilityStoneCoverWaveConditionsOutputProperties>();
            yield return new PropertyInfo<StabilityStoneCoverWaveConditionsInputContext, StabilityStoneCoverWaveConditionsInputContextProperties>
            {
                CreateInstance = context => new StabilityStoneCoverWaveConditionsInputContextProperties(
                    context,
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<StabilityStoneCoverFailureMechanismContext, StabilityStoneCoverFailureMechanismView>
            {
                GetViewName = (view, mechanism) => mechanism.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseStabilityStoneCoverFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return new ViewInfo<FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>,
                IEnumerable<StabilityStoneCoverFailureMechanismSectionResult>,
                StabilityStoneCoverResultView>
            {
                GetViewName = (view, results) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
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
                WaveConditionsCalculationContextChildNodeObjects,
                WaveConditionsCalculationContextContextMenuStrip,
                WaveConditionsCalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<EmptyStabilityStoneCoverOutput>
            {
                Text = emptyOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityStoneCoverWaveConditionsOutput>
            {
                Text = emptyOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
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

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<StabilityStoneCoverWaveConditionsCalculationGroupContext>(
                (context, filePath) =>
                    new WaveConditionsCalculationConfigurationImporter<StabilityStoneCoverWaveConditionsCalculation>(
                        filePath,
                        context.WrappedData,
                        context.HydraulicBoundaryLocations,
                        context.ForeshoreProfiles));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<StabilityStoneCoverWaveConditionsCalculationGroupContext>
            {
                Name = RingtoetsCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) => new StabilityStoneCoverWaveConditionsExporter(context.WrappedData.GetCalculations().Cast<StabilityStoneCoverWaveConditionsCalculation>(), filePath),
                IsEnabled = context => context.WrappedData.GetCalculations().Cast<StabilityStoneCoverWaveConditionsCalculation>().Any(c => c.HasOutput),
                FileFilterGenerator = new FileFilterGenerator(
                    RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                    RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return new ExportInfo<StabilityStoneCoverWaveConditionsCalculationContext>
            {
                Name = RingtoetsCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) => new StabilityStoneCoverWaveConditionsExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                FileFilterGenerator = new FileFilterGenerator(
                    RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                    RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<StabilityStoneCoverWaveConditionsCalculationGroupContext>(
                (context, filePath) => new StabilityStoneCoverCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<StabilityStoneCoverWaveConditionsCalculationContext>(
                (context, filePath) => new StabilityStoneCoverCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
        }

        #region ViewInfos

        #region StabilityStoneCoverFailureMechanismView ViewInfo

        private static bool CloseStabilityStoneCoverFailureMechanismViewForData(StabilityStoneCoverFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as StabilityStoneCoverFailureMechanism;

            var viewFailureMechanismContext = (StabilityStoneCoverFailureMechanismContext) view.Data;
            StabilityStoneCoverFailureMechanism viewFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewFailureMechanism, failureMechanism);
        }

        #endregion

        #region FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>

        private static bool CloseFailureMechanismResultViewForData(StabilityStoneCoverResultView view, object dataToCloseFor)
        {
            object viewData = view.Data;
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

        private static object[] FailureMechanismEnabledChildNodeObjects(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            StabilityStoneCoverFailureMechanism wrappedData = failureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, failureMechanismContext.Parent), TreeFolderCategory.Input),
                new StabilityStoneCoverWaveConditionsCalculationGroupContext(wrappedData.WaveConditionsCalculationGroup, null, wrappedData, failureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                failureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IList GetInputs(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IList GetOutputs(StabilityStoneCoverFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(StabilityStoneCoverFailureMechanismContext failureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(StabilityStoneCoverFailureMechanismContext failureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        #endregion

        #region StabilityStoneCoverWaveConditionsCalculationGroupContext

        private static object[] WaveConditionsCalculationGroupContextChildNodeObjects(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as StabilityStoneCoverWaveConditionsCalculation;
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                                 nodeData.WrappedData,
                                                                                                 nodeData.FailureMechanism,
                                                                                                 nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new StabilityStoneCoverWaveConditionsCalculationGroupContext(group,
                                                                                                      nodeData.WrappedData,
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

        private ContextMenuStrip WaveConditionsCalculationGroupContextContextMenuStrip(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData,
                                                                                       object parentData,
                                                                                       TreeViewControl treeViewControl)
        {
            CalculationGroup group = nodeData.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);
            bool isNestedGroup = parentData is StabilityStoneCoverWaveConditionsCalculationGroupContext;

            StabilityStoneCoverWaveConditionsCalculation[] calculations = group
                .GetCalculations()
                .OfType<StabilityStoneCoverWaveConditionsCalculation>()
                .ToArray();

            builder.AddImportItem()
                   .AddExportItem()
                   .AddSeparator();

            if (!isNestedGroup)
            {
                builder.AddCustomItem(CreateGenerateWaveConditionsCalculationsItem(nodeData))
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(nodeData, AddWaveConditionsCalculation)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddUpdateForeshoreProfileOfCalculationsItem(calculations,
                                                                inquiryHelper,
                                                                SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(nodeData,
                                                          ValidateAll,
                                                          ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(group, nodeData, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup)
                   .AddSeparator()
                   .AddClearAllCalculationOutputInGroupItem(group);

            if (isNestedGroup)
            {
                builder.AddDeleteItem();
            }
            else
            {
                builder.AddRemoveAllChildrenItem();
            }

            return builder.AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static void ValidateAll(StabilityStoneCoverWaveConditionsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StabilityStoneCoverWaveConditionsCalculation>(),
                        context.AssessmentSection.HydraulicBoundaryDatabase);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup(StabilityStoneCoverWaveConditionsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculation(StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        private StrictContextMenuItem CreateGenerateWaveConditionsCalculationsItem(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = nodeData.AssessmentSection.HydraulicBoundaryDatabase;
            bool locationsAvailable = hydraulicBoundaryDatabase != null && hydraulicBoundaryDatabase.Locations.Any();

            string stabilityStoneCoverWaveConditionsCalculationGroupContextToolTip = locationsAvailable
                                                                                         ? RingtoetsCommonFormsResources.CalculationGroup_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip
                                                                                         : RingtoetsCommonFormsResources.CalculationGroup_No_HRD_To_Generate_ToolTip;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                             stabilityStoneCoverWaveConditionsCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowHydraulicBoundaryLocationSelectionDialog(nodeData))
            {
                Enabled = locationsAvailable
            };
        }

        private void ShowHydraulicBoundaryLocationSelectionDialog(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            using (var dialog = new HydraulicBoundaryLocationSelectionDialog(Gui.MainWindow, nodeData.AssessmentSection.HydraulicBoundaryDatabase.Locations))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateStabilityStoneCoverCalculations(dialog.SelectedItems, nodeData.WrappedData.Children);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateStabilityStoneCoverCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                    List<ICalculationBase> calculationCollection)
        {
            StabilityStoneCoverCalculationConfigurationHelper.AddCalculationsFromLocations(
                hydraulicBoundaryLocations,
                calculationCollection);
        }

        private static void AddWaveConditionsCalculation(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children,
                                                  RingtoetsCommonDataResources.Calculation_DefaultName,
                                                  c => c.Name)
            };
            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private static void ValidateAll(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations, HydraulicBoundaryDatabase database)
        {
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculations)
            {
                StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation, database.FilePath);
            }
        }

        private void CalculateAll(CalculationGroup group, StabilityStoneCoverWaveConditionsCalculationGroupContext context)
        {
            StabilityStoneCoverWaveConditionsCalculation[] calculations = group.GetCalculations().OfType<StabilityStoneCoverWaveConditionsCalculation>().ToArray();

            CalculateAll(calculations, context.FailureMechanism, context.AssessmentSection);
        }

        private void CalculateAll(StabilityStoneCoverWaveConditionsCalculation[] calculations,
                                  StabilityStoneCoverFailureMechanism failureMechanism,
                                  IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                calculations
                    .Select(calculation => new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                                    assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                    failureMechanism,
                                                                                                    assessmentSection))
                    .ToList());

            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculations)
            {
                calculation.NotifyObservers();
            }
        }

        private static void WaveConditionsCalculationGroupContextOnNodeRemoved(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (StabilityStoneCoverWaveConditionsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        #endregion

        #region StabilityStoneCoverWaveConditionsCalculationContext

        private static object[] WaveConditionsCalculationContextChildNodeObjects(StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            StabilityStoneCoverWaveConditionsCalculation calculation = context.WrappedData;

            var childNodes = new List<object>
            {
                calculation.Comments,
                new StabilityStoneCoverWaveConditionsInputContext(calculation.InputParameters,
                                                                  calculation,
                                                                  context.FailureMechanism.ForeshoreProfiles,
                                                                  context.AssessmentSection)
            };

            if (calculation.HasOutput)
            {
                childNodes.Add(calculation.Output);
            }
            else
            {
                childNodes.Add(new EmptyStabilityStoneCoverOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip WaveConditionsCalculationContextContextMenuStrip(StabilityStoneCoverWaveConditionsCalculationContext nodeData,
                                                                                  object parentData,
                                                                                  TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);

            StabilityStoneCoverWaveConditionsCalculation calculation = nodeData.WrappedData;
            return builder
                .AddExportItem()
                .AddSeparator()
                .AddRenameItem()
                .AddUpdateForeshoreProfileOfCalculationItem(calculation,
                                                            inquiryHelper,
                                                            SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                .AddSeparator()
                .AddValidateCalculationItem(nodeData,
                                            Validate,
                                            ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                .AddPerformCalculationItem(calculation, nodeData, PerformCalculation, ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                .AddSeparator()
                .AddClearCalculationOutputItem(calculation)
                .AddDeleteItem()
                .AddSeparator()
                .AddCollapseAllItem()
                .AddExpandAllItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        private static void Validate(StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            StabilityStoneCoverWaveConditionsCalculationService.Validate(context.WrappedData, context.AssessmentSection.HydraulicBoundaryDatabase.FilePath);
        }

        private void PerformCalculation(StabilityStoneCoverWaveConditionsCalculation calculation,
                                        StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             new StabilityStoneCoverWaveConditionsCalculationActivity(calculation,
                                                                                                      context.AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                      context.FailureMechanism,
                                                                                                      context.AssessmentSection));
            calculation.NotifyObservers();
        }

        private static void WaveConditionsCalculationContextOnNodeRemoved(StabilityStoneCoverWaveConditionsCalculationContext nodeData, object parentNodeData)
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