﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2024. All rights reserved.
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
using Riskeer.Common.Data.AssemblyTool;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Plugin;
using Riskeer.Common.Plugin.FileImporters;
using Riskeer.Common.Service;
using Riskeer.Common.Util.Helpers;
using Riskeer.Revetment.Data;
using Riskeer.Revetment.Forms.Views;
using Riskeer.Revetment.Service;
using Riskeer.StabilityStoneCover.Data;
using Riskeer.StabilityStoneCover.Forms;
using Riskeer.StabilityStoneCover.Forms.PresentationObjects;
using Riskeer.StabilityStoneCover.Forms.PropertyClasses;
using Riskeer.StabilityStoneCover.Forms.Views;
using Riskeer.StabilityStoneCover.IO.Configurations;
using Riskeer.StabilityStoneCover.IO.Exporters;
using Riskeer.StabilityStoneCover.Service;
using HydraulicLoadsStateFailureMechanismContext = Riskeer.StabilityStoneCover.Forms.PresentationObjects.HydraulicLoadsState.StabilityStoneCoverFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.StabilityStoneCover.Forms.PresentationObjects.RegistrationState.StabilityStoneCoverFailureMechanismContext;
using HydraulicLoadsStateFailureMechanismProperties = Riskeer.StabilityStoneCover.Forms.PropertyClasses.HydraulicLoadsState.StabilityStoneCoverFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.StabilityStoneCover.Forms.PropertyClasses.RegistrationState.StabilityStoneCoverFailureMechanismProperties;
using HydraulicLoadsStateFailureMechanismView = Riskeer.StabilityStoneCover.Forms.Views.HydraulicLoadsState.StabilityStoneCoverFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.StabilityStoneCover.Forms.Views.RegistrationState.StabilityStoneCoverFailureMechanismView;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;

namespace Riskeer.StabilityStoneCover.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="StabilityStoneCoverFailureMechanism"/>.
    /// </summary>
    public class StabilityStoneCoverPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<HydraulicLoadsStateFailureMechanismContext, HydraulicLoadsStateFailureMechanismProperties>
            {
                CreateInstance = context => new HydraulicLoadsStateFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<RegistrationStateFailureMechanismContext, RegistrationStateFailureMechanismProperties>
            {
                CreateInstance = context => new RegistrationStateFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<StabilityStoneCoverWaveConditionsOutputContext, StabilityStoneCoverWaveConditionsOutputProperties>
            {
                CreateInstance = context => new StabilityStoneCoverWaveConditionsOutputProperties(context.WrappedData, context.Input)
            };
            yield return new PropertyInfo<StabilityStoneCoverWaveConditionsInputContext, StabilityStoneCoverWaveConditionsInputContextProperties>
            {
                CreateInstance = context => new StabilityStoneCoverWaveConditionsInputContextProperties(
                    context,
                    () => WaveConditionsInputHelper.GetAssessmentLevel(context.WrappedData, context.AssessmentSection),
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new RiskeerViewInfo<HydraulicLoadsStateFailureMechanismContext, HydraulicLoadsStateFailureMechanismView>(() => Gui)
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                CreateInstance = context => new HydraulicLoadsStateFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new RiskeerViewInfo<RegistrationStateFailureMechanismContext, RegistrationStateFailureMechanismView>(() => Gui)
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                AdditionalDataCheck = context => context.WrappedData.InAssembly,
                CreateInstance = context => new RegistrationStateFailureMechanismView(context.WrappedData, context.Parent),
                CloseForData = CloseFailureMechanismViewForData
            };

            yield return new RiskeerViewInfo<StabilityStoneCoverFailureMechanismSectionResultContext,
                IObservableEnumerable<NonAdoptableFailureMechanismSectionResult>,
                NonAdoptableFailureMechanismResultView<StabilityStoneCoverFailureMechanism>>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context =>
                {
                    var failureMechanism = (StabilityStoneCoverFailureMechanism) context.FailureMechanism;
                    IAssessmentSection assessmentSection = context.AssessmentSection;

                    return new NonAdoptableFailureMechanismResultView<StabilityStoneCoverFailureMechanism>(
                        context.WrappedData,
                        failureMechanism,
                        assessmentSection,
                        FailureMechanismAssemblyFactory.AssembleFailureMechanism);
                }
            };

            yield return new RiskeerViewInfo<StabilityStoneCoverWaveConditionsInputContext,
                StabilityStoneCoverWaveConditionsCalculation,
                WaveConditionsInputView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new WaveConditionsInputView(
                    context.Calculation,
                    () => WaveConditionsInputHelper.GetHydraulicBoundaryLocationCalculation(context.WrappedData, context.AssessmentSection),
                    new StabilityStoneCoverWaveConditionsInputViewStyle())
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<HydraulicLoadsStateFailureMechanismContext>(
                HydraulicLoadsStateFailureMechanismChildNodeObjects,
                HydraulicLoadsStateFailureMechanismContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<RegistrationStateFailureMechanismContext>(
                RegistrationStateFailureMechanismEnabledChildNodeObjects,
                RegistrationStateFailureMechanismDisabledChildNodeObjects,
                RegistrationStateFailureMechanismEnabledContextMenuStrip,
                RegistrationStateFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<StabilityStoneCoverCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<StabilityStoneCoverWaveConditionsCalculationContext>(
                WaveConditionsCalculationContextChildNodeObjects,
                WaveConditionsCalculationContextContextMenuStrip,
                WaveConditionsCalculationContextOnNodeRemoved,
                CalculationType.Hydraulic);

            yield return new TreeNodeInfo<EmptyStabilityStoneCoverOutput>
            {
                Text = emptyOutput => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RiskeerCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityStoneCoverWaveConditionsOutputContext>
            {
                Text = context => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityStoneCoverFailureMechanismSectionResultContext>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityStoneCoverWaveConditionsInputContext>
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

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<StabilityStoneCoverCalculationGroupContext>(
                (context, filePath) =>
                    new StabilityStoneCoverWaveConditionsCalculationConfigurationImporter(
                        filePath,
                        context.WrappedData,
                        context.AssessmentSection.HydraulicBoundaryData.GetLocations(),
                        context.ForeshoreProfiles,
                        context.AssessmentSection.FailureMechanismContribution,
                        context.AssessmentSection.WaterLevelCalculationsForUserDefinedTargetProbabilities));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<StabilityStoneCoverCalculationGroupContext>
            {
                Name = context => RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                Extension = RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                CreateFileExporter = (context, filePath) =>
                    new StabilityStoneCoverWaveConditionsExporter(context.WrappedData.GetCalculations().Cast<StabilityStoneCoverWaveConditionsCalculation>(), filePath,
                                                                  input => WaveConditionsInputHelper.GetTargetProbability(input, context.AssessmentSection)
                                                                                                    .ToString(CultureInfo.InvariantCulture)),
                IsEnabled = context => context.WrappedData.GetCalculations().Cast<StabilityStoneCoverWaveConditionsCalculation>().Any(c => c.HasOutput),
                GetExportPath = context => ExportHelper.GetFilePath(GetInquiryHelper(), GetWaveConditionsFileFilterGenerator())
            };

            yield return new ExportInfo<StabilityStoneCoverWaveConditionsCalculationContext>
            {
                Name = context => RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                Extension = RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                CreateFileExporter = (context, filePath) => new StabilityStoneCoverWaveConditionsExporter(new[]
                                                                                                          {
                                                                                                              context.WrappedData
                                                                                                          }, filePath,
                                                                                                          input =>
                                                                                                              WaveConditionsInputHelper.GetTargetProbability(input, context.AssessmentSection)
                                                                                                                                       .ToString(CultureInfo.InvariantCulture)),
                IsEnabled = context => context.WrappedData.HasOutput,
                GetExportPath = context => ExportHelper.GetFilePath(GetInquiryHelper(), GetWaveConditionsFileFilterGenerator())
            };

            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<StabilityStoneCoverCalculationGroupContext>(
                (context, filePath) => new StabilityStoneCoverWaveConditionsCalculationConfigurationExporter(
                    context.WrappedData.Children, filePath, context.AssessmentSection),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<StabilityStoneCoverWaveConditionsCalculationContext>(
                (context, filePath) => new StabilityStoneCoverWaveConditionsCalculationConfigurationExporter(
                    new[]
                    {
                        context.WrappedData
                    }, filePath, context.AssessmentSection),
                GetInquiryHelper());
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                StabilityStoneCoverFailureMechanismSectionsContext, StabilityStoneCoverFailureMechanism, NonAdoptableFailureMechanismSectionResult>(
                new NonAdoptableFailureMechanismSectionResultUpdateStrategy());
        }

        private static FileFilterGenerator GetWaveConditionsFileFilterGenerator()
        {
            return new FileFilterGenerator(
                RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description);
        }

        #region ViewInfos

        private static bool CloseFailureMechanismViewForData(RegistrationStateFailureMechanismView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as StabilityStoneCoverFailureMechanism;

            return dataToCloseFor is IAssessmentSection assessmentSection
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        private static bool CloseFailureMechanismResultViewForData(NonAdoptableFailureMechanismResultView<StabilityStoneCoverFailureMechanism> view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as StabilityStoneCoverFailureMechanism;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<StabilityStoneCoverFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is IFailureMechanismContext<StabilityStoneCoverFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #region TreeNodeInfos

        #region HydraulicLoadsStateFailureMechanismContext TreeNodeInfo

        private static object[] HydraulicLoadsStateFailureMechanismChildNodeObjects(HydraulicLoadsStateFailureMechanismContext context)
        {
            StabilityStoneCoverFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetHydraulicLoadsStateFailureMechanismInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new StabilityStoneCoverCalculationGroupContext(failureMechanism.CalculationsGroup, null,
                                                               failureMechanism, assessmentSection)
            };
        }

        private static IEnumerable<object> GetHydraulicLoadsStateFailureMechanismInputs(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                failureMechanism.CalculationsInputComments
            };
        }

        private ContextMenuStrip HydraulicLoadsStateFailureMechanismContextMenuStrip(HydraulicLoadsStateFailureMechanismContext context,
                                                                                     object parentData,
                                                                                     TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              context,
                              CalculateAllInFailureMechanism)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void CalculateAllInFailureMechanism(HydraulicLoadsStateFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                                        context.Parent));
        }

        #endregion

        #region RegistrationStateFailureMechanismContext TreeNodeInfo

        private static object[] RegistrationStateFailureMechanismEnabledChildNodeObjects(RegistrationStateFailureMechanismContext context)
        {
            StabilityStoneCoverFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetRegistrationStateFailureMechanismInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetRegistrationStateFailureMechanismOutputs(failureMechanism, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] RegistrationStateFailureMechanismDisabledChildNodeObjects(RegistrationStateFailureMechanismContext context)
        {
            return new object[]
            {
                context.WrappedData.NotInAssemblyComments
            };
        }

        private static IEnumerable<object> GetRegistrationStateFailureMechanismInputs(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new StabilityStoneCoverFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetRegistrationStateFailureMechanismOutputs(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new StabilityStoneCoverFailureMechanismSectionResultContext(failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.InAssemblyOutputComments
            };
        }

        private ContextMenuStrip RegistrationStateFailureMechanismEnabledContextMenuStrip(RegistrationStateFailureMechanismContext context,
                                                                                          object parentData,
                                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleInAssemblyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip RegistrationStateFailureMechanismDisabledContextMenuStrip(RegistrationStateFailureMechanismContext context,
                                                                                           object parentData,
                                                                                           TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddToggleInAssemblyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(RegistrationStateFailureMechanismContext context)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(context);
        }

        #endregion

        #region StabilityStoneCoverCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(StabilityStoneCoverCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                if (item is StabilityStoneCoverWaveConditionsCalculation calculation)
                {
                    childNodeObjects.Add(new StabilityStoneCoverWaveConditionsCalculationContext(calculation,
                                                                                                 nodeData.WrappedData,
                                                                                                 nodeData.FailureMechanism,
                                                                                                 nodeData.AssessmentSection));
                }
                else if (item is CalculationGroup group)
                {
                    childNodeObjects.Add(new StabilityStoneCoverCalculationGroupContext(group,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(StabilityStoneCoverCalculationGroupContext nodeData,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = nodeData.WrappedData;
            IInquiryHelper inquiryHelper = GetInquiryHelper();

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            bool isNestedGroup = parentData is StabilityStoneCoverCalculationGroupContext;

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
                                                          ValidateAllInCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(nodeData,
                                                         CalculateAllInCalculationGroup)
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

        private StrictContextMenuItem CreateGenerateWaveConditionsCalculationsItem(StabilityStoneCoverCalculationGroupContext nodeData)
        {
            bool locationsAvailable = nodeData.AssessmentSection.HydraulicBoundaryData.GetLocations().Any();

            string calculationGroupContextToolTip = locationsAvailable
                                                        ? RiskeerCommonFormsResources.CalculationGroup_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip
                                                        : RiskeerCommonFormsResources.CalculationGroup_No_HydraulicBoundaryDatabase_To_Generate_ToolTip;

            return new StrictContextMenuItem(RiskeerCommonFormsResources.CalculationGroup_Generate_calculations,
                                             calculationGroupContextToolTip,
                                             RiskeerCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowHydraulicBoundaryLocationSelectionDialog(nodeData))
            {
                Enabled = locationsAvailable
            };
        }

        private void ShowHydraulicBoundaryLocationSelectionDialog(StabilityStoneCoverCalculationGroupContext nodeData)
        {
            using (var dialog = new HydraulicBoundaryLocationSelectionDialog(Gui.MainWindow, nodeData.AssessmentSection.HydraulicBoundaryData.GetLocations()))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateStabilityStoneCoverCalculations(dialog.SelectedItems,
                                                            nodeData.WrappedData.Children,
                                                            nodeData.AssessmentSection.FailureMechanismContribution.NormativeProbabilityType);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateStabilityStoneCoverCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                    List<ICalculationBase> calculationCollection,
                                                                    NormativeProbabilityType normativeProbabilityType)
        {
            StabilityStoneCoverCalculationConfigurationHelper.AddCalculationsFromLocations(
                hydraulicBoundaryLocations,
                calculationCollection,
                normativeProbabilityType);
        }

        private static void AddWaveConditionsCalculation(StabilityStoneCoverCalculationGroupContext nodeData)
        {
            var calculation = new StabilityStoneCoverWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children,
                                                  RiskeerCommonDataResources.Calculation_DefaultName,
                                                  c => c.Name)
            };
            WaveConditionsInputHelper.SetWaterLevelType(calculation.InputParameters,
                                                        nodeData.AssessmentSection.FailureMechanismContribution.NormativeProbabilityType);

            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private static void CalculationGroupContextOnNodeRemoved(StabilityStoneCoverCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (StabilityStoneCoverCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        private static void ValidateAllInCalculationGroup(StabilityStoneCoverCalculationGroupContext context)
        {
            foreach (StabilityStoneCoverWaveConditionsCalculation calculation in context.WrappedData.GetCalculations().OfType<StabilityStoneCoverWaveConditionsCalculation>())
            {
                WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                              WaveConditionsInputHelper.GetAssessmentLevel(calculation.InputParameters, context.AssessmentSection),
                                                              context.AssessmentSection.HydraulicBoundaryData);
            }
        }

        private void CalculateAllInCalculationGroup(StabilityStoneCoverCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                                        context.FailureMechanism,
                                                                                                        context.AssessmentSection));
        }

        #endregion

        #region StabilityStoneCoverWaveConditionsCalculationContext TreeNodeInfo

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
                childNodes.Add(new StabilityStoneCoverWaveConditionsOutputContext(calculation.Output, calculation.InputParameters));
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
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            StabilityStoneCoverWaveConditionsCalculation calculation = nodeData.WrappedData;
            return builder
                   .AddExportItem()
                   .AddSeparator()
                   .AddDuplicateCalculationItem(calculation, nodeData)
                   .AddSeparator()
                   .AddRenameItem()
                   .AddUpdateForeshoreProfileOfCalculationItem(calculation, GetInquiryHelper(),
                                                               SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                   .AddSeparator()
                   .AddValidateCalculationItem(nodeData,
                                               Validate)
                   .AddPerformCalculationItem<StabilityStoneCoverWaveConditionsCalculation, StabilityStoneCoverWaveConditionsCalculationContext>(nodeData,
                                                                                                                                                 Calculate)
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

            WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                          WaveConditionsInputHelper.GetAssessmentLevel(calculation.InputParameters, context.AssessmentSection),
                                                          assessmentSection.HydraulicBoundaryData);
        }

        private void Calculate(StabilityStoneCoverWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             StabilityStoneCoverWaveConditionsCalculationActivityFactory.CreateCalculationActivity(
                                                 context.WrappedData,
                                                 context.FailureMechanism,
                                                 context.AssessmentSection));
        }

        private static void WaveConditionsCalculationContextOnNodeRemoved(StabilityStoneCoverWaveConditionsCalculationContext nodeData, object parentNodeData)
        {
            if (parentNodeData is StabilityStoneCoverCalculationGroupContext calculationGroupContext)
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