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

using System;
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
using Ringtoets.AssemblyTool.Data;
using Ringtoets.Common.Data.AssemblyTool;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Service;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.IO.Exporters;
using Ringtoets.GrassCoverErosionOutwards.Plugin.Properties;
using Ringtoets.GrassCoverErosionOutwards.Service;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;
using Ringtoets.Revetment.Data;
using Ringtoets.Revetment.IO.Configurations;
using RingtoetsGrassCoverErosionOutwardsFormsResources = Ringtoets.GrassCoverErosionOutwards.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonIoResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin
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
            yield return new PropertyInfo<GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext, GrassCoverErosionOutwardsDesignWaterLevelCalculationsProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsDesignWaterLevelCalculationsProperties(context.WrappedData)
            };
            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveHeightCalculationsContext, GrassCoverErosionOutwardsWaveHeightCalculationsProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsWaveHeightCalculationsProperties(context.WrappedData)
            };

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveConditionsOutput, GrassCoverErosionOutwardsWaveConditionsOutputProperties>();

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveConditionsInputContext, GrassCoverErosionOutwardsWaveConditionsInputContextProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsWaveConditionsInputContextProperties(
                    context,
                    () => GetAssessmentLevel(context.Calculation),
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };

            yield return new PropertyInfo<GrassCoverErosionOutwardsDesignWaterLevelCalculationContext, GrassCoverErosionOutwardsDesignWaterLevelCalculationProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsDesignWaterLevelCalculationProperties(context.WrappedData)
            };

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveHeightCalculationContext, GrassCoverErosionOutwardsWaveHeightCalculationProperties>
            {
                CreateInstance = context => new GrassCoverErosionOutwardsWaveHeightCalculationProperties(context.WrappedData)
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>(
                (context, filePath) =>
                    new WaveConditionsCalculationConfigurationImporter<GrassCoverErosionOutwardsWaveConditionsCalculation>(
                        filePath,
                        context.WrappedData,
                        context.HydraulicBoundaryLocations,
                        context.ForeshoreProfiles));
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<GrassCoverErosionOutwardsFailureMechanismContext, GrassCoverErosionOutwardsFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseGrassCoverErosionOutwardsFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new GrassCoverErosionOutwardsFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>,
                IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>,
                GrassCoverErosionOutwardsFailureMechanismResultView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new GrassCoverErosionOutwardsFailureMechanismResultView(
                    context.WrappedData,
                    (GrassCoverErosionOutwardsFailureMechanism) context.FailureMechanism)
            };

            yield return new ViewInfo<
                GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext,
                IEnumerable<HydraulicBoundaryLocationCalculation>,
                GrassCoverErosionOutwardsDesignWaterLevelCalculationsView>
            {
                GetViewName = (view, context) => RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext_DisplayName,
                GetViewData = context => context.WrappedData,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                CreateInstance = context => new GrassCoverErosionOutwardsDesignWaterLevelCalculationsView(context.WrappedData,
                                                                                                          context.FailureMechanism,
                                                                                                          context.AssessmentSection,
                                                                                                          () => context.AssessmentSection.FailureMechanismContribution.Norm),
                AfterCreate = (view, context) => { view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService; },
                CloseForData = (view, data) => CloseHydraulicBoundaryCalculationsViewForData(view.AssessmentSection, data)
            };

            yield return new ViewInfo<
                GrassCoverErosionOutwardsWaveHeightCalculationsContext,
                IEnumerable<HydraulicBoundaryLocationCalculation>,
                GrassCoverErosionOutwardsWaveHeightCalculationsView>
            {
                GetViewName = (view, context) => RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaveHeightCalculationsContext_DisplayName,
                GetViewData = context => context.WrappedData,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                CreateInstance = context => new GrassCoverErosionOutwardsWaveHeightCalculationsView(context.WrappedData,
                                                                                                    context.FailureMechanism,
                                                                                                    context.AssessmentSection,
                                                                                                    () => context.AssessmentSection.FailureMechanismContribution.Norm),
                AfterCreate = (view, context) => { view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService; },
                CloseForData = (view, data) => CloseHydraulicBoundaryCalculationsViewForData(view.AssessmentSection, data)
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<GrassCoverErosionOutwardsFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>(
                WaveConditionsCalculationGroupChildrenNodeObjects,
                WaveConditionsCalculationGroupContextMenuStrip,
                WaveConditionsCalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>(
                WaveConditionsCalculationContextChildNodeObjects,
                WaveConditionsCalculationContextMenuStrip,
                WaveConditionsCalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<HydraulicBoundariesGroupContext>
            {
                Text = context => RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ChildNodeObjects = GetHydraulicBoundariesGroupContextChildNodeObjects,
                ForeColor = context => context.AssessmentSection.HydraulicBoundaryDatabase.IsLinked()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext>
            {
                Text = context => Resources.GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContextMenuStrip,
                ChildNodeObjects = DesignWaterlevelCalculationsGroupContextChildNodeObjects
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext>
            {
                Text = context => Resources.GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsWaveHeightCalculationsGroupContextMenuStrip,
                ChildNodeObjects = WaveHeightCalculationsGroupContextChildNodeObjects
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext>
            {
                Text = context => context.CategoryBoundaryName,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsDesignWaterLevelCalculationsContextMenuStrip
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveHeightCalculationsContext>
            {
                Text = context => RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaveHeightCalculationsContext_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsWaveHeightCalculationsContextMenuStrip
            };

            yield return new TreeNodeInfo<EmptyGrassCoverErosionOutwardsOutput>
            {
                Text = emptyOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsOutput>
            {
                Text = emptyOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<HydraulicBoundariesGroupContext>
            {
                Name = RingtoetsCommonFormsResources.HydraulicBoundaryLocationsExporter_DisplayName,
                CreateFileExporter = (context, filePath) =>
                    new GrassCoverErosionOutwardsHydraulicBoundaryLocationsExporter(context.FailureMechanism, context.AssessmentSection,
                                                                                    filePath),
                IsEnabled = context => context.WrappedData.Locations.Count > 0,
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonIoResources.Shape_file_filter_Extension,
                                                              RingtoetsCommonIoResources.Shape_file_filter_Description)
            };

            yield return new ExportInfo<HydraulicBoundariesGroupContext>
            {
                Name = RingtoetsCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) =>
                {
                    IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations = context.FailureMechanism.WaveConditionsCalculationGroup.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
                    return new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath);
                },
                IsEnabled = context => context.FailureMechanism.WaveConditionsCalculationGroup.GetCalculations().Any(c => c.HasOutput),
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                              RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return new ExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>
            {
                Name = RingtoetsCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) =>
                {
                    IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations = context.WrappedData.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
                    return new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath);
                },
                IsEnabled = context => context.WrappedData.GetCalculations().Any(c => c.HasOutput),
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                              RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return new ExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>
            {
                Name = RingtoetsCommonFormsResources.WaveConditionsExporter_DisplayName,
                CreateFileExporter = (context, filePath) => new GrassCoverErosionOutwardsWaveConditionsExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                FileFilterGenerator = new FileFilterGenerator(RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Extension,
                                                              RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter_Description)
            };

            yield return RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>(
                (context, filePath) => new GrassCoverErosionOutwardsCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>(
                (context, filePath) => new GrassCoverErosionOutwardsCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
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

        private static RoundedDouble GetAssessmentLevel(ICalculation<WaveConditionsInput> calculation)
        {
            return calculation.InputParameters.HydraulicBoundaryLocation?.DesignWaterLevelCalculation1.Output?.Result ?? RoundedDouble.NaN;
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
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(failureMechanism, failureMechanismContext.Parent), TreeFolderCategory.Input),
                new HydraulicBoundariesGroupContext(failureMechanismContext.Parent.HydraulicBoundaryDatabase, failureMechanism, failureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(failureMechanism), TreeFolderCategory.Output)
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
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(GrassCoverErosionOutwardsFailureMechanismContext grassCoverErosionOutwardsFailureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionOutwardsFailureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionOutwardsFailureMechanismContext, RemoveAllViewsForItem)
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
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionOutwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionOutwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static object[] GetHydraulicBoundariesGroupContextChildNodeObjects(HydraulicBoundariesGroupContext context)
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

        private ContextMenuStrip GrassCoverErosionOutwardsDesignWaterLevelCalculationsContextMenuStrip(GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var designWaterLevelItem = new StrictContextMenuItem(
                RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All,
                RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    IAssessmentSection assessmentSection = nodeData.AssessmentSection;
                    GrassCoverErosionOutwardsFailureMechanism failureMechanism = nodeData.FailureMechanism;

                    double mechanismSpecificNorm = GetFailureMechanismSpecificNorm(assessmentSection, failureMechanism);

                    hydraulicBoundaryLocationCalculationGuiService.CalculateDesignWaterLevels(
                        assessmentSection.HydraulicBoundaryDatabase.FilePath,
                        assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                        nodeData.WrappedData,
                        mechanismSpecificNorm,
                        new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider());
                });

            string validationText = ValidateAllDataAvailableAndGetErrorMessage(nodeData.AssessmentSection, nodeData.FailureMechanism);
            if (!string.IsNullOrEmpty(validationText))
            {
                designWaterLevelItem.Enabled = false;
                designWaterLevelItem.ToolTipText = validationText;
            }

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(designWaterLevelItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region GrassCoverErosionOutwardsWaveHeightCalculationsContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsWaveHeightCalculationsContextMenuStrip(GrassCoverErosionOutwardsWaveHeightCalculationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var waveHeightItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwards_WaveHeight_Calculate_All_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    IAssessmentSection assessmentSection = nodeData.AssessmentSection;
                    GrassCoverErosionOutwardsFailureMechanism failureMechanism = assessmentSection.GetFailureMechanisms().OfType<GrassCoverErosionOutwardsFailureMechanism>().FirstOrDefault();
                    if (failureMechanism == null)
                    {
                        return;
                    }

                    double mechanismSpecificNorm = GetFailureMechanismSpecificNorm(assessmentSection, failureMechanism);

                    hydraulicBoundaryLocationCalculationGuiService.CalculateWaveHeights(
                        assessmentSection.HydraulicBoundaryDatabase.FilePath,
                        assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                        nodeData.WrappedData,
                        mechanismSpecificNorm,
                        new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider());
                });

            string validationText = ValidateAllDataAvailableAndGetErrorMessage(nodeData.AssessmentSection, nodeData.FailureMechanism);
            if (!string.IsNullOrEmpty(validationText))
            {
                waveHeightItem.Enabled = false;
                waveHeightItem.ToolTipText = validationText;
            }

            return Gui.Get(nodeData, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddCustomItem(waveHeightItem)
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
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
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
                                                                                               ? RingtoetsCommonFormsResources.CalculationGroup_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip
                                                                                               : RingtoetsCommonFormsResources.CalculationGroup_No_HRD_To_Generate_ToolTip;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                             grassCoverErosionOutwardsWaveConditionsCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowHydraulicBoundaryLocationSelectionDialog(nodeData))
            {
                Enabled = locationsAvailable
            };
        }

        private void ShowHydraulicBoundaryLocationSelectionDialog(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData)
        {
            using (var dialog = new HydraulicBoundaryLocationSelectionDialog(Gui.MainWindow, nodeData.FailureMechanism.HydraulicBoundaryLocations))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateGrassCoverErosionOutwardsWaveConditionsCalculations(dialog.SelectedItems, nodeData.WrappedData.Children);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateGrassCoverErosionOutwardsWaveConditionsCalculations(IEnumerable<HydraulicBoundaryLocation> hydraulicBoundaryLocations,
                                                                                        List<ICalculationBase> calculationCollection)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculationHelper.AddCalculationsFromLocations(
                hydraulicBoundaryLocations,
                calculationCollection);
        }

        private static void AddWaveConditionsCalculation(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData)
        {
            var calculation = new GrassCoverErosionOutwardsWaveConditionsCalculation
            {
                Name = NamingHelper.GetUniqueName(nodeData.WrappedData.Children,
                                                  RingtoetsCommonDataResources.Calculation_DefaultName,
                                                  c => c.Name)
            };
            nodeData.WrappedData.Children.Add(calculation);
            nodeData.WrappedData.NotifyObservers();
        }

        private static void ValidateAll(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations, IAssessmentSection assessmentSection)
        {
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculations)
            {
                GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation,
                                                                                   GetAssessmentLevel(calculation),
                                                                                   assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                   assessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory());
            }
        }

        private static void ValidateAll(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>(),
                        context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            if (failureMechanism.Contribution <= 0.0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        private void CalculateAll(CalculationGroup group, GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculation[] calculations = group.GetCalculations().OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>().ToArray();

            CalculateAll(calculations, context.FailureMechanism, context.AssessmentSection);
        }

        private void CalculateAll(GrassCoverErosionOutwardsWaveConditionsCalculation[] calculations,
                                  GrassCoverErosionOutwardsFailureMechanism failureMechanism,
                                  IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(
                Gui.MainWindow,
                calculations
                    .Select(calculation => new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                                          assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                          failureMechanism,
                                                                                                          assessmentSection))
                    .ToList());

            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculations)
            {
                calculation.NotifyObservers();
            }
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
                childNodes.Add(calculation.Output);
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
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
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

        private static string ValidateAllDataAvailableAndGetErrorMessage(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static void Validate(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(context.WrappedData,
                                                                               GetAssessmentLevel(context.WrappedData),
                                                                               context.AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                               context.AssessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory());
        }

        private void PerformCalculation(GrassCoverErosionOutwardsWaveConditionsCalculation calculation,
                                        GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             new GrassCoverErosionOutwardsWaveConditionsCalculationActivity(calculation,
                                                                                                            context.AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                            context.FailureMechanism,
                                                                                                            context.AssessmentSection));
            calculation.NotifyObservers();
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

        #region GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContextMenuStrip(
            GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .Build();
        }

        private static object[] DesignWaterlevelCalculationsGroupContextChildNodeObjects(GrassCoverErosionOutwardsDesignWaterLevelCalculationsGroupContext context)
        {
            return new object[]
            {
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.FailureMechanism.WaterLevelCalculationsForMechanismSpecificFactorizedSignalingNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetMechanismSpecificFactorizedSignalingHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_mechanismSpecificFactorizedSignalingNorm_name),
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.FailureMechanism.WaterLevelCalculationsForMechanismSpecificSignalingNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetMechanismSpecificSignalingHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_mechanismSpecificSignalingNorm_name),
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.FailureMechanism.WaterLevelCalculationsForMechanismSpecificLowerLimitNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetMechanismSpecificLowerLimitHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_mechanismSpecificLowerLimitNorm_name),
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.AssessmentSection.WaterLevelCalculationsForLowerLimitNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetLowerLimitHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_lowerLimitNorm_name),
                new GrassCoverErosionOutwardsDesignWaterLevelCalculationsContext(
                    context.AssessmentSection.WaterLevelCalculationsForFactorizedLowerLimitNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetFactorizedLowerLimitHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_factorizedLowerLimitNorm_name)
            };
        }

        #endregion

        #region GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsWaveHeightCalculationsGroupContextMenuStrip(
            GrassCoverErosionOutwardsWaveHeightCalculationsGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
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
                    context.FailureMechanism, context.AssessmentSection, () => GetMechanismSpecificFactorizedSignalingHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_mechanismSpecificFactorizedSignalingNorm_name),
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.FailureMechanism.WaveHeightCalculationsForMechanismSpecificSignalingNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetMechanismSpecificSignalingHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_mechanismSpecificSignalingNorm_name),
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.FailureMechanism.WaveHeightCalculationsForMechanismSpecificLowerLimitNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetMechanismSpecificLowerLimitHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_mechanismSpecificLowerLimitNorm_name),
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.AssessmentSection.WaveHeightCalculationsForLowerLimitNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetLowerLimitHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_lowerLimitNorm_name),
                new GrassCoverErosionOutwardsWaveHeightCalculationsContext(
                    context.AssessmentSection.WaveHeightCalculationsForFactorizedLowerLimitNorm,
                    context.FailureMechanism, context.AssessmentSection, () => GetFactorizedLowerLimitHydraulicBoundaryNorm(
                        context.AssessmentSection,
                        context.FailureMechanism), Resources.Hydraulic_category_boundary_factorizedLowerLimitNorm_name)
            };
        }

        #endregion

        private static double GetFailureMechanismSpecificNorm(IAssessmentSection assessmentSection, GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return RingtoetsCommonDataCalculationService.ProfileSpecificRequiredProbability(
                assessmentSection.FailureMechanismContribution.Norm,
                failureMechanism.Contribution,
                failureMechanism.GeneralInput.N);
        }

        #endregion

        #region Hydraulic boundary norms

        private static double GetMechanismSpecificFactorizedSignalingHydraulicBoundaryNorm(IAssessmentSection assessmentSection,
                                                                                           GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return CreateFailureMechanismAssemblyCategories(assessmentSection, failureMechanism)
                   .Single(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.IIv)
                   .LowerBoundary;
        }

        private static double GetMechanismSpecificSignalingHydraulicBoundaryNorm(IAssessmentSection assessmentSection,
                                                                                 GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return CreateFailureMechanismAssemblyCategories(assessmentSection, failureMechanism)
                   .Single(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.IIIv)
                   .LowerBoundary;
        }

        private static double GetMechanismSpecificLowerLimitHydraulicBoundaryNorm(IAssessmentSection assessmentSection,
                                                                                  GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return CreateFailureMechanismAssemblyCategories(assessmentSection, failureMechanism)
                   .Single(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.IVv)
                   .LowerBoundary;
        }

        private static double GetLowerLimitHydraulicBoundaryNorm(IAssessmentSection assessmentSection,
                                                                 GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return CreateFailureMechanismAssemblyCategories(assessmentSection, failureMechanism)
                   .Single(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.Vv)
                   .LowerBoundary;
        }

        private static double GetFactorizedLowerLimitHydraulicBoundaryNorm(IAssessmentSection assessmentSection,
                                                                           GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return CreateFailureMechanismAssemblyCategories(assessmentSection, failureMechanism)
                   .Single(c => c.Group == FailureMechanismSectionAssemblyCategoryGroup.VIv)
                   .LowerBoundary;
        }

        private static IEnumerable<FailureMechanismSectionAssemblyCategory> CreateFailureMechanismAssemblyCategories(
            IAssessmentSection assessmentSection, GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return AssemblyToolCategoriesFactory.CreateFailureMechanismSectionAssemblyCategories(
                assessmentSection.FailureMechanismContribution.SignalingNorm,
                assessmentSection.FailureMechanismContribution.LowerLimitNorm,
                failureMechanism.Contribution, failureMechanism.GeneralInput.N);
        }

        #endregion
    }
}