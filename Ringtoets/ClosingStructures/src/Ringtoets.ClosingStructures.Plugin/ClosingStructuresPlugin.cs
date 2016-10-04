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
using Core.Common.Gui.Plugin;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;

namespace Ringtoets.ClosingStructures.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="ClosingStructuresFailureMechanism"/>.
    /// </summary>
    public class ClosingStructuresPlugin : PluginBase
    {
        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>,
                IEnumerable<ClosingStructuresFailureMechanismSectionResult>,
                ClosingStructuresFailureMechanismResultView>
            {
                GetViewName = (v, o) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };
        }

        /// <summary>
        /// Returns all <see cref="TreeNodeInfo"/> instances provided for data of <see cref="ClosingStructuresPlugin"/>.
        /// </summary>
        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<ClosingStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<ClosingStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ClosingStructuresContext>()
            {
                Text = context => RingtoetsCommonFormsResources.StructuresCollection_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any() ? Color.FromKnownColor(KnownColor.ControlText) : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = context => context.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddImportItem()
                                                                                 .AddSeparator()
                                                                                 .AddExpandAllItem()
                                                                                 .AddCollapseAllItem()
                                                                                 .Build()
            };
        }

        #region ViewInfo

        #region ClosingStructuresFailureMechanismResultView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(ClosingStructuresFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as ClosingStructuresFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<ClosingStructuresFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<ClosingStructuresFailureMechanism>()
                    .Any(fm => ReferenceEquals(view.Data, fm.SectionResults));
            }
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }
            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.SectionResults);
        }

        #endregion

        #endregion

        #region TreeNodeInfo

        #region ClosingStructuresFailureMechanismContext TreeNodeInfo

        private object[] FailureMechanismEnabledChildNodeObjects(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext)
        {
            ClosingStructuresFailureMechanism wrappedData = closingStructuresFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, closingStructuresFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new ClosingStructuresCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, closingStructuresFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(closingStructuresFailureMechanismContext.WrappedData)
            };
        }

        private static IList GetInputs(ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ClosingStructuresContext(failureMechanism.ClosingStructures, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private static IList GetOutputs(ClosingStructuresFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(closingStructuresFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(closingStructuresFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              closingStructuresFailureMechanismContext,
                              c => ValidateAll(),
                              ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism)
                          .AddPerformAllCalculationsInFailureMechanismItem(closingStructuresFailureMechanismContext, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism)
                          .AddClearAllCalculationOutputInFailureMechanismItem(closingStructuresFailureMechanismContext.WrappedData)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(ClosingStructuresFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(closingStructuresFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(closingStructuresFailureMechanismContext, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        private void ValidateAll()
        {
            //Add validation service - currently a place holder
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism(ClosingStructuresFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent, context.WrappedData);
        }

        private void CalculateAll(ClosingStructuresFailureMechanismContext context)
        {
            CalculateAll(context.WrappedData, context.WrappedData.Calculations.OfType<ClosingStructuresCalculation>());
        }

        #endregion

        #region ClosingStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(ClosingStructuresCalculationGroupContext context)
        {
            //Part of WTI-550
            return new object[0];
        }

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(ClosingStructuresCalculationGroupContext context, object parentData, TreeViewControl treeViewControl)
        {
            var group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var isNestedGroup = parentData is ClosingStructuresCalculationGroupContext;

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation);

            if (!isNestedGroup)
            {
                builder
                    .AddSeparator()
                    .AddRemoveAllChildrenItem(group, Gui.ViewCommands);
            }
            
            builder.AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(context, c => ValidateAll(), ValidateAllDataAvailableAndGetErrorMessage)
                   .AddPerformAllCalculationsInGroupItem(group, context, CalculateAll, ValidateAllDataAvailableAndGetErrorMessage)
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

        private static string ValidateAllDataAvailableAndGetErrorMessage(ClosingStructuresCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static void CalculateAll(CalculationGroup group, ClosingStructuresCalculationGroupContext context)
        {
            CalculateAll(context.FailureMechanism, group.GetCalculations().OfType<ClosingStructuresCalculation>());
        }

        private void AddCalculation(ClosingStructuresCalculationGroupContext closingStructuresCalculationGroupContext)
        {
            
        }

        private static void CalculationGroupContextOnNodeRemoved(ClosingStructuresCalculationGroupContext context, object parentNodeData)
        {
            //Part of WTI-550
        }

        #endregion

        #endregion

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, ClosingStructuresFailureMechanism failureMechanism)
        {
            //Check for database connection/part of a validation issue - currently a placeholder
            return string.Empty;
        }

        private static void CalculateAll(ClosingStructuresFailureMechanism failureMechanism, IEnumerable<ClosingStructuresCalculation> calculations)
        {
            //Add calculate logic, part of WTI-554
        }
    }
}