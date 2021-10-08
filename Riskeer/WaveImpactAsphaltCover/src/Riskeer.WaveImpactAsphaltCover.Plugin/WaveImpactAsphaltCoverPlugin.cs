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

using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Util;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.ProgressDialog;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Plugin;
using Riskeer.Common.Service;
using Riskeer.Common.Util.Helpers;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.Views;
using Riskeer.Revetment.Service;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses;
using Riskeer.WaveImpactAsphaltCover.Forms.Views;
using Riskeer.WaveImpactAsphaltCover.IO.Configurations;
using Riskeer.WaveImpactAsphaltCover.IO.Exporters;
using Riskeer.WaveImpactAsphaltCover.Plugin.FileImporters;
using Riskeer.WaveImpactAsphaltCover.Service;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.WaveImpactAsphaltCover.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="WaveImpactAsphaltCoverFailureMechanism"/>.
    /// </summary>
    public class WaveImpactAsphaltCoverPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<WaveImpactAsphaltCoverHydraulicLoadsContext, WaveImpactAsphaltCoverHydraulicLoadsProperties>
            {
                CreateInstance = context => new WaveImpactAsphaltCoverHydraulicLoadsProperties(context.WrappedData)
            };
            yield return new PropertyInfo<WaveImpactAsphaltCoverFailurePathContext, WaveImpactAsphaltCoverFailurePathProperties>
            {
                CreateInstance = context => new WaveImpactAsphaltCoverFailurePathProperties(context.WrappedData, context.Parent)
            };
            yield return new PropertyInfo<WaveImpactAsphaltCoverWaveConditionsOutput, WaveImpactAsphaltCoverWaveConditionsOutputProperties>();
            yield return new PropertyInfo<WaveImpactAsphaltCoverWaveConditionsInputContext, WaveImpactAsphaltCoverWaveConditionsInputContextProperties>
            {
                CreateInstance = context => new WaveImpactAsphaltCoverWaveConditionsInputContextProperties(
                    context,
                    () => WaveConditionsInputHelper.GetAssessmentLevel(context.WrappedData, context.AssessmentSection),
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<WaveImpactAsphaltCoverHydraulicLoadsContext, WaveImpactAsphaltCoverFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CreateInstance = context => new WaveImpactAsphaltCoverFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<WaveImpactAsphaltCoverFailurePathContext, WaveImpactAsphaltCoverFailurePathView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CreateInstance = context => new WaveImpactAsphaltCoverFailurePathView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>,
                IObservableEnumerable<WaveImpactAsphaltCoverFailureMechanismSectionResult>,
                WaveImpactAsphaltCoverFailureMechanismResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new WaveImpactAsphaltCoverFailureMechanismResultView(
                    context.WrappedData,
                    (WaveImpactAsphaltCoverFailureMechanism) context.FailureMechanism)
            };

            yield return new ViewInfo<WaveImpactAsphaltCoverWaveConditionsInputContext,
                ICalculation<WaveConditionsInput>,
                WaveConditionsInputView>
            {
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new WaveConditionsInputView(
                    context.Calculation,
                    () => WaveConditionsInputHelper.GetHydraulicBoundaryLocationCalculation(context.WrappedData, context.AssessmentSection),
                    new WaveImpactAsphaltCoverWaveConditionsInputViewStyle())
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext>(
                (context, filePath) =>
                    new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationImporter(
                        filePath,
                        context.WrappedData,
                        context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                        context.ForeshoreProfiles,
                        context.AssessmentSection.FailureMechanismContribution,
                        context.AssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities));
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<WaveImpactAsphaltCoverHydraulicLoadsContext>(
                HydraulicLoadsChildNodeObjects,
                HydraulicLoadsContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<WaveImpactAsphaltCoverFailurePathContext>(
                FailurePathChildNodeObjects,
                FailurePathContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext>(
                WaveConditionsCalculationGroupContextChildNodeObjects,
                WaveConditionsCalculationGroupContextContextMenuStrip,
                WaveConditionsCalculationGroupContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<WaveImpactAsphaltCoverWaveConditionsCalculationContext>(
                WaveConditionsCalculationContextChildNodeObjects,
                WaveConditionsCalculationContextContextMenuStrip,
                WaveConditionsCalculationContextOnNodeRemoved,
                CalculationType.Hydraulic);

            yield return new TreeNodeInfo<EmptyWaveImpactAsphaltCoverOutput>
            {
                Text = emptyOutput => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RiskeerCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<WaveImpactAsphaltCoverWaveConditionsOutput>
            {
                Text = emptyOutput => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RiskeerCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<WaveImpactAsphaltCoverWaveConditionsInputContext>
            {
                Text = context => RiskeerCommonFormsResources.Calculation_Input,
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext>
            {
                Name = context => RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                Extension = RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                CreateFileExporter = (context, filePath) =>
                {
                    IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations = context.WrappedData.GetCalculations().Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>();
                    return new WaveImpactAsphaltCoverWaveConditionsExporter(calculations, filePath, input =>
                                                                                WaveConditionsInputHelper.GetTargetProbability(input, context.AssessmentSection)
                                                                                                         .ToString(CultureInfo.InvariantCulture));
                },
                IsEnabled = context => context.WrappedData.GetCalculations().Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>().Any(c => c.HasOutput),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), GetWaveConditionsFileFilterGenerator())
            };

            yield return new ExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationContext>
            {
                Name = context => RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                Extension = RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new WaveImpactAsphaltCoverWaveConditionsExporter(new[]
                                                                                                             {
                                                                                                                 context.WrappedData
                                                                                                             }, filePath,
                                                                                                             input =>
                                                                                                                 WaveConditionsInputHelper.GetTargetProbability(input, context.AssessmentSection)
                                                                                                                                          .ToString(CultureInfo.InvariantCulture)),
                IsEnabled = context => context.WrappedData.HasOutput,
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), GetWaveConditionsFileFilterGenerator())
            };

            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext>(
                (context, filePath) => new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationExporter(
                    context.WrappedData.Children, filePath, context.AssessmentSection),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationContext>(
                (context, filePath) => new WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationExporter(
                    new[]
                    {
                        context.WrappedData
                    }, filePath, context.AssessmentSection),
                GetInquiryHelper());
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                WaveImpactAsphaltCoverFailureMechanismSectionsContext, WaveImpactAsphaltCoverFailureMechanism, WaveImpactAsphaltCoverFailureMechanismSectionResult>(
                new WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategy());
        }

        private static FileFilterGenerator GetWaveConditionsFileFilterGenerator()
        {
            return new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                           RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description);
        }

        #region ViewInfos

        private static bool CloseFailureMechanismResultViewForData(WaveImpactAsphaltCoverFailureMechanismResultView view, object dataToCloseFor)
        {
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = null;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<WaveImpactAsphaltCoverFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is IFailureMechanismContext<WaveImpactAsphaltCoverFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #region TreeNodeInfos

        #region WaveImpactAsphaltCoverHydraulicLoadsContext TreeNodeInfo

        private static object[] HydraulicLoadsChildNodeObjects(WaveImpactAsphaltCoverHydraulicLoadsContext context)
        {
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetHydraulicLoadsInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup, null,
                                                                                failureMechanism, assessmentSection)
            };
        }

        private static IEnumerable<object> GetHydraulicLoadsInputs(WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private ContextMenuStrip HydraulicLoadsContextMenuStrip(WaveImpactAsphaltCoverHydraulicLoadsContext context,
                                                                object parentData,
                                                                TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              context,
                              CalculateAllInFailureMechanism,
                              EnableValidateAndCalculateMenuItemForFailureMechanism)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static string EnableValidateAndCalculateMenuItemForFailureMechanism(WaveImpactAsphaltCoverHydraulicLoadsContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.Parent);
        }

        private void CalculateAllInFailureMechanism(WaveImpactAsphaltCoverHydraulicLoadsContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                                           context.Parent));
        }

        #endregion

        #region WaveImpactAsphaltCoverFailurePathContext TreeNodeInfo

        private static object[] FailurePathChildNodeObjects(WaveImpactAsphaltCoverFailurePathContext context)
        {
            WaveImpactAsphaltCoverFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetFailurePathInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetFailurePathOutputs(failureMechanism, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetFailurePathInputs(WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new WaveImpactAsphaltCoverFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetFailurePathOutputs(WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(
                    failureMechanism, assessmentSection, () => failureMechanism.GeneralWaveImpactAsphaltCoverInput.GetN(
                        assessmentSection.ReferenceLine.Length)),
                new FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailurePathContextMenuStrip(WaveImpactAsphaltCoverFailurePathContext context,
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

        #region WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext TreeNodeInfo

        private static object[] WaveConditionsCalculationGroupContextChildNodeObjects(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as WaveImpactAsphaltCoverWaveConditionsCalculation;
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new WaveImpactAsphaltCoverWaveConditionsCalculationContext(calculation,
                                                                                                    nodeData.WrappedData,
                                                                                                    nodeData.FailureMechanism,
                                                                                                    nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(group,
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

        private ContextMenuStrip WaveConditionsCalculationGroupContextContextMenuStrip(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData,
                                                                                       object parentData, TreeViewControl treeViewControl)
        {
            CalculationGroup group = nodeData.WrappedData;
            IInquiryHelper inquiryHelper = GetInquiryHelper();

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            bool isNestedGroup = parentData is WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext;

            WaveImpactAsphaltCoverWaveConditionsCalculation[] calculations = group
                                                                             .GetCalculations()
                                                                             .OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>().ToArray();

            StrictContextMenuItem generateCalculationsItem = CreateGenerateWaveConditionsCalculationsItem(nodeData);

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
                builder.AddCustomItem(generateCalculationsItem)
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(nodeData, AddWaveConditionsCalculation, CalculationType.Hydraulic)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddUpdateForeshoreProfileOfCalculationsItem(calculations, inquiryHelper,
                                                                SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(nodeData,
                                                          ValidateAllInCalculationGroup,
                                                          EnableValidateAndCalculateMenuItemForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(nodeData,
                                                         CalculateAllInCalculationGroup,
                                                         EnableValidateAndCalculateMenuItemForCalculationGroup)
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

        private StrictContextMenuItem CreateGenerateWaveConditionsCalculationsItem(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData)
        {
            bool locationsAvailable = nodeData.AssessmentSection.HydraulicBoundaryDatabase.Locations.Any();

            string waveImpactAsphaltCoverWaveConditionsCalculationGroupContextToolTip = locationsAvailable
                                                                                            ? RiskeerCommonFormsResources.CalculationGroup_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip
                                                                                            : RiskeerCommonFormsResources.CalculationGroup_No_HydraulicBoundaryDatabase_To_Generate_ToolTip;

            return new StrictContextMenuItem(RiskeerCommonFormsResources.CalculationGroup_Generate_calculations,
                                             waveImpactAsphaltCoverWaveConditionsCalculationGroupContextToolTip,
                                             RiskeerCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowHydraulicBoundaryLocationSelectionDialog(nodeData))
            {
                Enabled = locationsAvailable
            };
        }

        private void ShowHydraulicBoundaryLocationSelectionDialog(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData)
        {
            using (var dialog = new HydraulicBoundaryLocationSelectionDialog(Gui.MainWindow, nodeData.AssessmentSection.HydraulicBoundaryDatabase.Locations))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateWaveImpactAsphaltCoverWaveConditionsCalculations(dialog.SelectedItems,
                                                                             nodeData.WrappedData.Children,
                                                                             nodeData.AssessmentSection.FailureMechanismContribution.NormativeNorm);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateWaveImpactAsphaltCoverWaveConditionsCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                                     List<ICalculationBase> calculationCollection,
                                                                                     NormType normType)
        {
            WaveImpactAsphaltCoverWaveConditionsCalculationConfigurationHelper.AddCalculationsFromLocations(
                hydraulicBoundaryLocations,
                calculationCollection,
                normType);
        }

        private static void AddWaveConditionsCalculation(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData)
        {
            var calculation = new WaveImpactAsphaltCoverWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children,
                                                  RiskeerCommonDataResources.Calculation_DefaultName,
                                                  c => c.Name)
            };
            WaveConditionsInputHelper.SetWaterLevelType(calculation.InputParameters,
                                                        nodeData.AssessmentSection.FailureMechanismContribution.NormativeNorm);
            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private static void WaveConditionsCalculationGroupContextOnNodeRemoved(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculationGroup(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void ValidateAllInCalculationGroup(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext context)
        {
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in context.WrappedData.GetCalculations().OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>())
            {
                WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                              WaveConditionsInputHelper.GetAssessmentLevel(calculation.InputParameters, context.AssessmentSection),
                                                              context.AssessmentSection.HydraulicBoundaryDatabase);
            }
        }

        private void CalculateAllInCalculationGroup(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                                           context.FailureMechanism,
                                                                                                           context.AssessmentSection));
        }

        #endregion

        #region WaveImpactAsphaltCoverWaveConditionsCalculationContext TreeNodeInfo

        private static object[] WaveConditionsCalculationContextChildNodeObjects(WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = context.WrappedData;

            var childNodes = new List<object>
            {
                calculation.Comments,
                new WaveImpactAsphaltCoverWaveConditionsInputContext(calculation.InputParameters,
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
                childNodes.Add(new EmptyWaveImpactAsphaltCoverOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip WaveConditionsCalculationContextContextMenuStrip(WaveImpactAsphaltCoverWaveConditionsCalculationContext nodeData,
                                                                                  object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = nodeData.WrappedData;

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, nodeData)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddUpdateForeshoreProfileOfCalculationItem(calculation, GetInquiryHelper(),
                                                                      SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                          .AddSeparator()
                          .AddValidateCalculationItem(nodeData,
                                                      Validate,
                                                      EnableValidateAndCalculateMenuItemForCalculation)
                          .AddPerformCalculationItem<WaveImpactAsphaltCoverWaveConditionsCalculation, WaveImpactAsphaltCoverWaveConditionsCalculationContext>(
                              nodeData, Calculate, EnableValidateAndCalculateMenuItemForCalculation)
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

        private static string EnableValidateAndCalculateMenuItemForCalculation(WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void Validate(WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            IAssessmentSection assessmentSection = context.AssessmentSection;
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = context.WrappedData;

            WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                          WaveConditionsInputHelper.GetAssessmentLevel(calculation.InputParameters, context.AssessmentSection),
                                                          assessmentSection.HydraulicBoundaryDatabase);
        }

        private void Calculate(WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(context.WrappedData,
                                                                                                                                      context.FailureMechanism,
                                                                                                                                      context.AssessmentSection));
        }

        private static void WaveConditionsCalculationContextOnNodeRemoved(WaveImpactAsphaltCoverWaveConditionsCalculationContext nodeData, object parentNodeData)
        {
            if (parentNodeData is WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext calculationGroupContext)
            {
                bool successfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);
                if (successfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
            }
        }

        #endregion

        private static string EnableValidateAndCalculateMenuItem(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        #endregion
    }
}