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
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.MainWindow;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Helpers;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Riskeer.Common.Data.AssessmentSection;
using Riskeer.Common.Data.Calculation;
using Riskeer.Common.Data.Contribution;
using Riskeer.Common.Data.FailureMechanism;
using Riskeer.Common.Data.Hydraulics;
using Riskeer.Common.Forms;
using Riskeer.Common.Forms.ChangeHandlers;
using Riskeer.Common.Forms.ExportInfos;
using Riskeer.Common.Forms.GuiServices;
using Riskeer.Common.Forms.Helpers;
using Riskeer.Common.Forms.ImportInfos;
using Riskeer.Common.Forms.PresentationObjects;
using Riskeer.Common.Forms.PropertyClasses;
using Riskeer.Common.Forms.TreeNodeInfos;
using Riskeer.Common.Forms.UpdateInfos;
using Riskeer.Common.Forms.Views;
using Riskeer.Common.Plugin;
using Riskeer.Common.Service;
using Riskeer.Common.Util;
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
        private IHydraulicBoundaryLocationCalculationGuiService hydraulicBoundaryLocationCalculationGuiService;

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionOutwardsFailureMechanismContext, GrassCoverErosionOutwardsFailureMechanismProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsFailureMechanismProperties(
                    context.WrappedData,
                    new GrassCoverErosionOutwardsFailureMechanismPropertyChangeHandler())
            };
            yield return new PropertyInfo<GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext, DesignWaterLevelCalculationsProperties>
            {
                CreateInstance = context => new DesignWaterLevelCalculationsProperties(context.WrappedData)
            };
            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveHeightCalculationsContext, WaveHeightCalculationsProperties>
            {
                CreateInstance = context => new WaveHeightCalculationsProperties(context.WrappedData)
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

            yield return new PropertyInfo<GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext, DesignWaterLevelCalculationsGroupProperties>
            {
                CreateInstance = context =>
                {
                    IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary =
                        DesignWaterLevelCalculationsGroupContextChildNodeObjects(context)
                            .Cast<GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext>()
                            .Select(childContext => new Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>(childContext.CategoryBoundaryName,
                                                                                                                         childContext.WrappedData));
                    return new DesignWaterLevelCalculationsGroupProperties(context.WrappedData, calculationsPerCategoryBoundary);
                }
            };

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext, WaveHeightCalculationsGroupProperties>
            {
                CreateInstance = context =>
                {
                    IEnumerable<Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>> calculationsPerCategoryBoundary =
                        WaveHeightCalculationsGroupContextChildNodeObjects(context)
                            .Cast<GrassCoverErosionOutwardsWaveHeightCalculationsContext>()
                            .Select(childContext => new Tuple<string, IEnumerable<HydraulicBoundaryLocationCalculation>>(childContext.CategoryBoundaryName,
                                                                                                                         childContext.WrappedData));
                    return new WaveHeightCalculationsGroupProperties(context.WrappedData, calculationsPerCategoryBoundary);
                }
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
            yield return new ViewInfo<GrassCoverErosionOutwardsFailureMechanismContext, GrassCoverErosionOutwardsFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RiskeerCommonFormsResources.CalculationIcon,
                CloseForData = CloseGrassCoverErosionOutwardsFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new GrassCoverErosionOutwardsFailureMechanismView(context.WrappedData, context.Parent)
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

            yield return new ViewInfo<
                GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext,
                IObservableEnumerable<HydraulicBoundaryLocationCalculation>,
                DesignWaterLevelCalculationsView>
            {
                GetViewName = (view, context) => $"{RiskeerCommonFormsResources.WaterLevelCalculations_DisplayName} " +
                                                 $"- {RiskeerPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName)}",
                GetViewData = context => context.WrappedData,
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                CreateInstance = context => new DesignWaterLevelCalculationsView(context.WrappedData,
                                                                                 context.AssessmentSection,
                                                                                 context.GetNormFunc,
                                                                                 context.CategoryBoundaryName),
                AfterCreate = (view, context) => view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService,
                CloseForData = (view, data) => CloseHydraulicBoundaryCalculationsViewForData(view.AssessmentSection, data)
            };

            yield return new ViewInfo<
                GrassCoverErosionOutwardsWaveHeightCalculationsContext,
                IObservableEnumerable<HydraulicBoundaryLocationCalculation>,
                WaveHeightCalculationsView>
            {
                GetViewName = (view, context) => $"{RiskeerCommonFormsResources.WaveHeightCalculations_DisplayName} " +
                                                 $"- {RiskeerPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName)}",
                GetViewData = context => context.WrappedData,
                Image = RiskeerCommonFormsResources.GenericInputOutputIcon,
                CreateInstance = context => new WaveHeightCalculationsView(context.WrappedData,
                                                                           context.AssessmentSection,
                                                                           context.GetNormFunc,
                                                                           context.CategoryBoundaryName),
                AfterCreate = (view, context) => view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService,
                CloseForData = (view, data) => CloseHydraulicBoundaryCalculationsViewForData(view.AssessmentSection, data)
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
            yield return RiskeerTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<GrassCoverErosionOutwardsFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>(
                WaveConditionsCalculationGroupChildrenNodeObjects,
                WaveConditionsCalculationGroupContextMenuStrip,
                WaveConditionsCalculationGroupContextOnNodeRemoved);

            yield return RiskeerTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>(
                WaveConditionsCalculationContextChildNodeObjects,
                WaveConditionsCalculationContextMenuStrip,
                WaveConditionsCalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>>
            {
                Text = context => RiskeerCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RiskeerCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext>
            {
                Text = context => RiskeerCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ChildNodeObjects = GetHydraulicBoundaryDatabaseContextChildNodeObjects,
                ForeColor = context => context.AssessmentSection.HydraulicBoundaryDatabase.IsLinked()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContextMenuStrip
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext>
            {
                Text = context => RiskeerCommonFormsResources.WaterLevelCalculations_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContextMenuStrip,
                ChildNodeObjects = DesignWaterLevelCalculationsGroupContextChildNodeObjects
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext>
            {
                Text = context => RiskeerCommonFormsResources.WaveHeightCalculations_DisplayName,
                Image = context => RiskeerCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsWaveHeightCalculationsGroupContextMenuStrip,
                ChildNodeObjects = WaveHeightCalculationsGroupContextChildNodeObjects
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext>
            {
                Text = context => RiskeerPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName),
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsDesignWaterLevelCalculationsContextMenuStrip
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveHeightCalculationsContext>
            {
                Text = context => RiskeerPluginHelper.FormatCategoryBoundaryName(context.CategoryBoundaryName),
                Image = context => RiskeerCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsWaveHeightCalculationsContextMenuStrip
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
            yield return new ExportInfo<GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext>
            {
                Name = RiskeerCommonFormsResources.HydraulicBoundaryLocationsExporter_DisplayName,
                CreateFileExporter = (context, filePath) =>
                    new GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(context.FailureMechanism, context.AssessmentSection,
                                                                                    filePath),
                IsEnabled = context => context.WrappedData.Locations.Count > 0,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonIoResources.Shape_file_filter_Extension,
                                                              RiskeerCommonIoResources.Shape_file_filter_Description)
            };

            yield return new ExportInfo<GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext>
            {
                Name = RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) =>
                {
                    IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations = context.FailureMechanism.WaveConditionsCalculationGroup.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
                    return new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath);
                },
                IsEnabled = context => context.FailureMechanism.WaveConditionsCalculationGroup.GetCalculations().Any(c => c.HasOutput),
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                              RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return new ExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>
            {
                Name = RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) =>
                {
                    IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations = context.WrappedData.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
                    return new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath);
                },
                IsEnabled = context => context.WrappedData.GetCalculations().Any(c => c.HasOutput),
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                              RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return new ExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>
            {
                Name = RiskeerCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) => new GrassCoverErosionOutwardsWaveConditionsExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                FileFilterGenerator = new FileFilterGenerator(RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                              RiskeerCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return RiskeerExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>(
                (context, filePath) => new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RiskeerExportInfoFactory.CreateCalculationConfigurationExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>(
                (context, filePath) => new GrassCoverErosionOutwardsWaveConditionsCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return RiskeerUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                GrassCoverErosionOutwardsFailureMechanismSectionsContext, GrassCoverErosionOutwardsFailureMechanism, GrassCoverErosionOutwardsFailureMechanismSectionResult>(
                new GrassCoverErosionOutwardsFailureMechanismSectionResultUpdateStrategy());
        }

        public override void Activate()
        {
            base.Activate();

            if (Gui == null)
            {
                throw new InvalidOperationException("Gui cannot be null");
            }

            hydraulicBoundaryLocationCalculationGuiService = new HydraulicBoundaryLocationCalculationGuiService(Gui.MainWindow);
        }

        #region ViewInfos

        #region GrassCoverErosionOutwardsFailureMechanismView ViewInfo

        private static bool CloseGrassCoverErosionOutwardsFailureMechanismViewForData(GrassCoverErosionOutwardsFailureMechanismView view, object data)
        {
            var assessmentSection = data as IAssessmentSection;
            var failureMechanism = data as GrassCoverErosionOutwardsFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
        }

        #endregion

        #region GrassCoverErosionOutwardsFailureMechanismResultView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(GrassCoverErosionOutwardsFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as GrassCoverErosionOutwardsFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<GrassCoverErosionOutwardsFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        private static bool CloseHydraulicBoundaryCalculationsViewForData(IAssessmentSection viewAssessmentSection,
                                                                          object dataToCloseFor)
        {
            GrassCoverErosionOutwardsFailureMechanism viewFailureMechanism = viewAssessmentSection.GetFailureMechanisms()
                                                                                                  .OfType<GrassCoverErosionOutwardsFailureMechanism>()
                                                                                                  .Single();

            var failureMechanismContext = dataToCloseFor as GrassCoverErosionOutwardsFailureMechanismContext;
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as GrassCoverErosionOutwardsFailureMechanism;

            if (assessmentSection != null)
            {
                failureMechanism = ((IAssessmentSection) dataToCloseFor).GetFailureMechanisms().OfType<GrassCoverErosionOutwardsFailureMechanism>().Single();
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.Parent.GetFailureMechanisms().OfType<GrassCoverErosionOutwardsFailureMechanism>().Single();
            }

            return failureMechanism != null && ReferenceEquals(failureMechanism, viewFailureMechanism);
        }

        #endregion

        #region TreeNodeInfos

        #region GrassCoverErosionOutwardsFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(GrassCoverErosionOutwardsFailureMechanismContext failureMechanismContext)
        {
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = failureMechanismContext.WrappedData;
            IAssessmentSection assessmentSection = failureMechanismContext.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(failureMechanism, assessmentSection), TreeFolderCategory.Input),
                new GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext(assessmentSection.HydraulicBoundaryDatabase, failureMechanism, assessmentSection),
                new CategoryTreeFolder(RiskeerCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(failureMechanism, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(GrassCoverErosionOutwardsFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                failureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new GrassCoverErosionOutwardsFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
                new FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(GrassCoverErosionOutwardsFailureMechanismContext failureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            IAssessmentSection assessmentSection = failureMechanismContext.Parent;
            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        Gui.MainWindow,
                        GrassCoverErosionOutwardsCalculationActivityFactory
                            .CreateCalculationActivities(failureMechanismContext.WrappedData, assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection,
                                                        calculateAllItem);

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCustomItem(calculateAllItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(GrassCoverErosionOutwardsFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(GrassCoverErosionOutwardsFailureMechanismContext grassCoverErosionOutwardsFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RiskeerContextMenuBuilder(Gui.Get(grassCoverErosionOutwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionOutwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private static object[] GetHydraulicBoundaryDatabaseContextChildNodeObjects(GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext context)
        {
            IAssessmentSection assessmentSection = context.AssessmentSection;
            if (assessmentSection.HydraulicBoundaryDatabase.IsLinked())
            {
                ObservableList<HydraulicBoundaryLocation> locations = context.WrappedData.Locations;
                GrassCoverErosionOutwardsFailureMechanism failureMechanism = context.FailureMechanism;
                return new object[]
                {
                    new GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext(locations, failureMechanism, assessmentSection),
                    new GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext(locations, failureMechanism, assessmentSection),
                    new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(failureMechanism.WaveConditionsCalculationGroup,
                                                                                       null,
                                                                                       failureMechanism,
                                                                                       assessmentSection)
                };
            }

            return new object[0];
        }

        #endregion

        #region GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsDesignWaterLevelCalculationsContextMenuStrip(GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext nodeData,
                                                                                                       object parentData,
                                                                                                       TreeViewControl treeViewControl)
        {
            var designWaterLevelItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.DesignWaterLevel_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    IAssessmentSection assessmentSection = nodeData.AssessmentSection;

                    hydraulicBoundaryLocationCalculationGuiService.CalculateDesignWaterLevels(
                        nodeData.WrappedData,
                        assessmentSection,
                        nodeData.GetNormFunc(),
                        nodeData.CategoryBoundaryName);
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection,
                                                        nodeData.GetNormFunc(),
                                                        designWaterLevelItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper,
                RiskeerPluginHelper.FormatCategoryBoundaryName(nodeData.CategoryBoundaryName),
                () => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(nodeData.WrappedData));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddCustomItem(designWaterLevelItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => IllustrationPointsHelper.HasIllustrationPoints(nodeData.WrappedData), changeHandler)
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        #endregion

        #region GrassCoverErosionOutwardsWaveHeightCalculationsContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsWaveHeightCalculationsContextMenuStrip(GrassCoverErosionOutwardsWaveHeightCalculationsContext nodeData,
                                                                                                 object parentData,
                                                                                                 TreeViewControl treeViewControl)
        {
            var waveHeightItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.WaveHeight_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    IAssessmentSection assessmentSection = nodeData.AssessmentSection;

                    hydraulicBoundaryLocationCalculationGuiService.CalculateWaveHeights(
                        nodeData.WrappedData,
                        assessmentSection,
                        nodeData.GetNormFunc(),
                        nodeData.CategoryBoundaryName);
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(nodeData.AssessmentSection,
                                                        nodeData.GetNormFunc(),
                                                        waveHeightItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper,
                RiskeerPluginHelper.FormatCategoryBoundaryName(nodeData.CategoryBoundaryName),
                () => RiskeerCommonDataSynchronizationService.ClearHydraulicBoundaryLocationCalculationIllustrationPoints(nodeData.WrappedData));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddCustomItem(waveHeightItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => IllustrationPointsHelper.HasIllustrationPoints(nodeData.WrappedData), changeHandler)
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
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);
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
                   .AddCreateCalculationItem(nodeData, AddWaveConditionsCalculation)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddUpdateForeshoreProfileOfCalculationsItem(calculations,
                                                                inquiryHelper,
                                                                SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(
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

        private static void ValidateAll(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations,
                                        GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                        IAssessmentSection assessmentSection)
        {
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculations)
            {
                WaveConditionsCalculationServiceBase.Validate(calculation.InputParameters,
                                                              failureMechanism.GetAssessmentLevel(assessmentSection,
                                                                                                  calculation.InputParameters.HydraulicBoundaryLocation,
                                                                                                  calculation.InputParameters.CategoryType),
                                                              assessmentSection.HydraulicBoundaryDatabase,
                                                              failureMechanism.GetNorm(assessmentSection, calculation.InputParameters.CategoryType));
            }
        }

        private static void ValidateAll(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>(),
                        context.FailureMechanism,
                        context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        private void CalculateAll(CalculationGroup group, GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveConditionsCalculationActivities(
                    group, context.FailureMechanism, context.AssessmentSection));
        }

        private static void WaveConditionsCalculationGroupContextOnNodeRemoved(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData,
                                                                               object parentNodeData)
        {
            var parentGroupContext = (GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
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
            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = nodeData.WrappedData;

            return builder
                   .AddExportItem()
                   .AddSeparator()
                   .AddDuplicateCalculationItem(calculation, nodeData)
                   .AddSeparator()
                   .AddRenameItem()
                   .AddUpdateForeshoreProfileOfCalculationItem(calculation,
                                                               inquiryHelper,
                                                               SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
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

        private void PerformCalculation(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                        GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             GrassCoverErosionOutwardsCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                           context.FailureMechanism,
                                                                                                                           context.AssessmentSection));
        }

        private static void WaveConditionsCalculationContextOnNodeRemoved(GrassCoverErosionOutwardsWaveConditionsCalculationContext nodeData,
                                                                          object parentNodeData)
        {
            var calculationGroupContext = parentNodeData as GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext;
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

        #region GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContextMenuStrip(
            GrassCoverErosionOutwardsHydraulicBoundaryDatabaseContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = nodeData.FailureMechanism;

            IMainWindow guiMainWindow = Gui.MainWindow;

            var calculateAllItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.CalculationGroup_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        guiMainWindow,
                        GrassCoverErosionOutwardsCalculationActivityFactory
                            .CreateCalculationActivities(failureMechanism, assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection,
                                                        calculateAllItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(guiMainWindow);
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper,
                RiskeerCommonFormsResources.WaterLevel_and_WaveHeight_DisplayName,
                () => GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelAndWaveHeightCalculations(
                    failureMechanism, assessmentSection));

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddCustomItem(calculateAllItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => HasIllustrationPoints(failureMechanism, assessmentSection), changeHandler)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static bool HasIllustrationPoints(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return DesignWaterLevelCalculationsHaveIllustrationPoints(failureMechanism, assessmentSection)
                   || WaveHeightCalculationsHaveIllustrationPoints(failureMechanism, assessmentSection);
        }

        #endregion

        #region GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContextMenuStrip(
            GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = nodeData.FailureMechanism;

            IMainWindow guiMainWindow = Gui.MainWindow;

            var designWaterLevelItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.DesignWaterLevel_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        guiMainWindow,
                        GrassCoverErosionOutwardsCalculationActivityFactory.CreateDesignWaterLevelCalculationActivities(
                            failureMechanism, assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, designWaterLevelItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(guiMainWindow);
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper,
                RiskeerCommonFormsResources.WaterLevelCalculations_DisplayName,
                () => GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForDesignWaterLevelCalculations(
                    failureMechanism, assessmentSection));

            return builder.AddCustomItem(designWaterLevelItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => DesignWaterLevelCalculationsHaveIllustrationPoints(failureMechanism, assessmentSection),
                                                                        changeHandler)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static object[] DesignWaterLevelCalculationsGroupContextChildNodeObjects(GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext context)
        {
            return new object[]
            {
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.FailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName),
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.FailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.MechanismSpecificSignalingNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName),
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.FailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName),
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.AssessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.LowerLimitNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName),
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.AssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.FactorizedLowerLimitNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName)
            };
        }

        private static bool DesignWaterLevelCalculationsHaveIllustrationPoints(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                               IAssessmentSection assessmentSection)
        {
            return IllustrationPointsHelper.HasIllustrationPoints(failureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(failureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(failureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForLowerLimitNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm);
        }

        #endregion

        #region GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsWaveHeightCalculationsGroupContextMenuStrip(
            GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            IAssessmentSection assessmentSection = nodeData.AssessmentSection;
            GrassCoverErosionOutwardsFailureMechanism failureMechanism = nodeData.FailureMechanism;

            IMainWindow guiMainWindow = Gui.MainWindow;

            var waveHeightItem = new StrictContextMenuItem(
                RiskeerCommonFormsResources.Calculate_All,
                RiskeerCommonFormsResources.WaveHeight_Calculate_All_ToolTip,
                RiskeerCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(
                        guiMainWindow,
                        GrassCoverErosionOutwardsCalculationActivityFactory.CreateWaveHeightCalculationActivities(
                            failureMechanism, assessmentSection));
                });

            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, waveHeightItem);

            var builder = new RiskeerContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(guiMainWindow);
            var changeHandler = new ClearIllustrationPointsOfHydraulicBoundaryLocationCalculationCollectionChangeHandler(
                inquiryHelper,
                RiskeerCommonFormsResources.WaveHeightCalculations_DisplayName,
                () => GrassCoverErosionOutwardsDataSynchronizationService.ClearIllustrationPointResultsForWaveHeightCalculations(
                    failureMechanism, assessmentSection));

            return builder.AddCustomItem(waveHeightItem)
                          .AddSeparator()
                          .AddClearIllustrationPointsOfCalculationsItem(() => WaveHeightCalculationsHaveIllustrationPoints(failureMechanism, assessmentSection),
                                                                        changeHandler)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static object[] WaveHeightCalculationsGroupContextChildNodeObjects(GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext context)
        {
            return new object[]
            {
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.FailureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.MechanismSpecificFactorizedSignalingNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificFactorizedSignalingNorm_DisplayName),
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.FailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.MechanismSpecificSignalingNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificSignalingNorm_DisplayName),
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.FailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.MechanismSpecificLowerLimitNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_MechanismSpecificLowerLimitNorm_DisplayName),
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.AssessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.LowerLimitNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_LowerLimitNorm_DisplayName),
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.AssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                    context.FailureMechanism,
                    context.AssessmentSection,
                    () => context.FailureMechanism.GetNorm(context.AssessmentSection, FailureMechanismCategoryType.FactorizedLowerLimitNorm),
                    RiskeerCommonDataResources.FailureMechanismCategoryType_FactorizedLowerLimitNorm_DisplayName)
            };
        }

        private static bool WaveHeightCalculationsHaveIllustrationPoints(GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                                                         IAssessmentSection assessmentSection)
        {
            return IllustrationPointsHelper.HasIllustrationPoints(failureMechanism.WaveHeightCalculationsForMechanismSpecificFactorizedSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(failureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(failureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaveHeightCalculationsForLowerLimitNorm)
                   || IllustrationPointsHelper.HasIllustrationPoints(assessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm);
        }

        #endregion

        private static void SetHydraulicsMenuItemEnabledStateAndTooltip(IAssessmentSection assessmentSection,
                                                                        StrictContextMenuItem menuItem)
        {
            string validationText = ValidateAllDataAvailableAndGetErrorMessage(assessmentSection);
            if (!string.IsNullOrEmpty(validationText))
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = validationText;
            }
        }

        private static void SetHydraulicsMenuItemEnabledStateAndTooltip(IAssessmentSection assessmentSection,
                                                                        double norm,
                                                                        StrictContextMenuItem menuItem)
        {
            SetHydraulicsMenuItemEnabledStateAndTooltip(assessmentSection, menuItem);
            if (!menuItem.Enabled)
            {
                return;
            }

            TargetProbabilityCalculationServiceHelper.ValidateTargetProbability(norm, logMessage =>
            {
                menuItem.Enabled = false;
                menuItem.ToolTipText = logMessage;
            });
        }

        #endregion
    }
}