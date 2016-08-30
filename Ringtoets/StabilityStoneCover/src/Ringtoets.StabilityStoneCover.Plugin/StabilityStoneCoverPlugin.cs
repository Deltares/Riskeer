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

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.StabilityStoneCover.Data;
using Ringtoets.StabilityStoneCover.Forms.PresentationObjects;
using Ringtoets.StabilityStoneCover.Forms.PropertyClasses;
using Ringtoets.StabilityStoneCover.Forms.Views;
using Ringtoets.StabilityStoneCover.Plugin.Properties;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.StabilityStoneCover.Plugin
{
    /// <summary>
    /// The plugin for the <see cref="StabilityStoneCoverFailureMechanism"/>.
    /// </summary>
    public class StabilityStoneCoverPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<StabilityStoneCoverFailureMechanismContext, StabilityStoneCoverFailureMechanismContextProperties>();
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>,
                IEnumerable<StabilityStoneCoverFailureMechanismSectionResult>,
                StabilityStoneCoverResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<StabilityStoneCoverFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<StabilityStoneCoverHydraulicBoundariesCalculationGroupContext>(
                HydraulicBoundariesCalculationGroupContextChildNodeObjects,
                HydraulicBoundariesCalculationGroupContextContextMenuStrip,
                HydraulicBoundariesCalculationGroupContextOnNodeRemoved);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        #region ViewInfos

        #region FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>

        private static bool CloseFailureMechanismResultViewForData(StabilityStoneCoverResultView view, object dataToCloseFor)
        {
            var viewData = view.Data;
            var assessmentSection = dataToCloseFor as IAssessmentSection;
            var failureMechanism = dataToCloseFor as StabilityStoneCoverFailureMechanism;
            var failureMechanismContext = dataToCloseFor as IFailureMechanismContext<StabilityStoneCoverFailureMechanism>;

            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<StabilityStoneCoverFailureMechanism>()
                    .Any(fm => ReferenceEquals(viewData, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #endregion

        #region TreeNodeInfos

        #region StabilityStoneCoverFailureMechanismContext

        private object[] FailureMechanismEnabledChildNodeObjects(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            StabilityStoneCoverFailureMechanism wrappedData = failureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, failureMechanismContext.Parent), TreeFolderCategory.Input),
                new StabilityStoneCoverHydraulicBoundariesCalculationGroupContext(wrappedData.HydraulicBoundariesCalculationGroup, wrappedData),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(failureMechanismContext.WrappedData)
            };
        }

        private static IList GetInputs(StabilityStoneCoverFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private IList GetOutputs(StabilityStoneCoverFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<StabilityStoneCoverFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(StabilityStoneCoverFailureMechanismContext failureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(StabilityStoneCoverFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(StabilityStoneCoverFailureMechanismContext failureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        #endregion

        #region StabilityStoneCoverHydraulicBoundariesCalculationGroupContext

        private object[] HydraulicBoundariesCalculationGroupContextChildNodeObjects(StabilityStoneCoverHydraulicBoundariesCalculationGroupContext nodeData)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase item in nodeData.WrappedData.Children)
            {
                var calculation = item as ICalculation; // TODO: Change cast to HydraulicBoundaryCalculation; Implementation part of WTI-724
                var group = item as CalculationGroup;

                if (calculation != null)
                {
                    // TODO: Implementation part of WTI-724
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new StabilityStoneCoverHydraulicBoundariesCalculationGroupContext(group,
                                                                                                           nodeData.FailureMechanism));
                }
                else
                {
                    childNodeObjects.Add(item);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip HydraulicBoundariesCalculationGroupContextContextMenuStrip(StabilityStoneCoverHydraulicBoundariesCalculationGroupContext nodeData, object parentData, TreeViewControl treeViewControl)
        {
            var group = nodeData.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(nodeData, treeViewControl));
            var isNestedGroup = parentData is StabilityStoneCoverHydraulicBoundariesCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateHydraulicBoundaryCalculationsItem(nodeData);

            if (!isNestedGroup)
            {
                builder.AddCustomItem(generateCalculationsItem)
                       .AddSeparator();
            }

            StrictContextMenuItem addCalculationItem = RingtoetsContextMenuItemFactory.CreateAddCalculationItem(nodeData, AddHydraulicBoundaryCalculation);
            addCalculationItem.Enabled = false;
            builder.AddCreateCalculationGroupItem(group)
                   .AddCustomItem(addCalculationItem);
            //.AddCreateCalculationItem(nodeData, AddHydraulicBoundaryCalculation); // TODO: Replace the AddCustomItem(addCalculationItem) call with this; Implemented in WTI-427

            if (!isNestedGroup)
            {
                builder.AddSeparator()
                       .AddRemoveAllChildrenItem(group, Gui.ViewCommands);
            }

            builder.AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(nodeData, c => ValidateAll(c.WrappedData.GetCalculations().OfType<ICalculation>())) // TODO: Replace ICalculation with concrete type; Implemented in WTI-427
                   .AddPerformAllCalculationsInGroupItem(group, nodeData, CalculateAll)
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

        private StrictContextMenuItem CreateGenerateHydraulicBoundaryCalculationsItem(StabilityStoneCoverHydraulicBoundariesCalculationGroupContext nodeData)
        {
            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationsGroup_Generate_calculations,
                                             Resources.StabilityStoneCoverPlugin_CreateGenerateHydraulicBoundaryCalculationsItem_ToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) =>
                                             {
                                                 /* TODO: Implement for WTI-427 */
                                             })
            {
                Enabled = false
            };
        }

        private void AddHydraulicBoundaryCalculation(StabilityStoneCoverHydraulicBoundariesCalculationGroupContext stabilityStoneCoverHydraulicBoundariesCalculationGroupContext)
        {
            // TODO: Implemented in WTI-427
        }

        private void ValidateAll(IEnumerable<ICalculation> calculations)
        {
            // TODO: Implemented in WTI-427
        }

        private void CalculateAll(CalculationGroup group, StabilityStoneCoverHydraulicBoundariesCalculationGroupContext context)
        {
            // TODO: Implemented in WTI-427
        }

        private void HydraulicBoundariesCalculationGroupContextOnNodeRemoved(StabilityStoneCoverHydraulicBoundariesCalculationGroupContext nodeData, object parentNodeData)
        {
            var parentGroupContext = (StabilityStoneCoverHydraulicBoundariesCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(nodeData.WrappedData);

            parentGroupContext.NotifyObservers();
        }

        #endregion

        #endregion
    }
}