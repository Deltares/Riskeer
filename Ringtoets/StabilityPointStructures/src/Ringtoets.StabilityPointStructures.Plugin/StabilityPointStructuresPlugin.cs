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
using Core.Common.Base;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Service;
using Ringtoets.Common.Utils;
using Ringtoets.StabilityPointStructures.Data;
using Ringtoets.StabilityPointStructures.Forms.PresentationObjects;
using Ringtoets.StabilityPointStructures.Forms.PropertyClasses;
using Ringtoets.StabilityPointStructures.Forms.Views;
using Ringtoets.StabilityPointStructures.IO;
using Ringtoets.StabilityPointStructures.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.StabilityPointStructures.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="StabilityPointStructuresFailureMechanism"/>.
    /// </summary>
    public class StabilityPointStructuresPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<StabilityPointStructuresFailureMechanismContext, StabilityPointStructuresFailureMechanismProperties>
            {
                CreateInstance = context => new StabilityPointStructuresFailureMechanismProperties(
                    context.WrappedData,
                    new FailureMechanismPropertyChangeHandler<StabilityPointStructuresFailureMechanism>())
            };
            yield return new PropertyInfo<StabilityPointStructure, StabilityPointStructureProperties>();
            yield return new PropertyInfo<StabilityPointStructuresInputContext, StabilityPointStructuresInputContextProperties>
            {
                CreateInstance = context => new StabilityPointStructuresInputContextProperties(context)
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<StabilityPointStructuresFailureMechanismContext, StabilityPointStructuresFailureMechanismView>
            {
                GetViewName = (view, mechanism) => mechanism.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseStabilityPointStructuresFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

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

            yield return new ViewInfo<StabilityPointStructuresScenariosContext, CalculationGroup, StabilityPointStructuresScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = RingtoetsCommonFormsResources.ScenariosIcon,
                AfterCreate = (view, context) => view.FailureMechanism = context.ParentFailureMechanism,
                CloseForData = CloseScenariosViewForData
            };
        }

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
                                                                                 .AddDeleteChildrenItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<StabilityPointStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RingtoetsCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddDeleteItem()
                                                                                  .AddSeparator()
                                                                                  .AddPropertiesItem()
                                                                                  .Build(),
                CanRemove = CanRemoveStabilityPointStructure,
                OnNodeRemoved = OnStabilityPointStructureRemoved
            };

            yield return new TreeNodeInfo<StabilityPointStructuresScenariosContext>
            {
                Text = context => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
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
                FileFilter = new ExpectedFile(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                              RingtoetsCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null
            };
        }

        #region ViewInfo

        #region StabilityPointStructuresFailureMechanismView ViewInfo

        private static bool CloseStabilityPointStructuresFailureMechanismViewForData(StabilityPointStructuresFailureMechanismView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as StabilityPointStructuresFailureMechanism;

            var viewFailureMechanismContext = (StabilityPointStructuresFailureMechanismContext) view.Data;
            var viewFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewFailureMechanism, failureMechanism);
        }

        #endregion

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

        #region StabilityPointStructuresScenariosView ViewInfo

        private static bool CloseScenariosViewForData(StabilityPointStructuresScenariosView view, object removedData)
        {
            var failureMechanism = removedData as StabilityPointStructuresFailureMechanism;

            var failureMechanismContext = removedData as StabilityPointStructuresFailureMechanismContext;
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            var assessmentSection = removedData as IAssessmentSection;
            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<StabilityPointStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion

        #endregion

        #region Validation and Calculation

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            if (failureMechanism.Contribution <= 0.0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        private static void ValidateAll(IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations, IAssessmentSection assessmentSection)
        {
            foreach (var calculation in calculations)
            {
                StabilityPointStructuresCalculationService.Validate(calculation, assessmentSection);
            }
        }

        private void CalculateAll(StabilityPointStructuresFailureMechanism failureMechanism,
                                  IEnumerable<StructuresCalculation<StabilityPointStructuresInput>> calculations,
                                  IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             calculations.Select(calc => new StabilityPointStructuresCalculationActivity(
                                                                     calc,
                                                                     assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                     failureMechanism,
                                                                     assessmentSection)).ToArray());
        }

        #endregion

        #region TreeNodeInfo

        #region StabilityPointStructure TreeNodeInfo

        private static bool CanRemoveStabilityPointStructure(StabilityPointStructure nodeData, object parentData)
        {
            return parentData is StabilityPointStructuresContext;
        }

        private static void OnStabilityPointStructureRemoved(StabilityPointStructure nodeData, object parentData)
        {
            var parentContext = (StabilityPointStructuresContext) parentData;
            IEnumerable<IObservable> changedObservables = StabilityPointStructuresDataSynchronizationService.RemoveStructure(parentContext.FailureMechanism,
                                                                                                                             nodeData);
            foreach (IObservable observable in changedObservables)
            {
                observable.NotifyObservers();
            }
        }

        #endregion

        #region StabilityPointStructuresFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(StabilityPointStructuresFailureMechanismContext stabilityPointStructuresFailureMechanismContext)
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

        private static object[] FailureMechanismDisabledChildNodeObjects(StabilityPointStructuresFailureMechanismContext stabilityPointStructuresFailureMechanismContext)
        {
            return new object[]
            {
                stabilityPointStructuresFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static object[] GetInputs(StabilityPointStructuresFailureMechanism failureMechanism,
                                          IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new StabilityPointStructuresContext(failureMechanism.StabilityPointStructures, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static object[] GetOutputs(StabilityPointStructuresFailureMechanism failureMechanism)
        {
            return new object[]
            {
                new StabilityPointStructuresScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new FailureMechanismSectionResultContext<StabilityPointStructuresFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(StabilityPointStructuresFailureMechanismContext failureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(failureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(failureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              failureMechanismContext,
                              ValidateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              failureMechanismContext,
                              CalculateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(failureMechanismContext.WrappedData)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
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

            return builder.AddToggleRelevancyOfFailureMechanismItem(stabilityPointStructuresFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(StabilityPointStructuresFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent, context.WrappedData);
        }

        private static void ValidateAll(StabilityPointStructuresFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<StructuresCalculation<StabilityPointStructuresInput>>(),
                        context.Parent);
        }

        private void CalculateAll(StabilityPointStructuresFailureMechanismContext context)
        {
            CalculateAll(context.WrappedData, context.WrappedData.Calculations.OfType<StructuresCalculation<StabilityPointStructuresInput>>(), context.Parent);
        }

        #endregion

        #region StabilityPointStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(StabilityPointStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                var calculation = calculationItem as StructuresCalculation<StabilityPointStructuresInput>;
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

            if (!isNestedGroup)
            {
                builder.AddCustomItem(CreateGenerateStabilityPointStructuresCalculationsItem(context))
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddValidateAllCalculationsInGroupItem(
                       context,
                       ValidateAll,
                       ValidateAllDataAvailableAndGetErrorMessage)
                   .AddPerformAllCalculationsInGroupItem(
                       group,
                       context,
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

        private StrictContextMenuItem CreateGenerateStabilityPointStructuresCalculationsItem(StabilityPointStructuresCalculationGroupContext nodeData)
        {
            ObservableList<StabilityPointStructure> stabilityPointStructures = nodeData.FailureMechanism.StabilityPointStructures;
            bool structuresAvailable = stabilityPointStructures.Any();

            string stabilityPointStructuresCalculationGroupContextToolTip = structuresAvailable
                                                                                ? RingtoetsCommonFormsResources.StructuresPlugin_Generate_calculations_for_selected_structures
                                                                                : RingtoetsCommonFormsResources.StructuresPlugin_No_structures_to_generate_for;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                             stabilityPointStructuresCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => { ShowStabilityPointStructuresSelectionDialog(nodeData); })
            {
                Enabled = structuresAvailable
            };
        }

        private void ShowStabilityPointStructuresSelectionDialog(StabilityPointStructuresCalculationGroupContext nodeData)
        {
            using (var dialog = new StructureSelectionDialog(Gui.MainWindow, nodeData.FailureMechanism.StabilityPointStructures))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateStabilityPointStructuresCalculations(
                        nodeData.FailureMechanism.SectionResults,
                        dialog.SelectedItems.Cast<StabilityPointStructure>(),
                        nodeData.WrappedData.Children);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateStabilityPointStructuresCalculations(IEnumerable<StabilityPointStructuresFailureMechanismSectionResult> sectionResults, IEnumerable<StabilityPointStructure> structures, IList<ICalculationBase> calculations)
        {
            foreach (var structure in structures)
            {
                var calculation = new StructuresCalculation<StabilityPointStructuresInput>
                {
                    Name = NamingHelper.GetUniqueName(calculations, structure.Name, c => c.Name),
                    InputParameters =
                    {
                        Structure = structure
                    }
                };
                calculations.Add(calculation);
            }
            StructuresHelper.UpdateCalculationToSectionResultAssignments(
                sectionResults,
                calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>());
        }

        private static void CalculationGroupContextOnNodeRemoved(StabilityPointStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (StabilityPointStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            var stabilityPointStructuresCalculations = context.FailureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>().ToArray();

            StructuresHelper.UpdateCalculationToSectionResultAssignments(context.FailureMechanism.SectionResults,
                                                                         stabilityPointStructuresCalculations);

            parentGroupContext.NotifyObservers();
        }

        private static void AddCalculation(StabilityPointStructuresCalculationGroupContext context)
        {
            var calculation = new StructuresCalculation<StabilityPointStructuresInput>
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RingtoetsCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(StabilityPointStructuresCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static void ValidateAll(StabilityPointStructuresCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StructuresCalculation<StabilityPointStructuresInput>>(),
                        context.AssessmentSection);
        }

        private void CalculateAll(CalculationGroup group, StabilityPointStructuresCalculationGroupContext context)
        {
            CalculateAll(context.FailureMechanism, group.GetCalculations().OfType<StructuresCalculation<StabilityPointStructuresInput>>(), context.AssessmentSection);
        }

        #endregion

        #region StabilityPointStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(StabilityPointStructuresCalculationContext context)
        {
            var childNodes = new List<object>
            {
                context.WrappedData.Comments,
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

            StructuresCalculation<StabilityPointStructuresInput> calculation = context.WrappedData;

            return builder.AddRenameItem()
                          .AddValidateCalculationItem(
                              context,
                              Validate,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddPerformCalculationItem(
                              calculation,
                              context,
                              Calculate,
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

        private static string ValidateAllDataAvailableAndGetErrorMessage(StabilityPointStructuresCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private void Calculate(StructuresCalculation<StabilityPointStructuresInput> calculation, StabilityPointStructuresCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             new StabilityPointStructuresCalculationActivity(calculation,
                                                                                             context.AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                             context.FailureMechanism,
                                                                                             context.AssessmentSection));
        }

        private static void Validate(StabilityPointStructuresCalculationContext context)
        {
            StabilityPointStructuresCalculationService.Validate(context.WrappedData, context.AssessmentSection);
        }

        private static void CalculationContextOnNodeRemoved(StabilityPointStructuresCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as StabilityPointStructuresCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                StructuresHelper.UpdateCalculationToSectionResultAssignments(
                    context.FailureMechanism.SectionResults,
                    context.FailureMechanism.Calculations.Cast<StructuresCalculation<StabilityPointStructuresInput>>());
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion

        #endregion
    }
}