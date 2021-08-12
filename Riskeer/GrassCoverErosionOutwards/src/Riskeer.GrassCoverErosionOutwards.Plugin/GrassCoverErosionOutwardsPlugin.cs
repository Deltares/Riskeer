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
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Plugin;
using Riskeer.Common.Service;
using Riskeer.GrassCoverErosionOutwards.Data;
using Riskeer.GrassCoverErosionOutwards.Forms;
using Riskeer.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Riskeer.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Riskeer.GrassCoverErosionOutwards.Forms.Views;
using Riskeer.GrassCoverErosionOutwards.IO.Configurations;
using Riskeer.GrassCoverErosionOutwards.IO.Exporters;
using Riskeer.GrassCoverErosionOutwards.Plugin.FileImporters;
using Riskeer.GrassCoverErosionOutwards.Service;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.Views;
using Riskeer.Revetment.Service;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonIoResources = Riskeer.Common.IO.Properties.Resources;

namespace Riskeer.GrassCoverErosionOutwards.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionOutwardsHydraulicLoadsContext, GrassCoverErosionOutwardsHydraulicLoadsProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsHydraulicLoadsProperties(context.WrappedData)
            };

            yield return new PropertyInfo<GrassCoverErosionOutwardsFailurePathContext, GrassCoverErosionOutwardsFailurePathProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsFailurePathProperties(
                    context.WrappedData)
            };

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveConditionsOutputContext, GrassCoverErosionOutwardsWaveConditionsOutputProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsWaveConditionsOutputProperties(context.WrappedData, context.Input)
            };

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveConditionsInputContext, GrassCoverErosionOutwardsWaveConditionsInputContextProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(
                    context,
                    () => context.FailureMechanism.GetAssessmentLevel(context.AssessmentSection,
                                                                      context.Calculation.InputParameters.HydraulicBoundaryLocation,
                                                                      context.Calculation.InputParameters.CategoryType),
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>(
                (context, filePath) =>
                    new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationImporter(
                        filePath,
                        context.WrappedData,
                        context.HydraulicBoundaryLocations,
                        context.ForeshoreProfiles,
                        context.AssessmentSection.FailureMechanismContribution.NormativeNorm));
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<GrassCoverErosionOutwardsHydraulicLoadsContext, GrassCoverErosionOutwardsFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CreateInstance = context => new GrassCoverErosionOutwardsFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<GrassCoverErosionOutwardsFailurePathContext, GrassCoverErosionOutwardsFailurePathView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CreateInstance = context => new GrassCoverErosionOutwardsFailurePathView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>,
                IObservableEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>,
                GrassCoverErosionOutwardsFailureMechanismResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new GrassCoverErosionOutwardsFailureMechanismResultView(
                    context.WrappedData,
                    (GrassCoverErosionOutwardsFailureMechanism) context.FailureMechanism)
            };

            yield return new ViewInfo<GrassCoverErosionOutwardsWaveConditionsInputContext,
                ICalculation<FailureMechanismCategoryWaveConditionsInput>,
                WaveConditionsInputView>
            {
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new WaveConditionsInputView(
                    context.Calculation,
                    () => context.FailureMechanism.GetHydraulicBoundaryLocationCalculation(
                        context.AssessmentSection,
                        context.Calculation.InputParameters.HydraulicBoundaryLocation,
                        context.Calculation.InputParameters.CategoryType),
                    new GrassCoverErosionOutwardsWaveConditionsInputViewStyle())
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<GrassCoverErosionOutwardsHydraulicLoadsContext>(
                HydraulicLoadsChildNodeObjects,
                HydraulicLoadsContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<GrassCoverErosionOutwardsFailurePathContext>(
                FailurePathChildNodeObjects,
                FailurePathContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>(
                WaveConditionsCalculationGroupChildrenNodeObjects,
                WaveConditionsCalculationGroupContextMenuStrip,
                WaveConditionsCalculationGroupContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>(
                WaveConditionsCalculationContextChildNodeObjects,
                WaveConditionsCalculationContextMenuStrip,
                WaveConditionsCalculationContextOnNodeRemoved,
                CalculationType.Hydraulic);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyGrassCoverErosionOutwardsOutput>
            {
                Text = emptyOutput => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RiskeerCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsOutputContext>
            {
                Text = context => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsInputContext>
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
            yield return new ExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>
            {
                Name = RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                Extension = RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                CreateFileExporter = (context, filePath) =>
                {
                    IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations = context.WrappedData.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
                    return new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath);
                },
                IsEnabled = context => context.WrappedData.GetCalculations().Any(c => c.HasOutput),
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                                                                           RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description))
            };

            yield return new ExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>
            {
                Name = RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                Extension = RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new GrassCoverErosionOutwardsWaveConditionsExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                                                                           RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description))
            };

            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>(
                (context, filePath) => new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>(
                (context, filePath) => new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                GrassCoverErosionOutwardsFailureMechanismSectionsContext, GrassCoverErosionOutwardsFailureMechanism, GrassCoverErosionOutwardsFailureMechanismSectionResult>(
                new GrassCoverErosionOutwardsFailureMechanismSectionResultUpdateStrategy());
        }

        #region ViewInfos

        private static bool CloseFailureMechanismResultViewForData(GrassCoverErosionOutwardsFailureMechanismResultView view, object dataToCloseFor)
        {
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = null;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is IFailureMechanismContext<GrassCoverErosionOutwardsFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #region TreeNodeInfos

        #region GrassCoverErosionOutwardsHydraulicLoadsContext TreeNodeInfo

        private static object[] HydraulicLoadsChildNodeObjects(GrassCoverErosionOutwardsHydraulicLoadsContext context)
        {
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetHydraulicLoadsInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                   null, failureMechanism, assessmentSection)
            };
        }

        private static IEnumerable<object> GetHydraulicLoadsInputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private ContextMenuStrip HydraulicLoadsContextMenuStrip(GrassCoverErosionOutwardsHydraulicLoadsContext context,
                                                                object parentData,
                                                                TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            IAssessmentSection assessmentSection = context.Parent;
            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        Gui.MainWindow,
                        GrassCoverErosionOutwardsCalculationActivityFactory
                            .CreateWaveConditionsCalculationActivities(context.WrappedData.WaveConditionsCalculationGroup,
                                                                       context.WrappedData,
                                                                       assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection,
                                                        calculateAllItem);

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddCustomItem(calculateAllItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

        #region GrassCoverErosionOutwardsFailurePathContext TreeNodeInfo

        private static object[] FailurePathChildNodeObjects(GrassCoverErosionOutwardsFailurePathContext context)
        {
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetFailurePathInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetFailurePathOutputs(failureMechanism, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetFailurePathInputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverErosionOutwardsFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetFailurePathOutputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
                new FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailurePathContextMenuStrip(GrassCoverErosionOutwardsFailurePathContext context,
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

        #region GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext TreeNodeInfo

        private static object[] WaveConditionsCalculationGroupChildrenNodeObjects(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as GrassCoverErosionOutwardsWaveConditionsCalculation;
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionOutwardsWaveConditionsCalculationContext(calculation,
                                                                                                       nodeData.WrappedData,
                                                                                                       nodeData.FailureMechanism,
                                                                                                       nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
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

        private ContextMenuStrip WaveConditionsCalculationGroupContextMenuStrip(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData,
                                                                                object parentData,
                                                                                TreeViewControl treeViewControl)
        {
            CalculationGroup group = nodeData.WrappedData;
            IInquiryHelper inquiryHelper = GetInquiryHelper();
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            bool isNestedGroup = parentData is GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext;

            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculations = group
                                                                                .GetCalculations()
                                                                                .OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>()
                                                                                .ToArray();

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
                   .AddValidateAllCalculationsInGroupItem(
                       nodeData,
                       ValidateAllInCalculationGroup,
                       EnableValidateAndCalculateMenuItemForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(
                       nodeData,
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

        private StrictContextMenuItem CreateGenerateWaveConditionsCalculationsItem(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData)
        {
            bool locationsAvailable = nodeData.AssessmentSection.HydraulicBoundaryDatabase.Locations.Any();

            string grassCoverErosionOutwardsWaveConditionsCalculationGroupContextToolTip = locationsAvailable
                                                                                               ? RiskeerCommonFormsResources.CalculationGroup_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip
                                                                                               : RiskeerCommonFormsResources.CalculationGroup_No_HydraulicBoundaryDatabase_To_Generate_ToolTip;

            return new StrictContextMenuItem(RiskeerCommonFormsResources.CalculationGroup_Generate_calculations,
                                             grassCoverErosionOutwardsWaveConditionsCalculationGroupContextToolTip,
                                             RiskeerCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowHydraulicBoundaryLocationSelectionDialog(nodeData))
            {
                Enabled = locationsAvailable
            };
        }

        private void ShowHydraulicBoundaryLocationSelectionDialog(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData)
        {
            using (var dialog = new HydraulicBoundaryLocationSelectionDialog(Gui.MainWindow, nodeData.AssessmentSection.HydraulicBoundaryDatabase.Locations))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateGrassCoverErosionOutwardsWaveConditionsCalculations(dialog.SelectedItems,
                                                                                nodeData.WrappedData.Children,
                                                                                nodeData.AssessmentSection.FailureMechanismContribution.NormativeNorm);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateGrassCoverErosionOutwardsWaveConditionsCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                                        List<ICalculationBase> calculationCollection,
                                                                                        NormType normType)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(
                hydraulicBoundaryLocations,
                calculationCollection,
                normType);
        }

        private static void AddWaveConditionsCalculation(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData)
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children,
                                                  RiskeerCommonDataResources.Calculation_DefaultName,
                                                  c => c.Name)
            };
            WaveConditionsInputHelper.SetCategoryType(calculation.InputParameters,
                                                      nodeData.AssessmentSection.FailureMechanismContribution.NormativeNorm);
            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private static void WaveConditionsCalculationGroupContextOnNodeRemoved(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData,
                                                                               object parentNodeData)
        {
            var parentGroupContext = (GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        private static string EnableValidateAndCalculateMenuItemForCalculationGroup(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void ValidateAllInCalculationGroup(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = context.FailureMechanism;
            CalculationGroup calculationGroup = context.WrappedData;

            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculationGroup.GetCalculations().OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>())
            {
                WaveConditionsCalculationServiceBase.Validate(
                    calculation.InputParameters,
                    failureMechanism.GetAssessmentLevel(context.AssessmentSection,
                                                        calculation.InputParameters.HydraulicBoundaryLocation,
                                                        calculation.InputParameters.CategoryType),
                    context.AssessmentSection.HydraulicBoundaryDatabase,
                    failureMechanism.GetNorm(context.AssessmentSection, calculation.InputParameters.CategoryType));
            }
        }

        private void CalculateAllInCalculationGroup(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivities(context.WrappedData,
                                                                                                              context.FailureMechanism,
                                                                                                              context.AssessmentSection));
        }

        #endregion

        #region GrassCoverErosionOutwardsWaveConditionsCalculationContext TreeNodeInfo

        private static object[] WaveConditionsCalculationContextChildNodeObjects(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = context.WrappedData;

            var childNodes = new List<object>
            {
                calculation.Comments,
                new GrassCoverErosionOutwardsWaveConditionsInputContext(calculation.InputParameters,
                                                                        calculation,
                                                                        context.AssessmentSection,
                                                                        context.FailureMechanism)
            };

            if (calculation.HasOutput)
            {
                childNodes.Add(new GrassCoverErosionOutwardsWaveConditionsOutputContext(calculation.Output, calculation.InputParameters));
            }
            else
            {
                childNodes.Add(new EmptyGrassCoverErosionOutwardsOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip WaveConditionsCalculationContextMenuStrip(GrassCoverErosionOutwardsWaveConditionsCalculationContext nodeData,
                                                                           object parentData,
                                                                           TreeViewControl treeViewControl)
        {
            IInquiryHelper inquiryHelper = GetInquiryHelper();
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = nodeData.WrappedData;

            return builder
                   .AddExportItem()
                   .AddSeparator()
                   .AddDuplicateCalculationItem(calculation, nodeData)
                   .AddSeparator()
                   .AddRenameItem()
                   .AddUpdateForeshoreProfileOfCalculationItem(calculation, inquiryHelper,
                                                               SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                   .AddSeparator()
                   .AddValidateCalculationItem(
                       nodeData,
                       Validate,
                       EnableValidateAndCalculateMenuItemForCalculation)
                   .AddPerformCalculationItem<GrassCoverErosionOutwardsWaveConditionsCalculation, GrassCoverErosionOutwardsWaveConditionsCalculationContext>(
                       nodeData,
                       Calculate,
                       EnableValidateAndCalculateMenuItemForCalculation)
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

        private static string EnableValidateAndCalculateMenuItemForCalculation(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            return EnableValidateAndCalculateMenuItem(context.AssessmentSection);
        }

        private static void Validate(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            IAssessmentSection assessmentSection = context.AssessmentSection;
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = context.FailureMechanism;
            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = context.WrappedData;

            WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                          failureMechanism.GetAssessmentLevel(assessmentSection,
                                                                                              calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                              calculation.InputParameters.CategoryType),
                                                          assessmentSection.HydraulicBoundaryDatabase,
                                                          failureMechanism.GetNorm(assessmentSection, calculation.InputParameters.CategoryType));
        }

        private void Calculate(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivity(context.WrappedData,
                                                                                                                                         context.FailureMechanism,
                                                                                                                                         context.AssessmentSection));
        }

        private static void WaveConditionsCalculationContextOnNodeRemoved(GrassCoverErosionOutwardsWaveConditionsCalculationContext nodeData,
                                                                          object parentNodeData)
        {
            if (parentNodeData is GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext calculationGroupContext)
            {
                bool successfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);
                if (successfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
            }
        }

        #endregion

        private static void SetHydraulicsMenuItemEnabledStateAndTooltip(IAssessmentSection assessmentSection,
                                                                        StrictContextMenuItem menuItem)
        {
            string validationText = EnableValidateAndCalculateMenuItem(assessmentSection);
            if (!string.IsNullOrEmpty(validationText))
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = validationText;
            }
        }

        private static string EnableValidateAndCalculateMenuItem(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        #endregion
    }
}