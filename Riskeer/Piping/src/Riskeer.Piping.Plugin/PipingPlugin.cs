// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Util;
using Core.Gui.ContextMenu;
using Core.Gui.Forms.ProgressDialog;
using Core.Gui.Helpers;
using Core.Gui.Plugin;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SurfaceLines;
using Riskeer.Common.Plugin;
using Riskeer.Common.Service;
using Riskeer.Common.Util.Helpers;
using Riskeer.Piping.Data;
using Riskeer.Piping.Data.Probabilistic;
using Riskeer.Piping.Data.SemiProbabilistic;
using Riskeer.Piping.Data.SoilProfile;
using Riskeer.Piping.Forms;
using Riskeer.Piping.Forms.ChangeHandlers;
using Riskeer.Piping.Forms.PresentationObjects;
using Riskeer.Piping.Forms.PresentationObjects.Probabilistic;
using Riskeer.Piping.Forms.PresentationObjects.SemiProbabilistic;
using Riskeer.Piping.Forms.PropertyClasses;
using Riskeer.Piping.Forms.PropertyClasses.Probabilistic;
using Riskeer.Piping.Forms.PropertyClasses.SemiProbabilistic;
using Riskeer.Piping.Forms.Views;
using Riskeer.Piping.IO.Configurations;
using Riskeer.Piping.Plugin.FileImporter;
using Riskeer.Piping.Plugin.ImportInfos;
using Riskeer.Piping.Plugin.Properties;
using Riskeer.Piping.Plugin.UpdateInfos;
using Riskeer.Piping.Primitives;
using Riskeer.Piping.Service;
using Riskeer.Piping.Service.Probabilistic;
using Riskeer.Piping.Service.SemiProbabilistic;
using Riskeer.Piping.Util;
using CalculationsStateFailureMechanismContext = Riskeer.Piping.Forms.PresentationObjects.CalculationsState.PipingFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.Piping.Forms.PresentationObjects.RegistrationState.PipingFailureMechanismContext;
using CalculationsStateFailureMechanismProperties = Riskeer.Piping.Forms.PropertyClasses.CalculationsState.PipingFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.Piping.Forms.PropertyClasses.RegistrationState.PipingFailureMechanismProperties;
using CalculationsStateFailureMechanismView = Riskeer.Piping.Forms.Views.CalculationsState.PipingFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.Piping.Forms.Views.RegistrationState.PipingFailureMechanismView;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using PipingFormsResources = Riskeer.Piping.Forms.Properties.Resources;

namespace Riskeer.Piping.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class PipingPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<CalculationsStateFailureMechanismContext, CalculationsStateFailureMechanismProperties>
            {
                CreateInstance = context => new CalculationsStateFailureMechanismProperties(context.WrappedData, new FailureMechanismPropertyChangeHandler<PipingFailureMechanism>())
            };
            yield return new PropertyInfo<RegistrationStateFailureMechanismContext, RegistrationStateFailureMechanismProperties>
            {
                CreateInstance = context => new RegistrationStateFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<SemiProbabilisticPipingInputContext, SemiProbabilisticPipingInputContextProperties>
            {
                CreateInstance = context => new SemiProbabilisticPipingInputContextProperties(
                    context, () => GetNormativeAssessmentLevel(context.AssessmentSection, context.PipingCalculation),
                    new ObservablePropertyChangeHandler(context.PipingCalculation, context.WrappedData))
            };
            yield return new PropertyInfo<ProbabilisticPipingInputContext, ProbabilisticPipingInputContextProperties>
            {
                CreateInstance = context => new ProbabilisticPipingInputContextProperties(context, new ObservablePropertyChangeHandler(context.PipingCalculation, context.WrappedData))
            };
            yield return new PropertyInfo<SemiProbabilisticPipingOutputContext, SemiProbabilisticPipingOutputProperties>
            {
                CreateInstance = context => new SemiProbabilisticPipingOutputProperties(context.WrappedData, context.AssessmentSection.FailureMechanismContribution.NormativeProbability)
            };
            yield return new PropertyInfo<PipingSurfaceLinesContext, PipingSurfaceLineCollectionProperties>
            {
                CreateInstance = context => new PipingSurfaceLineCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<PipingSurfaceLine, PipingSurfaceLineProperties>();
            yield return new PropertyInfo<PipingStochasticSoilModelCollectionContext, PipingStochasticSoilModelCollectionProperties>
            {
                CreateInstance = context => new PipingStochasticSoilModelCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<PipingStochasticSoilModel, PipingStochasticSoilModelProperties>
            {
                CreateInstance = stochasticSoilModel => new PipingStochasticSoilModelProperties(stochasticSoilModel)
            };
            yield return new PropertyInfo<PipingStochasticSoilProfile, PipingStochasticSoilProfileProperties>
            {
                CreateInstance = stochasticSoilProfile => new PipingStochasticSoilProfileProperties(stochasticSoilProfile)
            };
            yield return new PropertyInfo<PipingFailureMechanismSectionsContext, FailureMechanismSectionsProperties>
            {
                CreateInstance = context => new FailureMechanismSectionsProperties(context.WrappedData)
            };
            yield return new PropertyInfo<ProbabilisticPipingProfileSpecificOutputContext, ProbabilisticPipingOutputProperties>
            {
                CreateInstance = context => CreateProbabilisticPipingOutputProperties(context.WrappedData.Output?.ProfileSpecificOutput)
            };
            yield return new PropertyInfo<ProbabilisticPipingSectionSpecificOutputContext, ProbabilisticPipingOutputProperties>
            {
                CreateInstance = context => CreateProbabilisticPipingOutputProperties(context.WrappedData.Output?.SectionSpecificOutput)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<PipingSurfaceLinesContext>
            {
                Name = RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = PipingFormsResources.PipingSurfaceLineIcon,
                FileFilterGenerator = PipingSurfaceLineFileFilter,
                IsEnabled = context => HasGeometry(context.AssessmentSection.ReferenceLine),
                CreateFileImporter = (context, filePath) => new SurfaceLinesCsvImporter<PipingSurfaceLine>(
                    context.WrappedData,
                    filePath,
                    new ImportMessageProvider(),
                    SurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(context.FailureMechanism, context.AssessmentSection.ReferenceLine)),
                VerifyUpdates = context => VerifyPipingSurfaceLineUpdates(context, Resources.PipingPlugin_VerifyPipingSurfaceLineImport_When_importing_surface_lines_calculation_output_will_be_cleared_confirm)
            };

            yield return new ImportInfo<PipingStochasticSoilModelCollectionContext>
            {
                Name = RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = PipingFormsResources.PipingSoilProfileIcon,
                FileFilterGenerator = StochasticSoilModelFileFilter,
                IsEnabled = context => HasGeometry(context.AssessmentSection.ReferenceLine),
                CreateFileImporter = (context, filePath) => new StochasticSoilModelImporter<PipingStochasticSoilModel>(
                    context.WrappedData,
                    filePath,
                    new ImportMessageProvider(),
                    PipingStochasticSoilModelImporterConfigurationFactory.CreateReplaceStrategyConfiguration(context.FailureMechanism)
                ),
                VerifyUpdates = context => VerifyStochasticSoilModelUpdates(context, Resources.PipingPlugin_VerifyStochasticSoilModelImport_When_importing_StochasticSoilModels_calculation_output_will_be_cleared_confirm)
            };

            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<PipingCalculationGroupContext>(
                (context, filePath) =>
                    new PipingCalculationConfigurationImporter(
                        filePath,
                        context.WrappedData,
                        context.AssessmentSection.HydraulicBoundaryData.GetLocations(),
                        context.FailureMechanism));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<PipingCalculationGroupContext>(
                (context, filePath) => new PipingCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<SemiProbabilisticPipingCalculationScenarioContext>(
                (context, filePath) => new PipingCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<ProbabilisticPipingCalculationScenarioContext>(
                (context, filePath) => new PipingCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<PipingSurfaceLinesContext>
            {
                Name = RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = PipingFormsResources.PipingSurfaceLineIcon,
                FileFilterGenerator = PipingSurfaceLineFileFilter,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                CreateFileImporter = (context, filePath) => new SurfaceLinesCsvImporter<PipingSurfaceLine>(
                    context.WrappedData,
                    filePath,
                    new UpdateMessageProvider(),
                    SurfaceLinesCsvImporterConfigurationFactory.CreateUpdateStrategyConfiguration(context.FailureMechanism, context.AssessmentSection.ReferenceLine)),
                VerifyUpdates = context => VerifyPipingSurfaceLineUpdates(context, Resources.PipingPlugin_VerifyPipingSurfaceLineUpdates_When_updating_surface_lines_definitions_assigned_to_calculation_output_will_be_cleared_confirm)
            };

            yield return new UpdateInfo<PipingStochasticSoilModelCollectionContext>
            {
                Name = RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = PipingFormsResources.PipingSoilProfileIcon,
                FileFilterGenerator = StochasticSoilModelFileFilter,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                CreateFileImporter = (context, filePath) => new StochasticSoilModelImporter<PipingStochasticSoilModel>(
                    context.WrappedData,
                    filePath,
                    new UpdateMessageProvider(),
                    PipingStochasticSoilModelImporterConfigurationFactory.CreateUpdateStrategyConfiguration(context.FailureMechanism)
                ),
                VerifyUpdates = context => VerifyStochasticSoilModelUpdates(context, Resources.PipingPlugin_VerifyStochasticSoilModelUpdates_When_updating_StochasticSoilModel_definitions_assigned_to_calculation_output_will_be_cleared_confirm)
            };

            yield return PipingUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo(GetInquiryHelper());
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new RiskeerViewInfo<CalculationsStateFailureMechanismContext, CalculationsStateFailureMechanismView>(() => Gui)
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                CreateInstance = context => new CalculationsStateFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new RiskeerViewInfo<RegistrationStateFailureMechanismContext, RegistrationStateFailureMechanismView>(() => Gui)
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                AdditionalDataCheck = context => context.WrappedData.InAssembly,
                CreateInstance = context => new RegistrationStateFailureMechanismView(context.WrappedData, context.Parent),
                CloseForData = ClosePipingFailureMechanismViewForData
            };

            yield return new RiskeerViewInfo<
                PipingFailureMechanismSectionResultContext,
                IObservableEnumerable<AdoptableFailureMechanismSectionResult>,
                PipingFailureMechanismResultView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new PipingFailureMechanismResultView(
                    context.WrappedData,
                    (PipingFailureMechanism) context.FailureMechanism,
                    context.AssessmentSection)
            };

            yield return new RiskeerViewInfo<PipingCalculationGroupContext, CalculationGroup, PipingCalculationsView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => context.WrappedData.Name,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CreateInstance = context => new PipingCalculationsView(context.WrappedData, context.FailureMechanism, context.AssessmentSection),
                CloseForData = ClosePipingCalculationsViewForData
            };

            yield return new RiskeerViewInfo<SemiProbabilisticPipingInputContext, SemiProbabilisticPipingCalculationScenario, PipingInputView>(() => Gui)
            {
                GetViewData = context => context.PipingCalculation,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                CloseForData = ClosePipingInputViewForData
            };

            yield return new RiskeerViewInfo<ProbabilisticPipingInputContext, ProbabilisticPipingCalculationScenario, PipingInputView>(() => Gui)
            {
                GetViewData = context => context.PipingCalculation,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                CloseForData = ClosePipingInputViewForData
            };

            yield return new RiskeerViewInfo<PipingScenariosContext, CalculationGroup, PipingScenariosView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Scenarios_DisplayName,
                CloseForData = ClosePipingScenariosViewForData,
                CreateInstance = context => new PipingScenariosView(context.WrappedData, context.FailureMechanism, context.AssessmentSection)
            };

            yield return new RiskeerViewInfo<PipingFailureMechanismSectionsContext, IEnumerable<FailureMechanismSection>, FailureMechanismSectionsView>(() => Gui)
            {
                GetViewData = context => context.WrappedData.Sections,
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView,
                CreateInstance = context => new FailureMechanismSectionsView(context.WrappedData.Sections, context.WrappedData)
            };

            yield return new RiskeerViewInfo<ProbabilisticPipingProfileSpecificOutputContext, ProbabilisticPipingCalculationScenario, ProbabilisticFaultTreePipingOutputView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => PipingFormsResources.ProbabilisticProfileSpecificOutput_DisplayName,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                AdditionalDataCheck = context => !context.WrappedData.HasOutput || context.WrappedData.Output.ProfileSpecificOutput is PartialProbabilisticFaultTreePipingOutput,
                CreateInstance = context => new ProbabilisticFaultTreePipingOutputView(
                    context.WrappedData,
                    () => ((PartialProbabilisticFaultTreePipingOutput) context.WrappedData.Output?.ProfileSpecificOutput)?.GeneralResult)
            };

            yield return new RiskeerViewInfo<ProbabilisticPipingProfileSpecificOutputContext, ProbabilisticPipingCalculationScenario, ProbabilisticSubMechanismPipingOutputView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => PipingFormsResources.ProbabilisticProfileSpecificOutput_DisplayName,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                AdditionalDataCheck = context => context.WrappedData.HasOutput && context.WrappedData.Output.ProfileSpecificOutput is PartialProbabilisticSubMechanismPipingOutput,
                CreateInstance = context =>
                {
                    return new ProbabilisticSubMechanismPipingOutputView(
                        context.WrappedData,
                        () => ((PartialProbabilisticSubMechanismPipingOutput) context.WrappedData.Output?.ProfileSpecificOutput)?.GeneralResult);
                }
            };

            yield return new RiskeerViewInfo<ProbabilisticPipingSectionSpecificOutputContext, ProbabilisticPipingCalculationScenario, ProbabilisticFaultTreePipingOutputView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => PipingFormsResources.ProbabilisticSectionSpecificOutput_DisplayName,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                AdditionalDataCheck = context => !context.WrappedData.HasOutput || context.WrappedData.Output.SectionSpecificOutput is PartialProbabilisticFaultTreePipingOutput,
                CreateInstance = context =>
                {
                    return new ProbabilisticFaultTreePipingOutputView(
                        context.WrappedData,
                        () => ((PartialProbabilisticFaultTreePipingOutput) context.WrappedData.Output?.SectionSpecificOutput)?.GeneralResult);
                }
            };

            yield return new RiskeerViewInfo<ProbabilisticPipingSectionSpecificOutputContext, ProbabilisticPipingCalculationScenario, ProbabilisticSubMechanismPipingOutputView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => PipingFormsResources.ProbabilisticSectionSpecificOutput_DisplayName,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                AdditionalDataCheck = context => context.WrappedData.HasOutput && context.WrappedData.Output.SectionSpecificOutput is PartialProbabilisticSubMechanismPipingOutput,
                CreateInstance = context =>
                {
                    return new ProbabilisticSubMechanismPipingOutputView(
                        context.WrappedData,
                        () => ((PartialProbabilisticSubMechanismPipingOutput) context.WrappedData.Output?.SectionSpecificOutput)?.GeneralResult);
                }
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismStateContextTreeNodeInfo<CalculationsStateFailureMechanismContext>(
                CalculationsStateFailureMechanismChildNodeObjects,
                CalculationsStateFailureMechanismContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateRegistrationStateContextTreeNodeInfo<RegistrationStateFailureMechanismContext>(
                RegistrationStateFailureMechanismEnabledChildNodeObjects,
                RegistrationStateFailureMechanismDisabledChildNodeObjects,
                RegistrationStateFailureMechanismEnabledContextMenuStrip,
                RegistrationStateFailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<SemiProbabilisticPipingCalculationScenarioContext>(
                SemiProbabilisticPipingCalculationScenarioContextChildNodeObjects,
                SemiProbabilisticPipingCalculationScenarioContextContextMenuStrip,
                SemiProbabilisticPipingCalculationScenarioContextOnNodeRemoved,
                CalculationType.SemiProbabilistic);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<ProbabilisticPipingCalculationScenarioContext>(
                ProbabilisticPipingCalculationScenarioContextChildNodeObjects,
                ProbabilisticPipingCalculationScenarioContextContextMenuStrip,
                ProbabilisticPipingCalculationScenarioContextOnNodeRemoved,
                CalculationType.Probabilistic);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<PipingCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return new TreeNodeInfo<PipingFailureMechanismSectionResultContext>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<SemiProbabilisticPipingInputContext>
            {
                Text = pipingInputContext => RiskeerCommonFormsResources.Calculation_Input,
                Image = pipingInputContext => PipingFormsResources.PipingInputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ProbabilisticPipingInputContext>
            {
                Text = pipingInputContext => RiskeerCommonFormsResources.Calculation_Input,
                Image = pipingInputContext => PipingFormsResources.PipingInputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingSurfaceLinesContext>
            {
                Text = pipingSurfaceLine => RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Image = pipingSurfaceLine => RiskeerCommonFormsResources.GeneralFolderIcon,
                ForeColor = pipingSurfaceLine => pipingSurfaceLine.WrappedData.Any()
                                                     ? Color.FromKnownColor(KnownColor.ControlText)
                                                     : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = pipingSurfaceLine => pipingSurfaceLine.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = PipingSurfaceLinesContextContextMenuStrip
            };

            yield return new TreeNodeInfo<PipingSurfaceLine>
            {
                Text = pipingSurfaceLine => pipingSurfaceLine.Name,
                Image = pipingSurfaceLine => PipingFormsResources.PipingSurfaceLineIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingStochasticSoilModelCollectionContext>
            {
                Text = stochasticSoilModelContext => RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Image = stochasticSoilModelContext => RiskeerCommonFormsResources.GeneralFolderIcon,
                ForeColor = stochasticSoilModelContext => stochasticSoilModelContext.WrappedData.Any()
                                                              ? Color.FromKnownColor(KnownColor.ControlText)
                                                              : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = stochasticSoilModelContext => stochasticSoilModelContext.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = StochasticSoilModelCollectionContextContextMenuStrip
            };

            yield return new TreeNodeInfo<PipingStochasticSoilModel>
            {
                Text = stochasticSoilModel => stochasticSoilModel.Name,
                Image = stochasticSoilModel => PipingFormsResources.StochasticSoilModelIcon,
                ChildNodeObjects = stochasticSoilModel => stochasticSoilModel.StochasticSoilProfiles.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingStochasticSoilProfile>
            {
                Text = pipingSoilProfile => pipingSoilProfile.SoilProfile.Name,
                Image = pipingSoilProfile => PipingFormsResources.PipingSoilProfileIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<SemiProbabilisticPipingOutputContext>
            {
                Text = pipingOutput => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = pipingOutput => RiskeerCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingScenariosContext>
            {
                Text = context => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = context => RiskeerCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptySemiProbabilisticPipingOutput>
            {
                Text = emptyPipingOutput => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RiskeerCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ProbabilisticPipingOutputContext>
            {
                Text = context => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = context => RiskeerCommonFormsResources.CalculationOutputFolderIcon,
                ForeColor = context => context.WrappedData.HasOutput
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = ProbabilisticOutputChildNodeObjects
            };

            yield return new TreeNodeInfo<ProbabilisticPipingProfileSpecificOutputContext>
            {
                Text = context => PipingFormsResources.ProbabilisticProfileSpecificOutput_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralOutputIcon,
                ForeColor = context => context.WrappedData.HasOutput
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ProbabilisticPipingSectionSpecificOutputContext>
            {
                Text = context => PipingFormsResources.ProbabilisticSectionSpecificOutput_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralOutputIcon,
                ForeColor = context => context.WrappedData.HasOutput
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingFailureMechanismSectionsContext>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = context => RiskeerCommonFormsResources.SectionsIcon,
                ForeColor = context => context.WrappedData.Sections.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = FailureMechanismSectionsContextMenuStrip
            };
        }

        #region PropertyInfos

        private static ProbabilisticPipingOutputProperties CreateProbabilisticPipingOutputProperties(
            IPartialProbabilisticPipingOutput partialProbabilisticPipingOutput)
        {
            switch (partialProbabilisticPipingOutput)
            {
                case PartialProbabilisticFaultTreePipingOutput partialProbabilisticFaultTreePipingOutput:
                    return new ProbabilisticFaultTreePipingOutputProperties(
                        partialProbabilisticFaultTreePipingOutput);
                case PartialProbabilisticSubMechanismPipingOutput partialProbabilisticSubMechanismPipingOutput:
                    return new ProbabilisticSubMechanismPipingOutputProperties(
                        partialProbabilisticSubMechanismPipingOutput);
                default:
                    return null;
            }
        }

        #endregion

        #region ViewInfos

        private static bool ClosePipingFailureMechanismViewForData(RegistrationStateFailureMechanismView view, object dataToCloseFor)
        {
            var pipingFailureMechanism = dataToCloseFor as PipingFailureMechanism;
            return dataToCloseFor is IAssessmentSection assessmentSection
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, pipingFailureMechanism);
        }

        private static bool CloseFailureMechanismResultViewForData(PipingFailureMechanismResultView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as PipingFailureMechanism;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<PipingFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is IFailureMechanismContext<PipingFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        private static bool ClosePipingCalculationsViewForData(PipingCalculationsView view, object dataToCloseFor)
        {
            PipingFailureMechanism failureMechanism = null;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<PipingFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is CalculationsStateFailureMechanismContext context)
            {
                failureMechanism = context.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        private static bool ClosePipingScenariosViewForData(PipingScenariosView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as PipingFailureMechanism;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<PipingFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is FailureMechanismContext<PipingFailureMechanism> context)
            {
                failureMechanism = context.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        private static bool ClosePipingInputViewForData(PipingInputView view, object dataToCloseFor)
        {
            PipingFailureMechanism failureMechanism = null;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<PipingFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is CalculationsStateFailureMechanismContext context)
            {
                failureMechanism = context.WrappedData;
            }

            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations = null;

            if (failureMechanism != null)
            {
                calculations = failureMechanism.CalculationsGroup.GetCalculations()
                                               .Cast<IPipingCalculationScenario<PipingInput>>();
            }

            if (dataToCloseFor is PipingCalculationGroupContext pipingCalculationGroupContext)
            {
                calculations = pipingCalculationGroupContext.WrappedData.GetCalculations()
                                                            .Cast<IPipingCalculationScenario<PipingInput>>();
            }

            if (dataToCloseFor is ProbabilisticPipingCalculationScenarioContext probabilisticPipingCalculationScenarioContext)
            {
                calculations = new[]
                {
                    probabilisticPipingCalculationScenarioContext.WrappedData
                };
            }

            if (dataToCloseFor is SemiProbabilisticPipingCalculationScenarioContext semiProbabilisticPipingCalculationScenarioContext)
            {
                calculations = new[]
                {
                    semiProbabilisticPipingCalculationScenarioContext.WrappedData
                };
            }

            return calculations != null && calculations.Any(ci => ReferenceEquals(view.Data, ci));
        }

        #endregion

        #region TreeNodeInfos

        #region PipingFailureMechanismSectionsContext TreeNodeInfo

        private ContextMenuStrip FailureMechanismSectionsContextMenuStrip(PipingFailureMechanismSectionsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IInquiryHelper inquiryHelper = GetInquiryHelper();

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddImportItem(new ImportInfo[]
                      {
                          PipingImportInfoFactory.CreateFailureMechanismSectionsImportInfo(inquiryHelper)
                      })
                      .AddUpdateItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region PipingSurfaceLinesContext TreeNodeInfo

        private ContextMenuStrip PipingSurfaceLinesContextContextMenuStrip(PipingSurfaceLinesContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddImportItem()
                      .AddUpdateItem()
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region StochasticSoilModelCollectionContext TreeNodeInfo

        private ContextMenuStrip StochasticSoilModelCollectionContextContextMenuStrip(PipingStochasticSoilModelCollectionContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddImportItem()
                      .AddUpdateItem()
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        private static RoundedDouble GetNormativeAssessmentLevel(IAssessmentSection assessmentSection,
                                                                 SemiProbabilisticPipingCalculationScenario calculation)
        {
            return assessmentSection.GetNormativeAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation);
        }

        #region CalculationsStateFailureMechanismContext TreeNodeInfo

        private static object[] CalculationsStateFailureMechanismChildNodeObjects(CalculationsStateFailureMechanismContext context)
        {
            PipingFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetCalculationsStateFailureMechanismInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new PipingCalculationGroupContext(failureMechanism.CalculationsGroup, null, failureMechanism.SurfaceLines,
                                                  failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection)
            };
        }

        private static IEnumerable<object> GetCalculationsStateFailureMechanismInputs(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new PipingSurfaceLinesContext(failureMechanism.SurfaceLines, failureMechanism, assessmentSection),
                new PipingStochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection),
                failureMechanism.CalculationsInputComments
            };
        }

        private ContextMenuStrip CalculationsStateFailureMechanismContextMenuStrip(CalculationsStateFailureMechanismContext context,
                                                                                   object parentData,
                                                                                   TreeViewControl treeViewControl)
        {
            IEnumerable<ProbabilisticPipingCalculationScenario> calculations = context.WrappedData
                                                                                      .Calculations
                                                                                      .OfType<ProbabilisticPipingCalculationScenario>();
            IInquiryHelper inquiryHelper = GetInquiryHelper();

            var builder = new RiskeerContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              context,
                              ValidateAllInFailureMechanism)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              context,
                              CalculateAllInFailureMechanism)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(context.WrappedData)
                          .AddClearIllustrationPointsOfCalculationsInFailureMechanismItem(
                              () => ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(calculations),
                              CreateChangeHandler(inquiryHelper, calculations))
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        /// <summary>
        /// Validates all calculations in <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to validate the calculations from.</param>
        /// <exception cref="NotSupportedException">Thrown when any of the calculations in <paramref name="context"/>
        /// is of a type that is not supported.</exception>
        private static void ValidateAllInFailureMechanism(CalculationsStateFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.Cast<IPipingCalculationScenario<PipingInput>>(),
                        context.WrappedData, context.Parent);
        }

        /// <summary>
        /// Performs all calculations in <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to perform the calculations from.</param>
        /// <exception cref="NotSupportedException">Thrown when any of the calculations in <paramref name="context"/>
        /// is of a type that is not supported.</exception>
        private void CalculateAllInFailureMechanism(CalculationsStateFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow, PipingCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                             context.Parent));
        }

        #endregion

        #region RegistrationStateFailureMechanismContext TreeNodeInfo

        private static object[] RegistrationStateFailureMechanismEnabledChildNodeObjects(RegistrationStateFailureMechanismContext context)
        {
            PipingFailureMechanism failureMechanism = context.WrappedData;
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

        private static IEnumerable<object> GetRegistrationStateFailureMechanismInputs(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetRegistrationStateFailureMechanismOutputs(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new PipingScenariosContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection),
                new PipingFailureMechanismSectionResultContext(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
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

        #region PipingCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(PipingCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                switch (item)
                {
                    case SemiProbabilisticPipingCalculationScenario semiProbabilisticPipingCalculationScenario:
                        childNodeObjects.Add(new SemiProbabilisticPipingCalculationScenarioContext(semiProbabilisticPipingCalculationScenario,
                                                                                                   nodeData.WrappedData,
                                                                                                   nodeData.AvailablePipingSurfaceLines,
                                                                                                   nodeData.AvailableStochasticSoilModels,
                                                                                                   nodeData.FailureMechanism,
                                                                                                   nodeData.AssessmentSection));
                        break;
                    case ProbabilisticPipingCalculationScenario probabilisticPipingCalculationScenario:
                        childNodeObjects.Add(new ProbabilisticPipingCalculationScenarioContext(probabilisticPipingCalculationScenario,
                                                                                               nodeData.WrappedData,
                                                                                               nodeData.AvailablePipingSurfaceLines,
                                                                                               nodeData.AvailableStochasticSoilModels,
                                                                                               nodeData.FailureMechanism,
                                                                                               nodeData.AssessmentSection));
                        break;
                    case CalculationGroup group:
                        childNodeObjects.Add(new PipingCalculationGroupContext(group,
                                                                               nodeData.WrappedData,
                                                                               nodeData.AvailablePipingSurfaceLines,
                                                                               nodeData.AvailableStochasticSoilModels,
                                                                               nodeData.FailureMechanism,
                                                                               nodeData.AssessmentSection));
                        break;
                    default:
                        childNodeObjects.Add(item);
                        break;
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(PipingCalculationGroupContext nodeData,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = nodeData.WrappedData;
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            bool isNestedGroup = parentData is PipingCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGeneratePipingCalculationsItem(nodeData);
            StrictContextMenuItem addSemiProbabilisticCalculationItem = CreateAddSemiProbabilisticCalculationItem(nodeData);
            StrictContextMenuItem addProbabilisticCalculationItem = CreateAddProbabilisticCalculationItem(nodeData);

            IPipingCalculationScenario<PipingInput>[] calculations = nodeData.WrappedData.GetCalculations()
                                                                             .Cast<IPipingCalculationScenario<PipingInput>>()
                                                                             .ToArray();
            StrictContextMenuItem updateEntryAndExitPointsItem = CreateCalculationGroupUpdateEntryAndExitPointItem(calculations);

            IInquiryHelper inquiryHelper = GetInquiryHelper();

            if (!isNestedGroup)
            {
                builder.AddOpenItem()
                       .AddSeparator();
            }

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
                   .AddCustomItem(addSemiProbabilisticCalculationItem)
                   .AddCustomItem(addProbabilisticCalculationItem)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            ProbabilisticPipingCalculationScenario[] probabilisticCalculations = calculations.OfType<ProbabilisticPipingCalculationScenario>().ToArray();
            builder.AddCustomItem(updateEntryAndExitPointsItem)
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(
                       nodeData,
                       ValidateAllInCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(
                       nodeData,
                       CalculateAllInCalculationGroup)
                   .AddSeparator()
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddClearIllustrationPointsOfCalculationsInGroupItem(
                       () => ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(probabilisticCalculations),
                       CreateChangeHandler(inquiryHelper, probabilisticCalculations));

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

        private StrictContextMenuItem CreateCalculationGroupUpdateEntryAndExitPointItem(IEnumerable<IPipingCalculationScenario<PipingInput>> calculations)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_all_calculations_with_surface_line_ToolTip;

            IPipingCalculationScenario<PipingInput>[] calculationsToUpdate = calculations
                                                                             .Where(calc => calc.InputParameters.SurfaceLine != null
                                                                                            && !calc.InputParameters.IsEntryAndExitPointInputSynchronized)
                                                                             .ToArray();

            if (!calculationsToUpdate.Any())
            {
                contextMenuEnabled = false;
                toolTipMessage = RiskeerCommonFormsResources.CreateUpdateContextMenuItem_No_calculations_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_all_entry_and_exit_points,
                toolTipMessage,
                RiskeerCommonFormsResources.UpdateItemIcon,
                (sender, args) => UpdateEntryAndExitPointsOfAllCalculations(calculationsToUpdate))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateEntryAndExitPointsOfAllCalculations(IEnumerable<IPipingCalculationScenario<PipingInput>> calculations)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
            if (VerifyEntryAndExitPointUpdates(calculations, message))
            {
                foreach (IPipingCalculationScenario<PipingInput> calculation in calculations)
                {
                    UpdateSurfaceLineDependentData(calculation);
                }
            }
        }

        private StrictContextMenuItem CreateGeneratePipingCalculationsItem(PipingCalculationGroupContext nodeData)
        {
            bool surfaceLineAvailable = nodeData.AvailablePipingSurfaceLines.Any() && nodeData.AvailableStochasticSoilModels.Any();

            string pipingCalculationGroupGeneratePipingCalculationsToolTip = surfaceLineAvailable
                                                                                 ? PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_ToolTip
                                                                                 : PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip;

            return new StrictContextMenuItem(
                RiskeerCommonFormsResources.CalculationGroup_Generate_Scenarios,
                pipingCalculationGroupGeneratePipingCalculationsToolTip,
                RiskeerCommonFormsResources.GenerateScenariosIcon, (o, args) => ShowSurfaceLineSelectionDialog(nodeData))
            {
                Enabled = surfaceLineAvailable
            };
        }

        private static StrictContextMenuItem CreateAddSemiProbabilisticCalculationItem(PipingCalculationGroupContext context)
        {
            return new StrictContextMenuItem(
                Resources.CalculationGroup_Add_SemiProbabilisticCalculation,
                Resources.CalculationGroup_Add_SemiProbabilisticCalculation_ToolTip,
                RiskeerCommonFormsResources.SemiProbabilisticCalculationIcon,
                (sender, args) => AddCalculation(() => new SemiProbabilisticPipingCalculationScenario(), context.WrappedData));
        }

        private static StrictContextMenuItem CreateAddProbabilisticCalculationItem(PipingCalculationGroupContext context)
        {
            return new StrictContextMenuItem(
                Resources.CalculationGroup_Add_ProbabilisticCalculation,
                Resources.CalculationGroup_Add_ProbabilisticCalculation_ToolTip,
                RiskeerCommonFormsResources.ProbabilisticCalculationIcon,
                (sender, args) => AddCalculation(() => new ProbabilisticPipingCalculationScenario(), context.WrappedData));
        }

        private static void AddCalculation<TCalculationScenario>(Func<TCalculationScenario> createCalculationScenarioFunc, CalculationGroup parentGroup)
            where TCalculationScenario : IPipingCalculationScenario<PipingInput>
        {
            TCalculationScenario calculation = createCalculationScenarioFunc();
            calculation.Name = NamingHelper.GetUniqueName(parentGroup.Children.OfType<TCalculationScenario>(),
                                                          RiskeerCommonDataResources.Calculation_DefaultName,
                                                          c => c.Name);

            parentGroup.Children.Add(calculation);
            parentGroup.NotifyObservers();
        }

        private void ShowSurfaceLineSelectionDialog(PipingCalculationGroupContext nodeData)
        {
            using (var dialog = new PipingSurfaceLineSelectionDialog(Gui.MainWindow, nodeData.AvailablePipingSurfaceLines))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    foreach (ICalculationBase group in PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(
                                 dialog.SelectedItems,
                                 dialog.GenerateSemiProbabilistic,
                                 dialog.GenerateProbabilistic,
                                 nodeData.AvailableStochasticSoilModels))
                    {
                        nodeData.WrappedData.Children.Add(group);
                    }

                    nodeData.NotifyObservers();
                }
            }
        }

        private static void CalculationGroupContextOnNodeRemoved(PipingCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (PipingCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        /// <summary>
        /// Validates all calculations in <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to validate the calculations from.</param>
        /// <exception cref="NotSupportedException">Thrown when any of the calculations in <paramref name="context"/> is of
        /// a type that is not supported.</exception>
        private static void ValidateAllInCalculationGroup(PipingCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().Cast<IPipingCalculationScenario<PipingInput>>(),
                        context.FailureMechanism, context.AssessmentSection);
        }

        /// <summary>
        /// Performs all calculations in the calculation group wrapped by <paramref name="calculationGroupContext"/>.
        /// </summary>
        /// <param name="calculationGroupContext">The context that wraps the calculation group.</param>
        /// <exception cref="NotSupportedException">Thrown when any of the calculations in <paramref name="calculationGroupContext"/>
        /// is of a type that is not supported.</exception>
        private void CalculateAllInCalculationGroup(PipingCalculationGroupContext calculationGroupContext)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow, PipingCalculationActivityFactory.CreateCalculationActivities(calculationGroupContext.WrappedData,
                                                                                             calculationGroupContext.FailureMechanism,
                                                                                             calculationGroupContext.AssessmentSection));
        }

        #endregion

        #region SemiProbabilisticPipingCalculationScenarioContext TreeNodeInfo

        private static object[] SemiProbabilisticPipingCalculationScenarioContextChildNodeObjects(SemiProbabilisticPipingCalculationScenarioContext context)
        {
            SemiProbabilisticPipingCalculationScenario pipingCalculationScenario = context.WrappedData;

            var childNodes = new List<object>
            {
                pipingCalculationScenario.Comments,
                new SemiProbabilisticPipingInputContext(pipingCalculationScenario.InputParameters,
                                                        pipingCalculationScenario,
                                                        context.AvailablePipingSurfaceLines,
                                                        context.AvailableStochasticSoilModels,
                                                        context.FailureMechanism,
                                                        context.AssessmentSection)
            };

            if (pipingCalculationScenario.HasOutput)
            {
                childNodes.Add(new SemiProbabilisticPipingOutputContext(
                                   pipingCalculationScenario.Output,
                                   context.FailureMechanism,
                                   context.AssessmentSection));
            }
            else
            {
                childNodes.Add(new EmptySemiProbabilisticPipingOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip SemiProbabilisticPipingCalculationScenarioContextContextMenuStrip(SemiProbabilisticPipingCalculationScenarioContext nodeData,
                                                                                                   object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            SemiProbabilisticPipingCalculationScenario calculation = nodeData.WrappedData;

            StrictContextMenuItem updateEntryAndExitPoint = CreateUpdateEntryAndExitPointItem(calculation);

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, nodeData)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddCustomItem(updateEntryAndExitPoint)
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              nodeData,
                              ValidateSemiProbabilistic)
                          .AddPerformCalculationItem<SemiProbabilisticPipingCalculationScenario, SemiProbabilisticPipingCalculationScenarioContext>(
                              nodeData,
                              CalculateSemiProbabilistic)
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

        private static void SemiProbabilisticPipingCalculationScenarioContextOnNodeRemoved(SemiProbabilisticPipingCalculationScenarioContext context, object parentNodeData)
        {
            CalculationContextOnNodeRemoved(parentNodeData, context.WrappedData);
        }

        private static void ValidateSemiProbabilistic(SemiProbabilisticPipingCalculationScenarioContext context)
        {
            SemiProbabilisticPipingCalculationService.Validate(context.WrappedData,
                                                               context.FailureMechanism.GeneralInput,
                                                               GetNormativeAssessmentLevel(context.AssessmentSection, context.WrappedData));
        }

        private void CalculateSemiProbabilistic(SemiProbabilisticPipingCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             PipingCalculationActivityFactory.CreateSemiProbabilisticPipingCalculationActivity(context.WrappedData,
                                                                                                                               context.FailureMechanism.GeneralInput,
                                                                                                                               context.AssessmentSection));
        }

        #endregion

        #region ProbabilisticPipingCalculationScenarioContext TreeNodeInfo

        private static object[] ProbabilisticPipingCalculationScenarioContextChildNodeObjects(ProbabilisticPipingCalculationScenarioContext context)
        {
            ProbabilisticPipingCalculationScenario calculation = context.WrappedData;

            var childNodes = new List<object>
            {
                calculation.Comments,
                new ProbabilisticPipingInputContext(calculation.InputParameters,
                                                    calculation,
                                                    context.AvailablePipingSurfaceLines,
                                                    context.AvailableStochasticSoilModels,
                                                    context.FailureMechanism,
                                                    context.AssessmentSection),
                new ProbabilisticPipingOutputContext(calculation)
            };

            return childNodes.ToArray();
        }

        private ContextMenuStrip ProbabilisticPipingCalculationScenarioContextContextMenuStrip(ProbabilisticPipingCalculationScenarioContext nodeData,
                                                                                               object parentData, TreeViewControl treeViewControl)
        {
            ProbabilisticPipingCalculationScenario calculation = nodeData.WrappedData;
            var changeHandler = new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(GetInquiryHelper(),
                                                                                                         calculation);
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            StrictContextMenuItem updateEntryAndExitPoint = CreateUpdateEntryAndExitPointItem(calculation);

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, nodeData)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddCustomItem(updateEntryAndExitPoint)
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              nodeData,
                              ValidateProbabilistic)
                          .AddPerformCalculationItem<ProbabilisticPipingCalculationScenario, ProbabilisticPipingCalculationScenarioContext>(
                              nodeData,
                              CalculateProbabilistic)
                          .AddSeparator()
                          .AddClearCalculationOutputItem(calculation)
                          .AddClearIllustrationPointsOfCalculationItem(
                              () => ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(calculation),
                              changeHandler)
                          .AddDeleteItem()
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static void ProbabilisticPipingCalculationScenarioContextOnNodeRemoved(ProbabilisticPipingCalculationScenarioContext calculationContext, object parentNodeData)
        {
            CalculationContextOnNodeRemoved(parentNodeData, calculationContext.WrappedData);
        }

        private static void ValidateProbabilistic(ProbabilisticPipingCalculationScenarioContext context)
        {
            ProbabilisticPipingCalculationService.Validate(context.WrappedData, context.FailureMechanism, context.AssessmentSection);
        }

        private void CalculateProbabilistic(ProbabilisticPipingCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             PipingCalculationActivityFactory.CreateProbabilisticPipingCalculationActivity(context.WrappedData,
                                                                                                                           context.FailureMechanism,
                                                                                                                           context.AssessmentSection));
        }

        #endregion

        #region ProbabilisticPipingOutputContext TreeNodeInfo

        private static object[] ProbabilisticOutputChildNodeObjects(ProbabilisticPipingOutputContext context)
        {
            ProbabilisticPipingCalculationScenario calculation = context.WrappedData;

            return new object[]
            {
                new ProbabilisticPipingProfileSpecificOutputContext(calculation),
                new ProbabilisticPipingSectionSpecificOutputContext(calculation)
            };
        }

        #endregion

        private StrictContextMenuItem CreateUpdateEntryAndExitPointItem(IPipingCalculationScenario<PipingInput> calculation)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_calculation_with_characteristic_points_ToolTip;
            if (calculation.InputParameters.SurfaceLine == null)
            {
                contextMenuEnabled = false;
                toolTipMessage = Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_calculation_no_surface_line_ToolTip;
            }
            else if (calculation.InputParameters.IsEntryAndExitPointInputSynchronized)
            {
                contextMenuEnabled = false;
                toolTipMessage = RiskeerCommonFormsResources.CalculationItem_No_changes_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_entry_and_exit_point,
                toolTipMessage,
                RiskeerCommonFormsResources.UpdateItemIcon,
                (o, args) => UpdatedSurfaceLineDependentDataOfCalculation(calculation))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdatedSurfaceLineDependentDataOfCalculation(IPipingCalculationScenario<PipingInput> calculation)
        {
            string message = RiskeerCommonFormsResources.VerifyUpdate_Confirm_calculation_output_cleared;
            if (VerifyEntryAndExitPointUpdates(new[]
            {
                calculation
            }, message))
            {
                UpdateSurfaceLineDependentData(calculation);
            }
        }

        private static void CalculationContextOnNodeRemoved(object parentNodeData, IPipingCalculationScenario<PipingInput> calculation)
        {
            if (parentNodeData is PipingCalculationGroupContext calculationGroupContext)
            {
                bool successfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(calculation);
                if (successfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
            }
        }

        private static ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler CreateChangeHandler(
            IInquiryHelper inquiryHelper, IEnumerable<ProbabilisticPipingCalculationScenario> calculations)
        {
            return new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(inquiryHelper, calculations);
        }

        /// <summary>
        /// Validates the provided <paramref name="pipingCalculations"/>.
        /// </summary>
        /// <param name="pipingCalculations">The calculations to validate.</param>
        /// <param name="failureMechanism">The failure mechanism the <paramref name="pipingCalculations"/> belong to.</param>
        /// <param name="assessmentSection">The assessment section the <paramref name="pipingCalculations"/> belong to.</param>
        /// <exception cref="NotSupportedException">Thrown when any of the provided calculations is of a type that is not supported.</exception>
        private static void ValidateAll(IEnumerable<IPipingCalculationScenario<PipingInput>> pipingCalculations,
                                        PipingFailureMechanism failureMechanism,
                                        IAssessmentSection assessmentSection)
        {
            foreach (IPipingCalculationScenario<PipingInput> calculation in pipingCalculations)
            {
                switch (calculation)
                {
                    case SemiProbabilisticPipingCalculationScenario semiProbabilisticPipingCalculationScenario:
                        SemiProbabilisticPipingCalculationService.Validate(semiProbabilisticPipingCalculationScenario,
                                                                           failureMechanism.GeneralInput,
                                                                           GetNormativeAssessmentLevel(assessmentSection, semiProbabilisticPipingCalculationScenario));
                        break;
                    case ProbabilisticPipingCalculationScenario probabilisticPipingCalculationScenario:
                        ProbabilisticPipingCalculationService.Validate(probabilisticPipingCalculationScenario,
                                                                       failureMechanism, assessmentSection);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }
        }

        private bool VerifyEntryAndExitPointUpdates(IEnumerable<IPipingCalculationScenario<PipingInput>> calculations, string query)
        {
            var changeHandler = new CalculationChangeHandler(calculations, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void UpdateSurfaceLineDependentData(ICalculation<PipingInput> calculation)
        {
            calculation.InputParameters.SynchronizeEntryAndExitPointInput();

            var affectedObjects = new List<IObservable>
            {
                calculation.InputParameters
            };

            affectedObjects.AddRange(RiskeerCommonDataSynchronizationService.ClearCalculationOutput(calculation));

            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        #endregion

        #region ImportInfos

        private static FileFilterGenerator PipingSurfaceLineFileFilter =>
            new FileFilterGenerator(
                RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                $"{RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor} {RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description}");

        private static FileFilterGenerator StochasticSoilModelFileFilter =>
            new FileFilterGenerator(Resources.Soil_file_Extension, Resources.Soil_file_Description);

        private bool VerifyPipingSurfaceLineUpdates(PipingSurfaceLinesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static bool HasGeometry(ReferenceLine referenceLine)
        {
            return referenceLine.Points.Any();
        }

        private bool VerifyStochasticSoilModelUpdates(PipingStochasticSoilModelCollectionContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism, query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}