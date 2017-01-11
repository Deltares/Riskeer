// Copyright (C) Stichting Deltares 2016. All rights reserved.
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
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.DuneErosion.Data;
using Ringtoets.DuneErosion.Forms;
using Ringtoets.DuneErosion.Forms.PresentationObjects;
using Ringtoets.DuneErosion.Forms.PropertyClasses;
using Ringtoets.DuneErosion.Forms.Views;
using Ringtoets.DuneErosion.IO;
using Ringtoets.DuneErosion.Plugin.Properties;
using Ringtoets.DuneErosion.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;

namespace Ringtoets.DuneErosion.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="DuneErosionFailureMechanism"/>.
    /// </summary>
    public class DuneErosionPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<DuneErosionFailureMechanismContext, DuneErosionFailureMechanismProperties>
            {
                CreateInstance = context => new DuneErosionFailureMechanismProperties(context.WrappedData,
                                                                                      new DuneErosionFailureMechanismPropertyChangeHandler())
            };
            yield return new PropertyInfo<DuneLocationsContext, DuneLocationsContextProperties>
            {
                CreateInstance = context => new DuneLocationsContextProperties(context.WrappedData)
            };

            yield return new PropertyInfo<DuneLocationContext, DuneLocationContextProperties>();
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<DuneErosionFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip
                );

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<DuneLocationsContext>
            {
                Text = context => RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ForeColor = context => context.WrappedData.Count > 0 ?
                                           Color.FromKnownColor(KnownColor.ControlText) :
                                           Color.FromKnownColor(KnownColor.GrayText),
                ContextMenuStrip = DuneLocationsContextMenuStrip
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<DuneErosionFailureMechanismSectionResult>,
                IEnumerable<DuneErosionFailureMechanismSectionResult>,
                DuneErosionFailureMechanismResultView>
            {
                GetViewName = (view, results) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };

            yield return new ViewInfo<DuneErosionFailureMechanismContext, DuneErosionFailureMechanismView>
            {
                GetViewName = (view, mechanism) => mechanism.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return new ViewInfo<DuneLocationsContext, IEnumerable<DuneLocation>, DuneLocationsView>
            {
                GetViewName = (view, context) => RingtoetsCommonDataResources.HydraulicBoundaryConditions_DisplayName,
                Image = RingtoetsCommonFormsResources.GenericInputOutputIcon,
                GetViewData = context => context.WrappedData,
                CloseForData = CloseDuneLocationViewForData,
                AfterCreate = (view, context) => view.AssessmentSection = context.AssessmentSection,
                AdditionalDataCheck = context => context.WrappedData.Any()
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return new ExportInfo<DuneLocationsContext>
            {
                CreateFileExporter = (context, filePath) => new DuneLocationsExporter(context.WrappedData, filePath),
                IsEnabled = context => context.WrappedData.Any(dl => dl.Output != null),
                FileFilter = Resources.DuneErosionPlugin_GetExportInfos_MorphAn_boundary_conditions_file_filter
            };
        }

        #region TreeNodeInfo

        #region failureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(DuneErosionFailureMechanismContext failureMechanismContext)
        {
            DuneErosionFailureMechanism wrappedData = failureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, failureMechanismContext.Parent), TreeFolderCategory.Input),
                new DuneLocationsContext(wrappedData.DuneLocations, wrappedData, failureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static IList GetInputs(DuneErosionFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IList GetOutputs(DuneErosionFailureMechanism failureMechanism)
        {
            return new ArrayList
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

        private ContextMenuStrip DuneLocationsContextMenuStrip(DuneLocationsContext context, object parent, TreeViewControl treeViewControl)
        {
            bool locationsAvailable = context.FailureMechanism.DuneLocations.Any();

            string toolTip = locationsAvailable
                                 ? Resources.DuneErosionPlugin_DuneLocationsContextMenuStrip_Calculate_all_ToolTip
                                 : Resources.DuneErosionPlugin_DuneLocationsContextMenuStrip_Calculate_all_ToolTip_no_locations;

            var calculateAllItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Calculate_all,
                toolTip,
                RingtoetsCommonFormsResources.CalculateAllIcon,
                (sender, args) =>
                {
                    ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                                     context.WrappedData
                                                            .Select(location => new DuneErosionBoundaryCalculationActivity(location,
                                                                                                                           context.FailureMechanism,
                                                                                                                           context.AssessmentSection)).ToArray());
                    context.NotifyObservers();
                })
            {
                Enabled = locationsAvailable
            };

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
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #region DuneErosionFailureMechanismView ViewInfo

        private static bool CloseFailureMechanismViewForData(DuneErosionFailureMechanismView view, object data)
        {
            var assessmentSection = data as IAssessmentSection;
            var failureMechanism = data as DuneErosionFailureMechanism;

            var viewFailureMechanismContext = (DuneErosionFailureMechanismContext) view.Data;
            var viewFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewFailureMechanism, failureMechanism);
        }

        #endregion

        #region DuneLocationsView ViewInfo

        private static bool CloseDuneLocationViewForData(DuneLocationsView view, object dataToCloseFor)
        {
            DuneErosionFailureMechanism viewFailureMechanism = null;
            if (view.AssessmentSection != null)
            {
                viewFailureMechanism = view.AssessmentSection.GetFailureMechanisms().OfType<DuneErosionFailureMechanism>().Single();
            }

            var failureMechanismContext = dataToCloseFor as DuneErosionFailureMechanismContext;
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as DuneErosionFailureMechanism;

            if (assessmentSection != null)
            {
                failureMechanism = ((IAssessmentSection)dataToCloseFor).GetFailureMechanisms().OfType<DuneErosionFailureMechanism>().Single();
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.Parent.GetFailureMechanisms().OfType<DuneErosionFailureMechanism>().Single();
            }
            return failureMechanism != null && ReferenceEquals(failureMechanism, viewFailureMechanism);
        }

        #endregion

        #endregion
    }
}