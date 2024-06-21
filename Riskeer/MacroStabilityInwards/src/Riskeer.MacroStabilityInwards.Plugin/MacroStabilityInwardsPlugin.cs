﻿// Copyright (C) Stichting Deltares and State of the Netherlands 2023. All rights reserved.
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
using Components.Persistence.Stability;
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
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.IO.FileImporters.MessageProviders;
using Riskeer.Common.IO.SoilProfile;
using Riskeer.Common.IO.SurfaceLines;
using Riskeer.Common.Plugin;
using Riskeer.Common.Plugin.FileImporters;
using Riskeer.Common.Util.Helpers;
using Riskeer.MacroStabilityInwards.Data;
using Riskeer.MacroStabilityInwards.Data.SoilProfile;
using Riskeer.MacroStabilityInwards.Forms;
using Riskeer.MacroStabilityInwards.Forms.PresentationObjects;
using Riskeer.MacroStabilityInwards.Forms.PropertyClasses;
using Riskeer.MacroStabilityInwards.Forms.Views;
using Riskeer.MacroStabilityInwards.IO.Configurations;
using Riskeer.MacroStabilityInwards.IO.Exporters;
using Riskeer.MacroStabilityInwards.Plugin.FileImporter;
using Riskeer.MacroStabilityInwards.Plugin.Properties;
using Riskeer.MacroStabilityInwards.Primitives;
using Riskeer.MacroStabilityInwards.Service;
using CalculationsStateFailureMechanismContext = Riskeer.MacroStabilityInwards.Forms.PresentationObjects.CalculationsState.MacroStabilityInwardsFailureMechanismContext;
using RegistrationStateFailureMechanismContext = Riskeer.MacroStabilityInwards.Forms.PresentationObjects.RegistrationState.MacroStabilityInwardsFailureMechanismContext;
using CalculationsStateFailureMechanismProperties = Riskeer.MacroStabilityInwards.Forms.PropertyClasses.CalculationsState.MacroStabilityInwardsFailureMechanismProperties;
using RegistrationStateFailureMechanismProperties = Riskeer.MacroStabilityInwards.Forms.PropertyClasses.RegistrationState.MacroStabilityInwardsFailureMechanismProperties;
using CalculationsStateFailureMechanismView = Riskeer.MacroStabilityInwards.Forms.Views.CalculationsState.MacroStabilityInwardsFailureMechanismView;
using RegistrationStateFailureMechanismView = Riskeer.MacroStabilityInwards.Forms.Views.RegistrationState.MacroStabilityInwardsFailureMechanismView;
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
using RiskeerCommonIOResources = Riskeer.Common.IO.Properties.Resources;
using MacroStabilityInwardsFormsResources = Riskeer.MacroStabilityInwards.Forms.Properties.Resources;

namespace Riskeer.MacroStabilityInwards.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="MacroStabilityInwardsFailureMechanism"/>.
    /// </summary>
    public class MacroStabilityInwardsPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<CalculationsStateFailureMechanismContext, CalculationsStateFailureMechanismProperties>
            {
                CreateInstance = context => new CalculationsStateFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<RegistrationStateFailureMechanismContext, RegistrationStateFailureMechanismProperties>
            {
                CreateInstance = context => new RegistrationStateFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<MacroStabilityInwardsInputContext, MacroStabilityInwardsInputContextProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsInputContextProperties(
                    context, () => GetNormativeAssessmentLevel(context.AssessmentSection, context.MacroStabilityInwardsCalculation),
                    new ObservablePropertyChangeHandler(context.MacroStabilityInwardsCalculation, context.WrappedData))
            };
            yield return new PropertyInfo<MacroStabilityInwardsOutputContext, MacroStabilityInwardsOutputProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsOutputProperties(context.WrappedData.Output, context.FailureMechanism.GeneralInput.ModelFactor)
            };
            yield return new PropertyInfo<MacroStabilityInwardsSurfaceLinesContext, MacroStabilityInwardsSurfaceLineCollectionProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsSurfaceLineCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<MacroStabilityInwardsSurfaceLine, MacroStabilityInwardsSurfaceLineProperties>();
            yield return new PropertyInfo<MacroStabilityInwardsStochasticSoilModelCollectionContext, MacroStabilityInwardsStochasticSoilModelCollectionProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsStochasticSoilModelCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<MacroStabilityInwardsStochasticSoilModel, MacroStabilityInwardsStochasticSoilModelProperties>
            {
                CreateInstance = soilModel => new MacroStabilityInwardsStochasticSoilModelProperties(soilModel)
            };
            yield return new PropertyInfo<MacroStabilityInwardsStochasticSoilProfile, MacroStabilityInwardsStochasticSoilProfileProperties>
            {
                CreateInstance = soilProfile => new MacroStabilityInwardsStochasticSoilProfileProperties(soilProfile)
            };
            yield return new PropertyInfo<MacroStabilityInwardsFailureMechanismSectionsContext, FailureMechanismSectionsProperties>
            {
                CreateInstance = context => new FailureMechanismSectionsProperties(context.WrappedData)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<MacroStabilityInwardsSurfaceLinesContext>
            {
                Name = RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = MacroStabilityInwardsFormsResources.SurfaceLineIcon,
                FileFilterGenerator = SurfaceLineFileFilter,
                IsEnabled = context => HasGeometry(context.AssessmentSection.ReferenceLine),
                CreateFileImporter = (context, filePath) => new SurfaceLinesCsvImporter<MacroStabilityInwardsSurfaceLine>(
                    context.WrappedData,
                    filePath,
                    new ImportMessageProvider(),
                    SurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(context.FailureMechanism, context.AssessmentSection.ReferenceLine)),
                VerifyUpdates = context => VerifySurfaceLineUpdates(context, Resources.MacroStabilityInwardsPlugin_VerifySurfaceLineImports_When_importing_surface_lines_calculation_output_will_be_cleared_confirm)
            };

            yield return new ImportInfo<MacroStabilityInwardsStochasticSoilModelCollectionContext>
            {
                Name = RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = MacroStabilityInwardsFormsResources.SoilProfileIcon,
                FileFilterGenerator = StochasticSoilModelFileFilter,
                IsEnabled = context => HasGeometry(context.AssessmentSection.ReferenceLine),
                CreateFileImporter = (context, filePath) => new StochasticSoilModelImporter<MacroStabilityInwardsStochasticSoilModel>(
                    context.WrappedData,
                    filePath,
                    new ImportMessageProvider(),
                    MacroStabilityInwardsStochasticSoilModelImporterConfigurationFactory.CreateReplaceStrategyConfiguration(context.FailureMechanism)
                ),
                VerifyUpdates = context => VerifyStochasticSoilModelUpdates(context, Resources.MacroStabilityInwardsPlugin_VerifyStochasticSoilModelImport_When_importing_StochasticSoilModels_calculation_output_will_be_cleared_confirm)
            };

            yield return RiskeerImportInfoFactory.CreateCalculationConfigurationImportInfo<MacroStabilityInwardsCalculationGroupContext>(
                (context, filePath) =>
                    new MacroStabilityInwardsCalculationConfigurationImporter(
                        filePath,
                        context.WrappedData,
                        context.AssessmentSection.HydraulicBoundaryData.GetLocations(),
                        context.FailureMechanism));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<MacroStabilityInwardsCalculationGroupContext>(
                (context, filePath) => new MacroStabilityInwardsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any(),
                GetInquiryHelper());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<MacroStabilityInwardsCalculationScenarioContext>(
                (context, filePath) => new MacroStabilityInwardsCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                GetInquiryHelper());

            yield return new ExportInfo<MacroStabilityInwardsCalculationScenarioContext>
            {
                Name = context => Resources.MacroStabilityInwardsCalculationExporter_DisplayName,
                Extension = Resources.Stix_file_filter_extension,
                CreateFileExporter = (context, filePath) => new MacroStabilityInwardsCalculationExporter(
                    context.WrappedData, context.FailureMechanism.GeneralInput, new PersistenceFactory(), filePath,
                    () => GetNormativeAssessmentLevel(context.AssessmentSection, context.WrappedData)),
                IsEnabled = context => context.WrappedData.HasOutput,
                GetExportPath = context => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(Resources.Stix_file_filter_extension,
                                                                                                                Resources.Stix_file_filter_description))
            };

            yield return new ExportInfo<MacroStabilityInwardsCalculationGroupContext>
            {
                Name = context => Resources.MacroStabilityInwardsCalculationExporter_DisplayName,
                Extension = Resources.Stix_file_filter_extension,
                CreateFileExporter = (context, filePath) => new MacroStabilityInwardsCalculationGroupExporter(context.WrappedData, context.FailureMechanism.GeneralInput,
                                                                                                              new PersistenceFactory(), filePath, Resources.Stix_file_filter_extension,
                                                                                                              calculation => GetNormativeAssessmentLevel(context.AssessmentSection, calculation)),
                IsEnabled = context => context.WrappedData.HasOutput(),
                GetExportPath = context => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(RiskeerCommonIOResources.Zip_file_filter_Extension,
                                                                                                                RiskeerCommonIOResources.Zip_file_filter_Description))
            };
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<MacroStabilityInwardsSurfaceLinesContext>
            {
                Name = RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = MacroStabilityInwardsFormsResources.SurfaceLineIcon,
                FileFilterGenerator = SurfaceLineFileFilter,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                CreateFileImporter = (context, filePath) => new SurfaceLinesCsvImporter<MacroStabilityInwardsSurfaceLine>(
                    context.WrappedData,
                    filePath,
                    new UpdateMessageProvider(),
                    SurfaceLinesCsvImporterConfigurationFactory.CreateUpdateStrategyConfiguration(context.FailureMechanism, context.AssessmentSection.ReferenceLine)),
                VerifyUpdates = context => VerifySurfaceLineUpdates(context, Resources.MacroStabilityInwardsPlugin_VerifySurfaceLineUpdates_When_updating_surface_lines_definitions_assigned_to_calculation_output_will_be_cleared_confirm)
            };

            yield return new UpdateInfo<MacroStabilityInwardsStochasticSoilModelCollectionContext>
            {
                Name = RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Category = RiskeerCommonFormsResources.Riskeer_Category,
                Image = MacroStabilityInwardsFormsResources.SoilProfileIcon,
                FileFilterGenerator = StochasticSoilModelFileFilter,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                CreateFileImporter = (context, filePath) => new StochasticSoilModelImporter<MacroStabilityInwardsStochasticSoilModel>(
                    context.WrappedData,
                    filePath,
                    new UpdateMessageProvider(),
                    MacroStabilityInwardsStochasticSoilModelImporterConfigurationFactory.CreateUpdateStrategyConfiguration(context.FailureMechanism)
                ),
                VerifyUpdates = context =>
                    VerifyStochasticSoilModelUpdates(
                        context,
                        Resources.MacroStabilityInwardsPlugin_VerifyStochasticSoilModelUpdates_When_updating_StochasticSoilModel_definitions_assigned_to_calculation_output_will_be_cleared_confirm)
            };

            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                MacroStabilityInwardsFailureMechanismSectionsContext, MacroStabilityInwardsFailureMechanism, AdoptableFailureMechanismSectionResult>(
                c => new MacroStabilityInwardsFailureMechanismSectionUpdateStrategy((MacroStabilityInwardsFailureMechanism) c.WrappedData, new AdoptableFailureMechanismSectionResultUpdateStrategy()));
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
                CloseForData = CloseFailureMechanismViewForData
            };

            yield return new RiskeerViewInfo<
                MacroStabilityInwardsFailureMechanismSectionResultContext,
                IObservableEnumerable<AdoptableFailureMechanismSectionResult>,
                MacroStabilityInwardsFailureMechanismResultView>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new MacroStabilityInwardsFailureMechanismResultView(
                    context.WrappedData, (MacroStabilityInwardsFailureMechanism) context.FailureMechanism, context.AssessmentSection)
            };

            yield return new RiskeerViewInfo<MacroStabilityInwardsCalculationGroupContext, CalculationGroup, MacroStabilityInwardsCalculationsView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => context.WrappedData.Name,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CreateInstance = context => new MacroStabilityInwardsCalculationsView(context.WrappedData, context.FailureMechanism, context.AssessmentSection),
                CloseForData = CloseCalculationsViewForData
            };

            yield return new RiskeerViewInfo<MacroStabilityInwardsInputContext, MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInputView>(() => Gui)
            {
                GetViewData = context => context.MacroStabilityInwardsCalculation,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                CloseForData = CloseInputViewForData,
                CreateInstance = context => new MacroStabilityInwardsInputView(context.MacroStabilityInwardsCalculation,
                                                                               context.FailureMechanism.GeneralInput,
                                                                               context.AssessmentSection,
                                                                               () => context.AssessmentSection.GetNormativeHydraulicBoundaryLocationCalculation(context.WrappedData.HydraulicBoundaryLocation))
            };

            yield return new RiskeerViewInfo<MacroStabilityInwardsScenariosContext, CalculationGroup, MacroStabilityInwardsScenariosView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Scenarios_DisplayName,
                CloseForData = CloseScenariosViewForData,
                CreateInstance = context => new MacroStabilityInwardsScenariosView(context.WrappedData, context.FailureMechanism)
            };

            yield return new RiskeerViewInfo<MacroStabilityInwardsOutputContext, MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsOutputView>(() => Gui)
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new MacroStabilityInwardsOutputView(context.WrappedData, context.FailureMechanism.GeneralInput,
                                                                                () => GetNormativeAssessmentLevel(context.AssessmentSection, context.WrappedData))
            };

            yield return new RiskeerViewInfo<MacroStabilityInwardsFailureMechanismSectionsContext, IEnumerable<FailureMechanismSection>,
                FailureMechanismSectionConfigurationsView<FailureMechanismSectionConfiguration, FailureMechanismSectionConfigurationRow>>(() => Gui)
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView,
                CreateInstance = context =>
                {
                    var failureMechanism = (MacroStabilityInwardsFailureMechanism) context.WrappedData;
                    return new FailureMechanismSectionConfigurationsView<FailureMechanismSectionConfiguration, FailureMechanismSectionConfigurationRow>(
                        failureMechanism.SectionConfigurations, failureMechanism, (configuration, start, end) => new FailureMechanismSectionConfigurationRow(configuration, start, end, failureMechanism.GeneralInput.B));
                },
                GetViewData = context => context.WrappedData.Sections
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

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<MacroStabilityInwardsCalculationScenarioContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved,
                CalculationType.SemiProbabilistic);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<MacroStabilityInwardsCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return new TreeNodeInfo<MacroStabilityInwardsFailureMechanismSectionResultContext>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsInputContext>
            {
                Text = context => RiskeerCommonFormsResources.Calculation_Input,
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsSurfaceLinesContext>
            {
                Text = context => RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = context => context.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = MacroStabilityInwardsSurfaceLinesContextContextMenuStrip
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsSurfaceLine>
            {
                Text = surfaceLine => surfaceLine.Name,
                Image = surfaceLine => MacroStabilityInwardsFormsResources.SurfaceLineIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsStochasticSoilModelCollectionContext>
            {
                Text = stochasticSoilModelContext => RiskeerCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Image = stochasticSoilModelContext => RiskeerCommonFormsResources.GeneralFolderIcon,
                ForeColor = stochasticSoilModelContext => stochasticSoilModelContext.WrappedData.Any()
                                                              ? Color.FromKnownColor(KnownColor.ControlText)
                                                              : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = stochasticSoilModelContext => stochasticSoilModelContext.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = StochasticSoilModelCollectionContextContextMenuStrip
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsStochasticSoilModel>
            {
                Text = stochasticSoilModel => stochasticSoilModel.Name,
                Image = stochasticSoilModel => MacroStabilityInwardsFormsResources.StochasticSoilModelIcon,
                ChildNodeObjects = stochasticSoilModel => stochasticSoilModel.StochasticSoilProfiles.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsStochasticSoilProfile>
            {
                Text = stochasticSoilProfile => stochasticSoilProfile.SoilProfile != null ? stochasticSoilProfile.SoilProfile.Name : "Profile",
                Image = stochasticSoilProfile => MacroStabilityInwardsFormsResources.SoilProfileIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsOutputContext>
            {
                Text = context => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
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

            yield return new TreeNodeInfo<MacroStabilityInwardsScenariosContext>
            {
                Text = context => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = context => RiskeerCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
            
            yield return new TreeNodeInfo<MacroStabilityInwardsFailureMechanismSectionsContext>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = context => RiskeerCommonFormsResources.SectionsIcon,
                ForeColor = context => context.WrappedData.Sections.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = FailureMechanismSectionsContextMenuStrip
            };
        }

        private static RoundedDouble GetNormativeAssessmentLevel(IAssessmentSection assessmentSection, MacroStabilityInwardsCalculation calculation)
        {
            return assessmentSection.GetNormativeAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation);
        }

        private static void ValidateAll(IEnumerable<MacroStabilityInwardsCalculation> calculations, GeneralMacroStabilityInwardsInput generalInput, IAssessmentSection assessmentSection)
        {
            foreach (MacroStabilityInwardsCalculation calculation in calculations)
            {
                MacroStabilityInwardsCalculationService.Validate(calculation, generalInput, GetNormativeAssessmentLevel(assessmentSection, calculation));
            }
        }

        #region ViewInfos

        private static bool CloseFailureMechanismViewForData(RegistrationStateFailureMechanismView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as MacroStabilityInwardsFailureMechanism;

            return dataToCloseFor is IAssessmentSection assessmentSection
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        private static bool CloseFailureMechanismResultViewForData(MacroStabilityInwardsFailureMechanismResultView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as MacroStabilityInwardsFailureMechanism;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<MacroStabilityInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is IFailureMechanismContext<MacroStabilityInwardsFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        private static bool CloseCalculationsViewForData(MacroStabilityInwardsCalculationsView view, object dataToCloseFor)
        {
            MacroStabilityInwardsFailureMechanism failureMechanism = null;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<MacroStabilityInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is CalculationsStateFailureMechanismContext failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        private static bool CloseScenariosViewForData(MacroStabilityInwardsScenariosView view, object dataToCloseFor)
        {
            var failureMechanism = dataToCloseFor as MacroStabilityInwardsFailureMechanism;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<MacroStabilityInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is FailureMechanismContext<MacroStabilityInwardsFailureMechanism> failureMechanismContext)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        private static bool CloseInputViewForData(MacroStabilityInwardsInputView view, object dataToCloseFor)
        {
            MacroStabilityInwardsFailureMechanism failureMechanism = null;

            if (dataToCloseFor is IAssessmentSection assessmentSection)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<MacroStabilityInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (dataToCloseFor is CalculationsStateFailureMechanismContext calculationsContext)
            {
                failureMechanism = calculationsContext.WrappedData;
            }

            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = null;

            if (failureMechanism != null)
            {
                calculations = failureMechanism.CalculationsGroup.GetCalculations()
                                               .OfType<MacroStabilityInwardsCalculationScenario>();
            }

            if (dataToCloseFor is MacroStabilityInwardsCalculationGroupContext calculationGroupContext)
            {
                calculations = calculationGroupContext.WrappedData.GetCalculations()
                                                      .OfType<MacroStabilityInwardsCalculationScenario>();
            }

            if (dataToCloseFor is MacroStabilityInwardsCalculationScenarioContext calculationScenarioContext)
            {
                calculations = new[]
                {
                    calculationScenarioContext.WrappedData
                };
            }

            return calculations != null && calculations.Any(ci => ReferenceEquals(view.Data, ci));
        }

        #endregion

        #region TreeNodeInfos

        #region MacroStabilityInwardsFailureMechanismSectionsContext TreeNodeInfo

        private ContextMenuStrip FailureMechanismSectionsContextMenuStrip(MacroStabilityInwardsFailureMechanismSectionsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddImportItem(new ImportInfo[]
                      {
                          RiskeerImportInfoFactory.CreateFailureMechanismSectionsImportInfo<MacroStabilityInwardsFailureMechanismSectionsContext>(
                              c => new MacroStabilityInwardsFailureMechanismSectionReplaceStrategy((MacroStabilityInwardsFailureMechanism) c.WrappedData))
                      })
                      .AddUpdateItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion
        
        #region MacroStabilityInwardsSurfaceLinesContext TreeNodeInfo

        private ContextMenuStrip MacroStabilityInwardsSurfaceLinesContextContextMenuStrip(MacroStabilityInwardsSurfaceLinesContext nodeData, object parentData, TreeViewControl treeViewControl)
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

        #region MacroStabilityInwardsStochasticSoilModelCollectionContext TreeNodeInfo

        private ContextMenuStrip StochasticSoilModelCollectionContextContextMenuStrip(MacroStabilityInwardsStochasticSoilModelCollectionContext nodeData, object parentData, TreeViewControl treeViewControl)
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

        #region CalculationsStateFailureMechanismContext TreeNodeInfo

        private static object[] CalculationsStateFailureMechanismChildNodeObjects(CalculationsStateFailureMechanismContext context)
        {
            MacroStabilityInwardsFailureMechanism failureMechanism = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetCalculationsStateFailureMechanismInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new MacroStabilityInwardsCalculationGroupContext(failureMechanism.CalculationsGroup, null, failureMechanism.SurfaceLines,
                                                                 failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection)
            };
        }

        private static IEnumerable<object> GetCalculationsStateFailureMechanismInputs(MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MacroStabilityInwardsSurfaceLinesContext(failureMechanism.SurfaceLines, failureMechanism, assessmentSection),
                new MacroStabilityInwardsStochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection),
                failureMechanism.CalculationsInputComments
            };
        }

        private ContextMenuStrip CalculationsStateFailureMechanismContextMenuStrip(CalculationsStateFailureMechanismContext context,
                                                                                   object parentData,
                                                                                   TreeViewControl treeViewControl)
        {
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
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static void ValidateAllInFailureMechanism(CalculationsStateFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<MacroStabilityInwardsCalculation>(), context.WrappedData.GeneralInput, context.Parent);
        }

        private void CalculateAllInFailureMechanism(CalculationsStateFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData, context.Parent));
        }

        #endregion

        #region RegistrationStateFailureMechanismContext TreeNodeInfo

        private static object[] RegistrationStateFailureMechanismEnabledChildNodeObjects(RegistrationStateFailureMechanismContext context)
        {
            MacroStabilityInwardsFailureMechanism failureMechanism = context.WrappedData;
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

        private static IEnumerable<object> GetRegistrationStateFailureMechanismInputs(MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MacroStabilityInwardsFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InAssemblyInputComments
            };
        }

        private static IEnumerable<object> GetRegistrationStateFailureMechanismOutputs(MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MacroStabilityInwardsScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new MacroStabilityInwardsFailureMechanismSectionResultContext(
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

        #region MacroStabilityInwardsCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(MacroStabilityInwardsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                if (item is MacroStabilityInwardsCalculationScenario calculation)
                {
                    childNodeObjects.Add(new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             nodeData.WrappedData,
                                                                                             nodeData.AvailableMacroStabilityInwardsSurfaceLines,
                                                                                             nodeData.AvailableStochasticSoilModels,
                                                                                             nodeData.FailureMechanism,
                                                                                             nodeData.AssessmentSection));
                }
                else if (item is CalculationGroup group)
                {
                    childNodeObjects.Add(new MacroStabilityInwardsCalculationGroupContext(group,
                                                                                          nodeData.WrappedData,
                                                                                          nodeData.AvailableMacroStabilityInwardsSurfaceLines,
                                                                                          nodeData.AvailableStochasticSoilModels,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(MacroStabilityInwardsCalculationGroupContext nodeData,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = nodeData.WrappedData;
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            bool isNestedGroup = parentData is MacroStabilityInwardsCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateMacroStabilityInwardsCalculationsItem(nodeData);

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
                   .AddCreateCalculationItem(nodeData, AddCalculationScenario, CalculationType.SemiProbabilistic)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem()
                       .AddSeparator();
            }

            builder.AddValidateAllCalculationsInGroupItem(
                       nodeData,
                       ValidateAllInCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(
                       nodeData,
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

        private StrictContextMenuItem CreateGenerateMacroStabilityInwardsCalculationsItem(MacroStabilityInwardsCalculationGroupContext nodeData)
        {
            bool surfaceLineAvailable = nodeData.AvailableMacroStabilityInwardsSurfaceLines.Any() && nodeData.AvailableStochasticSoilModels.Any();

            string calculationGroupGenerateCalculationsToolTip = surfaceLineAvailable
                                                                     ? MacroStabilityInwardsFormsResources.MacroStabilityInwardsCalculationGroup_Generate_MacroStabilityInwardsCalculations_ToolTip
                                                                     : MacroStabilityInwardsFormsResources.MacroStabilityInwardsCalculationGroup_Generate_MacroStabilityInwardsCalculations_NoSurfaceLinesOrSoilModels_ToolTip;

            return new StrictContextMenuItem(
                RiskeerCommonFormsResources.CalculationGroup_Generate_Scenarios,
                calculationGroupGenerateCalculationsToolTip,
                RiskeerCommonFormsResources.GenerateScenariosIcon, (o, args) => ShowSurfaceLineSelectionDialog(nodeData))
            {
                Enabled = surfaceLineAvailable
            };
        }

        private void ShowSurfaceLineSelectionDialog(MacroStabilityInwardsCalculationGroupContext nodeData)
        {
            using (var dialog = new MacroStabilityInwardsSurfaceLineSelectionDialog(Gui.MainWindow, nodeData.AvailableMacroStabilityInwardsSurfaceLines))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateCalculations(nodeData.WrappedData, dialog.SelectedItems, nodeData.AvailableStochasticSoilModels);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateCalculations(CalculationGroup target, IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines, IEnumerable<MacroStabilityInwardsStochasticSoilModel> soilModels)
        {
            foreach (ICalculationBase group in MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(surfaceLines, soilModels))
            {
                target.Children.Add(group);
            }
        }

        private static void AddCalculationScenario(MacroStabilityInwardsCalculationGroupContext nodeData)
        {
            var calculation = new MacroStabilityInwardsCalculationScenario
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children, RiskeerCommonDataResources.Calculation_DefaultName, c => c.Name)
            };

            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private static void CalculationGroupContextOnNodeRemoved(MacroStabilityInwardsCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (MacroStabilityInwardsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        private static void ValidateAllInCalculationGroup(MacroStabilityInwardsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<MacroStabilityInwardsCalculation>(), context.FailureMechanism.GeneralInput, context.AssessmentSection);
        }

        private void CalculateAllInCalculationGroup(MacroStabilityInwardsCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData,
                                                                                            context.FailureMechanism.GeneralInput,
                                                                                            context.AssessmentSection));
        }

        #endregion

        #region MacroStabilityInwardsCalculationScenarioContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(MacroStabilityInwardsCalculationScenarioContext context)
        {
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = context.WrappedData;

            return new object[]
            {
                macroStabilityInwardsCalculationScenario.Comments,
                new MacroStabilityInwardsInputContext(macroStabilityInwardsCalculationScenario.InputParameters,
                                                      macroStabilityInwardsCalculationScenario,
                                                      context.AvailableMacroStabilityInwardsSurfaceLines,
                                                      context.AvailableStochasticSoilModels,
                                                      context.FailureMechanism,
                                                      context.AssessmentSection),
                new MacroStabilityInwardsOutputContext(macroStabilityInwardsCalculationScenario,
                                                       context.FailureMechanism,
                                                       context.AssessmentSection)
            };
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(MacroStabilityInwardsCalculationScenarioContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            MacroStabilityInwardsCalculationScenario calculation = nodeData.WrappedData;

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, nodeData)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              nodeData,
                              Validate)
                          .AddPerformCalculationItem<MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsCalculationScenarioContext>(
                              nodeData,
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

        private static void CalculationContextOnNodeRemoved(MacroStabilityInwardsCalculationScenarioContext macroStabilityInwardsCalculationScenarioContext, object parentNodeData)
        {
            if (parentNodeData is MacroStabilityInwardsCalculationGroupContext calculationGroupContext)
            {
                bool successfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(macroStabilityInwardsCalculationScenarioContext.WrappedData);
                if (successfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
            }
        }

        private static void Validate(MacroStabilityInwardsCalculationScenarioContext context)
        {
            MacroStabilityInwardsCalculationService.Validate(context.WrappedData, context.FailureMechanism.GeneralInput, GetNormativeAssessmentLevel(context.AssessmentSection, context.WrappedData));
        }

        private void Calculate(MacroStabilityInwardsCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivity(context.WrappedData,
                                                                                          context.FailureMechanism.GeneralInput,
                                                                                          context.AssessmentSection));
        }

        #endregion

        #endregion

        #region ImportInfos

        private static FileFilterGenerator SurfaceLineFileFilter =>
            new FileFilterGenerator(
                RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                $"{RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor} {RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description}");

        private static FileFilterGenerator StochasticSoilModelFileFilter =>
            new FileFilterGenerator(Resources.Soil_file_Extension, Resources.Soil_file_Description);

        private bool VerifySurfaceLineUpdates(MacroStabilityInwardsSurfaceLinesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism,
                                                                             query, GetInquiryHelper());

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static bool HasGeometry(ReferenceLine referenceLine)
        {
            return referenceLine.Points.Any();
        }

        private bool VerifyStochasticSoilModelUpdates(MacroStabilityInwardsStochasticSoilModelCollectionContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism,
                                                                             query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}