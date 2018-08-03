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

using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using Core.Common.Base;
using Core.Common.Base.IO;
using Core.Common.Controls.TreeView;
using Core.Common.Gui;
using Core.Common.Gui.ContextMenu;
using Core.Common.Gui.Forms.ProgressDialog;
using Core.Common.Gui.Plugin;
using Core.Common.Util;
using Ringtoets.ClosingStructures.Data;
using Ringtoets.ClosingStructures.Forms.PresentationObjects;
using Ringtoets.ClosingStructures.Forms.PropertyClasses;
using Ringtoets.ClosingStructures.Forms.Views;
using Ringtoets.ClosingStructures.IO;
using Ringtoets.ClosingStructures.IO.Configurations;
using Ringtoets.ClosingStructures.Plugin.FileImporters;
using Ringtoets.ClosingStructures.Service;
using Ringtoets.ClosingStructures.Util;
using Ringtoets.Common.Data.AssessmentSection;
using Ringtoets.Common.Data.Calculation;
using Ringtoets.Common.Data.FailureMechanism;
using Ringtoets.Common.Data.Structures;
using Ringtoets.Common.Forms;
using Ringtoets.Common.Forms.ChangeHandlers;
using Ringtoets.Common.Forms.ExportInfos;
using Ringtoets.Common.Forms.Helpers;
using Ringtoets.Common.Forms.ImportInfos;
using Ringtoets.Common.Forms.PresentationObjects;
using Ringtoets.Common.Forms.PropertyClasses;
using Ringtoets.Common.Forms.TreeNodeInfos;
using Ringtoets.Common.Forms.UpdateInfos;
using Ringtoets.Common.IO.FileImporters.MessageProviders;
using Ringtoets.Common.IO.Structures;
using Ringtoets.Common.Service;
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
                CreateInstance = context => new ClosingStructuresFailureMechanismProperties(context.WrappedData)
            };
            yield return new PropertyInfo<ClosingStructure, ClosingStructureProperties>();
            yield return new PropertyInfo<ClosingStructuresContext, StructureCollectionProperties<ClosingStructure>>
            {
                CreateInstance = context => new StructureCollectionProperties<ClosingStructure>(context.WrappedData)
            };
            yield return new PropertyInfo<ClosingStructuresInputContext, ClosingStructuresInputContextProperties>
            {
                CreateInstance = context => new ClosingStructuresInputContextProperties(
                    context,
                    new ObservablePropertyChangeHandler(context.Calculation, context.WrappedData))
            };
            yield return new PropertyInfo<ClosingStructuresOutputContext, ClosingStructuresOutputProperties>
            {
                CreateInstance = context => new ClosingStructuresOutputProperties(context.WrappedData.Output,
                                                                                  context.FailureMechanism,
                                                                                  context.AssessmentSection)
            };
        }

        public override IEnumerable<ViewInfo> GetViewInfos()
        {
            yield return new ViewInfo<ClosingStructuresFailureMechanismContext, ClosingStructuresFailureMechanismView>
            {
                GetViewName = (view, context) => context.WrappedData.Name,
                Image = RingtoetsCommonFormsResources.CalculationIcon,
                CloseForData = CloseClosingStructuresFailureMechanismViewForData,
                AdditionalDataCheck = context => context.WrappedData.IsRelevant,
                CreateInstance = context => new ClosingStructuresFailureMechanismView(context.WrappedData, context.Parent)
            };

            yield return new ViewInfo<
                ProbabilityFailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>,
                IObservableEnumerable<ClosingStructuresFailureMechanismSectionResult>,
                ClosingStructuresFailureMechanismResultView>
            {
                GetViewName = (view, context) => RingtoetsCommonFormsResources.FailureMechanism_AssessmentResult_DisplayName,
                Image = RingtoetsCommonFormsResources.FailureMechanismSectionResultIcon,
                CloseForData = CloseFailureMechanismResultViewForData,
                GetViewData = context => context.WrappedData,
                CreateInstance = context => new ClosingStructuresFailureMechanismResultView(
                    context.WrappedData,
                    (ClosingStructuresFailureMechanism) context.FailureMechanism, context.AssessmentSection)
            };

            yield return new ViewInfo<ClosingStructuresScenariosContext, CalculationGroup, ClosingStructuresScenariosView>
            {
                GetViewData = context => context.WrappedData,
                GetViewName = (view, context) => RingtoetsCommonFormsResources.Scenarios_DisplayName,
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

            yield return new TreeNodeInfo<ProbabilityFailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>>
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
                                                                                 .AddUpdateItem()
                                                                                 .AddSeparator()
                                                                                 .AddCollapseAllItem()
                                                                                 .AddExpandAllItem()
                                                                                 .AddSeparator()
                                                                                 .AddPropertiesItem()
                                                                                 .Build()
            };

            yield return new TreeNodeInfo<ClosingStructure>
            {
                Text = structure => structure.Name,
                Image = structure => RingtoetsCommonFormsResources.StructuresIcon,
                ContextMenuStrip = (structure, parentData, treeViewControl) => Gui.Get(structure, treeViewControl)
                                                                                  .AddPropertiesItem()
                                                                                  .Build()
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
                CreateFileImporter = (context, filePath) => CreateClosingStructuresImporter(
                    context,
                    filePath,
                    new ImportMessageProvider(),
                    new ClosingStructureReplaceDataStrategy(context.FailureMechanism)),
                Name = RingtoetsCommonFormsResources.StructuresImporter_DisplayName,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                FileFilterGenerator = CreateClosingStructureFileFilter(),
                IsEnabled = context => context.AssessmentSection.ReferenceLine != null,
                VerifyUpdates = context => VerifyStructuresShouldUpdate(
                    context.FailureMechanism,
                    RingtoetsCommonIOResources.VerifyStructuresShouldUpdate_When_importing_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RingtoetsImportInfoFactory.CreateCalculationConfigurationImportInfo<ClosingStructuresCalculationGroupContext>(
                (context, filePath) => new ClosingStructuresCalculationConfigurationImporter(
                    filePath,
                    context.WrappedData,
                    context.AssessmentSection.HydraulicBoundaryDatabase.Locations,
                    context.AvailableForeshoreProfiles,
                    context.AvailableStructures,
                    context.FailureMechanism));
        }

        public override IEnumerable<UpdateInfo> GetUpdateInfos()
        {
            yield return new UpdateInfo<ClosingStructuresContext>
            {
                CreateFileImporter = (context, filePath) => CreateClosingStructuresImporter(
                    context,
                    filePath,
                    new UpdateMessageProvider(),
                    new ClosingStructureUpdateDataStrategy(context.FailureMechanism)),
                Name = RingtoetsCommonDataResources.StructureCollection_TypeDescriptor,
                Category = RingtoetsCommonFormsResources.Ringtoets_Category,
                Image = RingtoetsCommonFormsResources.StructuresIcon,
                FileFilterGenerator = CreateClosingStructureFileFilter(),
                IsEnabled = c => c.FailureMechanism.ClosingStructures.SourcePath != null,
                CurrentPath = context => context.WrappedData.SourcePath,
                VerifyUpdates = context => VerifyStructuresShouldUpdate(
                    context.FailureMechanism,
                    RingtoetsCommonIOResources.VerifyStructuresShouldUpdate_When_updating_Calculation_with_Structure_data_output_will_be_cleared_confirm)
            };

            yield return RingtoetsUpdateInfoFactory.CreateFailureMechanismSectionsUpdateInfo<
                ClosingStructuresFailureMechanismSectionsContext, ClosingStructuresFailureMechanism, ClosingStructuresFailureMechanismSectionResult>(
                new ClosingStructuresFailureMechanismSectionResultUpdateStrategy());
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

        private static string ValidateAllDataAvailableAndGetErrorMessage(IAssessmentSection assessmentSection)
        {
            return HydraulicBoundaryDatabaseConnectionValidator.Validate(assessmentSection.HydraulicBoundaryDatabase);
        }

        #region ViewInfo

        #region ClosingStructuresFailureMechanismView ViewInfo

        private static bool CloseClosingStructuresFailureMechanismViewForData(ClosingStructuresFailureMechanismView view, object data)
        {
            var assessmentSection = data as IAssessmentSection;
            var failureMechanism = data as ClosingStructuresFailureMechanism;

            return assessmentSection != null
                       ? ReferenceEquals(view.AssessmentSection, assessmentSection)
                       : ReferenceEquals(view.FailureMechanism, failureMechanism);
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
                       .Any(fm => ReferenceEquals(view.FailureMechanism.SectionResults, fm.SectionResults));
            }

            if (failureMechanismContext != null)
            {
                failureMechanism = failureMechanismContext.WrappedData;
            }

            return failureMechanism != null && ReferenceEquals(view.FailureMechanism.SectionResults, failureMechanism.SectionResults);
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

        private static object[] FailureMechanismEnabledChildNodeObjects(ClosingStructuresFailureMechanismContext context)
        {
            ClosingStructuresFailureMechanism wrappedData = context.WrappedData;
            IAssessmentSection assessmentSection = context.Parent;

            return new object[]
            {
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Inputs_DisplayName, GetInputs(wrappedData, assessmentSection), TreeFolderCategory.Input),
                new ClosingStructuresCalculationGroupContext(wrappedData.CalculationsGroup, null, wrappedData, assessmentSection),
                new CategoryTreeFolder(RingtoetsCommonFormsResources.FailureMechanism_Outputs_DisplayName, GetOutputs(wrappedData, assessmentSection), TreeFolderCategory.Output)
            };
        }

        private static object[] FailureMechanismDisabledChildNodeObjects(ClosingStructuresFailureMechanismContext closingStructuresFailureMechanismContext)
        {
            return new object[]
            {
                closingStructuresFailureMechanismContext.WrappedData.NotRelevantComments
            };
        }

        private static IEnumerable<object> GetInputs(ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new ClosingStructuresFailureMechanismSectionsContext(failureMechanism, assessmentSection),
                new ForeshoreProfilesContext(failureMechanism.ForeshoreProfiles, failureMechanism, assessmentSection),
                new ClosingStructuresContext(failureMechanism.ClosingStructures, failureMechanism, assessmentSection),
                failureMechanism.InputComments
            };
        }

        private static IEnumerable<object> GetOutputs(ClosingStructuresFailureMechanism failureMechanism, IAssessmentSection assessmentSection)
        {
            return new object[]
            {
                new FailureMechanismAssemblyCategoriesContext(failureMechanism, assessmentSection, () => failureMechanism.GeneralInput.N),
                new ClosingStructuresScenariosContext(failureMechanism.CalculationsGroup, failureMechanism),
                new ProbabilityFailureMechanismSectionResultContext<ClosingStructuresFailureMechanismSectionResult>(
                    failureMechanism.SectionResults, failureMechanism, assessmentSection),
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
            return ValidateAllDataAvailableAndGetErrorMessage(context.Parent);
        }

        private void CalculateAll(ClosingStructuresFailureMechanismContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(context.WrappedData, context.Parent));
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
                                                                                 context.WrappedData,
                                                                                 context.FailureMechanism,
                                                                                 context.AssessmentSection));
                }
                else if (group != null)
                {
                    childNodeObjects.Add(new ClosingStructuresCalculationGroupContext(group,
                                                                                      context.WrappedData,
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

        private ContextMenuStrip CalculationGroupContextContextMenuStrip(ClosingStructuresCalculationGroupContext context,
                                                                         object parentData,
                                                                         TreeViewControl treeViewControl)
        {
            CalculationGroup group = context.WrappedData;
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);
            bool isNestedGroup = parentData is ClosingStructuresCalculationGroupContext;

            StructuresCalculation<ClosingStructuresInput>[] calculations = group
                                                                           .GetCalculations()
                                                                           .OfType<StructuresCalculation<ClosingStructuresInput>>().ToArray();

            builder.AddImportItem()
                   .AddExportItem()
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddDuplicateCalculationItem(group, context)
                       .AddSeparator();
            }
            else
            {
                builder.AddCustomItem(CreateGenerateClosingStructuresCalculationsItem(context))
                       .AddSeparator();
            }

            builder.AddCreateCalculationGroupItem(group)
                   .AddCreateCalculationItem(context, AddCalculation)
                   .AddSeparator();

            if (isNestedGroup)
            {
                builder.AddRenameItem();
            }

            builder.AddUpdateForeshoreProfileOfCalculationsItem(calculations,
                                                                inquiryHelper,
                                                                SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                   .AddCustomItem(CreateUpdateStructureItem(calculations))
                   .AddSeparator()
                   .AddValidateAllCalculationsInGroupItem(
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

        private StrictContextMenuItem CreateUpdateStructureItem(
            IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = RingtoetsCommonFormsResources.StructuresPlugin_CreateUpdateStructureItem_Update_all_calculations_with_Structure_Tooltip;

            StructuresCalculation<ClosingStructuresInput>[] calculationsToUpdate = calculations
                                                                                   .Where(calc => calc.InputParameters.Structure != null
                                                                                                  && !calc.InputParameters.IsStructureInputSynchronized)
                                                                                   .ToArray();

            if (!calculationsToUpdate.Any())
            {
                contextMenuEnabled = false;
                toolTipMessage = RingtoetsCommonFormsResources.CreateUpdateContextMenuItem_No_calculations_to_update_ToolTip;
            }

            return new StrictContextMenuItem(RingtoetsCommonFormsResources.StructuresPlugin_CreateUpdateStructureItem_Update_all_Structures,
                                             toolTipMessage,
                                             RingtoetsCommonFormsResources.UpdateItemIcon,
                                             (o, args) => UpdateStructureDependentDataOfCalculations(calculationsToUpdate))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateStructureDependentDataOfCalculations(IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations)
        {
            string message = RingtoetsCommonFormsResources.VerifyUpdate_Confirm_calculation_outputs_cleared;
            if (StructureDependentDataShouldUpdate(calculations, message))
            {
                foreach (StructuresCalculation<ClosingStructuresInput> calculation in calculations)
                {
                    UpdateStructureDerivedCalculationInput(calculation);
                }
            }
        }

        private StrictContextMenuItem CreateGenerateClosingStructuresCalculationsItem(ClosingStructuresCalculationGroupContext nodeData)
        {
            bool structuresAvailable = nodeData.FailureMechanism.ClosingStructures.Any();

            string closingStructuresCalculationGroupContextToolTip = structuresAvailable
                                                                         ? RingtoetsCommonFormsResources.Generate_Calculations_for_selected_Structures
                                                                         : RingtoetsCommonFormsResources.No_Structures_to_generate_Calculations_for;

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

        private static void GenerateClosingStructuresCalculations(ClosingStructuresFailureMechanism failureMechanism, IEnumerable<ClosingStructure> structures, List<ICalculationBase> calculations)
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

            ClosingStructuresHelper.UpdateCalculationToSectionResultAssignments(failureMechanism);
        }

        private static void ValidateAll(ClosingStructuresCalculationGroupContext context)
        {
            ValidateAll(context.WrappedData.GetCalculations().OfType<StructuresCalculation<ClosingStructuresInput>>(), context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(ClosingStructuresCalculationGroupContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private void CalculateAll(CalculationGroup group, ClosingStructuresCalculationGroupContext context)
        {
            ActivityProgressDialogRunner.Run(Gui.MainWindow,
                                             ClosingStructuresCalculationActivityFactory.CreateCalculationActivities(group,
                                                                                                                     context.FailureMechanism,
                                                                                                                     context.AssessmentSection));
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

            ClosingStructuresHelper.UpdateCalculationToSectionResultAssignments(context.FailureMechanism);

            parentGroupContext.WrappedData.Children.Remove(context.WrappedData);
            parentGroupContext.NotifyObservers();
        }

        #endregion

        #region ClosingStructuresCalculationContext TreeNodeInfo

        private static object[] CalculationContextChildNodeObjects(ClosingStructuresCalculationContext context)
        {
            StructuresCalculation<ClosingStructuresInput> calculation = context.WrappedData;

            return new object[]
            {
                calculation.Comments,
                new ClosingStructuresInputContext(calculation.InputParameters,
                                                  calculation,
                                                  context.FailureMechanism,
                                                  context.AssessmentSection),
                new ClosingStructuresOutputContext(calculation, context.FailureMechanism, context.AssessmentSection)
            };
        }

        private ContextMenuStrip CalculationContextContextMenuStrip(ClosingStructuresCalculationContext context,
                                                                    object parentData,
                                                                    TreeViewControl treeViewControl)
        {
            var builder = new RingtoetsContextMenuBuilder(Gui.Get(context, treeViewControl));
            var inquiryHelper = new DialogBasedInquiryHelper(Gui.MainWindow);

            StructuresCalculation<ClosingStructuresInput> calculation = context.WrappedData;
            return builder.AddExportItem()
                          .AddSeparator()
                          .AddDuplicateCalculationItem(calculation, context)
                          .AddSeparator()
                          .AddRenameItem()
                          .AddUpdateForeshoreProfileOfCalculationItem(calculation,
                                                                      inquiryHelper,
                                                                      SynchronizeCalculationWithForeshoreProfileHelper.UpdateForeshoreProfileDerivedCalculationInput)
                          .AddCustomItem(CreateUpdateStructureItem(context))
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
                                             ClosingStructuresCalculationActivityFactory.CreateCalculationActivity(calculation,
                                                                                                                   context.FailureMechanism,
                                                                                                                   context.AssessmentSection));
        }

        private static void ValidateAll(ClosingStructuresCalculationContext context)
        {
            ClosingStructuresCalculationService.Validate(context.WrappedData, context.AssessmentSection);
        }

        private static string ValidateAllDataAvailableAndGetErrorMessage(ClosingStructuresCalculationContext context)
        {
            return ValidateAllDataAvailableAndGetErrorMessage(context.AssessmentSection);
        }

        private static void CalculationContextOnNodeRemoved(ClosingStructuresCalculationContext context, object parentData)
        {
            var calculationGroupContext = parentData as ClosingStructuresCalculationGroupContext;
            if (calculationGroupContext != null)
            {
                calculationGroupContext.WrappedData.Children.Remove(context.WrappedData);
                ClosingStructuresHelper.UpdateCalculationToSectionResultAssignments(context.FailureMechanism);
                calculationGroupContext.NotifyObservers();
            }
        }

        private StrictContextMenuItem CreateUpdateStructureItem(ClosingStructuresCalculationContext context)
        {
            var contextMenuEnabled = true;
            string toolTipMessage = RingtoetsCommonFormsResources.Update_Calculation_with_Structure_ToolTip;
            if (context.WrappedData.InputParameters.Structure == null)
            {
                contextMenuEnabled = false;
                toolTipMessage = RingtoetsCommonFormsResources.Structure_must_be_selected_ToolTip;
            }
            else if (context.WrappedData.InputParameters.IsStructureInputSynchronized)
            {
                contextMenuEnabled = false;
                toolTipMessage = RingtoetsCommonFormsResources.CalculationItem_No_changes_to_update_ToolTip;
            }

            return new StrictContextMenuItem(
                RingtoetsCommonFormsResources.Update_Structure_data,
                toolTipMessage,
                RingtoetsCommonFormsResources.UpdateItemIcon,
                (o, args) => UpdateStructureDependentDataOfCalculation(context.WrappedData))
            {
                Enabled = contextMenuEnabled
            };
        }

        private void UpdateStructureDependentDataOfCalculation(StructuresCalculation<ClosingStructuresInput> calculation)
        {
            string message = RingtoetsCommonFormsResources.VerifyUpdate_Confirm_calculation_output_cleared;
            if (StructureDependentDataShouldUpdate(new[]
            {
                calculation
            }, message))
            {
                UpdateStructureDerivedCalculationInput(calculation);
            }
        }

        private bool StructureDependentDataShouldUpdate(IEnumerable<StructuresCalculation<ClosingStructuresInput>> calculations, string query)
        {
            var changeHandler = new CalculationChangeHandler(calculations,
                                                             query,
                                                             new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        private static void UpdateStructureDerivedCalculationInput(StructuresCalculation<ClosingStructuresInput> calculation)
        {
            calculation.InputParameters.SynchronizeStructureInput();

            var affectedObjects = new List<IObservable>
            {
                calculation.InputParameters
            };

            affectedObjects.AddRange(RingtoetsCommonDataSynchronizationService.ClearCalculationOutput(calculation));

            foreach (IObservable affectedObject in affectedObjects)
            {
                affectedObject.NotifyObservers();
            }
        }

        #endregion

        #endregion

        #region Importers

        #region ClosingStructuresImporter

        private static IFileImporter CreateClosingStructuresImporter(ClosingStructuresContext context,
                                                                     string filePath,
                                                                     IImporterMessageProvider importerMessageProvider,
                                                                     IStructureUpdateStrategy<ClosingStructure> structureUpdateStrategy)
        {
            return new ClosingStructuresImporter(context.WrappedData,
                                                 context.AssessmentSection.ReferenceLine,
                                                 filePath,
                                                 importerMessageProvider,
                                                 structureUpdateStrategy);
        }

        private static FileFilterGenerator CreateClosingStructureFileFilter()
        {
            return new FileFilterGenerator(
                RingtoetsCommonIOResources.Shape_file_filter_Extension,
                RingtoetsCommonIOResources.Shape_file_filter_Description);
        }

        private bool VerifyStructuresShouldUpdate(IFailureMechanism failureMechanism, string query)
        {
            var changeHandler = new FailureMechanismCalculationChangeHandler(failureMechanism,
                                                                             query,
                                                                             new DialogBasedInquiryHelper(Gui.MainWindow));

            return !changeHandler.RequireConfirmation() || changeHandler.InquireConfirmation();
        }

        #endregion

        #endregion
    }
}