﻿// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using System.IO;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.Service;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using log4net;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Piping.Data;
using Ringtoets.Piping.Forms;
using Ringtoets.Piping.Forms.PresentationObjects;
using Ringtoets.Piping.Forms.PropertyClasses;
using Ringtoets.Piping.Forms.Views;
using Ringtoets.Piping.IO.Importer;
using Ringtoets.Piping.Plugin.FileImporter;
using Ringtoets.Piping.Primitives;
using Ringtoets.Piping.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using PipingFormsResources = Ringtoets.Piping.Forms.Properties.Resources;
using PipingPluginResources = Ringtoets.Piping.Plugin.Properties.Resources;

namespace Ringtoets.Piping.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="PipingFailureMechanism"/>.
    /// </summary>
    public class PipingPlugin : PluginBase
    {
        private readonly ILog log = LogManager.GetLogger(typeof(PipingPlugin));

        public override IRibbonCommandHandler RibbonCommandHandler
        {
            get
            {
                return new PipingRibbon();
            }
        }

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<PipingFailureMechanismContext, PipingFailureMechanismContextProperties>
            {
                CreateInstance = context => new PipingFailureMechanismContextProperties(context, new FailureMechanismPropertyChangeHandler<PipingFailureMechanism>())
            };
            yield return new PropertyInfo<PipingInputContext, PipingInputContextProperties>
            {
                CreateInstance = context => new PipingInputContextProperties(context, new CalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>())
            };
            yield return new PropertyInfo<PipingOutputContext, PipingOutputContextProperties>();
            yield return new PropertyInfo<RingtoetsPipingSurfaceLinesContext, RingtoetsPipingSurfaceLineCollectionProperties>
            {
                CreateInstance = context => new RingtoetsPipingSurfaceLineCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<RingtoetsPipingSurfaceLine, RingtoetsPipingSurfaceLineProperties>();
            yield return new PropertyInfo<StochasticSoilModelCollectionContext, StochasticSoilModelCollectionProperties>
            {
                CreateInstance = context => new StochasticSoilModelCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<StochasticSoilModel, StochasticSoilModelProperties>();
            yield return new PropertyInfo<StochasticSoilProfile, StochasticSoilProfileProperties>();
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<RingtoetsPipingSurfaceLinesContext>
            {
                Name = PipingFormsResources.PipingSurfaceLinesCollection_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = PipingFormsResources.PipingSurfaceLineIcon,
                FileFilter = $"{PipingFormsResources.PipingSurfaceLinesCollection_DisplayName} {RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter}",
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                CreateFileImporter = (context, filePath) => new PipingSurfaceLinesCsvImporter(context.WrappedData,
                                                                                              context.AssessmentSection.ReferenceLine,
                                                                                              new RingtoetsPipingSurfaceLineReplaceDataStrategy(),
                                                                                              filePath)
            };

            yield return new ImportInfo<StochasticSoilModelCollectionContext>
            {
                Name = PipingFormsResources.StochasticSoilModelCollection_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = PipingFormsResources.PipingSoilProfileIcon,
                FileFilter = $"{PipingPluginResources.Soil_file_name} (*.soil)|*.soil",
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                CreateFileImporter = (context, filePath) => new StochasticSoilModelImporter(context.WrappedData,
                                                                                            filePath,
                                                                                            new StochasticSoilModelReplaceDataStrategy(),
                                                                                            new StochasticSoilModelChangeHandler(context.FailureMechanism))
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<PipingFailureMechanismContext, PipingFailureMechanismView>
            {
                GetViewName = (view, mechanism) => mechanism.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = ClosePipingFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return new ViewInfo<
                FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>,
                IEnumerable<PipingFailureMechanismSectionResult>,
                PipingFailureMechanismResultView>
            {
                GetViewName = (view, results) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };

            yield return new ViewInfo<PipingCalculationGroupContext, CalculationGroup, PipingCalculationsView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => calculationGroup.Name,
                Image = RingtoetsCommonFormsResources.GeneralFolderIcon,
                AdditionalDataCheck = context => context.WrappedData == context.FailureMechanism.CalculationsGroup,
                CloseForData = ClosePipingCalculationsViewForData,
                AfterCreate = (view, context) =>
                {
                    view.AssessmentSection = context.AssessmentSection;
                    view.PipingFailureMechanism = context.FailureMechanism;
                },
                CreateInstance = () => new PipingCalculationsView(new CalculationInputPropertyChangeHandler<PipingInput, PipingCalculationScenario>())
            };

            yield return new ViewInfo<PipingInputContext, PipingCalculationScenario, PipingInputView>
            {
                GetViewData = context => context.PipingCalculation,
                GetViewName = (view, input) => RingtoetsCommonFormsResources.Calculation_Input,
                Image = PipingFormsResources.PipingInputIcon,
                CloseForData = ClosePipingInputViewForData
            };

            yield return new ViewInfo<PipingScenariosContext, CalculationGroup, PipingScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = RingtoetsCommonFormsResources.ScenariosIcon,
                AdditionalDataCheck = context => context.WrappedData == context.ParentFailureMechanism.CalculationsGroup,
                CloseForData = ClosePipingScenariosViewForData,
                AfterCreate = (view, context) => { view.PipingFailureMechanism = context.ParentFailureMechanism; }
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

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>>
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

            yield return new TreeNodeInfo<RingtoetsPipingSurfaceLinesContext>
            {
                Text = ringtoetsPipingSurfaceLine => PipingFormsResources.PipingSurfaceLinesCollection_DisplayName,
                Image = ringtoetsPipingSurfaceLine => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = ringtoetsPipingSurfaceLine => ringtoetsPipingSurfaceLine.WrappedData.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = ringtoetsPipingSurfaceLine => ringtoetsPipingSurfaceLine.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddSeparator()
                                                                                 .AddDeleteChildrenItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<RingtoetsPipingSurfaceLine>
            {
                Text = pipingSurfaceLine => pipingSurfaceLine.Name,
                Image = pipingSurfaceLine => PipingFormsResources.PipingSurfaceLineIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddDeleteItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build(),
                CanRemove = CanRemoveSurfaceLine,
                OnNodeRemoved = OnSurfaceLineRemoved
            };

            yield return new TreeNodeInfo<StochasticSoilModelCollectionContext>
            {
                Text = stochasticSoilModelContext => PipingFormsResources.StochasticSoilModelCollection_DisplayName,
                Image = stochasticSoilModelContext => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = stochasticSoilModelContext => stochasticSoilModelContext.WrappedData.Any() ?
                                                              Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = stochasticSoilModelContext => stochasticSoilModelContext.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = StochasticSoilModelCollectionContextContextMenuStrip
            };

            yield return new TreeNodeInfo<StochasticSoilModel>
            {
                Text = stochasticSoilModel => stochasticSoilModel.Name,
                Image = stochasticSoilModel => PipingFormsResources.StochasticSoilModelIcon,
                ChildNodeObjects = stochasticSoilModel => stochasticSoilModel.StochasticSoilProfiles.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddDeleteItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build(),
                CanRemove = CanRemoveStochasticSoilModel,
                OnNodeRemoved = OnStochasticSoilModelRemoved
            };

            yield return new TreeNodeInfo<StochasticSoilProfile>
            {
                Text = pipingSoilProfile => pipingSoilProfile.SoilProfile != null ? pipingSoilProfile.SoilProfile.Name : "Profile",
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

        #region PipingFailureMechanismView ViewInfo

        private static bool ClosePipingFailureMechanismViewForData(PipingFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var pipingFailureMechanism = o as PipingFailureMechanism;

            var viewPipingFailureMechanismContext = (PipingFailureMechanismContext) view.Data;
            var viewPipingFailureMechanism = viewPipingFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewPipingFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewPipingFailureMechanism, pipingFailureMechanism);
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
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
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
            var pipingCalculationScenario = o as PipingCalculationScenario;
            if (pipingCalculationScenario != null)
            {
                return ReferenceEquals(view.Data, pipingCalculationScenario);
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

        private void CalculateAll(PipingFailureMechanismContext failureMechanismContext)
        {
            var calculations = GetAllPipingCalculations(failureMechanismContext.WrappedData);
            var assessmentInput = failureMechanismContext.WrappedData.PipingProbabilityAssessmentInput;
            var norm = failureMechanismContext.Parent.FailureMechanismContribution.Norm;
            var contribution = failureMechanismContext.WrappedData.Contribution;

            CalculateAll(calculations, assessmentInput, norm, contribution);
        }

        private void CalculateAll(CalculationGroup group, PipingCalculationGroupContext context)
        {
            var calculations = group.GetCalculations().OfType<PipingCalculation>().ToArray();
            var assessmentInput = context.FailureMechanism.PipingProbabilityAssessmentInput;
            var norm = context.AssessmentSection.FailureMechanismContribution.Norm;
            var contribution = context.FailureMechanism.Contribution;

            CalculateAll(calculations, assessmentInput, norm, contribution);
        }

        private static void ValidateAll(IEnumerable<PipingCalculation> pipingCalculations)
        {
            foreach (PipingCalculation calculation in pipingCalculations)
            {
                PipingCalculationService.Validate(calculation);
            }
        }

        private void CalculateAll(IEnumerable<PipingCalculation> calculations, PipingProbabilityAssessmentInput assessmentInput,
                                  double norm, double contribution)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                calculations
                    .Select(pc => new PipingCalculationActivity(pc,
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

        #region StochasticSoilModelCollectionContext TreeNodeInfo

        private ContextMenuStrip StochasticSoilModelCollectionContextContextMenuStrip(StochasticSoilModelCollectionContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddImportItem()
                      .AddCustomItem(
                          CreateUpdateStochasticSoilModelsItem(
                              nodeData.WrappedData,
                              nodeData.FailureMechanism))
                      .AddSeparator()
                      .AddDeleteChildrenItem()
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .Build();
        }

        private StrictContextMenuItem CreateUpdateStochasticSoilModelsItem(ObservableCollectionWithSourcePath<StochasticSoilModel> soilModelCollection, PipingFailureMechanism failureMechanism)
        {
            var enabled = soilModelCollection.SourcePath != null;

            return new StrictContextMenuItem(
                PipingPluginResources.PipingPlugin_UpdateStochasticSoilModelsMenuItem_Text,
                enabled ? PipingPluginResources.PipingPlugin_UpdateStochasticSoilModelsMenuItem_ToolTip : PipingPluginResources.PipingPlugin_UpdateStochasticSoilModelsMenuItem_ToolTip_No_SourcePath_set,
                PipingPluginResources.RefreshIcon,
                (sender, args) => UpdateStochasticSoilModelFromKnownSourceFile(soilModelCollection, failureMechanism))
            {
                Enabled = enabled
            };
        }

        private void UpdateStochasticSoilModelFromKnownSourceFile(ObservableCollectionWithSourcePath<StochasticSoilModel> soilModelCollection, PipingFailureMechanism failureMechanism)
        {
            string updateFromPath = soilModelCollection.SourcePath;
            if (!File.Exists(updateFromPath))
            {
                updateFromPath = InquireUserForNewPath();
            }

            if (updateFromPath != null)
            {
                RunUpdateStochasticSoilModel(soilModelCollection, failureMechanism, updateFromPath);
            }
            else
            {
                log.Info(string.Format(
                    PipingPluginResources.PipingPlugin_UpdateStochasticSoilModels_Update_of_StochasticSoilModels_from_File_0_canceled_by_user, 
                    soilModelCollection.SourcePath));
            }
        }

        private string InquireUserForNewPath()
        {
            var openFileDialog = new OpenFileDialog
            {
                Multiselect = false,
                Title = PipingPluginResources.PipingPlugin_UpdateStochasticSoilModelsFileDialog_Title,
                Filter = $"{PipingPluginResources.Soil_file_name} (*.soil)|*.soil"
            };
            if (openFileDialog.ShowDialog(Gui.MainWindow) == DialogResult.OK)
            {
                return openFileDialog.FileName;
            }
            return null;
        }

        private void RunUpdateStochasticSoilModel(ObservableCollectionWithSourcePath<StochasticSoilModel> soilModelCollection, PipingFailureMechanism failureMechanism, string sourceFilePath)
        {
            var importer = new StochasticSoilModelImporter(soilModelCollection,
                                                           sourceFilePath,
                                                           new StochasticSoilModelUpdateDataStrategy(failureMechanism),
                                                           new StochasticSoilModelChangeHandler(failureMechanism));

            var activity = new FileImportActivity(importer, PipingPluginResources.PipingPlugin_RunUpdateStochasticSoilModel_Update_StochasticSoilModels);
            ActivityProgressDialogRunner.Run(Gui.MainWindow, activity);
        }

        #endregion

        #region RingtoetsPipingSurfaceLine TreeNodeInfo

        private static bool CanRemoveSurfaceLine(RingtoetsPipingSurfaceLine nodeData, object parentData)
        {
            return parentData is RingtoetsPipingSurfaceLinesContext;
        }

        private static void OnSurfaceLineRemoved(RingtoetsPipingSurfaceLine nodeData, object parentData)
        {
            var context = (RingtoetsPipingSurfaceLinesContext) parentData;
            IObservable[] changedObservables = PipingDataSynchronizationService.RemoveSurfaceLine(context.FailureMechanism, nodeData).ToArray();

            foreach (IObservable observable in changedObservables)
            {
                observable.NotifyObservers();
            }
        }

        #endregion

        #region StochasticSoilModel TreeNodeInfo

        private static bool CanRemoveStochasticSoilModel(StochasticSoilModel nodeData, object parentData)
        {
            return parentData is StochasticSoilModelCollectionContext;
        }

        private static void OnStochasticSoilModelRemoved(StochasticSoilModel nodeData, object parentData)
        {
            var context = (StochasticSoilModelCollectionContext) parentData;
            IObservable[] changedObservables = PipingDataSynchronizationService.RemoveStochasticSoilModel(context.FailureMechanism,
                                                                                                          nodeData).ToArray();

            foreach (IObservable observable in changedObservables)
            {
                observable.NotifyObservers();
            }
        }

        #endregion

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
                              ValidateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              pipingFailureMechanismContext,
                              CalculateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
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
            ValidateAll(context.WrappedData.Calculations.OfType<PipingCalculation>());
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
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(PipingFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.WrappedData);
        }

        private static IEnumerable<PipingCalculation> GetAllPipingCalculations(PipingFailureMechanism failureMechanism)
        {
            return failureMechanism.Calculations.OfType<PipingCalculation>();
        }

        private static object[] FailureMechanismEnabledChildNodeObjects(PipingFailureMechanismContext pipingFailureMechanismContext)
        {
            PipingFailureMechanism wrappedData = pipingFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, pipingFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new PipingCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData.SurfaceLines, wrappedData.StochasticSoilModels, wrappedData, pipingFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(PipingFailureMechanismContext pipingFailureMechanismContext)
        {
            return new object[]
            {
                pipingFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IList GetInputs(PipingFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new RingtoetsPipingSurfaceLinesContext(failureMechanism.SurfaceLines, failureMechanism, assessmentSection),
                new StochasticSoilModelCollectionContext(failureMechanism.StochasticSoilModels, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IList GetOutputs(PipingFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new PipingScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new FailureMechanismSectionResultContext<PipingFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        #endregion

        #region PipingCalculationScenarioContext TreeNodeInfo

        private ContextMenuStrip PipingCalculationContextContextMenuStrip(PipingCalculationScenarioContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));

            PipingCalculation calculation = nodeData.WrappedData;

            return builder.AddRenameItem()
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

        private static object[] PipingCalculationContextChildNodeObjects(PipingCalculationScenarioContext pipingCalculationScenarioContext)
        {
            PipingCalculationScenario pipingCalculationScenario = pipingCalculationScenarioContext.WrappedData;

            var childNodes = new List<object>
            {
                pipingCalculationScenario.Comments,
                new PipingInputContext(pipingCalculationScenario.InputParameters,
                                       pipingCalculationScenario,
                                       pipingCalculationScenarioContext.AvailablePipingSurfaceLines,
                                       pipingCalculationScenarioContext.AvailableStochasticSoilModels,
                                       pipingCalculationScenarioContext.FailureMechanism,
                                       pipingCalculationScenarioContext.AssessmentSection)
            };

            if (pipingCalculationScenario.HasOutput)
            {
                childNodes.Add(new PipingOutputContext(
                                   pipingCalculationScenario.Output,
                                   pipingCalculationScenario.SemiProbabilisticOutput));
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
            PipingCalculationService.Validate(context.WrappedData);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(PipingCalculationScenarioContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.FailureMechanism);
        }

        private void PerformCalculation(PipingCalculation calculation, PipingCalculationScenarioContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             new PipingCalculationActivity(calculation,
                                                                           context.FailureMechanism.PipingProbabilityAssessmentInput,
                                                                           context.AssessmentSection.FailureMechanismContribution.Norm,
                                                                           context.FailureMechanism.Contribution));
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
                                                                              nodeData.AvailablePipingSurfaceLines,
                                                                              nodeData.AvailableStochasticSoilModels,
                                                                              nodeData.FailureMechanism,
                                                                              nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new PipingCalculationGroupContext(group,
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

        private ContextMenuStrip PipingCalculationGroupContextContextMenuStrip(PipingCalculationGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var group = nodeData.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var isNestedGroup = parentData is PipingCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGeneratePipingCalculationsItem(nodeData);

            if (!isNestedGroup)
            {
                builder.AddOpenItem()
                       .AddSeparator()
                       .AddCustomItem(generateCalculationsItem)
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(nodeData, AddCalculationScenario)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
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

        private static void ValidateAll(PipingCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<PipingCalculation>());
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

            var generateCalculationsItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
                pipingCalculationGroupGeneratePipingCalculationsToolTip,
                RingtoetsCommonFormsResources.GenerateScenariosIcon, (o, args) => { ShowSurfaceLineSelectionDialog(nodeData); })
            {
                Enabled = surfaceLineAvailable
            };
            return generateCalculationsItem;
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(PipingCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.FailureMechanism);
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

        private static void GeneratePipingCalculations(CalculationGroup target, IEnumerable<RingtoetsPipingSurfaceLine> surfaceLines, IEnumerable<StochasticSoilModel> soilModels, GeneralPipingInput generalInput)
        {
            foreach (var group in PipingCalculationConfigurationHelper.GenerateCalculationItemsStructure(surfaceLines, soilModels, generalInput))
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

        #endregion
    }
}