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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using log4net;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.GuiServices;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.IO;
using Ringtoets.Common.IO.HydraRing;
using Ringtoets.GrassCoverErosionOutwards.Data;
using Ringtoets.GrassCoverErosionOutwards.Forms;
using Ringtoets.GrassCoverErosionOutwards.Forms.PresentationObjects;
using Ringtoets.GrassCoverErosionOutwards.Forms.PropertyClasses;
using Ringtoets.GrassCoverErosionOutwards.Forms.Views;
using Ringtoets.GrassCoverErosionOutwards.IO;
using Ringtoets.GrassCoverErosionOutwards.Plugin.Properties;
using Ringtoets.GrassCoverErosionOutwards.Service;
using Ringtoets.GrassCoverErosionOutwards.Service.MessageProviders;
using Ringtoets.HydraRing.Data;
using RingtoetsGrassCoverErosionOutwardsFormsResources = Ringtoets.GrassCoverErosionOutwards.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonIoResources = Ringtoets.Common.IO.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsGrassCoverErosionOutwardsServiceResources = Ringtoets.GrassCoverErosionOutwards.Service.Properties.Resources;

namespace Ringtoets.GrassCoverErosionOutwards.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="GrassCoverErosionOutwardsFailureMechanism"/>.
    /// </summary>
    public class GrassCoverErosionOutwardsPlugin : PluginBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(GrassCoverErosionOutwardsPlugin));
        private IHydraulicBoundaryLocationCalculationGuiService hydraulicBoundaryLocationCalculationGuiService;

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<GrassCoverErosionOutwardsFailureMechanismContext, GrassCoverErosionOutwardsFailureMechanismProperties>
            {
                GetObjectPropertiesData = context => context.WrappedData
            };
            yield return new PropertyInfo<GrassCoverErosionOutwardsDesignWaterLevelLocationsContext, GrassCoverErosionOutwardsDesignWaterLevelLocationsContextProperties>
            {
                GetObjectPropertiesData = context => context.WrappedData
            };
            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveHeightLocationsContext, GrassCoverErosionOutwardsWaveHeightLocationsContextProperties>
            {
                GetObjectPropertiesData = context => context.WrappedData
            };

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveConditionsOutput, GrassCoverErosionOutwardsWaveConditionsOutputProperties>();

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveConditionsInputContext, GrassCoverErosionOutwardsWaveConditionsInputContextProperties>();

            yield return new PropertyInfo<GrassCoverErosionOutwardsDesignWaterLevelLocationContext, GrassCoverErosionOutwardsDesignWaterLevelLocationContextProperties>();

            yield return new PropertyInfo<GrassCoverErosionOutwardsWaveHeightLocationContext, GrassCoverErosionOutwardsWaveHeightLocationContextProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>,
                IEnumerable<GrassCoverErosionOutwardsFailureMechanismSectionResult>,
                GrassCoverErosionOutwardsFailureMechanismResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };

            yield return new ViewInfo<
                GrassCoverErosionOutwardsDesignWaterLevelLocationsContext,
                IEnumerable<HydraulicBoundaryLocation>,
                GrassCoverErosionOutwardsDesignWaterLevelLocationsView>
            {
                GetViewName = (v, o) => RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaterLevelLocations_DisplayName,
                GetViewData = context => context.WrappedData,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                AfterCreate = (view, context) =>
                {
                    view.AssessmentSection = context.AssessmentSection;
                    view.FailureMechanism = context.FailureMechanism;
                    view.ApplicationSelection = Gui;
                    view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService;
                },
                CloseForData = CloseDesignWaterLevelLocationsViewForData
            };

            yield return new ViewInfo<
                GrassCoverErosionOutwardsWaveHeightLocationsContext,
                IEnumerable<HydraulicBoundaryLocation>,
                GrassCoverErosionOutwardsWaveHeightLocationsView>
            {
                GetViewName = (v, o) => RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaveHeightLocationsContext_DisplayName,
                GetViewData = context => context.WrappedData,
                CloseForData = CloseGrassCoverErosionOutwardsLocationsViewForData,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                AfterCreate = (view, context) =>
                {
                    view.AssessmentSection = context.AssessmentSection;
                    view.FailureMechanism = context.FailureMechanism;
                    view.ApplicationSelection = Gui;
                    view.CalculationGuiService = hydraulicBoundaryLocationCalculationGuiService;
                }
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
                ChildNodeObjects = nodeData => GetHydraulicBoundariesGroupContextChildNodeObjects(nodeData),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddExportItem()
                                                                                 .AddSeparator()
                                                                                 .AddExpandAllItem()
                                                                                 .AddCollapseAllItem()
                                                                                 .Build(),
                ForeColor = context => context.AssessmentSection.HydraulicBoundaryDatabase == null ?
                                           Color.FromKnownColor(KnownColor.GrayText) :
                                           Color.FromKnownColor(KnownColor.ControlText)
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsDesignWaterLevelLocationsContext>
            {
                Text = context => RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaterLevelLocations_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsDesignWaterLevelLocationsContextMenuStrip
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveHeightLocationsContext>
            {
                Text = context => RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaveHeightLocationsContext_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = GrassCoverErosionOutwardsWaveHeightLocationsContextMenuStrip
            };

            yield return new TreeNodeInfo<EmptyGrassCoverErosionOutwardsOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ForeColor = emptyPipingOutput => Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsOutput>
            {
                Text = emptyPipingOutput => RingtoetsCommonFormsResources.CalculationOutput_DisplayName,
                Image = emptyPipingOutput => RingtoetsCommonFormsResources.GeneralOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<GrassCoverErosionOutwardsWaveConditionsInputContext>
            {
                Text = context => RingtoetsCommonFormsResources.Calculation_Input,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
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
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.PointShapefileIcon,
                CreateFileExporter = (context, filePath) =>
                                     new HydraulicBoundaryLocationsExporter(context.WrappedData,
                                                                            filePath, Resources.DesignWaterLevel_Description, Resources.WaveHeight_Description),
                IsEnabled = context => context.WrappedData.Count > 0,
                FileFilter = RingtoetsCommonIoResources.DataTypeDisplayName_shape_file_filter
            };

            yield return new ExportInfo<HydraulicBoundariesGroupContext>
            {
                Name = RingtoetsCommonFormsResources.WaveConditionsExporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.GeneralOutputIcon,
                CreateFileExporter = (context, filePath) =>
                {
                    var calculations = context.FailureMechanism.WaveConditionsCalculationGroup.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
                    return new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath);
                },
                IsEnabled = context => context.FailureMechanism.WaveConditionsCalculationGroup.GetCalculations().Any(c => c.HasOutput),
                FileFilter = RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter
            };

            yield return new ExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext>
            {
                CreateFileExporter = (context, filePath) =>
                {
                    var calculations = context.WrappedData.GetCalculations().Cast<GrassCoverErosionOutwardsWaveConditionsCalculation>();
                    return new GrassCoverErosionOutwardsWaveConditionsExporter(calculations, filePath);
                },
                IsEnabled = context => context.WrappedData.GetCalculations().Any(c => c.HasOutput),
                FileFilter = RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter
            };

            yield return new ExportInfo<GrassCoverErosionOutwardsWaveConditionsCalculationContext>
            {
                CreateFileExporter = (context, filePath) => new GrassCoverErosionOutwardsWaveConditionsExporter(new[]
                {
                    context.WrappedData
                }, filePath),
                IsEnabled = context => context.WrappedData.HasOutput,
                FileFilter = RingtoetsCommonFormsResources.DataTypeDisplayName_csv_file_filter
            };
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
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #region GrassCoverErosionOutwardsWaveHeightLocationsView

        private static bool CloseGrassCoverErosionOutwardsLocationsViewForData(GrassCoverErosionOutwardsWaveHeightLocationsView view, object dataToCloseFor)
        {
            return CloseHydraulicBoundaryLocationsViewForData(view.AssessmentSection, dataToCloseFor);
        }

        #endregion

        #region CloseDesignWaterLevelLocationsViewForData

        private static bool CloseDesignWaterLevelLocationsViewForData(GrassCoverErosionOutwardsDesignWaterLevelLocationsView view, object dataToCloseFor)
        {
            return CloseHydraulicBoundaryLocationsViewForData(view.AssessmentSection, dataToCloseFor);
        }

        #endregion

        private static bool CloseHydraulicBoundaryLocationsViewForData(IAssessmentSection viewAssessmentSection,
                                                                       object dataToCloseFor)
        {
            GrassCoverErosionOutwardsFailureMechanism viewFailureMechanism = null;
            if (viewAssessmentSection != null)
            {
                viewFailureMechanism = viewAssessmentSection.GetFailureMechanisms().OfType<GrassCoverErosionOutwardsFailureMechanism>().Single();
            }

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
                new HydraulicBoundariesGroupContext(failureMechanism.HydraulicBoundaryLocations, failureMechanism, failureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(failureMechanism), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(GrassCoverErosionOutwardsFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(failureMechanismContext.WrappedData)
            };
        }

        private static IList GetInputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private static IList GetOutputs(GrassCoverErosionOutwardsFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<GrassCoverErosionOutwardsFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(GrassCoverErosionOutwardsFailureMechanismContext grassCoverErosionOutwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionOutwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionOutwardsFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(GrassCoverErosionOutwardsFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(GrassCoverErosionOutwardsFailureMechanismContext grassCoverErosionOutwardsFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(grassCoverErosionOutwardsFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(grassCoverErosionOutwardsFailureMechanismContext, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        private static object[] GetHydraulicBoundariesGroupContextChildNodeObjects(HydraulicBoundariesGroupContext hydraulicBoundariesGroupContext)
        {
            IAssessmentSection assessmentSection = hydraulicBoundariesGroupContext.AssessmentSection;
            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                return new object[0];
            }

            ObservableList<HydraulicBoundaryLocation> locations = hydraulicBoundariesGroupContext.WrappedData;
            var failureMechanism = hydraulicBoundariesGroupContext.FailureMechanism;
            return new object[]
            {
                new GrassCoverErosionOutwardsDesignWaterLevelLocationsContext(locations, assessmentSection, failureMechanism),
                new GrassCoverErosionOutwardsWaveHeightLocationsContext(locations, assessmentSection, failureMechanism),
                new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(hydraulicBoundariesGroupContext.FailureMechanism.WaveConditionsCalculationGroup,
                                                                                   hydraulicBoundariesGroupContext.FailureMechanism,
                                                                                   assessmentSection)
            };
        }

        #endregion

        #region GrassCoverErosionOutwardsDesignWaterLevelLocationsContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsDesignWaterLevelLocationsContextMenuStrip(GrassCoverErosionOutwardsDesignWaterLevelLocationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var designWaterLevelItem = new StrictContextMenuItem(
                RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All,
                nodeData.AssessmentSection.HydraulicBoundaryDatabase != null ?
                    RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaterLevelLocation_Calculate_All_ToolTip :
                    RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwardsWaterLevelLocation_No_HRD_To_Calculate,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (hydraulicBoundaryLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    IAssessmentSection assessmentSection = nodeData.AssessmentSection;
                    GrassCoverErosionOutwardsFailureMechanism failureMechanism = nodeData.FailureMechanism;

                    var calculationBeta = GetModifiedBeta(failureMechanism, assessmentSection);

                    if (!double.IsNaN(calculationBeta))
                    {
                        bool successfulCalculation = hydraulicBoundaryLocationCalculationGuiService.CalculateDesignWaterLevels(
                            assessmentSection.HydraulicBoundaryDatabase.FilePath,
                            nodeData.WrappedData,
                            assessmentSection.Id,
                            calculationBeta,
                            new GrassCoverErosionOutwardsDesignWaterLevelCalculationMessageProvider());

                        if (successfulCalculation)
                        {
                            nodeData.WrappedData.NotifyObservers();
                        }
                    }
                });

            if (nodeData.AssessmentSection.HydraulicBoundaryDatabase == null)
            {
                designWaterLevelItem.Enabled = false;
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

        #region GrassCoverErosionOutwardsWaveHeightLocationsContext TreeNodeInfo

        private ContextMenuStrip GrassCoverErosionOutwardsWaveHeightLocationsContextMenuStrip(GrassCoverErosionOutwardsWaveHeightLocationsContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var waveHeightItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                nodeData.AssessmentSection.HydraulicBoundaryDatabase == null
                    ? RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwards_WaveHeight_No_HRD_To_Calculate
                    : RingtoetsGrassCoverErosionOutwardsFormsResources.GrassCoverErosionOutwards_WaveHeight_Calculate_All_ToolTip,
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

                    var calculationBeta = GetModifiedBeta(failureMechanism, assessmentSection);

                    if (!double.IsNaN(calculationBeta))
                    {
                        bool successfulCalculation = hydraulicBoundaryLocationCalculationGuiService.CalculateWaveHeights(
                            assessmentSection.HydraulicBoundaryDatabase.FilePath,
                            nodeData.WrappedData,
                            assessmentSection.Id,
                            calculationBeta,
                            new GrassCoverErosionOutwardsWaveHeightCalculationMessageProvider());

                        if (successfulCalculation)
                        {
                            nodeData.WrappedData.NotifyObservers();
                        }
                    }
                });

            if (nodeData.AssessmentSection.HydraulicBoundaryDatabase == null)
            {
                waveHeightItem.Enabled = false;
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
                                                                                                       nodeData.FailureMechanism,
                                                                                                       nodeData.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext(group,
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
            var group = nodeData.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var isNestedGroup = parentData is GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateWaveConditionsCalculationsItem(nodeData);

            if (!isNestedGroup)
            {
                builder.AddCustomItem(generateCalculationsItem);
            }

            builder.AddExportItem()
                   .AddSeparator()
                   .AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(nodeData, AddWaveConditionsCalculation);

            if (!isNestedGroup)
            {
                builder.AddSeparator()
                       .AddRemoveAllChildrenItem();
            }

            builder.AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(nodeData,
                                                          ValidateAll,
                                                          ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup)
                   .AddPerformAllCalculationsInGroupItem(group, nodeData, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup)
                   .AddClearAllCalculationOutputInGroupItem(group)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem()
                       .AddDeleteItem()
                       .AddSeparator();
            }

            return builder.AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private StrictContextMenuItem CreateGenerateWaveConditionsCalculationsItem(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext nodeData)
        {
            HydraulicBoundaryDatabase hydraulicBoundaryDatabase = nodeData.AssessmentSection.HydraulicBoundaryDatabase;
            bool locationsAvailable = hydraulicBoundaryDatabase != null && hydraulicBoundaryDatabase.Locations.Any();

            string grassCoverErosionOutwardsWaveConditionsCalculationGroupContextToolTip = locationsAvailable
                                                                                               ? RingtoetsCommonFormsResources.CalculationGroup_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip
                                                                                               : RingtoetsCommonFormsResources.CalculationGroup_No_HRD_To_Generate_ToolTip;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                             grassCoverErosionOutwardsWaveConditionsCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => { ShowHydraulicBoundaryLocationSelectionDialog(nodeData); })
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
                                                                                        IList<ICalculationBase> calculationCollection)
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

        private static void ValidateAll(IEnumerable<GrassCoverErosionOutwardsWaveConditionsCalculation> calculations, HydraulicBoundaryDatabase database)
        {
            foreach (GrassCoverErosionOutwardsWaveConditionsCalculation calculation in calculations)
            {
                GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(calculation, database.FilePath);
            }
        }

        private static void ValidateAll(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>(),
                        context.AssessmentSection.HydraulicBoundaryDatabase);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationGroup(GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection)
        {
            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported;
            }

            string validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }

            return null;
        }

        private void CalculateAll(CalculationGroup group, GrassCoverErosionOutwardsWaveConditionsCalculationGroupContext context)
        {
            var calculations = group.GetCalculations().OfType<GrassCoverErosionOutwardsWaveConditionsCalculation>().ToArray();

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

            foreach (var calculation in calculations)
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
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(context.WrappedData),
                new GrassCoverErosionOutwardsWaveConditionsInputContext(context.WrappedData.InputParameters,
                                                                        context.FailureMechanism)
            };

            if (context.WrappedData.HasOutput)
            {
                childNodes.Add(context.WrappedData.Output);
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

            GrassCoverErosionOutwardsWaveConditionsCalculation calculation = nodeData.WrappedData;

            return builder
                .AddExportItem()
                .AddSeparator()
                .AddValidateCalculationItem(nodeData,
                                            Validate,
                                            ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                .AddPerformCalculationItem(calculation, nodeData, PerformCalculation, ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                .AddClearCalculationOutputItem(calculation)
                .AddSeparator()
                .AddRenameItem()
                .AddDeleteItem()
                .AddSeparator()
                .AddExpandAllItem()
                .AddCollapseAllItem()
                .AddSeparator()
                .AddPropertiesItem()
                .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculation(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static void Validate(GrassCoverErosionOutwardsWaveConditionsCalculationContext context)
        {
            GrassCoverErosionOutwardsWaveConditionsCalculationService.Validate(context.WrappedData, context.AssessmentSection.HydraulicBoundaryDatabase.FilePath);
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

        private static double GetModifiedBeta(GrassCoverErosionOutwardsFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            var calculationBeta = double.NaN;
            try
            {
                calculationBeta = failureMechanism.CalculationBeta(assessmentSection);
            }
            catch (ArgumentException e)
            {
                log.ErrorFormat(e.Message);
            }
            return calculationBeta;
        }

        #endregion
    }
}