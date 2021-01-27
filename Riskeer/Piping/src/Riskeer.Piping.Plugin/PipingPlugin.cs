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

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.Commands;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.IllustrationPoints;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.Helpers;
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
            yield return new PropertyInfo<PipingFailureMechanismContext, PipingFailureMechanismProperties>
            {
                CreateInstance = context => new PipingFailureMechanismProperties(context.WrappedData, context.Parent,
                                                                                 new FailureMechanismPropertyChangeHandler<PipingFailureMechanism>())
            };
            yield return new PropertyInfo<SemiProbabilisticPipingInputContext, SemiProbabilisticPipingInputContextProperties>
            {
                CreateInstance = context => new SemiProbabilisticPipingInputContextProperties(context,
                                                                                              () => GetNormativeAssessmentLevel(context.AssessmentSection, context.PipingCalculation),
                                                                                              new ObservablePropertyChangeHandler(context.PipingCalculation, context.WrappedData))
            };
            yield return new PropertyInfo<ProbabilisticPipingInputContext, ProbabilisticPipingInputContextProperties>
            {
                CreateInstance = context => new ProbabilisticPipingInputContextProperties(context, new ObservablePropertyChangeHandler(context.PipingCalculation, context.WrappedData))
            };
            yield return new PropertyInfo<SemiProbabilisticPipingOutputContext, SemiProbabilisticPipingOutputProperties>
            {
                CreateInstance = context => new SemiProbabilisticPipingOutputProperties(context.WrappedData, context.FailureMechanism, context.AssessmentSection)
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
            yield return new PropertyInfo<PipingFailureMechanismSectionsContext, FailureMechanismSectionsProbabilityAssessmentProperties>
            {
                CreateInstance = context => new FailureMechanismSectionsProbabilityAssessmentProperties(
                    context.WrappedData, ((PipingFailureMechanism) context.WrappedData).PipingProbabilityAssessmentInput)
            };
            yield return new PropertyInfo<ProbabilisticPipingProfileSpecificOutputContext, ProbabilisticPipingProfileSpecificOutputProperties>
            {
                CreateInstance = CreateProbabilisticPipingProfileSpecificOutputProperties
            };
            yield return new PropertyInfo<ProbabilisticPipingSectionSpecificOutputContext, ProbabilisticPipingSectionSpecificOutputProperties>
            {
                CreateInstance = CreateProbabilisticPipingSectionSpecificOutputProperties
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
                        context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
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
            yield return new ViewInfo<PipingFailureMechanismContext, PipingFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.FailureMechanismIcon,
                CloseForData = ClosePipingFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new PipingFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>,
                IObservableEnumerable<PipingFailureMechanismSectionResult>,
                PipingFailureMechanismResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new PipingFailureMechanismResultView(
                    context.WrappedData,
                    (PipingFailureMechanism) context.FailureMechanism,
                    context.AssessmentSection)
            };

            yield return new ViewInfo<PipingCalculationGroupContext, CalculationGroup, PipingCalculationsView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.GeneralFolderIcon,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CreateInstance = context => new PipingCalculationsView(context.WrappedData, context.FailureMechanism, context.AssessmentSection),
                CloseForData = ClosePipingCalculationsViewForData
            };

            yield return new ViewInfo<SemiProbabilisticPipingInputContext, SemiProbabilisticPipingCalculationScenario, PipingInputView>
            {
                GetViewData = context => context.PipingCalculation,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                Image = PipingFormsResources.PipingInputIcon,
                CloseForData = ClosePipingInputViewForData
            };

            yield return new ViewInfo<ProbabilisticPipingInputContext, ProbabilisticPipingCalculationScenario, PipingInputView>
            {
                GetViewData = context => context.PipingCalculation,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                Image = PipingFormsResources.PipingInputIcon,
                CloseForData = ClosePipingInputViewForData
            };

            yield return new ViewInfo<PipingScenariosContext, CalculationGroup, PipingScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = RiskeerCommonFormsResources.ScenariosIcon,
                CloseForData = ClosePipingScenariosViewForData,
                CreateInstance = context => new PipingScenariosView(context.WrappedData, context.FailureMechanism, context.AssessmentSection)
            };

            yield return new ViewInfo<PipingFailureMechanismSectionsContext, IEnumerable<FailureMechanismSection>, FailureMechanismSectionsProbabilityAssessmentView>
            {
                GetViewData = context => context.WrappedData.Sections,
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = RiskeerCommonFormsResources.SectionsIcon,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView,
                CreateInstance = context => new FailureMechanismSectionsProbabilityAssessmentView(context.WrappedData.Sections,
                                                                                                  context.WrappedData,
                                                                                                  ((PipingFailureMechanism) context.WrappedData).PipingProbabilityAssessmentInput)
            };

            yield return new ViewInfo<ProbabilisticPipingProfileSpecificOutputContext, ProbabilisticPipingCalculationScenario, ProbabilisticFaultTreePipingProfileSpecificOutputView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => PipingFormsResources.ProbabilisticProfileSpecificOutput_DisplayName,
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                CloseForData = (view, o) => CloseProbabilisticPipingOutputViewForData(
                    view, o, calculation => (PartialProbabilisticPipingOutput<TopLevelFaultTreeIllustrationPoint>) calculation.Output.ProfileSpecificOutput),
                AdditionalDataCheck = context => context.WrappedData.HasOutput
                                                 && context.WrappedData.Output.ProfileSpecificOutput.HasGeneralResult
                                                 && context.WrappedData.Output.ProfileSpecificOutput is PartialProbabilisticFaultTreePipingOutput,
                CreateInstance = context => new ProbabilisticFaultTreePipingProfileSpecificOutputView(
                    context.WrappedData,
                    () => ((PartialProbabilisticFaultTreePipingOutput) context.WrappedData.Output?.ProfileSpecificOutput)?.GeneralResult)
            };

            yield return new ViewInfo<ProbabilisticPipingProfileSpecificOutputContext, ProbabilisticPipingCalculationScenario, ProbabilisticSubMechanismPipingProfileSpecificOutputView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => PipingFormsResources.ProbabilisticProfileSpecificOutput_DisplayName,
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                CloseForData = (view, o) => CloseProbabilisticPipingOutputViewForData(
                    view, o, calculation => (PartialProbabilisticPipingOutput<TopLevelSubMechanismIllustrationPoint>) calculation.Output.ProfileSpecificOutput),
                AdditionalDataCheck = context => context.WrappedData.HasOutput
                                                 && context.WrappedData.Output.ProfileSpecificOutput.HasGeneralResult
                                                 && context.WrappedData.Output.ProfileSpecificOutput is PartialProbabilisticSubMechanismPipingOutput,
                CreateInstance = context =>
                {
                    return new ProbabilisticSubMechanismPipingProfileSpecificOutputView(
                        context.WrappedData,
                        () => ((PartialProbabilisticSubMechanismPipingOutput) context.WrappedData.Output?.ProfileSpecificOutput)?.GeneralResult);
                }
            };

            yield return new ViewInfo<ProbabilisticPipingSectionSpecificOutputContext, ProbabilisticPipingCalculationScenario, ProbabilisticFaultTreePipingSectionSpecificOutputView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => PipingFormsResources.ProbabilisticSectionSpecificOutput_DisplayName,
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                CloseForData = (view, o) => CloseProbabilisticPipingOutputViewForData(
                    view, o, calculation => (PartialProbabilisticPipingOutput<TopLevelFaultTreeIllustrationPoint>) calculation.Output.SectionSpecificOutput),
                AdditionalDataCheck = context => context.WrappedData.HasOutput
                                                 && context.WrappedData.Output.SectionSpecificOutput.HasGeneralResult
                                                 && context.WrappedData.Output.SectionSpecificOutput is PartialProbabilisticFaultTreePipingOutput,
                CreateInstance = context =>
                {
                    return new ProbabilisticFaultTreePipingSectionSpecificOutputView(
                        context.WrappedData,
                        () => ((PartialProbabilisticFaultTreePipingOutput) context.WrappedData.Output?.SectionSpecificOutput)?.GeneralResult);
                }
            };

            yield return new ViewInfo<ProbabilisticPipingSectionSpecificOutputContext, ProbabilisticPipingCalculationScenario, ProbabilisticSubMechanismPipingSectionSpecificOutputView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => PipingFormsResources.ProbabilisticSectionSpecificOutput_DisplayName,
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                CloseForData = (view, o) => CloseProbabilisticPipingOutputViewForData(
                    view, o, calculation => (PartialProbabilisticPipingOutput<TopLevelSubMechanismIllustrationPoint>) calculation.Output.SectionSpecificOutput),
                AdditionalDataCheck = context => context.WrappedData.HasOutput
                                                 && context.WrappedData.Output.SectionSpecificOutput.HasGeneralResult
                                                 && context.WrappedData.Output.SectionSpecificOutput is PartialProbabilisticSubMechanismPipingOutput,
                CreateInstance = context =>
                {
                    return new ProbabilisticSubMechanismPipingSectionSpecificOutputView(
                        context.WrappedData,
                        () => ((PartialProbabilisticSubMechanismPipingOutput) context.WrappedData.Output?.SectionSpecificOutput)?.GeneralResult);
                }
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<PipingFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

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

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>>
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

        #region ProbabilisticPipingProfileSpecificOutputContext PropertyInfo

        private static ProbabilisticPipingProfileSpecificOutputProperties CreateProbabilisticPipingProfileSpecificOutputProperties(
            ProbabilisticPipingProfileSpecificOutputContext context)
        {
            switch (context.WrappedData.Output?.ProfileSpecificOutput)
            {
                case PartialProbabilisticFaultTreePipingOutput partialProbabilisticFaultTreePipingOutput:
                    return new ProbabilisticFaultTreePipingProfileSpecificOutputProperties(
                        partialProbabilisticFaultTreePipingOutput, context.WrappedData,
                        context.FailureMechanism, context.AssessmentSection);
                case PartialProbabilisticSubMechanismPipingOutput partialProbabilisticSubMechanismPipingOutput:
                    return new ProbabilisticSubMechanismPipingProfileSpecificOutputProperties(
                        partialProbabilisticSubMechanismPipingOutput, context.WrappedData,
                        context.FailureMechanism, context.AssessmentSection);
                default:
                    return null;
            }
        }

        #endregion

        #region ProbabilisticPipingSectionSpecificOutputContext PropertyInfo

        private static ProbabilisticPipingSectionSpecificOutputProperties CreateProbabilisticPipingSectionSpecificOutputProperties(
            ProbabilisticPipingSectionSpecificOutputContext context)
        {
            switch (context.WrappedData.Output?.SectionSpecificOutput)
            {
                case PartialProbabilisticFaultTreePipingOutput partialProbabilisticFaultTreePipingOutput:
                    return new ProbabilisticFaultTreePipingSectionSpecificOutputProperties(
                        partialProbabilisticFaultTreePipingOutput);
                case PartialProbabilisticSubMechanismPipingOutput partialProbabilisticSubMechanismPipingOutput:
                    return new ProbabilisticSubMechanismPipingSectionSpecificOutputProperties
                        (partialProbabilisticSubMechanismPipingOutput);
                default:
                    return null;
            }
        }

        #endregion

        #endregion

        #region ViewInfos

        private static bool ClosePipingFailureMechanismViewForData(PipingFailureMechanismView view, object o)
        {
            var pipingFailureMechanism = o as PipingFailureMechanism;

            return o is IAssessmentSection assessmentSection
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, pipingFailureMechanism);
        }

        private static bool CloseFailureMechanismResultViewForData(PipingFailureMechanismResultView view, object o)
        {
            if (o is IAssessmentSection assessmentSection)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<PipingFailureMechanism>()
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            var failureMechanism = o as PipingFailureMechanism;
            if (o is IFailureMechanismContext<PipingFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        private static bool ClosePipingCalculationsViewForData(PipingCalculationsView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var pipingFailureMechanism = o as PipingFailureMechanism;

            if (o is PipingFailureMechanismContext pipingFailureMechanismContext)
            {
                pipingFailureMechanism = pipingFailureMechanismContext.WrappedData;
            }

            if (assessmentSection != null)
            {
                pipingFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                          .OfType<PipingFailureMechanism>()
                                                          .FirstOrDefault();
            }

            return pipingFailureMechanism != null && ReferenceEquals(view.Data, pipingFailureMechanism.CalculationsGroup);
        }

        private static bool ClosePipingScenariosViewForData(PipingScenariosView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var pipingFailureMechanism = o as PipingFailureMechanism;

            if (o is PipingFailureMechanismContext pipingFailureMechanismContext)
            {
                pipingFailureMechanism = pipingFailureMechanismContext.WrappedData;
            }

            if (assessmentSection != null)
            {
                pipingFailureMechanism = assessmentSection.GetFailureMechanisms()
                                                          .OfType<PipingFailureMechanism>()
                                                          .FirstOrDefault();
            }

            return pipingFailureMechanism != null && ReferenceEquals(view.Data, pipingFailureMechanism.CalculationsGroup);
        }

        private static bool ClosePipingInputViewForData(PipingInputView view, object o)
        {
            if (o is ProbabilisticPipingCalculationScenarioContext probabilisticPipingCalculationScenarioContext)
            {
                return ReferenceEquals(view.Data, probabilisticPipingCalculationScenarioContext.WrappedData);
            }

            if (o is SemiProbabilisticPipingCalculationScenarioContext semiProbabilisticPipingCalculationScenarioContext)
            {
                return ReferenceEquals(view.Data, semiProbabilisticPipingCalculationScenarioContext.WrappedData);
            }

            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations = null;

            if (o is PipingCalculationGroupContext pipingCalculationGroupContext)
            {
                calculations = pipingCalculationGroupContext.WrappedData.GetCalculations()
                                                            .Cast<IPipingCalculationScenario<PipingInput>>();
            }

            var failureMechanism = o as PipingFailureMechanism;

            if (o is PipingFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (o is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<PipingFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (failureMechanism != null)
            {
                calculations = failureMechanism.CalculationsGroup.GetCalculations()
                                               .Cast<IPipingCalculationScenario<PipingInput>>();
            }

            return calculations != null && calculations.Any(ci => ReferenceEquals(view.Data, ci));
        }

        private static bool CloseProbabilisticPipingOutputViewForData<T>(
            GeneralResultIllustrationPointView<T> view, object o,
            Func<ProbabilisticPipingCalculationScenario, PartialProbabilisticPipingOutput<T>> getPartialOutputFunc)
            where T : TopLevelIllustrationPointBase
        {
            var calculation = (ProbabilisticPipingCalculationScenario) view.Data;
            if (o is GeneralResult<T> illustrationPoints)
            {
                return ReferenceEquals(getPartialOutputFunc(calculation).GeneralResult, illustrationPoints);
            }

            if (o is ProbabilisticPipingOutput output)
            {
                return ReferenceEquals(calculation.Output, output);
            }

            return RiskeerPluginHelper.ShouldCloseViewWithCalculationData(view, o);
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

        #region PipingFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(PipingFailureMechanismContext context)
        {
            PipingFailureMechanism wrappedData = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, assessmentSection), TreeFolderCategory.Input),
                new PipingCalculationGroupContext(wrappedData.CalculationsGroup, null, wrappedData.SurfaceLines, wrappedData.StochasticSoilModels, wrappedData, assessmentSection),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(PipingFailureMechanismContext pipingFailureMechanismContext)
        {
            return new object[]
            {
                pipingFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new PipingFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new PipingSurfaceLinesContext(failureMechanism.SurfaceLines, failureMechanism, assessmentSection),
                new PipingStochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            PipingProbabilityAssessmentInput probabilityAssessmentInput = failureMechanism.PipingProbabilityAssessmentInput;
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism,
                                                              assessmentSection,
                                                              () => probabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length)),
                new PipingScenariosContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection),
                new ProbabilityFailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(PipingFailureMechanismContext pipingFailureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            IPipingCalculationScenario<PipingInput>[] calculations = pipingFailureMechanismContext.WrappedData
                                                                                                  .Calculations
                                                                                                  .OfType<IPipingCalculationScenario<PipingInput>>()
                                                                                                  .ToArray();

            ProbabilisticPipingCalculationScenario[] probabilisticCalculations = calculations.OfType<ProbabilisticPipingCalculationScenario>()
                                                                                             .ToArray();
            IInquiryHelper inquiryHelper = GetInquiryHelper();
            IViewCommands viewCommands = Gui.ViewCommands;

            var builder = new RiskeerContextMenuBuilder(Gui.Get(pipingFailureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(pipingFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              pipingFailureMechanismContext,
                              ValidateAllInFailureMechanism)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              pipingFailureMechanismContext,
                              CalculateAllInFailureMechanism)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(
                              () => calculations.Any(c => c.HasOutput),
                              CreateClearCalculationOutputChangeHandler(calculations, inquiryHelper, viewCommands))
                          .AddClearIllustrationPointsOfCalculationsInFailureMechanismItem(
                              () => ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(probabilisticCalculations),
                              CreateIllustrationPointsChangeHandler(inquiryHelper, probabilisticCalculations))
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(PipingFailureMechanismContext pipingFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(pipingFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(pipingFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(PipingFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        /// <summary>
        /// Validates all calculations in <paramref name="context"/>.
        /// </summary>
        /// <param name="context">The context to validate the calculations from.</param>
        /// <exception cref="NotSupportedException">Thrown when any of the calculations in <paramref name="context"/>
        /// is of a type that is not supported.</exception>
        private static void ValidateAllInFailureMechanism(PipingFailureMechanismContext context)
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
        private void CalculateAllInFailureMechanism(PipingFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow, PipingCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                             context.Parent));
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
            IViewCommands viewCommands = Gui.ViewCommands;

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
                   .AddClearAllCalculationOutputInGroupItem(
                       () => calculations.Any(c => c.HasOutput),
                       CreateClearCalculationOutputChangeHandler(calculations, inquiryHelper, viewCommands))
                   .AddClearIllustrationPointsOfCalculationsInGroupItem(
                       () => ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(probabilisticCalculations),
                       CreateIllustrationPointsChangeHandler(inquiryHelper, probabilisticCalculations));

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

            IInquiryHelper inquiryHelper = GetInquiryHelper();
            IViewCommands viewCommands = Gui.ViewCommands;

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
                          .AddClearCalculationOutputItem(
                              () => calculation.HasOutput,
                              CreateClearCalculationOutputChangeHandler(new[]
                              {
                                  calculation
                              }, inquiryHelper, viewCommands))
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
                new ProbabilisticPipingOutputContext(calculation, context.FailureMechanism, context.AssessmentSection)
            };

            return childNodes.ToArray();
        }

        private ContextMenuStrip ProbabilisticPipingCalculationScenarioContextContextMenuStrip(ProbabilisticPipingCalculationScenarioContext nodeData,
                                                                                               object parentData, TreeViewControl treeViewControl)
        {
            ProbabilisticPipingCalculationScenario calculation = nodeData.WrappedData;
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            IInquiryHelper inquiryHelper = GetInquiryHelper();
            IViewCommands viewCommands = Gui.ViewCommands;

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
                          .AddClearCalculationOutputItem(
                              () => calculation.HasOutput,
                              CreateClearCalculationOutputChangeHandler(new[]
                              {
                                  calculation
                              }, inquiryHelper, viewCommands))
                          .AddClearIllustrationPointsOfCalculationItem(
                              () => ProbabilisticPipingIllustrationPointsHelper.HasIllustrationPoints(calculation),
                              new ClearIllustrationPointsOfProbabilisticPipingCalculationChangeHandler(calculation, inquiryHelper))
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
                new ProbabilisticPipingProfileSpecificOutputContext(calculation, context.FailureMechanism, context.AssessmentSection),
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

        private static ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler CreateIllustrationPointsChangeHandler(
            IInquiryHelper inquiryHelper, IEnumerable<ProbabilisticPipingCalculationScenario> calculations)
        {
            return new ClearIllustrationPointsOfProbabilisticPipingCalculationCollectionChangeHandler(inquiryHelper, calculations);
        }

        private static ClearPipingCalculationOutputChangeHandler CreateClearCalculationOutputChangeHandler(
            IEnumerable<IPipingCalculationScenario<PipingInput>> calculations, IInquiryHelper inquiryHelper, IViewCommands viewCommands)
        {
            return new ClearPipingCalculationOutputChangeHandler(
                calculations.Where(c => c.HasOutput), inquiryHelper, viewCommands,
                calculation =>
                {
                    switch (calculation)
                    {
                        case SemiProbabilisticPipingCalculationScenario semiProbabilisticPipingCalculationScenario:
                            return semiProbabilisticPipingCalculationScenario.Output;
                        case ProbabilisticPipingCalculationScenario probabilisticPipingCalculationScenario:
                            return probabilisticPipingCalculationScenario.Output;
                        default:
                            throw new NotSupportedException();
                    }
                });
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