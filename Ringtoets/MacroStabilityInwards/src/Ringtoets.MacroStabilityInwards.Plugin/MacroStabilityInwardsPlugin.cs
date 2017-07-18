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
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.MacroStabilityInwards.Data;
using Ringtoets.MacroStabilityInwards.Forms;
using Ringtoets.MacroStabilityInwards.Forms.PresentationObjects;
using Ringtoets.MacroStabilityInwards.Forms.PropertyClasses;
using Ringtoets.MacroStabilityInwards.Forms.Views;
using Ringtoets.MacroStabilityInwards.IO.Configurations;
using Ringtoets.MacroStabilityInwards.IO.Importers;
using Ringtoets.MacroStabilityInwards.Plugin.FileImporter;
using Ringtoets.MacroStabilityInwards.Plugin.Properties;
using Ringtoets.MacroStabilityInwards.Primitives;
using Ringtoets.MacroStabilityInwards.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using MacroStabilityInwardsDataResources = Ringtoets.MacroStabilityInwards.Data.Properties.Resources;
using MacroStabilityInwardsFormsResources = Ringtoets.MacroStabilityInwards.Forms.Properties.Resources;

namespace Ringtoets.MacroStabilityInwards.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="MacroStabilityInwardsFailureMechanism"/>.
    /// </summary>
    public class MacroStabilityInwardsPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<MacroStabilityInwardsFailureMechanismContext, MacroStabilityInwardsFailureMechanismContextProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsFailureMechanismContextProperties(context, new FailureMechanismPropertyChangeHandler<MacroStabilityInwardsFailureMechanism>())
            };
            yield return new PropertyInfo<MacroStabilityInwardsInputContext, MacroStabilityInwardsInputContextProperties>
            {
                CreateInstance = context => new MacroStabilityInwardsInputContextProperties(context, new ObservablePropertyChangeHandler(context.MacroStabilityInwardsCalculation, context.WrappedData))
            };
            yield return new PropertyInfo<MacroStabilityInwardsOutputContext, MacroStabilityInwardsOutputContextProperties>();
            yield return new PropertyInfo<RingtoetsMacroStabilityInwardsSurfaceLinesContext, RingtoetsMacroStabilityInwardsSurfaceLineCollectionProperties>
            {
                CreateInstance = context => new RingtoetsMacroStabilityInwardsSurfaceLineCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<RingtoetsMacroStabilityInwardsSurfaceLine, RingtoetsMacroStabilityInwardsSurfaceLineProperties>();
            yield return new PropertyInfo<StochasticSoilModelCollectionContext, StochasticSoilModelCollectionProperties>
            {
                CreateInstance = context => new StochasticSoilModelCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<StochasticSoilModel, StochasticSoilModelProperties>();
            yield return new PropertyInfo<StochasticSoilProfile, StochasticSoilProfileProperties>();
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<RingtoetsMacroStabilityInwardsSurfaceLinesContext>
            {
                Name = MacroStabilityInwardsDataResources.MacroStabilityInwardsSurfaceLineCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = MacroStabilityInwardsFormsResources.SurfaceLineIcon,
                FileFilterGenerator = SurfaceLineFileFilter,
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                CreateFileImporter = (context, filePath) => new SurfaceLinesCsvImporter<RingtoetsMacroStabilityInwardsSurfaceLine>(
                    context.WrappedData,
                    filePath,
                    new ImportMessageProvider(),
                    SurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(context.FailureMechanism, context.AssessmentSection.ReferenceLine)),
                VerifyUpdates = context => VerifySurfaceLineUpdates(context, Resources.MacroStabilityInwardsPlugin_VerifySurfaceLineImports_When_importing_surfacelines_calculation_output_will_be_cleared_confirm)
            };

            yield return new ImportInfo<StochasticSoilModelCollectionContext>
            {
                Name = MacroStabilityInwardsDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = MacroStabilityInwardsFormsResources.SoilProfileIcon,
                FileFilterGenerator = StochasticSoilModelFileFilter,
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                CreateFileImporter = (context, filePath) => StochasticSoilModelImporter(context, filePath, new ImportMessageProvider(), new StochasticSoilModelReplaceDataStrategy(context.FailureMechanism)),
                VerifyUpdates = context => VerifyStochasticSoilModelUpdates(context, Resources.MacroStabilityInwardsPlugin_VerifyStochasticSoilModelImport_When_importing_StochasticSoilModels_calculation_output_will_be_cleared_confirm)
            };

            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<MacroStabilityInwardsCalculationGroupContext>(
                (context, filePath) =>
                    new MacroStabilityInwardsCalculationConfigurationImporter(
                        filePath,
                        context.WrappedData,
                        context.AvailableHydraulicBoundaryLocations,
                        context.FailureMechanism));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<MacroStabilityInwardsCalculationGroupContext>(
                (context, filePath) => new MacroStabilityInwardsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<MacroStabilityInwardsCalculationScenarioContext>(
                (context, filePath) => new MacroStabilityInwardsCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<RingtoetsMacroStabilityInwardsSurfaceLinesContext>
            {
                Name = MacroStabilityInwardsDataResources.MacroStabilityInwardsSurfaceLineCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = MacroStabilityInwardsFormsResources.SurfaceLineIcon,
                FileFilterGenerator = SurfaceLineFileFilter,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                CreateFileImporter = (context, filePath) => new SurfaceLinesCsvImporter<RingtoetsMacroStabilityInwardsSurfaceLine>(
                    context.WrappedData,
                    filePath,
                    new UpdateMessageProvider(),
                    SurfaceLinesCsvImporterConfigurationFactory.CreateUpdateStrategyConfiguration(context.FailureMechanism, context.AssessmentSection.ReferenceLine)),
                VerifyUpdates = context => VerifySurfaceLineUpdates(context, Resources.MacroStabilityInwardsPlugin_VerifySurfaceLineUpdates_When_updating_surfacelines_definitions_assigned_to_calculation_output_will_be_cleared_confirm)
            };

            yield return new UpdateInfo<StochasticSoilModelCollectionContext>
            {
                Name = MacroStabilityInwardsDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = MacroStabilityInwardsFormsResources.SoilProfileIcon,
                FileFilterGenerator = StochasticSoilModelFileFilter,
                IsEnabled = context => context.WrappedData.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                CreateFileImporter = (context, filePath) => StochasticSoilModelImporter(context, filePath, new UpdateMessageProvider(), new StochasticSoilModelUpdateDataStrategy(context.FailureMechanism)),
                VerifyUpdates = context =>
                    VerifyStochasticSoilModelUpdates(
                        context,
                        Resources.MacroStabilityInwardsPlugin_VerifyStochasticSoilModelUpdates_When_updating_StochasticSoilModel_definitions_assigned_to_calculation_output_will_be_cleared_confirm)
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<MacroStabilityInwardsFailureMechanismContext, MacroStabilityInwardsFailureMechanismView>
            {
                GetViewName = (view, mechanism) => mechanism.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return new ViewInfo<
                FailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>,
                IEnumerable<MacroStabilityInwardsFailureMechanismSectionResult>,
                MacroStabilityInwardsFailureMechanismResultView>
            {
                GetViewName = (view, results) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };

            yield return new ViewInfo<MacroStabilityInwardsCalculationGroupContext, CalculationGroup, MacroStabilityInwardsCalculationsView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => calculationGroup.Name,
                Image = RingtoetsCommonFormsResources.GeneralFolderIcon,
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
                GetViewName = (view, input) => RingtoetsCommonFormsResources.Calculation_Input,
                Image = MacroStabilityInwardsFormsResources.MacroStabilityInwardsInputIcon,
                CloseForData = CloseInputViewForData
            };

            yield return new ViewInfo<MacroStabilityInwardsScenariosContext, CalculationGroup, MacroStabilityInwardsScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = RingtoetsCommonFormsResources.ScenariosIcon,
                AdditionalDataCheck = context => context.WrappedData == context.ParentFailureMechanism.CalculationsGroup,
                CloseForData = CloseScenariosViewForData,
                AfterCreate = (view, context) => { view.MacroStabilityInwardsFailureMechanism = context.ParentFailureMechanism; }
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<MacroStabilityInwardsFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<MacroStabilityInwardsCalculationScenarioContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<MacroStabilityInwardsCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsInputContext>
            {
                Text = context => RingtoetsCommonFormsResources.Calculation_Input,
                Image = context => MacroStabilityInwardsFormsResources.MacroStabilityInwardsInputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<RingtoetsMacroStabilityInwardsSurfaceLinesContext>
            {
                Text = context => MacroStabilityInwardsDataResources.MacroStabilityInwardsSurfaceLineCollection_TypeDescriptor,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = context => context.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = RingtoetsMacroStabilityInwardsSurfaceLinesContextContextMenuStrip
            };

            yield return new TreeNodeInfo<RingtoetsMacroStabilityInwardsSurfaceLine>
            {
                Text = surfaceLine => surfaceLine.Name,
                Image = surfaceLine => MacroStabilityInwardsFormsResources.SurfaceLineIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StochasticSoilModelCollectionContext>
            {
                Text = stochasticSoilModelContext => MacroStabilityInwardsDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Image = stochasticSoilModelContext => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = stochasticSoilModelContext => stochasticSoilModelContext.WrappedData.Any() ?
                                                              Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = stochasticSoilModelContext => stochasticSoilModelContext.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = StochasticSoilModelCollectionContextContextMenuStrip
            };

            yield return new TreeNodeInfo<StochasticSoilModel>
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

            yield return new TreeNodeInfo<StochasticSoilProfile>
            {
                Text = stochasticSoilProfile => stochasticSoilProfile.SoilProfile != null ? stochasticSoilProfile.SoilProfile.Name : "Profile",
                Image = stochasticSoilProfile => MacroStabilityInwardsFormsResources.SoilProfileIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsOutputContext>
            {
                Text = context => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<MacroStabilityInwardsScenariosContext>
            {
                Text = context => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyMacroStabilityInwardsOutput>
            {
                Text = output => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = output => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = output => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
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

        private static StochasticSoilModelImporter StochasticSoilModelImporter(StochasticSoilModelCollectionContext context, string filePath,
                                                                               IImporterMessageProvider messageProvider, IStochasticSoilModelUpdateModelStrategy updateStrategy)
        {
            return new StochasticSoilModelImporter(context.WrappedData,
                                                   filePath,
                                                   messageProvider,
                                                   updateStrategy);
        }

        private bool VerifyStochasticSoilModelUpdates(StochasticSoilModelCollectionContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism,
                                                                             query,
                                                                             new DialogBasedInquiryHelper(Gui.MainWindow));
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #region MacroStabilityInwardsFailureMechanismView ViewInfo

        private static bool CloseFailureMechanismViewForData(MacroStabilityInwardsFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as MacroStabilityInwardsFailureMechanism;

            var viewFailureMechanismContext = (MacroStabilityInwardsFailureMechanismContext) view.Data;
            MacroStabilityInwardsFailureMechanism viewMacroStabilityInwardsFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewMacroStabilityInwardsFailureMechanism, failureMechanism);
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
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
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

        private void CalculateAll(MacroStabilityInwardsFailureMechanismContext failureMechanismContext)
        {
            IEnumerable<MacroStabilityInwardsCalculation> calculations = GetAllMacroStabilityInwardsCalculations(failureMechanismContext.WrappedData);
            MacroStabilityInwardsProbabilityAssessmentInput assessmentInput = failureMechanismContext.WrappedData.MacroStabilityInwardsProbabilityAssessmentInput;
            double norm = failureMechanismContext.Parent.FailureMechanismContribution.Norm;
            double contribution = failureMechanismContext.WrappedData.Contribution;

            CalculateAll(calculations, assessmentInput, norm, contribution);
        }

        private void CalculateAll(CalculationGroup group, MacroStabilityInwardsCalculationGroupContext context)
        {
            MacroStabilityInwardsCalculation[] calculations = group.GetCalculations().OfType<MacroStabilityInwardsCalculation>().ToArray();
            MacroStabilityInwardsProbabilityAssessmentInput assessmentInput = context.FailureMechanism.MacroStabilityInwardsProbabilityAssessmentInput;
            double norm = context.AssessmentSection.FailureMechanismContribution.Norm;
            double contribution = context.FailureMechanism.Contribution;

            CalculateAll(calculations, assessmentInput, norm, contribution);
        }

        private static void ValidateAll(IEnumerable<MacroStabilityInwardsCalculation> calculations)
        {
            foreach (MacroStabilityInwardsCalculation calculation in calculations)
            {
                MacroStabilityInwardsCalculationService.Validate(calculation);
            }
        }

        private void CalculateAll(IEnumerable<MacroStabilityInwardsCalculation> calculations, MacroStabilityInwardsProbabilityAssessmentInput assessmentInput,
                                  double norm, double contribution)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                calculations
                    .Select(pc => new MacroStabilityInwardsCalculationActivity(pc,
                                                                               assessmentInput,
                                                                               norm,
                                                                               contribution))
                    .ToList());
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IFailureMechanism failureMechanism)
        {
            if (failureMechanism.Contribution <= 0.0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            return null;
        }

        #region  RingtoetsMacroStabilityInwardsSurfaceLinesContext TreeNodeInfo

        private ContextMenuStrip RingtoetsMacroStabilityInwardsSurfaceLinesContextContextMenuStrip(RingtoetsMacroStabilityInwardsSurfaceLinesContext nodeData, object parentData, TreeViewControl treeViewControl)
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

        private ContextMenuStrip StochasticSoilModelCollectionContextContextMenuStrip(StochasticSoilModelCollectionContext nodeData, object parentData, TreeViewControl treeViewControl)
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
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(macroStabilityInwardsFailureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(macroStabilityInwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              macroStabilityInwardsFailureMechanismContext,
                              ValidateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              macroStabilityInwardsFailureMechanismContext,
                              CalculateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
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

        private static void ValidateAll(MacroStabilityInwardsFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<MacroStabilityInwardsCalculation>());
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(MacroStabilityInwardsFailureMechanismContext macroStabilityInwardsFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(macroStabilityInwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(macroStabilityInwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(MacroStabilityInwardsFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.WrappedData);
        }

        private static IEnumerable<MacroStabilityInwardsCalculation> GetAllMacroStabilityInwardsCalculations(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            return failureMechanism.Calculations.OfType<MacroStabilityInwardsCalculation>();
        }

        private static object[] FailureMechanismEnabledChildNodeObjects(MacroStabilityInwardsFailureMechanismContext macroStabilityInwardsFailureMechanismContext)
        {
            MacroStabilityInwardsFailureMechanism wrappedData = macroStabilityInwardsFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, macroStabilityInwardsFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new MacroStabilityInwardsCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData.SurfaceLines, wrappedData.StochasticSoilModels, wrappedData, macroStabilityInwardsFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(MacroStabilityInwardsFailureMechanismContext macroStabilityInwardsFailureMechanismContext)
        {
            return new object[]
            {
                macroStabilityInwardsFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IList GetInputs(MacroStabilityInwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new RingtoetsMacroStabilityInwardsSurfaceLinesContext(failureMechanism.SurfaceLines, failureMechanism, assessmentSection),
                new StochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IList GetOutputs(MacroStabilityInwardsFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new MacroStabilityInwardsScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new FailureMechanismSectionResultContext<MacroStabilityInwardsFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        #endregion

        #region MacroStabilityInwardsCalculationScenarioContext TreeNodeInfo

        private ContextMenuStrip CalculationContextContextMenuStrip(MacroStabilityInwardsCalculationScenarioContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            MacroStabilityInwardsCalculation calculation = nodeData.WrappedData;

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddRenameItem()
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              nodeData,
                              Validate,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddPerformCalculationItem(
                              calculation,
                              nodeData,
                              PerformCalculation,
                              ValidateAllDataAvailableAndGetErrorMessage)
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

        private static object[] CalculationContextChildNodeObjects(MacroStabilityInwardsCalculationScenarioContext macroStabilityInwardsCalculationScenarioContext)
        {
            MacroStabilityInwardsCalculationScenario macroStabilityInwardsCalculationScenario = macroStabilityInwardsCalculationScenarioContext.WrappedData;

            var childNodes = new List<object>
            {
                macroStabilityInwardsCalculationScenario.Comments,
                new MacroStabilityInwardsInputContext(macroStabilityInwardsCalculationScenario.InputParameters,
                                                      macroStabilityInwardsCalculationScenario,
                                                      macroStabilityInwardsCalculationScenarioContext.AvailableMacroStabilityInwardsSurfaceLines,
                                                      macroStabilityInwardsCalculationScenarioContext.AvailableStochasticSoilModels,
                                                      macroStabilityInwardsCalculationScenarioContext.FailureMechanism,
                                                      macroStabilityInwardsCalculationScenarioContext.AssessmentSection)
            };

            if (macroStabilityInwardsCalculationScenario.HasOutput)
            {
                childNodes.Add(new MacroStabilityInwardsOutputContext(
                                   macroStabilityInwardsCalculationScenario.Output,
                                   macroStabilityInwardsCalculationScenario.SemiProbabilisticOutput));
            }
            else
            {
                childNodes.Add(new EmptyMacroStabilityInwardsOutput());
            }

            return childNodes.ToArray();
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
            MacroStabilityInwardsCalculationService.Validate(context.WrappedData);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(MacroStabilityInwardsCalculationScenarioContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.FailureMechanism);
        }

        private void PerformCalculation(MacroStabilityInwardsCalculation calculation, MacroStabilityInwardsCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             new MacroStabilityInwardsCalculationActivity(calculation,
                                                                                          context.FailureMechanism.MacroStabilityInwardsProbabilityAssessmentInput,
                                                                                          context.AssessmentSection.FailureMechanismContribution.Norm,
                                                                                          context.FailureMechanism.Contribution));
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
                                                                                             nodeData.AvailableMacroStabilityInwardsSurfaceLines,
                                                                                             nodeData.AvailableStochasticSoilModels,
                                                                                             nodeData.FailureMechanism,
                                                                                             nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new MacroStabilityInwardsCalculationGroupContext(group,
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
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
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

            if (!isNestedGroup)
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
                       ValidateAll,
                       ValidateAllDataAvailableAndGetErrorMessage)
                   .AddPerformAllCalculationsInGroupItem(
                       group,
                       nodeData,
                       CalculateAll,
                       ValidateAllDataAvailableAndGetErrorMessage)
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

        private static void ValidateAll(MacroStabilityInwardsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<MacroStabilityInwardsCalculation>());
        }

        private static void AddCalculationScenario(MacroStabilityInwardsCalculationGroupContext nodeData)
        {
            var calculation = new MacroStabilityInwardsCalculationScenario(nodeData.FailureMechanism.GeneralInput)
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children, RingtoetsCommonDataResources.Calculation_DefaultName, c => c.Name)
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
                RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
                calculationGroupGenerateCalculationsToolTip,
                RingtoetsCommonFormsResources.GenerateScenariosIcon, (o, args) => { ShowSurfaceLineSelectionDialog(nodeData); })
            {
                Enabled = surfaceLineAvailable
            };
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(MacroStabilityInwardsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.FailureMechanism);
        }

        private void ShowSurfaceLineSelectionDialog(MacroStabilityInwardsCalculationGroupContext nodeData)
        {
            using (var view = new MacroStabilityInwardsSurfaceLineSelectionDialog(Gui.MainWindow, nodeData.AvailableMacroStabilityInwardsSurfaceLines))
            {
                view.ShowDialog();
                GenerateCalculations(nodeData.WrappedData, view.SelectedItems, nodeData.AvailableStochasticSoilModels, nodeData.FailureMechanism.GeneralInput);
            }
            nodeData.NotifyObservers();
        }

        private static void GenerateCalculations(CalculationGroup target, IEnumerable<RingtoetsMacroStabilityInwardsSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> soilModels, GeneralMacroStabilityInwardsInput generalInput)
        {
            foreach (ICalculationBase group in MacroStabilityInwardsCalculationConfigurationHelper.GenerateCalculationItemsStructure(surfaceLines, soilModels, generalInput))
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

        #region Ringtoets macro stability inwards surface line importer

        private static FileFilterGenerator SurfaceLineFileFilter
        {
            get
            {
                return new FileFilterGenerator(
                    RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                    $"{MacroStabilityInwardsDataResources.MacroStabilityInwardsSurfaceLineCollection_TypeDescriptor} {RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description}");
            }
        }

        private bool VerifySurfaceLineUpdates(RingtoetsMacroStabilityInwardsSurfaceLinesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism,
                                                                             query,
                                                                             new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}