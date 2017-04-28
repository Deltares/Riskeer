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
using Core.Common.Utils;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.ClosingStructures.IO;
using Ringtoets.ClosingStructures.IO.Configurations;
using Ringtoets.ClosingStructures.Service;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Probability;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Service;
using Ringtoets.Common.Utils;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.ClosingStructures.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="ClosingStructuresFailureMechanism"/>.
    /// </summary>
    public class ClosingStructuresPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<ClosingStructuresFailureMechanismContext, ClosingStructuresFailureMechanismProperties>
            {
                CreateInstance = context => new ClosingStructuresFailureMechanismProperties(
                    context.WrappedData,
                    new FailureMechanismPropertyChangeHandler<ClosingStructuresFailureMechanism>())
            };
            yield return new PropertyInfo<ClosingStructure, ClosingStructureProperties>();
            yield return new PropertyInfo<ClosingStructuresInputContext, ClosingStructuresInputContextProperties>
            {
                CreateInstance = context => new ClosingStructuresInputContextProperties(
                    context,
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<ClosingStructuresFailureMechanismContext, ClosingStructuresFailureMechanismView>
            {
                GetViewName = (view, mechanism) => mechanism.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseClosingStructuresFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return new ViewInfo<
                FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>,
                IEnumerable<ClosingStructuresFailureMechanismSectionResult>,
                ClosingStructuresFailureMechanismResultView>
            {
                GetViewName = (view, results) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };

            yield return new ViewInfo<ClosingStructuresScenariosContext, CalculationGroup, ClosingStructuresScenariosView>
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
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<ClosingStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<ClosingStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<ClosingStructuresCalculationContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ClosingStructuresContext>
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

            yield return new TreeNodeInfo<ClosingStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RingtoetsCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddDeleteItem()
                                                                                  .AddSeparator()
                                                                                  .AddPropertiesItem()
                                                                                  .Build(),
                CanRemove = CanRemoveClosingStructure,
                OnNodeRemoved = OnClosingStructureRemoved
            };

            yield return new TreeNodeInfo<ClosingStructuresInputContext>
            {
                Text = inputContext => RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                Image = inputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ClosingStructuresScenariosContext>
            {
                Text = context => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<ClosingStructuresContext>
            {
                CreateFileImporter = (context, filePath) => new ClosingStructuresImporter(context.WrappedData,
                                                                                          context.AssessmentSection.ReferenceLine,
                                                                                          filePath),
                Name = RingtoetsCommonFormsResources.StructuresImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                FileFilterGenerator = new FileFilterGenerator(
                    RingtoetsCommonIOResources.Shape_file_filter_Extension,
                    RingtoetsCommonIOResources.Shape_file_filter_Description),
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null
            };

            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<ClosingStructuresCalculationGroupContext>(
                (context, filePath) => new ClosingStructuresCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AvailableHydraulicBoundaryLocations,
                    context.AvailableForeshoreProfiles,
                    context.AvailableStructures));
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<ClosingStructuresCalculationGroupContext>(
                (context, filePath) => new ClosingStructuresCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<ClosingStructuresCalculationContext>(
                (context, filePath) => new ClosingStructuresCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            if (failureMechanism.Contribution <= 0.0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        private void CalculateAll(ClosingStructuresFailureMechanism failureMechanism,
                                  IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations,
                                  IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             calculations.Select(calc => new ClosingStructuresCalculationActivity(calc,
                                                                                                                  assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                                  failureMechanism,
                                                                                                                  assessmentSection)).ToArray());
        }

        #region ViewInfo

        #region ClosingStructuresFailureMechanismView ViewInfo

        private static bool CloseClosingStructuresFailureMechanismViewForData(ClosingStructuresFailureMechanismView view, object data)
        {
            var assessmentSection = data as IAssessmentSection;
            var failureMechanism = data as ClosingStructuresFailureMechanism;

            var viewFailureMechanismContext = (ClosingStructuresFailureMechanismContext) view.Data;
            ClosingStructuresFailureMechanism viewFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewFailureMechanism, failureMechanism);
        }

        #endregion

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

        #region ClosingStructuresScenariosView ViewInfo

        private static bool CloseScenariosViewForData(ClosingStructuresScenariosView view, object removedData)
        {
            var failureMechanism = removedData as ClosingStructuresFailureMechanism;

            var failureMechanismContext = removedData as ClosingStructuresFailureMechanismContext;
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            var assessmentSection = removedData as IAssessmentSection;
            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<ClosingStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion

        #endregion

        #region TreeNodeInfo

        #region ClosingStructuresFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext)
        {
            ClosingStructuresFailureMechanism wrappedData = closingStructuresFailureMechanismContext.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, closingStructuresFailureMechanismContext.Parent), TreeFolderCategory.Input),
                new ClosingStructuresCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, closingStructuresFailureMechanismContext.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext)
        {
            return new object[]
            {
                closingStructuresFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IList GetInputs(ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new ClosingStructuresContext(failureMechanism.ClosingStructures, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IList GetOutputs(ClosingStructuresFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new ClosingStructuresScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new FailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(closingStructuresFailureMechanismContext, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(closingStructuresFailureMechanismContext, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              closingStructuresFailureMechanismContext,
                              ValidateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              closingStructuresFailureMechanismContext,
                              CalculateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(closingStructuresFailureMechanismContext.WrappedData)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(ClosingStructuresFailureMechanismContext failureMechanismContext)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(failureMechanismContext);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(closingStructuresFailureMechanismContext,
                                                                  treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(closingStructuresFailureMechanismContext,
                                                                    RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static void ValidateAll(IEnumerable<StructuresCalculation<ClosingStructuresInput>> closingStructuresCalculations, IAssessmentSection assessmentSection)
        {
            foreach (StructuresCalculation<ClosingStructuresInput> calculation in closingStructuresCalculations)
            {
                ClosingStructuresCalculationService.Validate(calculation, assessmentSection);
            }
        }

        private static void ValidateAll(ClosingStructuresFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<StructuresCalculation<ClosingStructuresInput>>(),
                        context.Parent);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(ClosingStructuresFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent, context.WrappedData);
        }

        private void CalculateAll(ClosingStructuresFailureMechanismContext context)
        {
            CalculateAll(context.WrappedData, context.WrappedData.Calculations.OfType<StructuresCalculation<ClosingStructuresInput>>(), context.Parent);
        }

        #endregion

        #region ClosingStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(ClosingStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                var calculation = calculationItem as StructuresCalculation<ClosingStructuresInput>;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new ClosingStructuresCalculationContext(calculation,
                                                                                 context.FailureMechanism,
                                                                                 context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new ClosingStructuresCalculationGroupContext(group,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(ClosingStructuresCalculationGroupContext context, object parentData, TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            bool isNestedGroup = parentData is ClosingStructuresCalculationGroupContext;

            builder.AddImportItem()
                   .AddExportItem()
                   .AddSeparator();

            if (!isNestedGroup)
            {
                builder.AddCustomItem(CreateGenerateClosingStructuresCalculationsItem(context))
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem()
                       .AddSeparator();
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

        private StrictContextMenuItem CreateGenerateClosingStructuresCalculationsItem(ClosingStructuresCalculationGroupContext nodeData)
        {
            bool structuresAvailable = nodeData.FailureMechanism.ClosingStructures.Any();

            string closingStructuresCalculationGroupContextToolTip = structuresAvailable
                                                                         ? RingtoetsCommonFormsResources.StructuresPlugin_Generate_calculations_for_selected_structures
                                                                         : RingtoetsCommonFormsResources.StructuresPlugin_No_structures_to_generate_for;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                             closingStructuresCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => ShowClosingStructuresSelectionDialog(nodeData))
            {
                Enabled = structuresAvailable
            };
        }

        private void ShowClosingStructuresSelectionDialog(ClosingStructuresCalculationGroupContext nodeData)
        {
            using (var dialog = new StructureSelectionDialog(Gui.MainWindow, nodeData.FailureMechanism.ClosingStructures))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateClosingStructuresCalculations(
                        nodeData.FailureMechanism,
                        dialog.SelectedItems.Cast<ClosingStructure>(),
                        nodeData.WrappedData.Children);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateClosingStructuresCalculations(ClosingStructuresFailureMechanism failureMechanism, IEnumerable<ClosingStructure> structures, IList<ICalculationBase> calculations)
        {
            foreach (ClosingStructure structure in structures)
            {
                var calculation = new StructuresCalculation<ClosingStructuresInput>
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
                failureMechanism.SectionResults,
                failureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>());
        }

        private static void ValidateAll(ClosingStructuresCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StructuresCalculation<ClosingStructuresInput>>(), context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(ClosingStructuresCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private void CalculateAll(CalculationGroup group, ClosingStructuresCalculationGroupContext context)
        {
            CalculateAll(context.FailureMechanism, group.GetCalculations().OfType<StructuresCalculation<ClosingStructuresInput>>(), context.AssessmentSection);
        }

        private static void AddCalculation(ClosingStructuresCalculationGroupContext context)
        {
            var calculation = new StructuresCalculation<ClosingStructuresInput>
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RingtoetsCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static void CalculationGroupContextOnNodeRemoved(ClosingStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (ClosingStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);

            StructuresHelper.UpdateCalculationToSectionResultAssignments(
                context.FailureMechanism.SectionResults,
                context.FailureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>().ToArray());

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        #endregion

        #region ClosingStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(ClosingStructuresCalculationContext context)
        {
            var childNodes = new List<object>
            {
                context.WrappedData.Comments,
                new ClosingStructuresInputContext(context.WrappedData.InputParameters,
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

        private ContextMenuStrip CalculationContextContextMenuStrip(ClosingStructuresCalculationContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            StructuresCalculation<ClosingStructuresInput> calculation = context.WrappedData;

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddRenameItem()
                          .AddSeparator()
                          .AddValidateCalculationItem(
                              context,
                              ValidateAll,
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

        private void Calculate(StructuresCalculation<ClosingStructuresInput> calculation, ClosingStructuresCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             new ClosingStructuresCalculationActivity(calculation,
                                                                                      context.AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                      context.FailureMechanism,
                                                                                      context.AssessmentSection));
        }

        private static void ValidateAll(ClosingStructuresCalculationContext context)
        {
            ClosingStructuresCalculationService.Validate(context.WrappedData, context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(ClosingStructuresCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static void CalculationContextOnNodeRemoved(ClosingStructuresCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as ClosingStructuresCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                StructuresHelper.UpdateCalculationToSectionResultAssignments(
                    context.FailureMechanism.SectionResults,
                    context.FailureMechanism.Calculations.Cast<StructuresCalculation<ClosingStructuresInput>>());
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion

        #region ClosingStructure TreeNodeInfo

        private static bool CanRemoveClosingStructure(ClosingStructure nodeData, object parentData)
        {
            return parentData is ClosingStructuresContext;
        }

        private static void OnClosingStructureRemoved(ClosingStructure nodeData, object parentData)
        {
            var parentContext = (ClosingStructuresContext) parentData;
            IEnumerable<IObservable> changedObservables = ClosingStructuresDataSynchronizationService.RemoveStructure(parentContext.FailureMechanism,
                                                                                                                      nodeData);
            foreach (IObservable observable in changedObservables)
            {
                observable.NotifyObservers();
            }
        }

        #endregion

        #endregion
    }
}