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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.HydraRing.IO;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.Views;
using Ringtoets.StabilityPointStructures.IO;
using Ringtoets.StabilityPointStructures.Plugin.Properties;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonServiceResources = Ringtoets.Common.Service.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="StabilityPointStructuresFailureMechanism"/>.
    /// </summary>
    public class StabilityPointStructuresPlugin : PluginBase
    {
        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<
                FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>,
                IEnumerable<StabilityPointStructuresFailureMechanismSectionResult>,
                StabilityPointStructuresFailureMechanismResultView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };
        }

        /// <summary>
        /// Returns all <see cref="TreeNodeInfo"/> instances provided for data of <see cref="StabilityPointStructuresPlugin"/>.
        /// </summary>
        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<StabilityPointStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityPointStructuresContext>
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

            yield return new TreeNodeInfo<StabilityPointStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RingtoetsCommonFormsResources.StructuresIcon
            };

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<StabilityPointStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<StabilityPointStructuresCalculationContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<StabilityPointStructuresInputContext>
            {
                Text = inputContext => RingtoetsCommonFormsResources.Calculation_Input,
                Image = inputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<StabilityPointStructuresContext>
            {
                CreateFileImporter = (context, filePath) => new StabilityPointStructuresImporter(context.WrappedData,
                                                                                                 context.AssessmentSection.ReferenceLine,
                                                                                                 filePath),
                Name = RingtoetsCommonFormsResources.StructuresImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                FileFilter = RingtoetsCommonIOResources.DataTypeDisplayName_shape_file_filter,
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null
            };
        }

        #region ViewInfo

        #region StabilityPointStructuresFailureMechanismResultView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(StabilityPointStructuresFailureMechanismResultView view, object viewData)
        {
            var assessmentSection = viewData as IAssessmentSection;
            var failureMechanism = viewData as StabilityPointStructuresFailureMechanism;
            var failureMechanismContext = viewData as IFailureMechanismContext<StabilityPointStructuresFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<StabilityPointStructuresFailureMechanism>()
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

        #region Validation and Calculation

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, StabilityPointStructuresFailureMechanism failureMechanism)
        {
            if (!failureMechanism.Sections.Any())
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_failure_mechanism_sections_imported;
            }

            if (assessmentSection.HydraulicBoundaryDatabase == null)
            {
                return RingtoetsCommonFormsResources.Plugin_AllDataAvailable_No_hydraulic_boundary_database_imported;
            }

            var validationProblem = HydraulicDatabaseHelper.ValidatePathForCalculation(assessmentSection.HydraulicBoundaryDatabase.FilePath);
            if (!string.IsNullOrEmpty(validationProblem))
            {
                return string.Format(RingtoetsCommonServiceResources.Hydraulic_boundary_database_connection_failed_0_,
                                     validationProblem);
            }

            return null;
        }

        private static void ValidateAll(IEnumerable<StabilityPointStructuresCalculation> calculations, IAssessmentSection assessmentSection) {}
        private static void CalculateAll(StabilityPointStructuresFailureMechanismContext context) {}
        private static void CalculateAll(CalculationGroup group, StabilityPointStructuresCalculationGroupContext context) {}

        #endregion

        #region TreeNodeInfo

        #region StabilityPointStructuresFailureMechanismContext TreeNodeInfo

        private object[] FailureMechanismEnabledChildNodeObjects(StabilityPointStructuresFailureMechanismContext stabilityPointStructuresFailureMechanismContext)
        {
            StabilityPointStructuresFailureMechanism wrappedData = stabilityPointStructuresFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                                       GetInputs(wrappedData, stabilityPointStructuresFailureMechanismContext.Parent),
                                       TreeFolderCategory.Input),
                new StabilityPointStructuresCalculationGroupContext(wrappedData.CalculationsGroup,
                                                                    wrappedData,
                                                                    stabilityPointStructuresFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName,
                                       GetOutputs(wrappedData),
                                       TreeFolderCategory.Output)
            };
        }

        private object[] FailureMechanismDisabledChildNodeObjects(StabilityPointStructuresFailureMechanismContext stabilityPointStructuresFailureMechanismContext)
        {
            return new object[]
            {
                new CommentContext<ICommentable>(stabilityPointStructuresFailureMechanismContext.WrappedData)
            };
        }

        private static object[] GetInputs(StabilityPointStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures, assessmentSection),
                new CommentContext<ICommentable>(failureMechanism)
            };
        }

        private static object[] GetOutputs(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            return new object[]
            {
                new FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>(failureMechanism.SectionResults, failureMechanism)
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(StabilityPointStructuresFailureMechanismContext failureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              failureMechanismContext,
                              c => ValidateAll(c.WrappedData.Calculations.OfType<StabilityPointStructuresCalculation>(), c.Parent),
                              ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism)
                          .AddPerformAllCalculationsInFailureMechanismItem(failureMechanismContext, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism)
                          .AddClearAllCalculationOutputInFailureMechanismItem(failureMechanismContext.WrappedData)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(StabilityPointStructuresFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(StabilityPointStructuresFailureMechanismContext stabilityPointStructuresFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(stabilityPointStructuresFailureMechanismContext, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(stabilityPointStructuresFailureMechanismContext, null)
                          .AddSeparator()
                          .AddExpandAllItem()
                          .AddCollapseAllItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationsInFailureMechanism(StabilityPointStructuresFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent, context.WrappedData);
        }

        #endregion

        #region StabilityPointStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(StabilityPointStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                var calculation = calculationItem as StabilityPointStructuresCalculation;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new StabilityPointStructuresCalculationContext(calculation,
                                                                                        context.FailureMechanism,
                                                                                        context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new StabilityPointStructuresCalculationGroupContext(group,
                                                                                             context.FailureMechanism,
                                                                                             context.AssessmentSection));
                }
                else
                {
                    childNodeObjects.Add(calculationItem);
                }
            }

            return childNodeObjects.ToArray();
        }

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(StabilityPointStructuresCalculationGroupContext context,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            bool isNestedGroup = parentData is StabilityPointStructuresCalculationGroupContext;

            StrictContextMenuItem generateCalculationsItem = CreateGenerateCalculationsItem(context);

            if (!isNestedGroup)
            {
                builder.AddCustomItem(generateCalculationsItem)
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation);

            if (!isNestedGroup)
            {
                builder.AddSeparator()
                       .AddRemoveAllChildrenItem(group, Gui.ViewCommands);
            }

            builder.AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(context,
                                                          c => ValidateAll(c.WrappedData.GetCalculations().OfType<StabilityPointStructuresCalculation>(), c.AssessmentSection),
                                                          ValidateAllDataAvailableAndGetErrorMessageForCalculationsInGroup)
                   .AddPerformAllCalculationsInGroupItem(group, context, CalculateAll, ValidateAllDataAvailableAndGetErrorMessageForCalculationsInGroup)
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

        private StrictContextMenuItem CreateGenerateCalculationsItem(StabilityPointStructuresCalculationGroupContext nodeData)
        {
            var generateCalculationsItem = new StrictContextMenuItem(
                RingtoetsCommonFormsResources.CalculationGroup_Generate_Scenarios,
                Resources.StabilityPointStructuresPlugin_CreateGenerateCalculationsItem_ToolTip,
                RingtoetsCommonFormsResources.GenerateScenariosIcon, (o, args) => { })
            {
                Enabled = false
            };
            return generateCalculationsItem;
        }

        private static void CalculationGroupContextOnNodeRemoved(StabilityPointStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (StabilityPointStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        private static void AddCalculation(StabilityPointStructuresCalculationGroupContext context)
        {
            var calculation = new StabilityPointStructuresCalculation
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RingtoetsCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculationsInGroup(StabilityPointStructuresCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        #endregion

        #region StabilityPointStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(StabilityPointStructuresCalculationContext context)
        {
            var childNodes = new List<object>
            {
                new CommentContext<ICommentable>(context.WrappedData),
                new StabilityPointStructuresInputContext(context.WrappedData.InputParameters,
                                                         context.WrappedData,
                                                         context.FailureMechanism,
                                                         context.AssessmentSection)
            };

            if (context.WrappedData.HasOutput)
            {
                childNodes.Add(context.WrappedData.Output);
            }
            else
            {
                childNodes.Add(new EmptyProbabilityAssessmentOutput());
            }

            return childNodes.ToArray();
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(StabilityPointStructuresCalculationContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            StabilityPointStructuresCalculation calculation = context.WrappedData;

            return builder.AddValidateCalculationItem(context, delegate { }, ValidateAllDataAvailableAndGetErrorMessageForCalculation)
                          .AddPerformCalculationItem(calculation, context, Calculate, ValidateAllDataAvailableAndGetErrorMessageForCalculation)
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

        private static string ValidateAllDataAvailableAndGetErrorMessageForCalculation(StabilityPointStructuresCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private void Calculate(StabilityPointStructuresCalculation calculation, StabilityPointStructuresCalculationContext context) {}

        private void CalculationContextOnNodeRemoved(StabilityPointStructuresCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as StabilityPointStructuresCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion

        #endregion
    }
}