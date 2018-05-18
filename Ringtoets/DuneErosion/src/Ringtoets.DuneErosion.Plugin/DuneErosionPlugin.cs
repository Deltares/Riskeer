﻿// Copyright (C) Stichting Deltares 2017. All rights reserved.
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
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Hydraulics;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Service;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms;
using Ringtoets.DuneErosion.Forms.GuiServices;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.PropertyClasses;
using Ringtoets.DuneErosion.Forms.Views;
using Ringtoets.DuneErosion.IO;
using Ringtoets.DuneErosion.Plugin.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.DuneErosion.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public class DuneErosionPlugin : PluginBase
    {
        private DuneLocationCalculationGuiService duneLocationCalculationGuiService;

        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<DuneErosionFailureMechanismContext, DuneErosionFailureMechanismProperties>
            {
                CreateInstance = context => new DuneErosionFailureMechanismProperties(context.WrappedData,
                                                                                      new DuneErosionFailureMechanismPropertyChangeHandler())
            };
            yield return new PropertyInfo<DuneLocationCalculationsContext, DuneLocationCalculationsProperties>
            {
                CreateInstance = context => new DuneLocationCalculationsProperties(context.WrappedData)
            };

            yield return new PropertyInfo<DuneLocation, DuneLocationCalculationProperties>
            {
                CreateInstance = location => new DuneLocationCalculationProperties(location.Calculation)
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<DuneErosionFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<DuneLocationCalculationsContext>
            {
                Text = context => RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ForeColor = context => context.WrappedData.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = DuneLocationsContextMenuStrip
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>,
                IObservableEnumerable<DuneErosionFailureMechanismSectionResult>,
                DuneErosionFailureMechanismResultView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new DuneErosionFailureMechanismResultView(
                    context.WrappedData,
                    (DuneErosionFailureMechanism) context.FailureMechanism)
            };

            yield return new ViewInfo<DuneErosionFailureMechanismContext, DuneErosionFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return new ViewInfo<DuneLocationCalculationsContext, IEnumerable<DuneLocationCalculation>, DuneLocationCalculationsView>
            {
                GetViewName = (view, context) => RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                GetViewData = context => context.WrappedData,
                CloseForData = CloseDuneLocationsViewForData,
                CreateInstance = context => new DuneLocationCalculationsView(context.WrappedData,
                                                                  context.FailureMechanism,
                                                                  context.AssessmentSection),
                AfterCreate = (view, context) => { view.CalculationGuiService = duneLocationCalculationGuiService; },
                AdditionalDataCheck = context => context.WrappedData.Any()
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<DuneLocationCalculationsContext>
            {
                Name = RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                CreateFileExporter = (context, filePath) => new DuneLocationCalculationsExporter(context.WrappedData, filePath),
                IsEnabled = context => context.WrappedData.Any(calculation => calculation.Output != null),
                FileFilterGenerator = new FileFilterGenerator(
                    Resources.DuneErosionPlugin_GetExportInfos_MorphAn_boundary_conditions_file_filter_Extension,
                    Resources.DuneErosionPlugin_GetExportInfos_MorphAn_boundary_conditions_file_filter_Description)
            };
        }

        public override void Activate()
        {
            base.Activate();

            if (Gui == null)
            {
                throw new InvalidOperationException("Gui cannot be null");
            }

            duneLocationCalculationGuiService = new DuneLocationCalculationGuiService(Gui.MainWindow);
        }

        #region TreeNodeInfo

        #region FailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(DuneErosionFailureMechanismContext failureMechanismContext)
        {
            DuneErosionFailureMechanism wrappedData = failureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, failureMechanismContext.Parent), TreeFolderCategory.Input),
                new DuneLocationCalculationsContext(wrappedData.CalculationsForFactorizedLowerLimitNorm, wrappedData, failureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static IEnumerable<object> GetInputs(DuneErosionFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(DuneErosionFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                failureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(DuneErosionFailureMechanismContext failureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(DuneErosionFailureMechanismContext failureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext,
                                                                  treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext,
                                                                    RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(DuneErosionFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        #endregion

        #region DuneLocationsContext TreeNodeInfo

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, DuneErosionFailureMechanism failureMechanism)
        {
            if (failureMechanism.Contribution <= 0.0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            if (!failureMechanism.DuneLocations.Any())
            {
                return Resources.DuneErosionPlugin_DuneLocationsContextMenuStrip_Calculate_all_ToolTip_no_locations;
            }

            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        private ContextMenuStrip DuneLocationsContextMenuStrip(DuneLocationCalculationsContext context, object parent, TreeViewControl treeViewControl)
        {
            var calculateAllItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                Resources.DuneErosionPlugin_DuneLocationsContextMenuStrip_Calculate_all_ToolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    if (duneLocationCalculationGuiService == null)
                    {
                        return;
                    }

                    duneLocationCalculationGuiService.Calculate(context.WrappedData,
                                                                context.AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                context.AssessmentSection.HydraulicBoundaryDatabase.EffectivePreprocessorDirectory(),
                                                                context.FailureMechanism.GetMechanismSpecificNorm(context.AssessmentSection.FailureMechanismContribution.Norm));
                });

            string validationText = ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
            if (!string.IsNullOrEmpty(validationText))
            {
                calculateAllItem.Enabled = false;
                calculateAllItem.ToolTipText = validationText;
            }

            return Gui.Get(context, treeViewControl)
                      .AddOpenItem()
                      .AddSeparator()
                      .AddExportItem()
                      .AddSeparator()
                      .AddCustomItem(calculateAllItem)
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #endregion

        #region ViewInfo

        #region DuneErosionFailureMechanismResultView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(DuneErosionFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as DuneErosionFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<DuneErosionFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                       .GetFailureMechanisms()
                       .OfType<DuneErosionFailureMechanism>()
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
        }

        #endregion

        #region DuneErosionFailureMechanismView ViewInfo

        private static bool CloseFailureMechanismViewForData(DuneErosionFailureMechanismView view, object data)
        {
            var assessmentSection = data as IAssessmentSection;
            var failureMechanism = data as DuneErosionFailureMechanism;

            var viewFailureMechanismContext = (DuneErosionFailureMechanismContext) view.Data;
            DuneErosionFailureMechanism viewFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewFailureMechanism, failureMechanism);
        }

        #endregion

        #region DuneLocationsView ViewInfo

        private static bool CloseDuneLocationsViewForData(DuneLocationCalculationsView view, object dataToCloseFor)
        {
            var failureMechanismContext = dataToCloseFor as DuneErosionFailureMechanismContext;
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as DuneErosionFailureMechanism;

            if (assessmentSection != null)
            {
                failureMechanism = ((IAssessmentSection) dataToCloseFor).GetFailureMechanisms().OfType<DuneErosionFailureMechanism>().Single();
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(failureMechanism, view.FailureMechanism);
        }

        #endregion

        #endregion
    }
}