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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Contribution;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Plugin;
using Ringtoets.Common.Service;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.Forms.Views;
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
                CreateInstance = context => new StabilityStoneCoverFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<StabilityStoneCoverWaveConditionsOutput, StabilityStoneCoverWaveConditionsOutputProperties>();
            yield return new PropertyInfo<StabilityStoneCoverWaveConditionsInputContext, StabilityStoneCoverWaveConditionsInputContextProperties>
            {
                CreateInstance = context => new StabilityStoneCoverWaveConditionsInputContextProperties(
                    context,
                    () => context.AssessmentSection.GetAssessmentLevel(context.Calculation.InputParameters.HydraulicBoundaryLocation,
                                                                       context.Calculation.InputParameters.CategoryType),
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<StabilityStoneCoverFailureMechanismContext, StabilityStoneCoverFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseStabilityStoneCoverFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new StabilityStoneCoverFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>,
                IObservableEnumerable<StabilityStoneCoverFailureMechanismSectionResult>,
                StabilityStoneCoverResultView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new StabilityStoneCoverResultView(
                    context.WrappedData,
                    (StabilityStoneCoverFailureMechanism) context.FailureMechanism)
            };

            yield return new ViewInfo<StabilityStoneCoverWaveConditionsInputContext,
                ICalculation<AssessmentSectionCategoryWaveConditionsInput>,
                WaveConditionsInputView>
            {
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                GetViewName = (view, context) => RingtoetsCommonFormsResources.Calculation_Input,
                CloseForData = RingtoetsPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new WaveConditionsInputView(
                    context.Calculation,
                    () => context.AssessmentSection.GetHydraulicBoundaryLocationCalculation(context.Calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                            context.Calculation.InputParameters.CategoryType),
                    new StabilityStoneCoverWaveConditionsInputViewStyle())
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

            yield return new TreeNodeInfo<StabilityStoneCoverWaveConditionsInputContext>
            {
                Text = context => RingtoetsCommonFormsResources.Calculation_Input,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<StabilityStoneCoverWaveConditionsCalculationGroupContext>(
                (context, filePath) =>
                    new AssessmentSectionCategoryWaveConditionsCalculationConfigurationImporter<StabilityStoneCoverWaveConditionsCalculation>(
                        filePath,
                        context.WrappedData,
                        context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
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
                (context, filePath) => new AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<StabilityStoneCoverWaveConditionsCalculationContext>(
                (context, filePath) => new AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter(new[]
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

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        #endregion

        #region FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>

        private static bool CloseFailureMechanismResultViewForData(StabilityStoneCoverResultView view, object dataToCloseFor)
        {
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as StabilityStoneCoverFailureMechanism;
            var failureMechanismContext = dataToCloseFor as IFailureMechanismContext<StabilityStoneCoverFailureMechanism>;

            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<StabilityStoneCoverFailureMechanism>()
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #endregion

        #region TreeNodeInfos

        #region StabilityStoneCoverFailureMechanismContext

        private static object[] FailureMechanismEnabledChildNodeObjects(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            StabilityStoneCoverFailureMechanism wrappedData = failureMechanismContext.WrappedData;
            IAssessmentSection assessmentSection = failureMechanismContext.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, assessmentSection), TreeFolderCategory.Input),
                new StabilityStoneCoverWaveConditionsCalculationGroupContext(wrappedData.WaveConditionsCalculationGroup, null, wrappedData, assessmentSection),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                failureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
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
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              failureMechanismContext,
                              CalculateAll,
                              ValidateAllDataAvailableAndGetErrorMessageForFailureMechanism)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void CalculateAll(StabilityStoneCoverFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                                        context.Parent));
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

            if (isNestedGroup)
            {
                builder.AddDuplicateCalculationItem(group, nodeData)
                       .AddSeparator();
            }
            else
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
                        context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForFailureMechanism(StabilityStoneCoverFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent);
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
            bool locationsAvailable = nodeData.AssessmentSection.HydraulicBoundaryDatabase.Locations.Any();

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
                    GenerateStabilityStoneCoverCalculations(dialog.SelectedItems,
                                                            nodeData.WrappedData.Children,
                                                            nodeData.AssessmentSection.FailureMechanismContribution.NormativeNorm);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateStabilityStoneCoverCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                    List<ICalculationBase> calculationCollection,
                                                                    NormType normType)
        {
            StabilityStoneCoverCalculationConfigurationHelper.AddCalculationsFromLocations(
                hydraulicBoundaryLocations,
                calculationCollection,
                normType);
        }

        private static void AddWaveConditionsCalculation(StabilityStoneCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children,
                                                  RingtoetsCommonDataResources.Calculation_DefaultName,
                                                  c => c.Name)
            };
            WaveConditionsInputHelper.SetCategoryType(calculation.InputParameters,
                                                      nodeData.AssessmentSection.FailureMechanismContribution.NormativeNorm);

            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private static void ValidateAll(IEnumerable<StabilityStoneCoverWaveConditionsCalculation> calculations, IAssessmentSection assessmentSection)
        {
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in calculations)
            {
                StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation,
                                                                             assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                                                  calculation.InputParameters.CategoryType),
                                                                             assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                             assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                                                                             assessmentSection.GetNorm(calculation.InputParameters.CategoryType));
            }
        }

        private void CalculateAll(CalculationGroup group, StabilityStoneCoverWaveConditionsCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(group,
                                                                                                        context.FailureMechanism,
                                                                                                        context.AssessmentSection));
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
                                                                  context.AssessmentSection,
                                                                  context.FailureMechanism.ForeshoreProfiles)
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
                   .AddDuplicateCalculationItem(calculation, nodeData)
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
            IAssessmentSection assessmentSection = context.AssessmentSection;
            StabilityStoneCoverWaveConditionsCalculation calculation = context.WrappedData;

            StabilityStoneCoverWaveConditionsCalculationService.Validate(calculation,
                                                                         assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                                              calculation.InputParameters.CategoryType),
                                                                         assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                         assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                                                                         assessmentSection.GetNorm(calculation.InputParameters.CategoryType));
        }

        private void PerformCalculation(StabilityStoneCoverWaveConditionsCalculation calculation,
                                        StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                                                 calculation,
                                                 context.FailureMechanism,
                                                 context.AssessmentSection));
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