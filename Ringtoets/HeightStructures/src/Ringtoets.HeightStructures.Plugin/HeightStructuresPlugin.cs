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
using Core.Common.Base.IO;
using Core.Common.Controls.TreeView;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Utils;
using Ringtoets.Common.Data;
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
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Structures;
using Ringtoets.Common.Service;
using Ringtoets.Common.Utils;
using Ringtoets.HeightStructures.Data;
using Ringtoets.HeightStructures.Forms.PresentationObjects;
using Ringtoets.HeightStructures.Forms.PropertyClasses;
using Ringtoets.HeightStructures.Forms.Views;
using Ringtoets.HeightStructures.IO;
using Ringtoets.HeightStructures.IO.Configurations;
using Ringtoets.HeightStructures.Plugin.FileImporters;
using Ringtoets.HeightStructures.Service;
using RingtoetsCommonFormsResources = Ringtoets.Common.Forms.Properties.Resources;
using RingtoetsCommonDataResources = Ringtoets.Common.Data.Properties.Resources;
using RingtoetsCommonIOResources = Ringtoets.Common.IO.Properties.Resources;

namespace Ringtoets.HeightStructures.Plugin
{
    /// <summary>
    /// The plug-in for the <see cref="HeightStructuresFailureMechanism"/>.
    /// </summary>
    public class HeightStructuresPlugin : PluginBase
    {
        public override IEnumerable<PropertyInfo> GetPropertyInfos()
        {
            yield return new PropertyInfo<HeightStructuresFailureMechanismContext, HeightStructuresFailureMechanismProperties>
            {
                CreateInstance = context => new HeightStructuresFailureMechanismProperties(
                    context.WrappedData,
                    new FailureMechanismPropertyChangeHandler<HeightStructuresFailureMechanism>())
            };
            yield return new PropertyInfo<HeightStructure, HeightStructureProperties>();
            yield return new PropertyInfo<HeightStructuresContext, HeightStructureCollectionProperties>
            {
                CreateInstance = context => new HeightStructureCollectionProperties(context.WrappedData)
            };
            yield return new PropertyInfo<HeightStructuresInputContext, HeightStructuresInputContextProperties>
            {
                CreateInstance = context => new HeightStructuresInputContextProperties(
                    context,
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
        }

        public override IEnumerable<ImportInfo> GetImportInfos()
        {
            yield return new ImportInfo<HeightStructuresContext>
            {
                Name = RingtoetsCommonFormsResources.StructuresImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                IsEnabled = IsHeightStructuresImporterEnabled,
                FileFilterGenerator = HeightStructureFileFilter(),
                CreateFileImporter = (context, filePath) => CreateHeightStructuresImporter(
                    context, filePath, new ImportMessageProvider(), new HeightStructureReplaceDataStrategy(context.FailureMechanism))
            };

            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<HeightStructuresCalculationGroupContext>(
                (context, filePath) => new HeightStructuresCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AvailableHydraulicBoundaryLocations,
                    context.AvailableForeshoreProfiles,
                    context.AvailableStructures,
                    context.FailureMechanism));
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<HeightStructuresContext>
            {
                Name = RingtoetsCommonDataResources.StructureCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                IsEnabled = IsHeightStructuresImporterEnabled,
                FileFilterGenerator = HeightStructureFileFilter(),
                CreateFileImporter = (context, filePath) => CreateHeightStructuresImporter(
                    context, filePath, new UpdateMessageProvider(), new HeightStructureUpdateDataStrategy(context.FailureMechanism)),
                CurrentPath = context => context.WrappedData.SourcePath
            };
        }

        public override IEnumerable<ExportInfo> GetExportInfos()
        {
            yield return RingtoetsExportInfoFactory.CreateCalculationGroupConfigurationExportInfo<HeightStructuresCalculationGroupContext>(
                (context, filePath) => new HeightStructuresCalculationConfigurationExporter(context.WrappedData.Children, filePath),
                context => context.WrappedData.Children.Any());

            yield return RingtoetsExportInfoFactory.CreateCalculationConfigurationExportInfo<HeightStructuresCalculationContext>(
                (context, filePath) => new HeightStructuresCalculationConfigurationExporter(new[]
                {
                    context.WrappedData
                }, filePath));
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<HeightStructuresFailureMechanismContext, HeightStructuresFailureMechanismView>
            {
                GetViewName = (view, mechanism) => mechanism.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseHeightStructuresFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant
            };

            yield return new ViewInfo<
                HeightStructuresScenariosContext,
                CalculationGroup,
                HeightStructuresScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, calculationGroup) => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                AfterCreate = (view, context) => view.FailureMechanism = context.ParentFailureMechanism,
                CloseForData = CloseScenariosViewForData,
                Image = RingtoetsCommonFormsResources.ScenariosIcon
            };

            yield return new ViewInfo<
                FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>,
                IEnumerable<HeightStructuresFailureMechanismSectionResult>,
                HeightStructuresFailureMechanismResultView>
            {
                GetViewName = (view, results) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                AfterCreate = (view, context) => view.FailureMechanism = context.FailureMechanism
            };
        }

        public override IEnumerable<TreeNodeInfo> GetTreeNodeInfos()
        {
            yield return RingtoetsTreeNodeInfoFactory.CreateFailureMechanismContextTreeNodeInfo<HeightStructuresFailureMechanismContext>(
                FailureMechanismEnabledChildNodeObjects,
                FailureMechanismDisabledChildNodeObjects,
                FailureMechanismEnabledContextMenuStrip,
                FailureMechanismDisabledContextMenuStrip);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationGroupContextTreeNodeInfo<HeightStructuresCalculationGroupContext>(
                CalculationGroupContextChildNodeObjects,
                CalculationGroupContextContextMenuStrip,
                CalculationGroupContextOnNodeRemoved);

            yield return RingtoetsTreeNodeInfoFactory.CreateCalculationContextTreeNodeInfo<HeightStructuresCalculationContext>(
                CalculationContextChildNodeObjects,
                CalculationContextContextMenuStrip,
                CalculationContextOnNodeRemoved);

            yield return new TreeNodeInfo<HeightStructuresInputContext>
            {
                Text = inputContext => RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName,
                Image = inputContext => RingtoetsCommonFormsResources.GenericInputOutputIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<HeightStructuresContext>
            {
                Text = context => RingtoetsCommonFormsResources.StructuresCollection_DisplayName,
                Image = context => RingtoetsCommonFormsResources.GeneralFolderIcon,
                ForeColor = context => context.WrappedData.Any()
                                           ? Color.FromKnownColor(KnownColor.ControlText)
                                           : Color.FromKnownColor(KnownColor.GrayText),
                ChildNodeObjects = context => context.WrappedData.Cast<object>().ToArray(),
                ContextMenuStrip = HeightStructuresContextContextMenuStrip
            };

            yield return new TreeNodeInfo<HeightStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RingtoetsCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddDeleteItem()
                                                                                  .AddSeparator()
                                                                                  .AddPropertiesItem()
                                                                                  .Build(),
                CanRemove = CanRemoveHeightStructure,
                OnNodeRemoved = OnHeightStructureRemoved
            };

            yield return new TreeNodeInfo<HeightStructuresScenariosContext>
            {
                Text = context => RingtoetsCommonFormsResources.Scenarios_DisplayName,
                Image = context => RingtoetsCommonFormsResources.ScenariosIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>>
            {
                Text = context => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = context => RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                ContextMenuStrip = (nodeData, parentData, treeViewControl) => Gui.Get(nodeData, treeViewControl)
                                                                                 .AddOpenItem()
                                                                                 .Build()
            };
        }

        private void CalculateAll(HeightStructuresFailureMechanism failureMechanism,
                                  IEnumerable<StructuresCalculation<HeightStructuresInput>> calculations,
                                  IAssessmentSection assessmentSection)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             calculations.Select(calc => new HeightStructuresCalculationActivity(calc,
                                                                                                                 assessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                                 failureMechanism,
                                                                                                                 assessmentSection)).ToArray());
        }

        private static void ValidateAll(IEnumerable<StructuresCalculation<HeightStructuresInput>> heightStructuresCalculations,
                                        IAssessmentSection assessmentSection)
        {
            foreach (StructuresCalculation<HeightStructuresInput> calculation in heightStructuresCalculations)
            {
                HeightStructuresCalculationService.Validate(calculation, assessmentSection);
            }
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection, IFailureMechanism failureMechanism)
        {
            if (failureMechanism.Contribution <= 0.0)
            {
                return RingtoetsCommonFormsResources.Contribution_of_failure_mechanism_zero;
            }

            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        #region ViewInfo

        #region HeightStructuresFailureMechanismView ViewInfo

        private static bool CloseHeightStructuresFailureMechanismViewForData(HeightStructuresFailureMechanismView view, object data)
        {
            var assessmentSection = data as IAssessmentSection;
            var failureMechanism = data as HeightStructuresFailureMechanism;

            var viewFailureMechanismContext = (HeightStructuresFailureMechanismContext) view.Data;
            HeightStructuresFailureMechanism viewFailureMechanism = viewFailureMechanismContext.WrappedData;

            return assessmentSection != null
                       ? ReferenceEquals(viewFailureMechanismContext.Parent, assessmentSection)
                       : ReferenceEquals(viewFailureMechanism, failureMechanism);
        }

        #endregion

        #region HeightStructuresScenariosView ViewInfo

        private static bool CloseScenariosViewForData(HeightStructuresScenariosView view, object removedData)
        {
            var failureMechanism = removedData as HeightStructuresFailureMechanism;

            var failureMechanismContext = removedData as HeightStructuresFailureMechanismContext;
            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            var assessmentSection = removedData as IAssessmentSection;
            if (assessmentSection != null)
            {
                failureMechanism = assessmentSection.GetFailureMechanisms()
                                                    .OfType<HeightStructuresFailureMechanism>()
                                                    .FirstOrDefault();
            }

            return failureMechanism != null && ReferenceEquals(view.Data, failureMechanism.CalculationsGroup);
        }

        #endregion

        #region HeightStructuresFailureMechanismResultView ViewInfo

        private static bool CloseFailureMechanismResultViewForData(HeightStructuresFailureMechanismResultView view, object o)
        {
            var assessmentSection = o as IAssessmentSection;
            var failureMechanism = o as HeightStructuresFailureMechanism;
            var failureMechanismContext = o as IFailureMechanismContext<HeightStructuresFailureMechanism>;
            if (assessmentSection != null)
            {
                return assessmentSection
                    .GetFailureMechanisms()
                    .OfType<HeightStructuresFailureMechanism>()
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

        #region TreeNodeInfos

        #region HeightStructure TreeNodeInfo

        private static bool CanRemoveHeightStructure(HeightStructure nodeData, object parentData)
        {
            return parentData is HeightStructuresContext;
        }

        private static void OnHeightStructureRemoved(HeightStructure nodeData, object parentData)
        {
            var parentContext = (HeightStructuresContext) parentData;
            IEnumerable<IObservable> changedObservables = HeightStructuresDataSynchronizationService.RemoveStructure(parentContext.FailureMechanism,
                                                                                                                     nodeData);
            foreach (IObservable observable in changedObservables)
            {
                observable.NotifyObservers();
            }
        }

        #endregion

        #region HeightStructuresContext TreeNodeInfo

        private ContextMenuStrip HeightStructuresContextContextMenuStrip(HeightStructuresContext nodeData,
                                                                         object parentData, TreeViewControl treeViewControl)
        {
            return Gui.Get(nodeData, treeViewControl)
                      .AddImportItem()
                      .AddUpdateItem()
                      .AddSeparator()
                      .AddCollapseAllItem()
                      .AddExpandAllItem()
                      .AddSeparator()
                      .AddPropertiesItem()
                      .Build();
        }

        #endregion

        #region HeightStructuresFailureMechanismContext TreeNodeInfo

        private static object[] FailureMechanismEnabledChildNodeObjects(HeightStructuresFailureMechanismContext context)
        {
            HeightStructuresFailureMechanism wrappedData = context.WrappedData;
            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, context.Parent), TreeFolderCategory.Input),
                new HeightStructuresCalculationGroupContext(wrappedData.CalculationsGroup, wrappedData, context.Parent),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData), TreeFolderCategory.Output)
            };
        }

        private static IList GetInputs(HeightStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new ArrayList
            {
                new FailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new HeightStructuresContext(failureMechanism.HeightStructures, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IList GetOutputs(HeightStructuresFailureMechanism failureMechanism)
        {
            return new ArrayList
            {
                new HeightStructuresScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new FailureMechanismSectionResultContext<HeightStructuresFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism),
                failureMechanism.OutputComments
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(HeightStructuresFailureMechanismContext context)
        {
            return new object[]
            {
                context.WrappedData.NotRelevantComments
            };
        }

        private ContextMenuStrip FailureMechanismEnabledContextMenuStrip(HeightStructuresFailureMechanismContext context,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddOpenItem()
                          .AddSeparator()
                          .AddToggleRelevancyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddValidateAllCalculationsInFailureMechanismItem(
                              context,
                              ValidateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddPerformAllCalculationsInFailureMechanismItem(
                              context,
                              CalculateAll,
                              ValidateAllDataAvailableAndGetErrorMessage)
                          .AddSeparator()
                          .AddClearAllCalculationOutputInFailureMechanismItem(context.WrappedData)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .AddSeparator()
                          .AddPropertiesItem()
                          .Build();
        }

        private void RemoveAllViewsForItem(HeightStructuresFailureMechanismContext context)
        {
            Gui.ViewCommands.RemoveAllViewsForItem(context);
        }

        private ContextMenuStrip FailureMechanismDisabledContextMenuStrip(HeightStructuresFailureMechanismContext context,
                                                                          object parentData,
                                                                          TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            return builder.AddToggleRelevancyOfFailureMechanismItem(context, RemoveAllViewsForItem)
                          .AddSeparator()
                          .AddCollapseAllItem()
                          .AddExpandAllItem()
                          .Build();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(HeightStructuresFailureMechanismContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent, context.WrappedData);
        }

        private static void ValidateAll(HeightStructuresFailureMechanismContext context)
        {
            ValidateAll(context.WrappedData.Calculations.OfType<StructuresCalculation<HeightStructuresInput>>(),
                        context.Parent);
        }

        private void CalculateAll(HeightStructuresFailureMechanismContext context)
        {
            CalculateAll(context.WrappedData, context.WrappedData.Calculations.OfType<StructuresCalculation<HeightStructuresInput>>(), context.Parent);
        }

        #endregion

        #region HeightStructuresCalculationGroupContext TreeNodeInfo

        private static object[] CalculationGroupContextChildNodeObjects(HeightStructuresCalculationGroupContext context)
        {
            var childNodeObjects = new List<object>();

            foreach (ICalculationBase calculationItem in context.WrappedData.Children)
            {
                var calculation = calculationItem as StructuresCalculation<HeightStructuresInput>;
                var group = calculationItem as CalculationGroup;

                if (calculation != null)
                {
                    childNodeObjects.Add(new HeightStructuresCalculationContext(calculation,
                                                                                context.FailureMechanism,
                                                                                context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new HeightStructuresCalculationGroupContext(group,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(HeightStructuresCalculationGroupContext context, object parentData, TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            bool isNestedGroup = parentData is HeightStructuresCalculationGroupContext;

            builder.AddImportItem()
                   .AddExportItem()
                   .AddSeparator();

            if (!isNestedGroup)
            {
                builder.AddCustomItem(CreateGenerateHeightStructuresCalculationsItem(context))
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

        private StrictContextMenuItem CreateGenerateHeightStructuresCalculationsItem(HeightStructuresCalculationGroupContext nodeData)
        {
            StructureCollection<HeightStructure> heightStructures = nodeData.FailureMechanism.HeightStructures;
            bool structuresAvailable = heightStructures.Any();

            string heightStructuresCalculationGroupContextToolTip = structuresAvailable
                                                                        ? RingtoetsCommonFormsResources.StructuresPlugin_Generate_calculations_for_selected_structures
                                                                        : RingtoetsCommonFormsResources.StructuresPlugin_No_structures_to_generate_for;

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.CalculationGroup_Generate_calculations,
                                             heightStructuresCalculationGroupContextToolTip,
                                             RingtoetsCommonFormsResources.GenerateScenariosIcon,
                                             (sender, args) => { ShowHeightStructuresSelectionDialog(nodeData); })
            {
                Enabled = structuresAvailable
            };
        }

        private void ShowHeightStructuresSelectionDialog(HeightStructuresCalculationGroupContext nodeData)
        {
            using (var dialog = new StructureSelectionDialog(Gui.MainWindow, nodeData.FailureMechanism.HeightStructures))
            {
                dialog.ShowDialog();

                if (dialog.SelectedItems.Any())
                {
                    GenerateHeightStructuresCalculations(
                        nodeData.FailureMechanism,
                        dialog.SelectedItems.Cast<HeightStructure>(),
                        nodeData.WrappedData.Children);
                    nodeData.NotifyObservers();
                }
            }
        }

        private static void GenerateHeightStructuresCalculations(HeightStructuresFailureMechanism failureMechanism, IEnumerable<HeightStructure> structures, IList<ICalculationBase> calculations)
        {
            foreach (HeightStructure structure in structures)
            {
                var calculation = new StructuresCalculation<HeightStructuresInput>
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
                failureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>());
        }

        private static void CalculationGroupContextOnNodeRemoved(HeightStructuresCalculationGroupContext context, object parentNodeData)
        {
            var parentGroupContext = (HeightStructuresCalculationGroupContext) parentNodeData;

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);

            StructuresHelper.UpdateCalculationToSectionResultAssignments(
                context.FailureMechanism.SectionResults,
                context.FailureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>().ToArray());

            parentGroupContext.NotifyObservers();
        }

        private static void AddCalculation(HeightStructuresCalculationGroupContext context)
        {
            var calculation = new StructuresCalculation<HeightStructuresInput>
            {
                Name = NamingHelper.GetUniqueName(context.WrappedData.Children, RingtoetsCommonDataResources.Calculation_DefaultName, c => c.Name)
            };
            context.WrappedData.Children.Add(calculation);
            context.WrappedData.NotifyObservers();
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(HeightStructuresCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static void ValidateAll(HeightStructuresCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StructuresCalculation<HeightStructuresInput>>(), context.AssessmentSection);
        }

        private void CalculateAll(CalculationGroup group, HeightStructuresCalculationGroupContext context)
        {
            CalculateAll(context.FailureMechanism, group.GetCalculations().OfType<StructuresCalculation<HeightStructuresInput>>(), context.AssessmentSection);
        }

        #endregion

        #region HeightStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(HeightStructuresCalculationContext context)
        {
            var childNodes = new List<object>
            {
                context.WrappedData.Comments,
                new HeightStructuresInputContext(context.WrappedData.InputParameters,
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

        private ContextMenuStrip CalculationContextContextMenuStrip(HeightStructuresCalculationContext context, object parentData, TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));

            StructuresCalculation<HeightStructuresInput> calculation = context.WrappedData;

            return builder.AddExportItem()
                          .AddSeparator()
                          .AddRenameItem()
                          .AddSeparator()
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

        private static string ValidateAllDataAvailableAndGetErrorMessage(HeightStructuresCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection, context.FailureMechanism);
        }

        private static void Validate(HeightStructuresCalculationContext context)
        {
            HeightStructuresCalculationService.Validate(context.WrappedData, context.AssessmentSection);
        }

        private void Calculate(StructuresCalculation<HeightStructuresInput> calculation, HeightStructuresCalculationContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow, new HeightStructuresCalculationActivity(calculation,
                                                                                                     context.AssessmentSection.HydraulicBoundaryDatabase.FilePath,
                                                                                                     context.FailureMechanism,
                                                                                                     context.AssessmentSection));
        }

        private static void CalculationContextOnNodeRemoved(HeightStructuresCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as HeightStructuresCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                StructuresHelper.UpdateCalculationToSectionResultAssignments(
                    context.FailureMechanism.SectionResults,
                    context.FailureMechanism.Calculations.Cast<StructuresCalculation<HeightStructuresInput>>());
                calculationGroupContext.NotifyObservers();
            }
        }

        #endregion

        #endregion

        #region Importers

        #region HeightStructuresImporter

        private static IFileImporter CreateHeightStructuresImporter(HeightStructuresContext context, string filePath,
                                                                    IImporterMessageProvider messageProvider, IStructureUpdateStrategy<HeightStructure> strategy)
        {
            return new HeightStructuresImporter(context.WrappedData, context.AssessmentSection.ReferenceLine,
                                                filePath, messageProvider, strategy);
        }

        private static bool IsHeightStructuresImporterEnabled(HeightStructuresContext context)
        {
            return context.AssessmentSection.ReferenceLine != null;
        }

        private static FileFilterGenerator HeightStructureFileFilter()
        {
            return new FileFilterGenerator(RingtoetsCommonIOResources.Shape_file_filter_Extension,
                                           RingtoetsCommonIOResources.Shape_file_filter_Description);
        }

        #endregion

        #endregion
    }
}