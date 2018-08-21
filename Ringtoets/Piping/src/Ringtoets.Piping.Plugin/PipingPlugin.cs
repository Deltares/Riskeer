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
using Core.Common.Base.Data;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Forms.UpdateInfos;
using Ringtoets.Common.Forms.Views;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.SoilProfile;
using Ringtoets.Common.IO.SurfaceLines;
using Ringtoets.Common.Plugin;
using Ringtoets.Common.Service;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Data.SoilProfile;
using Ringtoets.Piping.Forms;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.IO.Configurations;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Plugin.Properties;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;

namespace Ringtoets.Piping.Plugin
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
                CreateInstance = context => new PipingFailureMechanismProperties(context.WrappedData, new FailureMechanismPropertyChangeHandler<PipingFailureMechanism>())
            };
            yield return new PropertyInfo<PipingInputContext, PipingInputContextProperties>
            {
                CreateInstance = context => new PipingInputContextProperties(context,
                                                                             () => GetNormativeAssessmentLevel(context.AssessmentSection, context.PipingCalculation),
                                                                             new ObservablePropertyChangeHandler(context.PipingCalculation, context.WrappedData))
            };
            yield return new PropertyInfo<PipingOutputContext, PipingOutputProperties>
            {
                CreateInstance = context => new PipingOutputProperties(context.WrappedData, context.FailureMechanism, context.AssessmentSection)
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
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<PipingSurfaceLinesContext>
            {
                Name = RingtoetsCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = PipingFormsResources.PipingSurfaceLineIcon,
                FileFilterGenerator = PipingSurfaceLineFileFilter,
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                CreateFileImporter = (context, filePath) => new SurfaceLinesCsvImporter<PipingSurfaceLine>(
                    context.WrappedData,
                    filePath,
                    new ImportMessageProvider(),
                    SurfaceLinesCsvImporterConfigurationFactory.CreateReplaceStrategyConfiguration(context.FailureMechanism, context.AssessmentSection.ReferenceLine)),
                VerifyUpdates = context => VerifyPipingSurfaceLineUpdates(context, Resources.PipingPlugin_VerifyPipingSurfaceLineImport_When_importing_surface_lines_calculation_output_will_be_cleared_confirm)
            };

            yield return new ImportInfo<PipingStochasticSoilModelCollectionContext>
            {
                Name = RingtoetsCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = PipingFormsResources.PipingSoilProfileIcon,
                FileFilterGenerator = StochasticSoilModelFileFilter,
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                CreateFileImporter = (context, filePath) => new StochasticSoilModelImporter<PipingStochasticSoilModel>(
                    context.WrappedData,
                    filePath,
                    new ImportMessageProvider(),
                    PipingStochasticSoilModelImporterConfigurationFactory.CreateReplaceStrategyConfiguration(context.FailureMechanism)
                ),
                VerifyUpdates = context => VerifyStochasticSoilModelUpdates(context, Resources.PipingPlugin_VerifyStochasticSoilModelImport_When_importing_StochasticSoilModels_calculation_output_will_be_cleared_confirm)
            };

            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<PipingCalculationGroupContext>(
                (context, filePath) =>
                    new PipingCalculationConfigurationImporter(
                        filePath,
                        context.WrappedData,
                        context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                        context.FailureMechanism));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<PipingCalculationGroupContext>(
                (context, filePath) => new PipingCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<PipingCalculationScenarioContext>(
                (context, filePath) => new PipingCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<PipingSurfaceLinesContext>
            {
                Name = RingtoetsCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
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
                Name = RingtoetsCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
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

            yield return RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                PipingFailureMechanismSectionsContext, PipingFailureMechanism, PipingFailureMechanismSectionResult>(
                new PipingFailureMechanismSectionResultUpdateStrategy());
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<PipingFailureMechanismContext, PipingFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = ClosePipingFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new PipingFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>,
                IObservableEnumerable<PipingFailureMechanismSectionResult>,
                PipingFailureMechanismResultView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
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
                Image = RingtoetsCommonFormsResources.GeneralFolderIcon,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CloseForData = ClosePipingCalculationsViewForData,
                AfterCreate = (view, context) =>
                {
                    view.AssessmentSection = context.AssessmentSection;
                    view.PipingFailureMechanism = context.FailureMechanism;
                }
            };

            yield return new ViewInfo<PipingInputContext, PipingCalculationScenario, PipingInputView>
            {
                GetViewData = context => context.PipingCalculation,
                GetViewName = (view, context) => RingtoetsCommonFormsResources.Calculation_Input,
                Image = PipingFormsResources.PipingInputIcon,
                CloseForData = ClosePipingInputViewForData
            };

            yield return new ViewInfo<PipingScenariosContext, CalculationGroup, PipingScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = RingtoetsCommonFormsResources.ScenariosIcon,
                CloseForData = ClosePipingScenariosViewForData,
                CreateInstance = context => new PipingScenariosView(context.AssessmentSection),
                AfterCreate = (view, context) => { view.PipingFailureMechanism = context.FailureMechanism; }
            };

            yield return new ViewInfo<PipingFailureMechanismSectionsContext, IEnumerable<FailureMechanismSection>, FailureMechanismSectionsProbabilityAssessmentView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanismSections_DisplayName,
                Image = RingtoetsCommonFormsResources.SectionsIcon,
                CloseForData = RingtoetsPluginHelper.ShouldCloseFailureMechanismSectionsView,
                CreateInstance = context => new FailureMechanismSectionsProbabilityAssessmentView(context.WrappedData.Sections,
                                                                                                  context.WrappedData,
                                                                                                  ((PipingFailureMechanism) context.WrappedData).PipingProbabilityAssessmentInput),
                GetViewData = context => context.WrappedData.Sections
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<PipingFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<PipingCalculationScenarioContext>(
                PipingCalculationContextChildNodeObjects,
                PipingCalculationContextContextMenuStrip,
                PipingCalculationContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<PipingCalculationGroupContext>(
                PipingCalculationGroupContextChildNodeObjects,
                PipingCalculationGroupContextContextMenuStrip,
                PipingCalculationGroupContextOnNodeRemoved);

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingInputContext>
            {
                Text = pipingInputContext => RingtoetsCommonFormsResources.Calculation_Input,
                Image = pipingInputContext => PipingFormsResources.PipingInputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingSurfaceLinesContext>
            {
                Text = pipingSurfaceLine => RingtoetsCommonDataResources.SurfaceLineCollection_TypeDescriptor,
                Image = pipingSurfaceLine => RingtoetsCommonFormsResources.GeneralFolderIcon,
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
                Text = stochasticSoilModelContext => RingtoetsCommonDataResources.StochasticSoilModelCollection_TypeDescriptor,
                Image = stochasticSoilModelContext => RingtoetsCommonFormsResources.GeneralFolderIcon,
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

            yield return new TreeNodeInfo<PipingOutputContext>
            {
                Text = pipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = pipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<PipingScenariosContext>
            {
                Text = context => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<EmptyPipingOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
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

        private bool VerifyStochasticSoilModelUpdates(PipingStochasticSoilModelCollectionContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism,
                                                                             query,
                                                                             new DialogBasedInquiryHelper(Gui.MainWindow));
            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #region PipingFailureMechanismView ViewInfo

        private static bool ClosePipingFailureMechanismViewForData(PipingFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var pipingFailureMechanism = o as PipingFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, pipingFailureMechanism);
        }

        #endregion

        #region FailureMechanismResultsView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(PipingFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as PipingFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<PipingFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<PipingFailureMechanism>()
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #region PipingCalculationsView ViewInfo

        private static bool ClosePipingCalculationsViewForData(PipingCalculationsView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var pipingFailureMechanism = o as PipingFailureMechanism;
            var pipingFailureMechanismContext = o as PipingFailureMechanismContext;

            if (pipingFailureMechanismContext != null)
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

        #endregion endregion

        #region PipingScenariosView ViewInfo

        private static bool ClosePipingScenariosViewForData(PipingScenariosView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var pipingFailureMechanism = o as PipingFailureMechanism;
            var pipingFailureMechanismContext = o as PipingFailureMechanismContext;

            if (pipingFailureMechanismContext != null)
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

        #endregion endregion

        #region PipingInputView ViewInfo

        private static bool ClosePipingInputViewForData(PipingInputView view, object o)
        {
            var pipingCalculationScenarioContext = o as PipingCalculationScenarioContext;
            if (pipingCalculationScenarioContext != null)
            {
                return ReferenceEquals(view.Data, pipingCalculationScenarioContext.WrappedData);
            }

            IEnumerable<PipingCalculationScenario> calculations = null;

            var pipingCalculationGroupContext = o as PipingCalculationGroupContext;
            if (pipingCalculationGroupContext != null)
            {
                calculations = pipingCalculationGroupContext.WrappedData.GetCalculations()
                                                            .OfType<PipingCalculationScenario>();
            }

            var failureMechanism = o as PipingFailureMechanism;

            var failureMechanismContext = o as PipingFailureMechanismContext;
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            var assessmentSection = o as IAssessmentSection;
            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<PipingFailureMechanism>()
                                                    .FirstOrDefault();
            }

            if (failureMechanism != null)
            {
                calculations = failureMechanism.CalculationsGroup.GetCalculations()
                                               .OfType<PipingCalculationScenario>();
            }

            return calculations != null && calculations.Any(ci => ReferenceEquals(view.Data, ci));
        }

        #endregion

        private static void ValidateAll(IEnumerable<PipingCalculation> pipingCalculations, IAssessmentSection assessmentSection)
        {
            foreach (PipingCalculation calculation in pipingCalculations)
            {
                PipingCalculationService.Validate(calculation, GetNormativeAssessmentLevel(assessmentSection, calculation));
            }
        }

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

        private static RoundedDouble GetNormativeAssessmentLevel(IAssessmentSection assessmentSection, PipingCalculation calculation)
        {
            return assessmentSection.GetNormativeAssessmentLevel(calculation.InputParameters.HydraulicBoundaryLocation);
        }

        #region PipingFailureMechanismContext TreeNodeInfo

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(PipingFailureMechanismContext pipingFailureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(pipingFailureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(pipingFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              pipingFailureMechanismContext,
                              ValidateAll)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              pipingFailureMechanismContext,
                              CalculateAll)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(pipingFailureMechanismContext.WrappedData)
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

        private static void ValidateAll(PipingFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<PipingCalculation>(), context.Parent);
        }

        private void CalculateAll(PipingFailureMechanismContext failureMechanismContext)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow, PipingCalculationActivityFactory.CreateCalculationActivities(failureMechanismContext.WrappedData, failureMechanismContext.Parent));
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(PipingFailureMechanismContext pipingFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(pipingFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(pipingFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static object[] FailureMechanismEnabledChildNodeObjects(PipingFailureMechanismContext context)
        {
            PipingFailureMechanism wrappedData = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, assessmentSection), TreeFolderCategory.Input),
                new PipingCalculationGroupContext(wrappedData.CalculationsGroup, null, wrappedData.SurfaceLines, wrappedData.StochasticSoilModels, wrappedData, assessmentSection),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, assessmentSection), TreeFolderCategory.Output)
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
                                                              () => probabilityAssessmentInput.GetN(probabilityAssessmentInput.SectionLength)),
                new PipingScenariosContext(failureMechanism.CalculationsGroup, failureMechanism, assessmentSection),
                new ProbabilityFailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism, assessmentSection),
                failureMechanism.OutputComments
            };
        }

        #endregion

        #region PipingCalculationScenarioContext TreeNodeInfo

        private ContextMenuStrip PipingCalculationContextContextMenuStrip(PipingCalculationScenarioContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            PipingCalculation calculation = nodeData.WrappedData;

            StrictContextMenuItem updateEntryAndExitPoint = CreateUpdateEntryAndExitPointItem(nodeData);

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, nodeData)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddCustomItem(updateEntryAndExitPoint)
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

        private static object[] PipingCalculationContextChildNodeObjects(PipingCalculationScenarioContext context)
        {
            PipingCalculationScenario pipingCalculationScenario = context.WrappedData;

            var childNodes = new List<object>
            {
                pipingCalculationScenario.Comments,
                new PipingInputContext(pipingCalculationScenario.InputParameters,
                                       pipingCalculationScenario,
                                       context.AvailablePipingSurfaceLines,
                                       context.AvailableStochasticSoilModels,
                                       context.FailureMechanism,
                                       context.AssessmentSection)
            };

            if (pipingCalculationScenario.HasOutput)
            {
                childNodes.Add(new PipingOutputContext(
                                   pipingCalculationScenario.Output,
                                   context.FailureMechanism,
                                   context.AssessmentSection));
            }
            else
            {
                childNodes.Add(new EmptyPipingOutput());
            }

            return childNodes.ToArray();
        }

        private static void PipingCalculationContextOnNodeRemoved(PipingCalculationScenarioContext pipingCalculationScenarioContext, object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as PipingCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                bool successfullyRemovedData = calculationGroupContext.WrappedData.Children.Remove(pipingCalculationScenarioContext.WrappedData);
                if (successfullyRemovedData)
                {
                    calculationGroupContext.NotifyObservers();
                }
            }
        }

        private static void Validate(PipingCalculationScenarioContext context)
        {
            PipingCalculationService.Validate(context.WrappedData, GetNormativeAssessmentLevel(context.AssessmentSection, context.WrappedData));
        }

        private void PerformCalculation(PipingCalculation calculation, PipingCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             PipingCalculationActivityFactory.CreateCalculationActivity(calculation, context.AssessmentSection));
        }

        private StrictContextMenuItem CreateUpdateEntryAndExitPointItem(PipingCalculationScenarioContext context)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_calculation_with_characteristic_points_ToolTip;
            if (context.WrappedData.InputParameters.SurfaceLine == null)
            {
                contextMenuEnabled = false;
                toolTipMessage = Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_calculation_no_surface_line_ToolTip;
            }
            else if (context.WrappedData.InputParameters.IsEntryAndExitPointInputSynchronized)
            {
                contextMenuEnabled = false;
                toolTipMessage = RingtoetsCommonFormsResources.CalculationItem_No_changes_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_entry_and_exit_point,
                toolTipMessage,
                RingtoetsCommonFormsResources.UpdateItemIcon,
                (o, args) => UpdatedSurfaceLineDependentDataOfCalculation(context.WrappedData))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdatedSurfaceLineDependentDataOfCalculation(PipingCalculation scenario)
        {
            string message = RingtoetsCommonFormsResources.VerifyUpdate_Confirm_calculation_output_cleared;
            if (VerifyEntryAndExitPointUpdates(new[]
            {
                scenario
            }, message))
            {
                UpdateSurfaceLineDependentData(scenario);
            }
        }

        private static void UpdateSurfaceLineDependentData(PipingCalculation scenario)
        {
            scenario.InputParameters.SynchronizeEntryAndExitPointInput();

            var affectedObjects = new List<IObservable>
            {
                scenario.InputParameters
            };

            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(scenario));

            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        #endregion

        #region PipingCalculationGroupContext TreeNodeInfo

        private static object[] PipingCalculationGroupContextChildNodeObjects(PipingCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as PipingCalculationScenario;
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new PipingCalculationScenarioContext(calculation,
                                                                              nodeData.WrappedData,
                                                                              nodeData.AvailablePipingSurfaceLines,
                                                                              nodeData.AvailableStochasticSoilModels,
                                                                              nodeData.FailureMechanism,
                                                                              nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new PipingCalculationGroupContext(group,
                                                                           nodeData.WrappedData,
                                                                           nodeData.AvailablePipingSurfaceLines,
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

        private ContextMenuStrip PipingCalculationGroupContextContextMenuStrip(PipingCalculationGroupContext nodeData,
                                                                               object parentData,
                                                                               TreeViewControl treeViewControl)
        {
            CalculationGroup group = nodeData.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            bool isNestedGroup = parentData is PipingCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGeneratePipingCalculationsItem(nodeData);

            PipingCalculationScenario[] calculations = nodeData.WrappedData.GetCalculations()
                                                               .OfType<PipingCalculationScenario>()
                                                               .ToArray();
            StrictContextMenuItem updateEntryAndExitPointsItem = CreateUpdateEntryAndExitPointItem(calculations);

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
                builder.AddRenameItem();
            }

            builder.AddCustomItem(updateEntryAndExitPointsItem)
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(
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

        private void CalculateAll(CalculationGroup group, PipingCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow, PipingCalculationActivityFactory.CreateCalculationActivities(group, context.AssessmentSection));
        }

        private static void ValidateAll(PipingCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<PipingCalculation>(), context.AssessmentSection);
        }

        private static void AddCalculationScenario(PipingCalculationGroupContext nodeData)
        {
            var calculation = new PipingCalculationScenario(nodeData.FailureMechanism.GeneralInput)
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children, RingtoetsCommonDataResources.Calculation_DefaultName, c => c.Name)
            };

            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private StrictContextMenuItem CreateGeneratePipingCalculationsItem(PipingCalculationGroupContext nodeData)
        {
            bool surfaceLineAvailable = nodeData.AvailablePipingSurfaceLines.Any() && nodeData.AvailableStochasticSoilModels.Any();

            string pipingCalculationGroupGeneratePipingCalculationsToolTip = surfaceLineAvailable
                                                                                 ? PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_ToolTip
                                                                                 : PipingFormsResources.PipingCalculationGroup_Generate_PipingCalculations_NoSurfaceLinesOrSoilModels_ToolTip;

            return new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
                pipingCalculationGroupGeneratePipingCalculationsToolTip,
                RingtoetsCommonFormsResources.GenerateScenariosIcon, (o, args) => ShowSurfaceLineSelectionDialog(nodeData))
            {
                Enabled = surfaceLineAvailable
            };
        }

        private void ShowSurfaceLineSelectionDialog(PipingCalculationGroupContext nodeData)
        {
            using (var view = new PipingSurfaceLineSelectionDialog(Gui.MainWindow, nodeData.AvailablePipingSurfaceLines))
            {
                view.ShowDialog();
                GeneratePipingCalculations(nodeData.WrappedData, view.SelectedItems, nodeData.AvailableStochasticSoilModels, nodeData.FailureMechanism.GeneralInput);
            }

            nodeData.NotifyObservers();
        }

        private static void GeneratePipingCalculations(CalculationGroup target,
                                                       IEnumerable<PipingSurfaceLine> surfaceLines,
                                                       IEnumerable<PipingStochasticSoilModel> soilModels,
                                                       GeneralPipingInput generalInput)
        {
            foreach (ICalculationBase group in PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(surfaceLines, soilModels, generalInput))
            {
                target.Children.Add(group);
            }
        }

        private static void PipingCalculationGroupContextOnNodeRemoved(PipingCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (PipingCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        private StrictContextMenuItem CreateUpdateEntryAndExitPointItem(IEnumerable<PipingCalculationScenario> calculations)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_all_calculations_with_surface_line_ToolTip;

            PipingCalculationScenario[] calculationsToUpdate = calculations
                                                               .Where(calc => calc.InputParameters.SurfaceLine != null
                                                                              && !calc.InputParameters.IsEntryAndExitPointInputSynchronized)
                                                               .ToArray();

            if (!calculationsToUpdate.Any())
            {
                contextMenuEnabled = false;
                toolTipMessage = RingtoetsCommonFormsResources.CreateUpdateContextMenuItem_No_calculations_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                Resources.PipingPlugin_CreateUpdateEntryAndExitPointItem_Update_all_entry_and_exit_points,
                toolTipMessage,
                RingtoetsCommonFormsResources.UpdateItemIcon,
                (sender, args) => UpdateEntryAndExitPointsOfAllCalculations(calculationsToUpdate))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateEntryAndExitPointsOfAllCalculations(IEnumerable<PipingCalculationScenario> calculations)
        {
            string message = RingtoetsCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
            if (VerifyEntryAndExitPointUpdates(calculations, message))
            {
                foreach (PipingCalculationScenario calculation in calculations)
                {
                    UpdateSurfaceLineDependentData(calculation);
                }
            }
        }

        private bool VerifyEntryAndExitPointUpdates(IEnumerable<PipingCalculation> calculations, string query)
        {
            var changeHandler = new CalculationChangeHandler(calculations, query, new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion

        #region Ringtoets piping surface line importer

        private static FileFilterGenerator PipingSurfaceLineFileFilter
        {
            get
            {
                return new FileFilterGenerator(
                    RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                    $"{RingtoetsCommonDataResources.SurfaceLineCollection_TypeDescriptor} {RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description}");
            }
        }

        private bool VerifyPipingSurfaceLineUpdates(PipingSurfaceLinesContext context, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(context.FailureMechanism,
                                                                             query,
                                                                             new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion
    }
}