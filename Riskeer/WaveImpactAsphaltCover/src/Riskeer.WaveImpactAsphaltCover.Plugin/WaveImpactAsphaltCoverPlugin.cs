// Copyright (C) Stichting Deltares 2019. All rights reserved.
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
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
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
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.Views;
using Riskeer.Revetment.IO.Configurations;
using Riskeer.Revetment.Service;
using Riskeer.WaveImpactAsphaltCover.Data;
using Riskeer.WaveImpactAsphaltCover.Forms;
using Riskeer.WaveImpactAsphaltCover.Forms.PresentationObjects;
using Riskeer.WaveImpactAsphaltCover.Forms.PropertyClasses;
using Riskeer.WaveImpactAsphaltCover.Forms.Views;
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
        private IInquiryHelper inquiryHelper;

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<WaveImpactAsphaltCoverFailureMechanismContext, WaveImpactAsphaltCoverFailureMechanismProperties>
            {
                CreateInstance = context => new WaveImpactAsphaltCoverFailureMechanismProperties(context.WrappedData, context.Parent)
            };
            yield return new PropertyInfo<WaveImpactAsphaltCoverWaveConditionsOutput, WaveImpactAsphaltCoverWaveConditionsOutputProperties>();
            yield return new PropertyInfo<WaveImpactAsphaltCoverWaveConditionsInputContext, WaveImpactAsphaltCoverWaveConditionsInputContextProperties>
            {
                CreateInstance = context => new WaveImpactAsphaltCoverWaveConditionsInputContextProperties(
                    context,
                    () => context.AssessmentSection.GetAssessmentLevel(context.Calculation.InputParameters.HydraulicBoundaryLocation,
                                                                       context.Calculation.InputParameters.CategoryType),
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<WaveImpactAsphaltCoverFailureMechanismContext, WaveImpactAsphaltCoverFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.CalculationIcon,
                CloseForData = CloseWaveImpactAsphaltCoverFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new WaveImpactAsphaltCoverFailureMechanismView(context.WrappedData, context.Parent)
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
                ICalculation<AssessmentSectionCategoryWaveConditionsInput>,
                WaveConditionsInputView>
            {
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new WaveConditionsInputView(
                    context.Calculation,
                    () => context.AssessmentSection.GetHydraulicBoundaryLocationCalculation(context.Calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                            context.Calculation.InputParameters.CategoryType),
                    new WaveImpactAsphaltCoverWaveConditionsInputViewStyle())
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext>(
                (context, filePath) =>
                    new AssessmentSectionCategoryWaveConditionsCalculationConfigurationImporter<WaveImpactAsphaltCoverWaveConditionsCalculation>(
                        filePath,
                        context.WrappedData,
                        context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                        context.ForeshoreProfiles,
                        context.AssessmentSection.FailureMechanismContribution.NormativeNorm));
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<WaveImpactAsphaltCoverFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

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
                WaveConditionsCalculationContextOnNodeRemoved);

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
                Name = RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) =>
                {
                    IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations = context.WrappedData.GetCalculations().Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>();
                    return new WaveImpactAsphaltCoverWaveConditionsExporter(calculations, filePath);
                },
                IsEnabled = context => context.WrappedData.GetCalculations().Cast<WaveImpactAsphaltCoverWaveConditionsCalculation>().Any(c => c.HasOutput),
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                              RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return new ExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationContext>
            {
                Name = RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) => new WaveImpactAsphaltCoverWaveConditionsExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                              RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext>(
                (context, filePath) => new AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<WaveImpactAsphaltCoverWaveConditionsCalculationContext>(
                (context, filePath) => new AssessmentSectionCategoryWaveConditionsCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                WaveImpactAsphaltCoverFailureMechanismSectionsContext, WaveImpactAsphaltCoverFailureMechanism, WaveImpactAsphaltCoverFailureMechanismSectionResult>(
                new WaveImpactAsphaltCoverFailureMechanismSectionResultUpdateStrategy());
        }

        private void CalculateAll(WaveImpactAsphaltCoverFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                                           context.Parent));
        }

        private void CalculateAll(CalculationGroup group, WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(group,
                                                                                                           context.FailureMechanism,
                                                                                                           context.AssessmentSection));
        }

        private static void ValidateAll(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<WaveImpactAsphaltCoverWaveConditionsCalculation>(),
                        context.AssessmentSection);
        }

        private static void ValidateAll(IEnumerable<WaveImpactAsphaltCoverWaveConditionsCalculation> calculations, IAssessmentSection assessmentSection)
        {
            foreach (WaveImpactAsphaltCoverWaveConditionsCalculation calculation in calculations)
            {
                WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                              assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                                   calculation.InputParameters.CategoryType),
                                                              assessmentSection.HydraulicBoundaryDatabase,
                                                              assessmentSection.GetNorm(calculation.InputParameters.CategoryType));
            }
        }

        #region ViewInfos

        #region WaveImpactAsphaltCoverFailureMechanismView ViewInfo

        private static bool CloseWaveImpactAsphaltCoverFailureMechanismViewForData(WaveImpactAsphaltCoverFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as WaveImpactAsphaltCoverFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        #endregion

        #region FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>

        private static bool CloseFailureMechanismResultViewForData(WaveImpactAsphaltCoverFailureMechanismResultView view, object dataToCloseFor)
        {
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as WaveImpactAsphaltCoverFailureMechanism;
            var failureMechanismContext = dataToCloseFor as IFailureMechanismContext<WaveImpactAsphaltCoverFailureMechanism>;

            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<WaveImpactAsphaltCoverFailureMechanism>()
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

        #region WaveImpactAsphaltCoverFailureMechanismContext

        private static object[] FailureMechanismEnabledChildNodeObjects(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext)
        {
            WaveImpactAsphaltCoverFailureMechanism wrappedData = failureMechanismContext.WrappedData;
            IAssessmentSection assessmentSection = failureMechanismContext.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, assessmentSection), TreeFolderCategory.Input),
                new WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext(wrappedData.WaveConditionsCalculationGroup, null, wrappedData, assessmentSection),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                failureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(WaveImpactAsphaltCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new WaveImpactAsphaltCoverFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(WaveImpactAsphaltCoverFailureMechanism failureMechanism,
                                                      IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralWaveImpactAsphaltCoverInput.GetN(
                                                                  assessmentSection.ReferenceLine.Length)),
                new FailureMechanismSectionResultContext<WaveImpactAsphaltCoverFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

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

        private void RemoveAllViewsForItem(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(WaveImpactAsphaltCoverFailureMechanismContext failureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

        #endregion

        #region WaveImpactAsphaltCover TreeNodeInfo

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
                   .AddCreateCalculationItem(nodeData, AddWaveConditionsCalculation)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddUpdateForeshoreProfileOfCalculationsItem(calculations, GetInquiryHelper(),
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

        private static string ValidateAllDataAvailableAndGetErrorMessageForFailureMechanism(WaveImpactAsphaltCoverFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup(WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculation(WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
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
            WaveConditionsInputHelper.SetCategoryType(calculation.InputParameters,
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

        #endregion

        #region WaveImpactAsphaltCoverWaveConditionsCalculationContext

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

        private static void Validate(WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            IAssessmentSection assessmentSection = context.AssessmentSection;
            WaveImpactAsphaltCoverWaveConditionsCalculation calculation = context.WrappedData;

            WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                          assessmentSection.GetAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                               calculation.InputParameters.CategoryType),
                                                          assessmentSection.HydraulicBoundaryDatabase,
                                                          assessmentSection.GetNorm(calculation.InputParameters.CategoryType));
        }

        private void PerformCalculation(WaveImpactAsphaltCoverWaveConditionsCalculation calculation,
                                        WaveImpactAsphaltCoverWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             WaveImpactAsphaltCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                                      context.FailureMechanism,
                                                                                                                                      context.AssessmentSection));
        }

        private static void WaveConditionsCalculationContextOnNodeRemoved(WaveImpactAsphaltCoverWaveConditionsCalculationContext nodeData, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as WaveImpactAsphaltCoverWaveConditionsCalculationGroupContext;
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

        private IInquiryHelper GetInquiryHelper()
        {
            return inquiryHelper ?? (inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow));
        }
    }
}