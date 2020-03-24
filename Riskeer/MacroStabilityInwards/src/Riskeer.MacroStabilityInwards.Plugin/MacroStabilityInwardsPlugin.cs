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
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Probability;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.Helpers;
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
using RiskeerCommonFormsResources = Riskeer.Common.Forms.Properties.Resources;
using RiskeerCommonDataResources = Riskeer.Common.Data.Properties.Resources;
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
            yield return new PropertyInfo<MacroStabilityInwardsFailureMechanismContext, MacroStabilityInwardsFailureMechanismProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsFailureMechanismProperties(context.WrappedData, context.Parent)
            };
            yield return new PropertyInfo<MacroStabilityInwardsInputContext, MacroStabilityInwardsInputContextProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsInputContextProperties(context,
                                                                                            () => GetNormativeAssessmentLevel(context.AssessmentSection, context.MacroStabilityInwardsCalculation),
                                                                                            new ObservablePropertyChangeHandler(context.MacroStabilityInwardsCalculation, context.WrappedData))
            };
            yield return new PropertyInfo<MacroStabilityInwardsOutputContext, MacroStabilityInwardsOutputProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsOutputProperties(context.WrappedData.Output, context.FailureMechanism, context.AssessmentSection)
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
            yield return new PropertyInfo<MacroStabilityInwardsFailureMechanismSectionsContext, FailureMechanismSectionsProbabilityAssessmentProperties>
            {
                CreateInstance = context => new FailureMechanismSectionsProbabilityAssessmentProperties(
                    context.WrappedData, ((MacroStabilityInwardsFailureMechanism) context.WrappedData).MacroStabilityInwardsProbabilityAssessmentInput)
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
                        context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
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
                Name = Resources.MacroStabilityInwardsCalculationExporter_DisplayName,
                Extension = Resources.Stix_file_filter_extension,
                CreateFileExporter = (context, filePath) => new MacroStabilityInwardsCalculationExporter(context.WrappedData, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                GetExportPath = () => ExportHelper.GetFilePath(GetInquiryHelper(), new FileFilterGenerator(Resources.Stix_file_filter_extension,
                                                                                                           Resources.Stix_file_filter_description))
            };

            yield return new ExportInfo<MacroStabilityInwardsCalculationGroupContext>
            {
                Name = Resources.MacroStabilityInwardsCalculationExporter_DisplayName,
                Extension = Resources.Stix_file_filter_extension,
                CreateFileExporter = (context, filePath) => new MacroStabilityInwardsCalculationGroupExporter(context.WrappedData, filePath),
                IsEnabled = context => context.WrappedData.HasOutput(),
                GetExportPath = () => ExportHelper.GetFolderPath(GetInquiryHelper())
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
                MacroStabilityInwardsFailureMechanismSectionsContext, MacroStabilityInwardsFailureMechanism, MacroStabilityInwardsFailureMechanismSectionResult>(
                new MacroStabilityInwardsFailureMechanismSectionResultUpdateStrategy());
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<MacroStabilityInwardsFailureMechanismContext, MacroStabilityInwardsFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new MacroStabilityInwardsFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>,
                IObservableEnumerable<MacroStabilityInwardsFailureMechanismSectionResult>,
                MacroStabilityInwardsFailureMechanismResultView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new MacroStabilityInwardsFailureMechanismResultView(
                    context.WrappedData,
                    (MacroStabilityInwardsFailureMechanism) context.FailureMechanism, context.AssessmentSection)
            };

            yield return new ViewInfo<MacroStabilityInwardsCalculationGroupContext, CalculationGroup, MacroStabilityInwardsCalculationsView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.GeneralFolderIcon,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CloseForData = CloseCalculationsViewForData,
                AfterCreate = (view, context) =>
                {
                    view.AssessmentSection = context.AssessmentSection;
                    view.MacroStabilityInwardsFailureMechanism = context.FailureMechanism;
                }
            };

            yield return new ViewInfo<MacroStabilityInwardsInputContext, MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsInputView>
            {
                GetViewData = context => context.MacroStabilityInwardsCalculation,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Calculation_Input,
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                CloseForData = CloseInputViewForData,
                CreateInstance = context => new MacroStabilityInwardsInputView(context.MacroStabilityInwardsCalculation,
                                                                               context.AssessmentSection,
                                                                               () => context.AssessmentSection.GetNormativeHydraulicBoundaryLocationCalculation(context.WrappedData.HydraulicBoundaryLocation))
            };

            yield return new ViewInfo<MacroStabilityInwardsScenariosContext, CalculationGroup, MacroStabilityInwardsScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.Scenarios_DisplayName,
                Image = RiskeerCommonFormsResources.ScenariosIcon,
                CloseForData = CloseScenariosViewForData,
                AfterCreate = (view, context) => view.MacroStabilityInwardsFailureMechanism = context.FailureMechanism,
                CreateInstance = context => new MacroStabilityInwardsScenariosView(context.AssessmentSection)
            };

            yield return new ViewInfo<MacroStabilityInwardsOutputContext, MacroStabilityInwardsCalculationScenario, MacroStabilityInwardsOutputView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RiskeerCommonFormsResources.CalculationOutput_DisplayName,
                Image = RiskeerCommonFormsResources.GeneralOutputIcon,
                CloseForData = RiskeerPluginHelper.ShouldCloseViewWithCalculationData,
                CreateInstance = context => new MacroStabilityInwardsOutputView(context.WrappedData,
                                                                                () => GetNormativeAssessmentLevel(context.AssessmentSection, context.WrappedData))
            };

            yield return new ViewInfo<MacroStabilityInwardsFailureMechanismSectionsContext, IEnumerable<FailureMechanismSection>, FailureMechanismSectionsProbabilityAssessmentView>
            {
                GetViewName = (view, context) => RiskeerCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = RiskeerCommonFormsResources.SectionsIcon,
                CloseForData = RiskeerPluginHelper.ShouldCloseForFailureMechanismView,
                CreateInstance = context => new FailureMechanismSectionsProbabilityAssessmentView(context.WrappedData.Sections,
                                                                                                  context.WrappedData,
                                                                                                  ((MacroStabilityInwardsFailureMechanism) context.WrappedData).MacroStabilityInwardsProbabilityAssessmentInput),
                GetViewData = context => context.WrappedData.Sections
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<MacroStabilityInwardsFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<MacroStabilityInwardsCalculationScenarioContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<MacroStabilityInwardsCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>>
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
        }

        private static FileFilterGenerator StochasticSoilModelFileFilter
        {
            get
            {
                return new FileFilterGenerator(Resources.Soil_file_Extension, Resources.Soil_file_Description);
            }
        }

        private static RoundedDouble GetNormativeAssessmentLevel(IAssessmentSection assessmentSection, MacroStabilityInwardsCalculation calculation)
        {
            return assessmentSection.GetNormativeAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation);
        }

        private bool VerifyStochasticSoilModelUpdates(MacroStabilityInwardsStochasticSoilModelCollectionContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism,
                                                                             query, GetInquiryHelper());
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static bool HasGeometry(ReferenceLine referenceLine)
        {
            return referenceLine.Points.Any();
        }

        #region MacroStabilityInwardsFailureMechanismView ViewInfo

        private static bool CloseFailureMechanismViewForData(MacroStabilityInwardsFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as MacroStabilityInwardsFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        #endregion

        #region FailureMechanismResultsView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(MacroStabilityInwardsFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as MacroStabilityInwardsFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<MacroStabilityInwardsFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<MacroStabilityInwardsFailureMechanism>()
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #region MacroStabilityInwardsCalculationsView ViewInfo

        private static bool CloseCalculationsViewForData(MacroStabilityInwardsCalculationsView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as MacroStabilityInwardsFailureMechanism;
            var failureMechanismContext = o as MacroStabilityInwardsFailureMechanismContext;

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<MacroStabilityInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion endregion

        #region MacroStabilityInwardsScenariosView ViewInfo

        private static bool CloseScenariosViewForData(MacroStabilityInwardsScenariosView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as MacroStabilityInwardsFailureMechanism;
            var failureMechanismContext = o as MacroStabilityInwardsFailureMechanismContext;

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<MacroStabilityInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion endregion

        #region MacroStabilityInwardsInputView ViewInfo

        private static bool CloseInputViewForData(MacroStabilityInwardsInputView view, object o)
        {
            var calculationScenarioContext = o as MacroStabilityInwardsCalculationScenarioContext;
            if (calculationScenarioContext != null)
            {
                return ReferenceEquals(view.Data, calculationScenarioContext.WrappedData);
            }

            IEnumerable<MacroStabilityInwardsCalculationScenario> calculations = null;

            var calculationGroupContext = o as MacroStabilityInwardsCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculations = calculationGroupContext.WrappedData.GetCalculations()
                                                      .OfType<MacroStabilityInwardsCalculationScenario>();
            }

            var failureMechanism = o as MacroStabilityInwardsFailureMechanism;

            var failureMechanismContext = o as MacroStabilityInwardsFailureMechanismContext;
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            var assessmentSection = o as IAssessmentSection;
            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<MacroStabilityInwardsFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (failureMechanism != null)
            {
                calculations = failureMechanism.CalculationsGroup.GetCalculations()
                                               .OfType<MacroStabilityInwardsCalculationScenario>();
            }

            return calculations != null && calculations.Any(ci => ReferenceEquals(view.Data, ci));
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

        #region MacroStabilityInwardsFailureMechanismContext TreeNodeInfo

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(MacroStabilityInwardsFailureMechanismContext macroStabilityInwardsFailureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(macroStabilityInwardsFailureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(macroStabilityInwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              macroStabilityInwardsFailureMechanismContext,
                              ValidateAll)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              macroStabilityInwardsFailureMechanismContext,
                              CalculateAll)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(macroStabilityInwardsFailureMechanismContext.WrappedData)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(MacroStabilityInwardsFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(MacroStabilityInwardsFailureMechanismContext macroStabilityInwardsFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(macroStabilityInwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(macroStabilityInwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static object[] FailureMechanismEnabledChildNodeObjects(MacroStabilityInwardsFailureMechanismContext macroStabilityInwardsFailureMechanismContext)
        {
            MacroStabilityInwardsFailureMechanism wrappedData = macroStabilityInwardsFailureMechanismContext.WrappedData;
            IAssessmentSection assessmentSection = macroStabilityInwardsFailureMechanismContext.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, assessmentSection), TreeFolderCategory.Input),
                new MacroStabilityInwardsCalculationGroupContext(wrappedData.CalculationsGroup, null, wrappedData.SurfaceLines, wrappedData.StochasticSoilModels, wrappedData, assessmentSection),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(MacroStabilityInwardsFailureMechanismContext macroStabilityInwardsFailureMechanismContext)
        {
            return new object[]
            {
                macroStabilityInwardsFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new MacroStabilityInwardsFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new MacroStabilityInwardsSurfaceLinesContext(failureMechanism.SurfaceLines, failureMechanism, assessmentSection),
                new MacroStabilityInwardsStochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            MacroStabilityInwardsProbabilityAssessmentInput probabilityAssessmentInput = failureMechanism.MacroStabilityInwardsProbabilityAssessmentInput;
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism,
                                                              assessmentSection,
                                                              () => probabilityAssessmentInput.GetN(assessmentSection.ReferenceLine.Length)),
                new MacroStabilityInwardsScenariosContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection),
                new ProbabilityFailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.OutputComments
            };
        }

        #endregion

        #region MacroStabilityInwardsCalculationScenarioContext TreeNodeInfo

        private ContextMenuStrip CalculationContextContextMenuStrip(MacroStabilityInwardsCalculationScenarioContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            MacroStabilityInwardsCalculation calculation = nodeData.WrappedData;

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, nodeData)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              nodeData,
                              Validate)
                          .AddPerformCalculationItem(
                              calculation,
                              nodeData,
                              PerformCalculation)
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

        private static void CalculationContextOnNodeRemoved(MacroStabilityInwardsCalculationScenarioContext macroStabilityInwardsCalculationScenarioContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as MacroStabilityInwardsCalculationGroupContext;
            if (calculationGroupContext != null)
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
            MacroStabilityInwardsCalculationService.Validate(context.WrappedData, GetNormativeAssessmentLevel(context.AssessmentSection, context.WrappedData));
        }

        private void PerformCalculation(MacroStabilityInwardsCalculation calculation, MacroStabilityInwardsCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivity(calculation, context.AssessmentSection));
        }

        #endregion

        #region MacroStabilityInwardsCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(MacroStabilityInwardsCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as MacroStabilityInwardsCalculationScenario;
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new MacroStabilityInwardsCalculationScenarioContext(calculation,
                                                                                             nodeData.WrappedData,
                                                                                             nodeData.AvailableMacroStabilityInwardsSurfaceLines,
                                                                                             nodeData.AvailableStochasticSoilModels,
                                                                                             nodeData.FailureMechanism,
                                                                                             nodeData.AssessmentSection));
                }
                else if (group != null)
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
                   .AddCreateCalculationItem(nodeData, AddCalculationScenario)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem()
                       .AddSeparator();
            }

            builder.AddValidateAllCalculationsInGroupItem(
                       nodeData,
                       ValidateAll)
                   .AddPerformAllCalculationsInGroupItem(
                       group,
                       nodeData,
                       CalculateAll)
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

        private void CalculateAll(MacroStabilityInwardsFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(context.WrappedData, context.Parent));
        }

        private void CalculateAll(CalculationGroup group, MacroStabilityInwardsCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                MacroStabilityInwardsCalculationActivityFactory.CreateCalculationActivities(group, context.AssessmentSection));
        }

        private static void ValidateAll(IEnumerable<MacroStabilityInwardsCalculation> calculations, IAssessmentSection assessmentSection)
        {
            foreach (MacroStabilityInwardsCalculation calculation in calculations)
            {
                MacroStabilityInwardsCalculationService.Validate(calculation, GetNormativeAssessmentLevel(assessmentSection, calculation));
            }
        }

        private static void ValidateAll(MacroStabilityInwardsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<MacroStabilityInwardsCalculation>(), context.AssessmentSection);
        }

        private static void ValidateAll(MacroStabilityInwardsFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<MacroStabilityInwardsCalculation>(), context.Parent);
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
            using (var view = new MacroStabilityInwardsSurfaceLineSelectionDialog(Gui.MainWindow, nodeData.AvailableMacroStabilityInwardsSurfaceLines))
            {
                view.ShowDialog();
                GenerateCalculations(nodeData.WrappedData, view.SelectedItems, nodeData.AvailableStochasticSoilModels);
            }

            nodeData.NotifyObservers();
        }

        private static void GenerateCalculations(CalculationGroup target, IEnumerable<MacroStabilityInwardsSurfaceLine> surfaceLines, IEnumerable<MacroStabilityInwardsStochasticSoilModel> soilModels)
        {
            foreach (ICalculationBase group in MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(surfaceLines, soilModels))
            {
                target.Children.Add(group);
            }
        }

        private static void CalculationGroupContextOnNodeRemoved(MacroStabilityInwardsCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (MacroStabilityInwardsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        #endregion

        #region Macro stability inwards surface line importer

        private static FileFilterGenerator SurfaceLineFileFilter
        {
            get
            {
                return new FileFilterGenerator(
                    RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                    $"{RiskeerCommonDataResources.SurfaceLineCollection_TypeDescriptor} {RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description}");
            }
        }

        private bool VerifySurfaceLineUpdates(MacroStabilityInwardsSurfaceLinesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism,
                                                                             query, GetInquiryHelper());

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}